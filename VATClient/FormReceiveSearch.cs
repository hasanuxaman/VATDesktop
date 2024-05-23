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
    public partial class FormReceiveSearch : Form
    {
        #region Constructors

        public FormReceiveSearch()
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
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        DataGridViewRow selectedRow = new DataGridViewRow();

        //public string VFIN = "302";
        private DataTable ReceiveResult;
        private static string transactionType;
        private string post = string.Empty; //background worker
        string[] IdArray;
        private string SearchBranchId = "0";
        private string RecordCount = "0";

        #endregion

        public static DataGridViewRow SelectOne(string TransType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                FormReceiveSearch frmReceiveSearch = new FormReceiveSearch();
                transactionType = TransType;
                if (TransType == "Other")
                    frmReceiveSearch.Text = "Prodution Receive";
                else if (TransType == "WIP")
                    frmReceiveSearch.Text = "WIP (Prodution Receive)";
                else if (TransType == "ReceiveReturn")
                    frmReceiveSearch.Text = "Prodution Receive Return";
                else if (TransType == "TollFinishReceive")
                    frmReceiveSearch.Text = "Prodution Receive(Toll Production)";
                else if (TransType == "TollFinishReceiveWithoutBOM")
                    frmReceiveSearch.Text = "Prodution Receive Without BOM(Toll Production)";


                frmReceiveSearch.ShowDialog();
                selectedRowTemp = frmReceiveSearch.selectedRow;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {

                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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


                if (dtpReceiveFromDate.Checked == false)
                {
                    ReceiveFromDate = "";
                }
                else
                {
                    ReceiveFromDate = dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpReceiveToDate.Checked == false)
                {
                    ReceiveToDate = "";
                }
                else
                {
                    ReceiveToDate = dtpReceiveToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                if (txtGrandTotalFrom.Text == "")
                {
                    txtGrandTotalFrom.Text = "0.00";
                }

                if (txtGrandTotalTo.Text == "")
                {
                    txtGrandTotalTo.Text = "0.00";
                }

                if (txtVatAmountFrom.Text == "")
                {
                    txtVatAmountFrom.Text = "0.00";
                }

                if (txtVatAmountTo.Text == "")
                {
                    txtVatAmountTo.Text = "0.00";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "NullCheck", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

        private void grbTransactionHistory_Enter(object sender, EventArgs e)
        {

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

                ////new FormAdjustmentSearch().BranchLoad(cmbBranch);

                //dgvReceiveHistory.Columns["Comments"].Visible = false;

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                //Search();
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
                txtGrandTotalFrom.Text = "0.00";
                txtGrandTotalTo.Text = "0.00";
                txtReceiveNo.Text = "";
                txtRefNo.Text = "";
                txtVatAmountFrom.Text = "0.00";
                txtVatAmountTo.Text = "0.00";
                dtpReceiveFromDate.Text = "";
                dtpReceiveToDate.Text = "";
                dgvReceiveHistory.Rows.Clear();
                cmbPost.SelectedIndex = -1;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
                if (dgvReceiveHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvReceiveHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                this.Close();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "GridSeleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

            CloseForm();
        }

        private void dgvReceiveHistory_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //MDIMainInterface mdi = new MDIMainInterface();
                FormReceive frmReceive = new FormReceive();
                //mdi.RollDetailsInfo(frmReceive.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmReceive.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmReceive.Show();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion Catch
        }

        private void dgvReceiveHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // == Search == //
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

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Search", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
                //transactionType = "Other";

                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);


                //ReceiveResult = receiveDal.SearchReceiveHeaderDTNew(txtReceiveNo.Text.Trim(), ReceiveFromDate,
                //    ReceiveToDate, txtRefNo.Text.Trim(), transactionType, post, vCustomerID);  // Change 04

                string[] cValues =
                {
                    txtReceiveNo.Text.Trim(), ReceiveFromDate, ReceiveToDate, txtRefNo.Text.Trim(), transactionType,
                    post, vCustomerID, txtSerialNo.Text.Trim(), SearchBranchId, txtImportID.Text.Trim(), RecordCount
                };
                string[] cFields =
                {
                    "rh.ReceiveNo like", "rh.ReceiveDateTime>", "rh.ReceiveDateTime<", "rh.ReferenceNo like",
                    "rh.TransactionType like", "rh.Post like", "rh.CustomerID", "rh.SerialNo like", "rh.BranchId",
                    "rh.ImportIDExcel like", "SelectTop"
                };
                ReceiveResult = receiveDal.SelectAll(0, cFields, cValues, null, null, null, true, connVM);

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
                dgvReceiveHistory.DataSource = null;
                if (ReceiveResult != null && ReceiveResult.Rows.Count > 0)
                {


                    TotalTecordCount = ReceiveResult.Rows[ReceiveResult.Rows.Count - 1][0].ToString();

                    ReceiveResult.Rows.RemoveAt(ReceiveResult.Rows.Count - 1);

                    dgvReceiveHistory.DataSource = ReceiveResult;

                    #region Specific Coloumns Visible False
                    dgvReceiveHistory.Columns["CustomerID"].Visible = false;
                    dgvReceiveHistory.Columns["ReturnId"].Visible = false;
                    dgvReceiveHistory.Columns["Id"].Visible = false;
                    dgvReceiveHistory.Columns["BranchId"].Visible = false;
                    dgvReceiveHistory.Columns["ShiftId"].Visible = false;

                    #endregion
                }


                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

        private void txtCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                vCustomerID = "0";
                txtCustomer.Text = "";
                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormCustomerFinder.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                }
                txtCustomer.Focus();
                RecordCount = cmbRecordCount.Text.Trim();
                Search();
            }

        }
        //=============//

        public string vCustomerID = "0";

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            vCustomerID = "0";
            txtCustomer.Text = "";
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            int j = 0;
            string ids = "";
            for (int i = 0; i < dgvReceiveHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value) == true)
                {
                    if (dgvReceiveHistory["Post", i].Value.ToString() == "N")
                    {
                        string Id = dgvReceiveHistory["ID", i].Value.ToString();
                        ids += Id + "~";
                    }
                }
            }
            IdArray = ids.Split('~');
            if (IdArray.Length <= 1)
            {
                MessageBox.Show(this, "All data already posted !");
                return;
            }
            bgwMultiplePost.RunWorkerAsync();
        }

        private void bgwMultiplePost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (IdArray == null || IdArray.Length <= 0)
                {
                    return;
                }
                else if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                //ReceiveDAL _RecDal = new ReceiveDAL();
                IReceive _RecDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                //progressBar1.Visible = true;
                string[] results = _RecDal.MultiplePost(IdArray, connVM,Program.CurrentUserID);
                if (results[0] == "Success")
                {
                    MessageBox.Show("All Sales posted successfully");
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
            SelectAll();
        }

        private void SelectAll()
        {
            if (chkSelectAll.Checked == true)
            {
                for (int i = 0; i < dgvReceiveHistory.RowCount; i++)
                {
                    dgvReceiveHistory["Select", i].Value = true;
                }
            }
            else
            {
                for (int i = 0; i < dgvReceiveHistory.RowCount; i++)
                {
                    dgvReceiveHistory["Select", i].Value = false;
                }
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvReceiveHistory.Rows.Count;

                //if (len == 0)
                //{
                //    MessageBox.Show("No Data Found"); && dgvReceiveHistory["PostR", i].Value.ToString() != "Y"
                //    return;
                //}

                //for (var i = 0; i < len; i++)
                //{
                //    if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value) && dgvReceiveHistory["PostR", i].Value.ToString() != "Y")
                //    {
                //        invoiceList.Add(dgvReceiveHistory["ReceiveNo", i].Value.ToString());
                //    }
                //}


                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value))
                    {
                        invoiceList.Add(dgvReceiveHistory["ReceiveNo", i].Value.ToString());
                    }
                }





                //if (invoiceList.Count == 0)
                //    return;

                //var dal = new ReceiveDAL();
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

        private void btnMultiple_Click(object sender, EventArgs e)
        {
            try
            {

                FormCollection fc = Application.OpenForms;

                foreach (Form frm in fc)
                {
                    if (frm.Name == "FormReceiveMultiple")
                    {
                        frm.BringToFront();
                        return;
                    }
                }

                FormReceiveMultiple frmNew = new FormReceiveMultiple(transactionType);
                frmNew.Show();
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion Catch
        }

        private void FormReceiveSearch_FormClosing(object sender, FormClosingEventArgs e)
        {

            CloseForm();
        }

        private void CloseForm()
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Name == "FormReceiveMultiple")
                {
                    frm.Close();
                    return;
                }
            }

        }

    }
}
