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
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;
namespace VATClient
{
    public partial class FormTransferReceiveSearch : Form
    {
        #region Constructors
        public FormTransferReceiveSearch()
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
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        ////private string SelectedValue = string.Empty;
        string selectedRow ="";
        private int ReceiveForm;

        string[] IdArray;
        private static string transactionType { get; set; }
        private static string transactionType1 { get; set; }
        //public string VFIN = "3021";
        #region Global Variables For BackGroundWorker
        //string ReceiveData;
        private DataTable ReceiveResult;
        private string cmbPostText;
        string POSTCMB = string.Empty;
        #endregion
        #endregion
        private void FormTransferReceiveSearch_Load(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                if (Program.fromOpen == "Me")
                {
                    //btnAdd.Visible = false;
                }
                if (transactionType == "62In")
                {
                    btnPost.Visible = true;
                }
                else
                {
                    btnPost.Visible = false;
                }
                string[] Condition = new string[] { "ActiveStatus='Y' AND BranchID NOT IN(" + Program.BranchId + ")" };
                cmbReceiveForm = new CommonDAL().ComboBoxLoad(cmbReceiveForm, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbReceiveForm.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                //Search();
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferReceiveSearch_Load", exMessage);
            }
            #endregion
        }
        public static string SelectOne(string transType)
        {
            string selectedRowTemp =null;
            try
            {
                #region Statement
                FormTransferReceiveSearch frmReceiveSearch = new FormTransferReceiveSearch();
                transactionType = transType;
                if (transType == "62In")
                {
                frmReceiveSearch.Text = "VAT FG (In)";
                transactionType = transType;
            }
                else if (transType == "61In")
                {
                    frmReceiveSearch.Text = "VAT RM (In)";
                    transactionType = transType;
                }
                frmReceiveSearch.ShowDialog();
                selectedRowTemp=frmReceiveSearch.selectedRow;
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, "FormTransferReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTransferReceiveSearch", "SelectOne", exMessage);
            }
            #endregion
            return selectedRowTemp;
        }
        string ReceiveFromDate, ReceiveToDate;
        private void DtpNullChecking()
        {
            try
            {
                #region Statement
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
                    ReceiveToDate = dtpReceiveToDate.Value.ToString("yyyy-MMM-dd");
                }
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
            }
            #endregion
        }
        private void grbTransactionHistory_Enter(object sender, EventArgs e)
        {
        }
        public void ClearAllFields()
        {
            try
            {
                #region Statement
                txtReceiveNo.Text = "";
                txtSerialNo.Text = "";
                dtpReceiveFromDate.Text = "";
                dtpReceiveToDate.Text = "";
                dgvReceiveHistory.Rows.Clear();
                cmbPost.SelectedIndex = -1;
                #endregion
            }
            #region catch
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
            #endregion
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                ClearAllFields();
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            #endregion
        }
        #region TextBox KeyDown Event
        private void txtReceiveNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }
        private void cmbPost_KeyDown(object sender, KeyEventArgs e)
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
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                GridSelected();
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnOk_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnOk_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnOk_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnOk_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnOk_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOk_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOk_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnOk_Click", exMessage);
            }
            #endregion
        }
        private void GridSelected()
        {
            try
            {
                #region Statement
                if (dgvReceiveHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvReceiveHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count>0)
                {
                    selectedRow =dgvReceiveHistory.CurrentRow.Cells["ReceiveNo"].Value.ToString();
                }
                this.Close();
                #endregion
            }
            #region catch
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
            #endregion
        }
        private void dgvReceiveHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                GridSelected();
                #endregion
            }
            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_DoubleClick", exMessage);
            }
            #endregion
        }
        // == Search == //
        #region // == Search == //
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                ReceiveForm = Convert.ToInt32(cmbReceiveForm.SelectedValue);

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
                #region Statement
                POSTCMB = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";
                //transactionType = "Other";

                #region DtpNull Checking

                DtpNullChecking();

                #endregion

                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;

                #region Transfer ReceiveVM

                TransferReceiveVM vm = new TransferReceiveVM();
                vm.TransferReceiveNo = txtReceiveNo.Text.Trim();
                vm.ReceiveDateFrom = ReceiveFromDate;
                vm.ReceiveDateTo = ReceiveToDate;
                vm.TransactionType = transactionType;
                vm.Post = POSTCMB;
                vm.TransferFromNo = txtTransferFromNo.Text.Trim();
                vm.TransferNo = txtTransferNo.Text.Trim();
                vm.BranchId = Program.BranchId;
                vm.TransferFrom = ReceiveForm;

                #endregion

                #region Search Transfer Receive

                //TransferReceiveDAL receiveDal = new TransferReceiveDAL();
                ITransferReceive receiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                //ReceiveResult = receiveDal.SearchTransferReceive(txtReceiveNo.Text.Trim(), ReceiveFromDate, ReceiveToDate,
                //                transactionType,  POSTCMB);  // Change 04
                ReceiveResult = receiveDal.SearchTransferReceive(vm,connVM);

                #endregion

                #region DataGridView
                dgvReceiveHistory.Rows.Clear();
                int j = 0;
                //if (transactionType=="All")
                //{
                //    DataRow[] receiveRows = ReceiveResult.Select("transactionType='62Out'");
                //    ReceiveResult = receiveRows.CopyToDataTable();
                //}
                if (ReceiveResult != null)
                    foreach (DataRow item in ReceiveResult.Rows)
                    //foreach (DataRow item in ReceiveRows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvReceiveHistory.Rows.Add(NewRow);
                        dgvReceiveHistory.Rows[j].Cells["ReceiveNo"].Value = item["TransferReceiveNo"].ToString();                // ReceiveFields[0].ToString();
                        dgvReceiveHistory.Rows[j].Cells["ReceiveDate"].Value = item["ReceiveDateTime"].ToString();        // ReceiveFields[1].ToString();
                        dgvReceiveHistory.Rows[j].Cells["TotalAmount"].Value = item["TotalAmount"].ToString();        //Convert.ToDecimal(ReceiveFields[3].ToString()).ToString("0.00");
                        dgvReceiveHistory.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();              // ReceiveFields[4].ToString();
                        dgvReceiveHistory.Rows[j].Cells["ReferenceNo"].Value = item["ReferenceNo"].ToString();              // ReceiveFields[4].ToString();
                        dgvReceiveHistory.Rows[j].Cells["TransferFrom"].Value = item["BranchName"].ToString();              // ReceiveFields[4].ToString();
                        dgvReceiveHistory.Rows[j].Cells["TransferFromNo"].Value = item["TransferFromNo"].ToString();              // ReceiveFields[4].ToString();
                       // dgvReceiveHistory.Rows[j].Cells["TransferNo"].Value = item["TransferNo"].ToString();              // ReceiveFields[4].ToString();
                        //dgvReceiveHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();              // ReceiveFields[5].ToString();
                        dgvReceiveHistory.Rows[j].Cells["Post"].Value = item["Post"].ToString();                      // ReceiveFields[6].ToString();
                        dgvReceiveHistory.Rows[j].Cells["Id"].Value = item["Id"].ToString();                      // ReceiveFields[7].ToString();
                        j = j + 1;
                    }
                #endregion

                 #endregion
            }

            #region catch
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
            #endregion

            finally
            {
                LRecordCount.Text = "Record Count: " + dgvReceiveHistory.Rows.Count.ToString();
                this.progressBar1.Visible = false;
                this.btnSearch.Enabled = true;
            }
        }
        private void dtpReceiveToDate_ValueChanged(object sender, EventArgs e)
        {
        }
        #endregion

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvReceiveHistory.RowCount; i++)
            {
                dgvReceiveHistory["Select", i].Value = chkSelectAll.Checked;
            }
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
                        string Id = dgvReceiveHistory["Id", i].Value.ToString();
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
                //TransferReceiveDAL _TransferReceiveDal = new TransferReceiveDAL();
                ITransferReceive _TransferReceiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                //this.progressBar1.Visible = true;
                string[] results = _TransferReceiveDal.MultiplePost(IdArray,connVM);
                if (results[0] == "Success")
                {
                    MessageBox.Show("All Purchases posted successfully");
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvReceiveHistory.Rows.Count;

                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value))
                    {
                        invoiceList.Add(dgvReceiveHistory["ReceiveNo", i].Value.ToString());
                    }
                }

                var dal = new TransferReceiveDAL();

                var data = dal.GetExcelData(invoiceList, null, null, connVM);

                if (data.Rows.Count == 0)
                {
                    data.Rows.Add(data.NewRow());
                }
                OrdinaryVATDesktop.SaveExcel(data, "TransferReceive", "TransferReceiveM");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");

                ////if (cmbExport.Text == "EXCEL")
                ////{
                ////    if (data.Rows.Count == 0)
                ////    {
                ////        data.Rows.Add(data.NewRow());
                ////    }
                ////    OrdinaryVATDesktop.SaveExcel(data, "TransferReceive", "TransferReceiveM");
                ////    MessageBox.Show("Successfully Exported data in Excel files of root directory");
                ////}
                ////else if (cmbExport.Text == "TEXT")
                ////{
                ////    OrdinaryVATDesktop.WriteDataToFile(data, "TransferIssue");
                ////    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                ////}
               
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }

        }
       
        //=============//
    }
}
