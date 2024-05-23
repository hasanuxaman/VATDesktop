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
using System.Security.Cryptography;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Utilities;
//
using VATClient.ReportPages;
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Threading;
//
using VATViewModel.DTOs;
using VATServer.Library;
using System.Data.OleDb;
using VATServer.License;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormTraking : Form
    {
        #region Constructors

        public FormTraking()
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
        private DepositMasterVM depositMaster;
        private AdjustmentHistoryVM adjustmentHistory;
        private List<VDSMasterVM> vdmMaster;

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DataTable VDSResult;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private DataSet ReportResult;
        private DataSet ReportSubResult;
        private string transId = string.Empty;
        string transactionType = string.Empty;
        public string InEnglish = string.Empty;
        List<AdjustmentVM> adjNames = new List<AdjustmentVM>();
        CommonDAL commonDal = new CommonDAL();
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private int searchBranchId = 0;
        private bool ChangeData = false;
        string NextID = string.Empty;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string company;
        private string DepositData;
        private string VDSData;
        private string BankInformationData;
        private string[] BankInformationLines;
        private string result;
        private string[] DepositResultFields;
        private DataTable BankInformationResult;
        private DataTable AdjTypeResult;
        private decimal VDSPercentRate = 0;
        private bool Add = false;
        private bool Edit = false;
        private string CategoryId { get; set; }
        #endregion


        #region Methods 01
        
        //public string VFIN = "151";
     


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picBankID_Click(object sender, EventArgs e)
        {

        }

        private void picDepositID_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 02

        #region Textbox KeyDown Event

        private void txtDepositId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTreasuryNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        //private void txtDepositType_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        //}

        private void txtDepositAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDepositDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpChequeDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeBank_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtChequeBankBranch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

     

        private void txtBranchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTreasuryCopy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

       
      
        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion

        

        private void txtDepositAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

       

        private void cmbBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BankDetailsInfo();
            ChangeData = true;
        }

       
      

      

        

      
      

     

        private void txtTreasuryNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        //private void txtDepositType_TextChanged(object sender, EventArgs e)
        //{
        //    ChangeData = true;
        //}

        private void dtpDepositDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtBranchName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 03

        private void txtAccountNumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTreasuryCopy_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDepositPersonDesignation_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDepositPerson_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeBankBranch_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeBank_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpChequeDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtChequeNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

      



        #endregion

        private void FormTraking_Load(object sender, EventArgs e)
        {

        }

        private void txtCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                customerLoad();
            }
        }
        private void customerLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select cg.CustomerGroupName,c.CustomerCode,c.CustomerName,c.ShortName,c.Address1,c.CustomerID, IsExamted,IsSpecialRate
                            from Customers c 
                            left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID 
                            where 1=1 and c.ActiveStatus='Y' 
";
                string ShowAllCustomer = commonDal.settingsDesktop("Setup", "ShowAllCustomer", null, connVM);
                if (ShowAllCustomer.ToLower() == "n")
                {
                    SqlText += @" and c.BranchId='" + Program.BranchId + "'";
                }
                string SQLTextRecordCount = @" select count(CustomerCode)RecordNo from Customers  
                            ";


                string[] shortColumnName = { "cg.CustomerGroupName", "c.CustomerCode", "c.CustomerName", "c.ShortName", "c.Address1" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    //vCustomerID = "0";
                    //txtCustomer.Text = "";
                    //txtDeliveryAddress1.Text = "";
                    txtCustomerId.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                    //txtDeliveryAddress1.Text = selectedRow.Cells["Address1"].Value.ToString();//ProductInfo[0];
                    //IsCustomerExempted = selectedRow.Cells["IsExamted"].Value.ToString();//ProductInfo[0];
                    //IsCustomerSpecialRate = selectedRow.Cells["IsSpecialRate"].Value.ToString();//ProductInfo[0];

                    txtCustomer.Focus();
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

        private void cmbProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                //ProductLoad();
            }
        }
        private void ProductLoad(bool IsSales)
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";
                if (!string.IsNullOrEmpty(CategoryId))
                {
                    SqlText += @"  and pc.CategoryID='" + CategoryId + "'  ";
                }
                //else if (!string.IsNullOrEmpty(IsRawParam))
                //{
                //    SqlText += @"  and pc.IsRaw='" + IsRawParam + "'  ";
                //}

                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products ";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    if (IsSales)
                    {
                        //cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();
                        txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                        txtProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();

                    }
                    else
                    {
                        txtItemNoP.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                        txtProductNameP.Text = selectedRow.Cells["ProductName"].Value.ToString();

                    }
                   
                }
                //cmbProductName.Focus();
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dal = new SaleDAL();
            try
            {
                progressBar1.Visible = true;
                btnExport.Enabled = false;
              
              string InvoicrFrom  = dtpInvoiceFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
              string InvoiceTo = dtpInvoiceToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");

              string customer = txtCustomer.Text;
              //string ProductName = cmbProductName.Text;
              string customerId = txtCustomerId.Text;
              string ItemNo = txtItemNo.Text;


              DataTable data = dal.GetSaleTrakingExcelData(customerId, ItemNo, InvoicrFrom, InvoiceTo);

              if (data.Rows.Count == 0)
              {
                  data.Rows.Add(data.NewRow());
              }

              OrdinaryVATDesktop.SaveExcel(data, "SaleTraking", "SaleTrakingM");
              MessageBox.Show("Successfully Exported data in Excel files of root directory");

                // get value

                // send value

                // check null values

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }
            finally
            {
                progressBar1.Visible = false;
                btnExport.Enabled = true;
            }

        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                //ProductLoad();
            }
        }

        private void txtVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorLoad();
            }
        }
        private void VendorLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select v.VendorID, v.VendorCode,v.VendorName,v.ShortName,vg.VendorGroupName,v.VATRegistrationNo,v.TINNo
                                    from Vendors v
                                    left outer join VendorGroups vg on v.VendorGroupID=vg.VendorGroupID
                                    where 1=1 and  v.ActiveStatus='Y'   ";
                string ShowAllVendor = new CommonDAL().settingsDesktop("Setup", "ShowAllVendor", null, connVM);
                if (ShowAllVendor.ToLower() == "n")
                {
                    SqlText += @" and v.BranchId='" + Program.BranchId + "'";
                }
                string SQLTextRecordCount = @" select count(VendorCode)RecordNo from Vendors";

                string[] shortColumnName = { "v.VendorID", "v.VendorCode", "v.VendorName", "v.ShortName", "vg.VendorGroupName", "v.VATRegistrationNo", "v.VATRegistrationNo", "v.TINNo" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtVendorId.Text = selectedRow.Cells["VendorID"].Value.ToString();
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

                //StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var dal = new SaleDAL();
            try
            {
                progressBar1.Visible = true;
                button1.Enabled = false;
              
                string InvoicrFrom = dtpInvoiceFrom.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                string InvoiceTo = dtpInvoiceTo.Value.ToString("yyyy-MMM-dd HH:mm:ss");

                string ReceiveDateFrom = dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                string ReceiveDateTo = dtpReceiveToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");

                string ExpireDateFrom = dtpExpireFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                string ExpireDateTo = dtpExpireToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");


                string VendorID = txtVendorId.Text;
                string ItemNo = txtItemNoP.Text;
                //string Product = cmbProduct.Text;

                PurchaseDAL dal = new PurchaseDAL();

                DataTable data = dal.GetPurchaseTrakingExcelData(VendorID, ItemNo, InvoicrFrom, InvoiceTo, ReceiveDateFrom, ReceiveDateTo, ExpireDateFrom, ExpireDateTo);



                if (data.Rows.Count == 0)
                {
                    data.Rows.Add(data.NewRow());
                }

                OrdinaryVATDesktop.SaveExcel(data, "PurchaseTraking", "PurchaseTrakingM");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");

                // get value

                // send value

                // check null values

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }
            finally
            {
                progressBar1.Visible = false;
                button1.Enabled = true;
            }
        }

        private void txtProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad(true);
            }
        }


        public void ClearAllFields()
        {
            txtCustomerId.Text = "";
            txtCustomer.Text = "";
            txtProduct.Text = "";
            txtItemNo.Text = "";
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void txtCustomer_DoubleClick(object sender, EventArgs e)
        {
            customerLoad();
        }

        private void txtProduct_DoubleClick(object sender, EventArgs e)
        {
            ProductLoad(true);
        }

        private void txtProductNameP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad(false);
            }
        }

        private void txtProductNameP_DoubleClick(object sender, EventArgs e)
        {
            ProductLoad(false);
        }

        private void txtVendorName_DoubleClick(object sender, EventArgs e)
        {
            VendorLoad();

        }

        private void btnRefreshP_Click(object sender, EventArgs e)
        {

        }
        public void ClearAllFieldsP()
        {
            txtVendorId.Text = "";
            txtVendorName.Text = "";
            txtProductNameP.Text = "";
            txtItemNoP.Text = "";

        }

        

    }
}
