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
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using Excel;

namespace VATClient
{
    public partial class FormProductCategorySearch : Form
    {
        #region Constructors

        public FormProductCategorySearch()
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
        private string SelectedValue = string.Empty;
        //public string VFIN = "306";

        #region Global Variables For BackGroundWorker

        private DataTable ProductCategoryResult;
        private string cmbActiveStatusValue = string.Empty;
        private string txtPTypeText = string.Empty;

        #endregion

        #endregion

        #region Methods

        public static string SelectOne()
        {
            string frmSearchSelectValue = String.Empty;

            #region try

            try
            {
                FormProductCategorySearch frmProductCategorySearch = new FormProductCategorySearch();
                frmProductCategorySearch.ShowDialog();
                return frmSearchSelectValue = frmProductCategorySearch.SelectedValue;
            }
            #endregion
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
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
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
            }

            #endregion

            return frmSearchSelectValue;
        }

        private void NullChecking()
        {
            if (txtVatRateFrom.Text == "")
            {
                txtVatRateFrom.Text = "0.00";
            }

            if (txtVatRateTo.Text == "")
            {
                txtVatRateTo.Text = "0.00";
            }
        }

        private void ClearAllFields()
        {
            txtCategoryID.Text = "";
            txtCategoryName.Text = "";
            txtHSCodeNo.Text = "";
            txtVatRate.Text = "0.00";
            cmbActive.SelectedIndex = -1;
            txtPType.Text = "";
            dgvProductCategories.Rows.Clear();
            txtVatRateFrom.Text = "0.00";
            txtVatRateTo.Text = "0.00";
        }

        private void GridSelected()
        {
            #region try

            try
            {
                if (dgvProductCategories.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (dgvProductCategories.Rows.Count > 0)
                {
                    string ProductCategoryInfo = string.Empty;
                    int ColIndex = dgvProductCategories.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvProductCategories.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        if (Program.fromOpen != "Me" &&
                            dgvProductCategories.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                        {
                            MessageBox.Show("This Selected Item is not Active");
                            return;
                        }
                        string CategoryID = dgvProductCategories.Rows[RowIndex1].Cells["CategoryID"].Value.ToString();
                        string CategoryName =
                            dgvProductCategories.Rows[RowIndex1].Cells["CategoryName"].Value.ToString();
                        string Description = dgvProductCategories.Rows[RowIndex1].Cells["Description"].Value.ToString();
                        string Comments = dgvProductCategories.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                        string ReportType = dgvProductCategories.Rows[RowIndex1].Cells["ReportType"].Value.ToString();
                        string IsRaw = dgvProductCategories.Rows[RowIndex1].Cells["IsRaw"].Value.ToString();
                        string HSCodeNo = dgvProductCategories.Rows[RowIndex1].Cells["HSCodeNo"].Value.ToString();
                        string VATRate = dgvProductCategories.Rows[RowIndex1].Cells["VATRate"].Value.ToString();
                        string PropergatingRate =
                            dgvProductCategories.Rows[RowIndex1].Cells["PropergatingRate"].Value.ToString();
                        string ActiveStatus =
                            dgvProductCategories.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString();
                        string HSDescription =
                            dgvProductCategories.Rows[RowIndex1].Cells["HSDescription"].Value.ToString();
                        string SD = dgvProductCategories.Rows[RowIndex1].Cells["SDRate"].Value.ToString();
                        string Trading = dgvProductCategories.Rows[RowIndex1].Cells["Trading"].Value.ToString();
                        string NonStock = dgvProductCategories.Rows[RowIndex1].Cells["NonStock"].Value.ToString();


                        ProductCategoryInfo = CategoryID + FieldDelimeter + CategoryName + FieldDelimeter + Description + FieldDelimeter +
                            Comments + FieldDelimeter + IsRaw + FieldDelimeter + HSCodeNo + FieldDelimeter + VATRate + FieldDelimeter +
                            PropergatingRate + FieldDelimeter + ActiveStatus + FieldDelimeter + HSDescription + FieldDelimeter +
                            SD + FieldDelimeter + Trading + FieldDelimeter + NonStock + FieldDelimeter + ReportType;

                        SelectedValue = ProductCategoryInfo;
                    }
                }
                this.Close();
            }
            #endregion
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

        private void Search()
        {
            NullChecking();
            //string ProductCategoryData = string.Empty;


            cmbActiveStatusValue = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
          

            #region try

            try
            {
                txtPTypeText = txtPType.Text.Trim();
                backgroundWorkerSearch.RunWorkerAsync();
            }
            #endregion
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

        #endregion

        private void dgvProductCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormProductCategorySearch_Load(object sender, EventArgs e)
        {
            #region try

            try
            {

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                Search();
            }
            #endregion
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormProductCategorySearch_Load",
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

                FileLogger.Log(this.Name, "FormProductCategorySearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load",
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
                FileLogger.Log(this.Name, "FormProductCategorySearch_Load", exMessage);
            }

            #endregion
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtCategoryID_KeyDown(object sender, KeyEventArgs e)
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

        private void cmbActiveStatus_KeyDown(object sender, KeyEventArgs e)
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

        private void txtHSCodeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVatRateFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVatRateTo_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void dgvProductCategories_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //No SOAP Service

            #region try

            try
            {
                //MDIMainInterface mdi = new MDIMainInterface();
                FormProductCategory frmProductCategory = new FormProductCategory();
                //mdi.RollDetailsInfo(frmProductCategory.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmProductCategory.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmProductCategory.Show();
            }
            #endregion
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

        private void txtVatRateFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVatRateFrom, "VAT Rate From");
        }

        private void txtVatRateTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVatRateTo, "VAT RateTo");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                LoadDataGrid();
               
                
            }
            #endregion
            #region catch

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

            #endregion

        }
        private void LoadDataGrid()
        {
            try
            {
                DataSet ds = new DataSet();
                string newPath = @"d:\SaleTBL.xlsx";
                FileStream stream = File.Open(newPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (newPath.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (newPath.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                reader.Close();
                int i = 1;
                 string customer = "";
                 string Date = "";
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    string customer1 = "";
                    string Date1 = "";
                    if (string.IsNullOrWhiteSpace(customer))
                    {
                        customer = item["CustomerName"].ToString();
                        Date = item["PostingDate"].ToString();
                        customer1 = item["CustomerName"].ToString();
                        Date1 = item["PostingDate"].ToString();
                    }
                    else
                    {
                        customer1 = item["CustomerName"].ToString();
                        Date1 = item["PostingDate"].ToString();
                    }
                    if (customer1 == customer && Date1 == Date)
                    {
                        item["Id"] = i.ToString();
                    }
                    else
                    {
                        i++;
                        item["Id"] = i.ToString();

                        customer = item["CustomerName"].ToString();
                        Date = item["PostingDate"].ToString();
                    }
                }

                OrdinaryVATDesktop.SaveExcel(ds.Tables[0], "Sale", "SaleM");
                 

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        //=========================================================================================
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.btnSearch.Enabled = false;
            txtVatRateFrom.Text = "";
            txtVatRateTo.Text = "";
            Search();

        }

        #region Background Worker Events

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                //string encriptedProductCategoryData = Converter.DESEncrypt(PassPhrase, EnKey, ProductCategoryData);

                //ProductCategorySoapClient ProductCategorySearch = new ProductCategorySoapClient();
                //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
                IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);

                //ProductCategoryResult = productCategoryDal.SearchProductCategory(txtCategoryID.Text.Trim(), txtCategoryName.Text.Trim(),txtPTypeText, cmbActiveStatusValue, Program.DatabaseName);
               
                string[] cValues = { txtCategoryID.Text.Trim(), txtCategoryName.Text.Trim(), txtPTypeText, cmbActiveStatusValue };
                string[] cFields = { "CategoryID like", "CategoryName like", "IsRaw like", "ActiveStatus like" };
                ProductCategoryResult = productCategoryDal.SelectAll(0, cFields, cValues, null, null, true,"",connVM);

            }
            #endregion

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

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            try
            {
                int j = 0;
                dgvProductCategories.Rows.Clear();
                //dgvProductCategories.DataSource = ProductCategoryResult;

                foreach (DataRow item in ProductCategoryResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductCategories.Rows.Add(NewRow);
                    dgvProductCategories.Rows[j].Cells["CategoryID"].Value = item["CategoryID"].ToString();// Convert.ToDecimal(ProductCategoryFields[0]);
                    dgvProductCategories.Rows[j].Cells["CategoryName"].Value = item["CategoryName"].ToString();// ProductCategoryFields[1].ToString();
                    dgvProductCategories.Rows[j].Cells["Description"].Value = item["Description"].ToString();// ProductCategoryFields[2].ToString();
                    dgvProductCategories.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// ProductCategoryFields[3].ToString();
                    dgvProductCategories.Rows[j].Cells["IsRaw"].Value = item["IsRaw"].ToString();// ProductCategoryFields[4].ToString();
                    dgvProductCategories.Rows[j].Cells["HSCodeNo"].Value = item["HSCodeNo"].ToString();// ProductCategoryFields[5].ToString();
                    dgvProductCategories.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();//Convert.ToDecimal(ProductCategoryFields[6].ToString()).ToString("0.00");
                    dgvProductCategories.Rows[j].Cells["PropergatingRate"].Value = item["PropergatingRate"].ToString();// ProductCategoryFields[7].ToString();
                    dgvProductCategories.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();// ProductCategoryFields[8].ToString();
                    dgvProductCategories.Rows[j].Cells["HSDescription"].Value = item["HSCodeNo"].ToString();// ProductCategoryFields[9].ToString();
                    dgvProductCategories.Rows[j].Cells["SDRate"].Value = item["SD"].ToString();//Convert.ToDecimal(ProductCategoryFields[10].ToString()).ToString("0.00");
                    dgvProductCategories.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();// ProductCategoryFields[11].ToString();
                    dgvProductCategories.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();// ProductCategoryFields[12].ToString();
                    dgvProductCategories.Rows[j].Cells["ReportType"].Value = item["ReportType"].ToString();// ProductCategoryFields[13].ToString();
                    j = j + 1;
                }
                dgvProductCategories.Columns["Description"].Visible = false;
                dgvProductCategories.Columns["Comments"].Visible = false;
                dgvProductCategories.Columns["HSCodeNo"].Visible = false;
                dgvProductCategories.Columns["PropergatingRate"].Visible = false;
                dgvProductCategories.Columns["HSDescription"].Visible = false;
            }
            #endregion

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

            finally
            {
                LRecordCount.Text = "Record Count: " + dgvProductCategories.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            ds.Tables.Add(ProductCategoryResult);
            string xml = ds.GetXml();
            string path = @"d:\pCategory.xml";
            ds.WriteXml(path);

            StringReader theReader = new StringReader(xml);
            DataSet theDataSet = new DataSet();
            theDataSet.ReadXml(theReader);
        }
    }
}