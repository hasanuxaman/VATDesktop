using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

//
//
using System.IO;
using System.Security.Cryptography;
using SymphonySofttech.Utilities;
using System.Data.SqlClient;
using System.Diagnostics;
//
using System.Globalization;
using VATClient.ModelDTO;
using System.Web.Services.Protocols;
using VATServer.Library;
using OfficeOpenXml;
using System.Reflection;
using System.Xml;
using VATServer.Ordinary;
using Newtonsoft.Json;
using VATDesktop.Repo;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormProductSearch : Form
    {
        #region Constructors

        public FormProductSearch()
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

        ParameterVM paramVM = new ParameterVM();
        ResultVM rVM = new ResultVM();
        ProductDAL _ProductDAL = new ProductDAL();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public string VFIN = "305";
        private int searchBranchId = 0;
        private string SelectedValue = string.Empty;
        private string category = string.Empty;
        private string uom = string.Empty;
        private string type = string.Empty;
        private string isVDS = string.Empty;
        private string activeStatus = string.Empty;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private List<ProductDTO> Products = new List<ProductDTO>();
        public static string ProductCategoryName = string.Empty;
        private string RecordCount = "0";

        #region Global Variables For BackGroundWorker

        private string ProductData = string.Empty;
        private DataTable ProductResult;

        private DataTable ProductTypeResult;
        //private string ProductCategoryResult = string.Empty;
        private DataTable ProductCategoryResult;


        #endregion

        #endregion

        #region Methods 01
        
        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;

            #region try

            try
            {
                FormProductSearch frmSearch = new FormProductSearch();
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

        private void ClearAllFields()
        {
            //cmbActiveStatus.SelectedIndex = 0;
            txtCategoryID.Text = "";
            cmbProductCategoryName.SelectedIndex = -1;
            txtCostPrice.Text = "0.00";
            txtHSCodeNo.Text = "";
            cmbProductType.SelectedIndex = -1;
            txtVATRateFrom.Text = "0.00";
            txtVATRateTo.Text = "0.00";
            cmbUOM.SelectedIndex = -1;

            txtItemNo.Text = "";
            txtNRBPrice.Text = "0.00";
            txtProductName.Text = "";
            txtSalesPrice.Text = "0.00";
            txtSerialNo.Text = "";
            txtVATRate.Text = "0.00";
            cmbProduct.SelectedIndex = -1;
            cmbActive.Text = "";

            //dgvProduct.DataSource = null;
            dgvProduct.Rows.Clear();
        }

        private void GridSeleted()
        {
            #region try
            try
            {
                if (dgvProduct.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvProduct.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }


                this.Close();
            }
            #endregion

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

        private void ProductCategorySearch()
        {
            #region try
            try
            {
                backgroundWorkerProductCategorySearch.RunWorkerAsync();
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
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
                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
            }
            #endregion
        }

        private void PTypeSearch()
        {
            #region try
            try
            {
                backgroundWorkerPTypeSearch.RunWorkerAsync();

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
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
                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
            }
            #endregion
        }

        private void nullCheck()
        {
            #region try
            try
            {
                if (txtCostPriceFrom.Text == "")
                {
                    txtCostPriceFrom.Text = "0.00";
                }
                if (txtCostPriceTo.Text == "")
                {
                    txtCostPriceTo.Text = "0.00";
                }
                if (txtSalesPriceFrom.Text == "")
                {
                    txtSalesPriceFrom.Text = "0.00";
                }
                if (txtSalesPriceTo.Text == "")
                {
                    txtSalesPriceTo.Text = "0.00";
                }
                if (txtNBRPriceFrom.Text == "")
                {
                    txtNBRPriceFrom.Text = "0.00";
                }
                if (txtNBRPriceTo.Text == "")
                {
                    txtNBRPriceTo.Text = "0.00";
                }
                if (txtVATRateFrom.Text == "")
                {
                    txtVATRateFrom.Text = "0.00";
                }
                if (txtVATRateTo.Text == "")
                {
                    txtVATRateTo.Text = "0.00";
                }
                if (txtSDRate.Text == "")
                {
                    txtSDRate.Text = "0.00";
                }
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "nullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "nullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "nullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "nullCheck", exMessage);
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
                FileLogger.Log(this.Name, "nullCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "nullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "nullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "nullCheck", exMessage);
            }
            #endregion
        }

        private void SearchDT()
        {
            ProductLoad();
            ProductComplete();
        }

        private void SearchDTBack()
        {
            #region try
            try
            {
                //              


                category = string.Empty;
                uom = string.Empty;
                type = string.Empty;
                activeStatus = string.Empty;
                if (!string.IsNullOrEmpty(ProductCategoryName))
                {
                    category = ProductCategoryName;
                }
                else
                {
                    category = cmbProductCategoryName.Text.Trim();
                }

                uom = cmbUOM.Text.Trim();
                if (Program.ItemType == "Overhead")
                {
                    //type = "Overhead";
                }
                else
                {
                    type = cmbProductType.Text.Trim();
                } activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;


                backgroundWorkerSearch.RunWorkerAsync();

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchDT", exMessage);
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
                FileLogger.Log(this.Name, "SearchDT", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchDT", exMessage);
            }
            #endregion
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            nullCheck();
            //PTypeSearch();
            RecordCount = cmbRecordCount.Text.Trim();

            this.btnSearch.Enabled = false;
            this.progressBar1.Visible = true;
            searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);

            SearchDT();

            txtProductName.Focus();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtItemNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCategoryID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbIsRaw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVATRateFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVATRateTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtHSCodeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbActiveStatus_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void dgvProduct_DoubleClick(object sender, EventArgs e)
        {

            GridSeleted();

        }

        private void FormProductSearch_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                //this.progressBar1.Visible = true;

                //dgvProduct.Columns["ProductDescription"].Visible = false;
                //dgvProduct.Columns["CategoryID"].Visible = false;
                //dgvProduct.Columns["SalesPrice"].Visible = false;
                //dgvProduct.Columns["SerialNo"].Visible = false;
                //dgvProduct.Columns["OpeningBalance"].Visible = false;
                //dgvProduct.Columns["Comments"].Visible = false;
                //dgvProduct.Columns["HSDescription"].Visible = false;

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                CommonDAL dal = new CommonDAL();
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

                cmbBranch.SelectedValue = Program.BranchId;
                cmbImport.SelectedIndex = 0;
                //BranchLoad(cmbBranch);
                FormLoad();

                //PTypeSearch(); //SOAP Service
                //ProductCategorySearch(); //SOAP Service
                //SearchDT(); //SOAP Service
                //LoadProductList();

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormProductSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormProductSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormProductSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormProductSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormProductSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormProductSearch_Load", exMessage);
            }
            #endregion

        }

        //public void BranchLoad(ComboBox cmbBranch)
        //{

        //    BranchProfileDAL _BranchProfileDAL = new BranchProfileDAL();
        //    DataTable dtBranchProfile = new DataTable();

        //    dtBranchProfile = _BranchProfileDAL.SelectAll(null, null, null, null, null, true);


        //    DataRow dr = dtBranchProfile.NewRow();
        //    dr["BranchID"] = "0";
        //    dr["BranchName"] = "All Branch";
        //    dtBranchProfile.Rows.InsertAt(dr, 0);

        //    cmbBranch.DataSource = dtBranchProfile;
        //    cmbBranch.ValueMember = "BranchID";
        //    cmbBranch.DisplayMember = "BranchName";

        //}

        private void FormLoad()
        {
            //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
            IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);

            string[] cValues = new string[] { "0", "0", "Y", "0" };
            string[] cFields = new string[] { "VATRate>=", "VATRate<=", "ActiveStatus like", "SD" };
            ProductCategoryResult = productCategoryDal.SelectAll(0, cFields, cValues, null, null, false, "", connVM);

            //ProductCategoryResult = productCategoryDal.SearchProductCategory("", "", "", "", 0, 0, "Y", 0, "", "", Program.DatabaseName);

            cmbProductType.Items.Clear();
            ProductDAL productTypeDal = new ProductDAL();

            //cmbProductType.DataSource = productTypeDal.ProductTypeList;
            //cmbProductType.SelectedIndex = -1;


            IList<string> protype = productTypeDal.ProductTypeList;

            if (Program.ItemType != "Overhead")
            {
                protype.Remove("Overhead");

            }
            if (Program.ItemType == "Overhead")
            {
                protype.Clear();
                protype.Add("Overhead");
            }


            cmbProductType.DataSource = protype;
            if (Program.ItemType != "Overhead")
            {
                cmbProductType.SelectedIndex = -1;

            }


            var prodCategories = (from DataRow row in ProductCategoryResult.Rows
                                  select row["CategoryName"].ToString()

                     ).ToList();

            if (prodCategories != null && prodCategories.Any())
            {
                //var uoms = from uom in UOMs.ToList()
                //           select uom.UOMTo;
                cmbProductCategoryName.Items.Clear();
                cmbProductCategoryName.Items.AddRange(prodCategories.ToArray());
            }
            //SearchDT();
            txtProductName.Focus();
            #region cmbBranch DropDownWidth Change
            CommonDAL commonDal = new CommonDAL();
            string cmbBranchDropDownWidth = commonDal.settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

        }

        #endregion

        #region Methods 02

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                //MDIMainInterface mdi = new MDIMainInterface();
                FormProduct frmProduct = new FormProduct();
                //mdi.RollDetailsInfo(frmProduct.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmProduct.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmProduct.Show();
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }

            #endregion
        }

        private void txtVATRateFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVATRateFrom, "VAT Amount From");
        }

        private void txtVATRateTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVATRateTo, "VAT Amount To");
        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            #region try
            try
            {

                //string result = FormProductTypeSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());
                //    cmbProductType.Text = TypeInfo[1];
                //}

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            }
            #endregion
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSearchProductCategory_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            #region try
            try
            {
                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormProductCategorySearch frm = new FormProductCategorySearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();

                if (result == "")
                {
                    return;
                }
                else//if (result == ""){return;}else//if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtCategoryID.Text = ProductCategoryInfo[0];
                    cmbProductCategoryName.Text = ProductCategoryInfo[1];
                }
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }
            #endregion
        }

        #region Background Worker Events

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {

                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResult = productDal.SearchProductDT(
                    txtItemNo.Text.Trim()
                    , txtProductName.Text.Trim()
                    , txtCategoryID.Text.Trim()
                    , category
                    , uom
                    , type
                    , txtSerialNo.Text.Trim()
                    , txtHSCodeNo.Text.Trim()
                    , activeStatus
                    , "" //trading
                    , ""//
                    , txtPCode.Text.Trim()
                    , Program.DatabaseName
                    , "0"
                    , ""
                    , searchBranchId
                    , connVM)
                ;
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
        private void ProductLoad()
        {
            #region Try

            try
            {
                //BugsBD
                string ItemNo = OrdinaryVATDesktop.SanitizeInput(txtItemNo.Text.Trim());
                string  ProductName = OrdinaryVATDesktop.SanitizeInput(txtProductName.Text.Trim());
                string CategoryID = OrdinaryVATDesktop.SanitizeInput(txtCategoryID.Text.Trim());
                string SerialNo = OrdinaryVATDesktop.SanitizeInput(txtSerialNo.Text.Trim());
                string HSCodeNo = OrdinaryVATDesktop.SanitizeInput(txtHSCodeNo.Text.Trim());
                string PCode = OrdinaryVATDesktop.SanitizeInput(txtPCode.Text.Trim());


                if (Program.ItemType != "Other")
                {
                    //type = Program.ItemType;
                }
                category = "";
                activeStatus = "";
                type = "";
                isVDS = "";
                if (!string.IsNullOrEmpty(cmbProductCategoryName.Text.Trim()))
                    category = cmbProductCategoryName.Text.Trim();
                if (!string.IsNullOrEmpty(cmbActive.Text.Trim()))
                    activeStatus = cmbActive.Text.Trim();
                if (!string.IsNullOrEmpty(cmbProductType.Text.Trim()))
                    type = cmbProductType.Text.Trim();

                if (!string.IsNullOrEmpty(cmbIsVDS.Text.Trim()))
                    isVDS = cmbIsVDS.Text.Trim();

                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                IUserBranchDetail userBranch = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);

                List<UserBranchDetailVM> vms = null;

                CommonDAL commonDal = new CommonDAL();

                string value = commonDal.settingsDesktop("Setup", "ShowAllProduct",null,connVM);

                if (value == "N")
                {
                    vms = userBranch.SelectAllLst(0, new[] { "uf.UserId" }, new[] { Program.CurrentUserID });
                }

                string IsOverhead = "N";
                if (Program.ItemType == "Overhead")
                {
                    IsOverhead = "Y";
                }


                string[] cValues =
                {
                    //txtItemNo.Text.Trim(),
                    //txtProductName.Text.Trim(),
                    //txtCategoryID.Text.Trim(),
                     //txtSerialNo.Text.Trim(),
                    //txtHSCodeNo.Text.Trim(), 
                    //txtPCode.Text.Trim(),

                    ItemNo,
                    ProductName,
                    CategoryID,
                    category,
                    uom,
                    type,                  
                    SerialNo,
                    HSCodeNo,
                    activeStatus,
                    PCode,
                    chkIsConfirmed.Text,
                    RecordCount

                };
                string[] cFields =
                {
                    "Products.ItemNo", "Products.ProductName like", "Products.CategoryID",
                    "ProductCategories.CategoryName like", "Products.UOM like",
                    "ProductCategories.IsRaw like", "Products.SerialNo like", "Products.HSCodeNo like",
                    "Products.ActiveStatus like", "Products.ProductCode like","Products.IsConfirmed isnull",
                    "SelectTop"
                };
                ProductResult = productDal.SelectProductDTAll(cFields, cValues, null, null, true, 0, Program.DatabaseName, isVDS, vms, connVM, IsOverhead);


                #region Comments Oct-04-2020

                //ProductResult = productDal.SearchProductDT(
                //    txtItemNo.Text.Trim()
                //    , txtProductName.Text.Trim()
                //    , txtCategoryID.Text.Trim()
                //    , category
                //    , uom
                //    , type
                //    , txtSerialNo.Text.Trim()
                //    , txtHSCodeNo.Text.Trim()
                //    , activeStatus
                //    , "" //trading
                //    , ""//
                //    , txtPCode.Text.Trim()
                //    , Program.DatabaseName
                //    , null
                //    , isVDS
                //    ,0 //// searchBranchId
                //    ,connVM
                //    ,vms
                //    )
                //;
                //string[] retResults = new string[4];
                //retResults[0] = "Fail";
                //retResults[1] = "Fail1";
                //retResults[2] = "2";
                //retResults[3] = "3";

                //string json = JsonConvert.SerializeObject(ProductResult, Newtonsoft.Json.Formatting.Indented);
                //string json1 = JsonConvert.SerializeObject(ProductResult);
                //DataTable dataSet = JsonConvert.DeserializeObject<DataTable>(json);


                //json = JsonConvert.SerializeObject(retResults);


                //JsonConvert.DeserializeObject<string[]>(json);

                #endregion

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
        private void ProductComplete()
        {
            string TotalTecordCount = "0";

            #region Try

            try
            {

                dgvProduct.DataSource = null;
                if (ProductResult != null && ProductResult.Rows.Count > 0)
                {


                    TotalTecordCount = ProductResult.Rows[ProductResult.Rows.Count - 1][0].ToString();

                    ProductResult.Rows.RemoveAt(ProductResult.Rows.Count - 1);

                    dgvProduct.DataSource = ProductResult;
                    #region Specific Coloumns Visible False

                    dgvProduct.Columns["ItemNo"].Visible = false;
                    dgvProduct.Columns["CategoryID"].Visible = false;
                    dgvProduct.Columns["BranchId"].Visible = false;

                    ////if (Program.ItemType == "Overhead")
                    ////{
                    ////    dgvProduct.Columns["WastageTotalQuantity"].Visible = false;
                    ////    dgvProduct.Columns["WastageTotalValue"].Visible = false; 
            
                    ////}
                    
                    #endregion

                }


                #region Comments Oct-04-2020


                //    dgvProduct.Rows.Clear();
                //    int j = 0;
                //    foreach (DataRow item in ProductResult.Rows)
                //    {
                //        DataGridView dgvNew = new DataGridView();
                //        dgvProduct.Rows.Add(dgvNew);

                //        dgvProduct.Rows[j].Cells["Select"].Value = false;
                //        dgvProduct.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                //        dgvProduct.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                //        dgvProduct.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();
                //        dgvProduct.Rows[j].Cells["CategoryID"].Value = item["CategoryID"].ToString();
                //        dgvProduct.Rows[j].Cells["CategoryName"].Value = item["CategoryName"].ToString();
                //        dgvProduct.Rows[j].Cells["UOM1"].Value = item["UOM"].ToString();
                //        dgvProduct.Rows[j].Cells["CostPrice"].Value = item["CostPrice"].ToString();
                //        dgvProduct.Rows[j].Cells["SalesPrice"].Value = item["SalesPrice"].ToString();
                //        dgvProduct.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();
                //        dgvProduct.Rows[j].Cells["IsRaw"].Value = item["IsRaw"].ToString();
                //        dgvProduct.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();
                //        dgvProduct.Rows[j].Cells["HSCodeNo"].Value = item["HSCodeNo"].ToString();
                //        dgvProduct.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                //        dgvProduct.Rows[j].Cells["ActiveStatus1"].Value = item["ActiveStatus"].ToString();
                //        dgvProduct.Rows[j].Cells["OpeningBalance"].Value = item["OpeningBalance"].ToString();
                //        dgvProduct.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                //        dgvProduct.Rows[j].Cells["HSDescription"].Value = item["HSDescription"].ToString();
                //        dgvProduct.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                //        dgvProduct.Rows[j].Cells["SD"].Value = item["SD"].ToString();
                //        dgvProduct.Rows[j].Cells["VDSRate"].Value = item["VDSRate"].ToString();
                //        dgvProduct.Rows[j].Cells["Packetprice"].Value = item["Packetprice"].ToString();
                //        dgvProduct.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                //        dgvProduct.Rows[j].Cells["TradingMarkUp"].Value = item["TradingMarkUp"].ToString();
                //        dgvProduct.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();
                //        dgvProduct.Rows[j].Cells["QuantityInHand"].Value = item["QuantityInHand"].ToString();
                //        dgvProduct.Rows[j].Cells["OpeningDate"].Value = item["OpeningDate"].ToString();
                //        dgvProduct.Rows[j].Cells["ReceivePrice"].Value = item["ReceivePrice"].ToString();
                //        dgvProduct.Rows[j].Cells["IssuePrice"].Value = item["IssuePrice"].ToString();
                //        dgvProduct.Rows[j].Cells["ProductCode"].Value = item["ProductCode"].ToString();
                //        dgvProduct.Rows[j].Cells["TollCharge"].Value = item["TollCharge"].ToString();
                //        dgvProduct.Rows[j].Cells["Rebate"].Value = item["RebatePercent"].ToString();
                //        dgvProduct.Rows[j].Cells["OpeningTotalCost"].Value = item["OpeningTotalCost"].ToString();
                //        dgvProduct.Rows[j].Cells["Banderol"].Value = item["Banderol"].ToString();
                //        dgvProduct.Rows[j].Cells["TollProduct"].Value = item["TollProduct"].ToString();


                //        dgvProduct.Rows[j].Cells["CDRate"].Value = item["CDRate"].ToString();
                //        dgvProduct.Rows[j].Cells["RDRate"].Value = item["RDRate"].ToString();
                //        dgvProduct.Rows[j].Cells["TVARate"].Value = item["TVARate"].ToString();
                //        dgvProduct.Rows[j].Cells["ATVRate"].Value = item["ATVRate"].ToString();
                //        dgvProduct.Rows[j].Cells["VATRate2"].Value = item["VATRate2"].ToString();
                //        dgvProduct.Rows[j].Cells["AITRate"].Value = item["AITRate"].ToString();
                //        dgvProduct.Rows[j].Cells["SDRate"].Value = item["SDRate"].ToString();
                //        dgvProduct.Rows[j].Cells["VATRate3"].Value = item["VATRate3"].ToString();

                //        dgvProduct.Rows[j].Cells["IsExempted"].Value = item["IsExempted"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedVAT"].Value = item["IsFixedVAT"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedSD"].Value = item["IsFixedSD"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedCD"].Value = item["IsFixedCD"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedRD"].Value = item["IsFixedRD"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedAIT"].Value = item["IsFixedAIT"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedVAT1"].Value = item["IsFixedVAT1"].ToString();
                //        dgvProduct.Rows[j].Cells["IsFixedAT"].Value = item["IsFixedAT"].ToString();
                //        dgvProduct.Rows[j].Cells["IsZeroVAT"].Value = item["IsZeroVAT"].ToString();
                //        dgvProduct.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                //        dgvProduct.Rows[j].Cells["TDSCode"].Value = item["TDSCode"].ToString();

                //        dgvProduct.Rows[j].Cells["IsVDS1"].Value = item["IsVDS"].ToString();

                //        dgvProduct.Rows[j].Cells["HPSRate"].Value = item["HPSRate"].ToString();



                //        j = j + 1;

                //    }

                //    dgvProduct.Columns["TollCharge"].Visible = false;
                //    dgvProduct.Columns["ProductDescription"].Visible = false;
                //    dgvProduct.Columns["CategoryID"].Visible = false;
                //    dgvProduct.Columns["Comments"].Visible = false;
                //    dgvProduct.Columns["HSDescription"].Visible = false;
                //    dgvProduct.Columns["SalesPrice"].Visible = false;
                //    dgvProduct.Columns["PacketPrice"].Visible = false;
                //    dgvProduct.Columns["SerialNo"].Visible = false;
                //    dgvProduct.Columns["QuantityInHand"].Visible = false;
                //    dgvProduct.Columns["OpeningDate"].Visible = false;
                //    dgvProduct.Columns["IssuePrice"].Visible = false;
                //    dgvProduct.Columns["ReceivePrice"].Visible = false;

                //    txtCostPriceFrom.Text = "";
                //    txtCostPriceTo.Text = "";
                //    txtSalesPriceFrom.Text = "";
                //    txtSalesPriceTo.Text = "";
                //    txtNBRPriceFrom.Text = "";
                //    txtNBRPriceTo.Text = "";
                //    txtVATRateFrom.Text = "";
                //    txtVATRateTo.Text = "";
                //    txtSDRate.Text = "";

                #endregion

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
                //LRecordCount.Text = "Record Count: " + dgvProduct.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (ProductResult.Rows.Count) + " of " + TotalTecordCount.ToString();

            }
        }
        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                dgvProduct.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ProductResult.Rows)
                {
                    DataGridView dgvNew = new DataGridView();
                    dgvProduct.Rows.Add(dgvNew);

                    dgvProduct.Rows[j].Cells["Select"].Value = false;
                    dgvProduct.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvProduct.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                    dgvProduct.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();
                    dgvProduct.Rows[j].Cells["CategoryID"].Value = item["CategoryID"].ToString();
                    dgvProduct.Rows[j].Cells["CategoryName"].Value = item["CategoryName"].ToString();
                    dgvProduct.Rows[j].Cells["UOM1"].Value = item["UOM"].ToString();
                    dgvProduct.Rows[j].Cells["CostPrice"].Value = item["CostPrice"].ToString();
                    dgvProduct.Rows[j].Cells["SalesPrice"].Value = item["SalesPrice"].ToString();
                    dgvProduct.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();
                    dgvProduct.Rows[j].Cells["IsRaw"].Value = item["IsRaw"].ToString();
                    dgvProduct.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();
                    dgvProduct.Rows[j].Cells["HSCodeNo"].Value = item["HSCodeNo"].ToString();
                    dgvProduct.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                    dgvProduct.Rows[j].Cells["ActiveStatus1"].Value = item["ActiveStatus"].ToString();
                    dgvProduct.Rows[j].Cells["OpeningBalance"].Value = item["OpeningBalance"].ToString();
                    dgvProduct.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvProduct.Rows[j].Cells["HSDescription"].Value = item["HSDescription"].ToString();
                    dgvProduct.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                    dgvProduct.Rows[j].Cells["SD"].Value = item["SD"].ToString();
                    dgvProduct.Rows[j].Cells["Packetprice"].Value = item["Packetprice"].ToString();
                    dgvProduct.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    dgvProduct.Rows[j].Cells["TradingMarkUp"].Value = item["TradingMarkUp"].ToString();
                    dgvProduct.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();
                    dgvProduct.Rows[j].Cells["QuantityInHand"].Value = item["QuantityInHand"].ToString();
                    dgvProduct.Rows[j].Cells["OpeningDate"].Value = item["OpeningDate"].ToString();
                    dgvProduct.Rows[j].Cells["ReceivePrice"].Value = item["ReceivePrice"].ToString();
                    dgvProduct.Rows[j].Cells["IssuePrice"].Value = item["IssuePrice"].ToString();
                    dgvProduct.Rows[j].Cells["ProductCode"].Value = item["ProductCode"].ToString();
                    dgvProduct.Rows[j].Cells["TollCharge"].Value = item["TollCharge"].ToString();
                    dgvProduct.Rows[j].Cells["Rebate"].Value = item["RebatePercent"].ToString();
                    dgvProduct.Rows[j].Cells["OpeningTotalCost"].Value = item["OpeningTotalCost"].ToString();
                    dgvProduct.Rows[j].Cells["Banderol"].Value = item["Banderol"].ToString();
                    dgvProduct.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                    dgvProduct.Rows[j].Cells["TransactionHoldDate"].Value = item["TransactionHoldDate"].ToString();

                    j = j + 1;

                }



                dgvProduct.Columns["TollCharge"].Visible = false;
                dgvProduct.Columns["ProductDescription"].Visible = false;
                dgvProduct.Columns["CategoryID"].Visible = false;
                dgvProduct.Columns["Comments"].Visible = false;
                dgvProduct.Columns["HSDescription"].Visible = false;
                dgvProduct.Columns["SalesPrice"].Visible = false;
                dgvProduct.Columns["PacketPrice"].Visible = false;
                dgvProduct.Columns["SerialNo"].Visible = false;
                dgvProduct.Columns["QuantityInHand"].Visible = false;
                dgvProduct.Columns["OpeningDate"].Visible = false;
                dgvProduct.Columns["IssuePrice"].Visible = false;
                dgvProduct.Columns["ReceivePrice"].Visible = false;

                txtCostPriceFrom.Text = "";
                txtCostPriceTo.Text = "";
                txtSalesPriceFrom.Text = "";
                txtSalesPriceTo.Text = "";
                txtNBRPriceFrom.Text = "";
                txtNBRPriceTo.Text = "";
                txtVATRateFrom.Text = "";
                txtVATRateTo.Text = "";
                txtSDRate.Text = "";
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
                LRecordCount.Text = "Record Count: " + dgvProduct.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void backgroundWorkerPTypeSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                //ProductTypeDAL productTypeDal = new ProductTypeDAL();
                //ProductTypeResult = productTypeDal.SearchProductTypeNew("", "", "Y");

                //end DoWork
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

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

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPTypeSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //start Complete
                cmbProductType.Items.Clear();
                ProductDAL productTypeDal = new ProductDAL();
                cmbProductType.DataSource = productTypeDal.ProductTypeList;
                cmbProductType.SelectedIndex = -1;
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

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

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted",

                               exMessage);
            }

            #endregion

            this.progressBar1.Visible = false;
        }

        private void backgroundWorkerProductCategorySearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //start DoWork
                ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
                IProductCategory _sDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);




                string[] cValues = new string[] { "0", "0", "Y", "0" };
                string[] cFields = new string[] { "VATRate>=", "VATRate<=", "ActiveStatus like", "SD" };
                ProductCategoryResult = productCategoryDal.SelectAll(0, cFields, cValues, null, null, false, "", connVM);

                //ProductCategoryResult = productCategoryDal.SearchProductCategory("", "", "", "", 0, 0, "Y", 0, "", "", Program.DatabaseName);

                //end DoWork
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

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

                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerProductCategorySearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var prodCategories = (from DataRow row in ProductCategoryResult.Rows
                                      select row["CategoryName"].ToString()

                                     ).ToList();

                if (prodCategories != null && prodCategories.Any())
                {
                    //var uoms = from uom in UOMs.ToList()
                    //           select uom.UOMTo;
                    cmbProductCategoryName.Items.Clear();
                    cmbProductCategoryName.Items.AddRange(prodCategories.ToArray());
                }

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

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

                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerProductCategorySearch_RunWorkerCompleted",

                               exMessage);
            }

            #endregion

            this.progressBar1.Visible = false;
        }

        #endregion

        private void txtPCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.Down))
            {
                dgvProduct.Select();
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Down))
            {
                dgvProduct.Select();
            }
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbProductCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbProductType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbActiveStatus_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            RecordCount = cmbRecordCount.Text.Trim();

            SearchDT();
        }

        private void txtPCode_TextChanged(object sender, EventArgs e)
        {
            RecordCount = cmbRecordCount.Text.Trim();

            SearchDT();
        }

        private void txtHSCodeNo_TextChanged(object sender, EventArgs e)
        {
            RecordCount = cmbRecordCount.Text.Trim();

            SearchDT();
        }

        #endregion

        #region Methods 03

        private void cmbProductCategoryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SearchDT();
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SearchDT();
        }

        private void cmbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SearchDT();
        }

        private void dgvProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                GridSeleted();
            }
            //if (e.KeyCode.Equals(Keys.Enter))
            //{
            //    GridSeleted();
            //}
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var ids = new List<string>();

                var len = dgvProduct.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                    {
                        ids.Add(dgvProduct["ItemNo", i].Value.ToString());
                    }
                }

                var productDAL = new ProductDAL();
                //IProduct productDAL = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = productDAL.GetExcelData(ids);
                var details = productDAL.GetExcelProductDetails(ids, null, null, connVM);
                //var ProductStock = productDAL.GetExcelProductStock(ids, null, null, connVM);

                DataTable ProductStock = new DataTable("ProductStock");

                decimal idsCount = ids.Count;

                int loopCount = Convert.ToInt32(Math.Ceiling(idsCount / 500));

                //int Skip = 0;

                //int Take = 5000;

                //for (int i = 0; i < loopCount; i++)
                //{
                //    var list = ids.Skip(Skip).Take(Take).ToList();

                //    Skip = Skip + Take;
                //    if(list.Count>0)
                //    {
                //        var ProductStockAll = productDAL.GetExcelProductStock(list, null, null, connVM);
                //        ProductStock.Merge(ProductStockAll);

                //    }

                   
                //}


                #region

                //if (details.Rows.Count == 0)
                //{
                //    details.Rows.Add(details.NewRow());
                //}

                //if (ProductResult != null && ProductResult.Rows.Count > 0)
                //{
                //    dt = ProductResult;
                //}

                #endregion

                var dataSet = new DataSet();

                var sheetNames = new List<string> { "Products" };

                dataSet.Tables.Add(dt);


                if (details.Rows.Count > 0)
                {
                    dataSet.Tables.Add(details);
                    sheetNames.Add("ProductDetails");
                }
                //if (ProductStock.Rows.Count > 0)
                //{
                //    dataSet.Tables.Add(ProductStock);
                //    sheetNames.Add("ProductStock");
                //}





                #region temp

                //for (int i = 0; i < dgvProduct.ColumnCount; i++)
                //{
                //    dt.Columns.Add(dgvProduct.Columns[i].Name);
                //}

                //for (int i = 0; i < dgvProduct.RowCount; i++)
                //{
                //    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                //    {
                //        dt.Rows.Add(dt.NewRow());
                //        dt.Rows[i]["ItemNo"] = dgvProduct["ItemNo",i].Value;
                //        dt.Rows[i]["ProductName"] = dgvProduct["ProductName",i].Value;
                //        dt.Rows[i]["ProductDescription"] = dgvProduct["ProductDescription",i].Value;
                //        dt.Rows[i]["CategoryID"] = dgvProduct["CategoryID",i].Value;
                //        dt.Rows[i]["CategoryName"] = dgvProduct["CategoryName",i].Value;
                //        dt.Rows[i]["UOM1"] = dgvProduct["UOM1",i].Value;
                //        dt.Rows[i]["CostPrice"] = dgvProduct["CostPrice",i].Value;
                //        dt.Rows[i]["SalesPrice"] = dgvProduct["SalesPrice",i].Value;
                //        dt.Rows[i]["NBRPrice"] = dgvProduct["NBRPrice",i].Value;
                //        dt.Rows[i]["IsRaw"] = dgvProduct["IsRaw",i].Value;
                //        dt.Rows[i]["SerialNo"] = dgvProduct["SerialNo",i].Value;
                //        dt.Rows[i]["HSCodeNo"] = dgvProduct["HSCodeNo",i].Value;
                //        dt.Rows[i]["VATRate"] = dgvProduct["VATRate",i].Value;
                //        dt.Rows[i]["ActiveStatus1"] = dgvProduct["ActiveStatus1",i].Value;
                //        dt.Rows[i]["OpeningBalance"] = dgvProduct["OpeningBalance",i].Value;
                //        dt.Rows[i]["Comments"] = dgvProduct["Comments",i].Value;
                //        dt.Rows[i]["HSDescription"] = dgvProduct["HSDescription",i].Value;
                //        dt.Rows[i]["Stock"] = dgvProduct["Stock",i].Value;
                //        dt.Rows[i]["SD"] = dgvProduct["SD",i].Value;
                //        dt.Rows[i]["Packetprice"] = dgvProduct["Packetprice",i].Value;
                //        dt.Rows[i]["Trading"] = dgvProduct["Trading",i].Value;
                //        dt.Rows[i]["TradingMarkUp"] = dgvProduct["TradingMarkUp",i].Value;
                //        dt.Rows[i]["NonStock"] = dgvProduct["NonStock",i].Value;
                //        dt.Rows[i]["QuantityInHand"] = dgvProduct["QuantityInHand",i].Value;
                //        dt.Rows[i]["OpeningDate"] = dgvProduct["OpeningDate",i].Value;
                //        dt.Rows[i]["ReceivePrice"] = dgvProduct["ReceivePrice",i].Value;
                //        dt.Rows[i]["IssuePrice"] = dgvProduct["IssuePrice",i].Value;
                //        dt.Rows[i]["ProductCode"] = dgvProduct["ProductCode",i].Value;
                //        dt.Rows[i]["TollCharge"] = dgvProduct["TollCharge",i].Value;
                //        dt.Rows[i]["Rebate"] = dgvProduct["Rebate", i].Value;
                //        dt.Rows[i]["OpeningTotalCost"] = dgvProduct["OpeningTotalCost",i].Value;
                //        dt.Rows[i]["Banderol"] = dgvProduct["Banderol",i].Value;
                //        dt.Rows[i]["BranchId"] = dgvProduct["BranchId",i].Value;
                //    }
                //}

                #endregion



                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "Products", sheetNames.ToArray());

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "Products");
                }



                //string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                //string fileDirectory = pathRoot + "//Excel Files";
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}
                //fileDirectory += "\\products.xlsx";
                //FileStream objFileStrm = File.Create(fileDirectory);

                //using (ExcelPackage pck = new ExcelPackage(objFileStrm))
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Products");
                //    ws.Cells["A1"].LoadFromDataTable(dt, true);
                //    pck.Save();
                //    objFileStrm.Close();
                //}
                //MessageBox.Show("Successfully Exported data as Products.xlsx in Excel files of root directory");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkSelectAll_CheckedChanged_1(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvProduct.RowCount; i++)
            {
                dgvProduct["Select", i].Value = chkSelectAll.Checked;
            }
        }

        DataTable dt = new DataTable();

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.btnLoad.Enabled = false;
            this.progressBar1.Visible = true;
            bgwVATProduct.RunWorkerAsync();

        }

        private void bgwVATProduct_DoWork(object sender, DoWorkEventArgs e)
        {
            dt = new DataTable();
            dt = VATProductLoad(dt);
        }

        private void bgwVATProduct_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {


                DataSet dataSet = new DataSet();
                string[] sheetNames = new[] { "Products" };
                dataSet.Tables.Add(dt);

                OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "Products", sheetNames);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                this.btnLoad.Enabled = true;
                this.progressBar1.Visible = false;

            }
        }

        private DataTable VATProductLoad(DataTable dt)
        {
            //////dt = new DataTable();
            try
            {

                dt = ProductResult.Copy();

                string tranDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:00");
                ReportDSDAL _reportDal = new ReportDSDAL();
                //IReport _reportDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();


                #region DataLoad

                if (chkVAT16.Checked)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string itemNo = dr["itemNo"].ToString();

                        #region Parmeter Assign (VAT 6.1)

                        VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

                        varVAT6_1ParamVM.ItemNo = itemNo;
                        varVAT6_1ParamVM.StartDate = tranDate;
                        varVAT6_1ParamVM.EndDate = tranDate;
                        varVAT6_1ParamVM.UserName = "";
                        varVAT6_1ParamVM.Post1 = "Y";
                        varVAT6_1ParamVM.Post2 = "Y";
                        varVAT6_1ParamVM.ReportName = "";
                        varVAT6_1ParamVM.BranchId = OrdinaryVATDesktop.BranchId;
                        varVAT6_1ParamVM.Opening = false;
                        varVAT6_1ParamVM.UserId = Program.CurrentUserID;

                        #endregion


                        DataSet AvgPriceVAT16 = _vatRegistersDAL.VAT6_1_WithConn(varVAT6_1ParamVM, null, null, connVM);
                        decimal Quantity = Convert.ToDecimal(AvgPriceVAT16.Tables[0].Rows[0]["Quantity"]);
                        decimal Amount = Convert.ToDecimal(AvgPriceVAT16.Tables[0].Rows[0]["UnitCost"]);

                        dr["OpeningBalance"] = Quantity;
                        dr["OpeningTotalCost"] = Amount;

                    }
                }
                else
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        string itemNo = dr["itemNo"].ToString();

                        #region Parmeter Assign

                        VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                        varVAT6_2ParamVM.ItemNo = itemNo;
                        varVAT6_2ParamVM.StartDate = tranDate;
                        varVAT6_2ParamVM.EndDate = tranDate;
                        varVAT6_2ParamVM.Post1 = "Y";
                        varVAT6_2ParamVM.Post2 = "Y";
                        varVAT6_2ParamVM.BranchId = OrdinaryVATDesktop.BranchId;
                        varVAT6_2ParamVM.Opening = true;
                        varVAT6_2ParamVM.UserId = Program.CurrentUserID;


                        #endregion


                        DataSet AvgPriceVAT17 = _vatRegistersDAL.VAT6_2(varVAT6_2ParamVM, null, null, connVM);

                        decimal Quantity = Convert.ToDecimal(AvgPriceVAT17.Tables[0].Rows[0]["Quantity"]);
                        decimal Amount = Convert.ToDecimal(AvgPriceVAT17.Tables[0].Rows[0]["UnitCost"]);

                        dr["OpeningBalance"] = Quantity;
                        dr["OpeningTotalCost"] = Amount;

                    }

                }
                #endregion
            }
            catch (Exception ex)
            {

                dt = new DataTable();
            }
            finally { }
            return dt;
        }

        private void chkVAT16_CheckedChanged(object sender, EventArgs e)
        {
            if (chkVAT16.Checked)
            {
                chkVAT16.Text = "VAT 16";
            }
            else
            {
                chkVAT16.Text = "VAT 17";
            }
        }

        private void btnAvgPrice_Click(object sender, EventArgs e)
        {
            try
            {

                progressBar1.Visible = true;

                bgwAvgPrice.RunWorkerAsync();


            }
            catch (Exception exception)
            {
                FileLogger.Log("ProductSearch", "Avgprice", exception.Message);
                MessageBox.Show(exception.Message);
            }
        }

        #endregion

        #region Methods 04

        private void bgwAvgPrice_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PurchaseDAL purchaseDal = new PurchaseDAL();

                string[] result = purchaseDal.UpdateIntialAvgPrice();  
            }
            catch (Exception exception)
            {
                FileLogger.Log("ProductSearch", "Search", exception.Message);
            }
        }

        private void bgwAvgPrice_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show("Avg Price Table Updated");

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnStockUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
                dgvr = dgvProduct.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true).ToList();

                paramVM = new ParameterVM();
                paramVM.IDs = new List<string>();

                paramVM.BranchId = Program.BranchId;

                foreach (DataGridViewRow dr in dgvr)
                {
                    paramVM.IDs.Add(dr.Cells["ItemNo"].Value.ToString());
                }

                if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                {
                    throw new Exception("Please Select Before Update!");
                }

                progressBar1.Visible = true;

                bgwStockUpdate.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnStockUpdate_Click", exMessage);
            }
        }

        private void bgwStockUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                rVM = _ProductDAL.Product_Stock_Update(paramVM,null,null,connVM,Program.CurrentUserID);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void bgwStockUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                FileLogger.Log(this.Name, "bgwStockUpdate_RunWorkerCompleted", exMessage);
            }
            finally { }

            progressBar1.Visible = false;
        }

        #endregion

        private void btnTradingExport_Click(object sender, EventArgs e)
        {
            try
            {
                var ids = new List<string>();

                var len = dgvProduct.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                    {
                        ids.Add(dgvProduct["ItemNo", i].Value.ToString());
                    }
                }

                ProductDAL productDAL = new ProductDAL();

                ////IProduct productDAL = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = productDAL.GetTradingExcelData(ids);
                var details = productDAL.GetExcelProductDetails(ids, null, null, connVM);
                //var ProductStock = productDAL.GetExcelProductStock(ids, null, null, connVM);

                DataTable ProductStock = new DataTable("ProductStock");

                decimal idsCount = ids.Count;

                int loopCount = Convert.ToInt32(Math.Ceiling(idsCount / 500));

                //int Skip = 0;

                //int Take = 5000;

                //for (int i = 0; i < loopCount; i++)
                //{
                //    var list = ids.Skip(Skip).Take(Take).ToList();

                //    Skip = Skip + Take;
                //    if (list.Count > 0)
                //    {
                //        var ProductStockAll = productDAL.GetExcelProductStock(list, null, null, connVM);
                //        ProductStock.Merge(ProductStockAll);

                //    }


                //}


                #region

                //if (details.Rows.Count == 0)
                //{
                //    details.Rows.Add(details.NewRow());
                //}

                //if (ProductResult != null && ProductResult.Rows.Count > 0)
                //{
                //    dt = ProductResult;
                //}

                #endregion

                var dataSet = new DataSet();

                var sheetNames = new List<string> { "Products" };

                dataSet.Tables.Add(dt);


                if (details.Rows.Count > 0)
                {
                    dataSet.Tables.Add(details);
                    sheetNames.Add("ProductDetails");
                }
                //if (ProductStock.Rows.Count > 0)
                //{
                //    dataSet.Tables.Add(ProductStock);
                //    sheetNames.Add("ProductStock");
                //}





                #region temp

                //for (int i = 0; i < dgvProduct.ColumnCount; i++)
                //{
                //    dt.Columns.Add(dgvProduct.Columns[i].Name);
                //}

                //for (int i = 0; i < dgvProduct.RowCount; i++)
                //{
                //    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                //    {
                //        dt.Rows.Add(dt.NewRow());
                //        dt.Rows[i]["ItemNo"] = dgvProduct["ItemNo",i].Value;
                //        dt.Rows[i]["ProductName"] = dgvProduct["ProductName",i].Value;
                //        dt.Rows[i]["ProductDescription"] = dgvProduct["ProductDescription",i].Value;
                //        dt.Rows[i]["CategoryID"] = dgvProduct["CategoryID",i].Value;
                //        dt.Rows[i]["CategoryName"] = dgvProduct["CategoryName",i].Value;
                //        dt.Rows[i]["UOM1"] = dgvProduct["UOM1",i].Value;
                //        dt.Rows[i]["CostPrice"] = dgvProduct["CostPrice",i].Value;
                //        dt.Rows[i]["SalesPrice"] = dgvProduct["SalesPrice",i].Value;
                //        dt.Rows[i]["NBRPrice"] = dgvProduct["NBRPrice",i].Value;
                //        dt.Rows[i]["IsRaw"] = dgvProduct["IsRaw",i].Value;
                //        dt.Rows[i]["SerialNo"] = dgvProduct["SerialNo",i].Value;
                //        dt.Rows[i]["HSCodeNo"] = dgvProduct["HSCodeNo",i].Value;
                //        dt.Rows[i]["VATRate"] = dgvProduct["VATRate",i].Value;
                //        dt.Rows[i]["ActiveStatus1"] = dgvProduct["ActiveStatus1",i].Value;
                //        dt.Rows[i]["OpeningBalance"] = dgvProduct["OpeningBalance",i].Value;
                //        dt.Rows[i]["Comments"] = dgvProduct["Comments",i].Value;
                //        dt.Rows[i]["HSDescription"] = dgvProduct["HSDescription",i].Value;
                //        dt.Rows[i]["Stock"] = dgvProduct["Stock",i].Value;
                //        dt.Rows[i]["SD"] = dgvProduct["SD",i].Value;
                //        dt.Rows[i]["Packetprice"] = dgvProduct["Packetprice",i].Value;
                //        dt.Rows[i]["Trading"] = dgvProduct["Trading",i].Value;
                //        dt.Rows[i]["TradingMarkUp"] = dgvProduct["TradingMarkUp",i].Value;
                //        dt.Rows[i]["NonStock"] = dgvProduct["NonStock",i].Value;
                //        dt.Rows[i]["QuantityInHand"] = dgvProduct["QuantityInHand",i].Value;
                //        dt.Rows[i]["OpeningDate"] = dgvProduct["OpeningDate",i].Value;
                //        dt.Rows[i]["ReceivePrice"] = dgvProduct["ReceivePrice",i].Value;
                //        dt.Rows[i]["IssuePrice"] = dgvProduct["IssuePrice",i].Value;
                //        dt.Rows[i]["ProductCode"] = dgvProduct["ProductCode",i].Value;
                //        dt.Rows[i]["TollCharge"] = dgvProduct["TollCharge",i].Value;
                //        dt.Rows[i]["Rebate"] = dgvProduct["Rebate", i].Value;
                //        dt.Rows[i]["OpeningTotalCost"] = dgvProduct["OpeningTotalCost",i].Value;
                //        dt.Rows[i]["Banderol"] = dgvProduct["Banderol",i].Value;
                //        dt.Rows[i]["BranchId"] = dgvProduct["BranchId",i].Value;
                //    }
                //}

                #endregion



                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "TradingProducts", sheetNames.ToArray());

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "TradingProducts");
                }



                //string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                //string fileDirectory = pathRoot + "//Excel Files";
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}
                //fileDirectory += "\\products.xlsx";
                //FileStream objFileStrm = File.Create(fileDirectory);

                //using (ExcelPackage pck = new ExcelPackage(objFileStrm))
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Products");
                //    ws.Cells["A1"].LoadFromDataTable(dt, true);
                //    pck.Save();
                //    objFileStrm.Close();
                //}
                //MessageBox.Show("Successfully Exported data as Products.xlsx in Excel files of root directory");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnBOMExport_Click(object sender, EventArgs e)
        {
            List<string> ids = new List<string>();

            int len = dgvProduct.RowCount;

            for (int i = 0; i < len; i++)
            {
                if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                {
                    ids.Add(dgvProduct["ItemNo", i].Value.ToString());
                }
            }

            if (ids.Count == 0)
            {
                MessageBox.Show("No Data Found");
                return;
            }

            ProductDAL productDal = new ProductDAL();

            DataTable bomData = productDal.GetBOMData(ids);

            OrdinaryVATDesktop.SaveExcel(bomData,"BOMs","Product");

        }

        private void btnProductStockExport_Click(object sender, EventArgs e)
        {
            try
            {
                var ids = new List<string>();

                var len = dgvProduct.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                    {
                        ids.Add(dgvProduct["ItemNo", i].Value.ToString());
                    }
                }

                var productDAL = new ProductDAL();
                //IProduct productDAL = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                //DataTable dt = productDAL.GetExcelData(ids);
                //var details = productDAL.GetExcelProductDetails(ids, null, null, connVM);
                //var ProductStock = productDAL.GetExcelProductStock(ids, null, null, connVM);

                DataTable ProductStock = new DataTable("ProductStock");

                decimal idsCount = ids.Count;

                int loopCount = Convert.ToInt32(Math.Ceiling(idsCount / 500));

                int Skip = 0;

                int Take = 5000;

                for (int i = 0; i < loopCount; i++)
                {
                    var list = ids.Skip(Skip).Take(Take).ToList();

                    Skip = Skip + Take;
                    if (list.Count > 0)
                    {
                        var ProductStockAll = productDAL.GetExcelProductStock(list, null, null, connVM);
                        ProductStock.Merge(ProductStockAll);

                    }


                }


                #region

                //if (details.Rows.Count == 0)
                //{
                //    details.Rows.Add(details.NewRow());
                //}

                //if (ProductResult != null && ProductResult.Rows.Count > 0)
                //{
                //    dt = ProductResult;
                //}

                #endregion

                var dataSet = new DataSet();

                var sheetNames = new List<string> {  };

                //dataSet.Tables.Add(dt);


                //if (details.Rows.Count > 0)
                //{
                //    dataSet.Tables.Add(details);
                //    sheetNames.Add("ProductDetails");
                //}

                if (ProductStock.Rows.Count > 0)
                {
                    dataSet.Tables.Add(ProductStock);
                    sheetNames.Add("ProductStock");
                }



                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "Products", sheetNames.ToArray());

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "Products");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPHistoryExport_Click(object sender, EventArgs e)
        {
            try
            {
                var ids = new List<string>();

                var len = dgvProduct.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvProduct["Select", i].Value))
                    {
                        ids.Add(dgvProduct["ItemNo", i].Value.ToString());
                    }
                }

                ProductDAL productDAL = new ProductDAL();

                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dtProductPriceHistory = new DataTable("ProductPriceHistory");

                decimal idsCount = ids.Count;

                int loopCount = Convert.ToInt32(Math.Ceiling(idsCount / 500));

                int Skip = 0;

                int Take = 5000;

                for (int i = 0; i < loopCount; i++)
                {
                    var list = ids.Skip(Skip).Take(Take).ToList();

                    Skip = Skip + Take;
                    if (list.Count > 0)
                    {
                        var ProductStockAll = productDAL.GetExcelProductPriceHistory(list, null, null, connVM);
                        dtProductPriceHistory.Merge(ProductStockAll);
                    }
                }

                #region

                #endregion

                var dataSet = new DataSet();

                var sheetNames = new List<string> { };

                if (dtProductPriceHistory.Rows.Count > 0)
                {
                    dataSet.Tables.Add(dtProductPriceHistory);
                    sheetNames.Add("ProductPriceHistory");
                }

                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "ProductPriceHistory", sheetNames.ToArray());

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "ProductPriceHistory");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

    }
}
