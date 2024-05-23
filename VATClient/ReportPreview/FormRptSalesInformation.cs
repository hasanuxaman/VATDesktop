using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
////
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
//
using VATServer.Library;
using DataTable = System.Data.DataTable;
using VATViewModel.DTOs;
using VATServer.License;
using VATServer.Ordinary;
using System.Diagnostics;
using OfficeOpenXml;
using VATDesktop.Repo;
using VATServer.Interface;
using OfficeOpenXml.Style;

namespace VATClient.ReportPages
{
    public partial class FormRptSalesInformation : Form
    {
        public FormRptSalesInformation()
        {
            InitializeComponent();

            connVM = Program.OrdinaryLoad();
        }

        //private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #region Global Variables For BackGroundWorker

        private DataSet ReportResult;
        private DataTable ReportResultDt;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private bool bPromotional = false;

        private string cmbTradingText,
            cmbNonStockText,
            cmbTypeText,
            cmbPostText,
            cmbwaste,
            DiscountOnly,
            Promotional;

        private string Report = string.Empty;
        private string vTransactionType = string.Empty;
        private string transactionType = string.Empty;

        #endregion

        string[] Condition = new string[] { "one" };
        private static string ShiftId = "0";
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet ReportMIS;
        public int SenderBranchId = 0;
        public int branchId;
        private string VatType = "";
        private string OrderBy = "";
        private string Channel = "";
        public string Toll = string.Empty;
        public string FormNumeric = string.Empty;
        public string cmbReportType = string.Empty;

        #region Excel

        private DataTable ReportDataTable;
        private DataTable dtsorted;
        private bool ExcelRpt = false;
        private bool IsMISExcel = false;
        private DataGridView dgvSale;
        private Microsoft.Office.Interop.Excel.Application xlApp;
        private Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        private object misValue = System.Reflection.Missing.Value;
        private string saveLocation;

        #endregion

        string SalesFromDate, SalesToDate;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VATTypeLoad()
        {
            cmbType.Items.Clear();
            cmbType.Items.Add("All");
            cmbType.Items.Add("Export");
            cmbType.Items.Add("DeemExport");

            cmbType.Items.Add("VAT");
            cmbType.Items.Add("NonVAT");
            cmbType.Items.Add("MRPRate");
            cmbType.Items.Add("FixedVAT");
            cmbType.Items.Add("OtherRate");
            cmbType.Items.Add("Retail");

            cmbType.SelectedIndex = 0;
        }

        private void ClearAllFields()
        {
            try
            {
                txtCustomerGroupID.Text = "";
                txtCustomerGroupName.Text = "";
                txtProductType.Text = "";

                cmbPost.SelectedIndex = -1;
                cmbDiscount.SelectedIndex = -1;
                cmbChannel.SelectedIndex = 0;

                txtCustomerName.Text = "";
                txtItemNo.Text = "";
                txtPGroupId.Text = "";
                txtPGroup.Text = "";
                txtProductName.Text = "";
                txtSalesInvoiceNo.Text = "";
                txtChassis.Text = "";
                txtEngine.Text = "";
                dtpSalesFromDate.Checked = false;
                dtpSalesToDate.Checked = false;
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

        private void NullCheck()
        {
            try
            {
                cmbPostText = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";
                cmbTypeText = txtProductType.Text.Trim(); // cmbType.SelectedIndex != 1 ? cmbType.Text.Trim() : "";
                DiscountOnly = cmbDiscount.SelectedIndex != -1 ? cmbDiscount.Text.Trim() : "";
                bPromotional = chkPromotional.Checked ? true : false;
                vTransactionType = txtTransactionType.Text.Trim();

                if (dtpSalesFromDate.Checked == false)
                {
                    SalesFromDate = "";
                }
                else
                {
                    SalesFromDate = dtpSalesFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"); // dtpSalesFromDate.Value.ToString("yyyy-MMM-dd");
                }

                if (dtpSalesToDate.Checked == false)
                {
                    SalesToDate = "";
                }
                else
                {
                    SalesToDate = dtpSalesToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"); //  dtpSalesToDate.Value.ToString("yyyy-MMM-dd");
                }

                if (chkShiftAll.Checked)
                {
                    ShiftId = "0";
                }
                else
                {
                    ShiftId = cmbShift.SelectedValue.ToString();
                }
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // No DAL Method
            string result;

            try
            {
                #region Transaction Type

                DataGridViewRow selectedRow = null;

                selectedRow = FormSaleSearch.SelectOne(txtTransactionType.Text.Trim());

                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtSalesInvoiceNo.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                }

                //////string result = FormSaleSearch.SelectOne("Other");
                ////if (result == "")
                ////{
                ////    return;
                ////}
                ////else //if (result == ""){return;}else//if (result != "")
                ////{
                ////    string[] Saleinfo = result.Split(FieldDelimeter.ToCharArray());

                ////    txtSalesInvoiceNo.Text = Saleinfo[0];
                ////    txtCustomerName.Text = Saleinfo[2];
                ////}
            }

            #region Catch

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

            #endregion Catch
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            // No DAL Method
            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormProductSearch frm = new FormProductSearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                Program.fromOpen = "Me";
                Program.R_F = "";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString(); //
                    //txtPGroupId.Text = selectedRow.Cells["CategoryID"].Value.ToString();// ProductInfo[3];
                    //txtPGroup.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];
                }
            }

            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }

            #endregion Catch
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // No DAL Method
            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormCustomerSearch frm = new FormCustomerSearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormCustomerSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtCustomerId.Text = selectedRow.Cells["CustomerID"].Value.ToString(); //[0]
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                }

                #region Old

                //string result = FormCustomerSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] CustomerInfo = result.Split(FieldDelimeter.ToCharArray());
                //    txtCustomerName.Text = CustomerInfo[1];
                //    txtCustomerId.Text = CustomerInfo[0];
                //}

                #endregion
            }

            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button1_Click", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }

            #endregion Catch
        }

        private void FormRptSalesInformation_Load(object sender, EventArgs e)
        {
            FormMaker();
            dtpSalesFromDate.Value =
                dtpSalesFromDate.Value; // Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            dtpSalesFromDate.Checked = false;
            dtpSalesToDate.Value =
                dtpSalesToDate.Value; //Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            dtpSalesToDate.Checked = false;
            txtChassis = autocomplete(txtChassis);
            //txtChassis.AutoCompleteMode = AutoCompleteMode.Suggest;
            //txtChassis.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //AutoCompleteStringCollection DataCollectionChasis = new AutoCompleteStringCollection();
            //getDataChassis(DataCollectionChasis);
            //txtChassis.AutoCompleteCustomSource = DataCollectionChasis;

            cmbOrderBy.Text = "ProductCode";

            txtEngine.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtEngine.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection DataCollectionEngine = new AutoCompleteStringCollection();
            getDataEngine(DataCollectionEngine);
            txtEngine.AutoCompleteCustomSource = DataCollectionEngine;

            #region cmbShift

            CommonDAL cDal = new CommonDAL();
            cmbShift.DataSource = null;
            cmbShift.Items.Clear();
            Condition = new string[0];
            cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true, true,
                connVM);

            #endregion cmbShift

            #region Branch DropDown

            string[] Conditions = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName",
                Conditions, "varchar", true, true, connVM);


            if (SenderBranchId > 0)
            {
                cmbBranchName.SelectedValue = SenderBranchId;
            }
            else
            {
                cmbBranchName.SelectedValue = Program.BranchId;
            }

            #region cmbBranch DropDownWidth Change

            string cmbBranchDropDownWidth =
                new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);

            #endregion

            #endregion

            #region Channel DropDown

            string[] Conditionss = new string[] { "1=1" };
            cmbChannel = new CommonDAL().ComboBoxLoad(cmbChannel, "Channel", "Code", "Name", Conditionss, "varchar",
                true, true, connVM);


            //if (SenderBranchId > 0)
            //{
            //    cmbBranchName.SelectedValue = SenderBranchId;
            //}
            //else
            //{
            //    cmbBranchName.SelectedValue = Program.BranchId;

            //}

            #endregion

            #region Preview Only

            CommonDAL commonDal = new CommonDAL();

            string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);
            FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);


            if (PreviewOnly.ToLower() == "n")
            {
                cmbPost.Text = "Y";
                cmbPost.Visible = false;
                label10.Visible = false;
            }

            cmbDecimal.Text = FormNumeric;

            #endregion

            dtpSalesFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            dtpSalesToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));
        }

        private System.Windows.Forms.TextBox autocomplete(System.Windows.Forms.TextBox txtBox)
        {
            txtBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection DataCollectionChasis = new AutoCompleteStringCollection();
            getDataChassis(DataCollectionChasis);
            txtBox.AutoCompleteCustomSource = DataCollectionChasis;

            return txtBox;
        }

        private void getDataChassis(AutoCompleteStringCollection dataCollection)
        {
            ProductDAL dal = new ProductDAL();
            DataTable dtC = dal.SearchChassis();
            try
            {
                foreach (DataRow row in dtC.Rows)
                {
                    dataCollection.Add(row[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        private void getDataEngine(AutoCompleteStringCollection dataCollection)
        {
            ProductDAL dal = new ProductDAL();
            DataTable dtE = dal.SearchEngine();
            try
            {
                foreach (DataRow row in dtE.Rows)
                {
                    dataCollection.Add(row[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        private void FormMaker()
        {
            try
            {
                VATTypeLoad();

                #region Transaction Type

                this.Text = "Report (Sales) ";
                if (string.IsNullOrEmpty(txtTransactionType.Text.Trim().ToLower()))
                {
                    this.Text = "Sales Report (Local) ";
                }
                else if (txtTransactionType.Text.Trim().ToLower() == "other")
                {
                    this.Text = "Sales Report (Local) ";
                }
                else
                    this.Text = "Sales Report (" + txtTransactionType.Text.Trim() + ")";

                #endregion Transaction Type

                #region Bureau

                if (Program.IsBureau == true)
                {
                    lblDiscount.Visible = false;
                    lblPSale.Visible = false;
                    chkPromotional.Visible = false;
                    cmbDiscount.Visible = false;
                }

                //////this.MaximumSize = new System.Drawing.Size(500, 366);
                //////this.MinimumSize = new System.Drawing.Size(500, 320);
                //////this.panel1.Location = new System.Drawing.Point(0, 288);
                //////grbBankInformation.Size = new System.Drawing.Size(456, 290);
                if (rbtnCE.Checked)
                {
                    this.Text = " Report(Local Sale With Chassis & Engine)";
                    txtChassis.Visible = true;
                    txtEngine.Visible = true;
                    LC.Visible = true;
                    LE.Visible = true;
                    ////this.MaximumSize = new System.Drawing.Size(500, 380);
                    ////this.MinimumSize = new System.Drawing.Size(500, 380);
                    ////grbBankInformation.Size = new System.Drawing.Size(456, 1);
                    //////this.panel1.Location = new System.Drawing.Point(0, 310);
                }

                #endregion

                cmbReport.SelectedIndex = 0;
                PTypeSearch();
            }

            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormMaker", exMessage);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }

            #endregion Catch
        }

        private void PTypeSearch()
        {
            cmbProductType.Items.Clear();

            ProductDAL productTypeDal = new ProductDAL();
            cmbProductType.DataSource = productTypeDal.ProductTypeList;
            cmbProductType.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtPGroupId.Text = ProductCategoryInfo[0];
                    txtPGroup.Text = ProductCategoryInfo[1];
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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion Catch
        }

        //===============================================
        private void btnPreview_Click(object sender, EventArgs e)
        {
            OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
        }

        #region backgroundWorkerPreview

        private void backgroundWorkerPreviewSummary_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.SaleNew(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                    txtCustomerId.Text.Trim(),
                    txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType,
                    cmbPostText, DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), false, "", "0", branchId,
                    "", connVM);

                #endregion
            }

            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreviewSummary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                #region Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                ReportClass objrpt = new ReportClass(); // Start Complete

                ReportResult.Tables[0].TableName = "DsSale";
                if (chkPromotional.Checked)
                {
                    objrpt = new RptSalesSummeryPromotional();
                    Promotional = "Promotional " + this.Text + " summary";
                }
                else
                {
                    objrpt = new RptSalesSummery();
                    Promotional = "" + this.Text + " summary";
                }

                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + Promotional + "'";


                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Sales summary Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                #region ReportFormula

                if (txtProductName.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + txtProductName.Text.Trim() + "'  ";
                }


                if (txtSalesInvoiceNo.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text =
                        "'" + txtSalesInvoiceNo.Text.Trim() + "'  ";
                }

                if (dtpSalesFromDate.Checked == false)
                {
                    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                                                                            dtpSalesFromDate.Value.ToString(
                                                                                "dd/MMM/yyyy") +
                                                                            "'  ";
                }

                if (dtpSalesToDate.Checked == false)
                {
                    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                                                                          dtpSalesToDate.Value.ToString("dd/MMM/yyyy") +
                                                                          "'  ";
                }

                if (txtCustomerName.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PCustomer"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PCustomer"].Text = "'" + txtCustomerName.Text.Trim() + "'  ";
                }

                if (txtProductType.Text == "")
                {
                    objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + txtProductType.Text.Trim() + "'  ";
                }

                #endregion ReportFormula

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                #endregion
            }

            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                if (Program.IsBureau == true)
                {
                    ReportResult = reportDsdal.BureauSaleNew(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                        txtCustomerId.Text.Trim(),
                        txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                        DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), ShiftId, branchId, connVM);
                }
                else
                {
                    if (rbtnCE.Checked)
                    {
                        ReportResult = reportDsdal.SaleNewWithChassisEngine(txtSalesInvoiceNo.Text.Trim(),
                            SalesFromDate, SalesToDate, txtCustomerId.Text.Trim(),
                            txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                            DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), txtChassis.Text.Trim(),
                            txtEngine.Text.Trim(), ShiftId, branchId, connVM, OrderBy);
                    }
                    else
                    {
                        if (IsMISExcel)
                        {
                            ReportResult = reportDsdal.SaleMISExcel(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                            txtCustomerId.Text.Trim(),
                            txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                            DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), chkCategoryLike.Checked,
                            txtPGroup.Text.Trim(), ShiftId, branchId, VatType, connVM, OrderBy, Channel, Toll, "",
                            Report);

                        }
                        else
                        {
                        ReportResult = reportDsdal.SaleNew(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                            txtCustomerId.Text.Trim(),
                            txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                            DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), chkCategoryLike.Checked,
                            txtPGroup.Text.Trim(), ShiftId, branchId, VatType, connVM, OrderBy, Channel, Toll, "",
                            Report);

                        }
                        //var c =  ReportResult.Tables[0].Rows.Count;
                    }
                }

                #endregion
            }

            #endregion

            #region catch

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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",
                    exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreviewDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            DataTable settingsDt = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            try
            {
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }

                string CompanyCode = new CommonDAL().settingsDesktop("CompanyCode", "Code", settingsDt, connVM);

                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (branchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(branchId.ToString(), null, null, null, null, true, connVM);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }

                if (ExcelRpt)
                {
                    if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    if (CompanyCode.ToLower() == "sqr" && cmbReport.Text == "SummaryByProduct")
                    {
                        SaveExcelForSQR(ReportResult, "Summary By Product(Sales)");
                    }
                    else if (CompanyCode.ToLower() == "tk" && cmbReport.Text.ToLower() == "detail")
                    {
                        SaveExcelForTk(ReportResult, "SalesInformation)");
                    }
                    else
                    {
                        SaveExcel(ReportResult, "SalesInformation", BranchName, null, SalesFromDate, SalesToDate);
                    }
                }
                else
                {
                    string VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);

                    #region Complete

                    DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                    if (ReportResult.Tables.Count <= 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    ////ReportClass objrpt = new ReportClass(); // Start Complete
                    ReportDocument objrpt = new ReportDocument();
                    if (cmbReport.Text.Trim().ToLower() == "detail") //Detail
                    {
                        ReportResult.Tables[0].TableName = "DsSale";

                        if (chkPromotional.Checked)
                        {
                            //objrpt.Load(Program.ReportAppPath + @"\RptVAT11SCBL.rpt");
                            objrpt = new RptSalesTransactionPromotional();
                            Promotional = "Promotional " + this.Text + "";
                        }
                        else
                        {
                            if (rbtnCE.Checked)
                            {
                                objrpt = new RptSalesTransactionWithCE();
                                Promotional = "" + this.Text + "";
                            }
                            else
                            {
                                if (VAT11Name.ToLower() == "qel")
                                {
                                    objrpt = new RptQuazi_SalesTransaction();
                                }
                                else
                                {
                                    objrpt = new RptSalesTransaction();
                                }

                                Promotional = "" + this.Text + "";
                            }
                        }
                    }
                    else if (cmbReport.Text.Trim().ToLower() == "summery") //summary
                    {
                        ReportResult.Tables[0].TableName = "DsSale";
                        if (chkPromotional.Checked)
                        {
                            objrpt = new RptSalesSummeryPromotional();
                            Promotional = "Promotional " + this.Text + " summary";
                        }
                        else
                        {
                            objrpt = new RptSalesSummery();
                            Promotional = "" + this.Text + " summary";
                        }
                    }
                    else if (cmbReport.Text.Trim().ToLower() == "summaryqtyonly") //summary
                    {
                        ReportResult.Tables[0].TableName = "DsSale";
                        ReportResult.Tables[1].TableName = "DsSalePCategory";
                        ReportResult.Tables[2].TableName = "DsSaleCustomerPCategory";

                        //objrpt.Load(Program.ReportAppPath + @"\RptSalessummaryQuantityOnly.rpt");
                        objrpt = new RptSalesSummeryQuantityOnly();
                        Promotional = "" + this.Text + " summary";
                    }
                    else if (cmbReport.Text.Trim().ToLower() == "summarybyproduct") //Summery
                    {
                        ReportResult.Tables[0].TableName = "DsSale";
                        if (VAT11Name.ToLower() == "qel")
                        {
                            objrpt = new Rpt_Quazi_SalesSummeryByProduct();
                        }
                        else
                        {
                            objrpt = new RptSalesSummeryByProduct();
                        }

                        //if (VAT11Name.ToLower() == "scbl")
                        //{
                        //    objrpt = new RptScblSalesSummeryByProduct();

                        //}
                        //else
                        //{
                        //    objrpt = new RptSalesSummeryByProduct();
                        //}

                        //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptSalesSummeryByProduct.rpt");


                        Promotional = "" + this.Text + " summary";
                    }

                    #region Settings Load

                    CommonDAL _cDal = new CommonDAL();

                    DataTable sDt = new DataTable();

                    if (settingVM.SettingsDTUser == null)
                    {
                        sDt = new CommonDAL().SettingDataAll(null, null, connVM);
                    }

                    #endregion

                    //string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", sDt);


                    objrpt.SetDataSource(ReportResult);

                    #region Formula

                    string shiftName = chkShiftAll.Checked == true ? "All Shift" : cmbShift.Text.Trim().ToString();
                    Promotional = Promotional + ". Shift : " + shiftName;
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + Promotional + "' ";

                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    //objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";

                    if (rbtnCE.Checked)
                    {
                        objrpt.DataDefinition.FormulaFields["Chassis"].Text = "'[All]'";
                        objrpt.DataDefinition.FormulaFields["Engine"].Text = "'[All]'";

                        if (!string.IsNullOrEmpty(txtChassis.Text))
                            objrpt.DataDefinition.FormulaFields["Chassis"].Text = "'" + txtChassis.Text.Trim() + "'  ";

                        if (!string.IsNullOrEmpty(txtEngine.Text))
                            objrpt.DataDefinition.FormulaFields["Engine"].Text = "'" + txtEngine.Text.Trim() + "'  ";

                        if (txtProductName.Text == "")
                        {
                            objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                        }
                        else
                        {
                            objrpt.DataDefinition.FormulaFields["PProduct"].Text =
                                "'" + txtProductName.Text.Trim() + "'  ";
                        }
                    }

                    if (txtSalesInvoiceNo.Text == "")
                    {
                        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text =
                            "'" + txtSalesInvoiceNo.Text.Trim() + "'  ";
                    }

                    if (dtpSalesFromDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                            dtpSalesFromDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                            "'  ";
                    }

                    if (dtpSalesToDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtpSalesToDate.Value.ToString("dd/MMM/yyyy HH:mm") + "'  ";
                    }

                    if (txtCustomerName.Text == "")
                    {
                        objrpt.DataDefinition.FormulaFields["PCustomer"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PCustomer"].Text =
                            "'" + txtCustomerName.Text.Trim() + "'  ";
                    }

                    if (txtProductType.Text == "")
                    {
                        objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + txtProductType.Text.Trim() + "'  ";
                    }

                    #endregion Formula

                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);


                    FormReport reports = new FormReport();
                    objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >=
                        Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                        //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                    #endregion
                }
            }

            #endregion

            #region catch

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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",
                    exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        #endregion

        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReportMIS = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                if (Program.IsBureau == true)
                {
                    ReportMIS = reportDsdal.BureauSaleMis(txtSalesInvoiceNo.Text.Trim(), ShiftId, branchId, connVM);
                }
                else
                {
                    ReportMIS = reportDsdal.SaleMis(txtSalesInvoiceNo.Text.Trim(), ShiftId, branchId, VatType, connVM,
                        OrderBy);
                }

                //ReportMIS = reportDsdal.ReportSaleMis("CRN-0002/0113");

                // CRN-0002/0113

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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                ReportClass objrpt = new ReportClass(); // Start Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (branchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(branchId.ToString(), null, null, null, null, true, connVM);
                    //DataTable dtBranch = _branchDAL.SelectAll(branchId.ToString(), null, null, null, null, true);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }


                if (ReportMIS.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                ReportMIS.Tables[0].TableName = "DsSaleHeader";
                ReportMIS.Tables[1].TableName = "DsSalesDetails";
                ReportMIS.Tables[2].TableName = "DsSalesHeadersExport";
                objrpt = new RptMISSales1();
                objrpt.SetDataSource(ReportMIS);

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                //string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt);


                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}


                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                // objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";
                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());

            //        txtProductType.Text = TypeInfo[1];
            //    }
            //}
            //#region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button1_Click", exMessage);
            //}
            //#endregion Catch
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void cmbDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnCustomerGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Me";
                string result = FormCustomerGroupSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] CustomerGroupinfo = result.Split(FieldDelimeter.ToCharArray());
                    txtCustomerGroupID.Text = CustomerGroupinfo[0];
                    txtCustomerGroupName.Text = CustomerGroupinfo[1];
                }
            }

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnCustomerGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCustomerGroup_Click", exMessage);
            }

            #endregion
        }

        private void btnMonthly_Click(object sender, EventArgs e)
        {
            #region Try

            ExcelRpt = false;

            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                FormNumeric = cmbDecimal.Text.Trim();
                //Detail summary Single Monthly
                branchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                VatType = cmbType.Text;
                OrderBy = cmbOrderBy.Text.Trim();
                Report = cmbReport.Text.Trim();
                Channel = Convert.ToString(cmbChannel.SelectedValue);
                Toll = chbToll.Checked ? "Y" : "N";

                if (VatType == "All")
                {
                    VatType = "";
                }

                if (Channel == "0")
                {
                    Channel = "";
                }

                if (Program.CheckLicence(dtpSalesToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                NullCheck();
                //                Detail
                //summary
                //summary1
                //Single 
                //Monthly

                if (cmbReport.Text.Trim().ToLower() == "detail"
                    || cmbReport.Text.Trim().ToLower() == "summery"
                    || cmbReport.Text.Trim().ToLower() == "summarybyproduct"
                    || cmbReport.Text.Trim().ToLower() == "summaryqtyonly") //summary  1
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
                }


                else if (cmbReport.Text.Trim().ToLower() == "creditnotedetails" ||
                         cmbReport.Text.Trim().ToLower() == "debitnotedetails")
                {
                    bgwCreditNoteDetails.RunWorkerAsync();
                }
                else if (cmbReport.Text.Trim().ToLower() == "single") //Single   2
                {
                    if (txtSalesInvoiceNo.Text == "")
                    {
                        MessageBox.Show("Please select the invoice No", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }

                    backgroundWorkerMIS.RunWorkerAsync();
                }
                else if (cmbReport.Text.Trim().ToLower() == "monthly") //Monthly  3
                {
                    if (dtpSalesFromDate.Checked == false)
                    {
                        MessageBox.Show("Please select start date.", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }

                    if (dtpSalesToDate.Checked == false)
                    {
                        MessageBox.Show("Please select end date.", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(txtCustomerName.Text))
                    {
                        MessageBox.Show("Please select customer.", this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        this.progressBar1.Visible = false;
                        this.btnPreview.Enabled = true;
                        return;
                    }

                    backgroundWorkerMonth.RunWorkerAsync();
                }
            }

            #endregion

            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }

            #endregion Catch
        }

        private void backgroundWorkerMonth_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportDataTable = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                if (Program.IsBureau == true)
                {
                    ReportDataTable = reportDsdal.BureauMonthlySales(null, SalesFromDate, SalesToDate,
                        txtCustomerId.Text.Trim(),
                        txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType,
                        cmbPostText, DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), ShiftId, branchId,
                        connVM);
                }
                else
                {
                    ReportDataTable = reportDsdal.MonthlySales(null, SalesFromDate, SalesToDate,
                        txtCustomerId.Text.Trim(),
                        txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType,
                        cmbPostText, DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), ShiftId, branchId,
                        VatType, connVM);
                }

                #endregion
            }

            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",
                    exMessage);
            }

            #endregion
        }

        private void backgroundWorkerMonth_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (ReportDataTable.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                DataView dtview = new DataView(ReportDataTable);
                //dtview.Sort = "CustomerID ASC,YearSerial ASC, MonthSerial ASC";
                //dtview.Sort = "YearSerial ASC, MonthSerial ASC";
                //dtview.Sort = "CustomerID ASC";
                dtsorted = dtview.ToTable();

                #region assign column name of datagridview

                //a.Product_Name,a.Product_Code,sum(a.SaleQuantity) Quantity,SUM(a.SubTotal) Amount,SUM(a.VATAmount) VAT,a.MonthName,a.ItemNo,a.CustomerName,a.MonthSerial,a.YearSerial,a.CustomerID

                dgvSale = new DataGridView();
                //int ee = dgvSale.Rows.Count;
                dgvSale.Columns.Add("sl", "S.N");
                dgvSale.Columns.Add("Product_Name", dtsorted.Columns["Product_Name"].ColumnName.Replace('_', ' '));
                dgvSale.Columns.Add("Product_Code", dtsorted.Columns["Product_Code"].ColumnName.Replace('_', ' '));
                dgvSale.Columns.Add("UOM", dtsorted.Columns["UOM"].ColumnName.Replace('_', ' '));

                DataRow[] foundRows = dtsorted.Select("", "MonthSerial ASC");

                //DataTable dt = foundRows.CopyToDataTable();
                dtsorted = foundRows.CopyToDataTable();

                DataTable monthNames = dtsorted.DefaultView.ToTable(true, "MonthNames");

                for (int i = 0; i < monthNames.Rows.Count; i++)
                {
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Quantity",
                        monthNames.Rows[i][0].ToString() + "_" + "Quantity");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Amount",
                        monthNames.Rows[i][0].ToString() + "_" + "Amount");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "VAT",
                        monthNames.Rows[i][0].ToString() + "_" + "VAT");
                }

                string lastClmn = monthNames.Rows[0][0].ToString() + "-" +
                                  monthNames.Rows[monthNames.Rows.Count - 1][0].ToString() + " summary";
                dgvSale.Columns.Add("TotQty", lastClmn + "_Quantity");
                dgvSale.Columns.Add("Total_Amount", "Total_Amount");
                dgvSale.Columns.Add("Total_VAT", "Total_VAT");
                //// for multiple customer
                //dgvSale.Columns.Add("CustomerID", "CustomerID");

                #endregion

                #region assign value to grid columns

                int j = 0;
                decimal totalQty = 0;
                decimal totalAmount = 0;
                decimal totalVAT = 0;


                dgvSale.Rows.Clear();
                int aaa = dgvSale.Rows.Count;
                //foreach (DataRow item in dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name", "UOM").Rows)
                foreach (DataRow item in dtsorted.DefaultView
                             .ToTable(true, "Product_Code", "Product_Name", "UOM", "CustomerID").Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);
                    dgvSale["sl", j].Value = j + 1;
                    dgvSale["Product_Code", j].Value = item["Product_Code"].ToString();
                    dgvSale["Product_Name", j].Value = item["Product_Name"].ToString();
                    dgvSale["UOM", j].Value = item["UOM"].ToString();
                    for (int i = 0; i < monthNames.Rows.Count; i++)
                    {
                        dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "Amount", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "VAT", j].Value = 0;
                    }
                    //// for multiple customer
                    //dgvSale["CustomerID", j].Value = item["CustomerID"].ToString(); 

                    j++;
                }

                int a = dgvSale.Rows.Count;
                int aa = dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name").Rows.Count;
                for (int rowItem = 0; rowItem < dgvSale.Rows.Count - 1; rowItem++)
                {
                    totalQty = 0;
                    totalAmount = 0;
                    totalVAT = 0;

                    #region Multiple Customer

                    //DataRow[] detail =
                    //    dtsorted.Select("Product_Code='" + dgvSale["Product_Code", rowItem].Value.ToString() + "'");
                    //previousCustID = dgvSale.Rows[rowNo].Cells["CustomerID"].Value.ToString();

                    //DataRow[] detail =
                    //   dtsorted.Select("Product_Code='" + dgvSale["Product_Code", rowItem].Value.ToString() +
                    //   "' and CustomerID='" + dgvSale["CustomerID", rowItem].Value + "'");

                    #endregion Multiple Customer

                    DataRow[] detail =
                        dtsorted.Select("Product_Code='" + dgvSale["Product_Code", rowItem].Value.ToString() + "'");

                    foreach (var row1 in detail)
                    {
                        for (int i = 0; i < monthNames.Rows.Count; i++)
                        {
                            if (row1["MonthNames"].ToString() == monthNames.Rows[i][0].ToString())
                            {
                                dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", rowItem].Value =
                                    Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                totalQty += Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "Amount", rowItem].Value =
                                    Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                totalAmount += Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "VAT", rowItem].Value =
                                    Convert.ToDecimal(row1["VAT"].ToString().Trim());

                                totalVAT += Convert.ToDecimal(row1["VAT"].ToString().Trim());
                            }
                        }
                    }

                    dgvSale["TotQty", rowItem].Value = totalQty.ToString();
                    dgvSale["Total_Amount", rowItem].Value = totalAmount.ToString();
                    dgvSale["Total_VAT", rowItem].Value = totalVAT.ToString();
                }

                #endregion

                //dgvSale.Sort(dgvSale.Columns["CustomerID"],ListSortDirection.Ascending);
                //ExportToExcel();
                //OpenExcel();
            }

            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",
                    exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }
        //=================================================

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        //private void ExportToExcel()
        //{
        //    Excel.Range chartRange;
        //    try
        //    {
        //        xlApp = new Excel.Application();
        //        xlWorkBook = xlApp.Workbooks.Add(misValue);
        //        xlWorkSheet = (Excel.Worksheet) xlWorkBook.Worksheets.get_Item(1);

        //        #region Report Header

        //        string companyName = Program.CompanyName;
        //        string companyAddress = Program.Address1;
        //        string reportName = "Customer wise Monthly Sales Statement";
        //        string customerName = "Customer: " + dtsorted.Rows[0]["CustomerName"].ToString();
        //        string productName = "ProductName: All";
        //        if (!string.IsNullOrEmpty(txtProductName.Text))
        //            productName = "ProductName: " + txtProductName.Text;
        //        string dateFrom = dtpSalesFromDate.Value.ToString("MMMM") + "," +
        //                          dtpSalesFromDate.Value.Year.ToString();
        //        string dateTo = dtpSalesToDate.Value.ToString("MMMM") + "," +
        //                        dtpSalesToDate.Value.Year.ToString();

        //        #endregion Report Header

        //        #region Report Header Style

        //        chartRange = xlWorkSheet.get_Range("a1", "m1");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyName;

        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;
        //        chartRange.Font.Bold = true;
        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a2", "m2");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyAddress;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a4", "m4");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = reportName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 16;
        //        chartRange.Font.Bold = true;


        //        chartRange = xlWorkSheet.get_Range("a5", "m5");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = customerName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;
        //        chartRange.Font.Bold = true;

        //        chartRange = xlWorkSheet.get_Range("a7", "m7");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = productName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a8", "f8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date From: " + dateFrom;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("g8", "m8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date To: " + dateTo;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        #endregion Report Header

        //        string[] col = new string[]
        //                           {
        //                               "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
        //                               "q",
        //                               "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac",
        //                               "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap"
        //                           };

        //        #region Table Caption

        //        //// Excel column name for one customer

        //        for (int colItem = 0; colItem < dgvSale.Columns.Count; colItem++)
        //        {
        //            if (colItem < 4)
        //            {
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText;
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //            }
        //            else
        //            {
        //                if ((colItem - 1) % 3 == 0)
        //                {
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                    chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[0];
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem + 2] + "9");
        //                    chartRange.Merge(false);
        //                    chartRange.Font.Bold = true;
        //                    chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                    chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                    chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic);

        //                }
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "10", col[colItem] + "10");
        //                //chartRange.FormulaR1C1 = dgvSale.Columns[colItem].Name.Substring(3);
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[1];
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);

        //            }
        //        }

        //        string headerStrRange = col[0] + "9";
        //        string headerEndRange = col[dgvSale.Columns.Count - 1].ToString() + "10";
        //        chartRange = xlWorkSheet.get_Range(headerStrRange, headerEndRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);


        //        #endregion Table Caption

        //        #region Assign value
        //        // for one customer
        //        string data = null;
        //        int rowNo = 0;
        //        int colNo = 0;
        //        for (rowNo = 0; rowNo < dgvSale.Rows.Count - 1; rowNo++)
        //        {
        //            for (colNo = 0; colNo < dgvSale.Columns.Count; colNo++)
        //            {
        //                data = dgvSale.Rows[rowNo].Cells[colNo].Value.ToString();
        //                xlWorkSheet.Cells[10 + rowNo + 1, colNo + 1] = data;
        //                chartRange = xlWorkSheet.get_Range(col[colNo] + (10 + rowNo + 1).ToString(),
        //                                                   col[colNo] + (10 + rowNo + 1).ToString());
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);
        //            }
        //        }
        //        // table border
        //        string cellStartName = col[0] + "11";
        //        string cellEndName = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        chartRange = xlWorkSheet.get_Range(cellStartName, cellEndName);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //        #endregion Assign value

        //        #region Assign value for multiple customer
        //        ////for multiple customer
        //        //string data = null;
        //        //int rowNo = 0;
        //        //int colNo = 0;
        //        //// 
        //        //string previousCustID = "";
        //        //int currentExcelRow = 9;
        //        //int custStartRow = 11;
        //        //    for (rowNo = 0; rowNo < dgvSale.Rows.Count - 1; rowNo++)
        //        //    {
        //        //      if (previousCustID != dgvSale.Rows[rowNo].Cells["CustomerID"].Value.ToString())
        //        //        {
        //        //            if (rowNo!=0)
        //        //            {
        //        //                string gSRange1 = col[dgvSale.Columns.Count - 4].ToString() + custStartRow.ToString();
        //        //                string gEndRange1 = col[dgvSale.Columns.Count - 4].ToString() + currentExcelRow.ToString();
        //        //                chartRange = xlWorkSheet.get_Range(gSRange1, gEndRange1);

        //        //                //MessageBox.Show(gSRange1 + "_" + gEndRange1);
        //        //                //CalculateTotal(chartRange, col, custStartRow, currentExcelRow);
        //        //                custStartRow = currentExcelRow + 6;
        //        //            }

        //        //            currentExcelRow+=4;
        //        //            MakeHeader(chartRange, currentExcelRow, col);
        //        //            currentExcelRow++;
        //        //        }
        //        //        currentExcelRow++;


        //        //        for (colNo = 0; colNo < dgvSale.Columns.Count - 1; colNo++)
        //        //        {

        //        //            data = dgvSale.Rows[rowNo].Cells[colNo].Value.ToString();
        //        //            xlWorkSheet.Cells[currentExcelRow, colNo + 1] = data;
        //        //            chartRange = xlWorkSheet.get_Range(col[colNo] + currentExcelRow.ToString(),
        //        //                                               col[colNo] + currentExcelRow.ToString());
        //        //            //chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //        //            //                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //        //            //                        Excel.XlColorIndex.xlColorIndexAutomatic);
        //        //        }
        //        //        previousCustID = dgvSale.Rows[rowNo].Cells["CustomerID"].Value.ToString();
        //        //    }
        //             ////table border
        //            //string cellStartName = col[0] + "11";
        //            //string cellEndName = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //            //chartRange = xlWorkSheet.get_Range(cellStartName, cellEndName);
        //            //chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                    //Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                    //Excel.XlColorIndex.xlColorIndexAutomatic);


        //        #endregion Assign value

        //            #region Grand Total Calculation

        //        //grand total style
        //        string gSRange = col[dgvSale.Columns.Count - 6].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        string gEndRange = col[dgvSale.Columns.Count - 4].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(gSRange, gEndRange);
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = " Grand Total:";

        //        chartRange.Font.Bold = true;
        //        chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //        //calculate total Qty
        //        string totQtyRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totQtyRange, totQtyRange);
        //        string startRange = col[dgvSale.Columns.Count - 3].ToString() + "11";
        //        string endRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";
        //        chartRange.Font.Bold = true;

        //        //calculate total Amount
        //        string totAmtRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totAmtRange, totAmtRange);
        //        startRange = col[dgvSale.Columns.Count - 2].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";
        //        chartRange.Font.Bold = true;

        //        //calculate total VAT
        //        string totVatRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totVatRange, totVatRange);
        //        startRange = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";
        //        chartRange.Font.Bold = true;

        //        chartRange = xlWorkSheet.get_Range(gSRange, totVatRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //            #endregion

        //        #region Save location

        //        string printLocation = Directory.GetCurrentDirectory() + @"\MIS_Reports\Sale";
        //        saveLocation = printLocation + @"\" + "Monthly Sale" + "_" +
        //                       dateFrom + " - " + dateTo + "_ " + DateTime.Now.ToString("MMM,yy, HH.mm") + ".xls";

        //        if (!Directory.Exists(printLocation))
        //            Directory.CreateDirectory(printLocation);

        //        xlWorkBook.SaveAs(saveLocation, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue,
        //                          misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue,
        //                          misValue);
        //        xlApp.Visible = true;

        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        xlWorkBook.Close(true, misValue, misValue);

        //        xlApp.Quit();

        //        releaseObject(xlWorkSheet);
        //        releaseObject(xlWorkBook);
        //        releaseObject(xlApp);


        //    }
        //    finally
        //    {

        //    }

        //    #region Convert list to datatable

        //    //PropertyDescriptorCollection props =
        //    //TypeDescriptor.GetProperties(typeof(Sale_Monthly));
        //    //            System.Data.DataTable table = new System.Data.DataTable();
        //    //            for (int ii = 0; ii < props.Count; ii++)
        //    //            {
        //    //                PropertyDescriptor prop = props[ii];
        //    //                table.Columns.Add(prop.Name, prop.PropertyType);
        //    //            }
        //    //            object[] values = new object[props.Count];
        //    //            foreach (Sale_Monthly item in monthlySales)
        //    //            {
        //    //                for (int k = 0; k < values.Length; k++)
        //    //                {
        //    //                    values[k] = props[k].GetValue(item);
        //    //                }
        //    //                table.Rows.Add(values);
        //    //            }

        //    #endregion Convert list to datatable

        //}

        private void OpenExcel()
        {
            xlApp = null;
            xlWorkBook = null;
            try
            {
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.Visible = true;
                xlWorkBook = xlApp.Workbooks.Open(saveLocation,
                    misValue, misValue, misValue, misValue,
                    misValue, misValue, misValue, misValue,
                    misValue, misValue, misValue, misValue,
                    misValue, misValue);
            }
            catch (Exception ex)
            {
                xlWorkBook.Close(false, misValue, misValue);

                throw;
            }
            finally
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlWorkBook);

                //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlApp);
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void MakeHeader(Microsoft.Office.Interop.Excel.Range chartRange, int rowNo, string[] col)
        {
            #region Table Caption

            // Excel column name

            for (int colItem = 0; colItem < dgvSale.Columns.Count - 1; colItem++)
            {
                if (colItem < 4)
                {
                    chartRange = xlWorkSheet.get_Range(col[colItem] + (rowNo).ToString(),
                        col[colItem] + (rowNo).ToString());
                    chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText;
                    chartRange.Font.Bold = true;
                    chartRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    chartRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    chartRange = xlWorkSheet.get_Range(col[colItem] + (rowNo).ToString(),
                        col[colItem] + (rowNo + 1).ToString());
                    chartRange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
                        Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium,
                        Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic,
                        Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);
                }
                else
                {
                    if ((colItem - 1) % 3 == 0)
                    {
                        chartRange = xlWorkSheet.get_Range(col[colItem] + (rowNo).ToString(),
                            col[colItem] + (rowNo).ToString());
                        chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[0];
                        chartRange = xlWorkSheet.get_Range(col[colItem] + (rowNo).ToString(),
                            col[colItem + 2] + (rowNo).ToString());
                        //chartRange.Merge(false);
                        chartRange.Font.Bold = true;
                        chartRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                        chartRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        chartRange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
                            Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium,
                            Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic,
                            Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);
                    }

                    chartRange = xlWorkSheet.get_Range(col[colItem] + (1 + rowNo).ToString(),
                        col[colItem] + (1 + rowNo).ToString());
                    //chartRange.FormulaR1C1 = dgvSale.Columns[colItem].Name.Substring(3);
                    chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[1];
                    chartRange.Font.Bold = true;
                    chartRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    chartRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    chartRange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
                        Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium,
                        Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic,
                        Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);
                }
            }

            //string headerStrRange = col[0] + (9 + rowNo).ToString();
            //string headerEndRange = col[dgvSale.Columns.Count - 1].ToString() + (10 + rowNo).ToString();
            //chartRange = xlWorkSheet.get_Range(headerStrRange, headerEndRange);
            //chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
            //                        Excel.XlColorIndex.xlColorIndexAutomatic,
            //                        Excel.XlColorIndex.xlColorIndexAutomatic);

            #endregion Table Caption
        }

        private void CalculateTotal(Microsoft.Office.Interop.Excel.Range chartRange, string[] col, int startRow,
            int endRow)
        {
            try
            {
                #region Grand Total Calculation

                //grand total style
                //string gSRange = col[dgvSale.Columns.Count - 6].ToString() + (dgvSale.Rows.Count + 10).ToString();
                //string gEndRange = col[dgvSale.Columns.Count - 4].ToString() + (dgvSale.Rows.Count + 10).ToString();
                string gSRange = col[dgvSale.Columns.Count - 6].ToString() + startRow.ToString();
                string gEndRange = col[dgvSale.Columns.Count - 4].ToString() + endRow.ToString();
                chartRange = xlWorkSheet.get_Range(gSRange, gEndRange);
                //chartRange.Merge(false);
                chartRange.FormulaR1C1 = " Grand Total:";

                chartRange.Font.Bold = true;
                chartRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                chartRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //calculate total Qty
                string totQtyRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count + 10).ToString();
                chartRange = xlWorkSheet.get_Range(totQtyRange, totQtyRange);
                string startRange = col[dgvSale.Columns.Count - 3].ToString() + "11";
                string endRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
                chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
                chartRange.NumberFormat = "000,00";

                //calculate total Amount
                string totAmtRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count + 10).ToString();
                chartRange = xlWorkSheet.get_Range(totAmtRange, totAmtRange);
                startRange = col[dgvSale.Columns.Count - 2].ToString() + "11";
                endRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
                chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
                chartRange.NumberFormat = "000,00";

                //calculate total VAT
                string totVatRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count + 10).ToString();
                chartRange = xlWorkSheet.get_Range(totVatRange, totVatRange);
                startRange = col[dgvSale.Columns.Count - 1].ToString() + "11";
                endRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
                chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
                chartRange.NumberFormat = "000,00";

                chartRange = xlWorkSheet.get_Range(gSRange, totVatRange);
                chartRange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
                    Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium,
                    Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic,
                    Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtProductType.Text = cmbProductType.Text;
        }

        private void txtChassis_TextChanged(object sender, EventArgs e)
        {
            //TextBox t = sender as TextBox;
            //if (t != null)
            //{
            //    //say you want to do a search when user types 3 or more chars
            //    if (t.Text.Length >= 3)
            //    {
            //        //SuggestStrings will have the logic to return array of strings either from cache/db
            //        string[] arr =  t.Text ;

            //        AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            //        collection.AddRange(arr);

            //        this.txtChassis.AutoCompleteCustomSource = collection;
            //    }
            //}
        }

        private void chkCategoryLike_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void chkCategoryLike_Click(object sender, EventArgs e)
        {
            txtPGroup.ReadOnly = true;

            if (chkCategoryLike.Checked)
            {
                txtPGroup.ReadOnly = false;
            }
        }

        private void cmbShift_Leave(object sender, EventArgs e)
        {
        }

        private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void grbBankInformation_Enter(object sender, EventArgs e)
        {
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            #region Try

            ExcelRpt = true;
            try
            {
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }
                string CompanyCode = new CommonDAL().settingsDesktop("CompanyCode", "Code", settingsDt, connVM);

                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //Detail summary Single Monthly
                branchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                VatType = cmbType.Text;
                OrderBy = cmbOrderBy.Text.Trim();
                Channel = Convert.ToString(cmbChannel.SelectedValue);
                cmbReportType = cmbReport.Text.Trim();
                if (VatType == "All")
                {
                    VatType = "";
                }

                if (Channel == "0")
                {
                    Channel = "";
                }

                if (Program.CheckLicence(dtpSalesToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                NullCheck();
                //                Detail
                //summary
                //summary1
                //Single 
                //Monthly
                if (CompanyCode.ToLower() != "sqr" && CompanyCode.ToLower() != "dhl")
                {
                    if (cmbReport.Text.Trim().ToLower() == "detail"
                        || cmbReport.Text.Trim().ToLower() == "summery"
                        || cmbReport.Text.Trim().ToLower() == "summarybyproduct"
                        || cmbReport.Text.Trim().ToLower() == "summaryqtyonly") //summary  1
                    {
                        IsMISExcel = true;

                        backgroundWorkerPreviewDetails.RunWorkerAsync();
                    }

                    else if (cmbReport.Text.Trim().ToLower() == "channel-wise" || string.Equals(cmbReport.Text.Trim(), "Channel-Wise-Summary", StringComparison.OrdinalIgnoreCase))
                    {
                        bgwChannelReport.RunWorkerAsync(cmbReport.Text.Trim().ToLower() == "channel-wise");
                    }
                    else if (cmbReport.Text.Trim().ToLower() == "Invoice-Wise".ToLower() || cmbReport.Text.Trim().ToLower() == "Date-Wise".ToLower())
                    {
                        bgwInvoiceWiseMIS.RunWorkerAsync();
                    }
                    else if (cmbReport.Text.Trim().ToLower() == "Branch-Wise-Summary".ToLower())
                    {

                        if (string.IsNullOrWhiteSpace(SalesFromDate))
                        {
                            MessageBox.Show("Please select the From Date", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.progressBar1.Visible = false;
                            this.btnDownload.Enabled = true;
                            this.btnPreview.Enabled = true;

                            return;
                        }
                        if (string.IsNullOrWhiteSpace(SalesToDate))
                        {
                            MessageBox.Show("Please select the To Date", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.progressBar1.Visible = false;
                            this.btnDownload.Enabled = true;
                            this.btnPreview.Enabled = true;

                            return;
                        }

                        bgwBranchWiseSummary.RunWorkerAsync();
                    }
                }
                else if (CompanyCode.ToLower() == "sqr")
                {
                    if (cmbReport.Text.Trim().ToLower() == "detail"
                        || cmbReport.Text.Trim().ToLower() == "summery"
                        || cmbReport.Text.Trim().ToLower() == "summaryqtyonly") //summary  1
                    {
                        backgroundWorkerPreviewDetails.RunWorkerAsync();
                    }

                    if (cmbReport.Text.Trim().ToLower() == "summarybyproduct")
                    {
                        bgwSqrSummary.RunWorkerAsync();
                    }
                }
                //////else if (cmbReport.Text.Trim().ToLower() == "channel-wise")
                //////{
                //////    bgwChannelReport.RunWorkerAsync();
                //////}
                else if (cmbReport.Text.Trim().ToLower() == "datasource")
                {
                    bgwDataSource.RunWorkerAsync();
                }

            }

            #endregion

            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnDownload_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDownload_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnDownload_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnDownload_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnDownload_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDownload_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDownload_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnDownload_Click", exMessage);
            }

            #endregion Catch
        }

        private void SaveExcelForSQR(DataSet ds, string ReportType = "", string BranchName = "")
        {
            DataTable dt = new DataTable();

            DataTable dtresult = ds.Tables["Table"];
            if (dtresult.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var newDt = dtresult.AsEnumerable()
                .GroupBy(r => new
                {
                    CategoryName = r["CategoryName"],
                    ProductName = r["ProductName"],
                    ProductCode = r["ProductCode"]
                })
                .Select(g =>
                {
                    var row = dtresult.NewRow();

                    row["ProductName"] = g.Key.ProductName;
                    row["CategoryName"] = g.Key.CategoryName;
                    row["ProductCode"] = g.Key.ProductCode;
                    row["Quantity"] = g.Sum(r => r.Field<decimal>("Quantity"));
                    row["SubTotal"] = g.Sum(r => r.Field<decimal>("SubTotal"));
                    row["VATAmount"] = g.Sum(r => r.Field<decimal>("VATAmount"));
                    row["SDAmount"] = g.Sum(r => r.Field<decimal>("SDAmount"));
                    row["TradingMarkUp"] = g.Sum(r => r.Field<decimal>("TradingMarkUp"));

                    return row;
                }).CopyToDataTable();
            var dataView = new DataView(newDt);
            dataView.Sort = "CategoryName asc";
            dt = dataView.ToTable(true, "CategoryName", "ProductCode", "ProductName", "UOM", "Quantity",
                "SubTotal", "TradingMarkUp", "SDAmount", "VATAmount");

            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("", connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();


            string[] ReportHeaders = new string[]
            {
                " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, ReportType,
                " Period: " + SalesFromDate + " TO: " + SalesToDate
            };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }


            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
                }

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void SaveExcel(DataSet ds, string ReportType = "", string BranchName = "", string CompanyCode = null, string SalesFromDate = "", string SalesToDate = "")
        {
            ////DataTable dt = new DataTable();

            //////DataTable dtresult = ds.Tables["Table"];
            DataTable dt = ds.Tables["Table"];

            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            //var dataView = new DataView(dtresult);
            //dt = dataView.ToTable(true, "SalesInvoiceNo", "LCNumber", "LCDate", "InvoiceDateTime", "CustomerName",
            //    "Address1", "VATRegistrationNo",
            //    "ProductCode", "ProductName", "Quantity", "UOM", "NBRPrice", "SubTotal", "VATRate", "VATAmount", "SD", "SDAmount" );
            
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("");

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();

            string branchName = "";

            if (!string.IsNullOrWhiteSpace(BranchName))
            {
                branchName = " Branch Name : " + BranchName;
            }

            string ParamFromDate;
            string ParamToDate;

            if (!string.IsNullOrWhiteSpace(SalesFromDate))
            {
                ParamFromDate = Convert.ToDateTime(SalesFromDate).ToString("dd-MMM-yyyy");
            }
            else
            {
                ParamFromDate = "All";
            }

            if (!string.IsNullOrWhiteSpace(SalesToDate))
            {
                ParamToDate = Convert.ToDateTime(SalesToDate).ToString("dd-MMM-yyyy");
            }
            else
            {
                ParamToDate = "All";
            }

            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, "Form Date:" + ParamFromDate  + "    To Date:" + ParamToDate, branchName };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }

            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            //if (string.IsNullOrEmpty(sheetName))
            //{
            //    sheetName = ReportType;
            //}

            string sheetName = ReportType;


            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
                }

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void SaveExcelForTk(DataSet ds, string ReportType = "", string BranchName = "",
            string CompanyCode = null)
        {
            DataTable dt = new DataTable();

            DataTable dtresult = ds.Tables["Table"];
            if (dtresult.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var dataView = new DataView(dtresult);
            dt = dataView.ToTable(true, "LCNumber", "LCDate", "InvoiceDateTime", "CustomerName", "Address1",
                "VATRegistrationNo", "VehicleNo",
                "ProductName", "Quantity", "UOM", "NBRPrice", "SubTotal", "VATRate", "VATAmount", "Total");
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("");

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();


            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }


            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "PurchaseInformation";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
                }

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void bgwCreditNoteDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                if (Report.ToLower() == "creditnotedetails")
                {
                    vTransactionType = vTransactionType == "" ? "Credit" : vTransactionType;
                }
                else
                {
                    vTransactionType = vTransactionType == "" ? "Debit" : vTransactionType;
                }


                ReportResult = reportDsdal.VATCreditNoteMis(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                    txtCustomerId.Text.Trim(),
                    txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                    DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), chkCategoryLike.Checked,
                    txtPGroup.Text.Trim(), ShiftId, branchId, VatType, connVM, OrderBy);

                #endregion
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
                FileLogger.Log(this.Name, "", exMessage);
            }

            #endregion
        }

        private void bgwCreditNoteDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                ReportClass objrpt = new ReportClass(); // Start Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (branchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(branchId.ToString(), null, null, null, null, true);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula

                if (ReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                ReportResult.Tables[0].TableName = "DsVAT11";
                if (Report.ToLower() == "creditnotedetails")
                {
                    objrpt = new RptVATCreditNoteInformation();
                }
                else
                {
                    objrpt = new RptVATDebitNoteInformation();
                }

                objrpt.SetDataSource(ReportResult);

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);


                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                if (Report.ToLower() == "creditnotedetails")
                {
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Credit Note Information'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Debit Note Information'";
                }

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["Quantity6_3"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["Amount6_3"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete

                #endregion
            }

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted",
                    ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void dtpSalesFromDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtpSalesFromDate.Checked == true)
            //{
            //    dtpSalesFromDate.Value = Convert.ToDateTime(dtpSalesFromDate.Value.ToString("yyyy-MMM-dd 00:00:00"));
            //}
        }

        private void dtpSalesToDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtpSalesToDate.Checked == true)
            //{
            //    dtpSalesToDate.Value = Convert.ToDateTime(dtpSalesToDate.Value.ToString("yyyy-MMM-dd 23:59:59"));
            //}
        }

        private void btnDownload1_Click(object sender, EventArgs e)
        {
            #region Try

            ExcelRpt = true;
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //Detail summary Single Monthly
                branchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                VatType = cmbType.Text;
                OrderBy = cmbOrderBy.Text.Trim();

                if (VatType == "All")
                {
                    VatType = "";
                }

                if (Program.CheckLicence(dtpSalesToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                NullCheck();
                //                Detail
                //summary
                //summary1
                //Single 
                //Monthly

                if (cmbReport.Text.Trim().ToLower() == "detail"
                    || cmbReport.Text.Trim().ToLower() == "summery"
                    || cmbReport.Text.Trim().ToLower() == "summarybyproduct"
                    || cmbReport.Text.Trim().ToLower() == "summaryqtyonly")
                {
                    backgroundWorkerPreviewDetails.RunWorkerAsync();
                }
            }

            #endregion

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
                FileLogger.Log(this.Name, "btnDownload1_Click", exMessage);
            }

            #endregion Catch
        }

        private void bgwChannelReport_DoWork(object sender, DoWorkEventArgs e)
        {
            SaleDAL saleDal = new SaleDAL();

            bool isDetail = (bool)e.Argument;
            DataSet result = new DataSet();

            if (isDetail)
            {
                result = saleDal.GetChannelData(SalesFromDate, SalesToDate, branchId.ToString(), Channel, null,
                    null, connVM);
            }
            else
            {
                result = saleDal.GetChannelDataSummary(SalesFromDate, SalesToDate, branchId.ToString(), Channel, null,
                    null, connVM);

            }


            e.Result = result;
        }

        private void bgwChannelReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else
                {
                    DataSet result = (DataSet)e.Result;
                    OrdinaryVATDesktop.SaveExcelMultiple(result, "Channel-Wise Sale Report",
                        new[] { "Sale = (Sale-Credit)", "Sale", "Credit Notes" });
                }
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwDataSource_DoWork(object sender, DoWorkEventArgs e)
        {
            SaleDAL saleDal = new SaleDAL();
            DataSet finalDS = new DataSet();

            DataTable result = saleDal.GetDHLSaleData(SalesFromDate, SalesToDate, "0", "other", "IBS");
            finalDS.Tables.Add(result.Copy());

            result = saleDal.GetDHLSaleData(SalesFromDate, SalesToDate, "0", "credit", "IBS");
            finalDS.Tables.Add(result.Copy());

            result = saleDal.GetDHLSaleData(SalesFromDate, SalesToDate, "0", "debit", "IBS");
            finalDS.Tables.Add(result.Copy());


            result = saleDal.GetDHLSaleData(SalesFromDate, SalesToDate, "2", "other", "");
            finalDS.Tables.Add(result.Copy());

            result = saleDal.GetDHLSaleData(SalesFromDate, SalesToDate, "0", "other", "IAS");
            finalDS.Tables.Add(result.Copy());

            e.Result = finalDS;
        }

        private void bgwDataSource_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else
                {
                    DataSet result = (DataSet)e.Result;
                    OrdinaryVATDesktop.SaveExcelMultiple(result, "Sale Monthly Report",
                        new[] { "IBS", "IBS(Credit)", "IBS(Debit)", "Airport", "IAS" });
                }
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwSqrSummary_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start


                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();


                ReportResult = reportDsdal.SaleSummaryByProduct(txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate,
                            txtCustomerId.Text.Trim(),
                            txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                            DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), chkCategoryLike.Checked,
                            txtPGroup.Text.Trim(), ShiftId, branchId, VatType, connVM, OrderBy, Channel, Toll, "",
                            Report);

                //var c =  ReportResult.Tables[0].Rows.Count;

                #endregion
            }

            #endregion

            #region catch






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
                FileLogger.Log(this.Name, "bgwSqrSummary_DoWork",
                    exMessage);
            }

            #endregion
        }

        private void bgwSqrSummary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveExcelSQR(ReportResult, "Summary By Product(Sales)");

            this.progressBar1.Visible = false;
            this.btnPreview.Enabled = true;

        }
        private void SaveExcelSQR(DataSet ds, string ReportType = "", string BranchName = "")
        {
            DataTable dt = new DataTable();

            DataTable dtresult = ds.Tables["Table"];
            if (dtresult.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            var dataView = new DataView(dtresult);
            dataView.Sort = "CategoryName asc";
            dt = dataView.ToTable(true, "CategoryName", "ProductCode", "ProductName", "UOM", "Quantity",
                "SubTotal", "SDAmount", "VATAmount");

            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("", connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();


            string[] ReportHeaders = new string[]
            {
                " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, ReportType,
                " Period: " + SalesFromDate + " TO: " + SalesToDate
            };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }


            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
                }

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void bgwInvoiceWiseMIS_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                string CustomerId = txtCustomerId.Text.Trim();
                string ItemNo = txtItemNo.Text.Trim();
                string PGroupId = txtPGroupId.Text.Trim();
                string Type = cmbTypeText;
                string Post = cmbPostText;
                string CustomerGroupID = txtCustomerGroupID.Text.Trim();
                string Vat_Type = VatType;

                SalesFromDate = Convert.ToDateTime(SalesFromDate).ToString("yyyy-MM-dd HH:mm:ss");
                SalesToDate = Convert.ToDateTime(SalesToDate).ToString("yyyy-MM-dd HH:mm:ss");

                ReportResultDt = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                string[] conditionFields = new[] { "sid.SalesInvoiceNo", "sid.InvoiceDateTime>=", "sid.InvoiceDateTime<=", "cg.CustomerGroupID", "cus.CustomerID", "sid.ItemNo", "pc.IsRaw", "sih.Post", "sid.Type", "pc.CategoryID", "sid.BranchId" };

                string[] conditionValues = new[] { txtSalesInvoiceNo.Text.Trim(), SalesFromDate, SalesToDate, CustomerGroupID, CustomerId, ItemNo, Type, Post, Vat_Type, PGroupId, branchId.ToString() };

                if (cmbReportType.ToLower() == "Date-Wise".ToLower())
                {
                    ReportResultDt = reportDsdal.ExportDateWiseSalesData(conditionFields, conditionValues, null, null, connVM);
                }
                else
                {
                    ReportResultDt = reportDsdal.ExportInvoiceWiseSalesData(conditionFields, conditionValues, null, null, connVM);
                }

                #endregion
            }

            #endregion

            #region catch

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
                FileLogger.Log(this.Name, "bgwInvoiceWiseMIS_DoWork", exMessage);
            }

            #endregion
        }

        private void bgwInvoiceWiseMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            DataTable settingsDt = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            try
            {
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }

                string CompanyCode = new CommonDAL().settingsDesktop("CompanyCode", "Code", settingsDt, connVM);

                if (ExcelRpt)
                {
                    if (ReportResultDt == null || ReportResultDt.Rows.Count == 0)
                    {
                        MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    DownloadExcel(ReportResultDt, "SalesInformation", "SalesInformation");

                }

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +
                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwInvoiceWiseMIS_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void DownloadExcel(DataTable dt, string ReportName = "", string sheetName = "", string CompanyCode = null)
        {

            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal =
                OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("");

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();

            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k],
                    OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }


            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };

                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
                }

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total

                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                     ws.Cells[TableHeadRow + 1, colNumber].Address +
                                                                     ":" + ws.Cells[(TableHeadRow + RowCount),
                                                                         colNumber].Address + ")";

                        #endregion
                    }
                }

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] +
                    (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[
                    "A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] +
                    (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);

        }

        private void bgwBranchWiseSummary_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.BranchWiseSummaryData(SalesFromDate, SalesToDate);

                #endregion
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwBranchWiseSummary_DoWork", exMessage);
            }

            #endregion
        }

        private void bgwBranchWiseSummary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region Try

            DataTable settingsDt = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            try
            {
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }

                string CompanyCode = new CommonDAL().settingsDesktop("CompanyCode", "Code", settingsDt, connVM);

                if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string fromDate = Convert.ToDateTime(SalesFromDate).ToString("dd-MMM-yyyy");
                string ToDate = Convert.ToDateTime(SalesToDate).ToString("dd-MMM-yyyy");

                var sheetNames = new[] { "Branch Wise", "Product Wise" };

                var reportHeaders = new[] { "Sales Information", "From Date : " + fromDate, "To Date : " + ToDate };

                OrdinaryVATDesktop.SaveExcelMultiple(ReportResult, "Sales Summary", sheetNames, reportHeaders);

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwBranchWiseSummary_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
                this.btnDownload.Enabled = true;
            }

        }



    }
}