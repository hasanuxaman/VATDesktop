using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATClient.ReportPages;
using VATClient.ReportPreview;
using VATClient.ModelDTO;
using VATViewModel.DTOs;
using VATServer.License;
using SymphonySofttech.Reports;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using System.Text.RegularExpressions;

namespace VATClient
{
    public partial class FormDutyDrawBack : Form
    {
        public FormDutyDrawBack()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        List<ProductMiniDTO> ProductsMiniS = new List<ProductMiniDTO>();
        List<ProductbySaleInvoiceDTO> ProductsMini = new List<ProductbySaleInvoiceDTO>();

        List<ProductbyPurchaseInvoiceDTO> PurchaseMini = new List<ProductbyPurchaseInvoiceDTO>();

        List<ProductCmbDTO> productCmb = new List<ProductCmbDTO>();

        private DDBHeaderVM ddbackMaster = new DDBHeaderVM();
        private List<DDBDetailsVM> ddbackDetails = new List<DDBDetailsVM>();
        private List<DDBSaleInvoicesVM> ddbSaleInvoicesVM = new List<DDBSaleInvoicesVM>();

        private DataTable ddbackDetailResult;


        private DDBHeaderVM MasterVM;
        private DataSet ReportResult;
        private DataTable ReportResultDt;
        private ReportDocument reportDocument = new ReportDocument(); 

        private List<DDBDetailsVM> DetailVMs = new List<DDBDetailsVM>();



        List<UomDTO> UOMs = new List<UomDTO>();
        List<CurrencyVM> country = new List<CurrencyVM>();
        List<Currency> currency = new List<Currency>();

        #region variable

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string HSCodeNoParam = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";
        private bool isTrnsTypeImport = false;
        private string vPTransType = string.Empty;

        private string varSalesInvoiceNo = string.Empty;
        private string varTrading = string.Empty;

        private string customerCode = string.Empty;
        private string customerName = string.Empty;
        private string customerGId = string.Empty;
        private string customerGName = string.Empty;
        private string tinNo = string.Empty;
        private string customerVReg = string.Empty;
        private string activeStatusCustomer = string.Empty;
        private string CGType = "Local";

        private DataTable CustomerResult;

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;

        private string vehicleId = string.Empty;
        private string vehicleType = string.Empty;
        private string vehicleNo = string.Empty;
        private string vehicleActiveStatus = string.Empty;
        private DataTable vehicleResult;

        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private DataTable uomResult;
        private DataTable CurrencyConversionResult;

        private string ExportData = string.Empty;
        private DataTable ExportResult;

        private string TenderId = "0";

        private string PurchaseDetailData;

        private DataTable TenderSearchResult;


        private DataTable ProductResultDs;

        private DataTable PurchaseResultDs;

        private DataTable PurchaseDetailResult;

        private DataTable ProductResultDsInvoice;

        private DataTable CountryResultDs;
        private DataTable CurrencyResultDs;

        private string ddbackDetailData = string.Empty;


        private DataSet ReportResultVAT11;
        private DataSet ReportResultVAT20;
        private DataSet ReportResultCreditNote;
        private DataSet ReportResultDebitNote;

        private bool ChangeData = false;
        private bool Post = false;
        private bool IsPost = false;
        private bool PreviewOnly = false;
        private bool IsUpdate = false;
        private string NextID = "";
        private decimal PreVATAmount = 0;
        private string item;
        private decimal cost;

        private string post1, post2;

        private string CategoryId { get; set; }


        private DataTable SaleDetailResult;

        private string SaleDetailData = string.Empty;
        private DateTime varInvoiceDate;


        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        private string ProductData;

        private string Totalprice;
        private string SearchBranchId = "0";
        string transactionType = string.Empty;

        #endregion variable


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnOldSale_Click_1(object sender, EventArgs e)
        {
            cmbProductCode.Text = "";
            cmbProductCode.Items.Clear();
            cmbProduct.Text = "";
            cmbProduct.Items.Clear();
            txtUOM.Text = "";
            txtPkSize.Text = "";
            txtQuantity.Text = "";
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtCountry.Text = "";
            txtCurrencyName.Text = "";
            txtExpCurr.Text = "";
            txtTotalPrice.Text = "";


            try
            {
                #region Statement

                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormSaleSearch frm = new FormSaleSearch();
                //mdi.RollDetailsInfo("303");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}

                Program.fromOpen = "Me";
                Program.SalesType = "New";
                DataGridViewRow selectedRow = FormSaleSearch.SelectOne("Export");


                if (selectedRow != null && selectedRow.Selected == true)
                {
                    //if (selectedRow.Cells["Post"].Value.ToString() == "N" || selectedRow.Cells["Isprint"].Value.ToString() == "N")
                    //{
                    //    MessageBox.Show("this transaction was not posted or not printed", this.Text);
                    //    return;
                    //}

                    if (selectedRow.Cells["IsPrint"].Value.ToString() == "Y")
                    {
                        txtOldSaleId.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();

                        txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                        txtCustomerId.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                        txtAddress.Text = selectedRow.Cells["DeliveryAddress1"].Value.ToString();

                        dptSalesDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());

                        txtCurrencyName.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                        txtExpCurrId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();

                        this.progressBar1.Visible = true;


                        //bgwSaleSearch.RunWorkerAsync();


                        //----------------------

                        //ruba
                        Customercountry(selectedRow.Cells["CustomerName"].Value.ToString());
                        Customercurrency(selectedRow.Cells["SalesInvoiceNo"].Value.ToString());
                        txtOldSaleId.Text = "'" + txtOldSaleId.Text.Trim() + "'";

                        ProductSearchDsFormLoad();
                    }
                    else
                    {
                        MessageBox.Show("You need to select only Printed");
                    }
                }
                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnOldID_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnOldID_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnOldID_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnOldID_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnOldID_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOldID_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOldID_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnOldID_Click", exMessage);
            }

            #endregion
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {

            //---------------
            #region UOM
            //UOMDAL uomdal = new UOMDAL();
            IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

            string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
            string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
            uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false,connVM);

            //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

            #endregion UOM

            //------------------


            #region Product
            ProductResultDs = new DataTable();
            ProductDAL productDal = new ProductDAL();
            ////IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
            ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam,connVM);

            #endregion Product
        }
        private void FormLoad()
        {
            rbtnCode.Checked = true;
            rbtnPurCode.Checked = true;

            #region Customer
            //CGType = "Local";
            //if (rbtnExport.Checked)
            //{
            //    CGType = "Export";
            //}
            CGType = string.Empty;
            customerCode = string.Empty;
            customerName = string.Empty;
            customerGId = string.Empty;
            customerGName = string.Empty;
            tinNo = string.Empty;
            customerVReg = string.Empty;
            activeStatusCustomer = string.Empty;
            #endregion Customer

            #region vehicle
            vehicleId = string.Empty;
            vehicleType = string.Empty;
            vehicleNo = string.Empty;
            vehicleActiveStatus = string.Empty;
            #endregion vehicle

            #region SetupVATStatus
            varInvoiceDate = Convert.ToDateTime(dptSalesDate.MaxDate.ToString("yyyy-MMM-dd"));

            #endregion SetupVATStatus

            #region Product
            ItemNoParam = string.Empty;
            CategoryIDParam = string.Empty;
            IsRawParam = string.Empty;
            HSCodeNoParam = string.Empty;
            ActiveStatusProductParam = string.Empty;
            TradingParam = string.Empty;
            NonStockParam = string.Empty;
            ProductCodeParam = string.Empty;

            if (CategoryId != null)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = CategoryId;
                IsRawParam = string.Empty;
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                //txtCategoryName.Text = "Finish";

            }//start

            #endregion Product


            #region UOM
            UOMIdParam = string.Empty;
            UOMFromParam = string.Empty;
            UOMToParam = string.Empty;
            ActiveStatusUOMParam = string.Empty;

            UOMIdParam = string.Empty;
            UOMFromParam = string.Empty;
            UOMToParam = string.Empty;
            ActiveStatusUOMParam = "Y";
            #endregion UOM

            #region CurrencyConversion

            #endregion CurrencyConversion
            
            #region Setting
            CommonDAL commonDal = new CommonDAL();
            Totalprice = commonDal.settingsDesktop("Purchase", "Totalprice",null,connVM);
            #endregion Setting

        }

        private void FormDutyDrawBack_Load(object sender, EventArgs e)
        {

            FormLoad();
            ClearAllFields();
            ChangeData = false;
            TransactionTypes();
            FormMaker();
            //-------------------
            bgwLoad.RunWorkerAsync();



        }
        private void FormMaker()
        {
            
                label50.Visible = false;
                txtApprovedSD.Visible = false;
                label51.Visible = false;
                txtSubTotalSD.Visible = false;
                btnPreview.Visible = false;
           
            if (rbtnOther.Checked)
            {
                this.Text = "DDB";
            }
            else if (rbtnVDB.Checked)
            {
                this.Text = "VAT Adjustment";
                label1.Text = "VDB No";
                label9.Text = "VDB Date";
            }
            else if (rbtnSDB.Checked)
            {
                this.Text = "SD Adjustment";
                label1.Text = "SDB No";
                label9.Text = "SDB Date";
                label50.Visible = true;
                txtApprovedSD.Visible = true;
                label51.Visible = true;
                txtSubTotalSD.Visible = true;
                btnPreview.Visible = true;
            }
        }
        private void TransactionTypes()
        {
            #region Transaction Type
            transactionType = string.Empty;
            if (rbtnOther.Checked)
            {
                transactionType = "DDB";
            }
            else if (rbtnVDB.Checked)
            {
                transactionType = "VDB";
            }
            else if (rbtnSDB.Checked)
            {
                transactionType = "SDB";
            }
            #endregion Transaction Type
        }

        private void Uoms()
        {
            try
            {
                #region Statement

                string uOMFrom = txtPurUom.Text.Trim().ToLower();
                cmbUom.Items.Clear();
                if (UOMs != null && UOMs.Any())
                {

                    var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom).ToList()
                               select uom.UOMTo;

                    if (uoms != null && uoms.Any())
                    {
                        cmbUom.Items.AddRange(uoms.ToArray());
                        cmbUom.Items.Add(txtPurUom.Text.Trim());
                        //txtUomConv.Text = uoms.First().ToString();
                    }
                }
                cmbUom.Text = txtPurUom.Text.Trim();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Uoms", exMessage);
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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }

            #endregion

        }

        public void Customercurrency(string SalesInvoice)
        {
            txtExpCurr.Text = "";
            txtTotalPrice.Text = "";

            //CurrenciesDAL dd = new CurrenciesDAL();
            ICurrencies dd = OrdinaryVATDesktop.GetObject<CurrenciesDAL, CurrenciesRepo, ICurrencies>(OrdinaryVATDesktop.IsWCF);
            CurrencyResultDs = dd.SearchCurrency(SalesInvoice,connVM);
            currency.Clear();

            foreach (DataRow item2 in CurrencyResultDs.Rows)
            {
                var coun = new Currency();
                coun.SubTotal = Convert.ToDecimal(item2["SubTotal"].ToString());
                txtExpCurr.Text = item2["SubTotal"].ToString();
                coun.CurrencyValue = Convert.ToDecimal(item2["CurrencyValue"].ToString());
                txtTotalPrice.Text = item2["CurrencyValue"].ToString();
                currency.Add(coun);
            }

            if (txtCurrencyName.Text == "BDT")
            {
                txtTotalPrice.Text = txtExpCurr.Text;

            }

        }


        public void Customercountry(string Customer)
        {
            txtCountry.Text = "";
            //CustomerDAL dd = new CustomerDAL();
            ICustomer dd = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);
            CountryResultDs = dd.SearchCountry(Customer,connVM);
            country.Clear();

            foreach (DataRow item2 in CountryResultDs.Rows)
            {

                var coun = new CurrencyVM();
                coun.Country = item2["Country"].ToString();
                txtCountry.Text = item2["Country"].ToString();
                country.Add(coun);
            }

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region UOM
            UOMs.Clear();
            cmbUom.Items.Clear();
            foreach (DataRow item2 in uomResult.Rows)
            {
                var uom = new UomDTO();
                uom.UOMId = item2["UOMId"].ToString();
                uom.UOMFrom = item2["UOMFrom"].ToString();
                uom.UOMTo = item2["UOMTo"].ToString();
                uom.Convertion = Convert.ToDecimal(item2["Convertion"].ToString());
                uom.CTypes = item2["CTypes"].ToString();
                cmbUom.Items.Add(item2["UOMTo"].ToString());

                UOMs.Add(uom);

            }
            #endregion UOM

        }

        private void bgwSaleSearch_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void bgwSaleSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }





        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                // txtCategoryName.Text = ProductCategoryInfo[1];
                // cmbProductType.Text = ProductCategoryInfo[4];


                // ProductSearchDsFormLoad();



                ////UOMSearch();

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button1_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button1_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button1_Click_1", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click_1", exMessage);
            }
            #endregion
        }

        private void ProductSearchDsFormLoad()
        {
            ProductData = string.Empty;
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            try
            {
                ProductResultDs = productDal.SearchProductbyMultipleSaleInvoice(txtOldSaleId.Text,connVM);
                foreach (DataRow item2 in ProductResultDs.Rows)
                {

                    //var prod = new ProductMiniDTO();
                    //var prodcmb = new ProductCmbDTO();

                    var prod = new ProductbySaleInvoiceDTO();

                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();

                    prod.UOM = item2["UOM"].ToString();
                    cmbProductCode.Items.Add(item2["ProductCode"]);
                    cmbProduct.Items.Add(item2["ProductName"]);
                    prod.Quantity = Convert.ToDecimal(item2["Quantity"]);



                    ProductsMini.Add(prod);
                }
                txtOldSaleId.Text = txtOldSaleId.Text.ToString().Replace("'", "");


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            #endregion

        }
        private void bgwLoadProduct_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductbyMultipleSaleInvoice(txtOldSaleId.Text,connVM);



                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwLoadProduct_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                #region Product

                ProductsMini.Clear();
                ProductsMiniS.Clear();
                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                productCmb.Clear();




                foreach (DataRow item2 in ProductResultDs.Rows)
                {

                    //var prod = new ProductMiniDTO();
                    //var prodcmb = new ProductCmbDTO();

                    var prod = new ProductbySaleInvoiceDTO();

                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();

                    prod.UOM = item2["UOM"].ToString();
                    cmbProductCode.Items.Add(item2["ProductCode"]);
                    cmbProduct.Items.Add(item2["ProductName"]);
                    prod.Quantity = Convert.ToDecimal(item2["Quantity"]);



                    ProductsMini.Add(prod);
                }
                #endregion Product
                txtOldSaleId.Text = txtOldSaleId.Text.ToString().Replace("'", "");


                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                cmbProduct.Enabled = true;
                cmbProductCode.Enabled = true;

                this.progressBar1.Visible = false;
            }

        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {
            cmbProductCode.Enabled = true;
            cmbProduct.Enabled = false;
        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            cmbProductCode.Enabled = false;
            cmbProduct.Enabled = true;
        }

        private void cmbProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                //txtQuantity.Focus();

                //btnPurSearch.Focus();
            }
        }

        private void cmbProductCode_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtProductCode.Text = "";

                var searchText = cmbProductCode.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     where prd.ProductCode.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        txtProductName.Text = products.ProductName;
                        cmbProduct.Text = products.ProductName;
                        txtProductCode.Text = products.ItemNo;
                        txtFgItem.Text = products.ItemNo;

                        txtUOM.Text = products.UOM;

                        if (rbtnTollReceive.Checked)
                        {
                            NBRPriceCall();
                        }
                        else if (rbtnPurchaseReturn.Checked)
                        {
                            //txtUnitCost.Text = products.IssuePrice.ToString();
                        }

                        txtPCode.Text = products.ProductCode;
                        cmbProductCode.Text = products.ProductCode;
                        txtQuantity.Text = products.Quantity.ToString();
                        //txtNBRPrice.Text = products.NBRPrice.ToString();//ToString("0,0.00");
                        //txtBOMPrice.Text = products.BOMPrice.ToString();//ToString("0,0.00");
                        txtPkSize.Text = products.UOM.ToString();

                        ProductDAL productDal = new ProductDAL();


                        //MessageBox.Show("Stock Quantity: " + stockInHand, this.Text);

                        //Uoms();
                    }
                    else
                    {

                        MessageBox.Show("Please select the right item", this.Text);
                        cmbProductCode.Text = "Select";
                    }
                }

                //cmbUom.Focus();

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            #endregion
        }
        private void NBRPriceCall()
        {
            ProductDAL productDal = new ProductDAL();
            if (rbtnTollReceive.Checked)
            {
                //txtUnitCost.Text = productDal.GetLastTollChargeFromBOMOH(txtProductName.Text.Trim(), "VAT 4.3 (Toll Receive)", Convert.ToDateTime(dtpReceiveDate.Value.ToString("yyyy-MMM-dd")), null, null).ToString();

            }

        }


        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string item = cmbProduct.Text;
            //string[] filteredItems = items.Where(x => x.Contains(item)).ToArray();
            //cmbProduct.Items.Clear();
            //cmbProduct.Items.AddRange(filteredItems);
            //cmbProduct.SelectionStart = item.Length;
            //cmbProduct.DroppedDown = true;
            ChangeData = true;
            //ProductDetailsInfo();
        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtQuantity.Focus();
            }
        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtProductCode.Text = "";
                var searchText = cmbProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     where prd.ProductName.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        txtProductName.Text = products.ProductName;
                        txtProductCode.Text = products.ItemNo;
                        txtFgItem.Text = products.ItemNo;
                        txtUOM.Text = products.UOM;

                        //txtUnitCost.Text = "0.00";

                        if (rbtnTollReceive.Checked)
                        {
                            NBRPriceCall();

                        }

                        else if (rbtnPurchaseReturn.Checked)
                        {




                            // txtUnitCost.Text = products.IssuePrice.ToString();
                        }

                        //ProductDAL productDal = new ProductDAL();
                        //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                        txtPCode.Text = products.ProductCode;
                        cmbProductCode.Text = products.ProductCode;
                        txtQuantity.Text = products.Quantity.ToString();
                        txtPkSize.Text = products.UOM.ToString();
                        //txtNBRPrice.Text = products.NBRPrice.ToString();//ToString("0,0.0000");
                        // txtBOMPrice.Text = products.BOMPrice.ToString();//ToString("0,0.0000");
                        //Uoms();
                    }
                    else
                    {
                        ProductManualSearchUseCode();
                        MessageBox.Show("Please select the right item", this.Text);
                        cmbProductCode.Text = "Select";
                    }
                }


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            }
            #endregion
        }




        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPurSearch_Click(object sender, EventArgs e)
        {
            #region clear
            chkCategory.Checked = false;
            cmbPurProductCode.Text = "";
            cmbPurProductCode.Items.Clear();
            cmbPurProductName.Text = "";
            cmbPurProductName.Items.Clear();
            txtPurQuantity.Text = "";
            txtPurUnitPrice.Text = "";
            txtPurUom.Text = "";
            txtUsedQty.Text = "";
            txtSubTotal.Text = "";
            txtCd.Text = "";
            txtRd.Text = "";
            txtSd.Text = "";
            txtVat.Text = "";

            dptPurNoDate.Enabled = true;
            txtBeNo.ReadOnly = false;
            lblUnitPrice.Text = "Unit Price";
            txtPurQuantity.ReadOnly = true;
            txtPurUnitPrice.ReadOnly = true;
            txtPurUom.ReadOnly = true;
            txtUsedQty.ReadOnly = false;
            txtSubTotal.ReadOnly = true; //AV
            txtCd.ReadOnly = true;
            txtRd.ReadOnly = true;
            txtSd.ReadOnly = true;
            txtVat.ReadOnly = true;
            dptPurNoDate.Enabled = false;
            txtBeNo.ReadOnly = true;
            #endregion clear



            try
            {
                //  dutyCalculate = false;

                #region Statement

                string result;
                Program.fromOpen = "Me";
                #region TransactionType
                DataGridViewRow selectedRow = FormPurchaseSearch.SelectOne("All");
                //DataGridViewRow selectedRow = FormPurchaseSearch.SelectTwo("All");

                #endregion TransactionType

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    if (selectedRow.Cells["post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("You need to select only Posted");
                    }
                    else if (Convert.ToDateTime(dptSalesDate.Value.ToString("yyyy-MMM-dd")) < Convert.ToDateTime(Convert.ToDateTime(selectedRow.Cells["ReceiveDate"].Value).ToString("yyyy-MMM-dd")))
                    {
                        MessageBox.Show("Export Invoice date not before Purchase date", this.Text);
                        return;
                    }
                    else
                    {
                        txtPurNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                        txtBeNo.Text = selectedRow.Cells["BENumber"].Value.ToString();
                        vPTransType = selectedRow.Cells["TransactionType"].Value.ToString();
                        if (vPTransType == "Import" || vPTransType == "TradingImport")
                        {
                            isTrnsTypeImport = true;

                        }
                        else
                        {
                            isTrnsTypeImport = false;
                        }
                        dptPurNoDate.Value = Convert.ToDateTime(selectedRow.Cells["ReceiveDate"].Value.ToString());
                        if (txtPurNo.Text == "")
                        {
                            PurchaseDetailData = "00";
                        }
                        else
                        {
                            PurchaseDetailData = txtPurNo.Text.Trim();
                        }
                        bgwSearchPurchase.RunWorkerAsync();
                        //if (rbtnTradingImport.Checked || rbtnImport.Checked)
                        //{ bgwDutiesSearch.RunWorkerAsync(); }
                        this.btnPurSearch.Enabled = true;
                        this.progressBar1.Visible = true;
                    }

                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion


        }


        private void ProductSearchDsPurch()
        {
            ProductData = string.Empty;

            try
            {


                cmbPurProductName.Enabled = false;
                cmbPurProductCode.Enabled = false;
                btnPurSearch.Enabled = true;

                this.progressBar1.Visible = true;
                bgwLoadProductbyInvoice.RunWorkerAsync();

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            #endregion

        }

        private void bgwLoadProductbyInvoice_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                HSCodeNoParam = String.Empty;
                ProductResultDsInvoice = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam,connVM);


                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoadProduct_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwLoadProductbyInvoice_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                #region Product

                ProductsMiniS.Clear();
                //ProductsMini.Clear();
                cmbPurProductName.Items.Clear();
                cmbPurProductCode.Items.Clear();
                productCmb.Clear();
                foreach (DataRow item2 in ProductResultDsInvoice.Rows)
                {

                    var prod = new ProductMiniDTO();
                    var prodcmb = new ProductCmbDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();

                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();

                    cmbPurProductCode.Items.Add(item2["ProductCode"]);
                    cmbPurProductName.Items.Add(item2["ProductName"]);


                    prod.CategoryID = item2["CategoryID"].ToString();
                    prod.CategoryName = item2["CategoryName"].ToString();
                    prod.UOM = item2["UOM"].ToString();
                    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                    prod.IsRaw = item2["IsRaw"].ToString();

                    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
                    prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
                    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                    prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
                    prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
                    prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());

                    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());



                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    prod.RebatePercent = Convert.ToDecimal(item2["RebatePercent"].ToString());
                    //prod.BOMPrice = Convert.ToDecimal(item2["BOMPrice"].ToString());
                    ProductsMiniS.Add(prod);
                }
                #endregion Product


                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                cmbPurProductName.Enabled = true;
                cmbPurProductCode.Enabled = true;
                btnPurSearch.Enabled = true;
                rbtnCode.Checked = true;
                this.progressBar1.Visible = false;
            }
        }

        private void cmbPurProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////string item = cmbProduct.Text;
            ////string[] filteredItems = items.Where(x => x.Contains(item)).ToArray();
            ////cmbProduct.Items.Clear();
            ////cmbProduct.Items.AddRange(filteredItems);
            ////cmbProduct.SelectionStart = item.Length;
            ////cmbProduct.DroppedDown = true;
            //ChangeData = true;
            ////ProductDetailsInfo();
        }

        private void cmbPurProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter))
            //{
            //    txtPurQuantity.Focus();
            //}
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (chkCategory.Checked)
                {
                    txtPurQuantity.Focus();
                }
                else
                {
                    txtUsedQty.Focus();
                }

            }
        }

        private void cmbPurProductCode_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtPurItem.Text = "";
                var searchText = cmbPurProductCode.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in PurchaseMini.ToList()
                                     where prd.ProductCode.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        cmbPurProductName.Text = products.ProductName;
                        cmbPurProductCode.Text = products.ProductCode;
                        txtPurUom.Text = products.UOM;
                        //cmbUom.Text = products.UOM;
                        txtPurUnitPrice.Text = products.CostPrice.ToString();
                        txtPurQuantity.Text = products.Quantity.ToString();
                        txtSubTotal.Text = products.SubTotal.ToString();
                        txtCd.Text = products.CD.ToString();
                        txtSd.Text = products.SD.ToString();
                        txtRd.Text = products.RD.ToString();
                        txtVat.Text = products.VATAmount.ToString();
                        txtCnFAmount.Text = products.CnFAmount.ToString();
                        txtInsuranceAmount.Text = products.InsuranceAmount.ToString();
                        txtTVAAmount.Text = products.TVAAmount.ToString();
                        txtTVBAmount.Text = products.TVBAmount.ToString();
                        txtATVAmount.Text = products.ATVAmount.ToString();
                        txtOthersAmount.Text = products.OthersAmount.ToString();
                        txtPurItem.Text = products.ItemNo.ToString();

                        //GetBomIdFromBOM();
                        //UseQtyFromBOM();
                        //txtUsedQty.Focus();

                    }
                    else
                    {
                        bool isManualEntry = ProductManualSearchUseCode();
                        if (!isManualEntry)
                        {
                            MessageBox.Show("Please select the right item", this.Text);
                            cmbPurProductCode.Text = "Select";
                        }

                    }
                }

                txtUsedQty.Text = "00.00";

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    GetBomIdFromBOM();
                    UseQtyFromBOM();
                    Uoms();
                }



                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            #endregion
        }

        private void cmbPurProductName_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (chkCategory.Checked)
                {
                    txtPurQuantity.Focus();
                }
                else
                {
                    txtUsedQty.Focus();
                }

            }
        }

        private void cmbPurProductName_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtPurItem.Text = "";

                var searchText = cmbPurProductName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in PurchaseMini.ToList()
                                     where prd.ProductName.Trim().ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        cmbPurProductName.Text = products.ProductName;
                        cmbPurProductCode.Text = products.ProductCode;
                        txtPurUom.Text = products.UOM;
                        //cmbUom.Text = products.UOM;
                        txtPurQuantity.Text = products.Quantity.ToString();
                        txtPurUnitPrice.Text = products.CostPrice.ToString();
                        txtSubTotal.Text = products.SubTotal.ToString();
                        txtCd.Text = products.CD.ToString();
                        txtSd.Text = products.SD.ToString();
                        txtRd.Text = products.RD.ToString();
                        txtVat.Text = products.VATAmount.ToString();
                        txtCnFAmount.Text = products.CnFAmount.ToString();
                        txtInsuranceAmount.Text = products.InsuranceAmount.ToString();
                        txtTVAAmount.Text = products.TVAAmount.ToString();
                        txtTVBAmount.Text = products.TVBAmount.ToString();
                        txtATVAmount.Text = products.ATVAmount.ToString();
                        txtOthersAmount.Text = products.OthersAmount.ToString();
                        txtPurItem.Text = products.ItemNo.ToString();

                        //GetBomIdFromBOM();
                        //UseQtyFromBOM();
                        //Uoms(); 

                    }
                    else
                    {
                        bool isManualEntry = ProductManualSearchUseName();
                        if (!isManualEntry)
                        {
                            MessageBox.Show("Please select the right item", this.Text);
                            cmbPurProductName.Text = "Select";
                        }
                    }
                }
                txtUsedQty.Text = "00.00";
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    GetBomIdFromBOM();
                    UseQtyFromBOM();
                    Uoms();
                }



                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            #endregion
        }
        private void GetBomIdFromBOM()
        {
            string vPurItem = "0";
            string vFinItem = "0";
            if (string.IsNullOrEmpty(txtProductCode.Text.Trim()))
            {
                vFinItem = "0";

            }
            else
            {
                vFinItem = txtProductCode.Text.Trim();

            }
            if (string.IsNullOrEmpty(txtPurItem.Text.Trim()))
            {
                vPurItem = "0";
            }
            else
            {
                vPurItem = txtPurItem.Text.Trim();

            }
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
            string tt = productDal.GetBomIdFromBOM(vFinItem, vPurItem,
                                                                   "VAT 4.3", // if not then vat1
                                                                   Convert.ToDateTime(
                                                                       dptSalesDate.Value.ToString("yyyy-MMM-dd")),
                                                                   null, null,connVM).ToString();
            if (tt == "0")
            {
                MessageBox.Show("This Purchase Item('" + cmbPurProductName.Text.Trim() + "') not use for this Export Item('" + cmbProduct.Text.Trim() + "')", this.Text);
            }

        }
        private void UseQtyFromBOM()
        {
            string vPurItem = "0";
            string vFinItem = "0";
            if (string.IsNullOrEmpty(txtProductCode.Text.Trim()))
            {
                vFinItem = "0";

            }
            else
            {
                vFinItem = txtProductCode.Text.Trim();

            }
            if (string.IsNullOrEmpty(txtPurItem.Text.Trim()))
            {
                vPurItem = "0";
            }
            else
            {
                vPurItem = txtPurItem.Text.Trim();

            }
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            DateTime SalesDate = Convert.ToDateTime(dptSalesDate.Value.ToString("yyyy-MMM-dd"));
            
            txtUsedQty.Text = productDal.GetLastUseQuantityFromBOM(vFinItem, vPurItem, "VAT 4.3",
                                                                   Convert.ToDateTime( dptSalesDate.Value.ToString("yyyy-MMM-dd")),
                                                                   null, null,connVM).ToString();
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) || txtQuantity.Text.Trim() == "0")
            {
                txtUsedQty.Text = "0";
            }
            else
            {
                txtUsedQty.Text = Convert.ToDecimal(Convert.ToDecimal(txtUsedQty.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim())).ToString();
            }

        }
        private void UomsValue()
        {
            try
            {
                #region Statement

                string uOMFrom = txtPurUom.Text.Trim().ToLower();
                string uOMTo = cmbUom.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    txtUomConv.Text = "0";
                    if (uOMFrom == uOMTo)
                    {
                        txtUomConv.Text = "1";
                        //txtQuantity.Focus();
                        return;
                    }

                    else if (UOMs != null && UOMs.Any())
                    {

                        var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom && x.UOMTo.Trim().ToLower() == uOMTo).ToList()
                                   select uom.Convertion;
                        if (uoms != null && uoms.Any())
                        {
                            txtUomConv.Text = uoms.First().ToString();
                        }
                        else
                        {
                            MessageBox.Show("Please select the UOM", this.Text);
                            txtUomConv.Text = "0";
                            return;
                        }
                    }
                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "UomsValue", exMessage);
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
                FileLogger.Log(this.Name, "UomsValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UomsValue", exMessage);
            }
            #endregion

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {

            try
            {
                #region Null Ckeck

                if ((txtProductCode.Text.Trim() == ""))
                {
                    MessageBox.Show("Please select a Finish Item");
                    cmbProductCode.Focus();
                    return;
                }

                else if ((txtPurItem.Text.Trim() == ""))
                {
                    MessageBox.Show("Please select a Purchase Item");
                    cmbPurProductCode.Focus();
                    return;
                }
                else if (cmbPurProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item");
                    cmbPurProductCode.Focus();
                    return;
                }
                else if (cmbPurProductName.Text == "")
                {
                    MessageBox.Show("Please select a Product");
                    cmbPurProductName.Focus();
                    return;
                }
                else if (txtUsedQty.Text == "")
                {
                    MessageBox.Show("Please insert quantity");
                    txtUsedQty.Focus();
                    return;
                }
                else if (txtUsedQty.Text != "")
                {
                    bool isQtyMore = CalculateSingleQtyEntry();
                    if (isQtyMore)
                    {
                        MessageBox.Show("Used Quantity Should not more than Purchase Quantity");
                        txtUsedQty.Focus();
                        return;
                    }

                }
                else if (txtPurUom.Text == "")
                {
                    MessageBox.Show("Please select UOM");
                    txtPurUom.Focus();
                    return;
                }

                else if (txtPurQuantity.Text == "")
                {
                    txtQuantity.Text = "0";
                }

                else if (txtSubTotal.Text == "")
                {
                    MessageBox.Show("Please select Total Price");
                    txtSubTotal.Focus();
                    return;
                }
                else if (txtCd.Text == "")
                {
                    txtCd.Text = "0.00";
                }
                else if (txtRd.Text == "")
                {
                    txtRd.Text = "0.00";
                }

                else if (txtSd.Text == "")
                {
                    txtSd.Text = "0.00";
                }
                else if (txtSd.Text == "")
                {
                    txtSd.Text = "0.00";
                }

                //else if (txtUsedQty.Text == "")
                //{
                //    txtUsedQty.Text = "0.00";
                //}

                #endregion

                #region CheckNextDDB
                UomsValue();
                if (!chkCategory.Checked)
                {

                    decimal vPurchaseQty = 0;
                    decimal vTotalDDBQty = 0;
                    decimal vUomc = Convert.ToDecimal(txtUomConv.Text.Trim());
                    decimal vUsedQty = Convert.ToDecimal(txtUsedQty.Text.Trim());
                    //DutyDrawBackDAL ddDal = new DutyDrawBackDAL();
                    //DataTable dt = new DataTable();
                    //dt = ddDal.Purchase_DDBQty(txtPurNo.Text.Trim(), txtPurItem.Text.Trim(), null, null);
                    //if (dt != null)
                    //{
                    //    foreach (DataRow item in dt.Rows)
                    //    {
                    //        vPurchaseQty = Convert.ToDecimal(item["PurchaseQty"].ToString());
                    //        vTotalDDBQty = Convert.ToDecimal(item["TotalDDBQty"].ToString());

                    //    }
                    //}
                    //if (vPurchaseQty < vTotalDDBQty + (vUsedQty * vUomc))
                    //{
                    //    MessageBox.Show("DDB of this Purchase for This Item Already Complete", this.Text);
                    //    return;
                    //}
                }
                else
                {
                    isTrnsTypeImport = true;
                }

                #endregion CheckNextDDB

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    if (dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString().Trim() != txtOldSaleId.Text.Trim())
                    {
                        MessageBox.Show("More Sale Information cant save in a single DDB.", this.Text);
                        txtOldSaleId.Focus();
                        return;
                    }


                }

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    if (dgvSale.Rows[i].Cells["FGCode"].Value.ToString() == cmbProductCode.Text && dgvSale.Rows[i].Cells["ProductCode"].Value.ToString() == cmbPurProductCode.Text)
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        cmbPurProductCode.Focus();
                        return;
                    }



                }

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvSale.Rows.Add(NewRow);

                //for (int i = 0; i < dgvSale.RowCount; i++)
                //{
                //    // dgvSale["LineNo", dgvSale.RowCount - 1].Value = dgvSale[0, i].Value = i + 1;
                //    dgvSale["LineNo", dgvSale.RowCount - 1].Value = i + 1;
                //   // SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["Quantity",i].Value);


                //}


                dgvSale["FgItemNo", dgvSale.RowCount - 1].Value = txtFgItem.Text.Trim();
                dgvSale["ItemNo", dgvSale.RowCount - 1].Value = txtPurItem.Text.Trim();
                dgvSale["FGCode", dgvSale.RowCount - 1].Value = cmbProductCode.Text.Trim();
                dgvSale["FGName", dgvSale.RowCount - 1].Value = cmbProduct.Text.Trim();
                if (!chkCategory.Checked)
                {
                    dgvSale["PurchaseIdNo", dgvSale.RowCount - 1].Value = txtPurNo.Text.Trim();
                }
                else
                {
                    dgvSale["PurchaseIdNo", dgvSale.RowCount - 1].Value = "-";

                }
                dgvSale["PurchaseDate", dgvSale.RowCount - 1].Value = dptPurNoDate.Text.Trim();
                dgvSale["BillOfEntry", dgvSale.RowCount - 1].Value = txtBeNo.Text.Trim();
                dgvSale["ProductCode", dgvSale.RowCount - 1].Value = cmbPurProductCode.Text.Trim();
                dgvSale["ProductName", dgvSale.RowCount - 1].Value = cmbPurProductName.Text.Trim();
                dgvSale["UOM", dgvSale.RowCount - 1].Value = txtPurUom.Text.Trim();
                dgvSale["Quantity", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtPurQuantity.Text.Trim()).ToString();//ToString();//ToString("0,0.000000");
                dgvSale["UOMc", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim()).ToString();//ToString();//ToString("0,0.000000");
                dgvSale["UOMn", dgvSale.RowCount - 1].Value = cmbUom.Text.Trim();
                if (chkCategory.Checked && Totalprice == "Y")
                {
                    decimal purchaseQty = Convert.ToDecimal(txtPurQuantity.Text.Trim());
                    decimal totalPrice = Convert.ToDecimal(txtPurUnitPrice.Text.Trim());
                    dgvSale["UnitPrice", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject((totalPrice / purchaseQty).ToString());//ToString();//ToString("0,0.00000");
                }
                else
                {
                    dgvSale["UnitPrice", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtPurUnitPrice.Text.Trim()).ToString();//ToString();//ToString("0,0.00000");
                }
                dgvSale["UseQuantity", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtUsedQty.Text.Trim()).ToString();//ToString();//ToString("0,0.000000");
                dgvSale["FGQty", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();//ToString();//ToString("0,0.000000");
                dgvSale["SalesInvoiceNo", dgvSale.RowCount - 1].Value = txtOldSaleId.Text.Trim();
                dgvSale["PTransType", dgvSale.RowCount - 1].Value = vPTransType.ToString();

                if (isTrnsTypeImport == true)
                {
                    dgvSale["AV", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtSubTotal.Text.Trim()).ToString();//ToString();//ToString("0,0.00000");
                    dgvSale["CD", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtCd.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["RD", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtRd.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["SD", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtSd.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["VAT", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtVat.Text.Trim()).ToString();//ToString();//ToString("0,0.000000");
                    dgvSale["CnFAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtCnFAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["InsuranceAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtInsuranceAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["TVBAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtTVBAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["TVAAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtTVAAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["ATVAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtATVAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");
                    dgvSale["OthersAmount", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtOthersAmount.Text.Trim()).ToString();//ToString();//ToString("0,0.000000000");


                }
                else
                {
                    dgvSale["AV", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtSubTotal.Text.Trim()).ToString();//ToString("0,0.00000");
                    dgvSale["CD", dgvSale.RowCount - 1].Value = 0; //Convert.ToDecimal(txtCd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["RD", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtRd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["SD", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtSd.Text.Trim()).ToString();//ToString("0,0.000000000");

                    if (transactionType == "VDB")
                    {
                        dgvSale["VAT", dgvSale.RowCount - 1].Value =Program.ParseDecimalObject(txtVat.Text.Trim()).ToString();
                    }
                    else
                    {
                        dgvSale["VAT", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtVat.Text.Trim()).ToString();//ToString("0,0.000000");
                    }

                    dgvSale["CnFAmount", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtCnFAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["InsuranceAmount", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtInsuranceAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVBAmount", dgvSale.RowCount - 1].Value = 0;//Convert.ToDecimal(txtTVBAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVAAmount", dgvSale.RowCount - 1].Value = 0;//Convert.ToDecimal(txtTVAAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["ATVAmount", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtATVAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["OthersAmount", dgvSale.RowCount - 1].Value = 0;// Convert.ToDecimal(txtOthersAmount.Text.Trim()).ToString();//ToString("0,0.000000000");


                }

                Rowcalculate();

                if (rbtnPurCode.Enabled == true)
                {
                    cmbPurProductCode.Focus();
                }
                else
                {
                    cmbPurProductName.Focus();
                }




            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
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

        private bool CalculateSingleQtyEntry()
        {
            bool isQtyMore = false;
            decimal usedQty = 0;
            for (int i = 0; i < dgvSale.RowCount; i++)
            {
                if (dgvSale.Rows[i].Cells["ProductCode"].Value.ToString() == cmbPurProductCode.Text)
                {
                    usedQty += Convert.ToDecimal(dgvSale.Rows[i].Cells["UseQuantity"].Value.ToString());
                }
            }
            if ((Convert.ToDecimal(txtUsedQty.Text.Trim()) + usedQty) > (Convert.ToDecimal(txtPurQuantity.Text.Trim())))
            {
                isQtyMore = true;
            }
            return isQtyMore;
        }

        private void Rowcalculate()
        {
            decimal TotalQuantity = 0;
            decimal TotalClaimCD = 0;
            decimal TotalClaimRD = 0;
            decimal TotalClaimSD = 0;
            decimal TotalDDBack = 0;
            decimal TotalClaimVAT = 0;

            decimal uomc = 0;


            decimal TotalClaimCnFAmount = 0;
            decimal TotalClaimInsuranceAmount = 0;
            decimal TotalClaimTVBAmount = 0;
            decimal TotalClaimTVAAmount = 0;
            decimal TotalClaimATVAmount = 0;
            decimal TotalClaimOthersAmount = 0;
            decimal TotalInputSD = 0;



            decimal cnf, ins, tvb, tva, atv, others, useqty, cd, rd, sd, vat, qty = 0;




            for (int i = 0; i < dgvSale.RowCount; i++)
            {

                //Quantity = Convert.ToDecimal(dgvIssue["Quantity", i].Value);

                qty = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                if (qty == 0)
                    qty = 1;
                useqty = Convert.ToDecimal(dgvSale["UseQuantity", i].Value);

                cd = Convert.ToDecimal(dgvSale["CD", i].Value);
                rd = Convert.ToDecimal(dgvSale["RD", i].Value);
                sd = Convert.ToDecimal(dgvSale["SD", i].Value);
                vat = Convert.ToDecimal(dgvSale["VAT", i].Value);
                cnf = Convert.ToDecimal(dgvSale["CnFAmount", i].Value);
                ins = Convert.ToDecimal(dgvSale["InsuranceAmount", i].Value);
                tvb = Convert.ToDecimal(dgvSale["TVBAmount", i].Value);
                tva = Convert.ToDecimal(dgvSale["TVAAmount", i].Value);
                atv = Convert.ToDecimal(dgvSale["ATVAmount", i].Value);
                others = Convert.ToDecimal(dgvSale["OthersAmount", i].Value);
                TotalInputSD = TotalInputSD + sd;
                uomc = Convert.ToDecimal(dgvSale["UOMc", i].Value);


                //dgvIssue["VATAmount", i].Value = 0;
                dgvSale["ClaimCD", i].Value = (cd * useqty / qty).ToString();//ToString();//ToString("0,0.000000000");
                dgvSale["ClaimRD", i].Value = (rd * useqty / qty).ToString();//ToString();//ToString("0,0.000000000");
                dgvSale["ClaimSD", i].Value = (sd * useqty / qty).ToString();//ToString();//ToString("0,0.000000000");
                dgvSale["TOtalDDBack", i].Value = ((cd * useqty / qty) + (rd * useqty / qty) + (sd * useqty / qty)).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimVAT", i].Value = (vat * useqty / qty).ToString();//ToString("0,0.000000000");

                dgvSale["ClaimCnFAmount", i].Value = (cnf * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimInsuranceAmount", i].Value = (ins * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimTVBAmount", i].Value = (tvb * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimTVAAmount", i].Value = (tva * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimATVAmount", i].Value = (atv * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["ClaimOthersAmount", i].Value = (others * useqty / qty).ToString();//ToString("0,0.000000000");

                dgvSale["UOMCD", i].Value = (uomc * cd * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMRD", i].Value = (uomc * rd * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMSD", i].Value = (uomc * sd * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMSubTotalDDB", i].Value = ((uomc * cd * useqty / qty) + (uomc * rd * useqty / qty) + (uomc * sd * useqty / qty)).ToString();//ToString("0,0.000000000");
                dgvSale["UOMVAT", i].Value = (uomc * vat * useqty / qty).ToString();//ToString("0,0.000000000");

                dgvSale["UOMCnF", i].Value = (uomc * cnf * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMInsurance", i].Value = (uomc * ins * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMTVB", i].Value = (uomc * tvb * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMTVA", i].Value = (uomc * tva * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMATV", i].Value = (uomc * atv * useqty / qty).ToString();//ToString("0,0.000000000");
                dgvSale["UOMOthers", i].Value = (uomc * others * useqty / qty).ToString();//ToString("0,0.000000000");



                dgvSale["LineNo", i].Value = i + 1;
                TotalQuantity = TotalQuantity + Convert.ToDecimal(dgvSale["Quantity", i].Value);
                TotalClaimCD = TotalClaimCD + Convert.ToDecimal(dgvSale["ClaimCD", i].Value);
                TotalClaimRD = TotalClaimRD + Convert.ToDecimal(dgvSale["ClaimRD", i].Value);
                TotalClaimSD = TotalClaimSD + Convert.ToDecimal(dgvSale["ClaimSD", i].Value);
                TotalDDBack = TotalDDBack + Convert.ToDecimal(dgvSale["TOtalDDBack", i].Value);

                TotalClaimVAT = TotalClaimVAT + Convert.ToDecimal(dgvSale["ClaimVAT", i].Value);

                TotalClaimCnFAmount = TotalClaimCnFAmount + Convert.ToDecimal(dgvSale["ClaimCnFAmount", i].Value);
                TotalClaimInsuranceAmount = TotalClaimInsuranceAmount + Convert.ToDecimal(dgvSale["ClaimInsuranceAmount", i].Value);
                TotalClaimTVBAmount = TotalClaimTVBAmount + Convert.ToDecimal(dgvSale["ClaimTVBAmount", i].Value);
                TotalClaimTVAAmount = TotalClaimTVAAmount + Convert.ToDecimal(dgvSale["ClaimTVAAmount", i].Value);
                TotalClaimATVAmount = TotalClaimATVAmount + Convert.ToDecimal(dgvSale["ClaimATVAmount", i].Value);
                TotalClaimOthersAmount = TotalClaimOthersAmount + Convert.ToDecimal(dgvSale["ClaimOthersAmount", i].Value);

            }

            txtTotalQuantity.Text =Program.ParseDecimalObject(TotalQuantity).ToString();//ToString("0,0.00000");
            txtTotalCD.Text = Program.ParseDecimalObject(TotalClaimCD).ToString();//ToString("0,0.000000000");
            txtTotalRD.Text = Program.ParseDecimalObject(TotalClaimRD).ToString();//ToString("0,0.000000000");
            txtTotalSD.Text = Program.ParseDecimalObject(TotalClaimSD).ToString();//ToString("0,0.000000000");
            txtTotalDDBack.Text = Program.ParseDecimalObject(TotalDDBack).ToString();//ToString("0,0.000000000");
            txtTotalVat.Text = Program.ParseDecimalObject(TotalClaimVAT).ToString();//ToString("0,0.000000000");



            txtTotalClaimCnFAmount.Text = Program.ParseDecimalObject(TotalClaimCnFAmount).ToString();//ToString("0,0.000000000");
            txtTotalClaimInsuranceAmount.Text = Program.ParseDecimalObject(TotalClaimInsuranceAmount).ToString();//ToString("0,0.000000000");
            txtTotalClaimTVBAmount.Text = Program.ParseDecimalObject(TotalClaimTVBAmount).ToString();//ToString("0,0.000000000");
            txtTotalClaimTVAAmount.Text = Program.ParseDecimalObject(TotalClaimTVAAmount).ToString();//ToString("0,0.000000000");
            txtTotalClaimATVAmount.Text = Program.ParseDecimalObject(TotalClaimATVAmount).ToString();//ToString("0,0.000000000");
            txtTotalClaimOthersAmount.Text = Program.ParseDecimalObject(TotalClaimOthersAmount).ToString();//ToString("0,0.000000000");
            txtSubTotalSD.Text = Program.ParseDecimalObject(TotalInputSD).ToString();//ToString("0,0.000000000");

        }

        private void btnChange_Click(object sender, EventArgs e)
        {

            try
            {
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    if (dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString().Trim() != txtOldSaleId.Text.Trim() && dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString().Trim() != "-")
                    {
                        MessageBox.Show("More Sale Information cant save in a single DDB.", this.Text);
                        txtOldSaleId.Focus();
                        return;
                    }

                }

                if (dgvSale.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                UomsValue();
                if (
                    string.IsNullOrEmpty(cmbPurProductCode.Text)
                   || string.IsNullOrEmpty(cmbPurProductName.Text)
                   || string.IsNullOrEmpty(txtPurQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToDecimal(txtPurQuantity.Text) <= 0)
                {
                    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (Convert.ToDecimal(txtUsedQty.Text) <= 0)
                {
                    MessageBox.Show("Zero Used Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtUsedQty.Focus();
                    return;
                }

                if ((Convert.ToDecimal(txtUsedQty.Text.Trim())) > (Convert.ToDecimal(txtPurQuantity.Text.Trim())))
                {
                    MessageBox.Show("Used Quantity Should not more than Purchase Quantity");
                    txtUsedQty.Focus();
                    return;

                }

                else if (txtComments.Text == "")
                {
                    txtComments.Text = "NA";
                }


                ChangeData = true;
                decimal PreUseQty, CurrUseQty;

                PreUseQty = Convert.ToDecimal(dgvSale.CurrentRow.Cells["UseQuantity"].Value.ToString());
                CurrUseQty = Convert.ToDecimal(txtUsedQty.Text);



                //----------------------

                dgvSale["FgItemNo", dgvSale.CurrentRow.Index].Value = txtFgItem.Text.Trim();
                dgvSale["ItemNo", dgvSale.CurrentRow.Index].Value = txtPurItem.Text.Trim();
                dgvSale["FGCode", dgvSale.CurrentRow.Index].Value = cmbProductCode.Text.Trim();
                dgvSale["FGName", dgvSale.CurrentRow.Index].Value = cmbProduct.Text.Trim();
                dgvSale["PurchaseIdNo", dgvSale.CurrentRow.Index].Value = txtPurNo.Text.Trim();
                dgvSale["BillOfEntry", dgvSale.CurrentRow.Index].Value = txtBeNo.Text.Trim();
                dgvSale["ProductCode", dgvSale.CurrentRow.Index].Value = cmbPurProductCode.Text.Trim();
                dgvSale["ProductName", dgvSale.CurrentRow.Index].Value = cmbPurProductName.Text.Trim();
                dgvSale["UOM", dgvSale.CurrentRow.Index].Value = txtPurUom.Text.Trim();
                dgvSale["Quantity", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtPurQuantity.Text.Trim()).ToString();//ToString("0,0.000000");
                dgvSale["UseQuantity", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtUsedQty.Text.Trim()).ToString();//ToString("0,0.000000");
                dgvSale["FGQty", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();//ToString("0,0.000000");
                dgvSale["SalesInvoiceNo", dgvSale.CurrentRow.Index].Value = txtOldSaleId.Text.Trim();
                dgvSale["PTransType", dgvSale.CurrentRow.Index].Value = vPTransType;

                if (chkCategory.Checked && Totalprice == "Y")
                {
                    decimal purchaseQty = Convert.ToDecimal(txtPurQuantity.Text.Trim());
                    decimal totalPrice = Convert.ToDecimal(txtPurUnitPrice.Text.Trim());
                    dgvSale["UnitPrice", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject((totalPrice / purchaseQty).ToString());//ToString();//ToString("0,0.00000");
                }
                else
                {
                    dgvSale["UnitPrice", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(txtPurUnitPrice.Text.Trim()).ToString();//ToString();//ToString("0,0.00000");
                }

                if (vPTransType == "Import" || vPTransType == "TradingImport")
                {
                    isTrnsTypeImport = true;

                }
                else
                {
                    isTrnsTypeImport = false;
                }
                if (txtPurNo.Text.Trim() == "-")
                {
                    isTrnsTypeImport = true;
                }


                if (isTrnsTypeImport == true)
                {


                    dgvSale["AV", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtSubTotal.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["CD", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtCd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["RD", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtRd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["SD", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtSd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["VAT", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtVat.Text.Trim()).ToString();//ToString("0,0.000000");
                    dgvSale["CnFAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtCnFAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["InsuranceAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtInsuranceAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVBAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtTVBAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVAAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtTVAAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["ATVAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtATVAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["OthersAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(txtOthersAmount.Text.Trim()).ToString();//ToString("0,0.000000000");



                }
                else
                {


                    dgvSale["AV", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtSubTotal.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["CD", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtCd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["RD", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtRd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["SD", dgvSale.CurrentRow.Index].Value = 0;//Convert.ToDecimal(txtSd.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["VAT", dgvSale.CurrentRow.Index].Value = 0;//Convert.ToDecimal(txtVat.Text.Trim()).ToString();//ToString("0,0.000000");
                    dgvSale["CnFAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtCnFAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["InsuranceAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtInsuranceAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVBAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtTVBAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["TVAAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtTVAAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["ATVAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtATVAmount.Text.Trim()).ToString();//ToString("0,0.000000000");
                    dgvSale["OthersAmount", dgvSale.CurrentRow.Index].Value = 0;// Convert.ToDecimal(txtOthersAmount.Text.Trim()).ToString();//ToString("0,0.000000000");



                }



                Rowcalculate();
                dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }

            #endregion

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                //if (dgvSale.RowCount > 0)
                //{
                //if (
                //    string.IsNullOrEmpty(cmbPurProductCode.Text)
                //    || string.IsNullOrEmpty(cmbPurProductName.Text)
                //    || string.IsNullOrEmpty(txtPurQuantity.Text))
                //{
                //    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text,
                //                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}
                //else
                //{
                //ReceiveRemoveSingle();
                //if (MessageBox.Show(
                //MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvSale.CurrentRow.Cells["Productcode"].Value,
                //this.Text, MessageBoxButtons.YesNo,
                //MessageBoxIcon.Question) == DialogResult.Yes)
                //{
                //    dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                //    Rowcalculate();
                //    cmbPurProductCode.Text = "";
                //    cmbPurProductName.Text = "";
                //}

                //    }
                //}
                if (dgvSale.RowCount > 0)
                {
                    if (MessageBox.Show(
                        MessageVM.msgWantToRemoveRow + "\nItem Code: " +
                        dgvSale.CurrentRow.Cells["Productcode"].Value,
                        this.Text, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                        Rowcalculate();
                        cmbPurProductCode.Text = "";
                        cmbPurProductName.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Details Section.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    return;
                }


            }
            #region catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion
        }


        private void ReceiveRemoveSingle()
        {
            dgvSale.CurrentRow.Cells["Quantity"].Value = 0;
            dgvSale.CurrentRow.Cells["AV"].Value = 0;
            dgvSale.CurrentRow.Cells["CD"].Value = 0;
            dgvSale.CurrentRow.Cells["RD"].Value = 0;
            dgvSale.CurrentRow.Cells["SD"].Value = 0;
            dgvSale.CurrentRow.Cells["VAT"].Value = 0;
            dgvSale.CurrentRow.Cells["UseQuantity"].Value = 0;
            dgvSale.CurrentRow.Cells["FGQty"].Value = 0;
            dgvSale.CurrentRow.Cells["UnitPrice"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimCD"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimRD"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimSD"].Value = 0;
            dgvSale.CurrentRow.Cells["TotalDDBack"].Value = 0;

            dgvSale.CurrentRow.Cells["UOMc"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMCD"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMRD"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMSD"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMVAT"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMCnF"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMInsurance"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMTVB"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMTVA"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMATV"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMOthers"].Value = 0;
            dgvSale.CurrentRow.Cells["UOMSubTotalDDB"].Value = 0;

            dgvSale.CurrentRow.Cells["CnFAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["InsuranceAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["TVBAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["TVAAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ATVAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["OthersAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimVAT"].Value = 0;

            dgvSale.CurrentRow.Cells["ClaimCnFAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimInsuranceAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimTVBAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimTVAAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimATVAmount"].Value = 0;
            dgvSale.CurrentRow.Cells["ClaimOthersAmount"].Value = 0;





            dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
            dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
            Rowcalculate();
            cmbPurProductCode.Text = "";
            cmbPurProductName.Text = "";
            //cmbProduct.Text = "";
            //return false;
        }

        private void bgwSearchPurchase_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);
                PurchaseDetailResult = purchaseDal.SearchProductbyPurchaseInvoice(PurchaseDetailData,connVM);  // Change 04
                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSearchPurchase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                #region Product

                PurchaseMini.Clear();
                cmbPurProductName.Items.Clear();
                cmbPurProductCode.Items.Clear();
                productCmb.Clear();
                foreach (DataRow item2 in PurchaseDetailResult.Rows)
                {
                    var prod = new ProductbyPurchaseInvoiceDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    cmbPurProductCode.Items.Add(item2["ProductCode"]);
                    cmbPurProductName.Items.Add(item2["ProductName"]);
                    prod.UOM = item2["UOM"].ToString();
                    prod.Quantity = Convert.ToDecimal(item2["Quantity"]);
                    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"]);
                    prod.CD = Convert.ToDecimal(item2["CDAmount"]);
                    prod.SD = Convert.ToDecimal(item2["SDAmount"]);
                    prod.RD = Convert.ToDecimal(item2["RDAmount"]);
                    prod.VATAmount = Convert.ToDecimal(item2["VATAmount"]);
                    prod.SubTotal = Convert.ToDecimal(item2["SubTotal"]);
                    prod.CnFAmount = Convert.ToDecimal(item2["CnFAmount"]);
                    prod.InsuranceAmount = Convert.ToDecimal(item2["InsuranceAmount"]);
                    prod.TVBAmount = Convert.ToDecimal(item2["TVBAmount"]);
                    prod.ATVAmount = Convert.ToDecimal(item2["ATVAmount"]);
                    prod.TVAAmount = Convert.ToDecimal(item2["TVAAmount"]);
                    prod.OthersAmount = Convert.ToDecimal(item2["OthersAmount"]);
                    PurchaseMini.Add(prod);


                }
                #endregion Product


                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                cmbProduct.Enabled = true;
                cmbProductCode.Enabled = true;
                btnPurSearch.Visible = true;
                this.progressBar1.Visible = false;
                cmbPurProductCode.Focus();
            }
        }

        private void txtTotalCD_TextChanged(object sender, EventArgs e)
        {

        }



        private void btnVat18_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dptDutyDrawBackDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT18 frmRptVAT18 = new FormRptVAT18();

                //mdi.RollDetailsInfo(frmRptVAT18.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //frmRptVAT18.dtpToDate.Value = dptSalesDate.Value;
                //frmRptVAT18.dtpFromDate.Value = dptSalesDate.Value;
                frmRptVAT18.dtpToDate.Value = dptDutyDrawBackDate.Value;
                frmRptVAT18.dtpFromDate.Value = dptDutyDrawBackDate.Value;
                frmRptVAT18.ShowDialog();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }

            #endregion
        }



        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                PreviewOnly = true;

                if (txtDutyDrawNo.Text == "")
                {
                    MessageBox.Show("Please select the DutyDrawBackNo", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                ReportShowDs();
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
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

        private void ReportShowDs()
        {
            try
            {
                this.progressBar1.Visible = true;
                backgroundWorkerPreview.RunWorkerAsync();

                #region Start
                ;
                #endregion
                #region Complete


                #endregion

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion Catch

        }

        private void dgvSale_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvSale != null)
                {
                    DataGridViewSelectedRowCollection selectedRows = dgvSale.SelectedRows;
                    if (selectedRows != null && selectedRows.Count > 0)
                    {
                        DataGridViewRow userSelRow = selectedRows[0];
                        if (userSelRow != null)
                        {
                            txtFgItem.Text = userSelRow.Cells["FgItemNo"].Value.ToString();
                            txtPurItem.Text = userSelRow.Cells["ItemNo"].Value.ToString();
                            cmbProductCode.Text = userSelRow.Cells["FGCode"].Value.ToString();
                            cmbProduct.Text = userSelRow.Cells["FGName"].Value.ToString();
                            txtPurNo.Text = userSelRow.Cells["PurchaseIdNo"].Value.ToString();
                            txtBeNo.Text = userSelRow.Cells["BillOfEntry"].Value.ToString();
                            cmbPurProductCode.Text = userSelRow.Cells["ProductCode"].Value.ToString();
                            cmbPurProductName.Text = userSelRow.Cells["ProductName"].Value.ToString();
                            txtPurUom.Text = userSelRow.Cells["UOMn"].Value.ToString();
                            cmbUom.Text = userSelRow.Cells["UOM"].Value.ToString();
                            txtPurQuantity.Text = Convert.ToDecimal(userSelRow.Cells["Quantity"].Value).ToString();//ToString("0,0.00000");

                            if (chkCategory.Checked && Totalprice == "Y")
                            {
                                decimal purchaseQty = Convert.ToDecimal(txtPurQuantity.Text.Trim());
                                decimal unitPrice = Convert.ToDecimal(userSelRow.Cells["UnitPrice"].Value);
                                txtPurUnitPrice.Text = Convert.ToDecimal(unitPrice * purchaseQty).ToString();//ToString("0,0.00000");
                            }
                            else
                            {
                                txtPurUnitPrice.Text = Convert.ToDecimal(userSelRow.Cells["UnitPrice"].Value).ToString();//ToString("0,0.00000");
                            }

                            txtSubTotal.Text = Convert.ToDecimal(userSelRow.Cells["AV"].Value).ToString();//ToString("0.000000000");
                            txtCd.Text = userSelRow.Cells["CD"].Value.ToString();
                            txtRd.Text = userSelRow.Cells["RD"].Value.ToString();
                            txtSd.Text = userSelRow.Cells["SD"].Value.ToString();
                            txtVat.Text = userSelRow.Cells["VAT"].Value.ToString();
                            txtUsedQty.Text = Convert.ToDecimal(userSelRow.Cells["UseQuantity"].Value).ToString();//ToString("0.00000");
                            txtQuantity.Text = Convert.ToDecimal(userSelRow.Cells["FGQty"].Value).ToString();//ToString("0.00000");
                            vPTransType = userSelRow.Cells["PTransType"].Value.ToString();

                            txtCnFAmount.Text = Convert.ToDecimal(userSelRow.Cells["CnFAmount"].Value).ToString();//ToString("0.000000000");
                            txtInsuranceAmount.Text = Convert.ToDecimal(userSelRow.Cells["InsuranceAmount"].Value).ToString();//ToString("0.000000000");
                            txtTVBAmount.Text = Convert.ToDecimal(userSelRow.Cells["TVBAmount"].Value).ToString();//ToString("0.000000000");
                            txtTVAAmount.Text = Convert.ToDecimal(userSelRow.Cells["TVAAmount"].Value).ToString();//ToString("0.000000000");
                            txtATVAmount.Text = Convert.ToDecimal(userSelRow.Cells["ATVAmount"].Value).ToString();//ToString("0.000000000");
                            txtOthersAmount.Text = Convert.ToDecimal(userSelRow.Cells["OthersAmount"].Value).ToString();//ToString("0.000000000");
                            //txtOldSaleId.Text = userSelRow.Cells["SalesInvoiceNo"].Value.ToString();



                        }
                    }
                }


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvSale_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvSale_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", exMessage);
            }

            #endregion
        }

        private void txtTotalRD_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnVat11_Click(object sender, EventArgs e)
        {

        }


        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TransactionTypes();
            try
            {
                #region save
                #region start

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Convert.ToDateTime(dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd")) < Convert.ToDateTime(dptSalesDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("DDB date not before Export Invoice date", this.Text);
                    return;
                }
                if (dgvSale.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }


                #endregion start


                #region Master
                ddbackMaster = new DDBHeaderVM();
                ddbackMaster.TransactionType = transactionType;
                ddbackMaster.DDBackNo = txtDutyDrawNo.Text.Trim();
                ddbackMaster.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ddbackMaster.SalesInvoiceNo = txtOldSaleId.Text.Trim();
                ddbackMaster.SalesDate = dptSalesDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ddbackMaster.CustormerID = txtCustomerId.Text.Trim();
                ddbackMaster.CurrencyId = Convert.ToInt32(txtExpCurrId.Text.Trim());
                ddbackMaster.ExpCurrency = Convert.ToDecimal(txtExpCurr.Text.Trim());
                ddbackMaster.BDTCurrency = Convert.ToDecimal(txtTotalPrice.Text.Trim());
                ddbackMaster.FgItemNo = txtFgItem.Text.Trim();
                ddbackMaster.TotalClaimCD = Convert.ToDecimal(txtTotalCD.Text.Trim());
                ddbackMaster.TotalClaimRD = Convert.ToDecimal(txtTotalRD.Text.Trim());
                ddbackMaster.TotalClaimSD = Convert.ToDecimal(txtTotalSD.Text.Trim());
                ddbackMaster.TotalClaimVAT = Convert.ToDecimal(txtTotalVat.Text.Trim());
                ddbackMaster.TotalDDBack = Convert.ToDecimal(txtTotalDDBack.Text.Trim());
                ddbackMaster.TotalClaimCnFAmount = Convert.ToDecimal(txtTotalClaimCnFAmount.Text.Trim());
                ddbackMaster.TotalClaimInsuranceAmount = Convert.ToDecimal(txtTotalClaimInsuranceAmount.Text.Trim());
                ddbackMaster.TotalClaimTVBAmount = Convert.ToDecimal(txtTotalClaimTVBAmount.Text.Trim());
                ddbackMaster.TotalClaimTVAAmount = Convert.ToDecimal(txtTotalClaimTVAAmount.Text.Trim());
                ddbackMaster.TotalClaimATVAmount = Convert.ToDecimal(txtTotalClaimATVAmount.Text.Trim());
                ddbackMaster.TotalClaimOthersAmount = Convert.ToDecimal(txtTotalClaimOthersAmount.Text.Trim());

                ddbackMaster.ApprovedSD = Convert.ToDecimal(txtApprovedSD.Text.Trim() == "" ? "0" : txtApprovedSD.Text.Trim());
                ddbackMaster.TotalSDAmount = Convert.ToDecimal(txtSubTotalSD.Text.Trim() == "" ? "0" : txtSubTotalSD.Text.Trim());

                ddbackMaster.Comments = txtComments.Text.Trim();
                ddbackMaster.CreatedBy = Program.CurrentUser;
                ddbackMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ddbackMaster.LastModifiedBy = Program.CurrentUser;
                ddbackMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ddbackMaster.Post = "N"; //Post

                ddbackMaster.BranchId = Program.BranchId;

                #endregion Master
                #region detail

                ddbackDetails = new List<DDBDetailsVM>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    DDBDetailsVM ddbackdetail = new DDBDetailsVM();
                    ddbackdetail.DDBackNo = txtDutyDrawNo.Text.Trim();
                    ddbackdetail.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    ddbackdetail.DDLineNo = Convert.ToInt32(dgvSale.Rows[i].Cells["LineNo"].Value.ToString());
                    ddbackdetail.PurchaseInvoiceNo = dgvSale.Rows[i].Cells["PurchaseIdNo"].Value.ToString();
                    ddbackdetail.PurchaseDate = dgvSale.Rows[i].Cells["PurchaseDate"].Value.ToString();
                    ddbackdetail.FgItemNo = dgvSale.Rows[i].Cells["FgItemNo"].Value.ToString();
                    ddbackdetail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    ddbackdetail.BillOfEntry = dgvSale.Rows[i].Cells["BillOfEntry"].Value.ToString();
                    ddbackdetail.PurchaseUom = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    ddbackdetail.PurchaseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                    ddbackdetail.UnitPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    ddbackdetail.AV = Convert.ToDecimal(dgvSale.Rows[i].Cells["AV"].Value.ToString());
                    ddbackdetail.CD = Convert.ToDecimal(dgvSale.Rows[i].Cells["CD"].Value.ToString());
                    ddbackdetail.RD = Convert.ToDecimal(dgvSale.Rows[i].Cells["RD"].Value.ToString());
                    ddbackdetail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    ddbackdetail.VAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["VAT"].Value.ToString());
                    ddbackdetail.CnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["CnFAmount"].Value.ToString());
                    ddbackdetail.Insurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["InsuranceAmount"].Value.ToString());
                    ddbackdetail.TVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVBAmount"].Value.ToString());
                    ddbackdetail.TVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVAAmount"].Value.ToString());
                    ddbackdetail.ATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                    ddbackdetail.Others = Convert.ToDecimal(dgvSale.Rows[i].Cells["OthersAmount"].Value.ToString());
                    ddbackdetail.UseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["UseQuantity"].Value.ToString());
                    ddbackdetail.ClaimCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCD"].Value.ToString());
                    ddbackdetail.ClaimRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimRD"].Value.ToString());
                    ddbackdetail.ClaimSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimSD"].Value.ToString());
                    ddbackdetail.ClaimVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimVAT"].Value.ToString());
                    ddbackdetail.ClaimCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCnFAmount"].Value.ToString());
                    ddbackdetail.ClaimInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimInsuranceAmount"].Value.ToString());
                    ddbackdetail.ClaimTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVBAmount"].Value.ToString());
                    ddbackdetail.ClaimTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVAAmount"].Value.ToString());
                    ddbackdetail.ClaimATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimATVAmount"].Value.ToString());
                    ddbackdetail.ClaimOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimOthersAmount"].Value.ToString());
                    ddbackdetail.SubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalDDBack"].Value.ToString());
                    ddbackdetail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    ddbackdetail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    ddbackdetail.UOMCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCD"].Value.ToString());
                    ddbackdetail.UOMRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMRD"].Value.ToString());
                    ddbackdetail.UOMSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSD"].Value.ToString());
                    ddbackdetail.UOMVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMVAT"].Value.ToString());
                    ddbackdetail.UOMCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCnF"].Value.ToString());
                    ddbackdetail.UOMInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMInsurance"].Value.ToString());
                    ddbackdetail.UOMTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVB"].Value.ToString());
                    ddbackdetail.UOMTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVA"].Value.ToString());
                    ddbackdetail.UOMATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMATV"].Value.ToString());
                    ddbackdetail.UOMOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMOthers"].Value.ToString());
                    ddbackdetail.UOMSubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSubTotalDDB"].Value.ToString());
                    ddbackdetail.Post = "N";
                    ddbackdetail.CreatedBy = Program.CurrentUser;
                    ddbackdetail.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    ddbackdetail.LastModifiedBy = Program.CurrentUser;
                    ddbackdetail.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    ddbackdetail.SalesInvoiceNo = dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString();
                    ddbackdetail.FGQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["FGQty"].Value.ToString());
                    ddbackdetail.PTransType = dgvSale.Rows[i].Cells["PTransType"].Value.ToString();
                    ddbackdetail.BranchId = Program.BranchId;
                    ddbackDetails.Add(ddbackdetail);
                }
                ddbSaleInvoicesVM = new List<DDBSaleInvoicesVM>();
                for (int i = 0; i < dgvSaleHistory.RowCount; i++)
                {
                    DDBSaleInvoicesVM ddbackdetail = new DDBSaleInvoicesVM();
                    ddbackdetail.DDBackNo = txtDutyDrawNo.Text.Trim();
                    ddbackdetail.SL = dgvSaleHistory.Rows[i].Cells["SL"].Value.ToString(); ;
                    ddbackdetail.SalesInvoiceNo = dgvSaleHistory.Rows[i].Cells["SalesInvoiceNoP"].Value.ToString(); ;
                    ddbackdetail.SalesDate = dgvSaleHistory.Rows[i].Cells["SalesDate"].Value.ToString();
                    ddbackdetail.BranchId = Program.BranchId;
                    ddbSaleInvoicesVM.Add(ddbackdetail);
                }

                #endregion detail



                #endregion save
                if (ddbackDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.progressBar1.Visible = true;
                this.btnSave.Enabled = false;
                backgroundWorkerSave.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                //DutyDrawBackDAL DDBackDal = new DutyDrawBackDAL();
                IDutyDrawBack DDBackDal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);
                sqlResults = DDBackDal.DutyDrawBacknsert(ddbackMaster, ddbackDetails, ddbSaleInvoicesVM,connVM);
                SAVE_DOWORK_SUCCESS = true;

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region statement

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }
                        if (result == "Success")
                        {
                            txtDutyDrawNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            //for (int i = 0; i < dgvSale.RowCount; i++)
                            //{
                            //    dgvSale["Status", dgvSale.RowCount - 1].Value = "Old";
                            //}
                        }

                    }


                #endregion
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            #endregion
            this.progressBar1.Visible = false;
            this.btnSave.Enabled = true;

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            TransactionTypes();
            try
            {

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dptPurNoDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //for (int i = 0; i < dgvSaleHistory.RowCount; i++)
                //{

                //    string SalesDate = dgvSaleHistory.Rows[i].Cells["SalesDate"].Value.ToString();

                //   if (Convert.ToDateTime(dptDutyDrawBackDate.Value) <= Convert.ToDateTime(SalesDate))
                //    {
                //        MessageBox.Show("DDB date not before Export Invoice date", this.Text);
                //        return;
                //    }
                //}

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDutyDrawNo.Text.Trim();
                }
                //#region Transaction Type
                //string transactionType = string.Empty;
                //if (rbtnOther.Checked)
                //{
                //    transactionType = "Other";
                //}


                //else if (rbtnIssueReturn.Checked)
                //{
                //    transactionType = "IssueReturn";
                //}

                //#endregion Transaction Type
                //if (btnSave.Text == "&Add")
                //{
                //    //NextID = DBConstant.NextIDFinder("IssueHeaders", "IssueNo");
                //}
                //else
                //{
                //    NextID = txtIssueNo.Text.Trim();
                //}
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }


                if (dgvSale.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }



                MasterVM = new DDBHeaderVM();

                MasterVM.TransactionType = transactionType;
                MasterVM.DDBackNo = txtDutyDrawNo.Text.Trim();
                MasterVM.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.SalesInvoiceNo = txtOldSaleId.Text.Trim();
                MasterVM.SalesDate = dptSalesDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.CustormerID = txtCustomerId.Text.Trim();
                MasterVM.CurrencyId = Convert.ToInt32(txtExpCurrId.Text.Trim());
                MasterVM.ExpCurrency = Convert.ToDecimal(txtExpCurr.Text.Trim());
                MasterVM.BDTCurrency = Convert.ToDecimal(txtTotalPrice.Text.Trim());
                MasterVM.FgItemNo = txtFgItem.Text.Trim();
                MasterVM.TotalClaimCD = Convert.ToDecimal(txtTotalCD.Text.Trim());
                MasterVM.TotalClaimRD = Convert.ToDecimal(txtTotalRD.Text.Trim());
                MasterVM.TotalClaimSD = Convert.ToDecimal(txtTotalSD.Text.Trim());
                MasterVM.TotalDDBack = Convert.ToDecimal(txtTotalDDBack.Text.Trim());
                MasterVM.TotalClaimVAT = Convert.ToDecimal(txtTotalVat.Text.Trim());
                MasterVM.TotalClaimCnFAmount = Convert.ToDecimal(txtTotalClaimCnFAmount.Text.Trim());
                MasterVM.TotalClaimInsuranceAmount = Convert.ToDecimal(txtTotalClaimInsuranceAmount.Text.Trim());
                MasterVM.TotalClaimTVBAmount = Convert.ToDecimal(txtTotalClaimTVBAmount.Text.Trim());
                MasterVM.TotalClaimTVAAmount = Convert.ToDecimal(txtTotalClaimTVAAmount.Text.Trim());
                MasterVM.TotalClaimATVAmount = Convert.ToDecimal(txtTotalClaimATVAmount.Text.Trim());
                MasterVM.TotalClaimOthersAmount = Convert.ToDecimal(txtTotalClaimOthersAmount.Text.Trim());

                MasterVM.ApprovedSD = Convert.ToDecimal(txtApprovedSD.Text.Trim() == "" ? "0" : txtApprovedSD.Text.Trim());
                MasterVM.TotalSDAmount = Convert.ToDecimal(txtSubTotalSD.Text.Trim() == "" ? "0" : txtSubTotalSD.Text.Trim());

                MasterVM.Comments = txtComments.Text.Trim();
                MasterVM.CreatedBy = Program.CurrentUser;
                MasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.LastModifiedBy = Program.CurrentUser;
                MasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.Post = "N"; //Post
                MasterVM.BranchId = Program.BranchId;



                DetailVMs = new List<DDBDetailsVM>();

                for (int i = 0; i < dgvSale.RowCount; i++)
                {


                    DDBDetailsVM detail = new DDBDetailsVM();

                    detail.DDBackNo = txtDutyDrawNo.Text.Trim();
                    detail.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.DDLineNo = Convert.ToInt32(dgvSale.Rows[i].Cells["LineNo"].Value.ToString());
                    detail.PurchaseInvoiceNo = dgvSale.Rows[i].Cells["PurchaseIdNo"].Value.ToString();
                    detail.PurchaseDate = dgvSale.Rows[i].Cells["PurchaseDate"].Value.ToString();
                    detail.FgItemNo = dgvSale.Rows[i].Cells["FgItemNo"].Value.ToString();
                    detail.FGQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["FGQty"].Value.ToString());
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.BillOfEntry = dgvSale.Rows[i].Cells["BillOfEntry"].Value.ToString();
                    detail.PurchaseUom = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.PurchaseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.UnitPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.AV = Convert.ToDecimal(dgvSale.Rows[i].Cells["AV"].Value.ToString());
                    detail.CD = Convert.ToDecimal(dgvSale.Rows[i].Cells["CD"].Value.ToString());
                    detail.RD = Convert.ToDecimal(dgvSale.Rows[i].Cells["RD"].Value.ToString());
                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.VAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["VAT"].Value.ToString());
                    detail.CnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["CnFAmount"].Value.ToString());
                    detail.Insurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["InsuranceAmount"].Value.ToString());
                    detail.TVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVBAmount"].Value.ToString());
                    detail.TVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVAAmount"].Value.ToString());
                    detail.ATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                    detail.Others = Convert.ToDecimal(dgvSale.Rows[i].Cells["OthersAmount"].Value.ToString());
                    detail.UseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["UseQuantity"].Value.ToString());
                    detail.ClaimCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCD"].Value.ToString());
                    detail.ClaimRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimRD"].Value.ToString());
                    detail.ClaimSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimSD"].Value.ToString());
                    detail.ClaimVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimVAT"].Value.ToString());
                    detail.ClaimCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCnFAmount"].Value.ToString());
                    detail.ClaimInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimInsuranceAmount"].Value.ToString());
                    detail.ClaimTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVBAmount"].Value.ToString());
                    detail.ClaimTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVAAmount"].Value.ToString());
                    detail.ClaimATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimATVAmount"].Value.ToString());
                    detail.ClaimOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimOthersAmount"].Value.ToString());
                    detail.SubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalDDBack"].Value.ToString());
                    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCD"].Value.ToString());
                    detail.UOMRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMRD"].Value.ToString());
                    detail.UOMSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSD"].Value.ToString());
                    detail.UOMVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMVAT"].Value.ToString());
                    detail.UOMCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCnF"].Value.ToString());
                    detail.UOMInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMInsurance"].Value.ToString());
                    detail.UOMTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVB"].Value.ToString());
                    detail.UOMTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVA"].Value.ToString());
                    detail.UOMATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMATV"].Value.ToString());
                    detail.UOMOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMOthers"].Value.ToString());
                    detail.UOMSubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSubTotalDDB"].Value.ToString());
                    detail.Post = "N";
                    detail.CreatedBy = Program.CurrentUser;
                    detail.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.LastModifiedBy = Program.CurrentUser;
                    detail.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.SalesInvoiceNo = dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString();
                    detail.FGQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["FGQty"].Value.ToString());

                    detail.PTransType = dgvSale.Rows[i].Cells["PTransType"].Value.ToString();
                    detail.BranchId = Program.BranchId;

                    //detail.SalesInvoiceNo = txtOldSaleId.Text.Trim();


                    DetailVMs.Add(detail);

                }// End For

                ddbSaleInvoicesVM = new List<DDBSaleInvoicesVM>();
                for (int i = 0; i < dgvSaleHistory.RowCount; i++)
                {
                    DDBSaleInvoicesVM ddbackdetail = new DDBSaleInvoicesVM();
                    ddbackdetail.DDBackNo = txtDutyDrawNo.Text.Trim();
                    ddbackdetail.SL = dgvSaleHistory.Rows[i].Cells["SL"].Value.ToString(); ;
                    ddbackdetail.SalesInvoiceNo = dgvSaleHistory.Rows[i].Cells["SalesInvoiceNoP"].Value.ToString();
                    ddbackdetail.SalesDate = dgvSaleHistory.Rows[i].Cells["SalesDate"].Value.ToString();
                    ddbSaleInvoicesVM.Add(ddbackdetail);
                }

                if (DetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                bgwUpdate.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DutyDrawBackDAL DDBackDal = new DutyDrawBackDAL();
                IDutyDrawBack DDBackDal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);
                sqlResults = DDBackDal.DutyDrawBackUpdate(MasterVM, DetailVMs, ddbSaleInvoicesVM,connVM);
                UPDATE_DOWORK_SUCCESS = true;

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDutyDrawNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            //for (int i = 0; i < dgvSale.RowCount; i++)
                            //{
                            //    dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                            //}
                        }
                    }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            ChangeData = false;

            #region try
            try
            {
                string result;

                Program.fromOpen = "Me";
                #region Transaction Type
                DataGridViewRow selectedRow = new DataGridViewRow();
                TransactionTypes();
                selectedRow = FormDutyDrawBackSearch.SelectOne(transactionType);


                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtDutyDrawNo.Text = selectedRow.Cells["DDBackNo"].Value.ToString();
                    dptDutyDrawBackDate.Value = Convert.ToDateTime(selectedRow.Cells["DDBackDate"].Value.ToString());
                    txtOldSaleId.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    dptSalesDate.Value = Convert.ToDateTime(selectedRow.Cells["SalesDate"].Value.ToString());
                    txtCustomerName.Text = selectedRow.Cells["CustormerName"].Value.ToString();
                    cmbProductCode.Text = selectedRow.Cells["FgItemNo"].Value.ToString();
                    cmbProduct.Text = selectedRow.Cells["FgItemName"].Value.ToString();
                    //txtUOM.Text = selectedRow.Cells["DDBackNo"].Value.ToString();
                    //txtQuantity.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//ToString("0,0.00");
                    txtCurrencyName.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtExpCurr.Text = Program.ParseDecimalObject(selectedRow.Cells["ExpCurrency"].Value.ToString()).ToString();//ToString("0,0.000000");
                    txtTotalPrice.Text = Program.ParseDecimalObject(selectedRow.Cells["BDTCurrency"].Value.ToString()).ToString();//ToString("0,0.000000");
                    txtApprovedSD.Text = Program.ParseDecimalObject(Convert.ToDecimal(selectedRow.Cells["ApprovedSD"].Value.ToString())).ToString();//ToString("0,0.000000");
                    txtSubTotalSD.Text = Program.ParseDecimalObject(Convert.ToDecimal(selectedRow.Cells["TotalSDAmount"].Value.ToString())).ToString();//ToString("0,0.000000");
                    //txtQuantity.Text = Convert.ToDecimal(selectedRow.Cells["FGQty"].Value.ToString()).ToString();//ToString("0,0.000000");
                    txtFgItem.Text = selectedRow.Cells["FgItemNo"].Value.ToString();
                    txtCustomerId.Text = selectedRow.Cells["CustormerID"].Value.ToString();
                    txtExpCurrId.Text = selectedRow.Cells["CurrencyId"].Value.ToString();
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    SearchBranchId = selectedRow.Cells["BranchId"].Value.ToString();

                    backgroundWorkerSearchDDBackNo.RunWorkerAsync();
                }



            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearchDDBackNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ddbackDetailData = txtDutyDrawNo.Text.Trim();
                var oldSaleId = txtOldSaleId.Text.Trim();
                ddbackDetailResult = new DataTable();

                //DutyDrawBackDAL DDBackDal = new DutyDrawBackDAL();

                IDutyDrawBack DDBackDal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);
                ddbackDetailResult = DDBackDal.SearchddBackDetails(ddbackDetailData, oldSaleId,connVM); // Change 04

                #endregion
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearchDDBackNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                dgvSale.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ddbackDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);
                    dgvSale.Rows[j].Cells["LineNo"].Value = item["DDLineNo"].ToString();
                    dgvSale.Rows[j].Cells["FgItemNo"].Value = item["FgItemNo"].ToString();
                    dgvSale.Rows[j].Cells["FGCode"].Value = item["fitemcode"].ToString();
                    dgvSale.Rows[j].Cells["FGName"].Value = item["fitemname"].ToString();
                    dgvSale.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvSale.Rows[j].Cells["PurchaseIdNo"].Value = item["PurchaseInvoiceNo"].ToString();
                    dgvSale.Rows[j].Cells["PurchaseDate"].Value = item["PurchaseDate"].ToString();
                    dgvSale.Rows[j].Cells["ProductCode"].Value = item["pitemcode"].ToString();
                    dgvSale.Rows[j].Cells["ProductName"].Value = item["pitemname"].ToString();
                    dgvSale.Rows[j].Cells["BillOfEntry"].Value = item["BillOfEntry"].ToString();
                    dgvSale.Rows[j].Cells["UOM"].Value = item["PurchaseUom"].ToString();
                    dgvSale.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["PurchaseQuantity"].ToString());
                    dgvSale.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["UnitPrice"].ToString());
                    dgvSale.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item["UOMc"].ToString());
                    dgvSale.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvSale.Rows[j].Cells["AV"].Value = Program.ParseDecimalObject(item["AV"].ToString());
                    dgvSale.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item["CD"].ToString());
                    dgvSale.Rows[j].Cells["RD"].Value = Program.ParseDecimalObject(item["RD"].ToString());
                    dgvSale.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    dgvSale.Rows[j].Cells["VAT"].Value = Program.ParseDecimalObject(item["VAT"].ToString());
                    dgvSale.Rows[j].Cells["UseQuantity"].Value = Program.ParseDecimalObject(item["UseQuantity"].ToString());
                    dgvSale.Rows[j].Cells["CnFAmount"].Value = Program.ParseDecimalObject(item["CnF"].ToString());
                    dgvSale.Rows[j].Cells["InsuranceAmount"].Value = Program.ParseDecimalObject(item["Insurance"].ToString());
                    dgvSale.Rows[j].Cells["TVBAmount"].Value = Program.ParseDecimalObject(item["TVB"].ToString());
                    dgvSale.Rows[j].Cells["TVAAmount"].Value = Program.ParseDecimalObject(item["TVA"].ToString());
                    dgvSale.Rows[j].Cells["ATVAmount"].Value = Program.ParseDecimalObject(item["ATV"].ToString());
                    dgvSale.Rows[j].Cells["OthersAmount"].Value = Program.ParseDecimalObject(item["Others"].ToString());
                    dgvSale.Rows[j].Cells["SalesInvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                    dgvSale.Rows[j].Cells["FGQty"].Value = Program.ParseDecimalObject(item["FGQty"].ToString());
                    dgvSale.Rows[j].Cells["PTransType"].Value = item["PurchasetransactionType"].ToString();

                    Rowcalculate();

                    j = j + 1;
                }  //End For

                #endregion
                //DutyDrawBackDAL _dal = new DutyDrawBackDAL();
                IDutyDrawBack _dal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);
                DataTable dt = new DataTable();
                dt = _dal.SearchddbSaleInvoices(txtDutyDrawNo.Text.Trim(),connVM);
                dgvSaleHistory.Rows.Clear();
                j = 0;
                foreach (DataRow item in dt.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSaleHistory.Rows.Add(NewRow);
                    dgvSaleHistory.Rows[j].Cells["SalesInvoiceNoP"].Value = item["SalesInvoiceNo"].ToString();
                    dgvSaleHistory.Rows[j].Cells["SalesDate"].Value = item["SalesDate"].ToString();
                    dgvSaleHistory.Rows[j].Cells["SL"].Value = item["SL"].ToString();
                    j = j + 1;
                }
                MultipleInvoiceSelect();

                //Customercountry(txtCustomerName.Text.Trim());
                //Customercurrency(txtOldSaleId.Text.Trim());
                //ProductSearchDsFormLoad();
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDDBackNo_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.btnSearchInvoiceNo.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dptPurNoDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (Convert.ToDateTime(dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd")) < Convert.ToDateTime(dptSalesDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("DDB date not before Export Invoice date", this.Text);
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDutyDrawNo.Text.Trim();
                }
                //#region Transaction Type
                //string transactionType = string.Empty;
                //if (rbtnOther.Checked)
                //{
                //    transactionType = "Other";
                //}


                //else if (rbtnIssueReturn.Checked)
                //{
                //    transactionType = "IssueReturn";
                //}

                //#endregion Transaction Type
                //if (btnSave.Text == "&Add")
                //{
                //    //NextID = DBConstant.NextIDFinder("IssueHeaders", "IssueNo");
                //}
                //else
                //{
                //    NextID = txtIssueNo.Text.Trim();
                //}
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }


                if (dgvSale.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }



                MasterVM = new DDBHeaderVM();

                MasterVM.DDBackNo = txtDutyDrawNo.Text.Trim();
                MasterVM.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.SalesInvoiceNo = txtOldSaleId.Text.Trim();
                MasterVM.SalesDate = dptSalesDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.CustormerID = txtCustomerId.Text.Trim();
                MasterVM.CurrencyId = Convert.ToInt32(txtExpCurrId.Text.Trim());
                MasterVM.ExpCurrency = Convert.ToDecimal(txtExpCurr.Text.Trim());
                MasterVM.BDTCurrency = Convert.ToDecimal(txtTotalPrice.Text.Trim());
                MasterVM.FgItemNo = txtFgItem.Text.Trim();
                MasterVM.TotalClaimCD = Convert.ToDecimal(txtTotalCD.Text.Trim());
                MasterVM.TotalClaimRD = Convert.ToDecimal(txtTotalRD.Text.Trim());
                MasterVM.TotalClaimSD = Convert.ToDecimal(txtTotalSD.Text.Trim());
                MasterVM.TotalClaimVAT = Convert.ToDecimal(txtTotalVat.Text.Trim());
                MasterVM.TotalClaimCnFAmount = Convert.ToDecimal(txtTotalClaimCnFAmount.Text.Trim());
                MasterVM.TotalClaimInsuranceAmount = Convert.ToDecimal(txtTotalClaimInsuranceAmount.Text.Trim());
                MasterVM.TotalClaimTVBAmount = Convert.ToDecimal(txtTotalClaimTVBAmount.Text.Trim());
                MasterVM.TotalClaimTVAAmount = Convert.ToDecimal(txtTotalClaimTVAAmount.Text.Trim());
                MasterVM.TotalClaimATVAmount = Convert.ToDecimal(txtTotalClaimATVAmount.Text.Trim());
                MasterVM.TotalClaimOthersAmount = Convert.ToDecimal(txtTotalClaimOthersAmount.Text.Trim());
                MasterVM.Comments = txtComments.Text.Trim();
                MasterVM.CreatedBy = Program.CurrentUser;
                MasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.LastModifiedBy = Program.CurrentUser;
                MasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.Post = "Y"; //Post
                MasterVM.BranchId = Program.BranchId;


                DetailVMs = new List<DDBDetailsVM>();

                for (int i = 0; i < dgvSale.RowCount; i++)
                {


                    DDBDetailsVM detail = new DDBDetailsVM();

                    detail.DDBackNo = txtDutyDrawNo.Text.Trim();
                    detail.DDBackDate = dptDutyDrawBackDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.DDLineNo = Convert.ToInt32(dgvSale.Rows[i].Cells["LineNo"].Value.ToString());
                    detail.PurchaseInvoiceNo = dgvSale.Rows[i].Cells["PurchaseIdNo"].Value.ToString();
                    detail.PurchaseDate = dgvSale.Rows[i].Cells["PurchaseDate"].Value.ToString();
                    detail.FgItemNo = dgvSale.Rows[i].Cells["FgItemNo"].Value.ToString();
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.BillOfEntry = dgvSale.Rows[i].Cells["BillOfEntry"].Value.ToString();
                    detail.PurchaseUom = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.PurchaseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.UnitPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.AV = Convert.ToDecimal(dgvSale.Rows[i].Cells["AV"].Value.ToString());
                    detail.CD = Convert.ToDecimal(dgvSale.Rows[i].Cells["CD"].Value.ToString());
                    detail.RD = Convert.ToDecimal(dgvSale.Rows[i].Cells["RD"].Value.ToString());
                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.VAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["VAT"].Value.ToString());
                    detail.CnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["CnFAmount"].Value.ToString());
                    detail.Insurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["InsuranceAmount"].Value.ToString());
                    detail.TVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVBAmount"].Value.ToString());
                    detail.TVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["TVAAmount"].Value.ToString());
                    detail.ATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                    detail.Others = Convert.ToDecimal(dgvSale.Rows[i].Cells["OthersAmount"].Value.ToString());
                    detail.UseQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["UseQuantity"].Value.ToString());
                    detail.ClaimCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCD"].Value.ToString());
                    detail.ClaimRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimRD"].Value.ToString());
                    detail.ClaimSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimSD"].Value.ToString());
                    detail.ClaimVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimVAT"].Value.ToString());
                    detail.ClaimCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimCnFAmount"].Value.ToString());
                    detail.ClaimInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimInsuranceAmount"].Value.ToString());
                    detail.ClaimTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVBAmount"].Value.ToString());
                    detail.ClaimTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimTVAAmount"].Value.ToString());
                    detail.ClaimATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimATVAmount"].Value.ToString());
                    detail.ClaimOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["ClaimOthersAmount"].Value.ToString());
                    detail.SubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalDDBack"].Value.ToString());
                    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMCD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCD"].Value.ToString());
                    detail.UOMRD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMRD"].Value.ToString());
                    detail.UOMSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSD"].Value.ToString());
                    detail.UOMVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMVAT"].Value.ToString());
                    detail.UOMCnF = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMCnF"].Value.ToString());
                    detail.UOMInsurance = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMInsurance"].Value.ToString());
                    detail.UOMTVB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVB"].Value.ToString());
                    detail.UOMTVA = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMTVA"].Value.ToString());
                    detail.UOMATV = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMATV"].Value.ToString());
                    detail.UOMOthers = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMOthers"].Value.ToString());
                    detail.UOMSubTotalDDB = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMSubTotalDDB"].Value.ToString());
                    detail.Post = "Y";
                    detail.CreatedBy = Program.CurrentUser;
                    detail.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.LastModifiedBy = Program.CurrentUser;
                    detail.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.BranchId = Program.BranchId;
                    DetailVMs.Add(detail);

                }// End For



                if (DetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                bgwPost.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DutyDrawBackDAL DDBackDal = new DutyDrawBackDAL();
                IDutyDrawBack DDBackDal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);
                sqlResults = DDBackDal.DutyDrawBackPost(MasterVM, DetailVMs,connVM);
                UPDATE_DOWORK_SUCCESS = true;

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDutyDrawNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            //for (int i = 0; i < dgvSale.RowCount; i++)
                            //{
                            //    dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                            //}
                        }
                    }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                IsPost = false;

                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                    {

                        //ProductSearchDsFormLoad();

                        ClearAllFields();
                        //btnSave.Text = "&Add";
                        txtDutyDrawNo.Text = "~~~ New ~~~";

                    }
                }
                else if (ChangeData == false)
                {
                    //ProductSearchDsFormLoad();
                    //ProductSearchDsLoad();
                    ClearAllFields();
                    //btnSave.Text = "&Add";
                    txtDutyDrawNo.Text = "~~~ New ~~~";
                }
                IsUpdate = false;
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            #endregion
        }

        public void ClearAllFields()
        {
            cmbProductCode.Text = "Select";
            cmbProduct.Text = "Select";
            dptDutyDrawBackDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dptSalesDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            txtOldSaleId.Text = "";
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtCountry.Text = "";
            txtUOM.Text = "";
            txtPkSize.Text = "";
            txtQuantity.Text = "";
            txtCurrencyName.Text = "";
            txtExpCurr.Text = "";
            txtTotalPrice.Text = "";
            txtPurNo.Text = "";
            dptPurNoDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            txtBeNo.Text = "";
            cmbPurProductCode.Text = "Select";
            cmbPurProductName.Text = "Select";
            txtPurQuantity.Text = "";
            txtPurUnitPrice.Text = "";
            txtPurUom.Text = "";
            cmbUom.Text = "";
            txtUsedQty.Text = "0.00";
            txtSubTotal.Text = "";
            txtCd.Text = "";
            txtRd.Text = "";
            txtSd.Text = "";
            txtVat.Text = "";
            txtTotalQuantity.Text = "";
            txtTotalCD.Text = "";
            txtTotalRD.Text = "";
            txtTotalSD.Text = "";
            txtTotalDDBack.Text = "";
            txtComments.Text = "";
            txtCnFAmount.Text = "";
            txtTVBAmount.Text = "";
            txtTVAAmount.Text = "";
            txtATVAmount.Text = "";
            txtOthersAmount.Text = "";
            txtPurItem.Text = "";
            txtFgItem.Text = "";
            txtCustomerId.Text = "";
            txtExpCurrId.Text = "";
            txtPCode.Text = "";
            txtBOMPrice.Text = "";
            txtNBRPrice.Text = "";
            txtBOMPrice.Text = "";
            txtUomConv.Text = "";
            txtProductName.Text = "";
            txtTotalClaimInsuranceAmount.Text = "";
            txtTotalClaimTVBAmount.Text = "";
            txtTotalClaimTVAAmount.Text = "";
            txtTotalClaimATVAmount.Text = "";
            txtTotalClaimOthersAmount.Text = "";
            txtTotalVat.Text = "";
            dgvSale.Rows.Clear();
            dgvSaleHistory.Rows.Clear();
            txtApprovedSD.Text = "";
            txtSubTotalSD.Text = "";
        }

        private void FormDutyDrawBack_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                if (ChangeData == true)
                {
                    if (DialogResult.Yes != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to close without saving?",
                        this.Text,

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question,

                        MessageBoxDefaultButton.Button2))
                    {
                        e.Cancel = true;
                    }

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void btnVat24_Click(object sender, EventArgs e)
        {
            ChangeData = false;

            #region try
            try
            {
                string result;

                Program.fromOpen = "Me";
                #region Transaction Type

                FormRptVAT24 form = new FormRptVAT24();


                #endregion Transaction Type

                if (dgvSale.Rows.Count > 0)
                {
                    //frmRptVAT16.txtItemNo.Text = dgvSale.CurrentRow.Cells["ItemNo"].Value.ToString();

                    form.txtDutyDrawBackNo.Text = txtDutyDrawNo.Text.Trim();
                    form.txtItemName.Text = dgvSale.CurrentRow.Cells["FGName"].Value.ToString();
                    form.txtItemNo.Text = dgvSale.CurrentRow.Cells["FgItemNo"].Value.ToString();
                    form.txtSalesInvoiceNo.Text = txtOldSaleId.Text.Trim();
                }
                else
                {
                    MessageBox.Show("No Details Found to Preview.", form.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }



                //}


                //backgroundWorkerSearchDDBackNo.RunWorkerAsync();
                form.ShowDialog();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVat24_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVat24_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVat24_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVat24_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVat24_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVat24_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVat24_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVat24_Click", exMessage);
            }
            #endregion


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
       

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {

            string salesNo = "";
            try
            {
                #region Statement


                //DutyDrawBackDAL _dal = new DutyDrawBackDAL();

                //ReportResultDt = new DataTable();
                //ReportDSDAL reportDsdal = new ReportDSDAL();
                salesNo = txtOldSaleId.Text.Trim().Replace(",", "','");
                salesNo = "'" + salesNo + "'";

                //ReportResultDt = _dal.VAT7_1(txtDutyDrawNo.Text.Trim(), salesNo);

                NBRReports _report = new NBRReports();

                string dutyDrawNo = OrdinaryVATDesktop.SanitizeInput(txtDutyDrawNo.Text.Trim());
                string SalesNo = OrdinaryVATDesktop.SanitizeInput(salesNo);

                reportDocument = _report.VAT7_1(dutyDrawNo, SalesNo, connVM);

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                //ReportDocument objrpt = new ReportDocument();                // Start Complete

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();


                //if (ReportResultDt.Rows.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //ReportResultDt.TableName = "dtVAT7_1";
                //objrpt = new Rpt7_1();
                ////objrpt.Load(Program.ReportAppPath + @"\Rpt7_1.rpt");
                //objrpt.SetDataSource(ReportResultDt);

                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                ////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //// objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                ////objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }
                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void txtUsedQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSubTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtRd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVat_Leave(object sender, EventArgs e)
        {
            txtVat.Text = Program.ParseDecimalObject(txtVat.Text.Trim()).ToString();
            //Program.FormatTextBox(txtVat, "Vat");
            //if (e.KeyCode.Equals(Keys.Enter))
            //{
            //btnAdd.Focus();
            //}
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUsedQty_Leave(object sender, EventArgs e)
        {
            txtUsedQty.Text = Program.ParseDecimalObject(txtUsedQty.Text.Trim()).ToString();
            //Program.FormatTextBox(txtUsedQty, "UsedQty");

            if (chkCategory.Checked == false)
            {
                //    txtSubTotal.Focus();
                //}
                //else
                //{
                btnAdd.Focus();

            }

        }

        private void cmbProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                #region clear

                cmbPurProductCode.Text = "";
                cmbPurProductCode.Items.Clear();
                cmbPurProductName.Text = "";
                cmbPurProductName.Items.Clear();
                dptPurNoDate.Value = DateTime.Now.Date;
                txtBeNo.Text = "";
                txtPurQuantity.Text = "";
                txtPurUnitPrice.Text = "";
                txtPurUom.Text = "";
                txtUsedQty.Text = "";
                txtSubTotal.Text = "";
                txtCd.Text = "";
                txtRd.Text = "";
                txtSd.Text = "";
                txtVat.Text = "";
                txtPurNo.Text = "";

                txtPurQuantity.ReadOnly = false;
                txtPurUnitPrice.ReadOnly = false;
                txtPurUom.ReadOnly = false;
                txtUsedQty.ReadOnly = false;
                txtSubTotal.ReadOnly = false; //AV
                txtCd.ReadOnly = false;
                txtRd.ReadOnly = false;
                txtSd.ReadOnly = false;
                txtVat.ReadOnly = false;
                dptPurNoDate.Enabled = true;
                txtBeNo.ReadOnly = false;
                #endregion clear

                #region Setting

                if (Totalprice == "Y")
                {
                    lblUnitPrice.Text = "Total Price";
                }
                #endregion Setting
                #region Statement

                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                txtCatName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];
                chkCategory.Checked = true;

                //ProductSearchDsForManual();
                ProductSearchDsPurch();



                ////UOMSearch();

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnCategory_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCategory_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCategory_Click", exMessage);
            }
            #endregion
        }

        private bool ProductManualSearchUseCode()
        {
            bool isManualEntry = false;
            try
            {
                #region Statement

                txtPurItem.Text = "";
                var searchText = cmbPurProductCode.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in ProductsMiniS.ToList()
                                     where prd.ProductCode.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        cmbPurProductName.Text = products.ProductName;
                        cmbPurProductCode.Text = products.ProductCode;
                        txtPurUom.Text = products.UOM;
                        //cmbUom.Text = products.UOM;
                        txtPurUnitPrice.Text = "0";
                        txtPurQuantity.Text = "0";
                        txtSubTotal.Text = "0";
                        txtCd.Text = "0";
                        txtSd.Text = "0";
                        txtRd.Text = "0";
                        txtVat.Text = "0";
                        txtCnFAmount.Text = "0";
                        txtInsuranceAmount.Text = "0";
                        txtTVAAmount.Text = "0";
                        txtTVBAmount.Text = "0";
                        txtATVAmount.Text = "0";
                        txtOthersAmount.Text = "0";
                        txtPurItem.Text = products.ItemNo.ToString();
                        isManualEntry = true;
                    }
                    else
                    {
                        isManualEntry = false;
                    }
                }
                //Uoms();
                //txtUsedQty.Text = "00.00";

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }

            return isManualEntry;
            #endregion
        }

        private bool ProductManualSearchUseName()
        {
            bool isManualEntry = false;
            try
            {
                #region Statement

                txtPurItem.Text = "";
                var searchText = cmbPurProductName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByName = from prd in ProductsMiniS.ToList()
                                     where prd.ProductName.Trim().ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByName != null && prodByName.Any())
                    {
                        var products = prodByName.First();
                        cmbPurProductName.Text = products.ProductName;
                        cmbPurProductCode.Text = products.ProductCode;
                        txtPurUom.Text = products.UOM;
                        //cmbUom.Text = products.UOM;
                        txtPurUnitPrice.Text = "0";
                        txtPurQuantity.Text = "0";
                        txtSubTotal.Text = "0";
                        txtCd.Text = "0";
                        txtSd.Text = "0";
                        txtRd.Text = "0";
                        txtVat.Text = "0";
                        txtCnFAmount.Text = "0";
                        txtInsuranceAmount.Text = "0";
                        txtTVAAmount.Text = "0";
                        txtTVBAmount.Text = "0";
                        txtATVAmount.Text = "0";
                        txtOthersAmount.Text = "0";
                        txtPurItem.Text = products.ItemNo.ToString();
                        isManualEntry = true;
                    }
                    else
                    {
                        isManualEntry = false;
                    }
                }
                //Uoms();
                //txtUsedQty.Text = "00.00";

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }

            return isManualEntry;
            #endregion
        }

        private void txtPurQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
                //txtPurUnitPrice.Focus();
            }
        }

        private void txtPurUnitPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtPurUom_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPurUom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }

        }

        private void cmbPurProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rbtnPurCode_CheckedChanged(object sender, EventArgs e)
        {
            cmbPurProductCode.Enabled = true;
            cmbPurProductName.Enabled = false;
        }

        private void rbtnPurProduct_CheckedChanged(object sender, EventArgs e)
        {
            cmbPurProductCode.Enabled = false;
            cmbPurProductName.Enabled = true;
        }

        private void btnMultipleInvoice_Click(object sender, EventArgs e)
        {
            string invoices = "";
            try
            {




                FormSaleVAT20Multiple frm = new FormSaleVAT20Multiple();

                invoices = FormSaleVAT20Multiple.SelectOne();

                if (!string.IsNullOrWhiteSpace(invoices))
                {
                    dgvSaleHistory.Rows.Clear();
                    int j = 1;
                    DateTime firstsaledate = DateTime.Now;
                    DateTime firstsaledate1 = DateTime.Now;
                    for (int i = 0; i < invoices.Split('^').Length; i++)
                    {
                        string getValue = invoices.Split('^')[i];
                        if (!string.IsNullOrEmpty(getValue.Trim()))
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvSaleHistory.Rows.Add(NewRow);

                            dgvSaleHistory["SL", dgvSaleHistory.RowCount - 1].Value = j;
                            dgvSaleHistory["SalesInvoiceNoP", dgvSaleHistory.RowCount - 1].Value = getValue.ToString().Trim().Split('~')[0];
                            if (i == 0)
                            {
                                firstsaledate = Convert.ToDateTime(getValue.ToString().Trim().Split('~')[1]);
                                firstsaledate1 = Convert.ToDateTime(getValue.ToString().Trim().Split('~')[1]);

                            }
                            else
                            {
                                firstsaledate = Convert.ToDateTime(getValue.ToString().Trim().Split('~')[1]);
                            
                            }
                            if (firstsaledate<=firstsaledate1)
                            {
                                firstsaledate1 = firstsaledate;
                            }
                            dgvSaleHistory["SalesDate", dgvSaleHistory.RowCount - 1].Value = getValue.ToString().Trim().Split('~')[1];
                            j++;
                        }
                    }

                    dptSalesDate.Value = firstsaledate1;

                    MultipleInvoiceSelect();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }





        private void MultipleInvoiceSelect()
        {
            cmbProductCode.Text = "";
            cmbProductCode.Items.Clear();
            cmbProduct.Text = "";
            cmbProduct.Items.Clear();
            txtUOM.Text = "";
            txtPkSize.Text = "";
            txtQuantity.Text = "";
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtCountry.Text = "";
            txtCurrencyName.Text = "";
            txtExpCurr.Text = "";
            txtTotalPrice.Text = "";
            txtCustomerId.Text = "";
            txtExpCurrId.Text = "";
            decimal ExpCurr = 0;
            decimal TotalPrice = 0;
            txtExpCurr.Text = "";
            txtTotalPrice.Text = "";
            string invoiceNo = "";
            string multipleInvoiceNo = "";

            //SaleDAL _salDAL = new SaleDAL();
            ISale _salDAL = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);
            for (int i = 0; i < dgvSaleHistory.RowCount; i++)
            {
                DataTable dt = new DataTable();
                dt = _salDAL.SearchSalesHeaderDTNew(dgvSaleHistory["SalesInvoiceNoP", i].Value.ToString(),"",connVM);


                invoiceNo = "";
                invoiceNo = dt.Rows[0]["SalesInvoiceNo"].ToString().Trim();
                multipleInvoiceNo = multipleInvoiceNo + "," + invoiceNo;


                if (!string.IsNullOrEmpty(txtCustomerId.Text.Trim()) && txtCustomerId.Text.Trim() != dt.Rows[0]["CustomerID"].ToString().Trim())
                {
                    MessageBox.Show("Customer Not Same for these Invoices");
                    return;
                }
                else if (!string.IsNullOrEmpty(txtAddress.Text.Trim()) && txtAddress.Text.Trim() != dt.Rows[0]["DeliveryAddress1"].ToString().Trim())
                {
                    MessageBox.Show("Customer Delivery Address Not Same for these Invoices");
                    return;
                }
                else if (!string.IsNullOrEmpty(txtExpCurrId.Text.Trim()) && txtExpCurrId.Text.Trim() != dt.Rows[0]["CurrencyID"].ToString().Trim())
                {
                    MessageBox.Show("Export Currency Not Same for these Invoices");
                    return;
                }

                txtCustomerName.Text = dt.Rows[0]["CustomerName"].ToString().Trim();// selectedRow.Cells["CustomerName"].Value.ToString();
                txtCustomerId.Text = dt.Rows[0]["CustomerID"].ToString().Trim();//selectedRow.Cells["CustomerID"].Value.ToString();
                txtAddress.Text = dt.Rows[0]["DeliveryAddress1"].ToString().Trim();// selectedRow.Cells["DeliveryAddress1"].Value.ToString();
                txtCurrencyName.Text = dt.Rows[0]["CurrencyCode"].ToString().Trim();// selectedRow.Cells["CurrencyCode"].Value.ToString();
                txtExpCurrId.Text = dt.Rows[0]["CurrencyID"].ToString().Trim();// selectedRow.Cells["CurrencyID"].Value.ToString();

                //CustomerDAL dd = new CustomerDAL();
                ICustomer dd = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);
                DataTable CountryResultDs = new DataTable();
                CountryResultDs = dd.SearchCountry(txtCustomerName.Text.Trim(),connVM);
                if (!string.IsNullOrEmpty(txtCountry.Text.Trim()) && txtCountry.Text.Trim() != CountryResultDs.Rows[0]["Country"].ToString())
                {
                    MessageBox.Show("Export Country of Customer Not Same for these Invoices");
                    return;
                }
                else
                {
                    txtCountry.Text = CountryResultDs.Rows[0]["Country"].ToString();
                }

                //CurrenciesDAL _currenciesDAL = new CurrenciesDAL();
                ICurrencies _currenciesDAL = OrdinaryVATDesktop.GetObject<CurrenciesDAL, CurrenciesRepo, ICurrencies>(OrdinaryVATDesktop.IsWCF);
                DataTable CurrencyResultDs = new DataTable();
                CurrencyResultDs = _currenciesDAL.SearchCurrency(invoiceNo,connVM);

                ExpCurr = ExpCurr + Convert.ToDecimal(CurrencyResultDs.Rows[0]["SubTotal"].ToString());
                TotalPrice = TotalPrice + Convert.ToDecimal(CurrencyResultDs.Rows[0]["CurrencyValue"].ToString());



                if (dt.Rows[0]["IsPrint"].ToString().Trim().ToLower() != "y")
                {
                    MessageBox.Show("You need to select only Printed");
                    return;
                }

            }
            ProductDAL productDal = new ProductDAL();
            if (!string.IsNullOrWhiteSpace(multipleInvoiceNo))
            {
                multipleInvoiceNo = multipleInvoiceNo.Substring(1);
                multipleInvoiceNo = multipleInvoiceNo.Replace(",", "','");
                multipleInvoiceNo = "'" + multipleInvoiceNo + "'";
            }

            txtOldSaleId.Text = multipleInvoiceNo;

            ProductSearchDsFormLoad();
            txtExpCurr.Text = Program.ParseDecimalObject(ExpCurr.ToString());
            txtTotalPrice.Text =Program.ParseDecimalObject( TotalPrice.ToString());


        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();

            //Program.FormatTextBox(txtQuantity, "Quantity");
        }

        private void txtExpCurr_Leave(object sender, EventArgs e)
        {
            txtExpCurr.Text = Program.ParseDecimalObject(txtExpCurr.Text.Trim()).ToString();

            //Program.FormatTextBox(txtExpCurr, "ExpCurr");
        }

        private void txtTotalPrice_Leave(object sender, EventArgs e)
        {
            txtTotalPrice.Text = Program.ParseDecimalObject(txtTotalPrice.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalPrice, "TotalPrice");
        }

        private void txtPurQuantity_Leave(object sender, EventArgs e)
        {
            txtPurQuantity.Text = Program.ParseDecimalObject(txtPurQuantity.Text.Trim()).ToString();
            //Program.FormatTextBox(txtPurQuantity, "PurQuantity");
        }

        private void txtPurUnitPrice_Leave(object sender, EventArgs e)
        {
            txtPurUnitPrice.Text = Program.ParseDecimalObject(txtPurUnitPrice.Text.Trim()).ToString();
            //Program.FormatTextBox(txtPurUnitPrice, "PurUnitPrice");
        }

        private void txtSubTotal_Leave(object sender, EventArgs e)
        {
            txtSubTotal.Text = Program.ParseDecimalObject(txtSubTotal.Text.Trim()).ToString();
            //Program.FormatTextBox(txtSubTotal, "SubTotal");
        }

        private void txtCd_Leave(object sender, EventArgs e)
        {
            txtCd.Text = Program.ParseDecimalObject(txtCd.Text.Trim()).ToString();
            //Program.FormatTextBox(txtCd, "Cd");
        }

        private void txtRd_Leave(object sender, EventArgs e)
        {
            txtRd.Text = Program.ParseDecimalObject(txtRd.Text.Trim()).ToString();
            //Program.FormatTextBox(txtRd, "Rd");
        }

        private void txtSd_Leave(object sender, EventArgs e)
        {
            txtSd.Text = Program.ParseDecimalObject(txtSd.Text.Trim()).ToString();
            //Program.FormatTextBox(txtSd, "Sd");
        }

        private void txtTotalQuantity_Leave(object sender, EventArgs e)
        {
            txtTotalQuantity.Text = Program.ParseDecimalObject(txtTotalQuantity.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalQuantity, "TotalQuantity");
        }

        private void txtTotalCD_Leave(object sender, EventArgs e)
        {
            txtTotalCD.Text = Program.ParseDecimalObject(txtTotalCD.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalCD, "TotalCD");
        }

        private void txtTotalRD_Leave(object sender, EventArgs e)
        {
            txtTotalRD.Text = Program.ParseDecimalObject(txtTotalRD.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalRD, "TotalRD");
        }

        private void txtTotalSD_Leave(object sender, EventArgs e)
        {
            txtTotalSD.Text = Program.ParseDecimalObject(txtTotalSD.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalSD, "TotalSD");
        }

        private void txtTotalDDBack_Leave(object sender, EventArgs e)
        {
            txtTotalDDBack.Text = Program.ParseDecimalObject(txtTotalDDBack.Text.Trim()).ToString();
            //Program.FormatTextBox(txtTotalDDBack, "TotalDDBack");
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void txtApprovedSD_Leave(object sender, EventArgs e)
        {

            try
            {
                Program.FormatTextBox(txtApprovedSD, "ApprovedSD");
                decimal ApprovedSDAmount = 0;
                decimal TotalSDAmount = 0;

                ApprovedSDAmount = Convert.ToDecimal(txtApprovedSD.Text.Trim());
                TotalSDAmount = Convert.ToDecimal(txtTotalSD.Text.Trim());


                if (ApprovedSDAmount > TotalSDAmount)
                {

                    MessageBox.Show("Approved SD Can't be bigger than Total Claim SD", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
