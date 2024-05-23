// ---------form //
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
//
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using System.Diagnostics;
using SymphonySofttech.Reports;
using CrystalDecisions.CrystalReports.Engine;
using VATClient.ReportPreview;

namespace VATClient
{
    public partial class FormPurchaseSearch : Form
    {
        #region Constructors

        public FormPurchaseSearch()
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
        DataGridViewRow selectedRow = new DataGridViewRow();
        private string SelectedValue = string.Empty;
        private static string transactionType = string.Empty;
        private static string t1Type = string.Empty;
        private static string t2Type = string.Empty;
        private string PurchaseData = string.Empty;
        private DataTable PurchaseResult;
        private DataTable PurchaseResult2;
        //public string VFIN = "301";
        private string post = string.Empty;
        private string IsRebate = string.Empty;
        private string IsBankingChannelPay = string.Empty;
        private string vds = string.Empty;
        private string tds = string.Empty;
        private static bool _isVDs = false;
        private static bool _isTDs = false;
        private static bool IsClints6_3Search = false;

        public bool isMultiple = false;
        public static bool isDisposeRawSearch = false;
        private static string itemNo = null;

        private DataTable multiple = new DataTable();

        private string PurchaseFromDate, PurchaseToDate;
        string[] IdArray;
        private string SearchBranchId = "0";
        public int SenderBranchId = 0;
        private string RecordCount = "0";

        PurchasePostIds PostId = new PurchasePostIds();
        List<PurchasePostIds> PostIds = new List<PurchasePostIds>();
        private DataSet ReportResult;
        //private ReportDocument reportDocument = new ReportDocument(); 
        private string PeriodName = string.Empty;

        #endregion

        #region Methods

        public static DataGridViewRow SelectOne(string tType, bool isVds = false, bool IsDisposeRaw = false, string ItemNo = null)
        {
            DataGridViewRow selectedRowTemp = null;
            _isVDs = isVds;
            isDisposeRawSearch = IsDisposeRaw;
            itemNo = ItemNo;
            if (tType == "All")
            {
                transactionType = "All";
            }
            else
            { transactionType = tType; }
            try
            {
                #region Statement

                FormPurchaseSearch frmPurchaseSearch = new FormPurchaseSearch();
                frmPurchaseSearch.SenderBranchId = Program.BranchId;


                if (tType == "All")
                {
                    frmPurchaseSearch.Text = "All Purchase Search";
                }
                else if (tType == "Other")
                {
                    frmPurchaseSearch.Text = "Purchase Search";
                }
                else if (tType == "Trading")
                {
                    frmPurchaseSearch.Text = "Purchase Search(Trading)";

                }
                else if (tType == "IssueReturn")
                {

                    frmPurchaseSearch.Text = "Issue Return Search";
                }
                else if (tType == "TollReceive")
                {

                    frmPurchaseSearch.Text = "Toll Receive Search";

                }

                else if (tType == "PurchaseReturn")
                {

                    frmPurchaseSearch.Text = "Purchase Return Search";

                }
                else if (tType == "InputService")
                {

                    frmPurchaseSearch.Text = "Input Service Search";

                }
                else if (tType == "Import")
                {

                    frmPurchaseSearch.Text = "Import Search";

                }

                else if (tType == "TollReceiveRaw")
                {

                    frmPurchaseSearch.Text = "Toll Receive Raw Search";

                }
                else if (tType == "ClientRawReceive")
                {
                    frmPurchaseSearch.Text = "Raw Receive Search";
                }
                else if (tType == "ClientFGReceiveWOBOM")
                {
                    frmPurchaseSearch.Text = "FG Receive WOBOM Receive Search";
                }

                frmPurchaseSearch.ShowDialog();

                //return frmPurchaseSearchSelectedValue = frmPurchaseSearch.SelectedValue;
                selectedRowTemp = frmPurchaseSearch.selectedRow;

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
                MessageBox.Show(ex.Message, "FormPurchaseSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormPurchaseSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }


        public static DataTable SelectMultiple(string tType, bool isVds = false, bool isMultiple = false, bool isTds = false, bool IsDisposeRaw = false, string ItemNo = null,bool IsClints6_3 = false)
        {
            DataTable selectedRowTemp = null;
            _isVDs = isVds;
            _isTDs = isTds;
            isDisposeRawSearch = IsDisposeRaw;
            itemNo = ItemNo;
            IsClints6_3Search = IsClints6_3;

            if (tType == "All")
            {
                transactionType = "All";
            }
            else
            {
                transactionType = tType;
            }


            try
            {
                #region Statement

                FormPurchaseSearch frmPurchaseSearch = new FormPurchaseSearch();
                frmPurchaseSearch.btnMultiple.Visible = isMultiple;
                frmPurchaseSearch.btnOk.Visible = false;
                frmPurchaseSearch.SenderBranchId = Program.BranchId;

                #region Conditional Values

                if (_isTDs)
                {
                    frmPurchaseSearch.cmbTds.Enabled = false;
                    frmPurchaseSearch.cmbTds.SelectedIndex = 0;
                }
                else if (_isVDs)
                {
                    frmPurchaseSearch.cmbVDS.Enabled = false;
                    frmPurchaseSearch.cmbVDS.SelectedIndex = 0;
                    frmPurchaseSearch.cmbPost.Enabled = false;
                    frmPurchaseSearch.cmbPost.SelectedIndex = 0;

                }

                if (tType == "All")
                {
                    frmPurchaseSearch.Text = "All Purchase Search";
                }
                else if (tType == "Other")
                {
                    frmPurchaseSearch.Text = "Purchase Search";
                }
                else if (tType == "Trading")
                {
                    frmPurchaseSearch.Text = "Purchase Search(Trading)";
                }
                else if (tType == "IssueReturn")
                {
                    frmPurchaseSearch.Text = "Issue Return Search";
                }
                else if (tType == "TollReceive")
                {
                    frmPurchaseSearch.Text = "Toll Receive Search";
                }
                else if (tType == "PurchaseReturn")
                {
                    frmPurchaseSearch.Text = "Purchase Return Search";
                }
                else if (tType == "InputService")
                {
                    frmPurchaseSearch.Text = "Input Service Search";
                }
                else if (tType == "Import")
                {
                    frmPurchaseSearch.Text = "Import Search";
                }
                else if (tType == "TollReceiveRaw")
                {
                    frmPurchaseSearch.Text = "Toll Receive Raw Search";
                }

                #endregion


                frmPurchaseSearch.ShowDialog();

                //return frmPurchaseSearchSelectedValue = frmPurchaseSearch.SelectedValue;
                selectedRowTemp = frmPurchaseSearch.multiple;

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
                MessageBox.Show(ex.Message, "FormPurchaseSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormPurchaseSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        //------------------

        public static DataGridViewRow SelectTwo(string tType1, string tType2)
        {
            DataGridViewRow selectedRowTemp = null;
            t1Type = tType1;
            t2Type = tType2;
            try
            {
                #region Statement

                FormPurchaseSearch frmPurchaseSearch = new FormPurchaseSearch();

                if (tType1 == "Import" || tType2 == "TrandingImport")
                {
                    frmPurchaseSearch.Text = "Import or TradingImport";
                }




                frmPurchaseSearch.ShowDialog();

                //return frmPurchaseSearchSelectedValue = frmPurchaseSearch.SelectedValue;
                selectedRowTemp = frmPurchaseSearch.selectedRow;

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
                MessageBox.Show(ex.Message, "FormPurchaseSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormPurchaseSearch", "SelectTwo", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }
        //------------------

        private void GridSeleted()
        {
            try
            {
                #region Statement

                if (dgvPurchaseHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvPurchaseHistory.SelectedRows;

                ////////if (rbtnBankChannelPayment.Checked)
                ////////{
                ////////    FormBankChannelPayment frm = new FormBankChannelPayment();

                ////////    #region Statement

                ////////    if (selectedRows != null && selectedRows.Count > 0)
                ////////    {
                ////////        DataGridViewRow dgvRow = selectedRows[0];
                ////////        if (dgvRow != null)
                ////////        {
                ////////            #region Value Assign

                ////////            frm.txtPurchaseInvoiceNo.Text = dgvRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                ////////            frm.txtPaymentAmount.Text = dgvRow.Cells["TotalAmount"].Value.ToString();
                ////////            frm.txtVATAmount.Text = dgvRow.Cells["TotalVATAmount"].Value.ToString();

                ////////            #endregion

                ////////        }
                ////////    }

                ////////    frm.ShowDialog();

                ////////    #endregion

                ////////}
                ////////else
                ////////{
                    if (selectedRows != null && selectedRows.Count > 0)
                    {
                        selectedRow = selectedRows[0];
                    }

                    this.Close();
                //////}

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }


        private void MultipleRowSelect()
        {
            var gd1 = dgvPurchaseHistory;
            var table = new DataTable();

            foreach (DataGridViewColumn col in gd1.Columns)
            {
                table.Columns.Add(col.Name);
            }

            //  dgvPurchaseHistory.Rows[j].Cells["ID"].Value

            for (int i = 0; i < gd1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(gd1["Select", i].Value))
                {
                    DataRow dRow = table.NewRow();
                    foreach (DataGridViewCell cell in gd1.Rows[i].Cells)
                    {
                        dRow[cell.ColumnIndex] = cell.Value;
                    }

                    table.Rows.Add(dRow);
                }
            }

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.Close();

            multiple = table;
        }

        private void NullCheck()
        {
            try
            {
                #region Statement

                if (dtpPurchaseFromDate.Checked == false)
                {
                    PurchaseFromDate = "";
                }
                else
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpPurchaseToDate.Checked == false)
                {
                    PurchaseToDate = "";
                }
                else
                {
                    //PurchaseToDate = dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
                    PurchaseToDate = dtpPurchaseToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void ClearAllFields()
        {
            txtGrandTotalFrom.Text = "0.00";
            txtGrandTotalTo.Text = "0.00";
            txtInvoiceNo.Text = "";
            txtSerialNo.Text = "";
            txtVatAmountFrom.Text = "0.00";
            txtVatAmountTo.Text = "0.00";
            txtVendorGroupName.Text = "";

            txtVendorName.Text = "";
            dtpPurchaseFromDate.Text = "";
            dtpPurchaseToDate.Text = "";
            dgvPurchaseHistory.DataSource = null;
            ////.Rows.Clear();
            cmbPost.SelectedIndex = -1;
            cmbVDS.SelectedIndex = -1;
            txtBENumber.Text = "";
        }

        private void Search()
        {
            try
            {
                NullCheck();

                post = string.Empty;
                post = cmbPost.Text.Trim();
                vds = string.Empty;
                vds = cmbVDS.Text.Trim();
                tds = cmbTds.Text.Trim();

                IsRebate = string.Empty;
                IsRebate = cmbIsRebate.Text.Trim();
                IsBankingChannelPay = cmbIsBankingChannelPay.Text.Trim();

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                bgwPurchaseSearch.RunWorkerAsync();

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBranchId = cmbBranch.SelectedValue.ToString();
            RecordCount = cmbRecordCount.Text.Trim();

            Search();

        }


        private void VendorLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select v.VendorID, v.VendorCode,v.VendorName,vg.VendorGroupName,v.VATRegistrationNo,v.TINNo
                                    from Vendors v
                                    left outer join VendorGroups vg on v.VendorGroupID=vg.VendorGroupID
                                    where 1=1 and  v.ActiveStatus='Y'   ";
                string ShowAllVendor = new CommonDAL().settingsDesktop("Setup", "ShowAllVendor",null,connVM);
                if (ShowAllVendor.ToLower() == "n")
                {
                    SqlText += @" and v.BranchId='" + Program.BranchId + "'";
                }
                string SQLTextRecordCount = @" select count(VendorCode)RecordNo from Vendors 
                                ";

                string[] shortColumnName = { "v.VendorID", "v.VendorCode", "v.VendorName", "vg.VendorGroupName", "v.VATRegistrationNo", "v.VATRegistrationNo", "v.TINNo" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtVendorID.Text = "";
                    txtVendorName.Text = "";
                    txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();

                }
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void VendorGroupLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select VendorGroupID,VendorGroupName,VendorGroupDescription,GroupType
                                    from VendorGroups
                                  
                                    where 1=1 and  ActiveStatus='Y'   ";
                string SQLTextRecordCount = @" select count(VendorGroupID)RecordNo from VendorGroups ";

                string[] shortColumnName = { "VendorGroupID", "VendorGroupName", "VendorGroupDescription", "GroupType" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtVendorGroupID.Text = "";
                    txtVendorGroupName.Text = "";
                    txtVendorGroupID.Text = selectedRow.Cells["VendorGroupID"].Value.ToString();
                    txtVendorGroupName.Text = selectedRow.Cells["VendorGroupName"].Value.ToString();

                }
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

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

        private void txtVendorID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorLoad();
            }
        }

        private void txtVendorGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorGroupLoad();
            }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpPurchaseFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpPurchaseToDate_KeyDown(object sender, KeyEventArgs e)
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

        }

        #endregion

        private void dgvPurchaseHistory_DoubleClick(object sender, EventArgs e)
        {

            if (!rbtnBankChannelPayment.Checked)
            {
                GridSeleted();

                selectedRow.Cells["Select"].Value = true;
                MultipleRowSelect();

            }

        }

        private void txtVatAmountFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVatAmountFrom, "VAT Amount From");
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

        private void FormPurchaseSearch_Load(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                bool RebateWithGRN = false;

                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

                cmbBranch.SelectedValue = Program.BranchId;
                cmbExport.SelectedIndex = 0;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                #region RebateWithGRN

                CommonDAL commonDal = new CommonDAL();
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null,connVM);
                }

                RebateWithGRN = commonDal.settingsDesktop("Purchase", "RebateWithGRN", settingsDt,connVM) == "Y";

                if (RebateWithGRN)
                {
                    btnRebate.Visible = false;
                    label7.Visible = false;
                    dtpRebateDate.Visible = false;
                }
                else
                {
                    btnRebate.Visible = true;
                    label7.Visible = true;
                    dtpRebateDate.Visible = true;
                }

                #endregion

                // btnMultiple.Visible = isMultiple;

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                //Search();

                if (rbtnBankChannelPayment.Checked)
                {
                    this.Text = "Bank Channel Payment";
                    btnOk.Visible = false;
                    btnBankChannel.Visible = true;
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void dgvPurchaseHistory_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void dgvPurchaseHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        #region backgounrWorker Events

        private void bgwPurchaseSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                //////PurchaseDAL purchaseDal = new PurchaseDAL();


                if (transactionType.ToLower() == "all")
                {
                    transactionType = "";
                }

                bool BankingChannel = false;

                if (rbtnBankChannelPayment.Checked)
                {
                    BankingChannel = true;
                }

                string[] cValues =
                {
                    txtInvoiceNo.Text.Trim(), vds, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                    txtVendorGroupName.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtSerialNo.Text.Trim(),
                    transactionType, txtBENumber.Text.Trim(),
                    post, SearchBranchId, tds,RecordCount,IsRebate,IsBankingChannelPay
                };//transactionType was 'all'
                string[] cFields =
                {
                    "pih.PurchaseInvoiceNo like", 
                    "pih.WithVDS like", 
                    "v.VendorName  like", 
                    "v.VendorGroupID ", 
                    "vg.VendorGroupName like", "pih.ReceiveDate>", 
                    "pih.ReceiveDate<", 
                    "pih.SerialNo like", 
                    "pih.TransactionType", 
                    "pih.BENumber like", 
                    "pih.post like",
                    "pih.BranchId",
                    "pih.IsTDS isnull",
                    "SelectTop",
                    "pih.IsRebate like",
                    "pih.IsBankingChannelPay"
                };

                if (_isTDs)
                {
                    string[] newFields =
                    {
                    "pih.PurchaseInvoiceNo like", 
                    "pih.WithVDS like", 
                    "v.VendorName  like", 
                    "v.VendorGroupID ", 
                    "vg.VendorGroupName like", "pih.ReceiveDate>", 
                    "pih.ReceiveDate<", 
                    "pih.SerialNo like", 
                    "pih.TransactionType", 
                    "pih.BENumber like", 
                    "pih.post like",
                    "pih.BranchId",
                    "pih.IsTDS isnull",
                    "pih.IsTDSCompleted isnull",
                    "SelectTop"
                    };

                    string[] newcValues =
                     {
                    txtInvoiceNo.Text.Trim(), vds, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                    txtVendorGroupName.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtSerialNo.Text.Trim(),
                    transactionType, txtBENumber.Text.Trim(),
                    post, SearchBranchId, "Y","N",RecordCount
                    };
                    cFields = newFields;
                    cValues = newcValues;

                }
                else if (_isVDs)
                {
                    string[] newFields =
                    {
                        "pih.PurchaseInvoiceNo like", 
                        "pih.WithVDS ", 
                        "v.VendorName  like", 
                        "v.VendorGroupID ", 
                        "vg.VendorGroupName like", "pih.ReceiveDate>", 
                        "pih.ReceiveDate<", 
                        "pih.SerialNo like", 
                        "pih.BENumber like", 
                        "pih.post like",
                        "pih.BranchId",
                        "pih.IsTDS isnull",
                        "pih.IsVDSCompleted isnull",
                        "SelectTop"
                    };

                    string[] newcValues =
                    {
                        txtInvoiceNo.Text.Trim(), vds, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                        txtVendorGroupName.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtSerialNo.Text.Trim(),
                        txtBENumber.Text.Trim(),
                        post, SearchBranchId, tds,"N",RecordCount
                    };
                    cFields = newFields;
                    cValues = newcValues;

                }

                else if (IsClints6_3Search)
                {
                    string[] newFields =
                    {
                    "pih.PurchaseInvoiceNo like", 
                    "pih.WithVDS like", 
                    "v.VendorName  like", 
                    "v.VendorGroupID ", 
                    "vg.VendorGroupName like", "pih.ReceiveDate>", 
                    "pih.ReceiveDate<", 
                    "pih.SerialNo like",                     
                    "pih.BENumber like", 
                    "pih.post like",
                    "pih.BranchId",
                    "pih.IsTDS isnull",
                    "SelectTop",
                    "pih.IsRebate like",
                    "pih.IsBankingChannelPay",
                    "pih.IsClients6_3Complete isnull"

                    };

                    ////string[] newcValues =
                    ////{
                    ////    txtInvoiceNo.Text.Trim(), vds, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                    ////txtVendorGroupName.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtSerialNo.Text.Trim(),
                    ////transactionType, txtBENumber.Text.Trim(),
                    ////post, SearchBranchId, tds,RecordCount,IsRebate,IsBankingChannelPay,"N"
                    ////};
                    string[] newcValues =
                    {
                        txtInvoiceNo.Text.Trim(), vds, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                    txtVendorGroupName.Text.Trim(), PurchaseFromDate, PurchaseToDate, txtSerialNo.Text.Trim(),
                     txtBENumber.Text.Trim(),
                    post, SearchBranchId, tds,RecordCount,IsRebate,IsBankingChannelPay,"N"
                    };
                    cFields = newFields;
                    cValues = newcValues;
                }

                PurchaseResult = purchaseDal.SelectAll(0, cFields, cValues, null, null, null, true, connVM, _isVDs, isDisposeRawSearch, itemNo, BankingChannel, IsClints6_3Search);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                PurchaseResult = OrdinaryVATDesktop.DtDeleteColumns(PurchaseResult, columnNames);

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void bgwPurchaseSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string TotalTecordCount = "0";
            try
            {
                #region Statement
                dgvPurchaseHistory.DataSource = null;
                if (PurchaseResult != null && PurchaseResult.Rows.Count > 0)
                {

                    TotalTecordCount = PurchaseResult.Rows[PurchaseResult.Rows.Count - 1][0].ToString();

                    PurchaseResult.Rows.RemoveAt(PurchaseResult.Rows.Count - 1);
                    dgvPurchaseHistory.DataSource = PurchaseResult;
                    #region Specific Coloumns Visible False
                    dgvPurchaseHistory.Columns["Id"].Visible = false;
                    dgvPurchaseHistory.Columns["VendorGroupID"].Visible = false;
                    dgvPurchaseHistory.Columns["VendorID"].Visible = false;
                    dgvPurchaseHistory.Columns["PurchaseReturnId"].Visible = false;
                    dgvPurchaseHistory.Columns["BranchID"].Visible = false;
                    dgvPurchaseHistory.Columns["CurrencyID"].Visible = false;
                    #endregion
                }
                #endregion

                #region Old

                // Start Complete
                //dgvPurchaseHistory.Rows.Clear();
                //int j = 0;

                //foreach (DataRow item2 in PurchaseResult.Rows)
                //{

                //    DataGridViewRow NewRow = new DataGridViewRow();

                //    dgvPurchaseHistory.Rows.Add(NewRow);
                //    dgvPurchaseHistory.Rows[j].Cells["ID"].Value = item2["Id"].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["PurchaseInvoiceNo"].Value = item2["PurchaseInvoiceNo"].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["VendorID"].Value = item2["VendorID"].ToString();// PurchaseFields[1].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["CustomHouse"].Value = item2["CustomHouse"].ToString();// PurchaseFields[1].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["VendorName"].Value = item2["VendorName"].ToString();// PurchaseFields[2].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["VendorGroupID"].Value = item2["VendorGroupID"].ToString();// PurchaseFields[3].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["VendorGroupName"].Value = item2["VendorGroupName"].ToString();// PurchaseFields[4].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["InvoiceDate"].Value = item2["InvoiceDateTime"].ToString();// Convert.ToDateTime(PurchaseFields[5]).ToString("dd/MMM/yyyy");
                //    dgvPurchaseHistory.Rows[j].Cells["TotalAmount"].Value = item2["TotalAmount"].ToString();// Convert.ToDecimal(PurchaseFields[6].ToString()).ToString("0.00");
                //    dgvPurchaseHistory.Rows[j].Cells["TotalVATAmount"].Value = item2["TotalVATAmount"].ToString();// Convert.ToDecimal(PurchaseFields[7].ToString()).ToString("0.00");
                //    dgvPurchaseHistory.Rows[j].Cells["SerialNo"].Value = item2["SerialNo"].ToString();// PurchaseFields[8].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["LCNumber"].Value = item2["LCNumber"].ToString();// PurchaseFields[8].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// PurchaseFields[9].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["ProductType"].Value = item2["ProductType"].ToString();// PurchaseFields[10].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["Post1"].Value = item2["Post"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["WithVDS"].Value = item2["WithVDS"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["ReceiveDate"].Value = item2["ReceiveDate"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["BENumber"].Value = item2["BENumber"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["ReturnId"].Value = item2["PurchaseReturnId"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["TransType"].Value = item2["TransactionType"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["ReturnId"].Value = item2["PurchaseReturnId"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["TransType"].Value = item2["TransactionType"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["LCDate"].Value = item2["LCDate"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["LandedCost"].Value = item2["LandedCost"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["VDSAmount"].Value = item2["VDSAmount"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["TDSAmount"].Value = item2["TDSAmount"].ToString();// PurchaseFields[11].ToString();
                //    dgvPurchaseHistory.Rows[j].Cells["BranchId"].Value = item2["BranchId"].ToString();// PurchaseFields[11].ToString();
                //    j = j + 1;

                //}


                //dgvPurchaseHistory.Columns["VendorID"].Visible = false;
                //dgvPurchaseHistory.Columns["VendorGroupID"].Visible = false;
                //dgvPurchaseHistory.Columns["Comments"].Visible = false;


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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
            finally
            {
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                IsClints6_3Search = false;
                if (PurchaseResult == null)
                {
                    LRecordCount.Text = "Total Record Count " + 0;
                }
                else
                {
                    LRecordCount.Text = "Total Record Count " + (PurchaseResult.Rows.Count) + " of " + TotalTecordCount.ToString();
                }
            }
        }

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void bgwPurchaseSearch2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                PurchaseDAL purchaseDal = new PurchaseDAL();
                //IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);


                PurchaseResult2 = purchaseDal.SearchPurchaseHeaderDTNew2(
                txtInvoiceNo.Text.Trim(),
                vds,
                txtVendorName.Text.Trim(),
                txtVendorGroupID.Text.Trim(),
                txtVendorGroupName.Text.Trim(),
                PurchaseFromDate,
                PurchaseToDate,
                txtSerialNo.Text.Trim(),
                t1Type,
                t2Type,
                txtBENumber.Text.Trim(),
                post, connVM);  // Change 04 robin

                // End DoWork

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void bgwPurchaseSearch2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                dgvPurchaseHistory.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in PurchaseResult2.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvPurchaseHistory.Rows.Add(NewRow);
                    dgvPurchaseHistory.Rows[j].Cells["PurchaseInvoiceNo"].Value = item2["PurchaseInvoiceNo"].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["VendorID"].Value = item2["VendorID"].ToString();// PurchaseFields[1].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["VendorName"].Value = item2["VendorName"].ToString();// PurchaseFields[2].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["VendorGroupID"].Value = item2["VendorGroupID"].ToString();// PurchaseFields[3].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["VendorGroupName"].Value = item2["VendorGroupName"].ToString();// PurchaseFields[4].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["InvoiceDate"].Value = item2["InvoiceDateTime"].ToString();// Convert.ToDateTime(PurchaseFields[5]).ToString("dd/MMM/yyyy");
                    dgvPurchaseHistory.Rows[j].Cells["TotalAmount"].Value = item2["TotalAmount"].ToString();// Convert.ToDecimal(PurchaseFields[6].ToString()).ToString("0.00");
                    dgvPurchaseHistory.Rows[j].Cells["TotalVATAmount"].Value = item2["TotalVATAmount"].ToString();// Convert.ToDecimal(PurchaseFields[7].ToString()).ToString("0.00");
                    dgvPurchaseHistory.Rows[j].Cells["SerialNo"].Value = item2["SerialNo"].ToString();// PurchaseFields[8].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// PurchaseFields[9].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["ProductType"].Value = item2["ProductType"].ToString();// PurchaseFields[10].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["Post1"].Value = item2["Post"].ToString();// PurchaseFields[11].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["WithVDS"].Value = item2["WithVDS"].ToString();// PurchaseFields[11].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["ReceiveDate"].Value = item2["ReceiveDate"].ToString();// PurchaseFields[11].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["BENumber"].Value = item2["BENumber"].ToString();// PurchaseFields[11].ToString();
                    dgvPurchaseHistory.Rows[j].Cells["ReturnId"].Value = item2["PurchaseReturnId"].ToString();// PurchaseFields[11].ToString();

                    j = j + 1;

                }


                dgvPurchaseHistory.Columns["VendorID"].Visible = false;
                dgvPurchaseHistory.Columns["VendorGroupID"].Visible = false;
                dgvPurchaseHistory.Columns["Comments"].Visible = false;


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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

            this.btnSearch.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void btnDutyDownload_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            int j = 0;
            string ids = "";
            for (int i = 0; i < dgvPurchaseHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvPurchaseHistory["Select", i].Value) == true)
                {
                    if (dgvPurchaseHistory["post", i].Value.ToString() == "N")
                    {
                        string Id = dgvPurchaseHistory["ID", i].Value.ToString();
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
                //PurchaseDAL _purDal = new PurchaseDAL();
                IPurchase _purDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                //this.progressBar1.Visible = true;
                string[] results = _purDal.MultiplePost(IdArray, connVM);
                if (results[0] == "Success")
                {
                    MessageBox.Show("All Purchases posted successfully");
                }

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
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
                for (int i = 0; i < dgvPurchaseHistory.RowCount; i++)
                {
                    dgvPurchaseHistory["Select", i].Value = true;
                }
            }
            else
            {
                for (int i = 0; i < dgvPurchaseHistory.RowCount; i++)
                {
                    dgvPurchaseHistory["Select", i].Value = false;
                }
            }
        }

        private void btnMultiple_Click(object sender, EventArgs e)
        {
            MultipleRowSelect();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvPurchaseHistory.Rows.Count;

                //if (len == 0)
                //{
                //    MessageBox.Show("No Data Found");
                //    return;
                //}

                //for (var i = 0; i < len; i++)
                //{
                //    if (Convert.ToBoolean(dgvPurchaseHistory["Select", i].Value) && dgvPurchaseHistory["post1", i].Value.ToString() != "Y")
                //    {
                //        invoiceList.Add(dgvPurchaseHistory["PurchaseInvoiceNo", i].Value.ToString());
                //    }
                //}

                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvPurchaseHistory["Select", i].Value))
                    {
                        invoiceList.Add(dgvPurchaseHistory["PurchaseInvoiceNo", i].Value.ToString());
                    }
                }



                //if (invoiceList.Count == 0)
                //    return;

                //var dal = new PurchaseDAL();
                IPurchase dal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                var data = dal.GetExcelData(invoiceList, chkDuty.Checked, null, null, connVM);


                if (cmbExport.Text == "EXCEL")
                {
                    if (data.Rows.Count == 0)
                    {
                        data.Rows.Add(data.NewRow());
                    }

                    OrdinaryVATDesktop.SaveExcel(data, "Purchase", "PurchaseM");
                    MessageBox.Show("Successfully Exported data in Excel files of root directory");
                }
                else if (cmbExport.Text == "TEXT")
                {
                    OrdinaryVATDesktop.WriteDataToFile(data, "Purchase");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }
                else if (cmbExport.Text == "XML")
                {
                    var dataSet = new DataSet("Purchase");
                    data.TableName = "PurchaseM";
                    dataSet.Tables.Add(data);

                    OrdinaryVATDesktop.SaveXML(dataSet, "Purchase");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnVendorRefresh_Click(object sender, EventArgs e)
        {
            txtVendorID.Text = "";
            txtVendorName.Text = "";
        }

        private void btnVendorGroupRefresh_Click(object sender, EventArgs e)
        {
            txtVendorGroupID.Text = "";
            txtVendorGroupName.Text = "";
        }

        private void btnRebate_Click(object sender, EventArgs e)
        {
            //int j = 0;
            //string ids = "";
            PostId = new PurchasePostIds();
            PostIds = new List<PurchasePostIds>();

            #region Find Fiscal Year Lock
            string PeriodName = dtpRebateDate.Value.ToString("MMMM-yyyy");
            string[] cValues = { PeriodName };
            string[] cFields = { "PeriodName" };
            FiscalYearVM varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues,null,null,connVM).FirstOrDefault();

            if (varFiscalYearVM == null)
            {
                throw new ArgumentException(PeriodName + ": This Fiscal Period is not Exist!");
            }

            #endregion

            for (int i = 0; i < dgvPurchaseHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvPurchaseHistory["Select", i].Value) == true)
                {
                    if (dgvPurchaseHistory["post", i].Value.ToString() == "Y" && dgvPurchaseHistory["IsRebate", i].Value.ToString() == "N")
                    {
                        string Id = dgvPurchaseHistory["ID", i].Value.ToString();
                        //ids += Id + "~";
                        PostId = new PurchasePostIds();
                        PostId.Id = Convert.ToInt32(Id);
                        PostId.IsRebate = "Y";
                        PostId.RebatePeriodId = varFiscalYearVM.PeriodID;
                        PostId.RebateDate = varFiscalYearVM.PeriodStart;

                        PostIds.Add(PostId);
                    }
                }
            }
            //IdArray = ids.Split('~');
            if (PostIds.Count <= 0)
            {
                MessageBox.Show(this, "All data already Rebate !");
                return;
            }

            if (PostIds == null || PostIds.Count <= 0)
            {
                return;
            }
            else if (
            MessageBox.Show(MessageVM.msgWantToRebate, this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            bgwMultipleRebate.RunWorkerAsync();
        }

        private void bgwMultipleRebate_DoWork(object sender, DoWorkEventArgs e)
        {
            PurchaseDAL _purDal = new PurchaseDAL();

            string[] results = _purDal.MultipleRebate(PostIds, connVM);
            if (results[0] == "Success")
            {
                MessageBox.Show("All Purchases Rebate successfully");
            }
        }

        private void bgwMultipleRebate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Search();
        }

        private void btnBankChannel_Click(object sender, EventArgs e)
        {
            try
            {


                var gd1 = dgvPurchaseHistory;
                var table = new DataTable();

                foreach (DataGridViewColumn col in gd1.Columns)
                {
                    table.Columns.Add(col.Name);
                }

                //  dgvPurchaseHistory.Rows[j].Cells["ID"].Value

                for (int i = 0; i < gd1.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(gd1["Select", i].Value))
                    {
                        DataRow dRow = table.NewRow();
                        foreach (DataGridViewCell cell in gd1.Rows[i].Cells)
                        {
                            dRow[cell.ColumnIndex] = cell.Value;
                        }

                        table.Rows.Add(dRow);
                    }
                }

                if (table.Rows.Count == 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                multiple = table;

                FormBankChannelPayment frm = new FormBankChannelPayment();
                frm.SetGridViewData(multiple);
                frm.ShowDialog();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnBankChannelMIS_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                FormRptBankChannel frm = new FormRptBankChannel();
                frm.Show();

            }

            #endregion

            #region catch

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            #endregion

        }

        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorkerMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

           

        }

        private void btnMltSave_Click(object sender, EventArgs e)
        {
            try
            {

                FormCollection fc = Application.OpenForms;

                foreach (Form frm in fc)
                {
                    if (frm.Name == "FormPurchaseMultiples")
                    {
                        frm.BringToFront();
                        return;
                    }
                }

                FormPurchaseMultiples frmNew = new FormPurchaseMultiples(transactionType);

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

        private void btnExportTDS_Click(object sender, EventArgs e)
        {
            try
            {
                var invoiceList = new List<string>();

                var len = dgvPurchaseHistory.Rows.Count;

                if (len == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

              
                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvPurchaseHistory["Select", i].Value))
                    {
                        invoiceList.Add(dgvPurchaseHistory["PurchaseInvoiceNo", i].Value.ToString());
                    }
                }


                if (invoiceList.Count == 0)
                {
                    MessageBox.Show("Please select invoice Before export");
                    return;

                }

                PurchaseDAL dal = new PurchaseDAL();

                var data = dal.GetPurchaseTDSExcelData(invoiceList, null, null, connVM);


                if (cmbExport.Text == "EXCEL")
                {
                    if (data.Rows.Count == 0)
                    {
                        data.Rows.Add(data.NewRow());
                    }

                    OrdinaryVATDesktop.SaveExcel(data, "PurchaseTDS", "PurchaseTDS");
                    MessageBox.Show("Successfully Exported data in Excel files of root directory");
                }
                else if (cmbExport.Text == "TEXT")
                {
                    OrdinaryVATDesktop.WriteDataToFile(data, "Purchase");
                    MessageBox.Show("Successfully Exported data in TEXT file of root directory");
                }
                else if (cmbExport.Text == "XML")
                {
                    var dataSet = new DataSet("PurchaseTDS");
                    data.TableName = "PurchaseTDS";
                    dataSet.Tables.Add(data);

                    OrdinaryVATDesktop.SaveXML(dataSet, "PurchaseTDS");
                    MessageBox.Show("Successfully Exported data in XML file of root directory");
                }

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void SyncHSCode_Click(object sender, EventArgs e)
        {
            PeriodName = dateTimePeriodIdPicker.Value.ToString("MMMM-yyyy");
            bgwSyncHSCode.RunWorkerAsync();

        }

        private void bgwSyncHSCode_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                PurchaseDAL _purchaseDal = new PurchaseDAL();

                string[] results = _purchaseDal.HSCodeUpdatePurchase(PeriodName, connVM);

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
