using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;

using VATClient.ReportPages;

using VATClient.ModelDTO;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.Odbc;
using VATServer.License;

namespace VATClient
{
    public partial class FormSaleOld : Form
    {
        #region Constructors

        public FormSaleOld()
        {
            InitializeComponent();
            //dgvSale.DefaultCellStyle.Font = new Font("SutonnyMJ", 12, FontStyle.Italic);


            // DefaultCellStyle.Font.Name= "SutonnyMJ";
        }

        #endregion

        #region Global Variables

        private List<UomDTO> UOMs = new List<UomDTO>();
        private List<CurrencyConversionVM> CurrencyConversions = new List<CurrencyConversionVM>();

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private bool PrepaidVAT;
        private bool ChangeableNBRPrice;
        private bool VAT11Letter;
        private bool VAT11A4;
        private bool VAT11Legal;

        private bool LocalInVAT1;
        private bool LocalInVAT1KaTarrif;
        private bool TenderInVAT1;
        private bool TenderInVAT1Tender;
        private bool TenderPriceWithVAT;
        private decimal vNBRPrice;
        
        
        private DateTime tenderDate;
        private int numberOfItems;
        private bool UPDATEPRINT_DOWORK_SUCCESS = false;
        private bool TabFromCode = true;
        private string ItemNature = string.Empty;
        private string VAT11PageSize = string.Empty;
        private List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private List<CustomerSinmgleDTO> customerMini = new List<CustomerSinmgleDTO>();
        private List<VehicleDTO> vehicles = new List<VehicleDTO>();

        private SaleMasterVM saleMaster = new SaleMasterVM();
        private List<SaleDetailVm> saleDetails = new List<SaleDetailVm>();
        private List<SaleExportVM> saleExport = new List<SaleExportVM>();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private int SalePlaceQty;
        private int SalePlaceTaka;
        private int SalePlaceDollar;
        private int SalePlaceRate;
        private string transactionType = string.Empty;
        private string transactionTypeOld = string.Empty;
        private string vSalePlaceQty, vChangeableNBRPrice, vSalePlaceTaka, vSalePlaceDollar, vSalePlaceRate, vItemNature,
                   vPrepaidVAT, vNumberofItems, vVAT11Letter, vVAT11A4, vVAT11Legal,
                   vLocalInVAT1, vLocalInVAT1KaTarrif, vTenderInVAT1, vTenderInVAT1Tender, vIs3Plyer, vTenderPriceWithVAT,vNegativeStock,
                   vCreditWithoutTransaction = string.Empty;

        private bool NegativeStock;
        private bool CreditWithoutTransaction;
        private List<BomOhDTO> BomOHs = new List<BomOhDTO>();

        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();

        private string PrintLocation=String.Empty;
        private string vPrintCopy=String.Empty;
        private int PrintCopy=1;
        private string WantToPrint="N";
        private int AlReadyPrintNo;
        private int NewPrintCopy;
        private string Is3Plyer = string.Empty;
        private string VPrinterName = string.Empty;
        private string CurrencyConversiondate = string.Empty;
        private string ProductCode = string.Empty;
        private string ProductName = string.Empty;
        #region Currency Variable

        //private string currencyFromID;
        //private string currencyBDTID = "260";
        //private string currencyUSDID = "249";
        //private decimal CurrencyRateFromBDT = 0;
        //private decimal dollerRate = 0;
        private string currencyConversionDate;
        #endregion Currency Variable

        #region variable

        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string HSCodeNoParam = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";

        private string customerCode = string.Empty;
        private string customerName = string.Empty;
        private string customerGId = string.Empty;
        private string customerGName = string.Empty;
        private string tinNo = string.Empty;
        private string customerVReg = string.Empty;
        private string activeStatusCustomer = string.Empty;
        private string CGType = "Local";

        private DataTable CustomerResult;

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

        private DataTable TenderSearchResult;


        private DataTable ProductResultDs;
        private bool ChangeData = false;
        private bool Post = false;
        private bool IsPost = false;
        private bool PreviewOnly = false;
        private bool IsUpdate = false;
        private string NextID = "";
        private decimal PreVATAmount = 0;
        private string item;
        private decimal cost;
        private decimal NBRPriceWithVAT = 0;

        private string CategoryId { get; set; }


        private DataTable SaleDetailResult;

        private string SaleDetailData = string.Empty;

        private bool ImportByCustGrp;



        #endregion variable

        #region Global Variables For BackGroundWorker

        //arafat

        //string SalesInvoiceHeaderData = string.Empty;
        //string encriptedSalesInvoiceHeaderData = string.Empty;

        //string SalesInvoiceDetailData = string.Empty;
        //string SalesInvoiceExportData = string.Empty;

        private DataSet StatusResult;
        private DateTime varInvoiceDate;
        private DataSet ReportResultVAT11;
        private DataSet ReportResultVAT20;
        private DataSet ReportResultCreditNote;
        private DataSet ReportResultDebitNote;
        private DataSet ReportResultDelivry;
        ReportDSDAL reportDsdal = new ReportDSDAL();

        private string varSalesInvoiceNo = string.Empty;
        private string varTrading = string.Empty;
        private string post1, post2;
        private bool Add = false;
        private bool Edit = false;
        private string ReturnTransType = string.Empty;
        //private string CustomerGrpID = string.Empty;
        //private string CustomerGrpName = string.Empty;
        private DataSet ReportResultTracking;
        #endregion 

        #region Vabiables for tracking
        List<TrackingCmbDTO> trackingsCmb = new List<TrackingCmbDTO>();
        List<TrackingVM> trackingsVm = new List<TrackingVM>();
        private bool TrackingTrace;
        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;

        #endregion                             

        string ImportExcelID = string.Empty;
        #endregion

        private void FormSale_Load(object sender, EventArgs e)
        {

            try
            {

                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchInvoiceNo, "Existing information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchCustomer, "Customer");
                ToolTip1.SetToolTip(this.btnSearchVehicle, "Vehicle");
                ToolTip1.SetToolTip(this.btnProductType, "Product Type");

                ToolTip1.SetToolTip(this.btnOldID, "Orginal Sales invoice no");
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");

                ClearAllFields();
                VATName vname = new VATName();
                cmbVAT1Name.DataSource = vname.VATNameList;
                
                FormMaker();
               
                FormLoad();
                txtSalesInvoiceNo.Text = "~~~ New ~~~";
                //if(!rbtnExport.Checked)
                //txtNBRPrice.ReadOnly = true;
                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
               this.Enabled = false;
                bgwLoad.RunWorkerAsync();

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormSale_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormSale_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormSale_Load", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void FormLoad()
        {
            

            #region Customer
            
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
            varInvoiceDate =Convert.ToDateTime(dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd"));

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

            }
            else if (rbtnExport.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Export";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Export";
            }//start112
            else if (rbtnOther.Checked || rbtnDN.Checked || rbtnCN.Checked
                || rbtnTender.Checked || rbtnTradingTender.Checked
                || rbtnExport.Checked || rbtnPackageSale.Checked) // start other
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Finish";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Finish";

            }
            else if (rbtnTollFinishIssue.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "OverHead";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "OverHead";


            }
            else if (rbtnTrading.Checked)
            {


                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Trading";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Trading";
            }
            else if (rbtnService.Checked || rbtnServiceNS.Checked) //start
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Service";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Service";


            }
            else if (rbtnInternalIssue.Checked || rbtnTollIssue.Checked || rbtnVAT11GaGa.Checked) //transfer
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Raw";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Raw";


            }



            #endregion Product


            #region UOM
            //UOMIdParam = string.Empty;
            //UOMFromParam = string.Empty;
            //UOMToParam = string.Empty;
            //ActiveStatusUOMParam = string.Empty;

            UOMIdParam = string.Empty;
            UOMFromParam = string.Empty;
            UOMToParam = string.Empty;
            ActiveStatusUOMParam = "Y";
            #endregion UOM


        }
        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Product
                ProductResultDs = new DataTable();
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                    ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam);

                #endregion Product

                #region Customer
                CustomerResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();

                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName, tinNo, customerVReg, activeStatusCustomer, CGType };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName like", "c.TINNo like", "c.VATRegistrationNo like", "c.ActiveStatus like", "cg.GroupType like" };
                CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false);

                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                //customerGName, tinNo, customerVReg, activeStatusCustomer, CGType); // Change 04

                #endregion Customer


                #region vehicle

                vehicleResult = new DataTable();
                VehicleDAL vehicleDal = new VehicleDAL();
                //vehicleResult = vehicleDal.SearchVehicleDataTable(vehicleId, vehicleType, vehicleNo, vehicleActiveStatus);
                string[] cValue = { vehicleId, vehicleType, vehicleNo, vehicleActiveStatus };
                string[] cField = { "VehicleID like", "VehicleType like", "VehicleNo like", "ActiveStatus like" };
                vehicleResult = vehicleDal.SelectAll(0, cField, cValue, null, null, true);
                #endregion vehicle

                #region SetupVATStatus
                SetupDAL setupDal = new SetupDAL();
                StatusResult = setupDal.ResultVATStatus(varInvoiceDate, Program.DatabaseName);

                #endregion SetupVATStatus

                #region UOM
                UOMDAL uomdal = new UOMDAL();

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

                #region CurrencyConversion



                CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();

                //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
                //     string.Empty, "Y", dtpInvoiceDate.Value.ToString("MMM/dd/yyyy HH:mm:ss"));

                string[] cValu = new string[] { "Y", dtpConversionDate.Value.ToString("MMM/dd/yyyy HH:mm:ss") };
                string[] cFilds= new string[] { "cc.ActiveStatus like", "cc.ConversionDate" };
                CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFilds, cValu, null, null, false);

                //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
                //     string.Empty, "Y", dtpConversionDate.Value.ToString("MMM/dd/yyyy HH:mm:ss"));



                #endregion CurrencyConversion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion

        }
        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region region

                #region Product
                ProductsMini.Clear();
                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    if (rbtnTender.Checked != true)
                    {

                        cmbProductCode.Items.Add(item2["ProductCode"]);
                        cmbProduct.Items.Add(item2["ProductName"]);

                    }

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


                    //bool TradingF = false;
                    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                    //prod.Trading = Trading;

                    //bool NonStockF = false;
                    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                    //prod.NonStock = NonStock;


                    ProductsMini.Add(prod);
                }
                #endregion Product
                
                #region Customer
                customerMini.Clear();
                cmbCustomerName.Items.Clear();
                foreach (DataRow item2 in CustomerResult.Rows)
                {
                    var customer = new CustomerSinmgleDTO();
                    customer.CustomerID = item2["CustomerID"].ToString();
                    customer.CustomerName = item2["CustomerName"].ToString();
                    customer.GroupType = item2["GroupType"].ToString();
                    customer.CustomerCode = item2["CustomerCode"].ToString();
                    customer.CustomerGroupID = item2["CustomerGroupID"].ToString();
                    customer.CustomerGroupName = item2["CustomerGroupName"].ToString();
                    customer.Address1 = item2["Address1"].ToString();
                    customer.Address2 = item2["Address2"].ToString();
                    customer.Address3 = item2["Address3"].ToString();
                    customer.City = item2["City"].ToString();
                    customer.TelephoneNo = item2["TelephoneNo"].ToString();
                    customer.FaxNo = item2["FaxNo"].ToString();
                    customer.Email = item2["Email"].ToString();
                    customer.TINNo = item2["TINNo"].ToString();
                    customer.VATRegistrationNo = item2["VATRegistrationNo"].ToString();
                    customer.ActiveStatus = item2["ActiveStatus"].ToString();
                    customer.Country = item2["Country"].ToString();
                    customerMini.Add(customer);

                }
                #region CGroup
                if (rbtnExport.Checked == false)
                {
                    if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                    {
                        cmbCGroup.Items.Clear();
                        var vcmbCGroup = customerMini.Where(x => x.GroupType == "Local").Select(x => x.CustomerGroupName).Distinct().ToList();

                        if (vcmbCGroup.Any())
                        {
                            cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                        }
                        cmbCGroup.Items.Insert(0, "Select");
                        cmbCGroup.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                    {
                        cmbCGroup.Items.Clear();
                        var vcmbCGroup = customerMini.Where(x => x.GroupType == "Export").Select(x => x.CustomerGroupName).Distinct().ToList();

                        if (vcmbCGroup.Any())
                        {
                            cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                        }
                        cmbCGroup.Items.Insert(0, "Select");
                        cmbCGroup.SelectedIndex = 0;
                    }
                }

                #endregion CGroup

                customerloadToCombo();
                #endregion Customer

                #region vehicle
                vehicles.Clear();
                cmbVehicleNo.Items.Clear();

                DataRow[] newVehicles = vehicleResult.Select("ActiveStatus='Y'");
                foreach (DataRow item2 in newVehicles)
                {
                    var vehicle = new VehicleDTO();
                    vehicle.VehicleID = item2["VehicleID"].ToString();
                    vehicle.VehicleType = item2["VehicleType"].ToString();
                    vehicle.VehicleNo = item2["VehicleNo"].ToString();
                    vehicle.Description = item2["Description"].ToString();
                    vehicle.Comments = item2["Comments"].ToString();
                    vehicle.ActiveStatus = item2["ActiveStatus"].ToString();
                    cmbVehicleNo.Items.Add(item2["VehicleNo"]);
                    vehicles.Add(vehicle);
                }
                cmbVehicleNo.Items.Insert(0, "Select");
                #endregion vehicle

                #region SetupVATStatus
                if (StatusResult != null && StatusResult.Tables.Count > 0 && StatusResult.Tables[0].Rows.Count > 0)
                {
                    txtTDBalance.Text = Convert.ToDecimal(StatusResult.Tables[0].Rows[0][0]).ToString();//"0.00");
                    Program.VATAmount = Convert.ToDecimal(StatusResult.Tables[0].Rows[0][0]);
                }

                #endregion SetupVATStatus


                #region UOM
                UOMs.Clear();
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
                //foreach (DataRow item2 in uomResult.Tables[1].Rows)
                //{
                //    cmbUom.Items.Add(item2["UOMCode"].ToString());
                //}

                #endregion UOM

                #region CurrencyConversion


                
                CurrencyConversions.Clear();

                foreach (DataRow item2 in CurrencyConversionResult.Rows)
                {
                    var cConversion = new CurrencyConversionVM();
                    cConversion.CurrencyConversionId = item2["CurrencyConversionId"].ToString();
                    cConversion.CurrencyCodeFrom = item2["CurrencyCodeFrom"].ToString();
                    cConversion.CurrencyCodeF = item2["CurrencyCodeF"].ToString();
                    cConversion.CurrencyNameF = item2["CurrencyNameF"].ToString();
                    cConversion.CurrencyCodeTo = item2["CurrencyCodeTo"].ToString();
                    cConversion.CurrencyCodeT = item2["CurrencyCodeT"].ToString();
                    cConversion.CurrencyNameT = item2["CurrencyNameT"].ToString();
                    cConversion.CurrencyRate = Convert.ToDecimal(item2["CurrencyRate"].ToString());
                    cConversion.Comments = item2["Comments"].ToString();
                    cConversion.ActiveStatus = item2["ActiveStatus"].ToString();
                    cConversion.ConvertionDate = item2["ConversionDate"].ToString();
                    CurrencyConversions.Add(cConversion);

                }
                if (CurrencyConversionResult != null || CurrencyConversionResult.Rows.Count > 0)
                {

                    cmbCurrency.Items.Clear();
                    var cmbSCurrencyCodeF = CurrencyConversions.Select(x => x.CurrencyCodeF).Distinct().ToList();

                    if (cmbSCurrencyCodeF.Any())
                    {
                        cmbCurrency.Items.AddRange(cmbSCurrencyCodeF.ToArray());
                    }
                   
                    //cmbCurrency.Items.Insert(0, "USD");
                    cmbCurrency.Text = "USD";
                    string cDate =
                        CurrencyConversions.Where(x => x.CurrencyCodeF == "USD").Select(x => x.ConvertionDate).
                            FirstOrDefault();

                    dtpConversionDate.Value = Convert.ToDateTime(cDate);
                }
                if (rbtnExport.Checked == false)
                {
                    cmbCurrency.Items.Clear();
                    //cmbCurrency.Items.Insert(0, "BDT");
                    cmbCurrency.Text = "BDT";
                    string cDate =
                        CurrencyConversions.Where(x => x.CurrencyCodeF == "BDT").Select(x => x.ConvertionDate).
                            FirstOrDefault();

                    dtpConversionDate.Value = Convert.ToDateTime(cDate);

                }




                #endregion CurrencyConversion

                #endregion region
                txtNBRPrice.ReadOnly = ChangeableNBRPrice==true? false:true;

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.Enabled = true;
                this.progressBar1.Visible = false;
               
                //cmbCurrency.Focus();
                CurrencyValue();
                ChangeData = false;


            }
        }

        private void customerloadToCombo()
        {
            try
            {
                if (string.IsNullOrEmpty(cmbCGroup.Text.Trim().ToLower()))
                {
                    MessageBox.Show("Please select Customer Group first", this.Text);
                    return;
                }
                if (chkSameCustomer.Checked==false)
                {

               
                cmbCustomerName.Items.Clear();

                if (chkCustCode.Checked == true)
                {

                    var CByCode = from cus in customerMini.ToList()
                                  where cus.CustomerGroupName.ToLower() == cmbCGroup.Text.Trim().ToLower()
                                  orderby cus.CustomerCode
                                  select cus.CustomerCode;


                    if (CByCode != null && CByCode.Any())
                    {
                        cmbCustomerName.Items.AddRange(CByCode.ToArray());
                    }
                }
                else
                {
                    var CByName = from cus in customerMini.ToList()
                                  where cus.CustomerGroupName.ToLower() == cmbCGroup.Text.Trim().ToLower()
                                  orderby cus.CustomerName
                                  select cus.CustomerName;


                    if (CByName != null && CByName.Any())
                    {
                        cmbCustomerName.Items.AddRange(CByName.ToArray());
                    }
                }

                cmbCustomerName.Items.Insert(0, "Select");
            }
        }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
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
                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
            }
            #endregion Catch

        }

        private void bgwUOM_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                UOMDAL uomdal = new UOMDAL();
                //uomResult = uomdal.SearchUOMNew(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUOM_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUOM_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUOM_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUOM_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUOM_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUOM_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUOM_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUOM_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwUOM_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

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
                ////UOMs = (from DataRow row in uomResult.Rows
                ////        select new UomDTO()
                ////        {
                ////            UOMId = row["UOMId"].ToString(),
                ////            UOMFrom = row["UOMFrom"].ToString(),
                ////            UOMTo = row["UOMTo"].ToString(),
                ////            Convertion = Convert.ToDecimal(row["Convertion"].ToString()),
                ////            CTypes = row["CTypes"].ToString()
                ////            cmbUom.Items


                ////        }
                ////           ).ToList();
                ////if (UOMs != null && UOMs.Any())
                ////{
                ////    var uoms = from uom in UOMs.ToList()
                ////               select uom.UOMTo;
                ////    cmbUom.Items.AddRange(uoms.ToArray());
                ////}
                #endregion UOM

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUOM_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                //this.Enabled = true;
                cmbUom.Enabled = true;
                this.progressBar1.Visible = false;
         

            }
        }

        private void FormMaker()
        {
            try
            {

                #region Settings
                PrepaidVAT = false;
                numberOfItems = 0;
                CommonDAL commonDal = new CommonDAL();

                vChangeableNBRPrice = commonDal.settingsDesktop("Sale", "ChangeableNBRPrice");
                vSalePlaceQty = commonDal.settingsDesktop("Sale", "QuantityDecimalPlace");
                vSalePlaceTaka = commonDal.settingsDesktop("Sale", "TakaDecimalPlace");
                vSalePlaceDollar = commonDal.settingsDesktop("Sale", "DollerDecimalPlace");
                vSalePlaceRate = commonDal.settingsDesktop("Sale", "RateDecimalPlace");
                vPrepaidVAT = commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT");
                vNumberofItems = commonDal.settingsDesktop("Sale", "NumberOfItems");
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature");
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter");
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4");
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal");
                //PrintLocation = commonDal.settingsDesktop("Sale", "ReportSaveLocation");
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint");

                vLocalInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3");
                vLocalInVAT1KaTarrif = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3Ka(Tarrif)");
                vTenderInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3");
                vTenderInVAT1Tender = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3(Tender)");
                vIs3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer");
                //for tender with vat
                vTenderPriceWithVAT = commonDal.settingsDesktop("PriceDeclaration", "TenderPriceWithVAT");
                vNegativeStock = commonDal.settingsDesktop("Sale", "NegStockAllow");
                // for import credit info
                vCreditWithoutTransaction = commonDal.settingsDesktop("Sale", "CreditWithoutTransaction");
                string vCustGrp = commonDal.settingsDesktop("ImportTender", "CustomerGroup");
               
               


                if (string.IsNullOrEmpty(vSalePlaceQty)
                    || string.IsNullOrEmpty(vSalePlaceTaka)
                    || string.IsNullOrEmpty(vChangeableNBRPrice)
                    || string.IsNullOrEmpty(vSalePlaceDollar)
                    || string.IsNullOrEmpty(vSalePlaceRate)
                    || string.IsNullOrEmpty(vPrepaidVAT)
                    || string.IsNullOrEmpty(vNumberofItems)
                    || string.IsNullOrEmpty(vVAT11Letter)
                    || string.IsNullOrEmpty(vItemNature)
                    || string.IsNullOrEmpty(vVAT11Legal)
                    || string.IsNullOrEmpty(vVAT11A4)
                    || string.IsNullOrEmpty(vPrintCopy)

                    || string.IsNullOrEmpty(vLocalInVAT1)
                    || string.IsNullOrEmpty(vLocalInVAT1KaTarrif)
                    || string.IsNullOrEmpty(vTenderInVAT1)
                    || string.IsNullOrEmpty(vTenderInVAT1Tender)
                    || string.IsNullOrEmpty(vIs3Plyer)
                    || string.IsNullOrEmpty(vTenderPriceWithVAT)
                    || string.IsNullOrEmpty(vNegativeStock)
                    || string.IsNullOrEmpty(vCreditWithoutTransaction)
                   || string.IsNullOrEmpty(vCustGrp)

                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                SalePlaceQty = Convert.ToInt32(vSalePlaceQty);
                SalePlaceTaka = Convert.ToInt32(vSalePlaceTaka);
                SalePlaceDollar = Convert.ToInt32(vSalePlaceDollar);
                SalePlaceRate = Convert.ToInt32(vSalePlaceRate);
                numberOfItems = int.Parse(vNumberofItems);
                PrintCopy = Convert.ToInt32(vPrintCopy);
                PrepaidVAT = Convert.ToBoolean(vPrepaidVAT == "Y" ? true : false);
                ChangeableNBRPrice = Convert.ToBoolean(vChangeableNBRPrice == "Y" ? true : false);
                VAT11Letter = Convert.ToBoolean(vVAT11Letter == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(vVAT11A4 == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(vVAT11Legal == "Y" ? true : false);

                LocalInVAT1 = Convert.ToBoolean(vLocalInVAT1 == "Y" ? true : false);
                LocalInVAT1KaTarrif = Convert.ToBoolean(vLocalInVAT1KaTarrif == "Y" ? true : false);
                TenderInVAT1 = Convert.ToBoolean(vTenderInVAT1 == "Y" ? true : false);
                TenderInVAT1Tender = Convert.ToBoolean(vTenderInVAT1Tender == "Y" ? true : false);
                Is3Plyer = vIs3Plyer;
                TenderPriceWithVAT = Convert.ToBoolean(vTenderPriceWithVAT == "Y" ? true : false);
                NegativeStock = Convert.ToBoolean(vNegativeStock == "Y" ? true : false);
                CreditWithoutTransaction = Convert.ToBoolean(vCreditWithoutTransaction == "Y" ? true : false);
                ImportByCustGrp = Convert.ToBoolean(vCustGrp == "Y" ? true : false);
                
                ItemNature = vItemNature;
                txtNBRPrice.ReadOnly = ChangeableNBRPrice;
                if (VAT11A4 == true)
                {
                    VAT11PageSize = "A4";
                }
                else if (VAT11Letter == true)
                {
                    VAT11PageSize = "Letter";
                }
                else if (VAT11Legal == true)
                {
                    VAT11PageSize = "Legal";
                }
                else
                {
                    VAT11PageSize = "A4";
                }

                string vTracking = string.Empty;
                string vHeading1, vHeading2 = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking");
                vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1");
                vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2");

                if (string.IsNullOrEmpty(vTracking) || string.IsNullOrEmpty(vHeading1) || string.IsNullOrEmpty(vHeading2))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

                if (TrackingTrace == true)
                {
                    btnTracking.Visible = true;
                    Heading1 = vHeading1;
                    Heading2 = vHeading2;
                }
                else
                {
                    btnTracking.Visible = false;
                }


                #endregion Settings
                #region Transaction Type

                #region Close
                rbtnCode.Checked = true;
                txtLCNumber.Visible = false;
                //btnPSale.Text = "Sale";
                this.Text = "Local Sales";
                btnPrint.Text = "11";
                label22.Visible = true;
                btnExport.Visible = false;
                
                btn20.Visible = false;
                btnDebitCredit.Top = btnPrint.Top;
                btnDebitCredit.Left = btnPrint.Left;
                btnTenderNew.Visible = false;
                txtLineNo.Visible = false;
                labelLC.Visible = false;
                label18.Visible = false;
                txtTenderStock.Visible = false;

                btnSearchCustomer.Enabled = true;
                btnProductType.Enabled = true;
                //cmbVAT1Name.Visible = false; start
                label1.Visible = false;
                //cmbVAT1Name.SelectedIndex = 0;
                btnDebitCredit.Visible = false;
                chkIs11.Visible = false;
                chkIsBlank.Visible = false;
                //chkServiceStock.Visible = false;
                //txtNBRPrice.ReadOnly = false; //st

                txtQuantity.ReadOnly = false;
                txtQuantity.Text = "0";
                ChkExpLoc.Visible = false;
                btnFormKaT.Visible = false;
                #endregion Close
                #region  rbtnOther
                if (rbtnOther.Checked)
                {
                    chkIsBlank.Visible = true;
                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;

                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";

                    //cmbVAT1Name.Visible = true;
                    label1.Visible = true;
                    if (LocalInVAT1==true)
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }
                    else if (LocalInVAT1KaTarrif == true)
                    {
                        cmbVAT1Name.SelectedIndex = 1;
                    } 
                    else
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }

                    this.Text = "Sales Entry";

                }
                #endregion  rbtnOther
                #region  PackageSale
                else if (rbtnPackageSale.Checked)
                {
                    chkIsBlank.Visible = true;
                    ChkExpLoc.Visible = true;

                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;

                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";

                   
                    label1.Visible = true;
                
                    cmbVAT1Name.SelectedIndex = 9;

                    this.Text = "Sales Entry(Package)";

                }
                #endregion  PackageSale
                #region  rbtnTollFinishIssue

                else if (rbtnTollFinishIssue.Checked)
                {
                    chkIsBlank.Visible = true;
                    cmbType.Text = "New";
                    chkNonStock.Checked = true;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;

                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 6.3";

                    //cmbVAT1Name.Visible = true;
                    //cmbVAT1Name.Enabled = false;
                    //cmbVAT1Name.Items.Add("VAT 4.3 (Toll Issue)");
                    //cmbVAT1Name.Text = "VAT 4.3 (Toll Issue)";
                    label1.Visible = true;
                    //cmbVATType.SelectedIndex =0;
                    cmbVATType.Enabled = false;
                    this.Text = "Toll Finish Item Issue to Client";
                    cmbVAT1Name.SelectedIndex = 6;



                }
                #endregion  rbtnTollFinishIssue
                #region  rbtnTollIssue

                else if (rbtnTollIssue.Checked)
                {
                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;

                    ////
                    this.Text = "Toll Issue Entry";
                    //btnPSale.Text = "Toll Issue";
                    btnPrint.Text = "VAT 11 Ga";
                    cmbVATType.Text = "Non VAT";
                    txtSD.Visible = false;
                    txtVATRate.Visible = false;
                    LSD.Visible = false;
                    LVat.Visible = false;
                    cmbVATType.Enabled = false;
                    btnVAT17.Visible = false;
                    btnVAT18.Visible = false;
                    btnVAT16.Visible = true;
                    cmbVAT1Name.Visible = false;
                    label1.Visible = false;
                    //cmbVAT1Name.SelectedIndex = 6;

                    dgvSale.Columns["VATRate"].Visible = false;
                    dgvSale.Columns["VATAmount"].Visible = false;
                    dgvSale.Columns["SD"].Visible = false;
                    dgvSale.Columns["SDAmount"].Visible = false;
                }
                #endregion  rbtnTollIssue
                #region  rbtnDN

                else if (rbtnDN.Checked)
                {
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 12 Ka";

                    cmbType.Text = "Debit";
                    label2.Text = "Debit No";
                    cmbVATType.Text = "VAT";
                    btnDebitCredit.Visible = true;
                    //cmbVAT1Name.Visible = true;
                    label1.Visible = true;
                    cmbVAT1Name.SelectedIndex = 0;
                    //if (CreditWithoutTransaction == true)
                    //{
                    //    txtOldID.ReadOnly = false;
                    //    btnAdd.Enabled = true;
                    //    btnProductType.Enabled = true;
                    //}
                    //else
                    //{
                        btnAdd.Enabled = false;
                        btnProductType.Enabled=false;
                    //}
                    
                    this.Text = "Sales Debit Note Entry";

                }
                #endregion  rbtnDN
                #region  rbtnCN

                else if (rbtnCN.Checked)
                {
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 12";

                    cmbType.Text = "Credit";
                    label2.Text = "Credit No";
                    cmbVATType.Text = "VAT";
                    //btnDebitCredit.Visible = true;
                    //btnPrint.Visible = false;
                    //btnDebitCredit.Text = "VAT 12";
                    //cmbVAT1Name.Visible = true;
                    label1.Visible = true;
                    cmbVAT1Name.SelectedIndex = 0;
                    //if (CreditWithoutTransaction == true)
                    //{
                    //    txtOldID.ReadOnly = false;
                    //    btnAdd.Enabled = true;
                    //    btnProductType.Enabled = true;
                    //}
                    //else
                    //{
                        btnAdd.Enabled = false;
                        btnProductType.Enabled=false;
                    //}
                    
                    
                    this.Text = "Sales Credit Note Entry";

                }
                #endregion  rbtnCN
                #region  rbtnTrading

                else if (rbtnTrading.Checked)
                {
                    ChkExpLoc.Visible = true;
                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    label1.Visible = true;

                    this.Text = "Trading Sales Entry";
                    btnPrint.Text = "VAT 11 Ka";

                    //cmbVAT1Name.Items.Add("VAT 1 Kha (Trading)");

                    //cmbVAT1Name.Text = "VAT 1 Kha (Trading)";
                    label1.Visible = true;
                    btnFormKaT.Visible = true;

                    //btnProductType.Enabled = false;
                    //btnAdd.Enabled = false;
                    //rbtnProduct.Enabled = false;
                    //rbtnCode.Enabled = false;
                    //cmbProduct.Enabled = false;
                    //cmbProductCode.Enabled = false;
                    cmbVAT1Name.SelectedIndex = 2;


                }
                #endregion  rbtnTrading
                #region  rbtnTradingTender

                else if (rbtnTradingTender.Checked)
                {
                    ChkExpLoc.Visible = true;
                    cmbType.Text = "New";
                    cmbProductType.Text = "Trading";
                    cmbVATType.Text = "VAT";
                    chkTrading.Checked = true;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";


                    this.Text = "Tender Trading Sales Entry";
                    btnTenderNew.Visible = true;
                   
                    label18.Visible = true;
                    txtTenderStock.Visible = true;
                    btnSearchCustomer.Visible = false;
                    //btnSearchCustomer.Enabled = false;
                   

                    //cmbVAT1Name.Visible = true;
                    //cmbVAT1Name.Enabled = false;
                    //cmbVAT1Name.Items.Add("VAT 4.3 (Tender)");

                    cmbVAT1Name.Text = "VAT 4.3 (Tender)";
                    label1.Visible = true;
                    btnFormKaT.Visible = true;
                    #region Product
                   
                    #endregion Product

                    if (TenderInVAT1 == true)
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }
                    else if (TenderInVAT1Tender == true)
                    {
                        cmbVAT1Name.SelectedIndex = 8;

                    }
                    else
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }



                }

                #endregion  rbtnTender
                #region  rbtnExport 

                else if (rbtnExport.Checked )
                {
                    cmbType.Text = "New";
                    //ChkExpLoc.Visible = true;

                    

                    chkNonStock.Checked = true;
                    cmbVATType.Text = "Non VAT";
                    cmbVATType.Enabled = false;
                    cmbVATType.Items.Add("Export");
                    cmbVATType.Text = "Export";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";

                    this.Text = "Export Sales Entry";
                    btnExport.Visible = true;
                    btnTracking.Visible = false;
                    btn20.Visible = true;
                    txtLineNo.Visible = true;
                    labelLC.Visible = true;
                    txtLCNumber.Visible = true;

                   
                    //cmbVAT1Name.Items.Add("VAT 1 Ga (Export)");
                    //cmbVAT1Name.Text = "VAT 1 Ga (Export)";
                    label1.Visible = true;
                    txtNBRPrice.ReadOnly = false;
                    cmbVAT1Name.SelectedIndex = 3;
                    


                }
                #endregion  rbtnExport
                #region  rbtnInternalIssue

                else if (rbtnInternalIssue.Checked) // transfer
                {
                    this.Text = "Transfer Entry";

                    //btnPSale.Text = "Internal Issue";
                    btnVAT16.Visible = true;

                    cmbType.Text = "New";
                    Text = "Inner Issue(Transfer)";
                    rbtnInternalIssue.Checked = true;
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;

                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";

                    //cmbVAT1Name.Visible = true;
                    //cmbVAT1Name.Enabled = false;
                    //cmbVAT1Name.Items.Add("VAT 4.3 (Internal Issue)");
                    //cmbVAT1Name.Text = "VAT 4.3 (Internal Issue)";
                    label1.Visible = true;
                    cmbVAT1Name.SelectedIndex = 5;

                }
                #endregion  rbtnInternalIssue
                #region  rbtnService


                else if (rbtnService.Checked)
                {

                    ChkExpLoc.Visible = true;
                    cmbType.Text = "New";
                    cmbProductType.Text = "Service";
                    cmbVATType.Text = "VAT";
                    txtNBRPrice.ReadOnly = false;
                    chkNonStock.Checked = true;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    l5.Visible = false;
                    txtVehicleType.Visible = false;

                    cmbVehicleNo.Visible = false;
                    btnSearchVehicle.Visible = false;
                    chkSameVehicle.Visible = false;

                    this.Text = "Service Sales Entry";
                    btnPrint.Text = "VAT 11 Gha";
                    chkIs11.Visible = true;
                    chkIs11.Checked = true;
                    cmbVAT1Name.Text = "Service";
                    label1.Visible = true;

                    btnDL.Visible = false;
                    btnGPR.Visible = false;



                }
                #endregion  rbtnService
                #region  rbtnServiceNS


                else if (rbtnServiceNS.Checked)
                {

                    ChkExpLoc.Visible = true;
                    cmbType.Text = "New";
                    cmbProductType.Text = "Service";
                    cmbVATType.Text = "VAT";
                    txtNBRPrice.ReadOnly = false;
                    txtQuantityInHand.Visible = false; //start
                    LINhand.Visible = false;
                    chkNonStock.Checked = true;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    l5.Visible = false;
                    txtVehicleType.Visible = false;

                    cmbVehicleNo.Visible = false;
                    btnSearchVehicle.Visible = false;
                    chkSameVehicle.Visible = false;

                    this.Text = "Service Sales Entry(Non Stock)";
                    btnPrint.Text = "VAT 11 Gha";
                    chkIs11.Visible = true;
                    chkIs11.Checked = true;
                    cmbVAT1Name.Text = "Service";
                    label1.Visible = true;
                    txtQuantity.ReadOnly = true;
                    txtQuantity.Text = "1";
                    btnDL.Visible = false;
                    btnGPR.Visible = false;



                }
                #endregion  rbtnService
                #region  rbtnTender

                else if (rbtnTender.Checked)
                {
                    ChkExpLoc.Visible = true;
                    cmbType.Text = "New";
                    cmbProductType.Text = "Trading";
                    cmbVATType.Text = "VAT";
                    chkTrading.Checked = true;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    chkPreviousAddress.Visible = false;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 11";
                    btnAdd.Enabled = false;

                    this.Text = "Tender Sales Entry";
                    btnTenderNew.Visible = true;
                    btnProductType.Enabled = false;
                    label18.Visible = true;
                    txtTenderStock.Visible = true;
                    btnSearchCustomer.Visible = false;
                    btnProductType.Visible = false;
                    if (TenderInVAT1 == true)
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }
                    else if (TenderInVAT1Tender == true)
                    {
                        cmbVAT1Name.SelectedIndex = 8;
                    }
                    else
                    {
                        cmbVAT1Name.SelectedIndex = 0;
                    }
                    label1.Visible = true;


                }

                #endregion  rbtnTender
              
                #region  rbtnVAT11GaGa

                else if (rbtnVAT11GaGa.Checked)
                {
                    btnPrint.Visible = true;
                    btnPrint.Text = "11 GaGa";
                    chkIs11.Visible = false;

                    btnVAT16.Visible = false;
                    btnVAT17.Visible = false;
                    btnVAT18.Visible = false;
                    btn20.Visible = false;
                    btnExport.Visible = false;
                   
                    btnFormKaT.Visible = false;
                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    cmbVATType.Text = "VAT";
                    cmbVATType.Visible = false;

                    //txtNBRPrice.ReadOnly = true;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;
                    label22.Visible = false;
                    label1.Visible = false;
                    chkDiscount.Visible = false;
                    txtDiscountAmountInput.Visible = false;
                    chkPreviousAddress.Visible = false;

                    cmbVAT1Name.Visible = false;
                    //cmbVAT1Name.Items.Clear();
                    cmbVAT1Name.Items.Add("VAT 11 GaGa");
                    cmbVAT1Name.Text = "VAT 11 GaGa";

                    //txtNBRPrice.ReadOnly = false;

                    this.Text = "VAT 11 GaGa";




                }
                #endregion  rbtnVAT11GaGa


                #endregion Transaction Type


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
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

            #endregion


        }

        private void SetupVATStatus()
        {
            try
            {
                //string ReportData = dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd") + FieldDelimeter + LineDelimeter;

                varInvoiceDate =Convert.ToDateTime(dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd"));


                //this.Enabled = false;

                backgroundWorkerSetupVATStatus.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SetupVATStatus", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SetupVATStatus", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SetupVATStatus", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SetupVATStatus", exMessage);
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
                FileLogger.Log(this.Name, "SetupVATStatus", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SetupVATStatus", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SetupVATStatus", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SetupVATStatus", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerSetupVATStatus_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //start DoWork
                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //SetupSoapClient ShowStatus = new SetupSoapClient();
                //StatusResult = ShowStatus.VATStatus(encriptedData.ToString(), Program.DatabaseName);

                SetupDAL setupDal = new SetupDAL();
                StatusResult = setupDal.ResultVATStatus(varInvoiceDate, Program.DatabaseName);


                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerSetupVATStatus_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (StatusResult != null && StatusResult.Tables.Count > 0 && StatusResult.Tables[0].Rows.Count > 0)
                {


                    //start complete
                    txtTDBalance.Text = Convert.ToDecimal(StatusResult.Tables[0].Rows[0][0]).ToString();//"0.00");
                    Program.VATAmount = Convert.ToDecimal(StatusResult.Tables[0].Rows[0][0]);
                }
                //end complete
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupVATStatus_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.progressBar1.Visible = false;
            //this.Enabled = true;
        }


        private void ProductSearchDs()
        {
            //string ProductData = string.Empty;
            try
            {

                //ProductData = FieldDelimeter + // ItemNo,
                //    CategoryId + FieldDelimeter + // CategoryID,
                //    FieldDelimeter + // IsRaw,
                //    FieldDelimeter + // HSCodeNo,
                //    FieldDelimeter + // ActiveStatus,
                //    FieldDelimeter + // Trading, 
                //    FieldDelimeter + // NonStock,
                //    FieldDelimeter + // ProductCode
                //    LineDelimeter;

                ////start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);
                ////end DoWork

                ////start Complete
                //ProductsMini.Clear();
                //cmbProduct.Items.Clear();
                //cmbProductCode.Items.Clear();
                //foreach (DataRow item2 in ProductResultDs.Rows)
                //{
                //    var prod = new ProductMiniDTO();
                //    prod.ItemNo = item2["ItemNo"].ToString();
                //    prod.ProductName = item2["ProductName"].ToString();
                //    cmbProduct.Items.Add(item2["ProductName"]);
                //    prod.ProductCode = item2["ProductCode"].ToString();
                //    cmbProductCode.Items.Add(item2["ProductCode"]);

                //    prod.CategoryID = item2["CategoryID"].ToString();
                //    prod.CategoryName = item2["CategoryName"].ToString();
                //    prod.UOM = item2["UOM"].ToString();
                //    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                //    prod.IsRaw = item2["IsRaw"].ToString();

                //    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
                //    prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
                //    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                //    prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
                //    prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
                //    prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());

                //    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                //    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                //    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                //    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                //    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                //    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

                //    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                //    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                //    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                //    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                //    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                //    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                //    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                //    //bool TradingF = false;
                //    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                //    //prod.Trading = Trading;
                //    //bool NonStockF = false;
                //    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                //    //prod.NonStock = NonStock;

                //    ProductsMini.Add(prod);

                //}//end foreach
                ////end Complete
                //this.Enabled = false;

                //if (ChkExpLoc.Checked)
                //{
                //    //for export category product
                //    CategoryId = "10";
                //}
                //else
                //{
                //    //for finish product
                //    CategoryId = "3";
                //}

                backgroundWorkerProductSearchDs.RunWorkerAsync();

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
            }

            #endregion
        }

        private void ProductSearchDsFormLoad()
        {
            try
            {
                #region Transaction Select
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = string.Empty;
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = string.Empty;
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;

                string ProductData = string.Empty;
                if (CategoryId != null)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = CategoryId;
                    IsRawParam = string.Empty;
                    HSCodeNoParam = txtCategoryName.Text.Trim();
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    //txtCategoryName.Text = "Finish";

                }
                else if (rbtnOther.Checked || rbtnPackageSale.Checked || rbtnDN.Checked
                    || rbtnCN.Checked || rbtnTollFinishIssue.Checked)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Finish";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Finish";

                }
                else if (rbtnTrading.Checked)
                {


                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Trading";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Trading";
                }
                else if (rbtnExport.Checked )
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Finish";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    //txtCategoryName.Text = "Finish";
                    txtCategoryName.Text = "Export";
                }
                else if (rbtnInternalIssue.Checked) //transfer
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Raw";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Raw";


                }
                else if (rbtnService.Checked || rbtnServiceNS.Checked)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Service";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Service";


                }
                else if (rbtnTender.Checked)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Finish";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Finish";



                }
                else if (rbtnTollIssue.Checked)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Raw";
                    HSCodeNoParam = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Raw";



                }
                #endregion Transaction Select


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
            finally
            {
                //cmbProductCode.Enabled = false;
                //cmbProduct.Enabled = false;
                btnProductType.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                bgwProduct.RunWorkerAsync();


            }
        }
        private void bgwProduct_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement




                // Start DoWork
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                    ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwProduct_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwProduct_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwProduct_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwProduct_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwProduct_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                ProductsMini.Clear();
                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    cmbProduct.Items.Add(item2["ProductName"]);
                    prod.ProductCode = item2["ProductCode"].ToString();
                    cmbProductCode.Items.Add(item2["ProductCode"]);

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


                    //bool TradingF = false;
                    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                    //prod.Trading = Trading;

                    //bool NonStockF = false;
                    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                    //prod.NonStock = NonStock;


                    ProductsMini.Add(prod);
                }

                // End Complete

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                //cmbProductCode.Enabled = true;
                //cmbProduct.Enabled = true;
                btnProductType.Enabled = true;
                this.progressBar1.Visible = false;
                //this.Enabled = true;

            }
        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProductName))
            {
                if (ProductName == cmbProduct.Text)
                {
                    return;
                }
            }
            txtProductCode.Text = "";
            try
            {
                var searchText = cmbProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByName = from prd in ProductsMini.ToList()
                                     where prd.ProductName.ToLower() == searchText.ToLower()
                                     select prd;

                    if (prodByName != null && prodByName.Any())
                    {
                        var products = prodByName.First();
                        txtProductName.Text = products.ProductName;
                        txtProductCode.Text = products.ItemNo;
                        txtVATRate.Text = products.VATRate.ToString();//"0.00");
                        txtSD.Text = products.SD.ToString();//"0.00");

                        ProductDAL productDal = new ProductDAL();
                        //txtNBRPrice.Text = products.NBRPrice.ToString();

                        if (rbtnTender.Checked == false)
                        {
                            NBRPriceCall();
                        }
                        txtUOM.Text = products.UOM;
                        cmbUom.Text = products.UOM;
                        txtHSCode.Text = products.HSCodeNo;

                        if (cmbVAT1Name.Text.Trim() == "VAT 4.3 (Wastage)")
                        {
                            txtQuantityInHand.Text = "0";
                        }
                        else
                        {
                            txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                                             DateTime.Now.ToString(" HH:mm:00"), null, null, false).Rows[0]["Quantity"].ToString();

                        }

                        txtTenderStock.Text = products.TenderStock.ToString();//"0,0.0000");

                        txttradingMarkup.Text = products.TradingMarkUp.ToString();//"0.00");
                        txtPCode.Text = products.ProductCode;
                        cmbProductCode.Text = products.ProductCode;
                        chkNonStock.Checked = products.NonStock;
                        //txtQuantity.Text = "" + products.PurchaseQty;
                        Uoms();
                        //cmbUom.Focus();
                        if (rbtnServiceNS.Checked == true)
                        {
                            txtQuantity.Text = "1";

                        }
                        //UomsValue();
                    }
                    else
                    {
                        MessageBox.Show("Please select the right item", this.Text);
                        cmbProduct.Text = "Select";
                    }

                }
                txtQty1.Focus();

                //ProductDetailsInfo();

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void bgwCustomer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement




                // Start DoWork
                CustomerResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();

                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName, tinNo, customerVReg, activeStatusCustomer, CGType };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName like", "c.TINNo like", "c.VATRegistrationNo like", "c.ActiveStatus like", "cg.GroupType like" };
                CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false);


                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                // customerGName, tinNo, customerVReg, activeStatusCustomer, CGType); // Change 04


                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwCustomer_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwCustomer_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwCustomer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                customerMini.Clear();
                cmbCustomerName.Items.Clear();
                foreach (DataRow item2 in CustomerResult.Rows)
                {
                    var customer = new CustomerSinmgleDTO();
                    customer.CustomerID = item2["CustomerID"].ToString();
                    customer.CustomerCode = item2["CustomerCode"].ToString();
                    customer.CustomerName = item2["CustomerName"].ToString();
                    cmbCustomerName.Items.Add(item2["CustomerName"]);
                    customer.CustomerGroupID = item2["CustomerGroupID"].ToString();
                    customer.CustomerGroupName = item2["CustomerGroupName"].ToString();
                    customer.Address1 = item2["Address1"].ToString();
                    customer.Address2 = item2["Address2"].ToString();
                    customer.Address3 = item2["Address3"].ToString();
                    customer.City = item2["City"].ToString();
                    customer.TelephoneNo = item2["TelephoneNo"].ToString();
                    customer.FaxNo = item2["FaxNo"].ToString();
                    customer.Email = item2["Email"].ToString();
                    customer.TINNo = item2["TINNo"].ToString();
                    customer.VATRegistrationNo = item2["VATRegistrationNo"].ToString();
                    customer.ActiveStatus = item2["ActiveStatus"].ToString();

                    customer.Country = item2["Country"].ToString();

                    customerMini.Add(customer);

                }

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwCustomer_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                btnSearchCustomer.Enabled = true;
                //this.Enabled = true;

            }
        }

        private void bgwVehicle_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement




                // Start DoWork
                vehicleResult = new DataTable();
                VehicleDAL vehicleDal = new VehicleDAL();
                //vehicleResult = vehicleDal.SearchVehicleDataTable(vehicleId, vehicleType, vehicleNo, vehicleActiveStatus);
                string[] cValues = { vehicleId, vehicleType, vehicleNo, vehicleActiveStatus };
                string[] cFields = { "VehicleID like", "VehicleType like", "VehicleNo like", "ActiveStatus like" };
                vehicleResult = vehicleDal.SelectAll(0, cFields, cValues, null, null, true);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwVehicle_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwVehicle_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwVehicle_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwVehicle_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                vehicles.Clear();
                cmbVehicleNo.Items.Clear();

                foreach (DataRow item2 in vehicleResult.Rows)
                {
                    var vehicle = new VehicleDTO();
                    vehicle.VehicleID = item2["VehicleID"].ToString();
                    vehicle.VehicleType = item2["VehicleType"].ToString();
                    vehicle.VehicleNo = item2["VehicleNo"].ToString();
                    vehicle.Description = item2["Description"].ToString();
                    vehicle.Comments = item2["Comments"].ToString();
                    vehicle.ActiveStatus = item2["ActiveStatus"].ToString();
                    cmbVehicleNo.Items.Add(item2["VehicleNo"]);
                    vehicles.Add(vehicle);
                }
                cmbVehicleNo.Items.Insert(0, "Select");

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwVehicle_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                btnSearchVehicle.Enabled = true;
                //this.Enabled = true;
            }

        }

        private void cmbVehicleNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void picSale_Click(object sender, EventArgs e)
        {


        }

        private void picCustomerID_Click(object sender, EventArgs e)
        {

        }

        private void picVehicleID_Click(object sender, EventArgs e)
        {

        }

        private void picProductName_Click(object sender, EventArgs e)
        {

        }

        private void selectLastRow()
        {
            try
            {
                if (dgvSale.Rows.Count > 0)
                {

                    dgvSale.Rows[dgvSale.Rows.Count - 1].Selected = true;
                    dgvSale.CurrentCell = dgvSale.Rows[dgvSale.Rows.Count - 1].Cells[1];
                   
                }

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "selectLastRow", exMessage);
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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }

            #endregion
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //no serverside method
            try
            {
                if (dgvSale.RowCount > numberOfItems - 1)
                {
                    MessageBox.Show("You can't add more items ", this.Text);
                    return;
                }

                UomsValue();

                if (txtUomConv.Text == "0")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                #region Null Ckeck



                if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtNBRPrice.Focus();
                    return;
                }

                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item");
                    cmbProduct.Focus();
                    return;
                }
                else if (txtUomConv.Text == "0")
                {
                    MessageBox.Show("Please select Convert UOM");
                    cmbUom.Focus();
                    return;
                }

                else if (txtQuantity.Text == "")
                {
                    txtQuantity.Text = "0";
                }

                else if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                else if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }

                else if (txtUnitCost.Text == "")
                {
                    txtUnitCost.Text = "0.00";
                }

                else if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                else if (txtSD.Text == "")
                {
                    txtSD.Text = "0";
                }

                else if (txttradingMarkup.Text == "")
                {
                    txttradingMarkup.Text = "0.00";
                }
                else if (chkTrading.Checked == false)
                {
                    txttradingMarkup.Text = "0.00";
                }

                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity");
                    //txtQuantity.Focus();
                    txtQty1.Focus();
                    return;
                }
                else if (Convert.ToDecimal(txtNBRPrice.Text) <= 0)
                {
                    MessageBox.Show("Please declare the NBR Price");
                    //txtQuantity.Focus();
                    txtQty1.Focus();
                    return;
                }


                if (rbtnExport.Checked || rbtnExportPackage.Checked)
                {
                    if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" || string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                    {
                        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        return;
                    }
                }

                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                #endregion Null Ckeck


                
                #region Check Stock
                decimal minValue= 0;
                if (Convert.ToDecimal(txtQuantityInHand.Text) < Convert.ToDecimal(txtTenderStock.Text))
                {
                    minValue = Convert.ToDecimal(txtQuantityInHand.Text);
                }
                else
                {
                    minValue = Convert.ToDecimal(txtTenderStock.Text);

                }
                if (rbtnTender.Checked)
                {
                    
                    if (
                        Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text) > Convert.ToDecimal(minValue))
                    {
                        MessageBox.Show("Stock Not available");
                        //txtQuantity.Focus();
                        txtQty1.Focus();
                        return;
                    }
                }
                else if (rbtnTradingTender.Checked)
                {
                    if (Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text) > Convert.ToDecimal(minValue))
                    {
                        MessageBox.Show("Stock Not available");
                        //txtQuantity.Focus();
                        txtQty1.Focus();
                        return;
                    }
                }
                else if (rbtnCN.Checked == false
                    && rbtnVAT11GaGa.Checked == false
                    && rbtnServiceNS.Checked == false
                    && rbtnExportServiceNS.Checked == false
                    && cmbVAT1Name.Text.Trim()!="VAT 4.3 (Wastage)")
                {
                    if (NegativeStock == false)
                    {
                        if (
                            Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQty1.Focus();
                            return;
                        }
                    }
                }


                #endregion Check Stock

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    if (dgvSale.Rows[i].Cells["ItemNo"].Value.ToString().Trim() == txtProductCode.Text)
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        cmbProduct.Focus();
                        return;
                    }
                }
                if (cmbVATType.Text != "VAT")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }

                ChangeData = true;
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvSale.Rows.Add(NewRow);

                string strDiscount = txtDiscountAmount.Text.Trim();
                string strDiscountNBRPrice = txtDiscountedNBRPrice.Text.Trim();
                decimal vDiscount = 0;
                decimal vDiscountNBRPrice = 0;

                if (!string.IsNullOrEmpty(strDiscount))
                {
                    vDiscount = Convert.ToDecimal(strDiscount);
                }
                if (!string.IsNullOrEmpty(strDiscountNBRPrice))
                {
                    vDiscountNBRPrice = Convert.ToDecimal(strDiscountNBRPrice);
                }


                dgvSale["ItemNo", dgvSale.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvSale["ItemName", dgvSale.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvSale["PCode", dgvSale.RowCount - 1].Value = txtPCode.Text.Trim();
                dgvSale["UOM", dgvSale.RowCount - 1].Value = cmbUom.Text.Trim(); // txtUOM.Text.Trim();
                dgvSale["Comments", dgvSale.RowCount - 1].Value = "NA"; // txtCommentsDetail.Text.Trim();

                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), SalePlaceTaka).ToString();

                if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                    txtVATRate.Text = Program.FormatingNumeric(txtVATRate.Text.Trim(), SalePlaceRate).ToString();
                if (Program.CheckingNumericTextBox(txtSD, "txtSD") == true)
                    txtSD.Text = Program.FormatingNumeric(txtSD.Text.Trim(), SalePlaceRate).ToString();
                //if transaction type export or service then no need to read settings.
                if (txtCategoryName.Text != "Export")
                {
                    if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                        txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), SalePlaceTaka).ToString();
                }

                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), SalePlaceQty).ToString();

                if (Program.CheckingNumericTextBox(txtDiscountAmount, "txtDiscountAmount") == true)
                    txtDiscountAmount.Text = Program.FormatingNumeric(txtDiscountAmount.Text.Trim(), SalePlaceTaka).ToString();
                if (Program.CheckingNumericTextBox(txtDiscountedNBRPrice, "txtDiscountedNBRPrice") == true)
                    txtDiscountedNBRPrice.Text = Program.FormatingNumeric(txtDiscountedNBRPrice.Text.Trim(), SalePlaceTaka).ToString();

                //if (Program.CheckingNumericString(vDiscount.ToString(), "vDiscount") == true)
                //    vDiscount =(decimal)Program.FormatingNumeric(txtDiscountAmount.ToString(), SalePlaceTaka);

                //if (Program.CheckingNumericString(vDiscountNBRPrice.ToString(), "vDiscountNBRPrice") == true)
                //    vDiscountNBRPrice = (decimal)Program.FormatingNumeric(vDiscountNBRPrice.ToString(), SalePlaceTaka);



                dgvSale["UnitPrice", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString();
                dgvSale["VATRate", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtVATRate.Text.Trim()).ToString();
                dgvSale["SD", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtSD.Text.Trim())).ToString();

                dgvSale["NBRPrice", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtNBRPrice.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvSale["Quantity", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim())).ToString();
                dgvSale["SaleQuantity", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtQty1.Text.Trim())).ToString();
                dgvSale["PromotionalQuantity", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtQty2.Text.Trim())).ToString();
                dgvSale["UOMPrice", dgvSale.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtNBRPrice.Text.Trim())).ToString();
                dgvSale["UOMc", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtUomConv.Text.Trim()).ToString(); // txtUOM.Text.Trim();
                dgvSale["UOMn", dgvSale.RowCount - 1].Value = txtUOM.Text.Trim(); // txtUOM.Text.Trim();
                dgvSale["UOMQty", dgvSale.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();


                dgvSale["Status", dgvSale.RowCount - 1].Value = "New";
                dgvSale["Stock", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString();
                dgvSale["Previous", dgvSale.RowCount - 1].Value = 0; // txtQuantity.Text.Trim();
                dgvSale["Change", dgvSale.RowCount - 1].Value = 0;
                dgvSale["TradingMarkUp", dgvSale.RowCount - 1].Value =
                    Convert.ToDecimal(txttradingMarkup.Text.Trim()).ToString();
                dgvSale["Trading", dgvSale.RowCount - 1].Value = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                dgvSale["NonStock", dgvSale.RowCount - 1].Value = Convert.ToString(rbtnServiceNS.Checked ? "Y" : "N");
                dgvSale["Type", dgvSale.RowCount - 1].Value = cmbVATType.Text.Trim();

                //dgvSale["DiscountAmount", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtDiscountAmount.Text.Trim()).ToString();
                //dgvSale["DiscountedNBRPrice", dgvSale.RowCount - 1].Value = Convert.ToDecimal(txtDiscountedNBRPrice.Text.Trim()).ToString(); 

                dgvSale["DiscountAmount", dgvSale.RowCount - 1].Value = txtDiscountAmount.Text.Trim();
                dgvSale["DiscountedNBRPrice", dgvSale.RowCount - 1].Value = txtDiscountedNBRPrice.Text.Trim();
                dgvSale["VATName", dgvSale.RowCount - 1].Value = cmbVAT1Name.Text.Trim();
                dgvSale["Weight", dgvSale.RowCount - 1].Value = txtWeight.Text.Trim();

                if (!chkTrading.Checked)
                dgvSale["CConvDate", dgvSale.RowCount - 1].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");

                ////if transaction type export or service then no need to read settings.
                if (txtCategoryName.Text == "Export" )
                {
               
                    //CalculateSelectedExportRow();
                    //GTotal();
                    Rowcalculate();
                }
                else
                {
                    Rowcalculate();
                }
               
                selectLastRow();
               
                if (rbtnCode.Checked)
                {
                    cmbProductCode.Focus();
                }
                else
                {
                    cmbProduct.Focus();

                }


                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "0";
                txtQty1.Text = "0";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0.00";
                TransactionTypes();
                if (transactionType=="Trading")
                {
                    cmbVAT1Name.SelectedIndex = 2;
                }
                else if (transactionType=="Export")
                {
                    cmbVAT1Name.SelectedIndex = 3;
                }
                else if (transactionType == "InternalIssue")
                {
                    cmbVAT1Name.SelectedIndex = 5;
                }
                else if (cmbVAT1Name.Text.Trim() == "VAT 4.3 (Wastage)")
                {
                    cmbVAT1Name.SelectedIndex = 10;
                }
                else
                {
                    cmbVAT1Name.SelectedIndex = 0;   
                }
                cmbCurrency.Enabled = false;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void CalculateSelectedExportRow()
        {
            try
            {
                decimal NBR = 0;
                decimal Quantity = 0;
                decimal SumTotalS = 0;
                decimal Trading = 0;
                decimal TradingAmount = 0;
                decimal SD = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal bDTValue = 0;
                decimal dollerValue = 0;

                decimal CurrencyRateFromBDT = 0;
                decimal dollerRate = 0;
                decimal NBRPrice = 0;

                decimal Total = 0;


                if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" ||
                    string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }

                else
                {
                    CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                    dollerRate = Convert.ToDecimal(txtDollerRate.Text.Trim());

                }



                if (Program.CheckingNumericString(dollerRate.ToString(), "dollerRate") == true)
                    //dollerRate = Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));
                dollerRate = Convert.ToDecimal(dollerRate.ToString());

                if (Program.CheckingNumericString(CurrencyRateFromBDT.ToString(), "CurrencyRateFromBDT") == true)
                    CurrencyRateFromBDT =
                        //Convert.ToDecimal(Program.FormatingNumeric(CurrencyRateFromBDT.ToString(), SalePlaceDollar));
                Convert.ToDecimal(CurrencyRateFromBDT.ToString());
                for (int j = 0; j < dgvSale.RowCount; j++)
                {
                    SD = Convert.ToDecimal(dgvSale["SD", j].Value);
                    VATRate = Convert.ToDecimal(dgvSale["VATRate", j].Value);
                    Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", j].Value);
                }
                    int i= dgvSale.CurrentRow.Index;
                    SD = Convert.ToDecimal(dgvSale["SD", i].Value);
                    VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);
                    Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);

                    #region Tender Price with VAT

                    if (rbtnTender.Checked)
                    {
                        if (TenderPriceWithVAT == true)
                        {
                            NBRPriceWithVAT = Convert.ToDecimal(dgvSale["NBRPWithVAT", i].Value);
                            decimal NBRpricewithoutVAT, NBRpricewithoutSD, NBRpricewithoutTM = 0;

                            NBRpricewithoutVAT = (NBRPriceWithVAT * 100) / (100 + VATRate);
                            VATAmount = (NBRpricewithoutVAT * VATRate) / 100;


                            NBRpricewithoutSD = (NBRpricewithoutVAT * 100) / (100 + SD);
                            SDAmount = (NBRpricewithoutSD * SD) / 100;

                            NBRpricewithoutTM = (NBRpricewithoutSD * 100) / (100 + Trading);
                            TradingAmount = (NBRpricewithoutTM * Trading) / 100;

                            NBR = NBRpricewithoutTM;

                            dgvSale["NBRPrice", i].Value = NBR;

                        }
                    }

                        #endregion Tender Price with VAT

                        Quantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                        NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);

                        if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                            Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
                        //if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
                        //    NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

                        SumTotalS = Quantity*NBR;

                        if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
                            SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));


                        bDTValue = CurrencyRateFromBDT*SumTotalS;
                        if (Program.CheckingNumericString(bDTValue.ToString(), "bDTValue") == true)
                            bDTValue = Convert.ToDecimal(Program.FormatingNumeric(bDTValue.ToString(), SalePlaceDollar));

                        dgvSale["BDTValue", i].Value = Convert.ToDecimal(bDTValue);
                        dgvSale["DollerValue", i].Value = 0;
                        if (rbtnExport.Checked)
                        {
                            if (dollerRate == 0)
                            {
                                MessageBox.Show(
                                    "Please select the Currency or check the currency rate from BDT and USD", this.Text);
                                return;
                            }
                            dollerValue = bDTValue/dollerRate;
                            if (Program.CheckingNumericString(dollerValue.ToString(), "dollerValue") == true)
                                dollerValue =
                                    Convert.ToDecimal(Program.FormatingNumeric(dollerValue.ToString(), SalePlaceDollar));

                            dgvSale["DollerValue", i].Value = Convert.ToDecimal(dollerValue);
                        }
                       
                        if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                            SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
                        if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
                            VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));
                        if (Program.CheckingNumericString(Trading.ToString(), "Trading") == true)
                            Trading = Convert.ToDecimal(Program.FormatingNumeric(Trading.ToString(), SalePlaceRate));

                        TradingAmount = SumTotalS*Trading/100;
                        SDAmount = (SumTotalS + TradingAmount)*SD/100;
                        VATAmount = (SumTotalS + TradingAmount + SDAmount)*VATRate/100;
                        if (Program.CheckingNumericString(TradingAmount.ToString(), "TradingAmount") == true)
                            TradingAmount =
                                Convert.ToDecimal(Program.FormatingNumeric(TradingAmount.ToString(), SalePlaceTaka));
                        if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                            SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), SalePlaceTaka));
                        if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                            VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), SalePlaceTaka));


                        Total = SumTotalS + TradingAmount + SDAmount + VATAmount;



                        if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                            Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), SalePlaceTaka));

                        dgvSale[0, i].Value = i + 1;

                        dgvSale["VATAmount", i].Value = Convert.ToDecimal(VATAmount).ToString(); //"0,0.00");
                        dgvSale["SDAmount", i].Value = Convert.ToDecimal(SDAmount).ToString(); //"0,0.00");
                        dgvSale["SubTotal", i].Value = Convert.ToDecimal(SumTotalS).ToString();
                        dgvSale["Total", i].Value = Convert.ToDecimal(Total).ToString(); //"0,0.00");

                        dgvSale["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvSale["Quantity", i].Value)
                                                                       - Convert.ToDecimal(dgvSale["Previous", i].Value)).ToString();

                   
            }
            #region catch
            catch
                (ArgumentNullException
                ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (IndexOutOfRangeException
                ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (NullReferenceException
                ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (FormatException
                ex)
            {

                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch
                (SoapHeaderException
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (SoapException
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            catch
                (EndpointNotFoundException
                ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch
                (WebException
                ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch
                (Exception
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion
        }
        private void GTotal()
        {
            decimal SumSubTotal = 0;
            decimal SumSDTotal = 0;
            decimal SumVATAmount = 0;
            decimal SumGTotal = 0;
            decimal SumTrading = 0;
            decimal GrandTotal = 0;
            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {

                SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                SumSDTotal = SumSDTotal + Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                SumTrading = SumTrading+Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);
                SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);

               
            }

            //GrandTotal = SumSubTotal + SumTrading + SumSDTotal + SumVATAmount;
            //txtTotalAmount.Text = Convert.ToDecimal(GrandTotal).ToString(); //"0,0.00");
            txtTotalSubTotal.Text = Convert.ToDecimal(SumSubTotal).ToString(); //"0,0.00");
            txtTotalSDAmount.Text = Convert.ToDecimal(SumSDTotal).ToString(); //"0,0.00");

            txtTotalVATAmount.Text = Convert.ToDecimal(SumVATAmount).ToString(); //"0,0.00");
            txtTotalAmount.Text = Convert.ToDecimal(SumGTotal).ToString(); //"0,0.00");

        }
        

        private void btnChange_Click(object sender, EventArgs e)
        {
            //decimal a = 2000;
            //decimal b =60;
            //decimal c = a/b*b;

            //MessageBox.Show(c.ToString());
            //no serverside method
            try
            {
                if (dgvSale.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
               
                if (txtUomConv.Text == "0"
                    || string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtUomConv.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                } UomsValue();
                if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtNBRPrice.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (rbtnExport.Checked || rbtnExportPackage.Checked)
                {
                    if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" || string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                    {
                        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        return;
                    }
                }

                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                else if (txttradingMarkup.Text == "")
                {
                    txttradingMarkup.Text = "0.00";
                }
                else if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                else if (chkTrading.Checked == false)
                {
                    txttradingMarkup.Text = "0.00";
                }

                ChangeData = true;


                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue,tenderQty, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text);

                decimal minValue = 0;
                if (Convert.ToDecimal(txtQuantityInHand.Text) < Convert.ToDecimal(txtTenderStock.Text))
                {
                    minValue = Convert.ToDecimal(txtQuantityInHand.Text);
                }
                else
                {
                    minValue = Convert.ToDecimal(txtTenderStock.Text);

                }

                if (rbtnTender.Checked)
                {
                    if (
                       Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                    {
                        MessageBox.Show("Stock Not available");
                        txtQuantity.Focus();
                        return;
                    }
                }
                else if (rbtnTradingTender.Checked)
                {
                    if (
                        Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                    {
                        MessageBox.Show("Stock Not available");
                        txtQuantity.Focus();
                        return;
                    }
                }
                else if (rbtnCN.Checked == false
                    && rbtnVAT11GaGa.Checked == false
                    && rbtnServiceNS.Checked == false
                    && rbtnExportServiceNS.Checked == false
                    && cmbVAT1Name.Text.Trim()!="VAT 4.3 (Wastage)")
                {
                    if (CurrentValue > PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }
                    }
                }
                else if (cmbVAT1Name.Text.Trim()!="VAT 4.3 (Wastage)")
                {
                    
                   
                    if (CurrentValue <= PreviousValue)
                    {
                        if (rbtnCN.Checked == false)
                        {
                            if (
                                Convert.ToDecimal(PreviousValue - CurrentValue) >
                                Convert.ToDecimal(StockValue))
                            {
                                MessageBox.Show("Stock Not available");
                                txtQuantity.Focus();
                                return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Credit Note can not be greater than actual quantity.");
                        txtQty1.Text = PreviousValue.ToString();
                        txtQty1.Focus();
                        return;
                    }

                    
                }


                #endregion Stock Chekc
                if (cmbVATType.Text != "VAT")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }

                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), SalePlaceTaka).ToString();
                if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                    txtVATRate.Text = Program.FormatingNumeric(txtVATRate.Text.Trim(), SalePlaceRate).ToString();
                if (Program.CheckingNumericTextBox(txtSD, "txtSD") == true)
                    txtSD.Text = Program.FormatingNumeric(txtSD.Text.Trim(), SalePlaceRate).ToString();

                //if transaction type export no need to read settings.
                if (txtCategoryName.Text != "Export")
                {
                   if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), SalePlaceTaka).ToString();
                }

                
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), SalePlaceQty).ToString();

                if (Program.CheckingNumericTextBox(txtDiscountAmount, "txtDiscountAmount") == true)
                    txtDiscountAmount.Text = Program.FormatingNumeric(txtDiscountAmount.Text.Trim(), SalePlaceTaka).ToString();
                if (Program.CheckingNumericTextBox(txtDiscountedNBRPrice, "txtDiscountedNBRPrice") == true)
                    txtDiscountedNBRPrice.Text = Program.FormatingNumeric(txtDiscountedNBRPrice.Text.Trim(), SalePlaceTaka).ToString();



                dgvSale["UnitPrice", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString();//"0,0.00");


                dgvSale["SD", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtSD.Text.Trim())).ToString();//"0.00");
                dgvSale["VATRate", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(txtVATRate.Text.Trim()).ToString();//"0.00");
                dgvSale["Comments", dgvSale.CurrentRow.Index].Value = "NA"; // txtCommentsDetail.Text.Trim();


                dgvSale["UOM", dgvSale.CurrentRow.Index].Value = cmbUom.Text.Trim(); // txtUOM.Text.Trim();

                dgvSale["Quantity", dgvSale.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();//"0,0.0000");
                dgvSale["SaleQuantity", dgvSale.CurrentRow.Index].Value = Convert.ToDecimal(txtQty1.Text.Trim()).ToString();//"0,0.0000");
                dgvSale["PromotionalQuantity", dgvSale.CurrentRow.Index].Value = Convert.ToDecimal(txtQty2.Text.Trim()).ToString();//"0,0.0000");

                dgvSale["NBRPrice", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtNBRPrice.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();

                dgvSale["UOMPrice", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtNBRPrice.Text.Trim())).ToString();
                dgvSale["UOMc", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(txtUomConv.Text.Trim()).ToString(); // txtUOM.Text.Trim();
                dgvSale["UOMn", dgvSale.CurrentRow.Index].Value = txtUOM.Text.Trim(); // txtUOM.Text.Trim();
                dgvSale["UOMQty", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();


                if (dgvSale.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvSale["Status", dgvSale.CurrentRow.Index].Value = "Change";
                }
                dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvSale["TradingMarkUp", dgvSale.CurrentRow.Index].Value =
                    Convert.ToDecimal(txttradingMarkup.Text.Trim()).ToString();//"0.00");
                dgvSale["Type", dgvSale.CurrentRow.Index].Value = cmbVATType.Text.Trim();

                dgvSale["DiscountAmount", dgvSale.CurrentRow.Index].Value = Convert.ToDecimal(txtDiscountAmount.Text.Trim()).ToString();
                dgvSale["DiscountedNBRPrice", dgvSale.CurrentRow.Index].Value = Convert.ToDecimal(txtDiscountedNBRPrice.Text.Trim()).ToString();

                dgvSale["Trading", dgvSale.CurrentRow.Index].Value = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                dgvSale["NonStock", dgvSale.CurrentRow.Index].Value = Convert.ToString(rbtnServiceNS.Checked ? "Y" : "N");
                dgvSale["VATName", dgvSale.CurrentRow.Index].Value = cmbVAT1Name.Text.Trim();
                dgvSale["Weight", dgvSale.CurrentRow.Index].Value = txtWeight.Text.Trim();
                if (!chkTrading.Checked)
                    dgvSale["CConvDate", dgvSale.CurrentRow.Index].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                
                //dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                if (txtCategoryName.Text == "Export")
                {
                //if (rbtnExport.Checked || ChkExpLoc.Checked)
                //{
                    CalculateSelectedExportRow();
                    GTotal();
                }
                else
                {
                    Rowcalculate();
                }

                txtProductCode.Text = "";
                txtProductName.Text = "";
                cmbProductCode.Text = "Select";
                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "0";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0.00";
                //cmbVAT1Name.SelectedIndex = 0;
                if (rbtnCN.Checked == false && rbtnDN.Checked == false)
                {
                    if (rbtnCode.Checked)
                    {
                        cmbProductCode.Focus();
                    }
                    else
                    {
                        cmbProduct.Focus();

                    }
                }
                
               
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
                if (dgvSale.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvSale.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                        //Rowcalculate();
                        GTotal();
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

                if (dgvSale.RowCount > 0)
                {
                    if (txtUomConv.Text == "0"
                    || string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtUomConv.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                    {
                        MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                   
                }
                else
                {
                    MessageBox.Show("No Items Found in Details Section.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (txttradingMarkup.Text == "")
                {
                    txttradingMarkup.Text = "0.00";
                }

                ChangeData = true;

                #region Stock Chekc

                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                CurrentValue = 0;


                if (rbtnCN.Checked == false
                    && rbtnVAT11GaGa.Checked == false
                    && rbtnServiceNS.Checked == false
                    && rbtnExportServiceNS.Checked == false)

                {
                    if (CurrentValue > PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }

                    }
                }
                else
                {
                    if (CurrentValue < PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(PreviousValue - CurrentValue) >
                            Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }

                    }

                }

                #endregion Stock Chekc



                dgvSale.CurrentRow.Cells["Status"].Value = "Delete";


                dgvSale.CurrentRow.Cells["Quantity"].Value = 0.00;
                dgvSale.CurrentRow.Cells["SaleQuantity"].Value = 0.00;
                dgvSale.CurrentRow.Cells["PromotionalQuantity"].Value = 0.00;
                dgvSale.CurrentRow.Cells["NBRPrice"].Value = 0.00;
                dgvSale.CurrentRow.Cells["TradingMarkUp"].Value = 0.00;
                dgvSale.CurrentRow.Cells["SubTotal"].Value = 0.00;
                dgvSale.CurrentRow.Cells["SD"].Value = 0.00;
                dgvSale.CurrentRow.Cells["VATRate"].Value = 0.00;
                dgvSale.CurrentRow.Cells["VATAmount"].Value = 0.00;
                dgvSale.CurrentRow.Cells["Total"].Value = 0.00;
                dgvSale.CurrentRow.Cells["UnitPrice"].Value = 0.00;
                dgvSale.CurrentRow.Cells["SDAmount"].Value = 0.00;
                dgvSale.CurrentRow.Cells["UOMPrice"].Value = 0.00;
                dgvSale.CurrentRow.Cells["UOMc"].Value = 0.00;
                dgvSale.CurrentRow.Cells["UOMQty"].Value = 0.00;
                dgvSale.CurrentRow.Cells["DiscountAmount"].Value = 0.00;
                dgvSale.CurrentRow.Cells["DiscountedNBRPrice"].Value = 0.00;
                dgvSale.CurrentRow.Cells["BDTValue"].Value = 0.00;
                dgvSale.CurrentRow.Cells["DollerValue"].Value = 0.00;


                dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                //Rowcalculate();
                GTotal();
                txtProductCode.Text = "";
                txtProductName.Text = "";

                if (rbtnCode.Checked)
                {
                    cmbProductCode.Focus();
                }
                else
                {
                    cmbProduct.Focus();

                }
                //return false;

            }


        private void Rowcalculate()
        {
            try
            {
                decimal NBR = 0;
                decimal Quantity = 0;
                //decimal UomC = 0;
                decimal SumTotalS = 0;
                decimal Trading = 0;
                decimal TradingAmount = 0;
                decimal SD = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal SumSubTotal = 0;
                decimal SumSDTotal = 0;
                decimal SumVATAmount = 0;
                decimal SumGTotal = 0;
                decimal bDTValue = 0;
                decimal dollerValue = 0;

                decimal CurrencyRateFromBDT = 0;
                decimal dollerRate = 0;
                decimal NBRPrice = 0;


                //decimal Cost = 0;
                decimal Total = 0;

                if (rbtnExport.Checked || rbtnExportPackage.Checked)
                {
                    if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" ||
                        string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                    {
                        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        return;
                    }
                    else
                    {
                        dollerRate = Convert.ToDecimal(txtDollerRate.Text);
                        CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text);
                    }
                }

                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                else
                {
                    CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                    dollerRate = Convert.ToDecimal(txtDollerRate.Text.Trim());

                }



                if (Program.CheckingNumericString(dollerRate.ToString(), "dollerRate") == true)
                dollerRate = Convert.ToDecimal(dollerRate.ToString());

                if (Program.CheckingNumericString(CurrencyRateFromBDT.ToString(), "CurrencyRateFromBDT") == true)
                    CurrencyRateFromBDT = Convert.ToDecimal(CurrencyRateFromBDT.ToString());

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SD = Convert.ToDecimal(dgvSale["SD", i].Value);
                    VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);
                    Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);

                    #region Tender Price with VAT


                    if (rbtnTender.Checked)
                    {
                        if (TenderPriceWithVAT == true)
                        {
                            NBRPriceWithVAT = Convert.ToDecimal(dgvSale["NBRPWithVAT", i].Value);
                            decimal NBRpricewithoutVAT, NBRpricewithoutSD, NBRpricewithoutTM = 0;

                            NBRpricewithoutVAT = (NBRPriceWithVAT * 100) / (100 + VATRate);
                            VATAmount = (NBRpricewithoutVAT * VATRate) / 100;


                            NBRpricewithoutSD = (NBRpricewithoutVAT * 100) / (100 + SD);
                            SDAmount = (NBRpricewithoutSD * SD) / 100;

                            NBRpricewithoutTM = (NBRpricewithoutSD * 100) / (100 + Trading);
                            TradingAmount = (NBRpricewithoutTM * Trading) / 100;

                            NBR = NBRpricewithoutTM;

                            dgvSale["NBRPrice", i].Value = NBR;

                        }
                    }

                        #endregion Tender Price with VAT

                        Quantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                        NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);

                        if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                            Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
                        if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
                            NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

                        SumTotalS = Quantity*NBR;

                        if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
                            SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));


                        bDTValue = CurrencyRateFromBDT*SumTotalS;
                        if (Program.CheckingNumericString(bDTValue.ToString(), "bDTValue") == true)
                            bDTValue = Convert.ToDecimal(Program.FormatingNumeric(bDTValue.ToString(), SalePlaceDollar));

                        dgvSale["BDTValue", i].Value = Convert.ToDecimal(bDTValue);
                        dgvSale["DollerValue", i].Value = 0;
                        if (rbtnExport.Checked)
                        {
                            if (dollerRate == 0)
                            {
                                MessageBox.Show(
                                    "Please select the Currency or check the currency rate from BDT and USD", this.Text);
                                return;
                            }
                            dollerValue = bDTValue/dollerRate;
                            if (Program.CheckingNumericString(dollerValue.ToString(), "dollerValue") == true)
                                dollerValue =
                                    Convert.ToDecimal(Program.FormatingNumeric(dollerValue.ToString(), SalePlaceDollar));

                            dgvSale["DollerValue", i].Value = Convert.ToDecimal(dollerValue);
                        }
                       
                        if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                            SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
                        if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
                            VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));
                        if (Program.CheckingNumericString(Trading.ToString(), "Trading") == true)
                            Trading = Convert.ToDecimal(Program.FormatingNumeric(Trading.ToString(), SalePlaceRate));

                        TradingAmount = SumTotalS*Trading/100;
                        SDAmount = (SumTotalS + TradingAmount)*SD/100;
                        VATAmount = (SumTotalS + TradingAmount + SDAmount)*VATRate/100;
                        if (Program.CheckingNumericString(TradingAmount.ToString(), "TradingAmount") == true)
                            TradingAmount =
                                Convert.ToDecimal(Program.FormatingNumeric(TradingAmount.ToString(), SalePlaceTaka));
                        if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                            SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), SalePlaceTaka));
                        if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                            VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), SalePlaceTaka));


                        Total = SumTotalS + TradingAmount + SDAmount + VATAmount;



                        if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                            Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), SalePlaceTaka));

                        dgvSale[0, i].Value = i + 1;

                        dgvSale["VATAmount", i].Value = Convert.ToDecimal(VATAmount).ToString(); //"0,0.00");
                        dgvSale["SDAmount", i].Value = Convert.ToDecimal(SDAmount).ToString(); //"0,0.00");
                        dgvSale["SubTotal", i].Value = Convert.ToDecimal(SumTotalS).ToString();
                        dgvSale["Total", i].Value = Convert.ToDecimal(Total).ToString(); //"0,0.00");



                        SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                        SumSDTotal = SumSDTotal + Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                        SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                        SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);

                        dgvSale["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvSale["Quantity", i].Value)
                                                                       - Convert.ToDecimal(dgvSale["Previous", i].Value))
                            .
                            ToString(); //"0,0.0000");

                    

                    txtTotalSubTotal.Text = Convert.ToDecimal(SumSubTotal).ToString(); //"0,0.00");
                    txtTotalSDAmount.Text = Convert.ToDecimal(SumSDTotal).ToString(); //"0,0.00");

                    txtTotalVATAmount.Text = Convert.ToDecimal(SumVATAmount).ToString(); //"0,0.00");
                    txtTotalAmount.Text = Convert.ToDecimal(SumGTotal).ToString(); //"0,0.00");
                }
                   
            }
            #region catch
            catch
                (ArgumentNullException
                ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (IndexOutOfRangeException
                ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (NullReferenceException
                ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (FormatException
                ex)
            {

                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch
                (SoapHeaderException
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
                (SoapException
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            catch
                (EndpointNotFoundException
                ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch
                (WebException
                ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch
                (Exception
                ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion
        }

        private void TransactionTypes()
        {
            #region Transaction Type
            string a = transactionType;
            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }
            else if (rbtnPackageSale.Checked)
            {
                transactionType = "PackageSale";
            }
            else if (rbtnExportPackage.Checked)
            {
                transactionType = "ExportPackage";
            }
            else if (rbtnDN.Checked)
            {
                transactionType = "Debit";
            }
            else if (rbtnVAT11GaGa.Checked)
            {
                transactionType = "VAT11GaGa";
            }
            else if (rbtnCN.Checked)
            {
                transactionType = "Credit";
            }
            else if (rbtnTrading.Checked)
            {
                transactionType = "Trading";
            }
            else if (rbtnTradingTender.Checked)
            {
                transactionType = "TradingTender";
            }
            else if (rbtnExport.Checked)
            {
                transactionType = "Export";
                //txtCategoryName.Text = "Export";
                ChkExpLoc.Checked = true;
            }
            else if (rbtnExportTrading.Checked)
            {
                transactionType = "ExportTrading";
                //txtCategoryName.Text = "Export";
            }
            else if (rbtnExportTradingTender.Checked)
            {
                transactionType = "ExportTradingTender";
                //txtCategoryName.Text = "Export";
            }
            else if (rbtnExportService.Checked)
            {
                transactionType = "ExportService";
                //txtCategoryName.Text = "Export";
            }
            else if (rbtnExportTender.Checked)
            {
                transactionType = "ExportTender";
                ////txtCategoryName.Text = "Export";
            }

            else if (rbtnExportServiceNS.Checked)
            {
                transactionType = "ExportServiceNS";
                //txtCategoryName.Text = "Export";

            }
            else if (rbtnInternalIssue.Checked)
            {
                transactionType = "InternalIssue";
            }
            else if (rbtnService.Checked)
            {
                transactionType = "Service";
            }
            else if (rbtnServiceNS.Checked)
            {
                transactionType = "ServiceNS";
            }
            else if (rbtnTender.Checked)
            {
                transactionType = "Tender";
            }

            else if (rbtnTollIssue.Checked)
            {
                transactionType = "TollIssue";
            }
            else if (rbtnTollFinishIssue.Checked)
            {
                transactionType = "TollFinishIssue";
            }
            if (cmbVAT1Name.Text.Trim() == "VAT 4.3 (Wastage)")
            {
                transactionType = "Wastage";
            }



            #endregion Transaction Type
        }

        private void SelectVATName(string vatNameInCD)
        {
            try
            {
                #region Transaction Type

                if (!string.IsNullOrEmpty(vatNameInCD))
                {
                    VATName vname = new VATName();

                    IList<string> vatName = vname.VATNameList;
                    for (int i = 0; i < vatName.Count; i++)
                    {
                       if (vatNameInCD.Trim() == vatName[i].ToString().Trim())
                        {
                            cmbVAT1Name.SelectedIndex = i;
                            break;
                        }
                    }
                }

                SaleDAL saleDal = new SaleDAL();
                string categoryName = saleDal.GetCategoryName(txtProductCode.Text);
                if (!string.IsNullOrEmpty(categoryName))
                {
                    txtCategoryName.Text = categoryName;
                }


                #endregion Transaction Type
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
                FileLogger.Log(this.Name, "SelectVATName", exMessage);
            }
            
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                #region save

                TransactionTypes();
                #region start

                //if (Add == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                //    return;
                //}
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (dgvSale.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }
                if (Convert.ToDateTime(dtpDeliveryDate.Value.ToString("yyyy-MMM-dd")) <
                    Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Delivery date not before invoice date", this.Text);
                    return;
                }

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                SetupVATStatus();
                if ((PrepaidVAT == true && rbtnService.Checked == false) || (PrepaidVAT == true && rbtnServiceNS.Checked == false))
                {
                    if (Convert.ToDecimal(Program.VATAmount + PreVATAmount) <
                        Convert.ToDecimal(txtTotalVATAmount.Text.Trim()))
                    {
                        MessageBox.Show("You have no balance of VAT for sales", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                }

                NextID = IsUpdate == false ? string.Empty : txtSalesInvoiceNo.Text.Trim();

                #endregion start

                #region Null

                if (txtCustomerID.Text == "")
                {
                    MessageBox.Show("Please Enter Customer Information");
                    cmbCustomerName.Focus();
                    return;
                }
                if (rbtnService.Checked == false && rbtnServiceNS.Checked == false)
                {
                    if (cmbVehicleNo.Text == "")
                    {
                        MessageBox.Show("Please Enter Vehicle Number");
                        cmbVehicleNo.Focus();
                        return;
                    }
                    if (txtVehicleType.Text == "")
                    {

                        MessageBox.Show("Please Enter Vehicle Type");
                        txtVehicleType.Focus();
                        return;
                    }

                }

                if (cmbType.Text == "New")
                {
                    txtOldID.Text = "0.00";
                }
                else
                {
                    if (txtOldID.Text == "")
                    {
                        MessageBox.Show("Please Enter previous Invoice No");

                        return;
                    }
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtDeliveryAddress1.Text == "")
                {
                    txtDeliveryAddress1.Text = "-";
                }
                if (txtDeliveryAddress2.Text == "")
                {
                    txtDeliveryAddress2.Text = "-";
                }
                if (txtDeliveryAddress3.Text == "")
                {
                    txtDeliveryAddress3.Text = "-";
                }
                if (ChkExpLoc.Checked)
                {
                    if (txtLCNumber.Text == "")
                    {
                        MessageBox.Show("Please Enter LC Number");
                        txtLCNumber.Focus();
                        return;
                    }
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }


                #endregion Null
                #region Tracking Check

                if (TrackingTrace == true)
                {
                    if (trackingsVm.Count <= 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information have not been added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                        {
                            btnTracking_Click(sender, e);
                            return;
                        }

                    }

                }

                #endregion

                #region Master
                saleMaster = new SaleMasterVM();
                saleMaster.SalesInvoiceNo = NextID.ToString();
                saleMaster.CustomerID = txtCustomerID.Text.Trim();
                saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                saleMaster.VehicleType = txtVehicleType.Text.Trim();
                saleMaster.VehicleNo = cmbVehicleNo.Text.Trim();
                saleMaster.vehicleSaveInDB = false;
                if (chkVehicleSaveInDatabase.Checked == true)
                {
                    saleMaster.vehicleSaveInDB = true;
                }
                //saleMaster.VehicleType = txtVehicleType.Text.Trim();
                //saleMaster.VehicleNo = cmbVehicleNo.Text.Trim();

                saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                saleMaster.SerialNo = txtSerialNo.Text.Trim();
                saleMaster.Comments = txtComments.Text.Trim();
                saleMaster.CreatedBy = Program.CurrentUser;
                saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.LastModifiedBy = Program.CurrentUser;
                saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.SaleType = cmbType.Text.Trim();
                saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                saleMaster.IsPrint = "N";
                saleMaster.TenderId = TenderId;
                saleMaster.TransactionType = transactionType;
                saleMaster.DeliveryDate = dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.Post = "N"; //Post
                saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate
                saleMaster.LCNumber = txtLCNumber.Text.Trim(); //Post
                //saleMaster.ImportID = txtImportID.Text.Trim(); //ImportID
                               
                #endregion Master
                #region detail

                saleDetails = new List<SaleDetailVm>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleDetailVm detail = new SaleDetailVm();

                    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());

                    detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());


                    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();

                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.SaleTypeD = cmbType.Text.Trim();
                    detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();
                    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                    detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                    detail.TradingMarkUp = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                    detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                    detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                    detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                    detail.VatName = dgvSale.Rows[i].Cells["VATName"].Value.ToString();
                    detail.Weight = dgvSale.Rows[i].Cells["Weight"].Value.ToString();
                    if (!chkTrading.Checked)
                    {
                        detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                    }

                    if (rbtnDN.Checked || rbtnCN.Checked)
                    {
                        detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                    }

                    saleDetails.Add(detail);
                }
                #endregion detail
                #region export

                if (ChkExpLoc.Checked)
                {
                    saleExport = new List<SaleExportVM>();

                    for (int i = 0; i < dgvExport.RowCount; i++)
                    {

                        if (string.IsNullOrEmpty(dgvExport.Rows[i].Cells["Description"].Value.ToString()) ||
                             dgvExport.Rows[i].Cells["Description"].Value == null ||
                             dgvExport.Rows[i].Cells["QuantityE"].Value == null ||
                             dgvExport.Rows[i].Cells["GrossWeight"].Value == null ||
                             dgvExport.Rows[i].Cells["NetWeight"].Value == null ||
                             dgvExport.Rows[i].Cells["NumberFrom"].Value == null ||
                             dgvExport.Rows[i].Cells["NumberTo"].Value == null ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["QuantityE"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NetWeight"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberTo"].Value.ToString()))
                        {
                            MessageBox.Show("Please check the export Details, All the data must fill", this.Text);
                            ExportPackagingInfoAdd();
                            return;
                        }
                        SaleExportVM expDetail = new SaleExportVM();
                        expDetail.SaleLineNo = dgvExport.Rows[i].Cells["LineNoE"].Value.ToString();
                        expDetail.Description = dgvExport.Rows[i].Cells["Description"].Value.ToString();
                        expDetail.QuantityE = dgvExport.Rows[i].Cells["QuantityE"].Value.ToString();
                        expDetail.GrossWeight = dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString();
                        expDetail.NetWeight = dgvExport.Rows[i].Cells["NetWeight"].Value.ToString();
                        expDetail.NumberFrom = dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString();
                        expDetail.NumberTo = dgvExport.Rows[i].Cells["NumberTo"].Value.ToString();
                        expDetail.CommentsE = "NA";
                        expDetail.RefNo = "NA";

                        saleExport.Add(expDetail);
                    }
                #endregion export

                #region Tracking
                    //if (trackingsVm.Count > 0)
                    //{
                    //    trackingsVm[0].IsSale = "Y";
                    //}
                    #endregion Tracking

                }
                #endregion save
                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (ChkExpLoc.Checked)
                {
                    if (saleExport.Count <= 0)
                    {
                        MessageBox.Show("Please insert packing information for export", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                        ExportPackagingInfoAdd();
                        return;
                    }
                }

                if (TrackingTrace == true)
                {
                    for (int i = 0; i < dgvSale.Rows.Count; i++)
                    {
                        string itemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());

                        var p = from productCmb in trackingsVm.ToList()
                                where productCmb.FinishItemNo == itemNo
                                select productCmb;

                        if (p != null && p.Any())
                        {
                            var trackingInfo = p.First();
                            decimal fQty = trackingInfo.Quantity;

                            if (Quantity > fQty || Quantity < fQty)
                            {
                                MessageBox.Show("Please insert correct number of tracking.", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                }
                this.progressBar1.Visible = true;
                this.btnSave.Enabled = false;
                //this.Enabled = false;

                backgroundWorkerSave.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
        private void backgroundWorkerSave_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {

                #region statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                SaleDAL saleDal = new SaleDAL();
                sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport, trackingsVm, null, null);
                SAVE_DOWORK_SUCCESS = true;

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 2)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
        private void backgroundWorkerSave_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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

                        }
                        if (result == "Success")
                        {
                            txtSalesInvoiceNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvSale.RowCount; i++)
                            {
                                dgvSale["Status", dgvSale.RowCount - 1].Value = "Old";
                            }
                        }

                    }

                ChangeData = false;
                #endregion
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
                this.btnSave.Enabled = true;
                //this.Enabled = true;

            }

        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if( ChkExpLoc.Checked)
                {
                    var tt = "";
                }
                #region save
                TransactionTypes();
                #region start
                //if (Edit == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
                //    return;
                //}
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Convert.ToDateTime(dtpDeliveryDate.Value.ToString("yyyy-MMM-dd")) <
                    Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Delivery date not before invoice date", this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                SetupVATStatus();
                if ((PrepaidVAT == true && rbtnService.Checked == false) || (PrepaidVAT == true && rbtnServiceNS.Checked == false))
                {
                    if (Convert.ToDecimal(Program.VATAmount + PreVATAmount) <
                        Convert.ToDecimal(txtTotalVATAmount.Text.Trim()))
                    {
                        MessageBox.Show("You have no balance of VAT for sales", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                }

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtSalesInvoiceNo.Text.Trim();
                }
                #endregion start
                #region Null

                if (txtCustomerID.Text == "")
                {

                    MessageBox.Show("Please Enter Customer Information");
                    cmbCustomerName.Focus();
                    return;
                }
                if (rbtnService.Checked == false && rbtnServiceNS.Checked == false)
                {
                    if (cmbVehicleNo.Text == "")
                    {

                        MessageBox.Show("Please Enter Vehicle Number");
                        cmbVehicleNo.Focus();
                        return;
                    }
                    if (txtVehicleType.Text == "")
                    {
                        txtVehicleType.Focus();
                        MessageBox.Show("Please Enter Vehicle Type");
                        txtVehicleType.ReadOnly = false;
                        return;
                    }
                   
                }
               
                if (cmbType.Text == "New")
                {
                    txtOldID.Text = "0.00";
                }
                else
                {
                    if (txtOldID.Text == "")
                    {
                        MessageBox.Show("Please Enter previous Invoice No");

                        return;
                    }
                }

                if (dgvSale.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }
                if (rbtnExport.Checked)
                {
                    if (dgvExport.RowCount <= 0)
                    {
                        MessageBox.Show("Please insert packing information for export", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                        return;
                    }
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtDeliveryAddress1.Text == "")
                {
                    txtDeliveryAddress1.Text = "-";
                }
                if (txtDeliveryAddress2.Text == "")
                {
                    txtDeliveryAddress2.Text = "-";
                }
                if (txtDeliveryAddress3.Text == "")
                {
                    txtDeliveryAddress3.Text = "-";
                }
                if (rbtnExport.Checked == true)
                {
                    if (txtLCNumber.Text == "")
                    {
                        MessageBox.Show("Please Enter LC Number");
                        txtLCNumber.Focus();
                        return;
                    }
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }


                #endregion Null
                #region Tracking Check

                if (TrackingTrace == true)
                {
                    if (trackingsVm.Count <= 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information have not been added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                        {
                            btnTracking_Click(sender, e);
                            return;
                        }

                    }

                }

                #endregion


                saleMaster = new SaleMasterVM();
                saleMaster.SalesInvoiceNo = NextID.ToString();

                saleMaster.CustomerID = txtCustomerID.Text.Trim();
                saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                saleMaster.vehicleSaveInDB = false;
                if (chkVehicleSaveInDatabase.Checked == true)
                {
                    saleMaster.vehicleSaveInDB = true;
                }
                saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                saleMaster.SerialNo = txtSerialNo.Text.Trim();
                saleMaster.Comments = txtComments.Text.Trim();
                saleMaster.CreatedBy = Program.CurrentUser;
                saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.LastModifiedBy = Program.CurrentUser;
                saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.SaleType = cmbType.Text.Trim();
                saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                saleMaster.IsPrint = "N";
                saleMaster.TenderId = TenderId;
                saleMaster.TransactionType = transactionType;
                saleMaster.DeliveryDate = dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.VehicleNo = cmbVehicleNo.Text.Trim();
                saleMaster.VehicleType = txtVehicleType.Text.Trim();
                saleMaster.Post = "N"; //Post
                saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate
                saleMaster.LCNumber = txtLCNumber.Text.Trim(); //Post
                saleMaster.ImportIDExcel = txtImportID.Text.Trim(); //ImportID
                

                saleDetails = new List<SaleDetailVm>();

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleDetailVm detail = new SaleDetailVm();

                    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());

                    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();

                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.SaleTypeD = cmbType.Text.Trim();
                    detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();
                    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                    detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                    detail.TradingMarkUp = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                    detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                    detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                    detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                    detail.VatName = dgvSale.Rows[i].Cells["VATName"].Value.ToString();
                    detail.Weight = dgvSale.Rows[i].Cells["Weight"].Value.ToString();
                    if (!chkTrading.Checked)
                    {
                        detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                    }
                    if (rbtnDN.Checked || rbtnCN.Checked)
                    {
                        detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                    }

                    saleDetails.Add(detail);
                }
                if (ChkExpLoc.Checked)
                {

                    saleExport.Clear();
                    for (int i = 0; i < dgvExport.RowCount; i++)
                    {
                        if (string.IsNullOrEmpty(dgvExport.Rows[i].Cells["Description"].Value.ToString()) ||
                             dgvExport.Rows[i].Cells["Description"].Value == null ||
                             dgvExport.Rows[i].Cells["QuantityE"].Value == null ||
                             dgvExport.Rows[i].Cells["GrossWeight"].Value == null ||
                             dgvExport.Rows[i].Cells["NetWeight"].Value == null ||
                             dgvExport.Rows[i].Cells["NumberFrom"].Value == null ||
                             dgvExport.Rows[i].Cells["NumberTo"].Value == null ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["QuantityE"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NetWeight"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString()) ||
                            string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberTo"].Value.ToString()))
                        {
                            MessageBox.Show("Please check the export Details, All the data must fill", this.Text);
                            ExportPackagingInfoAdd();
                            return;
                        }

                        SaleExportVM expDetail = new SaleExportVM();
                        expDetail.SaleLineNo = dgvExport.Rows[i].Cells["LineNoE"].Value.ToString();
                        expDetail.Description = dgvExport.Rows[i].Cells["Description"].Value.ToString();
                        expDetail.QuantityE = dgvExport.Rows[i].Cells["QuantityE"].Value.ToString();
                        expDetail.GrossWeight =dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString();
                        expDetail.NetWeight = dgvExport.Rows[i].Cells["NetWeight"].Value.ToString();
                        expDetail.NumberFrom = dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString();
                        expDetail.NumberTo = dgvExport.Rows[i].Cells["NumberTo"].Value.ToString();
                        expDetail.CommentsE = "NA";
                        expDetail.RefNo = "NA";

                        saleExport.Add(expDetail);
                    }
                }
                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (ChkExpLoc.Checked)
                {
                    if (saleExport.Count <= 0)
                    {
                        MessageBox.Show("Please insert packing information for export", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                        return;
                    }
                }

                if (TrackingTrace == true)
                {
                    for (int i = 0; i < dgvSale.Rows.Count; i++)
                    {
                        string itemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());

                        var p = from productCmb in trackingsVm.ToList()
                                where productCmb.FinishItemNo == itemNo
                                select productCmb;

                        if (p != null && p.Any())
                        {
                            var trackingInfo = p.First();
                            decimal fQty = trackingInfo.Quantity;

                            if (Quantity > fQty || Quantity < fQty)
                            {
                                MessageBox.Show("Please insert correct number of tracking.", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                }
                #endregion save
                this.bthUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                bgwUpdate.RunWorkerAsync();

            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                // Start DoWork
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                SaleDAL saleDal = new SaleDAL();

                sqlResults = saleDal.SalesUpdate(saleMaster, saleDetails, saleExport, trackingsVm);
                UPDATE_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 2)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
                string exMessage = ex.StackTrace;
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
                        var ss = transactionType;

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

                        }

                        if (result == "Success")
                        {
                            txtSalesInvoiceNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvSale.RowCount; i++)
                            {
                                dgvSale["Status", dgvSale.RowCount - 1].Value = "Old";
                            }
                        }
                    }




                // End Complete

                #endregion
                ChangeData = false;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
                ChangeData = false;
                //this.Enabled = true;
                this.bthUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {


                TransactionTypes();

                #region Start
  if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                else if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }
                else
                {
                    if (Convert.ToDateTime(dtpDeliveryDate.Value.ToString("yyyy-MMM-dd")) <
                        Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")))
                    {
                        MessageBox.Show("Delivery date  and time not before invoice date", this.Text);
                        return;
                    }
                    if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                    {
                        MessageBox.Show(
                            "Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }
                    SetupVATStatus();
                    if ((PrepaidVAT == true && rbtnService.Checked == false) || (PrepaidVAT == true && rbtnServiceNS.Checked == false))
                    {
                        if (Convert.ToDecimal(Program.VATAmount + PreVATAmount) <
                            Convert.ToDecimal(txtTotalVATAmount.Text.Trim()))
                        {
                            MessageBox.Show("You have no balance of VAT for sales", this.Text, MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            return;
                        }
                    }
                    //arafat

                    //string SalesInvoiceHeaderData = string.Empty;//string encriptedSalesInvoiceHeaderData = string.Empty;//string SalesInvoiceDetailData = string.Empty;//string SalesInvoiceExportData = string.Empty;

                    if (IsUpdate == false)
                    {
                        NextID = string.Empty;
                    }
                    else
                    {
                        NextID = txtSalesInvoiceNo.Text.Trim();
                    }

                    #region Null

                    if (txtCustomerID.Text == "")
                    {

                        MessageBox.Show("Please Enter Customer Information");
                        cmbCustomerName.Focus();
                        return;
                    }
                    if (rbtnService.Checked == false && rbtnServiceNS.Checked == false)
                    {
                        if (cmbVehicleNo.Text == "")
                        {

                            MessageBox.Show("Please Enter Vehicle Number");
                            cmbVehicleNo.Focus();
                            return;
                        }
                        if (txtVehicleType.Text == "")
                        {
                            txtVehicleType.Focus();
                            MessageBox.Show("Please Enter Vehicle Type");
                            txtVehicleType.ReadOnly = false;

                            return;
                        }

                    }
                    if (cmbType.Text == "New")
                    {
                        txtOldID.Text = "0.00";
                    }
                    else
                    {
                        if (txtOldID.Text == "")
                        {
                            MessageBox.Show("Please Enter previous Invoice No");

                            return;
                        }
                    }

                    if (dgvSale.RowCount <= 0)
                    {
                        MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
                        return;
                    }

                    if (txtComments.Text == "")
                    {
                        txtComments.Text = "-";
                    }
                    if (txtDeliveryAddress1.Text == "")
                    {
                        txtDeliveryAddress1.Text = "-";
                    }
                    if (txtDeliveryAddress2.Text == "")
                    {
                        txtDeliveryAddress2.Text = "-";
                    }
                    if (txtDeliveryAddress3.Text == "")
                    {
                        txtDeliveryAddress3.Text = "-";
                    }
                    if (txtSerialNo.Text == "")
                    {
                        if (rbtnExport.Checked)
                        {
                            MessageBox.Show("Please Enter Referance Number");
                            txtSerialNo.Focus();
                            return;
                        }
                        else
                        {
                            txtSerialNo.Text = "-";
                        }



                    }

                    #endregion Null



                #endregion Start

                    saleMaster = new SaleMasterVM();
                    saleMaster.SalesInvoiceNo = NextID.ToString();

                    saleMaster.CustomerID = txtCustomerID.Text.Trim();
                    saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                    saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                    saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                    saleMaster.VehicleType = txtVehicleType.Text.Trim();
                    saleMaster.InvoiceDateTime =
                        dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                    saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                    saleMaster.SerialNo = txtSerialNo.Text.Trim();
                    saleMaster.Comments = txtComments.Text.Trim();
                    saleMaster.CreatedBy = Program.CurrentUser;
                    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.LastModifiedBy = Program.CurrentUser;
                    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.SaleType = cmbType.Text.Trim();
                    saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                    saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                    saleMaster.IsPrint = "N";
                    saleMaster.TenderId = TenderId;
                    saleMaster.TransactionType = transactionType;
                    saleMaster.DeliveryDate =
                        dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.VehicleNo = cmbVehicleNo.Text.Trim();
                    saleMaster.Post = "Y"; //Post
                    saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                    saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate
                    saleMaster.LCNumber = txtLCNumber.Text.Trim(); //Post

                    saleDetails = new List<SaleDetailVm>();

                    for (int i = 0; i < dgvSale.RowCount; i++)
                    {
                        SaleDetailVm detail = new SaleDetailVm();

                        detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                        detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                        detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                        detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());
                        detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                        detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                        detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                        detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                        detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                        detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                        detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();

                        detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                        detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                        detail.SaleTypeD = cmbType.Text.Trim();
                        detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();
                        detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                        detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                        detail.TradingMarkUp =
                            Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                        detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                        detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                        detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                        detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                        detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                        detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                        detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                        detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                        detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                        saleDetails.Add(detail);
                    } // end of for
                    if (rbtnExport.Checked)
                    {

                        saleExport.Clear();
                        for (int i = 0; i < dgvExport.RowCount; i++)
                        {
                            SaleExportVM expDetail = new SaleExportVM();
                            expDetail.SaleLineNo = dgvExport.Rows[i].Cells["LineNoE"].Value.ToString();
                            expDetail.Description = dgvExport.Rows[i].Cells["Description"].Value.ToString();
                            expDetail.QuantityE = dgvExport.Rows[i].Cells["QuantityE"].Value.ToString();
                            expDetail.GrossWeight = dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString();
                            expDetail.NetWeight = dgvExport.Rows[i].Cells["NetWeight"].Value.ToString();
                            expDetail.NumberFrom = dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString();
                            expDetail.NumberTo = dgvExport.Rows[i].Cells["NumberTo"].Value.ToString();
                            expDetail.CommentsE = "NA";
                            saleExport.Add(expDetail);
                        }
                    } //end of if (rbtnExport.Checked)
                }


                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (rbtnExport.Checked)
                {
                    if (saleExport.Count <= 0)
                    {
                        MessageBox.Show("Please insert packing information for export", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                        return;
                    }
                }
                //#endregion
                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;
                while (bgwPost.IsBusy)
                {
                    
                }
                bgwPost.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                string exMessage = ex.Message;
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
        private void bgwPost_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
            try
            {
                #region Statement
                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                SaleDAL saleDal = new SaleDAL();

                sqlResults = saleDal.SalesPost(saleMaster, saleDetails, trackingsVm);
                POST_DOWORK_SUCCESS = true;


                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message;
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length>1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                

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
                string exMessage = ex.StackTrace;
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
        private void bgwPost_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (POST_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string message = sqlResults[1];
                        string result = sqlResults[0];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwPost_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;

                        }

                        if (result == "Success")
                        {
                            txtSalesInvoiceNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvSale.RowCount; i++)
                            {
                                dgvSale["Status", dgvSale.RowCount - 1].Value = "Old";
                            }
                        }
                    }




                // End Complete

                #endregion
                ChangeData = false;
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
                ChangeData = false;
                //this.Enabled = true;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }

        public void ClearAllFields()
        {
            txtVehicleType.Text = string.Empty;
            txtTotalSDAmount.Text = "0.00";

            txtOldID.Text = "";
            if (chkSameCustomer.Checked == false)
            {
                cmbCustomerName.Text = "Select";
                txtCustomerID.Text = "";
                txtCustomerGroupName.Text = "";
                txtDeliveryAddress1.Text = "";
                txtDeliveryAddress2.Text = "";
                txtDeliveryAddress3.Text = "";


            }
            if (chkSameVehicle.Checked == false)
            {
                txtVehicleID.Text = "";
                txtVehicleNo.Text = "";
                txtVehicleType.Text = "";
            }
            chkVehicleSaveInDatabase.Visible = false;
            cmbProduct.Text = "Select";
            //cmbProductType.Text = "Select";
            //cmbType.Text = "Select";

            dtpInvoiceDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dtpDeliveryDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            //zTvrNxNvP08=);
            txtNBRPrice.Text = "0.00";
            //txtCategoryName.Text = "";
            txtComments.Text = "";
            txtCommentsDetail.Text = "NA";
            
            txtHSCode.Text = "";
            txtLineNo.Text = "";
            //txtNBRPrice.Text = "0.00";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            if (rbtnServiceNS.Checked)
            {
                txtQuantity.Text = "1";
                txtQty1.Text = "1";
                txtQty2.Text = "0";

            }
            else
            {
                txtQuantity.Text = "0.00";
                txtQty1.Text = "0";
                txtQty2.Text = "0";
            }




            txtSalesInvoiceNo.Text = "";
            txtSerialNo.Text = "";
            txtTotalAmount.Text = "0.00";
            txtTotalVATAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            txtVATRate.Text = "0.00";
            
            txtUOM.Text = "";
            txttradingMarkup.Text = "0.00";
            txtSD.Text = "0.00";
            txtQuantityInHand.Text = "0.0";
            txtTDBalance.Text = "0.00";
            txtLCNumber.Text = "";
            txtTotalSubTotal.Clear();
            dgvSale.Rows.Clear();
            dgvExport.Rows.Clear();
            if (gbExport.Visible == true)
            {
                gbExport.Visible = false;
                GF.Visible = true;
            }
            txtCustomerName.Text = "";
            txtWeight.Text = "";
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {

            if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            {
                txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), SalePlaceQty).ToString();
            }

        }

        private void txtNBRPrice_Leave(object sender, EventArgs e)
        {
            //btnAdd.Focus();
            if (Program.CheckingNumericTextBox(txtNBRPrice, "NBR price") == true)
            {
                txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), SalePlaceRate).ToString();
                if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) < vNBRPrice)
                {
                    MessageBox.Show("Vatable Sale Price can't less then Price declaration Price");
                    txtNBRPrice.Text = vNBRPrice.ToString();
                    return;
                }
            }
           
        }
        private void NBRPriceCall()
        {
            decimal tenderPrice = 0;
            ProductDAL productDal = new ProductDAL();
            var prodByCode = from prd in ProductsMini.ToList()
                             where prd.ItemNo.ToLower() == txtProductCode.Text.Trim().ToLower()
                             select prd;
            if (prodByCode != null && prodByCode.Any())
            {
                var products = prodByCode.First();
                txtProductName.Text = products.ProductName;
                cmbProduct.Text = products.ProductName;
                txtProductCode.Text = products.ItemNo;
                txtVATRate.Text = products.VATRate.ToString(); //"0.00");
                txtSD.Text = products.SD.ToString(); //"0.00");
                tenderPrice = products.NBRPrice;

            }

            //txtNBRPrice.Text = products.NBRPrice.ToString();     
            //txtNBRPrice.Text = products.NBRPrice.ToString();

            if (rbtnTender.Checked == true)
            {
                //txtDiscountedNBRPrice.Text = tenderPrice.ToString();
            }
            else if (rbtnTradingTender.Checked == true)
            {
                //txtDiscountedNBRPrice.Text = tenderPrice.ToString();
            }
            else if (rbtnTollIssue.Checked)
            {

                DataTable priceData = productDal.AvgPriceNew(txtProductCode.Text.Trim(),
                                                             dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                             DateTime.Now.ToString(" HH:mm:00"), null, null, false);

                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                if (quantity > 0)
                {
                    txtDiscountedNBRPrice.Text = (amount/quantity).ToString();
                }
                else
                {
                    txtDiscountedNBRPrice.Text = "0";
                }
            }
            else if (rbtnServiceNS.Checked)
            {
                txtQuantity.Text = "1";
                txtDiscountedNBRPrice.Text =
                    productDal.GetLastNBRPriceFromBOM_VatName(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                      dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null).ToString();
            }
            else if (rbtnVAT11GaGa.Checked)
            {
                txtDiscountedNBRPrice.Text = "0";

            }
            else
            {
                if (string.IsNullOrEmpty(cmbVAT1Name.Text.Trim()))
                {
                    MessageBox.Show("Please Select NBR Price Declaration", this.Text);
                    return;
                }
                txtDiscountedNBRPrice.Text = productDal.GetLastNBRPriceFromBOM_VatName(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(), 
                                                                        dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null).ToString();

                //txtNBRPrice.Text = productDal.GetLastNBRPriceFromBOM(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                //                                                        dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null).ToString();

            }
            txtDiscountedNBRPrice.Text = Program.FormatingNumeric(txtDiscountedNBRPrice.Text.Trim(), SalePlaceRate).ToString();
            if (rbtnExport.Checked || rbtnExportService.Checked
                || rbtnExportServiceNS.Checked
                || rbtnExportTender.Checked
                || rbtnExportTrading.Checked
                || rbtnExportTradingTender.Checked
                )
            {
                txtVATRate.Text = "0";
                txtSD.Text = "0";
            }
            Discount();

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
                            cmbProductCode.Text = userSelRow.Cells["PCode"].Value.ToString();

                            cmbProduct.Text = userSelRow.Cells["ItemName"].Value.ToString();
                            txtLineNo.Text = dgvSale.CurrentCellAddress.Y.ToString();
                            txtProductCode.Text = userSelRow.Cells["ItemNo"].Value.ToString();
                            txtPCode.Text = userSelRow.Cells["PCode"].Value.ToString();

                            txtProductName.Text = userSelRow.Cells["ItemName"].Value.ToString();



                            txtQuantity.Text = Convert.ToDecimal(userSelRow.Cells["Quantity"].Value).ToString();//"0,0.0000");

                            txtQty1.Text =
                                Convert.ToDecimal(Convert.ToDecimal(userSelRow.Cells["Quantity"].Value) -
                                                  Convert.ToDecimal(userSelRow.Cells["PromotionalQuantity"].Value)).
                                    ToString();
                            txtQty2.Text = Convert.ToDecimal(userSelRow.Cells["PromotionalQuantity"].Value).ToString();//"0,0.0000");


                            txtUnitCost.Text =
                                Convert.ToDecimal(userSelRow.Cells["UnitPrice"].Value).ToString();//"0.00");
                            txtVATRate.Text = Convert.ToDecimal(userSelRow.Cells["VATRate"].Value).ToString();//"0.00");
                            txtCommentsDetail.Text = "NA";
                            //= dgvSale.CurrentRow.Cells["Comments"].Value.ToString();
                            //txtNBRPrice.Text = Convert.ToDecimal(dgvSale.CurrentRow.Cells["NBRPrice"].Value).ToString();//"0.00");
                            txtNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["UOMPrice"].Value).ToString();//"0.00");
                            txtUOM.Text = userSelRow.Cells["UOMn"].Value.ToString();
                            cmbUom.Items.Insert(0, txtUOM.Text.Trim());

                            //UomsValue();
                            Uoms();
                            cmbUom.Text = userSelRow.Cells["UOM"].Value.ToString();

                            txtSD.Text = Convert.ToDecimal(userSelRow.Cells["SD"].Value).ToString();//"0.00");
                            txtPrevious.Text = Convert.ToDecimal(userSelRow.Cells["Previous"].Value).ToString();//"0.00");
                            txtUomConv.Text = Convert.ToDecimal(userSelRow.Cells["UOMc"].Value).ToString();//"0.00");

                            txtDiscountAmountInput.Text = Convert.ToDecimal(userSelRow.Cells["DiscountAmount"].Value).ToString();//"0.00");
                            txtDiscountAmount.Text = Convert.ToDecimal(userSelRow.Cells["DiscountAmount"].Value).ToString();//"0.00");
                            txtDiscountedNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["DiscountedNBRPrice"].Value).ToString();//"0.00");
                            cmbVAT1Name.Text= userSelRow.Cells["VATName"].Value.ToString();
                            cmbVAT1Name.SelectedItem = userSelRow.Cells["VATName"].Value.ToString().Trim();
                            if (userSelRow.Cells["VATName"].Value.ToString().Trim()=="VAT 4.3 (Wastage)")
                            {
                                cmbVAT1Name.SelectedIndex = 10; 
                            }
                            if (!chkTrading.Checked)
                            {
                                dtpConversionDate.Value = Convert.ToDateTime(userSelRow.Cells["CConvDate"].Value.ToString());

                                CurrencyConversiondate = userSelRow.Cells["CConvDate"].Value.ToString();

                                dtpConversionDate_Leave(sender, e);
                            }
                           
                            ProductDAL productDal = new ProductDAL();
                            if ( cmbVAT1Name.SelectedIndex != 10)
                            {
                                txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                     DateTime.Now.ToString(" HH:mm:00"), null, null, false).Rows[0]["Quantity"].ToString();
                            }
                            else
                            {
                                txtQuantityInHand.Text = "0";
                            }
                            
                            chkDiscount.Checked = false;

                            if (rbtnCN.Checked == true || rbtnDN.Checked == true)
                            {
                                string vatNameInCD = userSelRow.Cells["VATName"].Value.ToString();
                                SelectVATName(vatNameInCD);
                            }
                           
                            NBRPriceCall();
                            if (transactionTypeOld == "Service" || transactionTypeOld == "ServiceNS" || txtCategoryName.Text=="Export"||transactionTypeOld=="Export")
                            {
                                //if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) == 0)
                                //{
                                    txtNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["NBRPrice"].Value).ToString();

                                //}
                            }
                            #region Find Type
                             cmbVATType.Text= userSelRow.Cells["Type"].Value.ToString().Trim();
                            cmbVATType.SelectedItem = userSelRow.Cells["Type"].Value.ToString().Trim();
                            #endregion
                            txtWeight.Text = userSelRow.Cells["Weight"].Value.ToString().Trim();

                        }
                    }
                }



            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
           this.Close();
        }


        private void txtSalesInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }



        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {
                        txtQty1.Focus();
                }
                if (e.KeyCode.Equals(Keys.F9))
                {
                    
                    DataGridViewRow selectedRow = new DataGridViewRow();
                    selectedRow = FormProductSearch.SelectOne();
                    if (selectedRow != null && selectedRow.Selected == true)
                    {
                        //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                        cmbProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                        cmbProductCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();// ProductInfo[27].ToString();//27
                    }
                    cmbProduct.Focus();

                }
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_KeyDown", exMessage);
            }

            #endregion


        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {

                    //btnAdd.Focus();
                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "txtQuantity_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "txtQuantity_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "txtQuantity_KeyDown", exMessage);
            }

            #endregion
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtSD.Focus();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                PreviewOnly = false;

                //this.btnPrint.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                backgroundWorkerPrint.RunWorkerAsync();

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }

            #endregion

        }
        private void backgroundWorkerPrint_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {


                UPDATEPRINT_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

               

                Program.fromOpen = "Me";
                string result = FormSalePrint.SelectOne(AlReadyPrintNo);
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());
                    PrintCopy = Convert.ToInt32(PrintResult[0]);
                    WantToPrint = PrintResult[1];
                    VPrinterName = PrintResult[2];

                }
                
                #region Update table
                
                if (WantToPrint=="Y")
                {
                    SaleDAL PrintUpdate = new SaleDAL();
                    sqlResults = PrintUpdate.UpdatePrintNew(txtSalesInvoiceNo.Text.Trim(), PrintCopy);// Change 04

                    UPDATEPRINT_DOWORK_SUCCESS = true;
                }
                else
                {
                    UPDATEPRINT_DOWORK_SUCCESS = false;

                }
                #endregion

                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerPrint_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                //start Complete

                if (UPDATEPRINT_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        NewPrintCopy =Convert.ToInt32(sqlResults[3]);

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerPrint_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            if (result == "Fail")
                            {
                                MessageBox.Show("Can't print please contact with administrator");
                            }
                            else if (result == "Success")
                            {
                                Program.Trading = Convert.ToString(chkTrading.Checked ? true : false);
                                ReportVAT11Ds();
                                chkPrint.Checked = true;
                            }
                        }

                    }
                }


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", exMessage);
            }

            #endregion

            //this.btnPrint.Enabled = true;
            //this.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void backgroundWorkerReportVAT11Ds_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                ReportDSDAL showReport = new ReportDSDAL();
                ReportResultVAT11 = new DataSet();
                ReportResultCreditNote = new DataSet();
                ReportResultDebitNote = new DataSet();
                ReportResultTracking = new DataSet();
                if (rbtnCN.Checked)
                {
                    ReportResultCreditNote = showReport.CreditNoteNew(txtSalesInvoiceNo.Text.Trim(), post1, post2);
                }
                else if (rbtnDN.Checked)
                {
                    ReportResultDebitNote = showReport.DebitNoteNew(txtSalesInvoiceNo.Text.Trim(), post1, post2);
                }
                else
                {
                    ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, varTrading, post1, post2);
                    ReportResultTracking = showReport.SaleTrackingReport(varSalesInvoiceNo);
                }
                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerReportVAT11Ds_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region
                string vQuantity, vSDAmount, vQty_UnitCost, vQty_UnitCost_SDAmount, vVATAmount, vSubtotal, QtyInWord;// = string.Empty;  

                vQuantity = string.Empty;
                vSDAmount = string.Empty;
                vQty_UnitCost = string.Empty;
                vQty_UnitCost_SDAmount = string.Empty;
                vVATAmount = string.Empty;
                vSubtotal = string.Empty;
                QtyInWord = string.Empty;
                decimal Quantity = 0,
                           SDAmount = 0,
                           Qty_UnitCost = 0,
                           Qty_UnitCost_SDAmount = 0,
                           VATAmount = 0,
                           Subtotal = 0;

                //start Complete
                if (rbtnCN.Checked)
                {
                    ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";
                }
                else if (rbtnDN.Checked)
                {

                    ReportResultDebitNote.Tables[0].TableName = "DsDebitNote";
                }
                else
                {
                    ReportResultVAT11.Tables[0].TableName = "DsVAT11";
                    ReportResultTracking.Tables[0].TableName = "DsSaleTracking";
                    #region InWord

                    for (int i = 0; i < ReportResultVAT11.Tables[1].Rows.Count; i++)
                    {
                        QtyInWord = QtyInWord + ReportResultVAT11.Tables[1].Rows[i]["uom"].ToString() + ": " +
                                    Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[1].Rows[i]["qty"])) + ", ";
                    }
                    
                    //DataTable uomNames = ReportResultVAT11.Tables[1].DefaultView.ToTable(true, "uom");
                    //for (int i = 0; i < uomNames.Rows.Count; i++)
                    //{
                    //    decimal qty = 0;
                    //    DataRow[] dt = ReportResultVAT11.Tables[1].Select("uom ='" + uomNames.Rows[i][0].ToString() + "'");
                    //    foreach (var item in dt)
                    //    {
                    //        qty = qty + Convert.ToDecimal(item["qty"].ToString());
                    //    }
                    //    QtyInWord = QtyInWord + uomNames.Rows[i][0].ToString() + ": " +
                    //               Program.NumberToWords(Convert.ToInt32(qty)) + ", ";
                    //}
                   

                    if (Convert.ToInt32(QtyInWord.Length) <= 0)
                    {
                        QtyInWord = "In Words(Quantity): .";

                    }
                    else
                    {
                        QtyInWord = "In Words(Quantity): " +
                                    QtyInWord.Substring(0, Convert.ToInt32(QtyInWord.Length) - 2).ToString() + ".";

                    }
                    QtyInWord = QtyInWord.ToUpper();


                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                        SDAmount = SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        Qty_UnitCost = Qty_UnitCost + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]);
                        Qty_UnitCost_SDAmount = Qty_UnitCost_SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]) + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        VATAmount = VATAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["VATAmount"]);
                        Subtotal = Subtotal + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Subtotal"]);
                    }

                    vQuantity = Convert.ToDecimal(Quantity).ToString("0,0.00");
                    vSDAmount = Convert.ToDecimal(SDAmount).ToString("0,0.00");
                    vQty_UnitCost = Convert.ToDecimal(Qty_UnitCost).ToString("0,0.00");
                    vQty_UnitCost_SDAmount = Convert.ToDecimal(Qty_UnitCost_SDAmount).ToString("0,0.00");
                    vVATAmount = Convert.ToDecimal(VATAmount).ToString("0,0.00");
                    vSubtotal = Convert.ToDecimal(Subtotal).ToString("0,0.00");


                    #endregion InWord


                }


                #endregion
                string prefix = "";
                ReportClass objrpt = new ReportClass();
                #region CN
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (rbtnCN.Checked)
                {

                    objrpt = new RptCreditNote();
                    prefix = "CreditNote";

                    objrpt.SetDataSource(ReportResultCreditNote);


                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";


                }
                #endregion CN
                #region DN
                else if (rbtnDN.Checked)
                {
                    objrpt = new RptDebitNote();
                    prefix = "DebitNote";


                    objrpt.SetDataSource(ReportResultDebitNote);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                }
                #endregion CN
                #region rbtnTrading
                else if (rbtnTrading.Checked)
                {

                    //objrpt = new RptVAT11Ka();
                    prefix = "VAT11Ka";

                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                }//end if
                #endregion rbtnTrading
                #region rbtnService
                else if (rbtnService.Checked || rbtnServiceNS.Checked)
                {
                    if (chkIs11.Checked)
                    {
                        //objrpt = new RptVAT11();
                        prefix = "VAT11";
                    }
                    else
                    {
                        //objrpt = new RptVAT11Gha();
                        prefix = "VAT11Gha";
                    }
                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11 Gha'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo +
                                                                                    "'";

                    #region NEW ADDED BY LITON

                    //vQuantity,vSDAmount,vQty_UnitCost,vQty_UnitCost_SDAmount,vVATAmount,vSubtotal
                    objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";
                    objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                    objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                    objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                    objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                    objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                    objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                    objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";

                    #endregion NEW ADDED BY LITON


                }//end
                #endregion rbtnService
                #region rbtnTollIssue
                else if (rbtnTollIssue.Checked)
                {
                    //objrpt = new RptVAT11Ga();
                    prefix = "VAT11Ga";
                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11 Ga'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo +
                                                                                    "'";
                }
                #endregion rbtnTollIssue
                #region rbtnVAT11GaGa
                else if (rbtnVAT11GaGa.Checked)
                {
                    //objrpt = new RptVAT11GaGa();
                    prefix = "VAT11GaGa";
                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                }
                #endregion rbtnVAT11GaGa
                #region Other
                else
                {
                    prefix = "VAT11";
                    if (chkIsBlank.Checked)
                    {
                        //objrpt = new RptVAT11_Blank();
                        if (TrackingTrace == true)
                        {
                            //objrpt = new RptVAT11T_Blank();
                        }
                        else
                        {
                            //Letter format
                            if (string.IsNullOrEmpty(txtCustomerName.Text))
                            {
                                //objrpt = new RptVAT11LetterFormat();
                            }
                            else
                            {
                                //objrpt = new RptVAT11_Blank();

                            }
                            
                        }
                        
                    }
                    else
                    {

                        if (VAT11PageSize == "A4")
                        {
                            if (TrackingTrace == true)
                            {
                                //objrpt = new RptVAT11T();
                            }
                            else
                            {
                                //objrpt = new RptVAT11();
                            }
                        }
                        else if (VAT11PageSize == "Letter")
                        {
                            if (TrackingTrace == true)
                            {
                                //objrpt = new RptVAT11LetterT();
                            }
                            else
                            {
                                //objrpt = new RptVAT11Letter();
                            }
                        }
                        else if (VAT11PageSize == "Legal")
                        {
                            
                            if (TrackingTrace == true)
                            {
                                //objrpt = new RptVAT11LegalT();
                            }
                            else
                            {
                                //objrpt = new RptVAT11Legal();
                            }

                        }
                        else
                        {
                            if (TrackingTrace == true)
                            {
                                //objrpt = new RptVAT11T();
                            }
                            else
                            {
                                //objrpt = new RptVAT11();
                            }
                        }
                    }
                        ////vQuantity,vSDAmount,vQty_UnitCost,vQty_UnitCost_SDAmount,vVATAmount,vSubtotal
                        objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                        objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";
                        objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";

                    objrpt.SetDataSource(ReportResultVAT11);

                   
                    if (TrackingTrace == true)
                    {
                        objrpt.Subreports[0].SetDataSource(ReportResultTracking);
                        objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName1"].Text = "'" + Heading1 + "'";
                        objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName2"].Text = "'" + Heading2 + "'";
                    }

                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                    objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";


                }//end else
                #endregion Other

                #region currency
                SaleDAL saleDal=new SaleDAL();

                string currencyMajor = "";
                string currencyMinor = "";
                string currencySymbol = "";
                sqlResults=new string[4];
                if (!string.IsNullOrEmpty(txtCustomerName.Text))
                {
                    sqlResults = saleDal.CurrencyInfo(txtSalesInvoiceNo.Text);
                    if (sqlResults[0].ToString() != "Fail")
                    {
                        currencyMajor = sqlResults[1].ToString();
                        currencyMinor = sqlResults[2].ToString();
                        currencySymbol = sqlResults[3].ToString();
                    }
                }
                

                #endregion currency

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMajor"].Text = "'" + currencyMajor + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMinor"].Text = "'" + currencyMinor + "'";
                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "'" +currencySymbol + "'";

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                if (PreviewOnly == true)
                {
                    //Preview Only
                    if (!string.IsNullOrEmpty(txtCustomerName.Text))
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                    }
                    objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "''";

                    reports.setReportSource(objrpt);
                        if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                            //reports.ShowDialog();
                            reports.WindowState = FormWindowState.Maximized;
                        reports.Show();
                }
                else
                {

                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                        string copiesNo = (AlReadyPrintNo + i).ToString();
                        int cpno = AlReadyPrintNo + i;

                        #region CopyName

                        if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                            (cpno >= 44 && cpno <= 50))
                        {
                            copiesNo = cpno + " th copy";
                        }
                        else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        {
                            copiesNo = cpno + " st copy";
                        }
                        else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        {
                            copiesNo = cpno + " nd copy";
                        }
                        else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        {
                            copiesNo = cpno + " rd copy";
                        }
                        else
                        {

                            copiesNo = cpno + " copy";

                        }



                        #endregion CopyName

                        if (Is3Plyer == "Y")
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "";
                        }
                        else
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                        }


                        #region Save & Print in PDF format

                        ReportDocument rptDoc = objrpt;
                        string printInvoiceNo = txtSalesInvoiceNo.Text.Trim().Replace("/", "-");
                        //PrintLocation = Directory.GetCurrentDirectory() + @"\NBR_Reports";
                        PrintLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBR_Reports");

                        
                        string saveLocation = PrintLocation + @"\" + prefix + "_" + printInvoiceNo + "_" + copiesNo +
                                              ".pdf";
                        if (!Directory.Exists(PrintLocation))
                            Directory.CreateDirectory(PrintLocation);

                        rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, saveLocation);
                        Application.DoEvents();

                        rptDoc.PrintOptions.PrinterName = VPrinterName;
                        rptDoc.PrintToPrinter(1, false, 1, 1);

                        Thread.Sleep(2000);
                    }
                    MessageBox.Show("You have successfully print " + PrintCopy + " Copy(s)");


                        #endregion Save & Print in PDF format
                }
            }
                #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 2)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_RunWorkerCompleted", exMessage);
            }

            #endregion

            btnPrev.Enabled = true;
            //this.Enabled = true;
            this.progressBar1.Visible = false;
        }
        // for retrive os system and others
        //public static string GetOSFriendlyName()
        //{
        //    string result = string.Empty;
        //    //ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        //    foreach (ManagementObject os in searcher.Get())
        //    {
        //        result = os["Caption"].ToString();
        //        //result = os["OSArchitecture"].ToString();

        //        //result = os["CSDVersion"].ToString();
        //        break;
        //    }
        //    return result;
        //}

        //============22/01/13 =============

        private void ReportVAT11Ds()
        {
            try
            {
                #region Statement

                //string post1, post2 = string.Empty;

                if (PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                CommonDAL commonDal = new CommonDAL();

                varSalesInvoiceNo = txtSalesInvoiceNo.Text.Trim();
                varTrading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                #region VAT 11 Page Setup and Item Nature
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature");
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter");
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4");
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal");


                if (string.IsNullOrEmpty(vVAT11Letter)
                    || string.IsNullOrEmpty(vItemNature)
                    || string.IsNullOrEmpty(vVAT11Legal)
                    || string.IsNullOrEmpty(vVAT11A4))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                PrepaidVAT = Convert.ToBoolean(vPrepaidVAT == "Y" ? true : false);
                VAT11Letter = Convert.ToBoolean(vVAT11Letter == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(vVAT11A4 == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(vVAT11Legal == "Y" ? true : false);

                ItemNature = vItemNature;

                if (VAT11A4 == true)
                {
                    VAT11PageSize = "A4";
                }
                else if (VAT11Letter == true)
                {
                    VAT11PageSize = "Letter";
                }
                else if (VAT11Legal == true)
                {
                    VAT11PageSize = "Legal";
                }
                else
                {
                    VAT11PageSize = "A4";
                }
                #endregion VAT 11 Page Setup and Item Nature
                //this.Enabled = false;

                backgroundWorkerReportVAT11Ds.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
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
                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT11Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportVAT11Ds", exMessage);
            }

            #endregion

        }

        private void ReportVAT20Ds()
        {

            try
            {
                #region Statement

                //this.Enabled = false;

                backgroundWorkerReportVAT20Ds.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportVAT20Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportVAT20Ds", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportVAT20Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportVAT20Ds", exMessage);
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
                FileLogger.Log(this.Name, "ReportVAT20Ds", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT20Ds", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportVAT20Ds", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportVAT20Ds", exMessage);
            }

            #endregion

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //ReportList();
        }

        private void cmbProduct_CursorChanged(object sender, EventArgs e)
        {

        }

        private void cmbCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            //ChangeData = true; txtProductName.Text = "";
            ////cmbProduct.Text = "";
            //txtCategoryName.Text = "";
            //txtUOM.Text = "";
            //txtUnitCost.Text = "0.00";
            //txtNBRPrice.Text = "0.00";
            //txtHSCode.Text = "";
            //txtVATRate.Text = "0.00";
            //txtQuantityInHand.Text = "0.00";

            //for (int j = 0; j < Convert.ToInt32(ProductResultDs.Tables[0].Rows.Count); j++) //for (int j = 0; j < Convert.ToInt32(ProductLines.Length); j++)
            //{
            //    //string[] ProductFields = ProductLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    try
            //    {
            //        if (ProductResultDs.Tables[0].Rows[j]["ItemNo"].ToString() == txtProductCode.Text.Trim())
            //        {


            //            txtProductName.Text= ProductResultDs.Tables[0].Rows[j]["ProductName"].ToString(); // ProductFields[1].ToString();
            //            cmbProduct.Text =ProductResultDs.Tables[0].Rows[j]["ProductName"].ToString(); // ProductFields[1].ToString();
            //            txtProductCode.Text = ProductResultDs.Tables[0].Rows[j]["ItemNo"].ToString(); // ProductFields[0].ToString();
            //            txtNBRPrice.Text = ProductResultDs.Tables[0].Rows[j]["NBRPrice"].ToString(); // Convert.ToDecimal(ProductFields[8].ToString()).ToString();//"0.00");
            //            txtCategoryName.Text = ProductResultDs.Tables[0].Rows[j]["CategoryName"].ToString(); // ProductFields[4].ToString();
            //            txtUOM.Text = ProductResultDs.Tables[0].Rows[j]["UOM"].ToString(); // ProductFields[5].ToString();
            //            txtHSCode.Text = ProductResultDs.Tables[0].Rows[j]["HSCodeNo"].ToString(); // ProductFields[11].ToString();
            //            txtVATRate.Text = ProductResultDs.Tables[0].Rows[j]["VATRate"].ToString(); // Convert.ToDecimal(ProductFields[12]).ToString();//"0.00");
            //            txttradingMarkup.Text = ProductResultDs.Tables[0].Rows[j]["TradingMarkUp"].ToString(); // ProductFields[21].ToString();
            //            txtSD.Text = ProductResultDs.Tables[0].Rows[j]["SD"].ToString(); // Convert.ToDecimal(ProductFields[18]).ToString();//"0.00");
            //            txtQuantityInHand.Text = ProductResultDs.Tables[0].Rows[j]["Stock"].ToString(); // ProductFields[17].ToString();

            //            if (ProductResultDs.Tables[0].Rows[j]["NonStock"].ToString() == "Y")//22
            //            {
            //                chkNonStock.Checked = true;
            //            }
            //            else
            //            {
            //                chkNonStock.Checked = false;
            //            }
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699) { MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
        }

        //no serverside method
        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormCustomerSearch frm = new FormCustomerSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormCustomerSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();//[0];
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();//[1];
                    cmbCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();//[1];
                    txtCustomerGroupName.Text = selectedRow.Cells["CustomerGroupID"].Value.ToString();//[2];
                    txtCustomerGroupName.Text = selectedRow.Cells["CustomerGroupName"].Value.ToString();//[3];
                    txtDeliveryAddress1.Text = selectedRow.Cells["Address1"].Value.ToString();//[4];
                    txtDeliveryAddress2.Text = selectedRow.Cells["Address2"].Value.ToString();//[5];
                    txtDeliveryAddress3.Text = selectedRow.Cells["Address3"].Value.ToString();//[6];

                }


                #region Old

                //Program.fromOpen = "Other";
                //string result = FormCustomerSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] CustomerInfo = result.Split(FieldDelimeter.ToCharArray());

                //    txtCustomerID.Text = CustomerInfo[0];
                //    txtCustomerName.Text = CustomerInfo[1];
                //    cmbCustomerName.Text = CustomerInfo[1];
                //    txtCustomerGroupName.Text = CustomerInfo[2];
                //    txtCustomerGroupName.Text = CustomerInfo[3];
                //    txtDeliveryAddress1.Text = CustomerInfo[4];
                //    txtDeliveryAddress2.Text = CustomerInfo[5];
                //    txtDeliveryAddress3.Text = CustomerInfo[6];


                //}

                #endregion

                //SearchCustomer();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchCustomer_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchCustomer_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", exMessage);
            }

            #endregion

        }

        //no serverside method
        private void btnSearchVehicle_Click(object sender, EventArgs e)
        {

            //try
            //{
            //    #region Statement


            //    ////MDIMainInterface mdi = new MDIMainInterface();
            //    //FormVehicleSearch frm = new FormVehicleSearch();
            //    //mdi.RollDetailsInfo(frm.VFIN);
            //    //if (Program.Access != "Y")
            //    //{
            //    //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    //    return;
            //    //}

            //    Program.fromOpen = "Other";
            //    string result = FormVehicleSearch.SelectOne();
            //    if (result == "")
            //    {
            //        return;
            //    }
            //    else //if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] VehicleInfo = result.Split(FieldDelimeter.ToCharArray());

            //        txtVehicleID.Text = VehicleInfo[0];
            //        txtVehicleType.Text = VehicleInfo[1];
            //        txtVehicleNo.Text = VehicleInfo[2];
            //        cmbVehicleNo.Text = VehicleInfo[2];
            //    }
            //    //SearchVehicle();

            //    #endregion

            //}
            //#region catch
            //catch (ArgumentNullException ex)
            //{
            //    string err = ex.ToString();
            //    string[] error = err.Split(FieldDelimeter.ToCharArray());
            //    FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", exMessage);
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
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
            //    FileLogger.Log(this.Name, "btnSearchVehicle_Click", exMessage);
            //}

            //#endregion

        }

        //no serverside method
        private void btnProductType_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement


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
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];

                ProductSearchDsFormLoad();


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
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

        private void cmbVehicleNo_TextChanged(object sender, EventArgs e)
        {
            //VehicleDetailsInfo();
        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void txtCommentsDetail_TextChanged(object sender, EventArgs e)
        {
            //ChangeData = true;
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                #region Statement

                if (e.KeyCode.Equals(Keys.Enter))
                {
                    btnAdd.Focus();
                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "txtCommentsDetail_KeyDown", exMessage);
            }

            #endregion

        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCustomerGroupName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDeliveryAddress1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVehicleType_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDeliveryAddress2_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDeliveryAddress3_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkPreviousAddress_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtOldID_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {
            if (txtCategoryName.Text == "Export" || txtCategoryName.Text == "Service")
            {
                txtNBRPrice.ReadOnly = false;
            }
            else
            {
                txtNBRPrice.ReadOnly = true;
            }
            ChangeData = true;
        }

        private void txtHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUOM_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txttradingMarkup_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkTrading_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtNBRPrice_TextChanged(object sender, EventArgs e)
        {
            
            ChangeData = true;
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSD_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantityInHand_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalVATAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormSale_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                #region Statement

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

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormSale_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormSale_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormSale_FormClosing", exMessage);
            }

            #endregion


        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtSD, "SD Rate");
        }

        private void cmbCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
                //cmbVehicleNo.Focus();
            }
        }

        private void dtpInvoiceDate_KeyDown(object sender, KeyEventArgs e)
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
                //dtpInvoiceDate.Focus();
            }
        }

        private void cmbVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }

        }

        private void txtDeliveryAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtDeliveryAddress2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtDeliveryAddress3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

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
                        if (rbtnTender.Checked != true)
                        {
                            if (CategoryId == "")
                            {
                                //ProductSearchDsFormLoad();
                                //ProductSearchDsLoad();
                            }
                            else
                            {
                                //ProductSearchDs();
                                //ProductSearchDsLoad();
                            }

                            //SearchCustomer();
                        }

                        if (chkSameVehicle.Checked == false)
                        {
                            //SearchVehicle();
                            cmbVehicleNo.Text = "Select";
                        }
                        if (rbtnExport.Checked)
                        {
                            cmbCurrency.SelectedIndex = 0;
                        }
                        ////SearchVehicle();
                        ClearAllFields();
                        //btnSave.Text = "&Add";
                        txtSalesInvoiceNo.Text = "~~~ New ~~~";
                        SetupVATStatus();
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    if (CategoryId == null)
                    {
                        //ProductSearchDsFormLoad();
                        //ProductSearchDsLoad();
                    }
                    else
                    {
                        //ProductSearchDs();
                        //ProductSearchDsLoad();
                    }
                    //ProductSearchDs();
                    //ProductSearchDsLoad();
                    //SearchCustomer();
                    if (chkSameVehicle.Checked == false)
                    {
                        //SearchVehicle();
                        //cmbVehicleNo.Text = "Select";
                    }
                    ClearAllFields();
                    //btnSave.Text = "&Add";
                    txtSalesInvoiceNo.Text = "~~~ New ~~~";
                    SetupVATStatus();
                    ChangeData = false;
                    chkPrint.Checked = false;
                }

                IsUpdate = false;



                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
            finally
            {
                ChangeData = false;

            }
        }

        private void btnDebitCredit_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                this.btnDebitCredit.Enabled = false;
                this.progressBar1.Visible = true;

                #region Statement

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                this.btnDebitCredit.Enabled = false;
                this.progressBar1.Visible = true;

                ////start DoWork
                //string encriptedSalesInvoiceNo = Converter.DESEncrypt(PassPhrase, EnKey, txtSalesInvoiceNo.Text.Trim());
                //SaleSoapClient PrintUpdate = new SaleSoapClient();
                //string result = PrintUpdate.UpdatePrint(encriptedSalesInvoiceNo.ToString(), Program.DatabaseName);// Change 04
                ////end DoWork

                ////start Complete
                //if (Convert.ToDecimal(result) < 0)
                //{
                //    MessageBox.Show("Can't print please contact with administrator");
                //}
                //else
                //{

                //    if (rbtnCN.Checked)
                //    {
                //        try
                //        {
                //            ReportCreditNoteDs();
                //            Print = false;
                //            chkPrint.Checked = true;
                //        }
                //        catch (Exception ex)
                //        {
                //            if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
                //            {
                //                MessageBox.Show(
                //                    "Communication not performed" + "\n\n" + "Please contact with Administrator.",
                //                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //                return;
                //            }
                //            MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
                //                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                //            return;
                //        }

                //    }
                //    if (rbtnDN.Checked)
                //    {
                //        try
                //        {
                //            ReportDebitNoteDs();
                //            Print = false;
                //            chkPrint.Checked = true;
                //        }
                //        catch (Exception ex)
                //        {
                //            if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
                //            {
                //                MessageBox.Show(
                //                    "Communication not performed" + "\n\n" + "Please contact with Administrator.",
                //                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //                return;
                //            }
                //            MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
                //                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                //            return;
                //        }

                //    }

                //}
                ////end Complete

                //backgroundWorkerDebitCredit.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnDebitCredit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDebitCredit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnDebitCredit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnDebitCredit_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnDebitCredit_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDebitCredit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDebitCredit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnDebitCredit_Click", exMessage);
            }

            #endregion

        }

        private void ReportCreditNoteDs()
        {

            try
            {
                #region Statement

                //string ReportData = txtSalesInvoiceNo.Text.Trim() + FieldDelimeter + LineDelimeter;

                ////start DoWork
                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //ReportDSSoapClient ShowReport = new ReportDSSoapClient();
                //DataSet ReportResultCreditNote = ShowReport.CreditNote(encriptedData.ToString(), Program.DatabaseName);
                ////end DoWork

                ////start Complete
                //ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";
                //RptCreditNote objrpt = new RptCreditNote();
                //objrpt.SetDataSource(ReportResultCreditNote);
                ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                ////end Complete
                //this.Enabled = false;

                backgroundWorkerReportCreditNoteDs.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportCreditNoteDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportCreditNoteDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportCreditNoteDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportCreditNoteDs", exMessage);
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
                FileLogger.Log(this.Name, "ReportCreditNoteDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportCreditNoteDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportCreditNoteDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportCreditNoteDs", exMessage);
            }

            #endregion

        }

      

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement



                Program.fromOpen = "Me";
                Program.SalesType = cmbType.Text.Trim();
                if (cmbProductType.Text == "Trading")
                {
                    Program.Trading = "Y";
                }
                if (cmbProductType.Text != "Trading")
                {
                    Program.Trading = "N";
                }

                #region Transaction Type
                DataGridViewRow selectedRow = null;
                TransactionTypes();
                #endregion Transaction Type
                selectedRow = FormSaleSearch.SelectOne(transactionType);
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtSalesInvoiceNo.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    cmbCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    txtCustomerGroupName.Text = selectedRow.Cells["CustomerGroupName"].Value.ToString();
                    txtDeliveryAddress1.Text = selectedRow.Cells["DeliveryAddress1"].Value.ToString();
                    txtDeliveryAddress2.Text = selectedRow.Cells["DeliveryAddress2"].Value.ToString();
                    txtDeliveryAddress3.Text = selectedRow.Cells["DeliveryAddress3"].Value.ToString();
                    txtVehicleID.Text = selectedRow.Cells["VehicleID"].Value.ToString();
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    cmbVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtTotalVATAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtSerialNo.Text = selectedRow.Cells["SerialNo"].Value.ToString();
                    dtpInvoiceDate.Value =Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                    dtpDeliveryDate.Value = Convert.ToDateTime(selectedRow.Cells["DeliveryDate"].Value.ToString());
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    txtOldID.Text = selectedRow.Cells["PID"].Value.ToString();
                    if (string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Credit" ||
                        string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Debit")
                    {
                        if (CreditWithoutTransaction==true)
                        {
                            txtOldID.ReadOnly = false;
                        }
                    }
                    

                    cmbCurrency.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtCurrencyId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();
                    txtBDTRate.Text = selectedRow.Cells["CurrencyRateFromBDT"].Value.ToString();
                    txtLCNumber.Text = selectedRow.Cells["LCNumber"].Value.ToString();
                    ImportExcelID = selectedRow.Cells["ImportID"].Value.ToString();
                    if (DatabaseInfoVM.DatabaseName == "CPB_DB")
                    {
                        txtImportID.Text = selectedRow.Cells["ImportID"].Value.ToString();
                    }
                    else
                    {
                        txtImportID.Text = "";
                    }
                    
                    if (string.IsNullOrEmpty(selectedRow.Cells["SaleReturnId"].Value.ToString()))
                    {
                        txtDollerRate.Text = "0";
                    }
                    else
                    {
                        txtDollerRate.Text = selectedRow.Cells["SaleReturnId"].Value.ToString();
                    }
                    if (selectedRow.Cells["Trading"].Value.ToString() == "Y")
                    {
                        chkTrading.Checked = true;
                    }
                    else
                    {
                        chkTrading.Checked = false;
                    }
                    if (selectedRow.Cells["Isprint"].Value.ToString() == "Y")
                    {
                        chkPrint.Checked = true;
                    }
                    else
                    {
                        chkPrint.Checked = false;
                    }
                    TenderId = selectedRow.Cells["TenderId"].Value.ToString();
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;


                    if (txtSalesInvoiceNo.Text == "")
                    {
                        SaleDetailData = "0";
                    }
                    else
                    {
                        SaleDetailData = txtSalesInvoiceNo.Text.Trim();
                    }
                    AlReadyPrintNo = Convert.ToInt32(selectedRow.Cells["AlReadyPrint"].Value.ToString());

                    #region Delivery & GatePass
                    //if (selectedRow.Cells["DeliveryChallan"].Value.ToString() == "N")
                    //{
                    //    btnDL.Enabled = true;
                    //}
                    //else
                    //{
                    //    btnDL.Enabled = false;
                    //}
                    //if (selectedRow.Cells["IsGatePass"].Value.ToString() == "N")
                    //{
                    //    btnGPR.Enabled = true;
                    //}
                    //else
                    //{
                    //    btnGPR.Enabled = false;
                    //}
                    #endregion Delivery & GatePass
                    InsertTrackingInfo();
                    this.btnSearchInvoiceNo.Enabled = false;
                    this.progressBar1.Visible = true;
                    //this.Enabled = false;

                    backgroundWorkerChallanSearch.RunWorkerAsync();
                }
                ChangeData = false;



                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
        private void backgroundWorkerChallanSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            try
            {
                SaleDAL saleDal = new SaleDAL();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData);
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerChallanSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            try
            {

                dgvSale.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SaleDetailResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);

                    dgvSale.Rows[j].Cells["LineNo"].Value = item["InvoiceLineNo"].ToString();
                    // SaleDetailFields[1].ToString();
                    dgvSale.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    // SaleDetailFields[2].ToString();
                    dgvSale.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                    dgvSale.Rows[j].Cells["SaleQuantity"].Value = item["SaleQuantity"].ToString();
                    dgvSale.Rows[j].Cells["PromotionalQuantity"].Value = item["PromotionalQuantity"].ToString();

                    //Convert.ToDecimal(SaleDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UnitPrice"].Value = item["SalesPrice"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["UOM"].Value = item["UOM"].ToString(); // SaleDetailFields[6].ToString();
                    dgvSale.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[7].ToString()).ToString();//"0.00");
                    dgvSale.Rows[j].Cells["VATAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["SubTotal"].Value = "0";
                    dgvSale.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    // SaleDetailFields[10].ToString();
                    dgvSale.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    // SaleDetailFields[11].ToString();
                    dgvSale.Rows[j].Cells["Status"].Value = "Old";
                    dgvSale.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvSale.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["SD"].Value = item["SD"].ToString();
                    // Convert.ToDecimal(SaleDetailFields[13].ToString()).ToString();//"0.00");
                    dgvSale.Rows[j].Cells["SDAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["Change"].Value = 0;
                    dgvSale.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    // SaleDetailFields[17].ToString();
                    dgvSale.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();
                    // SaleDetailFields[18].ToString();
                    dgvSale.Rows[j].Cells["TradingMarkUp"].Value = item["tradingMarkup"].ToString();
                    // SaleDetailFields[19].ToString();
                    dgvSale.Rows[j].Cells["Type"].Value = item["Type"].ToString(); // SaleDetailFields[20].ToString();
                    dgvSale.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    // SaleDetailFields[21].ToString();
                    dgvSale.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                    // Convert.ToDecimal(SaleDetailFields[22].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString(); // SaleDetailFields[23].ToString();
                    dgvSale.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[24].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();
                    // Convert.ToDecimal(SaleDetailFields[25].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["DiscountAmount"].Value = item["DiscountAmount"].ToString();
                    dgvSale.Rows[j].Cells["DiscountedNBRPrice"].Value = item["DiscountedNBRPrice"].ToString();
                    dgvSale.Rows[j].Cells["DollerValue"].Value = item["DollerValue"].ToString();
                    dgvSale.Rows[j].Cells["BDTValue"].Value = item["CurrencyValue"].ToString();
                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvSale.Rows[j].Cells["VATName"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvSale.Rows[j].Cells["VATName"].Value = item["VATName"].ToString();

                    }
                    dgvSale.Rows[j].Cells["CConvDate"].Value = item["CConversionDate"].ToString();
                    dgvSale.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();

                    #region currency convertion date change for export
                    if (rbtnExport.Checked || ChkExpLoc.Checked)
                    {
                        dgvSale.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
                        dgvSale.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();
                        dgvSale.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();

                        decimal GTotal = Convert.ToDecimal(item["SubTotal"].ToString()) + Convert.ToDecimal(item["SDAmount"].ToString()) + Convert.ToDecimal(item["tradingMarkup"].ToString()) + Convert.ToDecimal(item["VATAmount"].ToString());

                        if (Program.CheckingNumericString(GTotal.ToString(), "Total") == true)
                            GTotal = Convert.ToDecimal(Program.FormatingNumeric(GTotal.ToString(), SalePlaceTaka));

                        dgvSale.Rows[j].Cells["Total"].Value = GTotal.ToString();
                    }

                    dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = item["ReturnTransactionType"].ToString();
                    #endregion currency convertion date change for export
                    j = j + 1;
                } // End For
                //btnSave.Text = "&Save";
                if (rbtnExport.Checked || ChkExpLoc.Checked)
                {
                    GTotal();
                }
                else
                {
                    Rowcalculate();
                }
                
                
                ExportSearch();
                SetupVATStatus();
                IsUpdate = true;
                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

               
                // End Complete
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }

        private void btnOldID_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                dgvReceiveHistory.Top = dgvSale.Top;
                dgvReceiveHistory.Left = dgvSale.Left;
                dgvReceiveHistory.Height = dgvSale.Height;
                dgvReceiveHistory.Width = dgvSale.Width;

                dgvSale.Visible = false;
                dgvReceiveHistory.Visible = true;


                Program.fromOpen = "Me";
                Program.SalesType = "New";
                string queryParam = "";
                //DataGridViewRow selectedRow = FormSaleSearch.SelectOne("'Other'");
                if (rbtnDN.Checked || rbtnCN.Checked)
                {
                    queryParam = "All";
                }
                DataGridViewRow selectedRow = FormSaleSearch.SelectOne(queryParam);


                if (selectedRow != null && selectedRow.Selected == true)
                {
                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }
                    else if (selectedRow.Cells["Isprint"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not printed", this.Text);
                        return;
                    }
                    //txtSalesInvoiceNo.Text = Saleinfo[0];
                    txtOldID.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    cmbCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    txtCustomerGroupName.Text = selectedRow.Cells["CustomerGroupName"].Value.ToString();
                    txtDeliveryAddress1.Text = selectedRow.Cells["DeliveryAddress1"].Value.ToString();
                    txtDeliveryAddress2.Text = selectedRow.Cells["DeliveryAddress2"].Value.ToString();
                    txtDeliveryAddress3.Text = selectedRow.Cells["DeliveryAddress3"].Value.ToString();
                    txtVehicleID.Text = selectedRow.Cells["VehicleID"].Value.ToString();
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    cmbVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtTotalVATAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtSerialNo.Text = selectedRow.Cells["SerialNo"].Value.ToString();
                    dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    cmbCurrency.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtCurrencyId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();
                    txtBDTRate.Text = selectedRow.Cells["CurrencyRateFromBDT"].Value.ToString();
                    txtDollerRate.Text = selectedRow.Cells["SaleReturnId"].Value.ToString();
                    transactionTypeOld = selectedRow.Cells["transactionType"].Value.ToString();
                    txtLCNumber.Text = selectedRow.Cells["LCNumber"].Value.ToString();

                    if (selectedRow.Cells["Trading"].Value == "Y")
                    {
                        chkTrading.Checked = true;
                    }
                    else
                    {
                        chkTrading.Checked = false;
                    }
                    if (rbtnDN.Checked || rbtnCN.Checked)
                    {
                        ReturnTransType = transactionTypeOld;
                    }

                    TenderId = selectedRow.Cells["TenderId"].Value.ToString();

                    SaleDetailData = string.Empty;
                    if (txtOldID.Text == "")
                    {
                        SaleDetailData = "0";
                    }
                    else
                    {
                        SaleDetailData = txtOldID.Text.Trim();
                    }

                    if (transactionTypeOld == "Service")
                    {
                        txtQty1.Enabled = true;
                        txtQty2.Enabled = true;
                        txtNBRPrice.Enabled = true;

                        txtQty1.ReadOnly = false;
                        txtQty2.ReadOnly = false;
                       
                        txtQuantityInHand.Visible = true;
                        LINhand.Visible = true;
                    }
                    if (transactionTypeOld == "ServiceNS")
                    {
                        txtQty1.Enabled = false;
                        txtQty2.Enabled = false;
                        txtNBRPrice.Enabled = true;

                        txtQty1.ReadOnly = true;
                        txtQty2.ReadOnly = true;
                        
                        txtQuantityInHand.Visible = false;
                        LINhand.Visible = false;

                    }


                    this.btnOldID.Enabled = false;
                    this.progressBar1.Visible = true;
                    //this.Enabled = false;

                    backgroundWorkerBtnOldIdSearch.RunWorkerAsync();


                }






                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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
        private void backgroundWorkerBtnOldIdSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SaleDAL saleDal = new SaleDAL();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData);


                #endregion
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerBtnOldIdSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {


            try
            {
                #region Statement

                SaleDAL saleDal = new SaleDAL();

                #region Sale product load

                dgvReceiveHistory.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SaleDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceiveHistory.Rows.Add(NewRow);
                    dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = item["InvoiceLineNo"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNoP"].Value = item["ItemNo"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["QuantityP"].Value = item["Quantity"].ToString();

                    dgvReceiveHistory.Rows[j].Cells["SaleQuantityP"].Value = item["SaleQuantity"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PromotionalQuantityP"].Value =
                        item["PromotionalQuantity"].ToString();


                    dgvReceiveHistory.Rows[j].Cells["UnitPriceP"].Value = item["SalesPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["NBRPriceP"].Value = item["NBRPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMP"].Value = item["UOM"].ToString();
                        // SaleDetailFields[6].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATRateP"].Value = item["VATRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = "0";
                    dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = "0";
                    dgvReceiveHistory.Rows[j].Cells["CommentsP"].Value = item["Comments"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNameP"].Value = item["ProductName"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StatusP"].Value = "Old";
                    //dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value = item["Quantity"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StockP"].Value = item["Stock"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SDP"].Value = item["SD"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = "0";
                    dgvReceiveHistory.Rows[j].Cells["ChangeP"].Value = 0;
                    dgvReceiveHistory.Rows[j].Cells["TradingP"].Value = item["Trading"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["NonStockP"].Value = item["NonStock"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TradingMarkUpP"].Value = item["tradingMarkup"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TypeP"].Value = item["Type"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PCodeP"].Value = item["ProductCode"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMQtyP"].Value = item["UOMQty"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMnP"].Value = item["UOMn"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMcP"].Value = item["UOMc"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMPriceP"].Value = item["UOMPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DiscountAmountP"].Value = item["DiscountAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DiscountedNBRPriceP"].Value = item["DiscountedNBRPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DollerValueP"].Value = item["DollerValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["BDTValueP"].Value = item["CurrencyValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["WeightP"].Value = item["Weight"].ToString();
                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = item["VATName"].ToString();

                    }

                    //dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = item["VATName"].ToString();
                    decimal returnQty = saleDal.ReturnSaleQty(SaleDetailData, item["ItemNo"].ToString());
                    dgvReceiveHistory.Rows[j].Cells["RestQtyR"].Value =
                        (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value =
                        (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvReceiveHistory.Rows[j].Cells["CConvDateP"].Value = item["CConversionDate"].ToString();

                    if (rbtnExport.Checked || ChkExpLoc.Checked)
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = item["VATAmount"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = item["SubTotal"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = item["SDAmount"].ToString();
                    }


                    j++;


                }

                #endregion Sale product load


                if (rbtnExport.Checked)

                    SetupVATStatus();
                IsUpdate = false;
                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                ChangeData = false;



                #endregion
            }
                #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.btnOldID.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }


        private void ExportSearch()
        {

            try
            {
                #region Statement

                ExportData = string.Empty;
                if (txtSalesInvoiceNo.Text == "")
                {
                    ExportData = "0";
                }
                else
                {
                    ExportData = txtSalesInvoiceNo.Text.Trim();
                }


                #endregion
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                bgwExportSearch.RunWorkerAsync();
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ExportSearch", exMessage);
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
                FileLogger.Log(this.Name, "ExportSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ExportSearch", exMessage);
            }

            #endregion


        }
        private void bgwExportSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork

                // donot use new open connection

                //ExportResult = SaleDAL.SearchSaleExportDT(ExportData, Program.DatabaseName); // Change 04
                SaleDAL saleDal = new SaleDAL();
                ExportResult = saleDal.SearchSaleExportDTNew(ExportData, Program.DatabaseName); // Change 04

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwExportSearch_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwExportSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                dgvExport.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ExportResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvExport.Rows.Add(NewRow);

                    dgvExport.Rows[j].Cells["LineNoE"].Value = item["SaleLineNo"].ToString();// IssueDetailFields[0].ToString();
                    dgvExport.Rows[j].Cells["Description"].Value = item["Description"].ToString();// IssueDetailFields[1].ToString();
                    dgvExport.Rows[j].Cells["QuantityE"].Value = item["Quantity"].ToString();//"0,0.0000");// Convert.ToDecimal(IssueDetailFields[2].ToString()).ToString();//"0.00");
                    dgvExport.Rows[j].Cells["GrossWeight"].Value = item["GrossWeight"].ToString();//"0,0.0000");// Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0.00");
                    dgvExport.Rows[j].Cells["NetWeight"].Value = item["NetWeight"].ToString();//"0,0.0000");// Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0.00");
                    dgvExport.Rows[j].Cells["NumberFrom"].Value = item["NumberFrom"].ToString();//  item["cahnge"].ToString();// IssueDetailFields[5].ToString();
                    dgvExport.Rows[j].Cells["NumberTo"].Value = item["NumberTo"].ToString();//  item["cahnge"].ToString();// IssueDetailFields[6].ToString();
                    dgvExport.Rows[j].Cells["CommentsE"].Value = item["Comments"].ToString();//  IssueDetailFields[7].ToString();

                    j = j + 1;
                }

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwExportSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                //this.Enabled = true;
                this.progressBar1.Visible = false;

            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (chkPrint.Checked == true)
                {
                    btnPrint.Enabled = false;
                    btnSave.Enabled = false;
                    btnDebitCredit.Enabled = false;
                    btnAdd.Enabled = false;
                    btnChange.Enabled = false;
                    btnRemove.Enabled = false;
                    btnPost.Enabled = false;
                    bthUpdate.Enabled = false;
                }
                if (chkPrint.Checked == false)
                {
                    btnPrint.Enabled = true;
                    btnSave.Enabled = true;
                    btnDebitCredit.Enabled = true;
                    btnAdd.Enabled = true;
                    btnChange.Enabled = true;
                    btnRemove.Enabled = true;
                    btnPost.Enabled = true;
                    bthUpdate.Enabled = true;



                }


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkPrint_CheckedChanged", exMessage);
            }

            #endregion


        }

        private void btnVAT18_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT18 frmRptVAT18 = new FormRptVAT18();

                //mdi.RollDetailsInfo("8401");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                frmRptVAT18.dtpToDate.Value = dtpInvoiceDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpInvoiceDate.Value;
                frmRptVAT18.ShowDialog();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void btnPSale_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                FormRptSalesInformation frmRptSalesInformation = new FormRptSalesInformation();

                TransactionTypes();
                frmRptSalesInformation.txtTransactionType.Text = transactionType;

                if (txtSalesInvoiceNo.Text == "~~~ New ~~~")
                    frmRptSalesInformation.txtSalesInvoiceNo.Text = "";
                else
                    frmRptSalesInformation.txtSalesInvoiceNo.Text = txtSalesInvoiceNo.Text.Trim();


                frmRptSalesInformation.ShowDialog();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPSale_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPSale_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPSale_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPSale_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPSale_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPSale_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPSale_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPSale_Click", exMessage);
            }

            #endregion


        }

        private void btnVAT17_Click(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT17 frmRptVAT17 = new FormRptVAT17();

                //mdi.RollDetailsInfo("8301");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvSale.Rows.Count > 0)
                {
                    frmRptVAT17.txtItemNo.Text = dgvSale.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT17.txtProductName.Text = dgvSale.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT17.dtpFromDate.Value = dtpInvoiceDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpInvoiceDate.Value;
                    if (rbtnService.Checked || rbtnServiceNS.Checked)
                    {
                        frmRptVAT17.rbtnService.Checked = true;
                    }
                    if (rbtnInternalIssue.Checked || rbtnTrading.Checked)
                    {
                        frmRptVAT17.rbtnWIP.Checked = true;
                    }
                    if (transactionType == "Wastage")
                    {
                        frmRptVAT17.rbtnWIP.Checked = true;
                    }
                }



                frmRptVAT17.Show();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
            }

            #endregion


        }

        private void txtVehicleNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerID_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkPCode_CheckedChanged(object sender, EventArgs e)
        {

            //ProductSearchDsLoad();

            cmbProduct.Focus();
        }

        private void txtSD_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter)) { txtCommentsDetail.Focus(); }
        }

        private void rbtnService_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void btn11Ga_Click(object sender, EventArgs e)
        {

        }

        private void dgvExport_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                #region Statement


                numericCheckinGrid("QuantityE", "Quantity");
                numericCheckinGrid("GrossWeight", "Gross Weight");
                numericCheckinGrid("NetWeight", "New Weight");
                numericCheckinGrid("NumberFrom", "NumberFrom");
                numericCheckinGrid("NumberTo", "NumberTo");
                //dgvExport
                for (int i = 0; i < dgvExport.RowCount; i++)
                {
                    dgvExport[0, i].Value = i + 1;
                }
                //NulllExport();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", exMessage);
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
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvExport_CellEndEdit", exMessage);
            }

            #endregion
        }

        private void NulllExport()
        {
            try
            {
                #region Statement


                for (int i = 0; i < dgvExport.RowCount; i++)
                {
                    dgvExport[0, i].Value = i + 1;
                    for (int k = 0; k < dgvExport.ColumnCount; i++)
                    {
                        if (dgvExport[i, k].Value == "" || dgvExport[i, k].Value == null)
                        {
                            dgvExport[i, k].Value = "-";
                        }
                    }
                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NulllExport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NulllExport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NulllExport", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "NulllExport", exMessage);
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
                FileLogger.Log(this.Name, "NulllExport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NulllExport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NulllExport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NulllExport", exMessage);
            }

            #endregion

        }

        private void numericCheckinGrid(string gridColumn, string message)
        {
            try
            {
                #region Statement

                for (int i = 0; i < dgvExport.RowCount - 1; i++)
                {

                    if (Program.FormatQty(dgvExport[gridColumn, i].Value.ToString()) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + message + " field");
                        dgvExport[gridColumn, i].Value = 0;
                        return;
                    }

                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "numericCheckinGrid", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "numericCheckinGrid", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "numericCheckinGrid", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "numericCheckinGrid", exMessage);
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
                FileLogger.Log(this.Name, "numericCheckinGrid", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "numericCheckinGrid", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "numericCheckinGrid", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "numericCheckinGrid", exMessage);
            }

            #endregion
        }

        private void btnEAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnERemove_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                //dgvExport.Rows.Remove( dgvExport.CurrentRow.Index);
                //dgvExport.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                int tp = dgvExport.CurrentRow.Index;

                MessageBox.Show(dgvExport.CurrentRow.Index.ToString());
                dgvExport.Rows.Remove(dgvExport.Rows[dgvExport.CurrentRow.Index]);

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnERemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnERemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnERemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnERemove_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnERemove_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnERemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnERemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnERemove_Click", exMessage);
            }

            #endregion

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportPackagingInfoAdd();
        }

        private void ExportPackagingInfoAdd()
        {
            try
            {
                #region Statement

                if (gbExport.Visible == false)
                {
                    gbExport.Visible = true;
                    GF.Visible = false;
                }
                else if (gbExport.Visible == true)
                {
                    gbExport.Visible = false;
                    GF.Visible = true;
                }

                gbExport.Top = GF.Top;
                gbExport.Left = GF.Left;

                gbExport.Height = GF.Height;
                gbExport.Width = GF.Width;

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnExport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnExport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnExport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnExport_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnExport_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnExport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnExport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnExport_Click", exMessage);
            }

            #endregion
        }

        private void btnEAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                dgvExport.Rows.Add();
                //MessageBox.Show(dgvExport.CurrentRow.Index.ToString());
                //dgvExport.CurrentRow.Cells["QuantityE"].Value = 0;
                //dgvExport.CurrentRow.Cells["GrossWeight"].Value = 0;
                //dgvExport.CurrentRow.Cells["NetWeight"].Value = 0;
                dgvExport["QuantityE", dgvExport.Rows.Count - 1].Value = "0";
                dgvExport["GrossWeight", dgvExport.Rows.Count - 1].Value = "0";
                dgvExport["NetWeight", dgvExport.Rows.Count - 1].Value = "0";
                dgvExport["NumberFrom", dgvExport.Rows.Count - 1].Value = "0";
                dgvExport["NumberTo", dgvExport.Rows.Count - 1].Value = "0";

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnEAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnEAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnEAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnEAdd_Click_1", exMessage);
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
                FileLogger.Log(this.Name, "btnEAdd_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnEAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnEAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnEAdd_Click_1", exMessage);
            }

            #endregion

        }

        private void btnERemove_Click_1(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                if (dgvExport.Rows.Count <= 0)
                {
                    return;
                }
                dgvExport.Rows.Remove(dgvExport.Rows[dgvExport.CurrentRow.Index]);


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnERemove_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnERemove_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnERemove_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnERemove_Click_1", exMessage);
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
                FileLogger.Log(this.Name, "btnERemove_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnERemove_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnERemove_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnERemove_Click_1", exMessage);
            }

            #endregion
        }

        private void btn20_Click(object sender, EventArgs e)
        {

            try
            {
                this.btn20.Enabled = false;
                //this.progressBar1.Visible = true;

                #region Multiple Vat20 form Open
                TransactionTypes();
                //FormSaleVAT20Multiple.SelectOne(transactionType, txtSalesInvoiceNo.Text);
                #endregion Multiple Vat20 form Open


                this.progressBar1.Visible = false;
                this.btn20.Enabled = true;

                //#region Statement

                //ReportVAT20Ds();
                ////chkPrint.Checked = true;

                //#endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btn20_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btn20_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btn20_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btn20_Click", exMessage);
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
                FileLogger.Log(this.Name, "btn20_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btn20_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btn20_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btn20_Click", exMessage);
            }

            #endregion

        }

        private void btnTender_Click(object sender, EventArgs e)
        {
            //no serverside method
            try
            {
                #region Statement

                //string result;
                
                //Program.fromOpen = "Me";
                //result = FormTenderSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] Tenderinfo = result.Split(FieldDelimeter.ToCharArray());

                //    TenderId = Tenderinfo[0];
                //    txtSerialNo.Text = Tenderinfo[1];

                //    txtCustomerID.Text = Tenderinfo[2];
                //    txtCustomerName.Text = Tenderinfo[3];
                //    cmbCustomerName.Text = Tenderinfo[3];
                //    txtDeliveryAddress1.Text = Tenderinfo[4];
                //    txtDeliveryAddress2.Text = Tenderinfo[5];
                //    txtDeliveryAddress3.Text = Tenderinfo[6];
                //    tenderDate = Convert.ToDateTime(Tenderinfo[7]);
                //    txtCustomerGroupName.Text = Tenderinfo[10];
                //    cmbCustomerName.Enabled = false;
                //    btnSearchCustomer.Enabled = false;
                //    cmbVehicleNo.Focus();

                //}
                //SearchTenderDetails();
                //ProductSearchDsLoad();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }

            #endregion

        }

        private void SearchTenderDetails()
        {
            try
            {
                #region Statement

                this.progressBar1.Visible = true;
                this.btnTender.Enabled = false;
                //this.Enabled = false;

                bgwTenderSearch.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
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
                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
            }

            #endregion

        }
        private void bgwTenderSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                TenderDAL tenderDal = new TenderDAL();

                TenderSearchResult = new DataTable();

                //TenderSearchResult = tenderDal.SearchTenderDetailSale(TenderId, dtpInvoiceDate.Value.ToString("yyyy/MMM/dd"));
                TenderSearchResult = tenderDal.SearchTenderDetailSale(TenderId, dtpInvoiceDate.Value.ToString("yyyy/MMM/dd HH:mm:ss"));
                
                //TenderSearchResult = tenderDal.SearchTenderDetail(TenderId);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwTenderSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                ProductsMini.Clear();
                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                foreach (DataRow item2 in TenderSearchResult.Rows)
                {
                    var prod = new ProductMiniDTO();

                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    cmbProduct.Items.Add(item2["ProductName"]);
                    prod.ProductCode = item2["ProductCode"].ToString();
                    cmbProductCode.Items.Add(item2["ProductCode"]);

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
                    prod.TenderStock = Convert.ToDecimal(item2["TenderStock"].ToString());

                    ProductsMini.Add(prod);
                }
                ProductSearchDsLoad();

                // End Complete

                #endregion

               
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                //this.Enabled = true;
                this.progressBar1.Visible = false;
                this.btnTender.Enabled = true;
            }
        }

        private void cmbVehicleNo_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                
                // txtVehicleNo.Text = "";
                txtVehicleType.ReadOnly = true;

                var searchText = cmbVehicleNo.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var vehicleNo = from prd in vehicles.ToList()
                                    where prd.VehicleNo.ToLower() == searchText.ToLower()
                                    select prd;
                    if (vehicleNo != null && vehicleNo.Any())
                    {
                        var products = vehicleNo.First();
                        txtVehicleNo.Text = products.VehicleNo;
                        txtVehicleType.Text = products.VehicleType;
                        dtpInvoiceDate.Focus();
                    }
                    else
                    {
                        if (rbtnCN.Checked == false || rbtnDN.Checked == false)
                        {
                            if (txtSalesInvoiceNo.Text == "~~~ New ~~~")
                            {
                                chkVehicleSaveInDatabase.Checked = true;
                                if (
                                MessageBox.Show("vehicle Number not in database, Add new Vehicle?", this.Text, MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) != DialogResult.Yes)
                                {

                                    chkVehicleSaveInDatabase.Checked = false;

                                }
                                txtVehicleType.ReadOnly = false;
                            }
                        }
                    }
                }

                // Check Vehicle Type
                if (string.IsNullOrEmpty(txtVehicleType.Text))
                {
                    txtVehicleType.ReadOnly = false;
                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
            }

            #endregion


        }

        private void Uoms()
        {
            try
            {
                #region Statement

                string uOMFrom = txtUOM.Text.Trim().ToLower();
                //string uOMTo = cmbUom.Text.ToLower();

                cmbUom.Items.Clear();
                if (UOMs != null && UOMs.Any())
                {

                    var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom).ToList()
                               select uom.UOMTo;

                    if (uoms != null && uoms.Any())
                    {
                        cmbUom.Items.AddRange(uoms.ToArray());
                        cmbUom.Items.Add(txtUOM.Text.Trim());
                        txtUomConv.Text = uoms.First().ToString();
                    }
                }
                //cmbUom.Text = uOMTo;
                cmbUom.Text = txtUOM.Text.Trim();



                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void UomsValue()
        {
            try
            {
                #region Statement

                string uOMFrom = txtUOM.Text.Trim().ToLower();
                string uOMTo = cmbUom.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    txtUomConv.Text = "0";
                    if (uOMFrom == uOMTo)
                    {
                        txtUomConv.Text = "1";

                        return;
                    }

                    else if (UOMs != null && UOMs.Any())
                    {

                        var uoms =
                            from uom in
                                UOMs.Where(
                                    x => x.UOMFrom.Trim().ToLower() == uOMFrom && x.UOMTo.Trim().ToLower() == uOMTo).
                                ToList()
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

        private void cmbUom_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                //UomsValue();
                txtQuantity.Focus();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUom_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUom_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUom_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUom_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbUom_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUom_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUom_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUom_Leave", exMessage);
            }

            #endregion
        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;

                #region Statement

                PreviewOnly = true;

                Program.Trading = Convert.ToString(chkTrading.Checked ? true : false);
                ReportVAT11Ds();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }

            #endregion

        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                #region Statement

                if (e.KeyCode.Equals(Keys.Enter))
                {
                    SendKeys.Send("{TAB}");
                    //cmbUom.Focus();
                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUom_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUom_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUom_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUom_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "cmbUom_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUom_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUom_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUom_KeyDown", exMessage);
            }

            #endregion

        }

        private void dtpDeliveryDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbCustomerName_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                cmbVehicleNo.Focus();
                txtCustomerID.Text = "";

                var searchText = cmbCustomerName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    var custs = new List<CustomerSinmgleDTO>();
                    if (chkCustCode.Checked)
                    {
                        custs = customerMini.Where(x => x.CustomerCode.ToLower() == searchText).ToList();
                    }
                    else
                    {
                        custs = customerMini.Where(x => x.CustomerName.ToLower() == searchText).ToList();
                    }
                    if (custs.Any())
                    {
                        var customer = custs.First();
                        txtCustomerID.Text = customer.CustomerID;
                        txtCustomerName.Text = customer.CustomerName;
                        txtCustomerGroupName.Text = customer.CustomerGroupName;

                        txtDeliveryAddress1.Text = customer.Address1;
                        txtDeliveryAddress2.Text = customer.Address2;
                        txtDeliveryAddress3.Text = customer.Address3;

                    }

                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            #endregion


        }

        private void cmbProductCode_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                if (!string.IsNullOrEmpty(ProductCode))
                {
                    if (ProductCode == cmbProductCode.Text)
                    {
                        return;
                    }
                }
                txtProductCode.Text = "";
                var searchText = cmbProductCode.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select") //start
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
                        txtVATRate.Text = products.VATRate.ToString();//"0.00");
                        txtSD.Text = products.SD.ToString();//"0.00");

                        ProductDAL productDal = new ProductDAL();

                        NBRPriceCall();
                        txtUOM.Text = products.UOM;
                        cmbUom.Text = products.UOM;
                        txtHSCode.Text = products.HSCodeNo;

                        if (cmbVAT1Name.Text.Trim() == "VAT 4.3 (Wastage)")
                        {
                            txtQuantityInHand.Text = "0";
                        }
                        else
                        {
                            txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                                             DateTime.Now.ToString(" HH:mm:00"), null, null, false).Rows[0]["Quantity"].ToString();

                        }

                        
                        txtTenderStock.Text = products.TenderStock.ToString();//"0,0.0000");
                        txttradingMarkup.Text = products.TradingMarkUp.ToString();
                        txtPCode.Text = products.ProductCode;
                        chkNonStock.Checked = products.NonStock;
                        Uoms();

                        ProductCode = txtProductCode.Text;
                    }
                    else
                    {
                        MessageBox.Show("Please select the right item", this.Text);
                        cmbProductCode.Text = "Select";
                    }

                    txtWeight.Text = "";

                }
                //txtQty1.Focus();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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

        private void cmbProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (rbtnProduct.Checked)
                {
                    //cmbProductCode.Enabled = false;
                    //cmbProduct.Enabled = true;

                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
            }

            #endregion
        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (rbtnCode.Checked)
                {
                    //cmbProduct.Enabled = false;
                    //cmbProductCode.Enabled = true;

                }


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "rbtnCode_CheckedChanged", exMessage);
            }

            #endregion
        }

        private void btnVAT16_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT16 frmRptVAT16 = new FormRptVAT16();

                //mdi.RollDetailsInfo("8201");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvSale.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvSale.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvSale.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpInvoiceDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpInvoiceDate.Value;
                }
                else
                {
                    MessageBox.Show("No Details Found to Preview.", frmRptVAT16.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                frmRptVAT16.ShowDialog();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }

            #endregion


        }

        private void backgroundWorkerProductSearchDs_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);

                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS("", CategoryId, "", "", "", "", "", "");

                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerProductSearchDs_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                //start Complete
                ProductsMini.Clear();
                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    cmbProduct.Items.Add(item2["ProductName"]);
                    prod.ProductCode = item2["ProductCode"].ToString();
                    cmbProductCode.Items.Add(item2["ProductCode"]);

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

                    //bool TradingF = false;
                    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                    //prod.Trading = Trading;
                    //bool NonStockF = false;
                    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                    //prod.NonStock = NonStock;

                    ProductsMini.Add(prod);

                }//end foreach
                //end Complete
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
            }

            #endregion
            //this.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void backgroundWorkerReportVAT20Ds_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {


                ReportDSDAL ShowReport = new ReportDSDAL();
                //ReportResultVAT20 = ShowReport.VAT20ReportNew(txtSalesInvoiceNo.Text.Trim(), "N");

                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerReportVAT20Ds_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                //start Complete
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                ReportResultVAT20.Tables[0].TableName = "DsVAT20Item";
                ReportResultVAT20.Tables[1].TableName = "DsVAT20Pack";
                ReportClass objrpt = new ReportClass();

                objrpt = new RptVAT20();
                objrpt.SetDataSource(ReportResultVAT20);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 20'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                        //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                //end Complete
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT20Ds_RunWorkerCompleted", exMessage);
            }

            #endregion
            //this.Enabled = true;
            this.progressBar1.Visible = false;
            this.btn20.Enabled = true;

        }

        private void backgroundWorkerReportCreditNoteDs_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //start DoWork
                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //ReportDSSoapClient ShowReport = new ReportDSSoapClient();
                //DataSet ReportResultCreditNote = ShowReport.CreditNote(encriptedData.ToString(), Program.DatabaseName);

                //donot use new connection open

                ReportDSDAL ShowReport = new ReportDSDAL();
                //ReportResultCreditNote = ShowReport.ReportCreditNote(txtSalesInvoiceNo.Text.Trim(), Program.DatabaseName);
                //ReportResultCreditNote = ShowReport.ReportCreditNoteNew(txtSalesInvoiceNo.Text.Trim(), Program.DatabaseName);

                //end DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_DoWork", exMessage);
            }

            #endregion

        }
        private void backgroundWorkerReportCreditNoteDs_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                //start Complete
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";
                RptCreditNote objrpt = new RptCreditNote();
                objrpt.SetDataSource(ReportResultCreditNote);
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                        //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                //end Complete
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", exMessage);
            }

            #endregion
            //this.Enabled = true;
            this.btnDebitCredit.Enabled = true;
            this.progressBar1.Visible = false;
        }



        private void btnSearchProductName_Click(object sender, EventArgs e)
        {
            item = "1";
            cost = 108;

            a();
            var tt1 = ProductsMini.ToList();

        }

        private void a()
        {
            var tt = ProductsMini.ToList();

            var aa = ProductsMini.Where(x => x.CategoryID == item).Select(x => x.CostPrice = cost);
            var p = aa;
        }

        private void chkIs11_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIs11.Checked)
            {
                chkIs11.Text = "VAT11";
                btnPrint.Text = "VAT11";
            }
            else
            {
                chkIs11.Text = "VAT11 Gha";
                btnPrint.Text = "VAT11 Gha";
            }
        }

        private void chkIsBlank_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bgwVAT12_DoWork(object sender, DoWorkEventArgs e)
        {

        }



        private void chkCustCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustCode.Checked)
            {
                chkCustCode.Text = "Code";
            }
            else
            {
                chkCustCode.Text = "Name";

            }
            customerloadToCombo();
        }


        private void Discount()
        {
            decimal discountPercent = 0;
            decimal discountAmount = 0;
            decimal discountedNBRPrice = 0;
            decimal bDTRate = 0;
            if (!string.IsNullOrEmpty(txtBDTRate.Text.Trim()))
            {
                bDTRate = Convert.ToDecimal(txtBDTRate.Text.Trim());

            }
            if (bDTRate == null || bDTRate == 0)
            {
                MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                return;
            }
            if (!string.IsNullOrEmpty(txtDiscountedNBRPrice.Text.Trim()))
            {
                discountedNBRPrice = Convert.ToDecimal(txtDiscountedNBRPrice.Text.Trim());
                discountedNBRPrice = discountedNBRPrice / bDTRate;
            }
            if (!string.IsNullOrEmpty(txtDiscountAmountInput.Text.Trim()))
            {
                if (chkDiscount.Checked)
                {
                    discountPercent = Convert.ToDecimal(txtDiscountAmountInput.Text.Trim());
                    discountAmount = discountedNBRPrice * discountPercent / 100;
                }
                else
                {
                    discountAmount = Convert.ToDecimal(txtDiscountAmountInput.Text.Trim());

                }
            }
            if (discountedNBRPrice < discountAmount)
            {
                MessageBox.Show("Discount amount can't greater then NBR price", this.Text);
                return;
            }
            else
            {
                txtDiscountAmount.Text = Convert.ToDecimal(discountAmount).ToString();
                if (string.IsNullOrEmpty(ImportExcelID))
                {
                    txtNBRPrice.Text = Convert.ToDecimal(discountedNBRPrice - discountAmount).ToString();
                }


            }

            vNBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());
        }


        private void chkDiscount_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDiscount.Checked)
            {
                chkDiscount.Text = "Discount(%)";
            }
            else
            {
                chkDiscount.Text = "Discount(F)";
            }
            Discount();
        }

        private void txtDiscountAmountInput_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtDiscountAmountInput_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxQty(txtDiscountAmountInput, "Discount Input Amount");
            Discount();
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbCurrency_Leave(object sender, EventArgs e)
        {
            CurrencyValue();
            Discount();
            //dtpInvoiceDate.Focus();

        }
        private void CurrencyValue()
        {

            try
            {
                #region Statement

                string CFrom = cmbCurrency.Text.Trim().ToLower();
                string CTo = "BDT";
                string currencyBdtId = "260";
                string currencyUsdId = "249";

                txtDollerRate.Text = "0";
                txtBDTRate.Text = "0";
                txtCurrencyId.Text = currencyBdtId;

                if (!string.IsNullOrEmpty(CFrom))
                {

                    if (CFrom.ToLower() == CTo.ToLower())
                    {
                        txtCurrencyId.Text = currencyBdtId;
                        txtBDTRate.Text = "1";
                    }
                    else if (CurrencyConversions != null && CurrencyConversions.Any())
                    {

                        var currencyRate =
                            from rates in
                                CurrencyConversions.Where(
                                    x =>
                                    x.CurrencyCodeF.Trim().ToLower() == CFrom.ToLower() &&
                                    x.CurrencyCodeT.Trim().ToLower() == CTo.ToLower()).
                                ToList()
                            select new { rates.CurrencyCodeFrom, rates.CurrencyRate, rates.ConvertionDate };
                        if (currencyRate != null && currencyRate.Any())
                        {
                            txtCurrencyId.Text = currencyRate.First().CurrencyCodeFrom.ToString();
                            txtBDTRate.Text = Convert.ToDecimal(currencyRate.First().CurrencyRate.ToString()).ToString();
                            dtpConversionDate.Value = Convert.ToDateTime(currencyRate.First().ConvertionDate.ToString());
                        }
                        else
                        {
                            MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                            txtBDTRate.Text = "0";
                            txtCurrencyId.Text = currencyBdtId;
                        }


                    }
                    else
                    {
                        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        txtBDTRate.Text = "0";
                        txtCurrencyId.Text = currencyBdtId;
                    }

                    if (CurrencyConversions != null && CurrencyConversions.Any())
                    {
                        var DolerValue =
                            from DValue in
                                CurrencyConversions.Where(
                                    x =>
                                    x.CurrencyCodeFrom.Trim().ToLower() == currencyUsdId &&
                                    x.CurrencyCodeTo.Trim().ToLower() == currencyBdtId).
                                ToList()
                            select new { DValue.CurrencyRate };
                        if (DolerValue != null && DolerValue.Any())
                        {
                            txtDollerRate.Text = Convert.ToDecimal(DolerValue.First().CurrencyRate).ToString();
                        }
                        else
                        {
                            txtDollerRate.Text = "0";
                            //MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        }
                    }
                    else
                    {
                        txtDollerRate.Text = "0";
                        //MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);

                    }
                }
                else
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }

                #endregion
            }


            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
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
                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
            }

            #endregion
        }
        private void CurrencyValueBackup()
        {

            try
            {
                //#region Statement

                //string CFrom = cmbCurrency.Text.Trim().ToLower();
                //string CTo = "BDT";
                //CurrencyRateFromBDT = 1;
                //txtDollerRate.Text = "0";
                //dollerRate = 0;
                //txtBDTRate.Text = "1";


                //currencyFromID = currencyBDTID;
                //if (!string.IsNullOrEmpty(CFrom))
                //{

                //    if (CFrom.ToLower() == CTo.ToLower())
                //    {
                //        currencyFromID = currencyBDTID;
                //        CurrencyRateFromBDT = 1;
                //        txtBDTRate.Text = "1";

                //        //return;
                //    }
                //    else if (CurrencyConversions != null && CurrencyConversions.Any())
                //    {

                //        var currencyRate =
                //            from rates in
                //                CurrencyConversions.Where(
                //                    x =>
                //                    x.CurrencyCodeF.Trim().ToLower() == CFrom.ToLower() &&
                //                    x.CurrencyCodeT.Trim().ToLower() == CTo.ToLower()).
                //                ToList()
                //            select new {rates.CurrencyCodeFrom, rates.CurrencyRate};
                //        if (currencyRate != null && currencyRate.Any())
                //        {
                //            currencyFromID = currencyRate.First().CurrencyCodeFrom.ToString();
                //            CurrencyRateFromBDT = Convert.ToDecimal(currencyRate.First().CurrencyRate.ToString());
                //            txtBDTRate.Text = Convert.ToDecimal(currencyRate.First().CurrencyRate.ToString()).ToString();
                //        }
                //        else
                //        {
                //            MessageBox.Show(
                //                "There is no price declaration, Please contact with Administrator",
                //                this.Text);
                //            txtBDTRate.Text = "0";
                //            CurrencyRateFromBDT = 0;
                //            //return;
                //        }


                //    }
                //    else
                //    {
                //        MessageBox.Show(
                //            "There is no price declaration, Please contact with Administrator",
                //            this.Text);
                //        txtBDTRate.Text = "0";
                //        CurrencyRateFromBDT = 0;
                //        //return;
                //    }

                //    if (CurrencyConversions != null && CurrencyConversions.Any())
                //    {
                //        var DolerValue =
                //            from DValue in
                //                CurrencyConversions.Where(
                //                    x =>
                //                    x.CurrencyCodeFrom.Trim().ToLower() == currencyUSDID &&
                //                    x.CurrencyCodeTo.Trim().ToLower() == currencyBDTID).
                //                ToList()
                //            select new {DValue.CurrencyRate};
                //        if (DolerValue != null && DolerValue.Any())
                //        {
                //            dollerRate = Convert.ToDecimal(DolerValue.First().CurrencyRate.ToString());
                //        }
                //        else
                //        {
                //            dollerRate = 0;
                //            MessageBox.Show(
                //                "There is no price declaration from BDT to USD, Please contact with Administrator",
                //                this.Text);
                //            //return;
                //        }
                //    }
                //    else
                //    {
                //        dollerRate = 0;
                //        MessageBox.Show(
                //            "There is no price declaration from BDT to USD, Please contact with Administrator",
                //            this.Text);
                //        //return;
                //    }
                //}
                //else
                //{
                //    MessageBox.Show(
                //        "There is no price declaration, Please contact with Administrator",
                //        this.Text);
                //    return;
                //}

                //#endregion
            }


            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
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
                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CurrencyValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CurrencyValue", exMessage);
            }

            #endregion
        }

        private void txtRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtBDTRate, "Currency Rate");
        }

        private void cmbCGroup_Leave(object sender, EventArgs e)
        {
            //cmbCustomerName.Focus();
            customerloadToCombo();
        }
        private void ProductSearchDsLoad()
        {
            try
            {

                cmbProduct.Items.Clear();
                cmbProductCode.Items.Clear();
                //if (rbtnCode.Checked == true)
                //{

                //    var prodByCode = from prd in ProductsMini.ToList()
                //                     orderby prd.ProductCode
                //                     select prd.ProductCode;


                //    if (prodByCode != null && prodByCode.Any())
                //    {
                //        cmbProduct.Items.AddRange(prodByCode.ToArray());
                //    }


                //}
                //else
                //{
                //    var prodByCode = from prd in ProductsMini.ToList()
                //                     orderby prd.ProductName
                //                     select prd.ProductName;


                //    if (prodByCode != null && prodByCode.Any())
                //    {
                //        cmbProduct.Items.AddRange(prodByCode.ToArray());
                //    }
                //}

                var prodByCode = from prd in ProductsMini.ToList()
                                 orderby prd.ProductCode
                                 select prd.ProductCode;
                if (prodByCode != null && prodByCode.Any())
                {
                    cmbProductCode.Items.AddRange(prodByCode.ToArray());
                }

                var prodByName = from prd in ProductsMini.ToList()
                                 orderby prd.ProductName
                                 select prd.ProductName;

                if (prodByName != null && prodByName.Any())
                {
                    cmbProduct.Items.AddRange(prodByName.ToArray());
                }

                cmbProduct.Items.Insert(0, "Select");
                cmbProductCode.Items.Insert(0, "Select");
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
            }
            #endregion


        }

        private void cmbCGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
            // cmbCustomerName.Focus();
        }

        private void txtVehicleType_KeyDown(object sender, KeyEventArgs e)
        {
            //cmbCurrency.Focus();
        }

        private void cmbCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
            //txtBDTRate.Focus();
        }

        private void txtBDTRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
            //txtDiscountAmount.Focus();
        }

        private void txtDiscountAmount_KeyDown(object sender, KeyEventArgs e)
        {
            //txtSerialNo.Focus();
        }

        private void dtpDeliveryDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");

            }

        }

        private void txtTDBalance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                cmbProductCode.Focus();
            }
        }

        private void cmbProductCode_KeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtQty1.Focus();   
            }
            if (e.KeyCode.Equals(Keys.F9))
            {

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                    cmbProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    cmbProductCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();// ProductInfo[27].ToString();//27
                }
                cmbProductCode.Focus();

            }

        }

        private void cmbVATType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                cmbVAT1Name.Focus();
            }

        }

        private void cmbVAT1Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtDiscountAmountInput.Focus();
            }

        }

        private void txtDiscountAmountInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {

                btnAdd.Focus();
            }
        }

        private void cmbCGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtBDTRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void dtpInvoiceDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtLCNumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtTDBalance_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbVATType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbVAT1Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtCurrencyId_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtDollerRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void dtpInvoiceDate_Leave(object sender, EventArgs e)
        {
            //CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
            //     string.Empty, "Y", dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));



            //CurrencyConversions.Clear();

            //foreach (DataRow item2 in CurrencyConversionResult.Rows)
            //{
            //    var cConversion = new CurrencyConversionVM();
            //    cConversion.CurrencyConversionId = item2["CurrencyConversionId"].ToString();
            //    cConversion.CurrencyCodeFrom = item2["CurrencyCodeFrom"].ToString();
            //    cConversion.CurrencyCodeF = item2["CurrencyCodeF"].ToString();
            //    cConversion.CurrencyNameF = item2["CurrencyNameF"].ToString();
            //    cConversion.CurrencyCodeTo = item2["CurrencyCodeTo"].ToString();
            //    cConversion.CurrencyCodeT = item2["CurrencyCodeT"].ToString();
            //    cConversion.CurrencyNameT = item2["CurrencyNameT"].ToString();
            //    cConversion.CurrencyRate = Convert.ToDecimal(item2["CurrencyRate"].ToString());
            //    cConversion.Comments = item2["Comments"].ToString();
            //    cConversion.ActiveStatus = item2["ActiveStatus"].ToString();
            //    CurrencyConversions.Add(cConversion);

            //}
            //if (CurrencyConversionResult != null || CurrencyConversionResult.Rows.Count > 0)
            //{

            //    cmbCurrency.Items.Clear();
            //    var cmbSCurrencyCodeF = CurrencyConversions.Select(x => x.CurrencyCodeF).Distinct().ToList();

            //    if (cmbSCurrencyCodeF.Any())
            //    {
            //        cmbCurrency.Items.AddRange(cmbSCurrencyCodeF.ToArray());
            //    }

            //    cmbCurrency.Items.Insert(0, "BDT");

            //    cmbCurrency.SelectedIndex = 0;
            //}
            //if (rbtnExport.Checked == false)
            //{
            //    cmbCurrency.Items.Clear();
            //    cmbCurrency.Items.Insert(0, "BDT");
            //    cmbCurrency.SelectedIndex = 0;
            //}

        }

        private void dtpDeliveryDate_Leave(object sender, EventArgs e)
        {

            cmbProductCode.Focus();
        }

        private void btnFormKaT_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                FormRptVATKa frmRptVATKa = new FormRptVATKa();


                if (dgvSale.Rows.Count > 0)
                {
                    frmRptVATKa.txtItemNo.Text = dgvSale.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVATKa.txtProductName.Text = dgvSale.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVATKa.dtpFromDate.Value = dtpInvoiceDate.Value;
                    frmRptVATKa.dtpToDate.Value = dtpInvoiceDate.Value;
                }


                frmRptVATKa.ShowDialog();

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnFormKaT_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnFormKaT_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnFormKaT_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnFormKaT_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnFormKaT_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnFormKaT_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnFormKaT_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnFormKaT_Click", exMessage);
            }
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            string[] cValues = new string[] { "Y", dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss") };
            string[] cFields = new string[] { "cc.CurrencyCodeFrom like", "cc.ActiveStatus like", "cc.ConversionDate" };
            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, false);

            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
            //     string.Empty, "Y", dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));



            CurrencyConversions.Clear();

            foreach (DataRow item2 in CurrencyConversionResult.Rows)
            {
                var cConversion = new CurrencyConversionVM();
                cConversion.CurrencyConversionId = item2["CurrencyConversionId"].ToString();
                cConversion.CurrencyCodeFrom = item2["CurrencyCodeFrom"].ToString();
                cConversion.CurrencyCodeF = item2["CurrencyCodeF"].ToString();
                cConversion.CurrencyNameF = item2["CurrencyNameF"].ToString();
                cConversion.CurrencyCodeTo = item2["CurrencyCodeTo"].ToString();
                cConversion.CurrencyCodeT = item2["CurrencyCodeT"].ToString();
                cConversion.CurrencyNameT = item2["CurrencyNameT"].ToString();
                cConversion.CurrencyRate = Convert.ToDecimal(item2["CurrencyRate"].ToString());
                cConversion.Comments = item2["Comments"].ToString();
                cConversion.ActiveStatus = item2["ActiveStatus"].ToString();
                CurrencyConversions.Add(cConversion);

            }
            if (CurrencyConversionResult != null || CurrencyConversionResult.Rows.Count > 0)
            {

                cmbCurrency.Items.Clear();
                var cmbSCurrencyCodeF = CurrencyConversions.Select(x => x.CurrencyCodeF).Distinct().ToList();

                if (cmbSCurrencyCodeF.Any())
                {
                    cmbCurrency.Items.AddRange(cmbSCurrencyCodeF.ToArray());
                }

                cmbCurrency.Items.Insert(0, "BDT");

                cmbCurrency.SelectedIndex = 0;
            }
            if (rbtnExport.Checked == false)
            {
                cmbCurrency.Items.Clear();
                cmbCurrency.Items.Insert(0, "BDT");
                //cmbCurrency.SelectedIndex = 0;
            }
        }

        private void dgvReceiveHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvReceiveHistory["Select", e.RowIndex].Value = !Convert.ToBoolean(dgvReceiveHistory["Select", e.RowIndex].Value);
            }
        }

        private void dgvReceiveHistory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.KeyCode.Equals(Keys.F2))
                {
                    dgvSale.Rows.Clear();
                    int j = 0;
                    for (int i = 0; i < dgvReceiveHistory.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value) == true)
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();

                            dgvSale.Rows.Add(NewRow);
                            dgvSale.Rows[j].Cells["LineNo"].Value = j + 1;
                            dgvSale.Rows[j].Cells["PCode"].Value = dgvReceiveHistory["PCodeP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["ItemNo"].Value = dgvReceiveHistory["ItemNoP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["ItemName"].Value = dgvReceiveHistory["ItemNameP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UOM"].Value = dgvReceiveHistory["UOMP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Quantity"].Value = dgvReceiveHistory["QuantityP", i].Value.ToString();

                            dgvSale.Rows[j].Cells["SaleQuantity"].Value = dgvReceiveHistory["SaleQuantityP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["PromotionalQuantity"].Value = dgvReceiveHistory["PromotionalQuantityP", i].Value.ToString();

                            dgvSale.Rows[j].Cells["NBRPrice"].Value = dgvReceiveHistory["NBRPriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["SubTotal"].Value = 0;// dgvReceiveHistory["SubTotalP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["TradingMarkUp"].Value = dgvReceiveHistory["TradingMarkUpP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["SD"].Value = dgvReceiveHistory["SDP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["VATRate"].Value = dgvReceiveHistory["VATRateP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["VATAmount"].Value = 0;// dgvReceiveHistory["VATAmountP", i].Value.ToString();
                            //dgvSale.Rows[j].Cells["Total"].Value = dgvReceiveHistory["TotalP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Comments"].Value = dgvReceiveHistory["CommentsP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UnitPrice"].Value = dgvReceiveHistory["UnitPriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["SDAmount"].Value = 0;// dgvReceiveHistory["SDAmountP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Status"].Value = "Old";// dgvReceiveHistory["StatusP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Previous"].Value = dgvReceiveHistory["PreviousP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Change"].Value = 0;// dgvReceiveHistory["ChangeP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Stock"].Value = dgvReceiveHistory["StockP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Trading"].Value = dgvReceiveHistory["TradingP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["NonStock"].Value = dgvReceiveHistory["NonStockP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Type"].Value = dgvReceiveHistory["TypeP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UOMPrice"].Value = dgvReceiveHistory["UOMPriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UOMc"].Value = dgvReceiveHistory["UOMcP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UOMn"].Value = dgvReceiveHistory["UOMnP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["UOMQty"].Value = dgvReceiveHistory["UOMQtyP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["DiscountAmount"].Value = dgvReceiveHistory["DiscountAmountP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["DiscountedNBRPrice"].Value = dgvReceiveHistory["DiscountedNBRPriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["BDTValue"].Value = dgvReceiveHistory["BDTValueP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["DollerValue"].Value = dgvReceiveHistory["DollerValueP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["VATName"].Value = dgvReceiveHistory["VATNameP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["NBRPWithVAT"].Value = dgvReceiveHistory["NBRPriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["Weight"].Value = dgvReceiveHistory["WeightP", i].Value.ToString();

                            
                            if (!chkTrading.Checked)
                            {
                                dgvSale.Rows[j].Cells["CConvDate"].Value = dgvReceiveHistory["CConvDateP", i].Value.ToString();
                            }
                            

                            #region currency convertion date change for export
                            if (rbtnExport.Checked || ChkExpLoc.Checked)
                            {
                                dgvSale.Rows[j].Cells["VATAmount"].Value = dgvReceiveHistory["VATAmountP", i].Value.ToString();
                                dgvSale.Rows[j].Cells["SubTotal"].Value = dgvReceiveHistory["SubTotalP", i].Value.ToString();
                                dgvSale.Rows[j].Cells["SDAmount"].Value = dgvReceiveHistory["SDAmountP", i].Value.ToString();
                            }
                            #endregion currency convertion date change for export

                            if (rbtnCN.Checked || rbtnDN.Checked)
                            {
                                dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                            }

                            j = j + 1;
                        }
                    }
                    if (rbtnExport.Checked || ChkExpLoc.Checked)
                    {
                        GTotal();
                    }
                    else
                    {
                        Rowcalculate();
                    }
                    dgvReceiveHistory.Visible = false;
                    dgvSale.Visible = true;

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
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



       
        private void txtDiscountedNBRPrice_TextChanged(object sender, EventArgs e)
        {

        }







        private void txtQty2_Leave(object sender, EventArgs e)
        {
            if (Program.CheckingNumericTextBox(txtQty2, "Total Quantity") == true)
            {
                txtQty2.Text = Program.FormatingNumeric(txtQty2.Text.Trim(), SalePlaceQty).ToString();
                TotalQty();
            }
        }

        private void txtQty1_Leave(object sender, EventArgs e)
        {
            //if (rbtnCN.Checked)
            //{
            //    decimal quantity = Convert.ToDecimal(dgvSale.CurrentRow.Cells["Quantity"].Value);
            //    if (Convert.ToDecimal(txtQty1.Text) > quantity)
            //    {
            //        MessageBox.Show("Credit Note can not be greater than actual quantity.");
            //        txtQty1.Text = "0";
            //        txtQty1.Focus();
            //        return;
            //    }
            //}
           
            if (Program.CheckingNumericTextBox(txtQty1, "Total Quantity") == true)
            {
                txtQty1.Text = Program.FormatingNumeric(txtQty1.Text.Trim(), SalePlaceQty).ToString();
                TotalQty();
            }

            //if (rbtnService.Checked)
            //{
            //    txtNBRPrice.Focus();
            //}
            //else if (rbtnServiceNS.Checked)
            //{
            //    txtNBRPrice.Focus();

            //}
            //else if (rbtnExport.Checked)
            //{
            //    txtNBRPrice.Focus();

            //}
            //else
            //{
            //    btnAdd.Focus();

            //}

        }
        private void TotalQty()
        {
            txtQuantity.Text =
                Convert.ToString(Convert.ToDecimal(txtQty1.Text.Trim()) + Convert.ToDecimal(txtQty2.Text.Trim()));
        }

        private void txtQty1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (rbtnService.Checked)
                {
                    txtNBRPrice.Focus();
                }
                else if (rbtnServiceNS.Checked)
                {
                    txtNBRPrice.Focus();

                }
                else if (rbtnExport.Checked)
                {
                    txtNBRPrice.Focus();
                }
                else
                {
                    btnAdd.Focus();

                }
            }
        }

        private void txtQty2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {

                btnAdd.Focus();
            }
        }

        private void ChkExpLoc_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkExpLoc.Checked)
            {
                ChkExpLoc.Text = "Export";
                btnExport.Visible = true;
                btn20.Visible = true;
                btnTracking.Visible = false;
                txtLCNumber.Visible = true;
                labelLC.Visible = true;
                //txtCategoryName.Text = "Export";
                if (rbtnTrading.Checked)
                {
                    rbtnExportTrading.Checked = true;
                    btnPrint.Text = "VAT 11";
                }
                else if (rbtnService.Checked)
                    rbtnExportService.Checked = true;
                else if (rbtnServiceNS.Checked)
                    rbtnExportServiceNS.Checked = true;
                else if (rbtnTradingTender.Checked)
                    rbtnExportTradingTender.Checked = true;

                else if (rbtnTender.Checked)
                    rbtnExportTender.Checked = true;
                else if (rbtnPackageSale.Checked)
                    rbtnExportPackage.Checked = true;
                #region Currency
                if (CurrencyConversionResult != null || CurrencyConversionResult.Rows.Count > 0)
                {

                    cmbCurrency.Items.Clear();
                    var cmbSCurrencyCodeF = CurrencyConversions.Select(x => x.CurrencyCodeF).Distinct().ToList();

                    if (cmbSCurrencyCodeF.Any())
                    {
                        cmbCurrency.Items.AddRange(cmbSCurrencyCodeF.ToArray());
                    }

                    cmbCurrency.Items.Insert(0, "BDT");

                    cmbCurrency.SelectedIndex = 0;
                    cmbCurrency.Text = "USD";
                }
                #endregion Currency

                #region CGroup

                if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                {
                    cmbCGroup.Items.Clear();
                    var vcmbCGroup = customerMini.Where(x => x.GroupType == "Export").Select(x => x.CustomerGroupName).Distinct().ToList();

                    if (vcmbCGroup.Any())
                    {
                        cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                    }
                    cmbCGroup.Items.Insert(0, "Select");
                    cmbCGroup.SelectedIndex = 0;
                }
                
                #endregion CGroup
            }
            else
            {
                //txtNBRPrice.ReadOnly = true;
                //txtCategoryName.Text = "Finish";

                ChkExpLoc.Text = "Local";
                btnExport.Visible = false;
                
                btn20.Visible = false;
                txtLCNumber.Visible = false;
                labelLC.Visible = false;
                if (rbtnExportTrading.Checked)
                {
                    rbtnTrading.Checked = true;
                    btnPrint.Text = "VAT 11 Ka";

                }
                else if (rbtnExportService.Checked)
                    rbtnService.Checked = true;

                else if (rbtnExportServiceNS.Checked)
                    rbtnServiceNS.Checked = true;

                else if (rbtnExportTradingTender.Checked)
                    rbtnTradingTender.Checked = true;
                else if (rbtnExportTender.Checked)
                    rbtnTender.Checked = true;
                else if (rbtnExportPackage.Checked)
                    rbtnPackageSale.Checked = true;
                #region  Currency

                cmbCurrency.Items.Clear();
                cmbCurrency.Items.Insert(0, "BDT");
                cmbCurrency.SelectedIndex = 0;

                #endregion currency

                #region CGroup


                if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                {
                    cmbCGroup.Items.Clear();
                    var vcmbCGroup = customerMini.Where(x => x.GroupType == "Export").Select(x => x.CustomerGroupName).Distinct().ToList();

                    if (vcmbCGroup.Any())
                    {
                        cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                    }
                    cmbCGroup.Items.Insert(0, "Select");
                    cmbCGroup.SelectedIndex = 0;
                }
                #endregion CGroup

            }
            ProductSearchDs();
        }

        private void txtQty1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNBRPrice_KeyDown(object sender, KeyEventArgs e)
        
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                //if (rbtnService.Checked)
                //{
                //    txtNBRPrice.Focus();
                //}
                //else if (rbtnServiceNS.Checked)
                //{
                //    txtNBRPrice.Focus();

                //}
                //else
                //{

                btnAdd.Focus();

                //}
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            if (chkSame.Checked == false)
            {
                BrowsFile();
            }
                fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }
            //}
            //else
            //{
            //    fileName = Program.ImportFileName;
            //}

            #region new process for bom import

            string[] extention = fileName.Split(".".ToCharArray());
            string[] retResults = new string[4];
            if (extention[extention.Length-1]=="txt")
            {
                retResults = ImportFromText();
            }
            else
            {
                retResults = ImportFromExcel();
            }
            //string[] retResults = Import();

            string result = retResults[0];
            string message = retResults[1];
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException("BomImport",
                                                "Unexpected error.");
            }
            else if (result == "Success" || result == "Fail")
            {
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            #endregion new process for bom import
        }

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";

                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "BrowsFile", exMessage);
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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private string[] Import()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            #endregion Close1

            #region try
            OleDbConnection theConnection = null;
            TransactionTypes();

            try
            {
            //    #region Load Excel

            //    string str1 = "";
            //    CommonDAL commonDal = new CommonDAL();
            //    bool AutoItem = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);
            //    string fileName = Program.ImportFileName;
            //    if (string.IsNullOrEmpty(fileName))
            //    {
            //        MessageBox.Show("Please select the right file for import");
            //        return sqlResults;
            //    }
            //    string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
            //                              + "Data Source=" + fileName + ";"
            //                              + "Extended Properties=" + "\""
            //                              + "Excel 12.0;HDR=YES;" + "\"";
            //    theConnection = new OleDbConnection(connectionString);
            //    theConnection.Open();
            //    OleDbDataAdapter adapterSaleM = new OleDbDataAdapter("SELECT * FROM [SaleM$]", theConnection);
            //    DataTable dtSaleM = new System.Data.DataTable();
            //    adapterSaleM.Fill(dtSaleM);

            //    OleDbDataAdapter adapterSaleD = new OleDbDataAdapter("SELECT * FROM [SaleD$]", theConnection);
            //    DataTable dtSaleD = new System.Data.DataTable();
            //    adapterSaleD.Fill(dtSaleD);

            //    OleDbDataAdapter adapterSaleE = new OleDbDataAdapter("SELECT * FROM [SaleE$]", theConnection);
            //    DataTable dtSaleE = new System.Data.DataTable();
            //    adapterSaleE.Fill(dtSaleE);

            //    theConnection.Close();

            //    #endregion Load Excel
                                
            //    #region RowCount
            //    int MRowCount = 0;
            //    int MRow = dtSaleM.Rows.Count;
            //    for (int i = 0; i < dtSaleM.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(dtSaleM.Rows[i]["ID"].ToString()))
            //        {
            //            MRowCount++;
            //        }

            //    }
            //    if (MRow != MRowCount)
            //    {
            //        MessageBox.Show("you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.");
            //        return sqlResults;
            //    }
            //    #endregion RowCount

            //    #region ID in Master or Detail table

            //    for (int i = 0; i < MRowCount; i++)
            //    {
            //        string importID = dtSaleM.Rows[i]["ID"].ToString();

            //        if (!string.IsNullOrEmpty(importID))
            //        {
            //            DataRow[] DetailRaws = dtSaleD.Select("ID='" + importID + "'");
            //            if (DetailRaws.Length == 0)
            //            {
            //                throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
            //            }

            //        }

            //    }

            //    #endregion

            //    #region Double ID in Master

            //    for (int i = 0; i < MRowCount; i++)
            //    {
            //        string id = dtSaleM.Rows[i]["ID"].ToString();
            //        DataRow[] tt = dtSaleM.Select("ID='" + id + "'");
            //        if (tt.Length > 1)
            //        {
            //            MessageBox.Show("you have duplicate master id " + id + " in import file.");
            //            return sqlResults;
            //        }

            //    }

            //    #endregion Double ID in Master

            //    CommonImport cImport = new CommonImport();

            //    #region checking from database is exist the information(NULL Check)
            //    #region Master
            //    string CurrencyId = string.Empty;
            //    string USDCurrencyId = string.Empty;

            //    for (int i = 0; i < MRowCount; i++)
            //    {
            //        CurrencyId = string.Empty;
            //        USDCurrencyId = string.Empty;
            //        #region Master
            //        #region FindCustomerId
            //        cImport.FindCustomerId(dtSaleM.Rows[i]["Customer_Name"].ToString().Trim(),
            //                               dtSaleM.Rows[i]["Customer_Code"].ToString().Trim(), null, null);
            //        #endregion FindCustomerId
                    
            //        #region FindCurrencyId
            //        CurrencyId = cImport.FindCurrencyId(dtSaleM.Rows[i]["Currency_Code"].ToString().Trim(), null, null);
            //        USDCurrencyId = cImport.FindCurrencyId("USD", null, null);
            //        cImport.FindCurrencyRateFromBDT(CurrencyId, null, null);
            //        cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId, null, null);

            //        #endregion FindCurrencyId

            //        #region FindTenderId
            //        cImport.FindTenderId(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), null, null);
            //        #endregion FindTenderId

            //        #region Checking Date is null or different formate
            //        bool IsInvoiceDate;
            //        IsInvoiceDate = cImport.CheckDate(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim());
            //        if (IsInvoiceDate != true)
            //        {
            //            throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date_Time field.");
            //        }
            //        bool IsDeliveryDate;
            //        IsDeliveryDate = cImport.CheckDate(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim());
            //        if (IsDeliveryDate != true)
            //        {
            //            throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Delivery_Date_Time field.");
            //        }
            //        #endregion Checking Date is null or different formate

            //        #region Checking Y/N value
            //        bool post;
            //        bool isPrint;
            //        post = cImport.CheckYN(dtSaleM.Rows[i]["Post"].ToString().Trim());
            //        if (post != true)
            //        {
            //            throw new ArgumentNullException("Please insert Y/N in Post field.");
            //        }
            //        isPrint = cImport.CheckYN(dtSaleM.Rows[i]["Is_Print"].ToString().Trim());
            //        if (isPrint != true)
            //        {
            //            throw new ArgumentNullException("Please insert Y/N in Is_Print field.");
            //        }
            //        #endregion Checking Y/N value

            //        #region Check previous invoice id
            //        string PreInvoiceId = string.Empty;
            //        string TenderId = string.Empty;

            //        PreInvoiceId = cImport.CheckIssueReturnID(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), null, null);
            //        TenderId = cImport.CheckTenderID(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), null, null);
            //        #endregion Check previous invoice id

            //        #endregion Master

            //    }
            //    #endregion Master

            //    #region Details

            //    #region Row count for details table
            //    int DRowCount = 0;
            //    for (int i = 0; i < dtSaleD.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["ID"].ToString()))
            //        {
            //            DRowCount++;
            //        }

            //    }
            //    #endregion Row count for details table

            //    for (int i = 0; i < DRowCount; i++)
            //    {
            //        string ItemNo = string.Empty;
            //        string UOMn = string.Empty;
            //        bool IsQuantity, IsNbrPrice, IsTrading, IsSDRate, IsVatRate, IsDiscount, IsPromoQuantity;


            //        #region FindItemId
            //        ItemNo = cImport.FindItemId(dtSaleD.Rows[i]["Item_Name"].ToString().Trim()
            //                     , dtSaleD.Rows[i]["Item_Code"].ToString().Trim(), null, null);
            //        #endregion FindItemId

            //        #region FindUOMn
            //        UOMn = cImport.FindUOMn(ItemNo, null, null);
            //        #endregion FindUOMn
            //        #region FindUOMn
            //        cImport.FindUOMc(UOMn, dtSaleD.Rows[i]["UOM"].ToString().Trim(), null, null);
            //        #endregion FindUOMn

            //        #region VATName
            //        cImport.FindVatName(dtSaleD.Rows[i]["VAT_Name"].ToString().Trim());

            //        #endregion VATName

            //        #region FindLastNBRPrice
            //        DataRow[] vmaster; //= new DataRow[];//

            //        string nbrPrice = string.Empty;
            //        //var transactionDate = DateTime.MinValue;
            //        var transactionDate = "";
            //        vmaster = dtSaleM.Select("ID='" + dtSaleD.Rows[i]["ID"].ToString().Trim() + "'");
            //        foreach (DataRow row in vmaster)
            //        {
            //            //var tt = row["Invoice_Date_Time"].ToString().Trim();
            //             var tt =Convert.ToDateTime(row["Invoice_Date_Time"].ToString()).ToString("yyyy-MMM-dd HH:mm:ss").Trim();
            //            transactionDate = tt;

            //        }

            //        nbrPrice = cImport.FindLastNBRPrice(ItemNo, dtSaleD.Rows[i]["VAT_Name"].ToString().Trim(),
            //            transactionDate, null, null);
            //        if (nbrPrice == "0")
            //        {
            //            if (rbtnExportService.Checked == false
            //                && rbtnExportServiceNS.Checked == false
            //                && rbtnExportServiceNS.Checked == false
            //                && rbtnService.Checked == false
            //                && rbtnServiceNS.Checked == false
            //                )
            //            {
            //                MessageBox.Show("Price declaration of item('" +
            //                                dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "') not find in database");
            //            }
            //        }





            //        #endregion FindLastNBRPrice

            //        #region Numeric value check
            //        IsQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Quantity"].ToString().Trim());
            //        if (IsQuantity != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in Quantity field.");
            //        }
            //        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["NBR_Price"].ToString().Trim());
            //        if (IsNbrPrice != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in NBR_Price field.");
            //        }
            //        IsVatRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim());
            //        if (IsVatRate != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in VAT_Rate field.");
            //        }
            //        IsSDRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["SD_Rate"].ToString().Trim());
            //        if (IsSDRate != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in SD_Rate field.");
            //        }
            //        IsTrading = cImport.CheckNumericBool(dtSaleD.Rows[i]["Trading_MarkUp"].ToString().Trim());
            //        if (IsTrading != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in Trading_MarkUp field.");
            //        }
            //        IsDiscount = cImport.CheckNumericBool(dtSaleD.Rows[i]["Discount_Amount"].ToString().Trim());
            //        if (IsDiscount != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in Discount_Amount field.");
            //        }
            //        IsPromoQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim());
            //        if (IsPromoQuantity != true)
            //        {
            //            throw new ArgumentNullException("Please insert decimal value in Promotional_Quantity field.");
            //        }
            //        #endregion Numeric value check

            //        #region Checking Y/N value
            //        bool NonStock;
            //        NonStock = cImport.CheckYN(dtSaleD.Rows[i]["Non_Stock"].ToString().Trim());
            //        if (NonStock != true)
            //        {
            //            throw new ArgumentNullException("Please insert Y/N in Non_Stock field.");
            //        }
            //        #endregion Checking Y/N value
            //    }

            //    #endregion Details
            //    #region Export
            //    #endregion Export

            //    #endregion checking from database is exist the information(NULL Check)
            //    #region Process model

            //    for (int i = 0; i < MRowCount; i++)
            //    {
            //        #region Master Sale

            //        string importId = dtSaleM.Rows[i]["ID"].ToString().Trim();
            //        string customerName = dtSaleM.Rows[i]["Customer_Name"].ToString().Trim();
            //        string customerCode = dtSaleM.Rows[i]["Customer_Code"].ToString().Trim();

            //        #region FindCustomerId

            //        string customerId = cImport.FindCustomerId(customerName, customerCode, null, null);

            //        #endregion FindCustomerId

            //        string deliveryAddress = dtSaleM.Rows[i]["Delivery_Address"].ToString().Trim();
            //        string vehicleNo = dtSaleM.Rows[i]["Vehicle_No"].ToString().Trim();
            //        //DateTime invoiceDateTime = dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim());
            //        var invoiceDateTime =Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MMM-dd HH:mm:ss");
            //        var deliveryDateTime =
            //            Convert.ToDateTime(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim()).ToString("yyyy-MMM-dd HH:mm:ss");
            //        #region CheckNull
            //        string referenceNo = cImport.ChecKNullValue(dtSaleM.Rows[i]["Reference_No"].ToString().Trim());
            //        string comments = cImport.ChecKNullValue(dtSaleM.Rows[i]["Comments"].ToString().Trim());
            //        #endregion CheckNull
            //        string saleType = dtSaleM.Rows[i]["Sale_Type"].ToString().Trim();
            //        #region Check previous invoice no.
            //        string previousInvoiceNo = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), null, null);
            //        #endregion Check previous invoice no.
            //        string isPrint = dtSaleM.Rows[i]["Is_Print"].ToString().Trim();
            //        #region Check Tender id
            //        string tenderId = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), null, null);
            //        #endregion Check Tender id
            //        string post = dtSaleM.Rows[i]["Post"].ToString().Trim();

            //        #region CheckNull
            //        string lCNumber = cImport.ChecKNullValue(dtSaleM.Rows[i]["LC_Number"].ToString().Trim());
            //        #endregion CheckNull
            //        string currencyCode = dtSaleM.Rows[i]["Currency_Code"].ToString().Trim();
            //        //string isVDS = dtSaleM.Rows[i]["Is_VDS"].ToString().Trim();
            //        //string getVDSCertificate = dtSaleM.Rows[i]["Get_VDS_Certificate"].ToString().Trim();
            //        //string vDSCertificateDate = dtSaleM.Rows[i]["VDS_Certificate_Date"].ToString().Trim();



            //        #region Master

            //        saleMaster = new SaleMasterVM();
            //        saleMaster.CustomerID = customerId;
            //        saleMaster.DeliveryAddress1 = deliveryAddress;
            //        saleMaster.VehicleNo = vehicleNo;
            //        saleMaster.InvoiceDateTime =
            //           Convert.ToDateTime(invoiceDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            //        saleMaster.SerialNo = referenceNo;
            //        saleMaster.Comments = comments;
            //        saleMaster.CreatedBy = Program.CurrentUser;
            //        saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
            //        saleMaster.LastModifiedBy = Program.CurrentUser;
            //        saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
            //        saleMaster.SaleType = saleType;
            //        saleMaster.PreviousSalesInvoiceNo = previousInvoiceNo;
            //        saleMaster.Trading = "N";
            //        saleMaster.IsPrint = isPrint;
            //        saleMaster.TenderId = tenderId;
            //        saleMaster.TransactionType = transactionType;
            //        saleMaster.DeliveryDate =
            //           Convert.ToDateTime(deliveryDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            //        saleMaster.Post = post; //Post
            //        var currencyid = cImport.FindCurrencyId(currencyCode, null, null);
            //        saleMaster.CurrencyID = currencyid; //Post
            //        saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, null, null));
            //        saleMaster.ReturnId = cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", null, null), null, null);
            //            // return ID is used for doller rate
            //        saleMaster.LCNumber = lCNumber;
            //        saleMaster.ImportID = importId;

            //        saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
            //        saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

            //        #endregion Master


            //        #endregion Master Sale

            //        #region Match

            //        DataRow[] DetailsRaws; //= new DataRow[];//
            //        if (!string.IsNullOrEmpty(importId))
            //        {
            //            DetailsRaws = dtSaleD.Select("ID='" + importId + "'");
            //        }
            //        else
            //        {
            //            DetailsRaws = null;
            //        }

                   

            //        #endregion Match

            //        #region Details Sale

            //        int dCounter = 1;
            //        saleDetails = new List<SaleDetailVM>();

            //        foreach (DataRow row in DetailsRaws)
            //        {
            //            string LastNBRPrice = string.Empty;
            //            string saleDID = row["ID"].ToString().Trim();
            //            string itemCode = row["Item_Code"].ToString().Trim();
            //            string itemName = row["Item_Name"].ToString().Trim();
            //            string itemNo = cImport.FindItemId(itemName, itemCode, null, null);
            //            string quantity = row["Quantity"].ToString().Trim();
            //            string nBRPrice = row["NBR_Price"].ToString().Trim();
            //            string vATName = row["VAT_Name"].ToString().Trim();


            //            if (rbtnExportService.Checked == true
            //                || rbtnExportServiceNS.Checked == true
            //                || rbtnExportServiceNS.Checked == true
            //                || rbtnService.Checked == true
            //                || rbtnServiceNS.Checked == true
            //                )
            //            {
            //                if (nBRPrice == "0")
            //                {
            //                    LastNBRPrice = cImport.FindLastNBRPrice(itemNo, vATName,
            //                                                           invoiceDateTime, null, null);
            //                }
            //                else
            //                {
            //                    LastNBRPrice = nBRPrice;
            //                }
            //            }
            //            else
            //            {
            //                LastNBRPrice = cImport.FindLastNBRPrice(itemNo, vATName,
            //                                                            invoiceDateTime, null, null);
            //            }
            //            string uOM = row["UOM"].ToString().Trim();
            //            string uOMn = cImport.FindUOMn(itemNo, null, null);
            //            string uOMc = cImport.FindUOMc(uOMn, uOM, null, null);


            //            string vATRate = row["VAT_Rate"].ToString().Trim();
            //            string sDRate = row["SD_Rate"].ToString().Trim();
            //            string nonStock = row["Non_Stock"].ToString().Trim();
            //            string tradingMarkUp = row["Trading_MarkUp"].ToString().Trim();
            //            string type = row["Type"].ToString().Trim();
            //            string discountAmount = row["Discount_Amount"].ToString().Trim();
            //            string promotionalQuantity = row["Promotional_Quantity"].ToString().Trim();

            //            SaleDetailVM detail = new SaleDetailVM();

            //            detail.InvoiceLineNo = dCounter.ToString();
            //            detail.ItemNo = itemNo;
            //            decimal vQuantity =
            //                Convert.ToDecimal(Convert.ToDecimal(quantity) + Convert.ToDecimal(promotionalQuantity));
            //            detail.Quantity = vQuantity;
            //            detail.PromotionalQuantity = Convert.ToDecimal(promotionalQuantity);
            //            detail.UOM = uOM;
            //            detail.VATRate = Convert.ToDecimal(vATRate);
            //            detail.SD = Convert.ToDecimal(sDRate);
            //            detail.CommentsD = "NA";
            //            detail.SaleTypeD = saleType;
            //            detail.PreviousSalesInvoiceNoD = previousInvoiceNo;
            //            detail.TradingD = "N";
            //            detail.NonStockD = nonStock;
            //            detail.Type = type;
            //            detail.TradingMarkUp = Convert.ToDecimal(tradingMarkUp);
            //            detail.DiscountAmount = Convert.ToDecimal(discountAmount);

            //            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
            //                                                           *Convert.ToDecimal(uOMc))
            //                                         - Convert.ToDecimal(discountAmount);

            //            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
            //            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);

            //            detail.SalesPrice = nbrPrice;
            //            detail.NBRPrice = nbrPrice;
            //            decimal subTotal = vQuantity*nbrPrice;
            //            detail.SubTotal = Convert.ToDecimal(subTotal);
            //            detail.VATAmount = subTotal*Convert.ToDecimal(vATRate)/100;
            //            detail.SDAmount = subTotal*Convert.ToDecimal(vATRate)/100;
            //            detail.UOMQty = Convert.ToDecimal(vQuantity)*Convert.ToDecimal(uOMc);
            //            detail.UOMn = uOMn;
            //            detail.UOMc = Convert.ToDecimal(uOMc);
            //            detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);

            //            detail.DollerValue = Convert.ToDecimal(nbrPrice)*
            //                                 Convert.ToDecimal(
            //                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", null, null), null, null));
            //            detail.CurrencyValue = Convert.ToDecimal(nbrPrice);

            //            saleDetails.Add(detail);


            //            dCounter++;

            //        } // details

            //        #endregion Details Sale

            //        #region Details Export

            //        int eCounter = 1;

            //        if (rbtnExport.Checked)
            //        {
            //            DataRow[] ExportRaws; //= new DataRow[];//
            //            if (!string.IsNullOrEmpty(importId))
            //            {
            //                ExportRaws = dtSaleE.Select("ID='" + importId + "'");
            //            }
            //            else
            //            {

            //                ExportRaws = null;
            //                if (ExportRaws == null)
            //                {
            //                    MessageBox.Show("For Export sale must filup the SaleE file");
            //                    return sqlResults;
            //                }

            //            }

            //            saleExport = new List<SaleExportVM>();
            //            foreach (DataRow row in ExportRaws)
            //            {
            //                string saleEID = row["ID"].ToString().Trim();
            //                string description = row["Description"].ToString().Trim();
            //                string quantityE = row["Quantity"].ToString().Trim();
            //                string grossWeight = row["GrossWeight"].ToString().Trim();

            //                string netWeight = row["NetWeight"].ToString().Trim();
            //                string numberFrom = row["NumberFrom"].ToString().Trim();
            //                string numberTo = row["NumberTo"].ToString().Trim();
            //                //string portFrom = row["PortFrom"].ToString().Trim();

            //                //string portTo = row["PortTo"].ToString().Trim();


            //                SaleExportVM expDetail = new SaleExportVM();
            //                expDetail.SaleLineNo = eCounter.ToString();
            //                expDetail.Description = description.ToString();
            //                expDetail.QuantityE = quantityE.ToString();
            //                expDetail.GrossWeight = grossWeight.ToString();
            //                expDetail.NetWeight = netWeight.ToString();
            //                expDetail.NumberFrom = numberFrom.ToString();
            //                expDetail.NumberTo = numberTo.ToString();
            //                expDetail.CommentsE = "NA";
            //                expDetail.RefNo = "NA";

            //                saleExport.Add(expDetail);



            //                eCounter++;

            //            } // details


            //        }
            //        #endregion Details Export

            //        SAVE_DOWORK_SUCCESS = false;
            //        sqlResults = new string[4];

            //        SaleDAL saleDal = new SaleDAL();
            //        sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport, null, null);
            //        SAVE_DOWORK_SUCCESS = true;
                   
            //    }// master

            //    return sqlResults;


            }
            //    #endregion Process model

            #endregion try
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
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductImport", exMessage);
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
                FileLogger.Log(this.Name, "ProductImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", exMessage);
            }
            return sqlResults;

            #endregion
        }

        private string[] ImportFromExcel()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            #endregion Close1

            #region try
            OleDbConnection theConnection = null;
            TransactionTypes();

            try
            {
                #region Load Excel

                string str1 = "";
                CommonDAL commonDal = new CommonDAL();
                bool AutoItem = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return sqlResults;
                }
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                                          + "Data Source=" + fileName + ";"
                                          + "Extended Properties=" + "\""
                                          + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter adapterSaleM = new OleDbDataAdapter("SELECT * FROM [SaleM$]", theConnection);
                DataTable dtSaleM = new System.Data.DataTable();
                adapterSaleM.Fill(dtSaleM);

                OleDbDataAdapter adapterSaleD = new OleDbDataAdapter("SELECT * FROM [SaleD$]", theConnection);
                DataTable dtSaleD = new System.Data.DataTable();
                adapterSaleD.Fill(dtSaleD);

                OleDbDataAdapter adapterSaleE = new OleDbDataAdapter("SELECT * FROM [SaleE$]", theConnection);
                DataTable dtSaleE = new System.Data.DataTable();
                adapterSaleE.Fill(dtSaleE);

                theConnection.Close();

                #endregion Load Excel
                dtSaleM.Columns.Add("Transection_Type");
                dtSaleM.Columns.Add("Created_By");
                dtSaleM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtSaleM.Rows)
                {
                    row["Transection_Type"] = transactionType;
                    row["Created_By"] = Program.CurrentUser;
                    row["LastModified_By"] = Program.CurrentUser;

                }
                SAVE_DOWORK_SUCCESS = false;
                //sqlResults = new string[4];

                SaleDAL saleDal = new SaleDAL();
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE,false,Program.BranchId,null,Program.CurrentUserID);
                SAVE_DOWORK_SUCCESS = true;
            }


            #endregion try
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SaleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SaleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SaleImport", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SaleImport", exMessage);
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
                FileLogger.Log(this.Name, "SaleImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SaleImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SaleImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SaleImport", exMessage);
            }
            return sqlResults;

            #endregion
        }

        public string[] ImportFromText()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";

            TransactionTypes();
            #endregion Close1

            string files = Program.ImportFileName;
            if (string.IsNullOrEmpty(files))
            {
                MessageBox.Show("Please select the right file for import");
                return sqlResults;
            }

            DataTable dtSaleM = new DataTable();
            DataTable dtSaleD = new DataTable();
            DataTable dtSaleE = new DataTable();

            #region Master table
            dtSaleM.Columns.Add("Identifier");
            dtSaleM.Columns.Add("ID");
            dtSaleM.Columns.Add("Customer_Code");
            dtSaleM.Columns.Add("Customer_Name");
            dtSaleM.Columns.Add("Delivery_Address");
            dtSaleM.Columns.Add("Vehicle_No");
            dtSaleM.Columns.Add("Invoice_Date_Time");
            dtSaleM.Columns.Add("Delivery_Date_Time");
            dtSaleM.Columns.Add("Reference_No");
            
            dtSaleM.Columns.Add("Sale_Type");
            dtSaleM.Columns.Add("Previous_Invoice_No");
            dtSaleM.Columns.Add("Is_Print");
            dtSaleM.Columns.Add("Tender_Id");
            dtSaleM.Columns.Add("Post");
            dtSaleM.Columns.Add("LC_Number");
            dtSaleM.Columns.Add("Currency_Code");
            dtSaleM.Columns.Add("Comments");
            dtSaleM.Columns.Add("Transection_Type").DefaultValue = transactionType;
            dtSaleM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
            dtSaleM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;



            #endregion Master table

            #region Details table
            dtSaleD.Columns.Add("Identifier");
            dtSaleD.Columns.Add("ID");
            dtSaleD.Columns.Add("Item_Code");
            dtSaleD.Columns.Add("Item_Name");
            dtSaleD.Columns.Add("Quantity");
            dtSaleD.Columns.Add("NBR_Price");
            dtSaleD.Columns.Add("UOM");
            dtSaleD.Columns.Add("VAT_Rate");
            dtSaleD.Columns.Add("SD_Rate");
            dtSaleD.Columns.Add("Non_Stock");
            dtSaleD.Columns.Add("Trading_MarkUp");
            dtSaleD.Columns.Add("Type");
            dtSaleD.Columns.Add("Discount_Amount");
            dtSaleD.Columns.Add("Promotional_Quantity");
            dtSaleD.Columns.Add("VAT_Name");
            dtSaleD.Columns.Add("Weight");
            
            #endregion Details table
            string[] fileNames = files.Split(";".ToCharArray());
            StreamReader sr = new StreamReader(fileNames[0]);
            try
            {
                #region Load Master Text file

                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper() == "M")
                            {
                                dtSaleM.Rows.Add(mItems);
                            }
                        }
                    }
                    else
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper() == "D")
                            {
                                dtSaleD.Rows.Add(dItems);
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
                }

                foreach (DataRow row in dtSaleM.Rows)
                {
                    row["Transection_Type"] = transactionType;
                    //row["Created_By"] = Program.CurrentUser;
                    //row["LastModified_By"] = Program.CurrentUser;

                }

                #endregion Load Master Text file
                #region Load Text file

                //string allData = sr.ReadToEnd();
                //string[] rows = allData.Split("\r".ToCharArray());
                //string delimeter = "|";
                //foreach (string r in rows)
                //{

                //    string[] items = r.Split(delimeter.ToCharArray());
                //    if (items[0].Replace("\n", "").ToUpper() == "M")
                //    {
                //        dtSaleM.Rows.Add(items);
                //    }
                //    else if (items[0].Replace("\n", "").ToUpper() == "D")
                //    {
                //        dtSaleD.Rows.Add(items);
                //    }
                //}
                //if (sr != null)
                //{
                //    sr.Close();
                //}
                #endregion Load Text file

                SAVE_DOWORK_SUCCESS = false;
                SaleDAL saleDal = new SaleDAL();
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally

            //catch (ArgumentNullException ex)
            //{
            //    string err = ex.Message.ToString();
            //    string[] error = err.Split(FieldDelimeter.ToCharArray());
            //    FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
            //    //if (error.Length > 1)
            //    //{
            //    //    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    //}
            //    //else
            //    //{
            //    //    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    //}
            //    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SaleImport", exMessage);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
            #endregion catch & finally

            return sqlResults;
        }
      
        private void btnTenderNew_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                //string result;

                dgvReceiveHistory.Top = dgvSale.Top;
                dgvReceiveHistory.Left = dgvSale.Left;
                dgvReceiveHistory.Height = dgvSale.Height;
                dgvReceiveHistory.Width = dgvSale.Width;

                dgvSale.Visible = false;
                dgvReceiveHistory.Visible = true;
               

                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormTenderSearch.SelectOne();

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    TenderId = selectedRow.Cells["TenderId"].Value.ToString();
                    txtSerialNo.Text = selectedRow.Cells["RefNo"].Value.ToString();
                    txtCustomerID.Text = selectedRow.Cells["CustomerId"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustName"].Value.ToString();
                    customerGId = selectedRow.Cells["GroupId"].Value.ToString();
                    customerGName = selectedRow.Cells["CustomerGroupName"].Value.ToString();
                    tenderDate = Convert.ToDateTime(selectedRow.Cells["TenderDate"].Value.ToString());
                }

                if (ImportByCustGrp == true)
                {
                    SearchCustomer();
                    cmbCustomerName.Focus();
                }
                else
                {
                    cmbCustomerName.Text = txtCustomerName.Text;
                    cmbCustomerName.Enabled = false;
                    btnSearchCustomer.Enabled = false;
                    cmbCustomerName_Leave(sender, e);
                }
                
                SearchTenderNewDetails();
#endregion
                #region Old
                
                
                //result = FormTenderSearch.SelectOne();

                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] Tenderinfo = result.Split(FieldDelimeter.ToCharArray());

                //    TenderId = Tenderinfo[0];
                //    txtSerialNo.Text = Tenderinfo[1];
                //    if (ImportByCustGrp == true)
                //    {
                //        customerGId = Tenderinfo[2];
                //        customerGName = Tenderinfo[10];
                //        SearchCustomer();
                //        cmbCustomerName.Focus();

                //    }
                //    else
                //    {
                //        txtCustomerID.Text = Tenderinfo[2];
                //        txtCustomerName.Text = Tenderinfo[3];
                //        cmbCustomerName.Text = Tenderinfo[3];
                //       //cmbCustomerName_Leave
                //        cmbCustomerName.Enabled = false;
                //        btnSearchCustomer.Enabled = false;
                //        cmbCustomerName_Leave(sender, e);
                //        //cmbVehicleNo.Focus();
                //    }

                //    //txtCustomerID.Text = Tenderinfo[2];
                //    //txtCustomerName.Text = Tenderinfo[3];
                //    //cmbCustomerName.Text = Tenderinfo[3];
                //    //txtDeliveryAddress1.Text = Tenderinfo[4];
                //    //txtDeliveryAddress2.Text = Tenderinfo[5];
                //    //txtDeliveryAddress3.Text = Tenderinfo[6];
                //    tenderDate =Convert.ToDateTime(Tenderinfo[7]);
                //    //dtpDeliveryDate
                //    //txtCustomerGroupName.Text = Tenderinfo[10];
                //    cmbCGroup.Text = Tenderinfo[10];
                    
                    

                //}
                
                //ProductSearchDsLoad();
                #endregion
                

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }

            #endregion
        }

        private void SearchCustomer()
        {
            try
            {
                 #region Customer
                CustomerResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();
                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName, tinNo, customerVReg, activeStatusCustomer, CGType };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName like", "c.TINNo like", "c.VATRegistrationNo like", "c.ActiveStatus like", "cg.GroupType like" };
                CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false);

                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                //customerGName, tinNo, customerVReg, activeStatusCustomer, CGType); // Change 04

                #endregion Customer
                #region Customer
                customerMini.Clear();
                cmbCustomerName.Items.Clear();
                foreach (DataRow item2 in CustomerResult.Rows)
                {
                    var customer = new CustomerSinmgleDTO();
                    customer.CustomerID = item2["CustomerID"].ToString();
                    customer.CustomerName = item2["CustomerName"].ToString();
                    customer.GroupType = item2["GroupType"].ToString();
                    customer.CustomerCode = item2["CustomerCode"].ToString();
                    customer.CustomerGroupID = item2["CustomerGroupID"].ToString();
                    customer.CustomerGroupName = item2["CustomerGroupName"].ToString();
                    customer.Address1 = item2["Address1"].ToString();
                    customer.Address2 = item2["Address2"].ToString();
                    customer.Address3 = item2["Address3"].ToString();
                    customer.City = item2["City"].ToString();
                    customer.TelephoneNo = item2["TelephoneNo"].ToString();
                    customer.FaxNo = item2["FaxNo"].ToString();
                    customer.Email = item2["Email"].ToString();
                    customer.TINNo = item2["TINNo"].ToString();
                    customer.VATRegistrationNo = item2["VATRegistrationNo"].ToString();
                    customer.ActiveStatus = item2["ActiveStatus"].ToString();
                    customer.Country = item2["Country"].ToString();
                    customerMini.Add(customer);

                }
                #region CGroup
                if (rbtnExport.Checked == false)
                {
                    if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                    {
                        cmbCGroup.Items.Clear();
                        var vcmbCGroup = customerMini.Where(x => x.GroupType == "Local").Select(x => x.CustomerGroupName).Distinct().ToList();

                        if (vcmbCGroup.Any())
                        {
                            cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                        }
                        //cmbCGroup.Items.Insert(0, "Select");
                        cmbCGroup.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (CustomerResult != null || CustomerResult.Rows.Count > 0)
                    {
                        cmbCGroup.Items.Clear();
                        var vcmbCGroup = customerMini.Where(x => x.GroupType == "Export").Select(x => x.CustomerGroupName).Distinct().ToList();

                        if (vcmbCGroup.Any())
                        {
                            cmbCGroup.Items.AddRange(vcmbCGroup.ToArray());
                        }
                        //cmbCGroup.Items.Insert(0, "Select");
                        //cmbCGroup.SelectedIndex = 0;
                    }
                }

                #endregion CGroup

                customerloadToCombo();
                #endregion Customer
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }

            #endregion
        }

        private void SearchTenderNewDetails()
        {
            try
            {
                #region Statement

                this.progressBar1.Visible = true;
                //this.Enabled = false;

                bgwTenderSearchNew.RunWorkerAsync();

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
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
                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchTenderDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchTenderDetails", exMessage);
            }

            #endregion

        }

        private void bgwTenderSearchNew_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                TenderDAL tenderDal = new TenderDAL();

                TenderSearchResult = new DataTable();

                //TenderSearchResult = tenderDal.SearchTenderDetailSaleNew(TenderId, dtpInvoiceDate.Value.ToString("yyyy/MMM/dd"));
                TenderSearchResult = tenderDal.SearchTenderDetailSaleNew(TenderId, dtpInvoiceDate.Value.ToString("yyyy/MMM/dd HH:mm:ss"));
                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwTenderSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwTenderSearchNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement


                #region Tender product load

                dgvReceiveHistory.Rows.Clear();
                int j = 0;
                foreach (DataRow item in TenderSearchResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceiveHistory.Rows.Add(NewRow);
                    //dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = item["SalesInvoiceNo"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = j + 1;

                    dgvReceiveHistory.Rows[j].Cells["ItemNoP"].Value = item["ItemNo"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["QuantityP"].Value = item["Quantity"].ToString();

                    dgvReceiveHistory.Rows[j].Cells["PromotionalQuantityP"].Value = item["PromotionalQuantity"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SaleQuantityP"].Value = item["SaleQuantity"].ToString();

                    dgvReceiveHistory.Rows[j].Cells["UnitPriceP"].Value = item["SalePrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["NBRPriceP"].Value = item["NBRPrice"].ToString();
                    //dgvReceiveHistory.Rows[j].Cells["NBRPWithVAT"].Value  = Convert.ToDecimal(item["NBRPrice"].ToString());
                    dgvReceiveHistory.Rows[j].Cells["UOMP"].Value = item["UOM"].ToString(); // SaleDetailFields[6].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATRateP"].Value = item["VATRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = item["VATAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = item["SubTotal"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CommentsP"].Value = item["Comments"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNameP"].Value = item["ProductName"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StockP"].Value = item["Stock"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SDP"].Value = item["SD"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = item["SDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StatusP"].Value = "New";
                    dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value = "0";
                    dgvReceiveHistory.Rows[j].Cells["TradingP"].Value = item["Trading"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["NonStockP"].Value = item["NonStock"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TradingMarkUpP"].Value = item["tradingMarkup"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TypeP"].Value = item["Type"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PCodeP"].Value = item["ProductCode"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMQtyP"].Value = item["UOMQty"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMnP"].Value = item["UOMn"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMcP"].Value = item["UOMc"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMPriceP"].Value = item["UOMPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DiscountAmountP"].Value = item["DiscountAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DiscountedNBRPriceP"].Value = item["DiscountedNBRPrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["DollerValueP"].Value = item["DollerValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["BDTValueP"].Value = item["CurrencyValue"].ToString();

                    if (item["BOMID"].ToString().Trim() == "")
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = "VAT 4.3 (Tender)";

                    }



                    dgvReceiveHistory.Rows[j].Cells["ChangeP"].Value = 0;
                    j++;
                }

                #endregion Tender product load



                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.btnOldID.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void chkSameVehicle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gbExport_Enter(object sender, EventArgs e)
        {

        }

        private void btnMultiple_Click(object sender, EventArgs e)
        {
            #region Transaction Type
            //DataGridViewRow selectedRow = null;
            TransactionTypes();
           //FormSaleVAT20Multiple.SelectOne(transactionType,null);

            #endregion Transaction Type
        }

        private void dtpConversionDate_Leave(object sender, EventArgs e)
        {
            string ccDate = dtpConversionDate.Value.ToString();
            if (CurrencyConversiondate == "1900-01-01 00:00:00")
            {
                ccDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            }
            else
            {
                ccDate = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");     
            }
            CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            string[] cValues = new string[] {cmbCurrency.Text.Trim(), "Y", ccDate };
            string[] cFields = new string[] { "cc.CurrencyCodeFrom like", "cc.ActiveStatus like", "cc.ConversionDate" };
            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, false);

            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(cmbCurrency.Text.Trim(), string.Empty, string.Empty,
            //     string.Empty, "Y", ccDate);

            CurrencyConversions.Clear();

            foreach (DataRow item2 in CurrencyConversionResult.Rows)
            {
                var cConversion = new CurrencyConversionVM();
                cConversion.CurrencyConversionId = item2["CurrencyConversionId"].ToString();
                cConversion.CurrencyCodeFrom = item2["CurrencyCodeFrom"].ToString();
                cConversion.CurrencyCodeF = item2["CurrencyCodeF"].ToString();
                cConversion.CurrencyNameF = item2["CurrencyNameF"].ToString();
                cConversion.CurrencyCodeTo = item2["CurrencyCodeTo"].ToString();
                cConversion.CurrencyCodeT = item2["CurrencyCodeT"].ToString();
                cConversion.CurrencyNameT = item2["CurrencyNameT"].ToString();
                cConversion.CurrencyRate = Convert.ToDecimal(item2["CurrencyRate"].ToString());
                cConversion.Comments = item2["Comments"].ToString();
                cConversion.ActiveStatus = item2["ActiveStatus"].ToString();
                cConversion.ConvertionDate = item2["ConversionDate"].ToString();
                CurrencyConversions.Add(cConversion);

            }
            if (CurrencyConversionResult != null && CurrencyConversionResult.Rows.Count > 0)
            {

                cmbCurrency.Items.Clear();
                var cmbSCurrencyCodeF = CurrencyConversions.Select(x => x.CurrencyCodeF).Distinct().ToList();

                if (cmbSCurrencyCodeF.Any())
                {
                    cmbCurrency.Items.AddRange(cmbSCurrencyCodeF.ToArray());
                }

                cmbCurrency.Text = cmbSCurrencyCodeF[0].ToString();
                //string cDate =
                //    CurrencyConversions.Where(x => x.CurrencyCodeF == "USD").Select(x => x.ConvertionDate).
                //        FirstOrDefault();

                //dtpConversionDate.Value = Convert.ToDateTime(cDate);
            }
            if (rbtnExport.Checked == false)
            {
                cmbCurrency.Items.Clear();
                cmbCurrency.Items.Insert(0, "BDT");
                cmbCurrency.Text = "BDT";
            }
            else
            {dtpConversionDate.Value = Convert.ToDateTime(ccDate);}

            CurrencyValue();
            
        }

        private void dtpConversionDate_ValueChanged(object sender, EventArgs e)
        {
            
            ChangeData = true;
        }

        private void dtpConversionDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");

            }
        }

        private void btnDL_Click(object sender, EventArgs e)
        {
            try
            {
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }

                this.progressBar1.Visible = true;
                #region Generate DCNo
                SaleDAL DeliveryChallan = new SaleDAL();
                DeliveryChallan.SetDeliveryChallanNo(txtSalesInvoiceNo.Text.Trim(), dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                #endregion Generate DCNo

                #region Delivery Report

                ReportResultDelivry = new DataSet();
                ReportResultDelivry = reportDsdal.RptDeliveryReport(txtSalesInvoiceNo.Text.Trim());

                #region Statement

                // Start Complete
                ReportResultDelivry.Tables[0].TableName = "DsDelivery";
                RptDeliveryReport objrpt = new RptDeliveryReport();
                objrpt.SetDataSource(ReportResultDelivry);

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'DELEVERY CHALLAN'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                // End Complete

                #endregion

                #endregion  Delivery Report

               
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnDL_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDL_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnDL_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnDL_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnDL_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDL_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDL_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnDL_Click", exMessage);
            }

            #endregion

            finally
            {
               
                this.progressBar1.Visible = false;
            }
        }

        private void btnGPR_Click(object sender, EventArgs e)
        {
            try
            {
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }

                this.progressBar1.Visible = true;


                #region GPR Report

                ReportResultDelivry = new DataSet();
                ReportResultDelivry = reportDsdal.RptDeliveryReport(txtSalesInvoiceNo.Text.Trim());

                #region Statement

                // Start Complete
                ReportResultDelivry.Tables[0].TableName = "DsDelivery";
                RptGatePass objrpt = new RptGatePass();
                objrpt.SetDataSource(ReportResultDelivry);

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Gate Pass'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                // End Complete

                #endregion

                #endregion  GPR Report


                #region Update Info
                SaleDAL DeliveryChallan = new SaleDAL();
                DeliveryChallan.SetGatePass(txtSalesInvoiceNo.Text.Trim());
                #endregion Update Info

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
            }

            #endregion


            finally
            {

                this.progressBar1.Visible = false;
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            #region Try


            try
            {
                trackingsCmb.Clear();

                if (dgvSale.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvSale.Rows.Count; i++)
                    {
                        var trackingCmb = new TrackingCmbDTO();
                        trackingCmb.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();  //FinishItemNo
                        trackingCmb.ProductCode = dgvSale.Rows[i].Cells["PCode"].Value.ToString(); //FinishItemCode
                        trackingCmb.ProductName = dgvSale.Rows[i].Cells["ItemName"].Value.ToString(); //FinishItemName
                        trackingCmb.VatName = dgvSale.Rows[i].Cells["VATName"].Value.ToString();
                        //trackingCmb.BOMId = dgvSale.Rows[i].Cells["BOMId"].Value.ToString();
                        trackingCmb.Qty = dgvSale.Rows[i].Cells["SaleQuantity"].Value.ToString();
                        trackingCmb.EffectiveDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                        if (rbtnDN.Checked || rbtnCN.Checked)
                        {
                            trackingCmb.SaleInvoiceNo = txtOldID.Text;
                        }
                        else if(IsUpdate == true)
                        {
                            trackingCmb.SaleInvoiceNo = txtSalesInvoiceNo.Text;
                        }

                        trackingsCmb.Add(trackingCmb);
                    }

                    trackingsVm.Clear();

                    Program.fromOpen = "Me";
                    if (rbtnDN.Checked || rbtnCN.Checked)
                    {
                        trackingsVm = FormTracking.SelectOne(trackingsCmb, "Sale_Return");
                    }
                    else
                    {
                        trackingsVm = FormTracking.SelectOne(trackingsCmb, "Sale");
                    }

                }
            }
            #endregion
            #region Catch


            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }
            #endregion
        }

        private void InsertTrackingInfo()
        {
            #region Try
            try
            {
                trackingsVm.Clear();
                TrackingDAL trackingDal = new TrackingDAL();
                DataTable trackingInfoDt = new DataTable();
                //trackingInfoDt = trackingDal.SearchTrackingItems("", "N", "Y", "Y", "", "", "", SaleDetailData);
                string saleNo = "";
                if (rbtnDN.Checked || rbtnCN.Checked)
                {
                    saleNo = txtOldID.Text;
                }
                else
                {
                    saleNo = SaleDetailData;
                }

                trackingInfoDt = trackingDal.SearchExistingTrackingItems("", "", "Y", saleNo);
                if (trackingInfoDt.Rows.Count > 0)
                {
                    for (int i = 0; i < trackingInfoDt.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = trackingInfoDt.Rows[i]["ItemNo"].ToString();
                        trackingVm.Heading1 = trackingInfoDt.Rows[i]["Heading1"].ToString();
                        trackingVm.IsReceive = trackingInfoDt.Rows[i]["IsReceive"].ToString();
                        trackingVm.ReceiveNo = trackingInfoDt.Rows[i]["ReceiveNo"].ToString();
                        trackingVm.FinishItemNo = trackingInfoDt.Rows[i]["FinishItemNo"].ToString();
                        trackingVm.IsIssue = trackingInfoDt.Rows[i]["IsIssue"].ToString();
                        trackingVm.IsSale = trackingInfoDt.Rows[i]["IsSale"].ToString();
                        trackingVm.SaleInvoiceNo = trackingInfoDt.Rows[i]["SaleInvoiceNo"].ToString();



                        trackingsVm.Add(trackingVm);
                    }
                }
            }

            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnGPR_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnGPR_Click", exMessage);
            }

            #endregion
        }

        private void dgvSale_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbProduct_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void chkName_CheckedChanged(object sender, EventArgs e)
        {
            if (chkName.Checked)
            {
                cmbProduct.Visible = true;
                cmbProductCode.Visible = false;
                label5.Text = "Name";
            }
            else
            {
                cmbProduct.Visible =false;
                cmbProductCode.Visible = true;
                label5.Text = "Code";

            }
        }

        
        
    }
}