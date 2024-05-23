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
using VATServer.Library;
using VATViewModel.DTOs;
using System.IO;
using System.Threading;
using SymphonySofttech.Reports;
using Newtonsoft.Json;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using System.Drawing.Printing;
using CrystalDecisions.Shared;
using VATServer.Library.Integration;

namespace VATClient
{
    public partial class FormSaleNestle : Form
    {
        #region Constructors

        public FormSaleNestle(string SalesInvoiceNo)
        {
            InitializeComponent();
            //dgvSale.DefaultCellStyle.Font = new Font("SutonnyMJ", 12, FontStyle.Italic);
             InvoiceNo = SalesInvoiceNo;

             connVM = Program.OrdinaryLoad();
            // DefaultCellStyle.Font.Name= "SutonnyMJ";
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
           

        }

        #endregion

        #region Global Variables
        private bool SearchPreviousForCNDN = false;
        CommonDAL commonDal = new CommonDAL();
        public DataSet DsFromMasterImport;
        public string tTypeFromMasterImport;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable dtSaleM = new System.Data.DataTable();
        private DataTable dtSaleD = new System.Data.DataTable();
        private DataTable dtSaleE = new System.Data.DataTable();
        CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
        private List<UomDTO> UOMs = new List<UomDTO>();
        private List<CurrencyConversionVM> CurrencyConversions = new List<CurrencyConversionVM>();
        string[] Condition = new string[] { "one" };
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private bool PrepaidVAT;
        private bool ChangeableNBRPrice;
        private bool ChangeableQuantity;
        private bool LocalInVAT1;
        private bool LocalInVAT1KaTarrif;
        private bool TenderInVAT1;
        private bool TenderInVAT1Tender;
        private bool TenderPriceWithVAT;
        private decimal vNBRPrice;
        private decimal vReverseUOMConversion;
        private decimal SelectedVATRate = 0;
        private DateTime tenderDate;
        private int numberOfItems;

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
        private string vSalePlaceQty, vChangeableNBRPrice, vChangeableQuantity, vSalePlaceTaka, vSalePlaceDollar, vSalePlaceRate,
                   vPrepaidVAT, vNumberofItems, vMultipleItemInsert, vCustomerWiseBOM, vPackingInExport, vLocalInVAT1
                   , vLocalInVAT1KaTarrif, vTenderInVAT1, vTenderInVAT1Tender, vIs3Plyer, vTenderPriceWithVAT, vNegativeStock
                   , DefaultVATTypeExport, DefaultVATTypeLocal, vDatabaseName, vCreditWithoutTransaction, vehicleRequired
                   , shiftRequired, vPGroupInReport, saleFromProduction = string.Empty, IsSourcePaid;

        private decimal cTotalValue = 0;
        private decimal cQuantity = 0;
        private decimal cATVablePrice = 0;
        private decimal cATVAmount = 0;
        private decimal cWareHouseRent = 0;
        private decimal cWareHouseVAT = 0;
        private decimal cVATRate = 0;
        private decimal cVATablePrice = 0;
        private decimal cVATAmount = 0;
        private decimal cATVRate;
        private decimal vVDSRatio;
        private decimal vVATRateForVDSRatio;
        private decimal vMinimumGrandTotalForVDS;

        private decimal OriginalHPSRate = 0;


        private int SearchBranchId = 0;
        //private decimal cWareHouseRentPerQuantity;
        //private decimal cTradeVATRate = Convert.ToDecimal(133.34);



        private bool NegativeStock;

        private bool CreditWithoutTransaction;
        private List<BomOhDTO> BomOHs = new List<BomOhDTO>();

        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();
        string InvoiceNo = string.Empty;
        private string IsCustomerExempted = "N";
        private string IsCustomerSpecialRate = "N";
        private string VAT11Name = String.Empty;
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
        private string ImportId = string.Empty;
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

        private string vehicleId = string.Empty;
        private string vehicleType = string.Empty;
        private string vehicleNo = string.Empty;
        private string vehicleActiveStatus = string.Empty;

        private DataTable vehicleResult;
        private DataTable shiftResult;

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
        private string ProductDropDownWidth = "";
        private string IsBlank = "";

        private string ReportNumberOfCopiesPrint = "1";
        private string ReportNumberOfCopiesPrint_Depo = "1";




        private bool ChangeData = false;
        private bool Post = false;
        private bool IsPost = false;
        private bool IsConvCompltd = true;
        private bool PreviewOnly = false;
        private bool IsUpdate = false;
        private string NextID = "";
        private decimal PreVATAmount = 0;
        private string item;
        private decimal cost;
        private decimal NBRPriceWithVAT = 0;
        private decimal cTradeVATRate = 0;

        private string CategoryId { get; set; }


        private DataTable SaleDetailResult;

        private string SaleDetailData = string.Empty;
        private string DatabaseName = string.Empty;

        private bool ImportByCustGrp;
        private bool CustomerWiseBOM = false;
        private bool CommercialImporter = false;
        private bool PackingInExport = false;



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
        //private string varTrading = string.Empty;
        private string post1, post2;
        private bool Add = false;
        private bool Edit = false;
        private string ReturnTransType = string.Empty;
        //private string CustomerGrpID = string.Empty;
        //private string CustomerGrpName = string.Empty;
        private DataSet ReportResultTracking;
        #endregion

        private DataTable dtTripLoad;

        #region Vabiables for tracking
        List<TrackingCmbDTO> trackingsCmb = new List<TrackingCmbDTO>();
        List<TrackingVM> trackingsVm = new List<TrackingVM>();
        private bool TrackingTrace;
        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;

        #endregion

        string ImportExcelID = string.Empty;

        FormIsPrinting frmIsPrinting = new FormIsPrinting();


        #endregion

        #region More Variables

        public static string vCustomerID = "0";
        public static string vItemNo = "0";
        public static string ItemNoD = "0";
        private string BranchId = "";
        private string BranchCode = "";
        private string BranchName = "";
        private string BranchLegalName = "";
        private string BranchAddress = "";
        private string BranchVatRegistrationNo = "";
        private const string DHL = "DHL";
        private const string KCL = "KCL";
        private ReportDocument reportDocument = null;

        #endregion

        #region Settings Variables

        private string DiscountInParcent = "Y";
        private string ExcludingVAT, VATTypeVATAutoChange = "Y";


        #endregion

        #region Methods 01 / Form Load, Form Maker

        private void FormSale_Load(object sender, EventArgs e)
        {
            try
            {
                
                
                
                    #region ToolTip

                    System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                    ToolTip1.SetToolTip(this.btnSearchInvoiceNo, "Existing information");
                    ToolTip1.SetToolTip(this.btnAddNew, "New");
                    ToolTip1.SetToolTip(this.btnSearchVehicle, "Vehicle");
                    ToolTip1.SetToolTip(this.btnProductType, "Product Type");

                    ToolTip1.SetToolTip(this.btnOldID, "Orginal Sales invoice no");
                    ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                    ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");
                    ToolTip1.SetToolTip(this.chkCustomerWiseBOM, "Customer Wise BOM");
                    ToolTip1.SetToolTip(this.btnCommentUpdate, "Comments Update");
                    ToolTip1.SetToolTip(this.txtSourcePaidQuantity, "Source Paid Quantity");
                    ToolTip1.SetToolTip(this.lSPQ, "Source Paid Quantity");

                    ToolTip1.SetToolTip(this.cmbShift, "Sale Shift");

                    #endregion

                    #region Reset Fields

                    ClearAllFields();

                    #endregion

                    #region VAT Name Load

                    VATName vname = new VATName();
                    cmbVAT1Name.DataSource = vname.VATNameList;

                    #endregion

                    #region Transaction Types

                    TransactionTypes();

                    #endregion

                    #region Form Making

                    FormMaker();

                    #endregion

                    #region Form Loading

                    FormLoad();

                    #endregion

                    #region Button Stats

                    txtSalesInvoiceNo.Text = "~~~ New ~~~";
                    //if(!rbtnExport.Checked)
                    Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                    Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                    Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
                    this.Enabled = false;

                    #endregion

                    #region Background Load

                    bgwLoad.RunWorkerAsync();

                    #endregion

                    #region Shift Load
                    //CommonDAL cDal = new CommonDAL();
                    //cmbShift.DataSource = null;
                    //cmbShift.Items.Clear();
                    //Condition = new string[0];
                    ShiftDAL _sDal = new ShiftDAL();

                    cmbShift.DataSource = _sDal.SearchForTime(DateTime.Now.ToString("HH:mm"),connVM);
                    cmbShift.DisplayMember = "ShiftName";// displayMember.Replace("[", "").Replace("]", "").Trim();
                    cmbShift.ValueMember = "Id";// valueMember.Trim();

                    //cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true);
                    #endregion

                    #region VAT Type Load

                    if (transactionType.ToLower() == "export")
                    {
                        VATTypeLoad(true);
                    }
                    else
                    {
                        VATTypeLoad(false);

                    }

                    #endregion

                    #region EXP Loc / Button Stats


                    if (!ChkExpLoc.Checked)
                    {
                        btnLCNo.Visible = false;
                        label39.Visible = false;
                        label13.Visible = false;
                        cmbCurrency.Visible = false;
                        txtBDTRate.Visible = false;
                        btnCurrencyEdit.Visible = false;

                    }

                    #endregion



                    #region Ref/Trip

                    string CompanyCode = commonDal.settingValue("CompanyCode", "Code",connVM);

                    if (CompanyCode == "SCBL")
                    {
                        label58.Visible = true;
                    }

                    #endregion
                    if (CompanyCode.ToUpper() == "NESTLE")
                    {
                        txtComments.Font = new Font("SutonnyMJ", 11, FontStyle.Regular);
                    }
                    if (!string.IsNullOrEmpty(InvoiceNo))
                    {
                        DataTable dataTable = new DataTable();
                        dataTable =new SaleDAL().SearchSalesHeaderDTNew(InvoiceNo, "", connVM);
                        SaleDataLoad(dataTable);
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSale_Load", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void VATTypeLoad(bool Export)
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicVATType = new Dictionary<string, string>();
            cmbVATType.DataSource = null;
            cmbVATType.Items.Clear();

            dicVATType = commonDal.TypeOfSaleVAT(Export);

            if (dicVATType != null && dicVATType.Count > 0)
            {
                cmbVATType.DataSource = new BindingSource(dicVATType, null);
                cmbVATType.DisplayMember = "Key";
                cmbVATType.ValueMember = "Value";
                cmbVATType.SelectedValue = Export == true ? DefaultVATTypeExport.ToLower() : DefaultVATTypeLocal.ToLower();
            }

        }

        private void TransactionTypes()
        {
            #region Transaction Type
            string a = transactionType;
            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }
            if (rbtnCN.Checked)
            {
                transactionType = "Credit";
            }
            if (rbtnDN.Checked)
            {
                transactionType = "Debit";
            }
            
            ////if (cmbVAT1Name.Text.Trim() == "VAT 4.3 (Wastage)")
            ////{
            ////    transactionType = "Wastage";
            ////}



            #endregion Transaction Type
        }

        private void FormMaker()
        {
            try
            {

                #region Settings

                DataTable SettingDt = new DataTable();
                DataTable SettingUserDt = new DataTable();

                DataRow[] settingRow;

                PrepaidVAT = false;
                numberOfItems = 0;

                #region Data load

                //string[] conditionFields = { "SettingGroup", "ActiveStatus" };
                //string[] conditionValues = { "Sale", "Y" };

                //SettingDt = new SettingDAL().SelectSettingAll(0, conditionFields, conditionValues);

                //if (UserInfoVM.IsMainSetting == null || UserInfoVM.IsMainSetting)
                //{
                //    SettingUserDt = SettingDt.Copy();
                //}
                //else
                //{
                //    string[] cFieldsSettingsRole = { "SettingGroup", "ActiveStatus", "UserId" };
                //    string[] cValuesSettingsRole = { "Sale", "Y", Program.CurrentUserID };

                //    SettingUserDt = new SettingRoleDAL().SelectSettingRoleAll(0, cFieldsSettingsRole, cValuesSettingsRole);
                //}

                #endregion


                #region comment 16-Sep-2020

                DiscountInParcent = commonDal.settingsDesktop("Sale", "DiscountInParcent",null,connVM);
                VATTypeVATAutoChange = commonDal.settingsDesktop("VATTypeVAT", "AutoChange",null,connVM);
                ExcludingVAT = commonDal.settingsDesktop("Sale", "ExcludingVAT",null,connVM);

                ReportNumberOfCopiesPrint = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);
                ReportNumberOfCopiesPrint_Depo = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint(Depo)",null,connVM);

                ProductDropDownWidth = commonDal.settingsDesktop("Product", "ProductDropDownWidth",null,connVM);
                IsBlank = commonDal.settingsDesktop("Sale", "IsBlank",null,connVM);
                DefaultVATTypeLocal = commonDal.settingsDesktop("DefaultVATType", "LocalSale",null,connVM);
                DefaultVATTypeExport = commonDal.settingsDesktop("DefaultVATType", "ExportSale",null,connVM);

                vMultipleItemInsert = commonDal.settingsDesktop("Sale", "MultipleItemInsert",null,connVM);
                IsSourcePaid = commonDal.settingsDesktop("Sale", "SourcePaid",null,connVM);
                vPGroupInReport = commonDal.settingsDesktop("Sale", "PGroupInReport",null,connVM);
                vehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired",null,connVM);


                shiftRequired = commonDal.settingsDesktop("Sale", "ShiftRequired",null,connVM);
                saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction",null,connVM);


                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3",null,connVM);
                //vCommercialImporter = commonDal.settingsDesktop("Sale", "CommercialImporter");
                vVDSRatio = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "VDSRatio",null,connVM));
                vVATRateForVDSRatio = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "VATRateForVDSRatio",null,connVM));
                vMinimumGrandTotalForVDS = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "MinimumGrandTotalForVDS",null,connVM));


                vDatabaseName = commonDal.settingsDesktop("DatabaseName", "DatabaseName",null,connVM);
                vCustomerWiseBOM = commonDal.settingsDesktop("Sale", "CustomerWiseBOM",null,connVM);
                vPackingInExport = commonDal.settingsDesktop("Sale", "PackingInExport",null,connVM);
                vChangeableNBRPrice = commonDal.settingsDesktop("Sale", "ChangeableNBRPrice",null,connVM);
                vChangeableQuantity = commonDal.settingsDesktop("Sale", "ChangeableQuantity",null,connVM);
                vSalePlaceQty = commonDal.settingsDesktop("Sale", "QuantityDecimalPlace",null,connVM);
                vSalePlaceTaka = commonDal.settingsDesktop("Sale", "TakaDecimalPlace",null,connVM);
                vSalePlaceDollar = commonDal.settingsDesktop("Sale", "DollerDecimalPlace",null,connVM);
                vSalePlaceRate = commonDal.settingsDesktop("Sale", "RateDecimalPlace",null,connVM);
                vPrepaidVAT = commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT",null,connVM);
                vNumberofItems = commonDal.settingsDesktop("Sale", "NumberOfItems",null,connVM);



                //PrintLocation = commonDal.settingsDesktop("Sale", "ReportSaveLocation");
                vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);

                vLocalInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3",null,connVM);
                vLocalInVAT1KaTarrif = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3Ka(Tarrif)",null,connVM);
                vTenderInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3",null,connVM);
                vTenderInVAT1Tender = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3(Tender)",null,connVM);
                vIs3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer",null,connVM);
                //for tender with vat
                vTenderPriceWithVAT = commonDal.settingsDesktop("PriceDeclaration", "TenderPriceWithVAT",null,connVM);
                vNegativeStock = commonDal.settingsDesktop("Sale", "NegStockAllow",null,connVM);
                // for import credit info
                vCreditWithoutTransaction = commonDal.settingsDesktop("Sale", "CreditWithoutTransaction",null,connVM);
                string vCustGrp = commonDal.settingsDesktop("ImportTender", "CustomerGroup",null,connVM);
                string LeaderPolicy = commonDal.settingsDesktop("Sale", "LeaderPolicy",null,connVM);


                #endregion

                if (string.IsNullOrEmpty(vSalePlaceQty)
                    || string.IsNullOrEmpty(vSalePlaceTaka)
                    || string.IsNullOrEmpty(vChangeableNBRPrice)
                    || string.IsNullOrEmpty(vSalePlaceDollar)
                    || string.IsNullOrEmpty(vSalePlaceRate)
                    || string.IsNullOrEmpty(vPrepaidVAT)
                    || string.IsNullOrEmpty(vNumberofItems)
                    || string.IsNullOrEmpty(vPrintCopy)
                    || string.IsNullOrEmpty(vCustomerWiseBOM)
                    || string.IsNullOrEmpty(vLocalInVAT1)
                    || string.IsNullOrEmpty(vLocalInVAT1KaTarrif)
                    || string.IsNullOrEmpty(vTenderInVAT1)
                    || string.IsNullOrEmpty(vTenderInVAT1Tender)
                    || string.IsNullOrEmpty(vIs3Plyer)
                    || string.IsNullOrEmpty(vTenderPriceWithVAT)
                    || string.IsNullOrEmpty(vNegativeStock)
                    || string.IsNullOrEmpty(vCreditWithoutTransaction)
                   || string.IsNullOrEmpty(vCustGrp)
                   || string.IsNullOrEmpty(vPackingInExport)
                   || string.IsNullOrEmpty(vDatabaseName)


                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                DatabaseName = vDatabaseName;
                CustomerWiseBOM = Convert.ToBoolean(vCustomerWiseBOM.ToString() == "Y" ? true : false);
                PackingInExport = Convert.ToBoolean(vPackingInExport.ToString() == "Y" ? true : false);
                SalePlaceQty = Convert.ToInt32(vSalePlaceQty);
                SalePlaceTaka = Convert.ToInt32(vSalePlaceTaka);
                SalePlaceDollar = Convert.ToInt32(vSalePlaceDollar);
                SalePlaceRate = Convert.ToInt32(vSalePlaceRate);
                numberOfItems = int.Parse(vNumberofItems);
                PrintCopy = Convert.ToInt32(vPrintCopy);
                PrepaidVAT = Convert.ToBoolean(vPrepaidVAT == "Y" ? true : false);
                ChangeableNBRPrice = Convert.ToBoolean(vChangeableNBRPrice == "Y" ? true : false);
                ChangeableQuantity = Convert.ToBoolean(vChangeableQuantity == "Y" ? true : false);



                LocalInVAT1 = Convert.ToBoolean(vLocalInVAT1 == "Y" ? true : false);
                LocalInVAT1KaTarrif = Convert.ToBoolean(vLocalInVAT1KaTarrif == "Y" ? true : false);
                TenderInVAT1 = Convert.ToBoolean(vTenderInVAT1 == "Y" ? true : false);
                TenderInVAT1Tender = Convert.ToBoolean(vTenderInVAT1Tender == "Y" ? true : false);
                Is3Plyer = vIs3Plyer;
                TenderPriceWithVAT = Convert.ToBoolean(vTenderPriceWithVAT == "Y" ? true : false);
                NegativeStock = Convert.ToBoolean(vNegativeStock == "Y" ? true : false);
                CreditWithoutTransaction = Convert.ToBoolean(vCreditWithoutTransaction == "Y" ? true : false);
                ImportByCustGrp = Convert.ToBoolean(vCustGrp == "Y" ? true : false);

                txtNBRPrice.ReadOnly = ChangeableNBRPrice == true ? false : true; ;
                txtQty1.ReadOnly = ChangeableQuantity == true ? false : true; ;

                string vTracking = string.Empty;
                string vHeading1, vHeading2 = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking",null,connVM);
                vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1",null,connVM);
                vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2",null,connVM);

                if (string.IsNullOrEmpty(vTracking) || string.IsNullOrEmpty(vHeading1) || string.IsNullOrEmpty(vHeading2))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

                if (IsSourcePaid == "Y")
                {
                    lSPQ.Visible = true;
                    txtSourcePaidQuantity.Visible = true;
                }
               

                #endregion Settings

                #region Transaction Type

                #region Close

                this.Text = "Local Sales";
                btnPrint.Text = "6.3";
                label22.Visible = true;
                btnExport.Visible = false;

                btn20.Visible = false;
                //btnDebitCredit.Top = btnPrint.Top;
                //btnDebitCredit.Left = btnPrint.Left;
                btnTenderNew.Visible = false;
                txtLineNo.Visible = false;


                label18.Visible = false;
                txtTenderStock.Visible = false;

                btnProductType.Enabled = true;
                //cmbVAT1Name.Visible = false; start
                label1.Visible = false;
                ////label64.Visible = false;

                //cmbVAT1Name.SelectedIndex = 0;
                //btnDebitCredit.Visible = false;
                chkIs11.Visible = false;
                chkIsBlank.Visible = false;
                //chkServiceStock.Visible = false;
                //txtNBRPrice.ReadOnly = false; //st

                txtQuantity.ReadOnly = false;
                txtQuantity.Text = "0";
                ChkExpLoc.Visible = false;
                btnFormKaT.Visible = true;
                #endregion Close
                chkValueOnly.Visible = false;
                chkValueOnly.Checked = false;

                #region  rbtnOther
                if (rbtnOther.Checked)
                {

                    chkIsBlank.Visible = true;
                    cmbType.Text = "New";
                    chkNonStock.Checked = false;
                    txtOldID.Visible = false;
                    btnOldID.Visible = false;
                    label27.Visible = false;

                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 6.3";

                    label1.Visible = true;

                    this.Text = "Sales Entry";
                   
                  

                }
                else if (rbtnCN.Checked)
                {
                    pnlCDN.Visible = true;


                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 6.7";

                    cmbType.Text = "Credit";
                    label2.Text = "Credit No";
                    //cmbVATType.Text = "VAT";
                    //btnDebitCredit.Visible = true;
                    //btnPrint.Visible = false;
                    //btnDebitCredit.Text = "VAT 12";
                    //cmbVAT1Name.Visible = true;
                    label1.Visible = true;
                    //cmbVAT1Name.SelectedIndex = 0;
                    //if (CreditWithoutTransaction == true)
                    //{
                    //    txtOldID.ReadOnly = false;
                    //    btnAdd.Enabled = true;
                    //    btnProductType.Enabled = true;
                    //}
                    //else
                    //{
                    btnAdd.Enabled = true;
                    btnProductType.Enabled = true;
                    //}
                    txtOldID.Visible = true;

                    this.Text = "Sales Credit Note Entry";
                    chkValueOnly.Visible = true;

                }
                #region  rbtnDN

                else if (rbtnDN.Checked)
                {
                    pnlCDN.Visible = true;

                    txtOldID.Visible = true;
                    btnPrint.Visible = true;
                    btnPrint.Text = "VAT 6.8";

                    cmbType.Text = "Debit";
                    label2.Text = "Debit No";
                    //cmbVATType.Text = "VAT";
                    //btnDebitCredit.Visible = true;
                    //cmbVAT1Name.Visible = true;
                    label1.Visible = true;
                    //cmbVAT1Name.SelectedIndex = 0;
                    //if (CreditWithoutTransaction == true)
                    //{
                    //    txtOldID.ReadOnly = false;
                    //    btnAdd.Enabled = true;
                    //    btnProductType.Enabled = true;
                    //}
                    //else
                    //{
                    btnAdd.Enabled = true;
                    btnProductType.Enabled = false;
                    //}

                    this.Text = "Sales Debit Note Entry";
                    chkValueOnly.Visible = true;



                }
                #endregion  rbtnDN
                #endregion

               


                #endregion Transaction Type

                #region Button Import Integration Lisence
                if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
                {
                    btnImport.Visible = false;
                }
                #endregion

                if (CommercialImporter)
                {
                    lbNBRPrice.Text = "Total Price";
                    txtTotalValue.Visible = true;
                    txtNBRPrice.Visible = false;
                }
                //cWareHouseRentPerQuantity = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "WareHouseRentPerQuantity"));
                cTradeVATRate = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "TradeVATRate",null,connVM));
                cATVRate = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "ATVRate",null,connVM));
                cATVRate = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "ATVRate",null,connVM));

                btnPrint.Enabled = true;

                #region Trading Lisence Issue
                if (Program.IsTrading == false)
                {
                    btnFormKaT.Visible = false;

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }

            #endregion


        }

        private void FormLoad()
        {

            
            #region Customer

            vCustomerID = "0";
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
            varInvoiceDate = Convert.ToDateTime(dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd"));

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
            else if (rbtnOther.Checked ) // start other
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

            #endregion Product

            #region 6.3ChkIsBlank
            if (IsBlank == "Y")
            {
                chkIsBlank.Checked = true;
            }
            else
            {
                chkIsBlank.Checked = false;

            }
            #endregion

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

            AlReadyPrintNo = 0;

            #region 2012 Law - Button Control    
                btnVAT17.Text = "6.2";
                btnVAT18.Visible = false;
                //btnPrint.Text = "6.3";
            #endregion

        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region UOM Functions
                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

                #region CurrencyConversion

                //CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
                ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);

                string[] cValue = new string[] { "Y", dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss") };
                string[] cField = new string[] { "cc.ActiveStatus like", "cc.ConversionDate<=" };
                CurrencyConversionResult = currencyConversionDal.SelectAll(0, cField, cValue, null, null, false, connVM);


                #endregion CurrencyConversion

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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {




                #region Setup VAT Status
                if (StatusResult != null && StatusResult.Tables.Count > 0 && StatusResult.Tables[0].Rows.Count > 0)
                {
                    txtTDBalance.Text = Convert.ToDecimal(StatusResult.Tables[0].Rows[0]["StartingVAT"]).ToString();//"0.00");
                    Program.VATAmount = Convert.ToDecimal(txtTDBalance.Text.Trim());
                }

                #endregion SetupVATStatus

                #region UOM Function

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

                #region Currency Conversion

                CurrencyConversions.Clear();

                foreach (DataRow item2 in CurrencyConversionResult.Rows)
                {
                    var cConversion = new CurrencyConversionVM();
                    cConversion.CurrencyConversionId = item2["CurrencyConversionId"].ToString();
                    cConversion.CurrencyCodeFrom = item2["CurrencyCodeFrom"].ToString();
                    cConversion.CurrencyCodeF = item2["CurrencyCodeF"].ToString();
                    cConversion.CurrencyNameF = item2["CurrencyNameFrom"].ToString();
                    cConversion.CurrencyCodeTo = item2["CurrencyCodeTo"].ToString();
                    cConversion.CurrencyCodeT = item2["CurrencyCodeT"].ToString();
                    cConversion.CurrencyNameT = item2["CurrencyNameTo"].ToString();
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

                    //cmbCurrency.Items.Insert(0, "USD");
                    string currencyText = commonDal.settingsDesktop("Sale", "DefaultCurrency",null,connVM);

                    cmbCurrency.Text = currencyText;
                    string cDate =
                        CurrencyConversions.Where(x => x.CurrencyCodeF == currencyText).Select(x => x.ConvertionDate).
                            FirstOrDefault();

                    dtpConversionDate.Value = Convert.ToDateTime(cDate);
                   
                        cmbCurrency.Items.Clear();
                        //cmbCurrency.Items.Insert(0, "BDT");
                        cmbCurrency.Text = "BDT";
                        cDate =
                            CurrencyConversions.Where(x => x.CurrencyCodeF == "BDT").Select(x => x.ConvertionDate).
                                FirstOrDefault();
                        //dtpConversionDate.Value = Convert.ToDateTime(cDate);

                        dtpConversionDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

                    

                   
                }

                #endregion CurrencyConversion

                txtNBRPrice.ReadOnly = ChangeableNBRPrice == true ? false : true;
                txtQty1.ReadOnly = ChangeableQuantity == true ? false : true; ;

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

        private void SetupVATStatus()
        {
            try
            {
                //string ReportData = dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd") + FieldDelimeter + LineDelimeter;

                varInvoiceDate = Convert.ToDateTime(dtpInvoiceDate.MaxDate.ToString("yyyy-MMM-dd"));


                //this.Enabled = false;

                backgroundWorkerSetupVATStatus.RunWorkerAsync();
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
                StatusResult = setupDal.ResultVATStatus(varInvoiceDate, Program.DatabaseName,connVM);


                //end DoWork
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
                else if (rbtnOther.Checked)
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
               
               
               
                #endregion Transaction Select


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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }

            #endregion
            finally
            {
                //cmbProductCode.Enabled = false;
                txtProductName.Enabled = false;
                btnProductType.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                bgwProduct.RunWorkerAsync();


            }
        }

        private void bgwProduct_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void bgwProduct_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            
        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {

            //ComboBox source = (ComboBox)sender;




            //#region Check Point

            //if (!string.IsNullOrEmpty(ProductName))
            //{
            //    if (ProductName == cmbProduct.Text)
            //    {
            //        return;
            //    }
            //}

            //#endregion

            //#region Initialization

            //SelectedVATRate = 0;

            //txtProductCode.Text = "";

            //#endregion


            //try
            //{

            //    string searchText = cmbProduct.Text.Trim();

            //    string ProductCode = "";
            //    if (searchText.Contains('~'))
            //    {
            //        ProductCode = searchText.Split('~')[1];
            //    }


            //    if (!string.IsNullOrWhiteSpace(ProductCode))
            //    {
            //        ProductDAL _ProductDal = new ProductDAL();

            //        DataTable dtProduct = new DataTable();

            //        #region Product Call

            //        dtProduct = _ProductDal.SelectProductDTAll(new[] { "Products.ProductCode" }, new[] { ProductCode });

            //        #endregion

            //        if (dtProduct != null && dtProduct.Rows.Count > 0)
            //        {

            //            DataRow drProduct = dtProduct.Rows[0];

            //            #region HPSRate

            //            OriginalHPSRate = Convert.ToDecimal(drProduct["HPSRate"].ToString());

            //            //if (cmbVATType.Text == "MRPRate(SC)")
            //            if (cmbVATType.SelectedValue.ToString().ToLower() == "mrprate(sc)")
            //            {
            //                txtHPSRate.Text = OriginalHPSRate.ToString();
            //            }
            //            else
            //            {
            //                txtHPSRate.Text = "0.00";
            //            }

            //            #endregion

            //            #region Value Assign to Form Elements

            //            #region Condtional Values

            //            txtProductName.Text = drProduct["ProductName"].ToString();// products.ProductName;
            //            txtProductDescription.Text = drProduct["ProductDescription"].ToString();// products.ProductName;
            //            if (string.IsNullOrWhiteSpace(txtProductDescription.Text.Trim())
            //                || txtProductDescription.Text.Trim() == "-"
            //                || txtProductDescription.Text.Trim() == "NA"
            //                || txtProductDescription.Text.Trim() == "N/A"
            //                )
            //            {
            //                txtProductDescription.Text = txtProductName.Text.Trim();
            //            }

            //            #endregion

            //            txtProductCode.Text = drProduct["ItemNo"].ToString();
            //            vItemNo = drProduct["ItemNo"].ToString();
            //            ItemNoD = vItemNo;
            //            txtVATRate.Text = drProduct["VATRate"].ToString();
            //            txtSD.Text = drProduct["SD"].ToString();
            //            if (transactionType != "Export")
            //            {
            //                cmbVATType.SelectedValue = VATTypeCal(drProduct["VATRate"].ToString()).ToLower();

            //            }

            //            #region Price Call

            //            if (rbtnTender.Checked == false)
            //            {
            //                var vNBRPriceCall = commonDal.settingsDesktop("Sale", "NBRPriceCall");
            //                if (vNBRPriceCall.ToLower() == "y")
            //                {
            //                    NBRPriceCall();
            //                }
            //            }

            //            #endregion

            //            txtUOM.Text = drProduct["UOM"].ToString();// products.UOM;
            //            cmbUom.Text = drProduct["UOM"].ToString();// products.UOM;
            //            txtHSCode.Text = drProduct["HSCodeNo"].ToString();// products.HSCodeNo;
            //            chkIsFixedVAT.Checked = false;
            //            txtFixedVATAmount.Text = "0";
            //            LVat.Text = "VAT(%)";
            //            LSD.Text = "SD(%)";
            //            txtFixedVATAmount.Text = "0";
            //            txtFixedSDAmount.Text = "0";
            //            chkIsFixedOtherSD.Checked = (drProduct["IsFixedSD"].ToString()) == "Y" ? true : false;
            //            chkIsFixedOtherVAT.Checked = (drProduct["IsFixedVAT"].ToString()) == "Y" ? true : false;


            //            if (chkIsFixedOtherSD.Checked)
            //            {
            //                LSD.Text = "SD(F)";

            //            }

            //            #endregion

            //            #region Conditional Values

            //            if (chkIsFixedOtherVAT.Checked)
            //            {
            //                txtFixedVATAmount.Visible = true;
            //                cmbVATType.SelectedValue = "fixedvat";
            //                LVat.Text = "VAT(F)";
            //                chkIsFixedVAT.Checked = true;
            //                txtFixedVATAmount.Text = Program.ParseDecimal(drProduct["FixedVATAmount"].ToString());
            //                txtVATRate.Text = Program.ParseDecimal(drProduct["FixedVATAmount"].ToString());

            //                if (cmbVATType.SelectedValue == "export")
            //                    txtFixedVATAmount.Text = "0";
            //                else if (cmbVATType.SelectedValue == "deemexport")
            //                    txtFixedVATAmount.Text = "0";
            //            }
            //            #endregion

            //            #region Stock / Avg Price

            //            txtQuantityInHand.Text = _ProductDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
            //                                                             DateTime.Now.ToString(" HH:mm:00"), null, null, true, true, true, false, null).Rows[0]["Quantity"].ToString();

            //            #endregion

            //            txttradingMarkup.Text = drProduct["TradingMarkUp"].ToString();
            //            txtPCode.Text = drProduct["ProductCode"].ToString();
            //            chkNonStock.Checked = drProduct["ItemNo"].ToString() == "Y" ? true : false;

            //            #region UOM Function

            //            Uoms();

            //            #endregion

            //            if (rbtnServiceNS.Checked == true)
            //            {
            //                txtQuantity.Text = "1";

            //            }

            //            #region TransactionType IssueWastage
            //            if (transactionType == "SaleWastage")
            //            {
            //                //////txtUnitCost.Text = "0";
            //                txtQuantityInHand.Text = "0";

            //                DataSet ds = new DataSet();
            //                ds = new ReportDSDAL().Wastage(vItemNo, "", "", "Y", "Y", "1900-Jan-01", DateTime.MaxValue.ToString("2100-Dec-31"), Program.BranchId, null);



            //                if (ds != null && ds.Tables.Count > 0)
            //                {
            //                    string WastageBalance = "0";
            //                    WastageBalance = ds.Tables[0].Rows[0]["WastageBalance"].ToString();

            //                    txtQuantityInHand.Text = WastageBalance;
            //                }


            //            }

            //            #endregion

            //        }

            //    }

            //    #region Export/DeemExport

            //    ReceiveDAL _sDal = new ReceiveDAL();
            //    DataTable dt = new DataTable();
            //    if (txtSerialNo.Text.Trim().Length >= 2)
            //    {
            //        dt = _sDal.SearchByReferenceNo(txtSerialNo.Text.Trim(), txtProductCode.Text.Trim(),null,transactionType);
            //        if (dt.Rows.Count > 0)
            //        {
            //            txtQty1.Text = Program.ParseDecimal(dt.Rows[0]["Quantity"].ToString());
            //            txtNBRPrice.Text = Program.ParseDecimal(dt.Rows[0]["CostPrice"].ToString());

            //        }
            //    }

            //    if (cmbVATType.SelectedValue == "export"
            //        || cmbVATType.SelectedValue == "deemexport"
            //        )
            //    {
            //        txtVATRate.Text = "0";
            //        txtSD.Text = "0";
            //        txtHPSRate.Text = "0";
            //        txtFixedVATAmount.Text = "0";
            //        txtFixedSDAmount.Text = "0";
            //    }

            //    //txtQty1.Focus();
            //    if (VATTypeVATAutoChange.ToLower() == "y")
            //    {
            //        SelectedVATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
            //    }
            //    #endregion

            //    #region CheckCustomerIsExempted

            //    if (IsCustomerExempted.ToLower() == "y")
            //    {
            //        txtSD.Text = "0";
            //        txtVATRate.Text = "0";

            //        cmbVATType.SelectedValue = VATTypeCal("0").ToLower();

            //    }

            //    #endregion

            //    vItemNo = "";
            //}
            //#region catch

            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            //}

            //#endregion
        }

        private string VATTypeCal(string vatRate)
        {
            try
            {
                decimal rate = Convert.ToDecimal(vatRate);

                string type = "Export";

                if (transactionType.ToLower() != "export")
                {
                    if (rate >= 15)
                    {
                        type = "VAT";
                    }
                    else if (rate == 0)
                    {
                        type = "NoNVAT";
                    }
                    else if (rate > 0 && rate < 15)
                    {
                        type = "OtherRate";
                    }
                }

                return type;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 02 / Add Row, Row Double Click, Change Row, Remove Row

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //no serverside method
            try
            {
                #region Initial Setup

                VATcalc();

                TransactionTypes();

                UomsValue();

                #endregion

                #region Check Point

                #region Check Point


                if (cmbVATType.SelectedValue == "nonvat")
                {
                    if (Convert.ToDecimal(Program.FormatingNumeric(txtVATRate.Text.Trim(), 2)) > 0)
                    {
                        MessageBox.Show("Please Check the VAT Rate as Zero(0)!", this.Text);
                        return;
                    }
                }

                if (vCustomerID == "0" || string.IsNullOrWhiteSpace(vCustomerID))
                {
                    MessageBox.Show("Please select the Customer!", this.Text);
                    return;
                }

                if (CommercialImporter)
                {
                    txtNBRPrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtTotalValue.Text.Trim()) / Convert.ToDecimal(txtQty1.Text.Trim())).ToString();// CommercialImporterCalculation(txtTotalValue.Text.Trim(), txtVATRate.Text.Trim(), txtQty1.Text.Trim()).ToString();
                }

                if (txtProductCode.Text.Trim() != vItemNo.Trim())
                {

                    if (CustomerWiseBOM)
                    {
                        //////if (transactionType != "TollIssue")
                        //////{
                        //////    MessageBox.Show("This item Not for the Selected Customer ", this.Text);
                        //////    return;
                        //////}
                    }

                }

                if (dgvSale.RowCount > numberOfItems - 1)
                {
                    MessageBox.Show("You can't add more items ", this.Text);
                    return;
                }


                if (txtUomConv.Text == "0")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                #endregion

                #region Null Ckeck

                //if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
                //{
                //    MessageBox.Show(MessageVM.msgNegPrice);

                //    txtNBRPrice.Focus();
                //    return;
                //}

                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item");
                    txtProductName.Focus();
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
                else if (txtHPSRate.Text == "")
                {
                    txtHPSRate.Text = "0";
                }

                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    //MessageBox.Show("Please input Quantity");
                    ////txtQuantity.Focus();
                    //txtQty1.Focus();
                    //return;
                }
                else if (Convert.ToDecimal(txtNBRPrice.Text) <= 0)
                {
                    //MessageBox.Show("Please declare the NBR Price");
                    ////txtQuantity.Focus();
                    //txtQty1.Focus();
                    //return;
                }


                //if (rbtnExport.Checked || rbtnExportPackage.Checked)
                //{
                //    if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) 
                //        || txtBDTRate.Text.Trim() == "0" 
                //        || string.IsNullOrEmpty(txtDollerRate.Text.Trim()) 
                //        || txtDollerRate.Text.Trim() == "0")
                //    {
                //        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                //        return;
                //    }
                //}

                //else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                //{
                //    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                //    return;
                //}
                #endregion Null Ckeck

                #region Multiple Insert

                if (vMultipleItemInsert.ToLower() == "n")
                {
                    for (int i = 0; i < dgvSale.RowCount; i++)
                    {
                        if (dgvSale.Rows[i].Cells["ItemNo"].Value.ToString().Trim() == txtProductCode.Text)
                        {
                            MessageBox.Show("Same Product already exist.", this.Text);
                            txtProductName.Focus();
                            return;
                        }
                    }
                }

                #endregion

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Set DataGridView

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvSale.Rows.Add(NewRow);

                int Index = dgvSale.RowCount - 1;

                SetDataGridView(Index, "New");


                #endregion

                #region Row Calculation

                Rowcalculate();
                LoadCDNAmount();
                #endregion

                #region Selecting Last Row

                selectLastRow();

                #endregion

                #region Focusing on Product

                txtProductName.Focus();

                #endregion

                #region Reset/Clear Product Entry/Detail Header Panel

                ResetDetailHead();

                #endregion

                #region Currency Dropdown Control

                //cmbCurrency.Enabled = false;

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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }

            #endregion
        }

        private void SetDataGridView(int paramIndex, string RowType)
        {

            try
            {
                #region Set DataGridView

                dgvSale["SourcePaidQuantity", paramIndex].Value = string.IsNullOrWhiteSpace(txtSourcePaidQuantity.Text.ToString()) ? "0" : txtSourcePaidQuantity.Text.ToString();
                dgvSale["BOMId", paramIndex].Value = string.IsNullOrEmpty(txtBOMId.Text.ToString()) ? "0" : txtBOMId.Text.ToString();
                dgvSale["BOMReferenceNo", paramIndex].Value = cmbBOMReferenceNo.Text.ToString();
                dgvSale["ItemNo", paramIndex].Value = txtProductCode.Text.Trim();
                dgvSale["ItemName", paramIndex].Value = txtProductName.Text.Trim();
                dgvSale["PCode", paramIndex].Value = txtPCode.Text.Trim();
                dgvSale["Comments", paramIndex].Value = "NA";

                #region Formatting

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


                #endregion

                dgvSale["UnitPrice", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString());
                dgvSale["VATRate", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATRate.Text.Trim()).ToString());
                dgvSale["SD", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSD.Text.Trim())).ToString();
                dgvSale["VATAmount", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtFixedVATAmount.Text.Trim()).ToString());
                dgvSale["SDAmount", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtFixedSDAmount.Text.Trim()).ToString());


                #region NBRPrice Calculation

                decimal Quantity = 0;
                decimal DeclaredNBRPrice = 0;
                decimal NBRPrice = 0;
                decimal NBRPriceInclusiveVAT = 0;
                decimal UomConv = 0;
                decimal DiscountAmount = 0;
                decimal UOMPrice = 0;
                decimal VATRate = 0;
                decimal SDRate = 0;

                Quantity = Convert.ToDecimal(txtQuantity.Text.Trim());


             


                UomConv = Convert.ToDecimal(txtUomConv.Text.Trim());
                UOMPrice = Convert.ToDecimal(txtNBRPrice.Text);
                NBRPrice = UOMPrice * UomConv;


                VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                SDRate = Convert.ToDecimal(txtSD.Text.Trim());

                if (ExcludingVAT == "N")
                {
                    NBRPriceInclusiveVAT = Convert.ToDecimal(txtNBRPrice.Text);
                    NBRPrice = ((((NBRPriceInclusiveVAT * 100) / (100 + VATRate)) * 100) / (100 + SDRate)); //// NBRPriceInclusiveVAT * (VATRate / (100 + VATRate));
                }




                #endregion

                dgvSale["Quantity", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQty1.Text.Trim())).ToString();
                dgvSale["SaleQuantity", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQty1.Text.Trim())).ToString();
                dgvSale["PromotionalQuantity", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQty2.Text.Trim())).ToString();
                dgvSale["UOMPrice", paramIndex].Value = Program.ParseDecimalObject(UOMPrice).ToString();
                dgvSale["UOMc", paramIndex].Value = OrdinaryVATDesktop.ParseDecimal_DecimalPlace(UomConv, 4).ToString();
                dgvSale["NBRPrice", paramIndex].Value = Program.ParseDecimalObject(NBRPrice).ToString();
                dgvSale["NBRPriceInclusiveVAT", paramIndex].Value = Program.ParseDecimalObject(NBRPriceInclusiveVAT).ToString();
                dgvSale["UOM", paramIndex].Value = cmbUom.Text.Trim();
                dgvSale["UOMn", paramIndex].Value = txtUOM.Text.Trim();
                dgvSale["UOMQty", paramIndex].Value = (Quantity * UomConv).ToString();

                dgvSale["DiscountAmount", paramIndex].Value = txtDiscountAmountInput.Text;
                dgvSale["DiscountedNBRPrice", paramIndex].Value = DeclaredNBRPrice.ToString();

                #region Conditional Values

                dgvSale["Status", paramIndex].Value = "New";

                if (RowType != "New")
                {
                    dgvSale["Status", dgvSale.CurrentRow.Index].Value = "Change";
                    dgvSale.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
                    dgvSale.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                }

                #endregion

                dgvSale["Stock", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString());
                dgvSale["Previous", paramIndex].Value = 0;
                dgvSale["Change", paramIndex].Value = 0;
                dgvSale["TradingMarkUp", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txttradingMarkup.Text.Trim()).ToString());
                dgvSale["Trading", paramIndex].Value = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                dgvSale["ValueOnly", paramIndex].Value = Convert.ToString(chkValueOnly.Checked ? "Y" : "N");
                dgvSale["Type", paramIndex].Value = cmbVATType.SelectedValue.ToString().Trim();
                dgvSale["VATName", paramIndex].Value = cmbVAT1Name.Text.Trim();
                if (txtWeight.Text.Trim()=="")
                {
                    dgvSale["Weight", paramIndex].Value = "0-" + Program.ParseDecimalObject(Convert.ToDecimal(txtQty1.Text.Trim())).ToString();

                }
                else
                {
                    dgvSale["Weight", paramIndex].Value = txtWeight.Text.Trim();

                }

                #region Conditional Values

                if (!chkTrading.Checked)
                {
                    dgvSale["CConvDate", paramIndex].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                }

                #endregion

                dgvSale["TotalValue", paramIndex].Value = cTotalValue.ToString();
                dgvSale["WareHouseRent", paramIndex].Value = cWareHouseRent.ToString();
                dgvSale["WareHouseVAT", paramIndex].Value = cWareHouseVAT.ToString();
                dgvSale["ATVRate", paramIndex].Value = cATVRate.ToString();
                dgvSale["ATVablePrice", paramIndex].Value = cATVablePrice.ToString();
                dgvSale["ATVAmount", paramIndex].Value = cATVAmount.ToString();
                dgvSale["IsCommercialImporter", paramIndex].Value = (CommercialImporter == true ? "Y" : "N");
                dgvSale["ProductDescription", paramIndex].Value = txtProductDescription.Text.ToString();
                dgvSale["IsFixedVAT", paramIndex].Value = Convert.ToString(chkIsFixedVAT.Checked ? "Y" : "N");
                dgvSale["FixedVATAmount", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtFixedVATAmount.Text.Trim()).ToString());
                dgvSale["HPSRate", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtHPSRate.Text.Trim()).ToString());

                #region New Add For DN/CN
                if (dgvSale["PreviousSalesInvoiceNo", paramIndex].Value == null)
                {
                    dgvSale["PreviousSalesInvoiceNo", paramIndex].Value = "";
                }
                if (dgvSale["PreviousInvoiceDateTime", paramIndex].Value == null)
                {
                    dgvSale["PreviousInvoiceDateTime", paramIndex].Value = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                }
                if (dgvSale["PreviousNBRPrice", paramIndex].Value == null)
                {
                    dgvSale["PreviousNBRPrice", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousQuantity", paramIndex].Value == null)
                {
                    dgvSale["PreviousQuantity", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousUOM", paramIndex].Value == null)
                {
                    dgvSale["PreviousUOM", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousSubTotal", paramIndex].Value == null)
                {
                    dgvSale["PreviousSubTotal", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousVATAmount", paramIndex].Value == null)
                {
                    dgvSale["PreviousVATAmount", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousVATRate", paramIndex].Value == null)
                {
                    dgvSale["PreviousVATRate", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousSD", paramIndex].Value == null)
                {
                    dgvSale["PreviousSD", paramIndex].Value = "0";
                }
                if (dgvSale["PreviousSDAmount", paramIndex].Value == null)
                {
                    dgvSale["PreviousSDAmount", paramIndex].Value = "0";
                }
                if (dgvSale["ReasonOfReturn", paramIndex].Value == null)
                {
                    dgvSale["ReasonOfReturn", paramIndex].Value = "N/A";
                }

                #endregion

                #region Conditional Values

               
                    dgvSale["ReturnTransactionType", paramIndex].Value = "Other";

                #endregion

                #region Leader policy
                if (dgvSale["IsLeader", paramIndex].Value == null)
                {
                    dgvSale["IsLeader", paramIndex].Value = "NA";
                }
                if (dgvSale["LeaderAmount", paramIndex].Value == null)
                {
                    dgvSale["LeaderAmount", paramIndex].Value =0;
                }
                if (dgvSale["LeaderVATAmount", paramIndex].Value == null)
                {
                    dgvSale["LeaderVATAmount", paramIndex].Value =0;
                }
                if (dgvSale["NonLeaderAmount", paramIndex].Value == null)
                {
                    dgvSale["NonLeaderAmount", paramIndex].Value = 0;
                }
                if (dgvSale["NonLeaderVATAmount", paramIndex].Value == null)
                {
                    dgvSale["NonLeaderVATAmount", paramIndex].Value =0;
                }
                #endregion

                #endregion

            }
            catch (Exception e)
            {
                throw e;
            }
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
                            #region Value Assign

                            txtSourcePaidQuantity.Text = userSelRow.Cells["SourcePaidQuantity"].Value.ToString();

                            txtBOMId.Text = userSelRow.Cells["BOMId"].Value.ToString();
                            txtProductName.Text = userSelRow.Cells["ItemName"].Value.ToString() + "~" +
                                              userSelRow.Cells["PCode"].Value.ToString();
                            txtLineNo.Text = dgvSale.CurrentCellAddress.Y.ToString();
                            txtProductCode.Text = userSelRow.Cells["ItemNo"].Value.ToString();
                            txtPCode.Text = userSelRow.Cells["PCode"].Value.ToString();
                            txtProductName.Text = userSelRow.Cells["ItemName"].Value.ToString();
                            txtQuantity.Text = Convert.ToDecimal(userSelRow.Cells["Quantity"].Value).ToString();//"0,0.0000");

                            txtQty1.Text = Convert.ToDecimal(userSelRow.Cells["Quantity"].Value).ToString();
                            //txtQty1.Text = Convert.ToDecimal(Convert.ToDecimal(userSelRow.Cells["Quantity"].Value) -
                            //                    Convert.ToDecimal(userSelRow.Cells["PromotionalQuantity"].Value)).ToString();
                            txtQty2.Text = Convert.ToDecimal(userSelRow.Cells["PromotionalQuantity"].Value).ToString();//"0,0.0000");

                            txtUnitCost.Text = Convert.ToDecimal(userSelRow.Cells["UnitPrice"].Value).ToString();//"0.00");
                            txtVATRate.Text = Convert.ToDecimal(userSelRow.Cells["VATRate"].Value).ToString();//"0.00");
                            txtCommentsDetail.Text = "NA";


                            #region NBR Price Reverse Calculation


                            decimal Quantity = 0;
                            decimal DeclaredNBRPrice = 0;
                            decimal NBRPrice = 0;
                            decimal UomConv = 0;
                            decimal DiscountAmount = 0;
                            decimal UOMPrice = 0;
                            decimal NBRPriceInclusiveVAT = 0;

                            Quantity = Convert.ToDecimal(userSelRow.Cells["Quantity"].Value);
                            DeclaredNBRPrice = Convert.ToDecimal(userSelRow.Cells["DiscountedNBRPrice"].Value);
                            DiscountAmount = Convert.ToDecimal(userSelRow.Cells["DiscountAmount"].Value);
                            UomConv = Convert.ToDecimal(userSelRow.Cells["UOMc"].Value);
                            UOMPrice = Convert.ToDecimal(userSelRow.Cells["UOMPrice"].Value); ////DeclaredNBRPrice - DiscountAmount;
                            NBRPrice = Convert.ToDecimal(userSelRow.Cells["NBRPrice"].Value); //// UOMPrice* UomConv;////UOMc
                            NBRPriceInclusiveVAT = Convert.ToDecimal(userSelRow.Cells["NBRPriceInclusiveVAT"].Value); //// UOMPrice* UomConv;////UOMc

                            NBRPrice = NBRPrice / UomConv;
                            NBRPrice = NBRPrice + DiscountAmount;

                            txtNBRPrice.Text = NBRPrice.ToString();


                            if (ExcludingVAT == "N")
                            {
                                txtNBRPrice.Text = NBRPriceInclusiveVAT.ToString();
                            }

                            #endregion

                            txtUOM.Text = userSelRow.Cells["UOMn"].Value.ToString();
                            cmbUom.Items.Insert(0, txtUOM.Text.Trim());

                            #region UOM Function

                            Uoms();

                            #endregion

                            cmbUom.Text = userSelRow.Cells["UOM"].Value.ToString();
                            txtSD.Text = Convert.ToDecimal(userSelRow.Cells["SD"].Value).ToString();//"0.00");
                            txtPrevious.Text = Convert.ToDecimal(userSelRow.Cells["Previous"].Value).ToString();//"0.00");
                            txtUomConv.Text = Convert.ToDecimal(userSelRow.Cells["UOMc"].Value).ToString();//"0.00");
                            txtDiscountAmountInput.Text = Convert.ToDecimal(userSelRow.Cells["DiscountAmount"].Value).ToString();//"0.00");
                            cmbVAT1Name.Text = userSelRow.Cells["VATName"].Value.ToString();
                            cmbVAT1Name.SelectedItem = userSelRow.Cells["VATName"].Value.ToString().Trim();
                            txtWeight.Text = userSelRow.Cells["Weight"].Value.ToString().Trim();
                            txtTotalValue.Text = userSelRow.Cells["SubTotal"].Value.ToString().Trim();
                            txtProductDescription.Text = userSelRow.Cells["ProductDescription"].Value.ToString().Trim();
                            chkIsFixedVAT.Checked = Convert.ToBoolean(userSelRow.Cells["IsFixedVAT"].Value.ToString().Trim() == "Y" ? true : false);
                            chkValueOnly.Checked = Convert.ToBoolean(userSelRow.Cells["ValueOnly"].Value.ToString().Trim() == "Y" ? true : false);

                            #region Conditional Values

                            if (userSelRow.Cells["VATName"].Value.ToString().Trim() == "VAT 4.3 (Wastage)")
                            {
                                //cmbVAT1Name.SelectedIndex = 10;
                            }
                            if (!chkTrading.Checked)
                            {
                                dtpConversionDate.Value = Convert.ToDateTime(userSelRow.Cells["CConvDate"].Value.ToString());

                                CurrencyConversiondate = userSelRow.Cells["CConvDate"].Value.ToString();

                                dtpConversionDate_Leave(sender, e);
                            }

                            ProductDAL productDal = new ProductDAL();
                            if (cmbVAT1Name.SelectedIndex != 10)
                            {
                                txtQuantityInHand.Text =
                                productDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                 DateTime.Now.ToString(" HH:mm:00"), null, null, true, true, true, false,connVM,Program.CurrentUserID).Rows[0]["Quantity"].ToString();
                            }
                            else
                            {
                                txtQuantityInHand.Text = "0";
                            }

                            chkDiscount.Checked = false;

                            

                            if (transactionTypeOld == "Service" || transactionTypeOld == "ServiceNS" || txtCategoryName.Text == "Export" || transactionTypeOld == "Export")
                            {
                                txtNBRPrice.Text = Convert.ToDecimal(userSelRow.Cells["NBRPrice"].Value).ToString();
                            }

                            #endregion

                            cmbVATType.SelectedValue = userSelRow.Cells["Type"].Value.ToString().Trim().ToLower();
                            txtWeight.Text = userSelRow.Cells["Weight"].Value.ToString().Trim();
                            vItemNo = txtProductCode.Text;
                            txtHPSRate.Text = Convert.ToDecimal(userSelRow.Cells["HPSRate"].Value).ToString();

                            #endregion

                            #region ReferenceNo

                            DataTable dt = new DataTable();
                            dt = LoadBOM_Dt();

                            if (dt != null && dt.Rows.Count > 0)
                            {

                                cmbBOMReferenceNo.DataSource = dt;
                                cmbBOMReferenceNo.DisplayMember = "ReferenceNo";
                                cmbBOMReferenceNo.ValueMember = "ReferenceNo";
                                cmbBOMReferenceNo.SelectedIndex = 0;
                            }

                            BOMReference();

                            #endregion

                        }
                    }
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSale_DoubleClick", exMessage);
            }

            #endregion
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                #region  Initial Setup

                TransactionTypes();

                VATcalc();

                UomsValue();

                #endregion

                #region Check Point

                #region Validations and Null Check

                //if (cmbVATType.Text == "NonVAT")
                string varRate = txtVATRate.Text.Trim();
                if (cmbVATType.SelectedValue == "nonvat")
                {
                    if (Convert.ToDecimal(varRate) > 0)
                    {
                        MessageBox.Show("Please Check the VAT Rate as Zero(0)!", this.Text);
                        return;
                    }
                }

                if (CommercialImporter)
                {
                    txtNBRPrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtTotalValue.Text.Trim()) / Convert.ToDecimal(txtQty1.Text.Trim())).ToString();// CommercialImporterCalculation(txtTotalValue.Text.Trim(), txtVATRate.Text.Trim(), txtQty1.Text.Trim()).ToString();
                }

                if (dgvSale.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (vCustomerID == "0" || string.IsNullOrWhiteSpace(vCustomerID))
                {
                    MessageBox.Show("Please select the Customer!", this.Text);
                    return;
                }

                if (txtProductCode.Text.Trim() != vItemNo.Trim())
                {

                    if (CustomerWiseBOM)
                    {
                        ////if (transactionType != "TollIssue")
                        ////{
                        ////    MessageBox.Show("This item Not for the Selected Customer ", this.Text);
                        ////    return;
                        ////}
                    }

                }

                if (txtUomConv.Text == "0"
                    || string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtUomConv.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                //    if (!chkValueOnly.Checked)
                //    {
                //        MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        return;
                //    }

                //}
                else if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                //else if (rbtnExport.Checked || rbtnExportPackage.Checked)
                //{
                //    if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" || string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                //    {
                //        MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                //        return;
                //    }
                //}


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

                #endregion

                #region Comments --- Jan-20-2020

                ////#region Stock Check

                ////decimal StockValue, PreviousValue, ChangeValue, CurrentValue, tenderQty, purchaseQty;
                ////StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                ////PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                ////CurrentValue = Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text);

                ////decimal minValue = 0;
                ////if (Convert.ToDecimal(txtQuantityInHand.Text) < Convert.ToDecimal(txtTenderStock.Text))
                ////{
                ////    minValue = Convert.ToDecimal(txtQuantityInHand.Text);
                ////}
                ////else
                ////{
                ////    minValue = Convert.ToDecimal(txtTenderStock.Text);

                ////}

                ////if (rbtnTender.Checked)
                ////{
                ////    if (
                ////       Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                ////    {
                ////        MessageBox.Show("Stock Not available");
                ////        txtQuantity.Focus();
                ////        return;
                ////    }
                ////}
                ////else if (rbtnTradingTender.Checked)
                ////{
                ////    if (
                ////        Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                ////    {
                ////        MessageBox.Show("Stock Not available");
                ////        txtQuantity.Focus();
                ////        return;
                ////    }
                ////}
                ////else if (rbtnCN.Checked == false
                ////    && rbtnVAT11GaGa.Checked == false
                ////    && rbtnServiceNS.Checked == false
                ////    && rbtnExportServiceNS.Checked == false
                ////    && cmbVAT1Name.Text.Trim() != "VAT 4.3 (Wastage)")
                ////{
                ////    if (CurrentValue > PreviousValue)
                ////    {
                ////        if (
                ////            Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                ////        {
                ////            if (NegativeStock == false)
                ////            {
                ////                MessageBox.Show("Stock Not available");
                ////                txtQuantity.Focus();
                ////                return;
                ////            }
                ////        }
                ////    }
                ////}
                ////else if (cmbVAT1Name.Text.Trim() != "VAT 4.3 (Wastage)")
                ////{


                ////    if (CurrentValue <= PreviousValue)
                ////    {
                ////        if (rbtnCN.Checked == false)
                ////        {
                ////            if (
                ////                Convert.ToDecimal(PreviousValue - CurrentValue) >
                ////                Convert.ToDecimal(StockValue))
                ////            {
                ////                MessageBox.Show("Stock Not available");
                ////                txtQuantity.Focus();
                ////                return;
                ////            }
                ////        }
                ////    }
                ////    else
                ////    {
                ////        if (rbtnServiceNS.Checked == false)
                ////        {


                ////            MessageBox.Show("Credit Note can not be greater than actual quantity.");
                ////            txtQty1.Text = PreviousValue.ToString();
                ////            txtQty1.Focus();
                ////            return;
                ////        }
                ////    }


                ////}


                ////#endregion Stock Check

                #endregion

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Formating


                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), SalePlaceTaka).ToString();
                if (Program.CheckingNumericTextBox(txtVATRate, "txtVATRate") == true)
                    txtVATRate.Text = Program.FormatingNumeric(varRate, SalePlaceRate).ToString();
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

              
                #endregion

                ////Discount();

                #region Set DataGridView

                SetDataGridView(dgvSale.CurrentRow.Index, "Change");

                #endregion

                #region Row Calculation

                Rowcalculate();

                #endregion

                #region Reset Detail Head

                ResetDetailHead();

                #endregion

                #region Focus on Product Dropdown

               
                #endregion

                #region CDN Amount

                LoadCDNAmount();

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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }

            #endregion
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveSelectedRow();
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion

        }

        private void backgroundWorkerChallanSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            try
            {
                ISale saleDal = GetObject();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData, connVM);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerChallanSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            try
            {

                #region DatagridView Sale Load

                dgvSale.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SaleDetailResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);
                    dgvSale.Rows[j].Cells["SourcePaidQuantity"].Value = item["SourcePaidQuantity"].ToString();
                    dgvSale.Rows[j].Cells["SourcePaidVATAmount"].Value = item["SourcePaidVATAmount"].ToString();
                    dgvSale.Rows[j].Cells["NBRPriceInclusiveVAT"].Value = item["NBRPriceInclusiveVAT"].ToString();


                    dgvSale.Rows[j].Cells["CDNVATAmount"].Value = Program.ParseDecimalObject(item["CDNVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["CDNSDAmount"].Value = Program.ParseDecimalObject(item["CDNSDAmount"].ToString());
                    dgvSale.Rows[j].Cells["CDNSubtotal"].Value = Program.ParseDecimalObject(item["CDNSubtotal"].ToString());

                    dgvSale.Rows[j].Cells["BOMReferenceNo"].Value = item["BOMReferenceNo"].ToString();
                    dgvSale.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvSale.Rows[j].Cells["LineNo"].Value = item["InvoiceLineNo"].ToString();
                    dgvSale.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvSale.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvSale.Rows[j].Cells["SaleQuantity"].Value = Program.ParseDecimalObject(item["SaleQuantity"].ToString());
                    dgvSale.Rows[j].Cells["PromotionalQuantity"].Value = Program.ParseDecimalObject(item["PromotionalQuantity"].ToString());
                    dgvSale.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["SalesPrice"].ToString());
                    dgvSale.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["NBRPrice"].ToString());
                    dgvSale.Rows[j].Cells["UOM"].Value = item["UOM"].ToString(); // SaleDetailFields[6].ToString();
                    dgvSale.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    dgvSale.Rows[j].Cells["VATAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["SubTotal"].Value = "0";
                    dgvSale.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvSale.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    dgvSale.Rows[j].Cells["Status"].Value = "Old";
                    dgvSale.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvSale.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                    dgvSale.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    dgvSale.Rows[j].Cells["SDAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["Change"].Value = 0;
                    dgvSale.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    dgvSale.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();
                    dgvSale.Rows[j].Cells["TradingMarkUp"].Value = Program.ParseDecimalObject(item["tradingMarkup"].ToString());
                    dgvSale.Rows[j].Cells["Type"].Value = item["Type"].ToString();
                    dgvSale.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvSale.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                    dgvSale.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvSale.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    dgvSale.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());
                    dgvSale.Rows[j].Cells["DiscountAmount"].Value = Program.ParseDecimalObject(item["DiscountAmount"].ToString());
                    dgvSale.Rows[j].Cells["ValueOnly"].Value = item["ValueOnly"].ToString();
                    dgvSale.Rows[j].Cells["DiscountedNBRPrice"].Value = Program.ParseDecimalObject(item["DiscountedNBRPrice"].ToString());
                    dgvSale.Rows[j].Cells["DollerValue"].Value = Program.ParseDecimalObject(item["DollerValue"].ToString());
                    dgvSale.Rows[j].Cells["BDTValue"].Value = Program.ParseDecimalObject(item["CurrencyValue"].ToString());
                    dgvSale.Rows[j].Cells["VDSAmount"].Value = Program.ParseDecimalObject(item["VDSAmount"].ToString());
                    dgvSale.Rows[j].Cells["TotalValue"].Value = Program.ParseDecimalObject(item["TotalValue"].ToString());
                    dgvSale.Rows[j].Cells["WareHouseRent"].Value = Program.ParseDecimalObject(item["WareHouseRent"].ToString());
                    dgvSale.Rows[j].Cells["WareHouseVAT"].Value = Program.ParseDecimalObject(item["WareHouseVAT"].ToString());
                    dgvSale.Rows[j].Cells["ATVRate"].Value = Program.ParseDecimalObject(item["ATVRate"].ToString());
                    dgvSale.Rows[j].Cells["ATVablePrice"].Value = Program.ParseDecimalObject(item["ATVablePrice"].ToString());
                    dgvSale.Rows[j].Cells["ATVAmount"].Value = Program.ParseDecimalObject(item["ATVAmount"].ToString());
                    dgvSale.Rows[j].Cells["IsCommercialImporter"].Value = item["IsCommercialImporter"].ToString();
                    dgvSale.Rows[j].Cells["TradeVATRate"].Value = Program.ParseDecimalObject(item["TradeVATRate"].ToString());
                    dgvSale.Rows[j].Cells["TradeVATAmount"].Value = Program.ParseDecimalObject(item["TradeVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["TradeVATableValue"].Value = item["TradeVATableValue"].ToString();
                    dgvSale.Rows[j].Cells["HPSRate"].Value = Program.ParseDecimalObject(item["HPSRate"].ToString());
                    dgvSale.Rows[j].Cells["HPSAmount"].Value = Program.ParseDecimalObject(item["HPSAmount"].ToString());

                    dgvSale.Rows[j].Cells["IsLeader"].Value = item["IsLeader"].ToString();
                    dgvSale.Rows[j].Cells["LeaderAmount"].Value = Program.ParseDecimalObject(item["LeaderAmount"].ToString());
                    dgvSale.Rows[j].Cells["LeaderVATAmount"].Value = Program.ParseDecimalObject(item["LeaderVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["NonLeaderAmount"].Value = Program.ParseDecimalObject(item["NonLeaderAmount"].ToString());
                    dgvSale.Rows[j].Cells["NonLeaderVATAmount"].Value = Program.ParseDecimalObject(item["NonLeaderVATAmount"].ToString());



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
                    dgvSale.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();
                    dgvSale.Rows[j].Cells["IsFixedVAT"].Value = item["IsFixedVAT"].ToString();
                    dgvSale.Rows[j].Cells["FixedVATAmount"].Value = Program.ParseDecimalObject(item["FixedVATAmount"].ToString());

                    #region New Field For DN/CN
                    dgvSale.Rows[j].Cells["PreviousSalesInvoiceNo"].Value = item["PreviousSalesInvoiceNo"].ToString();
                    dgvSale.Rows[j].Cells["PreviousInvoiceDateTime"].Value = item["PreviousInvoiceDateTime"].ToString();
                    dgvSale.Rows[j].Cells["PreviousNBRPrice"].Value = Program.ParseDecimalObject(item["PreviousNBRPrice"].ToString());
                    dgvSale.Rows[j].Cells["PreviousQuantity"].Value = Program.ParseDecimalObject(item["PreviousQuantity"].ToString());
                    dgvSale.Rows[j].Cells["PreviousUOM"].Value = item["PreviousUOM"].ToString();
                    dgvSale.Rows[j].Cells["PreviousSubTotal"].Value = Program.ParseDecimalObject(item["PreviousSubTotal"].ToString());
                    dgvSale.Rows[j].Cells["PreviousVATAmount"].Value = Program.ParseDecimalObject(item["PreviousVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["PreviousVATRate"].Value = Program.ParseDecimalObject(item["PreviousVATRate"].ToString());
                    dgvSale.Rows[j].Cells["PreviousSD"].Value = Program.ParseDecimalObject(item["PreviousSD"].ToString());
                    dgvSale.Rows[j].Cells["PreviousSDAmount"].Value = Program.ParseDecimalObject(item["PreviousSDAmount"].ToString());
                    dgvSale.Rows[j].Cells["ReasonOfReturn"].Value = item["ReasonOfReturn"].ToString();


                    #endregion

                    #region New Field For Leader Policy
                    dgvSale.Rows[j].Cells["IsLeader"].Value = item["IsLeader"].ToString();
                    dgvSale.Rows[j].Cells["LeaderAmount"].Value = Program.ParseDecimalObject(item["LeaderAmount"].ToString());
                    dgvSale.Rows[j].Cells["LeaderVATAmount"].Value = Program.ParseDecimalObject(item["LeaderVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["NonLeaderAmount"].Value = Program.ParseDecimalObject(item["NonLeaderAmount"].ToString());
                    dgvSale.Rows[j].Cells["NonLeaderVATAmount"].Value = Program.ParseDecimalObject(item["NonLeaderVATAmount"].ToString());

                    #endregion

                    #region currency convertion date change for export
             
                    dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = item["ReturnTransactionType"].ToString();
                    #endregion currency convertion date change for export
                    j = j + 1;
                }

                #endregion

                #region Row Calculation

               
                    Rowcalculate();
            

                #endregion

                #region Export Search

                ExportSearch();

                #endregion

                #region Flag Update

                IsUpdate = true;

                #endregion

                #region Pre VAT Amount

                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

                #endregion

                #region Load CDN Amount

                LoadCDNAmount();

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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
            finally
            {
                #region Button Stats

                ChangeData = false;
                //this.Enabled = true;

                this.btnFirst.Enabled = true;
                this.btnPrevious.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnLast.Enabled = true;

                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
                btnOldID.Enabled = true;

                #endregion
            }

        }

        private void ResetDetailHead()
        {

            #region Reset Detail Head
            txtDiscountAmountInput.Text = "0";
            txtSourcePaidQuantity.Text = "0";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtHSCode.Text = "";
            txtUnitCost.Text = "0.00";
            txtQuantity.Text = "0";
            txtQty1.Text = "0";
            txtQty2.Text = "0";
            txtVATRate.Text = "0.00";
            txtUOM.Text = "";
            txtHPSRate.Text = "0.00";
            txtBOMId.Text = "";
            txtQuantityInHand.Text = "0.00";

            #endregion


        }

        private void Rowcalculate()
        {

            try
            {
                #region Declarations

                decimal NBR = 0;
                decimal Quantity = 0;
                decimal SourcePaidQuantity = 0;
                decimal NewVATAmount = 0;
                decimal SourcePaidVATAmount = 0;
                decimal DiscountAmount = 0;
                decimal DiscountedNBRPrice = 0;
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
                decimal TradeVATableValue = 0;
                decimal TradeVATAmount = 0;
                decimal VDSAmountD = 0;
                decimal VDSAmount = 0;
                decimal Total = 0;
                decimal HPSRate = 0;
                decimal HPSAmount = 0;

                #endregion

                #region Checkpoint - Currency

     
               if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0")
                {
                    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                    return;
                }
                else
                {
                    CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());

                }


                if (Program.CheckingNumericString(dollerRate.ToString(), "dollerRate") == true)
                    dollerRate = Convert.ToDecimal(dollerRate.ToString());

                if (Program.CheckingNumericString(CurrencyRateFromBDT.ToString(), "CurrencyRateFromBDT") == true)
                    CurrencyRateFromBDT = Convert.ToDecimal(CurrencyRateFromBDT.ToString());

                #endregion

                string VATType = "";

                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    #region Value Assign

                    VATType = "";
                    VATType = dgvSale["Type", i].Value.ToString();
                    SD = Convert.ToDecimal(dgvSale["SD", i].Value);
                    VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);
                    HPSRate = Convert.ToDecimal(dgvSale["HPSRate", i].Value);
                    Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);

                    DiscountAmount = Convert.ToDecimal(dgvSale["DiscountAmount", i].Value);
                    DiscountedNBRPrice = Convert.ToDecimal(dgvSale["DiscountedNBRPrice", i].Value);
                   
                    #endregion

                    

                    #region Calculations and Formattings

                    Quantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                    SourcePaidQuantity = Convert.ToDecimal(dgvSale["SourcePaidQuantity", i].Value);
                    NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);
                    DiscountAmount = Convert.ToDecimal(dgvSale["DiscountAmount", i].Value);

                    //var tt = Convert.ToDecimal(dgvSale["SaleQuantity", i].Value);
                    if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                        Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
                    if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
                        NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

                    SumTotalS = Quantity * NBR;
                    if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
                        SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));


                    bDTValue = CurrencyRateFromBDT * SumTotalS;
                    if (Program.CheckingNumericString(bDTValue.ToString(), "bDTValue") == true)
                        bDTValue = Convert.ToDecimal(Program.FormatingNumeric(bDTValue.ToString(), SalePlaceDollar));

                    dgvSale["BDTValue", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(bDTValue));
                    dgvSale["DollerValue", i].Value = 0;
                    
                  

                    if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                        SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
                    if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
                        VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));
                    if (Program.CheckingNumericString(Trading.ToString(), "Trading") == true)
                        Trading = Convert.ToDecimal(Program.FormatingNumeric(Trading.ToString(), SalePlaceRate));
                    var IsFixedVAT = dgvSale["IsFixedVAT", i].Value.ToString();
                    var FixedVATAmount = Convert.ToDecimal(dgvSale["FixedVATAmount", i].Value);

                    #endregion

                    #region VAT/SD/TradingAmount Calculation

                   
                    
                        TradingAmount = SumTotalS * Trading / 100;
                        SDAmount = (SumTotalS + TradingAmount) * SD / 100;
                        VATAmount = (SumTotalS + TradingAmount + SDAmount) * VATRate / 100;
                        if (VATType == "FixedVAT".ToLower())
                        {
                            VATAmount = (Quantity) * VATRate;

                        }
                        else if (VATType == "MRPRate(SC)".ToLower())
                        {
                            VATAmount = (SumTotalS) * VATRate / 100;
                        }
                    #endregion

                    #region HPS Calculation

                    if (VATType == "MRPRate(SC)".ToLower())
                    {

                        HPSAmount = (SumTotalS) * HPSRate / 100;

                    }
                    else
                    {
                        HPSAmount = 0;
                    }

                    #endregion

                    #region CheckPoint - Conversion

                    if (VATType == "FixedVAT".ToLower())
                    {
                        VATAmount = (Quantity) * VATRate;

                    }
                    //if (chkIsFixedOtherVAT.Checked)
                    //{
                    //    VATAmount = Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                    //}
                    if (chkIsFixedOtherSD.Checked)
                    {
                        SDAmount = Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                    }

                    if (Program.CheckingNumericString(TradingAmount.ToString(), "TradingAmount") == true)
                    {
                        TradingAmount = Convert.ToDecimal(Program.FormatingNumeric(TradingAmount.ToString(), SalePlaceTaka));
                    }

                    if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                    {
                        SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), SalePlaceTaka));
                    }

                    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                    {
                        VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), SalePlaceTaka));
                    }

                    if (Program.CheckingNumericString(HPSAmount.ToString(), "HPSAmount") == true)
                    {
                        HPSAmount = Convert.ToDecimal(Program.FormatingNumeric(HPSAmount.ToString(), SalePlaceTaka));
                    }

                    if (Program.CheckingNumericString(TradeVATableValue.ToString(), "TradeVATableValue") == true)
                    {
                        TradeVATableValue = Convert.ToDecimal(Program.FormatingNumeric(TradeVATableValue.ToString(), SalePlaceTaka));
                    }

                    if (Program.CheckingNumericString(TradeVATAmount.ToString(), "VATAmount") == true)
                    {
                        TradeVATAmount = Convert.ToDecimal(Program.FormatingNumeric(TradeVATAmount.ToString(), SalePlaceTaka));
                    }

                    #endregion

                    #region Total Calculation

                    if (VATType == "MRPRate(SC)".ToLower())
                    {
                        Total = SumTotalS;
                    }
                    else
                    {
                        Total = SumTotalS + TradingAmount + SDAmount + VATAmount - DiscountAmount;
                    }

                    #endregion

                    if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                    {
                        Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), SalePlaceTaka));
                    }

                    #region Source Paid VAT Calculation

                    if (ExcludingVAT == "N")
                    {

                        if (SourcePaidQuantity > 0)
                        {

                            NewVATAmount = (Quantity - SourcePaidQuantity) * NBR * VATRate / (100 + VATRate);

                            SourcePaidVATAmount = (DiscountedNBRPrice * SourcePaidQuantity) * VATRate / (100 + VATRate);

                            VATAmount = NewVATAmount + SourcePaidVATAmount;

                        }

                    }
                    else
                    {
                        if (SourcePaidQuantity > 0)
                        {
                            NewVATAmount = (Quantity - SourcePaidQuantity) * NBR * VATRate;

                            SourcePaidVATAmount = (DiscountedNBRPrice * SourcePaidQuantity) * VATRate;

                            VATAmount = NewVATAmount + SourcePaidVATAmount;
                        }

                    }


                    #endregion

                    #region DGVSale Load

                    dgvSale[0, i].Value = i + 1;

                    dgvSale["SourcePaidQuantity", i].Value = Program.FormatingNumeric(SourcePaidQuantity.ToString());
                    dgvSale["SourcePaidVATAmount", i].Value = Program.FormatingNumeric(SourcePaidVATAmount.ToString());


                    dgvSale["TradeVATableValue", i].Value = Program.FormatingNumeric(Convert.ToDecimal(TradeVATableValue).ToString()); //"0,0.00");
                    dgvSale["TradeVATAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(TradeVATAmount).ToString()); //"0,0.00");
                    dgvSale["TradeVATRate", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(cTradeVATRate).ToString()); //"0,0.00");
                    dgvSale["VATAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(VATAmount).ToString()); //"0,0.00");
                    dgvSale["HPSAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(HPSAmount).ToString());

                    dgvSale["VATRate", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(VATRate).ToString()); //"0,0.00");


                    decimal AQuantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                    decimal PQuantity = Convert.ToDecimal(dgvSale["PromotionalQuantity", i].Value);
                    if (AQuantity == 0 && PQuantity>0)
                    {
                       
                            dgvSale["VATAmount", i].Value = "0"; //"0,0.00");
                        

                        dgvSale["SDAmount", i].Value = "0"; //"0,0.00");

                        dgvSale["SubTotal", i].Value = "0";
                        dgvSale["Total", i].Value = "0";//"0,0.00");
                    }
                    else
                    {
                        if (chkIsFixedOtherVAT.Checked)
                        {
                            dgvSale["VATAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(VATAmount).ToString()); //"0,0.00");
                        }

                        dgvSale["SDAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(SDAmount).ToString()); //"0,0.00");

                        dgvSale["SubTotal", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(SumTotalS).ToString());
                        dgvSale["Total", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(Total).ToString()); //"0,0.00");
                    }
                   


                    VDSAmountD = VATAmount;

                    if (VATRate == vVATRateForVDSRatio && vVDSRatio > 0)
                    {
                        VDSAmountD = VATAmount / vVDSRatio;
                    }

                    dgvSale["VDSAmount", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(VDSAmountD).ToString()); //"0,0.00");

                    #endregion

                    #region SubTotal Calculation

                    //SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                    //SumSDTotal = SumSDTotal + Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                    //SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                    //SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);
                    //VDSAmount = VDSAmount + Convert.ToDecimal(dgvSale["VDSAmount", i].Value);




                    #endregion

                    dgvSale["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvSale["Quantity", i].Value)
                                                                   - Convert.ToDecimal(dgvSale["Previous", i].Value)).ToString(); //"0,0.0000");

                }

                #region SubTotal Load

                #region Comments Nov-01-2020

                //txtTotalQuantity.Text = Convert.ToDecimal(Program.FormatingNumeric(TotalQuantuty.ToString(), 4)).ToString(); //"0,0.00");
                //txtTotalSubTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), 4)).ToString(); //"0,0.00");
                //txtTotalSDAmount.Text = Convert.ToDecimal(Program.FormatingNumeric(SumSDTotal.ToString(), 4)).ToString(); //"0,0.00");

                //txtTotalVATAmount.Text = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), 4)).ToString(); //"0,0.00");
                //txtTotalAmount.Text = Convert.ToDecimal(Program.FormatingNumeric(SumGTotal.ToString(), 4)).ToString(); //"0,0.00");

                //txtHPSTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(HPSTotalAmount.ToString(), 4)).ToString();

                #endregion

                CustomerVM vCustomerVM = new CustomerVM();

                if (vCustomerID == "0" || string.IsNullOrWhiteSpace(vCustomerID))
                {
                    MessageBox.Show("Please select the Customer!", this.Text);
                    return;
                }

                vCustomerVM = new CustomerDAL().SelectAllList(vCustomerID, null, null, null, null, null).FirstOrDefault();

                CompanyProfileVM vCompanyProfileVM = new CompanyProfileVM();
                vCompanyProfileVM = new CompanyprofileDAL().SelectAllList(null, null, null, null, null,connVM).FirstOrDefault();

                if (SumGTotal <= vMinimumGrandTotalForVDS)
                {
                    VDSAmount = 0;
                }
                else if (vCompanyProfileVM.IsVDSWithHolder == "N" && vCustomerVM.IsVDSWithHolder == "Y")
                {
                    ////Okay
                }
                else
                {
                    VDSAmount = 0;
                }

                txtVDSAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(Program.FormatingNumeric(VDSAmount.ToString(), 4)).ToString()); //"0,0.00");

                GTotal();

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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion
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
                vehicleResult = vehicleDal.SelectAll(0, cFields, cValues, null, null, true,connVM);

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
                decimal DiscountAmount=0;


                //if (string.IsNullOrEmpty(txtBDTRate.Text.Trim()) || txtBDTRate.Text.Trim() == "0" ||
                //    string.IsNullOrEmpty(txtDollerRate.Text.Trim()) || txtDollerRate.Text.Trim() == "0")
                //{
                //    MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                //    return;
                //}

                //else
                //{
                CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());

                //}



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
                int i = dgvSale.CurrentRow.Index;
                SD = Convert.ToDecimal(dgvSale["SD", i].Value);
                VATRate = Convert.ToDecimal(dgvSale["VATRate", i].Value);
                Trading = Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);

                

                Quantity = Convert.ToDecimal(dgvSale["Quantity", i].Value);
                NBR = Convert.ToDecimal(dgvSale["NBRPrice", i].Value);

                if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                    Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), SalePlaceQty));
                //if (Program.CheckingNumericString(NBR.ToString(), "NBR") == true)
                //    NBR = Convert.ToDecimal(Program.FormatingNumeric(NBR.ToString(), SalePlaceTaka));

                SumTotalS = Quantity * NBR;


                if (Program.CheckingNumericString(SumTotalS.ToString(), "SumTotalS") == true)
                    SumTotalS = Convert.ToDecimal(Program.FormatingNumeric(SumTotalS.ToString(), SalePlaceTaka));


                bDTValue = CurrencyRateFromBDT * SumTotalS;
                if (Program.CheckingNumericString(bDTValue.ToString(), "bDTValue") == true)
                    bDTValue = Convert.ToDecimal(Program.FormatingNumeric(bDTValue.ToString(), SalePlaceDollar));

                dgvSale["BDTValue", i].Value = Convert.ToDecimal(bDTValue);
                dgvSale["DollerValue", i].Value = 0;
               

                if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                    SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), SalePlaceRate));
                if (Program.CheckingNumericString(VATRate.ToString(), "VATRate") == true)
                    VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), SalePlaceRate));
                if (Program.CheckingNumericString(Trading.ToString(), "Trading") == true)
                    Trading = Convert.ToDecimal(Program.FormatingNumeric(Trading.ToString(), SalePlaceRate));

                TradingAmount = SumTotalS * Trading / 100;
                SDAmount = (SumTotalS + TradingAmount) * SD / 100;
                VATAmount = (SumTotalS + TradingAmount + SDAmount) * VATRate / 100;
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

            catch (Exception ex)
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
            decimal TotalQuantuty = 0;
            decimal TotalPQuantuty = 0;
            decimal HPSTotalAmount = 0;

            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {

                SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvSale["SubTotal", i].Value);
                SumSDTotal = SumSDTotal + Convert.ToDecimal(dgvSale["SDAmount", i].Value);
                SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvSale["VATAmount", i].Value);
                SumTrading = SumTrading + Convert.ToDecimal(dgvSale["TradingMarkUp", i].Value);
                SumGTotal = SumGTotal + Convert.ToDecimal(dgvSale["Total", i].Value);
                TotalQuantuty = TotalQuantuty + Convert.ToDecimal(dgvSale["Quantity", i].Value);
                TotalPQuantuty = TotalPQuantuty + Convert.ToDecimal(dgvSale["PromotionalQuantity", i].Value);
                HPSTotalAmount = HPSTotalAmount + Convert.ToDecimal(dgvSale["HPSAmount", i].Value);


            }
            
            txtTotalSubTotal.Text = Program.ParseDecimalObject(Convert.ToDecimal(SumSubTotal).ToString()); //"0,0.00");
            txtTotalSDAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(SumSDTotal).ToString()); //"0,0.00");

            txtTotalVATAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(SumVATAmount).ToString()); //"0,0.00");
            txtTotalAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(SumGTotal).ToString()); //"0,0.00");

            #region SubTotal Load

            txtTotalQuantity.Text = Program.ParseDecimalObject(Convert.ToDecimal(Program.FormatingNumeric((TotalQuantuty + TotalPQuantuty).ToString(), 4)).ToString()); //"0,0.00");

            txtHPSTotal.Text = Program.ParseDecimalObject(Convert.ToDecimal(Program.FormatingNumeric(HPSTotalAmount.ToString(), 4)).ToString());


            #endregion


        }

        private void RemoveSelectedRow()
        {
            if (dgvSale.RowCount > 0)
            {
                if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvSale.CurrentRow.Cells["PCode"].Value,
                        this.Text, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                    Rowcalculate();
                    ////GTotal();

                    LoadCDNAmount();
                }
            }
            else
            {
                MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
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


            if (rbtnOther.Checked == true)
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


            txtProductName.Focus();


            //return false;

        }

        #endregion

        #region Methods 03 / Save, Update, Post, Search Invoice

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

        public ISale GetObject()
        {
            if (Program.IsWCF.ToLower() == "y")
            {
                return new SaleRepo();
            }
            else
            {
                return new SaleDAL();

            }
        }

        private void ValueAssign()
        {
            try
            {

                #region Master Value Assign

                #region Next ID

                NextID = IsUpdate == false ? string.Empty : txtHiddenInvoiceNo.Text.Trim();

                #endregion

                saleMaster = new SaleMasterVM();
                saleMaster.SalesInvoiceNo = NextID;
                saleMaster.CustomerID = vCustomerID.Trim();
                saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                saleMaster.VehicleNo = txtVehicleNo.Text.Trim();
                saleMaster.VehicleType = txtVehicleType.Text.Trim();
                saleMaster.vehicleSaveInDB = true;
                saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                saleMaster.VDSAmount = Convert.ToDecimal(txtVDSAmount.Text.Trim());
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
                saleMaster.DeliveryDate = saleMaster.InvoiceDateTime;
                saleMaster.Post = "N";
                saleMaster.CurrencyID = txtCurrencyId.Text.Trim();
                saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                saleMaster.ImportIDExcel = txtImportID.Text.Trim(); //ImportID
                saleMaster.ShiftId = cmbShift.SelectedValue.ToString() == null ?"0": cmbShift.SelectedValue.ToString();
                saleMaster.EXPFormNo = txtEXPFormNo.Text.Trim();
                saleMaster.IsDeemedExport = chkIsDeemedExport.Checked ? "Y" : "N";
                saleMaster.BranchId = Program.BranchId;
                saleMaster.HPSTotalAmount = Convert.ToDecimal(txtHPSTotal.Text.Trim());

                if (string.IsNullOrWhiteSpace(txtDeductionAmount.Text))
                {
                    txtDeductionAmount.Text = "0";
                }

                saleMaster.DeductionAmount = Convert.ToDecimal(txtDeductionAmount.Text);

                saleMaster.AppVersion = Program.GetAppVersion();
                saleMaster.ImportIDExcel = txtImportID.Text.Trim(); //ImportID

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                saleMaster.SignatoryName = userInfo[0]["FullName"].ToString();
                saleMaster.SignatoryDesig = userInfo[0]["Designation"].ToString();
                #endregion

                #endregion

                #region Detail Value Assign

                saleDetails = new List<SaleDetailVm>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleDetailVm detail = new SaleDetailVm();

                    detail.SourcePaidQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidQuantity"].Value);
                    detail.SourcePaidVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidVATAmount"].Value);
                    detail.NBRPriceInclusiveVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPriceInclusiveVAT"].Value);
                    detail.BOMReferenceNo = Convert.ToString(dgvSale.Rows[i].Cells["BOMReferenceNo"].Value);
                    detail.BOMId = Convert.ToInt32(dgvSale.Rows[i].Cells["BOMId"].Value);
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
                    detail.VDSAmountD = Convert.ToDecimal(dgvSale.Rows[i].Cells["VDSAmount"].Value.ToString());
                    detail.SaleTypeD = cmbType.Text.Trim();
                    detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();
                    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                    //detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                    detail.ValueOnly = dgvSale.Rows[i].Cells["ValueOnly"].Value.ToString();
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
                    detail.TotalValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalValue"].Value.ToString());
                    detail.WareHouseRent = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseRent"].Value.ToString());
                    detail.WareHouseVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseVAT"].Value.ToString());
                    detail.ATVRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVRate"].Value.ToString());
                    detail.ATVablePrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVablePrice"].Value.ToString());
                    detail.ATVAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                    detail.IsCommercialImporter = dgvSale.Rows[i].Cells["IsCommercialImporter"].Value.ToString();
                    detail.TradeVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATRate"].Value.ToString());
                    detail.TradeVATableValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATableValue"].Value.ToString());
                    detail.TradeVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATAmount"].Value.ToString());
                    detail.CDNVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNVATAmount"].Value.ToString());
                    detail.CDNSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSDAmount"].Value.ToString());
                    detail.CDNSubtotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSubtotal"].Value.ToString());

                    if (!chkTrading.Checked)
                    {
                        detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                    }

                    detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();

                    detail.BranchId = Program.BranchId;
                    detail.ProductDescription = dgvSale.Rows[i].Cells["ProductDescription"].Value.ToString();
                    detail.IsFixedVAT = dgvSale.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                    detail.FixedVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["FixedVATAmount"].Value.ToString());

                    #region New Field For DN/CN

                    detail.PreviousInvoiceDateTime = dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value == null ? DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm") : dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value.ToString();
                    detail.PreviousNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value));
                    detail.PreviousQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value));
                    detail.PreviousSubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value));
                    detail.PreviousVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value));
                    detail.PreviousVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value));
                    detail.PreviousSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value));
                    detail.PreviousSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value));
                    detail.ReasonOfReturn = dgvSale.Rows[i].Cells["ReasonOfReturn"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                    detail.PreviousUOM = dgvSale.Rows[i].Cells["PreviousUOM"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["PreviousUOM"].Value.ToString();

                    #endregion

                    detail.HPSRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSRate"].Value));
                    detail.HPSAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSAmount"].Value));

                    #region LeaderPolicy
                    detail.IsLeader = dgvSale.Rows[i].Cells["IsLeader"].Value == null ? "NA" : dgvSale.Rows[i].Cells["IsLeader"].Value.ToString();
                    detail.LeaderAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["LeaderAmount"].Value== null ? 0 :Convert.ToDecimal(dgvSale.Rows[i].Cells["LeaderAmount"].Value));
                    detail.LeaderVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["LeaderVATAmount"].Value== null ? 0 :Convert.ToDecimal(dgvSale.Rows[i].Cells["LeaderVATAmount"].Value));
                    detail.NonLeaderAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["NonLeaderAmount"].Value== null ? 0 :Convert.ToDecimal(dgvSale.Rows[i].Cells["NonLeaderAmount"].Value));
                    detail.NonLeaderVATAmount =Convert.ToDecimal(dgvSale.Rows[i].Cells["NonLeaderVATAmount"].Value == null ? 0 :Convert.ToDecimal(dgvSale.Rows[i].Cells["NonLeaderVATAmount"].Value));
                    if (detail.Quantity == 0 && detail.PromotionalQuantity>0)
                    {
                        detail.ProductType = "P";
                    }
                    else
                    {
                        detail.ProductType = "R";
                    }
                    #endregion

                    saleDetails.Add(detail);
                }
                #endregion detail

                #region Export Value Assign

                if (ChkExpLoc.Checked)
                {
                    if (PackingInExport)
                    {
                        saleExport = new List<SaleExportVM>();

                        
                    }

                }
                #endregion export


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
                FileLogger.Log(this.Name, "ValueAssign", exMessage);
            }

            #endregion

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                #region Log Generation

                FileLogger.Log(this.Name, "Add", DateTime.Now.ToString("hh:mm:ss t z"));

                #endregion

                #region Transaction Types

                TransactionTypes();

                if (transactionType.ToLower() == "commercialimporter")
                {

                }

                #endregion

                #region Check Point
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
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #endregion

                #region NextID

                NextID = IsUpdate == false ? string.Empty : txtHiddenInvoiceNo.Text.Trim();

                #endregion start

                #region Check Point

                #region Exist Check

                SaleMasterVM vm = new SaleMasterVM();
                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new SaleDAL().SelectAllList(0, new[] { "sih.SalesInvoiceNo" }, new[] { NextID },null,null,null,connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #endregion

                #region Null Check

                //if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                //{
                //    MessageBox.Show("Please Select Shift");
                //    cmbShift.Focus();
                //    return;
                //}

                if (string.IsNullOrEmpty(txtCustomer.Text.Trim()))
                {
                    MessageBox.Show("Please Enter Customer Information");
                    txtCustomer.Focus();
                    return;
                }
                if (vCustomerID == "0")
                {
                    MessageBox.Show("Please Enter Customer Information");
                    txtCustomer.Focus();
                    return;
                }


                if (vehicleRequired.ToLower() == "y")
                {
                   
                        if (txtVehicleNo.Text == "")
                        {
                            MessageBox.Show("Please Enter Vehicle Number");
                            txtVehicleNo.Focus();
                            return;
                        }


                }
               

                if (cmbType.Text == "New")
                {
                    txtOldID.Text = "0.00";
                }
                else
                {
                   
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
                
                string code = commonDal.settingValue("CompanyCode", "Code",connVM);

                if (txtSerialNo.Text == "")
                {
                    if (code == "SCBL")
                    {
                        MessageBox.Show("Please Enter Production Ref/Trip Number");
                        txtSerialNo.Focus();
                        return;
                    }
                    else
                    {
                        txtSerialNo.Text = "-";

                    }
                }


                if (!string.IsNullOrEmpty(txtSerialNo.Text.Trim()) && txtSerialNo.Text.Trim() != "" && txtSerialNo.Text.Trim() != "-" && code != "SQR" && !OrdinaryVATDesktop.IsACICompany(code) && code != "CP")
                {
                    CommonDAL _CDAL = new CommonDAL();
                    if (_CDAL.DataAlreadyUsed("SalesInvoiceHeaders", "SerialNo", txtSerialNo.Text.Trim(), null, null,connVM) != 0)
                    {
                        MessageBox.Show("This Ref/Trip # Already Used." + " SalesInvoiceHeaders", this.Text);
                        return;
                    }
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

                #endregion

                #region Value Assign

                ValueAssign();

                #endregion

                #region Load CDN Amount

                LoadCDNAmount();

                #endregion

                #region Backup Oct-01-2020 / Value Assign

                #region Master Value Assign

                #region Backup Oct-01-2020


                ////saleMaster = new SaleMasterVM();
                ////saleMaster.SalesInvoiceNo = NextID.ToString();
                ////saleMaster.CustomerID = vCustomerID;
                ////saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                ////saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                ////saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                ////saleMaster.VehicleType = txtVehicleType.Text.Trim();
                ////saleMaster.VehicleNo = txtVehicleNo.Text.Trim();
                ////saleMaster.vehicleSaveInDB = true;
                ////saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.LCDate = dtpLCDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.LCBank = txtLCBank.Text.Trim();
                ////saleMaster.TotalSubtotal = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                ////saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                ////saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                ////saleMaster.VDSAmount = Convert.ToDecimal(txtVDSAmount.Text.Trim());
                ////saleMaster.SerialNo = txtSerialNo.Text.Trim();
                ////saleMaster.Comments = txtComments.Text.Trim();
                ////saleMaster.CreatedBy = Program.CurrentUser;
                ////saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.LastModifiedBy = Program.CurrentUser;
                ////saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.SaleType = cmbType.Text.Trim();
                ////saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                ////saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                ////saleMaster.IsPrint = "N";
                ////saleMaster.TenderId = TenderId;
                ////saleMaster.TransactionType = transactionType;
                ////saleMaster.DeliveryDate = saleMaster.InvoiceDateTime;
                ////saleMaster.Post = "N"; //Post
                ////saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                ////saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                ////saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate
                ////saleMaster.LCNumber = txtLCNumber.Text.Trim(); //Post
                ////saleMaster.ShiftId = cmbShift.SelectedValue.ToString(); //Post
                ////saleMaster.PINo = txtPINo.Text.Trim(); //Post
                ////saleMaster.PIDate = dtpPIDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.EXPFormNo = txtEXPFormNo.Text.Trim(); //Post
                ////saleMaster.EXPFormDate = dtpEXPFormDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.IsDeemedExport = chkIsDeemedExport.Checked ? "Y" : "N";
                ////saleMaster.AppVersion = Program.GetAppVersion();
                ////saleMaster.HPSTotalAmount = Convert.ToDecimal(txtHPSTotal.Text.Trim());


                ////saleMaster.BranchId = Program.BranchId;

                ////if (string.IsNullOrWhiteSpace(txtDeductionAmount.Text))
                ////{
                ////    txtDeductionAmount.Text = "0";
                ////}

                ////saleMaster.DeductionAmount = Convert.ToDecimal(txtDeductionAmount.Text);

                ////#region Find UserInfo

                ////DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                ////saleMaster.SignatoryName = userInfo[0]["FullName"].ToString();
                ////saleMaster.SignatoryDesig = userInfo[0]["Designation"].ToString();

                ////#endregion

                #endregion

                #endregion Master

                #region Detail Value Assign

                #region Backup Oct-01-2020

                ////saleDetails = new List<SaleDetailVM>();
                ////for (int i = 0; i < dgvSale.RowCount; i++)
                ////{
                ////    SaleDetailVM detail = new SaleDetailVM();

                ////    detail.SourcePaidQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidQuantity"].Value);
                ////    detail.SourcePaidVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidVATAmount"].Value);
                ////    detail.NBRPriceInclusiveVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPriceInclusiveVAT"].Value);
                ////    detail.BOMReferenceNo = Convert.ToString(dgvSale.Rows[i].Cells["BOMReferenceNo"].Value);
                ////    detail.BOMId = Convert.ToInt32(dgvSale.Rows[i].Cells["BOMId"].Value);
                ////    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                ////    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                ////    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                ////    detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());
                ////    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                ////    detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                ////    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                ////    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                ////    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                ////    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                ////    detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();
                ////    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                ////    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                ////    detail.VDSAmountD = Convert.ToDecimal(dgvSale.Rows[i].Cells["VDSAmount"].Value.ToString());
                ////    detail.SaleTypeD = cmbType.Text.Trim();

                ////    if (rbtnDN.Checked || rbtnCN.Checked || rbtnRCN.Checked)
                ////    {
                ////        detail.PreviousSalesInvoiceNoD = dgvSale.Rows[i].Cells["PreviousSalesInvoiceNo"].Value.ToString();
                ////    }
                ////    else
                ////    {
                ////        detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();

                ////    }

                ////    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                ////    detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                ////    detail.ValueOnly = dgvSale.Rows[i].Cells["ValueOnly"].Value.ToString();
                ////    detail.TradingMarkUp = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                ////    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                ////    detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                ////    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                ////    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                ////    detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                ////    detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                ////    detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                ////    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                ////    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                ////    detail.VatName = dgvSale.Rows[i].Cells["VATName"].Value.ToString();
                ////    detail.Weight = dgvSale.Rows[i].Cells["Weight"].Value.ToString();
                ////    detail.TotalValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalValue"].Value.ToString());
                ////    detail.WareHouseRent = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseRent"].Value.ToString());
                ////    detail.WareHouseVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseVAT"].Value.ToString());
                ////    detail.ATVRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVRate"].Value.ToString());
                ////    detail.ATVablePrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVablePrice"].Value.ToString());
                ////    detail.ATVAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                ////    detail.IsCommercialImporter = dgvSale.Rows[i].Cells["IsCommercialImporter"].Value.ToString();
                ////    detail.TradeVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATRate"].Value.ToString());
                ////    detail.TradeVATableValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATableValue"].Value.ToString());
                ////    detail.TradeVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATAmount"].Value.ToString());
                ////    detail.CDNVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNVATAmount"].Value.ToString());
                ////    detail.CDNSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSDAmount"].Value.ToString());
                ////    detail.CDNSubtotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSubtotal"].Value.ToString());

                ////    if (!chkTrading.Checked)
                ////    {
                ////        detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                ////    }

                ////    if (rbtnDN.Checked || rbtnCN.Checked || rbtnRCN.Checked)
                ////    {
                ////        detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                ////    }

                ////    detail.BranchId = Program.BranchId;
                ////    detail.ProductDescription = dgvSale.Rows[i].Cells["ProductDescription"].Value.ToString();
                ////    detail.IsFixedVAT = dgvSale.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                ////    detail.FixedVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["FixedVATAmount"].Value.ToString());

                ////    #region New Field For DN/CN

                ////    detail.PreviousInvoiceDateTime = dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value == null ? DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm") : dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value.ToString();
                ////    detail.PreviousNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value));
                ////    detail.PreviousQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value));
                ////    detail.PreviousSubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value));
                ////    detail.PreviousVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value));
                ////    detail.PreviousVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value));
                ////    detail.PreviousSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value));
                ////    detail.PreviousSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value));
                ////    detail.ReasonOfReturn = dgvSale.Rows[i].Cells["ReasonOfReturn"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                ////    detail.PreviousUOM = dgvSale.Rows[i].Cells["PreviousUOM"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["PreviousUOM"].Value.ToString();

                ////    #endregion

                ////    detail.HPSRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSRate"].Value));
                ////    detail.HPSAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSAmount"].Value));


                ////    saleDetails.Add(detail);
                ////}

                #endregion

                #endregion detail

                

                #endregion

                #region Check Point

                if (saleDetails.Count() <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                

                #region Buttons, ProgressBar Stats

                this.progressBar1.Visible = true;
                this.btnSave.Enabled = false;
                //this.Enabled = false;

                #endregion

                #region Background Worker Save

                backgroundWorkerSave.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion

        }

        private void backgroundWorkerSave_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            #region try

            try
            {

                #region statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                //string json = JsonConvert.SerializeObject(saleDetails);
                //List<SaleDetailVM> vms = JsonConvert.DeserializeObject<List<SaleDetailVM>>(json);

                //ISale saleDal = GetObject();


                //if (transactionType.ToLower() == "export" || transactionType.ToLower() == "other")
                //{
                //    SaleDALNew saleDal = new SaleDALNew();

                //    sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport, trackingsVm, null, null,
                //        Program.BranchId,
                //        connVM);
                //}
                //else
                //{
                //    ISale saleDal = GetObject();

                //    sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport, null, null, null, Program.BranchId, connVM);

                //}

                ISale saleDal = GetObject();

                sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport,trackingsVm, null, null, Program.BranchId, connVM);

                SAVE_DOWORK_SUCCESS = true;

                #endregion
            }

            #endregion

            #region catch

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
            #region try

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
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            FileLogger.Log(this.Name, "Add", DateTime.Now.ToString("hh:mm:ss t z"));

                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;

                        }
                        if (result == "Success")
                        {
                            txtSalesInvoiceNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvSale.RowCount; i++)
                            {
                                dgvSale["Status", dgvSale.RowCount - 1].Value = "Old";
                            }
                        }
                    }

                ChangeData = false;
                SearchBranchId = Program.BranchId;

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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }

            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
                this.btnSave.Enabled = true;
                //this.Enabled = true;

            }

            #endregion

        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                #region Log Generation

                FileLogger.Log(this.Name, "Update", DateTime.Now.ToString("hh:mm:ss t z"));

                #endregion

                #region Check Point

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #endregion

                #region TransactionTypes

                TransactionTypes();

                #endregion

                #region Check Point

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtHiddenInvoiceNo.Text.Trim();
                }

                #region Null Check

                //if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                //{
                //    MessageBox.Show("Please Select Shift");
                //    cmbShift.Focus();
                //    return;
                //}

                if (vCustomerID == "0")
                {

                    MessageBox.Show("Please Enter Customer Information");
                    txtCustomer.Focus();
                    return;
                }



                if (cmbType.Text == "New")
                {
                    txtOldID.Text = "0.00";
                }
                else
                {
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
               
                string code = commonDal.settingValue("CompanyCode", "Code",connVM);

                if (txtSerialNo.Text == "" || txtSerialNo.Text == "-")
                {
                    if (code == "SCBL")
                    {
                        MessageBox.Show("Please Enter Production Ref/Trip  Number");
                        txtSerialNo.Focus();
                        return;
                    }
                    else
                    {
                        txtSerialNo.Text = "-";

                    }
                }
                if (!string.IsNullOrEmpty(txtSerialNo.Text.Trim()) && txtSerialNo.Text.Trim() != "" && txtSerialNo.Text.Trim() != "-" && code != "SQR" && !OrdinaryVATDesktop.IsACICompany(code))
                {
                    CommonDAL _CDAL = new CommonDAL();
                    if (_CDAL.DataAlreadyUsedWithoutThis("SalesInvoiceHeaders", "SerialNo", txtSerialNo.Text.Trim(), "SalesInvoiceNo", txtHiddenInvoiceNo.Text.Trim(), null, null,connVM) != 0)
                    {
                        MessageBox.Show("This Ref/Trip # Already Used." + " SalesInvoiceHeaders", this.Text);
                        return;
                    }
                }

                #endregion

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

                #endregion

                #region Value Assign

                ValueAssign();

                #endregion

                #region Load CDN Amount

                LoadCDNAmount();

                #endregion

                #region Backup Oct-01-2020 / Value Assign

                #region Master Value Assign

                #region Backup Oct-01-2020

                ////saleMaster = new SaleMasterVM();
                ////saleMaster.SalesInvoiceNo = txtSalesInvoiceNo.Text.Trim();
                ////saleMaster.CustomerID = vCustomerID.Trim();
                ////saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                ////saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                ////saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                ////saleMaster.LCDate = dtpLCDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.LCBank = txtLCBank.Text.Trim();
                ////saleMaster.VehicleNo = txtVehicleNo.Text.Trim();
                ////saleMaster.VehicleType = txtVehicleType.Text.Trim();
                ////saleMaster.vehicleSaveInDB = true;
                ////saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                ////saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                ////saleMaster.VDSAmount = Convert.ToDecimal(txtVDSAmount.Text.Trim());
                ////saleMaster.SerialNo = txtSerialNo.Text.Trim();
                ////saleMaster.Comments = txtComments.Text.Trim();
                ////saleMaster.CreatedBy = Program.CurrentUser;
                ////saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.LastModifiedBy = Program.CurrentUser;
                ////saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.SaleType = cmbType.Text.Trim();
                ////saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                ////saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                ////saleMaster.IsPrint = "N";
                ////saleMaster.TenderId = TenderId;
                ////saleMaster.TransactionType = transactionType;
                ////saleMaster.DeliveryDate = saleMaster.InvoiceDateTime;
                ////saleMaster.Post = "N"; //Post
                ////saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                ////saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                ////saleMaster.ReturnId = txtDollerRate.Text.Trim();  // return ID is used for doller rate
                ////saleMaster.LCNumber = txtLCNumber.Text.Trim(); //Post
                ////saleMaster.ImportIDExcel = txtImportID.Text.Trim(); //ImportID
                ////saleMaster.PINo = txtPINo.Text.Trim(); //Post
                ////saleMaster.PIDate = dtpPIDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.ShiftId = cmbShift.SelectedValue.ToString(); //Post
                ////saleMaster.EXPFormNo = txtEXPFormNo.Text.Trim(); //Post
                ////saleMaster.EXPFormDate = dtpEXPFormDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////saleMaster.IsDeemedExport = chkIsDeemedExport.Checked ? "Y" : "N";
                ////saleMaster.BranchId = Program.BranchId;
                ////saleMaster.HPSTotalAmount = Convert.ToDecimal(txtHPSTotal.Text.Trim());

                ////if (string.IsNullOrWhiteSpace(txtDeductionAmount.Text))
                ////{
                ////    txtDeductionAmount.Text = "0";
                ////}

                ////saleMaster.DeductionAmount = Convert.ToDecimal(txtDeductionAmount.Text);

                ////#region Find UserInfo

                ////DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                ////saleMaster.SignatoryName = userInfo[0]["FullName"].ToString();
                ////saleMaster.SignatoryDesig = userInfo[0]["Designation"].ToString();
                ////#endregion

                #endregion

                #endregion

                #region Detail Value Assign

                #region Backup Oct-01-2020

                ////saleDetails = new List<SaleDetailVM>();

                ////for (int i = 0; i < dgvSale.RowCount; i++)
                ////{
                ////    SaleDetailVM detail = new SaleDetailVM();

                ////    detail.SourcePaidQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidQuantity"].Value);
                ////    detail.SourcePaidVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SourcePaidVATAmount"].Value);
                ////    detail.NBRPriceInclusiveVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPriceInclusiveVAT"].Value);

                ////    detail.BOMReferenceNo = Convert.ToString(dgvSale.Rows[i].Cells["BOMReferenceNo"].Value);
                ////    detail.BOMId = Convert.ToInt32(dgvSale.Rows[i].Cells["BOMId"].Value);
                ////    detail.CDNVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNVATAmount"].Value);
                ////    detail.CDNSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSDAmount"].Value);
                ////    detail.CDNSubtotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["CDNSubtotal"].Value);
                ////    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                ////    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                ////    detail.ValueOnly = dgvSale.Rows[i].Cells["ValueOnly"].Value.ToString();
                ////    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                ////    detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());
                ////    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                ////    detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                ////    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                ////    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                ////    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                ////    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                ////    detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();
                ////    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                ////    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                ////    var tt = dgvSale.Rows[i].Cells["VDSAmount"].Value.ToString();
                ////    detail.VDSAmountD = Convert.ToDecimal(dgvSale.Rows[i].Cells["VDSAmount"].Value.ToString());
                ////    detail.SaleTypeD = cmbType.Text.Trim();

                ////    if (rbtnDN.Checked || rbtnCN.Checked || rbtnRCN.Checked)
                ////    {
                ////        detail.PreviousSalesInvoiceNoD = dgvSale.Rows[i].Cells["PreviousSalesInvoiceNo"].Value.ToString();
                ////    }
                ////    else
                ////    {
                ////        detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();

                ////    }
                ////    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                ////    detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                ////    detail.TradingMarkUp = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                ////    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                ////    detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                ////    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                ////    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                ////    detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                ////    detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                ////    detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                ////    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                ////    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                ////    detail.VatName = dgvSale.Rows[i].Cells["VATName"].Value.ToString();
                ////    detail.Weight = dgvSale.Rows[i].Cells["Weight"].Value.ToString();
                ////    detail.TotalValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TotalValue"].Value.ToString());
                ////    detail.WareHouseRent = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseRent"].Value.ToString());
                ////    detail.WareHouseVAT = Convert.ToDecimal(dgvSale.Rows[i].Cells["WareHouseVAT"].Value.ToString());
                ////    detail.ATVRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVRate"].Value.ToString());
                ////    detail.ATVablePrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVablePrice"].Value.ToString());
                ////    detail.ATVAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["ATVAmount"].Value.ToString());
                ////    detail.IsCommercialImporter = dgvSale.Rows[i].Cells["IsCommercialImporter"].Value.ToString();
                ////    detail.TradeVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATRate"].Value.ToString());
                ////    detail.TradeVATableValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATableValue"].Value.ToString());
                ////    detail.TradeVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["TradeVATAmount"].Value.ToString());

                ////    if (!chkTrading.Checked)
                ////    {
                ////        detail.CConversionDate = dgvSale.Rows[i].Cells["CConvDate"].Value.ToString();
                ////    }
                ////    if (rbtnDN.Checked || rbtnCN.Checked || rbtnRCN.Checked)
                ////    {
                ////        detail.ReturnTransactionType = dgvSale.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                ////    }
                ////    detail.ProductDescription = dgvSale.Rows[i].Cells["ProductDescription"].Value.ToString();
                ////    detail.IsFixedVAT = dgvSale.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                ////    detail.FixedVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["FixedVATAmount"].Value.ToString());
                ////    detail.BranchId = Program.BranchId;

                ////    #region New Field For DN/CN

                ////    detail.PreviousInvoiceDateTime = dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value == null ? DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm") : dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value.ToString();
                ////    detail.PreviousNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value));
                ////    detail.PreviousQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value));
                ////    detail.PreviousSubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value));
                ////    detail.PreviousVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value));
                ////    detail.PreviousVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value));
                ////    detail.PreviousSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value));
                ////    detail.PreviousSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value));
                ////    detail.ReasonOfReturn = dgvSale.Rows[i].Cells["ReasonOfReturn"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                ////    detail.PreviousUOM = dgvSale.Rows[i].Cells["PreviousUOM"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["PreviousUOM"].Value.ToString();

                ////    #endregion

                ////    detail.HPSRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSRate"].Value);
                ////    detail.HPSAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["HPSAmount"].Value);

                ////    saleDetails.Add(detail);
                ////}

                #endregion

                #endregion

                #region Export Value Assign


                #endregion

                #endregion

                #region Check Point

                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion


                #region Buttons, ProgressBar Stats

                this.bthUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                #endregion

                #region Background Worker Update

                bgwUpdate.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement


                // Start DoWork
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                //if (transactionType.ToLower() == "export" || transactionType.ToLower() == "other")
                //{
                //    SaleDALNew saleDal = new SaleDALNew();

                //    sqlResults = saleDal.SalesUpdate(saleMaster, saleDetails, saleExport, null, connVM);
                //}
                //else
                //{
                //    ISale saleDal = GetObject();

                //    sqlResults = saleDal.SalesUpdate(saleMaster, saleDetails, saleExport, trackingsVm, connVM);
                //}


                //SaleDAL saleDal = new SaleDAL();

                ISale saleDal = GetObject();

                sqlResults = saleDal.SalesUpdate(saleMaster, saleDetails, saleExport, trackingsVm, connVM);
                UPDATE_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion

            }

            #endregion

            #region catch

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
            #region try

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
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

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

                FileLogger.Log(this.Name, "Update", DateTime.Now.ToString("hh:mm:ss t z"));

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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.bthUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }


                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Value Assign

                ValueAssign();

                #endregion

                #region Check Point

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                {
                    MessageBox.Show("Please Select Shift");
                    cmbShift.Focus();
                    return;
                }

                string Message = MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1;
                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }
                else
                {
                    #region Check Point

                    if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                    {
                        MessageBox.Show(
                            "Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }

                    if (IsUpdate == false)
                    {
                        NextID = string.Empty;
                    }
                    else
                    {
                        NextID = txtHiddenInvoiceNo.Text.Trim();
                    }

                    #region Null Check

                    if (vCustomerID == "0")
                    {

                        MessageBox.Show("Please Enter Customer Information");
                        txtCustomer.Focus();
                        return;
                    }

                    if (vehicleRequired.ToLower() == "y")
                    {
                       
                            if (txtVehicleNo.Text == "")
                            {

                                MessageBox.Show("Please Enter Vehicle Number");
                                txtVehicleNo.Focus();
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
                        //if (txtOldID.Text == "")
                        //{
                        //    MessageBox.Show("Please Enter previous Invoice No");

                        //    return;
                        //}
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
                        txtSerialNo.Text = "-";
                    }

                    #endregion

                    #endregion

                    #region Master

                    saleMaster = new SaleMasterVM();
                    saleMaster.SalesInvoiceNo = NextID.ToString();
                    saleMaster.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.TransactionType = transactionType;
                    saleMaster.LastModifiedBy = Program.CurrentUser;
                    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                    saleMaster.SignatoryName = userInfo[0]["FullName"].ToString();
                    saleMaster.SignatoryDesig = userInfo[0]["Designation"].ToString();


                    #region Not Required During Post

                    #region Backup Oct-01-2020


                    saleMaster.CustomerID = vCustomerID.Trim();
                    saleMaster.DeliveryAddress1 = txtDeliveryAddress1.Text.Trim();
                    saleMaster.DeliveryAddress2 = txtDeliveryAddress2.Text.Trim();
                    saleMaster.DeliveryAddress3 = txtDeliveryAddress3.Text.Trim();
                    saleMaster.VehicleType = txtVehicleType.Text.Trim();
                    saleMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                    saleMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                    saleMaster.SerialNo = txtSerialNo.Text.Trim();
                    saleMaster.Comments = txtComments.Text.Trim();
                    saleMaster.CreatedBy = Program.CurrentUser;
                    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    saleMaster.SaleType = cmbType.Text.Trim();
                    saleMaster.PreviousSalesInvoiceNo = txtOldID.Text.Trim();
                    saleMaster.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                    saleMaster.IsPrint = "N";
                    saleMaster.TenderId = TenderId;
                    saleMaster.DeliveryDate = saleMaster.InvoiceDateTime;//                        dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    saleMaster.VehicleNo = txtVehicleNo.Text.Trim();
                    saleMaster.Post = "Y"; //Post
                    saleMaster.CurrencyID = txtCurrencyId.Text.Trim(); //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text.Trim());
                    saleMaster.ShiftId = cmbShift.SelectedValue.ToString(); //Post
                    saleMaster.EXPFormNo = txtEXPFormNo.Text.Trim(); //Post

                    #endregion

                    #endregion

                    #endregion

                    #region Not Required During Post

                    #region Details Value Assing (Not Required During Post)

                    #region Backup Oct-01-2020


                    ////saleDetails = new List<SaleDetailVM>();

                    ////for (int i = 0; i < dgvSale.RowCount; i++)
                    ////{
                    ////    SaleDetailVM detail = new SaleDetailVM();

                    ////    detail.InvoiceLineNo = dgvSale.Rows[i].Cells["LineNo"].Value.ToString();
                    ////    detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();
                    ////    detail.Quantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["Quantity"].Value.ToString());
                    ////    detail.PromotionalQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PromotionalQuantity"].Value.ToString());
                    ////    detail.SalesPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UnitPrice"].Value.ToString());
                    ////    detail.NBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["NBRPrice"].Value.ToString());
                    ////    detail.UOM = dgvSale.Rows[i].Cells["UOM"].Value.ToString();
                    ////    detail.VATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATRate"].Value.ToString());
                    ////    detail.VATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["VATAmount"].Value.ToString());
                    ////    detail.SubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["SubTotal"].Value.ToString());
                    ////    detail.CommentsD = dgvSale.Rows[i].Cells["Comments"].Value.ToString();
                    ////    detail.ValueOnly = dgvSale.Rows[i].Cells["ValueOnly"].Value.ToString();

                    ////    detail.SD = Convert.ToDecimal(dgvSale.Rows[i].Cells["SD"].Value.ToString());
                    ////    detail.SDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["SDAmount"].Value.ToString());
                    ////    detail.SaleTypeD = cmbType.Text.Trim();
                    ////    detail.PreviousSalesInvoiceNoD = txtOldID.Text.Trim();
                    ////    detail.TradingD = dgvSale.Rows[i].Cells["Trading"].Value.ToString();
                    ////    detail.NonStockD = dgvSale.Rows[i].Cells["NonStock"].Value.ToString();
                    ////    detail.TradingMarkUp =
                    ////        Convert.ToDecimal(dgvSale.Rows[i].Cells["TradingMarkUp"].Value.ToString());
                    ////    detail.Type = dgvSale.Rows[i].Cells["Type"].Value.ToString();
                    ////    detail.UOMQty = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMQty"].Value.ToString());
                    ////    detail.UOMn = dgvSale.Rows[i].Cells["UOMn"].Value.ToString();
                    ////    detail.UOMc = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMc"].Value.ToString());
                    ////    detail.UOMPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["UOMPrice"].Value.ToString());
                    ////    detail.DiscountAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountAmount"].Value.ToString());
                    ////    detail.DiscountedNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["DiscountedNBRPrice"].Value.ToString());
                    ////    detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                    ////    detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                    ////    detail.ProductDescription = dgvSale.Rows[i].Cells["ProductDescription"].Value.ToString();
                    ////    detail.IsFixedVAT = dgvSale.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                    ////    detail.FixedVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["FixedVATAmount"].Value.ToString());
                    ////    #region New Field For DN/CN

                    ////    detail.PreviousInvoiceDateTime = dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value == null ? DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm") : dgvSale.Rows[i].Cells["PreviousInvoiceDateTime"].Value.ToString();
                    ////    detail.PreviousNBRPrice = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousNBRPrice"].Value));
                    ////    detail.PreviousQuantity = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousQuantity"].Value));
                    ////    detail.PreviousSubTotal = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSubTotal"].Value));
                    ////    detail.PreviousVATAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATAmount"].Value));
                    ////    detail.PreviousVATRate = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousVATRate"].Value));
                    ////    detail.PreviousSD = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSD"].Value));
                    ////    detail.PreviousSDAmount = Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value == null ? 0 : Convert.ToDecimal(dgvSale.Rows[i].Cells["PreviousSDAmount"].Value));
                    ////    detail.ReasonOfReturn = dgvSale.Rows[i].Cells["ReasonOfReturn"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                    ////    detail.PreviousUOM = dgvSale.Rows[i].Cells["PreviousUOM"].Value == null ? "N/A" : dgvSale.Rows[i].Cells["PreviousUOM"].Value.ToString();

                    ////    #endregion

                    ////    saleDetails.Add(detail);
                    ////}

                    #endregion

                    #endregion

                    #region Export Value Assign (Not Required During Post)


                    #endregion

                    #endregion

                }

                if (saleDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Button Stats

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;
                //this.Enabled = false;

                #endregion

                #region Post Data

                while (bgwPost.IsBusy)
                {

                }
                bgwPost.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }

            #endregion

        }

        private void bgwPost_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement
                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                //SaleDAL saleDal = new SaleDAL();
                ISale saleDal = GetObject();

                sqlResults = saleDal.SalesPost(saleMaster, saleDetails, trackingsVm, null, null, connVM,false,Program.CurrentUserID);
                POST_DOWORK_SUCCESS = true;

                var value = new CommonDAL().settingValue("CompanyCode", "Code");

                if (sqlResults[0].ToLower() == "success" && value == KCL && !string.IsNullOrEmpty(txtImportID.Text))
                {
                    var table = new DataTable();

                    table.Columns.Add(("ID"));

                    table.Rows.Add(txtImportID.Text);

                    saleDal.PostSales(table,connVM);
                }


                // End DoWork

                #endregion

            }

            #endregion

            #region catch

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
            #region try

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
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }

            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;

            }

            #endregion

        }

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            #region Initializ

            ISale sDal = GetObject();
            DataTable dataTable = new DataTable("SearchSalesHeader");

            #endregion

            #region try

            try
            {
                #region Statement

                #region Stating Values

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

                #endregion

                #region Transaction Type
                DataGridViewRow selectedRow = null;
                TransactionTypes();
                #endregion Transaction Type


                selectedRow = FormSaleSearch.SelectOne(transactionType, Program.BranchId);

                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    var tt = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm");
                    FileLogger.Log(this.Name, "Master Load Time", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));


                    dataTable = sDal.SearchSalesHeaderDTNew(selectedRow.Cells["SalesInvoiceNo"].Value.ToString(), "", connVM);
                    SaleDataLoad(dataTable);


                }
                ChangeData = false;



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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }

            #endregion

        }

        private void SaleDataLoad(DataTable dataTable)
        {

            #region CheckPoint

            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            #endregion

            DataRow dr = dataTable.Rows[0];

            #region Sales Invoice No



            txtSalesInvoiceNo.Text = dr["SalesInvoiceNo"].ToString();
            txtHiddenInvoiceNo.Text = dr["SalesInvoiceNo"].ToString();

            if (txtSalesInvoiceNo.Text == "")
            {
                SaleDetailData = "0";
            }
            else
            {
                SaleDetailData = txtSalesInvoiceNo.Text.Trim();
            }

            #endregion

            #region DGV Sale Load / Detail Data Load / Background Worker Challan Search

            backgroundWorkerChallanSearch.RunWorkerAsync();

            #endregion

            #region Value Assign to Form Elements

            txtId.Text = dr["Id"].ToString();
            txtFiscalYear.Text = dr["FiscalYear"].ToString();

            txtDeductionAmount.Text = Program.FormatingNumeric(Convert.ToString(dr["DeductionAmount"]));
            cmbShift.SelectedValue = dr["ShiftId"];
            vCustomerID = dr["CustomerID"].ToString();
            txtCustomer.Text = dr["CustomerName"].ToString();
            txtDeliveryAddress1.Text = dr["DeliveryAddress1"].ToString();
            txtDeliveryAddress2.Text = dr["DeliveryAddress2"].ToString();
            txtDeliveryAddress3.Text = dr["DeliveryAddress3"].ToString();
            txtVehicleID.Text = dr["VehicleID"].ToString();
            txtVehicleType.Text = dr["VehicleType"].ToString();
            txtVehicleNo.Text = dr["VehicleNo"].ToString();
            txtVehicleNo.Text = dr["VehicleNo"].ToString();
            txtTotalAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(dr["TotalAmount"].ToString()).ToString());//"0,0.00");
            txtTotalVATAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(dr["TotalVATAmount"].ToString()).ToString());//"0,0.00");
            txtHPSTotal.Text = Program.ParseDecimalObject(Convert.ToDecimal(dr["HPSTotalAmount"].ToString()).ToString());
            txtSerialNo.Text = dr["SerialNo"].ToString();
            dtpInvoiceDate.Value = Convert.ToDateTime(dr["InvoiceDateTime"].ToString());
            dtpDeliveryDate.Value = Convert.ToDateTime(dr["DeliveryDate"].ToString());
            txtComments.Text = dr["Comments"].ToString();
            txtEXPFormNo.Text = dr["EXPFormNo"].ToString();

            #region Conditional Values

           
            if (dr["IsDeemedExport"].ToString().ToLower() == "y")
            {
                chkIsDeemedExport.Checked = true;
            }
            txtOldID.Text = dr["PID"].ToString();
            if (string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Credit" ||
                string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Debit")
            {
                if (CreditWithoutTransaction == true)
                {
                    txtOldID.ReadOnly = false;
                }
            }

            #endregion

            cmbCurrency.Text = dr["CurrencyCode"].ToString();
            txtBDTRate.Text = Program.ParseDecimalObject(dr["CurrencyRateFromBDT"].ToString());
            ImportExcelID = dr["ImportID"].ToString();
            txtImportID.Text = dr["ImportID"].ToString();

            #region Conditional Values

            
            if (dr["Trading"].ToString() == "Y")
            {
                chkTrading.Checked = true;
            }
            else
            {
                chkTrading.Checked = false;
            }
            if (dr["Isprint"].ToString() == "Y")
            {
                chkPrint.Checked = true;
            }
            else
            {
                chkPrint.Checked = false;
            }

            #endregion

            TenderId = dr["TenderId"].ToString();
            IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
            IsConvCompltd = Convert.ToString(dr["IsCurrencyConvCompleted"].ToString()) == "Y";
            SearchBranchId = Convert.ToInt32(dr["BranchId"]);
            AlReadyPrintNo = Convert.ToInt32(dr["AlReadyPrint"].ToString());

            #endregion

            #region Delivery & GatePass
            //if (dr["DeliveryChallan"].ToString() == "N")
            //{
            //    btnDL.Enabled = true;
            //}
            //else
            //{
            //    btnDL.Enabled = false;
            //}
            //if (dr["IsGatePass"].ToString() == "N")
            //{
            //    btnGPR.Enabled = true;
            //}
            //else
            //{
            //    btnGPR.Enabled = false;
            //}
            #endregion Delivery & GatePass

            #region Tracking Info

            InsertTrackingInfo();

            #endregion

            #region Buttons, Progressbar Stats

            this.btnFirst.Enabled = false;
            this.btnPrevious.Enabled = false;
            this.btnNext.Enabled = false;
            this.btnLast.Enabled = false;

            this.btnSearchInvoiceNo.Enabled = false;
            this.progressBar1.Visible = true;
            //this.Enabled = false;
            btnPrint.Enabled = true;

            #endregion

        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                #region try

                try
                {

                    #region Transaction Types

                    TransactionTypes();

                    #endregion

                    #region Reset Fields / Elements

                    ClearAllFields();

                    #endregion

                    #region VAT Names

                    VATName vname = new VATName();
                    cmbVAT1Name.DataSource = vname.VATNameList;

                    #endregion

                    #region Form Maker

                    FormMaker();

                    #endregion

                    #region Form Load

                    FormLoad();

                    #endregion

                    #region Reset Fields

                    txtSalesInvoiceNo.Text = "~~~ New ~~~";

                    //if(!rbtnExport.Checked)
                    //txtNBRPrice.ReadOnly = true;
                    Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                    Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                    Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
                    this.Enabled = false;

                    #endregion

                    #region Background Load

                    bgwLoad.RunWorkerAsync();

                    #endregion

                    #region Element Stats

                    chkPrint.Checked = false;
                    IsPost = false;
                    IsConvCompltd = true;
                    vCustomerID = "0";

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
                    FileLogger.Log(this.Name, "FormSale_Load", exMessage);
                }

                #endregion

                #region finally

                finally
                {
                    ChangeData = false;
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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }

            #endregion

            #region finally

            finally
            {
                ChangeData = false;

            }

            #endregion

        }

        public void ClearAllFields()
        {
            //cmbProduct.Text = "Select";
            txtId.Text = "0";
            SearchBranchId = 0;
            txtFiscalYear.Text = "0";

            txtDeductionAmount.Text = "0";
            txtVehicleType.Text = string.Empty;
            txtTotalSDAmount.Text = "0.00";
            txtDeliveryAddress1.Text = "";
            txtProductDescription.Text = "";
            txtOldID.Text = "";
            txtVehicleNo.Text = "";
            txtTotalQuantity.Text = "";
            txtFixedVATAmount.Text = "0";
            txtFixedVATAmount.Visible = false;
            if (chkSameVehicle.Checked == false)
            {
                txtVehicleID.Text = "";
                txtVehicleNo.Text = "";
                txtVehicleType.Text = "";
            }
            chkVehicleSaveInDatabase.Visible = false;
            //cmbProductType.Text = "Select";
            //cmbType.Text = "Select";
            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate",null,connVM);
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpInvoiceDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpDeliveryDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpConversionDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            }
            else
            {
                dtpInvoiceDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpDeliveryDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpConversionDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
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
            txtQuantity.Text = "0.00";
            txtQty1.Text = "0";
            txtQty2.Text = "0";
            txtHiddenInvoiceNo.Text = "";
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
            txtEXPFormNo.Text = "";
            txtTotalSubTotal.Clear();
            dgvSale.Rows.Clear();
           
            txtCustomer.Text = "";
            txtWeight.Text = "";
            cmbCurrency.Enabled = true;
            chkIsDeemedExport.Checked = false;
            chkIsFixedVAT.Checked = false;
            txtFixedVATAmount.Text = "0.00";

            #region
            txtHPSTotal.Text = "0.00";
            txtHPSRate.Text = "0.00";
            #endregion

        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();


            if (Convert.ToDecimal(txtVATRate.Text.Trim()) < SelectedVATRate)
            {
                txtVATRate.Text = SelectedVATRate.ToString();
                MessageBox.Show("VAT Rate can not reduce");
                return;
            }

            //cmbVATType.Text = VATTypeCal(txtVATRate.Text.Trim());
            if (VATTypeVATAutoChange.ToLower() == "y")
            {
                cmbVATType.SelectedValue = VATTypeCal(txtVATRate.Text.Trim()).ToLower();
            }
            Program.CheckVATRate(txtVATRate, "VAT Rate Input Box");

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

            txtNBRPrice.Text = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();
            txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), SalePlaceRate).ToString();
            txtQty1.Text = Program.FormatingNumeric(txtQty1.Text.Trim(), SalePlaceRate).ToString();
            txtQty2.Text = Program.FormatingNumeric(txtQty2.Text.Trim(), SalePlaceRate).ToString();
            txtFixedVATAmount.Text = Program.FormatingNumeric(txtFixedVATAmount.Text.Trim(), SalePlaceRate).ToString();





            if (ExcludingVAT == "N")
            {
                decimal DeclaredNBRPrice = 0;
                decimal NBRPrice = 0;
                decimal NBRPriceInclusiveVAT = 0;
                decimal VATRate = 0;
                decimal SDRate = 0;

                VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                SDRate = Convert.ToDecimal(txtSD.Text.Trim());
                NBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());

                NBRPrice = ((((NBRPrice * 100) / (100 + VATRate)) * 100) / (100 + SDRate)); //// NBRPriceInclusiveVAT * (VATRate / (100 + VATRate));

            }


          

        }

        private void NBRPriceCall()
        {

            #region Product Call

            decimal tenderPrice = 0;
                ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF); //new ProductDAL();
            ProductVM products = new ProductVM();

            products = productDal.SelectAll(txtProductCode.Text.Trim().ToLower(), null, null, null, null, null, connVM).FirstOrDefault();

            tenderPrice = products.NBRPrice;

            #endregion

            #region BOM - NBRPrice

            DataTable dt = new DataTable();

            if (CustomerWiseBOM)
            {
                dt = productDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                                         dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID,connVM);
            }
            else
            {
                dt = productDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                              dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null,"0",connVM);

            }


            int BOMId = 0;
            decimal NBRPrice = 0;

            if (dt != null && dt.Rows.Count > 0)
            {
                #region ReferenceNo

                cmbBOMReferenceNo.DataSource = dt;
                cmbBOMReferenceNo.DisplayMember = "ReferenceNo";
                cmbBOMReferenceNo.ValueMember = "ReferenceNo";

                ////cmbBOMReferenceNo.Text = dt.Rows[0]["ReferenceNo"].ToString();

                cmbBOMReferenceNo.SelectedIndex = 0;


                #endregion

                #region BOMId and NBRPrice

                DataRow dr = dt.Rows[0];
                string tempBOMId = dr["BOMId"].ToString();
                string tempNBRPrice = dr["NBRPrice"].ToString();
                txtNBRPrice.Text = Program.ParseDecimalObject(Convert.ToDecimal(tempNBRPrice).ToString());

                if (!string.IsNullOrWhiteSpace(tempBOMId))
                {
                    BOMId = Convert.ToInt32(tempBOMId);
                }

                if (!string.IsNullOrWhiteSpace(tempNBRPrice))
                {
                    NBRPrice = Convert.ToDecimal(tempNBRPrice);
                }

                #endregion

            }


            if (!string.IsNullOrWhiteSpace(txtBOMId.Text))
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    SetBOM(dt);
                }
            }
            else
            {
                txtBOMId.Text = BOMId.ToString();
            }

            #endregion

            vNBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());

            

            

            #region Discount Function

            Discount();

            #endregion

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods 04 / Tender, Dispose Raw, Dispose Finish

        private void btnTenderNew_Click(object sender, EventArgs e)
        {
            try
            {
                #region Dispose Raw

                if (transactionType == "DisposeRaw")
                {

                    if (vCustomerID == "0" || string.IsNullOrWhiteSpace(vCustomerID))
                    {
                        MessageBox.Show("Please select the Customer!", this.Text);
                        return;
                    }

                    TransactionType_DisposeRaw();

                    return;
                }

                #endregion

                #region Dispose Finish

                else if (transactionType == "DisposeFinish")
                {

                    if (vCustomerID == "0" || string.IsNullOrWhiteSpace(vCustomerID))
                    {
                        MessageBox.Show("Please select the Customer!", this.Text);
                        return;
                    }

                    TransactionType_DisposeFinish();

                    return;
                }

                #endregion


                DataGridViewRow selectedRow = new DataGridViewRow();

                #region Statement

                dgvReceiveHistory.Top = dgvSale.Top;
                dgvReceiveHistory.Left = dgvSale.Left;
                dgvReceiveHistory.Height = dgvSale.Height;
                dgvReceiveHistory.Width = dgvSale.Width;

                dgvSale.Visible = false;
                dgvReceiveHistory.Visible = true;


                Program.fromOpen = "Me";

                selectedRow = FormTenderSearch.SelectOne();

                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    TenderId = selectedRow.Cells["TenderId"].Value.ToString();
                    txtSerialNo.Text = selectedRow.Cells["RefNo"].Value.ToString();
                    vCustomerID = selectedRow.Cells["CustomerId"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustName"].Value.ToString();
                    customerGId = selectedRow.Cells["GroupId"].Value.ToString();
                    customerGName = selectedRow.Cells["CustomerGroupName"].Value.ToString();
                    tenderDate = Convert.ToDateTime(selectedRow.Cells["TenderDate"].Value.ToString());
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTender_Click", exMessage);
            }

            #endregion
        }


        private void TransactionType_DisposeRaw()
        {

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                #region sqlText

                string sqlText = @"
select 
drd.Id
, drd.DisposeNo
, drd.PurchaseNo
, drd.TransactionDateTime
, isnull(DRD.Post,'N')Post      
, pro.ItemNo
, pro.ProductCode
, pro.ProductName
,isnull(DRD.Quantity,'0')Quantity
,DRD.UOM
,isnull(DRD.UnitPrice,'0')UnitPrice
,isnull(DRD.SubTotal,'0')SubTotal      
,isnull(DRD.OfferUnitPrice,'0')OfferUnitPrice      
,isnull(DRD.IsSaleable,'N')IsSaleable  

from DisposeRawDetails drd
left outer join Products pro on drd.ItemNo=pro.ItemNo
where 1=1 
and ISNULL(drd.IsSaleable,'N')='Y'
and drd.Post = 'Y'

";

                string SQLTextRecordCount = @" 
select count(ItemNo)RecordNo from DisposeRawDetails
where 1=1 
and ISNULL(IsSaleable,'N')='Y'
and Post = 'Y'
";


                string[] shortColumnName = { 
                                                   "drd.Id"
                                                  , "drd.DisposeNo" 
                                                  ,"drd.PurchaseNo"
                                                  ,"drd.TransactionDateTime" 
                                                  ,"drd.Post" 
                                                  ,"drd.ItemNo" 
                                                  ,"pro.ProductCode" 
                                                  ,"pro.ProductName" 
                                                  ,"drd.Quantity" 
                                                  ,"drd.UOM" 
                                                  ,"drd.UnitPrice" 
                                                  ,"drd.SubTotal" 
                                                  ,"drd.OfferUnitPrice" 
                                                  ,"drd.IsSaleable" 
                                               };


                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(sqlText, "", "", shortColumnName, null, SQLTextRecordCount);

                if (selectedRow != null && selectedRow.Index >= 0)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);

                    int Index = dgvSale.RowCount - 1;

                    SetDataGridView_DisposeRaw(selectedRow, Index);

                    Rowcalculate();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void TransactionType_DisposeFinish()
        {

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                #region sqlText

                string sqlText = @"
select 
df.Id
, df.DisposeNo
, df.TransactionDateTime
, isnull(df.Post,'N')Post      
, pro.ItemNo
, pro.ProductCode
, pro.ProductName
,isnull(df.Quantity,'0')Quantity
,df.UOM
,isnull(df.UnitPrice,'0')UnitPrice
,isnull(df.OfferUnitPrice,'0')OfferUnitPrice      
,isnull(df.IsSaleable,'N')IsSaleable  

from DisposeFinishs df
left outer join Products pro on df.FinishItemNo=pro.ItemNo
where 1=1 
and ISNULL(df.IsSaleable,'N')='Y'
and df.Post = 'Y'

";

                string SQLTextRecordCount = @" 
select count(FinishItemNo)RecordNo from DisposeFinishs
where 1=1 
and ISNULL(IsSaleable,'N')='Y'
and Post = 'Y'
";


                string[] shortColumnName = { 
                                                   "df.Id"
                                                  ,"df.DisposeNo" 
                                                  ,"df.TransactionDateTime" 
                                                  ,"df.Post" 
                                                  ,"pro.ItemNo" 
                                                  ,"pro.ProductCode" 
                                                  ,"pro.ProductName" 
                                                  ,"df.Quantity" 
                                                  ,"df.UOM" 
                                                  ,"df.UnitPrice" 
                                                  ,"df.OfferUnitPrice" 
                                                  ,"df.IsSaleable" 
                                               };


                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(sqlText, "", "", shortColumnName, null, SQLTextRecordCount);

                if (selectedRow != null && selectedRow.Index >= 0)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);

                    int Index = dgvSale.RowCount - 1;

                    SetDataGridView_DisposeFinish(selectedRow, Index);

                    Rowcalculate();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


 

        private void SetDataGridView_DisposeRaw(DataGridViewRow dgvr, int paramIndex)
        {

            try
            {

                #region Declarations

                int DisposeRawDetailId = 0;
                string DisposeNo = "";
                string PurchaseNo = "";
                string ItemNo = "";

                DataTable dt = new DataTable();

                DisposeRawDetailId = Convert.ToInt32(dgvr.Cells["Id"].Value);

                DisposeNo = dgvr.Cells["DisposeNo"].Value.ToString();
                PurchaseNo = dgvr.Cells["PurchaseNo"].Value.ToString();
                ItemNo = dgvr.Cells["ItemNo"].Value.ToString();


                string[] cFields = new[] { "drd.Id" };
                string[] cValues = new[] { DisposeRawDetailId.ToString() };


                #endregion

                #region Data Call

                dt = new DisposeRawDAL().Select_DisposeRawDetail("", cFields, cValues,null,null,connVM);

                DataRow dr = dt.Rows[0];

                #endregion

                #region Set DataGridView

                dgvSale["ItemNo", paramIndex].Value = dr["ItemNo"];
                dgvSale["ItemName", paramIndex].Value = dr["ProductName"];
                dgvSale["PCode", paramIndex].Value = dr["ProductCode"];
                dgvSale["Comments", paramIndex].Value = "NA";

                decimal UomConv = 1;
                decimal Quantity = Convert.ToDecimal(dr["Quantity"]);
                decimal UnitPrice = Convert.ToDecimal(dr["OfferUnitPrice"]);
                string UOM = Convert.ToString(dr["UOM"]);

                dgvSale["UnitPrice", paramIndex].Value = UnitPrice;
                dgvSale["SD", paramIndex].Value = dr["SD"];
                dgvSale["SDAmount", paramIndex].Value = dr["SDAmount"];
                dgvSale["VATRate", paramIndex].Value = dr["VATRate"];
                dgvSale["VATAmount", paramIndex].Value = dr["VATAmount"];

                dgvSale["Quantity", paramIndex].Value = Quantity;
                dgvSale["SaleQuantity", paramIndex].Value = Quantity;
                dgvSale["UOMPrice", paramIndex].Value = UnitPrice;
                dgvSale["UOMc", paramIndex].Value = UomConv;
                dgvSale["NBRPrice", paramIndex].Value = UnitPrice;
                dgvSale["UOM", paramIndex].Value = UOM;
                dgvSale["UOMn", paramIndex].Value = UOM;
                dgvSale["UOMQty", paramIndex].Value = Quantity * UomConv; ;

                dgvSale["DiscountAmount", paramIndex].Value = 0;
                dgvSale["DiscountedNBRPrice", paramIndex].Value = UnitPrice;

                dgvSale["Status", paramIndex].Value = "New";

                dgvSale["Stock", paramIndex].Value = 0;
                dgvSale["Previous", paramIndex].Value = 0;
                dgvSale["Change", paramIndex].Value = 0;
                dgvSale["Type", paramIndex].Value = "VAT";
                dgvSale["VATName", paramIndex].Value = "VAT 4.3";
                dgvSale["CConvDate", paramIndex].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");


                dgvSale["TotalValue", paramIndex].Value = UnitPrice * Quantity;
                dgvSale["ATVRate", paramIndex].Value = 0;
                dgvSale["ATVablePrice", paramIndex].Value = 0;
                dgvSale["ATVAmount", paramIndex].Value = 0;

                #endregion

                #region Optional Fields

                dgvSale["SourcePaidQuantity", paramIndex].Value = 0;
                dgvSale["BOMId", paramIndex].Value = 0;
                dgvSale["BOMReferenceNo", paramIndex].Value = 0;
                dgvSale["PromotionalQuantity", paramIndex].Value = 0;
                dgvSale["NBRPriceInclusiveVAT", paramIndex].Value = 0;

                dgvSale["TradingMarkUp", paramIndex].Value = 0;
                dgvSale["Trading", paramIndex].Value = 0;
                dgvSale["ValueOnly", paramIndex].Value = 0;
                dgvSale["NonStock", paramIndex].Value = 0;
                dgvSale["Weight", paramIndex].Value = 0;

                dgvSale["WareHouseRent", paramIndex].Value = 0;
                dgvSale["WareHouseVAT", paramIndex].Value = 0;
                dgvSale["IsCommercialImporter", paramIndex].Value = 0;
                dgvSale["ProductDescription", paramIndex].Value = 0;

                dgvSale["IsFixedVAT", paramIndex].Value = 0;
                dgvSale["FixedVATAmount", paramIndex].Value = 0;
                dgvSale["HPSRate", paramIndex].Value = 0;

                dgvSale["CDNVATAmount", paramIndex].Value = 0;
                dgvSale["CDNSDAmount", paramIndex].Value = 0;
                dgvSale["CDNSubtotal", paramIndex].Value = 0;





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
                FileLogger.Log(this.Name, "SetDataGridView_DisposeRaw", exMessage);
            }

            #endregion

        }

        private void SetDataGridView_DisposeFinish(DataGridViewRow dgvr, int paramIndex)
        {

            try
            {

                #region Declarations

                string DisposeNo = "";
                string ItemNo = "";

                DataTable dt = new DataTable();

                #endregion

                #region Data Call

                DisposeNo = dgvr.Cells["DisposeNo"].Value.ToString();
                ItemNo = dgvr.Cells["ItemNo"].Value.ToString();

                string[] cFields = new[] { "df.DisposeNo" };
                string[] cValues = new[] { DisposeNo.ToString() };

                dt = new DisposeFinishDAL().Select(cFields, cValues,null,null,connVM);

                DataRow dr = dt.Rows[0];

                #endregion

                #region Set DataGridView

                dgvSale["ItemNo", paramIndex].Value = dr["FinishItemNo"];
                dgvSale["ItemName", paramIndex].Value = dr["ProductName"];
                dgvSale["PCode", paramIndex].Value = dr["ProductCode"];
                dgvSale["Comments", paramIndex].Value = "NA";

                decimal UomConv = 1;
                decimal Quantity = Convert.ToDecimal(Program.ParseDecimalObject(dr["Quantity"]));
                decimal UnitPrice = Convert.ToDecimal(Program.ParseDecimalObject(dr["OfferUnitPrice"]));
                string UOM = Convert.ToString(dr["UOM"]);

                dgvSale["UnitPrice", paramIndex].Value = UnitPrice;
                dgvSale["SD", paramIndex].Value = 0;
                dgvSale["SDAmount", paramIndex].Value = 0;
                dgvSale["VATRate", paramIndex].Value = 0;
                dgvSale["VATAmount", paramIndex].Value = 0;

                dgvSale["Quantity", paramIndex].Value = Quantity;
                dgvSale["SaleQuantity", paramIndex].Value = Quantity;
                dgvSale["UOMPrice", paramIndex].Value = UnitPrice;
                dgvSale["UOMc", paramIndex].Value = UomConv;
                dgvSale["NBRPrice", paramIndex].Value = UnitPrice;
                dgvSale["UOM", paramIndex].Value = UOM;
                dgvSale["UOMn", paramIndex].Value = UOM;
                dgvSale["UOMQty", paramIndex].Value = Quantity * UomConv; ;

                dgvSale["DiscountAmount", paramIndex].Value = 0;
                dgvSale["DiscountedNBRPrice", paramIndex].Value = UnitPrice;

                dgvSale["Status", paramIndex].Value = "New";

                dgvSale["Stock", paramIndex].Value = 0;
                dgvSale["Previous", paramIndex].Value = 0;
                dgvSale["Change", paramIndex].Value = 0;
                dgvSale["Type", paramIndex].Value = "VAT";
                dgvSale["VATName", paramIndex].Value = "VAT 4.3";
                dgvSale["CConvDate", paramIndex].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");


                dgvSale["TotalValue", paramIndex].Value = UnitPrice * Quantity;
                dgvSale["ATVRate", paramIndex].Value = 0;
                dgvSale["ATVablePrice", paramIndex].Value = 0;
                dgvSale["ATVAmount", paramIndex].Value = 0;

                #endregion

                #region Optional Fields

                dgvSale["SourcePaidQuantity", paramIndex].Value = 0;
                dgvSale["BOMId", paramIndex].Value = 0;
                dgvSale["BOMReferenceNo", paramIndex].Value = 0;
                dgvSale["PromotionalQuantity", paramIndex].Value = 0;
                dgvSale["NBRPriceInclusiveVAT", paramIndex].Value = 0;

                dgvSale["TradingMarkUp", paramIndex].Value = 0;
                dgvSale["Trading", paramIndex].Value = 0;
                dgvSale["ValueOnly", paramIndex].Value = 0;
                dgvSale["NonStock", paramIndex].Value = 0;
                dgvSale["Weight", paramIndex].Value = 0;

                dgvSale["WareHouseRent", paramIndex].Value = 0;
                dgvSale["WareHouseVAT", paramIndex].Value = 0;
                dgvSale["IsCommercialImporter", paramIndex].Value = 0;
                dgvSale["ProductDescription", paramIndex].Value = 0;

                dgvSale["IsFixedVAT", paramIndex].Value = 0;
                dgvSale["FixedVATAmount", paramIndex].Value = 0;
                dgvSale["HPSRate", paramIndex].Value = 0;

                dgvSale["CDNVATAmount", paramIndex].Value = 0;
                dgvSale["CDNSDAmount", paramIndex].Value = 0;
                dgvSale["CDNSubtotal", paramIndex].Value = 0;





                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }



        private void GridViewSetData(DataTable result)
        {

            try
            {


                #region Declarations

                #endregion

                #region For Loop - Set Data Grid View
                dgvSale.Rows.Clear();
                dgvSale.DataSource = null;
                for (int i = 0; i < result.Rows.Count; i++)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);

                    DataRow dr = result.Rows[i];

                    int paramIndex = dgvSale.RowCount - 1;

                    var ProductName = dr["ProductName"].ToString();
                    ProductName = ProductName.Split('~')[0];
                    #region Set DataGridView
                    dgvSale["LineNo", paramIndex].Value = i + 1;

                    dgvSale["ItemNo", paramIndex].Value = dr["ItemNo"];
                    dgvSale["ItemName", paramIndex].Value = ProductName;
                    dgvSale["PCode", paramIndex].Value = dr["ProductCode"];
                    dgvSale["Comments", paramIndex].Value = "NA";

                    ProductDAL _ProductDal = new ProductDAL();

                    DataTable dtProduct = new DataTable();

                    #region Product Call

                    dtProduct = _ProductDal.SelectProductDTAll(new[] { "Products.ProductCode" }, new[] { dr["ProductCode"].ToString() },null,null,false,0,"","",null,connVM);

                    #endregion
                    decimal UomConv = Convert.ToDecimal(dr["UOMc"].ToString());
                    decimal Quantity = Convert.ToDecimal(dr["Quantity"]);
                    decimal UnitPrice = Convert.ToDecimal(dr["CostPrice"]);
                    string UOM = Convert.ToString(dr["UOM"]);
                    decimal VATRate = Convert.ToDecimal(dtProduct.Rows[0]["VATRate"].ToString());

                    dgvSale["UnitPrice", paramIndex].Value = UnitPrice;
                    dgvSale["SD", paramIndex].Value = dr["SD"];
                    dgvSale["SDAmount", paramIndex].Value = Program.ParseDecimalObject(dr["SDAmount"]);
                    dgvSale["VATRate", paramIndex].Value = Program.ParseDecimalObject(VATRate);
                    dgvSale["VATAmount", paramIndex].Value = Program.ParseDecimalObject(dr["VATAmount"]);

                    dgvSale["Quantity", paramIndex].Value = Program.ParseDecimalObject(Quantity);
                    dgvSale["SaleQuantity", paramIndex].Value = Program.ParseDecimalObject(Quantity);
                    dgvSale["UOMPrice", paramIndex].Value = Program.ParseDecimalObject(UnitPrice);
                    dgvSale["UOMc", paramIndex].Value = Program.ParseDecimalObject(UomConv);
                    dgvSale["NBRPrice", paramIndex].Value = Program.ParseDecimalObject(UnitPrice);
                    dgvSale["UOM", paramIndex].Value = UOM;
                    dgvSale["UOMn", paramIndex].Value = UOM;
                    dgvSale["UOMQty", paramIndex].Value = Quantity * UomConv; ;

                    dgvSale["DiscountAmount", paramIndex].Value = 0;
                    dgvSale["DiscountedNBRPrice", paramIndex].Value = Program.ParseDecimalObject(UnitPrice);

                    dgvSale["Status", paramIndex].Value = "New";

                    dgvSale["Stock", paramIndex].Value = 0;
                    dgvSale["Previous", paramIndex].Value = 0;
                    dgvSale["Change", paramIndex].Value = 0;
                    dgvSale["Type", paramIndex].Value = "VAT";
                    dgvSale["VATName", paramIndex].Value = "VAT 4.3";
                    dgvSale["CConvDate", paramIndex].Value = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");


                    dgvSale["TotalValue", paramIndex].Value = Program.ParseDecimalObject(UnitPrice * Quantity);
                    dgvSale["ATVRate", paramIndex].Value = 0;
                    dgvSale["ATVablePrice", paramIndex].Value = 0;
                    dgvSale["ATVAmount", paramIndex].Value = 0;

                    #endregion

                    #region Optional Fields

                    dgvSale["SourcePaidQuantity", paramIndex].Value = 0;
                    dgvSale["BOMId", paramIndex].Value = dr["BOMId"];
                    dgvSale["BOMReferenceNo", paramIndex].Value = 0;
                    dgvSale["PromotionalQuantity", paramIndex].Value = 0;
                    dgvSale["NBRPriceInclusiveVAT", paramIndex].Value = 0;

                    dgvSale["TradingMarkUp", paramIndex].Value = 0;
                    dgvSale["Trading", paramIndex].Value = 0;
                    dgvSale["ValueOnly", paramIndex].Value = 0;
                    dgvSale["NonStock", paramIndex].Value = 0;
                    dgvSale["Weight", paramIndex].Value = 0;

                    dgvSale["WareHouseRent", paramIndex].Value = 0;
                    dgvSale["WareHouseVAT", paramIndex].Value = 0;
                    dgvSale["IsCommercialImporter", paramIndex].Value = 0;
                    dgvSale["ProductDescription", paramIndex].Value = 0;

                    dgvSale["IsFixedVAT", paramIndex].Value = 0;
                    dgvSale["FixedVATAmount", paramIndex].Value = 0;
                    dgvSale["HPSRate", paramIndex].Value = 0;

                    dgvSale["CDNVATAmount", paramIndex].Value = 0;
                    dgvSale["CDNSDAmount", paramIndex].Value = 0;
                    dgvSale["CDNSubtotal", paramIndex].Value = 0;
                    #endregion
                }

                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }



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
                    ProductLoad();
                    //cmbProduct.Focus();
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
            PrintCopy = 1;
            try
            {
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                PreviewOnly = false;

                this.progressBar1.Visible = true;
                Program.fromOpen = "Me";
                string result = FormSalePrint.SelectOne(AlReadyPrintNo, txtSalesInvoiceNo.Text.Trim());
                string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());

                //FormIsPrinting.SelectOne(PrintResult[2], txtSalesInvoiceNo.Text.Trim());

                WantToPrint = PrintResult[1];
                if (WantToPrint == "N")
                {
                    this.progressBar1.Visible = false;
                    return;


                }
                else //if (result == ""){return;}else//if (result != "")
                {

                    frmIsPrinting = new FormIsPrinting();
                    frmIsPrinting.label3.Text = "VAT 6.3 # " + txtSalesInvoiceNo.Text.Trim() + " is printing on " + PrintResult[2] + "";
                    frmIsPrinting.Show();

                    //string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());

                    //////From Settings

                    #region Print Copy

                    try
                    {

                        if (false)
                        {


                            bool IsCentral = false;
                            IsCentral = settingVM.BranchInfoDT.Rows[0]["IsCentral"].ToString() == "Y" ? true : false;

                            if (IsCentral == true)
                            {
                                PrintCopy = Convert.ToInt32(ReportNumberOfCopiesPrint);

                            }
                            else
                            {
                                PrintCopy = Convert.ToInt32(ReportNumberOfCopiesPrint_Depo);

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        PrintCopy = 1;
                        ////throw;
                    }

                    #endregion

                    ////PrintCopy = Convert.ToInt32(PrintResult[0]);

                    WantToPrint = PrintResult[1];
                    VPrinterName = PrintResult[2];
                    backgroundWorkerPrint.RunWorkerAsync();

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }

            #endregion

        }

        private void backgroundWorkerPrint_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {

                reportDocument = Report_VAT6_3_Completed(PreviewOnly, PrintCopy, VPrinterName);


                //end DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_DoWork", ex.ToString());
            }

            #endregion
        }

        private void backgroundWorkerPrint_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                ReportPrint();

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
                FileLogger.Log(this.Name, "backgroundWorkerPrint_RunWorkerCompleted", ex.ToString());
            }
            finally
            {
                ////if (reportDocument != null)
                ////{
                ////    reportDocument.Close();
                ////    reportDocument.Dispose();
                ////}

                this.progressBar1.Visible = false;

            }

            #endregion

            //this.btnPrint.Enabled = true;
            //this.Enabled = true;
        }

        public void ReportPrint()
        {
            try
            {
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                //start Complete
                if (PreviewOnly)
                {
                    FormReport6_3 reports = new FormReport6_3();
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    //reportDocument.PrintOptions.PrinterName = "EPSON LQ-2190 ESC/P2";

                    //reportDocument.PrintOptions.PaperSize;
                    //
                    ////reports.ShowDialog();
                    reports.Show();
                }
                else
                {

                    try
                    {
                        for (int i = 1; i <= PrintCopy; i++)
                        {
                            PrinterSettings printerSettings =
                                new PrinterSettings();

                            printerSettings.PrinterName = VPrinterName;

                            reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);


                        }

                        //FormIsPrinting

                        frmIsPrinting.Close();
                        AlReadyPrintNo += 1;

                        MessageBox.Show("You have successfully print " + PrintCopy + " Copy(s)");
                    }
                    catch (Exception e)
                    {
                        FileLogger.Log(this.Name, "ReportShow", e.ToString());

                        throw e;
                    }
                    finally
                    {
                        if (reportDocument != null)
                        {
                            reportDocument.Close();
                            reportDocument.Dispose();
                        }
                    }

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

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.ToString());
            }
            finally
            {

            }

            #endregion


        }




        ////////To be Obsolete Sonn
        private void backgroundWorkerReportVAT11Ds_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {


        }
        private void backgroundWorkerReportVAT11Ds_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {




        }

        public void Report_VAT6_3_DoWorkX()
        {
            try
            {
                varSalesInvoiceNo = txtSalesInvoiceNo.Text.Trim();

                ReportDSDAL showReport = new ReportDSDAL();
                ReportResultVAT11 = new DataSet();
                ReportResultCreditNote = new DataSet();
                ReportResultDebitNote = new DataSet();
                ReportResultTracking = new DataSet();

                if (CommercialImporter)
                {
                    ReportResultVAT11 = showReport.VAT11ReportCommercialImporterNew(varSalesInvoiceNo, post1, post2);

                }
             
                else
                {
                    if (VAT11Name.ToLower() == "scbl")
                    {
                        ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n");

                    }
                    else
                    {
                        ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, post1, post2);
                        var tt = ReportResultVAT11.Tables[0].Rows[0]["ProductDescription"].ToString();
                        //var tt1 = ReportResultVAT11.Tables[0].Rows[0]["ProductCategory"].ToString();
                    }
                    BranchId = ReportResultVAT11.Tables[0].Rows[0]["BranchID"].ToString();

                    ReportResultTracking = showReport.SaleTrackingReport(varSalesInvoiceNo);
                }
                string[] cValues = { BranchId };

                string[] cFields = { "BranchID" };
                var branch = new BranchProfileDAL().SelectAll(null, cFields, cValues, null, null, true,connVM);

                BranchCode = branch.Rows[0]["BranchCode"].ToString();
                BranchName = branch.Rows[0]["BranchName"].ToString();
                BranchLegalName = branch.Rows[0]["BranchLegalName"].ToString();
                BranchAddress = branch.Rows[0]["Address"].ToString();
                BranchVatRegistrationNo = branch.Rows[0]["VatRegistrationNo"].ToString();

                //end DoWork
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
                FileLogger.Log(this.Name, "", exMessage);
            }


        }


        public ReportDocument Report_VAT6_3_Completed(bool IsPreview, int PrintCopy, string PrinterName = "")
        {
            try
            {
                SaleReport _report = new SaleReport();

                DataTable dt = new DataTable();
                string SKUCount = "";
                IntegrationParam vm = new IntegrationParam();
                vm.RefNo = txtSalesInvoiceNo.Text.Trim();
                vm.TransactionType = transactionType;
                dt = new SaleDAL().GetSource_SaleData_dis_Details(vm, connVM);
                if (dt != null && dt.Rows.Count > 0)
                {
                     SKUCount = dt.Rows[0]["SKUCount"].ToString();
                }

                reportDocument = _report.Report_VAT6_3_Completed(txtSalesInvoiceNo.Text.Trim(), transactionType
                    , rbtnCN.Checked
                    , rbtnDN.Checked
                    , false
                    , false
                    , false
                    , false
                    , PreviewOnly, PrintCopy, AlReadyPrintNo, chkIsBlank.Checked, chkIs11.Checked, chkValueOnly.Checked
                    , false, false, false, connVM, SKUCount);
            }
            catch (Exception ex)
            {
                reportDocument = new ReportDocument();
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "", exMessage);
            }
            return reportDocument;


        }

        #endregion

        #region History Methods

        //private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        //{
        //    Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    return File(stream, "application/PDF");
        //}
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

        #endregion

        #region Methods 05

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

        private void btnProductType_Click(object sender, EventArgs e)
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
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];

                ProductSearchDsFormLoad();


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

        #endregion

        #region Methods 06

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
                txtNBRPrice.ReadOnly = ChangeableNBRPrice == true ? false : true; ;
            }
            else
            {
                txtNBRPrice.ReadOnly = ChangeableNBRPrice == true ? false : true; ;
            }
            txtQty1.ReadOnly = ChangeableQuantity == true ? false : true; ;

            ChangeData = true;
        }

        #endregion

        #region Methods 07

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
            //Program.FormatTextBoxRate(txtSD, "SD Rate");
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();
        }

        #endregion

        #region Methods 08

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
            if (e.KeyCode.Equals(Keys.F9))
            {
                customerAddressLoad();
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


        private void btnDebitCredit_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                //this.btnDebitCredit.Enabled = false;
                this.progressBar1.Visible = true;

                #region Statement

                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                //this.btnDebitCredit.Enabled = false;
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



                backgroundWorkerReportCreditNoteDs.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ReportCreditNoteDs", exMessage);
            }

            #endregion

        }







        private void SaleDataLoadBeforeSave(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return;

            #region Clear Form

            TransactionTypes();


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

            chkPrint.Checked = false;
            IsPost = false;

            #endregion


            ImportId = dataTable.Rows[0]["Id"].ToString();
            cmbCurrency.Text = "BDT";

            bgwDetailsSearch.RunWorkerAsync();

            txtDeductionAmount.Text = Program.FormatingNumeric(Convert.ToString(dataTable.Rows[0]["DeductionAmount"]));

            cmbShift.SelectedValue = dataTable.Rows[0]["ShiftId"];
            vCustomerID = dataTable.Rows[0]["CustomerID"].ToString();
            txtCustomer.Text = dataTable.Rows[0]["CustomerName"].ToString();
            txtDeliveryAddress1.Text = dataTable.Rows[0]["DeliveryAddress1"].ToString();
            txtDeliveryAddress2.Text = dataTable.Rows[0]["DeliveryAddress2"].ToString();
            txtDeliveryAddress3.Text = dataTable.Rows[0]["DeliveryAddress3"].ToString();
            txtVehicleID.Text = dataTable.Rows[0]["VehicleID"].ToString();
            txtVehicleType.Text = dataTable.Rows[0]["VehicleType"].ToString();
            txtVehicleNo.Text = dataTable.Rows[0]["VehicleNo"].ToString();
            txtVehicleNo.Text = dataTable.Rows[0]["VehicleNo"].ToString();
            txtTotalAmount.Text = Convert.ToDecimal(dataTable.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
            txtTotalVATAmount.Text = Convert.ToDecimal(dataTable.Rows[0]["TotalVATAmount"].ToString()).ToString();//"0,0.00");

            #region

            txtHPSTotal.Text = Convert.ToDecimal(dataTable.Rows[0]["HPSTotalAmount"].ToString()).ToString();
            #endregion


            txtSerialNo.Text = dataTable.Rows[0]["SerialNo"].ToString();
            dtpInvoiceDate.Value = Convert.ToDateTime(dataTable.Rows[0]["InvoiceDate"].ToString());
            dtpDeliveryDate.Value = Convert.ToDateTime(dataTable.Rows[0]["DeliveryDate"].ToString());
            txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
            txtEXPFormNo.Text = dataTable.Rows[0]["EXPFormNo"].ToString();

          
          
            txtOldID.Text = dataTable.Rows[0]["PID"].ToString();
            if (string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Credit" ||
                string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Debit")
            {
                if (CreditWithoutTransaction == true)
                {
                    txtOldID.ReadOnly = false;
                }
            }


            cmbCurrency.Text = dataTable.Rows[0]["CurrencyCode"].ToString();
            txtCurrencyId.Text = dataTable.Rows[0]["CurrencyID"].ToString();
            txtBDTRate.Text = dataTable.Rows[0]["CurrencyRateFromBDT"].ToString();
            ImportExcelID = dataTable.Rows[0]["ImportID"].ToString();
            //if (DatabaseInfoVM.DatabaseName == "CPB_DB")
            //{
            txtImportID.Text = dataTable.Rows[0]["ImportID"].ToString();
          
            if (dataTable.Rows[0]["Trading"].ToString() == "Y")
            {
                chkTrading.Checked = true;
            }
            else
            {
                chkTrading.Checked = false;
            }
            if (dataTable.Rows[0]["Isprint"].ToString() == "Y")
            {
                chkPrint.Checked = true;
            }
            else
            {
                chkPrint.Checked = false;
            }
            TenderId = dataTable.Rows[0]["TenderId"].ToString();
            IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;
            SearchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"]);



            AlReadyPrintNo = Convert.ToInt32(dataTable.Rows[0]["AlReadyPrint"].ToString());

            this.btnSearchInvoiceNo.Enabled = false;
            this.progressBar1.Visible = true;
            
        }



        #endregion

        #region Methods 09


        private void btnOldID_Click(object sender, EventArgs e)
        {
            ISale sDal = GetObject();
            DataTable dataTable = new DataTable("SearchSalesHeader");
            try
            {
                #region Statement

                dgvReceiveHistory.Top = dgvSale.Top;
                dgvReceiveHistory.Left = dgvSale.Left;
                dgvReceiveHistory.Height = dgvSale.Height;
                dgvReceiveHistory.Width = dgvSale.Width;

                dgvSale.Visible = true;
                dgvReceiveHistory.Visible = false;


                Program.fromOpen = "Me";
                Program.SalesType = "New";
                string queryParam = "";
               
                    queryParam = "All";
                DataGridViewRow selectedRow = FormSaleSearch.SelectOne(queryParam);


                if (selectedRow != null && selectedRow.Index >= 0)
                {

                    #region Check Point

                    //if (selectedRow.Cells["Post1"].Value.ToString() == "N")
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

                    #endregion

                    #region Data Call

                    dataTable = sDal.SearchSalesHeaderDTNew(selectedRow.Cells["SalesInvoiceNo"].Value.ToString(), "", connVM);

                    #endregion

                    #region Value Assign to Form Elements
                    //txtSalesInvoiceNo.Text = Saleinfo[0];
                    txtOldID.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    txtDeliveryAddress1.Text = selectedRow.Cells["DeliveryAddress1"].Value.ToString();
                    txtDeliveryAddress2.Text = selectedRow.Cells["DeliveryAddress2"].Value.ToString();
                    txtDeliveryAddress3.Text = selectedRow.Cells["DeliveryAddress3"].Value.ToString();
                    txtVehicleID.Text = selectedRow.Cells["VehicleID"].Value.ToString();
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtTotalVATAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
                    txtSerialNo.Text = selectedRow.Cells["SerialNo"].Value.ToString();
                    //dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());InvoiceDateTime
                    //dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDateTime"].Value.ToString());
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    cmbCurrency.Text = selectedRow.Cells["CurrencyCode"].Value.ToString();
                    txtCurrencyId.Text = selectedRow.Cells["CurrencyID"].Value.ToString();
                    txtBDTRate.Text = selectedRow.Cells["CurrencyRateFromBDT"].Value.ToString();
                    transactionTypeOld = selectedRow.Cells["transactionType"].Value.ToString();

                    #region Condional Values

                    if (selectedRow.Cells["Trading"].Value == "Y")
                    {
                        chkTrading.Checked = true;
                    }
                    else
                    {
                        chkTrading.Checked = false;
                    }
                   
                        ReturnTransType = transactionTypeOld;
                  

                    #endregion

                    TenderId = selectedRow.Cells["TenderId"].Value.ToString();

                    SaleDetailData = string.Empty;

                    #region Conditional Values

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
                        txtNBRPrice.Enabled = ChangeableNBRPrice;
                        txtQty1.Enabled = ChangeableQuantity;

                        txtQty1.ReadOnly = false;
                        txtQty2.ReadOnly = false;

                        txtQuantityInHand.Visible = true;
                        LINhand.Visible = true;
                    }

                    if (transactionTypeOld == "ServiceNS")
                    {
                        txtQty1.Enabled = false;
                        txtQty2.Enabled = false;
                        txtNBRPrice.Enabled = ChangeableNBRPrice;
                        txtQty1.Enabled = ChangeableQuantity;

                        txtQty1.ReadOnly = true;
                        txtQty2.ReadOnly = true;

                        txtQuantityInHand.Visible = false;
                        LINhand.Visible = false;

                    }

                    #endregion

                    #endregion

                    #region Button Stats

                    this.btnOldID.Enabled = false;
                    this.progressBar1.Visible = true;
                    //this.Enabled = false;
                    SearchPreviousForCNDN = true;

                    #endregion

                    #region Background Worker

                    //backgroundWorkerBtnOldIdSearch.RunWorkerAsync();
                    backgroundNew.RunWorkerAsync();

                    #endregion

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

                ISale saleDal = GetObject();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData, connVM);


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
                FileLogger.Log(this.Name, "backgroundWorkerBtnOldIdSearch_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerBtnOldIdSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {


            try
            {
                #region Statement

                ISale saleDal = GetObject();

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
                    dgvReceiveHistory.Rows[j].Cells["ValueOnlyP"].Value = item["ValueOnly"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["BomIdP"].Value = item["BomId"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ProductDescriptionP"].Value = item["ProductDescription"].ToString();

                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = item["VATName"].ToString();

                    }

                    //dgvReceiveHistory.Rows[j].Cells["VATNameP"].Value = item["VATName"].ToString();
                    decimal returnQty = saleDal.ReturnSaleQty(SaleDetailData, item["ItemNo"].ToString(), connVM);
                    dgvReceiveHistory.Rows[j].Cells["RestQtyR"].Value =
                        (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value =
                        (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvReceiveHistory.Rows[j].Cells["CConvDateP"].Value = item["CConversionDate"].ToString();

                   
                    dgvReceiveHistory.Rows[j].Cells["WareHouseRentP"].Value = item["WareHouseRent"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["WareHouseVATP"].Value = item["WareHouseVAT"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ATVRateP"].Value = item["ATVRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ATVablePriceP"].Value = item["ATVablePrice"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ATVAmountP"].Value = item["ATVAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["IsCommercialImporterP"].Value = item["IsCommercialImporter"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TradeVATRateP"].Value = item["TradeVATRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TradeVATAmountP"].Value = item["TradeVATAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TradeVATableValueP"].Value = item["TradeVATableValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VDSAmountP"].Value = item["VDSAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["IsFixedVATP"].Value = item["IsFixedVAT"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["FixedVATAmountP"].Value = item["FixedVATAmount"].ToString();

                    j++;


                }

                #endregion Sale product load


               
                    IsUpdate = false;
                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                ChangeData = false;



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
           
        }
        private void bgwExportSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (chkPrint.Checked == true)
                {
                    //btnPrint.Enabled = false;
                    btnSave.Enabled = false;
                    //btnDebitCredit.Enabled = false;
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
                    //btnDebitCredit.Enabled = true;
                    btnAdd.Enabled = true;
                    btnChange.Enabled = true;
                    btnRemove.Enabled = true;
                    btnPost.Enabled = true;
                    bthUpdate.Enabled = true;



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
                    frmRptVAT17.txtUOM.Text = dgvSale.CurrentRow.Cells["UOMn"].Value.ToString();
                    frmRptVAT17.dtpFromDate.Value = dtpInvoiceDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpInvoiceDate.Value;
       
                    if (transactionType == "Wastage")
                    {
                        frmRptVAT17.rbtnWIP.Checked = true;
                    }
                }

                frmRptVAT17.Show();

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

            //cmbProduct.Focus();
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


        #endregion

        #region Methods 10
        private void btnEAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnERemove_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
           
        }


        private void btnEAdd_Click_1(object sender, EventArgs e)
        {
           
        }

        private void btnERemove_Click_1(object sender, EventArgs e)
        {

            
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
           
        }
        private void bgwTenderSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            
        }

        private void cmbVehicleNo_Leave(object sender, EventArgs e)
        {
           

        }

        #endregion

        #region Methods 11

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

                if (!string.IsNullOrEmpty(uOMFrom))
                {
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UomsValue", exMessage);
            }

            #endregion
        }

        private void UomsValueReverse()
        {
            try
            {
                #region Statement

                string uOMTo = txtUOM.Text.Trim().ToLower();
                string uOMFrom = cmbUom.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    vReverseUOMConversion = 0;
                    if (uOMFrom == uOMTo)
                    {
                        vReverseUOMConversion = 1;

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
                            vReverseUOMConversion = Convert.ToDecimal(uoms.First().ToString());

                        }
                        else
                        {
                            MessageBox.Show("Please select the UOM", this.Text);
                            vReverseUOMConversion = 0;
                            return;
                        }
                    }
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

                UomsValue();
                txtQuantity.Focus();

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
                this.progressBar1.Visible = true;
                PreviewOnly = true;
                PrintCopy = 1;
                backgroundWorkerPrint.RunWorkerAsync();
                //Report_VAT6_3_Completed(true,1);
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

        private void cmbProductCode_Leave(object sender, EventArgs e)
        {


        }

        private void cmbProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnVAT16_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                //MDIMainInterface mdi = new MDIMainInterface();
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
                    frmRptVAT16.txtUOM.Text = dgvSale.CurrentRow.Cells["UOMn"].Value.ToString();
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

                //ProductDAL productDal = new ProductDAL();
                //ProductResultDs = productDal.SearchProductMiniDS("", CategoryId, "", "", "", "", "", "");

                //end DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
            }

            #endregion
        }
        private void backgroundWorkerProductSearchDs_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //try
            //{
            //    //start Complete
            //    ProductsMini.Clear();
            //    cmbProduct.Items.Clear();

            //    foreach (DataRow item2 in ProductResultDs.Rows)
            //    {
            //        var prod = new ProductMiniDTO();
            //        prod.ItemNo = item2["ItemNo"].ToString();
            //        prod.ProductName = item2["ProductName"].ToString();
            //        cmbProduct.Items.Add(item2["ProductName"]);
            //        prod.ProductCode = item2["ProductCode"].ToString();


            //        prod.CategoryID = item2["CategoryID"].ToString();
            //        prod.CategoryName = item2["CategoryName"].ToString();
            //        prod.UOM = item2["UOM"].ToString();
            //        prod.HSCodeNo = item2["HSCodeNo"].ToString();
            //        prod.IsRaw = item2["IsRaw"].ToString();

            //        prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
            //        prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
            //        prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
            //        prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
            //        prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
            //        prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());

            //        prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
            //        prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
            //        prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
            //        prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
            //        prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
            //        prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

            //        prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
            //        prod.SD = Convert.ToDecimal(item2["SD"].ToString());
            //        prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
            //        prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
            //        prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
            //        prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
            //        prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;


            //        ProductsMini.Add(prod);

            //    }//end foreach
            //    //end Complete
            //}
            //#region catch

            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
            //}

            //#endregion
            ////this.Enabled = true;
            //this.progressBar1.Visible = false;
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
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                //end Complete
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
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                //end Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerReportCreditNoteDs_RunWorkerCompleted", exMessage);
            }

            #endregion
            //this.Enabled = true;
            //this.btnDebitCredit.Enabled = true;
            //this.progressBar1.Visible = false;
        }

        #endregion

        #region Methods 12

        private void btnSearchProductName_Click(object sender, EventArgs e)
        {


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
                //chkIs11.Text = "VAT11 Gha";
                //btnPrint.Text = "VAT11 Gha";
            }
        }

        private void chkIsBlank_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bgwVAT12_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void Discount()
        {
            decimal discountPercent = 0;
            decimal discountAmount = 0;
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


           


            if (!string.IsNullOrEmpty(txtDiscountAmountInput.Text.Trim()))
            {
                if (chkDiscount.Checked)
                {
                    discountAmount = Convert.ToDecimal(txtDiscountAmountInput.Text.Trim());

                   
                }
                else
                {
                    discountAmount = 0;
                }
            }

            txtDiscountAmountInput.Text = Program.ParseDecimalObject(discountAmount);
            
            



        }

        private void chkDiscount_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDiscount.Checked)
            {
                chkDiscount.Text = "Discount(F)";

            }
            else
            {
                chkDiscount.Text = "Discount(%)";

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
                        }
                        else
                        {
                            //MessageBox.Show("Please select the Currency or check the currency rate from BDT", this.Text);
                        }
                    }
                    else
                    {
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
            Program.FormatTextBox(txtBDTRate, "Currency Rate");
           
                txtBDTRate.Text = "1";
            

            Rowcalculate();
        }

        #endregion

        #region Methods 13


        private void cmbCGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
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

        }

        private void ProductLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();
                if (saleFromProduction == "Y" && !string.IsNullOrEmpty(txtSerialNo.Text.Trim()))
                {

                    string SqlText = @" select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,h.ReferenceNo,d.Quantity ,d.UOM,h.ReceiveNo,h.ReceiveDateTime
                    from ReceiveDetails d
                    left outer join  ReceiveHeaders h on d.ReceiveNo=h.ReceiveNo
                    left outer join  Products p on d.ItemNo=p.ItemNo";


                    string[] shortColumnName = { "h.ReferenceNo" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, txtSerialNo.Text.Trim(), shortColumnName);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();
                        txtProductCode.Text = vItemNo;

                        ////cmbProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();

                        txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();

                    }
                }
                else if (CustomerWiseBOM)
                {
                    selectedRow = FormProductFinder.SelectOne(vCustomerID, IsRawParam);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();
                        txtProductCode.Text = vItemNo;

                        ////cmbProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();
                        txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();

                    }
                }
                else
                {


                    string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";
                    if (!string.IsNullOrEmpty(CategoryId))
                    {
                        SqlText += @"  and pc.CategoryID='" + CategoryId + "'  ";
                    }
                    else if (!string.IsNullOrEmpty(IsRawParam))
                    {
                        SqlText += @"  and pc.IsRaw='" + IsRawParam + "'  ";
                    }

                    string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                    string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();
                        txtProductCode.Text = vItemNo;
                        ////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();
                        txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();

                    }

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #endregion

        #region Methods 14

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
            ShiftDAL _sDal = new ShiftDAL();
            DataTable shiftResult = new DataTable();
            string InvoiceDate = dtpInvoiceDate.Value.ToString("HH:mm");
            shiftResult = _sDal.SearchForTime(InvoiceDate);

            if (shiftResult.Rows.Count > 0)
            {
                cmbShift.DataSource = null;
                cmbShift.DataSource = shiftResult;
                cmbShift.DisplayMember = "ShiftName";// displayMember.Replace("[", "").Replace("]", "").Trim();
                cmbShift.ValueMember = "Id";
            }


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

            //if (cmbVATType.Text == "MRPRate(SC)")
            if (cmbVATType.SelectedValue == "mrprate(sc)")
            {
                txtHPSRate.Text = OriginalHPSRate.ToString();
            }
            else
            {
                txtHPSRate.Text = "0.00";
            }

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
            dtpInvoiceDate.Value = Program.ParseDate(dtpInvoiceDate);

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
        }

        #endregion

        #region Methods 15

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


                    ProductVM vProductVM = new ProductVM();
                    ProductDAL productDal = new ProductDAL();

                    vProductVM = productDal.SelectAll(frmRptVATKa.txtItemNo.Text, null, null, null, null, null, connVM).FirstOrDefault();

                    if (vProductVM.ProductType.ToLower() != "trading")
                    {
                        MessageBox.Show(vProductVM.ProductName + " is not a Trading Product!", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                        return;
                    }
                }


                frmRptVATKa.ShowDialog();

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
                FileLogger.Log(this.Name, "btnFormKaT_Click", exMessage);
            }
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);

            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew(string.Empty, string.Empty, string.Empty,
            //     string.Empty, "Y", dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));


            string[] cFields = new string[] { "Y", dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss") };
            string[] cValues = new string[] { "cc.ActiveStatus like", "cc.ConversionDate" };

            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, false, connVM);

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
                            dgvSale.Rows[j].Cells["ProductDescription"].Value = dgvReceiveHistory["ProductDescriptionP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["IsFixedVAT"].Value = dgvReceiveHistory["IsFixedVATP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["FixedVATAmount"].Value = dgvReceiveHistory["FixedVATAmountP", i].Value.ToString();

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
                            dgvSale.Rows[j].Cells["ValueOnly"].Value = dgvReceiveHistory["ValueOnlyP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["TotalValue"].Value = "0";
                            dgvSale.Rows[j].Cells["WareHouseRent"].Value = dgvReceiveHistory["WareHouseRentP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["WareHouseVAT"].Value = dgvReceiveHistory["WareHouseVATP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["ATVRate"].Value = dgvReceiveHistory["ATVRateP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["ATVablePrice"].Value = dgvReceiveHistory["ATVablePriceP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["ATVAmount"].Value = dgvReceiveHistory["ATVAmountP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["IsCommercialImporter"].Value = dgvReceiveHistory["IsCommercialImporterP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["TradeVATRate"].Value = dgvReceiveHistory["TradeVATRateP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["TradeVATAmount"].Value = dgvReceiveHistory["TradeVATAmountP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["TradeVATableValue"].Value = dgvReceiveHistory["TradeVATableValueP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["VDSAmount"].Value = dgvReceiveHistory["VDSAmountP", i].Value.ToString();
                            dgvSale.Rows[j].Cells["BomId"].Value = dgvReceiveHistory["BomIdP", i].Value.ToString();


                            if (!chkTrading.Checked)
                            {
                                dgvSale.Rows[j].Cells["CConvDate"].Value = dgvReceiveHistory["CConvDateP", i].Value.ToString();
                            }


                            #region currency convertion date change for export
                           
                            #endregion currency convertion date change for export

                                dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                           

                            j = j + 1;
                        }
                    }
                   
                   
                        Rowcalculate();
                   
                    dgvReceiveHistory.Visible = false;
                    dgvSale.Visible = true;

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
            VATcalc();
            btnAdd.Focus();
        }

        private void VATcalc()
        {
            UomsValue();

            decimal UomConv = Convert.ToDecimal(txtUomConv.Text == "" ? "1" : txtUomConv.Text);
            decimal Qty1 = Convert.ToDecimal(txtQty1.Text == "" ? "0" : txtQty1.Text);
            decimal Qty2 = Convert.ToDecimal(txtQty2.Text == "" ? "0" : txtQty2.Text);
            decimal VATRate = Convert.ToDecimal(txtVATRate.Text == "" ? "0" : txtVATRate.Text);
            decimal SD = Convert.ToDecimal(txtSD.Text == "" ? "0" : txtSD.Text);

            if (chkIsFixedOtherSD.Checked)
            {
                txtFixedSDAmount.Text = Program.ParseDecimal(((Qty1 + Qty2) * SD * UomConv).ToString()).ToString();

            }

            if (chkIsFixedOtherVAT.Checked)
            {
                txtFixedVATAmount.Text = Program.ParseDecimal(((Qty1 + Qty2) * VATRate * UomConv).ToString()).ToString();

            }

        }

        private void txtQty1_Leave(object sender, EventArgs e)
        {
            txtQty1.Text = Program.ParseDecimalObject(txtQty1.Text.Trim()).ToString();
            TotalQty();
            ////////if (rbtnCN.Checked)
            ////////{
            ////////    decimal quantity = Convert.ToDecimal(dgvSale.CurrentRow.Cells["Quantity"].Value);
            ////////    if (Convert.ToDecimal(txtQty1.Text) > quantity)
            ////////    {
            ////////        MessageBox.Show("Credit Note can not be greater than actual quantity.");
            ////////        txtQty1.Text = "0";
            ////////        txtQty1.Focus();
            ////////        return;
            ////////    }
            ////////}

            //if (Program.CheckingNumericTextBox(txtQty1, "Total Quantity") == true)
            //{
            //    txtQty1.Text = Program.FormatingNumeric(txtQty1.Text.Trim(), SalePlaceQty).ToString();
            //    TotalQty();
            //}
            VATcalc();
            ////////if (rbtnService.Checked)
            ////////{
            ////////    txtNBRPrice.Focus();
            ////////}
            ////////else if (rbtnServiceNS.Checked)
            ////////{
            ////////    txtNBRPrice.Focus();

            ////////}
            ////////else if (rbtnExport.Checked)
            ////////{
            ////////    txtNBRPrice.Focus();

            ////////}
            ////////else
            ////////{
            ////////    btnAdd.Focus();

            ////////}
            txtQty2.Focus();
        }

        private void TotalQty()
        {
            string qty2 = string.IsNullOrEmpty(txtQty2.Text.Trim()) ? "0" : txtQty2.Text.Trim();

            string qty1 = string.IsNullOrEmpty(txtQty1.Text.Trim()) ? "0" : txtQty1.Text.Trim();

            txtQuantity.Text =
                Convert.ToString(Convert.ToDecimal(qty1) + Convert.ToDecimal(qty2));
        }

        private void txtQty1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                    btnAdd.Focus();
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

            try
            {
                TransactionTypes();
                FormSaleImport fmi = new FormSaleImport();
                //fmi.preSelectTable = "Sales";
                //fmi.transactionType = transactionType;
                //fmi.Show();

                var value = new CommonDAL().settingValue("CompanyCode", "Code");

                if (value == DHL)
                {
                    string ImportId = FormSaleImportDHL.SelectOne(transactionType, chkCDN.Checked);
                    ISale _slDal = GetObject();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }

                }
                else if (value == KCL)
                {
                    string ImportId = FormSaleImportKCL.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "KCCL")
                {
                    string ImportId = FormSaleImportKCCL.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "SQR")
                {
                    string[] ids = FormSaleImportSQR.SelectOne(transactionType, chkCDN.Checked).Split('~');
                    string ImportId = ids[0];
                    IsCustomerExempted = ids[1];

                    ISale _slDal = GetObject();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        //DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        //SaleDataLoad(dt);

                        DataTable dt = _slDal.SelectAllHeaderTemp(0, new[] { "sih.ID", "sih.UserId" },
                            new[] { ImportId, Program.CurrentUserID });
                        SaleDataLoadBeforeSave(dt);
                    }
                }
                else if (value == "SMC")
                {
                    string ImportId = FormSaleImportSMC.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "GDIC")
                {
                    string ImportId = FormSaleImportGDIC.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }

                else if (value == "NESTLE")
                {
                    string ImportId = FormSaleImport.SelectOne(transactionType, chkCDN.Checked);
                    //string ImportId = "62114";
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        //DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        //SaleDataLoad(dt);

                        DataTable dt = _slDal.SelectAllHeaderTemp(0, new[] { "sih.ID", "sih.UserId" },
                            new[] { ImportId, Program.CurrentUserID });
                        SaleDataLoadBeforeSave(dt);
                    }
                }
                else if (value == "ITS")
                {
                    string ImportId = FormSaleImportIntertek.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (OrdinaryVATDesktop.IsACICompany(value))
                {
                    string ImportId = FormSaleImportACI.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "TEST")
                {
                    string ImportId = FormSaleImportNewProc.SelectOne(transactionType);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "BCL")
                {
                    string ImportId = FormSaleImportBCL.SelectOne(transactionType);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "BATA")
                {
                    string ImportId = FormSaleImportBata.SelectOne(transactionType);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "BTSL")
                {
                    string ImportId = FormSaleImportBTSL.SelectOne(transactionType);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else if (value == "SSD")
                {
                    string ImportId = FormSaleImportSSD.SelectOne(transactionType);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }
                else
                {
                    string ImportId = FormSaleImport.SelectOne(transactionType, chkCDN.Checked);
                    SaleDAL _slDal = new SaleDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        SaleDataLoad(dt);
                    }
                }


                //SetUpSaleForm(table);

            }
            catch (Exception ex)
            {

                throw;
            }

            finally
            {
                this.progressBar1.Visible = false;

            }


        }

        private void SetUpSaleForm(DataTable table)
        {
            try
            {
                // dataTable = sDal.SearchSalesHeaderDTNew(selectedRow.Cells["SalesInvoiceNo"].Value.ToString());
                txtDeductionAmount.Text = Program.FormatingNumeric(Convert.ToString(table.Rows[0]["DeductionAmount"]));

                cmbShift.SelectedValue = table.Rows[0]["ShiftId"];
                txtSalesInvoiceNo.Text = table.Rows[0]["SalesInvoiceNo"].ToString();
                vCustomerID = table.Rows[0]["CustomerID"].ToString();
                txtCustomer.Text = table.Rows[0]["CustomerName"].ToString();
                txtDeliveryAddress1.Text = table.Rows[0]["DeliveryAddress1"].ToString();
                txtDeliveryAddress2.Text = table.Rows[0]["DeliveryAddress2"].ToString();
                txtDeliveryAddress3.Text = table.Rows[0]["DeliveryAddress3"].ToString();
                txtVehicleID.Text = table.Rows[0]["VehicleID"].ToString();
                txtVehicleType.Text = table.Rows[0]["VehicleType"].ToString();
                txtVehicleNo.Text = table.Rows[0]["VehicleNo"].ToString();
                txtVehicleNo.Text = table.Rows[0]["VehicleNo"].ToString();
                txtTotalAmount.Text = Convert.ToDecimal(table.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
                txtTotalVATAmount.Text = Convert.ToDecimal(table.Rows[0]["TotalVATAmount"].ToString()).ToString();//"0,0.00");
                txtSerialNo.Text = table.Rows[0]["SerialNo"].ToString();
                dtpInvoiceDate.Value = Convert.ToDateTime(table.Rows[0]["InvoiceDate"].ToString());
                dtpDeliveryDate.Value = Convert.ToDateTime(table.Rows[0]["DeliveryDate"].ToString());
                txtComments.Text = table.Rows[0]["Comments"].ToString();
              
                txtEXPFormNo.Text = table.Rows[0]["EXPFormNo"].ToString();


               
                
                if (table.Rows[0]["IsDeemedExport"].ToString().ToLower() == "y")
                {
                    chkIsDeemedExport.Checked = true;
                }
                txtOldID.Text = table.Rows[0]["PID"].ToString();
                if (string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Credit" ||
                    string.IsNullOrEmpty(txtOldID.Text) && transactionType == "Debit")
                {
                    if (CreditWithoutTransaction == true)
                    {
                        txtOldID.ReadOnly = false;
                    }
                }


                cmbCurrency.Text = table.Rows[0]["CurrencyCode"].ToString();
                txtCurrencyId.Text = table.Rows[0]["CurrencyID"].ToString();
                txtBDTRate.Text = table.Rows[0]["CurrencyRateFromBDT"].ToString();
                ImportExcelID = table.Rows[0]["ImportID"].ToString();             
                txtImportID.Text = table.Rows[0]["ImportID"].ToString();             
                if (table.Rows[0]["Trading"].ToString() == "Y")
                {
                    chkTrading.Checked = true;
                }
                else
                {
                    chkTrading.Checked = false;
                }
                if (table.Rows[0]["Isprint"].ToString() == "Y")
                {
                    chkPrint.Checked = true;
                }
                else
                {
                    chkPrint.Checked = false;
                }
                TenderId = table.Rows[0]["TenderId"].ToString();
                IsPost = Convert.ToString(table.Rows[0]["Post1"].ToString()) == "Y" ? true : false;
                SearchBranchId = Convert.ToInt32(table.Rows[0]["BranchId"]);


                if (txtSalesInvoiceNo.Text == "")
                {
                    SaleDetailData = "0";
                }
                else
                {
                    SaleDetailData = txtSalesInvoiceNo.Text.Trim();
                }
                AlReadyPrintNo = Convert.ToInt32(table.Rows[0]["AlReadyPrint"].ToString());


                InsertTrackingInfo();
                this.btnSearchInvoiceNo.Enabled = false;


                btnPrint.Enabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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

        public string[] ImportFromText()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";


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
            dtSaleD.Columns.Add("Total_Price");
            dtSaleD.Columns.Add("TotalValue");

            dtSaleD.Columns.Add("WareHouseRent");
            dtSaleD.Columns.Add("WareHouseVAT");
            dtSaleD.Columns.Add("ATVRate");
            dtSaleD.Columns.Add("ATVablePrice");
            dtSaleD.Columns.Add("ATVAmount");
            dtSaleD.Columns.Add("IsCommercialImporter");
            dtSaleD.Columns.Add("ValueOnly");

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


                SAVE_DOWORK_SUCCESS = false;
                SaleDAL saleDal = new SaleDAL();
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE,false,Program.BranchId,null,Program.CurrentUserID);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally

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
            if (CurrencyConversiondate == "1900-01-01 00:00:00" || CurrencyConversiondate == "1900-Jan-01 00:00:00")
            {
                ccDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            }
            else
            {
                ccDate = dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
            }
            CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();

            string[] cValues = new string[] { cmbCurrency.Text.Trim(), ccDate };
            string[] cFields = new string[] { "c.CurrencyCode", "cc.ConversionDate<" };
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
           
             //dtpConversionDate.Value = Convert.ToDateTime(ccDate);
             dtpConversionDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

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

        #endregion

        #region Methods 16

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
                ISale DeliveryChallan = GetObject();
                DeliveryChallan.SetDeliveryChallanNo(txtSalesInvoiceNo.Text.Trim(), dtpDeliveryDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"), connVM);
                #endregion Generate DCNo

                #region Delivery Report

                //ReportResultDelivry = new DataSet();
                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ReportResultDelivry = reportDsdal.RptDeliveryReport(txtSalesInvoiceNo.Text.Trim(), connVM);

                #region Statement

                // Start Complete
                ReportResultDelivry.Tables[0].TableName = "DsDelivery";
                RptDeliveryReport objrpt = new RptDeliveryReport();
                objrpt.SetDataSource(ReportResultDelivry);

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'DELEVERY CHALLAN'";
                objrpt.DataDefinition.FormulaFields["TransportNo"].Text = "'" + txtVehicleNo.Text.Trim() + "'";
                objrpt.DataDefinition.FormulaFields["TripNo"].Text = "'" + txtSerialNo.Text.Trim() + "'";
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
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                // End Complete

                #endregion

                #endregion  Delivery Report


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

                //ReportResultDelivry = new DataSet();
                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ReportResultDelivry = reportDsdal.RptDeliveryReport(txtSalesInvoiceNo.Text.Trim(), connVM);

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
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                // End Complete

                #endregion

                #endregion  GPR Report


                #region Update Info
                //SaleDAL DeliveryChallan = new SaleDAL();
                ISale DeliveryChallan = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);

                DeliveryChallan.SetGatePass(txtSalesInvoiceNo.Text.Trim(), connVM);
                #endregion Update Info

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
                        
                    if (IsUpdate == true)
                        {
                            trackingCmb.SaleInvoiceNo = txtSalesInvoiceNo.Text;
                        }

                        trackingsCmb.Add(trackingCmb);
                    }

                    trackingsVm.Clear();

                    Program.fromOpen = "Me";
                   

                }
            }
            #endregion
            #region Catch

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
                
                
                    saleNo = SaleDetailData;

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
            DataGridView senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {

                if (e.ColumnIndex == 1) // checking remove button
                {
                    RemoveSelectedRow();
                }
                else if (e.ColumnIndex == 2)
                {
                    Rowcalculate();
                }
            }
        }

        private void cmbProduct_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            vCustomerID = "0";
            txtCustomer.Text = "";
            txtDeliveryAddress1.Text = "";
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
                string ShowAllCustomer = commonDal.settingsDesktop("Setup", "ShowAllCustomer");
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
                    vCustomerID = "0";
                    txtCustomer.Text = "";
                    txtDeliveryAddress1.Text = "";
                    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                    txtDeliveryAddress1.Text = selectedRow.Cells["Address1"].Value.ToString();//ProductInfo[0];
                    IsCustomerExempted = selectedRow.Cells["IsExamted"].Value.ToString();//ProductInfo[0];
                    IsCustomerSpecialRate = selectedRow.Cells["IsSpecialRate"].Value.ToString();//ProductInfo[0];

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
        private void customerAddressLoad()
        {
            try
            {

                if (string.IsNullOrEmpty(vCustomerID) || vCustomerID == "0")
                {
                    MessageBox.Show("Please select the customer first");
                    return;
                }
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select c.CustomerID, cg.CustomerGroupName,c.CustomerCode,c.CustomerName,isnull(ca.CustomerAddress,c.Address1)CustomerAddress,isnull(ca.CustomerVATRegNo,c.VATRegistrationNo)CustomerVATRegNo from Customers c 
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID 
left outer join CustomersAddress ca on c.CustomerID=ca.CustomerID 
where 1=1 and  c.ActiveStatus='Y'  ";
                SqlText += @"  and c.CustomerID='" + vCustomerID + "'  ";

                string SQLTextRecordCount = @" select count(CustomerCode)RecordNo from Customers";

                string[] shortColumnName = { "cg.CustomerGroupName", "c.CustomerCode", "c.CustomerName", "ca.CustomerAddress", "ca.CustomerVATRegNo" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtDeliveryAddress1.Text = "";
                    txtDeliveryAddress1.Text = selectedRow.Cells["CustomerAddress"].Value.ToString();//ProductInfo[0];
                    txtDeliveryAddress1.Focus();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerAddressLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //DataGridViewRow selectedRow = new DataGridViewRow();
            //selectedRow = FormCustomerAddressFinder.SelectOne(vCustomerID);
            //if (selectedRow != null && selectedRow.Index >= 0)
            //{
            //    txtDeliveryAddress1.Text = "";
            //    txtDeliveryAddress1.Text = selectedRow.Cells["CustomerAddress"].Value.ToString();//ProductInfo[0];
            //}
            //txtDeliveryAddress1.Focus();
        }
        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbProductCode_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void txtCustomer_DoubleClick(object sender, EventArgs e)
        {
            customerLoad();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ProductLoad();
            txtProductName.Focus();
        }

        private void bgwVAT11_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void dgvExport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnEAdd_Click_2(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Methods 17

        private void btnERemove_Click_2(object sender, EventArgs e)
        {
           
        }

        private void dgvExport_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvExport_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
           
        }

       

       
        private void btnTR_Click(object sender, EventArgs e)
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
              
                }

                frmRptVAT17.Show();

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
                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
            }

            #endregion
        }

        private void txtPacketPrice_Leave(object sender, EventArgs e)
        {
            //txtPacketPrice.Text = Program.FormatingNumeric(txtPacketPrice.Text.Trim(), SalePlaceTaka).ToString();
            txtPacketPrice.Text = Program.ParseDecimalObject(txtPacketPrice.Text.Trim()).ToString();
            UomsValueReverse();
            decimal PacketPrice = 0;
            decimal UomConv = 0;
            decimal NBRPrice = 0;
            //////NBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());
            PacketPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
            //////UomConv=Convert.ToDecimal( txtUomConv.Text.Trim());
            txtNBRPrice.Text = (PacketPrice * vReverseUOMConversion).ToString();

        }

        private void txtPacketPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTotalValue_Leave(object sender, EventArgs e)
        {
            //txtTotalValue.Text = Program.FormatingNumeric(txtTotalValue.Text.Trim(), SalePlaceTaka).ToString();
            txtTotalValue.Text = Program.ParseDecimalObject(txtTotalValue.Text.Trim()).ToString();
            if (CommercialImporter)
            {
                //txtNBRPrice.Text = CommercialImporterCalculation(txtTotalValue.Text.Trim(), txtVATRate.Text.Trim(), txtQty1.Text.Trim()).ToString();
                txtNBRPrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtTotalValue.Text.Trim()) / Convert.ToDecimal(txtQty1.Text.Trim())).ToString();// CommercialImporterCalculation(txtTotalValue.Text.Trim(), txtVATRate.Text.Trim(), txtQty1.Text.Trim()).ToString();

            }
        }

        private decimal CommercialImporterCalculation1(string TotalValue, string VATRate, string Quantity)
        {
            cTotalValue = 0;
            cQuantity = 0;
            cATVablePrice = 0;
            cATVAmount = 0;
            cWareHouseRent = 0;
            cWareHouseVAT = 0;
            cVATRate = 0;
            cVATablePrice = 0;
            cVATAmount = 0;
            // cTradeVATRate = Convert.ToDecimal(133.34);
            //cWareHouseRentPerQuantity = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "WareHouseRentPerQuantity"));
            //cATVRate = Convert.ToDecimal(commonDal.settingsDesktop("Sale", "ATVRate"));

            if (CommercialImporter)
            {
                cTotalValue = Convert.ToDecimal(TotalValue);
                cVATRate = Convert.ToDecimal(VATRate);
                cQuantity = Convert.ToDecimal(Quantity);
                //cWareHouseRent = cWareHouseRentPerQuantity * cQuantity;
                cWareHouseVAT = cWareHouseRent * cVATRate / 100;
                cATVablePrice = (cTotalValue - (cWareHouseRent + cWareHouseVAT)) * 100 / (cATVRate + 100);
                cATVAmount = cATVablePrice * cATVRate / 100;
                //cVATablePrice = (cATVablePrice * 100) / (cTradeVATRate);
                cVATAmount = cVATablePrice * cVATRate / 100;
            }
            return cVATablePrice;

        }

        private void txtTotalValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkCustomerWiseBOM_CheckedChanged(object sender, EventArgs e)
        {
            CustomerWiseBOM = false;
            if (chkCustomerWiseBOM.Checked)
            {
                CustomerWiseBOM = true;
            }
            else
            {
                CustomerWiseBOM = false;
            }
        }

        private void txtVehicleNo_DoubleClick(object sender, EventArgs e)
        {
            VehicleLoad();
        }

        private void txtVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.Equals(Keys.F9))
            {
                VehicleLoad();
            }
        }

        private void VehicleLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select  VehicleID,VehicleNo,VehicleType,Description,Comments from Vehicles
 where 1=1 and ActiveStatus='Y' ";

                string SQLTextRecordCount = @" select count(VehicleNo)RecordNo from Vehicles";


                string[] shortColumnName = { "VehicleNo", "VehicleType" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);


                //string[] shortColumnName = {  "VehicleNo", "VehicleType"};
                //string tableName = "Vehicles";
                //selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();//products.VehicleNo;
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();//products.VehicleType;
                    dtpInvoiceDate.Focus();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "VehicleLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtVehicleNo_Leave(object sender, EventArgs e)
        {
            try
            {
                //#region Statement


                //// txtVehicleNo.Text = "";
                ////txtVehicleType.ReadOnly = true;

                //var searchText = txtVehicleNo.Text.Trim().ToLower();
                //if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                //{
                //    var vehicleNo = from prd in vehicles.ToList()
                //                    where prd.VehicleNo.ToLower() == searchText.ToLower()
                //                    select prd;
                //    if (vehicleNo != null && vehicleNo.Any())
                //    {
                //        var vechicls = vehicleNo.First();
                //        txtVehicleNo.Text = vechicls.VehicleNo;
                //        txtVehicleType.Text = vechicls.VehicleType;
                //        dtpInvoiceDate.Focus();
                //    }
                //    else
                //    {
                //        if (rbtnCN.Checked == false || rbtnDN.Checked == false)
                //        {
                //            if (txtSalesInvoiceNo.Text == "~~~ New ~~~")
                //            {
                //                chkVehicleSaveInDatabase.Checked = true;
                //                if (
                //                MessageBox.Show("vehicle Number not in database, Add new Vehicle?", this.Text, MessageBoxButtons.YesNo,
                //                        MessageBoxIcon.Question) != DialogResult.Yes)
                //                {

                //                    chkVehicleSaveInDatabase.Checked = false;

                //                }
                //                //txtVehicleType.ReadOnly = false;
                //            }
                //        }
                //    }
                //}

                //// Check Vehicle Type
                //if (string.IsNullOrEmpty(txtVehicleType.Text))
                //{
                //    //txtVehicleType.ReadOnly = false;
                //}

                //#endregion
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
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
            }

            #endregion
        }

        private void gbExport_Enter_1(object sender, EventArgs e)
        {

        }

        private void TripLoad()
        {
            try
            {
                if (saleFromProduction.ToLower() == "y")
                {
                    //ReceiveDAL _sDal = new ReceiveDAL();
                    IReceive _sDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);
                    DataTable dt = new DataTable();
                    dt = _sDal.SearchByReferenceNo(txtSerialNo.Text.Trim(), "", connVM, transactionType);

                    if (dt.Rows.Count <= 0)
                    {

                        MessageBox.Show("This Ref/Trip # Not found .");
                        txtSerialNo.Text = "";
                        return;
                    }
                    string IsTripComplete = dt.Rows[0]["IsTripComplete"].ToString();
                    if (IsTripComplete.ToLower() == "y")
                    {
                        MessageBox.Show("This Ref/Trip # Already Used.");
                        txtSerialNo.Text = "";
                        return;
                    }
                    else
                    {
                        

                       
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnTripLoad_Click(object sender, EventArgs e)
        {

            TripLoad();
        }

        #endregion

        #region Methods 18

        private void button6_Click(object sender, EventArgs e)
        {
            

        }

        private void GF_Enter(object sender, EventArgs e)
        {

        }

        private void txtDeliveryAddress1_DoubleClick(object sender, EventArgs e)
        {
            customerAddressLoad();
        }

        private void txtSerialNo_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {

                TripLoad();

                txtProductName.Focus();
            }

        }

        private void txtSerialNo_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void btnCommentUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SaleDAL saleDal = new SaleDAL();
                if (txtSalesInvoiceNo.Text != "~~~ New ~~~")
                {
                    SaleMasterVM masterVm = new SaleMasterVM();

                    //masterVm.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text);
                    masterVm.Comments = txtComments.Text;
                    //masterVm.CurrencyID = txtCurrencyId.Text.Trim();
                    masterVm.SalesInvoiceNo = txtSalesInvoiceNo.Text;

                    string[] result = saleDal.SalesUpdateComment(masterVm);

                    if (result[0].ToLower() == "success")
                    {
                        MessageBox.Show("Comment updated Successfully");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void backgroundWorkerReportVAT11Ds12_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                ReportDSDAL showReport = new ReportDSDAL();
                ReportResultVAT11 = new DataSet();
                ReportResultCreditNote = new DataSet();
                ReportResultDebitNote = new DataSet();
                ReportResultTracking = new DataSet();
                
                    ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, post1, post2);
                    ReportResultTracking = showReport.SaleTrackingReport(varSalesInvoiceNo);
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
                FileLogger.Log(this.Name, "backgroundWorkerReportVAT11Ds_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerReportVAT11Ds12_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region
                string vQuantity, vSDAmount, vQty_UnitCost, vQty_UnitCost_SDAmount, vVATAmount, vSubtotal, QtyInWord, totalTAX, maxVDS;// = string.Empty;  
                vQuantity = string.Empty;
                vSDAmount = string.Empty;
                vQty_UnitCost = string.Empty;
                vQty_UnitCost_SDAmount = string.Empty;
                vVATAmount = string.Empty;
                vSubtotal = string.Empty;
                QtyInWord = string.Empty;
                maxVDS = string.Empty;
                totalTAX = string.Empty;
                decimal Quantity = 0,
                           SDAmount = 0,
                           Qty_UnitCost = 0,
                           Qty_UnitCost_SDAmount = 0,
                           VATAmount = 0,
                           Subtotal = 0;
                //start Complete
               
               
              
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
                    totalTAX = Convert.ToDecimal(VATAmount + SDAmount).ToString("0,0.00");
                    maxVDS = Convert.ToDecimal(VATAmount / 3).ToString("0,0.00");
                    #endregion InWord
              
                #endregion
                string prefix = "";
                ReportClass objrpt = new ReportClass();

                //Ignoring other business

                #region currency
                SaleDAL saleDal = new SaleDAL();
                string currencyMajor = "";
                string currencyMinor = "";
                string currencySymbol = "";
                sqlResults = new string[4];
                if (!string.IsNullOrEmpty(txtCustomer.Text))
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
                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "'" + currencySymbol + "'";
                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                if (PreviewOnly == true)
                {
                    //Preview Only
                    if (!string.IsNullOrEmpty(txtCustomer.Text))
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                    }
                    objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "''";
                    reports.setReportSource(objrpt);
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

        private void ChkExpLoc_Click(object sender, EventArgs e)
        {
            if (ChkExpLoc.Checked)
            {
                VATTypeLoad(true);
                ChkExpLoc.Text = "Export";


                btnExport.Visible = true;
                btn20.Visible = true;


                btnLCNo.Visible = true;
                label39.Visible = true;
                label13.Visible = true;
                cmbCurrency.Visible = true;
                txtBDTRate.Visible = true;
                btnCurrencyEdit.Visible = true;

                
              
                

            }
            else
            {
                //txtNBRPrice.ReadOnly = true;
                //txtCategoryName.Text = "Finish";

                ChkExpLoc.Text = "Local";
                btnExport.Visible = false;
                VATTypeLoad(false);

                //cmbVATType.Items.Clear();
                //cmbVATType.Items.Add("VAT");
                //cmbVATType.Items.Add("NonVAT");
                //cmbVATType.Items.Add("MRPRate");
                //cmbVATType.Items.Add("FixedVAT");
                //cmbVATType.Items.Add("OtherRate");
                //cmbVATType.Items.Add("Retail");
                //cmbVATType.SelectedIndex = 0;
                btn20.Visible = false;


                btnLCNo.Visible = false;
                label39.Visible = false;
                label13.Visible = false;
                cmbCurrency.Visible = false;
                txtBDTRate.Visible = false;
                btnCurrencyEdit.Visible = false;

              
               
                #region  Currency

                cmbCurrency.Items.Clear();
                cmbCurrency.Items.Insert(0, "BDT");
                cmbCurrency.SelectedIndex = 0;

                #endregion currency


            }
        }

        private void btnLoadCDNAmount_Click(object sender, EventArgs e)
        {
            try
            {

                LoadCDNAmount();


            }
            #region
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnLoadCDNAmount_Click", exMessage);
            }
            #endregion
        }


        private void LoadCDNAmount()
        {
            decimal DeductionAmount = 0;

            decimal CDNVATAmount = 0;
            decimal CDNSDAmount = 0;
            decimal CDNSubtotal = 0;


            decimal LineSubtotal = 0;
            decimal LineSDAmount = 0;
            decimal LineVATAmount = 0;

            decimal LineVATRate = 0;
            decimal LineSDRate = 0;


            decimal LineGrandtotal = 0;
            decimal LineDeductionAmount = 0;
            decimal LineDiscountedGrandTotal = 0;
            decimal LineSDablePrice = 0;

            decimal Grandtotal = 0;


            decimal TotalAmount = 0;
            //////decimal DeductionAmount = 0;
            decimal AmountWithVAT = 0;
            decimal TotalVATAmount = 0;
            decimal TotalSDAmount = 0;
            decimal TotalTaxAmount = 0;


            txtPTotalAmount.Text = "0";
            txtPDeductionAmount.Text = "0";
            txtPAmountWithVAT.Text = "0";
            txtPTotalVATAmount.Text = "0";
            txtPTotalSDAmount.Text = "0";
            txtPTotalTaxAmount.Text = "0";



            try
            {
                if (string.IsNullOrWhiteSpace(txtDeductionAmount.Text) || Convert.ToDecimal(txtDeductionAmount.Text) <= 0)
                {
                    //throw new ArgumentNullException("LoadCDNAmount", "Please Input Correct Deduction Amount!");
                }

                DeductionAmount = Convert.ToDecimal(txtDeductionAmount.Text);
                Grandtotal = Convert.ToDecimal(txtTotalAmount.Text);

                foreach (DataGridViewRow dgvr in dgvSale.Rows)
                {
                    ////if (DeductionAmount == 0)
                    ////{
                    ////    dgvr.Cells["CDNVATAmount"].Value = dgvr.Cells["VATAmount"].Value;
                    ////    dgvr.Cells["CDNSDAmount"].Value = dgvr.Cells["SDAmount"].Value;
                    ////    dgvr.Cells["CDNSubtotal"].Value = dgvr.Cells["SubTotal"].Value;
                    ////}
                    ////else
                    {
                        CDNVATAmount = 0;
                        CDNSDAmount = 0;
                        CDNSubtotal = 0;


                        LineSubtotal = 0;
                        LineSDAmount = 0;
                        LineVATAmount = 0;
                        LineGrandtotal = 0;
                        LineDeductionAmount = 0;
                        LineDiscountedGrandTotal = 0;
                        LineSDablePrice = 0;
                        LineVATRate = 0;
                        LineSDRate = 0;

                        LineSubtotal = Convert.ToDecimal(dgvr.Cells["SubTotal"].Value);
                        LineSDAmount = Convert.ToDecimal(dgvr.Cells["SDAmount"].Value);
                        LineVATAmount = Convert.ToDecimal(dgvr.Cells["VATAmount"].Value);

                        LineVATRate = Convert.ToDecimal(dgvr.Cells["VATRate"].Value);
                        LineSDRate = Convert.ToDecimal(dgvr.Cells["SD"].Value);

                        LineGrandtotal = LineSubtotal + LineSDAmount + LineVATAmount;
                        if (LineGrandtotal==0)
                        {
                            LineDeductionAmount = 0;
                        }
                        else
                        {
                            LineDeductionAmount = (DeductionAmount * LineGrandtotal) / Grandtotal;

                        }

                        LineDiscountedGrandTotal = LineGrandtotal - LineDeductionAmount;

                        CDNVATAmount = (LineDiscountedGrandTotal * LineVATRate) / (100 + LineVATRate);
                        LineSDablePrice = LineDiscountedGrandTotal - CDNVATAmount;

                        CDNSDAmount = (LineSDablePrice * LineSDRate) / (100 + LineSDRate);
                        CDNSubtotal = LineDiscountedGrandTotal - (CDNVATAmount + CDNSDAmount);


                        CDNVATAmount = Convert.ToDecimal(Program.FormatingNumeric(CDNVATAmount.ToString(), 4));
                        CDNSDAmount = Convert.ToDecimal(Program.FormatingNumeric(CDNSDAmount.ToString(), 4));
                        CDNSubtotal = Convert.ToDecimal(Program.FormatingNumeric(CDNSubtotal.ToString(), 4));

                        dgvr.Cells["CDNVATAmount"].Value = CDNVATAmount.ToString();
                        dgvr.Cells["CDNSDAmount"].Value = CDNSDAmount.ToString();
                        dgvr.Cells["CDNSubtotal"].Value = CDNSubtotal.ToString();



                        TotalAmount = TotalAmount + LineSubtotal;
                        TotalVATAmount = TotalVATAmount + CDNVATAmount;
                        TotalSDAmount = TotalSDAmount + CDNSDAmount;
                    }

                    AmountWithVAT = TotalAmount - DeductionAmount;
                    TotalTaxAmount = TotalVATAmount + TotalSDAmount;

                    DeductionAmount = Convert.ToDecimal(Program.FormatingNumeric(DeductionAmount.ToString(), 4));
                    AmountWithVAT = Convert.ToDecimal(Program.FormatingNumeric(AmountWithVAT.ToString(), 4));

                    txtPTotalAmount.Text = TotalAmount.ToString();
                    txtPDeductionAmount.Text = DeductionAmount.ToString();
                    txtPAmountWithVAT.Text = AmountWithVAT.ToString();
                    txtPTotalVATAmount.Text = TotalVATAmount.ToString();
                    txtPTotalSDAmount.Text = TotalSDAmount.ToString();
                    txtPTotalTaxAmount.Text = TotalTaxAmount.ToString();

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
                FileLogger.Log(this.Name, "LoadCDNAmount", exMessage);
            }
            #endregion

        }

        private void txtDeductionAmount_Leave(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Methods 19

        private void btnCDNInformation_Click(object sender, EventArgs e)
        {
            //////grbCDNInformation.Visible = !(grbCDNInformation.Visible);

            if (grbCDNInformation.Visible)
            {
                grbCDNInformation.Visible = false;
                grbCDNInformation.Height = 300;
                grbCDNInformation.Width = 300;
            }
            else
            {
                grbCDNInformation.Visible = true;
                grbCDNInformation.Height = 300;
                grbCDNInformation.Width = 300;
                grbCDNInformation.TabIndex = 1;

                this.grbCDNInformation.Location = new Point(400, 40);

            }
        }

        private void txtProductDescription_DoubleClick(object sender, EventArgs e)
        {
            ProductNamesLoad();
        }
        private void ProductNamesLoad()
        {
            try
            {

                if (string.IsNullOrEmpty(ItemNoD) || ItemNoD == "0")
                {
                    MessageBox.Show("Please select the product  first");
                    return;
                }
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select  ItemNo,ISNULL(ProductName,'')ProductName
                                 from ProductDetails 
                                 where 1=1 ";
                SqlText += @"  and ItemNo='" + ItemNoD + "'  ";

                string SQLTextRecordCount = @" select count(ProductName)RecordNo from ProductDetails";
                string[] shortColumnName = { "ItemNo", "ProductName" };
                string tableName = "";

                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtProductDescription.Text = "";
                    txtProductDescription.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[0];
                    txtProductDescription.Focus();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductNamesLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //DataGridViewRow selectedRow = new DataGridViewRow();
            //selectedRow = FormCustomerAddressFinder.SelectOne(vCustomerID);
            //if (selectedRow != null && selectedRow.Index >= 0)
            //{
            //    txtDeliveryAddress1.Text = "";
            //    txtDeliveryAddress1.Text = selectedRow.Cells["CustomerAddress"].Value.ToString();//ProductInfo[0];
            //}
            //txtDeliveryAddress1.Focus();
        }

        private void txtProductDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductNamesLoad();
            }

        }

        private void chkIsFixedVAT_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkIsFixedVAT.Checked)
            //{
            //    txtFixedVATAmount.Visible = true;
            //    txtVATRate.Visible = false;
            //}
            //else
            //{
            //    txtFixedVATAmount.Visible = false;
            //    txtVATRate.Visible = true;
            //}
        }

        private void grbCDNInformation_Enter(object sender, EventArgs e)
        {

        }

        private void txtFixedVATAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtFixedVATAmount, "FixedVATAmount");
            txtFixedVATAmount.Text = Program.ParseDecimalObject(txtFixedVATAmount.Text.Trim()).ToString();
        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
            txtQuantityInHand.Text = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
        }

        private void txtTenderStock_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTenderStock, "TenderStock");
            txtTenderStock.Text = Program.ParseDecimalObject(txtTenderStock.Text.Trim()).ToString();
        }

        private void txtTotalSubTotal_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalSubTotal, "TotalSubTotal");
            txtTotalSubTotal.Text = Program.ParseDecimalObject(txtTotalSubTotal.Text.Trim()).ToString();
        }

        private void txtTotalSDAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalSDAmount, "TotalSDAmount");
            txtTotalSDAmount.Text = Program.ParseDecimalObject(txtTotalSDAmount.Text.Trim()).ToString();
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalAmount, "TotalAmount");
            txtTotalAmount.Text = Program.ParseDecimalObject(txtTotalAmount.Text.Trim()).ToString();
        }

        private void txtVDSAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtVDSAmount, "VDSAmount");
            txtVDSAmount.Text = Program.ParseDecimalObject(txtVDSAmount.Text.Trim()).ToString();
        }

        private void txtEXPFormNo_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtQty2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvSale_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.ColumnIndex == 8)
                {

                    string SaleQuantity = dgvSale["SaleQuantity", dgvSale.CurrentRow.Index].Value.ToString();

                    string pQuantity = dgvSale["PromotionalQuantity", dgvSale.CurrentRow.Index].Value.ToString();

                    pQuantity = pQuantity.Substring(0,
                        pQuantity.IndexOf('.') > 0 ? pQuantity.IndexOf('.') : pQuantity.Length);

                    SaleQuantity = SaleQuantity.Substring(0,
                        SaleQuantity.IndexOf('.') > 0 ? SaleQuantity.IndexOf('.') : SaleQuantity.Length);


                    dgvSale["Quantity", dgvSale.CurrentRow.Index].Value =
                        Convert.ToInt32(SaleQuantity) +
                        Convert.ToInt32(pQuantity);

                    dgvSale["UOMQty", dgvSale.CurrentRow.Index].Value =
                        Convert.ToDecimal(dgvSale["Quantity", dgvSale.CurrentRow.Index].Value) * Convert.ToDecimal(dgvSale["UOMc", dgvSale.CurrentRow.Index].Value);

                    Rowcalculate();

                    LoadCDNAmount();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 20

        private void cmbBOMReferenceNo_Leave(object sender, EventArgs e)
        {
            BOMReference(true);
        }


        private DataTable LoadBOM_Dt()
        {
            DataTable dt = new DataTable();
            if (CustomerWiseBOM)
            {
                dt = new ProductDAL().GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                                         dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID);

            }
            else
            {
                dt = new ProductDAL().GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                              dtpInvoiceDate.Value.ToString("yyyy-MMM-dd"), null, null);

            }

            return dt;

        }


        private void BOMReference(bool resetBOMId = false)
        {
            try
            {
                DataTable dt = new DataTable();

                dt = LoadBOM_Dt();


                if (resetBOMId)
                {
                    txtBOMId.Text = "";
                }

                SetBOM(dt);


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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbReferenceNo_Leave", exMessage);
            }
            #endregion Catch


        }



        private void SetBOM(DataTable dt)
        {
           
        }

        private void bgwDetailsSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            ISale saleDal = GetObject();
            SaleDetailResult = saleDal.SearchSaleDetailTemp(ImportId, Program.CurrentUserID, connVM);
        }

        private void bgwDetailsSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                FileLogger.Log(this.Name, "Details Load Time before Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));

                dgvSale.Rows.Clear();
                int j = 0;

                FileLogger.Log(this.Name, "Foreach", (SaleDetailResult == null).ToString());


                foreach (DataRow item in SaleDetailResult.Rows)
                {

                    FileLogger.Log(this.Name, "Foreach", SaleDetailResult.Rows.Count.ToString());

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);
                    dgvSale.Rows[j].Cells["CDNVATAmount"].Value = item["CDNVATAmount"].ToString();
                    dgvSale.Rows[j].Cells["CDNSDAmount"].Value = item["CDNSDAmount"].ToString();
                    dgvSale.Rows[j].Cells["CDNSubtotal"].Value = item["CDNSubtotal"].ToString();


                    dgvSale.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
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
                    dgvSale.Rows[j].Cells["ValueOnly"].Value = item["ValueOnly"].ToString();
                    dgvSale.Rows[j].Cells["DiscountedNBRPrice"].Value = item["DiscountedNBRPrice"].ToString();
                    dgvSale.Rows[j].Cells["DollerValue"].Value = item["DollerValue"].ToString();

                    dgvSale.Rows[j].Cells["BDTValue"].Value = item["CurrencyValue"].ToString();

                    dgvSale.Rows[j].Cells["VDSAmount"].Value = item["VDSAmount"].ToString();
                    dgvSale.Rows[j].Cells["TotalValue"].Value = item["TotalValue"].ToString();
                    dgvSale.Rows[j].Cells["WareHouseRent"].Value = item["WareHouseRent"].ToString();
                    dgvSale.Rows[j].Cells["WareHouseVAT"].Value = item["WareHouseVAT"].ToString();
                    dgvSale.Rows[j].Cells["ATVRate"].Value = item["ATVRate"].ToString();
                    dgvSale.Rows[j].Cells["ATVablePrice"].Value = item["ATVablePrice"].ToString();
                    dgvSale.Rows[j].Cells["ATVAmount"].Value = item["ATVAmount"].ToString();
                    dgvSale.Rows[j].Cells["IsCommercialImporter"].Value = item["IsCommercialImporter"].ToString();
                    dgvSale.Rows[j].Cells["TradeVATRate"].Value = item["TradeVATRate"].ToString();
                    dgvSale.Rows[j].Cells["TradeVATAmount"].Value = item["TradeVATAmount"].ToString();
                    dgvSale.Rows[j].Cells["TradeVATableValue"].Value = item["TradeVATableValue"].ToString();

                    dgvSale.Rows[j].Cells["HPSRate"].Value = item["HPSRate"].ToString();
                    dgvSale.Rows[j].Cells["HPSAmount"].Value = item["HPSAmount"].ToString();
                    dgvSale.Rows[j].Cells["SourcePaidVATAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["SourcePaidQuantity"].Value = "0";



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
                    dgvSale.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();
                    dgvSale.Rows[j].Cells["IsFixedVAT"].Value = item["IsFixedVAT"].ToString();
                    dgvSale.Rows[j].Cells["FixedVATAmount"].Value = item["FixedVATAmount"].ToString();

                    
                    j = j + 1;
                } // End For
                //btnSave.Text = "&Save";


               
                    Rowcalculate();


                ExportSearch();
                //SetupVATStatus();
                IsUpdate = true;
                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());


                LoadCDNAmount();

                FileLogger.Log(this.Name, "Details Load Time after Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));
                dgvSale.PerformLayout();
                // End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage + "\n" + ex.StackTrace);
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

        private void dgvSale_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            if (e.ColumnIndex == 1)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.Delete.Width;
                var h = Properties.Resources.Delete.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.Delete, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void rbtnExportTradingTender_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtHPSRate_Leave(object sender, EventArgs e)
        {
            txtHPSRate.Text = Program.ParseDecimalObject(txtHPSRate.Text.Trim()).ToString();
        }

        private void txtWeight_Leave(object sender, EventArgs e)
        {
            //txtWeight.Text = Program.ParseDecimalObject(txtWeight.Text.Trim()).ToString();
        }

        private void txtTotalVATAmount_Leave(object sender, EventArgs e)
        {
            txtTotalVATAmount.Text = Program.ParseDecimalObject(txtTotalVATAmount.Text.Trim()).ToString();
        }

        private void txtTotalQuantity_Leave(object sender, EventArgs e)
        {
            txtTotalQuantity.Text = Program.ParseDecimalObject(txtTotalQuantity.Text.Trim()).ToString();
        }

        private void txtHPSTotal_Leave(object sender, EventArgs e)
        {
            txtHPSTotal.Text = Program.ParseDecimalObject(txtHPSTotal.Text.Trim()).ToString();
        }

        private void dgvSale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F2))
            {
                if (rbtnDN.Checked || rbtnCN.Checked)
                {
                    try
                    {
                        DataGridViewSelectedRowCollection selectedRows = dgvSale.SelectedRows;
                        if (selectedRows != null && selectedRows.Count > 0)
                        {
                            int selectedrowindex = dgvSale.SelectedCells[0].RowIndex;
                            DataGridViewRow selectedRow = dgvSale.Rows[selectedrowindex];

                            string result = FormDN_CNAdjustment.SelectOne(selectedRow);

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

        private void backgroundNew_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ISale saleDal = GetObject();
                SaleDetailResult = saleDal.SearchSaleDetailDTNew(SaleDetailData, connVM);
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                FileLogger.Log(this.Name, "Details Load Time before Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));

                dgvSale.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SaleDetailResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);


                    dgvSale.Rows[j].Cells["SourcePaidQuantity"].Value = Program.ParseDecimalObject(item["SourcePaidQuantity"].ToString());
                    dgvSale.Rows[j].Cells["SourcePaidVATAmount"].Value = Program.ParseDecimalObject(item["SourcePaidVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["NBRPriceInclusiveVAT"].Value = Program.ParseDecimalObject(item["NBRPriceInclusiveVAT"].ToString());



                    dgvSale.Rows[j].Cells["CDNVATAmount"].Value = Program.ParseDecimalObject(item["CDNVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["CDNSDAmount"].Value = Program.ParseDecimalObject(item["CDNSDAmount"].ToString());
                    dgvSale.Rows[j].Cells["CDNSubtotal"].Value = Program.ParseDecimalObject(item["CDNSubtotal"].ToString());

                    dgvSale.Rows[j].Cells["BOMReferenceNo"].Value = item["BOMReferenceNo"].ToString();
                    dgvSale.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvSale.Rows[j].Cells["LineNo"].Value = item["InvoiceLineNo"].ToString();
                    // SaleDetailFields[1].ToString();
                    dgvSale.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    // SaleDetailFields[2].ToString();
                    dgvSale.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvSale.Rows[j].Cells["SaleQuantity"].Value = Program.ParseDecimalObject(item["SaleQuantity"].ToString());
                    dgvSale.Rows[j].Cells["PromotionalQuantity"].Value = Program.ParseDecimalObject(item["PromotionalQuantity"].ToString());

                    //Convert.ToDecimal(SaleDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["SalesPrice"].ToString());
                    //Convert.ToDecimal(SaleDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["NBRPrice"].ToString());
                    //Convert.ToDecimal(SaleDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["UOM"].Value = item["UOM"].ToString(); // SaleDetailFields[6].ToString();
                    dgvSale.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    //Convert.ToDecimal(SaleDetailFields[7].ToString()).ToString();//"0.00");
                    dgvSale.Rows[j].Cells["VATAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["SubTotal"].Value = "0";
                    dgvSale.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    // SaleDetailFields[10].ToString();
                    dgvSale.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    // SaleDetailFields[11].ToString();
                    dgvSale.Rows[j].Cells["Status"].Value = "Old";
                    dgvSale.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    //Convert.ToDecimal(SaleDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvSale.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    // Convert.ToDecimal(SaleDetailFields[13].ToString()).ToString();//"0.00");
                    dgvSale.Rows[j].Cells["SDAmount"].Value = "0";
                    dgvSale.Rows[j].Cells["Change"].Value = 0;
                    dgvSale.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    // SaleDetailFields[17].ToString();
                    dgvSale.Rows[j].Cells["NonStock"].Value = item["NonStock"].ToString();
                    // SaleDetailFields[18].ToString();
                    dgvSale.Rows[j].Cells["TradingMarkUp"].Value = Program.ParseDecimalObject(item["tradingMarkup"].ToString());
                    // SaleDetailFields[19].ToString();

                    dgvSale.Rows[j].Cells["Type"].Value = item["Type"].ToString(); // SaleDetailFields[20].ToString();
                    dgvSale.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    // SaleDetailFields[21].ToString();
                    dgvSale.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                    // Convert.ToDecimal(SaleDetailFields[22].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString(); // SaleDetailFields[23].ToString();
                    dgvSale.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    //Convert.ToDecimal(SaleDetailFields[24].ToString()).ToString();//"0,0.0000");
                    dgvSale.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());
                    // Convert.ToDecimal(SaleDetailFields[25].ToString()).ToString();//"0,0.00");
                    dgvSale.Rows[j].Cells["DiscountAmount"].Value = Program.ParseDecimalObject(item["DiscountAmount"].ToString());
                    dgvSale.Rows[j].Cells["ValueOnly"].Value = item["ValueOnly"].ToString();
                    dgvSale.Rows[j].Cells["DiscountedNBRPrice"].Value = Program.ParseDecimalObject(item["DiscountedNBRPrice"].ToString());
                    dgvSale.Rows[j].Cells["DollerValue"].Value = Program.ParseDecimalObject(item["DollerValue"].ToString());

                    dgvSale.Rows[j].Cells["BDTValue"].Value = Program.ParseDecimalObject(item["CurrencyValue"].ToString());

                    dgvSale.Rows[j].Cells["VDSAmount"].Value = Program.ParseDecimalObject(item["VDSAmount"].ToString());
                    dgvSale.Rows[j].Cells["TotalValue"].Value = Program.ParseDecimalObject(item["TotalValue"].ToString());
                    dgvSale.Rows[j].Cells["WareHouseRent"].Value = Program.ParseDecimalObject(item["WareHouseRent"].ToString());
                    dgvSale.Rows[j].Cells["WareHouseVAT"].Value = Program.ParseDecimalObject(item["WareHouseVAT"].ToString());
                    dgvSale.Rows[j].Cells["ATVRate"].Value = Program.ParseDecimalObject(item["ATVRate"].ToString());
                    dgvSale.Rows[j].Cells["ATVablePrice"].Value = Program.ParseDecimalObject(item["ATVablePrice"].ToString());
                    dgvSale.Rows[j].Cells["ATVAmount"].Value = Program.ParseDecimalObject(item["ATVAmount"].ToString());
                    dgvSale.Rows[j].Cells["IsCommercialImporter"].Value = item["IsCommercialImporter"].ToString();
                    dgvSale.Rows[j].Cells["TradeVATRate"].Value = Program.ParseDecimalObject(item["TradeVATRate"].ToString());
                    dgvSale.Rows[j].Cells["TradeVATAmount"].Value = Program.ParseDecimalObject(item["TradeVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["TradeVATableValue"].Value = item["TradeVATableValue"].ToString();

                    dgvSale.Rows[j].Cells["HPSRate"].Value = Program.ParseDecimalObject(item["HPSRate"].ToString());
                    dgvSale.Rows[j].Cells["HPSAmount"].Value = Program.ParseDecimalObject(item["HPSAmount"].ToString());



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
                    dgvSale.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();
                    dgvSale.Rows[j].Cells["IsFixedVAT"].Value = item["IsFixedVAT"].ToString();
                    dgvSale.Rows[j].Cells["FixedVATAmount"].Value = Program.ParseDecimalObject(item["FixedVATAmount"].ToString());

                    #region New Field For DN/CN

                    dgvSale.Rows[j].Cells["PreviousSalesInvoiceNo"].Value = SearchPreviousForCNDN == true ? txtOldID.Text.Trim() : item["PreviousSalesInvoiceNo"].ToString();
                    dgvSale.Rows[j].Cells["PreviousInvoiceDateTime"].Value = SearchPreviousForCNDN == true ? item["PreviousInvoiceDateTime"].ToString() : dtpInvoiceDate.Value.ToString();
                    dgvSale.Rows[j].Cells["PreviousNBRPrice"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["NBRPrice"].ToString()) : Program.ParseDecimalObject(item["PreviousNBRPrice"].ToString());
                    dgvSale.Rows[j].Cells["PreviousQuantity"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["Quantity"].ToString()) : Program.ParseDecimalObject(item["PreviousQuantity"].ToString());
                    dgvSale.Rows[j].Cells["PreviousUOM"].Value = SearchPreviousForCNDN == true ? item["UOM"].ToString() : item["PreviousUOM"].ToString();
                    dgvSale.Rows[j].Cells["PreviousSubTotal"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["SubTotal"].ToString()) : Program.ParseDecimalObject(item["PreviousSubTotal"].ToString());
                    dgvSale.Rows[j].Cells["PreviousVATAmount"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["VATAmount"].ToString()) : Program.ParseDecimalObject(item["PreviousVATAmount"].ToString());
                    dgvSale.Rows[j].Cells["PreviousVATRate"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["VATRate"].ToString()) : Program.ParseDecimalObject(item["PreviousVATRate"].ToString());
                    dgvSale.Rows[j].Cells["PreviousSD"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["SD"].ToString()) : Program.ParseDecimalObject(item["PreviousSD"].ToString());
                    dgvSale.Rows[j].Cells["PreviousSDAmount"].Value = SearchPreviousForCNDN == true ? Program.ParseDecimalObject(item["SDAmount"].ToString()) : Program.ParseDecimalObject(item["PreviousSDAmount"].ToString());
                    dgvSale.Rows[j].Cells["ReasonOfReturn"].Value = item["ReasonOfReturn"].ToString();


                    #endregion

                   

                    #region currency convertion date change for export

                  
                        dgvSale.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                    #endregion currency convertion date change for export
                    j = j + 1;
                } // End For
                //btnSave.Text = "&Save";


              
                    Rowcalculate();


                ExportSearch();
                //SetupVATStatus();
                IsUpdate = true;
                PreVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());


                LoadCDNAmount();
                FileLogger.Log(this.Name, "Details Load Time After Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));


                // End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerChallanSearch_DoWork", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                //this.Enabled = true;
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
                btnOldID.Enabled = true;

            }
        }

        private void txtCustomer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (IsCustomerSpecialRate.ToLower() == "y")
                {
                    MessageBox.Show(MessageVM.CustomerMsgSpecialRateCustomer, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtCustomer_Leave", exMessage);
            }
            #endregion Catch
        }

        #endregion

        #region Sale Navigation

        private void txtSalesInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SaleNavigation("Current");
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            SaleNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SaleNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SaleNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            SaleNavigation("Last");
        }

        private void SaleNavigation(string ButtonName)
        {
            try
            {
                SaleDAL _SaleDAL = new SaleDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(string.IsNullOrWhiteSpace(txtId.Text) ? "0" : txtId.Text);
                vm.InvoiceNo = txtSalesInvoiceNo.Text;



                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _SaleDAL.Sale_Navigation(vm);

                if (!string.IsNullOrWhiteSpace(vm.InvoiceNo))
                {
                    DataTable dtMaster = new DataTable();

                    dtMaster = _SaleDAL.SearchSalesHeaderDTNew(vm.InvoiceNo, "", connVM);
                    SaleDataLoad(dtMaster);
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
                FileLogger.Log(this.Name, "btnPrevious_Click", exMessage);
            }
            #endregion Catch

        }

        #endregion



        private void btnCurrencyEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsConvCompltd)
                {
                    SaleMasterVM masterVm = new SaleMasterVM();

                    masterVm.CurrencyRateFromBDT = Convert.ToDecimal(txtBDTRate.Text);
                    masterVm.CurrencyID = txtCurrencyId.Text.Trim();
                    masterVm.SalesInvoiceNo = txtSalesInvoiceNo.Text;

                    saleDetails = new List<SaleDetailVm>();

                    for (int i = 0; i < dgvSale.RowCount; i++)
                    {
                        SaleDetailVm detail = new SaleDetailVm();

                        detail.ItemNo = dgvSale.Rows[i].Cells["ItemNo"].Value.ToString();

                        detail.DollerValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["DollerValue"].Value.ToString());
                        detail.CurrencyValue = Convert.ToDecimal(dgvSale.Rows[i].Cells["BDTValue"].Value.ToString());
                        saleDetails.Add(detail);
                    }


                    SaleDAL saleDal = new SaleDAL();

                    string[] result = saleDal.SalesCurrencyValueUpdate(masterVm, saleDetails);

                    if (result[0].ToLower() == "success")
                    {
                        IsConvCompltd = true;

                        MessageBox.Show("Conversion Updated Successfully");
                    }

                }



            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnGDIC_Click(object sender, EventArgs e)
        {
            try
            {
                #region Local Variable
                string Isleader ="";
                decimal LeaderAmount = 0;
                decimal Quantity = 0;
                decimal VATRate = 0;
                decimal UnitPrice = 0;
                decimal LeaderVATAmount = 0;
                decimal NonLeaderAmount = 0;
                decimal NonLeaderVATAmount = 0;
                #endregion

                DataGridViewSelectedRowCollection selectedRows = dgvSale.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    int selectedrowindex = dgvSale.SelectedCells[0].RowIndex;
                    DataGridViewRow selectedRow = dgvSale.Rows[selectedrowindex];

                    #region Value Assign From Selected Row 
                    Quantity = Convert.ToDecimal(selectedRow.Cells["Quantity"].Value);
                    VATRate = Convert.ToDecimal(selectedRow.Cells["VATRate"].Value);
                    UnitPrice = Convert.ToDecimal(selectedRow.Cells["NBRPrice"].Value);
                    #endregion

                    string result = FormLeaderPolicy.SelectOne(selectedRow);
                    string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());

                    Isleader = PrintResult[0];

                    #region Calculation
                    if (Isleader=="Y")
                    {
                        LeaderAmount = Convert.ToDecimal(PrintResult[1]);
                        LeaderVATAmount = ((Quantity * LeaderAmount) * (VATRate / 100));
                        NonLeaderAmount = (Quantity * UnitPrice) - LeaderAmount;
                        NonLeaderVATAmount = ((Quantity * UnitPrice)-LeaderAmount) * (VATRate / 100);

                    }
                    else if (Isleader == "N")
                    {
                        LeaderAmount = Convert.ToDecimal(PrintResult[1]);
                        NonLeaderAmount = LeaderAmount;
                        NonLeaderVATAmount=((Quantity * LeaderAmount) * (VATRate / 100));
                    }
                    else
                    {
                        LeaderAmount = Convert.ToDecimal(PrintResult[1]);
                        LeaderVATAmount = 0;
                        NonLeaderAmount = 0;
                        NonLeaderVATAmount=0;
                    }
                    #endregion

                    #region Set In Grid
                    dgvSale["IsLeader", dgvSale.CurrentRow.Index].Value = Isleader;
                    dgvSale["LeaderAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(LeaderAmount);
                    dgvSale["LeaderVATAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(LeaderVATAmount);
                    dgvSale["NonLeaderAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(NonLeaderAmount);
                    dgvSale["NonLeaderVATAmount", dgvSale.CurrentRow.Index].Value = Program.ParseDecimalObject(NonLeaderVATAmount);
                    #endregion

                }


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
                FileLogger.Log(this.Name, "btnGDIC_Click", exMessage);
            }
        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {
                    txtQty1.Focus();
                }
                if (e.KeyCode.Equals(Keys.F9))
                {
                    ProductLoad();
                    txtProductName.Focus();
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtProductName_KeyDown", exMessage);
            }

            #endregion

        }

        private void txtProductName_Leave(object sender, EventArgs e)
        {

            //ComboBox source = (ComboBox)sender;

            #region Check Point

            if (!string.IsNullOrEmpty(ProductName))
            {
                if (ProductName == txtProductName.Text)
                {
                    return;
                }
            }

            #endregion

            #region Initialization

            SelectedVATRate = 0;

            txtProductCode.Text = "";

            #endregion


            try
            {

                string searchText = txtProductName.Text.Trim();

                string ProductCode = "";
                if (searchText.Contains('~'))
                {
                    ProductCode = searchText.Split('~')[1];
                }


                if (!string.IsNullOrWhiteSpace(ProductCode))
                {
                    ProductDAL _ProductDal = new ProductDAL();

                    DataTable dtProduct = new DataTable();

                    #region Product Call

                    dtProduct = _ProductDal.SelectProductDTAll(new[] { "Products.ProductCode" }, new[] { ProductCode });

                    #endregion

                    if (dtProduct != null && dtProduct.Rows.Count > 0)
                    {

                        DataRow drProduct = dtProduct.Rows[0];

                        #region HPSRate

                        OriginalHPSRate = Convert.ToDecimal(drProduct["HPSRate"].ToString());

                        //if (cmbVATType.Text == "MRPRate(SC)")
                        if (cmbVATType.SelectedValue.ToString().ToLower() == "mrprate(sc)")
                        {
                            txtHPSRate.Text = OriginalHPSRate.ToString();
                        }
                        else
                        {
                            txtHPSRate.Text = "0.00";
                        }

                        #endregion

                        #region Value Assign to Form Elements

                        #region Condtional Values

                        txtProductName.Text = drProduct["ProductName"].ToString();// products.ProductName;
                        txtProductDescription.Text = drProduct["ProductDescription"].ToString();// products.ProductName;
                        if (string.IsNullOrWhiteSpace(txtProductDescription.Text.Trim())
                            || txtProductDescription.Text.Trim() == "-"
                            || txtProductDescription.Text.Trim() == "NA"
                            || txtProductDescription.Text.Trim() == "N/A"
                            )
                        {
                            txtProductDescription.Text = txtProductName.Text.Trim();
                        }

                        #endregion

                        txtProductCode.Text = drProduct["ItemNo"].ToString();
                        vItemNo = drProduct["ItemNo"].ToString();
                        ItemNoD = vItemNo;
                        txtVATRate.Text = drProduct["VATRate"].ToString();
                        txtSD.Text = drProduct["SD"].ToString();
                        if (transactionType != "Export")
                        {
                            cmbVATType.SelectedValue = VATTypeCal(drProduct["VATRate"].ToString()).ToLower();

                        }

                        #region Price Call

                        if (rbtnOther.Checked == true)
                        {
                            var vNBRPriceCall = commonDal.settingsDesktop("Sale", "NBRPriceCall");
                            if (vNBRPriceCall.ToLower() == "y")
                            {
                                NBRPriceCall();
                            }
                        }

                        #endregion

                        txtUOM.Text = drProduct["UOM"].ToString();// products.UOM;
                        cmbUom.Text = drProduct["UOM"].ToString();// products.UOM;
                        txtHSCode.Text = drProduct["HSCodeNo"].ToString();// products.HSCodeNo;
                        chkIsFixedVAT.Checked = false;
                        txtFixedVATAmount.Text = "0";
                        LVat.Text = "VAT(%)";
                        LSD.Text = "SD(%)";
                        txtFixedVATAmount.Text = "0";
                        txtFixedSDAmount.Text = "0";
                        chkIsFixedOtherSD.Checked = (drProduct["IsFixedSD"].ToString()) == "Y" ? true : false;
                        chkIsFixedOtherVAT.Checked = (drProduct["IsFixedVAT"].ToString()) == "Y" ? true : false;


                        if (chkIsFixedOtherSD.Checked)
                        {
                            LSD.Text = "SD(F)";

                        }

                        #endregion

                        #region Conditional Values

                        if (chkIsFixedOtherVAT.Checked)
                        {
                            txtFixedVATAmount.Visible = true;
                            cmbVATType.SelectedValue = "fixedvat";
                            LVat.Text = "VAT(F)";
                            chkIsFixedVAT.Checked = true;
                            txtFixedVATAmount.Text = Program.ParseDecimal(drProduct["FixedVATAmount"].ToString());
                            txtVATRate.Text = Program.ParseDecimal(drProduct["FixedVATAmount"].ToString());

                            if (cmbVATType.SelectedValue == "export")
                                txtFixedVATAmount.Text = "0";
                            else if (cmbVATType.SelectedValue == "deemexport")
                                txtFixedVATAmount.Text = "0";
                        }
                        #endregion

                        #region Stock / Avg Price

                        //BugsBD
                        string ProductCodeData = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());

                        
                        //txtQuantityInHand.Text = _ProductDal.AvgPriceNew(txtProductCode.Text.Trim(), dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                        txtQuantityInHand.Text = _ProductDal.AvgPriceNew(ProductCodeData, dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") +
                                                                         DateTime.Now.ToString(" HH:mm:00"), null, null, true, true, true, false, null,Program.CurrentUserID).Rows[0]["Quantity"].ToString();

                        #endregion

                        txttradingMarkup.Text = drProduct["TradingMarkUp"].ToString();
                        txtPCode.Text = drProduct["ProductCode"].ToString();
                        chkNonStock.Checked = drProduct["ItemNo"].ToString() == "Y" ? true : false;

                        #region UOM Function

                        Uoms();

                        #endregion

                        #region TransactionType IssueWastage
                        if (transactionType == "SaleWastage")
                        {
                            //////txtUnitCost.Text = "0";
                            txtQuantityInHand.Text = "0";

                            DataSet ds = new DataSet();
                            ds = new ReportDSDAL().Wastage(vItemNo, "", "", "Y", "Y", "1900-Jan-01", DateTime.MaxValue.ToString("2100-Dec-31"), Program.BranchId, null);



                            if (ds != null && ds.Tables.Count > 0)
                            {
                                string WastageBalance = "0";
                                WastageBalance = ds.Tables[0].Rows[0]["WastageBalance"].ToString();

                                txtQuantityInHand.Text = WastageBalance;
                            }


                        }

                        #endregion

                    }

                }

                #region Export/DeemExport

                ReceiveDAL _sDal = new ReceiveDAL();
                DataTable dt = new DataTable();
                if (txtSerialNo.Text.Trim().Length >= 2)
                {
                    dt = _sDal.SearchByReferenceNo(txtSerialNo.Text.Trim(), txtProductCode.Text.Trim(), null, transactionType);
                    if (dt.Rows.Count > 0)
                    {
                        txtQty1.Text = Program.ParseDecimal(dt.Rows[0]["Quantity"].ToString());
                        txtNBRPrice.Text = Program.ParseDecimal(dt.Rows[0]["CostPrice"].ToString());

                    }
                }

                if (cmbVATType.SelectedValue == "export"
                    || cmbVATType.SelectedValue == "deemexport"
                    )
                {
                    txtVATRate.Text = "0";
                    txtSD.Text = "0";
                    txtHPSRate.Text = "0";
                    txtFixedVATAmount.Text = "0";
                    txtFixedSDAmount.Text = "0";
                }

                //txtQty1.Focus();
                if (VATTypeVATAutoChange.ToLower() == "y")
                {
                    SelectedVATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                }
                #endregion

                #region CheckCustomerIsExempted

                if (IsCustomerExempted.ToLower() == "y")
                {
                    txtSD.Text = "0";
                    txtVATRate.Text = "0";

                    cmbVATType.SelectedValue = VATTypeCal("0").ToLower();

                }

                #endregion

                vItemNo = "";
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
                FileLogger.Log(this.Name, "txtProductName_Leave", exMessage);
            }

            #endregion
        }

    }
}