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
using System.Diagnostics;
////
using System.Security.Cryptography;
using System.IO;
using OfficeOpenXml;
using SymphonySofttech.Utilities;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Newtonsoft.Json;
namespace VATClient
{
    public partial class FormSaleSearch : Form
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #region Constructors

        public FormSaleSearch()
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

        private string p = "";

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private string SelectedValue = string.Empty;
        private static string transactionType { get; set; }
        //public string VFIN = "303";
        private string Type = string.Empty;
        private string TradingItem = string.Empty;
        private string Print = string.Empty;
        private string post = string.Empty;
        private string IsVDS = string.Empty;
        private string convCompltd = string.Empty;
        private string IsVDSCompltd = string.Empty;
        private DataTable SaleResult;
        string[] IdArray;
        string[] IdImportArray;

        private string SalesFromDate, SalesToDate;
        private string SearchBranchId = "0";
        private string RecordCount = "0";

        public int SenderBranchId = 0;
        string SearchFields = "";
        string SearchValue = "";
        bool _isVDS = false;
        private string PeriodName=string.Empty;
        #endregion

        public static DataGridViewRow SelectOne(string tType, int paramSenderBranchId = 0, bool VDS = false)
        {
            DataGridViewRow selectedRowTemp = null;
            try
            {
                #region Statement

                FormSaleSearch frmSaleSearch = new FormSaleSearch();
                frmSaleSearch.SenderBranchId = paramSenderBranchId;
                frmSaleSearch._isVDS = VDS;

                transactionType = tType;

                ////if (VDS)
                ////{

                ////    frmSaleSearch.cmbPost.Enabled = false;
                ////    frmSaleSearch.cmbPost.SelectedIndex = 0;
                ////    frmSaleSearch.cmbVDS.Enabled = false;
                ////    frmSaleSearch.cmbVDS.SelectedIndex = 0;
                ////    frmSaleSearch.cmbVDSCompleted.Enabled = false;
                ////    frmSaleSearch.cmbVDSCompleted.SelectedIndex = 1;

                ////}


                if (tType == "Other")
                {

                    frmSaleSearch.Text = "Sales Search";
                }
                else if (tType == "Debit")
                {
                    frmSaleSearch.Text = "Sales Search(Debit)";

                }
                else if (tType == "Credit")
                {
                    frmSaleSearch.Text = "Sales Search(Credit)";

                }
                else if (tType == "Trading")
                {
                    frmSaleSearch.Text = "Sales Search(Trading)";

                }
                else if (tType == "Export")
                {
                    frmSaleSearch.Text = "Export Search";
                }
                else if (tType == "InternalIssue")
                {
                    frmSaleSearch.Text = "Internal Issue(Transfer) Search";

                }

                else if (tType == "Service" || tType == "ServiceStock" || tType == "'ServiceStock,'Service'")
                {
                    frmSaleSearch.Text = "Stock / NonStock Service Search";

                }
                else if (tType == "Tender")
                {
                    frmSaleSearch.Text = "Tender Sale Search";

                }
                else if (tType == "TollIssue")
                {
                    frmSaleSearch.Text = "Toll Issue Search";

                }
                else if (tType == "TollFinishIssue")
                {
                    frmSaleSearch.Text = "Toll Finish Item Issue Search";

                }
                if (tType == "ContractorRawIssue")
                {

                    frmSaleSearch.Text = "Raw Issue Search";
                }
                if (tType == "TollSale")
                {

                    frmSaleSearch.Text = "Toll Sale Search";
                }
                frmSaleSearch.ShowDialog();
                selectedRowTemp = frmSaleSearch.selectedRow;

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        private void FormSaleSearch_Load(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                VATName vname = new VATName();

                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);

                cmbExport.SelectedIndex = 0;
                cmbBranch.SelectedValue = Program.BranchId;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                //new FormAdjustmentSearch().BranchLoad(cmbBranch);

                cmbType.Text = Program.SalesType;
                cmbTrading.Text = Program.Trading;


                if (Program.fromOpen == "Me")
                {

                }
                cmbSearchFields.Text = "RefNo";

                FormSale frms = new FormSale();


                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
            }
            #endregion

        }

        private void NullCheck()
        {
            try
            {
                #region Statement

                SalesFromDate = dtpSaleFromDate.Checked == false ? "1900-01-01 00:00:00" : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd 00:00:00");
                SalesToDate = dtpSaleToDate.Checked == false ? "9000-12-31 23:59:59" : dtpSaleToDate.Value.ToString("yyyy-MMM-dd 23:59:59");

                if (txtGrandTotalFrom.Text == "")
                {
                    txtGrandTotalFrom.Text = "0.00";
                }

                if (txtGrandTotalTo.Text == "")
                {
                    txtGrandTotalTo.Text = "0.00";
                }



                if (txtVatAmountTo.Text == "")
                {
                    txtVatAmountTo.Text = "0.00";
                }

                #endregion

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
            txtCustomerID.Text = "";
            txtInvoiceNo.Text = "";
            txtSerialNo.Text = "";
            txtVehicleNo.Text = "";
            txtVehicleID.Text = "";
            dtpSaleFromDate.Text = "";
            dtpSaleToDate.Text = "";

            txtCustomerName.Text = "";
            txtVehicleType.Text = "";
            txtSearchValue.Text = "";
            cmbPrint.SelectedIndex = -1;
            cmbTrading.SelectedIndex = -1;
            cmbPost.SelectedIndex = -1;
            cmbType.SelectedIndex = -1;
            cmbSearchFields.Text = "RefNo";

            dgvSalesHistory.DataSource = null;

        }
        private void customerLoad()
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select c.CustomerID, cg.CustomerGroupName,c.CustomerCode,c.CustomerName,c.Address1 
                            from Customers c 
                            left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID 
                            where 1=1 and c.ActiveStatus='Y' ";
                string ShowAllCustomer = commonDal.settingsDesktop("Setup", "ShowAllCustomer", null, connVM);
                if (ShowAllCustomer.ToLower() == "n")
                {
                    SqlText += @" and c.BranchId='" + Program.BranchId + "'";
                }


                string SQLTextRecordCount = @" select count(CustomerCode)RecordNo from Customers";

                string[] shortColumnName = { "cg.CustomerGroupName", "c.CustomerCode", "c.CustomerName", "c.Address1" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtCustomerID.Text = "";
                    txtCustomerName.Text = "";
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                    txtCustomerName.Focus();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //DataGridViewRow selectedRow = new DataGridViewRow();
            //selectedRow = FormCustomerFinder.SelectOne();
            //if (selectedRow != null && selectedRow.Index >= 0)
            //{
            //    vCustomerID = "0";
            //    txtCustomer.Text = "";
            //    txtDeliveryAddress1.Text = "";
            //    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
            //    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
            //    txtDeliveryAddress1.Text = selectedRow.Cells["Address1"].Value.ToString();//ProductInfo[0];
            //}
            //txtCustomer.Focus();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                customerLoad();
            }
        }

        private void txtVehicleType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpSaleFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpSaleToDate_KeyDown(object sender, KeyEventArgs e)
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
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void GridSeleted()
        {
            try
            {
                #region Statement


                if (dgvSalesHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvSalesHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                this.Close();

                #endregion

            }
            #region catch

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
            #endregion

        }

        private void dgvSalesHistory_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                #region Statement



                GridSeleted();
                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", exMessage);
            }
            #endregion

        }

        private void txtVatAmountFrom_Leave(object sender, EventArgs e)
        {
        }

        private void txtVatAmountTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVatAmountTo, "VAT Amount To");
        }

        private void txtGrandTotalFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtGrandTotalFrom, "Grand Amount From");
        }

        private void txtGrandTotalTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtGrandTotalTo, "Grand Amount To");
        }

        private void dgvSalesHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // == Search == //
        #region // == Search == //

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBranchId = cmbBranch.SelectedValue.ToString();
            RecordCount = cmbRecordCount.Text.Trim();
            searchDT();
        }
        private void searchDT()
        {
            try
            {
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                NullCheck();
                SearchFields = cmbSearchFields.Text;
                SearchValue = txtSearchValue.Text;

                if (SearchFields == "RefNo")
                {
                    SearchFields = "ImportIDExcel";
                }
                TradingItem = cmbTrading.SelectedIndex != -1 ? cmbTrading.Text.Trim() : string.Empty;
                Type = cmbType.SelectedIndex != -1 ? cmbType.Text.Trim() : string.Empty;
                Print = cmbPrint.SelectedIndex != -1 ? cmbPrint.Text.Trim() : string.Empty;
                post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : string.Empty;
                IsVDS = cmbVDS.SelectedIndex != -1 ? cmbVDS.Text.Trim() : string.Empty;
                IsVDSCompltd = cmbVDSCompleted.SelectedIndex != -1 ? cmbVDSCompleted.Text.Trim() : string.Empty;
                convCompltd = cmbConvComltd.SelectedIndex != -1 ? cmbConvComltd.Text.Trim() : string.Empty;
                bgwSearch.RunWorkerAsync();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "searchDT", exMessage);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            #endregion
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                if (string.IsNullOrEmpty(transactionType))
                    transactionType = "All";
                ISale saleDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);  //new SaleDAL();

                //SaleResult = saleDal.SearchSalesHeaderDTNew(
                //txtInvoiceNo.Text.Trim(), txtCustomerName.Text.Trim(),
                //txtCustomerGroupName.Text.Trim(),
                //txtVehicleType.Text.Trim(), txtVehicleNo.Text.Trim(), txtSerialNo.Text.Trim(), SalesFromDate,
                //SalesToDate, Type, TradingItem, Print, transactionType, post, txtEXPFormNo.Text.Trim());  // Change 04
                if (transactionType.ToLower() == "all")
                {
                    transactionType = "";
                }



                string[] cValues = {  txtInvoiceNo.Text.Trim(), 
                                      txtCustomerName.Text.Trim(),
                                      txtCustomerGroupName.Text.Trim(),
                                      txtVehicleType.Text.Trim(), 
                                      txtVehicleNo.Text.Trim(), 
                                      txtSerialNo.Text.Trim(), 
                                      SalesFromDate,
                                      SalesToDate, 
                                      Type, 
                                      TradingItem, 
                                      Print,                                      
                                      post, 
                                      convCompltd, 
                                      txtEXPFormNo.Text.Trim(),
                                      IsVDS,
                                      SearchBranchId
                                      //,SearchValue
                                      ,RecordCount
                                    //  ,"INV-001/02587/1019~INV-001/02586/1019"
                                   };
                string[] cFields = { "sih.SalesInvoiceNo like",
                                      "c.CustomerName like",
                                      "cg.CustomerGroupName  like",
                                      "v.VehicleType like",
                                      "v.VehicleNo like",
                                      "sih.SerialNo like",
                                      "sih.InvoiceDateTime>=",
                                      "sih.InvoiceDateTime<=",
                                      "sih.SaleType like",
                                      "sih.Trading like",
                                      "sih.IsPrint like",
                                      "sih.post like",
                                      "sih.IsCurrencyConvCompleted like",
                                      "sih.EXPFormNo like",
                                      "sih.IsVDS like",  
                                      "sih.BranchId",
                                      //"sih."+SearchFields+" like ",
                                      "SelectTop"
                                   //  , "sih.SalesInvoiceNo in"
                                     };


                if (_isVDS)
                {
                    string[] newFields =
                    {
                       "sih.SalesInvoiceNo like",
                                      "c.CustomerName like",
                                      "cg.CustomerGroupName  like",
                                      "v.VehicleType like",
                                      "v.VehicleNo like",
                                      "sih.SerialNo like",
                                      "sih.InvoiceDateTime>=",
                                      "sih.InvoiceDateTime<=",
                                      "sih.SaleType like",
                                      "sih.Trading like",
                                      "sih.IsPrint like",
                                      "sih.post like",
                                      "sih.IsCurrencyConvCompleted like",
                                      "sih.EXPFormNo like",
                                      "sih.BranchId",
                                      "sih.IsVDS like",                                     
                                      "SelectTop"
                    };

                    string[] newcValues =
                    {
                        txtInvoiceNo.Text.Trim(), 
                                      txtCustomerName.Text.Trim(),
                                      txtCustomerGroupName.Text.Trim(),
                                      txtVehicleType.Text.Trim(), 
                                      txtVehicleNo.Text.Trim(), 
                                      txtSerialNo.Text.Trim(), 
                                      SalesFromDate,
                                      SalesToDate, 
                                      Type, 
                                      TradingItem, 
                                      Print,                                      
                                      post, 
                                      convCompltd, 
                                      txtEXPFormNo.Text.Trim(),
                                      SearchBranchId,
                                      IsVDS,
                                      //"N",
                                      RecordCount
                    };
                    cFields = newFields;
                    cValues = newcValues;

                }


                SaleResult = saleDal.SelectAll(0, cFields, cValues, null, null, null, true, transactionType, "N", connVM, "Y", null, null, IsVDSCompltd);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                SaleResult = OrdinaryVATDesktop.DtDeleteColumns(SaleResult, columnNames);

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
                dgvSalesHistory.DataSource = null;
                if (SaleResult != null && SaleResult.Rows.Count > 0)
                {


                    TotalTecordCount = SaleResult.Rows[SaleResult.Rows.Count - 1][0].ToString();

                    SaleResult.Rows.RemoveAt(SaleResult.Rows.Count - 1);


                    System.Data.DataColumn newColumn = new System.Data.DataColumn("Select", typeof(Boolean));
                    newColumn.DefaultValue = false;
                    SaleResult.Columns.Add(newColumn);
                    newColumn.SetOrdinal(0);

                    #region Specific Coloumns Visible False
                    dgvSalesHistory.DataSource = SaleResult;
                    dgvSalesHistory.Columns["Id"].Visible = false;
                    dgvSalesHistory.Columns["PID"].Visible = false;
                    dgvSalesHistory.Columns["VehicleID"].Visible = false;
                    dgvSalesHistory.Columns["ShiftId"].Visible = false;
                    dgvSalesHistory.Columns["TenderId"].Visible = false;
                    dgvSalesHistory.Columns["BranchId"].Visible = false;
                    dgvSalesHistory.Columns["CustomerID"].Visible = false;
                    dgvSalesHistory.Columns["CurrencyID"].Visible = false;
                    dgvSalesHistory.Columns["SaleReturnId"].Visible = false;
                    #endregion

                    chkSelectAll.Checked = false;
                }

                #region Old


                //dgvSalesHistory.Rows.Clear();
                //int j = 0;
                //foreach (DataRow item in SaleResult.Rows)
                //{
                //    DataGridViewRow NewRow = new DataGridViewRow();

                //    dgvSalesHistory.Rows.Add(NewRow);
                //    dgvSalesHistory.Rows[j].Cells["ID"].Value = item["Id"].ToString();
                //    dgvSalesHistory.Rows[j].Cells["SalesInvoiceNo"].Value = item["SalesInvoiceNo"].ToString();// SaleFields[0].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CustomerID"].Value = item["CustomerID"].ToString();// SaleFields[1].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CustomerName"].Value = item["CustomerName"].ToString();// SaleFields[2].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CustomerGroupName"].Value = item["CustomerGroupName"].ToString();// SaleFields[3].ToString();
                //    dgvSalesHistory.Rows[j].Cells["DeliveryAddress1"].Value = item["DeliveryAddress1"].ToString();// SaleFields[4].ToString();
                //    dgvSalesHistory.Rows[j].Cells["DeliveryAddress2"].Value = item["DeliveryAddress2"].ToString();// SaleFields[5].ToString();
                //    dgvSalesHistory.Rows[j].Cells["DeliveryAddress3"].Value = item["DeliveryAddress3"].ToString();// SaleFields[6].ToString();
                //    dgvSalesHistory.Rows[j].Cells["VehicleID"].Value = item["VehicleID"].ToString();// SaleFields[7].ToString();
                //    dgvSalesHistory.Rows[j].Cells["VehicleType"].Value = item["VehicleType"].ToString();// SaleFields[8].ToString();
                //    dgvSalesHistory.Rows[j].Cells["VehicleNo"].Value = item["VehicleNo"].ToString();// SaleFields[9].ToString();
                //    dgvSalesHistory.Rows[j].Cells["TotalAmount"].Value = item["TotalAmount"].ToString();// SaleFields[10].ToString();
                //    dgvSalesHistory.Rows[j].Cells["TotalSubtotal"].Value = item["TotalSubtotal"].ToString();// SaleFields[10].ToString();
                //    dgvSalesHistory.Rows[j].Cells["TotalVATAmount"].Value = item["TotalVATAmount"].ToString();// SaleFields[11].ToString();
                //    dgvSalesHistory.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();// SaleFields[12].ToString();
                //    dgvSalesHistory.Rows[j].Cells["InvoiceDate"].Value = item["InvoiceDate"].ToString();// Convert.ToDateTime(SaleFields[13]).ToString("dd/MMM/yyyy");
                //    dgvSalesHistory.Rows[j].Cells["DeliveryDate"].Value = item["DeliveryDate"].ToString();// Convert.ToDateTime(SaleFields[13]).ToString("dd/MMM/yyyy");
                //    dgvSalesHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// SaleFields[14].ToString();
                //    dgvSalesHistory.Rows[j].Cells["SaleType"].Value = item["SaleType"].ToString();// SaleFields[15].ToString();
                //    dgvSalesHistory.Rows[j].Cells["PID"].Value = item["PID"].ToString();// SaleFields[16].ToString();
                //    dgvSalesHistory.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();// SaleFields[17].ToString();
                //    dgvSalesHistory.Rows[j].Cells["Isprint"].Value = item["IsPrint"].ToString();// SaleFields[18].ToString();
                //    dgvSalesHistory.Rows[j].Cells["TenderId"].Value = item["TenderId"].ToString();// SaleFields[19].ToString();
                //    dgvSalesHistory.Rows[j].Cells["Post1"].Value = item["Post"].ToString();// SaleFields[20].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CurrencyID"].Value = item["CurrencyID"].ToString();// SaleFields[21].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CurrencyCode"].Value = item["CurrencyCode"].ToString();// SaleFields[22].ToString();
                //    dgvSalesHistory.Rows[j].Cells["CurrencyRateFromBDT"].Value = item["CurrencyRateFromBDT"].ToString();// SaleFields[23].ToString();
                //    dgvSalesHistory.Rows[j].Cells["SaleReturnId"].Value = item["SaleReturnId"].ToString();// SaleFields[24].ToString();
                //    dgvSalesHistory.Rows[j].Cells["transactionType"].Value = item["transactionType"].ToString();// SaleFields[25].ToString();
                //    dgvSalesHistory.Rows[j].Cells["AlReadyPrint"].Value = item["AlReadyPrint"].ToString();// SaleFields[26].ToString();
                //    dgvSalesHistory.Rows[j].Cells["LCNumber"].Value = item["LCNumber"].ToString();// SaleFields[27].ToString();
                //    dgvSalesHistory.Rows[j].Cells["DeliveryChallan"].Value = item["DeliveryChallan"].ToString();// SaleFields[28].ToString();
                //    dgvSalesHistory.Rows[j].Cells["IsGatePass"].Value = item["IsGatePass"].ToString();// SaleFields[29].ToString();
                //    dgvSalesHistory.Rows[j].Cells["ImportID"].Value = item["ImportID"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["LCDate"].Value = item["LCDate"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["LCBank"].Value = item["LCBank"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["PINo"].Value = item["PINo"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["PIDate"].Value = item["PIDate"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["EXPFormNo"].Value = item["EXPFormNo"].ToString();// SaleFields[30].ToString();
                //    dgvSalesHistory.Rows[j].Cells["IsDeemedExport"].Value = item["IsDeemedExport"].ToString();
                //    dgvSalesHistory.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();

                //    j = j + 1;

                //}

                //dgvSalesHistory.Columns["DeliveryAddress2"].Visible = false;
                //dgvSalesHistory.Columns["DeliveryAddress3"].Visible = false;
                //dgvSalesHistory.Columns["VehicleID"].Visible = false;
                //dgvSalesHistory.Columns["TenderId"].Visible = false;
                //dgvSalesHistory.Columns["Trading"].Visible = false;
                //dgvSalesHistory.Columns["Comments"].Visible = false;
                ////dgvSalesHistory.Columns["Select"].Visible = false;

                #endregion


                // End Complete

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
                LRecordCount.Text = "Total Record Count " + (SaleResult.Rows.Count) + " of " + TotalTecordCount.ToString();
            }
        }

        #endregion

        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            int j = 0;
            string ids = "";
            string importIds = "";
            for (int i = 0; i < dgvSalesHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvSalesHistory["Select", i].Value) == true)
                {
                    if (dgvSalesHistory["Post", i].Value.ToString() == "N")
                    {
                        string Id = dgvSalesHistory["ID", i].Value.ToString();
                        string ImportID = dgvSalesHistory["ImportID", i].Value.ToString();

                        ids += Id + "~";

                        importIds += ImportID + "~";
                    }
                }
            }
            IdArray = ids.Split('~');
            IdImportArray = importIds.Split('~');

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
                SaleDAL _salDal = new SaleDAL();
                //progressBar1.Visible = true;

                string[] results = _salDal.MultiplePost(IdArray, connVM, Program.CurrentUserID);

                var value = new CommonDAL().settingValue("CompanyCode", "Code", connVM);

                if (results[0].ToLower() == "success" && value == "KCL")
                {
                    var table = new DataTable();

                    table.Columns.Add(("ID"));

                    foreach (var id in IdImportArray)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            table.Rows.Add(id);
                        }
                    }

                    _salDal.PostSales(table, connVM);
                }
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
            searchDT();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();

        }
        private void SelectAll()
        {
            try
            {
                DataTable dt = new DataTable();
                this.Enabled = false;
                dt = SaleResult.Copy();
                if (dt.Columns.Contains("Select"))
                {
                    dt.Columns.Remove("Select");
                }
                System.Data.DataColumn newColumn = new System.Data.DataColumn("Select", typeof(Boolean));
                newColumn.DefaultValue = chkSelectAll.Checked;
                dt.Columns.Add(newColumn);
                newColumn.SetOrdinal(0);
                dgvSalesHistory.DataSource = dt;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                this.Enabled = true;

            }

        }



        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvSalesHistory.Rows.Count;

                //if (len == 0)
                //{
                //    MessageBox.Show("No Data Found");
                //    return;
                //}

                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvSalesHistory["Select", i].Value)
                        //&& dgvSalesHistory["Post1", i].Value.ToString() != "Y"
                        )
                    {
                        invoiceList.Add(dgvSalesHistory["SalesInvoiceNo", i].Value.ToString());
                    }
                }


                //if (invoiceList.Count == 0)
                //{

                //}

                var dal = new SaleDAL();

                DataTable data = dal.GetSaleExcelData(invoiceList, null, null, connVM);

                if (cmbExport.Text == "EXCEL")
                {
                    if (data.Rows.Count == 0)
                    {
                        data.Rows.Add(data.NewRow());
                    }

                    OrdinaryVATDesktop.SaveExcel(data, "Sale", "SaleM");
                    MessageBox.Show("Successfully Exported data in Excel files of root directory");
                }
                else if (cmbExport.Text == "TEXT")
                {
                    OrdinaryVATDesktop.WriteDataToFile(data, "Sale");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }
                else if (cmbExport.Text == "XML")
                {
                    var dataSet = new DataSet("Sale");
                    data.TableName = "SaleM";
                    dataSet.Tables.Add(data);

                    OrdinaryVATDesktop.SaveXML(dataSet, "Sale");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }
                else if (cmbExport.Text.ToLower() == "json")
                {
                    var dataSet = new DataSet("Sale");
                    data.TableName = "Sales";
                    dataSet.Tables.Add(data);

                    OrdinaryVATDesktop.SaveJson(dataSet, "Sale");
                    MessageBox.Show("Successfully Exported data in Json file of root directory");

                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }

        }

        private void SaveExcel(DataTable data, string name, string sheetName)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromDataTable(data, true);
                package.Save();
                objFileStrm.Close();
            }

            MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            txtCustomerID.Text = "";
            txtCustomerName.Text = "";
        }

        private void SyncHSCode_Click(object sender, EventArgs e)
        {
       
            //string InvoicrFrom = dateTimePeriodIdPicker.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            PeriodName = dateTimePeriodIdPicker.Value.ToString("MMMM-yyyy");            
            bgwSyncHSCode.RunWorkerAsync();

        }

        private void bgwSyncHSCode_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                SaleDAL _salDal = new SaleDAL();
           
                string[] results = _salDal.HSCodeUpdate(PeriodName, connVM);

                if (results[0] == "success")
                {
                    MessageBox.Show("HSCode successfully updated");
                }

                this.progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwSyncHSCode_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnSearch.Enabled = true;
            this.progressBar1.Visible = false;
        }
    }
}