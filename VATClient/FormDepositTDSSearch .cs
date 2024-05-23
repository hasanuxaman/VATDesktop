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
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATViewModel.DTOs;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormDepositTDSSearch : Form
    {
        #region Constructors

        public FormDepositTDSSearch()
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
        private int searchBranchId = 0;
        #region Global Variables For BackGroundWorker

        //private string DepositResult;
        private DataTable DepositResult;
        private string cmbPostText;

        #endregion

        DataGridViewRow selectedRow = new DataGridViewRow();
        private string DepositpData = string.Empty;
        private static string transactionType1 = string.Empty;

        #endregion

        public static DataGridViewRow SelectOne(string transactionType)
        {
            DataGridViewRow selectedRowTemp = null;
            string frmSearchSelectValue = String.Empty;
            try
            {

                FormDepositTDSSearch frmDepositSearch = new FormDepositTDSSearch();
                
                    transactionType1 = transactionType;
                    if (transactionType.Split('-').Length > 1)
                    {

                        if (transactionType.Split('-')[1] == "Credit")
                        {
                            frmDepositSearch.Text = "Reverse " + transactionType.Split('-')[0] + " Search";
                        }
                    }
                    else
                    {
                        frmDepositSearch.Text = transactionType + " Search";
                    }
                    
                frmDepositSearch.ShowDialog();
                //frmSearchSelectValue = frmDepositSearch.SelectedValue;
                selectedRowTemp = frmDepositSearch.selectedRow;

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
            }
            #endregion
            return selectedRowTemp;
        }

        string ChequeFromDate, ChequeToDate, DepositFromDate, DepositToDate;

        private void NullCheck()
        {
            try
            {

                if (dtpDepositFromDate.Checked == false)
                {
                    DepositFromDate = "";
                }
                else
                {
                    DepositFromDate = dtpDepositFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpDepositToDate.Checked == false)
                {
                    DepositToDate = "";
                }
                else
                {
                    DepositToDate = dtpDepositToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                if (dtpChequeFromDate.Checked == false)
                {
                    ChequeFromDate = "";
                }
                else
                {
                    ChequeFromDate = dtpChequeFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpChequeToDate.Checked == false)
                {
                    ChequeToDate = "";
                }
                else
                {
                    ChequeToDate = dtpChequeToDate.Value.ToString("yyyy-MMM-dd");
                }

                if (txtDepositAmountFrom.Text == "")
                {
                    txtDepositAmountFrom.Text = "0.00";
                }

                if (txtDepositAmountTo.Text == "")
                {
                    txtDepositAmountTo.Text = "0.00";
                }
            }
            #region catch

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
            #endregion

        }

        public void ClearAllFields()
        {
            txtAccountNumber.Text = "";
            txtBankID.Text = "";
            txtBankName.Text = "";
            txtBranchName.Text = "";
            txtChequeBank.Text = "";
            txtChequeBankBranch.Text = "";
            txtChequeNo.Text = "";
            txtDepositAmountFrom.Text = "0.00";
            txtDepositAmountTo.Text = "0.00";
            txtDepositID.Text = "";
            txtDepositPerson.Text = "";
            txtDepositPersonDesignation.Text = "";
            txtDepositType.Text = "";
            txtTreasuryNo.Text = "";
            dtpChequeFromDate.Text = "";
            dtpChequeToDate.Text = "";
            dtpDepositFromDate.Text = "";
            dtpDepositToDate.Text = "";
            dgvDeposit.Rows.Clear();
            cmbPost.SelectedIndex = -1;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        #region TextBox KeyDown Event

        private void txtDepositID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTreasuryNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDepositFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDepositToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtDepositType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtDepositAmountFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtDepositAmountTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpChequeFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpChequeToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void GridSelected()
        {
            try
            {
                if (dgvDeposit.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvDeposit.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                ////if (dgvDeposit.Rows.Count > 0)
                ////{
                ////    string DepositInfo = string.Empty;

                ////    int ColIndex = dgvDeposit.CurrentCell.ColumnIndex;
                ////    int RowIndex1 = dgvDeposit.CurrentCell.RowIndex;
                ////    if (RowIndex1 >= 0)
                ////    {
                ////        //if (Program.fromOpen != "Me" && dgvDeposit.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                ////        //{
                ////        //    MessageBox.Show("This Selected Item is not Active");
                ////        //    return;
                ////        //}


                ////        string DepositId = dgvDeposit.Rows[RowIndex1].Cells["DepositId"].Value.ToString();
                ////        string TreasuryNo = dgvDeposit.Rows[RowIndex1].Cells["TreasuryNo"].Value.ToString();
                ////        string DepositDateTime = dgvDeposit.Rows[RowIndex1].Cells["DepositDate"].Value.ToString();
                ////        string DepositType = dgvDeposit.Rows[RowIndex1].Cells["DepositType"].Value.ToString();
                ////        string DepositAmount = dgvDeposit.Rows[RowIndex1].Cells["DepositAmount"].Value.ToString();
                ////        string ChequeNo = dgvDeposit.Rows[RowIndex1].Cells["ChequeNo"].Value.ToString();
                ////        string ChequeBank = dgvDeposit.Rows[RowIndex1].Cells["ChequeBank"].Value.ToString();
                ////        string ChequeBankBranch = dgvDeposit.Rows[RowIndex1].Cells["ChequeBankBranch"].Value.ToString();
                ////        string ChequeDate = dgvDeposit.Rows[RowIndex1].Cells["ChequeDate"].Value.ToString();
                ////        string BankID = dgvDeposit.Rows[RowIndex1].Cells["BankID"].Value.ToString();
                ////        string BankName = dgvDeposit.Rows[RowIndex1].Cells["BankName"].Value.ToString();
                ////        string BranchName = dgvDeposit.Rows[RowIndex1].Cells["BranchName"].Value.ToString();
                ////        string AccountNumber = dgvDeposit.Rows[RowIndex1].Cells["AccountNumber"].Value.ToString();
                ////        string DepositPerson = dgvDeposit.Rows[RowIndex1].Cells["DepositPerson"].Value.ToString();
                ////        string DepositPersonDesignation =
                ////            dgvDeposit.Rows[RowIndex1].Cells["DepositPersonDesignation"].Value.ToString();
                ////        string Comments = dgvDeposit.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                ////        string Post = dgvDeposit.Rows[RowIndex1].Cells["Post"].Value.ToString();

                ////        DepositInfo =

                ////            DepositId + FieldDelimeter +
                ////            TreasuryNo + FieldDelimeter +
                ////            DepositDateTime + FieldDelimeter +
                ////            DepositType + FieldDelimeter +
                ////            DepositAmount + FieldDelimeter +
                ////            ChequeNo + FieldDelimeter +
                ////            ChequeBank + FieldDelimeter +
                ////            ChequeBankBranch + FieldDelimeter +
                ////            ChequeDate + FieldDelimeter +
                ////            BankID + FieldDelimeter +
                ////            BankName + FieldDelimeter +
                ////            BranchName + FieldDelimeter +
                ////            AccountNumber + FieldDelimeter +
                ////            DepositPerson + FieldDelimeter +
                ////            DepositPersonDesignation + FieldDelimeter +
                ////            Comments + FieldDelimeter + Post;
                ////        SelectedValue = DepositInfo;

                ////    }
                ////}
                this.Close();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSelected", exMessage);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();

        }

        private void dgvDeposit_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ////MDIMainInterface mdi = new MDIMainInterface();
                FormDeposit frmDeposit = new FormDeposit();
                //mdi.RollDetailsInfo(frmDeposit.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmDeposit.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmDeposit.Show();
            }
            #region catch

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
            #endregion

        }

        private void txtDepositAmountFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtDepositAmountFrom, "Deposit Amount From");
        }

        private void txtDepositAmountTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtDepositAmountTo, "Deposit Amount To");
        }

        private void FormDepositSearch_Load(object sender, EventArgs e)
        {
            try
            {
                dgvDeposit.Columns["BankID"].Visible = false;
                dgvDeposit.Columns["DepositPersonDesignation"].Visible = false;
                dgvDeposit.Columns["Comments"].Visible = false;

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                CommonDAL dal = new CommonDAL();
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
                cmbBranch.SelectedValue = Program.BranchId;
                //Search();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormDepositSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDepositSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDepositSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDepositSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormDepositSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDepositSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDepositSearch_Load", exMessage);
            }
            #endregion

        }

        private void dgvDeposit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                Search();
                txtDepositAmountFrom.Text = "";
                txtDepositAmountTo.Text = "";
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
            NullCheck();

            try
            {
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                cmbPostText = cmbPost.Text.Trim();
                backgroundWorkerSearch.RunWorkerAsync();
                
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

        #region Background Worker Events

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {

                DepositResult = new DataTable();

                //DepositTDSDAL depositDal = new DepositTDSDAL();

                IDepositTDS depositDal = OrdinaryVATDesktop.GetObject<DepositTDSDAL, DepositTDSRepo, IDepositTDS>(OrdinaryVATDesktop.IsWCF);
                //DepositResult = depositDal.SearchDepositNew(txtDepositID.Text.Trim(),
                //    txtTreasuryNo.Text.Trim(), DepositFromDate, DepositToDate, txtDepositType.Text.Trim(),
                //    txtChequeNo.Text.Trim(), ChequeFromDate,
                //    ChequeToDate, txtBankName.Text.Trim(), txtBranchName.Text.Trim(),
                //    txtAccountNumber.Text.Trim(), transactionType1, cmbPostText);// Change 04

                string[] cValues = { txtDepositID.Text.Trim(), txtTreasuryNo.Text.Trim(), DepositFromDate, DepositToDate, txtDepositType.Text.Trim(), txtChequeNo.Text.Trim(), ChequeFromDate, ChequeToDate, txtBankName.Text.Trim(), txtBranchName.Text.Trim(), txtAccountNumber.Text.Trim(), transactionType1, cmbPostText, searchBranchId.ToString() };
                string[] cFields = { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>", "d.DepositDateTime<", "d.DepositType like", "d.ChequeNo like", "d.ChequeDate>", "d.ChequeDate<", "b.BankName like", "b.BranchName like", "b.AccountNumber like", "d.TransactionType like", "d.Post like", "d.BranchId" };

                DepositResult = depositDal.SelectAll(0, cFields, cValues, null, null, true,connVM);

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                //string decriptedDepositData = Converter.DESDecrypt(PassPhrase, EnKey, DepositResult);
                //string[] DepositLines = decriptedDepositData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                

                dgvDeposit.Rows.Clear();

                int j = 0;
                //for (int j = 0; j < Convert.ToInt32(DepositLines.Length); j++)
                foreach (DataRow item in DepositResult.Rows)
                {
                    //string[] DepositFields = DepositLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvDeposit.Rows.Add(NewRow);
                    dgvDeposit.Rows[j].Cells["DepositId"].Value = item["DepositId"].ToString();// DepositFields[0].ToString();
                    dgvDeposit.Rows[j].Cells["TreasuryNo"].Value = item["TreasuryNo"].ToString();// DepositFields[1].ToString();
                    dgvDeposit.Rows[j].Cells["DepositDate"].Value = item["DepositDateTime"].ToString();// Convert.ToDateTime(DepositFields[2]).ToString("dd/MMM/yyyy");
                    dgvDeposit.Rows[j].Cells["DepositType"].Value = item["DepositType"].ToString();// DepositFields[3].ToString();
                    dgvDeposit.Rows[j].Cells["DepositAmount"].Value = item["DepositAmount"].ToString();// Convert.ToDecimal(DepositFields[4]).ToString("0.00");
                    dgvDeposit.Rows[j].Cells["ChequeNo"].Value = item["ChequeNo"].ToString();// DepositFields[5].ToString();
                    dgvDeposit.Rows[j].Cells["BankDepositDate"].Value = item["BankDepositDate"].ToString();
                    dgvDeposit.Rows[j].Cells["ChequeBank"].Value = item["ChequeBank"].ToString();// DepositFields[6].ToString();
                    dgvDeposit.Rows[j].Cells["ChequeBankBranch"].Value = item["ChequeBankBranch"].ToString();// DepositFields[7].ToString();
                    dgvDeposit.Rows[j].Cells["ChequeDate"].Value = item["ChequeDate"].ToString();// Convert.ToDateTime(DepositFields[8]).ToString("dd/MMM/yyyy");
                    dgvDeposit.Rows[j].Cells["BankID"].Value = item["BankID"].ToString();// DepositFields[9].ToString();
                    dgvDeposit.Rows[j].Cells["BankName"].Value = item["BankName"].ToString();// DepositFields[10].ToString();
                    dgvDeposit.Rows[j].Cells["BranchName"].Value = item["BranchName"].ToString();// DepositFields[11].ToString();
                    dgvDeposit.Rows[j].Cells["AccountNumber"].Value = item["AccountNumber"].ToString();// DepositFields[12].ToString();
                    dgvDeposit.Rows[j].Cells["DepositPerson"].Value = item["DepositPerson"].ToString();// DepositFields[13].ToString();
                    dgvDeposit.Rows[j].Cells["DepositPersonDesignation"].Value = item["DepositPersonDesignation"].ToString();// DepositFields[14].ToString();
                    dgvDeposit.Rows[j].Cells["DepositPersonContactNo"].Value = item["DepositPersonContactNo"].ToString();// DepositFields[14].ToString();
                    dgvDeposit.Rows[j].Cells["DepositPersonAddress"].Value = item["DepositPersonAddress"].ToString();// DepositFields[14].ToString();
                    dgvDeposit.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// DepositFields[15].ToString();
                    dgvDeposit.Rows[j].Cells["Post"].Value = item["Post"].ToString();// DepositFields[16].ToString();
                    dgvDeposit.Rows[j].Cells["TransactionType"].Value = item["TransactionType"].ToString();// DepositFields[16].ToString();
                    dgvDeposit.Rows[j].Cells["ReverseID"].Value = item["ReverseDepositId"].ToString();// DepositFields[16].ToString();
                    dgvDeposit.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();// DepositFields[16].ToString();

                    
                    j = j + 1;
                }
                dgvDeposit.Columns["ChequeBankBranch"].Visible = false;
                dgvDeposit.Columns["BankID"].Visible = false;
                dgvDeposit.Columns["DepositPerson"].Visible = false;
                dgvDeposit.Columns["Comments"].Visible = false;
                dgvDeposit.Columns["DepositPersonDesignation"].Visible = false;
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted",

                               exMessage);
            }

            #endregion

            finally
            {
                LRecordCount.Text = "Record Count: " + dgvDeposit.Rows.Count.ToString();
                this.progressBar1.Visible = false;
                this.btnSearch.Enabled = true;
            }
        }

        #endregion

        private void label20_Click(object sender, EventArgs e)
        {

        }
    }
}
