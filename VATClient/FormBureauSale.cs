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
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using SymphonySofttech.Reports.Report.Rpt63;
using SymphonySofttech.Reports.Report.Rpt63V12V2;
using SymphonySofttech.Reports.Report.V12V2;

namespace VATClient
{
    public partial class FormBureauSale : Form
    {
        #region Constructors

        public FormBureauSale()
        {
            InitializeComponent();
            //dgvSale.DefaultCellStyle.Font = new Font("SutonnyMJ", 12, FontStyle.Italic);
            connVM = Program.OrdinaryLoad();

            // DefaultCellStyle.Font.Name= "SutonnyMJ";

            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private List<UomDTO> UOMs = new List<UomDTO>();
        private List<CurrencyConversionVM> CurrencyConversions = new List<CurrencyConversionVM>();

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private bool PrepaidVAT;
        private bool VAT11Letter;
        private bool VAT11A4;
        private bool VAT11Legal;

        private bool LocalInVAT1;
        private bool LocalInVAT1KaTarrif;
        private bool TenderInVAT1;
        private bool TenderInVAT1Tender;
        private bool TenderPriceWithVAT;
        string VAT11Name = "";


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
        private List<BureauSaleDetailVM> saleDetails = new List<BureauSaleDetailVM>();
        private List<SaleExportVM> saleExport = new List<SaleExportVM>();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private int SalePlaceQty;
        private int SalePlaceTaka;
        private int SalePlaceDollar;
        private int SalePlaceRate;
        private string transactionType = string.Empty;
        private string transactionTypeOld = string.Empty;
        private string vSalePlaceQty, vSalePlaceTaka, vSalePlaceDollar, vSalePlaceRate, vItemNature,
                   vPrepaidVAT, vNumberofItems, vVAT11Letter, vVAT11A4, vVAT11Legal,
                   vLocalInVAT1, vLocalInVAT1KaTarrif, vTenderInVAT1, vTenderInVAT1Tender, vIs3Plyer, vTenderPriceWithVAT,
                    vCreditWithoutTransaction = string.Empty;

        private bool CreditWithoutTransaction;
        private List<BomOhDTO> BomOHs = new List<BomOhDTO>();

        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();

        DateTime invoiceDateTimeBureau = DateTime.Now;
        private string PrintLocation = String.Empty;
        private string vPrintCopy = String.Empty;
        private int PrintCopy = 1;
        private string WantToPrint = "N";
        private int AlReadyPrintNo;
        private int NewPrintCopy;
        private string Is3Plyer = string.Empty;
        private string VPrinterName = string.Empty;
        private string CurrencyConversiondate = string.Empty;
        private string ProductCode = string.Empty;
        private string ProductName = string.Empty;
        private string SearchBranchId = "0";

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



        #endregion variable

        #region Global Variables For BackGroundWorker

        private BureauSaleDAL saleDal = new BureauSaleDAL();
        private DataSet StatusResult;
        private DateTime varInvoiceDate;
        private DataSet ReportResultVAT11;
        private DataSet ReportResultVAT20;
        private DataSet ReportResultCreditNote;
        private DataSet ReportResultDebitNote;

        private string varSalesInvoiceNo = string.Empty;
        private string varTrading = string.Empty;
        private string post1, post2;
        private bool Add = false;
        private bool Edit = false;
        private string ReturnTransType = string.Empty;


        #endregion

        //public string VFIN = "203";

        #endregion
        DataTable dtSale = new DataTable();
        private void FormBureauSale_Load(object sender, EventArgs e)
        {
            try
            {

                #region ToolTip

                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchInvoiceNo, "Existing information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchCustomer, "Customer");
                ToolTip1.SetToolTip(this.btnProductType, "Product Type");

                ToolTip1.SetToolTip(this.btnOldID, "Orginal Sales invoice no");
                ToolTip1.SetToolTip(this.btnTesting, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");

                #endregion

                ClearAllFields();

                #region Form Setup

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

                #endregion

                #region CPC Load
                string[] Condition = new string[] { "Type='Sale'" };
                cmbCPCName = new CommonDAL().ComboBoxLoad(cmbCPCName, "CPCDetails", "Code", "Name", Condition, "varchar", false, false, connVM, true);
                #endregion


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
                FileLogger.Log(this.Name, "FormBureauSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormBureauSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormBureauSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormBureauSale_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormBureauSale_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBureauSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBureauSale_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormBureauSale_Load", exMessage);
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

            #region SetupVATStatus
            varInvoiceDate = Convert.ToDateTime(dtpChallanDate.MaxDate.ToString("yyyy-MMM-dd"));

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
            else if (rbtnCN.Checked) // start other
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Finish";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;

            }

            else if (rbtnServiceNS.Checked) //start
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Service";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;



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
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                    ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam,connVM);

                #endregion Product

                #region Customer
                CustomerResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();
                //ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName, tinNo, customerVReg, activeStatusCustomer, CGType, "All" };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName like", "c.TINNo like", "c.VATRegistrationNo like", "c.ActiveStatus like", "cg.GroupType like", "SelectTop" };
                CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false,connVM);

                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                //customerGName, tinNo, customerVReg, activeStatusCustomer, CGType); // Change 04

                #endregion Customer

                #region SetupVATStatus
                SetupDAL setupDal = new SetupDAL();
                //ISetup setupDal = OrdinaryVATDesktop.GetObject<SetupDAL, SetupRepo, ISetup>(OrdinaryVATDesktop.IsWCF);

                StatusResult = setupDal.ResultVATStatus(varInvoiceDate, Program.DatabaseName,connVM);

                #endregion SetupVATStatus

                #region UOM
                UOMDAL uomdal = new UOMDAL();
                //IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false,connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

                #region CurrencyConversion


                CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
                //ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);

                string[] cValue = new string[] { "Y", dtpConversionDate.Value.ToString("dd/MMM/yyyy HH:mm:ss") };
                string[] cField = new string[] { "cc.ActiveStatus like", "cc.ConversionDate" };
                //CurrencyConversionResult = currencyConversionDal.SelectAll(0, cField, cValue, null, null, false);

                CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
                     string.Empty, "Y", dtpConversionDate.Value.ToString("dd/MMM/yyyy HH:mm:ss"),connVM);



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



                #endregion CGroup

                customerloadToCombo();
                #endregion Customer


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





                #endregion CurrencyConversion

                #endregion region

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
                if (chkSameCustomer.Checked == false)
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
                //IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

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

                vSalePlaceQty = commonDal.settingsDesktop("Sale", "QuantityDecimalPlace",null,connVM);
                vSalePlaceTaka = commonDal.settingsDesktop("Sale", "TakaDecimalPlace",null,connVM);
                vSalePlaceDollar = commonDal.settingsDesktop("Sale", "DollerDecimalPlace",null,connVM);
                vSalePlaceRate = commonDal.settingsDesktop("Sale", "RateDecimalPlace",null,connVM);
                vPrepaidVAT = commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT",null,connVM);
                vNumberofItems = commonDal.settingsDesktop("Sale", "NumberOfItems",null,connVM);
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature",null,connVM);
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter",null,connVM);
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4",null,connVM);
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal",null,connVM);
                //PrintLocation = commonDal.settingsDesktop("Sale", "ReportSaveLocation");
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3",null,connVM);

                vLocalInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3",null,connVM);
                vLocalInVAT1KaTarrif = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3Ka(Tarrif)",null,connVM);
                vTenderInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3",null,connVM);
                vTenderInVAT1Tender = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3(Tender)",null,connVM);
                vIs3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer",null,connVM);
                //for tender with vat
                vTenderPriceWithVAT = commonDal.settingsDesktop("PriceDeclaration", "TenderPriceWithVAT",null,connVM);
                vCreditWithoutTransaction = commonDal.settingsDesktop("Sale", "CreditWithoutTransaction",null,connVM);


                if (string.IsNullOrEmpty(vSalePlaceQty)
                    || string.IsNullOrEmpty(vSalePlaceTaka)
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
                    || string.IsNullOrEmpty(vCreditWithoutTransaction)

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
                VAT11Letter = Convert.ToBoolean(vVAT11Letter == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(vVAT11A4 == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(vVAT11Legal == "Y" ? true : false);

                LocalInVAT1 = Convert.ToBoolean(vLocalInVAT1 == "Y" ? true : false);
                LocalInVAT1KaTarrif = Convert.ToBoolean(vLocalInVAT1KaTarrif == "Y" ? true : false);
                TenderInVAT1 = Convert.ToBoolean(vTenderInVAT1 == "Y" ? true : false);
                TenderInVAT1Tender = Convert.ToBoolean(vTenderInVAT1Tender == "Y" ? true : false);
                Is3Plyer = vIs3Plyer;
                TenderPriceWithVAT = Convert.ToBoolean(vTenderPriceWithVAT == "Y" ? true : false);
                CreditWithoutTransaction = Convert.ToBoolean(vCreditWithoutTransaction == "Y" ? true : false);


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




                #endregion Settings
                #region Transaction Type

                #region Close
                this.Text = "Local Sales";
                btnPrint.Text = "11";
                label22.Visible = true;

                btn20.Visible = false;
                btnDebitCredit.Top = btnPrint.Top;
                btnDebitCredit.Left = btnPrint.Left;

                txtLineNo.Visible = false;
                label18.Visible = false;
                txtTenderStock.Visible = false;

                btnSearchCustomer.Enabled = true;
                btnProductType.Enabled = true;
                label1.Visible = false;
                btnDebitCredit.Visible = false;
                chkIs11.Visible = false;
                chkIsBlank.Visible = false;
                txtQuantity.ReadOnly = false;
                txtQuantity.Text = "0";
                ChkExpLoc.Visible = false;
                btnFormKaT.Visible = false;
                #endregion Close

                #region  rbtnCN

                if (rbtnCN.Checked)
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
                    if (CreditWithoutTransaction == true)
                    {
                        txtOldID.ReadOnly = false;
                        btnAdd.Enabled = true;
                        btnProductType.Enabled = true;
                    }
                    else
                    {
                        btnAdd.Enabled = false;
                        btnProductType.Enabled = false;
                    }
                    this.Text = "Sales Credit Note Entry";

                }
                #endregion  rbtnCN

                #region  rbtnServiceNS


                else if (rbtnServiceNS.Checked)
                {

                    //ChkExpLoc.Visible = true;
                    cmbType.Text = "New";

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

                    this.Text = "Service Sales Entry(Non Stock)";
                    btnPrint.Text = "VAT 6.3";
                    chkIs11.Visible = true;
                    chkIs11.Checked = true;
                    cmbVAT1Name.Text = "Service";
                    label1.Visible = true;
                    txtQuantity.ReadOnly = true;
                    txtQuantity.Text = "1";
                }

                #endregion  rbtnService



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

                varInvoiceDate = Convert.ToDateTime(dtpChallanDate.MaxDate.ToString("yyyy-MMM-dd"));


                //this.Enabled = false;


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


        private void bgwCustomer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement




                // Start DoWork
                CustomerResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();
                //ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);


                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                // customerGName, tinNo, customerVReg, activeStatusCustomer, CGType);
                // Change 04

                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName, tinNo, customerVReg, activeStatusCustomer, CGType };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName", "c.TINNo like", "c.VATRegistrationNo like", "c.ActiveStatus like", "cg.GroupType like" };
                CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false,connVM);

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

                //UomsValue();


                #region Null Ckeck



                if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtNBRPrice.Focus();
                    return;
                }

                if (txtInvoiceNo.Text == "")
                {
                    MessageBox.Show("Please enter invoice no.");

                    return;
                }


                //else if (txtQuantity.Text == "")
                //{
                //    txtQuantity.Text = "1";
                //}


                else if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }

                //else if (txtUnitCost.Text == "")
                //{
                //    txtUnitCost.Text = "0.00";
                //}

                else if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                else if (txtSD.Text == "")
                {
                    txtSD.Text = "0";
                }



                if (Convert.ToDecimal(txtQty1.Text) <= 0)
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



                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                #endregion Null Ckeck

                #region HsCode Multiple Check Msg
                var HSCode = new ProductDAL().SearchHSCode(new[] { "ItemNo" }, new[] { txtProductCode.Text });
                if (HSCode.Rows.Count > 1)
                {
                    MessageBox.Show(MessageVM.MsgMultipleHscode, "HSCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                #endregion




                if (cmbVATType.Text != "VAT")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }

                ChangeData = true;
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvSale.Rows.Add(NewRow);




                dgvSale["ItemNo", dgvSale.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvSale["InvoiceNo", dgvSale.RowCount - 1].Value = txtInvoiceNo.Text.Trim();
                dgvSale["InvoiceDate", dgvSale.RowCount - 1].Value = dtpInvoiceDate.Text.Trim();
                dgvSale["UOM", dgvSale.RowCount - 1].Value = cmbUom.Text.Trim(); // txtUOM.Text.Trim();

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    //txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), SalePlaceTaka).ToString();
                    txtNBRPrice.Text = txtNBRPrice.Text.Trim();

                if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                    txtVATRate.Text = txtVATRate.Text.Trim();
                if (Program.CheckingNumericTextBox(txtSD, "txtSD") == true)
                    txtSD.Text = txtSD.Text.Trim();

                if (Program.CheckingNumericTextBox(txtQty1, "txtQty1") == true)
                    txtQty1.Text = txtQty1.Text.Trim();

                dgvSale["SaleQuantity", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQty1.Text.Trim())).ToString();

                dgvSale["NBRPrice", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtNBRPrice.Text.Trim()).ToString());
                dgvSale["VATRate", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATRate.Text.Trim()).ToString());
                dgvSale["SD", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtSD.Text.Trim())).ToString());
                dgvSale["DollerValue", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtNBRPrice.Text.Trim()).ToString());





                dgvSale["Status", dgvSale.RowCount - 1].Value = "New";
                dgvSale["Stock", dgvSale.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString());
                dgvSale["Previous", dgvSale.RowCount - 1].Value = 0; // txtQuantity.Text.Trim();
                dgvSale["Change", dgvSale.RowCount - 1].Value = 0;


                dgvSale["NonStock", dgvSale.RowCount - 1].Value = Convert.ToString(rbtnServiceNS.Checked ? "Y" : "N");
                dgvSale["Type", dgvSale.RowCount - 1].Value = cmbVATType.Text.Trim();
                dgvSale["CPCName", dgvSale.RowCount - 1].Value = cmbCPCName.Text.ToLower() == "select" ? "-" : cmbCPCName.Text;
                dgvSale["HSCode", dgvSale.RowCount - 1].Value = txtHSCode.Text.Trim();
                dgvSale["CConvDate", dgvSale.RowCount - 1].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");


                Rowcalculate();

                selectLastRow();




                txtInvoiceNo.Text = "";
                dtpInvoiceDate.Value = DateTime.Now;
                //txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "0";
                txtQty1.Text = "1";
                //txtVATRate.Text = "0.00";
                //txtUOM.Text = "";
                txtQuantityInHand.Text = "0.00";
                txtNBRPrice.Text = "0.00";
                //cmbUom.SelectedIndex = -1;
                TransactionTypes();

                //cmbCurrency.Enabled = false;
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

        //private void CalculateSelectedExportRow()
        //{
        //    try
        //    {
        //        decimal NBR = 0;
        //        decimal Quantity = 0;
        //        decimal SumTotalS = 0;
        //        decimal Trading = 0;
        //        decimal TradingAmount = 0;
        //        decimal SD = 0;
        //        decimal SDAmount = 0;
        //        decimal VATRate = 0;
        //        decimal VATAmount = 0;
        //        decimal bDTValue = 0;
        //        decimal dollerValue = 0;

        //        decimal CurrencyRateFromBDT = 0;
        //        decimal dollerRate = 0;
        //        decimal NBRPrice = 0;

        //        decimal Total = 0;

        //        //if (rbtnExport.Checked || rbtnExportPackage.Checked)
        //        //{
        //            if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" ||
        //                string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
        //            {
        //                MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
        //                return;
        //            }
        //            //else
        //            //{
        //            //    dollerRate = Convert.ToDecimal(txtDollerRate.Text);
        //            //    CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text);
        //            //}
        //        //}

        //        //else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
        //        //{
        //        //    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
        //        //    return;
        //        //}
        //        else
        //        {
        //            CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
        //            dollerRate = Convert.ToDecimal(txtDollerRate.Text.Trim());

        //        }



        //        if (Program.CheckingNumericString(dollerRate.ToString(), "dollerRate") == true)
        //            //dollerRate = Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));
        //        dollerRate = Convert.ToDecimal(dollerRate.ToString());

        //        if (Program.CheckingNumericString(CurrencyRateFromBDT.ToString(), "CurrencyRateFromBDT") == true)
        //            CurrencyRateFromBDT =
        //                //Convert.ToDecimal(Program.FormatingNumeric(CurrencyRateFromBDT.ToString(), SalePlaceDollar));
        //        Convert.ToDecimal(CurrencyRateFromBDT.ToString());

        //            int i= dgvSale.CurrentRow.Index;
        //            SD = Convert.ToDecimal(dgvSale["SD", i].Value);
        //            VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);
        //            Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);

        //            #region Tender Price with VAT



        //                #endregion Tender Price with VAT

        //                Quantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
        //                NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);

        //                if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
        //                    Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
        //                //if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
        //                //    NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

        //                SumTotalS = Quantity*NBR;

        //                if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
        //                    SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));


        //                bDTValue = CurrencyRateFromBDT*SumTotalS;
        //                if (Program.CheckingNumericString(bDTValue.ToString(), "bDTValue") == true)
        //                    bDTValue = Convert.ToDecimal(Program.FormatingNumeric(bDTValue.ToString(), SalePlaceDollar));

        //                dgvSale["BDTValue", i].Value = Convert.ToDecimal(bDTValue);
        //                dgvSale["DollerValue", i].Value = 0;
        //                if (rbtnExport.Checked)
        //                {
        //                    if (dollerRate == 0)
        //                    {
        //                        MessageBox.Show(
        //                            "Please select the Currency or check the currency rate from BDT and USD", this.Text);
        //                        return;
        //                    }
        //                    dollerValue = bDTValue/dollerRate;
        //                    if (Program.CheckingNumericString(dollerValue.ToString(), "dollerValue") == true)
        //                        dollerValue =
        //                            Convert.ToDecimal(Program.FormatingNumeric(dollerValue.ToString(), SalePlaceDollar));

        //                    dgvSale["DollerValue", i].Value = Convert.ToDecimal(dollerValue);
        //                }

        //                if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
        //                    SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
        //                if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
        //                    VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));
        //                if (Program.CheckingNumericString(Trading.ToString(), "Trading") == true)
        //                    Trading = Convert.ToDecimal(Program.FormatingNumeric(Trading.ToString(), SalePlaceRate));

        //                TradingAmount = SumTotalS*Trading/100;
        //                SDAmount = (SumTotalS + TradingAmount)*SD/100;
        //                VATAmount = (SumTotalS + TradingAmount + SDAmount)*VATRate/100;
        //                if (Program.CheckingNumericString(TradingAmount.ToString(), "TradingAmount") == true)
        //                    TradingAmount =
        //                        Convert.ToDecimal(Program.FormatingNumeric(TradingAmount.ToString(), SalePlaceTaka));
        //                if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
        //                    SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), SalePlaceTaka));
        //                if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
        //                    VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), SalePlaceTaka));


        //                Total = SumTotalS + TradingAmount + SDAmount + VATAmount;



        //                if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
        //                    Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), SalePlaceTaka));

        //                dgvSale[0, i].Value = i + 1;

        //                dgvSale["VATAmount", i].Value = Convert.ToDecimal(VATAmount).ToString(); //"0,0.00");
        //                dgvSale["SDAmount", i].Value = Convert.ToDecimal(SDAmount).ToString(); //"0,0.00");
        //                dgvSale["SubTotal", i].Value = Convert.ToDecimal(SumTotalS).ToString();
        //                dgvSale["Total", i].Value = Convert.ToDecimal(Total).ToString(); //"0,0.00");

        //                dgvSale["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvSale["Quantity", i].Value)
        //                                                               - Convert.ToDecimal(dgvSale["Previous", i].Value)).ToString();


        //    }
        //    #region catch
        //    catch
        //        (ArgumentNullException
        //        ex)
        //    {
        //        string err = ex.ToString();
        //        string[] error = err.Split(FieldDelimeter.ToCharArray());
        //        FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch
        //        (IndexOutOfRangeException
        //        ex)
        //    {
        //        FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch
        //        (NullReferenceException
        //        ex)
        //    {
        //        FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch
        //        (FormatException
        //        ex)
        //    {

        //        FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }

        //    catch
        //        (SoapHeaderException
        //        ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }

        //        FileLogger.Log(this.Name, "Rowcalculate", exMessage);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch
        //        (SoapException
        //        ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "Rowcalculate", exMessage);
        //    }
        //    catch
        //        (EndpointNotFoundException
        //        ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch
        //        (WebException
        //        ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch
        //        (Exception
        //        ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "Rowcalculate", exMessage);
        //    }

        //    #endregion
        //}
        private void GTotal()
        {
            decimal SumSubTotal = 0;

            decimal SumVATAmount = 0;
            decimal SumGTotal = 0;

            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {

                SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);


            }

            //GrandTotal = SumSubTotal + SumTrading + SumSDTotal + SumVATAmount;
            //txtTotalAmount.Text = Convert.ToDecimal(GrandTotal).ToString(); //"0,0.00");
            txtTotalSubTotal.Text = Convert.ToDecimal(SumSubTotal).ToString(); //"0,0.00");

            txtTotalVATAmount.Text = Convert.ToDecimal(SumVATAmount).ToString(); //"0,0.00");
            txtTotalAmount.Text = Convert.ToDecimal(SumGTotal).ToString(); //"0,0.00");

        }


        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSale.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtNBRPrice.Focus();
                    return;
                }
                //if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                //{
                //    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}
                if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" || string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }


                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }


                ChangeData = true;



                if (cmbVATType.Text != "VAT")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }


                #region HsCode Multiple Check Msg
                var HSCode = new ProductDAL().SearchHSCode(new[] { "ItemNo" }, new[] { txtProductCode.Text });
                if (HSCode.Rows.Count > 1)
                {
                    MessageBox.Show(MessageVM.MsgMultipleHscode, "HSCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                #endregion

                #region Decimal place
                //if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                //    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), SalePlaceTaka).ToString();
                //if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                //    txtVATRate.Text = Program.FormatingNumeric(txtVATRate.Text.Trim(), SalePlaceRate).ToString();
                //if (Program.CheckingNumericTextBox(txtSD, "txtSD") == true)
                //    txtSD.Text = Program.FormatingNumeric(txtSD.Text.Trim(), SalePlaceRate).ToString();

                //if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), SalePlaceQty).ToString();
                #endregion



                if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                    txtVATRate.Text = txtVATRate.Text.Trim();
                if (Program.CheckingNumericTextBox(txtSD, "txtSD") == true)
                    txtSD.Text = txtSD.Text.Trim();
                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = txtNBRPrice.Text.Trim();
                if (Program.CheckingNumericTextBox(txtQty1, "txtQty1") == true)
                    txtQty1.Text = txtQty1.Text.Trim();


                dgvSale["SD", dgvSale.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtSD.Text.Trim())).ToString());//"0.00");
                dgvSale["VATRate", dgvSale.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtVATRate.Text.Trim()).ToString());//"0.00");

                dgvSale["UOM", dgvSale.CurrentRow.Index].Value = cmbUom.Text.Trim(); // txtUOM.Text.Trim();

                dgvSale["SaleQuantity", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQty1.Text.Trim()).ToString());//"0,0.0000");


                dgvSale["NBRPrice", dgvSale.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtNBRPrice.Text.Trim())).ToString());

                if (dgvSale.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvSale["Status", dgvSale.CurrentRow.Index].Value = "Change";
                }
                dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvSale["Type", dgvSale.CurrentRow.Index].Value = cmbVATType.Text.Trim();
                dgvSale["NonStock", dgvSale.CurrentRow.Index].Value = Convert.ToString(rbtnServiceNS.Checked ? "Y" : "N");
                dgvSale["CConvDate", dgvSale.CurrentRow.Index].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                dgvSale["CPCName", dgvSale.RowCount - 1].Value = cmbCPCName.Text.ToLower() == "select" ? "-" : cmbCPCName.Text;
                dgvSale["HSCode", dgvSale.RowCount - 1].Value = txtHSCode.Text.Trim();

                dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);


                Rowcalculate();


                txtInvoiceNo.Text = "";
                dtpInvoiceDate.Value = DateTime.Now;

                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "0";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0.00";
                //cmbVAT1Name.SelectedIndex = 0;


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
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nInvoice No: " + dgvSale.CurrentRow.Cells["InvoiceNo"].Value, this.Text, MessageBoxButtons.YesNo,
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
                //if (txtUomConv.Text == "0"
                //|| string.IsNullOrEmpty(txtUOM.Text)
                //|| string.IsNullOrEmpty(txtUomConv.Text)
                //|| string.IsNullOrEmpty(txtProductCode.Text)
                //|| string.IsNullOrEmpty(txtQuantity.Text))
                //{
                //    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text,
                //                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}

            }
            else
            {
                MessageBox.Show("No Items Found in Details Section.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            ChangeData = true;

            dgvSale.CurrentRow.Cells["Status"].Value = "Delete";


            dgvSale.CurrentRow.Cells["Quantity"].Value = 0.00;
            dgvSale.CurrentRow.Cells["SaleQuantity"].Value = 0.00;

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

            dgvSale.CurrentRow.Cells["BDTValue"].Value = 0.00;
            dgvSale.CurrentRow.Cells["DollerValue"].Value = 0.00;


            dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
            dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
            //Rowcalculate();
            GTotal();
            txtProductCode.Text = "";
            txtProductName.Text = "";

        }

        private void Rowcalculate()
        {
            try
            {
                decimal NBR = 0;
                decimal Quantity = 0;
                //decimal UomC = 0;
                decimal SumTotalS = 0;

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
                //decimal NBRPrice = 0;


                //decimal Cost = 0;
                decimal Total = 0;


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



                //if (Program.CheckingNumericString(dollerRate.ToString(), "dollerRate") == true)
                //    //dollerRate = Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));
                //dollerRate = Convert.ToDecimal(dollerRate.ToString());

                //if (Program.CheckingNumericString(CurrencyRateFromBDT.ToString(), "CurrencyRateFromBDT") == true)
                //    CurrencyRateFromBDT =
                //        //Convert.ToDecimal(Program.FormatingNumeric(CurrencyRateFromBDT.ToString(), SalePlaceDollar));
                //Convert.ToDecimal(CurrencyRateFromBDT.ToString());

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SD = Convert.ToDecimal(dgvSale["SD", i].Value);
                    VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);


                    Quantity = Convert.ToDecimal(dgvSale["SaleQuantity", i].Value);
                    //NBR = Convert.ToDecimal(dgvSale["DollerValue", i].Value);
                    NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);

                    //if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                    //    Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
                    //if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
                    //    NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

                    dollerValue = Quantity * NBR;

                    //if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
                    //    //SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));

                    bDTValue = CurrencyRateFromBDT * dollerValue;
                    dgvSale["BDTValue", i].Value = Convert.ToDecimal(bDTValue);
                    dgvSale["DollerValue", i].Value = dollerValue;
                    dgvSale["NBRPrice", i].Value = bDTValue;
                    SumTotalS = bDTValue;


                    if (dollerRate == 0)
                    {
                        MessageBox.Show(
                            "Please select the Currency or check the currency rate from BDT and USD", this.Text);
                        return;
                    }
                    //dollerValue = bDTValue/dollerRate;
                    //if (Program.CheckingNumericString(dollerValue.ToString(), "dollerValue") == true)
                    //    dollerValue =
                    //        Convert.ToDecimal(Program.FormatingNumeric(dollerValue.ToString(), SalePlaceDollar));

                    //dgvSale["DollerValue", i].Value = Convert.ToDecimal(dollerValue);


                    //if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                    //    SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
                    //if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
                    //    VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));

                    SDAmount = (SumTotalS) * SD / 100;
                    VATAmount = (SumTotalS + SDAmount) * VATRate / 100;

                    //if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                    //    SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), SalePlaceTaka));
                    //if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                    //    VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), SalePlaceTaka));


                    Total = SumTotalS + SDAmount + VATAmount;



                    //if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                    //    Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), SalePlaceTaka));

                    dgvSale[0, i].Value = i + 1;

                    dgvSale["VATAmount", i].Value = Convert.ToDecimal(VATAmount).ToString(); //"0,0.00");
                    dgvSale["SDAmount", i].Value = Convert.ToDecimal(SDAmount).ToString(); //"0,0.00");
                    dgvSale["SubTotal", i].Value = Convert.ToDecimal(SumTotalS).ToString();
                    dgvSale["Total", i].Value = Convert.ToDecimal(Total).ToString(); //"0,0.00");



                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                    SumSDTotal = SumSDTotal + Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                    SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                    SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);

                    //dgvSale["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvSale["Quantity", i].Value)
                    //                                               - Convert.ToDecimal(dgvSale["Previous", i].Value))
                    //    .
                    //    ToString(); //"0,0.0000");



                    txtTotalSubTotal.Text = Convert.ToDecimal(SumSubTotal).ToString(); //"0,0.00");

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

            if (rbtnCN.Checked)
            {
                transactionType = "Credit";
            }
            else if (rbtnExportServiceNS.Checked)
            {
                transactionType = "ExportServiceNS";
                //txtCategoryName.Text = "Export";

            }
            else if (rbtnServiceNS.Checked)
            {

                transactionType = "ServiceNS";

            }
            if (chkExport.Checked)
            {
                transactionType = "ExportServiceNS";

            }

            if (rbtnCN.Checked && chkExport.Checked)
            {
                transactionType = "ExportServiceNSCredit";
            }

            #endregion Transaction Type
        }

        private void SelectVATName(string vatNameInCD)
        {
            //    try
            //    {
            //        #region Transaction Type

            //        if (!string.IsNullOrEmpty(vatNameInCD))
            //        {
            //            VATName vname = new VATName();

            //            IList<string> vatName = vname.VATNameList;
            //            for (int i = 0; i < vatName.Count; i++)
            //            {
            //               if (vatNameInCD.Trim() == vatName[i].ToString().Trim())
            //                {
            //                    cmbVAT1Name.SelectedIndex = i;
            //                    break;
            //                }
            //            }
            //        }

            //        SaleDAL saleDal = new SaleDAL();
            //        string categoryName = saleDal.GetCategoryName(txtProductCode.Text);
            //        if (!string.IsNullOrEmpty(categoryName))
            //        {
            //            txtCategoryName.Text = categoryName;
            //        }


            //        #endregion Transaction Type
            //    }
            //    catch (Exception ex)
            //    {
            //        string exMessage = ex.Message;
            //        if (ex.InnerException != null)
            //        {
            //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                        ex.StackTrace;

            //        }
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        FileLogger.Log(this.Name, "SelectVATName", exMessage);
            //    }

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
                    Convert.ToDateTime(dtpChallanDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Delivery date not before invoice date", this.Text);
                    return;
                }

                if (Program.CheckLicence(dtpChallanDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                SetupVATStatus();

                NextID = IsUpdate == false ? string.Empty : txtSalesInvoiceNo.Text.Trim();

                #endregion start

                #region Null

                if (txtCustomerID.Text == "")
                {
                    MessageBox.Show("Please Enter Customer Information");
                    cmbCustomerName.Focus();
                    return;
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



                #endregion Null


                #region Master
                saleMaster = new SaleMasterVM();
                saleMaster.SalesInvoiceNo = NextID.ToString();
                saleMaster.CustomerID = txtCustomerID.Text.Trim();
                saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();


                saleMaster.InvoiceDateTime = dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

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
                saleMaster.SerialNo = "";

                saleMaster.BranchId = Program.BranchId;



                #endregion Master
                #region detail

                saleDetails = new List<BureauSaleDetailVM>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    BureauSaleDetailVM detail = new BureauSaleDetailVM();

                    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.InvoiceName = dgvSale.Rows[i].Cells["InvoiceNo"].Value.ToString();
                    detail.InvoiceDateTime = dgvSale.Rows[i].Cells["InvoiceDate"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SaleQuantity"].Value.ToString());

                    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());

                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());

                    detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                    //detail.CPCName = dgvSale.Rows[i].Cells["CPCName"].Value.ToString();
                    //detail.HSCode = dgvSale.Rows[i].Cells["HSCode"].Value.ToString();
                    detail.InvoiceCurrency = cmbCurrency.Text;
                    detail.BureauType = "";

                    if (rbtnCN.Checked)
                    {
                        //detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                        detail.ReturnTransactionType = "ServiceNS";
                        detail.PreviousSalesInvoiceNo = txtOldID.Text.Trim();

                    }

                    detail.BranchId = Program.BranchId;

                    saleDetails.Add(detail);
                }
                #endregion detail
                #region export

                if (ChkExpLoc.Checked)
                {
                    //saleExport = new List<SaleExportVM>();

                    //for (int i = 0; i < dgvExport.RowCount; i++)
                    //{

                    //    if (string.IsNullOrEmpty(dgvExport.Rows[i].Cells["Description"].Value.ToString()) ||
                    //         dgvExport.Rows[i].Cells["Description"].Value == null ||
                    //         dgvExport.Rows[i].Cells["QuantityE"].Value == null ||
                    //         dgvExport.Rows[i].Cells["GrossWeight"].Value == null ||
                    //         dgvExport.Rows[i].Cells["NetWeight"].Value == null ||
                    //         dgvExport.Rows[i].Cells["NumberFrom"].Value == null ||
                    //         dgvExport.Rows[i].Cells["NumberTo"].Value == null ||
                    //        string.IsNullOrEmpty(dgvExport.Rows[i].Cells["QuantityE"].Value.ToString()) ||
                    //        string.IsNullOrEmpty(dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString()) ||
                    //        string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NetWeight"].Value.ToString()) ||
                    //        string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString()) ||
                    //        string.IsNullOrEmpty(dgvExport.Rows[i].Cells["NumberTo"].Value.ToString()))
                    //    {
                    //        MessageBox.Show("Please check the export Details, All the data must fill", this.Text);
                    //        ExportPackagingInfoAdd();
                    //        return;
                    //    }
                    //    SaleExportVM expDetail = new SaleExportVM();
                    //    expDetail.SaleLineNo = dgvExport.Rows[i].Cells["LineNoE"].Value.ToString();
                    //    expDetail.Description = dgvExport.Rows[i].Cells["Description"].Value.ToString();
                    //    expDetail.QuantityE = dgvExport.Rows[i].Cells["QuantityE"].Value.ToString();
                    //    expDetail.GrossWeight = dgvExport.Rows[i].Cells["GrossWeight"].Value.ToString();
                    //    expDetail.NetWeight = dgvExport.Rows[i].Cells["NetWeight"].Value.ToString();
                    //    expDetail.NumberFrom = dgvExport.Rows[i].Cells["NumberFrom"].Value.ToString();
                    //    expDetail.NumberTo = dgvExport.Rows[i].Cells["NumberTo"].Value.ToString();
                    //    expDetail.CommentsE = "NA";
                    //    expDetail.RefNo = "NA";

                    //    saleExport.Add(expDetail);
                }
                #endregion export


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


                        return;
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


                sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, null, null,connVM);
                SAVE_DOWORK_SUCCESS = true;

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
                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
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
                    Convert.ToDateTime(dtpChallanDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Delivery date not before invoice date", this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpChallanDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                SetupVATStatus();

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


                #endregion Null



                saleMaster = new SaleMasterVM();
                saleMaster.SalesInvoiceNo = NextID.ToString();

                saleMaster.CustomerID = txtCustomerID.Text.Trim();
                saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();

                saleMaster.InvoiceDateTime = dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

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
                //saleMaster.CurrencyID = "260"; //BDT
                saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate



                saleDetails = new List<BureauSaleDetailVM>();

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    BureauSaleDetailVM detail = new BureauSaleDetailVM();

                    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.InvoiceName = dgvSale.Rows[i].Cells["InvoiceNo"].Value.ToString();
                    detail.InvoiceDateTime = dgvSale.Rows[i].Cells["InvoiceDate"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SaleQuantity"].Value.ToString());

                    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());

                    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());

                    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                    detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                    detail.PreviousSalesInvoiceNo = dgvSale.Rows[i].Cells["PreviousSalesInvoiceNo"].Value.ToString();
                    detail.PreviousInvoiceDateTime = dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value.ToString();
                    detail.PreviousNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value.ToString());
                    detail.PreviousQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value.ToString());
                    detail.PreviousSubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value.ToString());
                    detail.PreviousVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value.ToString());
                    //detail.CPCName = dgvSale.Rows[i].Cells["CPCName"].Value.ToString();
                    //detail.HSCode = dgvSale.Rows[i].Cells["HSCode"].Value.ToString();
                    if (rbtnCN.Checked)
                    {
                        detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                        //detail.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                    }

                    saleDetails.Add(detail);
                }

                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
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
                sqlResults = saleDal.SalesUpdate(saleMaster, saleDetails,connVM);
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
                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TransactionTypes();

                #region Start

                //if (Post == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotPostAccess, this.Text);
                //    return;
                //}
                //else 
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
                        Convert.ToDateTime(dtpChallanDate.Value.ToString("yyyy-MMM-dd")))
                    {
                        MessageBox.Show("Delivery date  and time not before invoice date", this.Text);
                        return;
                    }
                    if (Program.CheckLicence(dtpChallanDate.Value) == true)
                    {
                        MessageBox.Show(
                            "Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }
                    SetupVATStatus();

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

                    #endregion Null



                #endregion Start

                    saleMaster = new SaleMasterVM();
                    saleMaster.SalesInvoiceNo = NextID.ToString();

                    saleMaster.CustomerID = txtCustomerID.Text.Trim();
                    saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                    saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                    saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();

                    saleMaster.InvoiceDateTime =
                        dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                    saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

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

                    saleMaster.Post = "Y"; //Post
                    saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                    saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate

                    saleDetails = new List<BureauSaleDetailVM>();

                    for (int i = 0; i < dgvSale.RowCount; i++)
                    {
                        BureauSaleDetailVM detail = new BureauSaleDetailVM();

                        detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                        detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                        detail.InvoiceName = dgvSale.Rows[i].Cells["InvoiceNo"].Value.ToString();
                        detail.InvoiceDateTime = dgvSale.Rows[i].Cells["InvoiceDate"].Value.ToString();
                        detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SaleQuantity"].Value.ToString());
                        detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                        detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                        detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                        detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                        detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());

                        detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                        detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                        detail.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                        detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                        detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                        detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                        saleDetails.Add(detail);
                    } // end of for

                }


                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
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

                sqlResults = saleDal.SalesPost(saleMaster, saleDetails,connVM);
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

            txtOldID.Text = "";
            if (chkSameCustomer.Checked == false)
            {
                cmbCustomerName.Text = "Select";
                cmbCPCName.Text = "Select";
                txtCustomerID.Text = "";
                txtCustomerGroupName.Text = "";
                txtDeliveryAddress1.Text = "";
                txtDeliveryAddress2.Text = "";
                txtDeliveryAddress3.Text = "";
                txtHSCode.Text = "";


            }


            dtpChallanDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dtpDeliveryDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            txtNBRPrice.Text = "0.00";


            txtCommentsDetail.Text = "NA";

            txtHSCode.Text = "";
            txtLineNo.Text = "";

            txtProductCode.Text = "";
            txtProductName.Text = "";
            if (rbtnServiceNS.Checked)
            {
                txtQuantity.Text = "1";
                txtQty1.Text = "1";
                txtQty2.Text = "0";
                txtInvoiceNo.Text = "";
                dtpInvoiceDate.Value = DateTime.Now;

            }
            else
            {
                txtQuantity.Text = "0.00";
                txtQty1.Text = "0";
                txtQty2.Text = "0";
            }




            txtSalesInvoiceNo.Text = "";

            txtTotalAmount.Text = "0.00";
            txtTotalVATAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            txtVATRate.Text = "0.00";

            txtUOM.Text = "";

            txtSD.Text = "0.00";
            txtQuantityInHand.Text = "0.0";
            txtTDBalance.Text = "0.00";

            txtTotalSubTotal.Clear();
            dgvSale.Rows.Clear();


        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
            //if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            //{
            //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), SalePlaceQty).ToString();

            //}

        }

        private void txtNBRPrice_Leave(object sender, EventArgs e)
        {
            //btnAdd.Focus();
            txtNBRPrice.Text = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();

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
                            txtLineNo.Text = dgvSale.CurrentCellAddress.Y.ToString();
                            txtProductCode.Text = userSelRow.Cells["ItemNo"].Value.ToString();
                            dtpInvoiceDate.Value = Convert.ToDateTime(userSelRow.Cells["InvoiceDate"].Value.ToString());

                            txtInvoiceNo.Text = userSelRow.Cells["InvoiceNo"].Value.ToString();



                            txtQty1.Text = Convert.ToDecimal(userSelRow.Cells["SaleQuantity"].Value).ToString();//"0,0.0000");

                            //txtUnitCost.Text =
                            //    Convert.ToDecimal(userSelRow.Cells["UnitPrice"].Value).ToString();//"0.00");
                            txtVATRate.Text = Convert.ToDecimal(userSelRow.Cells["VATRate"].Value).ToString();//"0.00");

                            //txtNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["NBRPrice"].Value).ToString();//"0.00");

                            txtNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["DollerValue"].Value).ToString();//"0.00");
                            txtUOM.Text = userSelRow.Cells["UOM"].Value.ToString();
                            cmbUom.Items.Insert(0, txtUOM.Text.Trim());

                            //UomsValue();
                            Uoms();
                            cmbUom.Text = userSelRow.Cells["UOM"].Value.ToString();
                            cmbCPCName.Text = userSelRow.Cells["CPCName"].Value.ToString();
                            txtHSCode.Text = userSelRow.Cells["HSCode"].Value.ToString();

                            txtSD.Text = Convert.ToDecimal(userSelRow.Cells["SD"].Value).ToString();//"0.00");
                            txtPrevious.Text = Convert.ToDecimal(userSelRow.Cells["Previous"].Value).ToString();//"0.00");


                            //cmbVAT1Name.Text= userSelRow.Cells["VATName"].Value.ToString();
                            //cmbVAT1Name.SelectedItem = userSelRow.Cells["VATName"].Value.ToString();

                            dtpConversionDate.Value = Convert.ToDateTime(userSelRow.Cells["CConvDate"].Value.ToString());

                            CurrencyConversiondate = userSelRow.Cells["CConvDate"].Value.ToString();

                            dtpConversionDate_Leave(sender, e);
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
                    if (WantToPrint == "Y")
                    {
                        SaleDAL PrintUpdate = new SaleDAL();
                        //ISale PrintUpdate = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);

                        sqlResults = PrintUpdate.UpdatePrintNew(txtSalesInvoiceNo.Text.Trim(), PrintCopy, connVM);// Change 04

                        UPDATEPRINT_DOWORK_SUCCESS = true;
                    }
                    else
                    {
                        UPDATEPRINT_DOWORK_SUCCESS = false;

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
                        NewPrintCopy = Convert.ToInt32(sqlResults[3]);

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
                //IReport showReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ReportResultVAT11 = new DataSet();
                ReportResultCreditNote = new DataSet();
                ReportResultDebitNote = new DataSet();
                if (rbtnCN.Checked)
                {
                    ReportResultCreditNote = showReport.BureauCreditNote(txtSalesInvoiceNo.Text.Trim(), post1, post2);
                }

                else
                {
                    ReportResultVAT11 = showReport.BureauVAT11Report(varSalesInvoiceNo, post1, post2);
                    invoiceDateTimeBureau = Convert.ToDateTime(ReportResultVAT11.Tables[0].Rows[0]["InvoiceDate"]);

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
        private void backgroundWorkerReportVAT11Ds_RunWorkerCompletedOld(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                DataTable settingsDt = new DataTable();
                string vVAT2012V2 = "2020-Jul-01";

                vVAT2012V2 = new CommonDAL().settingsDesktop("Version", "VAT2012V2", settingsDt,connVM);

                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
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
                    ////ReportResultCreditNote.Tables[0].TableName = "DsVAT11";
                }
                else
                {
                    ReportResultVAT11.Tables[0].TableName = "DsVAT11";
                    #region InWord

                    //for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    //{
                    //    //QtyInWord = QtyInWord + ReportResultVAT11.Tables[1].Rows[i]["uom"].ToString() + ": " +
                    //    //            Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[1].Rows[i]["qty"])) + ", ";
                    //    QtyInWord = QtyInWord + ": " +
                    //         Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[0].Rows[i]["Quantity"])) + ", ";


                    //}
                    // It has only one product
                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                    }
                    QtyInWord = Program.NumberToWords(Convert.ToInt32(Quantity));

                    if (Convert.ToInt32(QtyInWord.Length) <= 0)
                    {
                        QtyInWord = "In Words(Quantity): .";

                    }
                    else
                    {
                        QtyInWord = "In Words(Quantity): " + QtyInWord.ToString() + " .";

                    }
                    QtyInWord = QtyInWord.ToUpper();


                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                        SDAmount = SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        Qty_UnitCost = Qty_UnitCost + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]);
                        //Qty_UnitCost_SDAmount = Qty_UnitCost_SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]) + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        VATAmount = VATAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["VATAmount"]);
                        Subtotal = Subtotal + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Subtotal"]);
                    }

                    vQuantity = Convert.ToDecimal(Quantity).ToString("0,0.00");
                    vSDAmount = Convert.ToDecimal(SDAmount).ToString("0,0.00");
                    vQty_UnitCost = Convert.ToDecimal(Qty_UnitCost).ToString("0,0.00");
                    //vQty_UnitCost_SDAmount = Convert.ToDecimal(Qty_UnitCost_SDAmount).ToString("0,0.00");
                    vVATAmount = Convert.ToDecimal(VATAmount).ToString("0,0.00");
                    vSubtotal = Convert.ToDecimal(Subtotal).ToString("0,0.00");


                    #endregion InWord


                }


                #endregion
                string prefix = "";
                ReportDocument objrpt = new ReportDocument();
                #region CN

                if (rbtnCN.Checked)
                {

                    objrpt = new RptCreditNote(); 
                    ////objrpt = new RptVAT6_7(); 
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

                #region rbtnService
                else if (rbtnServiceNS.Checked)
                {
                    if (chkIs11.Checked)
                    {
                        objrpt = new RptVAT6_3();
                        prefix = "VAT6.3";
                    }
                    else
                    {
                        objrpt = new RptVAT6_3();
                        prefix = "VAT6.3";
                    }

                    if (VAT11Name.ToLower() == "bvcps")
                    {
                        //objrpt = new ReportDocument();
                        //objrpt = new RptVAT6_3_BVCPS();
                        if (VAT2012V2 <= invoiceDateTimeBureau)
                        {
                            ////New Report -- 
                            objrpt = new ReportDocument();
                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_BVCPS_V12V2.rpt");
                            objrpt = new RptVAT6_3_BVCPS_V12V2();
                        }
                        else
                        {
                            objrpt = new ReportDocument();
                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BVCPS.rpt");

                            objrpt = new RptVAT6_3_BVCPS();
                        }
                    }


                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 6.3'";
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


                #region currency
                //SaleDAL saleDal=new SaleDAL();

                //string currencyMajor = "";
                //string currencyMinor = "";
                //string currencySymbol = "";
                //sqlResults=new string[4];

                //sqlResults = saleDal.CurrencyInfo(txtSalesInvoiceNo.Text);
                //if (sqlResults[0].ToString() != "Fail")
                //{
                //    currencyMajor = sqlResults[1].ToString();
                //    currencyMinor = sqlResults[2].ToString();
                //    currencySymbol = sqlResults[3].ToString();
                //}

                #endregion currency

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMajor"].Text = "'taka'";
                objrpt.DataDefinition.FormulaFields["CurrencyMinor"].Text = "'paisa'";
                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "tk";

                FormReport6_3 reports = new FormReport6_3();
                reports.crystalReportViewer1.Refresh();
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "''";

                    reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                        //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {
                    int j = 0;
                    int AlreadyPrint = Convert.ToInt32(ReportResultVAT11.Tables[0].Rows[0]["AlreadyPrint"]);
                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                        string copiesNo = (AlReadyPrintNo + i).ToString();
                        j = AlreadyPrint - PrintCopy + i;

                        //int cpno = AlreadyPrint + i;

                        //#region CopyName

                        //if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                        //    (cpno >= 44 && cpno <= 50))
                        //{
                        //    copiesNo = cpno + " th copy";
                        //}
                        //else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        //{
                        //    copiesNo = cpno + " st copy";
                        //}
                        //else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        //{
                        //    copiesNo = cpno + " nd copy";
                        //}
                        //else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        //{
                        //    copiesNo = cpno + " rd copy";
                        //}
                        //else
                        //{

                        //    copiesNo = cpno + " copy";

                        //}



                        //#endregion CopyName

                        if (Is3Plyer == "Y")
                        {
                            objrpt.DataDefinition.FormulaFields["CurrentPrintCopy"].Text = "";
                        }
                        else

                        {
                            objrpt.DataDefinition.FormulaFields["CurrentPrintCopy"].Text =  j.ToString() ;

                        }


                        #region Save & Print in PDF format

                        ReportDocument rptDoc = objrpt;
                        string printInvoiceNo = txtSalesInvoiceNo.Text.Trim().Replace("/", "-");
                        PrintLocation = Directory.GetCurrentDirectory() + @"\NBR_Reports";
                        PrintLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBR_Reports");

                        string saveLocation = PrintLocation + @"\" + prefix + "_" + printInvoiceNo + "_" + copiesNo +
                                              ".pdf";
                        if (!Directory.Exists(PrintLocation))
                            Directory.CreateDirectory(PrintLocation);

                        //rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, saveLocation);
                        Application.DoEvents();

                        rptDoc.PrintOptions.PrinterName = VPrinterName;
                        rptDoc.PrintToPrinter(1, false, 1, 1);

                        //Thread.Sleep(2000);
                    }
                    AlReadyPrintNo += PrintCopy;

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
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

        private void backgroundWorkerReportVAT11Ds_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                DataTable settingsDt = new DataTable();
                string vVAT2012V2 = "2020-Jul-01";

                vVAT2012V2 = new CommonDAL().settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);
                FormulaFieldDefinitions crFormulaF;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
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
                    //ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";

                    if (PreviewOnly == true)
                    {
                        ReportResultCreditNote.Tables[0].TableName = "DsVAT11";
                    }
                    else
                    {
                        ReportResultVAT11 = ReportResultCreditNote.Copy();

                        ReportResultVAT11.Tables[0].TableName = "DsVAT11";
                    }
                }
                else
                {
                    ReportResultVAT11.Tables[0].TableName = "DsVAT11";
                    #region InWord

                    //for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    //{
                    //    //QtyInWord = QtyInWord + ReportResultVAT11.Tables[1].Rows[i]["uom"].ToString() + ": " +
                    //    //            Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[1].Rows[i]["qty"])) + ", ";
                    //    QtyInWord = QtyInWord + ": " +
                    //         Program.NumberToWords(Convert.ToInt32(ReportResultVAT11.Tables[0].Rows[i]["Quantity"])) + ", ";


                    //}
                    // It has only one product
                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                    }
                    QtyInWord = Program.NumberToWords(Convert.ToInt32(Quantity));

                    if (Convert.ToInt32(QtyInWord.Length) <= 0)
                    {
                        QtyInWord = "In Words(Quantity): .";

                    }
                    else
                    {
                        QtyInWord = "In Words(Quantity): " + QtyInWord.ToString() + " .";

                    }
                    QtyInWord = QtyInWord.ToUpper();


                    for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                    {
                        Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                        SDAmount = SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        Qty_UnitCost = Qty_UnitCost + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]);
                        //Qty_UnitCost_SDAmount = Qty_UnitCost_SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]) + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                        VATAmount = VATAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["VATAmount"]);
                        Subtotal = Subtotal + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Subtotal"]);
                    }

                    vQuantity = Convert.ToDecimal(Quantity).ToString("0,0.00");
                    vSDAmount = Convert.ToDecimal(SDAmount).ToString("0,0.00");
                    vQty_UnitCost = Convert.ToDecimal(Qty_UnitCost).ToString("0,0.00");
                    //vQty_UnitCost_SDAmount = Convert.ToDecimal(Qty_UnitCost_SDAmount).ToString("0,0.00");
                    vVATAmount = Convert.ToDecimal(VATAmount).ToString("0,0.00");
                    vSubtotal = Convert.ToDecimal(Subtotal).ToString("0,0.00");


                    #endregion InWord


                }


                #endregion
                string prefix = "";
                ReportDocument objrpt = new ReportDocument();
                #region CN

                if (rbtnCN.Checked)
                {

                    ////objrpt = new RptCreditNote();
                    ////objrpt = new RptVAT6_7_V12V2();
                    objrpt = new RptVAT6_7_V12V2_BVCPS();
                    prefix = "CreditNote";


                    //objrpt.SetDataSource(ReportResultCreditNote);

                    if (PreviewOnly == true)
                    {
                        objrpt.SetDataSource(ReportResultCreditNote);

                    }
                    else
                    {
                        objrpt.SetDataSource(ReportResultVAT11);

                    }

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";


                }
                #endregion CN

                #region rbtnService
                else if (rbtnServiceNS.Checked)
                {
                    if (chkIs11.Checked)
                    {
                        objrpt = new RptVAT6_3();
                        prefix = "VAT6.3";
                    }
                    else
                    {
                        objrpt = new RptVAT6_3();
                        prefix = "VAT6.3";
                    }

                    if (VAT11Name.ToLower() == "bvcps")
                    {
                        //objrpt = new ReportDocument();
                        //objrpt = new RptVAT6_3_BVCPS();
                        if (VAT2012V2 <= invoiceDateTimeBureau)
                        {
                            UserInformationVM uvm = new UserInformationVM();
                            UserInformationDAL _udal = new UserInformationDAL();
                            uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();
                            ////New Report -- 
                            objrpt = new ReportDocument();
                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_BVCPS_V12V2.rpt");
                            objrpt = new RptVAT6_3_BVCPS_V12V2();

                            objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                            objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";
                        }
                        else
                        {
                            objrpt = new ReportDocument();
                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BVCPS.rpt");

                            objrpt = new RptVAT6_3_BVCPS();
                        }
                    }


                    objrpt.SetDataSource(ReportResultVAT11);
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 6.3'";
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


                #region currency
                //SaleDAL saleDal=new SaleDAL();

                //string currencyMajor = "";
                //string currencyMinor = "";
                //string currencySymbol = "";
                //sqlResults=new string[4];

                //sqlResults = saleDal.CurrencyInfo(txtSalesInvoiceNo.Text);
                //if (sqlResults[0].ToString() != "Fail")
                //{
                //    currencyMajor = sqlResults[1].ToString();
                //    currencyMinor = sqlResults[2].ToString();
                //    currencySymbol = sqlResults[3].ToString();
                //}

                #endregion currency

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                objrpt.DataDefinition.FormulaFields["CurrencyMajor"].Text = "'taka'";
                objrpt.DataDefinition.FormulaFields["CurrencyMinor"].Text = "'paisa'";
                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "tk";

                FormReport6_3 reports = new FormReport6_3();
                reports.crystalReportViewer1.Refresh();
                if (PreviewOnly == true)
                {

                    if (rbtnCN.Checked)
                    {
                        int AlreadyPrint = Convert.ToInt32(ReportResultCreditNote.Tables[0].Rows[0]["AlreadyPrint"]);

                        #region Formula Fields

                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrentPrintCopy", AlreadyPrint.ToString());

                        #endregion

                    }

                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "''";

                    reports.setReportSource(objrpt);
                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                        //reports.ShowDialog();
                        reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {
                    int j = 0;
                    int AlreadyPrint = Convert.ToInt32(ReportResultVAT11.Tables[0].Rows[0]["AlreadyPrint"]);
                    SaleDAL PrintUpdate = new SaleDAL();

                    for (int i = 1; i <= PrintCopy; i++)
                    {

                        sqlResults = PrintUpdate.UpdatePrintNew(txtSalesInvoiceNo.Text.Trim(), 1, connVM);// Change 04


                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                        string copiesNo = (AlReadyPrintNo + i).ToString();
                        j = AlreadyPrint - PrintCopy + i;


                        #region CopyName

                        //int cpno = AlreadyPrint + i;


                        //if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                        //    (cpno >= 44 && cpno <= 50))
                        //{
                        //    copiesNo = cpno + " th copy";
                        //}
                        //else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        //{
                        //    copiesNo = cpno + " st copy";
                        //}
                        //else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        //{
                        //    copiesNo = cpno + " nd copy";
                        //}
                        //else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        //{
                        //    copiesNo = cpno + " rd copy";
                        //}
                        //else
                        //{

                        //    copiesNo = cpno + " copy";

                        //}

                        #endregion CopyName

                        if (Is3Plyer == "Y")
                        {
                            objrpt.DataDefinition.FormulaFields["CurrentPrintCopy"].Text = "";
                        }
                        else
                        {
                            if (rbtnCN.Checked)
                            {
                                #region Formula Fields

                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrentPrintCopy", copiesNo);

                                #endregion

                            }



                            ////objrpt.DataDefinition.FormulaFields["CurrentPrintCopy"].Text = j.ToString();

                        }


                        #region Save & Print in PDF format

                        ReportDocument rptDoc = objrpt;
                        string printInvoiceNo = txtSalesInvoiceNo.Text.Trim().Replace("/", "-");
                        PrintLocation = Directory.GetCurrentDirectory() + @"\NBR_Reports";
                        PrintLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NBR_Reports");

                        string saveLocation = PrintLocation + @"\" + prefix + "_" + printInvoiceNo + "_" + copiesNo +
                                              ".pdf";
                        if (!Directory.Exists(PrintLocation))
                            Directory.CreateDirectory(PrintLocation);

                        //rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, saveLocation);
                        Application.DoEvents();

                        rptDoc.PrintOptions.PrinterName = VPrinterName;
                        rptDoc.PrintToPrinter(1, false, 1, 1);

                        //PrinterSettings printerSettings =
                        //       new PrinterSettings();

                        //printerSettings.PrinterName = VPrinterName;

                        //rptDoc.PrintToPrinter(printerSettings, new PageSettings(), false);


                        //Thread.Sleep(2000);
                    }
                    AlReadyPrintNo += PrintCopy;

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
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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



        //============22/01/13 =============

        private void 
            ReportVAT11Ds()
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
                vItemNature = commonDal.settingsDesktop("Sale", "ItemNature",null,connVM);
                vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter",null,connVM);
                vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4",null,connVM);
                vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal",null,connVM);


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

                Program.fromOpen = "Other";

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


        private void btnProductType_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                Program.fromOpen = "Other";

                //Program.R_F = "";
                //Program.ItemType = "Other";
                DataGridViewRow selectedRow = new DataGridViewRow();
                FormProductSearch.ProductCategoryName = "Service Non Stock";
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();
                    txtProductCode.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtPCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();
                    txtVATRate.Text = Convert.ToDecimal(selectedRow.Cells["VATRate"].Value.ToString()).ToString("0.00");
                    txtSD.Text = Convert.ToDecimal(selectedRow.Cells["SD"].Value.ToString()).ToString("0.00");


                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                    cmbUom.Text = selectedRow.Cells["UOM"].Value.ToString();
                    txtHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString();


                    chkNonStock.Checked = selectedRow.Cells["NonStock"].Value.ToString() == "Y" ? true : false;

                    Uoms();

                    if (rbtnServiceNS.Checked == true)
                    {
                        txtQuantity.Text = "1";

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

        private void FormBureauSale_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormBureauSale_FormClosing", exMessage);
            }

            #endregion


        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtSD, "SD Rate");
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();
        }

        private void cmbCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
                //cmbVehicleNo.Focus();
            }
        }

        private void dtpChallanDate_KeyDown(object sender, KeyEventArgs e)
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

                if (Program.CheckLicence(dtpChallanDate.Value) == true)
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
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) reports.ShowDialog();
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

                #region Transaction Type
                DataGridViewRow selectedRow = null;
                TransactionTypes();
                selectedRow = FormSaleSearch.SelectOne(transactionType);



                #endregion Transaction Type
                //SetupVATStatus();
                //Thread.Sleep(2000);
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


                    txtTotalAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString());//"0,0.00");
                    txtTotalVATAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString());//"0,0.00");

                    dtpChallanDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDateTime"].Value.ToString());
                    dtpDeliveryDate.Value = Convert.ToDateTime(selectedRow.Cells["DeliveryDate"].Value.ToString());

                    txtOldID.Text = selectedRow.Cells["PID"].Value.ToString();
                    if (string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Credit" ||
                        string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Debit")
                    {
                        if (CreditWithoutTransaction == true)
                        {
                            txtOldID.ReadOnly = false;
                        }
                    }

                    cmbCurrency.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtCurrencyId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();
                    txtBDTRate.Text = Program.ParseDecimalObject(selectedRow.Cells["CurrencyRateFromBDT"].Value.ToString());
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
                    SearchBranchId = selectedRow.Cells["BranchId"].Value.ToString();

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
                BureauSaleDAL saleDal = new BureauSaleDAL();
                //IBureauSale saleDal = OrdinaryVATDesktop.GetObject<BureauSaleDAL, BureauSaleRepo, IBureauSale>(OrdinaryVATDesktop.IsWCF);

                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData,connVM);
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

                    txtProductName.Text = item["ProductName"].ToString();
                    txtProductCode.Text = item["ItemNo"].ToString();
                    txtPCode.Text = item["ProductCode"].ToString();


                    dgvSale.Rows[j].Cells["LineNo"].Value = item["InvoiceLineNo"].ToString();

                    dgvSale.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvSale.Rows[j].Cells["InvoiceNo"].Value = item["InvoiceName"].ToString();
                    dgvSale.Rows[j].Cells["InvoiceDate"].Value = item["InvoiceDateTime"].ToString();
                    dgvSale.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvSale.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    dgvSale.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());

                    dgvSale.Rows[j].Cells["SaleQuantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvSale.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["SalesPrice"].ToString());
                    //dgvSale.Rows[j].Cells["NBRPrice"].Value = item["DollerValue"].ToString();
                    dgvSale.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    dgvSale.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    dgvSale.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());

                    dgvSale.Rows[j].Cells["Status"].Value = "Old";
                    dgvSale.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                    dgvSale.Rows[j].Cells["Change"].Value = 0;
                    dgvSale.Rows[j].Cells["NonStock"].Value = "Y";

                    //dgvSale.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvSale.Rows[j].Cells["DollerValue"].Value = Program.ParseDecimalObject(item["DollerValue"].ToString());
                    dgvSale.Rows[j].Cells["BDTValue"].Value = Program.ParseDecimalObject(item["CurrencyValue"].ToString());
                    dgvSale.Rows[j].Cells["CConvDate"].Value = item["CConversionDate"].ToString();
                    //dgvSale.Rows[j].Cells["CPCName"].Value = item["CPCName"].ToString();
                    //dgvSale.Rows[j].Cells["HSCode"].Value = item["HSCode"].ToString();

                    decimal GTotal = Convert.ToDecimal(item["SubTotal"].ToString()) + Convert.ToDecimal(item["VATAmount"].ToString());

                    if (Program.CheckingNumericString(GTotal.ToString(), "Total") == true)
                        GTotal = Convert.ToDecimal(Program.FormatingNumeric(GTotal.ToString(), SalePlaceTaka));

                    dgvSale.Rows[j].Cells["Total"].Value = Program.ParseDecimalObject(GTotal.ToString());
                    dgvSale.Rows[j].Cells["Type"].Value = item["Type"].ToString();

                    dgvSale.Rows[j].Cells["PreviousSalesInvoiceNo"].Value = item["PreviousSalesInvoiceNo"].ToString();
                    dgvSale.Rows[j].Cells["PreviousInvoiceDateTime"].Value = item["PreviousInvoiceDateTime"].ToString();
                    dgvSale.Rows[j].Cells["PreviousNBRPrice"].Value = item["PreviousNBRPrice"].ToString();
                    dgvSale.Rows[j].Cells["PreviousQuantity"].Value = item["PreviousQuantity"].ToString();
                    dgvSale.Rows[j].Cells["PreviousSubTotal"].Value = item["PreviousSubTotal"].ToString();
                    dgvSale.Rows[j].Cells["PreviousVATAmount"].Value = item["PreviousVATAmount"].ToString();
                    dgvSale.Rows[j].Cells["PreviousVATRate"].Value = item["PreviousVATRate"].ToString();
                    dgvSale.Rows[j].Cells["PreviousSD"].Value = item["PreviousSD"].ToString();
                    dgvSale.Rows[j].Cells["PreviousSDAmount"].Value = item["PreviousSDAmount"].ToString();
                    dgvSale.Rows[j].Cells["ReasonOfReturn"].Value = item["ReasonOfReturn"].ToString();
                    dgvSale.Rows[j].Cells["PreviousUOM"].Value = item["PreviousUOM"].ToString();

                    #region currency convertion date change for export
                    //if (ChkExpLoc.Checked)
                    //{
                    //    dgvSale.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
                    //    dgvSale.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();

                    //    decimal GTotal = Convert.ToDecimal(item["SubTotal"].ToString()) + Convert.ToDecimal(item["SDAmount"].ToString()) + Convert.ToDecimal(item["tradingMarkup"].ToString()) + Convert.ToDecimal(item["VATAmount"].ToString());

                    //    if (Program.CheckingNumericString(GTotal.ToString(), "Total") == true)
                    //        GTotal = Convert.ToDecimal(Program.FormatingNumeric(GTotal.ToString(), SalePlaceTaka));

                    //    dgvSale.Rows[j].Cells["Total"].Value = GTotal.ToString();
                    //}
                    if (transactionType == "Credit")
                    {
                        dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = item["ReturnTransactionType"].ToString();

                    }
                    #endregion currency convertion date change for export
                    j = j + 1;
                } // End For

                //btnSave.Text = "&Save";
                if (ChkExpLoc.Checked)
                {
                    GTotal();
                }
                else
                {
                    GTotal();
                    //Rowcalculate();
                }


                //ExportSearch();
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
                //DataGridViewRow selectedRow = FormBureauSaleSearch.SelectOne("'Other'");
                if (rbtnCN.Checked)
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


                    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtTotalVATAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");

                    dtpChallanDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                    cmbCurrency.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtCurrencyId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();
                    txtBDTRate.Text = selectedRow.Cells["CurrencyRateFromBDT"].Value.ToString();
                    txtDollerRate.Text = selectedRow.Cells["SaleReturnId"].Value.ToString();
                    transactionTypeOld = selectedRow.Cells["transactionType"].Value.ToString();

                    if (rbtnCN.Checked)
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
                //ISale saleDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);

                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData,connVM);


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
                //ISale saleDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);


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
                    dgvReceiveHistory.Rows[j].Cells["CPCNameP"].Value = item["CPCName"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["HSCodeP"].Value = item["HSCode"].ToString();

                    if (ChkExpLoc.Checked)
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = item["VATAmount"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = item["SubTotal"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = item["SDAmount"].ToString();
                    }


                    j++;


                }

                #endregion Sale product load



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

            //try
            //{
            //    #region Statement

            //    ExportData = string.Empty;
            //    if (txtSalesInvoiceNo.Text == "")
            //    {
            //        ExportData = "0";
            //    }
            //    else
            //    {
            //        ExportData = txtSalesInvoiceNo.Text.Trim();
            //    }


            //    #endregion
            //    this.progressBar1.Visible = true;
            //    //this.Enabled = false;

            //    bgwExportSearch.RunWorkerAsync();
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
            //    FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

            //    FileLogger.Log(this.Name, "ExportSearch", exMessage);
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
            //    FileLogger.Log(this.Name, "ExportSearch", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "ExportSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
            //    FileLogger.Log(this.Name, "ExportSearch", exMessage);
            //}

            //#endregion


        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (chkPrint.Checked == true)
                {
                    btnPrint.Enabled = true;
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

                if (Program.CheckLicence(dtpChallanDate.Value) == true)
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
                frmRptVAT18.dtpToDate.Value = dtpChallanDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpChallanDate.Value;
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


                if (Program.CheckLicence(dtpChallanDate.Value) == true)
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

                if (Program.CheckLicence(dtpChallanDate.Value) == true)
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
                    frmRptVAT17.txtItemNo.Text = txtProductCode.Text;
                    frmRptVAT17.txtProductName.Text = txtProductName.Text;
                    frmRptVAT17.dtpFromDate.Value = dtpChallanDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpChallanDate.Value;
                    if (rbtnServiceNS.Checked)
                    {
                        frmRptVAT17.rbtnService.Checked = true;
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

        //private void UomsValue()
        //{
        //    try
        //    {
        //        #region Statement

        //        string uOMFrom = txtUOM.Text.Trim().ToLower();
        //        string uOMTo = cmbUom.Text.Trim().ToLower();
        //        if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
        //        {

        //            if (uOMFrom == uOMTo)
        //            {
        //                txtUomConv.Text = "1";

        //                return;
        //            }

        //            else if (UOMs != null && UOMs.Any())
        //            {

        //                var uoms =
        //                    from uom in
        //                        UOMs.Where(
        //                            x => x.UOMFrom.Trim().ToLower() == uOMFrom && x.UOMTo.Trim().ToLower() == uOMTo).
        //                        ToList()
        //                    select uom.Convertion;
        //                if (uoms != null && uoms.Any())
        //                {
        //                    txtUomConv.Text = uoms.First().ToString();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Please select the UOM", this.Text);
        //                    txtUomConv.Text = "0";
        //                    return;
        //                }
        //            }
        //        }

        //        #endregion
        //    }
        //    #region catch

        //    catch (IndexOutOfRangeException ex)
        //    {
        //        FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (FormatException ex)
        //    {

        //        FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }

        //    catch (SoapHeaderException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }

        //        FileLogger.Log(this.Name, "UomsValue", exMessage);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (SoapException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "UomsValue", exMessage);
        //    }
        //    catch (EndpointNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "UomsValue", exMessage);
        //    }

        //    #endregion
        //}

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
                    frmRptVAT16.dtpFromDate.Value = dtpChallanDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpChallanDate.Value;
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


        private void backgroundWorkerReportVAT20Ds_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {


                ReportDSDAL ShowReport = new ReportDSDAL();
                //IReport ShowReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

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
                //IReport ShowReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

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
                chkIs11.Text = "VAT6.3";
                btnPrint.Text = "VAT6.3";
            }
            else
            {
                chkIs11.Text = "VAT6.3";
                btnPrint.Text = "VAT6.3";
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
            //decimal discountPercent = 0;
            //decimal discountAmount = 0;
            //decimal discountedNBRPrice = 0;
            //decimal bDTRate = 0;
            //if (!string.IsNullOrEmpty(txtBDTRate.Text.Trim()))
            //{
            //    bDTRate = Convert.ToDecimal(txtBDTRate.Text.Trim());

            //}
            //if (bDTRate == null || bDTRate == 0)
            //{
            //    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
            //    return;
            //}
            //i
            //if (!string.IsNullOrEmpty(txtDiscountAmountInput.Text.Trim()))
            //{
            //    if (chkDiscount.Checked)
            //    {
            //        discountPercent = Convert.ToDecimal(txtDiscountAmountInput.Text.Trim());
            //        discountAmount = discountedNBRPrice * discountPercent / 100;
            //    }
            //    else
            //    {
            //        discountAmount = Convert.ToDecimal(txtDiscountAmountInput.Text.Trim());

            //    }
            //}
            //if (discountedNBRPrice < discountAmount)
            //{
            //    MessageBox.Show("Discount amount can't greater then NBR price", this.Text);
            //    return;
            //}
            //else
            //{
            //    txtDiscountAmount.Text = Convert.ToDecimal(discountAmount).ToString();
            //    txtNBRPrice.Text = Convert.ToDecimal(discountedNBRPrice - discountAmount).ToString();


            //}
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
            //Program.FormatTextBoxQty(txtDiscountAmountInput, "Discount Input Amount");
            txtDiscountAmountInput.Text = Program.ParseDecimalObject(txtDiscountAmountInput.Text.Trim()).ToString();
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

        private void dtpChallanDate_ValueChanged(object sender, EventArgs e)
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

        private void dtpChallanDate_Leave(object sender, EventArgs e)
        {

        }

        private void dtpDeliveryDate_Leave(object sender, EventArgs e)
        {


        }

        private void btnFormKaT_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dtpChallanDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                FormRptVATKa frmRptVATKa = new FormRptVATKa();


                if (dgvSale.Rows.Count > 0)
                {
                    frmRptVATKa.txtItemNo.Text = dgvSale.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVATKa.txtProductName.Text = dgvSale.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVATKa.dtpFromDate.Value = dtpChallanDate.Value;
                    frmRptVATKa.dtpToDate.Value = dtpChallanDate.Value;
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
            //ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);

            string[] cValues = new string[] { "Y", dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss") };
            string[] cFields = new string[] { "cc.ActiveStatus like", "cc.ConversionDate" };
            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, false,connVM);

            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
            //     string.Empty, "Y", dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));



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
                            dgvSale.Rows[j].Cells["CPCName"].Value = dgvReceiveHistory["CPCNameP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["HSCode"].Value = dgvReceiveHistory["HSCodeP", i].Value.ToString();


                            if (!chkTrading.Checked)
                            {
                                dgvSale.Rows[j].Cells["CConvDate"].Value = dgvReceiveHistory["CConvDateP", i].Value.ToString();
                            }


                            #region currency convertion date change for export
                            //if (rbtnExport.Checked || ChkExpLoc.Checked)
                            //{
                            //    dgvSale.Rows[j].Cells["VATAmount"].Value = dgvReceiveHistory["VATAmountP", i].Value.ToString();
                            //    dgvSale.Rows[j].Cells["SubTotal"].Value = dgvReceiveHistory["SubTotalP", i].Value.ToString();
                            //    dgvSale.Rows[j].Cells["SDAmount"].Value = dgvReceiveHistory["SDAmountP", i].Value.ToString();
                            //}
                            #endregion currency convertion date change for export

                            //if (rbtnCN.Checked || rbtnDN.Checked)
                            //{
                            //    dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                            //}

                            j = j + 1;
                        }
                    }
                    //if (rbtnExport.Checked || ChkExpLoc.Checked)
                    //{
                    //    GTotal();
                    //}
                    //else
                    //{
                    Rowcalculate();
                    //}
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
            txtQty2.Text = Program.ParseDecimalObject(txtQty2.Text.Trim()).ToString();
            TotalQty();
            //if (Program.CheckingNumericTextBox(txtQty2, "Total Quantity") == true)
            //{
            //    txtQty2.Text = Program.FormatingNumeric(txtQty2.Text.Trim(), SalePlaceQty).ToString();
            //    TotalQty();
            //}
        }

        private void txtQty1_Leave(object sender, EventArgs e)
        {
            //////if (rbtnCN.Checked)
            //////{
            //////    decimal quantity = Convert.ToDecimal(dgvSale.CurrentRow.Cells["Quantity"].Value);
            //////    if (Convert.ToDecimal(txtQty1.Text) > quantity)
            //////    {
            //////        MessageBox.Show("Credit Note can not be greater than actual quantity.");
            //////        txtQty1.Text = "0";
            //////        txtQty1.Focus();
            //////        return;
            //////    }
            //////}
            txtQty1.Text = Program.ParseDecimalObject(txtQty1.Text.Trim()).ToString();
            TotalQty();
            //if (Program.CheckingNumericTextBox(txtQty1, "Total Quantity") == true)
            //{
            //    txtQty1.Text = Program.FormatingNumeric(txtQty1.Text.Trim(), SalePlaceQty).ToString();
            //    TotalQty();
            //}

            //////if (rbtnService.Checked)
            //////{
            //////    txtNBRPrice.Focus();
            //////}
            //////else if (rbtnServiceNS.Checked)
            //////{
            //////    txtNBRPrice.Focus();

            //////}
            //////else if (rbtnExport.Checked)
            //////{
            //////    txtNBRPrice.Focus();

            //////}
            //////else
            //////{
            //////    btnAdd.Focus();

            //////}

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
                if (rbtnServiceNS.Checked)
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

        private void btnTesting_Click(object sender, EventArgs e)
        {
            string fileName = "";
            if (chkSame.Checked == false)
            {
                BrowsFile();
                fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }
            }

            #region new process for bom import

            string[] extention = fileName.Split(".".ToCharArray());
            string[] retResults = new string[4];

            retResults = ImportTestingFromExcel();

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
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    Program.ImportFileName = fdlg.FileName;
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
                ccDate = dtpChallanDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            }
            else
            {
                ccDate = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            }
            CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            //ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);

            string[] cValues = new string[] { cmbCurrency.Text.Trim(), "Y", ccDate };
            string[] cFields = new string[] { "cc.CurrencyRate like", "cc.ActiveStatus like", "cc.ConversionDate like" };
            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, false,connVM);

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

            dtpConversionDate.Value = Convert.ToDateTime(ccDate);

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

        private DataSet LoadFromExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                ds = new ImportDAL().GetDataSetFromExcel(paramVm);
                //ds = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF).GetDataSetFromExcel(paramVm);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return ds;
        }

        private string[] ImportInspectionFromExcel()
        {
            //progressBar1.Visible = true;
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
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                bool AutoItem = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item",connVM) == "Y" ? true : false);
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return sqlResults;
                }
                DataTable dtSaleM = new System.Data.DataTable();

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                //                          + "Data Source=" + fileName + ";"
                //                          + "Extended Properties=" + "\""
                //                          + "Excel 12.0;HDR=YES;" + "\"";
                //theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter adapterSaleM = new OleDbDataAdapter("SELECT * FROM [Sales$]", theConnection);

                //adapterSaleM.Fill(dtSaleM);

                //theConnection.Close();

                dtSaleM = LoadFromExcel().Tables[0];
                #endregion Load Excel
                dtSaleM.Columns.Add("Transection_Type");
                dtSaleM.Columns.Add("Created_By");
                dtSaleM.Columns.Add("LastModified_By");
                dtSaleM.Columns.Add("BranchId");
                dtSaleM.Columns.Add("IsExport");
                dtSaleM.Columns.Add("Type");
                foreach (DataRow row in dtSaleM.Rows)
                {
                    if (chkExport.Checked)
                    {
                        row["IsExport"] = "Y";
                    }
                    else
                    {
                        row["IsExport"] = "N";
                    }
                    row["Type"] = cmbVATType.Text.Trim();
                    row["BranchId"] = Program.BranchId;
                    row["Transection_Type"] = transactionType;
                    row["Created_By"] = Program.CurrentUser;
                    row["LastModified_By"] = Program.CurrentUser;

                }
                SAVE_DOWORK_SUCCESS = false;
                CommonDAL cmnDal = new CommonDAL();
                //ICommon cmnDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                string companyCode = cmnDal.settingValue("CompanyCode", "Code",connVM);
                dtSale = new DataTable();
                DataRow[] dtCompanyRows = null;
                if (companyCode == "549")
                {
                    dtCompanyRows = dtSaleM.Select("[BU] in ('549105', '549107', '549108', '549113' )");
                }
                else if (companyCode == "246")
                {
                    dtCompanyRows = dtSaleM.Select("[BU] in ('246105', '246107', '246108', '246113' )");
                }
                if (dtCompanyRows.Length == 0)
                {
                    MessageBox.Show("There is no data to import", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return sqlResults;
                }
                dtSale = dtCompanyRows.CopyToDataTable();
                //bgwImport.RunWorkerAsync();

                BureauSaleDAL saleDal = new BureauSaleDAL();
                //IBureauSale saleDal = OrdinaryVATDesktop.GetObject<BureauSaleDAL, BureauSaleRepo, IBureauSale>(OrdinaryVATDesktop.IsWCF);

                sqlResults = saleDal.ImportInspectionData(dtSale, vNumberofItems,connVM);
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
            #endregion
            finally
            {
                //if (theConnection != null)
                //{
                //    theConnection.Close();

                //}
                //progressBar1.Visible = false;
            }
            return sqlResults;
        }
        private void bgwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            BureauSaleDAL saleDal = new BureauSaleDAL();
            //IBureauSale saleDal = OrdinaryVATDesktop.GetObject<BureauSaleDAL, BureauSaleRepo, IBureauSale>(OrdinaryVATDesktop.IsWCF);

            sqlResults = saleDal.ImportInspectionData(dtSale, vNumberofItems,connVM);
        }

        private void bgwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SAVE_DOWORK_SUCCESS = true;
        }
        private string[] ImportTestingFromExcel()
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
                OleDbDataAdapter adapterSaleM = new OleDbDataAdapter("SELECT * FROM [Test$]", theConnection);
                DataTable dtSaleM = new System.Data.DataTable();
                adapterSaleM.Fill(dtSaleM);

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

                //BureauSaleDAL saleDal = new BureauSaleDAL();
                //sqlResults = saleDal.ImportTestingData(dtSaleM);
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

        private void btnInspection_Click(object sender, EventArgs e)
        {
            try
            {
                //progressBar1.Visible = true;
                string fileName = "";
                if (chkSameIns.Checked == false)
                {
                    BrowsFile();
                    fileName = Program.ImportFileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        MessageBox.Show("Please select the right file for import");
                        return;
                    }
                }

                #region new process for bom import

                string[] extention = fileName.Split(".".ToCharArray());
                string[] retResults = new string[4];

                retResults = ImportInspectionFromExcel();

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
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //progressBar1.Visible = false;
            }

        }

        private void chkExport_CheckedChanged(object sender, EventArgs e)
        {
            cmbVATType.Text = "VAT";
            cmbVATType.Enabled = true;
            cmbVATType.Items.Remove("VAT");
            cmbVATType.Items.Remove("Non VAT");
            cmbVATType.Items.Remove("Export");
            cmbVATType.Items.Remove("DeemExport");

            if (chkExport.Checked)
            {
                cmbVATType.Items.Add("Export");
                cmbVATType.Items.Add("DeemExport");
                cmbVATType.Text = "Export";
                //cmbVATType.Enabled=false;
            }
            else
            {
                cmbVATType.Items.Add("VAT");
                cmbVATType.Items.Add("Non VAT");
                cmbVATType.Text = "VAT";
            }
        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
            txtQuantityInHand.Text = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
        }

        private void txtTenderStock_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTenderStock, "TenderStock");
        }

        private void txtHSCode_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HSCodeLoad_New();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void HSCodeLoad_New()
        {
            try
            {

                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select 

 Id
,HSCode
,Description
,CD
,RD
,SD
,VAT
,AT
,AIT
,OtherSD
,OtherVAT
,isnull(IsFixedCD,'N')IsFixedCD
,isnull(IsFixedRD,'N')IsFixedRD
,isnull(IsFixedSD,'N')IsFixedSD
,isnull(IsFixedVAT,'N')IsFixedVAT
,isnull(IsFixedAT,'N')IsFixedAT
,isnull(IsFixedAIT,'N')IsFixedAIT
,isnull(IsFixedOtherSD,'N')IsFixedOtherSD
,isnull(IsFixedOtherVAT,'N')IsFixedOtherVAT
,isnull(IsVDS,'N')IsVDS

from HSCodes
where IsArchive=0 ";

                string SQLTextRecordCount = @" select count(HSCode)RecordNo from HSCodes";

                string[] shortColumnName = { "HSCode", "Description", "CD", "RD", "SD", "VAT", "AT", "AIT", "OtherSD", "OtherVAT", "IsFixedVAT", "IsFixedSD", "IsFixedCD", "IsFixedRD", "IsFixedAIT", "IsFixedAT", "IsFixedOtherVAT", "IsFixedOtherSD", "IsVDS" };
                string tableName = "";

                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {

                    txtHSCode.Text = "";
                    txtHSCode.Text = selectedRow.Cells["HSCode"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "HSCodeLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void txtHSCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                HSCodeLoad_New();
            }
        }

        private void dgvSale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F2))
            {
                // if (rbtnDN.Checked || rbtnCN.Checked || rbtnRCN.Checked)
                if (rbtnCN.Checked)
                {
                    try
                    {
                        DataGridViewSelectedRowCollection selectedRows = dgvSale.SelectedRows;
                        if (selectedRows != null && selectedRows.Count > 0)
                        {
                            int selectedrowindex = dgvSale.SelectedCells[0].RowIndex;
                            DataGridViewRow selectedRow = dgvSale.Rows[selectedrowindex];

                            string result = FormBureauVeritasDN_CNAdjustment.SelectOne(selectedRow);

                            string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());

                            bool IsAll = Convert.ToBoolean(PrintResult[14]);
                            bool IsInvoiceNo = Convert.ToBoolean(PrintResult[11]);
                            bool IsInvoiceDate = Convert.ToBoolean(PrintResult[12]);
                            bool IsReason = Convert.ToBoolean(PrintResult[13]);

                            if (IsAll == true)
                            {
                                #region ApplyAll
                                for (int i = 0; i < dgvSale.RowCount; i++)
                                {
                                    if (IsInvoiceNo)
                                    {
                                        dgvSale["PreviousSalesInvoiceNo", i].Value = PrintResult[0];
                                    }

                                    if (IsInvoiceDate)
                                    {
                                        dgvSale["PreviousInvoiceDateTime", i].Value = Convert.ToDateTime(PrintResult[1]);
                                    }

                                    if (IsReason)
                                    {
                                        dgvSale["ReasonOfReturn", i].Value = PrintResult[10];
                                    }
                                }
                                #endregion
                            }
                            else
                            {

                                dgvSale["PreviousSalesInvoiceNo", dgvSale.CurrentRow.Index].Value = PrintResult[0];
                                dgvSale["PreviousInvoiceDateTime", dgvSale.CurrentRow.Index].Value = Convert.ToDateTime(PrintResult[1]);
                                dgvSale["PreviousNBRPrice", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[2]);
                                dgvSale["PreviousQuantity", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[3]);
                                dgvSale["PreviousSubTotal", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[4]);
                                dgvSale["PreviousVATAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[5]);
                                dgvSale["PreviousVATRate", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[6]);
                                dgvSale["PreviousSD", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[7]);
                                dgvSale["PreviousSDAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(PrintResult[8]);
                                dgvSale["PreviousUOM", dgvSale.CurrentRow.Index].Value = PrintResult[9];
                                dgvSale["ReasonOfReturn", dgvSale.CurrentRow.Index].Value = PrintResult[10];

                            }

                        }


                    }

                    catch (Exception ex)
                    {

                    }

                }
            }
        }

        private void btn6_7_Click(object sender, EventArgs e)
        {

        }





    }
}