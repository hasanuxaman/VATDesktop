// ---------form //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
////
// Change 01
using SymphonySofttech.Utilities;
using VATClient.ReportPages;
using VATClient.ReportPreview;
using VATClient.ModelDTO;
using VATServer.Library;
using VATViewModel.DTOs;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using System.Text;
using System.Web.UI.WebControls;
using VATServer.License;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;
using SymphonySofttech.Reports;
using VATClient.Integration.DBH;
using VATClient.Integration.Decathlon;

namespace VATClient
{
    public partial class FormPurchase : Form
    {
        public FormPurchase()
        {
            InitializeComponent();



            connVM = Program.OrdinaryLoad();

            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #region DTO Models

        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        List<ProductCmbDTO> productCmb = new List<ProductCmbDTO>();

        List<VendorMiniDTO> vendorsMini = new List<VendorMiniDTO>();
        List<DutyDTO> duties = new List<DutyDTO>();
        List<UomDTO> UOMs = new List<UomDTO>();

        PurchaseMasterVM purchaseMaster = new PurchaseMasterVM();
        List<PurchaseDutiesVM> purchaseDuties = new List<PurchaseDutiesVM>();
        List<PurchaseDetailVM> purchaseDetails = new List<PurchaseDetailVM>();
        List<TrackingVM> purchaseTrackings = new List<TrackingVM>();

        #endregion DTO

        #region variable

        CommonDAL commonDal = new CommonDAL();

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public DataSet DsFromMasterImport;
        public string tTypeFromMasterImport;
        private string realTimeEntry;

        private DataTable dtPurchaseM = new DataTable();
        private DataTable dtPurchaseD = new DataTable();
        private DataTable dtPurchaseI = new DataTable();
        private DataTable dtPurchaseT = new DataTable();

        private bool boolsearch = false;
        private bool dutyCalculate = true;
        private string transactionType = string.Empty;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;


        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string HSCodeNoParam = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";

        private string VendorIDParam = "";
        private string VendorNameParam = "";
        private string VendorGroupIDParam = "";
        private string VendorGroupNameParam = "";
        private string TINNoParam = "";
        private string VATRegistrationNoParam = "";
        private string ActiveStatusVendorParam = "";

        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";

        private string DutyTypeParam = "";
        private string ActiveStatusDutyParam = "";
        private string ExpireDateTracking = "";

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DataTable ProductResultDs;
        private DataTable PurchaseDetailResult;
        private DataTable PurchaseDutyResult;


        private DataTable VendorGroupResult;
        private DataTable uomResult;

        private string UomData;
        private string ProductData;
        private string VendorData;

        private string ProductName;
        private string ProductCode;
        private bool ChangeData = false;
        private string CategoryId { get; set; }
        private string NextID = string.Empty;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string dutyid { get; set; }
        private string[] items;
        private string[] arrayOld;

        private string[] ResultLines;
        private string[] ResultFields;
        private string result;


        private string PurchaseDetailData;

        private bool IsTotalPrice;
        private bool FixedVAT = true;
        private bool FixedCnF = true;
        private bool FixedInsurance = true;
        private bool CalculativeAV;
        private bool FixedCD = true;
        private bool FixedRD = true;
        private bool FixedTVB = true;
        private bool FixedTVA = true;
        private bool FixedAT = true;
        private bool FixedOthers = true;
        private bool FixedSD = true;
        private bool FixedAIT = true;
        private bool FixedVATImp;
        private int PurchasePlaceQty;
        private int PurchasePlaceAmt;
        private decimal RateChangePromote;

        private bool Add = false;
        private bool Edit = false;
        private string ReturnTransType = string.Empty;
        private string DefaultVATTypeLocal, DefaultVATTypeImport, VATTypeVATAutoChange = string.Empty;

        private string vMultipleItemInsert;

        private int SearchBranchId = 0;
        private bool PreviewOnly = false;
        private DataSet ReportResultDebitNote;
        ReportDSDAL reportDsdal = new ReportDSDAL();
        public static string vItemNo = "";
        //private ReportDocument reportDocument = new ReportDocument();
        private decimal vUnitCost = 0;
        #region Serial Track
        private string TrackingTrace = string.Empty;
        private string NoOfHeader = string.Empty;
        private string SHeading1 = string.Empty;
        private string SHeading2 = string.Empty;
        private List<string> Headings = new List<string>();
        #endregion

        #endregion variable

        #region Methods 01 / Form Load

        private void FindBomId()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            for (int i = 0; i < dgvPurchase.RowCount; i++)
            {
                string BOMId = dgvPurchase["BOMId", i].Value.ToString();
                string ItemNo = dgvPurchase["ItemNo", i].Value.ToString();
                //string VATName = dgvPurchase["VATName", i].Value.ToString();
                if (BOMId == "0")
                {
                    DataTable dt = productDal.SelectBOMRaw(ItemNo, dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        BOMId = dt.Rows[0]["BOMId"].ToString();

                    }
                    dgvPurchase["BOMId", i].Value = string.IsNullOrEmpty(BOMId) ? "0" : BOMId;
                }
            }

        }

        private void FormPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                #region Tool Tip

                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchInvoiceNo, "Existing information");
                ToolTip1.SetToolTip(this.btnVendorName, "Vendor");
                ToolTip1.SetToolTip(this.btnProductSearch, "Product");
                ToolTip1.SetToolTip(this.btnProductType, "Product Type");
                ToolTip1.SetToolTip(this.btnClose, "Close");
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");
                ToolTip1.SetToolTip(this.btnAdd, "Add Item in Details");
                ToolTip1.SetToolTip(this.btnChange, "Change Item in Details");
                ToolTip1.SetToolTip(this.HSCodePopUp, "HS Code PopUp Form");


                #endregion

                #region Reset Fields / Elements

                ClearAllFields();

                IsUpdate = false;

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
                tabPage2.Text = this.Text;

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Button Stats

                this.progressBar1.Visible = true;

                #endregion
                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Transaction Types

                transactionTypes();

                #endregion

                #region LC

                if (
                                   transactionType.ToLower() == "import"
                                   || transactionType.ToLower() == "tradingimport"
                                   || transactionType.ToLower() == "servicensimport"
                                   || transactionType.ToLower() == "serviceimport"

                                   )
                {

                    label51.Visible = true;
                }

                #endregion

                #region VAT Type Load

                VATTypeLoad();

                #endregion

                #region Purchase Return

                if (rbtnPurchaseReturn.Checked)
                {
                    IsTotalPrice = false;
                    FixedVAT = false;
                    LTP.Text = "Unit Price";
                    LVR.Text = "VAT(%)";

                    txtVendorName.Enabled = false;
                    l15.Text = "Vendor";
                    //////dtpReceiveDate.Enabled = false;
                    chkIsRebate.Enabled = false;
                    dtpRebateDate.Enabled = false;
                    txtBENumber.ReadOnly = true;
                    txtSerialNo.ReadOnly = true;
                    txtLCNumber.ReadOnly = true;
                    dtpLCDate.Enabled = false;
                    ////dtpInvoiceDate.Enabled = false;
                    button1.Visible = false;
                    cmbUom.Enabled = false;
                    //txtUnitCost.ReadOnly = true;
                    txtSD.ReadOnly = true;
                    txtLocalSDAmount.ReadOnly = true;
                    cmbVATRateInput.Enabled = false;
                    txtVATRate.ReadOnly = true;
                    txtLocalVATAmount.ReadOnly = true;
                    txtVDSRate.ReadOnly = true;
                    txtVDSAmount.ReadOnly = true;
                    chkTDS.Enabled = false;
                    cmbType.Enabled = false;
                    cmbProduct.Enabled = false;
                    txtBankGuarantee.ReadOnly = true;
                    txtComments.ReadOnly = true;
                    btnAdd.Visible = false;

                    dgvPurchase.Columns["CnF"].Visible = true;
                    dgvPurchase.Columns["Insurance"].Visible = true;
                    dgvPurchase.Columns["AV"].Visible = true;
                    dgvPurchase.Columns["CD"].Visible = true;
                    dgvPurchase.Columns["RD"].Visible = true;
                    ////dgvPurchase.Columns["TVB"].Visible = true;
                    ////dgvPurchase.Columns["TVA"].Visible = true;
                    dgvPurchase.Columns["ATV"].Visible = true;
                    dgvPurchase.Columns["Others"].Visible = true;


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

        private void VATTypeLoad()
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicVATType = new Dictionary<string, string>();
            cmbType.Items.Clear();

            dicVATType = commonDal.TypeOfPurchaseVAT();

            cmbType.DataSource = new BindingSource(dicVATType, null);
            cmbType.DisplayMember = "Key";
            cmbType.ValueMember = "Value";
            cmbType.SelectedValue = DefaultVATTypeLocal.ToLower();

        }

        private void FormMaker()
        {
            try
            {
                bool RebateWithGRN = false;

                #region Settings Data
                CommonDAL commonDal = new CommonDAL();


                string vPurchasePlaceQty, vPurchasePlaceAmt, vIsTotalPrice, vFixedVAT, vCalculativeAV, vRateChangePromotePercent
                    = string.Empty;

                #region Serial Tracking

                string vTrackingTrace, vNoOfHeader, vHeading1, vHeading2 = string.Empty;

                #endregion

                DefaultVATTypeLocal = commonDal.settingsDesktop("DefaultVATType", "Purchase", null, connVM);
                VATTypeVATAutoChange = commonDal.settingsDesktop("VATTypeVAT", "AutoChange", null, connVM);
                vPurchasePlaceQty = commonDal.settingsDesktop("Purchase", "Quantity", null, connVM);
                vPurchasePlaceAmt = commonDal.settingsDesktop("Purchase", "Amount", null, connVM);
                vIsTotalPrice = commonDal.settingsDesktop("Purchase", "TotalPrice", null, connVM);
                vFixedVAT = commonDal.settingsDesktop("Purchase", "FixedVAT", null, connVM);
                vCalculativeAV = commonDal.settingsDesktop("ImportPurchase", "CalculativeAV", null, connVM);
                ExpireDateTracking = commonDal.settingsDesktop("Purchase", "ExpireDateTracking", null, connVM);



                if (rbtnClientFGReceiveWOBOM.Checked)
                {
                    vIsTotalPrice = "N";
                }

                RebateWithGRN = commonDal.settingsDesktop("Purchase", "RebateWithGRN", null, connVM) == "Y";

                if (RebateWithGRN)
                {
                    chkIsRebate.Visible = false;
                    label39.Visible = false;
                    dtpRebateDate.Visible = false;
                }
                else
                {
                    chkIsRebate.Visible = true;
                    label39.Visible = true;
                    dtpRebateDate.Visible = true;
                }

                #region Product DropDown Width Change

                string ProductDropDownWidth = commonDal.settingsDesktop("Product", "ProductDropDownWidth", null, connVM);
                cmbProduct.DropDownWidth = Convert.ToInt32(ProductDropDownWidth);

                #endregion

                vRateChangePromotePercent = commonDal.settingsDesktop("Purchase", "RateChangePromote", null, connVM);

                vTrackingTrace = commonDal.settingsDesktop("TrackingTrace", "Tracking", null, connVM);
                vNoOfHeader = commonDal.settingsDesktop("TrackingTrace", "TrackingNo", null, connVM);
                vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", null, connVM);
                vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", null, connVM);

                realTimeEntry = commonDal.settingsDesktop("Purchase", "EntryRealTime", null, connVM);


                if (string.IsNullOrEmpty(vPurchasePlaceQty) || string.IsNullOrEmpty(vPurchasePlaceAmt)
                    || string.IsNullOrEmpty(vCalculativeAV)
                    || string.IsNullOrEmpty(vFixedVAT)
                    || string.IsNullOrEmpty(vIsTotalPrice)
                    || string.IsNullOrEmpty(vRateChangePromotePercent)
                    || string.IsNullOrEmpty(vTrackingTrace)
                    || string.IsNullOrEmpty(vNoOfHeader)
                    || string.IsNullOrEmpty(vHeading1)
                    || string.IsNullOrEmpty(vHeading2)
                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                RateChangePromote = Convert.ToDecimal(vRateChangePromotePercent);
                PurchasePlaceQty = Convert.ToInt32(vPurchasePlaceQty);
                PurchasePlaceAmt = Convert.ToInt32(vPurchasePlaceAmt);

                #region Total Price

                IsTotalPrice = Convert.ToBoolean(vIsTotalPrice == "Y" ? true : false);

                if (IsTotalPrice == true)
                {
                    LTP.Text = "Total Price";
                }
                else
                {
                    LTP.Text = "Unit Price";

                }

                #endregion TotalPrice

                #region Fixed VAT

                FixedVAT = Convert.ToBoolean(vFixedVAT == "Y" ? true : false);

                if (FixedVAT == true)
                {
                    LVR.Text = "VAT(Amt)";
                    txtVATRate.Text = "0.00";
                }
                else
                {
                    LVR.Text = "VAT(%)";

                }
                #endregion

                #region Purchase Return

                if (rbtnPurchaseReturn.Checked)
                {
                    IsTotalPrice = false;
                    FixedVAT = false;
                    LTP.Text = "Unit Price";
                    LVR.Text = "VAT(%)";

                    txtVendorName.Enabled = false;
                    l15.Text = "Vendor";
                    ////dtpReceiveDate.Enabled = false;
                    chkIsRebate.Enabled = false;
                    dtpRebateDate.Enabled = false;
                    txtBENumber.ReadOnly = true;
                    txtSerialNo.ReadOnly = true;
                    txtLCNumber.ReadOnly = true;
                    dtpLCDate.Enabled = false;
                    ////dtpInvoiceDate.Enabled = false;
                    button1.Visible = false;
                    cmbUom.Enabled = false;
                    //txtUnitCost.ReadOnly = true;
                    //txtUnitCost.ReadOnly = false;
                    txtSD.ReadOnly = true;
                    txtLocalSDAmount.ReadOnly = true;
                    cmbVATRateInput.Enabled = false;
                    txtVATRate.ReadOnly = true;
                    txtLocalVATAmount.ReadOnly = true;
                    txtVDSRate.ReadOnly = true;
                    txtVDSAmount.ReadOnly = true;
                    chkTDS.Enabled = false;
                    cmbType.Enabled = false;
                    //cmbProduct.Enabled = false;
                    //cmbProduct.Enabled = true;
                }

                #endregion

                #region AV Calculative

                CalculativeAV = Convert.ToBoolean(vCalculativeAV == "Y" ? true : false);

                if (CalculativeAV == true)
                {
                    LAV.Text = "Assessable Value(C)";
                }
                else
                {
                    LAV.Text = "Assessable Value(F)";

                }


                if (CalculativeAV == true)
                {
                    txtCnFInpValue.ReadOnly = false;
                    txtInsInpValue.ReadOnly = false;
                    txtAVInpValue.ReadOnly = true;
                }
                else
                {
                    txtCnFInpValue.ReadOnly = true;
                    txtInsInpValue.ReadOnly = true;
                    txtAVInpValue.ReadOnly = false;
                }

                #endregion AV Calculative

                #region Tracking Trace

                TrackingTrace = vTrackingTrace;
                NoOfHeader = vNoOfHeader;
                SHeading1 = vHeading1;
                SHeading2 = vHeading2;

                #endregion

                #endregion Settings

                #region Element Stats

                cmbType1.Items.Add("Local");
                cmbType1.Items.Add("Import");

                cmbType1.Visible = false;
                label13.Visible = false;
                cmbType1.SelectedIndex = 0;
                rbtnCode.Checked = true;
                tabPage2.Visible = true;
                tabPage4.Visible = false;
                tcPurchase.TabPages.Remove(tabPage4);
                tabPageSerial.Visible = false;
                tcPurchase.TabPages.Remove(tabPageSerial);
                txtRebatePercent.Visible = false;
                LRebate.Visible = false;
                chkImport.Checked = false;
                chkImport.Visible = false;

                this.Text = "Purchase Entry";
                l15.Visible = true;
                cmbVendor.Visible = true;
                btnVendorName.Visible = true;
                txtVendorName.Visible = true;
                txtVATRate.Visible = true;
                txtSD.Visible = true;
                txtUnitCost.Visible = true;
                LVR.Visible = true;
                l14.Visible = true;
                txtUnitCost.ReadOnly = false;
                cmbType.Enabled = true;

                dgvPurchase.Columns["CnF"].Visible = false;
                dgvPurchase.Columns["Insurance"].Visible = false;
                dgvPurchase.Columns["AV"].Visible = false;
                dgvPurchase.Columns["CD"].Visible = false;
                dgvPurchase.Columns["RD"].Visible = false;
                dgvPurchase.Columns["TVB"].Visible = false;
                dgvPurchase.Columns["TVA"].Visible = false;
                dgvPurchase.Columns["ATV"].Visible = false;
                dgvPurchase.Columns["Others"].Visible = false;

                dgvPurchase.Columns["Rebate"].Visible = false;
                dgvPurchase.Columns["RebateAmount"].Visible = false;

                txtRebatePercent.Text = "0";

                cmbVDS.Visible = false;
                LVDS.Visible = false;
                cmbVDS.Items.Clear();
                cmbVDS.Items.Add("Y");
                cmbVDS.Items.Add("N");
                cmbVDS.Text = "N";
                LPP.Visible = false;
                btnPrePurSearch.Visible = false;
                txtPrePurId.Visible = false;
                LPPQ.Visible = false;
                cmbType1.Enabled = false;
                txtPurQty.Visible = false;
                label13.Visible = false;
                chkImport.Checked = false;
                txtQuantity.ReadOnly = false;
                cmbUom.Enabled = true;

                #endregion

                #region Transaction Type

                txtUSDInvoiceValue.Text = "1";
                txtUSDInvoiceValue.Visible = false;
                txtUSDValue.Text = "1";
                txtUSDValue.Visible = false;

                label45.Visible = false;
                label46.Visible = false;
                txtVehicleNo.Visible = false;
                txtVehicleType.Visible = false;
                l5.Visible = false;
                #region Other


                if (rbtnOther.Checked) //start
                {

                    label2.Text = "Pur No";
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    cmbType1.Text = "Local";

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    cmbProductType.Text = "Raw";
                    cmbVDS.Text = "N";
                    this.Text = "Purchase Entry";

                }
                #endregion Other

                #region Client Raw Receive

                if (rbtnClientRawReceive.Checked)
                {

                    label2.Text = "Pur No";
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    cmbType1.Text = "Local";

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    cmbProductType.Text = "Raw";
                    cmbVDS.Text = "N";
                    this.Text = "Client Raw Receive Entry";
                    tcPurchase.TabPages[0].Text = "Client Raw Receive Entry";
                    btnVAT16.Visible = false;
                    btnFormKaT.Visible = false;
                    btnPurchaseInformation.Visible = false;

                }

                #endregion Client Raw Receive

                #region Client FG Receive WOBOM

                if (rbtnClientFGReceiveWOBOM.Checked)
                {

                    label2.Text = "Pur No";
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    cmbType1.Text = "Local";

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    cmbProductType.Text = "Finish";
                    txtCategoryName.Text = "Finish";
                    cmbVDS.Text = "N";
                    this.Text = "Client FG Receive WOBOM Entry";
                    tcPurchase.TabPages[0].Text = "Client FG Receive WOBOM Entry";
                    btnVAT16.Visible = false;
                    btnFormKaT.Visible = false;
                    btnPurchaseInformation.Visible = false;

                    btnVAT17.Visible = true;
                }

                #endregion Client FG Receive WOBOM

                #region rbtnPurchaseDN

                else if (rbtnPurchaseDN.Checked)
                {

                    LPPQ.Visible = true;

                    txtPurQty.Visible = true;
                    label2.Text = "Pur No";
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    cmbType1.Text = "Local";

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    cmbProductType.Text = "Raw";
                    cmbVDS.Text = "N";

                    LPP.Visible = true;
                    btnPrePurSearch.Visible = true;
                    txtPrePurId.Visible = true;
                    cmbVendor.Enabled = false;
                    btnVendorName.Enabled = false;
                    //button1.Enabled = false;
                    txtPrePurId.ReadOnly = false;

                    this.Text = "Purchase Decrease Entry";
                    tcPurchase.TabPages[0].Text = "Purchase Decrease Entry";

                }

                #endregion rbtnPurchaseDN

                #region rbtnPurchaseCN

                else if (rbtnPurchaseCN.Checked)
                {

                    LPPQ.Visible = true;

                    txtPurQty.Visible = true;
                    label2.Text = "Pur No";
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    cmbType1.Text = "Local";

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    cmbProductType.Text = "Raw";
                    cmbVDS.Text = "N";

                    LPP.Visible = true;
                    btnPrePurSearch.Visible = true;
                    txtPrePurId.Visible = true;
                    cmbVendor.Enabled = false;
                    btnVendorName.Enabled = false;
                    //button1.Enabled = false;
                    this.Text = "Purchase Increase Entry";
                    tcPurchase.TabPages[0].Text = "Purchase Increase Entry";

                }

                #endregion rbtnPurchaseCN

                #region rbtnPurchaseReturn

                else if (rbtnPurchaseReturn.Checked)
                {
                    //cmbProduct.Enabled = false;
                    dtpReceiveDate.Enabled = true;
                    rbtnCode.Enabled = false;
                    rbtnProduct.Enabled = false;
                    btnAdd.Enabled = true;

                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Local";
                    cmbType1.Visible = true;

                    cmbProductType.Text = "Raw";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";//start
                    LPP.Visible = true;
                    btnPrePurSearch.Visible = true;
                    txtPrePurId.Visible = true;


                    //txtUnitCost.ReadOnly = false;
                    txtVendorName.ReadOnly = true;
                    cmbVendor.Enabled = true;
                    btnVendorName.Enabled = true;
                    button1.Enabled = true;
                    button3.Visible = true;

                    this.Text = "Purchase Return Entry";
                    btnVAT12.Visible = false;
                    tcPurchase.TabPages[0].Text = "Purchase Return Entry";
                    txtVehicleNo.Visible = true;
                    txtVehicleType.Visible = true;
                    l5.Visible = true;

                }
                #endregion rbtnPurchaseReturn

                #region rbtnInputService
                else if (rbtnInputService.Checked || rbtnPurchaseTollcharge.Checked)
                {
                    chkIsHouseRent.Checked = true;
                    cmbVDS.Visible = true;
                    cmbVDS.Text = "Y";
                    LVDS.Visible = true;
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Local";
                    cmbType1.Visible = true;
                    cmbProductType.Text = "Overhead";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";
                    chkImport.Visible = true;
                    label13.Visible = true;

                    txtRebatePercent.Visible = true;
                    LRebate.Visible = true;
                    txtQuantity.Text = "1";
                    txtQuantity.ReadOnly = true;
                    txtQuantity.TabStop = false;
                    cmbUom.Enabled = false;
                    dgvPurchase.Columns["Rebate"].Visible = false;
                    dgvPurchase.Columns["RebateAmount"].Visible = true;

                    this.Text = "Input Service Purchase Entry";
                    tcPurchase.TabPages[0].Text = "Input Service Purchase Entry";
                    if (rbtnPurchaseTollcharge.Checked)
                    {
                        this.Text = "Purchase Toll Charge Entry";
                        tcPurchase.TabPages[0].Text = "Purchase Toll Charge Entry";
                        txtIsRaw.Text = "Overhead";
                    }



                }
                #endregion rbtnInputService

                #region rbtnTrading

                else if (rbtnTrading.Checked)//start
                {

                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Local";
                    cmbProductType.Text = "Trading";
                    btnProductType.Enabled = false;
                    txtIsRaw.Text = "Trading";
                    cmbType1.Visible = true;
                    label13.Visible = true;
                    chkImport.Visible = true;
                    btnFormKaT.Visible = true;

                    cmbVDS.Visible = true;
                    LVDS.Visible = true;

                    this.Text = "Trading Purchase Entry";
                    btnFormKaT.Visible = true;
                    tcPurchase.TabPages[0].Text = "Trading Purchase Entry";

                }
                #endregion rbtnTrading

                #region rbtnService

                else if (rbtnService.Checked)
                {
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Local";
                    cmbType1.Visible = true;
                    cmbProductType.Text = "Service";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";
                    label13.Visible = true;
                    chkImport.Visible = true;
                    txtRebatePercent.Visible = false;
                    LRebate.Visible = false;
                    //txtQuantity.Text = "1";
                    //txtQuantity.ReadOnly = true;
                    //txtQuantity.TabStop = false;
                    cmbUom.Enabled = false;
                    dgvPurchase.Columns["Rebate"].Visible = false;
                    dgvPurchase.Columns["RebateAmount"].Visible = false;
                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    this.Text = "Service Purchase Entry(Stock)";
                    tcPurchase.TabPages[0].Text = "Service Purchase Entry(Stock)";

                }
                #endregion   rbtnService

                #region   rbtnServiceNS
                else if (rbtnServiceNS.Checked)
                {
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Local";
                    cmbType1.Visible = true;
                    cmbProductType.Text = "Service(NonStock)";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";
                    label13.Visible = true;
                    chkImport.Visible = true;
                    txtRebatePercent.Visible = false;
                    LRebate.Visible = false;
                    txtQuantity.Text = "1";
                    txtQuantity.ReadOnly = true;
                    txtQuantity.TabStop = false;
                    cmbUom.Enabled = false;
                    dgvPurchase.Columns["Rebate"].Visible = false;
                    dgvPurchase.Columns["RebateAmount"].Visible = false;
                    cmbVDS.Visible = true;
                    LVDS.Visible = true;
                    this.Text = "Service Purchase Entry(Non Stock)";
                    tcPurchase.TabPages[0].Text = "Service Purchase Entry(Non Stock)";


                }
                #endregion   rbtnServiceNS

                #region   rbtnImport

                else if (rbtnImport.Checked)
                {
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Import";
                    cmbType.Visible = true;
                    cmbType1.Visible = true;
                    chkIsHouseRent.Visible = false;
                    cmbProductType.Text = "Raw";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";
                    chkImport.Checked = true;
                    this.Text = "Import Entry";
                    //tcPurchase.TabPages.Add(tabPage4);
                    dgvPurchase.Columns["CnF"].Visible = true;
                    dgvPurchase.Columns["Insurance"].Visible = true;
                    dgvPurchase.Columns["AV"].Visible = true;
                    dgvPurchase.Columns["CD"].Visible = true;
                    dgvPurchase.Columns["RD"].Visible = true;
                    ////dgvPurchase.Columns["TVB"].Visible = true;
                    ////dgvPurchase.Columns["TVA"].Visible = true;
                    dgvPurchase.Columns["ATV"].Visible = true;
                    dgvPurchase.Columns["Others"].Visible = true;
                    txtVDSRate.Visible = false;
                    txtVDSAmount.Visible = false;
                    lblVDSRate.Visible = false;
                    label43.Visible = false;
                    txtCategoryName.Text = "Raw";
                    label33.Visible = true;
                    txtCustomHouse.Visible = true;
                    label59.Visible = true;
                    tcPurchase.TabPages[0].Text = "Import Entry";


                }
                #endregion   rbtnImport

                #region   rbtnCommercialImporter

                else if (rbtnCommercialImporter.Checked)
                {
                    l2.Visible = true;
                    cmbType.Visible = true;
                    cmbType1.Text = "Import";
                    cmbType.Visible = true;
                    cmbType1.Visible = true;

                    cmbProductType.Text = "Trading";
                    btnProductType.Enabled = false;
                    //txtIsRaw.Text = "Trading";
                    chkImport.Checked = true;
                    this.Text = "Commercial Importer";
                    //tcPurchase.TabPages.Add(tabPage4);
                    dgvPurchase.Columns["CnF"].Visible = true;
                    dgvPurchase.Columns["Insurance"].Visible = true;
                    dgvPurchase.Columns["AV"].Visible = true;
                    dgvPurchase.Columns["CD"].Visible = true;
                    dgvPurchase.Columns["RD"].Visible = true;
                    ////dgvPurchase.Columns["TVB"].Visible = true;
                    ////dgvPurchase.Columns["TVA"].Visible = true;
                    dgvPurchase.Columns["ATV"].Visible = true;
                    dgvPurchase.Columns["Others"].Visible = true;
                    txtVDSRate.Visible = false;
                    txtVDSAmount.Visible = false;
                    lblVDSRate.Visible = false;
                    label43.Visible = false;
                    txtCategoryName.Text = "Trading";

                    txtUSDInvoiceValue.Text = "0";
                    txtUSDInvoiceValue.Visible = true;
                    txtUSDValue.Text = "0";
                    txtUSDValue.Visible = true;

                    label45.Visible = true;
                    label46.Visible = true;
                    tcPurchase.TabPages[0].Text = "Commercial Importer Entry";

                }
                #endregion   rbtnCommercialImporter

                #region   rbtnTollReceive

                else if (rbtnTollReceive.Checked)
                {
                    l2.Visible = false;
                    label55.Visible = false;
                    //////cmbType.Visible = false;

                    //txtIsRaw.Text = "Toll";
                    cmbType1.Text = "Local";

                    this.Text = "Toll Receive Entry";
                    cmbProductType.Text = "Service";

                    //txtUnitCost.ReadOnly = true;
                    btnVAT17.Visible = true;
                    btnVAT16.Visible = false;
                    IsTotalPrice = false;
                    LTP.Text = "Unit Price";
                    btnVAT17.Text = "TR(17)";

                    //06-Dec-2020
                    btnVAT16.Visible = false;
                    btnVAT17.Visible = false;
                    btnFormKaT.Visible = false;
                    tcPurchase.TabPages[0].Text = "Toll Receive Entry";

                }
                #endregion   rbtnTollReceive

                #region   rbtnTollReceiveRaw

                else if (rbtnTollReceiveRaw.Checked)
                {
                    cmbType1.Text = "Local";
                    cmbProductType.Text = "Raw";

                    this.Text = "Toll Receive(Raw) Entry";
                    btnVAT18.Visible = false;

                    string vTollItemReceive = string.Empty;
                    bool IsTollRegister = false;

                    vTollItemReceive = commonDal.settingsDesktop("TollItemReceive", "AttachedWithVAT6_1", null, connVM);
                    if (vTollItemReceive == "N")
                    {
                        IsTollRegister = true;
                    }
                    if (IsTollRegister == true)
                    {
                        btnVAT16.Text = "Register";

                    }
                    btnVAT16.Visible = false;

                    //06-Dec-2020
                    btnVAT16.Visible = false;
                    btnVAT17.Visible = false;
                    btnFormKaT.Visible = false;
                    tcPurchase.TabPages[0].Text = "Toll Receive(Raw) Entry";
                    LTP.Visible = false;
                    txtUnitCost.Visible = false;
                    label42.Visible = false;

                }
                #endregion   rbtnTollReceiveRaw

                cmbType1.Visible = false;

                #endregion Transaction Type

                #region Tracking Trace

                if (TrackingTrace == "Y")
                {
                    tcPurchase.TabPages.Add(tabPageSerial);
                    tabPageSerial.Visible = true;
                    groupBox6.Visible = true;
                    lblHeading1.Text = SHeading1.ToString();
                    lblHeading2.Text = SHeading2.ToString();
                    dgvSerialTrack.Columns["Heading1"].HeaderText = SHeading1.ToString();
                    dgvSerialTrack.Columns["Heading2"].HeaderText = SHeading2.ToString();

                    if (rbtnPurchaseDN.Checked || rbtnPurchaseCN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        btnAddS.Enabled = false;
                        btnChangeS.Enabled = false;
                        btnRemoveS.Enabled = false;
                    }
                }
                else
                {
                    tcPurchase.TabPages.Remove(tabPageSerial);
                    tabPageSerial.Visible = false;
                    groupBox6.Visible = false;
                }

                #endregion Tracking Trace

                #region Element Stats

                btnVendorName.Visible = false;

                rbtnProduct.Checked = true;

                #endregion

                #region Button Import Integration Lisence
                if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
                {
                    btnImport.Visible = false;
                }
                #endregion

                #region TDS Lisence Issue
                if (Program.IsTDS == false)
                {
                    chkTDS.Visible = false;
                    label48.Visible = false;
                    txtTDSAmount.Visible = false;
                    btnTDSCalc.Visible = false;
                    label49.Visible = false;
                    txtNetBill.Visible = false;
                }
                #endregion

                #region Trading Lisence Issue
                if (Program.IsTrading == false)
                {
                    btnFormKaT.Visible = false;

                }
                #endregion


                //cmbVDS.Visible = false;
                //LVDS.Visible = false;
                chkIsHouseRent.Enabled = false;

                if (realTimeEntry == "N")
                {
                    dtpReceiveDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy 00:00:00"));

                    dtpReceiveDate.CustomFormat = "dd-MM-yyyy";
                }

                string CompanyCode = commonDal.settingValue("CompanyCode", "Code", connVM);

                if (OrdinaryVATDesktop.IsACICompany(CompanyCode))
                {
                    label58.Visible = false;
                    txtBankGuarantee.Visible = false;
                    btnPurchase.Visible = false;
                }

                string IsTDSShow = commonDal.settingsMaster("Purchase", "IsTDSShow");

                if (IsTDSShow == "N")
                {
                    chkTDS.Visible = false;
                    label48.Visible = false;
                    txtTDSAmount.Visible = false;
                    label49.Visible = false;
                    txtNetBill.Visible = false;
                }
                else
                {
                    chkTDS.Visible = true;
                    label48.Visible = true;
                    txtTDSAmount.Visible = true;
                    label49.Visible = true;
                    txtNetBill.Visible = true;
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            #endregion

        }

        private void SelectproductGroupType()
        {
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
                //txtCategoryName.Text = "Trading";
            }
            else if (rbtnTrading.Checked || rbtnTradingImport.Checked)
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
            else if (rbtnCommercialImporter.Checked)
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
            else if (rbtnTollReceive.Checked)
            {

                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Overhead";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Overhead";
            }
            else if (rbtnInputService.Checked || rbtnInputServiceImport.Checked || rbtnPurchaseTollcharge.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Overhead";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Overhead";
            }
            else if (rbtnService.Checked || rbtnServiceImport.Checked)
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
            else if (rbtnServiceNS.Checked || rbtnServiceNSImport.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Service(NonStock)";
                HSCodeNoParam = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Service(NonStock)";
            }
            else if (rbtnPurchaseReturn.Checked)
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
            else if (rbtnTollReceiveRaw.Checked)
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
            else if (rbtnImport.Checked)
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
            else if (rbtnOther.Checked)
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
            else if (rbtnPurchaseDN.Checked)
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
            else if (rbtnPurchaseCN.Checked)
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
            else if (rbtnClientRawReceive.Checked)
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
            else if (rbtnClientFGReceiveWOBOM.Checked)
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
            #endregion Product
        }

        private void FormLoad()
        {

            SelectproductGroupType();

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

            #region vendor

            VendorIDParam = string.Empty;
            VendorNameParam = string.Empty;
            VendorGroupIDParam = string.Empty;
            VendorGroupNameParam = string.Empty;
            TINNoParam = string.Empty;
            VATRegistrationNoParam = string.Empty;
            ActiveStatusVendorParam = "Y";

            #endregion vendor

            #region 2012 Law - Button Control

            btnVAT16.Text = "6.1";
            btnVAT17.Text = "6.2";
            btnVAT18.Visible = false;
            btnVAT12.Text = "6.7";

            #endregion
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                if (true)
                {
                    #region Product
                    string SelectAll = "All";
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    List<string> IsRawParamList = new List<string>();
                    IsRawParamList.Add(IsRawParam);

                    ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam, ActiveStatusProductParam, TradingParam,
                                                                     NonStockParam, ProductCodeParam, connVM);

                    //2020-12-12
                    ////ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, "", HSCodeNoParam, ActiveStatusProductParam, TradingParam,
                    ////                                                 NonStockParam, ProductCodeParam, connVM, IsRawParamList);


                    #endregion Product
                    #region vendor

                    //VendorDAL vendorDal = new VendorDAL();
                    IVendor vendorDal = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);

                    string[] cvals = new string[] { VendorIDParam, VendorNameParam, VendorGroupIDParam, VendorGroupNameParam, TINNoParam, VATRegistrationNoParam, ActiveStatusVendorParam, SelectAll };
                    string[] cfields = new string[] { "v.VendorID like", "v.VendorName like", "v.VendorGroupID like", "vg.VendorGroupName like", "v.TINNo like", "v.VATRegistrationNo like", "v.ActiveStatus like", "SelectTop" };
                    VendorGroupResult = vendorDal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                    //VendorGroupResult = vendorDal.SearchVendorSingleDTNew(VendorIDParam, VendorNameParam,
                    //VendorGroupIDParam, VendorGroupNameParam, TINNoParam, VATRegistrationNoParam,
                    //                                                      ActiveStatusVendorParam);

                    #endregion vendor
                }

                #region UOM
                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cval = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfield = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfield, cval, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

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

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Statement
                if (true)
                {
                    #region Product

                    ProductsMini.Clear();
                    cmbProduct.Items.Clear();
                    productCmb.Clear();

                    //cmbProduct.DataSource = ProductResultDs;
                    //cmbProduct.DisplayMember = "ProductName";// displayMember.Replace("[", "").Replace("]", "").Trim();
                    ////cmbProduct.ValueMember = "ItemNo";// valueMember.Trim();
                    foreach (DataRow item2 in ProductResultDs.Rows)
                    {

                        var prod = new ProductMiniDTO();
                        var prodcmb = new ProductCmbDTO();
                        prod.ItemNo = item2["ItemNo"].ToString();

                        prodcmb.ItemNo = item2["ItemNo"].ToString();
                        prodcmb.ProductName = item2["ProductName"].ToString();
                        prodcmb.ProductCode = item2["ProductCode"].ToString();

                        prod.ProductName = item2["ProductName"].ToString();
                        prod.ProductCode = item2["ProductCode"].ToString();
                        cmbProduct.Items.Add(item2["ProductName"]);

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
                        ProductsMini.Add(prod);
                    }

                    #endregion Product

                    #region vendor

                    vendorsMini.Clear();
                    cmbVendor.Items.Clear();
                    foreach (DataRow item2 in VendorGroupResult.Rows)
                    {
                        var vendor = new VendorMiniDTO();
                        vendor.VendorID = item2["VendorID"].ToString();
                        vendor.VendorCode = item2["VendorCode"].ToString();
                        vendor.VendorName = item2["VendorName"].ToString();
                        //cmbVendor.Items.Add(item2["VendorName"]);
                        vendor.VendorGroupID = item2["VendorGroupID"].ToString();
                        vendor.VendorGroupName = item2["VendorGroupName"].ToString();
                        vendor.Address1 = item2["Address1"].ToString();
                        vendor.Address2 = item2["Address2"].ToString();
                        vendor.Address3 = item2["Address3"].ToString();
                        vendor.TelephoneNo = item2["TelephoneNo"].ToString();
                        vendor.FaxNo = item2["FaxNo"].ToString();
                        vendor.Email = item2["Email"].ToString();
                        vendor.TINNo = item2["TINNo"].ToString();
                        vendor.VATRegistrationNo = item2["VATRegistrationNo"].ToString();
                        vendor.ActiveStatus = item2["ActiveStatus"].ToString();
                        vendor.GroupType = item2["VendorGroupName"].ToString();
                        vendor.Country = item2["Country"].ToString();

                        vendorsMini.Add(vendor);

                    }

                    vendorloadToCombo();
                    vendors();


                    #endregion vendor
                }

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }

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
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];


                ProductSearchDsFormLoad();
                ////UOMSearch();

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

        private void ProductSearchDsFormLoad()
        {
            ProductData = string.Empty;

            try
            {
                SelectproductGroupType();

                //cmbProduct.Enabled = false;
                button1.Enabled = false;

                this.progressBar1.Visible = true;
                bgwLoadProduct.RunWorkerAsync();

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

        private void bgwLoadProduct_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                ActiveStatusProductParam, TradingParam, NonStockParam, ProductCodeParam, connVM);


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

        private void bgwLoadProduct_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                #region Product

                ProductsMini.Clear();
                cmbProduct.Items.Clear();
                cmbProduct.Refresh();
                productCmb.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {

                    var prod = new ProductMiniDTO();
                    var prodcmb = new ProductCmbDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();

                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();

                    cmbProduct.Items.Add(item2["ProductName"]);


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
                    ProductsMini.Add(prod);
                }
                #endregion Product


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
                ChangeData = false;
                //cmbProduct.Enabled = false;
                button1.Enabled = true;
                cmbProduct.Focus();
                this.progressBar1.Visible = false;
            }


        }

        private void bgwUOM_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);


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

        private void bgwUOM_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
            finally
            {
                cmbUom.Enabled = true;
                this.progressBar1.Visible = false;

            }
        }

        private void bgwLoadVendor_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                //VendorDAL vendorDal = new VendorDAL();
                IVendor vendorDal = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { VendorIDParam, VendorNameParam, VendorGroupIDParam, VendorGroupNameParam, TINNoParam, VATRegistrationNoParam, ActiveStatusVendorParam };
                string[] cfields = new string[] { "v.VendorID like", "v.VendorName like", "v.VendorGroupID like", "vg.VendorGroupName like", "v.TINNo like", "v.VATRegistrationNo like", "v.ActiveStatus like" };
                VendorGroupResult = vendorDal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //VendorGroupResult = vendorDal.SearchVendorSingleDTNew(VendorIDParam, VendorNameParam, VendorGroupIDParam, VendorGroupNameParam, TINNoParam,VATRegistrationNoParam, ActiveStatusVendorParam);

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

        private void bgwLoadVendor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                #region Vendor

                vendorsMini.Clear();
                cmbVendor.Items.Clear();
                foreach (DataRow item2 in VendorGroupResult.Rows)
                {
                    var vendor = new VendorMiniDTO();
                    vendor.VendorID = item2["VendorID"].ToString();
                    vendor.VendorName = item2["VendorName"].ToString();
                    //cmbVendor.Items.Add(item2["VendorName"]);
                    vendor.VendorGroupID = item2["VendorGroupID"].ToString();
                    vendor.VendorGroupName = item2["VendorGroupName"].ToString();
                    vendor.Address1 = item2["Address1"].ToString();
                    vendor.Address2 = item2["Address2"].ToString();
                    vendor.Address3 = item2["Address3"].ToString();
                    vendor.TelephoneNo = item2["TelephoneNo"].ToString();
                    vendor.FaxNo = item2["FaxNo"].ToString();
                    vendor.Email = item2["Email"].ToString();
                    vendor.TINNo = item2["TINNo"].ToString();
                    vendor.VATRegistrationNo = item2["VATRegistrationNo"].ToString();
                    vendor.ActiveStatus = item2["ActiveStatus"].ToString();
                    vendor.GroupType = item2["GroupType"].ToString();
                    vendor.Country = item2["Country"].ToString();

                    vendorsMini.Add(vendor);

                }

                #endregion Vendor


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
                cmbVendor.Enabled = true;
                btnVendorName.Enabled = true;
                this.progressBar1.Visible = false;

                vendorloadToCombo();
                vendors();

            }

        }

        private void vendorloadToCombo()
        {
            try
            {
                cmbVendor.Items.Clear();
                //var VByCode="";
                //var VByName="";
                string groupType = string.Empty;
                if (chkImport.Checked)
                {
                    groupType = "Import";
                }
                else
                {
                    groupType = "Local";
                }



                var VByName = from cus in vendorsMini.ToList()
                              where cus.GroupType.ToLower() == groupType.ToLower()

                              orderby cus.VendorName
                              select cus.VendorName;


                if (VByName != null && VByName.Any())
                {
                    cmbVendor.Items.AddRange(VByName.ToArray());
                }

                cmbVendor.Items.Insert(0, "Select");

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

        private void vendors()
        {

            try
            {
                #region Statement


                if (cmbType1.Text.Trim().ToLower() == "local")
                {
                    LBE.Text = "Challan/Other No.";
                    label38.Visible = true;
                }
                else
                {
                    LBE.Text = "BE No.";
                    //label39.Visible = true;
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

                        //txtUomConv.Text = uoms.First().ToString();
                    }
                }
                cmbUom.Text = txtUOM.Text.Trim();

                //cmbUom.Text = uOMTo;


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

        public void ClearAllFields()
        {

            txtBankGuarantee.Clear();
            txtId.Text = "0";
            SearchBranchId = 0;
            txtFiscalYear.Text = "0";


            cmbItemNo.Text = "Select";
            cmbProduct.Text = "Select";
            cmbProductName.Text = "Select";
            cmbVendor.Text = "Select";
            cmbVDS.Text = "N";
            chkIsHouseRent.Checked = false;

            txtBENumber.Text = "";
            txtComments.Text = "";
            txtCommentsDetail.Text = "NA";
            txtHSCode.Text = "";
            txtNBRPrice.Text = "0.00";
            txtSD.Text = "0.00";
            txtPreviousSubTotal.Text = "0.00";
            txtTDSAmount.Text = "0.00";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtPurchaseInvoiceNo.Text = "";
            if (rbtnInputService.Checked || rbtnInputServiceImport.Checked || rbtnServiceNS.Checked || rbtnServiceNSImport.Checked || rbtnPurchaseTollcharge.Checked)
            {
                txtQuantity.Text = "1";
            }


            else
            {
                txtQuantity.Text = "0.00";
            }
            txtSerialNo.Text = "";
            txtLCNumber.Text = "";
            txtTotalAmount.Text = "0.00";
            txtTotalVATAmount.Text = "0.00";
            txtUnitCost.Text = "";
            txtVATRate.Text = "0.0";
            txtVendorGroupName.Text = "";
            txtVendorID.Text = "";
            txtVendorName.Text = "";
            dgvPurchase.Rows.Clear();
            dgvReceiveHistory.Rows.Clear();
            txtCustomHouse.Text = "";
            txtCustomHouseCode.Text = "";
            CommonDAL commonDal = new CommonDAL();

            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate", null, connVM);
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpInvoiceDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpReceiveDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpLCDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
            else
            {
                dtpInvoiceDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpReceiveDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpLCDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            }
            txtUOM.Text = "";
            txtQuantityInHand.Text = "0.00";
            txtTotalSubTotal.Text = "0.00";
            txtTotalSDAmount.Text = "0.00";
            txtPrePurId.Text = "";
            txtLandedCost.Text = "0.00";

            #region Duties

            //Duties:
            txtTotalSubTotal1.Text = "0.00";
            txtCnFInpValue.Text = "0.00";
            txtCnFRate.Text = "0.00";
            txtCnFAmount.Text = "0.00";
            txtInsInpValue.Text = "0.00";
            txtInsRate.Text = "0.00";
            txtInsAmount.Text = "0.00";
            txtAVInpValue.Text = "0.00";
            txtAVAmount.Text = "0.00";
            txtCDInpValue.Text = "0.00";
            txtCDRate.Text = "0.00";
            txtCDAmount.Text = "0.00";
            txtRDInpValue.Text = "0.00";
            txtRDRate.Text = "0.00";
            txtRDAmount.Text = "0.00";
            txtTVBInpValue.Text = "0.00";
            txtTVBRate.Text = "0.00";
            txtTVBAmount.Text = "0.00";
            txtSDInpValue.Text = "0.00";
            txtSDRate.Text = "0.00";
            txtSDAmount.Text = "0.00";
            txtVATInpValue.Text = "0.00";
            txtVATRateI.Text = "0.00";
            txtVATAmount.Text = "0.00";
            txtTVAInpValue.Text = "0.00";
            txtTVARate.Text = "0.00";
            txtTVAAmount.Text = "0.00";
            txtATVInpValue.Text = "0.00";
            txtATVRate.Text = "0.00";
            txtATVAmount.Text = "0.00";
            txtOthersInpValue.Text = "0.00";
            txtOthersRate.Text = "0.00";
            txtOthersAmount.Text = "0.00";
            txtDutiesRemarks.Text = "";
            dgvDuty.Rows.Clear();
            txtAITInpValue.Text = "0.00";
            txtAITAmount.Text = "0.00";
            txtInvoiceValue.Text = "0.00";
            txtExchangeRate.Text = "0.00";
            txtCurrency.Text = "";

            #endregion

            #region Tracking

            //Tracking
            dgvSerialTrack.Rows.Clear();
            txtItemNoS.Text = "";
            txtTotalQtyS.Text = "";
            cmbPCodeS.Text = "";
            cmbPNameS.Text = "";
            txtHeading1.Text = "";
            txtHeading2.Text = "";
            chkIsFixedOtherVAT.Checked = false;
            chkIsFixedVATRebate.Checked = false;
            #endregion


        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void selectLastRow()
        {
            try
            {
                #region Statement

                if (dgvPurchase.Rows.Count > 0)
                {
                    dgvPurchase.Rows[dgvPurchase.Rows.Count - 1].Selected = true;
                    dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.Rows.Count - 1].Cells[1];
                    if (transactionType.ToLower().Contains("import"))
                    {
                        dgvDuty.CurrentCell = dgvDuty.Rows[dgvDuty.Rows.Count - 1].Cells[1];
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void RateCheck()
        {
            try
            {


                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {
                    string tt = string.Empty;

                    decimal vSubtotal = 0;
                    decimal vCnF = 0;
                    decimal vInsurance = 0;
                    decimal vCD = 0;
                    decimal vRD = 0;
                    decimal vTVB = 0;
                    decimal vSDAmount = 0;
                    decimal vVatableprice = 0;
                    decimal vunitPrice = 0;
                    decimal vQuantity = 0;

                    string vItemNo = string.Empty;
                    string vItemName = string.Empty;
                    string vItemCode = string.Empty;

                    vItemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                    vItemName = dgvPurchase.Rows[i].Cells["ItemName"].Value.ToString();
                    vItemCode = dgvPurchase.Rows[i].Cells["PCode"].Value.ToString();

                    vSubtotal = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Subtotal"].Value.ToString());
                    vCnF = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CnF"].Value.ToString());
                    vInsurance = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Insurance"].Value.ToString());
                    vCD = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CD"].Value.ToString());
                    vRD = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["RD"].Value.ToString());
                    vTVB = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["TVB"].Value.ToString());
                    vSDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SDAmount"].Value.ToString());
                    vQuantity = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                    vVatableprice = vSubtotal + vCnF + vInsurance + vCD + vRD + vTVB + vSDAmount;
                    if (vQuantity > 0)
                    {

                        vunitPrice = vVatableprice / vQuantity;
                        if (string.IsNullOrEmpty(vItemNo))
                        {
                            MessageBox.Show("There is no item to Check", this.Text);
                            return;
                        }
                        tt = RateChangePercent(vItemNo, vunitPrice);
                        if (string.IsNullOrEmpty(tt))
                        {
                            MessageBox.Show("Item " + vItemName + "(" + vItemCode + ")\n is not included in any price declaration", "Unit Price",
                                           MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //MessageBox.Show("This item is not included in any price declaration", "Unit Price",
                            //                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //return;
                        }
                        else
                        {
                            MessageBox.Show("Item " + vItemName + "(" + vItemCode + ")\n" + tt, "Unit Price",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private string RateChangePercent(string ItemNo, decimal unitPrice)
        {
            string result = string.Empty;


            decimal plusRateChangePromotPercent = 0;
            decimal minusRateChangePromotPercent = 0;
            plusRateChangePromotPercent = Convert.ToDecimal("+" + RateChangePromote);
            minusRateChangePromotPercent = Convert.ToDecimal("-" + RateChangePromote);
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            decimal bomVatablePrice = productDal.GetLastVatableFromBOM(ItemNo, null, null, connVM);
            if (bomVatablePrice <= 0)
            {
                //result = "This item is not included in any price declaration";
                return result;
            }
            decimal changes = (unitPrice - bomVatablePrice) * 100 / bomVatablePrice;
            if (changes > plusRateChangePromotPercent || changes < minusRateChangePromotPercent)
            {
                result = "In Purchase price : " + unitPrice.ToString("0,0.0000") + "\nIn Last Price Declaration : " + bomVatablePrice.ToString("0,0.0000") + "" +
                                "\nChanges : " + changes.ToString("0,0.0000") + "%";


            }
            return result;

        }

        private void transactionTypes()
        {
            if ((rbtnImport.Checked == true)
                           || (rbtnTradingImport.Checked == true)
                           || (rbtnServiceImport.Checked == true)
                            || (rbtnInputServiceImport.Checked == true)
                           || (rbtnServiceNSImport.Checked == true)
                           || (rbtnCommercialImporter.Checked == true)
                )
            {
                //dutyCalculate = true;
            }
            #region Transaction Type

            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }
            else if (rbtnPurchaseDN.Checked)
            {
                transactionType = "PurchaseDN";
            }
            else if (rbtnPurchaseCN.Checked)
            {
                transactionType = "PurchaseCN";
            }
            else if (rbtnTrading.Checked)
            {
                transactionType = "Trading";
            }
            else if (rbtnTradingImport.Checked)
            {
                transactionType = "TradingImport";
            }
            else if (rbtnTollReceive.Checked)
            {
                transactionType = "TollReceive";
            }
            else if (rbtnPurchaseReturn.Checked)
            {
                transactionType = "PurchaseReturn";
            }
            else if (rbtnInputService.Checked)
            {
                transactionType = "InputService";
            }
            else if (rbtnPurchaseTollcharge.Checked)
            {
                transactionType = "PurchaseTollcharge";
            }

            else if (rbtnInputServiceImport.Checked)
            {
                transactionType = "InputServiceImport";
            }
            else if (rbtnService.Checked)
            {
                transactionType = "Service";
            }
            else if (rbtnServiceNS.Checked)
            {
                transactionType = "ServiceNS";
            }
            else if (rbtnServiceNSImport.Checked)
            {
                transactionType = "ServiceNSImport";
            }
            else if (rbtnServiceImport.Checked)
            {
                transactionType = "ServiceImport";
            }
            else if (rbtnImport.Checked)
            {
                transactionType = "Import";
            }
            else if (rbtnCommercialImporter.Checked)
            {
                transactionType = "CommercialImporter";
            }
            else if (rbtnTollReceiveRaw.Checked)
            {
                transactionType = "TollReceiveRaw";
            }
            else if (rbtnClientRawReceive.Checked)
            {
                transactionType = "ClientRawReceive";
            }
            else if (rbtnClientFGReceiveWOBOM.Checked)
            {
                transactionType = "ClientFGReceiveWOBOM";
            }
            #endregion Transaction Type
        }

        #endregion

        #region Methods 02 / Add Row, Change Row, Remove Row

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                #region Conditional Values

                if (txtPrevious.Text == "")
                {
                    txtPrevious.Text = "0";
                }
                if (txtPurQty.Text == "")
                {
                    txtPurQty.Text = "0";
                }

                #endregion

                #region VATAmount Call

                VATCalculation();

                #endregion

                #region Check Point
                if (!chkVAT.Checked)
                {
                    if (cmbType.SelectedValue == "nonvat")
                    {
                        if (Convert.ToDecimal(txtVATRate.Text.Trim()) > 0)
                        {
                            MessageBox.Show("Please Check the VAT Rate as Zero(0)!", this.Text);
                            return;
                        }
                    }
                }

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Null Check
                if (txtVendorName.Text == "")
                {
                    MessageBox.Show("Please Select Vendor");
                    txtVendorName.Focus();
                    return;
                }
                if (chkNonStock.Checked == true)
                {
                    MessageBox.Show("Nonstock item purchase not required");
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please Select a Item");
                    return;
                }
                if (txtQuantity.Text == "")
                {
                    txtQuantity.Text = "0.00";
                }

                if (txtUnitCost.Text == "")
                {
                    txtUnitCost.Text = "0.00";
                }
                if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }
                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                if (txtSD.Text == "")
                {
                    txtSD.Text = "0.00";
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity");
                    txtQuantity.Focus();
                    return;
                }
                if (!rbtnTollReceiveRaw.Checked)
                {
                    if (Convert.ToDecimal(txtUnitCost.Text) <= 0)
                    {
                        MessageBox.Show("Please input Cost");
                        txtUnitCost.Focus();
                        return;
                    }
                }



                if (cmbType.SelectedValue == "nonvat"
                    || cmbType.SelectedValue == "non vat")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }
                #endregion Null

                #region UOM Call

                UomsValue();

                #endregion

                #region Check Stock

                if (rbtnServiceNS.Checked == false
                    && rbtnServiceNSImport.Checked == false
                    && rbtnInputService.Checked == false
                    && rbtnPurchaseTollcharge.Checked == false
                    && rbtnInputServiceImport.Checked == false)
                {
                    if (rbtnPurchaseReturn.Checked || rbtnPurchaseDN.Checked)
                    {

                    }
                }
                #endregion Check Stock

                #region Duplicate Check

                vMultipleItemInsert = commonDal.settingsDesktop("Purchase", "MultipleItemInsert", null, connVM);

                if (vMultipleItemInsert.ToLower() == "n")
                {
                    for (int i = 0; i < dgvPurchase.RowCount; i++)
                    {
                        if (dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString().ToLower() == txtProductCode.Text.Trim().ToLower())
                        {
                            MessageBox.Show("Same Product already exist.", this.Text);
                            cmbProduct.Focus();
                            return;
                        }
                    }
                }
                #endregion

                #region DutyGrid Load / dgvDuty
                if (
                  rbtnTradingImport.Checked
              || rbtnImport.Checked
              || rbtnServiceImport.Checked
              || rbtnServiceNSImport.Checked
              || rbtnInputServiceImport.Checked
              || rbtnCommercialImporter.Checked
                  )
                {
                    DataGridViewRow DNewRow = new DataGridViewRow();
                    dgvDuty.Rows.Add(DNewRow);

                    int dutyIndex = dgvDuty.RowCount - 1;

                    dgvDuty["ItemNoDuty", dutyIndex].Value = txtProductCode.Text.Trim();
                    if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                        txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), PurchasePlaceQty).ToString();

                    dgvDuty["QuantityDuty", dutyIndex].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim());
                    dgvDuty["ProductNameDuty", dutyIndex].Value = txtProductName.Text.Trim();
                    dgvDuty["ProductCodeDuty", dutyIndex].Value = txtPCode.Text.Trim();
                    dgvDuty["CnFInp", dutyIndex].Value = "0";// txtCnFInpValue.Text.Trim();
                    dgvDuty["CnFRate", dutyIndex].Value = "0";// txtCnFRate.Text.Trim();
                    dgvDuty["CnFAmount", dutyIndex].Value = "0";// txtCnFAmount.Text.Trim();
                    dgvDuty["InsuranceInp", dutyIndex].Value = "0";// txtInsInpValue.Text.Trim();
                    dgvDuty["InsuranceRate", dutyIndex].Value = "0";// txtInsRate.Text.Trim();
                    dgvDuty["InsuranceAmount", dutyIndex].Value = "0";// txtInsAmount.Text.Trim();
                    dgvDuty["AssessableInp", dutyIndex].Value = Program.ParseDecimalObject(txtAVInpValue.Text.Trim());
                    dgvDuty["AssessableValue", dutyIndex].Value = "0";// txtAVAmount.Text.Trim();
                    dgvDuty["CDInp", dutyIndex].Value = "0";// txtCDInpValue.Text.Trim();
                    dgvDuty["CDRate", dutyIndex].Value = "0";// txtCDRate.Text.Trim();
                    dgvDuty["CDAmount", dutyIndex].Value = "0";// vImpCD;
                    dgvDuty["RDInp", dutyIndex].Value = "0";// txtRDInpValue.Text.Trim();
                    dgvDuty["RDRate", dutyIndex].Value = "0";// txtRDRate.Text.Trim();
                    dgvDuty["RDAmount", dutyIndex].Value = "0";// txtRDAmount.Text.Trim();
                    dgvDuty["TVBInp", dutyIndex].Value = "0";// txtTVBInpValue.Text.Trim();
                    dgvDuty["TVBRate", dutyIndex].Value = "0";// txtTVBRate.Text.Trim();
                    dgvDuty["TVBAmount", dutyIndex].Value = "0";// txtTVBAmount.Text.Trim();
                    dgvDuty["SDInp", dutyIndex].Value = "0";// txtSDInpValue.Text.Trim();
                    dgvDuty["SDuty", dutyIndex].Value = "0";// txtSDRate.Text.Trim();
                    dgvDuty["SDutyAmount", dutyIndex].Value = "0";// txtSDAmount.Text.Trim();
                    dgvDuty["VATInp", dutyIndex].Value = "0";// txtVATInpValue.Text.Trim();
                    dgvDuty["VATRateDuty", dutyIndex].Value = "0";// txtVATRateI.Text.Trim();
                    dgvDuty["VATAmountDuty", dutyIndex].Value = "0";// txtVATAmount.Text.Trim();
                    dgvDuty["TVAInp", dutyIndex].Value = "0";// txtTVAInpValue.Text.Trim();
                    dgvDuty["TVARate", dutyIndex].Value = "0";// txtTVARate.Text.Trim();
                    dgvDuty["TVAAmount", dutyIndex].Value = "0";// vImpTVA;
                    dgvDuty["ATVInp", dutyIndex].Value = "0";// txtATVInpValue.Text.Trim();
                    dgvDuty["ATVRate", dutyIndex].Value = "0";// txtATVRate.Text.Trim();
                    dgvDuty["ATVAmount", dutyIndex].Value = "0";// vImpATV;
                    dgvDuty["OthersInp", dutyIndex].Value = "0";// txtOthersInpValue.Text.Trim();
                    dgvDuty["OthersRate", dutyIndex].Value = "0";// txtOthersRate.Text.Trim();
                    dgvDuty["OthersAmount", dutyIndex].Value = "0";// vImpOthers;
                    dgvDuty["AITInp", dutyIndex].Value = "0";// txtOthersInpValue.Text.Trim();
                    dgvDuty["AITAmount", dutyIndex].Value = "0";// vImpOthers;
                    dgvDuty["Remarks", dutyIndex].Value = "NA";// txtDutiesRemarks.Text.Trim();

                }

                #endregion Duty

                #region DataGrid Load / dgvPurchase

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvPurchase.Rows.Add(NewRow);

                int Index = dgvPurchase.RowCount - 1;

                SetDataGridView(Index, "New");

                #region Comments - Sep-29-2020


                #region Qty andUOMs

                #endregion Qty andUOMs

                #endregion

                #endregion

                #region Row Calculate

                Rowcalculate();

                #endregion

                #region SelectLastRow

                selectLastRow();

                #endregion

                #region Product Focus

                //cmbProduct.Focus();

                #endregion

                #region Purchase Retrun

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


        private void HSCodeMulti()
        {
            try
            {


                #region HSCode

                DataTable HSCodeResult = new DataTable();

                ProductDAL pDal = new ProductDAL();

                string[] cFields = new string[] { "ItemNo" };
                string[] cValues = new string[] { txtItemNo.Text };

                HSCodeResult = pDal.SearchHSCode(cFields, cValues);

                if (HSCodeResult.Rows.Count > 1)
                {
                    MessageBox.Show(MessageVM.MsgMultipleHscode, "HSCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                #endregion
            }

            #region catch
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "HSCodeMulti", ex.ToString());
            }
            #endregion

        }



        private void SetDataGridView(int paramIndex, string RowType)
        {
            try
            {

                #region DataGrid Load / dgvPurchase

                dgvPurchase["ItemNo", paramIndex].Value = txtProductCode.Text.Trim();
                dgvPurchase["ItemName", paramIndex].Value = txtProductName.Text.Trim();
                dgvPurchase["UOM", paramIndex].Value = cmbUom.Text.Trim();

                dgvPurchase["BOMId", paramIndex].Value = txtBOMId.Text.Trim();
                dgvPurchase["Quantity", paramIndex].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim());
                dgvPurchase["PCode", paramIndex].Value = txtPCode.Text.Trim();

                dgvPurchase["UOMc", paramIndex].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim());
                dgvPurchase["UOMn", paramIndex].Value = txtUOM.Text.Trim();
                dgvPurchase["UOMQty", paramIndex].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim()));

                dgvPurchase["VATRate", paramIndex].Value = Program.ParseDecimalObject(txtVATRate.Text.Trim());
                dgvPurchase["VATAmount", paramIndex].Value = Program.ParseDecimalObject(txtLocalVATAmount.Text.Trim());
                dgvPurchase["SD", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSD.Text.Trim()));
                dgvPurchase["SDAmount", paramIndex].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtLocalSDAmount.Text.Trim()));
                dgvPurchase["Comments", paramIndex].Value = "NA";

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), PurchasePlaceAmt).ToString();

                dgvPurchase["NBRPrice", paramIndex].Value = Program.ParseDecimalObject(txtNBRPrice.Text.Trim());
                dgvPurchase["VDSRate", paramIndex].Value = Program.ParseDecimalObject(txtVDSRate.Text.Trim());
                dgvPurchase["VDSAmount", paramIndex].Value = Program.ParseDecimalObject(txtVDSAmount.Text.Trim());
                dgvPurchase["USDValue", paramIndex].Value = Program.ParseDecimalObject(txtUSDValue.Text.Trim());
                dgvPurchase["TDSSection", paramIndex].Value = txtSection.Text.Trim();
                dgvPurchase["TDSCode", paramIndex].Value = txtCode.Text.Trim();
                dgvPurchase["USDVAT", paramIndex].Value = 0;
                dgvPurchase["Type", paramIndex].Value = cmbType.SelectedValue;
                dgvPurchase["FixedVATRebate", paramIndex].Value = "Y";
                dgvPurchase["IsFixedVAT", paramIndex].Value = Convert.ToString(chkIsFixedOtherVAT.Checked ? "Y" : "N");
                dgvPurchase["FixedVATAmount", paramIndex].Value = Program.ParseDecimalObject(txtLocalVATAmount.Text.Trim()).ToString();

                Program.FormatTextBoxRate(txtRebatePercent, "Rebate Rate");
                dgvPurchase["Rebate", paramIndex].Value = Program.ParseDecimalObject(txtRebatePercent.Text.Trim());

                dgvPurchase["Stock", paramIndex].Value = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim());
                dgvPurchase["Previous", paramIndex].Value = 0;

                dgvPurchase["Status", paramIndex].Value = RowType;//// "New";
                dgvPurchase["Change", paramIndex].Value = 0;
                if (RowType == "New")
                {
                    dgvPurchase["ExpireDate", paramIndex].Value = "2100-01-01";
                    dgvPurchase["CPCName", paramIndex].Value = "-";
                    dgvPurchase["BEItemNo", paramIndex].Value = "-";
                }
                dgvPurchase["HSCode", paramIndex].Value = txtHSCode.Text.Trim();
                dgvPurchase["Section21", paramIndex].Value = "N";



                if (RowType != "New")
                {
                    dgvPurchase.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                    dgvPurchase.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                }

                #endregion

                #region Qty andUOMs

                decimal vSubTotal = 0;
                string strQuantity = "";
                decimal vQuantity = 0;
                string strUOMc = "";
                decimal vUOMc = 0;
                string strSubTotal = string.Empty;
                string strUnitPrice = "";
                decimal vUnitPrice = 0;
                decimal vUOMPrice = 0;

                strQuantity = txtQuantity.Text.Trim();
                if (!string.IsNullOrEmpty(strQuantity))
                    vQuantity = Convert.ToDecimal(strQuantity);

                if (Program.CheckingNumericString(vQuantity.ToString(), "vQuantity") == true)
                    vQuantity = Convert.ToDecimal(Program.FormatingNumeric(vQuantity.ToString(), PurchasePlaceQty));

                strUOMc = txtUomConv.Text.Trim();
                if (!string.IsNullOrEmpty(strUOMc))
                    vUOMc = Convert.ToDecimal(strUOMc);

                if (IsTotalPrice == true)
                {
                    if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                        txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), PurchasePlaceAmt).ToString();

                    dgvPurchase["SubTotal", paramIndex].Value = Program.ParseDecimalObject(txtUnitCost.Text.Trim());

                    strSubTotal = txtUnitCost.Text.Trim();
                    if (!string.IsNullOrEmpty(strSubTotal))
                        vSubTotal = Convert.ToDecimal(strSubTotal);

                    if (Program.CheckingNumericString(vSubTotal.ToString(), "vSubTotal") == true)
                        vSubTotal = Convert.ToDecimal(Program.FormatingNumeric(vSubTotal.ToString(), PurchasePlaceAmt));

                    if (vQuantity > 0)
                        vUnitPrice = vSubTotal / vQuantity;
                    if (Program.CheckingNumericString(vUnitPrice.ToString(), "vUnitPrice") == true)
                        vUnitPrice = Convert.ToDecimal(Program.FormatingNumeric(vUnitPrice.ToString(), PurchasePlaceAmt));

                    dgvPurchase["UnitPrice", paramIndex].Value = Program.ParseDecimalObject(vUnitPrice);

                    if ((vQuantity * vUOMc) > 0)
                        vUOMPrice = vUnitPrice / (vUOMc);
                    if (Program.CheckingNumericString(vUOMPrice.ToString(), "vUOMPrice") == true)
                        vUOMPrice = Convert.ToDecimal(Program.FormatingNumeric(vUOMPrice.ToString(), PurchasePlaceAmt));


                    dgvPurchase["UOMPrice", paramIndex].Value = Program.ParseDecimalObject(vUOMPrice);
                }
                else
                {
                    if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                        txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), PurchasePlaceAmt).ToString();

                    dgvPurchase["UnitPrice", paramIndex].Value = Program.ParseDecimalObject(txtUnitCost.Text.Trim());
                    strUnitPrice = txtUnitCost.Text.Trim();
                    if (!string.IsNullOrEmpty(strUnitPrice))
                        vUnitPrice = Convert.ToDecimal(strUnitPrice);

                    vSubTotal = vUnitPrice * vQuantity;

                    if (Program.CheckingNumericString(vSubTotal.ToString(), "vSubTotal") == true)
                        vSubTotal = Convert.ToDecimal(Program.FormatingNumeric(vSubTotal.ToString(), PurchasePlaceAmt));

                    dgvPurchase["SubTotal", paramIndex].Value = Program.ParseDecimalObject(vSubTotal);
                    if (vUOMc > 0)
                        vUOMPrice = vUnitPrice / vUOMc;
                    dgvPurchase["UOMPrice", paramIndex].Value = Program.ParseDecimalObject(vUOMPrice);

                }

                #endregion Qty andUOMs

                #region Retrun Cal

                ////vSubTotal

                if (rbtnPurchaseReturn.Checked)
                {

                    decimal PreSubtotal = Convert.ToDecimal(txtPreSubTotal.Text);

                    decimal CDAmount = 0, RDAmount = 0, TVBAmount = 0, TVAAmount = 0, ATVAmount = 0, OthersAmount = 0, AITAmount = 0, SDAmount = 0;

                    CDAmount = Convert.ToDecimal(txtCDAmount.Text.Trim());
                    RDAmount = Convert.ToDecimal(txtRDAmount.Text.Trim());
                    TVBAmount = Convert.ToDecimal(txtTVBAmount.Text.Trim());
                    TVAAmount = Convert.ToDecimal(txtTVAAmount.Text.Trim());
                    ATVAmount = Convert.ToDecimal(txtATVAmount.Text.Trim());
                    OthersAmount = Convert.ToDecimal(txtOthersAmount.Text.Trim());
                    AITAmount = Convert.ToDecimal(txtAITAmount.Text.Trim());
                    SDAmount = Convert.ToDecimal(txtSDAmount.Text.Trim());
                    decimal localSDAmt = Convert.ToDecimal(txtLocalSDAmount.Text.Trim());
                    decimal PreUSDVAT = Convert.ToDecimal(txtPreUSDVAT.Text.Trim());
                    decimal PreFixedVATAmount = Convert.ToDecimal(txtPreFixedVATAmount.Text.Trim());

                    dgvPurchase["InvoiceValue", paramIndex].Value = txtInvoiceValue.Text.Trim();
                    dgvPurchase["ExchangeRate", paramIndex].Value = txtExchangeRate.Text.Trim();
                    dgvPurchase["Currency", paramIndex].Value = txtCurrency.Text.Trim();
                    dgvPurchase["CnF", paramIndex].Value = txtCnFAmount.Text.Trim();
                    dgvPurchase["Insurance", paramIndex].Value = txtInsAmount.Text.Trim();
                    dgvPurchase["AV", paramIndex].Value = txtAVAmount.Text.Trim();
                    dgvPurchase["RD", paramIndex].Value = 0;
                    dgvPurchase["TVB", paramIndex].Value = 0;
                    dgvPurchase["TVA", paramIndex].Value = 0;
                    dgvPurchase["ATV", paramIndex].Value = 0;
                    dgvPurchase["Others", paramIndex].Value = 0;
                    dgvPurchase["AIT", paramIndex].Value = 0;
                    dgvPurchase["FixedVATAmount", paramIndex].Value = 0;
                    dgvPurchase["Section21", paramIndex].Value = "N";

                    if (localSDAmt == 0)
                    {
                        if (SDAmount != 0)
                        {
                            dgvPurchase["SDAmount", paramIndex].Value = (vSubTotal * SDAmount) / PreSubtotal;
                        }
                    }
                    if (PreUSDVAT != 0)
                    {
                        dgvPurchase["USDVAT", paramIndex].Value = (vSubTotal * PreUSDVAT) / PreSubtotal;
                    }
                    if (CDAmount != 0)
                    {
                        dgvPurchase["CD", paramIndex].Value = (vSubTotal * CDAmount) / PreSubtotal;
                    }
                    if (RDAmount != 0)
                    {
                        dgvPurchase["RD", paramIndex].Value = (vSubTotal * RDAmount) / PreSubtotal;
                    }
                    if (TVBAmount != 0)
                    {
                        dgvPurchase["TVB", paramIndex].Value = (vSubTotal * TVBAmount) / PreSubtotal;
                    }
                    if (TVAAmount != 0)
                    {
                        dgvPurchase["TVA", paramIndex].Value = (vSubTotal * TVAAmount) / PreSubtotal;
                    }
                    if (ATVAmount != 0)
                    {
                        dgvPurchase["ATV", paramIndex].Value = (vSubTotal * ATVAmount) / PreSubtotal;
                    }
                    if (OthersAmount != 0)
                    {
                        dgvPurchase["Others", paramIndex].Value = (vSubTotal * OthersAmount) / PreSubtotal;
                    }
                    if (AITAmount != 0)
                    {
                        dgvPurchase["AIT", paramIndex].Value = (vSubTotal * AITAmount) / PreSubtotal;
                    }
                    if (PreFixedVATAmount != 0)
                    {
                        dgvPurchase["FixedVATAmount", paramIndex].Value = (vSubTotal * PreFixedVATAmount) / PreSubtotal;
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (dgvPurchase.Rows.Count <= 0)
                {
                    MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity");
                    txtQuantity.Focus();
                    return;
                }
                #endregion

                #region VAT Calculation

                VATCalculation();

                #endregion

                #region Condional Values

                if (cmbType.SelectedValue == "nonvat")
                {
                    string strVATRate = "";
                    strVATRate = txtVATRate.Text.Trim();

                    if (Convert.ToDecimal(strVATRate) > 0)
                    {
                        MessageBox.Show("Please Check the VAT Rate as Zero(0)!", this.Text);
                        return;
                    }
                }

                #region UOM Function

                UomsValue();

                #endregion

                if (chkNonStock.Checked == true)
                {
                    MessageBox.Show("Nonstock item purchase not required");
                    return;
                }

                if (dgvPurchase.Rows.Count <= 0)
                {
                    MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                }

                #endregion

                #region Stock Check
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text);
                purchaseQty = Convert.ToDecimal(txtPurQty.Text);


                if (rbtnServiceNS.Checked == false
                    && rbtnServiceNSImport.Checked == false
                    && rbtnInputService.Checked == false
                    && rbtnPurchaseTollcharge.Checked == false
                    && rbtnInputServiceImport.Checked == false)
                {
                    if (rbtnPurchaseReturn.Checked || rbtnPurchaseDN.Checked)
                    {
                        if (purchaseQty < CurrentValue)
                        {
                            MessageBox.Show(
                                "changes quantity Can't Greater than purchase quantity. \n purchaase quantity = '" +
                                purchaseQty + "'");
                            txtQuantity.Focus();
                            return;
                        }
                        else if (CurrentValue > PreviousValue)
                        {
                            if (
                                Convert.ToDecimal(Convert.ToDecimal(CurrentValue - PreviousValue) *
                                                  Convert.ToDecimal(txtUomConv.Text)) >
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
                        //if (CurrentValue < PreviousValue)
                        //{
                        //    if (
                        //        Convert.ToDecimal(Convert.ToDecimal(PreviousValue - CurrentValue) *
                        //                          Convert.ToDecimal(txtUomConv.Text)) >
                        //        Convert.ToDecimal(StockValue))
                        //    {
                        //        MessageBox.Show("Stock Not available");
                        //        txtQuantity.Focus();
                        //        return;
                        //    }

                        //}

                    }
                }
                #endregion Stock Chekc

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Conditional Values

                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }

                if (cmbType.SelectedValue == "nonvat" || cmbType.SelectedValue == "non vat")
                {
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";
                }

                #endregion
                #region Multiple HSCode Check

                HSCodeMulti();
                #endregion

                #region Value Assign on dgvPurchase

                int Index = dgvPurchase.CurrentRow.Index;

                SetDataGridView(Index, "Change");

                #region Comments Sep-29-2020


                #region Qty andUOMs


                #endregion Qty andUOMs

                #endregion

                #endregion

                #region Row Calculation

                Rowcalculate();

                #endregion

                #region Reset Form

                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "";
                txtQuantity.Text = "";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtSD.Text = "0.00";
                txtQuantityInHand.Text = "00";
                txtBOMId.Text = "";

                #endregion

                #region Product Focus

                //cmbProduct.Focus();

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
                if (dgvPurchase.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvPurchase.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //selectLastRow();
                        //dgvDuty.CurrentCell = dgvDuty.Rows[dgvPurchase.Rows.Count - 1].Cells[1];
                        //if (dgvDuty.Rows.Count > 0)
                        //{
                        //    dgvDuty.CurrentCell = dgvDuty.Rows[dgvPurchase.CurrentRow.Index].Cells[1];
                        //}
                        var index = dgvPurchase.CurrentRow.Index;
                        dgvPurchase.Rows.RemoveAt(dgvPurchase.CurrentRow.Index);
                        //////if (transactionType.ToLower().Contains("import"))
                        {

                            if (dgvDuty.Rows.Count > 0)
                            {
                                dgvDuty.Rows.RemoveAt(index);
                            }
                        }

                        SubtotalCalculation();

                        //////Rowcalculate();
                    }
                    if (dgvDuty.Rows.Count == 0)
                    {
                        ClearDutyFields();
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
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

        private void ClearDutyFields()
        {
            //Duties:
            txtTotalSubTotal1.Text = "0.00";
            txtCnFInpValue.Text = "0.00";
            txtCnFRate.Text = "0.00";
            txtCnFAmount.Text = "0.00";
            txtInsInpValue.Text = "0.00";
            txtInsRate.Text = "0.00";
            txtInsAmount.Text = "0.00";
            txtAVInpValue.Text = "0.00";
            txtAVAmount.Text = "0.00";
            txtCDInpValue.Text = "0.00";
            txtCDRate.Text = "0.00";
            txtCDAmount.Text = "0.00";
            txtRDInpValue.Text = "0.00";
            txtRDRate.Text = "0.00";
            txtRDAmount.Text = "0.00";
            txtTVBInpValue.Text = "0.00";
            txtTVBRate.Text = "0.00";
            txtTVBAmount.Text = "0.00";
            txtSDInpValue.Text = "0.00";
            txtSDRate.Text = "0.00";
            txtSDAmount.Text = "0.00";
            txtVATInpValue.Text = "0.00";
            txtVATRateI.Text = "0.00";
            txtVATAmount.Text = "0.00";
            txtTVAInpValue.Text = "0.00";
            txtTVARate.Text = "0.00";
            txtTVAAmount.Text = "0.00";
            txtATVInpValue.Text = "0.00";
            txtATVRate.Text = "0.00";
            txtATVAmount.Text = "0.00";
            txtOthersInpValue.Text = "0.00";
            txtOthersRate.Text = "0.00";
            txtOthersAmount.Text = "0.00";
            txtDutiesRemarks.Text = "";

            ////dgvDuty.Rows.Clear();
        }

        private void DutyAssign()
        {
            #region Variables
            decimal vImpSubTotal = 0;
            decimal vImpVAT = 0;
            decimal vImpCnF = 0;
            decimal vImpInsurance = 0;
            decimal vImpAV = 0;
            decimal vImpCD = 0;
            decimal vImpRD = 0;
            decimal vImpTVB = 0;
            decimal vImpTVA = 0;
            decimal vImpATV = 0;
            decimal vImpOthers = 0;
            decimal vImpSD = 0;
            decimal vImpTotalPrice;
            decimal SubTotal1 = 0;
            decimal SubTotal = 0;
            decimal VDSAmount = 0;

            decimal SD = 0;
            decimal SDAmount = 0;
            decimal VATRate = 0;
            decimal VATAmount = 0;
            decimal Total = 0;

            decimal SumSDAmount = 0;
            decimal SumVATAmount = 0;
            decimal SumSubTotal = 0;
            decimal SumSubTotal2 = 0;
            decimal SumTotal = 0;
            decimal vtotalSubTotal1 = 1;

            #endregion 0

            try
            {

                #region Value Assign to Purchase Data Grid View

                int Index = dgvPurchase.CurrentRow.Index;

                dgvPurchase[0, Index].Value = Index + 1;
                dgvPurchase["VATableValue", Index].Value = "0";

                string strSubTotal = "";
                decimal vQuantity = 0;

                #region null check of Duties

                if (dgvPurchase["CnF", Index].Value == null)
                {
                    dgvPurchase["CnF", Index].Value = "0";
                }
                if (dgvPurchase["Insurance", Index].Value == null)
                {
                    dgvPurchase["Insurance", Index].Value = "0";
                }
                if (dgvPurchase["AV", Index].Value == null)
                {
                    dgvPurchase["AV", Index].Value = "0";
                }
                if (dgvPurchase["CD", Index].Value == null)
                {
                    dgvPurchase["CD", Index].Value = "0";
                }
                if (dgvPurchase["RD", Index].Value == null)
                {
                    dgvPurchase["RD", Index].Value = "0";
                }
                if (dgvPurchase["TVB", Index].Value == null)
                {
                    dgvPurchase["TVB", Index].Value = "0";
                }
                if (dgvPurchase["TVA", Index].Value == null)
                {
                    dgvPurchase["TVA", Index].Value = "0";
                }
                if (dgvPurchase["ATV", Index].Value == null)
                {
                    dgvPurchase["ATV", Index].Value = "0";
                }
                if (dgvPurchase["Others", Index].Value == null)
                {
                    dgvPurchase["Others", Index].Value = "0";
                }
                if (dgvPurchase["AIT", Index].Value == null)
                {
                    dgvPurchase["AIT", Index].Value = "0";
                }
                if (dgvPurchase["InvoiceValue", Index].Value == null)
                {
                    dgvPurchase["InvoiceValue", Index].Value = "0";
                }
                if (dgvPurchase["ExchangeRate", Index].Value == null)
                {
                    dgvPurchase["ExchangeRate", Index].Value = "0";
                }
                if (dgvPurchase["Currency", Index].Value == null)
                {
                    dgvPurchase["Currency", Index].Value = "";
                }

                #endregion

                #region Value Assign

                SubTotal = Convert.ToDecimal(dgvPurchase["SubTotal", Index].Value);
                if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                    SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                if (boolsearch == false)
                {//imp start

                    #region Import Condition

                    if ((rbtnImport.Checked == true && dutyCalculate == true)
                        || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                        || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                         || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                         || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                        || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                    {
                        dgvDuty["SubTotalDuty", Index].Value = dgvPurchase["SubTotal", Index].Value.ToString();
                        dgvDuty[0, Index].Value = Index + 1;

                        #region Import Values

                        if (dgvPurchase["SubTotal", Index].Value != null)
                        {
                            strSubTotal = dgvPurchase["SubTotal", Index].Value.ToString();

                            if (!string.IsNullOrEmpty(strSubTotal))
                                vImpSubTotal = Convert.ToDecimal(strSubTotal);

                            if (Program.CheckingNumericString(vImpSubTotal.ToString(), "vImpSubTotal") == true)
                                vImpSubTotal =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpSubTotal.ToString(),
                                                                               PurchasePlaceAmt));

                        }
                        if (ChkSubTotalAll.Checked == false)
                        {
                            vImpSubTotal = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                        }

                        #region AV Amount

                        #region Comments 16 Feb 2021

                        ////if (CalculativeAV == false)
                        ////{
                        ////    if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                        ////        txtAVAmount.Text =
                        ////            Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////    if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                        ////    {
                        ////        vImpAV = Convert.ToDecimal(txtAVAmount.Text.Trim());
                        ////    }
                        ////    else
                        ////    {
                        ////        vImpAV = 0;
                        ////        vImpCnF = 0;
                        ////        vImpInsurance = 0;
                        ////    }


                        ////}
                        ////else
                        ////{
                        ////    ////////if (Program.CheckingNumericTextBox(txtCnFRate, "txtCnFRate") == true)
                        ////    ////////txtCnFRate.Text = Program.FormatingNumeric(txtCnFRate.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////    if (!string.IsNullOrEmpty(txtCnFRate.Text.Trim()))
                        ////    {
                        ////        vImpCnF = vImpSubTotal * Convert.ToDecimal(txtCnFRate.Text.Trim()) / 100;
                        ////    }
                        ////    else
                        ////    {
                        ////        vImpCnF = 0;
                        ////    }
                        ////    if (Program.CheckingNumericString(vImpCnF.ToString(), "vImpCnF") == true)
                        ////        vImpCnF =
                        ////            Convert.ToDecimal(Program.FormatingNumeric(vImpCnF.ToString(), PurchasePlaceAmt));

                        ////    //////            if (Program.CheckingNumericTextBox(txtInsRate, "txtInsRate") == true)
                        ////    //////txtInsRate.Text = Program.FormatingNumeric(txtInsRate.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////    if (!string.IsNullOrEmpty(txtInsRate.Text.Trim()))
                        ////    {
                        ////        vImpInsurance = (vImpSubTotal + vImpCnF) * Convert.ToDecimal(txtInsRate.Text.Trim()) /
                        ////                        100;
                        ////    }
                        ////    else
                        ////    {
                        ////        vImpInsurance = 0;
                        ////    }
                        ////    if (Program.CheckingNumericString(vImpInsurance.ToString(), "vImpInsurance") == true)
                        ////        vImpInsurance =
                        ////            Convert.ToDecimal(Program.FormatingNumeric(vImpInsurance.ToString(),
                        ////                                                       PurchasePlaceAmt));

                        ////    vImpAV = vImpSubTotal + vImpCnF + vImpInsurance;

                        ////}

                        #endregion

                        #region Formating Numeric

                        if (Program.CheckingNumericString(vImpAV.ToString(), "vImpAV") == true)
                            vImpAV = Convert.ToDecimal(Program.FormatingNumeric(vImpAV.ToString(), PurchasePlaceAmt));

                        if (Program.CheckingNumericTextBox(txtCnFAmount, "txtCnFAmount") == true)
                            txtCnFAmount.Text =
                                Program.FormatingNumeric(txtCnFAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                            txtInsAmount.Text =
                                Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                            txtAVAmount.Text =
                                Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtCnFInpValue, "txtCnFInpValue") == true)
                            txtCnFInpValue.Text =
                                Program.FormatingNumeric(txtCnFInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        //////if (Program.CheckingNumericTextBox(txtCnFRate, "txtCnFRate") == true)
                        //////    txtCnFRate.Text = Program.FormatingNumeric(txtCnFRate.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtInsInpValue, "txtInsInpValue") == true)
                            txtInsInpValue.Text =
                                Program.FormatingNumeric(txtInsInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////////if (Program.CheckingNumericTextBox(txtInsRate, "txtInsRate") == true)
                        ////////    txtInsRate.Text = Program.FormatingNumeric(txtInsRate.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                            txtInsAmount.Text =
                                Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtAVInpValue, "txtAVInpValue") == true)
                            txtAVInpValue.Text =
                                Program.FormatingNumeric(txtAVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                            txtNBRPrice.Text =
                                Program.FormatingNumeric(txtNBRPrice.Text.Trim(), PurchasePlaceAmt).ToString();
                        #endregion

                        #region 15 Feb 2021

                        dgvPurchase["CnF", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                        dgvPurchase["Insurance", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                        dgvPurchase["AV", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();

                        dgvDuty["CnFInp", dgvPurchase.CurrentRow.Index].Value = txtCnFInpValue.Text.Trim();
                        dgvDuty["CnFRate", dgvPurchase.CurrentRow.Index].Value = txtCnFRate.Text.Trim();
                        dgvDuty["CnFAmount", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                        dgvDuty["InsuranceInp", dgvPurchase.CurrentRow.Index].Value = txtInsInpValue.Text.Trim();
                        dgvDuty["InsuranceRate", dgvPurchase.CurrentRow.Index].Value = txtInsRate.Text.Trim();
                        dgvDuty["InsuranceAmount", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                        dgvDuty["AssessableInp", dgvPurchase.CurrentRow.Index].Value = txtAVInpValue.Text.Trim();
                        dgvDuty["AssessableValue", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();

                        #endregion

                        #region Comments 15 Feb 2021

                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{

                        ////    dgvPurchase["CnF", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                        ////    dgvPurchase["Insurance", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                        ////    dgvPurchase["AV", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();

                        ////    dgvDuty["CnFInp", dgvPurchase.CurrentRow.Index].Value = txtCnFInpValue.Text.Trim();
                        ////    dgvDuty["CnFRate", dgvPurchase.CurrentRow.Index].Value = txtCnFRate.Text.Trim();
                        ////    dgvDuty["CnFAmount", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                        ////    dgvDuty["InsuranceInp", dgvPurchase.CurrentRow.Index].Value = txtInsInpValue.Text.Trim();
                        ////    dgvDuty["InsuranceRate", dgvPurchase.CurrentRow.Index].Value = txtInsRate.Text.Trim();
                        ////    dgvDuty["InsuranceAmount", dgvPurchase.CurrentRow.Index].Value =
                        ////        txtInsAmount.Text.Trim();
                        ////    dgvDuty["AssessableInp", dgvPurchase.CurrentRow.Index].Value = txtAVInpValue.Text.Trim();
                        ////    dgvDuty["AssessableValue", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();
                        ////}
                        ////else
                        ////{
                        ////    vImpAV = vImpAV * SubTotal / vtotalSubTotal1;
                        ////    vImpInsurance = vImpInsurance * SubTotal / vtotalSubTotal1;
                        ////    vImpCnF = vImpCnF * SubTotal / vtotalSubTotal1;

                        ////    dgvPurchase["CnF", Index].Value = vImpCnF;
                        ////    dgvPurchase["Insurance", Index].Value = vImpInsurance;
                        ////    dgvPurchase["AV", Index].Value = vImpAV;

                        ////    dgvDuty["CnFInp", Index].Value = txtCnFInpValue.Text.Trim();
                        ////    dgvDuty["CnFRate", Index].Value = txtCnFRate.Text.Trim();
                        ////    dgvDuty["CnFAmount", Index].Value = vImpCnF;
                        ////    dgvDuty["InsuranceInp", Index].Value = txtInsInpValue.Text.Trim();
                        ////    dgvDuty["InsuranceRate", Index].Value = txtInsRate.Text.Trim();
                        ////    dgvDuty["InsuranceAmount", Index].Value = vImpInsurance;
                        ////    dgvDuty["AssessableInp", Index].Value = txtAVInpValue.Text.Trim();
                        ////    dgvDuty["AssessableValue", Index].Value = vImpAV;
                        ////}

                        #endregion

                        #endregion AV

                        #region CD Amount

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                            txtCDAmount.Text =
                                Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtCDInpValue, "txtCDInpValue") == true)
                            txtCDInpValue.Text =
                                Program.FormatingNumeric(txtCDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["CD", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                        dgvDuty["CDInp", dgvPurchase.CurrentRow.Index].Value = txtCDInpValue.Text.Trim();
                        dgvDuty["CDRate", dgvPurchase.CurrentRow.Index].Value = txtCDRate.Text.Trim();
                        dgvDuty["CDAmount", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        ////if (!string.IsNullOrEmpty(txtCDRate.Text.Trim()))
                        ////{
                        ////    vImpCD = vImpAV * Convert.ToDecimal(txtCDRate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpCD = vImpAV * 0 / 100;
                        ////}

                        ////if (Program.CheckingNumericString(vImpCD.ToString(), "vImpCD") == true)
                        ////    vImpCD = Convert.ToDecimal(Program.FormatingNumeric(vImpCD.ToString(), PurchasePlaceAmt));

                        ////if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                        ////    txtCDAmount.Text =
                        ////        Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtCDInpValue, "txtCDInpValue") == true)
                        ////    txtCDInpValue.Text =
                        ////        Program.FormatingNumeric(txtCDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////////////if (Program.CheckingNumericTextBox(txtCDRate, "txtCDRate") == true)
                        ////////////    txtCDRate.Text = Program.FormatingNumeric(txtCDRate.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////////////if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                        ////////////    txtCDAmount.Text = Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["CD", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                        ////    dgvDuty["CDInp", dgvPurchase.CurrentRow.Index].Value = txtCDInpValue.Text.Trim();
                        ////    dgvDuty["CDRate", dgvPurchase.CurrentRow.Index].Value = txtCDRate.Text.Trim();
                        ////    dgvDuty["CDAmount", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                        ////}
                        ////else
                        ////{
                        ////    //////vImpCD = vImpCD * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["CD", Index].Value = txtCDAmount.Text.Trim();
                        ////    dgvDuty["CDInp", Index].Value = txtCDInpValue.Text.Trim();
                        ////    dgvDuty["CDRate", Index].Value = txtCDRate.Text.Trim();
                        ////    dgvDuty["CDAmount", Index].Value = txtCDAmount.Text.Trim();
                        ////}

                        #endregion

                        #endregion CD

                        #region RD Amount

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                            txtRDAmount.Text =
                                Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtRDInpValue, "txtRDInpValue") == true)
                            txtRDInpValue.Text =
                                Program.FormatingNumeric(txtRDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["RD", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                        dgvDuty["RDInp", dgvPurchase.CurrentRow.Index].Value = txtRDInpValue.Text.Trim();
                        dgvDuty["RDRate", dgvPurchase.CurrentRow.Index].Value = txtRDRate.Text.Trim();
                        dgvDuty["RDAmount", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        ////////////if (Program.CheckingNumericTextBox(txtRDRate, "txtRDRate") == true)
                        ////////////    txtRDRate.Text = Program.FormatingNumeric(txtRDRate.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (!string.IsNullOrEmpty(txtRDRate.Text.Trim()))
                        ////{
                        ////    vImpRD = vImpAV * Convert.ToDecimal(txtRDRate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpRD = vImpAV * 0 / 100;
                        ////}

                        ////if (Program.CheckingNumericString(vImpRD.ToString(), "vImpRD") == true)
                        ////    vImpRD = Convert.ToDecimal(Program.FormatingNumeric(vImpRD.ToString(), PurchasePlaceAmt));

                        ////if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                        ////    txtRDAmount.Text =
                        ////        Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtRDInpValue, "txtRDInpValue") == true)
                        ////    txtRDInpValue.Text =
                        ////        Program.FormatingNumeric(txtRDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        //////////if (Program.CheckingNumericTextBox(txtRDRate, "txtRDRate") == true)
                        //////////    txtRDRate.Text = Program.FormatingNumeric(txtRDRate.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                        ////    txtRDAmount.Text =
                        ////        Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["RD", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                        ////    dgvDuty["RDInp", dgvPurchase.CurrentRow.Index].Value = txtRDInpValue.Text.Trim();
                        ////    dgvDuty["RDRate", dgvPurchase.CurrentRow.Index].Value = txtRDRate.Text.Trim();
                        ////    dgvDuty["RDAmount", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();

                        ////}
                        ////else
                        ////{
                        ////    //////vImpRD = vImpRD * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["RD", Index].Value = txtRDAmount.Text.Trim();
                        ////    dgvDuty["RDInp", Index].Value = txtRDInpValue.Text.Trim();
                        ////    dgvDuty["RDRate", Index].Value = txtRDRate.Text.Trim();
                        ////    dgvDuty["RDAmount", Index].Value = txtRDAmount.Text.Trim();
                        ////}

                        #endregion

                        #endregion RD

                        #region TVB Amount

                        #region 16 Feb 2021

                        dgvPurchase["TVB", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                        dgvDuty["TVBInp", dgvPurchase.CurrentRow.Index].Value = txtTVBInpValue.Text.Trim();
                        dgvDuty["TVBRate", dgvPurchase.CurrentRow.Index].Value = txtTVBRate.Text.Trim();
                        dgvDuty["TVBAmount", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        ////if (!string.IsNullOrEmpty(txtTVBRate.Text.Trim()))
                        ////{
                        ////    vImpTVB = (vImpAV + vImpCD + vImpRD) * Convert.ToDecimal(txtTVBRate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpTVB = (vImpAV + vImpCD + vImpRD) * 0 / 100;
                        ////}
                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["TVB", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                        ////    dgvDuty["TVBInp", dgvPurchase.CurrentRow.Index].Value = txtTVBInpValue.Text.Trim();
                        ////    dgvDuty["TVBRate", dgvPurchase.CurrentRow.Index].Value = txtTVBRate.Text.Trim();
                        ////    dgvDuty["TVBAmount", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();

                        ////}
                        ////else
                        ////{
                        ////    //////vImpTVB = vImpTVB * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["TVB", Index].Value = vImpTVB;
                        ////    dgvDuty["TVBInp", Index].Value = txtTVBInpValue.Text.Trim();
                        ////    dgvDuty["TVBRate", Index].Value = txtTVBRate.Text.Trim();
                        ////    dgvDuty["TVBAmount", Index].Value = vImpTVB;
                        ////}

                        #endregion

                        #endregion TVB

                        #region SD Amount

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtSDInpValue, "txtSDInpValue") == true)
                            txtSDInpValue.Text = Program.FormatingNumeric(txtSDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                            txtSDAmount.Text = Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["SDAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();
                        dgvPurchase["SD", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                        dgvDuty["SDInp", dgvPurchase.CurrentRow.Index].Value = txtSDInpValue.Text.Trim();
                        dgvDuty["SDuty", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                        dgvDuty["SDutyAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        //////if (!string.IsNullOrEmpty(txtSDRate.Text.Trim()))
                        //////{
                        //////    vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * Convert.ToDecimal(txtSDRate.Text.Trim()) / 100;
                        //////}
                        //////else
                        //////{
                        //////    vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * 0 / 100;
                        //////}
                        //////if (Program.CheckingNumericString(vImpSD.ToString(), "vImpSD") == true)
                        //////    vImpSD = Convert.ToDecimal(Program.FormatingNumeric(vImpSD.ToString(), PurchasePlaceAmt));

                        //////if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                        //////    txtSDAmount.Text = Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        //////if (Program.CheckingNumericTextBox(txtSDInpValue, "txtSDInpValue") == true)
                        //////    txtSDInpValue.Text = Program.FormatingNumeric(txtSDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        //////if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                        //////    txtSDAmount.Text = Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        //////if (ChkSubTotalAll.Checked) // Single
                        //////{
                        //////    dgvPurchase["SDAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();
                        //////    dgvPurchase["SD", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                        //////    dgvDuty["SDInp", dgvPurchase.CurrentRow.Index].Value = txtSDInpValue.Text.Trim();
                        //////    dgvDuty["SDuty", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                        //////    dgvDuty["SDutyAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();

                        //////}
                        //////else
                        //////{
                        //////    //////vImpSD = vImpSD * SubTotal / vtotalSubTotal1;
                        //////    dgvPurchase["SDAmount", Index].Value = txtSDAmount.Text.Trim();
                        //////    dgvPurchase["SD", Index].Value = txtSDRate.Text.Trim();
                        //////    dgvDuty["SDInp", Index].Value = txtSDInpValue.Text.Trim();
                        //////    dgvDuty["SDuty", Index].Value = txtSDRate.Text.Trim();
                        //////    dgvDuty["SDutyAmount", Index].Value = txtSDAmount.Text.Trim();

                        //////}

                        #endregion

                        #endregion SD

                        #region VAT Amount

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtVATInpValue, "txtVATInpValue") == true)
                            txtVATInpValue.Text = Program.FormatingNumeric(txtVATInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                            txtVATAmount.Text = Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                        decimal varVATAmount = Convert.ToDecimal(txtVATAmount.Text);
                        decimal varVATRate = 0;

                        if (!string.IsNullOrWhiteSpace(txtVATRateI.Text))
                        {
                            varVATRate = Convert.ToDecimal(txtVATRateI.Text.Trim());
                        }

                        //if (varAssesableValue > 0)
                        //{
                        //    varVATRate = (varVATAmount / varAssesableValue) * 100;
                        //}

                        dgvPurchase["VATAmount", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                        dgvPurchase["VATRate", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                        dgvDuty["VATInp", dgvPurchase.CurrentRow.Index].Value = txtVATInpValue.Text.Trim();
                        dgvDuty["VATRateDuty", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                        dgvDuty["VATAmountDuty", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        ////////////if (Program.CheckingNumericTextBox(txtVATRateI, "txtVATRateI") == true)
                        ////////////txtVATRateI.Text = Program.FormatingNumeric(txtVATRateI.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (!string.IsNullOrEmpty(txtVATRateI.Text.Trim()))
                        ////    vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                        ////              Convert.ToDecimal(txtVATRateI.Text.Trim()) / 100;
                        ////else
                        ////    vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;

                        ////if (Program.CheckingNumericString(vImpVAT.ToString(), "vImpVAT") == true)
                        ////    vImpVAT =
                        ////        Convert.ToDecimal(Program.FormatingNumeric(vImpVAT.ToString(), PurchasePlaceAmt));


                        ////if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                        ////    txtVATAmount.Text =
                        ////        Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtVATInpValue, "txtVATInpValue") == true)
                        ////    txtVATInpValue.Text =
                        ////        Program.FormatingNumeric(txtVATInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                        ////    txtVATAmount.Text =
                        ////        Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                        ////    decimal varVATAmount = Convert.ToDecimal(txtVATAmount.Text);
                        ////    decimal varVATRate = 0;

                        ////    if (varAssesableValue > 0)
                        ////    {
                        ////        varVATRate = (varVATAmount / varAssesableValue) * 100;
                        ////    }


                        ////    dgvPurchase["VATAmount", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                        ////    dgvPurchase["VATRate", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                        ////    dgvDuty["VATInp", dgvPurchase.CurrentRow.Index].Value = txtVATInpValue.Text.Trim();
                        ////    dgvDuty["VATRateDuty", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                        ////    dgvDuty["VATAmountDuty", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();

                        ////}
                        ////else
                        ////{
                        ////    decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                        ////    decimal varVATAmount = Convert.ToDecimal(txtVATAmount.Text);
                        ////    decimal varVATRate = 0;

                        ////    if (varAssesableValue > 0)
                        ////    {
                        ////        varVATRate = (varVATAmount / varAssesableValue) * 100;
                        ////    }


                        ////    //////vImpVAT = vImpVAT * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["VATAmount", Index].Value = txtVATAmount.Text.Trim();
                        ////    dgvPurchase["VATRate", Index].Value = varVATRate;
                        ////    dgvDuty["VATInp", Index].Value = txtVATInpValue.Text.Trim();
                        ////    dgvDuty["VATRateDuty", Index].Value = varVATRate;
                        ////    dgvDuty["VATAmountDuty", Index].Value = txtVATAmount.Text.Trim();

                        ////}

                        #endregion

                        decimal bdtTotalVATAmount, usdInvoiceValue, vatperDollar, usdValue, usdVAT = 0;
                        bdtTotalVATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                        usdInvoiceValue = Convert.ToDecimal(txtUSDInvoiceValue.Text.Trim());
                        vatperDollar = bdtTotalVATAmount / usdInvoiceValue;
                        usdValue = Convert.ToDecimal(dgvPurchase["USDValue", Index].Value);
                        usdVAT = usdValue * vatperDollar;
                        dgvPurchase["USDVAT", Index].Value = usdVAT;
                        dgvPurchase["VATableValue", Index].Value = Convert.ToDecimal(dgvPurchase["AV", Index].Value)
                            + Convert.ToDecimal(dgvPurchase["CD", Index].Value)
                            + Convert.ToDecimal(dgvPurchase["RD", Index].Value)
                            + Convert.ToDecimal(dgvPurchase["TVB", Index].Value)
                            + Convert.ToDecimal(dgvPurchase["SDAmount", Index].Value)

                            ;

                        #endregion VAT

                        #region TVA

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                            txtTVAAmount.Text = Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtTVAInpValue, "txtTVAInpValue") == true)
                            txtTVAInpValue.Text = Program.FormatingNumeric(txtTVAInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["TVA", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                        dgvDuty["TVAInp", dgvPurchase.CurrentRow.Index].Value = txtTVAInpValue.Text.Trim();
                        dgvDuty["TVARate", dgvPurchase.CurrentRow.Index].Value = txtTVARate.Text.Trim();
                        dgvDuty["TVAAmount", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021


                        //////////if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                        //////////    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (!string.IsNullOrEmpty(txtTVARate.Text.Trim()))
                        ////{
                        ////    vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                        ////              Convert.ToDecimal(txtTVARate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                        ////}

                        ////if (Program.CheckingNumericString(vImpTVA.ToString(), "vImpTVA") == true)
                        ////    vImpTVA =
                        ////        Convert.ToDecimal(Program.FormatingNumeric(vImpTVA.ToString(), PurchasePlaceAmt));

                        ////if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                        ////    txtTVAAmount.Text =
                        ////        Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtTVAInpValue, "txtTVAInpValue") == true)
                        ////    txtTVAInpValue.Text =
                        ////        Program.FormatingNumeric(txtTVAInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                        //////////if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                        //////////    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                        ////    txtTVAAmount.Text =
                        ////        Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();


                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["TVA", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                        ////    dgvDuty["TVAInp", dgvPurchase.CurrentRow.Index].Value = txtTVAInpValue.Text.Trim();
                        ////    dgvDuty["TVARate", dgvPurchase.CurrentRow.Index].Value = txtTVARate.Text.Trim();
                        ////    dgvDuty["TVAAmount", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();

                        ////}
                        ////else
                        ////{
                        ////    //////vImpTVA = vImpTVA * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["TVA", Index].Value = vImpTVA;
                        ////    dgvDuty["TVAInp", Index].Value = txtTVAInpValue.Text.Trim();
                        ////    dgvDuty["TVARate", Index].Value = txtTVARate.Text.Trim();
                        ////    dgvDuty["TVAAmount", Index].Value = vImpTVA;
                        ////}

                        #endregion

                        #endregion TVA

                        #region ATV

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                            txtATVAmount.Text =
                                Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        if (Program.CheckingNumericTextBox(txtATVInpValue, "txtATVInpValue") == true)
                            txtATVInpValue.Text =
                                Program.FormatingNumeric(txtATVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["ATV", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                        dgvDuty["ATVInp", dgvPurchase.CurrentRow.Index].Value = txtATVInpValue.Text.Trim();
                        dgvDuty["ATVRate", dgvPurchase.CurrentRow.Index].Value = txtATVRate.Text.Trim();
                        dgvDuty["ATVAmount", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();

                        #endregion

                        #region Comments 16 Feb 2021

                        //////////if (Program.CheckingNumericTextBox(txtATVRate, "txtATVRate") == true)
                        //////////    txtATVRate.Text = Program.FormatingNumeric(txtATVRate.Text.Trim(), PurchasePlaceAmt).ToString();


                        ////if (!string.IsNullOrEmpty(txtATVRate.Text.Trim()))
                        ////{
                        ////    vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) *
                        ////              Convert.ToDecimal(txtATVRate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) * 0 / 100;
                        ////}

                        ////if (Program.CheckingNumericString(vImpATV.ToString(), "vImpATV") == true)
                        ////    vImpATV =
                        ////        Convert.ToDecimal(Program.FormatingNumeric(vImpATV.ToString(), PurchasePlaceAmt));

                        ////if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                        ////    txtATVAmount.Text =
                        ////        Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtATVInpValue, "txtATVInpValue") == true)
                        ////    txtATVInpValue.Text =
                        ////        Program.FormatingNumeric(txtATVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////////////if (Program.CheckingNumericTextBox(txtATVRate, "txtATVRate") == true)
                        ////////////    txtATVRate.Text = Program.FormatingNumeric(txtATVRate.Text.Trim(), PurchasePlaceAmt).ToString();
                        ////if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                        ////    txtATVAmount.Text =
                        ////        Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["ATV", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                        ////    dgvDuty["ATVInp", dgvPurchase.CurrentRow.Index].Value = txtATVInpValue.Text.Trim();
                        ////    dgvDuty["ATVRate", dgvPurchase.CurrentRow.Index].Value = txtATVRate.Text.Trim();
                        ////    dgvDuty["ATVAmount", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                        ////}
                        ////else
                        ////{
                        ////    //////vImpATV = vImpATV * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["ATV", Index].Value = txtATVAmount.Text.Trim();
                        ////    dgvDuty["ATVInp", Index].Value = txtATVInpValue.Text.Trim();
                        ////    dgvDuty["ATVRate", Index].Value = txtATVRate.Text.Trim();
                        ////    dgvDuty["ATVAmount", Index].Value = txtATVAmount.Text.Trim();
                        ////}

                        #endregion

                        #endregion ATV

                        #region Others

                        #region 16 Feb 2021

                        if (Program.CheckingNumericTextBox(txtOthersInpValue, "txtOthersInpValue") == true)
                            txtOthersInpValue.Text = Program.FormatingNumeric(txtOthersInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        if (Program.CheckingNumericTextBox(txtOthersAmount, "txtOthersAmount") == true)
                            txtOthersAmount.Text = Program.FormatingNumeric(txtOthersAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        dgvPurchase["Others", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                        dgvDuty["OthersInp", dgvPurchase.CurrentRow.Index].Value = txtOthersInpValue.Text.Trim();
                        dgvDuty["OthersRate", dgvPurchase.CurrentRow.Index].Value = txtOthersRate.Text.Trim();
                        dgvDuty["OthersAmount", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();

                        #endregion

                        #region  Comments 16 Feb 2021

                        ////if (!string.IsNullOrEmpty(txtOthersRate.Text.Trim()))
                        ////{
                        ////    vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                        ////                 Convert.ToDecimal(txtOthersRate.Text.Trim()) / 100;
                        ////}
                        ////else
                        ////{
                        ////    vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                        ////}


                        ////if (Program.CheckingNumericString(vImpOthers.ToString(), "vImpOthers") == true)
                        ////    vImpOthers =
                        ////        Convert.ToDecimal(Program.FormatingNumeric(vImpOthers.ToString(), PurchasePlaceAmt));

                        ////if (Program.CheckingNumericTextBox(txtOthersInpValue, "txtOthersInpValue") == true)
                        ////    txtOthersInpValue.Text = Program.FormatingNumeric(txtOthersInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                        //////////if (Program.CheckingNumericTextBox(txtOthersRate, "txtOthersRate") == true)
                        //////////    txtOthersRate.Text = Program.FormatingNumeric(txtOthersRate.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (Program.CheckingNumericTextBox(txtOthersAmount, "txtOthersAmount") == true)
                        ////    txtOthersAmount.Text = Program.FormatingNumeric(txtOthersAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                        ////if (ChkSubTotalAll.Checked) // Single
                        ////{
                        ////    dgvPurchase["Others", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                        ////    dgvDuty["OthersInp", dgvPurchase.CurrentRow.Index].Value = txtOthersInpValue.Text.Trim();
                        ////    dgvDuty["OthersRate", dgvPurchase.CurrentRow.Index].Value = txtOthersRate.Text.Trim();
                        ////    dgvDuty["OthersAmount", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();

                        ////}
                        ////else
                        ////{
                        ////    //////vImpOthers = vImpOthers * SubTotal / vtotalSubTotal1;
                        ////    dgvPurchase["Others", Index].Value = txtOthersAmount.Text.Trim();
                        ////    dgvDuty["OthersInp", Index].Value = txtOthersInpValue.Text.Trim();
                        ////    dgvDuty["OthersRate", Index].Value = txtOthersRate.Text.Trim();
                        ////    dgvDuty["OthersAmount", Index].Value = txtOthersAmount.Text.Trim();
                        ////}

                        #endregion

                        #endregion Others

                        #region AIV

                        dgvPurchase["AIT", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();
                        dgvDuty["AITInp", dgvPurchase.CurrentRow.Index].Value = txtAITInpValue.Text.Trim();
                        dgvDuty["AITAmount", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();

                        #endregion ATV

                        #region InvoiceValue & ExchangeRate
                        dgvPurchase["InvoiceValue", dgvPurchase.CurrentRow.Index].Value = txtInvoiceValue.Text.Trim();
                        dgvPurchase["ExchangeRate", dgvPurchase.CurrentRow.Index].Value = txtExchangeRate.Text.Trim();
                        dgvPurchase["Currency", dgvPurchase.CurrentRow.Index].Value = txtCurrency.Text.Trim();
                        #endregion

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvDuty["Remarks", dgvPurchase.CurrentRow.Index].Value = txtDutiesRemarks.Text.Trim();
                        }
                        else
                        {
                            dgvDuty["Remarks", Index].Value = txtDutiesRemarks.Text.Trim();
                        }


                        #endregion If Import

                    }
                    else
                    {
                        #region Local / Not Import

                        string strSD = "";
                        decimal vSD = 0;

                        VATAmount = Convert.ToDecimal(dgvPurchase["VATAmount", Index].Value.ToString());
                        SDAmount = Convert.ToDecimal(dgvPurchase["SDAmount", Index].Value.ToString());

                        Total = (SDAmount + VATAmount + SubTotal);

                        #endregion Close
                    }

                    #endregion
                }

                #endregion

                #region Value Assign

                decimal vImpInsurance1 = Convert.ToDecimal(dgvPurchase["Insurance", Index].Value);
                decimal vImpCnF1 = Convert.ToDecimal(dgvPurchase["CnF", Index].Value);
                decimal vImpAV1 = Convert.ToDecimal(dgvPurchase["SubTotal", Index].Value);
                decimal vImpCD1 = Convert.ToDecimal(dgvPurchase["CD", Index].Value);
                decimal vImpRD1 = Convert.ToDecimal(dgvPurchase["RD", Index].Value);
                decimal vImpTVB1 = Convert.ToDecimal(dgvPurchase["TVB", Index].Value);
                decimal vImpSD1 = Convert.ToDecimal(dgvPurchase["SDAmount", Index].Value);
                decimal vImpVAT1 = Convert.ToDecimal(dgvPurchase["VATAmount", Index].Value);
                decimal vImpATV1 = Convert.ToDecimal(dgvPurchase["ATV", Index].Value);
                decimal vImpTVA1 = Convert.ToDecimal(dgvPurchase["TVA", Index].Value);
                decimal vImpOthers1 = Convert.ToDecimal(dgvPurchase["Others", Index].Value);
                decimal vImpAIT1 = Convert.ToDecimal(dgvPurchase["AIT", Index].Value);

                //////vImpTotalPrice = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpVAT + vImpATV +
                //////                          vImpOthers);
                vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                                          vImpOthers1 + vImpOthers1 + vImpTVA1 + vImpInsurance1 + vImpCnF1);

                Total = vImpTotalPrice;

                #region Conditional Values

                if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                    Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                    Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                #endregion

                dgvPurchase["RebateAmount", Index].Value = Convert.ToDecimal(dgvPurchase["VATAmount", Index].Value)
                        * Convert.ToDecimal(dgvPurchase["Rebate", Index].Value) / 100;
                dgvPurchase["Total", Index].Value = Total;

                #endregion

                #region AssessableValue

                if ((rbtnImport.Checked == true && dutyCalculate == true)
                       || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                       || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                        || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                        || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                       || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                {
                    DataGridViewColumnCollection dgvColumns = dgvDuty.Columns;

                    if (dgvColumns.Contains("AssessableInp"))
                    {
                        dgvDuty["AssessableInp", Index].Value = vImpAV1 + vImpInsurance1 + vImpCnF1;
                    }

                    if (dgvColumns.Contains("AssessableValue"))
                    {
                        dgvDuty["AssessableValue", Index].Value = vImpAV1 + vImpInsurance1 + vImpCnF1; ;
                    }



                }
                #endregion

                #region Value Assign

                VDSAmount = VDSAmount + Convert.ToDecimal(dgvPurchase["VDSAmount", Index].Value.ToString());
                string strSDAmount = "";
                decimal vSDAmount = 0;

                if (dgvPurchase["SDAmount", Index].Value != null)
                {
                    strSDAmount = dgvPurchase["SDAmount", Index].Value.ToString();
                    if (!string.IsNullOrEmpty(strSDAmount))
                        vSDAmount = Convert.ToDecimal(strSDAmount);
                }
                if (Program.CheckingNumericString(vSDAmount.ToString(), "vSDAmount") == true)
                    vSDAmount = Convert.ToDecimal(Program.FormatingNumeric(vSDAmount.ToString(), PurchasePlaceAmt));

                SumSDAmount = SumSDAmount + vSDAmount;

                if (Program.CheckingNumericString(SumSDAmount.ToString(), "SumSDAmount") == true)
                    SumSDAmount = Convert.ToDecimal(Program.FormatingNumeric(SumSDAmount.ToString(), PurchasePlaceAmt));


                string strVatAmount2 = "";
                decimal vVatAmount2 = 0;

                if (dgvPurchase["VATAmount", Index].Value != null)
                {
                    strVatAmount2 = dgvPurchase["VATAmount", Index].Value.ToString();
                    if (!string.IsNullOrEmpty(strVatAmount2))
                        vVatAmount2 = Convert.ToDecimal(strVatAmount2);
                }
                if (Program.CheckingNumericString(vVatAmount2.ToString(), "vVatAmount2") == true)
                    vVatAmount2 = Convert.ToDecimal(Program.FormatingNumeric(vVatAmount2.ToString(), PurchasePlaceAmt));

                #endregion

                #region Value Assign

                SumVATAmount = SumVATAmount + vVatAmount2;

                if (Program.CheckingNumericString(SumVATAmount.ToString(), "SumVATAmount") == true)
                    SumVATAmount = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), PurchasePlaceAmt));

                string strSumTotal = "";
                decimal vSumTotal = 0;

                if (dgvPurchase["Total", Index].Value != null)
                {
                    strSumTotal = dgvPurchase["Total", Index].Value.ToString();
                    if (!string.IsNullOrEmpty(strSumTotal))
                        vSumTotal = Convert.ToDecimal(strSumTotal);
                }

                if (Program.CheckingNumericString(vSumTotal.ToString(), "vSumTotal") == true)
                    vSumTotal = Convert.ToDecimal(Program.FormatingNumeric(vSumTotal.ToString(), PurchasePlaceAmt));

                SumTotal = SumTotal + vSumTotal;
                if (Program.CheckingNumericString(SumTotal.ToString(), "SumTotal") == true)
                    SumTotal = Convert.ToDecimal(Program.FormatingNumeric(SumTotal.ToString(), PurchasePlaceAmt));

                //vQuantity

                string strPrevious = "";
                decimal vPrevious = 0;

                if (dgvPurchase["Previous", Index].Value != null)
                {
                    strPrevious = dgvPurchase["Previous", Index].Value.ToString();
                    if (!string.IsNullOrEmpty(strPrevious))
                        vPrevious = Convert.ToDecimal(strPrevious);
                }
                dgvPurchase["Change", Index].Value = vQuantity - vPrevious;

                SubTotal1 = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1);

                #endregion

                #region Value Assign


                if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                    SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                SumSubTotal = SumSubTotal + SubTotal1;
                SumSubTotal2 = SumSubTotal2 + SubTotal;
                if (Program.CheckingNumericString(SumSubTotal.ToString(), "SumSubTotal") == true)
                    SumSubTotal = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), PurchasePlaceAmt));

                if (Program.CheckingNumericString(SumSubTotal2.ToString(), "SumSubTotal") == true)
                    SumSubTotal2 = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal2.ToString(), PurchasePlaceAmt));

                #endregion


                #endregion

                SubtotalCalculation();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Rowcalculate()
        {
            try
            {

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Statement

                #region Variables
                decimal vImpSubTotal = 0;
                decimal vImpVAT = 0;
                decimal vImpCnF = 0;
                decimal vImpInsurance = 0;
                decimal vImpAV = 0;
                decimal vImpCD = 0;
                decimal vImpRD = 0;
                decimal vImpTVB = 0;
                decimal vImpTVA = 0;
                decimal vImpATV = 0;
                decimal vImpOthers = 0;
                decimal vImpSD = 0;
                decimal vImpTotalPrice;
                decimal SubTotal1 = 0;
                decimal SubTotal = 0;
                decimal VDSAmount = 0;

                decimal SD = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal Total = 0;

                decimal SumSDAmount = 0;
                decimal SumVATAmount = 0;
                decimal SumSubTotal = 0;
                decimal SumSubTotal2 = 0;
                decimal SumTotal = 0;
                decimal vtotalSubTotal1 = 1;

                #endregion 0

                #endregion

                #region Sub Total

                if (Program.CheckingNumericTextBox(txtTotalSubTotal1, "txtTotalSubTotal1") == true)
                    txtTotalSubTotal1.Text = Program.FormatingNumeric(txtTotalSubTotal1.Text.Trim(), PurchasePlaceAmt).ToString();

                if (Convert.ToDecimal(txtTotalSubTotal1.Text.Trim()) == 0)
                {
                    vtotalSubTotal1 = 1;
                }
                else
                {
                    vtotalSubTotal1 = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                }

                #endregion

                #region For Loop / Value Assign to Purchase Data Grid View

                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {

                    dgvPurchase[0, i].Value = i + 1;
                    dgvPurchase["VATableValue", i].Value = "0";

                    string strSubTotal = "";
                    decimal vQuantity = 0;

                    #region Null Check of Duties

                    if (dgvPurchase["CnF", i].Value == null)
                    {
                        dgvPurchase["CnF", i].Value = "0";
                    }
                    if (dgvPurchase["Insurance", i].Value == null)
                    {
                        dgvPurchase["Insurance", i].Value = "0";
                    }
                    if (dgvPurchase["AV", i].Value == null)
                    {
                        dgvPurchase["AV", i].Value = "0";
                    }
                    if (dgvPurchase["CD", i].Value == null)
                    {
                        dgvPurchase["CD", i].Value = "0";
                    }
                    if (dgvPurchase["RD", i].Value == null)
                    {
                        dgvPurchase["RD", i].Value = "0";
                    }
                    if (dgvPurchase["TVB", i].Value == null)
                    {
                        dgvPurchase["TVB", i].Value = "0";
                    }
                    if (dgvPurchase["TVA", i].Value == null)
                    {
                        dgvPurchase["TVA", i].Value = "0";
                    }
                    if (dgvPurchase["ATV", i].Value == null)
                    {
                        dgvPurchase["ATV", i].Value = "0";
                    }
                    if (dgvPurchase["Others", i].Value == null)
                    {
                        dgvPurchase["Others", i].Value = "0";
                    }
                    if (dgvPurchase["AIT", i].Value == null)
                    {
                        dgvPurchase["AIT", i].Value = "0";
                    }
                    if (dgvPurchase["InvoiceValue", i].Value == null)
                    {
                        dgvPurchase["InvoiceValue", i].Value = "0";
                    }
                    if (dgvPurchase["ExchangeRate", i].Value == null)
                    {
                        dgvPurchase["ExchangeRate", i].Value = "0";
                    }
                    if (dgvPurchase["Currency", i].Value == null)
                    {
                        dgvPurchase["Currency", i].Value = "";
                    }
                    #endregion

                    #region Value Assign

                    SubTotal = Convert.ToDecimal(dgvPurchase["SubTotal", i].Value);
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                    if (boolsearch == false)
                    {//imp start


                        if ((rbtnImport.Checked == true && dutyCalculate == true)
                            || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                            || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                            || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                        {
                            #region Import Condition

                            dgvDuty["SubTotalDuty", i].Value = dgvPurchase["SubTotal", i].Value.ToString();
                            dgvDuty[0, i].Value = i + 1;

                            #region Import Values

                            if (dgvPurchase["SubTotal", i].Value != null)
                            {
                                strSubTotal = dgvPurchase["SubTotal", i].Value.ToString();

                                if (!string.IsNullOrEmpty(strSubTotal))
                                    vImpSubTotal = Convert.ToDecimal(strSubTotal);

                                if (Program.CheckingNumericString(vImpSubTotal.ToString(), "vImpSubTotal") == true)
                                    vImpSubTotal = Convert.ToDecimal(Program.FormatingNumeric(vImpSubTotal.ToString(), PurchasePlaceAmt));

                            }
                            if (ChkSubTotalAll.Checked == false)
                            {
                                vImpSubTotal = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                            }

                            #region AV Amount

                            if (CalculativeAV == false)
                            {
                                if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                                    txtAVAmount.Text = Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                                if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                                {
                                    vImpAV = Convert.ToDecimal(txtAVAmount.Text.Trim());
                                }
                                else
                                {
                                    vImpAV = 0;
                                    vImpCnF = 0;
                                    vImpInsurance = 0;
                                }


                            }
                            else
                            {

                                if (!string.IsNullOrEmpty(txtCnFRate.Text.Trim()))
                                {
                                    vImpCnF = vImpSubTotal * Convert.ToDecimal(txtCnFRate.Text.Trim()) / 100;
                                }
                                else
                                {
                                    vImpCnF = 0;
                                }
                                if (Program.CheckingNumericString(vImpCnF.ToString(), "vImpCnF") == true)
                                {

                                    vImpCnF = Convert.ToDecimal(Program.FormatingNumeric(vImpCnF.ToString(), PurchasePlaceAmt));
                                }


                                if (!string.IsNullOrEmpty(txtInsRate.Text.Trim()))
                                {
                                    vImpInsurance = (vImpSubTotal + vImpCnF) * Convert.ToDecimal(txtInsRate.Text.Trim()) /
                                                    100;
                                }
                                else
                                {
                                    vImpInsurance = 0;
                                }
                                if (Program.CheckingNumericString(vImpInsurance.ToString(), "vImpInsurance") == true)
                                {
                                    vImpInsurance = Convert.ToDecimal(Program.FormatingNumeric(vImpInsurance.ToString(), PurchasePlaceAmt));
                                }

                                vImpAV = vImpSubTotal + vImpCnF + vImpInsurance;

                            }


                            if (Program.CheckingNumericString(vImpAV.ToString(), "vImpAV") == true)
                                vImpAV = Convert.ToDecimal(Program.FormatingNumeric(vImpAV.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtCnFAmount, "txtCnFAmount") == true)
                                txtCnFAmount.Text =
                                    Program.FormatingNumeric(txtCnFAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                                txtInsAmount.Text =
                                    Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                                txtAVAmount.Text =
                                    Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtCnFInpValue, "txtCnFInpValue") == true)
                                txtCnFInpValue.Text =
                                    Program.FormatingNumeric(txtCnFInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsInpValue, "txtInsInpValue") == true)
                                txtInsInpValue.Text =
                                    Program.FormatingNumeric(txtInsInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                                txtInsAmount.Text =
                                    Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtAVInpValue, "txtAVInpValue") == true)
                                txtAVInpValue.Text =
                                    Program.FormatingNumeric(txtAVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                                txtNBRPrice.Text =
                                    Program.FormatingNumeric(txtNBRPrice.Text.Trim(), PurchasePlaceAmt).ToString();


                            if (ChkSubTotalAll.Checked == false)
                            {
                                vImpAV = vImpAV * SubTotal / vtotalSubTotal1;
                                vImpInsurance = vImpInsurance * SubTotal / vtotalSubTotal1;
                                vImpCnF = vImpCnF * SubTotal / vtotalSubTotal1;

                                dgvPurchase["CnF", i].Value = vImpCnF;
                                dgvPurchase["Insurance", i].Value = vImpInsurance;
                                dgvPurchase["AV", i].Value = vImpAV;

                                dgvDuty["CnFInp", i].Value = txtCnFInpValue.Text.Trim();
                                dgvDuty["CnFRate", i].Value = txtCnFRate.Text.Trim();
                                dgvDuty["CnFAmount", i].Value = vImpCnF;
                                dgvDuty["InsuranceInp", i].Value = txtInsInpValue.Text.Trim();
                                dgvDuty["InsuranceRate", i].Value = txtInsRate.Text.Trim();
                                dgvDuty["InsuranceAmount", i].Value = vImpInsurance;
                                dgvDuty["AssessableInp", i].Value = txtAVInpValue.Text.Trim();
                                dgvDuty["AssessableValue", i].Value = vImpAV;
                            }



                            #endregion AV

                            #region CD Amount

                            if (!string.IsNullOrEmpty(txtCDRate.Text.Trim()))
                            {
                                vImpCD = vImpAV * Convert.ToDecimal(txtCDRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpCD = vImpAV * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpCD.ToString(), "vImpCD") == true)
                                vImpCD = Convert.ToDecimal(Program.FormatingNumeric(vImpCD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                                txtCDAmount.Text =
                                    Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtCDInpValue, "txtCDInpValue") == true)
                                txtCDInpValue.Text =
                                    Program.FormatingNumeric(txtCDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["CD", i].Value = txtCDAmount.Text.Trim();
                                dgvDuty["CDInp", i].Value = txtCDInpValue.Text.Trim();
                                dgvDuty["CDRate", i].Value = txtCDRate.Text.Trim();
                                dgvDuty["CDAmount", i].Value = txtCDAmount.Text.Trim();
                            }



                            #endregion CD

                            #region RD Amount

                            if (!string.IsNullOrEmpty(txtRDRate.Text.Trim()))
                            {
                                vImpRD = vImpAV * Convert.ToDecimal(txtRDRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpRD = vImpAV * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpRD.ToString(), "vImpRD") == true)
                                vImpRD = Convert.ToDecimal(Program.FormatingNumeric(vImpRD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                                txtRDAmount.Text =
                                    Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtRDInpValue, "txtRDInpValue") == true)
                                txtRDInpValue.Text =
                                    Program.FormatingNumeric(txtRDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                                txtRDAmount.Text =
                                    Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["RD", i].Value = txtRDAmount.Text.Trim();
                                dgvDuty["RDInp", i].Value = txtRDInpValue.Text.Trim();
                                dgvDuty["RDRate", i].Value = txtRDRate.Text.Trim();
                                dgvDuty["RDAmount", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                            }




                            #endregion RD

                            #region TVB Amount

                            if (!string.IsNullOrEmpty(txtTVBRate.Text.Trim()))
                            {
                                vImpTVB = (vImpAV + vImpCD + vImpRD) * Convert.ToDecimal(txtTVBRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpTVB = (vImpAV + vImpCD + vImpRD) * 0 / 100;
                            }



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["TVB", i].Value = vImpTVB;
                                dgvDuty["TVBInp", i].Value = txtTVBInpValue.Text.Trim();
                                dgvDuty["TVBRate", i].Value = txtTVBRate.Text.Trim();
                                dgvDuty["TVBAmount", i].Value = vImpTVB;
                            }


                            #endregion TVB

                            #region SD Amount

                            if (!string.IsNullOrEmpty(txtSDRate.Text.Trim()))
                            {
                                vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * Convert.ToDecimal(txtSDRate.Text.Trim()) /
                                         100;
                            }
                            else
                            {
                                vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * 0 / 100;
                            }
                            if (Program.CheckingNumericString(vImpSD.ToString(), "vImpSD") == true)
                                vImpSD = Convert.ToDecimal(Program.FormatingNumeric(vImpSD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                                txtSDAmount.Text =
                                    Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtSDInpValue, "txtSDInpValue") == true)
                                txtSDInpValue.Text =
                                    Program.FormatingNumeric(txtSDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                                txtSDAmount.Text =
                                    Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["SDAmount", i].Value = txtSDAmount.Text.Trim();
                                dgvPurchase["SD", i].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDInp", i].Value = txtSDInpValue.Text.Trim();
                                dgvDuty["SDuty", i].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDutyAmount", i].Value = txtSDAmount.Text.Trim();

                            }

                            #endregion SD

                            #region VAT Amount

                            if (!string.IsNullOrEmpty(txtVATRateI.Text.Trim()))
                                vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                          Convert.ToDecimal(txtVATRateI.Text.Trim()) / 100;
                            else
                                vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;

                            if (Program.CheckingNumericString(vImpVAT.ToString(), "vImpVAT") == true)
                                vImpVAT =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpVAT.ToString(), PurchasePlaceAmt));


                            if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                                txtVATAmount.Text =
                                    Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtVATInpValue, "txtVATInpValue") == true)
                                txtVATInpValue.Text =
                                    Program.FormatingNumeric(txtVATInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                                txtVATAmount.Text =
                                    Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();





                            if (ChkSubTotalAll.Checked == false)
                            {
                                decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                                decimal varVATAmount = Convert.ToDecimal(txtVATAmount.Text);
                                decimal varVATRate = 0;
                                if (varAssesableValue > 0)
                                {
                                    varVATRate = (varVATAmount / varAssesableValue) * 100;
                                }



                                dgvPurchase["VATAmount", i].Value = txtVATAmount.Text.Trim();
                                dgvPurchase["VATRate", i].Value = varVATRate;
                                dgvDuty["VATInp", i].Value = txtVATInpValue.Text.Trim();
                                dgvDuty["VATRateDuty", i].Value = varVATRate;
                                dgvDuty["VATAmountDuty", i].Value = txtVATAmount.Text.Trim();
                            }

                            decimal bdtTotalVATAmount, usdInvoiceValue, vatperDollar, usdValue, usdVAT = 0;
                            bdtTotalVATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                            usdInvoiceValue = Convert.ToDecimal(txtUSDInvoiceValue.Text.Trim());
                            vatperDollar = bdtTotalVATAmount / usdInvoiceValue;
                            usdValue = Convert.ToDecimal(dgvPurchase["USDValue", i].Value);
                            usdVAT = usdValue * vatperDollar;
                            dgvPurchase["USDVAT", i].Value = usdVAT;
                            dgvPurchase["VATableValue", i].Value = Convert.ToDecimal(dgvPurchase["AV", i].Value)
                                + Convert.ToDecimal(dgvPurchase["CD", i].Value)
                                + Convert.ToDecimal(dgvPurchase["RD", i].Value)
                                + Convert.ToDecimal(dgvPurchase["TVB", i].Value)
                                + Convert.ToDecimal(dgvPurchase["SDAmount", i].Value)

                                ;


                            #endregion VAT

                            #region TVA Amount

                            //if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                            //    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (!string.IsNullOrEmpty(txtTVARate.Text.Trim()))
                            {
                                vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                          Convert.ToDecimal(txtTVARate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpTVA.ToString(), "vImpTVA") == true)
                                vImpTVA =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpTVA.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                                txtTVAAmount.Text =
                                    Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtTVAInpValue, "txtTVAInpValue") == true)
                                txtTVAInpValue.Text =
                                    Program.FormatingNumeric(txtTVAInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                            //    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                                txtTVAAmount.Text =
                                    Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();




                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["TVA", i].Value = vImpTVA;
                                dgvDuty["TVAInp", i].Value = txtTVAInpValue.Text.Trim();
                                dgvDuty["TVARate", i].Value = txtTVARate.Text.Trim();
                                dgvDuty["TVAAmount", i].Value = vImpTVA;
                            }

                            #endregion TVA

                            #region ATV Amount

                            if (!string.IsNullOrEmpty(txtATVRate.Text.Trim()))
                            {
                                vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) *
                                          Convert.ToDecimal(txtATVRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpATV.ToString(), "vImpATV") == true)
                                vImpATV =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpATV.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                                txtATVAmount.Text =
                                    Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtATVInpValue, "txtATVInpValue") == true)
                                txtATVInpValue.Text =
                                    Program.FormatingNumeric(txtATVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtATVRate, "txtATVRate") == true)
                            //    txtATVRate.Text = Program.FormatingNumeric(txtATVRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                                txtATVAmount.Text =
                                    Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvPurchase["ATV", i].Value = txtATVAmount.Text.Trim();
                                dgvDuty["ATVInp", i].Value = txtATVInpValue.Text.Trim();
                                dgvDuty["ATVRate", i].Value = txtATVRate.Text.Trim();
                                dgvDuty["ATVAmount", i].Value = txtATVAmount.Text.Trim();
                            }




                            #endregion ATV

                            #region Others Amount


                            if (!string.IsNullOrEmpty(txtOthersRate.Text.Trim()))
                            {
                                vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                             Convert.ToDecimal(txtOthersRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                            }


                            if (Program.CheckingNumericString(vImpOthers.ToString(), "vImpOthers") == true)
                                vImpOthers =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpOthers.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtOthersInpValue, "txtOthersInpValue") == true)
                                txtOthersInpValue.Text =
                                    Program.FormatingNumeric(txtOthersInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtOthersAmount, "txtOthersAmount") == true)
                                txtOthersAmount.Text =
                                    Program.FormatingNumeric(txtOthersAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked == false)
                            {
                                //vImpOthers = vImpOthers * SubTotal / vtotalSubTotal1;
                                dgvPurchase["Others", i].Value = txtOthersAmount.Text.Trim();
                                dgvDuty["OthersInp", i].Value = txtOthersInpValue.Text.Trim();
                                dgvDuty["OthersRate", i].Value = txtOthersRate.Text.Trim();
                                dgvDuty["OthersAmount", i].Value = txtOthersAmount.Text.Trim();
                            }



                            #endregion Others

                            #region AIV Amount





                            #endregion ATV



                            if (ChkSubTotalAll.Checked == false)
                            {
                                dgvDuty["Remarks", i].Value = txtDutiesRemarks.Text.Trim();
                            }


                            #endregion

                            #endregion

                        }
                        else
                        {
                            #region Local / Not Import

                            VATAmount = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value.ToString());
                            SDAmount = Convert.ToDecimal(dgvPurchase["SDAmount", i].Value.ToString());
                            dgvPurchase["AV", i].Value = SubTotal;

                            Total = (SDAmount + VATAmount + SubTotal);

                            #endregion Close
                        }

                    }

                    #endregion

                    #region Change Value

                    string strPrevious = "";
                    decimal vPrevious = 0;

                    if (dgvPurchase["Previous", i].Value != null)
                    {
                        strPrevious = dgvPurchase["Previous", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strPrevious))
                            vPrevious = Convert.ToDecimal(strPrevious);
                    }
                    dgvPurchase["Change", i].Value = vQuantity - vPrevious;

                    #endregion

                }

                #endregion

                #region Current Row

                if (boolsearch == false)
                {

                    #region Import Condition

                    if ((rbtnImport.Checked == true && dutyCalculate == true)
                        || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                        || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                        || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                        || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                        || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                    {

                        #region AV Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["CnF", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                            dgvPurchase["Insurance", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                            dgvPurchase["AV", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();
                            dgvDuty["CnFInp", dgvPurchase.CurrentRow.Index].Value = txtCnFInpValue.Text.Trim();
                            dgvDuty["CnFRate", dgvPurchase.CurrentRow.Index].Value = txtCnFRate.Text.Trim();
                            dgvDuty["CnFAmount", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                            dgvDuty["InsuranceInp", dgvPurchase.CurrentRow.Index].Value = txtInsInpValue.Text.Trim();
                            dgvDuty["InsuranceRate", dgvPurchase.CurrentRow.Index].Value = txtInsRate.Text.Trim();
                            dgvDuty["InsuranceAmount", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                            dgvDuty["AssessableInp", dgvPurchase.CurrentRow.Index].Value = txtAVInpValue.Text.Trim();
                            dgvDuty["AssessableValue", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();
                        }

                        #endregion

                        #region CD Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["CD", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                            dgvDuty["CDInp", dgvPurchase.CurrentRow.Index].Value = txtCDInpValue.Text.Trim();
                            dgvDuty["CDRate", dgvPurchase.CurrentRow.Index].Value = txtCDRate.Text.Trim();
                            dgvDuty["CDAmount", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                        }

                        #endregion

                        #region RD Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["RD", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                            dgvDuty["RDInp", dgvPurchase.CurrentRow.Index].Value = txtRDInpValue.Text.Trim();
                            dgvDuty["RDRate", dgvPurchase.CurrentRow.Index].Value = txtRDRate.Text.Trim();
                            dgvDuty["RDAmount", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                        }

                        #endregion

                        #region TVB Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["TVB", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                            dgvDuty["TVBInp", dgvPurchase.CurrentRow.Index].Value = txtTVBInpValue.Text.Trim();
                            dgvDuty["TVBRate", dgvPurchase.CurrentRow.Index].Value = txtTVBRate.Text.Trim();
                            dgvDuty["TVBAmount", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                        }

                        #endregion

                        #region SD Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["SDAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();
                            dgvPurchase["SD", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                            dgvDuty["SDInp", dgvPurchase.CurrentRow.Index].Value = txtSDInpValue.Text.Trim();
                            dgvDuty["SDuty", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                            dgvDuty["SDutyAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();

                        }

                        #endregion

                        #region VAT Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                            decimal varVATAmount = Convert.ToDecimal(txtVATAmount.Text);
                            decimal varVATRate = 0;
                            if (varAssesableValue > 0)
                            {
                                varVATRate = (varVATAmount / varAssesableValue) * 100;
                            }


                            dgvPurchase["VATAmount", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                            dgvPurchase["VATRate", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                            dgvDuty["VATInp", dgvPurchase.CurrentRow.Index].Value = txtVATInpValue.Text.Trim();
                            dgvDuty["VATRateDuty", dgvPurchase.CurrentRow.Index].Value = varVATRate;
                            dgvDuty["VATAmountDuty", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                        }

                        #endregion

                        #region TVA Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["TVA", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                            dgvDuty["TVAInp", dgvPurchase.CurrentRow.Index].Value = txtTVAInpValue.Text.Trim();
                            dgvDuty["TVARate", dgvPurchase.CurrentRow.Index].Value = txtTVARate.Text.Trim();
                            dgvDuty["TVAAmount", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                        }

                        #endregion

                        #region ATV Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["ATV", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                            dgvDuty["ATVInp", dgvPurchase.CurrentRow.Index].Value = txtATVInpValue.Text.Trim();
                            dgvDuty["ATVRate", dgvPurchase.CurrentRow.Index].Value = txtATVRate.Text.Trim();
                            dgvDuty["ATVAmount", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                        }


                        #endregion

                        #region Others Amount

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvPurchase["Others", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                            dgvDuty["OthersInp", dgvPurchase.CurrentRow.Index].Value = txtOthersInpValue.Text.Trim();
                            dgvDuty["OthersRate", dgvPurchase.CurrentRow.Index].Value = txtOthersRate.Text.Trim();
                            dgvDuty["OthersAmount", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                        }

                        #endregion

                        #region AIV Amount

                        if (ChkSubTotalAll.Checked)
                        {

                            dgvPurchase["AIT", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();
                            dgvDuty["AITInp", dgvPurchase.CurrentRow.Index].Value = txtAITInpValue.Text.Trim();
                            dgvDuty["AITAmount", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();

                        }

                        #endregion

                        if (ChkSubTotalAll.Checked) // Single
                        {
                            dgvDuty["Remarks", dgvPurchase.CurrentRow.Index].Value = txtDutiesRemarks.Text.Trim();
                        }
                    }
                    #endregion
                }
                #endregion

                SubtotalCalculation();

                boolsearch = false;

                FindBomId();

                if (true)
                {
                    //////throw new Exception("Test");
                }
            }
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            #endregion
        }

        private void SubtotalCalculation()
        {
            try
            {

                #region Total Calculation / For Loop

                decimal vImpTotalPrice = 0;

                decimal SubTotal1 = 0;
                decimal SubTotal = 0;
                decimal VDSAmount = 0;

                decimal Total = 0;

                decimal SumSDAmount = 0;
                decimal SumVATAmount = 0;
                decimal SumSubTotal = 0;
                decimal SumSubTotal2 = 0;
                decimal SumTotal = 0;

                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {

                    #region Line Total


                    decimal vImpInsurance1 = Convert.ToDecimal(dgvPurchase["Insurance", i].Value);
                    decimal vImpCnF1 = Convert.ToDecimal(dgvPurchase["CnF", i].Value);
                    decimal vImpAV1 = Convert.ToDecimal(dgvPurchase["SubTotal", i].Value);
                    decimal vImpCD1 = Convert.ToDecimal(dgvPurchase["CD", i].Value);
                    decimal vImpRD1 = Convert.ToDecimal(dgvPurchase["RD", i].Value);
                    decimal vImpTVB1 = Convert.ToDecimal(dgvPurchase["TVB", i].Value);
                    decimal vImpSD1 = Convert.ToDecimal(dgvPurchase["SDAmount", i].Value);
                    decimal vImpVAT1 = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value);
                    decimal vImpATV1 = Convert.ToDecimal(dgvPurchase["ATV", i].Value);
                    decimal vImpTVA1 = Convert.ToDecimal(dgvPurchase["TVA", i].Value);
                    decimal vImpOthers1 = Convert.ToDecimal(dgvPurchase["Others", i].Value);
                    decimal vImpAIT1 = Convert.ToDecimal(dgvPurchase["AIT", i].Value);


                    vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                                              vImpOthers1 + vImpOthers1 + vImpTVA1 + vImpInsurance1 + vImpCnF1);

                    Total = vImpTotalPrice;

                    #region Conditional Values

                    if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                        Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                    if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                        Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                    #endregion

                    dgvPurchase["RebateAmount", i].Value = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value)
                            * Convert.ToDecimal(dgvPurchase["Rebate", i].Value) / 100;
                    dgvPurchase["Total", i].Value = Total;

                    #endregion

                    #region AssessableValue

                    if ((rbtnImport.Checked == true && dutyCalculate == true)
                           || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                           || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                           || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                    {
                        DataGridViewColumnCollection dgvColumns = dgvDuty.Columns;

                        if (dgvColumns.Contains("AssessableInp"))
                        {
                            dgvDuty["AssessableInp", i].Value = vImpAV1 + vImpInsurance1 + vImpCnF1;
                        }

                        if (dgvColumns.Contains("AssessableValue"))
                        {
                            dgvDuty["AssessableValue", i].Value = vImpAV1 + vImpInsurance1 + vImpCnF1; ;
                        }



                    }
                    #endregion

                    #region Total VDSAmount, Total SDAmount

                    VDSAmount = VDSAmount + Convert.ToDecimal(dgvPurchase["VDSAmount", i].Value.ToString());
                    string strSDAmount = "";
                    decimal vSDAmount = 0;

                    if (dgvPurchase["SDAmount", i].Value != null)
                    {
                        strSDAmount = dgvPurchase["SDAmount", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strSDAmount))
                            vSDAmount = Convert.ToDecimal(strSDAmount);
                    }
                    if (Program.CheckingNumericString(vSDAmount.ToString(), "vSDAmount") == true)
                        vSDAmount = Convert.ToDecimal(Program.FormatingNumeric(vSDAmount.ToString(), PurchasePlaceAmt));

                    SumSDAmount = SumSDAmount + vSDAmount;

                    if (Program.CheckingNumericString(SumSDAmount.ToString(), "SumSDAmount") == true)
                        SumSDAmount = Convert.ToDecimal(Program.FormatingNumeric(SumSDAmount.ToString(), PurchasePlaceAmt));



                    #endregion

                    #region Total VAT, Total, Subtotal

                    string strVatAmount2 = "";
                    decimal vVatAmount2 = 0;

                    if (dgvPurchase["VATAmount", i].Value != null)
                    {
                        strVatAmount2 = dgvPurchase["VATAmount", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strVatAmount2))
                            vVatAmount2 = Convert.ToDecimal(strVatAmount2);
                    }
                    if (Program.CheckingNumericString(vVatAmount2.ToString(), "vVatAmount2") == true)
                        vVatAmount2 = Convert.ToDecimal(Program.FormatingNumeric(vVatAmount2.ToString(), PurchasePlaceAmt));



                    SumVATAmount = SumVATAmount + vVatAmount2;

                    if (Program.CheckingNumericString(SumVATAmount.ToString(), "SumVATAmount") == true)
                        SumVATAmount = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), PurchasePlaceAmt));

                    string strSumTotal = "";
                    decimal vSumTotal = 0;

                    if (dgvPurchase["Total", i].Value != null)
                    {
                        strSumTotal = dgvPurchase["Total", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strSumTotal))
                            vSumTotal = Convert.ToDecimal(strSumTotal);
                    }

                    if (Program.CheckingNumericString(vSumTotal.ToString(), "vSumTotal") == true)
                        vSumTotal = Convert.ToDecimal(Program.FormatingNumeric(vSumTotal.ToString(), PurchasePlaceAmt));

                    SumTotal = SumTotal + vSumTotal;
                    if (Program.CheckingNumericString(SumTotal.ToString(), "SumTotal") == true)
                        SumTotal = Convert.ToDecimal(Program.FormatingNumeric(SumTotal.ToString(), PurchasePlaceAmt));




                    SubTotal1 = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1);


                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                    SumSubTotal = SumSubTotal + SubTotal1;
                    SumSubTotal2 = SumSubTotal2 + SubTotal;
                    if (Program.CheckingNumericString(SumSubTotal.ToString(), "SumSubTotal") == true)
                        SumSubTotal = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), PurchasePlaceAmt));

                    if (Program.CheckingNumericString(SumSubTotal2.ToString(), "SumSubTotal") == true)
                        SumSubTotal2 = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal2.ToString(), PurchasePlaceAmt));

                    #endregion
                }


                #endregion


                #region Total Fields

                txtTotalSubTotal2.Text = Program.ParseDecimalObject(SumSubTotal2).ToString();

                txtTotalSubTotal.Text = Program.ParseDecimalObject(SumSubTotal).ToString();//"0,0.00");
                txtTotalVATAmount.Text = Program.ParseDecimalObject(SumVATAmount).ToString();//"0,0.00");
                txtTotalSDAmount.Text = Program.ParseDecimalObject(SumSDAmount).ToString();//"0,0.00");
                txtTotalAmount.Text = Program.ParseDecimalObject(SumTotal).ToString();//"0,0.00");
                txtTotalVDSAmount.Text = Program.ParseDecimalObject(VDSAmount).ToString();//"0,0.00");

                #endregion

            }
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            #endregion
        }

        private void PurchaseRemoveSingle()
        {
            #region Statement

            if (dgvPurchase.RowCount > 0)
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


            #endregion
            if (txtCommentsDetail.Text == "")
            {
                txtCommentsDetail.Text = "NA";
            }


            ChangeData = true;
            #region Stock Check

            decimal StockValue, PreviousValue, CurrentValue, purchaseQty;
            StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
            PreviousValue = Convert.ToDecimal(txtPrevious.Text);
            //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
            CurrentValue = 0;
            purchaseQty = Convert.ToDecimal(txtPurQty.Text);
            if (rbtnServiceNS.Checked == false
                    && rbtnServiceNSImport.Checked == false
                    && rbtnInputService.Checked == false
                    && rbtnPurchaseTollcharge.Checked == false
                    && rbtnInputServiceImport.Checked == false)
            {
                if (rbtnPurchaseReturn.Checked || rbtnPurchaseDN.Checked)
                {
                    if (purchaseQty < CurrentValue)
                    {
                        MessageBox.Show(
                            "changes quantity Can't Greater than purchase quantity. \n purchaase quantity = '" +
                            purchaseQty + "'");
                        txtQuantity.Focus();
                        return;
                    }
                    else if (CurrentValue > PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(Convert.ToDecimal(CurrentValue - PreviousValue) *
                                              Convert.ToDecimal(txtUomConv.Text)) >
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
                    if (CurrentValue < PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(Convert.ToDecimal(PreviousValue - CurrentValue) *
                                              Convert.ToDecimal(txtUomConv.Text)) >
                            Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }
                    }
                }

            }
            #endregion Stock Check


            if (Convert.ToDecimal(dgvPurchase.CurrentCellAddress.Y) < 0)
            {
                MessageBox.Show("There have no Data to Delete");
                return;
            }
            //dgvPurchase.Rows.RemoveAt(dgvPurchase.CurrentCellAddress.Y);
            dgvPurchase.CurrentRow.Cells["Status"].Value = "Delete";


            dgvPurchase.CurrentRow.Cells["Quantity"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["UnitPrice"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["SubTotal"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["Rebate"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["RebateAmount"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["CnF"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["Insurance"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["AV"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["CD"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["RD"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["TVB"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["SD"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["SDAmount"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["VATRate"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["VATAmount"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["TVA"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["ATV"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["Others"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["Total"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["NBRPrice"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["UOMPrice"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["UOMQty"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["UOMc"].Value = 0.00;
            dgvPurchase.CurrentRow.Cells["VDS"].Value = 0.00;
            if ((rbtnImport.Checked == true)
                                       || (rbtnTradingImport.Checked == true)
                                       || (rbtnServiceImport.Checked == true)
                                        || (rbtnInputServiceImport.Checked == true)
                                       || (rbtnServiceNSImport.Checked == true)
                                       || (rbtnCommercialImporter.Checked == true)
                                       || (rbtnImport.Checked == true)
                            )
            {

                dgvDuty.CurrentRow.Cells["CnFInp"].Value = 0.00;//"CnFInp"].ToString();
                dgvDuty.CurrentRow.Cells["CnFRate"].Value = 0.00;//"CnFRate"].ToString();
                dgvDuty.CurrentRow.Cells["CnFAmount"].Value = 0.00;//"CnFAmount"].ToString();
                dgvDuty.CurrentRow.Cells["InsuranceInp"].Value = 0.00;//"InsuranceInp"].ToString();
                dgvDuty.CurrentRow.Cells["InsuranceRate"].Value = 0.00;//"InsuranceRate"].ToString();
                dgvDuty.CurrentRow.Cells["InsuranceAmount"].Value = 0.00;//"InsuranceAmount"].ToString();
                dgvDuty.CurrentRow.Cells["AssessableInp"].Value = 0.00;//"AssessableInp"].ToString();
                dgvDuty.CurrentRow.Cells["AssessableValue"].Value = 0.00;//"AssessableValue"].ToString();
                dgvDuty.CurrentRow.Cells["CDInp"].Value = 0.00;//"CDInp"].ToString();
                dgvDuty.CurrentRow.Cells["CDRate"].Value = 0.00;//"CDRate"].ToString();
                dgvDuty.CurrentRow.Cells["CDAmount"].Value = 0.00;//"CDAmount"].ToString();
                dgvDuty.CurrentRow.Cells["RDInp"].Value = 0.00;//"RDInp"].ToString();
                dgvDuty.CurrentRow.Cells["RDRate"].Value = 0.00;//"RDRate"].ToString();
                dgvDuty.CurrentRow.Cells["RDAmount"].Value = 0.00;//"RDAmount"].ToString();
                dgvDuty.CurrentRow.Cells["TVBInp"].Value = 0.00;//"TVBInp"].ToString();
                dgvDuty.CurrentRow.Cells["TVBRate"].Value = 0.00;//"TVBRate"].ToString();
                dgvDuty.CurrentRow.Cells["TVBAmount"].Value = 0.00;//"TVBAmount"].ToString();
                dgvDuty.CurrentRow.Cells["SDInp"].Value = 0.00;//"SDInp"].ToString();
                dgvDuty.CurrentRow.Cells["SDuty"].Value = 0.00;//"SD"].ToString();
                dgvDuty.CurrentRow.Cells["SDutyAmount"].Value = 0.00;//"SDAmount"].ToString();
                dgvDuty.CurrentRow.Cells["VATInp"].Value = 0.00;//"VATInp"].ToString();
                dgvDuty.CurrentRow.Cells["VATRateDuty"].Value = 0.00;//"VATRate"].ToString();
                dgvDuty.CurrentRow.Cells["VATAmountDuty"].Value = 0.00;//"VATAmount"].ToString();
                dgvDuty.CurrentRow.Cells["TVAInp"].Value = 0.00;//"TVAInp"].ToString();
                dgvDuty.CurrentRow.Cells["TVARate"].Value = 0.00;//"TVARate"].ToString();
                dgvDuty.CurrentRow.Cells["TVAAmount"].Value = 0.00;//"TVAAmount"].ToString();
                dgvDuty.CurrentRow.Cells["ATVInp"].Value = 0.00;//"ATVInp"].ToString();
                dgvDuty.CurrentRow.Cells["ATVRate"].Value = 0.00;//"ATVRate"].ToString();
                dgvDuty.CurrentRow.Cells["ATVAmount"].Value = 0.00;//"ATVAmount"].ToString();
                dgvDuty.CurrentRow.Cells["OthersInp"].Value = 0.00;//"OthersInp"].ToString();
                dgvDuty.CurrentRow.Cells["OthersRate"].Value = 0.00;//"OthersRate"].ToString();
                dgvDuty.CurrentRow.Cells["OthersAmount"].Value = 0.00;//"OthersAmount"].ToString();
                dgvDuty.CurrentRow.Cells["Remarks"].Value = 0.00;//"Remarks"].ToString();
            }

            dgvPurchase.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
            dgvPurchase.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);


            Rowcalculate();
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtUOM.Text = "";
            txtUomConv.Text = "";
            txtQuantity.Text = "";


            cmbProduct.Focus();



        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private decimal CountReturnTrackingItem(string Item)
        {
            decimal noOfReturnItems = 0;

            for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
            {
                if (dgvSerialTrack["ItemNoS", i].Value.ToString() == Item)
                {
                    if (Convert.ToBoolean(dgvSerialTrack["SelectT", i].Value) == true)
                    {
                        noOfReturnItems = noOfReturnItems + 1;

                    }
                }
            }
            return noOfReturnItems;

        }

        #endregion

        #region Methods 03 / Save, Update, Post, Search, New

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region variable
            FiscalYearVM varFiscalYearVM = new FiscalYearVM();

            string periodId = "0";

            #endregion

            try
            {
                #region Check Point

                #region Find Fiscal Year Lock

                string PeriodName = dtpReceiveDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion

                #region Find Rebate Period Id

                string RebatePeriod = dtpRebateDate.Value.ToString("MMMM-yyyy");
                string[] conditionValues = { RebatePeriod };
                string[] conditionFields = { "PeriodName" };
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, conditionFields, conditionValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(RebatePeriod + ": This Fiscal Period is not Exist!");
                }

                periodId = varFiscalYearVM.PeriodID;

                #endregion

                #region Old Code

                //if (Add == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                //    return;
                //}


                //PurchaseMasterVM purchaseMaster=new PurchaseMasterVM();
                //List<PurchaseDetailVM> purchaseDetails=new List<PurchaseDetailVM>();


                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}

                #endregion

                if (Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")) > Convert.ToDateTime(dtpReceiveDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Receive date  and time not before invoice date", this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");

                    return;
                }
                if (rbtnImport.Checked)
                {
                    if (txtCustomHouse.Text == "")
                    {
                        MessageBox.Show("Please Select Custom House");
                        txtCustomHouse.Focus();
                        return;
                    }

                }


                result = string.Empty;


                #endregion

                #region Check Point

                NextID = IsUpdate == false ? string.Empty : txtHiddenInvoiceNo.Text.Trim();

                #region Exist Check

                PurchaseMasterVM vm = new PurchaseMasterVM();
                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new PurchaseDAL().SelectAllList(0, new[] { "pih.PurchaseInvoiceNo" }, new[] { NextID }, null, null, null, connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #endregion

                #endregion

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Null check

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtBENumber.Text == "")
                {
                    MessageBox.Show("Please Input BE/6.3 No");
                    return;
                }
                if (
                                   transactionType.ToLower() == "import"
                                   || transactionType.ToLower() == "tradingimport"
                                   || transactionType.ToLower() == "servicensimport"
                                   || transactionType.ToLower() == "serviceimport"

                                   )
                {

                    if (txtLCNumber.Text == "")
                    {
                        MessageBox.Show("Please Input LC No");
                        return;
                    }




                }
                else
                {

                    if (txtLCNumber.Text == "")
                    {
                        txtLCNumber.Text = "-";

                    }
                }

                if (txtVendorID.Text == "")
                {
                    MessageBox.Show("Please Enter Vendor Information");
                    cmbVendor.Focus();
                    return;
                }

                if (txtLCNumber.Text != "-")
                {
                    ////if (!OrdinaryVATDesktop.IsNumber(txtLCNumber.Text))
                    ////{
                    ////    MessageBox.Show("Please Enter LC No only number");
                    ////    txtLCNumber.Focus();
                    ////    return;
                    ////}

                    if (txtLCNumber.Text.Length >= 17)
                    {
                        MessageBox.Show("LC No only Number/LC No Maximum 16 digit.");
                        txtLCNumber.Focus();
                        return;
                    }

                }


                if (dgvPurchase.RowCount <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion null

                #region Check Point

                if (rbtnOther.Checked || rbtnTrading.Checked || rbtnTradingImport.Checked || rbtnImport.Checked || rbtnCommercialImporter.Checked || rbtnClientRawReceive.Checked || rbtnClientFGReceiveWOBOM.Checked)
                {
                    RateCheck();
                }

                #endregion

                #region Tracking

                if (TrackingTrace == "Y")
                {
                    if (dgvSerialTrack.Rows.Count == 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information not added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,

                         MessageBoxButtons.YesNo,

                         MessageBoxIcon.Question,

                         MessageBoxDefaultButton.Button2))
                        {
                            tcPurchase.SelectedTab = tabPageSerial;
                            return;

                        }

                    }
                    if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        string item = "";
                        decimal qty, returnItems = 0;

                        for (int i = 0; i < dgvPurchase.RowCount; i++)
                        {
                            item = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                            qty = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                            returnItems = CountReturnTrackingItem(item);
                            if (qty != returnItems)
                            {
                                MessageBox.Show("Please select tracking quantity " + qty + " for Item Code : " + dgvPurchase.Rows[i].Cells["PCode"].Value.ToString() + ".",
                                    this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                tcPurchase.SelectedTab = tabPageSerial;
                                return;
                            }
                        }


                    }

                }

                #endregion

                #region Value Assign

                string vDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                #region Master

                purchaseMaster = new PurchaseMasterVM();
                purchaseMaster.PurchaseInvoiceNo = NextID.ToString();
                purchaseMaster.VendorID = txtVendorID.Text.Trim();
                purchaseMaster.InvoiceDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                purchaseMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                purchaseMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                purchaseMaster.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                purchaseMaster.LCNumber = txtLCNumber.Text.Trim().Replace(" ", ""); //LCNumber
                purchaseMaster.Comments = txtComments.Text.Trim();
                purchaseMaster.CreatedBy = Program.CurrentUser;
                purchaseMaster.CreatedOn = vDateTime;
                purchaseMaster.LastModifiedBy = Program.CurrentUser;
                purchaseMaster.LastModifiedOn = vDateTime;
                purchaseMaster.BENumber = txtBENumber.Text.Trim();
                purchaseMaster.ProductType = txtIsRaw.Text.Trim();
                purchaseMaster.TransactionType = transactionType;
                purchaseMaster.ReceiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                purchaseMaster.Post = "N";
                purchaseMaster.WithVDS = cmbVDS.Text.Trim();
                purchaseMaster.ReturnId = txtPrePurId.Text.Trim();
                purchaseMaster.CustomHouse = txtCustomHouse.Text.Trim();
                purchaseMaster.CustomHouseCode = txtCustomHouseCode.Text.Trim();
                purchaseMaster.USDInvoiceValue = Convert.ToDecimal(txtUSDInvoiceValue.Text.Trim());
                purchaseMaster.LCDate = dtpLCDate.Value.ToString("yyyy-MMM-dd") + Convert.ToDateTime(vDateTime).ToString(" HH:mm:ss");

                purchaseMaster.LandedCost = Convert.ToDecimal(txtLandedCost.Text.Trim());
                purchaseMaster.BranchId = Program.BranchId;
                purchaseMaster.TDSAmount = Convert.ToDecimal(txtTDSAmount.Text.Trim());
                purchaseMaster.IsTDS = chkTDS.Checked ? "Y" : "N";
                purchaseMaster.AppVersion = Program.GetAppVersion();
                purchaseMaster.IsRebate = chkIsRebate.Checked ? "Y" : "N";
                purchaseMaster.RebatePeriodId = periodId;
                purchaseMaster.RebateDate = dtpRebateDate.Value.ToString("yyyy-MMM-dd");
                purchaseMaster.BankGuarantee = txtBankGuarantee.Text;
                purchaseMaster.VehicleNo = txtVehicleNo.Text;
                purchaseMaster.VehicleType = txtVehicleType.Text;

                if (rbtnImport.Checked || chkImport.Checked)
                {
                    purchaseMaster.IsBankingChannelPay = "Y";
                }
                else
                {
                    purchaseMaster.IsBankingChannelPay = "N";
                }

                #endregion Master

                #region duties
                if (
                 rbtnTradingImport.Checked
             || rbtnImport.Checked
             || rbtnServiceImport.Checked
             || rbtnServiceNSImport.Checked
             || rbtnInputServiceImport.Checked
             || rbtnCommercialImporter.Checked
                 )
                {
                    purchaseDuties = new List<PurchaseDutiesVM>();
                    for (int i = 0; i < dgvDuty.RowCount; i++)
                    {
                        PurchaseDutiesVM purchaseDuty = new PurchaseDutiesVM();

                        purchaseDuty.PIDutyID = "";
                        purchaseDuty.PurchaseInvoiceNo = NextID.ToString();
                        purchaseDuty.ItemNo = dgvDuty.Rows[i].Cells["ItemNoDuty"].Value.ToString();
                        purchaseDuty.Quantity = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                        purchaseDuty.CnFInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFInp"].Value.ToString());
                        purchaseDuty.CnFRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFRate"].Value.ToString());
                        purchaseDuty.CnFAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFAmount"].Value.ToString());
                        purchaseDuty.InsuranceInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceInp"].Value.ToString());
                        purchaseDuty.InsuranceRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceRate"].Value.ToString());
                        purchaseDuty.InsuranceAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceAmount"].Value.ToString());
                        purchaseDuty.AssessableInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableInp"].Value.ToString());
                        purchaseDuty.AssessableValue = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableValue"].Value.ToString());
                        purchaseDuty.CDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDInp"].Value.ToString());
                        purchaseDuty.CDRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDRate"].Value.ToString());
                        purchaseDuty.CDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDAmount"].Value.ToString());
                        purchaseDuty.RDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDInp"].Value.ToString());
                        purchaseDuty.RDRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDRate"].Value.ToString());
                        purchaseDuty.RDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDAmount"].Value.ToString());
                        purchaseDuty.TVBInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBInp"].Value.ToString());
                        purchaseDuty.TVBRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBRate"].Value.ToString());
                        purchaseDuty.TVBAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBAmount"].Value.ToString());
                        purchaseDuty.SDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDInp"].Value.ToString());
                        purchaseDuty.SD = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDuty"].Value.ToString());
                        purchaseDuty.SDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDutyAmount"].Value.ToString());
                        purchaseDuty.VATInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATInp"].Value.ToString());
                        purchaseDuty.VATRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATRateDuty"].Value.ToString());
                        purchaseDuty.VATAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATAmountDuty"].Value.ToString());
                        purchaseDuty.TVAInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAInp"].Value.ToString());
                        purchaseDuty.TVARate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVARate"].Value.ToString());
                        purchaseDuty.TVAAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAAmount"].Value.ToString());
                        purchaseDuty.ATVInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVInp"].Value.ToString());
                        purchaseDuty.ATVRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVRate"].Value.ToString());
                        purchaseDuty.ATVAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVAmount"].Value.ToString());
                        purchaseDuty.OthersInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersInp"].Value.ToString());
                        purchaseDuty.OthersRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersRate"].Value.ToString());
                        purchaseDuty.OthersAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersAmount"].Value.ToString());
                        purchaseDuty.AITInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITInp"].Value.ToString());
                        purchaseDuty.AITAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITAmount"].Value.ToString());
                        purchaseDuty.Remarks = dgvDuty.Rows[i].Cells["Remarks"].Value.ToString();
                        purchaseDuty.SetCost();
                        purchaseDuty.BranchId = Program.BranchId;

                        purchaseDuties.Add(purchaseDuty);
                    }
                }
                #endregion duties

                #region details
                purchaseDetails = new List<PurchaseDetailVM>();
                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {
                    PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();
                    purchaseDetailVm.BOMId = Convert.ToInt32(dgvPurchase.Rows[i].Cells["BOMId"].Value);

                    purchaseDetailVm.PurchaseInvoiceNo = NextID.ToString();
                    purchaseDetailVm.LineNo = dgvPurchase.Rows[i].Cells["LineNo"].Value.ToString();
                    purchaseDetailVm.ItemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                    purchaseDetailVm.Quantity = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                    purchaseDetailVm.UnitPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UnitPrice"].Value.ToString());
                    purchaseDetailVm.NBRPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["NBRPrice"].Value.ToString());
                    purchaseDetailVm.UOM = dgvPurchase.Rows[i].Cells["UOM"].Value.ToString();
                    purchaseDetailVm.VATRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VATRate"].Value.ToString());
                    purchaseDetailVm.SD = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SD"].Value.ToString());
                    purchaseDetailVm.VATAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VATAmount"].Value.ToString());
                    purchaseDetailVm.SubTotal = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SubTotal"].Value.ToString());
                    purchaseDetailVm.VDSRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VDSRate"].Value.ToString());
                    purchaseDetailVm.VDSAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VDSAmount"].Value.ToString());
                    purchaseDetailVm.Comments = dgvPurchase.Rows[i].Cells["Comments"].Value.ToString();
                    purchaseDetailVm.SDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SDAmount"].Value.ToString());
                    purchaseDetailVm.Type = dgvPurchase.Rows[i].Cells["Type"].Value.ToString();
                    purchaseDetailVm.ProductType = txtIsRaw.Text.Trim();
                    purchaseDetailVm.UOMQty = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMQty"].Value.ToString());
                    purchaseDetailVm.UOMn = dgvPurchase.Rows[i].Cells["UOMn"].Value.ToString();
                    purchaseDetailVm.UOMc = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMc"].Value.ToString());
                    purchaseDetailVm.UOMPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMPrice"].Value.ToString());
                    purchaseDetailVm.BENumber = txtBENumber.Text.Trim();
                    purchaseDetailVm.RebateRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Rebate"].Value.ToString());
                    purchaseDetailVm.RebateAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["RebateAmount"].Value.ToString());
                    purchaseDetailVm.CnFAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CnF"].Value.ToString());
                    purchaseDetailVm.InsuranceAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Insurance"].Value.ToString());
                    purchaseDetailVm.AssessableValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["AV"].Value.ToString());
                    purchaseDetailVm.CDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CD"].Value.ToString());
                    purchaseDetailVm.RDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["RD"].Value.ToString());
                    purchaseDetailVm.TVBAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["TVB"].Value.ToString());
                    purchaseDetailVm.TVAAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["TVA"].Value.ToString());
                    purchaseDetailVm.ATVAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["ATV"].Value.ToString());
                    purchaseDetailVm.AITAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["AIT"].Value.ToString());
                    purchaseDetailVm.OthersAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Others"].Value.ToString());
                    purchaseDetailVm.USDValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["USDValue"].Value.ToString());
                    purchaseDetailVm.USDVAT = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["USDVAT"].Value.ToString());
                    purchaseDetailVm.TDSSection = dgvPurchase.Rows[i].Cells["TDSSection"].Value.ToString();
                    purchaseDetailVm.TDSCode = dgvPurchase.Rows[i].Cells["TDSCode"].Value.ToString();

                    if (dgvPurchase.Rows[i].Cells["ExpireDate"].Value == null)
                    {
                        purchaseDetailVm.ExpireDate = "2100-01-01";
                    }
                    else
                    {
                        purchaseDetailVm.ExpireDate = dgvPurchase.Rows[i].Cells["ExpireDate"].Value.ToString();
                    }
                    if (dgvPurchase.Rows[i].Cells["CPCName"].Value != null)
                    {
                        purchaseDetailVm.CPCName = dgvPurchase.Rows[i].Cells["CPCName"].Value.ToString();

                    }
                    if (dgvPurchase.Rows[i].Cells["BEItemNo"].Value != null)
                    {
                        purchaseDetailVm.BEItemNo = dgvPurchase.Rows[i].Cells["BEItemNo"].Value.ToString();
                    }
                    purchaseDetailVm.HSCode = dgvPurchase.Rows[i].Cells["HSCode"].Value.ToString();

                    purchaseDetailVm.BranchId = Program.BranchId;
                    purchaseDetailVm.IsFixedVAT = dgvPurchase.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                    purchaseDetailVm.FixedVATAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["FixedVATAmount"].Value.ToString());

                    if (rbtnPurchaseDN.Checked || rbtnPurchaseCN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        if (dgvPurchase.Rows[i].Cells["ReturnTransactionType"].Value != null)
                        {
                            purchaseDetailVm.ReturnTransactionType = dgvPurchase.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                            purchaseDetailVm.ReasonOfReturn = dgvPurchase.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                        }
                    }
                    purchaseDetailVm.InvoiceValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["InvoiceValue"].Value.ToString());
                    purchaseDetailVm.ExchangeRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["ExchangeRate"].Value.ToString());
                    purchaseDetailVm.Currency = (dgvPurchase.Rows[i].Cells["Currency"].Value.ToString());
                    purchaseDetailVm.FixedVATRebate = dgvPurchase.Rows[i].Cells["FixedVATRebate"].Value.ToString();
                    purchaseDetailVm.Section21 = dgvPurchase.Rows[i].Cells["Section21"].Value == null
                        ? "N"
                        : dgvPurchase.Rows[i].Cells["Section21"].Value.ToString();

                    purchaseDetailVm.BranchId = Program.BranchId;
                    purchaseDetails.Add(purchaseDetailVm);

                }

                #endregion details

                #region Tracking
                purchaseTrackings = new List<TrackingVM>();

                if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                {
                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dgvSerialTrack["SelectT", i].Value) == true)
                        {
                            TrackingVM purchaseTrackingVm = new TrackingVM();
                            purchaseTrackingVm.ItemNo = dgvSerialTrack.Rows[i].Cells["ItemNoS"].Value.ToString();
                            purchaseTrackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                            purchaseTrackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();

                            purchaseTrackingVm.ReturnPurchase = "Y";
                            purchaseTrackingVm.ReturnPurchaseID = NextID.ToString();
                            purchaseTrackingVm.transactionType = "Purchase_Return";
                            purchaseTrackingVm.ReturnType = transactionType;
                            purchaseTrackingVm.ReturnPurDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + Convert.ToDateTime(vDateTime).ToString(" HH:mm:ss");
                            purchaseTrackingVm.BranchId = Program.BranchId;
                            purchaseTrackings.Add(purchaseTrackingVm);

                        }
                        //else
                        //{
                        //    purchaseTrackingVm.ReturnPurchase = "N";
                        //}

                        //purchaseTrackingVm.ReturnReceive = "N";
                        //purchaseTrackingVm.ReturnSale = "N";


                    }
                }
                else
                {
                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        TrackingVM purchaseTrackingVm = new TrackingVM();
                        purchaseTrackingVm.ItemNo = dgvSerialTrack.Rows[i].Cells["ItemNoS"].Value.ToString();
                        purchaseTrackingVm.TrackingLineNo = dgvSerialTrack.Rows[i].Cells["LineNoS"].Value.ToString();
                        purchaseTrackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                        purchaseTrackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();
                        purchaseTrackingVm.IsPurchase = dgvSerialTrack.Rows[i].Cells["StatusS"].Value.ToString();
                        purchaseTrackingVm.Quantity = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["QuantityS"].Value.ToString());
                        purchaseTrackingVm.UnitPrice = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["Value"].Value.ToString());
                        purchaseTrackingVm.IsIssue = "N";
                        purchaseTrackingVm.IsReceive = "N";
                        purchaseTrackingVm.IsSale = "N";
                        purchaseTrackingVm.BranchId = Program.BranchId;
                        purchaseTrackings.Add(purchaseTrackingVm);

                    }
                }

                #endregion Tracking

                #endregion

                #region Check Point

                if (chkImport.Checked && purchaseDuties.Count() <= 0)
                {
                    throw new Exception("Please insert Duties information for transaction");
                }
                else if (purchaseDetails.Count() <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                #region Tracking


                if (TrackingTrace == "Y")
                {
                    for (int i = 0; i < dgvPurchase.Rows.Count; i++)
                    {
                        string itemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());

                        var p = from productCmb in purchaseTrackings.ToList()
                                where productCmb.ItemNo == itemNo
                                select productCmb;

                        if (p != null && p.Any())
                        {
                            int ri = p.Count();
                            if (Quantity > ri || Quantity < ri)
                            {
                                MessageBox.Show("Please select " + Convert.ToInt32(Quantity) + " items.", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                }

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Purchase Value Check

                string settingValue = new CommonDAL().settingValue("Purchase", "BankingChannelPayRange", connVM);

                if (!rbtnImport.Checked || !chkImport.Checked)
                {
                    decimal totalAmount = 0, bankPay = 0;

                    if (!string.IsNullOrWhiteSpace(txtTotalSubTotal.Text.Trim()))
                    {
                        totalAmount = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(settingValue))
                    {
                        bankPay = Convert.ToDecimal(settingValue);
                    }

                    if (totalAmount > bankPay)
                    {
                        MessageBox.Show("Your Purchase Value Over " + bankPay + " Tk Please Pay Bill by Banking Channel. \n Other wise Rebate will Canceled. ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                #endregion

                #region Background Worker

                bgwSave.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage + "\n" + ex.StackTrace);
            }
            #endregion

        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                sqlResults = purchaseDal.PurchaseInsert(purchaseMaster, purchaseDetails, purchaseDuties, purchaseTrackings, null, null, Program.BranchId, connVM, Program.CurrentUserID);
                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage + "\n" + ex.StackTrace);
            }
            #endregion
        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Start

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];

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
                            txtPurchaseInvoiceNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvPurchase.RowCount; i++)
                            {
                                dgvPurchase["Status", dgvPurchase.RowCount - 1].Value = "Old";
                            }
                            TDSCalc();


                        }
                    }
                ChangeData = false;
                SearchBranchId = Program.BranchId;
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage + "\n" + ex.StackTrace);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            #region variable

            FiscalYearVM varFiscalYearVM = new FiscalYearVM();

            string periodId = "0";

            #endregion

            try
            {

                #region Find Rebate Period Id

                string RebatePeriod = dtpRebateDate.Value.ToString("MMMM-yyyy");
                string[] conditionValues = { RebatePeriod };
                string[] conditionFields = { "PeriodName" };

                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, conditionFields, conditionValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(RebatePeriod + ": This Fiscal Period is not Exist!");
                }

                periodId = varFiscalYearVM.PeriodID;

                #endregion

                #region Check Point


                #region Find Fiscal Year Lock

                string PeriodName = dtpReceiveDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };

                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //if (Edit == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
                //    return;
                //}

                #endregion

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Check Point

                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}


                if (Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")) > Convert.ToDateTime(dtpReceiveDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Receive date  and time not before invoice date", this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");

                    return;
                }

                if (rbtnImport.Checked)
                {
                    if (txtCustomHouse.Text == "")
                    {
                        MessageBox.Show("Please Select Custom House");
                        txtCustomHouse.Focus();
                        return;
                    }

                }
                result = string.Empty;



                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtHiddenInvoiceNo.Text.Trim();
                }
                #endregion

                #region Null Check

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtBENumber.Text == "")
                {
                    MessageBox.Show("Please Input BE/6.3 No");
                    return;
                }
                if (
                   transactionType.ToLower() == "import"
                   || transactionType.ToLower() == "tradingimport"
                   || transactionType.ToLower() == "servicensimport"
                   || transactionType.ToLower() == "serviceimport"

                   )
                {

                    if (txtLCNumber.Text == "")
                    {
                        MessageBox.Show("Please Input LC No");
                        return;
                    }
                }
                else
                {

                    if (txtLCNumber.Text == "")
                    {
                        txtLCNumber.Text = "-";

                    }
                }
                //   if (txtBENumber.Text == "")
                //   {

                //       if (
                //       transactionType.ToLower() == "import"
                //       || transactionType.ToLower() == "tradingimport"
                //       || transactionType.ToLower() == "servicensimport"
                //       || transactionType.ToLower() == "serviceimport"

                //       )
                //       {

                //       MessageBox.Show("Please Input BE No");
                //       return;
                //       }

                //   }
                //else
                //       {
                //       //txtBENumber.Text = "-";
                //       }
                //   if (txtLCNumber.Text == "")
                //   {
                //       txtLCNumber.Text = "-";
                //   }


                if (dgvPurchase.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }
                #endregion null

                #region Rate Check

                if (rbtnOther.Checked || rbtnTrading.Checked || rbtnTradingImport.Checked || rbtnImport.Checked || rbtnCommercialImporter.Checked)
                {
                    RateCheck();
                }

                #endregion

                #region Tracking

                if (TrackingTrace == "Y")
                {
                    if (dgvSerialTrack.Rows.Count == 0 && purchaseTrackings.Count == 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information not added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,

                         MessageBoxButtons.YesNo,

                         MessageBoxIcon.Question,

                         MessageBoxDefaultButton.Button2))
                        {
                            tcPurchase.SelectedTab = tabPageSerial;
                            return;

                        }

                    }
                    if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        string item = "";
                        decimal qty, returnItems = 0;

                        for (int i = 0; i < dgvPurchase.RowCount; i++)
                        {
                            item = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                            qty = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                            returnItems = CountReturnTrackingItem(item);
                            if (qty != returnItems)
                            {
                                MessageBox.Show("Please select tracking quantity " + qty + " for Item Code : " + dgvPurchase.Rows[i].Cells["PCode"].Value.ToString() + ".",
                                    this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                tcPurchase.SelectedTab = tabPageSerial;
                                return;
                            }
                        }

                    }
                }

                #endregion

                #region Value Assign

                #region Master Value Assign

                string vdateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                purchaseMaster = new PurchaseMasterVM();
                purchaseMaster.PurchaseInvoiceNo = NextID.ToString();
                purchaseMaster.VendorID = txtVendorID.Text.Trim();
                purchaseMaster.InvoiceDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd");// +DateTime.Now.ToString(" HH:mm:ss");
                purchaseMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                purchaseMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                purchaseMaster.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                purchaseMaster.LCNumber = txtLCNumber.Text.Trim().Replace(" ", "");
                purchaseMaster.Comments = txtComments.Text.Trim();
                purchaseMaster.CreatedBy = Program.CurrentUser;
                purchaseMaster.CreatedOn = vdateTime;
                purchaseMaster.LastModifiedBy = Program.CurrentUser;
                purchaseMaster.LastModifiedOn = vdateTime;

                purchaseMaster.BENumber = txtBENumber.Text.Trim();
                purchaseMaster.ProductType = txtIsRaw.Text.Trim();
                purchaseMaster.TransactionType = transactionType;
                purchaseMaster.ReceiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm");// +DateTime.Now.ToString(" HH:mm:ss");
                purchaseMaster.Post = "N";
                purchaseMaster.IsTDS = chkTDS.Checked ? "Y" : "N";
                purchaseMaster.WithVDS = cmbVDS.Text.Trim();
                purchaseMaster.ReturnId = txtPrePurId.Text.Trim();
                purchaseMaster.CustomHouse = txtCustomHouse.Text.Trim();
                purchaseMaster.CustomHouseCode = txtCustomHouseCode.Text.Trim();
                purchaseMaster.USDInvoiceValue = Convert.ToDecimal(txtUSDInvoiceValue.Text.Trim());
                purchaseMaster.TDSAmount = Convert.ToDecimal(txtTDSAmount.Text.Trim());
                purchaseMaster.BranchId = Program.BranchId;
                purchaseMaster.IsRebate = chkIsRebate.Checked ? "Y" : "N";
                purchaseMaster.RebatePeriodId = periodId;
                purchaseMaster.RebateDate = dtpRebateDate.Value.ToString("yyyy-MMM-dd");
                if (txtLCNumber.Text != "-")
                {
                    purchaseMaster.LCDate = dtpLCDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                }
                purchaseMaster.LandedCost = Convert.ToDecimal(txtLandedCost.Text.Trim());
                purchaseMaster.BankGuarantee = txtBankGuarantee.Text;
                purchaseMaster.VehicleNo = txtVehicleNo.Text;
                purchaseMaster.VehicleType = txtVehicleType.Text;

                #endregion Master

                decimal amnt = 1.0M;

                #region Duties Value Assign
                if (
                 rbtnTradingImport.Checked
             || rbtnImport.Checked
             || rbtnServiceImport.Checked
             || rbtnServiceNSImport.Checked
             || rbtnInputServiceImport.Checked
             || rbtnCommercialImporter.Checked
                 )
                {
                    purchaseDuties = new List<PurchaseDutiesVM>();
                    for (int i = 0; i < dgvDuty.RowCount; i++)
                    {
                        PurchaseDutiesVM purchaseDuty = new PurchaseDutiesVM();

                        purchaseDuty.PIDutyID = "";
                        purchaseDuty.PurchaseInvoiceNo = NextID.ToString();
                        string strItemNo = dgvDuty.Rows[i].Cells["ItemNoDuty"].Value.ToString();
                        if (string.IsNullOrEmpty(strItemNo))
                        {
                            throw new ArgumentNullException("Update", "Cannot find duty item no");
                        }
                        purchaseDuty.ItemNo = strItemNo;

                        purchaseDuty.Quantity = dgvPurchase.RowCount <= i ? amnt : Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());

                        purchaseDuty.CnFInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFInp"].Value.ToString());
                        purchaseDuty.CnFRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFRate"].Value.ToString());
                        purchaseDuty.CnFAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFAmount"].Value.ToString());
                        purchaseDuty.InsuranceInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceInp"].Value.ToString());
                        purchaseDuty.InsuranceRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceRate"].Value.ToString());
                        purchaseDuty.InsuranceAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceAmount"].Value.ToString());
                        purchaseDuty.AssessableInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableInp"].Value.ToString());
                        purchaseDuty.AssessableValue = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableValue"].Value.ToString());
                        purchaseDuty.CDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDInp"].Value.ToString());
                        purchaseDuty.CDRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDRate"].Value.ToString());
                        purchaseDuty.CDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDAmount"].Value.ToString());
                        purchaseDuty.RDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDInp"].Value.ToString());
                        purchaseDuty.RDRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDRate"].Value.ToString());
                        purchaseDuty.RDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDAmount"].Value.ToString());
                        purchaseDuty.TVBInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBInp"].Value.ToString());
                        purchaseDuty.TVBRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBRate"].Value.ToString());
                        purchaseDuty.TVBAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBAmount"].Value.ToString());
                        purchaseDuty.SDInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDInp"].Value.ToString());
                        purchaseDuty.SD = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDuty"].Value.ToString());
                        purchaseDuty.SDAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDutyAmount"].Value.ToString());
                        purchaseDuty.VATInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATInp"].Value.ToString());
                        purchaseDuty.VATRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATRateDuty"].Value.ToString());
                        purchaseDuty.VATAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATAmountDuty"].Value.ToString());
                        purchaseDuty.TVAInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAInp"].Value.ToString());
                        purchaseDuty.TVARate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVARate"].Value.ToString());
                        purchaseDuty.TVAAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAAmount"].Value.ToString());
                        purchaseDuty.ATVInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVInp"].Value.ToString());
                        purchaseDuty.ATVRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVRate"].Value.ToString());
                        purchaseDuty.ATVAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVAmount"].Value.ToString());
                        purchaseDuty.OthersInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersInp"].Value.ToString());
                        purchaseDuty.OthersRate = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersRate"].Value.ToString());

                        purchaseDuty.OthersAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersAmount"].Value.ToString());
                        purchaseDuty.AITInp = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITInp"].Value.ToString());
                        purchaseDuty.AITAmount = Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITAmount"].Value.ToString());

                        purchaseDuty.Remarks = dgvDuty.Rows[i].Cells["Remarks"].Value.ToString();
                        purchaseDuty.BranchId = Program.BranchId;

                        purchaseDuty.SetCost();
                        purchaseDuties.Add(purchaseDuty);
                    }
                }
                #endregion duties

                #region Details Value Assign

                purchaseDetails = new List<PurchaseDetailVM>();
                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {
                    PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();

                    purchaseDetailVm.BOMId = Convert.ToInt32(dgvPurchase.Rows[i].Cells["BOMId"].Value);
                    purchaseDetailVm.PurchaseInvoiceNo = NextID.ToString();
                    purchaseDetailVm.LineNo = dgvPurchase.Rows[i].Cells["LineNo"].Value.ToString();
                    purchaseDetailVm.ItemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                    purchaseDetailVm.Quantity = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());
                    purchaseDetailVm.UnitPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UnitPrice"].Value.ToString());
                    purchaseDetailVm.NBRPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["NBRPrice"].Value.ToString());
                    purchaseDetailVm.UOM = dgvPurchase.Rows[i].Cells["UOM"].Value.ToString();
                    purchaseDetailVm.VATRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VATRate"].Value.ToString());
                    purchaseDetailVm.VATAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VATAmount"].Value.ToString());
                    purchaseDetailVm.SubTotal = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SubTotal"].Value.ToString());
                    purchaseDetailVm.Comments = dgvPurchase.Rows[i].Cells["Comments"].Value.ToString();
                    purchaseDetailVm.VDSRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VDSRate"].Value.ToString());
                    purchaseDetailVm.VDSAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["VDSAmount"].Value.ToString());

                    purchaseDetailVm.SD = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SD"].Value.ToString());
                    purchaseDetailVm.SDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["SDAmount"].Value.ToString());
                    purchaseDetailVm.Type = dgvPurchase.Rows[i].Cells["Type"].Value.ToString();
                    purchaseDetailVm.ProductType = txtIsRaw.Text.Trim();
                    purchaseDetailVm.UOMQty = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMQty"].Value.ToString());
                    purchaseDetailVm.UOMn = dgvPurchase.Rows[i].Cells["UOMn"].Value.ToString();
                    purchaseDetailVm.UOMc = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMc"].Value.ToString());
                    purchaseDetailVm.UOMPrice = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["UOMPrice"].Value.ToString());
                    purchaseDetailVm.BENumber = txtBENumber.Text.Trim();

                    purchaseDetailVm.RebateRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Rebate"].Value.ToString());
                    purchaseDetailVm.RebateAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["RebateAmount"].Value.ToString());
                    purchaseDetailVm.CnFAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CnF"].Value.ToString());
                    purchaseDetailVm.InsuranceAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Insurance"].Value.ToString());
                    purchaseDetailVm.AssessableValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["AV"].Value.ToString());
                    purchaseDetailVm.CDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["CD"].Value.ToString());
                    purchaseDetailVm.RDAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["RD"].Value.ToString());
                    purchaseDetailVm.TVBAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["TVB"].Value.ToString());
                    purchaseDetailVm.TVAAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["TVA"].Value.ToString());
                    purchaseDetailVm.ATVAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["ATV"].Value.ToString());
                    purchaseDetailVm.AITAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["AIT"].Value.ToString());
                    purchaseDetailVm.OthersAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Others"].Value.ToString());
                    purchaseDetailVm.USDValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["USDValue"].Value.ToString());
                    purchaseDetailVm.USDVAT = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["USDVAT"].Value.ToString());
                    purchaseDetailVm.TDSSection = dgvPurchase.Rows[i].Cells["TDSSection"].Value.ToString();
                    purchaseDetailVm.TDSCode = dgvPurchase.Rows[i].Cells["TDSCode"].Value.ToString();
                    purchaseDetailVm.ExpireDate = dgvPurchase.Rows[i].Cells["ExpireDate"].Value.ToString();
                    purchaseDetailVm.CPCName = dgvPurchase.Rows[i].Cells["CPCName"].Value.ToString();
                    purchaseDetailVm.BEItemNo = dgvPurchase.Rows[i].Cells["BEItemNo"].Value.ToString();
                    purchaseDetailVm.HSCode = dgvPurchase.Rows[i].Cells["HSCode"].Value.ToString();
                    purchaseDetailVm.Section21 = dgvPurchase.Rows[i].Cells["Section21"].Value == null
                        ? "N"
                        : dgvPurchase.Rows[i].Cells["Section21"].Value.ToString();
                    //purchaseDetailVm.OtherRef = dgvPurchase.Rows[i].Cells["OtherRef"].Value.ToString();

                    purchaseDetailVm.BranchId = Program.BranchId;
                    if (rbtnPurchaseDN.Checked || rbtnPurchaseCN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        if (dgvPurchase.Rows[i].Cells["ReturnTransactionType"].Value != null)
                        {
                            purchaseDetailVm.ReturnTransactionType = dgvPurchase.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                            purchaseDetailVm.ReasonOfReturn = dgvPurchase.Rows[i].Cells["ReasonOfReturn"].Value.ToString();
                        }
                    }
                    purchaseDetailVm.IsFixedVAT = dgvPurchase.Rows[i].Cells["IsFixedVAT"].Value.ToString();
                    purchaseDetailVm.FixedVATAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["FixedVATAmount"].Value.ToString());
                    purchaseDetailVm.InvoiceValue = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["InvoiceValue"].Value.ToString());
                    purchaseDetailVm.ExchangeRate = Convert.ToDecimal(dgvPurchase.Rows[i].Cells["ExchangeRate"].Value.ToString());
                    purchaseDetailVm.Currency = (dgvPurchase.Rows[i].Cells["Currency"].Value.ToString());
                    purchaseDetailVm.FixedVATRebate = (dgvPurchase.Rows[i].Cells["FixedVATRebate"].Value.ToString());

                    purchaseDetails.Add(purchaseDetailVm);
                }
                #endregion details

                #region Tracking
                if (purchaseTrackings.Count <= 0)
                {

                    purchaseTrackings = new List<TrackingVM>();

                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        TrackingVM purchaseTrackingVm = new TrackingVM();
                        purchaseTrackingVm.ItemNo = dgvSerialTrack.Rows[i].Cells["ItemNoS"].Value.ToString();
                        purchaseTrackingVm.TrackingLineNo = dgvSerialTrack.Rows[i].Cells["LineNoS"].Value.ToString();
                        purchaseTrackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                        purchaseTrackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();
                        purchaseTrackingVm.IsPurchase = dgvSerialTrack.Rows[i].Cells["StatusS"].Value.ToString();
                        purchaseTrackingVm.Quantity = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["QuantityS"].Value.ToString());
                        if (string.IsNullOrEmpty(dgvSerialTrack.Rows[i].Cells["Value"].Value.ToString()))
                        {
                            purchaseTrackingVm.UnitPrice = Convert.ToDecimal(dgvPurchase.Rows[0].Cells["UnitPrice"].Value.ToString());
                        }
                        else
                        {
                            purchaseTrackingVm.UnitPrice = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["Value"].Value.ToString());

                        }

                        if (Convert.ToBoolean(dgvSerialTrack["SelectT", i].Value) == true)
                        {
                            purchaseTrackingVm.ReturnType = transactionType;
                            purchaseTrackingVm.ReturnPurchase = "Y";
                            purchaseTrackingVm.ReturnPurchaseID = NextID.ToString();
                            purchaseTrackingVm.transactionType = "Purchase_Return";
                            purchaseTrackingVm.ReturnPurDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");

                        }
                        else
                        {
                            purchaseTrackingVm.ReturnPurchase = "N";
                            purchaseTrackingVm.ReturnPurchaseID = "";
                            purchaseTrackingVm.ReturnType = "";
                        }
                        purchaseTrackingVm.BranchId = Program.BranchId;
                        purchaseTrackings.Add(purchaseTrackingVm);

                    }
                }

                #endregion Tracking

                #endregion

                #region Check Point

                if (chkImport.Checked && purchaseDuties.Count() <= 0)
                {
                    MessageBox.Show("Please insert Duties information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                else if (purchaseDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Purchase Value Check

                string settingValue = new CommonDAL().settingValue("Purchase", "BankingChannelPayRange", connVM);

                if (!rbtnImport.Checked || !chkImport.Checked)
                {
                    decimal totalAmount = 0, bankPay = 0;

                    if (!string.IsNullOrWhiteSpace(txtTotalSubTotal.Text.Trim()))
                    {
                        totalAmount = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(settingValue))
                    {
                        bankPay = Convert.ToDecimal(settingValue);
                    }

                    if (totalAmount > bankPay)
                    {
                        MessageBox.Show("Your Purchase Value Over " + bankPay + " Tk Please Pay Bill by Banking Channel. \n Other wise Rebate will Canceled. ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                #endregion

                #region Background Worker - Update

                bgwUpdate.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

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
                FileLogger.Log(this.Name, "bthUpdate_Click", exMessage);
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

                if (TrackingTrace == "Y")
                {
                    //TrackingDAL trackingDal = new TrackingDAL();
                    ITracking trackingDal = OrdinaryVATDesktop.GetObject<TrackingDAL, TrackingRepo, ITracking>(OrdinaryVATDesktop.IsWCF);

                    string result = trackingDal.TrackingDelete(Headings, connVM);
                }
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                sqlResults = purchaseDal.PurchaseUpdate(purchaseMaster, purchaseDetails, purchaseDuties, purchaseTrackings, connVM, Program.CurrentUserID);
                UPDATE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {


                #region Start

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
                            txtPurchaseInvoiceNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvPurchase.RowCount; i++)
                            {
                                dgvPurchase["Status", dgvPurchase.RowCount - 1].Value = "Old";
                            }
                            TDSCalc();

                        }
                    }

                #endregion

                ChangeData = false;

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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;

            }


        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }


                #region Find Fiscal Year Lock

                string PeriodName = dtpReceiveDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion

                if (SearchBranchId != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd")) > Convert.ToDateTime(dtpReceiveDate.Value.ToString("yyyy-MMM-dd")))
                {
                    MessageBox.Show("Receive date  and time not before invoice date", this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                result = string.Empty;



                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtHiddenInvoiceNo.Text.Trim();
                }

                #region Null Check

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtLCNumber.Text == "") //LCNumber
                {
                    txtLCNumber.Text = "-";
                }
                if (txtBENumber.Text == "")
                {
                    txtBENumber.Text = "-";
                }



                if (dgvPurchase.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }
                #endregion

                #endregion

                #region Transaction Types

                transactionTypes();

                #endregion

                #region Master
                purchaseMaster = new PurchaseMasterVM();
                purchaseMaster.PurchaseInvoiceNo = NextID.ToString();
                //purchaseMaster.VendorID = txtVendorID.Text.Trim();
                //purchaseMaster.InvoiceDate = Convert.ToDateTime(dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                //purchaseMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                //purchaseMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                //purchaseMaster.SerialNo = txtSerialNo.Text.Trim();
                //purchaseMaster.Comments = txtComments.Text.Trim();
                //purchaseMaster.CreatedBy = Program.CurrentUser;
                //purchaseMaster.CreatedOn = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));
                purchaseMaster.LastModifiedBy = Program.CurrentUser;
                purchaseMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //purchaseMaster.BENumber = txtBENumber.Text.Trim();
                //purchaseMaster.ProductType = txtIsRaw.Text.Trim();
                purchaseMaster.TransactionType = transactionType;
                purchaseMaster.ReceiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +DateTime.Now.ToString(" HH:mm:ss");
                purchaseMaster.RebateDate = dtpRebateDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +DateTime.Now.ToString(" HH:mm:ss");
                purchaseMaster.Post = "Y";
                purchaseMaster.BranchId = Program.BranchId;


                #endregion Master

                #region duties
                if (
                rbtnTradingImport.Checked
            || rbtnImport.Checked
            || rbtnServiceImport.Checked
            || rbtnServiceNSImport.Checked
            || rbtnInputServiceImport.Checked
            || rbtnCommercialImporter.Checked
                )
                {
                    purchaseDuties = new List<PurchaseDutiesVM>();
                    for (int i = 0; i < dgvDuty.RowCount; i++)
                    {
                        PurchaseDutiesVM purchaseDuty = new PurchaseDutiesVM();

                        purchaseDuty.PIDutyID = "";
                        purchaseDuty.PurchaseInvoiceNo = NextID.ToString();
                        purchaseDuty.ItemNo = dgvDuty.Rows[i].Cells["ItemNoDuty"].Value.ToString();

                        purchaseDuties.Add(purchaseDuty);
                    }
                }
                #endregion duties

                #region details
                purchaseDetails = new List<PurchaseDetailVM>();
                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {
                    PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();

                    purchaseDetailVm.PurchaseInvoiceNo = NextID.ToString();
                    purchaseDetailVm.ItemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                    purchaseDetails.Add(purchaseDetailVm);
                }
                #endregion details

                #region Tracking
                if (purchaseTrackings.Count <= 0)
                {

                    purchaseTrackings = new List<TrackingVM>();

                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        TrackingVM purchaseTrackingVm = new TrackingVM();

                        purchaseTrackingVm.ItemNo = dgvSerialTrack.Rows[i].Cells["ItemNoS"].Value.ToString();
                        purchaseTrackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                        purchaseTrackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();

                        purchaseTrackings.Add(purchaseTrackingVm);

                    }
                }
                #endregion Tracking

                #region Check Point

                if (chkImport.Checked && purchaseDuties.Count() <= 0)
                {
                    MessageBox.Show("Please insert Duties information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                else if (purchaseDetails.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Button Stats

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Purchase Value Check

                string settingValue = new CommonDAL().settingValue("Purchase", "BankingChannelPayRange", connVM);

                if (!rbtnImport.Checked || !chkImport.Checked)
                {
                    decimal totalAmount = 0, bankPay = 0;

                    if (!string.IsNullOrWhiteSpace(txtTotalSubTotal.Text.Trim()))
                    {
                        totalAmount = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(settingValue))
                    {
                        bankPay = Convert.ToDecimal(settingValue);
                    }

                    if (totalAmount > bankPay)
                    {
                        MessageBox.Show("Your Purchase Value Over " + bankPay + " Tk Please Pay Bill by Banking Channel. \n Other wise Rebate will Canceled. ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                #endregion

                #region Background Worker - Post

                bgwPost.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion

        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Statement

                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                sqlResults = purchaseDal.PurchasePost(purchaseMaster, purchaseDetails, purchaseDuties, purchaseTrackings, null, null, connVM);

                POST_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {


                #region Start

                if (POST_DOWORK_SUCCESS)
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
                            txtPurchaseInvoiceNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvPurchase.RowCount; i++)
                            {
                                dgvPurchase["Status", dgvPurchase.RowCount - 1].Value = "Old";
                            }
                        }
                    }

                #endregion

                ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;

            }


        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                ClearAllFields();

                FormMaker();

                FormLoad();

                IsUpdate = false;

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
                tabPage2.Text = this.Text;
                NextID = string.Empty;
                txtHiddenInvoiceNo.Text = string.Empty;

                #endregion
                IsPost = false;
                this.progressBar1.Visible = true;
                bgwLoad.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "FormPurchase_Load", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;

            }
        }

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {

                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region TransactionType

                transactionTypes();

                #endregion TransactionType

                #region Statement

                DataGridViewRow selectedRow = null;

                selectedRow = FormPurchaseSearch.SelectOne(transactionType);

                #region Value Assign to Form Elements


                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtPurchaseInvoiceNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    txtHiddenInvoiceNo.Text = txtPurchaseInvoiceNo.Text;

                    SearchInvoice();
                }

                #endregion


                #endregion

            }
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;

            }
        }

        private void SearchInvoice()
        {
            try
            {

                #region Flag Update

                dutyCalculate = false;
                boolsearch = true;


                #endregion

                IPurchase _purchaseDAL = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                DataTable dt = new DataTable();

                #region Data Call

                string[] cValues = { txtPurchaseInvoiceNo.Text.Trim(), transactionType };
                string[] cFields = { "pih.PurchaseInvoiceNo like", "pih.TransactionType like" };
                dt = _purchaseDAL.SelectAll(0, cFields, cValues, null, null, null, true, connVM, false);

                #endregion

                #region Value Assing to Form Elements

                DataRow dr = dt.Rows[0];

                txtId.Text = dr["Id"].ToString();
                txtFiscalYear.Text = dr["FiscalYear"].ToString();
                SearchBranchId = Convert.ToInt32(dr["BranchId"]);
                txtTDSAmount.Text = Program.ParseDecimalObject(dr["TDSAmount"].ToString());
                txtVendorID.Text = dr["VendorID"].ToString();
                txtVendorName.Text = dr["VendorName"].ToString();
                cmbVendor.Text = dr["VendorName"].ToString();
                txtVendorGroupName.Text = dr["VendorGroupName"].ToString();
                dtpInvoiceDate.Value = Convert.ToDateTime(dr["InvoiceDateTime"].ToString());
                txtTotalAmount.Text = Program.ParseDecimalObject(dr["TotalAmount"].ToString());
                txtTotalVATAmount.Text = Program.ParseDecimalObject(dr["TotalVATAmount"].ToString());
                txtSerialNo.Text = dr["SerialNo"].ToString();
                txtLCNumber.Text = dr["LCNumber"].ToString();
                txtComments.Text = dr["Comments"].ToString();
                IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                txtBENumber.Text = dr["BENumber"].ToString();
                cmbVDS.Text = dr["WithVDS"].ToString();
                txtPrePurId.Text = dr["PurchaseReturnId"].ToString();
                txtCustomHouse.Text = dr["CustomHouse"].ToString();
                txtCustomHouseCode.Text = dr["CustomCode"].ToString();

                txtUSDInvoiceValue.Text = dr["USDInvoiceValue"].ToString();
                chkTDS.Checked = dr["IsTDS"] != null && dr["IsTDS"].ToString() == "Y";
                dtpReceiveDate.Value = Convert.ToDateTime(dr["ReceiveDate"].ToString());
                dtpRebateDate.Value = Convert.ToDateTime(dr["RebateDate"].ToString());
                chkIsRebate.Checked = dr["IsRebate"].ToString() == "Y";
                txtBankGuarantee.Text = dr["BankGuarantee"].ToString();
                txtVehicleNo.Text = dr["VehicleNo"].ToString();
                txtVehicleType.Text = dr["VehicleType"].ToString();


                #region Conditional Values

                if (txtPurchaseInvoiceNo.Text == "")
                {
                    PurchaseDetailData = "00";
                }
                else
                {
                    PurchaseDetailData = txtPurchaseInvoiceNo.Text.Trim();
                }
                if (dr["LCNumber"].ToString() != "-")
                {
                    if (!string.IsNullOrEmpty(dr["LCDate"].ToString()))
                    {
                        dtpLCDate.Value = Convert.ToDateTime(dr["LCDate"].ToString());
                    }
                }

                #endregion

                txtLandedCost.Text = Program.ParseDecimalObject(dr["LandedCost"].ToString());

                #endregion

                #region Background Workder

                bgwSearchPurchase.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnFirst.Enabled = false;
                this.btnPrevious.Enabled = false;
                this.btnNext.Enabled = false;
                this.btnLast.Enabled = false;


                this.btnSearchInvoiceNo.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion
            }
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
                FileLogger.Log(this.Name, "SearchInvoice", exMessage);
            }
            #endregion

        }

        private void bgwSearchPurchase_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                PurchaseDetailResult = new DataTable();

                string[] cVals = new string[] { PurchaseDetailData, transactionType };
                string[] cFields = new string[] { "pd.PurchaseInvoiceNo like", "pd.TransactionType" };
                PurchaseDetailResult = purchaseDal.SelectPurchaseDetail(null, cFields, cVals, null, null, true, connVM);
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

                dgvPurchase.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in PurchaseDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvPurchase.Rows.Add(NewRow);
                    dgvPurchase.Rows[j].Cells["BOMId"].Value = item2["BOMId"].ToString();
                    dgvPurchase.Rows[j].Cells["LineNo"].Value = item2["POLineNo"].ToString();//= PurchaseDetailFields[1].ToString();
                    dgvPurchase.Rows[j].Cells["ItemNo"].Value = item2["ItemNo"].ToString();// PurchaseDetailFields[2].ToString();
                    dgvPurchase.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item2["Quantity"].ToString());// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item2["CostPrice"].ToString());// Convert.ToDecimal(PurchaseDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item2["NBRPrice"].ToString());// Convert.ToDecimal(PurchaseDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["UOM"].Value = item2["UOM"].ToString();// PurchaseDetailFields[6].ToString();
                    dgvPurchase.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item2["VATRate"].ToString());// Convert.ToDecimal(PurchaseDetailFields[7].ToString()).ToString();//"0.00");
                    dgvPurchase.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item2["VATAmount"].ToString());// Convert.ToDecimal(PurchaseDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item2["SubTotal"].ToString());// Convert.ToDecimal(PurchaseDetailFields[9].ToString()).ToString();//"0,00.00");
                    dgvPurchase.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// PurchaseDetailFields[10].ToString();
                    dgvPurchase.Rows[j].Cells["ItemName"].Value = item2["ProductName"].ToString();// PurchaseDetailFields[11].ToString();
                    dgvPurchase.Rows[j].Cells["Status"].Value = "Old";// PurchaseDetailFields[12].ToString();
                    dgvPurchase.Rows[j].Cells["Previous"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item2["Stock"].ToString());// Convert.ToDecimal(PurchaseDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["Change"].Value = 0;
                    dgvPurchase.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item2["SD"].ToString());// Convert.ToDecimal(PurchaseDetailFields[13].ToString()).ToString();//"0.00");
                    dgvPurchase.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item2["SDAmount"].ToString());// Convert.ToDecimal(PurchaseDetailFields[14].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["Type"].Value = item2["Type"].ToString();// PurchaseDetailFields[15].ToString();
                    dgvPurchase.Rows[j].Cells["PCode"].Value = item2["ProductCode"].ToString();// PurchaseDetailFields[16].ToString();
                    dgvPurchase.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item2["UOMQty"].ToString());// Convert.ToDecimal(PurchaseDetailFields[17].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UOMn"].Value = item2["UOMn"].ToString();// PurchaseDetailFields[18].ToString();
                    dgvPurchase.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item2["UOMc"].ToString());// Convert.ToDecimal(PurchaseDetailFields[19].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item2["UOMPrice"].ToString());//Convert.ToDecimal(PurchaseDetailFields[20].ToString()).ToString();//"0,0.00");

                    dgvPurchase.Rows[j].Cells["Rebate"].Value = Program.ParseDecimalObject(item2["RebateRate"].ToString());
                    dgvPurchase.Rows[j].Cells["RebateAmount"].Value = Program.ParseDecimalObject(item2["RebateAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CnF"].Value = Program.ParseDecimalObject(item2["CnFAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["Insurance"].Value = Program.ParseDecimalObject(item2["InsuranceAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["AV"].Value = Program.ParseDecimalObject(item2["AssessableValue"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["RD"].Value = Program.ParseDecimalObject(item2["RDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["TVB"].Value = Program.ParseDecimalObject(item2["TVBAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["TVA"].Value = Program.ParseDecimalObject(item2["TVAAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["ATV"].Value = Program.ParseDecimalObject(item2["ATVAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["VDSRate"].Value = Program.ParseDecimalObject(item2["VDSRate"].ToString());
                    dgvPurchase.Rows[j].Cells["VDSAmount"].Value = Program.ParseDecimalObject(item2["VDSAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["Others"].Value = Program.ParseDecimalObject(item2["OthersAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["AIT"].Value = Program.ParseDecimalObject(item2["AITAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["ReturnTransactionType"].Value = item2["ReturnTransactionType"].ToString();
                    dgvPurchase.Rows[j].Cells["USDValue"].Value = Program.ParseDecimalObject(item2["USDValue"].ToString());
                    dgvPurchase.Rows[j].Cells["USDVAT"].Value = Program.ParseDecimalObject(item2["USDVAT"].ToString());
                    dgvPurchase.Rows[j].Cells["VATableValue"].Value = Program.ParseDecimalObject(item2["VATableValue"].ToString());
                    dgvPurchase.Rows[j].Cells["TDSSection"].Value = item2["TDSSection"].ToString();
                    dgvPurchase.Rows[j].Cells["TDSCode"].Value = item2["TDSCode"].ToString();
                    dgvPurchase.Rows[j].Cells["FixedVATAmount"].Value = Program.ParseDecimalObject(item2["FixedVATAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["InvoiceValue"].Value = Program.ParseDecimalObject(item2["InvoiceValue"].ToString());
                    dgvPurchase.Rows[j].Cells["ExchangeRate"].Value = Program.ParseDecimalObject(item2["ExchangeRate"].ToString());
                    dgvPurchase.Rows[j].Cells["Currency"].Value = item2["Currency"].ToString();
                    dgvPurchase.Rows[j].Cells["IsFixedVAT"].Value = item2["IsFixedVAT"].ToString();
                    dgvPurchase.Rows[j].Cells["ExpireDate"].Value = item2["ExpireDate"].ToString();
                    dgvPurchase.Rows[j].Cells["CPCName"].Value = item2["CPCName"].ToString();
                    dgvPurchase.Rows[j].Cells["BEItemNo"].Value = item2["BEItemNo"].ToString();
                    dgvPurchase.Rows[j].Cells["HSCode"].Value = item2["HSCode"].ToString();
                    dgvPurchase.Rows[j].Cells["FixedVATRebate"].Value = item2["FixedVATRebate"].ToString();
                    dgvPurchase.Rows[j].Cells["OtherRef"].Value = item2["OtherRef"].ToString();
                    dgvPurchase.Rows[j].Cells["Section21"].Value = item2["Section21"].ToString();
                    dgvPurchase.Rows[j].Cells["ReasonOfReturn"].Value = item2["ReasonOfReturn"].ToString();


                    j = j + 1;
                }


                #region Duties Methods

                if (
                    rbtnTradingImport.Checked
                || rbtnImport.Checked
                || rbtnServiceImport.Checked
                || rbtnServiceNSImport.Checked
                || rbtnInputServiceImport.Checked
                || rbtnCommercialImporter.Checked
                    )
                {
                    bgwDutiesSearch.RunWorkerAsync();
                }
                else
                {
                    Rowcalculate();

                }

                #endregion


                #region Flag Update

                IsUpdate = true;
                ChangeData = false;

                #endregion

                #endregion

                #region Tracking

                if (TrackingTrace == "Y")
                {
                    purchaseTrackings.Clear();
                    for (int i = 0; i < dgvPurchase.Rows.Count; i++)
                    {
                        string itemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();
                        InsertTrackingInfo(itemNo);
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
                FileLogger.Log(this.Name, "bgwSearchPurchase_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                #region Button Stats

                this.btnFirst.Enabled = true;
                this.btnPrevious.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnLast.Enabled = true;

                this.btnSearchInvoiceNo.Enabled = true;

                this.progressBar1.Visible = false;
                #endregion
            }
        }

        #endregion

        #region Backup Methods

        //// Sep-28-2020
        private void Rowcalculate_Backup()
        {
            try
            {

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Statement

                #region Variables
                decimal vImpSubTotal = 0;
                decimal vImpVAT = 0;
                decimal vImpCnF = 0;
                decimal vImpInsurance = 0;
                decimal vImpAV = 0;
                decimal vImpCD = 0;
                decimal vImpRD = 0;
                decimal vImpTVB = 0;
                decimal vImpTVA = 0;
                decimal vImpATV = 0;
                decimal vImpOthers = 0;
                decimal vImpSD = 0;
                decimal vImpTotalPrice;
                decimal SubTotal1 = 0;
                decimal SubTotal = 0;
                decimal VDSAmount = 0;

                decimal SD = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal Total = 0;

                decimal SumSDAmount = 0;
                decimal SumVATAmount = 0;
                decimal SumSubTotal = 0;
                decimal SumSubTotal2 = 0;
                decimal SumTotal = 0;
                decimal vtotalSubTotal1 = 1;

                #endregion 0

                if (Program.CheckingNumericTextBox(txtTotalSubTotal1, "txtTotalSubTotal1") == true)
                    txtTotalSubTotal1.Text = Program.FormatingNumeric(txtTotalSubTotal1.Text.Trim(), PurchasePlaceAmt).ToString();

                if (Convert.ToDecimal(txtTotalSubTotal1.Text.Trim()) == 0)
                {
                    vtotalSubTotal1 = 1;
                }
                else
                {
                    vtotalSubTotal1 = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                }

                #endregion Qty andUOMs

                #region For Loop

                for (int i = 0; i < dgvPurchase.RowCount; i++)
                {

                    dgvPurchase[0, i].Value = i + 1;
                    dgvPurchase["VATableValue", i].Value = "0";

                    string strSubTotal = "";
                    decimal vQuantity = 0;

                    #region null check of Duties
                    if (dgvPurchase["CnF", i].Value == null)
                    {
                        dgvPurchase["CnF", i].Value = "0";
                    }
                    if (dgvPurchase["Insurance", i].Value == null)
                    {
                        dgvPurchase["Insurance", i].Value = "0";
                    }
                    if (dgvPurchase["AV", i].Value == null)
                    {
                        dgvPurchase["AV", i].Value = "0";
                    }
                    if (dgvPurchase["CD", i].Value == null)
                    {
                        dgvPurchase["CD", i].Value = "0";
                    }
                    if (dgvPurchase["RD", i].Value == null)
                    {
                        dgvPurchase["RD", i].Value = "0";
                    }
                    if (dgvPurchase["TVB", i].Value == null)
                    {
                        dgvPurchase["TVB", i].Value = "0";
                    }
                    if (dgvPurchase["TVA", i].Value == null)
                    {
                        dgvPurchase["TVA", i].Value = "0";
                    }
                    if (dgvPurchase["ATV", i].Value == null)
                    {
                        dgvPurchase["ATV", i].Value = "0";
                    }
                    if (dgvPurchase["Others", i].Value == null)
                    {
                        dgvPurchase["Others", i].Value = "0";
                    }
                    if (dgvPurchase["AIT", i].Value == null)
                    {
                        dgvPurchase["AIT", i].Value = "0";
                    }
                    #endregion

                    #region Value Assign

                    SubTotal = Convert.ToDecimal(dgvPurchase["SubTotal", i].Value);
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                    if (boolsearch == false)
                    {//imp start

                        #region If Import

                        if ((rbtnImport.Checked == true && dutyCalculate == true)
                            || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                            || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                             || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                             || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                            || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                        {
                            dgvDuty["SubTotalDuty", i].Value = dgvPurchase["SubTotal", i].Value.ToString();
                            dgvDuty[0, i].Value = i + 1;

                            #region If Import

                            if (dgvPurchase["SubTotal", i].Value != null)
                            {
                                strSubTotal = dgvPurchase["SubTotal", i].Value.ToString();

                                if (!string.IsNullOrEmpty(strSubTotal))
                                    vImpSubTotal = Convert.ToDecimal(strSubTotal);

                                if (Program.CheckingNumericString(vImpSubTotal.ToString(), "vImpSubTotal") == true)
                                    vImpSubTotal =
                                        Convert.ToDecimal(Program.FormatingNumeric(vImpSubTotal.ToString(),
                                                                                   PurchasePlaceAmt));

                            }
                            if (ChkSubTotalAll.Checked == false)
                            {
                                vImpSubTotal = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                            }

                            #region AV Amount

                            if (CalculativeAV == false)
                            {
                                if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                                    txtAVAmount.Text =
                                        Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                                if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                                {
                                    vImpAV = Convert.ToDecimal(txtAVAmount.Text.Trim());
                                }
                                else
                                {
                                    vImpAV = 0;
                                    vImpCnF = 0;
                                    vImpInsurance = 0;
                                }


                            }
                            else
                            {
                                //            if (Program.CheckingNumericTextBox(txtCnFRate, "txtCnFRate") == true)
                                //txtCnFRate.Text = Program.FormatingNumeric(txtCnFRate.Text.Trim(), PurchasePlaceAmt).ToString();

                                if (!string.IsNullOrEmpty(txtCnFRate.Text.Trim()))
                                {
                                    vImpCnF = vImpSubTotal * Convert.ToDecimal(txtCnFRate.Text.Trim()) / 100;
                                }
                                else
                                {
                                    vImpCnF = 0;
                                }
                                if (Program.CheckingNumericString(vImpCnF.ToString(), "vImpCnF") == true)
                                    vImpCnF =
                                        Convert.ToDecimal(Program.FormatingNumeric(vImpCnF.ToString(), PurchasePlaceAmt));

                                //            if (Program.CheckingNumericTextBox(txtInsRate, "txtInsRate") == true)
                                //txtInsRate.Text = Program.FormatingNumeric(txtInsRate.Text.Trim(), PurchasePlaceAmt).ToString();

                                if (!string.IsNullOrEmpty(txtInsRate.Text.Trim()))
                                {
                                    vImpInsurance = (vImpSubTotal + vImpCnF) * Convert.ToDecimal(txtInsRate.Text.Trim()) /
                                                    100;
                                }
                                else
                                {
                                    vImpInsurance = 0;
                                }
                                if (Program.CheckingNumericString(vImpInsurance.ToString(), "vImpInsurance") == true)
                                    vImpInsurance =
                                        Convert.ToDecimal(Program.FormatingNumeric(vImpInsurance.ToString(),
                                                                                   PurchasePlaceAmt));

                                vImpAV = vImpSubTotal + vImpCnF + vImpInsurance;

                            }
                            if (Program.CheckingNumericString(vImpAV.ToString(), "vImpAV") == true)
                                vImpAV = Convert.ToDecimal(Program.FormatingNumeric(vImpAV.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtCnFAmount, "txtCnFAmount") == true)
                                txtCnFAmount.Text =
                                    Program.FormatingNumeric(txtCnFAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                                txtInsAmount.Text =
                                    Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtAVAmount, "txtAVAmount") == true)
                                txtAVAmount.Text =
                                    Program.FormatingNumeric(txtAVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtCnFInpValue, "txtCnFInpValue") == true)
                                txtCnFInpValue.Text =
                                    Program.FormatingNumeric(txtCnFInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtCnFRate, "txtCnFRate") == true)
                            //    txtCnFRate.Text = Program.FormatingNumeric(txtCnFRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsInpValue, "txtInsInpValue") == true)
                                txtInsInpValue.Text =
                                    Program.FormatingNumeric(txtInsInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtInsRate, "txtInsRate") == true)
                            //    txtInsRate.Text = Program.FormatingNumeric(txtInsRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtInsAmount, "txtInsAmount") == true)
                                txtInsAmount.Text =
                                    Program.FormatingNumeric(txtInsAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtAVInpValue, "txtAVInpValue") == true)
                                txtAVInpValue.Text =
                                    Program.FormatingNumeric(txtAVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                                txtNBRPrice.Text =
                                    Program.FormatingNumeric(txtNBRPrice.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (ChkSubTotalAll.Checked) // Single
                            {



                                dgvPurchase["CnF", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                                dgvPurchase["Insurance", dgvPurchase.CurrentRow.Index].Value = txtInsAmount.Text.Trim();
                                dgvPurchase["AV", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();

                                dgvDuty["CnFInp", dgvPurchase.CurrentRow.Index].Value = txtCnFInpValue.Text.Trim();
                                dgvDuty["CnFRate", dgvPurchase.CurrentRow.Index].Value = txtCnFRate.Text.Trim();
                                dgvDuty["CnFAmount", dgvPurchase.CurrentRow.Index].Value = txtCnFAmount.Text.Trim();
                                dgvDuty["InsuranceInp", dgvPurchase.CurrentRow.Index].Value = txtInsInpValue.Text.Trim();
                                dgvDuty["InsuranceRate", dgvPurchase.CurrentRow.Index].Value = txtInsRate.Text.Trim();
                                dgvDuty["InsuranceAmount", dgvPurchase.CurrentRow.Index].Value =
                                    txtInsAmount.Text.Trim();
                                dgvDuty["AssessableInp", dgvPurchase.CurrentRow.Index].Value = txtAVInpValue.Text.Trim();
                                dgvDuty["AssessableValue", dgvPurchase.CurrentRow.Index].Value = txtAVAmount.Text.Trim();
                            }
                            else
                            {
                                vImpAV = vImpAV * SubTotal / vtotalSubTotal1;
                                vImpInsurance = vImpInsurance * SubTotal / vtotalSubTotal1;
                                vImpCnF = vImpCnF * SubTotal / vtotalSubTotal1;

                                dgvPurchase["CnF", i].Value = vImpCnF;
                                dgvPurchase["Insurance", i].Value = vImpInsurance;
                                dgvPurchase["AV", i].Value = vImpAV;
                                dgvDuty["CnFInp", i].Value = txtCnFInpValue.Text.Trim();
                                dgvDuty["CnFRate", i].Value = txtCnFRate.Text.Trim();
                                dgvDuty["CnFAmount", i].Value = vImpCnF;
                                dgvDuty["InsuranceInp", i].Value = txtInsInpValue.Text.Trim();
                                dgvDuty["InsuranceRate", i].Value = txtInsRate.Text.Trim();
                                dgvDuty["InsuranceAmount", i].Value = vImpInsurance;
                                dgvDuty["AssessableInp", i].Value = txtAVInpValue.Text.Trim();
                                dgvDuty["AssessableValue", i].Value = vImpAV;
                            }



                            #endregion AV

                            #region CD Amount

                            if (!string.IsNullOrEmpty(txtCDRate.Text.Trim()))
                            {
                                vImpCD = vImpAV * Convert.ToDecimal(txtCDRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpCD = vImpAV * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpCD.ToString(), "vImpCD") == true)
                                vImpCD = Convert.ToDecimal(Program.FormatingNumeric(vImpCD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                                txtCDAmount.Text =
                                    Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtCDInpValue, "txtCDInpValue") == true)
                                txtCDInpValue.Text =
                                    Program.FormatingNumeric(txtCDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtCDRate, "txtCDRate") == true)
                            //    txtCDRate.Text = Program.FormatingNumeric(txtCDRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtCDAmount, "txtCDAmount") == true)
                            //    txtCDAmount.Text = Program.FormatingNumeric(txtCDAmount.Text.Trim(), PurchasePlaceAmt).ToString();


                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["CD", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                                dgvDuty["CDInp", dgvPurchase.CurrentRow.Index].Value = txtCDInpValue.Text.Trim();
                                dgvDuty["CDRate", dgvPurchase.CurrentRow.Index].Value = txtCDRate.Text.Trim();
                                dgvDuty["CDAmount", dgvPurchase.CurrentRow.Index].Value = txtCDAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpCD = vImpCD * SubTotal / vtotalSubTotal1;
                                dgvPurchase["CD", i].Value = txtCDAmount.Text.Trim();
                                dgvDuty["CDInp", i].Value = txtCDInpValue.Text.Trim();
                                dgvDuty["CDRate", i].Value = txtCDRate.Text.Trim();
                                dgvDuty["CDAmount", i].Value = txtCDAmount.Text.Trim();
                            }



                            #endregion CD

                            #region RD Amount

                            //if (Program.CheckingNumericTextBox(txtRDRate, "txtRDRate") == true)
                            //    txtRDRate.Text = Program.FormatingNumeric(txtRDRate.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (!string.IsNullOrEmpty(txtRDRate.Text.Trim()))
                            {
                                vImpRD = vImpAV * Convert.ToDecimal(txtRDRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpRD = vImpAV * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpRD.ToString(), "vImpRD") == true)
                                vImpRD = Convert.ToDecimal(Program.FormatingNumeric(vImpRD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                                txtRDAmount.Text =
                                    Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtRDInpValue, "txtRDInpValue") == true)
                                txtRDInpValue.Text =
                                    Program.FormatingNumeric(txtRDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtRDRate, "txtRDRate") == true)
                            //    txtRDRate.Text = Program.FormatingNumeric(txtRDRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtRDAmount, "txtRDAmount") == true)
                                txtRDAmount.Text =
                                    Program.FormatingNumeric(txtRDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["RD", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                                dgvDuty["RDInp", dgvPurchase.CurrentRow.Index].Value = txtRDInpValue.Text.Trim();
                                dgvDuty["RDRate", dgvPurchase.CurrentRow.Index].Value = txtRDRate.Text.Trim();
                                dgvDuty["RDAmount", dgvPurchase.CurrentRow.Index].Value = txtRDAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpRD = vImpRD * SubTotal / vtotalSubTotal1;
                                dgvPurchase["RD", i].Value = txtRDAmount.Text.Trim();
                                dgvDuty["RDInp", i].Value = txtRDInpValue.Text.Trim();
                                dgvDuty["RDRate", i].Value = txtRDRate.Text.Trim();
                                dgvDuty["RDAmount", i].Value = txtRDAmount.Text.Trim();
                            }




                            #endregion RD

                            #region TVB Amount

                            if (!string.IsNullOrEmpty(txtTVBRate.Text.Trim()))
                            {
                                vImpTVB = (vImpAV + vImpCD + vImpRD) * Convert.ToDecimal(txtTVBRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpTVB = (vImpAV + vImpCD + vImpRD) * 0 / 100;
                            }
                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["TVB", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                                dgvDuty["TVBInp", dgvPurchase.CurrentRow.Index].Value = txtTVBInpValue.Text.Trim();
                                dgvDuty["TVBRate", dgvPurchase.CurrentRow.Index].Value = txtTVBRate.Text.Trim();
                                dgvDuty["TVBAmount", dgvPurchase.CurrentRow.Index].Value = txtTVBAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpTVB = vImpTVB * SubTotal / vtotalSubTotal1;
                                dgvPurchase["TVB", i].Value = vImpTVB;
                                dgvDuty["TVBInp", i].Value = txtTVBInpValue.Text.Trim();
                                dgvDuty["TVBRate", i].Value = txtTVBRate.Text.Trim();
                                dgvDuty["TVBAmount", i].Value = vImpTVB;
                            }


                            #endregion TVB

                            #region SD Amount

                            if (!string.IsNullOrEmpty(txtSDRate.Text.Trim()))
                            {
                                vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * Convert.ToDecimal(txtSDRate.Text.Trim()) /
                                         100;
                            }
                            else
                            {
                                vImpSD = (vImpAV + vImpCD + vImpRD + vImpTVB) * 0 / 100;
                            }
                            if (Program.CheckingNumericString(vImpSD.ToString(), "vImpSD") == true)
                                vImpSD = Convert.ToDecimal(Program.FormatingNumeric(vImpSD.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                                txtSDAmount.Text =
                                    Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtSDInpValue, "txtSDInpValue") == true)
                                txtSDInpValue.Text =
                                    Program.FormatingNumeric(txtSDInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtSDAmount, "txtSDAmount") == true)
                                txtSDAmount.Text =
                                    Program.FormatingNumeric(txtSDAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["SDAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();
                                dgvPurchase["SD", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDInp", dgvPurchase.CurrentRow.Index].Value = txtSDInpValue.Text.Trim();
                                dgvDuty["SDuty", dgvPurchase.CurrentRow.Index].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDutyAmount", dgvPurchase.CurrentRow.Index].Value = txtSDAmount.Text.Trim();

                            }
                            else
                            {
                                //vImpSD = vImpSD * SubTotal / vtotalSubTotal1;
                                dgvPurchase["SDAmount", i].Value = txtSDAmount.Text.Trim();
                                dgvPurchase["SD", i].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDInp", i].Value = txtSDInpValue.Text.Trim();
                                dgvDuty["SDuty", i].Value = txtSDRate.Text.Trim();
                                dgvDuty["SDutyAmount", i].Value = txtSDAmount.Text.Trim();

                            }

                            #endregion SD

                            #region VAT Amount

                            //if (Program.CheckingNumericTextBox(txtVATRateI, "txtVATRateI") == true)
                            //txtVATRateI.Text = Program.FormatingNumeric(txtVATRateI.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (!string.IsNullOrEmpty(txtVATRateI.Text.Trim()))
                                vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                          Convert.ToDecimal(txtVATRateI.Text.Trim()) / 100;
                            else
                                vImpVAT = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;

                            if (Program.CheckingNumericString(vImpVAT.ToString(), "vImpVAT") == true)
                                vImpVAT =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpVAT.ToString(), PurchasePlaceAmt));


                            if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                                txtVATAmount.Text =
                                    Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtVATInpValue, "txtVATInpValue") == true)
                                txtVATInpValue.Text =
                                    Program.FormatingNumeric(txtVATInpValue.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (Program.CheckingNumericTextBox(txtVATAmount, "txtVATAmount") == true)
                                txtVATAmount.Text =
                                    Program.FormatingNumeric(txtVATAmount.Text.Trim(), PurchasePlaceAmt).ToString();



                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["VATAmount", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                                dgvPurchase["VATRate", dgvPurchase.CurrentRow.Index].Value = txtVATRateI.Text.Trim();
                                dgvDuty["VATInp", dgvPurchase.CurrentRow.Index].Value = txtVATInpValue.Text.Trim();
                                dgvDuty["VATRateDuty", dgvPurchase.CurrentRow.Index].Value = txtVATRateI.Text.Trim();
                                dgvDuty["VATAmountDuty", dgvPurchase.CurrentRow.Index].Value = txtVATAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpVAT = vImpVAT * SubTotal / vtotalSubTotal1;
                                dgvPurchase["VATAmount", i].Value = txtVATAmount.Text.Trim();
                                dgvPurchase["VATRate", i].Value = txtVATRateI.Text.Trim();
                                dgvDuty["VATInp", i].Value = txtVATInpValue.Text.Trim();
                                dgvDuty["VATRateDuty", i].Value = txtVATRateI.Text.Trim();
                                dgvDuty["VATAmountDuty", i].Value = txtVATAmount.Text.Trim();
                            }
                            decimal bdtTotalVATAmount, usdInvoiceValue, vatperDollar, usdValue, usdVAT = 0;
                            bdtTotalVATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                            usdInvoiceValue = Convert.ToDecimal(txtUSDInvoiceValue.Text.Trim());
                            vatperDollar = bdtTotalVATAmount / usdInvoiceValue;
                            usdValue = Convert.ToDecimal(dgvPurchase["USDValue", i].Value);
                            usdVAT = usdValue * vatperDollar;
                            dgvPurchase["USDVAT", i].Value = usdVAT;
                            dgvPurchase["VATableValue", i].Value = Convert.ToDecimal(dgvPurchase["AV", i].Value)
                                + Convert.ToDecimal(dgvPurchase["CD", i].Value)
                                + Convert.ToDecimal(dgvPurchase["RD", i].Value)
                                + Convert.ToDecimal(dgvPurchase["TVB", i].Value)
                                + Convert.ToDecimal(dgvPurchase["SDAmount", i].Value)

                                ;


                            #endregion VAT

                            #region TVA

                            //if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                            //    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (!string.IsNullOrEmpty(txtTVARate.Text.Trim()))
                            {
                                vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                          Convert.ToDecimal(txtTVARate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpTVA = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpTVA.ToString(), "vImpTVA") == true)
                                vImpTVA =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpTVA.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                                txtTVAAmount.Text =
                                    Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtTVAInpValue, "txtTVAInpValue") == true)
                                txtTVAInpValue.Text =
                                    Program.FormatingNumeric(txtTVAInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtTVARate, "txtTVARate") == true)
                            //    txtTVARate.Text = Program.FormatingNumeric(txtTVARate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtTVAAmount, "txtTVAAmount") == true)
                                txtTVAAmount.Text =
                                    Program.FormatingNumeric(txtTVAAmount.Text.Trim(), PurchasePlaceAmt).ToString();


                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["TVA", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                                dgvDuty["TVAInp", dgvPurchase.CurrentRow.Index].Value = txtTVAInpValue.Text.Trim();
                                dgvDuty["TVARate", dgvPurchase.CurrentRow.Index].Value = txtTVARate.Text.Trim();
                                dgvDuty["TVAAmount", dgvPurchase.CurrentRow.Index].Value = txtTVAAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpTVA = vImpTVA * SubTotal / vtotalSubTotal1;
                                dgvPurchase["TVA", i].Value = vImpTVA;
                                dgvDuty["TVAInp", i].Value = txtTVAInpValue.Text.Trim();
                                dgvDuty["TVARate", i].Value = txtTVARate.Text.Trim();
                                dgvDuty["TVAAmount", i].Value = vImpTVA;
                            }



                            #endregion TVA

                            #region ATV

                            //if (Program.CheckingNumericTextBox(txtATVRate, "txtATVRate") == true)
                            //    txtATVRate.Text = Program.FormatingNumeric(txtATVRate.Text.Trim(), PurchasePlaceAmt).ToString();


                            if (!string.IsNullOrEmpty(txtATVRate.Text.Trim()))
                            {
                                vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) *
                                          Convert.ToDecimal(txtATVRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpATV = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpTVA) * 0 / 100;
                            }

                            if (Program.CheckingNumericString(vImpATV.ToString(), "vImpATV") == true)
                                vImpATV =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpATV.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                                txtATVAmount.Text =
                                    Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtATVInpValue, "txtATVInpValue") == true)
                                txtATVInpValue.Text =
                                    Program.FormatingNumeric(txtATVInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtATVRate, "txtATVRate") == true)
                            //    txtATVRate.Text = Program.FormatingNumeric(txtATVRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtATVAmount, "txtATVAmount") == true)
                                txtATVAmount.Text =
                                    Program.FormatingNumeric(txtATVAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["ATV", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                                dgvDuty["ATVInp", dgvPurchase.CurrentRow.Index].Value = txtATVInpValue.Text.Trim();
                                dgvDuty["ATVRate", dgvPurchase.CurrentRow.Index].Value = txtATVRate.Text.Trim();
                                dgvDuty["ATVAmount", dgvPurchase.CurrentRow.Index].Value = txtATVAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpATV = vImpATV * SubTotal / vtotalSubTotal1;
                                dgvPurchase["ATV", i].Value = txtATVAmount.Text.Trim();
                                dgvDuty["ATVInp", i].Value = txtATVInpValue.Text.Trim();
                                dgvDuty["ATVRate", i].Value = txtATVRate.Text.Trim();
                                dgvDuty["ATVAmount", i].Value = txtATVAmount.Text.Trim();
                            }




                            #endregion ATV

                            #region Others


                            if (!string.IsNullOrEmpty(txtOthersRate.Text.Trim()))
                            {
                                vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) *
                                             Convert.ToDecimal(txtOthersRate.Text.Trim()) / 100;
                            }
                            else
                            {
                                vImpOthers = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD) * 0 / 100;
                            }


                            if (Program.CheckingNumericString(vImpOthers.ToString(), "vImpOthers") == true)
                                vImpOthers =
                                    Convert.ToDecimal(Program.FormatingNumeric(vImpOthers.ToString(), PurchasePlaceAmt));

                            if (Program.CheckingNumericTextBox(txtOthersInpValue, "txtOthersInpValue") == true)
                                txtOthersInpValue.Text =
                                    Program.FormatingNumeric(txtOthersInpValue.Text.Trim(), PurchasePlaceAmt).ToString();
                            //if (Program.CheckingNumericTextBox(txtOthersRate, "txtOthersRate") == true)
                            //    txtOthersRate.Text = Program.FormatingNumeric(txtOthersRate.Text.Trim(), PurchasePlaceAmt).ToString();
                            if (Program.CheckingNumericTextBox(txtOthersAmount, "txtOthersAmount") == true)
                                txtOthersAmount.Text =
                                    Program.FormatingNumeric(txtOthersAmount.Text.Trim(), PurchasePlaceAmt).ToString();

                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvPurchase["Others", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                                dgvDuty["OthersInp", dgvPurchase.CurrentRow.Index].Value = txtOthersInpValue.Text.Trim();
                                dgvDuty["OthersRate", dgvPurchase.CurrentRow.Index].Value = txtOthersRate.Text.Trim();
                                dgvDuty["OthersAmount", dgvPurchase.CurrentRow.Index].Value = txtOthersAmount.Text.Trim();
                            }
                            else
                            {
                                //vImpOthers = vImpOthers * SubTotal / vtotalSubTotal1;
                                dgvPurchase["Others", i].Value = txtOthersAmount.Text.Trim();
                                dgvDuty["OthersInp", i].Value = txtOthersInpValue.Text.Trim();
                                dgvDuty["OthersRate", i].Value = txtOthersRate.Text.Trim();
                                dgvDuty["OthersAmount", i].Value = txtOthersAmount.Text.Trim();
                            }



                            #endregion Others

                            #region AIV



                            dgvPurchase["AIT", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();
                            dgvDuty["AITInp", dgvPurchase.CurrentRow.Index].Value = txtAITInpValue.Text.Trim();
                            dgvDuty["AITAmount", dgvPurchase.CurrentRow.Index].Value = txtAITAmount.Text.Trim();





                            #endregion ATV

                            if (ChkSubTotalAll.Checked) // Single
                            {
                                dgvDuty["Remarks", dgvPurchase.CurrentRow.Index].Value = txtDutiesRemarks.Text.Trim();
                            }
                            else
                            {
                                dgvDuty["Remarks", i].Value = txtDutiesRemarks.Text.Trim();
                            }


                            #endregion If Import

                        }
                        #endregion If Import
                        #region If not Import

                        else
                        {
                            #region Close

                            string strSD = "";
                            decimal vSD = 0;

                            #region Old

                            //if (dgvPurchase["SD", i].Value != null)
                            //{
                            //    strSD = dgvPurchase["SD", i].Value.ToString();
                            //    if (!string.IsNullOrEmpty(strSD))
                            //        vSD = Convert.ToDecimal(strSD);
                            //}


                            //SD = vSD;
                            //if (Program.CheckingNumericString(SD.ToString(), "SD") == true)
                            //    SD = Convert.ToDecimal(Program.FormatingNumeric(SD.ToString(), PurchasePlaceAmt));

                            //SDAmount = (SubTotal) * SD / 100;
                            //if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                            //    SDAmount =
                            //        Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), PurchasePlaceAmt));

                            //dgvPurchase["SDAmount", i].Value = Convert.ToDecimal(SDAmount);


                            //if (FixedVAT == true) // fixed
                            //{

                            //    string strVatAmount = "";
                            //    decimal vVatAmount = 0;

                            //    if (dgvPurchase["VATAmount", i].Value != null)
                            //    {
                            //        strVatAmount = dgvPurchase["VATAmount", i].Value.ToString();
                            //        if (!string.IsNullOrEmpty(strVatAmount))
                            //            vVatAmount = Convert.ToDecimal(strVatAmount);
                            //    }

                            //    VATAmount = vVatAmount;
                            //    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                            //        VATAmount =
                            //            Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(),
                            //                                                       PurchasePlaceAmt));

                            //    if ((SubTotal + SDAmount) > 0)
                            //        VATRate = (VATAmount * 100) / (SubTotal + SDAmount);

                            //    dgvPurchase["VATRate", i].Value = Convert.ToDecimal(VATRate);



                            //}

                            //else // rate
                            //{
                            //    string strVatRate = "";
                            //    decimal vVatRate = 0;

                            //    if (dgvPurchase["VATRate", i].Value != null)
                            //    {
                            //        strVatRate = dgvPurchase["VATRate", i].Value.ToString();
                            //        if (!string.IsNullOrEmpty(strVatRate))
                            //            vVatRate = Convert.ToDecimal(strVatRate);
                            //    }

                            //    VATRate = vVatRate;

                            //    VATAmount = (SDAmount + SubTotal) * VATRate / 100;
                            //    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                            //        VATAmount =
                            //            Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(),
                            //                                                       PurchasePlaceAmt));

                            //    dgvPurchase["VATAmount", i].Value = Convert.ToDecimal(VATAmount);
                            //}

                            #endregion

                            VATAmount = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value.ToString());
                            SDAmount = Convert.ToDecimal(dgvPurchase["SDAmount", i].Value.ToString());

                            Total = (SDAmount + VATAmount + SubTotal);

                            #endregion Close
                        }

                        #endregion If not Import
                    }

                    #endregion

                    #region Value Assign


                    decimal vImpInsurance1 = Convert.ToDecimal(dgvPurchase["Insurance", i].Value);
                    decimal vImpCnF1 = Convert.ToDecimal(dgvPurchase["CnF", i].Value);
                    decimal vImpAV1 = Convert.ToDecimal(dgvPurchase["SubTotal", i].Value);
                    decimal vImpCD1 = Convert.ToDecimal(dgvPurchase["CD", i].Value);
                    decimal vImpRD1 = Convert.ToDecimal(dgvPurchase["RD", i].Value);
                    decimal vImpTVB1 = Convert.ToDecimal(dgvPurchase["TVB", i].Value);
                    decimal vImpSD1 = Convert.ToDecimal(dgvPurchase["SDAmount", i].Value);
                    decimal vImpVAT1 = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value);
                    decimal vImpATV1 = Convert.ToDecimal(dgvPurchase["ATV", i].Value);
                    decimal vImpTVA1 = Convert.ToDecimal(dgvPurchase["TVA", i].Value);
                    decimal vImpOthers1 = Convert.ToDecimal(dgvPurchase["Others", i].Value);
                    decimal vImpAIT1 = Convert.ToDecimal(dgvPurchase["AIT", i].Value);



                    //vImpTotalPrice = (vImpAV + vImpCD + vImpRD + vImpTVB + vImpSD + vImpVAT + vImpATV +
                    //                          vImpOthers);
                    vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                                              vImpOthers1 + vImpOthers1 + vImpTVA1 + vImpInsurance1 + vImpCnF1);

                    Total = vImpTotalPrice;

                    #region Conditional Values

                    if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                        Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                    if (Program.CheckingNumericString(Total.ToString(), "Total") == true)
                        Total = Convert.ToDecimal(Program.FormatingNumeric(Total.ToString(), PurchasePlaceAmt));

                    #endregion

                    dgvPurchase["RebateAmount", i].Value = Convert.ToDecimal(dgvPurchase["VATAmount", i].Value)
                            * Convert.ToDecimal(dgvPurchase["Rebate", i].Value) / 100;
                    dgvPurchase["Total", i].Value = Total;

                    #endregion

                    #region AssessableValue

                    if ((rbtnImport.Checked == true && dutyCalculate == true)
                           || (rbtnTradingImport.Checked == true && dutyCalculate == true)
                           || (rbtnServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
                            || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
                           || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                    {
                        DataGridViewColumnCollection dgvColumns = dgvDuty.Columns;

                        if (dgvColumns.Contains("AssessableInp"))
                        {
                            dgvDuty["AssessableInp", i].Value = vImpAV1 + vImpInsurance1 + vImpCnF1;
                        }

                        if (dgvColumns.Contains("AssessableValue"))
                        {
                            dgvDuty["AssessableValue", i].Value = vImpAV1 + vImpInsurance1 + vImpCnF1; ;
                        }



                    }
                    #endregion

                    #region Value Assign

                    VDSAmount = VDSAmount + Convert.ToDecimal(dgvPurchase["VDSAmount", i].Value.ToString());
                    string strSDAmount = "";
                    decimal vSDAmount = 0;

                    if (dgvPurchase["SDAmount", i].Value != null)
                    {
                        strSDAmount = dgvPurchase["SDAmount", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strSDAmount))
                            vSDAmount = Convert.ToDecimal(strSDAmount);
                    }
                    if (Program.CheckingNumericString(vSDAmount.ToString(), "vSDAmount") == true)
                        vSDAmount = Convert.ToDecimal(Program.FormatingNumeric(vSDAmount.ToString(), PurchasePlaceAmt));

                    SumSDAmount = SumSDAmount + vSDAmount;

                    if (Program.CheckingNumericString(SumSDAmount.ToString(), "SumSDAmount") == true)
                        SumSDAmount = Convert.ToDecimal(Program.FormatingNumeric(SumSDAmount.ToString(), PurchasePlaceAmt));


                    string strVatAmount2 = "";
                    decimal vVatAmount2 = 0;

                    if (dgvPurchase["VATAmount", i].Value != null)
                    {
                        strVatAmount2 = dgvPurchase["VATAmount", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strVatAmount2))
                            vVatAmount2 = Convert.ToDecimal(strVatAmount2);
                    }
                    if (Program.CheckingNumericString(vVatAmount2.ToString(), "vVatAmount2") == true)
                        vVatAmount2 = Convert.ToDecimal(Program.FormatingNumeric(vVatAmount2.ToString(), PurchasePlaceAmt));

                    #endregion

                    #region Value Assign

                    SumVATAmount = SumVATAmount + vVatAmount2;

                    if (Program.CheckingNumericString(SumVATAmount.ToString(), "SumVATAmount") == true)
                        SumVATAmount = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), PurchasePlaceAmt));

                    string strSumTotal = "";
                    decimal vSumTotal = 0;

                    if (dgvPurchase["Total", i].Value != null)
                    {
                        strSumTotal = dgvPurchase["Total", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strSumTotal))
                            vSumTotal = Convert.ToDecimal(strSumTotal);
                    }

                    if (Program.CheckingNumericString(vSumTotal.ToString(), "vSumTotal") == true)
                        vSumTotal = Convert.ToDecimal(Program.FormatingNumeric(vSumTotal.ToString(), PurchasePlaceAmt));

                    SumTotal = SumTotal + vSumTotal;
                    if (Program.CheckingNumericString(SumTotal.ToString(), "SumTotal") == true)
                        SumTotal = Convert.ToDecimal(Program.FormatingNumeric(SumTotal.ToString(), PurchasePlaceAmt));

                    //vQuantity

                    string strPrevious = "";
                    decimal vPrevious = 0;

                    if (dgvPurchase["Previous", i].Value != null)
                    {
                        strPrevious = dgvPurchase["Previous", i].Value.ToString();
                        if (!string.IsNullOrEmpty(strPrevious))
                            vPrevious = Convert.ToDecimal(strPrevious);
                    }
                    dgvPurchase["Change", i].Value = vQuantity - vPrevious;

                    SubTotal1 = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1);

                    #endregion

                    #region Value Assign


                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), PurchasePlaceAmt));

                    SumSubTotal = SumSubTotal + SubTotal1;
                    SumSubTotal2 = SumSubTotal2 + SubTotal;
                    if (Program.CheckingNumericString(SumSubTotal.ToString(), "SumSubTotal") == true)
                        SumSubTotal = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), PurchasePlaceAmt));

                    if (Program.CheckingNumericString(SumSubTotal2.ToString(), "SumSubTotal") == true)
                        SumSubTotal2 = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal2.ToString(), PurchasePlaceAmt));

                    #endregion

                }

                #endregion


                #region Total Fields

                txtTotalSubTotal2.Text = Program.ParseDecimalObject(SumSubTotal2).ToString();

                txtTotalSubTotal.Text = Program.ParseDecimalObject(SumSubTotal).ToString();//"0,0.00");
                txtTotalVATAmount.Text = Program.ParseDecimalObject(SumVATAmount).ToString();//"0,0.00");
                txtTotalSDAmount.Text = Program.ParseDecimalObject(SumSDAmount).ToString();//"0,0.00");
                txtTotalAmount.Text = Program.ParseDecimalObject(SumTotal).ToString();//"0,0.00");
                txtTotalVDSAmount.Text = Program.ParseDecimalObject(VDSAmount).ToString();//"0,0.00");

                #endregion


                boolsearch = false;

                FindBomId();
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

        //        private void ProductLoad(string )
        //        {
        //            cmbProduct.DataSource = null;
        //            CommonDAL cDal = new CommonDAL();

        //            string sqlQuery = @"select P.ItemNo valueMember,p.ProductName displayMember from Products p
        //left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
        //where p.ActiveStatus='Y' and  pc.IsRaw='raw'
        //order by ProductName";
        //            cmbProduct = cDal.ComboBoxLoad(cmbProduct, sqlQuery);
        //        }

        #region Backup Methods Oct-04-2020

        private void RateChangePercentBackup(string ItemNo, decimal unitPrice)
        {
            string result = string.Empty;


            decimal plusRateChangePromotPercent = 0;
            decimal minusRateChangePromotPercent = 0;
            plusRateChangePromotPercent = Convert.ToDecimal("+" + RateChangePromote);
            minusRateChangePromotPercent = Convert.ToDecimal("-" + RateChangePromote);
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            decimal bomVatablePrice = productDal.GetLastVatableFromBOM(ItemNo, null, null, connVM);
            if (bomVatablePrice <= 0)
            {
                MessageBox.Show("This item is not included in any price declaration", "Unit Price", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            decimal changes = (unitPrice - bomVatablePrice) * 100 / bomVatablePrice;
            if (changes > plusRateChangePromotPercent || changes < minusRateChangePromotPercent)
            {
                MessageBox.Show("In Purchase price : " + unitPrice + "\n In Last Price Declaration : " + bomVatablePrice + "" +
                                "\nChanges : " + changes.ToString("0,0.0000") + "%", "Unit Price", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        #endregion

        #endregion

        #region Methods 04

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to refresh without saving?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                    {
                        //ProductSearch();
                        //ProductSearchDs();
                        //SearchVendor();
                        ClearAllFields();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
                    //ProductSearch();
                    //ProductSearchDs();
                    //SearchVendor();
                    ClearAllFields();
                    ChangeData = false;
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

        private void picVendor_Click(object sender, EventArgs e)
        {

        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            if (txtVendorName.Text == "")
            {
                MessageBox.Show("Please Select Vendor", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            //if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost/Total Cost") == true)
            //    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), PurchasePlaceAmt).ToString();
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();

            SDInputAmnt();
            VATCalculation();


            //Program.FormatTextBoxQty4(txtUnitCost, "Unit Cost/Total Cost");
        }

        private void SDInputAmnt()
        {
            try
            {


                decimal uCost = 0;
                decimal qty = 0;
                decimal sdRate = 0;

                uCost = Convert.ToDecimal(txtUnitCost.Text);
                qty = Convert.ToDecimal(txtQuantity.Text);
                sdRate = Convert.ToDecimal(txtSD.Text);
                if (IsTotalPrice)
                {
                    txtLocalSDAmount.Text = Program.ParseDecimal(Convert.ToDecimal(uCost * sdRate / 100).ToString());



                }
                else
                {
                    txtLocalSDAmount.Text = Program.ParseDecimal(Convert.ToDecimal(uCost * qty * sdRate / 100).ToString());


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
            VATCalculation();
            //Program.FormatTextBoxQty4(txtQuantity, "Quantity");
            // var tt = txtQuantity.Text;
            //if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            //{
            //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), PurchasePlaceQty).ToString();
            //    vatAmnt();
            //}
        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            if (FixedVAT == true)//fixed
            {
                if (Program.CheckingNumericTextBox(txtVATRate, "VAT Rate") == true)
                    txtVATRate.Text = Program.FormatingNumeric(txtVATRate.Text.Trim(), PurchasePlaceAmt).ToString();

                //Program.FormatTextBox(txtVATRate, "VAT Rate");

            }
            else
            {
                Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            }
            Program.CheckVATRate(txtVATRate, "VAT Rate Input Box");

            VATCalculation();
            //VDSStatus();


        }

        private void dgvPurchase_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (dgvPurchase.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign to Form Elements

                txtBOMId.Text = dgvPurchase.CurrentRow.Cells["BOMId"].Value.ToString();
                cmbType.SelectedValue = dgvPurchase.CurrentRow.Cells["Type"].Value.ToString().ToLower();
                cmbProduct.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                txtLineNo.Text = dgvPurchase.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                vItemNo = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvPurchase.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                txtItemNo.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();

                #region Price Load Function

                PriceLoad();

                #endregion

                txtQuantity.Text = dgvPurchase.CurrentRow.Cells["Quantity"].Value.ToString();
                txtPurQty.Text = dgvPurchase.CurrentRow.Cells["Quantity"].Value.ToString();
                txtPrevious.Text = dgvPurchase.CurrentRow.Cells["Previous"].Value.ToString();

                #region Conditional Values

                if (IsTotalPrice == true)
                {
                    txtUnitCost.Text = dgvPurchase.CurrentRow.Cells["SubTotal"].Value.ToString();
                }
                else
                {
                    txtUnitCost.Text = dgvPurchase.CurrentRow.Cells["UnitPrice"].Value.ToString();

                }

                #endregion

                txtVATRate.Text = dgvPurchase.CurrentRow.Cells["VATRate"].Value.ToString();

                txtUOM.Text = dgvPurchase.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());

                #region UOM Function

                Uoms();

                #endregion

                cmbUom.Text = dgvPurchase.CurrentRow.Cells["UOM"].Value.ToString();
                //cmbVDS.Text = dgvPurchase.CurrentRow.Cells["UOM"].Value.ToString();
                txtSD.Text = dgvPurchase.CurrentRow.Cells["SD"].Value.ToString();
                txtVDSRate.Text = dgvPurchase.CurrentRow.Cells["VDSRate"].Value.ToString();
                txtVDSAmount.Text = dgvPurchase.CurrentRow.Cells["VDSAmount"].Value.ToString();
                txtUSDValue.Text = dgvPurchase.CurrentRow.Cells["USDValue"].Value.ToString();
                txtSection.Text = dgvPurchase.CurrentRow.Cells["TDSSection"].Value.ToString();
                txtCode.Text = dgvPurchase.CurrentRow.Cells["TDSCode"].Value.ToString();
                txtCommentsDetail.Text = "NA";
                txtNBRPrice.Text = dgvPurchase.CurrentRow.Cells["NBRPrice"].Value.ToString();
                txtRebatePercent.Text = dgvPurchase.CurrentRow.Cells["Rebate"].Value.ToString();
                txtUomConv.Text = dgvPurchase.CurrentRow.Cells["UOMc"].Value.ToString();
                chkIsFixedOtherVAT.Checked = Convert.ToBoolean(dgvPurchase.CurrentRow.Cells["IsFixedVAT"].Value.ToString().Trim() == "Y" ? true : false);
                txtLocalVATAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["VATAmount"].Value).ToString();
                txtLocalSDAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["SDAmount"].Value).ToString();
                txtPreSubTotal.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["SubTotal"].Value).ToString();

                if (dgvPurchase.CurrentRow.Cells["HSCode"].Value != null)
                {
                    txtHSCode.Text = dgvPurchase.CurrentRow.Cells["HSCode"].Value.ToString();
                }

                #region Stock Call / Avg Price

                ////IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ////txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                ////                        DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM).Rows[0]["Quantity"].ToString();

                #endregion

                #endregion

                if (rbtnPurchaseReturn.Checked)
                {
                    txtInvoiceValue.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["InvoiceValue"].Value).ToString();
                    txtExchangeRate.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["ExchangeRate"].Value).ToString();

                    txtCurrency.Text = dgvPurchase.CurrentRow.Cells["Currency"].Value.ToString();
                    txtCnFAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["CnF"].Value).ToString();
                    txtInsAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["Insurance"].Value).ToString();
                    txtAVAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["AV"].Value).ToString();
                    txtCDAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["CD"].Value).ToString();
                    txtRDAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["RD"].Value).ToString();
                    txtTVBAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["TVB"].Value).ToString();
                    txtTVAAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["TVA"].Value).ToString();
                    txtATVAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["ATV"].Value).ToString();
                    txtOthersAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["Others"].Value).ToString();
                    txtAITAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["AIT"].Value).ToString();
                    txtSDAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["SDAmount"].Value).ToString();
                    txtPreUSDVAT.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["USDVAT"].Value).ToString();
                    txtPreFixedVATAmount.Text = Convert.ToDecimal(dgvPurchase.CurrentRow.Cells["FixedVATAmount"].Value).ToString();

                }

                #region Import and Duties

                if (rbtnTradingImport.Checked
                    || rbtnImport.Checked
                    || rbtnServiceImport.Checked
                    || rbtnInputServiceImport.Checked
                    || rbtnServiceNSImport.Checked
                    || rbtnCommercialImporter.Checked
                    )
                {
                    if (ChkSubTotalAll.Checked == false)
                    {
                        txtTotalSubTotal1.Text = txtTotalSubTotal2.Text.Trim();
                        //txtAVInpValue.Text = txtTotalSubTotal1.Text;
                        txtAVInpValue.Text = txtTotalSubTotal1.Text;
                        txtAVAmount.Text = txtTotalSubTotal1.Text;
                        txtInvoiceValue.Text = dgvPurchase.CurrentRow.Cells["InvoiceValue"].Value.ToString();
                        txtExchangeRate.Text = dgvPurchase.CurrentRow.Cells["ExchangeRate"].Value.ToString();
                        txtCurrency.Text = dgvPurchase.CurrentRow.Cells["Currency"].Value.ToString();
                    }
                    else
                    {
                        txtTotalSubTotal1.Text = dgvPurchase.CurrentRow.Cells["SubTotal"].Value.ToString();
                        //txtAVInpValue.Text = txtTotalSubTotal1.Text;
                        txtAVInpValue.Text = txtTotalSubTotal1.Text;
                        txtAVAmount.Text = txtTotalSubTotal1.Text;
                        txtInvoiceValue.Text = dgvPurchase.CurrentRow.Cells["InvoiceValue"].Value.ToString();
                        txtExchangeRate.Text = dgvPurchase.CurrentRow.Cells["ExchangeRate"].Value.ToString();
                        txtCurrency.Text = dgvPurchase.CurrentRow.Cells["Currency"].Value.ToString();

                    }

                    #region DutiesSelection

                    //txtUomConv.Text = dgvPurchase.CurrentRow.Cells["UOMc"].Value.ToString();
                    int tt = Convert.ToInt32(dgvPurchase.CurrentRow.Cells[0].Value.ToString());
                    int tt1 = Convert.ToInt32(dgvDuty.Rows.Count);
                    if (tt1 <= 0)
                    {
                        DataGridViewRow DNewRow = new DataGridViewRow();

                        #region Duty
                        dgvDuty.Rows.Add(DNewRow);

                        dgvDuty["ItemNoDuty", dgvDuty.RowCount - 1].Value = txtProductCode.Text.Trim();
                        if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                            txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), PurchasePlaceQty).ToString();

                        dgvDuty["QuantityDuty", dgvDuty.RowCount - 1].Value = txtQuantity.Text.Trim();
                        //QuantityDuty
                        dgvDuty["ProductNameDuty", dgvDuty.RowCount - 1].Value = txtProductName.Text.Trim();
                        dgvDuty["ProductCodeDuty", dgvDuty.RowCount - 1].Value = txtPCode.Text.Trim();

                        dgvDuty["CnFInp", dgvDuty.RowCount - 1].Value = "0";// txtCnFInpValue.Text.Trim();
                        dgvDuty["CnFRate", dgvDuty.RowCount - 1].Value = "0";// txtCnFRate.Text.Trim();
                        dgvDuty["CnFAmount", dgvDuty.RowCount - 1].Value = "0";// txtCnFAmount.Text.Trim();
                        dgvDuty["InsuranceInp", dgvDuty.RowCount - 1].Value = "0";// txtInsInpValue.Text.Trim();
                        dgvDuty["InsuranceRate", dgvDuty.RowCount - 1].Value = "0";// txtInsRate.Text.Trim();
                        dgvDuty["InsuranceAmount", dgvDuty.RowCount - 1].Value = "0";// txtInsAmount.Text.Trim();
                        dgvDuty["AssessableInp", dgvDuty.RowCount - 1].Value = txtTotalSubTotal1.Text.Trim();
                        dgvDuty["AssessableValue", dgvDuty.RowCount - 1].Value = txtTotalSubTotal1.Text.Trim();// txtAVAmount.Text.Trim();
                        dgvDuty["CDInp", dgvDuty.RowCount - 1].Value = "0";// txtCDInpValue.Text.Trim();
                        dgvDuty["CDRate", dgvDuty.RowCount - 1].Value = "0";// txtCDRate.Text.Trim();
                        dgvDuty["CDAmount", dgvDuty.RowCount - 1].Value = "0";// vImpCD;
                        dgvDuty["RDInp", dgvDuty.RowCount - 1].Value = "0";// txtRDInpValue.Text.Trim();
                        dgvDuty["RDRate", dgvDuty.RowCount - 1].Value = "0";// txtRDRate.Text.Trim();
                        dgvDuty["RDAmount", dgvDuty.RowCount - 1].Value = "0";// txtRDAmount.Text.Trim();
                        dgvDuty["TVBInp", dgvDuty.RowCount - 1].Value = "0";// txtTVBInpValue.Text.Trim();
                        dgvDuty["TVBRate", dgvDuty.RowCount - 1].Value = "0";// txtTVBRate.Text.Trim();
                        dgvDuty["TVBAmount", dgvDuty.RowCount - 1].Value = "0";// txtTVBAmount.Text.Trim();
                        dgvDuty["SDInp", dgvDuty.RowCount - 1].Value = "0";// txtSDInpValue.Text.Trim();
                        dgvDuty["SDuty", dgvDuty.RowCount - 1].Value = "0";// txtSDRate.Text.Trim();
                        dgvDuty["SDutyAmount", dgvDuty.RowCount - 1].Value = "0";// txtSDAmount.Text.Trim();
                        dgvDuty["VATInp", dgvDuty.RowCount - 1].Value = "0";// txtVATInpValue.Text.Trim();
                        dgvDuty["VATRateDuty", dgvDuty.RowCount - 1].Value = "0";// txtVATRateI.Text.Trim();
                        dgvDuty["VATAmountDuty", dgvDuty.RowCount - 1].Value = "0";// txtVATAmount.Text.Trim();
                        dgvDuty["TVAInp", dgvDuty.RowCount - 1].Value = "0";// txtTVAInpValue.Text.Trim();
                        dgvDuty["TVARate", dgvDuty.RowCount - 1].Value = "0";// txtTVARate.Text.Trim();
                        dgvDuty["TVAAmount", dgvDuty.RowCount - 1].Value = "0";// vImpTVA;
                        dgvDuty["ATVInp", dgvDuty.RowCount - 1].Value = "0";// txtATVInpValue.Text.Trim();
                        dgvDuty["ATVRate", dgvDuty.RowCount - 1].Value = "0";// txtATVRate.Text.Trim();
                        dgvDuty["ATVAmount", dgvDuty.RowCount - 1].Value = "0";// vImpATV;
                        dgvDuty["OthersInp", dgvDuty.RowCount - 1].Value = "0";// txtOthersInpValue.Text.Trim();
                        dgvDuty["OthersRate", dgvDuty.RowCount - 1].Value = "0";// txtOthersRate.Text.Trim();
                        dgvDuty["OthersAmount", dgvDuty.RowCount - 1].Value = "0";// vImpOthers;
                        dgvDuty["AITInp", dgvDuty.RowCount - 1].Value = "0";// txtOthersInpValue.Text.Trim();
                        dgvDuty["AITAmount", dgvDuty.RowCount - 1].Value = "0";// vImpOthers;

                        dgvDuty["Remarks", dgvDuty.RowCount - 1].Value = "NA";

                        #endregion Duty
                    }



                    txtCnFInpValue.Text = dgvDuty.Rows[tt - 1].Cells["CnFInp"].Value.ToString();
                    txtCnFRate.Text = dgvDuty.Rows[tt - 1].Cells["CnFRate"].Value.ToString();
                    txtCnFAmount.Text = dgvDuty.Rows[tt - 1].Cells["CnFAmount"].Value.ToString();
                    txtInsInpValue.Text = dgvDuty.Rows[tt - 1].Cells["InsuranceInp"].Value.ToString();
                    txtInsRate.Text = dgvDuty.Rows[tt - 1].Cells["InsuranceRate"].Value.ToString();
                    txtInsAmount.Text = dgvDuty.Rows[tt - 1].Cells["InsuranceAmount"].Value.ToString();
                    txtAVInpValue.Text = dgvDuty.Rows[tt - 1].Cells["AssessableInp"].Value.ToString();
                    txtAVAmount.Text = dgvDuty.Rows[tt - 1].Cells["AssessableValue"].Value.ToString();
                    txtCDInpValue.Text = dgvDuty.Rows[tt - 1].Cells["CDInp"].Value.ToString();
                    txtCDRate.Text = dgvDuty.Rows[tt - 1].Cells["CDRate"].Value.ToString();
                    txtCDAmount.Text = dgvDuty.Rows[tt - 1].Cells["CDAmount"].Value.ToString();
                    txtRDInpValue.Text = dgvDuty.Rows[tt - 1].Cells["RDInp"].Value.ToString();
                    txtRDRate.Text = dgvDuty.Rows[tt - 1].Cells["RDRate"].Value.ToString();
                    txtRDAmount.Text = dgvDuty.Rows[tt - 1].Cells["RDAmount"].Value.ToString();
                    txtTVBInpValue.Text = dgvDuty.Rows[tt - 1].Cells["TVBInp"].Value.ToString();
                    txtTVBRate.Text = dgvDuty.Rows[tt - 1].Cells["TVBRate"].Value.ToString();
                    txtTVBAmount.Text = dgvDuty.Rows[tt - 1].Cells["TVBAmount"].Value.ToString();
                    txtSDInpValue.Text = dgvDuty.Rows[tt - 1].Cells["SDInp"].Value.ToString();
                    txtSDRate.Text = dgvDuty.Rows[tt - 1].Cells["SDuty"].Value.ToString();
                    txtSDAmount.Text = dgvDuty.Rows[tt - 1].Cells["SDutyAmount"].Value.ToString();
                    txtVATInpValue.Text = dgvDuty.Rows[tt - 1].Cells["VATInp"].Value.ToString();
                    txtVATRateI.Text = dgvDuty.Rows[tt - 1].Cells["VATRateDuty"].Value.ToString();
                    txtVATAmount.Text = dgvDuty.Rows[tt - 1].Cells["VATAmountDuty"].Value.ToString();
                    txtTVAInpValue.Text = dgvDuty.Rows[tt - 1].Cells["TVAInp"].Value.ToString();
                    txtTVARate.Text = dgvDuty.Rows[tt - 1].Cells["TVARate"].Value.ToString();
                    txtTVAAmount.Text = dgvDuty.Rows[tt - 1].Cells["TVAAmount"].Value.ToString();
                    txtATVInpValue.Text = dgvDuty.Rows[tt - 1].Cells["ATVInp"].Value.ToString();
                    txtATVRate.Text = dgvDuty.Rows[tt - 1].Cells["ATVRate"].Value.ToString();
                    txtATVAmount.Text = dgvDuty.Rows[tt - 1].Cells["ATVAmount"].Value.ToString();
                    txtOthersInpValue.Text = dgvDuty.Rows[tt - 1].Cells["OthersInp"].Value.ToString();
                    txtOthersRate.Text = dgvDuty.Rows[tt - 1].Cells["OthersRate"].Value.ToString();
                    txtOthersAmount.Text = dgvDuty.Rows[tt - 1].Cells["OthersAmount"].Value.ToString();
                    txtAITInpValue.Text = dgvDuty.Rows[tt - 1].Cells["AITInp"].Value.ToString();
                    txtAITAmount.Text = dgvDuty.Rows[tt - 1].Cells["AITAmount"].Value.ToString();
                    txtDutiesRemarks.Text = dgvDuty.Rows[tt - 1].Cells["Remarks"].Value.ToString();


                    #endregion DutiesSelection
                }
                //VDSStatus();


                #endregion DutiesSelection
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

        private void txtPurchaseInvoiceNo_TextChanged(object sender, EventArgs e)
        {
        }

        private void rdbFinished_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
            //VendorDetailsInfo();
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {


        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                //txtQuantity.Focus();
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
            }

        }

        private void ProductLoad()
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
                else if (!string.IsNullOrEmpty(IsRawParam))
                {
                    SqlText += @"  and pc.IsRaw='" + IsRawParam + "'  ";
                }
                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();
                    txtProductCode.SelectedText = selectedRow.Cells["ItemNo"].Value.ToString();
                    vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();

                }
                cmbProduct.Focus();
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

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtUnitCost.Focus();
            }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtSD.Focus();
            }
        }

        private void cmbVendor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                dtpReceiveDate.Focus();
            }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            FormRptPurchaseInformation frmRptPurchaseInformation = new FormRptPurchaseInformation();
            //frmRptPurchaseInformation.txtInvoiceNo.Text = txtPurchaseInvoiceNo.Text.Trim();
            frmRptPurchaseInformation.Show();
        }

        private void btnVendorName_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormVendorSearch frm = new FormVendorSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                Program.fromOpen = "Other";

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormVendorSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                    cmbVendor.Text = selectedRow.Cells["VendorName"].Value.ToString();
                    txtVendorGroupName.Text = selectedRow.Cells["VendorGroupName"].Value.ToString();
                }

                //string result = FormVendorSearch.SelectOne();
                //if (result == "") { return; }
                //else//if (result == ""){return;}else//if (result != "")
                //{
                //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());

                //    txtVendorID.Text = VendorInfo[0];
                //    txtVendorName.Text = VendorInfo[1];
                //    cmbVendor.Text = VendorInfo[1];
                //    txtVendorGroupName.Text = VendorInfo[3];
                //}
                //////SearchVendor();

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

        #endregion

        #region Methods 05

        private void btnProductType_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    #region Statement

            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());
            //        if (TypeInfo[1].ToString() == "Trading")
            //        {
            //            MessageBox.Show("Please select Trading menu for trading item");
            //            return;
            //        }
            //        cmbProductType.Text = TypeInfo[1];
            //    }
            //    //ProductSearch();

            //    #endregion
            //}
            ////#region catch

            ////catch (Exception ex)
            ////{
            ////    string exMessage = ex.Message;
            ////    if (ex.InnerException != null)
            ////    {
            ////        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            ////                    ex.StackTrace;

            ////    }

            ////    StackTrace st = new StackTrace();

            ////    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////    FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            ////}
            ////#endregion
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void dgvPurchase_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtVendorGroupName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUOM_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
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

        private void txtCommentsDetail_TextChanged(object sender, EventArgs e)
        {
            //ChangeData = true;
        }

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 06

        private void dtpInvoiceDate_ValueChanged(object sender, EventArgs e)
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

        private void FormPurchase_FormClosing(object sender, FormClosingEventArgs e)
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtSD, "SD Rate");
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();

            SDInputAmnt();
            VATCalculation();
        }

        private void dtpInvoiceDate_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter))
            //{ 
            //    cmbProductCode.Focus(); 
            //}
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }



        private void SetFormHeader(DataTable dt)
        {
            try
            {
                if (dt.Rows.Count == 0)
                    return;


                #region Clear Form

                FormMaker();
                FormLoad();
                ClearAllFields();
                transactionTypes();

                #endregion

                txtTDSAmount.Text = Program.ParseDecimalObject(dt.Rows[0]["TDSAmount"].ToString());// selectedRow.Cells["VendorID"].Value.ToString();
                txtVendorID.Text = dt.Rows[0]["VendorID"].ToString();// selectedRow.Cells["VendorID"].Value.ToString();
                txtVendorName.Text = dt.Rows[0]["VendorName"].ToString();// selectedRow.Cells["VendorName"].Value.ToString();
                cmbVendor.Text = dt.Rows[0]["VendorName"].ToString();// selectedRow.Cells["VendorName"].Value.ToString();
                //txtVendorGroupID.Text = Purchaseinfo[3];
                txtVendorGroupName.Text = dt.Rows[0]["VendorGroupName"].ToString();//selectedRow.Cells["VendorGroupName"].Value.ToString();
                // Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                txtTotalAmount.Text = Program.ParseDecimalObject(dt.Rows[0]["TotalAmount"].ToString());//                        Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.0000");
                txtTotalVATAmount.Text = Program.ParseDecimalObject(dt.Rows[0]["TotalVATAmount"].ToString());//                        Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.0000");
                txtSerialNo.Text = dt.Rows[0]["SerialNo"].ToString();// selectedRow.Cells["SerialNo"].Value.ToString();
                txtLCNumber.Text = dt.Rows[0]["LCNumber"].ToString();// selectedRow.Cells["LCNumber"].Value.ToString();
                txtComments.Text = dt.Rows[0]["Comments"].ToString();// selectedRow.Cells["Comments"].Value.ToString();
                //txtIsRaw.Text = Purchaseinfo[10];
                IsPost = Convert.ToString(dt.Rows[0]["Post"].ToString()) == "Y" ? true : false;// Convert.ToString(selectedRow.Cells["Post1"].Value.ToString()) == "Y" ? true : false;
                txtBENumber.Text = dt.Rows[0]["BENumber"].ToString();// selectedRow.Cells["BENumber"].Value.ToString();
                cmbVDS.Text = dt.Rows[0]["WithVDS"].ToString();// selectedRow.Cells["WithVDS"].Value.ToString();
                txtPrePurId.Text = dt.Rows[0]["PurchaseReturnId"].ToString();// selectedRow.Cells["ReturnId"].Value.ToString();
                txtCustomHouse.Text = dt.Rows[0]["CustomHouse"].ToString();// selectedRow.Cells["CustomHouse"].Value.ToString();
                //txtCustomHouseCode.Text = dt.Rows[0]["CustomCode"].ToString() == null ? "" : dt.Rows[0]["CustomCode"].ToString();// selectedRow.Cells["CustomHouse"].Value.ToString();
                txtUSDInvoiceValue.Text = dt.Rows[0]["USDInvoiceValue"].ToString();// selectedRow.Cells["USDInvoiceValue"].Value.ToString();
                chkTDS.Checked = dt.Rows[0]["IsTDS"] != null && dt.Rows[0]["IsTDS"].ToString() == "Y";



                if (dt.Rows[0]["InvoiceDateTime"] != null &&
                    !string.IsNullOrEmpty(dt.Rows[0]["InvoiceDateTime"].ToString()))
                {
                    dtpInvoiceDate.Value = Convert.ToDateTime(dt.Rows[0]["InvoiceDateTime"].ToString());

                }
                else
                {
                    dtpInvoiceDate.Value = Convert.ToDateTime(dt.Rows[0]["ReceiveDate"].ToString());
                }


                if (dt.Rows[0]["ReceiveDate"] != null &&
                    !string.IsNullOrEmpty(dt.Rows[0]["ReceiveDate"].ToString()))
                {
                    dtpReceiveDate.Value = Convert.ToDateTime(dt.Rows[0]["ReceiveDate"].ToString());

                }
                else
                {
                    dtpReceiveDate.Value = DateTime.Now;
                }


                PurchaseDetailData = dt.Rows[0]["ID"].ToString();
                //if (selectedRow.Cells["LCNumber"].Value.ToString() != "-")
                if (dt.Rows[0]["LCNumber"].ToString() != "-")
                {
                    //if (!string.IsNullOrEmpty(selectedRow.Cells["LCDate"].Value.ToString()))
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LCDate"].ToString()))
                    {
                        dtpLCDate.Value = Convert.ToDateTime(dt.Rows[0]["LCDate"].ToString());// Convert.ToDateTime(selectedRow.Cells["LCDate"].Value.ToString());
                    }
                }

                txtLandedCost.Text = Program.ParseDecimalObject(dt.Rows[0]["LandedCost"].ToString());//   Convert.ToDecimal(selectedRow.Cells["LandedCost"].Value.ToString()).ToString();//"0,0.0000");


                bgwPurchaseDetailTemp.RunWorkerAsync();
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

        private void SetHeader(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;

            #region Value Assing to Form Elements

            DataRow dr = dt.Rows[0];
            txtPurchaseInvoiceNo.Text = dr["PurchaseInvoiceNo"].ToString();
            txtId.Text = dr["Id"].ToString();
            txtFiscalYear.Text = dr["FiscalYear"].ToString();
            SearchBranchId = Convert.ToInt32(dr["BranchId"]);
            txtTDSAmount.Text = Program.ParseDecimalObject(dr["TDSAmount"].ToString());
            txtVendorID.Text = dr["VendorID"].ToString();
            txtVendorName.Text = dr["VendorName"].ToString();
            cmbVendor.Text = dr["VendorName"].ToString();
            txtVendorGroupName.Text = dr["VendorGroupName"].ToString();
            dtpInvoiceDate.Value = Convert.ToDateTime(dr["InvoiceDateTime"].ToString());
            txtTotalAmount.Text = Program.ParseDecimalObject(dr["TotalAmount"].ToString());
            txtTotalVATAmount.Text = Program.ParseDecimalObject(dr["TotalVATAmount"].ToString());
            txtSerialNo.Text = dr["SerialNo"].ToString();
            txtLCNumber.Text = dr["LCNumber"].ToString();
            txtComments.Text = dr["Comments"].ToString();
            IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
            txtBENumber.Text = dr["BENumber"].ToString();
            cmbVDS.Text = dr["WithVDS"].ToString();
            txtPrePurId.Text = dr["PurchaseReturnId"].ToString();
            txtCustomHouse.Text = dr["CustomHouse"].ToString();
            txtCustomHouseCode.Text = dr["CustomCode"].ToString();
            txtUSDInvoiceValue.Text = dr["USDInvoiceValue"].ToString();
            chkTDS.Checked = dr["IsTDS"] != null && dr["IsTDS"].ToString() == "Y";
            dtpReceiveDate.Value = Convert.ToDateTime(dr["ReceiveDate"].ToString());

            #region Conditional Values

            if (txtPurchaseInvoiceNo.Text == "")
            {
                PurchaseDetailData = "00";
            }
            else
            {
                PurchaseDetailData = txtPurchaseInvoiceNo.Text.Trim();
            }
            if (dr["LCNumber"].ToString() != "-")
            {
                if (!string.IsNullOrEmpty(dr["LCDate"].ToString()))
                {
                    dtpLCDate.Value = Convert.ToDateTime(dr["LCDate"].ToString());
                }
            }

            #endregion


            txtHiddenInvoiceNo.Text = txtPurchaseInvoiceNo.Text;

            txtLandedCost.Text = Program.ParseDecimalObject(dr["LandedCost"].ToString());

            #endregion

            #region Background Workder

            bgwSearchPurchase.RunWorkerAsync();

            #endregion

            #region Button Stats

            this.btnSearchInvoiceNo.Enabled = false;
            this.progressBar1.Visible = true;

            #endregion
        }




        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtVATRate.Focus();
            }
        }

        private void btnProductSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement



                Program.fromOpen = "Other";
                Program.R_F = cmbProductType.Text.Trim();
                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    ProductCode = selectedRow.Cells["ItemNo"].Value.ToString(); //ProductInfo[0];
                    ProductName = selectedRow.Cells["ProductName"].Value.ToString();// ProductInfo[1];
                    txtPCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();// ProductInfo[27];

                    txtCategoryName.Text = selectedRow.Cells["CategoryName"].Value.ToString(); //ProductInfo[4];
                    txtHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString(); //ProductInfo[11];
                    txtIsRaw.Text = selectedRow.Cells["IsRaw"].Value.ToString(); //ProductInfo[9];
                    txtVATRate.Text = "0.00";
                    txtSD.Text = "0.00";

                    txtVATRate.Text = Convert.ToDecimal(selectedRow.Cells["VATRate"].Value.ToString()).ToString();//"0.00");//12
                    txtSD.Text = Convert.ToDecimal(selectedRow.Cells["SD"].Value.ToString()).ToString();//"0.00");//18
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    //txtQuantityInHand.Text = productDal.StockInHand(ProductCode,
                    //    dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null).ToString();
                    txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM, Program.CurrentUserID).Rows[0]["Quantity"].ToString();

                    //txtQuantityInHand.Text = Convert.ToDecimal(ProductInfo[17]).ToString();//"0.00");
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString(); //ProductInfo[5];
                    txtNBRPrice.Text = Convert.ToDecimal(selectedRow.Cells["NBRPrice"].Value.ToString()).ToString();//"0.0000");    // NBR Price 8
                }


                //ProductSearch();
                txtProductName.Text = ProductName;
                txtProductCode.Text = ProductCode;
                cmbProduct.Text = ProductName;

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

        private void txtSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }
        }

        #endregion

        #region Methods 07

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                ////MDIMainInterface mdi = new MDIMainInterface();
                FormRptPurchaseInformation frmRptPurchaseInformation = new FormRptPurchaseInformation();
                #region Transaction Type



                transactionTypes();

                #endregion Transaction Type


                if (txtPurchaseInvoiceNo.Text == "~~~ New ~~~")
                {
                    frmRptPurchaseInformation.txtInvoiceNo.Text = "";
                }
                else
                {
                    frmRptPurchaseInformation.txtInvoiceNo.Text = txtPurchaseInvoiceNo.Text.Trim();
                    frmRptPurchaseInformation.txtTransactionType.Text = transactionType;

                }

                frmRptPurchaseInformation.ShowDialog();

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

        private void btnVAT16_Click(object sender, EventArgs e)
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
                FormRptVAT16 frmRptVAT16 = new FormRptVAT16();

                if (SearchBranchId > 0)
                {
                    frmRptVAT16.SenderBranchId = SearchBranchId;
                }
                else
                {
                    frmRptVAT16.SenderBranchId = Program.BranchId;

                }


                //mdi.RollDetailsInfo("8201");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvPurchase.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.txtUOM.Text = dgvPurchase.CurrentRow.Cells["UOMn"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.txtTransType.Text = transactionType;
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
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
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                frmRptVAT18.dtpToDate.Value = dtpReceiveDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpReceiveDate.Value;
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void txtPCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void PriceLoad()
        {
            try
            {
                #region Statement


                var searchText = cmbProduct.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    ProductDAL _pdal = new ProductDAL();
                    //IProduct _pdal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                    DataTable pdt = new DataTable();

                    #region Condtional Values

                    if (string.IsNullOrWhiteSpace(vItemNo))
                    {
                        pdt = _pdal.ProductDTByItemNo("", searchText, null, null, connVM);
                    }
                    else
                    {
                        pdt = _pdal.ProductDTByItemNo(vItemNo, "");
                    }

                    #endregion

                    vItemNo = "";

                    if (pdt.Rows.Count > 0)
                    {

                        #region Value Assign

                        decimal VATRate = 0;
                        decimal VATRate2 = 0;
                        int i = 0;
                        int indexVATRate2 = 0;
                        int indexVATRate = 0;

                        //////vItemNo = pdt.Rows[0]["ItemNo"].ToString();

                        cmbVATRateInput.Items.Clear();
                        cmbVATRateInput.Refresh();
                        txtIsVDS.Text = pdt.Rows[0]["IsVDS"].ToString();
                        txtProductName.Text = pdt.Rows[0]["ProductName"].ToString();
                        txtProductCode.Text = pdt.Rows[0]["ItemNo"].ToString();
                        txtItemNo.Text = pdt.Rows[0]["ItemNo"].ToString();


                        txtUOM.Text = pdt.Rows[0]["UOM"].ToString();
                        txtHSCode.Text = pdt.Rows[0]["HSCodeNo"].ToString();
                        VATRate = Convert.ToDecimal(pdt.Rows[0]["VATRate"].ToString());
                        VATRate2 = Convert.ToDecimal(pdt.Rows[0]["VATRate2"].ToString());
                        cmbVATRateInput.Items.Insert(i, "0");

                        #region Conditional Values

                        if (VATRate2 > 0)
                        {
                            i++;
                            indexVATRate2 = i;
                            cmbVATRateInput.Items.Insert(i, Program.FormatingNumeric(VATRate2.ToString(), 2));
                        }
                        if (VATRate > 0)
                        {
                            i++;
                            indexVATRate = i;
                            cmbVATRateInput.Items.Insert(i, Program.FormatingNumeric(VATRate.ToString(), 2));
                        }

                        if (VATRate > VATRate2)
                        {
                            cmbVATRateInput.SelectedIndex = indexVATRate;
                        }
                        else if (VATRate2 > 0)
                        {
                            cmbVATRateInput.SelectedIndex = indexVATRate2;
                        }
                        else
                        {
                            cmbVATRateInput.SelectedIndex = 0;
                        }



                        if (VATRate != 0 && VATRate != 15)
                        {
                            txtVDSRate.Text = VATRate.ToString();////pdt.Rows[0]["VDSRate"].ToString();// products.HSCodeNo;

                        }
                        else
                        {
                            txtVDSRate.Text = "0";
                        }

                        #endregion

                        #region Conditional Values

                        if (rbtnTollReceive.Checked)
                        {
                            #region NBRPrice Call

                            NBRPriceCall();

                            #endregion
                        }
                        else if (rbtnPurchaseReturn.Checked)
                        {

                            txtVATRate.Text = pdt.Rows[0]["VATRate"].ToString();
                            txtSD.Text = pdt.Rows[0]["SD"].ToString();
                            txtPurQty.Text = "0";
                            txtUnitCost.Text = "0";
                        }
                        if (chkImport.Checked)
                        {
                            txtVATRate.ReadOnly = true;
                            txtSD.ReadOnly = true;
                            txtVATRate.Text = "0";
                            txtSD.Text = "0";
                        }

                        else
                        {
                            txtVATRate.ReadOnly = false;
                            txtSD.ReadOnly = false;
                            txtVATRate.Text = pdt.Rows[0]["VATRate"].ToString();// products.VATRate.ToString();//"0.00");
                            txtSD.Text = pdt.Rows[0]["SD"].ToString();// products.SD.ToString();//"0.00");
                        }

                        #endregion


                        txtRebatePercent.Text = pdt.Rows[0]["RebatePercent"].ToString();

                        #region Stock Call

                        bgwStock.RunWorkerAsync();

                        #endregion



                        txtPCode.Text = pdt.Rows[0]["ProductCode"].ToString();
                        txtNBRPrice.Text = pdt.Rows[0]["NBRPrice"].ToString();
                        txtBOMPrice.Text = "0";

                        if (transactionType == "ClientFGReceiveWOBOM")
                        {
                            txtUnitCost.Text = pdt.Rows[0]["NBRPrice"].ToString();
                            vUnitCost = Convert.ToDecimal(txtUnitCost.Text.Trim());
                        }

                        #region UOM Functions

                        Uoms();

                        #endregion

                        txtSection.Text = pdt.Rows[0]["TDSSection"].ToString();
                        txtCode.Text = pdt.Rows[0]["TDSCode"].ToString();
                        txtCDInput.Text = pdt.Rows[0]["CDRate"].ToString();
                        txtRDInput.Text = pdt.Rows[0]["RDRate"].ToString();
                        txtSDInput.Text = pdt.Rows[0]["SDRate"].ToString();
                        txtVATInput.Text = pdt.Rows[0]["VATRate3"].ToString();
                        txtATVInput.Text = pdt.Rows[0]["ATVRate"].ToString();
                        txtAITInput.Text = pdt.Rows[0]["AITRate"].ToString();
                        txtTVAInput.Text = pdt.Rows[0]["TVARate"].ToString();
                        chkIsHouseRent.Checked = (pdt.Rows[0]["IsHouseRent"].ToString()) == "Y" ? true : false;
                        chkSD.Checked = (pdt.Rows[0]["IsFixedSD"].ToString()) == "Y" ? true : false;
                        chkCD.Checked = (pdt.Rows[0]["IsFixedCD"].ToString()) == "Y" ? true : false;
                        chkRD.Checked = (pdt.Rows[0]["IsFixedRD"].ToString()) == "Y" ? true : false;
                        chkAIT.Checked = (pdt.Rows[0]["IsFixedAIT"].ToString()) == "Y" ? true : false;
                        chkVAT.Checked = (pdt.Rows[0]["IsFixedVAT1"].ToString()) == "Y" ? true : false;
                        chkAT.Checked = (pdt.Rows[0]["IsFixedAT"].ToString()) == "Y" ? true : false;
                        chkIsFixedOtherSD.Checked = (pdt.Rows[0]["IsFixedOtherSD"].ToString()) == "Y" ? true : false;
                        chkIsFixedOtherVAT.Checked = (pdt.Rows[0]["IsFixedVAT"].ToString()) == "Y" ? true : false;
                        chkIsFixedVATRebate.Checked = (pdt.Rows[0]["IsFixedVATRebate"].ToString()) == "Y";
                        //string IsFixedVATRebate = pdt.Rows[0]["IsFixedVATRebate"].ToString();

                        #region Condtional Values

                        if ((rbtnImport.Checked == true && dutyCalculate == true)
       || (rbtnTradingImport.Checked == true && dutyCalculate == true)
       || (rbtnServiceImport.Checked == true && dutyCalculate == true)
        || (rbtnInputServiceImport.Checked == true && dutyCalculate == true)
        || (rbtnCommercialImporter.Checked == true && dutyCalculate == true)
       || (rbtnServiceNSImport.Checked == true && dutyCalculate == true))
                        {
                            if (chkVAT.Checked)
                            {
                                LVR.Text = "VAT(F)";
                                txtVATRate.ReadOnly = true;
                                txtVATRate.Text = "0";
                                txtVATRate.Text = txtVATInput.Text;
                                cmbVATRateInput.Enabled = false;
                            }
                        }
                        else
                        {
                            txtLocalVATAmount.Text = "0";
                            txtLocalSDAmount.Text = "0";

                            txtSD.Text = Program.ParseDecimal(pdt.Rows[0]["SD"].ToString());
                            txtVATRate.Text = pdt.Rows[0]["VATRate"].ToString();

                            LVR.Text = "VAT(%)";
                            l14.Text = "SD(%)";
                            lblVDSRate.Text = "VDS(%)";

                            if (chkIsFixedOtherVAT.Checked)
                            {
                                LVR.Text = "VAT(F)";
                                txtVATRate.ReadOnly = true;
                                txtVATRate.Text = "0";
                                cmbVATRateInput.Enabled = false;

                                if (chkIsFixedVATRebate.Checked)
                                {
                                    cmbType.SelectedValue = "fixedvat(rebate)";
                                }
                                else
                                {
                                    cmbType.SelectedValue = "fixedvat";
                                }
                                txtVATRate.Text = pdt.Rows[0]["FixedVATAmount"].ToString();
                                lblVDSRate.Text = "VDS(F)";

                            }
                            if (chkIsFixedOtherSD.Checked)
                            {
                                l14.Text = "SD(F)";
                                txtSD.ReadOnly = true;
                                txtLocalSDAmount.ReadOnly = true;
                                txtSD.Text = "0";
                                txtSD.Text = Program.ParseDecimal(pdt.Rows[0]["SD"].ToString());
                            }

                        }

                        #endregion

                        #endregion

                    }
                    else
                    {
                        MessageBox.Show("Please select the right item", this.Text);
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Check Point

                if (cmbProduct.Text.Trim().ToLower() == "select")
                {
                    return;
                }
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "0.00";
                cmbVATRateInput.Text = "0.00";
                if (txtVendorName.Text == "")
                {
                    MessageBox.Show("Please Select Vendor", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
                if (rbtnInputService.Checked || rbtnInputServiceImport.Checked || rbtnServiceNS.Checked || rbtnServiceNSImport.Checked || rbtnPurchaseTollcharge.Checked)
                {
                    txtQuantity.Text = "1";
                }

                #endregion

                #region Price load

                PriceLoad();

                #endregion

                #region BOM Call
                if (rbtnTollReceive.Checked)
                {
                }
                else
                {
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    DataTable dt = new DataTable();
                    //BugsBD
                    string ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());

                    dt = productDal.SelectBOMRaw(ProductCode, dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

                    //dt = productDal.SelectBOMRaw(txtProductCode.Text.Trim(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

                    int BOMId = 0;



                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        string tempBOMId = dr["BOMId"].ToString();
                        if (!string.IsNullOrWhiteSpace(tempBOMId))
                        {
                            BOMId = Convert.ToInt32(tempBOMId);
                        }
                    }

                    txtBOMId.Text = BOMId.ToString();
                }

                #endregion

                #region Conditional Values
                if (VATTypeVATAutoChange.ToLower() == "y")
                {
                    if (cmbType.SelectedValue != null)
                    {
                        if (cmbType.SelectedValue.ToString() == "fixedvat" ||
                            cmbType.SelectedValue.ToString() == "fixedvat(rebate)")
                        {

                        }
                        else
                        {
                            cmbType.SelectedValue = VATTypeCal(cmbVATRateInput.Text.Trim()).ToLower();
                        }
                    }
                    else
                    {
                        cmbType.SelectedValue = VATTypeCal(cmbVATRateInput.Text.Trim()).ToLower();
                    }
                }
                #endregion

                #region VDS Status

                VDSStatus();

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
        private void VDSStatus()
        {
            chkIsHouseRent.Checked = false;
            cmbVDS.Text = "N";

            decimal vatRate = Convert.ToDecimal(txtVATRate.Text.Trim());
            bool IsVDS = txtIsVDS.Text.Trim().ToLower() == "y" ? true : false;

            if (chkImport.Checked == true)
            {
                chkIsHouseRent.Checked = false;
                cmbVDS.Text = "N";
            }
            else if (rbtnImport.Checked)
            {
                chkIsHouseRent.Checked = false;
                cmbVDS.Text = "N";
            }
            else
            {
                if (rbtnInputService.Checked || rbtnPurchaseTollcharge.Checked)
                {
                    if (IsVDS)
                    {
                        chkIsHouseRent.Checked = true;
                        cmbVDS.Text = "Y";
                    }
                }
                //else if (cmbType.Text=="")
                //{
                //    if (IsVDS)
                //    {
                //        chkIsHouseRent.Checked = true;
                //        cmbVDS.Text = "Y";
                //    }
                //}

                else
                {

                    if (vatRate == 15 || vatRate == 0)
                    {
                        chkIsHouseRent.Checked = false;
                        txtVDSRate.Text = "0";
                        txtVDSAmount.Text = "0";
                    }
                    else
                    {
                        chkIsHouseRent.Checked = true;
                        txtVDSRate.Text = vatRate.ToString();
                        txtVDSAmount.Text = txtLocalVATAmount.Text;
                    }

                }
            }
        }
        private string VATTypeCal(string vatRate)
        {
            try
            {
                decimal rate = Convert.ToDecimal(vatRate);

                string type = "";

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

                return type;
            }
            catch (Exception e)
            {

                return "";
            }

        }

        private void cmbVendor_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtVendorID.Text = "";
                txtVendorName.Text = "";
                txtVendorGroupName.Text = "";

                var searchText = cmbVendor.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    var vendors = new List<VendorMiniDTO>();


                    vendors = vendorsMini.Where(x => x.VendorName.ToLower() == searchText).ToList();

                    if (vendors.Any())
                    {
                        var customer = vendors.First();
                        txtVendorID.Text = customer.VendorID;
                        txtVendorName.Text = customer.VendorName;
                        txtVendorGroupName.Text = customer.VendorGroupName;
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void FormPurchase_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void dgvPurchase_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //Rowcalculate();
        }


        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void btnOHAdd_Click(object sender, EventArgs e)
        {
            try
            {


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

        #region Methods 08

        private void selectDLastRow()
        {
            try
            {
                #region Statement

                if (dgvDuty.Rows.Count > 0)
                {
                    dgvDuty.Rows[dgvDuty.Rows.Count - 1].Selected = true;
                    dgvDuty.CurrentCell = dgvDuty.Rows[dgvDuty.Rows.Count - 1].Cells[1];
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

        private void btnOHChange_Click(object sender, EventArgs e)
        {
            try
            {


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

        private void btnOHDelete_Click(object sender, EventArgs e)
        {
            try
            {

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

        private void dgvDuty_DoubleClick(object sender, EventArgs e)
        {
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void cmbType1_Leave(object sender, EventArgs e)
        {
            vendors();
        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {

            //cmbProduct.Enabled = false;

        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            //if (rbtnProduct.Checked)
            //{
            //    cmbProductCode.Enabled = false;
            //    //cmbProduct.Enabled = false;

            //}
        }

        private void NBRPriceCall()
        {
            ProductDAL productDal = new ProductDAL();

            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            //BugsBD
            string ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());
            string UnitCost = OrdinaryVATDesktop.SanitizeInput(txtUnitCost.Text.Trim());

            if (rbtnTollReceive.Checked)
            {

                txtUnitCost.Text = productDal.GetLastTollChargeFBOMOH(ProductCode, "VAT 4.3 (Toll Receive)", dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM).ToString();
                //txtUnitCost.Text = productDal.GetLastTollChargeFBOMOH(txtProductCode.Text.Trim(), "VAT 4.3 (Toll Receive)", dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM).ToString();

            }


            if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
            {

                txtUnitCost.Text = Program.FormatingNumeric(UnitCost, PurchasePlaceAmt).ToString();
                //txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), PurchasePlaceAmt).ToString();

            }

        }

        private void cmbUom_Leave(object sender, EventArgs e)
        {
            UomsValue();
            txtQuantity.Focus();
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

                        var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom && x.UOMTo.Trim().ToLower() == uOMTo).ToList()
                                   select uom.Convertion;
                        if (uoms != null && uoms.Any())
                        {
                            txtUomConv.Text = uoms.First().ToString();
                            decimal Vuomc = Convert.ToDecimal(txtUomConv.Text.Trim());
                            decimal amnt = 0;
                            if (IsTotalPrice)
                            {
                                amnt = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                                amnt = amnt * Vuomc;
                                txtTotalAmount.Text = amnt.ToString();
                                ////txtUnitCost.Text = "0";
                            }
                            else
                            {
                                amnt = Convert.ToDecimal(vUnitCost);
                                amnt = amnt * Vuomc;
                                txtUnitCost.Text = amnt.ToString();
                                txtTotalAmount.Text = "0";

                            }

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

        private void cmbType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

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
                //    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvPurchase.Rows.Count > 0)
                {
                    frmRptVAT17.txtItemNo.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT17.txtProductName.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT17.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpReceiveDate.Value;

                    frmRptVAT17.rbtnService.Checked = false;

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }


        #endregion

        #region Methods 09


        private void txtCnFInpValue_TextChanged(object sender, EventArgs e)
        {
            txtCnFAmount.Text = txtCnFInpValue.Text;

            ChangeData = true;
        }

        private void DutyPreCalculation()
        {
            decimal qty = Convert.ToDecimal(txtQuantity.Text.Trim());
            decimal UomConv = Convert.ToDecimal(txtUomConv.Text == "" ? "1" : txtUomConv.Text);
            if ((rbtnImport.Checked == true)
                || (rbtnTradingImport.Checked == true)
                || (rbtnServiceImport.Checked == true)
                || (rbtnInputServiceImport.Checked == true)
                || (rbtnCommercialImporter.Checked == true)
                || (rbtnServiceNSImport.Checked == true))
            {

                #region 15 Feb 2021

                decimal subtotal = 0;
                decimal PreCDRate = 0;
                decimal PreRDRate = 0;
                decimal PreSDRate = 0;
                decimal PreVATRate = 0;
                decimal PreATRate = 0;
                decimal PreAITRate = 0;



                #region Get InpValue

                if (!string.IsNullOrWhiteSpace(txtTotalSubTotal1.Text.ToString()))
                {
                    subtotal = Convert.ToDecimal(txtTotalSubTotal1.Text);
                }

                if (subtotal == 0)
                {
                    MessageBox.Show("There is no data in purchase details, Please input data first", this.Text);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(txtCDInput.Text.ToString()))
                {
                    PreCDRate = Convert.ToDecimal(txtCDInput.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtRDInput.Text.ToString()))
                {
                    PreRDRate = Convert.ToDecimal(txtRDInput.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtSDInput.Text.ToString()))
                {
                    PreSDRate = Convert.ToDecimal(txtSDInput.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtVATInput.Text.ToString()))
                {
                    PreVATRate = Convert.ToDecimal(txtVATInput.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtATVInput.Text.ToString()))
                {
                    PreATRate = Convert.ToDecimal(txtATVInput.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtAITInput.Text.ToString()))
                {
                    PreAITRate = Convert.ToDecimal(txtAITInput.Text);
                }

                #endregion

                decimal CDAmount = subtotal * PreCDRate / 100;
                decimal RDAmount = subtotal * PreRDRate / 100;
                decimal SDAmount = (subtotal + CDAmount + RDAmount) * PreSDRate / 100;
                decimal VATAmount = (subtotal + CDAmount + RDAmount + SDAmount) * PreVATRate / 100;
                if (chkVAT.Checked)
                {
                    VATAmount = qty * PreVATRate;
                }
                decimal ATAmount = (subtotal + CDAmount + RDAmount + SDAmount) * PreATRate / 100;
                decimal AITAmount = subtotal * PreAITRate / 100;

                #region Value Assign

                txtCDAmountP.Text = CDAmount.ToString();

                txtRDAmountP.Text = RDAmount.ToString();
                txtSDAmountP.Text = SDAmount.ToString();
                txtVATAmountP.Text = VATAmount.ToString();
                txtATVAmountP.Text = ATAmount.ToString();
                txtAITAmountP.Text = AITAmount.ToString();

                #endregion

                #endregion

                #region Comments 15 Feb 2021

                //decimal vsubTotal = 0;
                //decimal vInpCnf = 0;
                //decimal vInpInsurance = 0;
                //decimal vInpAV = 0;
                //decimal vAVAmount = 0;
                //decimal vInpCD = 0;
                //decimal vInpRD = 0;
                //////////decimal vInpTVB = 0;
                //decimal vInpSD = 0;
                //decimal vInpVAT = 0;
                //decimal vInpTVA = 0;
                //decimal vInpATV = 0;
                //decimal vInpAIT = 0;

                //////////decimal vInpOthers = 0;

                //if (!string.IsNullOrEmpty(txtTotalSubTotal1.Text.Trim()))
                //    vsubTotal = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                //else
                //{
                //    MessageBox.Show("There is no data in purchase details, Please input data first", this.Text);
                //    return;
                //}
                //////////vsubTotal = 1;
                //if (vsubTotal == 0)
                //{
                //    MessageBox.Show("There is no data in purchase details, Please input data first", this.Text);
                //    return;
                //}
                //#region InputValue

                //if (!string.IsNullOrEmpty(txtCnFInpValue.Text.Trim()))
                //    vInpCnf = Convert.ToDecimal(txtCnFInpValue.Text.Trim());
                //else
                //    vInpCnf = 0;

                //if (!string.IsNullOrEmpty(txtAITInput.Text.Trim()))
                //    vInpAIT = Convert.ToDecimal(txtAITInput.Text.Trim());
                //else
                //    vInpAIT = 0;



                //if (!string.IsNullOrEmpty(txtInsInpValue.Text.Trim()))
                //    vInpInsurance = Convert.ToDecimal(txtInsInpValue.Text.Trim());
                //else
                //    vInpInsurance = 0;

                //if (!string.IsNullOrEmpty(txtAVInpValue.Text.Trim()))
                //    vInpAV = Convert.ToDecimal(txtAVInpValue.Text.Trim());
                //else
                //    vInpAV = 0;

                //if (!string.IsNullOrEmpty(txtCDInput.Text.Trim()))
                //    vInpCD = Convert.ToDecimal(txtCDInput.Text.Trim());
                //else
                //    vInpCD = 0;



                //if (!string.IsNullOrEmpty(txtRDInput.Text.Trim()))
                //    vInpRD = Convert.ToDecimal(txtRDInput.Text.Trim());
                //else
                //    vInpRD = 0;

                //////////if (!string.IsNullOrEmpty(txtTVBInpValue.Text.Trim()))
                //////////    vInpTVB = Convert.ToDecimal(txtTVBInpValue.Text.Trim());
                //////////else
                //////////    vInpTVB = 0;

                //if (!string.IsNullOrEmpty(txtSDInput.Text.Trim()))
                //    vInpSD = Convert.ToDecimal(txtSDInput.Text.Trim());
                //else
                //    vInpSD = 0;

                //if (!string.IsNullOrEmpty(txtVATInput.Text.Trim()))
                //    vInpVAT = Convert.ToDecimal(txtVATInput.Text.Trim());
                //else
                //    vInpVAT = 0;

                //if (!string.IsNullOrEmpty(txtTVAInput.Text.Trim()))
                //    vInpTVA = Convert.ToDecimal(txtTVAInput.Text.Trim());
                //else
                //    vInpTVA = 0;



                //if (!string.IsNullOrEmpty(txtATVInput.Text.Trim()))
                //    vInpATV = Convert.ToDecimal(txtATVInput.Text.Trim());
                //else
                //    vInpATV = 0;

                //////////if (!string.IsNullOrEmpty(txtOthersInpValue.Text.Trim()))
                //////////    vInpOthers = Convert.ToDecimal(txtOthersInpValue.Text.Trim());
                //////////else
                //////////vInpOthers = 0;
                //#endregion InputValue

                //#region FixedCnF
                //if (FixedCnF == true)
                //{
                //    txtCnFAmount.Text = vInpCnf.ToString();
                //    txtCnFRate.Text = Convert.ToString(vInpCnf * 100 / vsubTotal);
                //}
                //else
                //{
                //    txtCnFAmount.Text = Convert.ToString(vsubTotal * vInpCnf / 100);
                //    txtCnFRate.Text = vInpCnf.ToString();
                //}
                //#endregion FixedCnF
                //#region FixedInsurance

                //decimal vTotalInsuranceApply = 0;
                //vTotalInsuranceApply = vsubTotal + Convert.ToDecimal(txtCnFAmount.Text.Trim());

                //if (FixedInsurance == true)
                //{
                //    txtInsAmount.Text = vInpInsurance.ToString();
                //    txtInsRate.Text = Convert.ToString(vInpInsurance * 100 / vTotalInsuranceApply);
                //}
                //else
                //{
                //    txtInsAmount.Text = Convert.ToString(vTotalInsuranceApply * vInpInsurance / 100);
                //    txtInsRate.Text = vInpInsurance.ToString();
                //}
                //#endregion FixedInsurance
                //#region CalculativeAV

                //if (CalculativeAV == true)
                //{
                //    vAVAmount = vsubTotal + Convert.ToDecimal(txtCnFAmount.Text.Trim()) +
                //                       Convert.ToDecimal(txtInsAmount.Text.Trim());
                //}
                //else
                //{
                //    vAVAmount = vInpAV;
                //}
                //txtAVAmount.Text = Convert.ToString(vAVAmount);
                ////////////if (vAVAmount == 0)
                ////////////vAVAmount = 1;
                //if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                //    vAVAmount = Convert.ToDecimal(txtAVAmount.Text.Trim());
                //else
                //{
                //    MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);
                //    return;
                //}
                //////////vsubTotal = 1;
                //if (vAVAmount == 0)
                //{
                //    MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);

                //    return;
                //}

                //#endregion CalculativeAV

                //#region FixedCD

                //txtCDAmountP.Text = Convert.ToString(vAVAmount * vInpCD / 100);
                //if (chkCD.Checked)
                //{
                //    txtCDAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpCD.ToString())).ToString();
                //}
                //#endregion FixedCD
                //#region FixedRD

                //txtRDAmountP.Text = Convert.ToString(vAVAmount * vInpRD / 100);
                //if (chkRD.Checked)
                //{
                //    txtRDAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpRD.ToString())).ToString();
                //}
                //#endregion FixedCD
                //#region FixedTVB

                //decimal vTotalTVBApply = 0;
                //vTotalTVBApply = vAVAmount + Convert.ToDecimal(txtCDAmountP.Text.Trim()) + Convert.ToDecimal(txtRDAmountP.Text.Trim());

                //////////////if (FixedTVB == true)
                //////////////{
                //////////////    txtTVBAmount.Text = vInpTVB.ToString();
                //////////////    txtTVBRate.Text = Convert.ToString(vInpTVB * 100 / vTotalTVBApply);
                //////////////}
                //////////////else
                //////////////{
                //////////////    txtTVBAmount.Text = Convert.ToString(vTotalTVBApply * vInpTVB / 100);
                //////////////    txtTVBRate.Text = vInpTVB.ToString();
                //////////////}

                //#endregion FixedTVB
                //#region FixedSD

                //decimal vTotalSDApply = 0;
                //vTotalSDApply = vTotalTVBApply;

                //txtSDAmountP.Text = Convert.ToString(vTotalSDApply * vInpSD / 100);
                //if (chkSD.Checked)
                //{
                //    txtSDAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpSD.ToString())).ToString();
                //}

                //#endregion FixedSD
                //#region FixedVAT

                //decimal vTotalVATApply = 0;
                //vTotalVATApply = vTotalSDApply + Convert.ToDecimal(txtSDAmountP.Text.Trim());


                //txtVATAmountP.Text = Convert.ToString(vTotalVATApply * vInpVAT / 100);
                //if (chkVAT.Checked)
                //{
                //    txtVATAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpVAT.ToString())).ToString();
                //}

                //#endregion FixedVAT
                //#region FixedTVA
                //vTotalVATApply = vTotalSDApply + Convert.ToDecimal(txtSDAmountP.Text.Trim());

                //txtTVAAmountP.Text = Convert.ToString(vTotalVATApply * vInpTVA / 100);


                //#endregion FixedTVA
                //#region FixedATV
                //decimal vTotalATVApply = 0;
                //vTotalATVApply = vTotalVATApply + Convert.ToDecimal(txtTVAAmountP.Text.Trim());


                //txtATVAmountP.Text = Convert.ToString(vTotalATVApply * vInpATV / 100);
                //if (chkAT.Checked)
                //{
                //    txtATVAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpATV.ToString())).ToString();
                //}
                //#endregion FixedTVA

                //#region FixedAIT
                //txtAITAmountP.Text = Convert.ToString(vAVAmount * vInpAIT / 100);

                //if (chkAIT.Checked)
                //{
                //    txtAITAmountP.Text = (qty * UomConv * Convert.ToDecimal(vInpAIT.ToString())).ToString();
                //}
                //#endregion FixedAIT


                //#region FixedOthers
                ////////////decimal vTotalOthersApply = 0;
                ////////////vTotalOthersApply = vTotalVATApply;

                ////////////if (FixedOthers == true)
                ////////////{
                ////////////    txtOthersAmount.Text = vInpOthers.ToString();
                ////////////    txtOthersRate.Text = Convert.ToString(vInpOthers * 100 / vTotalOthersApply);
                ////////////}
                ////////////else
                ////////////{
                ////////////    txtOthersAmount.Text = Convert.ToString(vTotalOthersApply * vInpOthers / 100);
                ////////////    txtOthersRate.Text = vInpOthers.ToString();
                ////////////}

                //#endregion FixedOthers

                #endregion

            }

        }

        private void DutyCalculation()
        {
            if ((rbtnImport.Checked == true)
                || (rbtnTradingImport.Checked == true)
                || (rbtnServiceImport.Checked == true)
                || (rbtnInputServiceImport.Checked == true)
                || (rbtnCommercialImporter.Checked == true)
                || (rbtnServiceNSImport.Checked == true))
            {

                decimal vsubTotal = 0;
                decimal vInpCnf = 0;
                decimal vInpInsurance = 0;
                decimal vInpAV = 0;
                decimal vAVAmount = 0;
                decimal vInpCD = 0;
                decimal vInpRD = 0;
                decimal vInpTVB = 0;
                decimal vInpSD = 0;
                decimal vInpVAT = 0;
                decimal vInpTVA = 0;
                decimal vInpATV = 0;
                decimal vInpOthers = 0;
                decimal vInpAIT = 0;

                if (!string.IsNullOrEmpty(txtTotalSubTotal1.Text.Trim()))
                    vsubTotal = Convert.ToDecimal(txtTotalSubTotal1.Text.Trim());
                else
                {
                    MessageBox.Show("There is no data in purchase details, Please input data first", this.Text);
                    return;
                }
                //vsubTotal = 1;
                if (vsubTotal == 0)
                {
                    MessageBox.Show("There is no data in purchase details, Please input data first", this.Text);
                    return;
                }
                #region InputValue

                if (!string.IsNullOrEmpty(txtCnFInpValue.Text.Trim()))
                    vInpCnf = Convert.ToDecimal(txtCnFInpValue.Text.Trim());
                else
                    vInpCnf = 0;

                if (!string.IsNullOrEmpty(txtInsInpValue.Text.Trim()))
                    vInpInsurance = Convert.ToDecimal(txtInsInpValue.Text.Trim());
                else
                    vInpInsurance = 0;

                if (!string.IsNullOrEmpty(txtAVInpValue.Text.Trim()))
                    vInpAV = Convert.ToDecimal(txtAVInpValue.Text.Trim());
                else
                    vInpAV = 0;

                if (!string.IsNullOrEmpty(txtCDInpValue.Text.Trim()))
                    vInpCD = Convert.ToDecimal(txtCDInpValue.Text.Trim());
                else
                    vInpCD = 0;



                if (!string.IsNullOrEmpty(txtRDInpValue.Text.Trim()))
                    vInpRD = Convert.ToDecimal(txtRDInpValue.Text.Trim());
                else
                    vInpRD = 0;

                if (!string.IsNullOrEmpty(txtTVBInpValue.Text.Trim()))
                    vInpTVB = Convert.ToDecimal(txtTVBInpValue.Text.Trim());
                else
                    vInpTVB = 0;

                if (!string.IsNullOrEmpty(txtSDInpValue.Text.Trim()))
                    vInpSD = Convert.ToDecimal(txtSDInpValue.Text.Trim());
                else
                    vInpSD = 0;

                if (!string.IsNullOrEmpty(txtVATInpValue.Text.Trim()))
                    vInpVAT = Convert.ToDecimal(txtVATInpValue.Text.Trim());
                else
                    vInpVAT = 0;

                if (!string.IsNullOrEmpty(txtTVAInpValue.Text.Trim()))
                    vInpTVA = Convert.ToDecimal(txtTVAInpValue.Text.Trim());
                else
                    vInpTVA = 0;



                if (!string.IsNullOrEmpty(txtATVInpValue.Text.Trim()))
                    vInpATV = Convert.ToDecimal(txtATVInpValue.Text.Trim());
                else
                    vInpATV = 0;

                if (!string.IsNullOrEmpty(txtOthersInpValue.Text.Trim()))
                    vInpOthers = Convert.ToDecimal(txtOthersInpValue.Text.Trim());
                else
                    vInpOthers = 0;

                if (!string.IsNullOrEmpty(txtAITInpValue.Text.Trim()))
                    vInpAIT = Convert.ToDecimal(txtAITInpValue.Text.Trim());
                else
                    vInpAIT = 0;
                #endregion InputValue

                #region FixedCnF
                if (FixedCnF == true)
                {
                    txtCnFAmount.Text = vInpCnf.ToString();
                    txtCnFRate.Text = Convert.ToString(vInpCnf * 100 / vsubTotal);
                }
                else
                {
                    txtCnFAmount.Text = Convert.ToString(vsubTotal * vInpCnf / 100);
                    txtCnFRate.Text = vInpCnf.ToString();
                }
                #endregion FixedCnF
                #region FixedInsurance

                decimal vTotalInsuranceApply = 0;
                vTotalInsuranceApply = vsubTotal + Convert.ToDecimal(txtCnFAmount.Text.Trim());

                if (FixedInsurance == true)
                {
                    txtInsAmount.Text = vInpInsurance.ToString();
                    txtInsRate.Text = Convert.ToString(vInpInsurance * 100 / vTotalInsuranceApply);
                }
                else
                {
                    txtInsAmount.Text = Convert.ToString(vTotalInsuranceApply * vInpInsurance / 100);
                    txtInsRate.Text = vInpInsurance.ToString();
                }
                #endregion FixedInsurance
                #region CalculativeAV

                if (CalculativeAV == true)
                {
                    vAVAmount = vsubTotal + Convert.ToDecimal(txtCnFAmount.Text.Trim()) +
                                       Convert.ToDecimal(txtInsAmount.Text.Trim());
                }
                else
                {
                    vAVAmount = vInpAV;
                }
                txtAVAmount.Text = Convert.ToString(vAVAmount);
                //if (vAVAmount == 0)
                //vAVAmount = 1;
                if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                    vAVAmount = Convert.ToDecimal(txtAVAmount.Text.Trim());
                else
                {
                    MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);
                    return;
                }
                //vsubTotal = 1;
                if (vAVAmount == 0)
                {
                    MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);

                    return;
                }

                #endregion CalculativeAV

                #region FixedCD
                if (FixedCD == true)
                {
                    txtCDAmount.Text = vInpCD.ToString();
                    txtCDRate.Text = Convert.ToString(vInpCD * 100 / vAVAmount);
                }
                else
                {
                    txtCDAmount.Text = Convert.ToString(vAVAmount * vInpCD / 100);
                    txtCDRate.Text = vInpCD.ToString();
                }
                #endregion FixedCD
                #region FixedRD
                if (FixedRD == true)
                {
                    txtRDAmount.Text = vInpRD.ToString();
                    txtRDRate.Text = Convert.ToString(vInpRD * 100 / vAVAmount);
                }
                else
                {
                    txtRDAmount.Text = Convert.ToString(vAVAmount * vInpRD / 100);
                    txtRDRate.Text = vInpRD.ToString();
                }
                #endregion FixedCD
                #region FixedTVB

                decimal vTotalTVBApply = 0;
                vTotalTVBApply = vAVAmount + Convert.ToDecimal(txtCDAmount.Text.Trim()) + Convert.ToDecimal(txtRDAmount.Text.Trim());

                if (FixedTVB == true)
                {
                    txtTVBAmount.Text = vInpTVB.ToString();
                    txtTVBRate.Text = Convert.ToString(vInpTVB * 100 / vTotalTVBApply);
                }
                else
                {
                    txtTVBAmount.Text = Convert.ToString(vTotalTVBApply * vInpTVB / 100);
                    txtTVBRate.Text = vInpTVB.ToString();
                }

                #endregion FixedTVB
                #region FixedSD

                decimal vTotalSDApply = 0;
                vTotalSDApply = vTotalTVBApply + Convert.ToDecimal(txtTVBAmount.Text.Trim());

                if (FixedSD == true)
                {
                    txtSDAmount.Text = vInpSD.ToString();
                    txtSDRate.Text = Convert.ToString(vInpSD * 100 / vTotalSDApply);
                }
                else
                {
                    txtSDAmount.Text = Convert.ToString(vTotalSDApply * vInpSD / 100);
                    txtSDRate.Text = vInpSD.ToString();
                }

                #endregion FixedSD
                #region FixedVAT

                decimal vTotalVATApply = 0;
                vTotalVATApply = vTotalSDApply + Convert.ToDecimal(txtSDAmount.Text.Trim());

                if (FixedVATImp == true)
                {
                    txtVATAmount.Text = vInpVAT.ToString();
                    txtVATRateI.Text = Convert.ToString(vInpVAT * 100 / vTotalVATApply);
                }
                else
                {
                    txtVATAmount.Text = Convert.ToString(vTotalVATApply * vInpVAT / 100);
                    txtVATRateI.Text = vInpVAT.ToString();
                }

                #endregion FixedVAT
                #region FixedTVA
                vTotalVATApply = vTotalSDApply + Convert.ToDecimal(txtSDAmount.Text.Trim());
                if (FixedTVA == true)
                {
                    txtTVAAmount.Text = vInpTVA.ToString();
                    txtTVARate.Text = Convert.ToString(vInpTVA * 100 / vTotalVATApply);
                }
                else
                {
                    txtTVAAmount.Text = Convert.ToString(vTotalVATApply * vInpTVA / 100);
                    txtTVARate.Text = vInpTVA.ToString();
                }

                #endregion FixedTVA
                #region FixedATV
                decimal vTotalATVApply = 0;
                vTotalATVApply = vTotalVATApply + Convert.ToDecimal(txtTVAAmount.Text.Trim());

                if (FixedAT == true)
                {
                    txtATVAmount.Text = vInpATV.ToString();
                    txtATVRate.Text = Convert.ToString(vInpATV * 100 / vTotalATVApply);
                }
                else
                {
                    txtATVAmount.Text = Convert.ToString(vTotalATVApply * vInpATV / 100);
                    txtATVRate.Text = vInpATV.ToString();
                }

                #endregion FixedTVA
                #region FixedOthers
                decimal vTotalOthersApply = 0;
                vTotalOthersApply = vTotalVATApply;

                if (FixedOthers == true)
                {
                    txtOthersAmount.Text = vInpOthers.ToString();
                    txtOthersRate.Text = Convert.ToString(vInpOthers * 100 / vTotalOthersApply);
                }
                else
                {
                    txtOthersAmount.Text = Convert.ToString(vTotalOthersApply * vInpOthers / 100);
                    txtOthersRate.Text = vInpOthers.ToString();
                }

                txtAITAmount.Text = vInpAIT.ToString();

                #endregion FixedOthers
            }
            #region Else

            #endregion Else

        }

        private void txtCnFInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtInsInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtAVInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtCDInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtRDInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtTVBInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtSDInpValue_Leave(object sender, EventArgs e)
        {
            SDRateCalulate();
            //DutyCalculation();
        }

        private void txtVATInpValue_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVATInpValue, "VATInpValue");
            //DutyCalculation();
            VATRateCal();
        }

        private void SDRateCalulate()
        {
            try
            {
                //Program.FormatTextBox(txtVATInpValue, "VATInpValue");

                decimal varAssesableValue = Convert.ToDecimal(txtAVAmount.Text);
                decimal SDAmount = Convert.ToDecimal(txtSDAmount.Text);
                decimal SDRate = 0;

                if (varAssesableValue > 0)
                {
                    SDRate = (SDAmount / varAssesableValue) * 100;
                }

                txtSDRate.Text = SDRate.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void VATRateCal()
        {
            try
            {
                decimal assesableValue = Convert.ToDecimal(txtAVAmount.Text);
                decimal vatAmount = Convert.ToDecimal(txtVATAmount.Text);
                decimal cdAmount = Convert.ToDecimal(txtCDAmount.Text);
                decimal rdAmount = Convert.ToDecimal(txtRDAmount.Text);
                decimal sdAmount = Convert.ToDecimal(txtSDAmount.Text);

                decimal varVATRate = 0;
                decimal baseValue = assesableValue + cdAmount + rdAmount + sdAmount;

                if (baseValue > 0)
                {
                    varVATRate = (vatAmount / baseValue) * 100;
                }
                txtVATRateI.Text = varVATRate.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtTVAInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtATVInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtOthersInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        #endregion

        #region Methods 10

        private void btnDuty_Click(object sender, EventArgs e)
        {
            dutyCalculate = true;

            DutyAssign();
            ////Rowcalculate();
        }

        private void bgwDutiesSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                PurchaseDutyResult = new DataTable();
                PurchaseDutyResult = purchaseDal.SearchPurchaseDutyDTNew(PurchaseDetailData, connVM);  // Change 04
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

        private void bgwDutiesSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                dgvDuty.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in PurchaseDutyResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvDuty.Rows.Add(NewRow);

                    dgvDuty.Rows[j].Cells["ItemNoDuty"].Value = item2["ItemNo"].ToString();
                    dgvDuty.Rows[j].Cells["ProductCodeDuty"].Value = item2["ProductCode"].ToString();
                    dgvDuty.Rows[j].Cells["ProductNameDuty"].Value = item2["ProductName"].ToString();
                    dgvDuty.Rows[j].Cells["CnFInp"].Value = item2["CnFInp"].ToString();
                    dgvDuty.Rows[j].Cells["CnFRate"].Value = item2["CnFRate"].ToString();
                    dgvDuty.Rows[j].Cells["CnFAmount"].Value = item2["CnFAmount"].ToString();
                    dgvDuty.Rows[j].Cells["InsuranceInp"].Value = item2["InsuranceInp"].ToString();
                    dgvDuty.Rows[j].Cells["InsuranceRate"].Value = item2["InsuranceRate"].ToString();
                    dgvDuty.Rows[j].Cells["InsuranceAmount"].Value = item2["InsuranceAmount"].ToString();
                    dgvDuty.Rows[j].Cells["AssessableInp"].Value = item2["AssessableInp"].ToString();
                    dgvDuty.Rows[j].Cells["AssessableValue"].Value = item2["AssessableValue"].ToString();
                    dgvDuty.Rows[j].Cells["CDInp"].Value = item2["CDInp"].ToString();
                    dgvDuty.Rows[j].Cells["CDRate"].Value = item2["CDRate"].ToString();
                    dgvDuty.Rows[j].Cells["CDAmount"].Value = item2["CDAmount"].ToString();
                    dgvDuty.Rows[j].Cells["RDInp"].Value = item2["RDInp"].ToString();
                    dgvDuty.Rows[j].Cells["RDRate"].Value = item2["RDRate"].ToString();
                    dgvDuty.Rows[j].Cells["RDAmount"].Value = item2["RDAmount"].ToString();
                    dgvDuty.Rows[j].Cells["TVBInp"].Value = item2["TVBInp"].ToString();
                    dgvDuty.Rows[j].Cells["TVBRate"].Value = item2["TVBRate"].ToString();
                    dgvDuty.Rows[j].Cells["TVBAmount"].Value = item2["TVBAmount"].ToString();
                    dgvDuty.Rows[j].Cells["SDInp"].Value = item2["SDInp"].ToString();
                    dgvDuty.Rows[j].Cells["SDuty"].Value = item2["SD"].ToString();
                    dgvDuty.Rows[j].Cells["SDutyAmount"].Value = item2["SDAmount"].ToString();
                    dgvDuty.Rows[j].Cells["VATInp"].Value = item2["VATInp"].ToString();
                    dgvDuty.Rows[j].Cells["VATRateDuty"].Value = item2["VATRate"].ToString();
                    dgvDuty.Rows[j].Cells["VATAmountDuty"].Value = item2["VATAmount"].ToString();
                    dgvDuty.Rows[j].Cells["TVAInp"].Value = item2["TVAInp"].ToString();
                    dgvDuty.Rows[j].Cells["TVARate"].Value = item2["TVARate"].ToString();
                    dgvDuty.Rows[j].Cells["TVAAmount"].Value = item2["TVAAmount"].ToString();
                    dgvDuty.Rows[j].Cells["ATVInp"].Value = item2["ATVInp"].ToString();
                    dgvDuty.Rows[j].Cells["ATVRate"].Value = item2["ATVRate"].ToString();
                    dgvDuty.Rows[j].Cells["ATVAmount"].Value = item2["ATVAmount"].ToString();
                    dgvDuty.Rows[j].Cells["OthersInp"].Value = item2["OthersInp"].ToString();
                    dgvDuty.Rows[j].Cells["OthersRate"].Value = item2["OthersRate"].ToString();
                    dgvDuty.Rows[j].Cells["OthersAmount"].Value = item2["OthersAmount"].ToString();
                    dgvDuty.Rows[j].Cells["AITInp"].Value = item2["AITInp"].ToString();
                    dgvDuty.Rows[j].Cells["AITAmount"].Value = item2["AITAmount"].ToString();


                    dgvDuty.Rows[j].Cells["Remarks"].Value = item2["Remarks"].ToString();
                    j = j + 1;
                }

                #region Flag Update

                IsUpdate = true;
                ChangeData = false;

                #endregion

                #region Row Calculate

                Rowcalculate();

                #endregion

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
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnPrePurSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            PurchaseDAL _purchaseDAL = new PurchaseDAL();

            try
            {
                if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked)
                {
                    //cmbProduct.Enabled = false;
                    rbtnCode.Enabled = false;
                    rbtnProduct.Enabled = false;
                    ////btnAdd.Enabled = false;
                    button1.Visible = false;

                }
                if (!rbtnPurchaseReturn.Checked)
                {
                    dgvReceiveHistory.Top = dgvPurchase.Top;
                    dgvReceiveHistory.Left = dgvPurchase.Left;
                    dgvReceiveHistory.Height = dgvPurchase.Height;
                    dgvReceiveHistory.Width = dgvPurchase.Width;

                    dgvPurchase.Visible = false;
                    dgvReceiveHistory.Visible = true;

                    this.btnPrePurSearch.Enabled = false;
                    this.progressBar1.Visible = true;

                }

                Program.fromOpen = "Me";

                transactionTypes();


                DataGridViewRow selectedRow = FormPurchaseSearch.SelectOne("All");

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    ////if (selectedRow.Cells["Post1"].Value.ToString() == "N")
                    ////{
                    ////    MessageBox.Show("this transaction was not posted ", this.Text);
                    ////    return;
                    ////}

                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }

                    txtPrePurId.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();

                    transactionType = selectedRow.Cells["TransactionType"].Value.ToString();

                    if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                    {
                        ReturnTransType = transactionType;
                    }

                    txtCategoryName.Text = "Raw";
                    //string PurchaseDetailData = string.Empty;
                    if (txtPrePurId.Text == "")
                    {
                        PurchaseDetailData = "00";
                    }
                    else
                    {
                        PurchaseDetailData = txtPrePurId.Text.Trim();
                    }
                    if (rbtnPurchaseReturn.Checked)
                    {
                        #region Data Call

                        string[] cValues = { txtPrePurId.Text.Trim(), transactionType };
                        string[] cFields = { "pih.PurchaseInvoiceNo", "pih.TransactionType" };
                        dt = _purchaseDAL.SelectAll(0, cFields, cValues, null, null, null, true, connVM, false);

                        #endregion

                        #region Value Assing to Form Elements

                        DataRow dr = dt.Rows[0];

                        txtFiscalYear.Text = dr["FiscalYear"].ToString();
                        SearchBranchId = Convert.ToInt32(dr["BranchId"]);
                        txtTDSAmount.Text = Program.ParseDecimalObject(dr["TDSAmount"].ToString());
                        txtVendorID.Text = dr["VendorID"].ToString();
                        txtVendorName.Text = dr["VendorName"].ToString();
                        cmbVendor.Text = dr["VendorName"].ToString();
                        txtVendorGroupName.Text = dr["VendorGroupName"].ToString();
                        dtpInvoiceDate.Value = Convert.ToDateTime(dr["InvoiceDateTime"].ToString());
                        txtTotalAmount.Text = Program.ParseDecimalObject(dr["TotalAmount"].ToString());
                        txtTotalVATAmount.Text = Program.ParseDecimalObject(dr["TotalVATAmount"].ToString());
                        txtSerialNo.Text = dr["SerialNo"].ToString();
                        txtLCNumber.Text = dr["LCNumber"].ToString();
                        txtComments.Text = dr["Comments"].ToString();
                        IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                        txtBENumber.Text = dr["BENumber"].ToString();
                        cmbVDS.Text = dr["WithVDS"].ToString();
                        txtCustomHouse.Text = dr["CustomHouse"].ToString();
                        txtCustomHouseCode.Text = dr["CustomCode"].ToString();
                        txtUSDInvoiceValue.Text = dr["USDInvoiceValue"].ToString();
                        chkTDS.Checked = dr["IsTDS"] != null && dr["IsTDS"].ToString() == "Y";
                        dtpReceiveDate.Value = Convert.ToDateTime(dr["ReceiveDate"].ToString());
                        dtpRebateDate.Value = Convert.ToDateTime(dr["RebateDate"].ToString());
                        chkIsRebate.Checked = dr["IsRebate"].ToString() == "Y";
                        txtBankGuarantee.Text = dr["BankGuarantee"].ToString();

                        #region Conditional Values

                        if (dr["LCNumber"].ToString() != "-")
                        {
                            if (!string.IsNullOrEmpty(dr["LCDate"].ToString()))
                            {
                                dtpLCDate.Value = Convert.ToDateTime(dr["LCDate"].ToString());
                            }
                        }

                        #endregion

                        txtLandedCost.Text = Program.ParseDecimalObject(dr["LandedCost"].ToString());

                        #endregion


                        bgwSearchPurchase.RunWorkerAsync();

                    }
                    else
                    {
                        txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                        cmbVendor.Text = selectedRow.Cells["VendorName"].Value.ToString();
                        txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                        ////////transactionType = selectedRow.Cells["TransType"].Value.ToString();

                        bgwPrePurSearch.RunWorkerAsync();

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
            finally
            {
                this.btnPrePurSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void bgwPrePurSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProductResultDs = new DataTable();
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                //ProductResultDs = purchaseDal.SearchPurchaseDetailDTNew(PurchaseDetailData, transactionType);  // Change 04

                string[] cVals = new string[] { PurchaseDetailData, transactionType };
                string[] cFields = new string[] { "pd.PurchaseInvoiceNo like", "pd.TransactionType like" };
                ProductResultDs = purchaseDal.SelectPurchaseDetail(null, cFields, cVals, null, null, true, connVM);
                //ProductDAL productDal = new ProductDAL();
                //ProductResultDs = new DataTable();
                //ProductResultDs = productDal.SearchProductMiniDSDispose(PurchaseDetailData);

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

        private void bgwPrePurSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                #region Statement

                // Start Complete

                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                dgvReceiveHistory.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceiveHistory.Rows.Add(NewRow);
                    dgvReceiveHistory.Rows[j].Cells["VDSAmountHistory"].Value = item2["VDSAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VDSRateHistory"].Value = item2["VDSRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["BOMIdHistory"].Value = item2["BOMId"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["USDValueHistory"].Value = item2["USDValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["USDVATHistory"].Value = item2["USDVAT"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["FixedVATAmountHistory"].Value = item2["FixedVATAmount"].ToString();

                    dgvReceiveHistory.Rows[j].Cells["TDSCodeHistory"].Value = item2["TDSCode"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TDSSectionHistory"].Value = item2["TDSSection"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["IsFixedVATHistory"].Value = item2["IsFixedVAT"].ToString();


                    dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = item2["POLineNo"].ToString();//= PurchaseDetailFields[1].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNoP"].Value = item2["ItemNo"].ToString();// PurchaseDetailFields[2].ToString();
                    dgvReceiveHistory.Rows[j].Cells["QuantityP"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UnitPriceP"].Value = item2["CostPrice"].ToString();// Convert.ToDecimal(PurchaseDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["NBRPriceP"].Value = item2["NBRPrice"].ToString();// Convert.ToDecimal(PurchaseDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["UOMP"].Value = item2["UOM"].ToString();// PurchaseDetailFields[6].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATRateP"].Value = item2["VATRate"].ToString();// Convert.ToDecimal(PurchaseDetailFields[7].ToString()).ToString();//"0.00");
                    dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = item2["VATAmount"].ToString();// Convert.ToDecimal(PurchaseDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = item2["SubTotal"].ToString();// Convert.ToDecimal(PurchaseDetailFields[9].ToString()).ToString();//"0,00.00");
                    dgvReceiveHistory.Rows[j].Cells["CommentsP"].Value = item2["Comments"].ToString();// PurchaseDetailFields[10].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNameP"].Value = item2["ProductName"].ToString();// PurchaseDetailFields[11].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StatusP"].Value = "Old";// PurchaseDetailFields[12].ToString();
                    // comment by ruba
                    //dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["StockP"].Value = item2["Stock"].ToString();// Convert.ToDecimal(PurchaseDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["ChangeP"].Value = 0;
                    dgvReceiveHistory.Rows[j].Cells["SDP"].Value = item2["SD"].ToString();// Convert.ToDecimal(PurchaseDetailFields[13].ToString()).ToString();//"0.00");
                    dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = item2["SDAmount"].ToString();// Convert.ToDecimal(PurchaseDetailFields[14].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["TypeP"].Value = item2["Type"].ToString();// PurchaseDetailFields[15].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PCodeP"].Value = item2["ProductCode"].ToString();// PurchaseDetailFields[16].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMQtyP"].Value = item2["UOMQty"].ToString();// Convert.ToDecimal(PurchaseDetailFields[17].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UOMnP"].Value = item2["UOMn"].ToString();// PurchaseDetailFields[18].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMcP"].Value = item2["UOMc"].ToString();// Convert.ToDecimal(PurchaseDetailFields[19].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UOMPriceP"].Value = item2["UOMPrice"].ToString();//Convert.ToDecimal(PurchaseDetailFields[20].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["TotalP"].Value = item2["SubTotal"].ToString();//
                    dgvReceiveHistory.Rows[j].Cells["VDSP"].Value = "NA";//
                    dgvReceiveHistory.Rows[j].Cells["RebateP"].Value = item2["RebateRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["RebateAmountP"].Value = item2["RebateAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CnFP"].Value = item2["CnFAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["InsuranceP"].Value = item2["InsuranceAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["AVP"].Value = item2["AssessableValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["RDP"].Value = item2["RDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TVBP"].Value = item2["TVBAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TVAP"].Value = item2["TVAAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ATVP"].Value = item2["ATVAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CDP"].Value = item2["CDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["OthersP"].Value = item2["OthersAmount"].ToString();
                    decimal returnQty = purchaseDal.ReturnQty(PurchaseDetailData, item2["ItemNo"].ToString(), connVM);
                    dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value = (Convert.ToDecimal(item2["Quantity"]) - returnQty).ToString();
                    dgvReceiveHistory.Rows[j].Cells["RestQtyP"].Value = (Convert.ToDecimal(item2["Quantity"]) - returnQty).ToString();

                    j = j + 1;
                }
                //btnSave.Text = "&Save";
                Rowcalculate();
                IsUpdate = true;
                ChangeData = false;

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
                ChangeData = false;
                this.btnPrePurSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
            /////---------------////
            #region Previous code for Load
            //try
            //{
            //    //Start Complete
            //    ProductsMini.Clear();

            //    if (ProductResultDs == null)
            //    {
            //        MessageBox.Show("there is no data");
            //        return;
            //    }
            //    cmbProductCode.Items.Clear();
            //    cmbProduct.Items.Clear();
            //    foreach (DataRow item2 in ProductResultDs.Rows)
            //    {
            //        var prod = new ProductMiniDTO();
            //        prod.ItemNo = item2["ItemNo"].ToString();
            //        prod.ProductName = item2["ProductName"].ToString();

            //        cmbProductCode.Items.Add(item2["ProductCode"]);
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
            //        prod.PurchaseQty = Convert.ToDecimal(item2["PurchaseQty"].ToString());

            //        ProductsMini.Add(prod);
            //    }
            //    //ProductSearchDsFormLoad();
            //    //End Complete
            //}
            //#region catch

            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", exMessage);
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
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
            //    FileLogger.Log(this.Name, "bgwPrePurSearch_RunWorkerCompleted", exMessage);
            //}
            //#endregion

            //this.btnPrePurSearch.Enabled = true;
            //this.progressBar1.Visible = false;
            #endregion Previous code for Load

        }

        private void chkImport_CheckedChanged(object sender, EventArgs e)
        {
            if (chkImport.Checked)
            {
                chkImport.Text = "Import";
                dgvPurchase.Columns["CnF"].Visible = true;
                dgvPurchase.Columns["Insurance"].Visible = true;
                dgvPurchase.Columns["AV"].Visible = true;
                dgvPurchase.Columns["CD"].Visible = true;
                dgvPurchase.Columns["RD"].Visible = true;
                ////dgvPurchase.Columns["TVB"].Visible = true;
                ////dgvPurchase.Columns["TVA"].Visible = true;
                dgvPurchase.Columns["ATV"].Visible = true;
                dgvPurchase.Columns["Others"].Visible = true;
                tcPurchase.TabPages.Add(tabPage4);
                cmbType1.Text = "Import";
                txtCustomHouse.Visible = true;
                label59.Visible = true;
                label33.Visible = true;


                if (rbtnTrading.Checked)
                    rbtnTradingImport.Checked = true;
                else if (rbtnInputService.Checked)
                    rbtnInputServiceImport.Checked = true;
                else if (rbtnService.Checked)
                    rbtnServiceImport.Checked = true;
                else if (rbtnServiceNS.Checked)
                    rbtnServiceNSImport.Checked = true;


            }
            else
            {
                chkImport.Text = "Local";

                tcPurchase.TabPages.Remove(tabPage4);
                dgvPurchase.Columns["CnF"].Visible = false;
                dgvPurchase.Columns["Insurance"].Visible = false;
                dgvPurchase.Columns["AV"].Visible = false;
                dgvPurchase.Columns["CD"].Visible = false;
                dgvPurchase.Columns["RD"].Visible = false;
                dgvPurchase.Columns["TVB"].Visible = false;
                dgvPurchase.Columns["TVA"].Visible = false;
                dgvPurchase.Columns["ATV"].Visible = false;
                dgvPurchase.Columns["Others"].Visible = false;
                cmbType1.Text = "Local";
                txtCustomHouse.Visible = false;
                label59.Visible = false;
                label33.Visible = false;

                if (rbtnTradingImport.Checked)
                    rbtnTrading.Checked = true;
                else if (rbtnInputServiceImport.Checked)
                    rbtnInputService.Checked = true;

                else if (rbtnServiceImport.Checked)
                    rbtnService.Checked = true;
                else if (rbtnServiceNSImport.Checked)
                    rbtnServiceNS.Checked = true;
            }

            vendorloadToCombo();

        }

        private void txtAVInpValue_TextChanged(object sender, EventArgs e)
        {
            txtAVAmount.Text = txtAVInpValue.Text;
            ChangeData = true;
        }

        private void chkVendorCode_CheckedChanged(object sender, EventArgs e)
        {

            vendorloadToCombo();
        }

        private void txtVATInpValue_TextChanged(object sender, EventArgs e)
        {
            txtVATAmount.Text = txtVATInpValue.Text;
            ChangeData = true;
        }

        private void ChkSubTotalAll_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvPurchase.Rows.Count > 0)
            {



                if (ChkSubTotalAll.Checked == false)
                {
                    txtTotalSubTotal1.Text = txtTotalSubTotal2.Text.Trim();
                }
                else
                {
                    txtTotalSubTotal1.Text = dgvPurchase.CurrentRow.Cells["SubTotal"].Value.ToString();


                }
            }
            if (ChkSubTotalAll.Checked == false)
            {
                ChkSubTotalAll.Text = "All";
            }
            else
            {
                ChkSubTotalAll.Text = "Single";


            }
        }

        private void txtCnFAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void dtpReceiveDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtSerialNo.Focus();
            }
        }

        #endregion

        #region Methods 11

        private void cmbType1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtBENumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter))
            //{
            //    SendKeys.Send("{TAB}");
            //}
        }

        private void dtpReceiveDate_ValueChanged(object sender, EventArgs e)
        {


            ChangeData = true;

        }

        private void txtBENumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbVDS_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
            var vdsFlag = cmbVDS.Text;
            chkIsHouseRent.Checked = vdsFlag == "Y";

            VATCalculation();
        }

        private void txtRebatePercent_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtTotalSubTotal1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCnFInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtInsInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAVInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCDInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtRDInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtTVBInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtSDInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        #endregion

        #region Methods 12

        private void txtVATInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtTVAInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtATVInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtOthersInpValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtDutiesRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
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
                //MDIMainInterface mdi = new MDIMainInterface();
                FormRptVATKa frmRptVATKa = new FormRptVATKa();


                if (dgvPurchase.Rows.Count > 0)
                {
                    frmRptVATKa.txtItemNo.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVATKa.txtProductName.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVATKa.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVATKa.dtpToDate.Value = dtpReceiveDate.Value;

                    ProductVM vProductVM = new ProductVM();
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    vProductVM = productDal.SelectAll(frmRptVATKa.txtItemNo.Text, null, null, null, null, null, connVM).FirstOrDefault();

                    if (vProductVM.ProductType != "Trading")
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void txtTotalSubTotal1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
            txtAVInpValue.Text = txtTotalSubTotal1.Text.Trim();
            txtAVAmount.Text = txtTotalSubTotal1.Text.Trim();

        }

        private void txtInsInpValue_TextChanged(object sender, EventArgs e)
        {
            txtInsAmount.Text = txtInsInpValue.Text;
            ChangeData = true;
        }

        private void txtCDInpValue_TextChanged(object sender, EventArgs e)
        {
            txtCDAmount.Text = txtCDInpValue.Text;
            ChangeData = true;
        }

        private void txtRDInpValue_TextChanged(object sender, EventArgs e)
        {
            txtRDAmount.Text = txtRDInpValue.Text;

            ChangeData = true;
        }

        private void txtTVBInpValue_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSDInpValue_TextChanged(object sender, EventArgs e)
        {
            txtSDAmount.Text = txtSDInpValue.Text;
            ChangeData = true;
        }

        private void txtTVAInpValue_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtATVInpValue_TextChanged(object sender, EventArgs e)
        {
            txtATVAmount.Text = txtATVInpValue.Text;
            ChangeData = true;
        }

        private void txtOthersInpValue_TextChanged(object sender, EventArgs e)
        {
            txtOthersAmount.Text = txtOthersInpValue.Text;
            ChangeData = true;
        }

        #endregion

        #region Methods 13

        private void txtDutiesRemarks_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCnFRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtInsRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCDRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtRDRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTVBRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSDRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATRateI_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTVARate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtATVRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtOthersRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtInsAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtAVAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCDAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtRDAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 14

        private void txtTVBAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSDAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTVAAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtATVAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtOthersAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void tcPurchase_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tcPurchase.SelectedTab == tabPageSerial)
            //{
            if (dgvPurchase.Rows.Count > 0)
            {
                productCmb.Clear();
                cmbPNameS.Items.Clear();
                cmbPCodeS.Items.Clear();


                for (int i = 0; i < dgvPurchase.Rows.Count; i++)
                {
                    var prodcmb = new ProductCmbDTO();

                    prodcmb.ItemNo = dgvPurchase.Rows[i].Cells["ItemNo"].Value.ToString();

                    prodcmb.ProductCode = dgvPurchase.Rows[i].Cells["PCode"].Value.ToString();
                    cmbPCodeS.Items.Add(dgvPurchase.Rows[i].Cells["PCode"].Value.ToString());

                    prodcmb.ProductName = dgvPurchase.Rows[i].Cells["ItemName"].Value.ToString();
                    cmbPNameS.Items.Add(dgvPurchase.Rows[i].Cells["ItemName"].Value.ToString());
                    prodcmb.Quantity = dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString();
                    prodcmb.Value = dgvPurchase.Rows[i].Cells["UnitPrice"].Value.ToString();



                    productCmb.Add(prodcmb);
                }

            }
            //}
        }

        private void bgwPurchasePrevious_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                PurchaseDAL purchaseDal = new PurchaseDAL();
                //PurchaseDetailResult = purchaseDal.SearchPurchaseDetailDTNew(PurchaseDetailData);  // Change 04
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

        private void bgwPurchasePrevious_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                dgvReceiveHistory.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in PurchaseDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceiveHistory.Rows.Add(NewRow);
                    dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = item2["POLineNo"].ToString();//= PurchaseDetailFields[1].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNoP"].Value = item2["ItemNo"].ToString();// PurchaseDetailFields[2].ToString();
                    dgvReceiveHistory.Rows[j].Cells["QuantityP"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UnitPriceP"].Value = item2["CostPrice"].ToString();// Convert.ToDecimal(PurchaseDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["NBRPriceP"].Value = item2["NBRPrice"].ToString();// Convert.ToDecimal(PurchaseDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["UOMP"].Value = item2["UOM"].ToString();// PurchaseDetailFields[6].ToString();
                    dgvReceiveHistory.Rows[j].Cells["VATRateP"].Value = item2["VATRate"].ToString();// Convert.ToDecimal(PurchaseDetailFields[7].ToString()).ToString();//"0.00");
                    dgvReceiveHistory.Rows[j].Cells["VATAmountP"].Value = item2["VATAmount"].ToString();// Convert.ToDecimal(PurchaseDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["SubTotalP"].Value = item2["SubTotal"].ToString();// Convert.ToDecimal(PurchaseDetailFields[9].ToString()).ToString();//"0,00.00");
                    dgvReceiveHistory.Rows[j].Cells["CommentsP"].Value = item2["Comments"].ToString();// PurchaseDetailFields[10].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ItemNameP"].Value = item2["ProductName"].ToString();// PurchaseDetailFields[11].ToString();
                    dgvReceiveHistory.Rows[j].Cells["StatusP"].Value = "Old";// PurchaseDetailFields[12].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PreviousP"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["StockP"].Value = item2["Stock"].ToString();// Convert.ToDecimal(PurchaseDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["ChangeP"].Value = 0;
                    dgvReceiveHistory.Rows[j].Cells["SDP"].Value = item2["SD"].ToString();// Convert.ToDecimal(PurchaseDetailFields[13].ToString()).ToString();//"0.00");
                    dgvReceiveHistory.Rows[j].Cells["SDAmountP"].Value = item2["SDAmount"].ToString();// Convert.ToDecimal(PurchaseDetailFields[14].ToString()).ToString();//"0,0.00");
                    dgvReceiveHistory.Rows[j].Cells["TypeP"].Value = item2["Type"].ToString();// PurchaseDetailFields[15].ToString();
                    dgvReceiveHistory.Rows[j].Cells["PCodeP"].Value = item2["ProductCode"].ToString();// PurchaseDetailFields[16].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMQtyP"].Value = item2["UOMQty"].ToString();// Convert.ToDecimal(PurchaseDetailFields[17].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UOMnP"].Value = item2["UOMn"].ToString();// PurchaseDetailFields[18].ToString();
                    dgvReceiveHistory.Rows[j].Cells["UOMcP"].Value = item2["UOMc"].ToString();// Convert.ToDecimal(PurchaseDetailFields[19].ToString()).ToString();//"0,0.0000");
                    dgvReceiveHistory.Rows[j].Cells["UOMPriceP"].Value = item2["UOMPrice"].ToString();//Convert.ToDecimal(PurchaseDetailFields[20].ToString()).ToString();//"0,0.00");

                    dgvReceiveHistory.Rows[j].Cells["RebateP"].Value = item2["RebateRate"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["RebateAmountP"].Value = item2["RebateAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CnFP"].Value = item2["CnFAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["InsuranceP"].Value = item2["InsuranceAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["AVP"].Value = item2["AssessableValue"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CDP"].Value = item2["CDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["RDP"].Value = item2["RDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TVBP"].Value = item2["TVBAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["TVAP"].Value = item2["TVAAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["ATVP"].Value = item2["ATVAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["CDP"].Value = item2["CDAmount"].ToString();
                    dgvReceiveHistory.Rows[j].Cells["OthersP"].Value = item2["OthersAmount"].ToString();


                    j = j + 1;
                }
                //btnSave.Text = "&Save";
                Rowcalculate();
                IsUpdate = true;
                ChangeData = false;

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
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
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
            if (e.KeyCode.Equals(Keys.F2))
            {
                dgvPurchase.Rows.Clear();
                int j = 0;
                for (int i = 0; i < dgvReceiveHistory.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value) == true)
                    {
                        if (Convert.ToDecimal(dgvReceiveHistory["RestQtyP", i].Value.ToString()) == 0)
                        {
                            MessageBox.Show("There is no quantity to return for Item Code: " + dgvReceiveHistory["PCodeP", i].Value.ToString(),
                                 this.Text, MessageBoxButtons.OK);
                            goto Outer;
                        }
                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvPurchase.Rows.Add(NewRow);

                        dgvPurchase.Rows[j].Cells["LineNo"].Value = j + 1;

                        dgvPurchase.Rows[j].Cells["VDSAmount"].Value = dgvReceiveHistory["VDSAmountHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["VDSRate"].Value = dgvReceiveHistory["VDSRateHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["BOMId"].Value = dgvReceiveHistory["BOMIdHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["USDValue"].Value = dgvReceiveHistory["USDValueHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["USDVAT"].Value = dgvReceiveHistory["USDVATHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["FixedVATAmount"].Value = dgvReceiveHistory["FixedVATAmountHistory", i].Value.ToString();

                        dgvPurchase.Rows[j].Cells["TDSCode"].Value = dgvReceiveHistory["TDSCodeHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["TDSSection"].Value = dgvReceiveHistory["TDSSectionHistory", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["IsFixedVAT"].Value = dgvReceiveHistory["IsFixedVATHistory", i].Value.ToString();


                        dgvPurchase.Rows[j].Cells["PCode"].Value = dgvReceiveHistory["PCodeP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["ItemNo"].Value = dgvReceiveHistory["ItemNoP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["ItemName"].Value = dgvReceiveHistory["ItemNameP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UOM"].Value = dgvReceiveHistory["UOMP", i].Value.ToString();
                        //dgvPurchase.Rows[j].Cells["Quantity"].Value = dgvReceiveHistory["QuantityP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Quantity"].Value = dgvReceiveHistory["RestQtyP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UnitPrice"].Value = dgvReceiveHistory["UnitPriceP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["SubTotal"].Value = dgvReceiveHistory["SubTotalP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Rebate"].Value = dgvReceiveHistory["RebateP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["RebateAmount"].Value = dgvReceiveHistory["RebateAmountP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["CnF"].Value = dgvReceiveHistory["CnFP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Insurance"].Value = dgvReceiveHistory["InsuranceP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["AV"].Value = dgvReceiveHistory["AVP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["CD"].Value = dgvReceiveHistory["CDP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["RD"].Value = dgvReceiveHistory["RDP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["TVB"].Value = dgvReceiveHistory["TVBP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["SD"].Value = dgvReceiveHistory["SDP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["SDAmount"].Value = dgvReceiveHistory["SDAmountP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["VATRate"].Value = dgvReceiveHistory["VATRateP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["VATAmount"].Value = dgvReceiveHistory["VATAmountP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["TVA"].Value = dgvReceiveHistory["TVAP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["ATV"].Value = dgvReceiveHistory["ATVP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Others"].Value = dgvReceiveHistory["OthersP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Total"].Value = dgvReceiveHistory["TotalP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Comments"].Value = dgvReceiveHistory["CommentsP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Type"].Value = dgvReceiveHistory["TypeP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["NBRPrice"].Value = dgvReceiveHistory["NBRPriceP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Status"].Value = "Old";// dgvReceiveHistory["StatusP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Stock"].Value = dgvReceiveHistory["StockP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Previous"].Value = dgvReceiveHistory["PreviousP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["Change"].Value = dgvReceiveHistory["ChangeP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UOMPrice"].Value = dgvReceiveHistory["UOMPriceP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UOMQty"].Value = dgvReceiveHistory["UOMQtyP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UOMn"].Value = dgvReceiveHistory["UOMnP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["UOMc"].Value = dgvReceiveHistory["UOMcP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["VDS"].Value = dgvReceiveHistory["VDSP", i].Value.ToString();
                        dgvPurchase.Rows[j].Cells["RestQty"].Value = dgvReceiveHistory["RestQtyP", i].Value.ToString();
                        if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                        {
                            dgvPurchase.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                        }


                        j = j + 1;

                    }
                Outer:
                    continue;



                }
                dgvReceiveHistory.Visible = false;
                dgvPurchase.Visible = true;
                Rowcalculate();
            }
        }

        private void rbtnPurchaseReturn_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtRebatePercent_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtRebatePercent, "Rebate Percent");
        }

        private void txtTotalSubTotal_TextChanged(object sender, EventArgs e)
        {
            if (ChkSubTotalAll.Checked == false)
            {
                txtTotalSubTotal1.Text = txtTotalSubTotal2.Text.Trim();
            }
        }

        private void txtTotalSubTotal2_TextChanged(object sender, EventArgs e)
        {
            if (ChkSubTotalAll.Checked == false)
            {
                txtTotalSubTotal1.Text = txtTotalSubTotal2.Text.Trim();
            }
        }

        #endregion

        #region Methods 15

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tcPurchase_Click(object sender, EventArgs e)
        {
            //DutyCalculation();
        }
        public IPurchase GetObject()
        {
            if (Program.IsWCF.ToLower() == "y")
            {
                return new PurchaseRepo();
            }
            else
            {
                return new PurchaseDAL();

            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {


            transactionTypes();



            string value = new CommonDAL().settingValue("CompanyCode", "Code", connVM);

            if (OrdinaryVATDesktop.IsACICompany(value))
            {
                //FormPurchaseImportACI aci = new FormPurchaseImportACI();
                //aci.preSelectTable = "Purchases";
                //aci.transactionType = transactionType;
                //aci.Show();
                string ImportID = FormPurchaseImportACI.SelectOne(transactionType, false);

                IPurchase _plDal = GetObject();

                if (!string.IsNullOrEmpty(ImportID))
                {
                    string[] cValues = { ImportID };
                    string[] cFields = { "pih.ImportIDExcel" };
                    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                    SetHeader(dt);
                }

            }

            else if (value.ToUpper() == "NESTLE")
            {

                string ImportID = FormPurchaseImportNESTLE.SelectOne(transactionType, false);

                IPurchase _plDal = GetObject();

                if (!string.IsNullOrEmpty(ImportID))
                {
                    string[] cValues = { ImportID };
                    string[] cFields = { "pih.ImportIDExcel" };
                    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                    SetHeader(dt);
                }

            }


            else if (value == "SQR")
            {
                try
                {
                    //"IMP-001/0520/0048";
                    string refNo = FormPurchaseImportSQR.SelectOne(transactionType);
                    //PurchaseDetailData = refNo;
                    PurchaseDAL purchaseDal = new PurchaseDAL();
                    DataTable dtHeader = purchaseDal.SelectAllFromTemp(refNo, new[] { "pih.UserId" }, new[] { Program.CurrentUserID }, null, null, null, false, connVM);

                    SetFormHeader(dtHeader);

                }
                catch (Exception ex)
                {
                    FileLogger.Log("SQR Purchase", "SQR Purchase", ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            else if (value == "EON" || value.ToLower() == "purofood" || value.ToLower() == "eahpl" || value.ToLower() == "eail" || value.ToLower() == "eeufl" || value.ToLower() == "exfl")
            {
                string ImportId = FormPurchaseImportEON.SelectOne(transactionType);
                PurchaseDAL _plDal = new PurchaseDAL();

                if (!string.IsNullOrEmpty(ImportId))
                {
                    string[] cValues = { ImportId };
                    string[] cFields = { "pih.ImportIDExcel" };
                    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                    SetHeader(dt);
                }
            }
            else if (value == "SMC" || value.ToLower() == "smcholding")
            {
                string ImportId = FormPurchaseImportSMC.SelectOne(transactionType);
                PurchaseDAL _plDal = new PurchaseDAL();

                if (!string.IsNullOrEmpty(ImportId))
                {
                    string[] cValues = { ImportId };
                    string[] cFields = { "pih.ImportIDExcel" };
                    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                    SetHeader(dt);
                }
            }
            else if (value.ToLower() == "dbh")
            {
                string ImportId = FormPurchaseImportDBH.SelectOne(transactionType);
                //PurchaseDAL _plDal = new PurchaseDAL();

                //if (!string.IsNullOrEmpty(ImportId))
                //{
                //    string[] cValues = { ImportId };
                //    string[] cFields = { "pih.ImportIDExcel" };
                //    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                //    SetHeader(dt);
                //}
            }
            else if (OrdinaryVATDesktop.IsNourishCompany(value))
            {
                string ImportId = FormPurchaseImportNourish.SelectOne(transactionType);
            }

            ////else if (value.ToLower() == "decathlon")
            ////{
            ////    string ImportId = FormPurchaseImportDecathlon.SelectOne(transactionType);
            ////    PurchaseDAL _plDal = new PurchaseDAL();

            ////    if (!string.IsNullOrEmpty(ImportId))
            ////    {
            ////        string[] cValues = { ImportId };
            ////        string[] cFields = { "pih.ImportIDExcel" };
            ////        DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
            ////        SetHeader(dt);
            ////    }
            ////}
            else if (value.ToLower() == "bsl")
            {
                string ImportID = FormPurchaseImportBSL.SelectOne(transactionType, false);

                IPurchase _plDal = GetObject();

                if (!string.IsNullOrEmpty(ImportID))
                {
                    string[] cValues = { ImportID };
                    string[] cFields = { "pih.ImportIDExcel" };
                    DataTable dt = _plDal.SelectAll(0, cFields, cValues, null, null, null, false, connVM);
                    SetHeader(dt);
                }
            }

            else
            {
                FormMasterImport fmi = new FormMasterImport();
                //fmi.isFileSelected = true;
                fmi.preSelectTable = "Purchases";
                fmi.transactionType = transactionType;
                fmi.Show(this);
            }



            #region old method

            //string[] extention = fileName.Split(".".ToCharArray());
            //string[] retResults = new string[4];
            //if (extention[extention.Length - 1] == "txt")
            //{
            //    retResults = ImportFromText();
            //}
            //else
            //{
            //    retResults = ImportFromExcel();
            //}

            ////string[] sqlResults = Import();
            //string result = retResults[0];
            //string message = retResults[1];
            //if (string.IsNullOrEmpty(result))
            //{
            //    throw new ArgumentNullException("Purchase Ipmort",
            //                                    "Unexpected error.");
            //}
            //else if (result == "Success" || result == "Fail")
            //{
            //    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

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
            try
            {
                transactionTypes();
                //DateTime vdateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));
                string vdateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

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

                OleDbDataAdapter adPurchaseM = new OleDbDataAdapter("SELECT * FROM [PurchaseM$]", theConnection);
                DataTable dtPurchaseM = new DataTable();
                adPurchaseM.Fill(dtPurchaseM);

                OleDbDataAdapter adapterDetail = new OleDbDataAdapter("SELECT * FROM [PurchaseD$]", theConnection);
                DataTable dtPurchaseD = new DataTable();
                adapterDetail.Fill(dtPurchaseD);

                OleDbDataAdapter adapterImport = new OleDbDataAdapter("SELECT * FROM [PurchaseI$]", theConnection);
                DataTable dtPurchaseI = new DataTable();
                adapterImport.Fill(dtPurchaseI);

                theConnection.Close();
                #endregion Load Excel

                #region RowCount
                int rowCount = 0;
                int MRow = dtPurchaseM.Rows.Count;
                for (int i = 0; i < dtPurchaseM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtPurchaseM.Rows[i]["ID"].ToString()))
                    {
                        rowCount++;
                    }

                }
                if (MRow != rowCount)
                {
                    MessageBox.Show("you have select " + MRow.ToString() + " data for import, but you have " + rowCount + " id.");
                    return sqlResults;
                }



                #endregion RowCount

                #region ID in Master or Detail table

                for (int i = 0; i < rowCount; i++)
                {
                    string importID = dtPurchaseM.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtPurchaseD.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }

                #endregion

                #region Double ID in Master

                for (int i = 0; i < rowCount; i++)
                {
                    string id = dtPurchaseM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtPurchaseM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        MessageBox.Show("you have duplicate master id " + id + " in import file.");
                        return sqlResults;

                    }

                }


                #endregion Double ID in Master

                #region Process model

                CommonImportDAL cImportD = new CommonImportDAL();

                //CommonImportDAL cImport = new CommonImportDAL();
                ICommonImport cImport = OrdinaryVATDesktop.GetObject<CommonImportDAL, CommonImportRepo, ICommonImport>(OrdinaryVATDesktop.IsWCF);

                #region checking from database is exist the information(NULL Check)
                #region Master
                string CurrencyId = string.Empty;
                string USDCurrencyId = string.Empty;
                string PrePurchaseId = string.Empty;

                for (int i = 0; i < rowCount; i++)
                {
                    CurrencyId = string.Empty;
                    USDCurrencyId = string.Empty;

                    #region FindVendorId
                    cImport.FindVendorId(dtPurchaseM.Rows[i]["Vendor_Name"].ToString().Trim(),
                                           dtPurchaseM.Rows[i]["Vendor_Code"].ToString().Trim(), null, null, false, connVM);
                    #endregion FindVendorId

                    #region Check Previous Purchase no.
                    PrePurchaseId = cImport.CheckPrePurchaseNo(dtPurchaseM.Rows[i]["Previous_Purchase_No"].ToString().Trim(), null, null, connVM);
                    #endregion Check Previous Purchase no.

                    #region FindCurrencyId
                    //CurrencyId = cImport.FindCurrencyId(dtPurchaseM.Rows[i]["Currency_Code"].ToString().Trim());
                    //USDCurrencyId = cImport.FindCurrencyId("USD");
                    //cImport.FindCurrencyRateFromBDT(CurrencyId);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId);

                    #endregion FindCurrencyId

                    #region Check Date

                    bool IsInvoiceDate;
                    bool IsReceiveDate;
                    IsInvoiceDate = cImportD.CheckDate(dtPurchaseM.Rows[i]["Invoice_Date"].ToString().Trim());
                    if (IsInvoiceDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date field.");
                    }
                    IsReceiveDate = cImportD.CheckDate(dtPurchaseM.Rows[i]["Receive_Date"].ToString().Trim());
                    if (IsReceiveDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
                    }

                    #endregion Check Date

                    #region Yes no check

                    bool post;
                    bool withVDS;

                    post = cImportD.CheckYN(dtPurchaseM.Rows[i]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    withVDS = cImportD.CheckYN(dtPurchaseM.Rows[i]["With_VDS"].ToString().Trim());
                    if (withVDS != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in With_VDSe field.");
                    }

                    #endregion Check Date

                }
                #endregion Master

                #region Details

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow[] DetailRaws = dtPurchaseD.Select("ID='" + dtPurchaseM.Rows[i]["ID"].ToString().Trim() + "'");

                    for (int j = 0; j < DetailRaws.Length; j++)
                    {
                        string ItemNo = string.Empty;
                        string UOMn = string.Empty;
                        bool IsQuantity, IsTotalPrice, IsRebateRate, IsSDAmount, IsVatAmount;

                        #region FindItemId
                        ItemNo = cImport.FindItemId(dtPurchaseD.Rows[j]["Item_Name"].ToString().Trim()
                                     , dtPurchaseD.Rows[j]["Item_Code"].ToString().Trim(), null, null, false, "-", 1, 0, 0, connVM);
                        #endregion FindItemId

                        #region FindUOMn
                        UOMn = cImport.FindUOMn(ItemNo, null, null, connVM);
                        #endregion FindUOMn
                        #region FindUOMc
                        cImport.FindUOMc(UOMn, dtPurchaseD.Rows[j]["UOM"].ToString().Trim(), null, null, connVM);
                        #endregion FindUOMc

                        #region Numeric value check
                        IsQuantity = cImportD.CheckNumericBool(dtPurchaseD.Rows[j]["Quantity"].ToString().Trim());
                        if (IsQuantity != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                        }
                        IsTotalPrice = cImportD.CheckNumericBool(dtPurchaseD.Rows[j]["Total_Price"].ToString().Trim());
                        if (IsTotalPrice != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Total_Price field.");
                        }
                        IsRebateRate = cImportD.CheckNumericBool(dtPurchaseD.Rows[j]["Rebate_Rate"].ToString().Trim());
                        if (IsRebateRate != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Rebate_Rate field.");
                        }
                        IsSDAmount = cImportD.CheckNumericBool(dtPurchaseD.Rows[j]["SD_Amount"].ToString().Trim());
                        if (IsSDAmount != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in SD_Amount field.");
                        }
                        IsVatAmount = cImportD.CheckNumericBool(dtPurchaseD.Rows[j]["VAT_Amount"].ToString().Trim());
                        if (IsVatAmount != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in VAT_Amount field.");
                        }

                        #endregion
                    }


                }

                #endregion Details

                #region Duties
                #endregion Duties

                #endregion checking from database is exist the information(NULL Check)

                purchaseMaster = new PurchaseMasterVM();
                purchaseDuties = new List<PurchaseDutiesVM>();
                purchaseDetails = new List<PurchaseDetailVM>();

                for (int i = 0; i < rowCount; i++)
                {
                    #region Block

                    #region Master Purchase


                    string importID = dtPurchaseM.Rows[i]["ID"].ToString().Trim();
                    string vendorName = dtPurchaseM.Rows[i]["Vendor_Name"].ToString().Trim();
                    string vendorCode = dtPurchaseM.Rows[i]["Vendor_Code"].ToString().Trim();
                    #region FindVendorId
                    string vendorId = cImport.FindVendorId(vendorName, vendorCode, null, null, false, connVM);
                    #endregion FindVendorId
                    string invoiceDate = dtPurchaseM.Rows[i]["Invoice_Date"].ToString().Trim();

                    #region CheckNull
                    string referanceNo = cImportD.ChecKNullValue(dtPurchaseM.Rows[i]["Referance_No"].ToString().Trim());
                    string bENumber = cImportD.ChecKNullValue(dtPurchaseM.Rows[i]["BE_Number"].ToString().Trim());
                    #endregion CheckNull

                    string receiveDate = dtPurchaseM.Rows[i]["Receive_Date"].ToString().Trim();
                    string post = dtPurchaseM.Rows[i]["Post"].ToString().Trim();
                    string withVDS = dtPurchaseM.Rows[i]["With_VDS"].ToString().Trim();

                    #region Check Previous Purchase no.
                    string previousPurchaseNo = cImport.CheckPrePurchaseNo(dtPurchaseM.Rows[i]["Previous_Purchase_No"].ToString().Trim(), null, null, connVM);
                    #endregion Check Previous Purchase no.
                    #region CheckNull
                    string comments = dtPurchaseM.Rows[i]["Comments"].ToString().Trim();
                    #endregion CheckNull

                    #region Master
                    purchaseMaster = new PurchaseMasterVM();
                    purchaseMaster.VendorID = vendorId;
                    purchaseMaster.InvoiceDate = Convert.ToDateTime(invoiceDate).ToString("yyyy-MMM-dd") + Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                    purchaseMaster.SerialNo = referanceNo.Replace(" ", "");
                    purchaseMaster.Comments = comments;
                    purchaseMaster.CreatedBy = Program.CurrentUser;
                    purchaseMaster.CreatedOn = vdateTime;
                    purchaseMaster.LastModifiedBy = Program.CurrentUser;
                    purchaseMaster.LastModifiedOn = vdateTime;
                    purchaseMaster.BENumber = bENumber;
                    purchaseMaster.TransactionType = transactionType;
                    purchaseMaster.ReceiveDate = Convert.ToDateTime(receiveDate).ToString("yyyy-MMM-dd") + Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                    purchaseMaster.Post = post;
                    purchaseMaster.ReturnId = previousPurchaseNo;

                    purchaseMaster.ProductType = "NA";
                    purchaseMaster.WithVDS = withVDS;
                    purchaseMaster.ImportID = importID;

                    //    purchaseMaster.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                    //purchaseMaster.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());

                    #endregion Master




                    #region fitemno
                    DataRow[] DetailRaws;//= new DataRow[];//
                    if (!string.IsNullOrEmpty(importID))
                    {

                        DetailRaws =
                           dtPurchaseD.Select("ID='" + importID + "'");

                    }
                    else
                    {
                        DetailRaws = null;
                    }


                    #endregion fitemno

                    #endregion Master Purchase

                    #region Details Purchase

                    int Dcounter = 1;

                    foreach (DataRow row in DetailRaws)
                    {
                        string itemCode = row["Item_Code"].ToString().Trim();
                        string itemName = row["Item_Name"].ToString().Trim();
                        string itemNo = cImport.FindItemId(itemName, itemCode, null, null, false, "-", 1, 0, 0, connVM);

                        string quantity = row["Quantity"].ToString().Trim();
                        string totalPrice = row["Total_Price"].ToString().Trim();
                        string uOM = row["UOM"].ToString().Trim();
                        string uOMn = cImport.FindUOMn(itemNo, null, null, connVM);
                        string uOMc = cImport.FindUOMc(uOMn, uOM, null, null, connVM);

                        string type = row["Type"].ToString().Trim();
                        string rebateRate = row["Rebate_Rate"].ToString().Trim();
                        //string cnFAmount = row["CnF_Amount"].ToString().Trim();
                        //string insuranceAmount = row["Insurance_Amount"].ToString().Trim();
                        //string assessableValue = row["Assessable_Value"].ToString().Trim();
                        //string cDAmount = row["CD_Amount"].ToString().Trim();
                        //string rDAmount = row["RD_Amount"].ToString().Trim();

                        string sDAmount = row["SD_Amount"].ToString().Trim();
                        //string sDAmount = row["SD_Amount"].ToString().Trim();
                        //string tVBAmount = row["TVB_Amount"].ToString().Trim();
                        string vATAmount = row["VAT_Amount"].ToString().Trim();

                        #region details
                        purchaseDetails = new List<PurchaseDetailVM>();

                        PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();

                        purchaseDetailVm.LineNo = Dcounter.ToString(); ;
                        purchaseDetailVm.ItemNo = itemNo.ToString();
                        purchaseDetailVm.Quantity = Convert.ToDecimal(quantity.ToString());

                        purchaseDetailVm.UOM = uOM.ToString();
                        purchaseDetailVm.Comments = "NA";
                        purchaseDetailVm.BENumber = bENumber.ToString();
                        purchaseDetailVm.Type = type.ToString();
                        purchaseDetailVm.ProductType = "NA";
                        purchaseDetailVm.UOMn = uOMn.ToString();
                        purchaseDetailVm.UOMc = Convert.ToDecimal(uOMc.ToString());
                        purchaseDetailVm.SubTotal = Convert.ToDecimal(totalPrice);
                        decimal unitPrice = Convert.ToDecimal(Convert.ToDecimal(totalPrice.ToString()) / Convert.ToDecimal(quantity.ToString()));
                        purchaseDetailVm.UnitPrice = unitPrice;
                        purchaseDetailVm.NBRPrice = unitPrice;
                        purchaseDetailVm.UOMPrice = Convert.ToDecimal(unitPrice) / Convert.ToDecimal(uOMc);
                        purchaseDetailVm.UOMQty = Convert.ToDecimal(quantity) * Convert.ToDecimal(uOMc);
                        purchaseDetailVm.VATAmount = Convert.ToDecimal(vATAmount);
                        purchaseDetailVm.SDAmount = Convert.ToDecimal(sDAmount);
                        purchaseDetailVm.RebateRate = Convert.ToDecimal(rebateRate.ToString());
                        purchaseDetailVm.RebateAmount =
                            Convert.ToDecimal(Convert.ToDecimal(vATAmount) * Convert.ToDecimal(rebateRate.ToString()) / 100);

                        purchaseDetailVm.VATRate = Convert.ToDecimal(Convert.ToDecimal(vATAmount) * 100 / Convert.ToDecimal(totalPrice));
                        purchaseDetailVm.SD = Convert.ToDecimal(Convert.ToDecimal(sDAmount) * 100 / Convert.ToDecimal(totalPrice));

                        purchaseDetailVm.CnFAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.InsuranceAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.AssessableValue = Convert.ToDecimal(0);
                        purchaseDetailVm.CDAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.RDAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.TVBAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.TVAAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.ATVAmount = Convert.ToDecimal(0);
                        purchaseDetailVm.OthersAmount = Convert.ToDecimal(0);

                        purchaseDetails.Add(purchaseDetailVm);
                        #endregion details
                        Dcounter++;

                    }

                    SAVE_DOWORK_SUCCESS = false;
                    sqlResults = new string[4];

                    //PurchaseDAL purchaseDal = new PurchaseDAL();
                    IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                    sqlResults = purchaseDal.PurchaseInsert(purchaseMaster, purchaseDetails, purchaseDuties, purchaseTrackings, null, null, 0, connVM, Program.CurrentUserID);
                    SAVE_DOWORK_SUCCESS = true;

                    #endregion Details Purchase

                    #endregion Block
                }

                return sqlResults;

                //this.btnSave.Enabled = false;
                //this.progressBar1.Visible = true;

                #endregion Process model

            }

            //bgwSave.RunWorkerAsync();
            //retResults[0] = "Success";
            //retResults[1] = "Price declaration import successful";

            #endregion try

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

            finally { }
            return sqlResults;
        }

        public string[] ImportFromText()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";

            transactionTypes();
            #endregion Close1

            string files = Program.ImportFileName;
            if (string.IsNullOrEmpty(files))
            {
                MessageBox.Show("Please select the right file for import");
                return sqlResults;
            }

            DataTable dtPurchaseM = new DataTable();
            DataTable dtPurchaseD = new DataTable();
            DataTable dtPurchaseI = new DataTable();
            DataTable dtPurchaseT = new DataTable();


            #region Master table

            dtPurchaseM.Columns.Add("Identifier");
            dtPurchaseM.Columns.Add("ID");
            dtPurchaseM.Columns.Add("Vendor_Name");
            dtPurchaseM.Columns.Add("Vendor_Code");
            dtPurchaseM.Columns.Add("Referance_No");
            dtPurchaseM.Columns.Add("BE_Number");
            dtPurchaseM.Columns.Add("Invoice_Date");
            dtPurchaseM.Columns.Add("Receive_Date");
            dtPurchaseM.Columns.Add("Post");
            dtPurchaseM.Columns.Add("With_VDS");
            dtPurchaseM.Columns.Add("Previous_Purchase_No");
            dtPurchaseM.Columns.Add("Comments");
            dtPurchaseM.Columns.Add("Transection_Type").DefaultValue = transactionType;
            dtPurchaseM.Columns.Add("LCDate");
            dtPurchaseM.Columns.Add("LandedCost");
            dtPurchaseM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
            dtPurchaseM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

            #endregion Master table

            #region Details table
            dtPurchaseD.Columns.Add("Identifier");
            dtPurchaseD.Columns.Add("ID");
            dtPurchaseD.Columns.Add("Item_Code");
            dtPurchaseD.Columns.Add("Item_Name");
            dtPurchaseD.Columns.Add("Quantity");
            dtPurchaseD.Columns.Add("Total_Price");
            dtPurchaseD.Columns.Add("UOM");
            dtPurchaseD.Columns.Add("Type");
            dtPurchaseD.Columns.Add("Rebate_Rate");
            dtPurchaseD.Columns.Add("SD_Amount");
            dtPurchaseD.Columns.Add("VAT_Amount");
            #endregion Details table


            //StreamReader sr = new StreamReader(files);
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
                                dtPurchaseM.Rows.Add(mItems);
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
                                dtPurchaseD.Rows.Add(dItems);
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
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
                //        dtPurchaseM.Rows.Add(items);
                //    }
                //    else if (items[0].Replace("\n", "").ToUpper() == "D")
                //    {
                //        dtPurchaseD.Rows.Add(items);
                //    }
                //}

                //if (sr != null)
                // {
                //     sr.Close();
                // }

                #endregion Load Text file

                SAVE_DOWORK_SUCCESS = false;
                //PurchaseDAL puchaseDal = new PurchaseDAL();
                IPurchase puchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                sqlResults = puchaseDal.ImportData(dtPurchaseM, dtPurchaseD, dtPurchaseI, dtPurchaseT, 0, null, null, null, connVM, Program.CurrentUserID);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally

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
                if (sr != null)
                {
                    sr.Close();
                }
            }
            #endregion catch & finally

            return sqlResults;
        }

        private void txtSerialNo1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSerialNo1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            { SendKeys.Send("{TAB}"); }

        }

        private void btnVAT12_Click(object sender, EventArgs e)
        {

            ReportVAT11Ds();



        }

        private void ReportVAT11Ds()
        {

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string post1, post2 = string.Empty;
            if (IsPost == true)
            {
                post1 = "Y";
                post2 = "Y";
            }
            else
            {
                PreviewOnly = true;
                post1 = "y";
                post2 = "N";
            }
            //if (PreviewOnly == true)
            //{
            //    post1 = "y";
            //    post2 = "N";
            //}
            //else
            //{
            //    post1 = "Y";
            //    post2 = "Y";
            //}
            //ReportDSDAL showReport = new ReportDSDAL();
            IReport showReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);


            ReportResultDebitNote = new DataSet();
            if (rbtnPurchaseReturn.Checked)
            {
                ReportResultDebitNote = showReport.PurchaseReturnNew(txtPurchaseInvoiceNo.Text.Trim(), post1, post2, connVM);
            }

            #region
            if (rbtnPurchaseReturn.Checked)
            {

                ReportResultDebitNote.Tables[0].TableName = "DsDebitNote";
            }
            string prefix = "";
            ReportClass objrpt = new ReportClass();


            #endregion


            #region DN
            if (rbtnPurchaseReturn.Checked)
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
            FormReport reports = new FormReport();
            reports.crystalReportViewer1.Refresh();

            if (PreviewOnly == true)
            {
                objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
            }

            reports.setReportSource(objrpt);
            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
            reports.Show();
            #endregion DN
        }

        private void btnEAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                if (dgvPurchase.Rows.Count > 0)
                {
                    if (cmbPCodeS.Text == null || cmbPCodeS.Text == "")
                    {
                        MessageBox.Show("Please select Item .");
                        return;
                    }

                    if (CountQuantity())
                    {
                        MessageBox.Show("Added all quantity.");
                        return;
                    }


                    dgvSerialTrack.Rows.Add();

                    dgvSerialTrack["ProductCodeS", dgvSerialTrack.Rows.Count - 1].Value = cmbPCodeS.Text;
                    dgvSerialTrack["ProductNameS", dgvSerialTrack.Rows.Count - 1].Value = cmbPNameS.Text;
                    dgvSerialTrack["ItemNoS", dgvSerialTrack.Rows.Count - 1].Value = txtItemNoS.Text;
                    dgvSerialTrack["Heading1", dgvSerialTrack.Rows.Count - 1].Value = "-";
                    dgvSerialTrack["Heading2", dgvSerialTrack.Rows.Count - 1].Value = "-";
                    dgvSerialTrack["QuantityS", dgvSerialTrack.Rows.Count - 1].Value = "1";
                    dgvSerialTrack["StatusS", dgvSerialTrack.Rows.Count - 1].Value = "Y";


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

        private bool CountQuantity()
        {
            decimal TotalQty = Convert.ToDecimal(txtTotalQtyS.Text);
            string ItemNo = txtItemNoS.Text.Trim();
            decimal qty = 0;
            bool result = false;

            try
            {
                for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                {
                    if (dgvSerialTrack["ItemNoS", i].Value.ToString().Trim() == ItemNo)
                    {
                        qty = qty + 1;
                        if (qty == TotalQty)
                        {
                            result = true;
                        }
                    }
                }
                //foreach (DataGrRow item in dgvSerialTrack.Rows)
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

            return result;
        }

        private void btnERemove_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (dgvSerialTrack.Rows.Count <= 0)
                {
                    return;
                }
                dgvSerialTrack.Rows.Remove(dgvSerialTrack.Rows[dgvSerialTrack.CurrentRow.Index]);


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

        private void cmbPCodeS_Leave(object sender, EventArgs e)
        {
            try
            {
                var searchText = cmbPCodeS.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in productCmb.ToList()
                                     where prd.ProductCode.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        cmbPNameS.Text = products.ProductName;
                        txtItemNoS.Text = products.ItemNo;
                        cmbPCodeS.Text = products.ProductCode;
                        txtTotalQtyS.Text = products.Quantity;
                        txtUnitPriceS.Text = products.Value;

                        txtHeading1.Text = "";
                        txtHeading2.Text = "";

                        if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                        {
                            SearchTrackingItems(txtPrePurId.Text, txtItemNoS.Text);
                        }
                        else
                        {
                            SearchTrackingItems(txtPurchaseInvoiceNo.Text, txtItemNoS.Text);
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void SearchTrackingItems(string invoiceNo, string itemNo)
        {
            try
            {
                purchaseTrackings.Clear();
                DataTable TrackingItems = new DataTable();
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);
                TrackingItems = purchaseDal.SearchPurchaseInvoiceTracking(invoiceNo, itemNo, connVM);

                #region Statement
                //int rowsCount = dgvSerialTrack.Rows.Count;
                //for (int i = 0; i < rowsCount; i++)
                //{
                //    if (dgvSerialTrack["ItemNoS", i].Value.ToString() == itemNo)
                //    {
                //        dgvSerialTrack.Rows.RemoveAt(i);
                //    }

                //}
                if (rbtnPurchaseCN.Checked || rbtnPurchaseDN.Checked || rbtnPurchaseReturn.Checked)
                {
                    dgvSerialTrack.Columns["SelectT"].Visible = true;
                }

                foreach (DataRow item2 in TrackingItems.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSerialTrack.Rows.Add(NewRow);
                    dgvSerialTrack["LineNoS", dgvSerialTrack.RowCount - 1].Value = dgvSerialTrack.RowCount;
                    dgvSerialTrack["ProductCodeS", dgvSerialTrack.RowCount - 1].Value = item2["ProductCode"].ToString();
                    dgvSerialTrack["ProductNameS", dgvSerialTrack.RowCount - 1].Value = item2["ProductName"].ToString();
                    dgvSerialTrack["ItemNoS", dgvSerialTrack.RowCount - 1].Value = item2["ItemNo"].ToString();
                    dgvSerialTrack["Heading1", dgvSerialTrack.RowCount - 1].Value = item2["Heading1"].ToString();
                    dgvSerialTrack["Heading2", dgvSerialTrack.RowCount - 1].Value = item2["Heading2"].ToString();
                    dgvSerialTrack["QuantityS", dgvSerialTrack.RowCount - 1].Value = item2["Quantity"].ToString();
                    dgvSerialTrack["StatusS", dgvSerialTrack.RowCount - 1].Value = item2["IsPurchase"].ToString();
                    dgvSerialTrack["Value", dgvSerialTrack.RowCount - 1].Value = item2["UnitPrice"].ToString();

                    if (item2["ReturnPurchase"].ToString() == "Y")
                    {
                        dgvSerialTrack["SelectT", dgvSerialTrack.RowCount - 1].Value = true;
                    }
                }

                IsUpdate = true;
                ChangeData = false;

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
                ChangeData = false;
            }
        }

        #endregion

        #region Methods 16

        private void cmbPNameS_Leave(object sender, EventArgs e)
        {
            try
            {
                var searchText = cmbPNameS.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var prodByCode = from prd in productCmb.ToList()
                                     where prd.ProductName.ToLower() == searchText.ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        cmbPNameS.Text = products.ProductName;
                        txtItemNoS.Text = products.ItemNo;
                        cmbPCodeS.Text = products.ProductCode;

                        SearchTrackingItems(txtPurchaseInvoiceNo.Text, txtItemNoS.Text);
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void rBtnCodeT_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnCodeT.Checked)
            {
                cmbPNameS.Enabled = false;
                cmbPCodeS.Enabled = true;

            }
            else if (rBtnNameT.Checked)
            {
                cmbPNameS.Enabled = true;
                cmbPCodeS.Enabled = false;

            }
        }

        private void dgvSerialTrack_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //for (int i = 0; i < dgvSerialTrack.RowCount; i++)
            //{
            //    dgvSerialTrack[0, i].Value = i + 1;
            //}
        }

        private void btnAddS_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                #region Statement
                if (dgvPurchase.Rows.Count > 0)
                {
                    if (cmbPCodeS.Text == null || cmbPCodeS.Text == "")
                    {
                        MessageBox.Show("Please select Item .");
                        return;
                    }

                    if (CountQuantity())
                    {
                        MessageBox.Show("Added all quantity.");
                        return;
                    }
                    decimal totalQty = 0;
                    totalQty = Convert.ToDecimal(txtTotalQtyS.Text) - Convert.ToDecimal(dgvSerialTrack.RowCount);

                    for (int i = 0; i < dgvSerialTrack.RowCount; i++)
                    {
                        if (dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString().ToLower() == txtHeading1.Text.Trim().ToLower())
                        {
                            MessageBox.Show("Same Tracking No. already exist.", this.Text);
                            txtHeading1.Focus();
                            return;
                        }
                        if (dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString().ToLower() == txtHeading2.Text.Trim().ToLower())
                        {
                            MessageBox.Show("Same Tracking No. already exist.", this.Text);
                            txtHeading2.Focus();
                            return;
                        }
                    }

                    if (chkHeading1.Checked == false && chkHeading2.Checked == false)
                    {
                        dgvSerialTrack.Rows.Add();
                        dgvSerialTrack["LineNoS", dgvSerialTrack.Rows.Count - 1].Value = dgvSerialTrack.Rows.Count;
                        dgvSerialTrack["ProductCodeS", dgvSerialTrack.Rows.Count - 1].Value = cmbPCodeS.Text;
                        dgvSerialTrack["ProductNameS", dgvSerialTrack.Rows.Count - 1].Value = cmbPNameS.Text;
                        dgvSerialTrack["ItemNoS", dgvSerialTrack.Rows.Count - 1].Value = txtItemNoS.Text;
                        dgvSerialTrack["Heading1", dgvSerialTrack.Rows.Count - 1].Value = txtHeading1.Text;
                        dgvSerialTrack["Heading2", dgvSerialTrack.Rows.Count - 1].Value = txtHeading2.Text;
                        dgvSerialTrack["QuantityS", dgvSerialTrack.Rows.Count - 1].Value = "1";
                        dgvSerialTrack["Value", dgvSerialTrack.Rows.Count - 1].Value = txtUnitPriceS.Text;
                        dgvSerialTrack["StatusS", dgvSerialTrack.Rows.Count - 1].Value = "Y";

                    }
                    else
                    {
                        for (int i = 0; i < totalQty; i++)
                        {
                            dgvSerialTrack.Rows.Add();

                            //dgvSerialTrack[0, dgvSerialTrack.Rows.Count - 1].Value = i + 1;
                            dgvSerialTrack["LineNoS", dgvSerialTrack.Rows.Count - 1].Value = dgvSerialTrack.Rows.Count;
                            dgvSerialTrack["ProductCodeS", dgvSerialTrack.Rows.Count - 1].Value = cmbPCodeS.Text;
                            dgvSerialTrack["ProductNameS", dgvSerialTrack.Rows.Count - 1].Value = cmbPNameS.Text;
                            dgvSerialTrack["ItemNoS", dgvSerialTrack.Rows.Count - 1].Value = txtItemNoS.Text;
                            if (chkHeading1.Checked)
                            {
                                string[] partsOfID = new string[2];
                                partsOfID = GenerateTrackingIDs(txtHeading1.Text.Trim());
                                string noPart = partsOfID[1];
                                decimal regenratePart = Convert.ToDecimal(noPart) + i;
                                string newNoPart = regenratePart.ToString();
                                if (noPart.Length > regenratePart.ToString().Length)
                                {
                                    decimal oCount = (noPart.Length) - (regenratePart.ToString().Length);

                                    for (int count = 0; count < oCount; count++)
                                    {
                                        newNoPart = "0" + newNoPart;
                                    }
                                }
                                var heading1 = partsOfID[0] + newNoPart;
                                dgvSerialTrack.Rows[dgvSerialTrack.Rows.Count - 1].Cells["Heading1"].Value = heading1.ToString();
                            }
                            else
                            {
                                dgvSerialTrack["Heading1", dgvSerialTrack.Rows.Count - 1].Value = txtHeading1.Text;
                            }
                            if (chkHeading2.Checked)
                            {
                                string[] partsOfID = new string[2];
                                partsOfID = GenerateTrackingIDs(txtHeading2.Text.Trim());
                                string noPart = partsOfID[1];
                                decimal regenratePart = Convert.ToDecimal(noPart) + i;
                                string newNoPart = regenratePart.ToString();
                                if (noPart.Length > regenratePart.ToString().Length)
                                {
                                    decimal oCount = (noPart.Length) - (regenratePart.ToString().Length);

                                    for (int count = 0; count < oCount; count++)
                                    {
                                        newNoPart = "0" + newNoPart;
                                    }
                                }
                                var heading2 = partsOfID[0] + newNoPart;
                                dgvSerialTrack.Rows[dgvSerialTrack.Rows.Count - 1].Cells["Heading2"].Value = heading2.ToString();
                            }
                            else
                            {
                                dgvSerialTrack["Heading2", dgvSerialTrack.Rows.Count - 1].Value = txtHeading2.Text;
                            }
                            dgvSerialTrack["QuantityS", dgvSerialTrack.Rows.Count - 1].Value = "1";
                            dgvSerialTrack["StatusS", dgvSerialTrack.Rows.Count - 1].Value = "Y";
                            dgvSerialTrack["Value", dgvSerialTrack.Rows.Count - 1].Value = txtUnitPriceS.Text;

                        }
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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private string[] GenerateTrackingIDs(string text)
        {
            string[] partsOfID = new string[2];
            try
            {
                int pre = 0;

                for (int i = text.Length; i <= text.Length; i--)
                {
                    if (i > 0)
                    {


                        var a = text.Substring(i - 1, 1);
                        char b = Convert.ToChar(a);

                        if (Char.IsNumber(b))
                        {
                            pre = pre + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;

                    }
                }
                string part1 = text.Substring(0, text.Length - pre);
                partsOfID[0] = part1;

                string part2 = text.Substring(text.Length - pre);

                if (Program.IsNumeric(part2))
                {
                    partsOfID[1] = part2;
                }


            }
            catch (Exception)
            {

                throw;
            }

            return partsOfID;

        }

        private void btnRemoveS_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                if (dgvSerialTrack.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvSerialTrack.CurrentRow.Cells["ProductCodeS"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Headings.Add(dgvSerialTrack.CurrentRow.Cells["Heading1"].Value.ToString());
                        dgvSerialTrack.Rows.RemoveAt(dgvSerialTrack.CurrentRow.Index);
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void btnChangeS_Click(object sender, EventArgs e)
        {
            dgvSerialTrack["ProductCodeS", dgvSerialTrack.CurrentRow.Index].Value = cmbPCodeS.Text;
            dgvSerialTrack["ProductNameS", dgvSerialTrack.CurrentRow.Index].Value = cmbPNameS.Text;
            dgvSerialTrack["ItemNoS", dgvSerialTrack.CurrentRow.Index].Value = txtItemNoS.Text;
            dgvSerialTrack["Heading1", dgvSerialTrack.CurrentRow.Index].Value = txtHeading1.Text;
            dgvSerialTrack["Heading2", dgvSerialTrack.CurrentRow.Index].Value = txtHeading2.Text;
            dgvSerialTrack["QuantityS", dgvSerialTrack.CurrentRow.Index].Value = "1";
            dgvSerialTrack["StatusS", dgvSerialTrack.CurrentRow.Index].Value = "Y";
            dgvSerialTrack["Value", dgvSerialTrack.CurrentRow.Index].Value = txtUnitPriceS.Text;

            dgvSerialTrack.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
            dgvSerialTrack.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
        }

        private void dgvSerialTrack_DoubleClick(object sender, EventArgs e)
        {
            #region Try
            try
            {
                #region Statement

                if (dgvSerialTrack.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                cmbPCodeS.Text = dgvSerialTrack.CurrentRow.Cells["ProductCodeS"].Value.ToString();
                cmbPNameS.Text = dgvSerialTrack.CurrentRow.Cells["ProductNameS"].Value.ToString();

                //txtLineNo.Text = dgvSerialTrack.CurrentCellAddress.Y.ToString();
                txtItemNoS.Text = dgvSerialTrack.CurrentRow.Cells["ItemNoS"].Value.ToString();
                txtHeading1.Text = dgvSerialTrack.CurrentRow.Cells["Heading1"].Value.ToString();
                txtHeading2.Text = dgvSerialTrack.CurrentRow.Cells["Heading2"].Value.ToString();
                txtUnitPriceS.Text = dgvSerialTrack.CurrentRow.Cells["Value"].Value.ToString();
                #endregion Statement

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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }

        private void InsertTrackingInfo(string itemNo)
        {
            #region Try
            try
            {
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                IPurchase purchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                DataTable trackingInfoDt = new DataTable();
                trackingInfoDt = purchaseDal.SearchPurchaseInvoiceTracking(PurchaseDetailData, itemNo, connVM);
                if (trackingInfoDt.Rows.Count > 0)
                {
                    for (int i = 0; i < trackingInfoDt.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = trackingInfoDt.Rows[i]["ItemNo"].ToString();
                        trackingVm.Heading1 = trackingInfoDt.Rows[i]["Heading1"].ToString();
                        trackingVm.Heading2 = trackingInfoDt.Rows[i]["Heading2"].ToString();
                        trackingVm.IsPurchase = trackingInfoDt.Rows[i]["IsPurchase"].ToString();
                        trackingVm.PurchaseInvoiceNo = trackingInfoDt.Rows[i]["PurchaseInvoiceNo"].ToString();
                        trackingVm.Quantity = Convert.ToDecimal(trackingInfoDt.Rows[i]["Quantity"].ToString());




                        purchaseTrackings.Add(trackingVm);
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
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
                FormRptVAT16 frmRptVAT16 = new FormRptVAT16();

                //mdi.RollDetailsInfo("8201");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvPurchase.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvPurchase.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvPurchase.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.txtTransType.Text = transactionType;
                }
                if (rbtnTollReceive.Checked)
                {
                    frmRptVAT16.rbtnTollRegisterRaw.Checked = true;
                    frmRptVAT16.Text = "Toll Register";
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

                StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion
        }

        private void txtVendorName_DoubleClick(object sender, EventArgs e)
        {
            VendorLoad();
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

        private void txtVendorName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVendorName_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorLoad();
            }
        }

        private void cmbVATRateInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            //vatAmnt();
        }

        #endregion

        #region Methods 17

        private void VATCalculation()
        {

            try
            {
                #region Variables

                decimal uCost = 0;
                decimal qty = 0;
                decimal sdRate = 0;
                decimal vdsRate = 0;
                decimal vatRate = 0;
                decimal TotalAmount = 0;

                decimal SDAmount = 0;
                decimal VATAmount = 0;

                #endregion

                #region Validations

                decimal UomConv = Convert.ToDecimal(txtUomConv.Text == "" ? "1" : txtUomConv.Text);
                if (UomConv == 0)
                {
                    UomConv = 1;
                }
                uCost = Convert.ToDecimal(txtUnitCost.Text == "" ? "0" : txtUnitCost.Text);
                qty = Convert.ToDecimal(txtQuantity.Text == "" ? "0" : txtQuantity.Text);
                vdsRate = Convert.ToDecimal(txtVDSRate.Text == "" ? "0" : txtVDSRate.Text);
                sdRate = Convert.ToDecimal(txtSD.Text == "" ? "0" : txtSD.Text);
                vatRate = Convert.ToDecimal(txtVATRate.Text == "" ? "0" : txtVATRate.Text);

                #endregion

                string varType = cmbType.SelectedValue.ToString();

                #region VAT and SD Calculation

                if (IsTotalPrice)
                {
                    TotalAmount = uCost;
                }
                else
                {
                    TotalAmount = qty * uCost;
                }
                if (chkIsFixedOtherSD.Checked)
                {
                    SDAmount = (qty * sdRate * UomConv);
                }
                else
                {
                    SDAmount = TotalAmount * sdRate / 100;
                }

                if (VATAmount <= 0)
                {
                    VATAmount = (TotalAmount + SDAmount) * vatRate / 100;
                }


                if (chkIsFixedOtherVAT.Checked)             ////if (varType.ToLower() == "FixedVAT".ToLower())
                {
                    VATAmount = Convert.ToDecimal(qty * vatRate * UomConv);
                }
                else
                {
                    VATAmount = (TotalAmount + SDAmount) * vatRate / 100;
                }


                #endregion

                #region VDS Calculation

                decimal VDSRate = 0;
                decimal VDSAmount = 0;


                if (chkIsHouseRent.Checked)
                {
                    if (chkIsFixedOtherVAT.Checked) ////if (varType.ToLower() == "FixedVAT".ToLower())
                    {
                        VDSAmount = VATAmount;
                    }
                    else
                    {
                        VDSRate = vatRate;
                        VDSAmount = (TotalAmount + SDAmount) * VDSRate / 100;
                    }
                }
                //else if (vatRate != 0 && vatRate != 15)
                //{
                //    VDSRate = vatRate;
                //    VDSAmount = (TotalAmount + SDAmount) * VDSRate / 100;
                //}

                else
                {
                    VDSRate = 0;
                    VDSAmount = 0;
                }

                if (varType == "UnRegister".ToLower())
                {
                    VDSRate = Convert.ToDecimal(txtVDSRate.Text);
                    VDSAmount = (TotalAmount + SDAmount) * VDSRate / 100;
                }



                #endregion VDS Calculation

                #region Set Form Value




                txtLocalVATAmount.Text = Program.ParseDecimal(VATAmount.ToString());
                txtLocalSDAmount.Text = Program.ParseDecimal(SDAmount.ToString());
                txtVDSRate.Text = Program.ParseDecimal(VDSRate.ToString());
                txtVDSAmount.Text = Program.ParseDecimal(VDSAmount.ToString());

                #endregion
            }
            catch (Exception e)
            {

            }
        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            txtCDInpValue.Text = Program.ParseDecimal(txtCDAmountP.Text);
            txtATVInpValue.Text = Program.ParseDecimal(txtATVAmountP.Text);
            txtRDInpValue.Text = Program.ParseDecimal(txtRDAmountP.Text);
            txtSDInpValue.Text = Program.ParseDecimal(txtSDAmountP.Text);
            txtVATInpValue.Text = Program.ParseDecimal(txtVATAmountP.Text);
            txtTVAInpValue.Text = Program.ParseDecimal(txtTVAAmountP.Text);
            txtAITInpValue.Text = Program.ParseDecimal(txtAITAmountP.Text);


            //txtATVInpValue.Text =Program.ParseDecimal( txtATVInput.Text);
            //txtCDInpValue.Text = Program.ParseDecimal(txtCDInput.Text);
            //txtRDInpValue.Text = Program.ParseDecimal(txtRDInput.Text);
            //txtSDInpValue.Text = Program.ParseDecimal(txtSDInput.Text);
            //txtVATInpValue.Text =Program.ParseDecimal( txtVATInput.Text);
            //txtTVAInpValue.Text =Program.ParseDecimal( txtTVAInput.Text);
            //DutyCalculation();
        }

        private void btnPreCalculation_Click(object sender, EventArgs e)
        {
            DutyPreCalculation();
        }

        private void cmbVATRateInput_Click(object sender, EventArgs e)
        {
            //vatAmnt();
        }

        private void txtVDSRate_Leave(object sender, EventArgs e)
        {
            //if (Program.CheckingNumericTextBox(txtVDSRate, "VDS Rate") == true)
            //    txtVDSRate.Text = Program.FormatingNumeric(txtVDSRate.Text.Trim(), PurchasePlaceAmt).ToString();
            txtVDSRate.Text = Program.ParseDecimalObject(txtVDSRate.Text.Trim()).ToString();


            VATCalculation();


        }

        private void txtVDSRate_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVDSAmount_Leave(object sender, EventArgs e)
        {
            txtVDSAmount.Text = Program.ParseDecimalObject(txtVDSAmount.Text.Trim()).ToString();

            //    if (Program.CheckingNumericTextBox(txtVDSAmount, "VDS Amount") == true)
            //        txtVDSAmount.Text = Program.FormatingNumeric(txtVDSAmount.Text.Trim(), PurchasePlaceAmt).ToString();
        }

        private void cmbVATRateInput_Leave(object sender, EventArgs e)
        {
            txtVATRate.Text = cmbVATRateInput.Text.Trim();

            VATCalculation();

            cmbType.SelectedValue = VATTypeCal(cmbVATRateInput.Text.Trim()).ToLower();

        }

        private void txtUSDValue_Leave(object sender, EventArgs e)
        {
            txtUSDValue.Text = Program.ParseDecimalObject(txtUSDValue.Text.Trim()).ToString();

            //if (Program.CheckingNumericTextBox(txtUSDValue, "USDValue") == true)
            //{
            //    txtUSDValue.Text = Program.FormatingNumeric(txtUSDValue.Text.Trim(), PurchasePlaceAmt).ToString();
            //}
        }

        private void txtUSDInvoiceValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void TDSCalc()
        {
            #region Get Value

            string vendorid = txtVendorID.Text.Trim();
            string invoiceNo = txtPurchaseInvoiceNo.Text.Trim();
            string receiveDate = dtpReceiveDate.Value.ToString("dd-MMM-yyyy");
            string TotalSubTotal = txtTotalSubTotal.Text.Trim();
            string TotalVatAmount = txtTotalVATAmount.Text.Trim();
            string TotalVDSAmount = txtTotalVDSAmount.Text.Trim();

            #endregion

            #region Variable

            //////decimal PreviousTotalTotal = 0;
            //////decimal CurrentAmount = 0;
            //////decimal PreviousTDSAmount = 0;
            //////decimal a = 0;
            //////decimal b = 0;
            //////decimal c = 0;
            //////decimal tds = 0;
            //////decimal tds1 = 0;
            //////decimal tds2 = 0;
            //////decimal tds3 = 0;
            //////decimal totalTds = 0;

            //////int row = 0;

            #endregion

            #region Try

            try
            {
                if (chkTDS.Checked == false)
                {
                    return;
                }


                if (!string.IsNullOrWhiteSpace(txtVendorID.Text.Trim()))
                {
                    #region Comments 01-Feb-2021

                    //DataTable dtCurrentTDSAmount = new DataTable();
                    //DataSet ds = new DataSet();
                    ////TDSsDAL vdal = new TDSsDAL();
                    //ITDSs vdal = OrdinaryVATDesktop.GetObject<TDSsDAL, TDSsRepo, ITDSs>(OrdinaryVATDesktop.IsWCF);

                    //dtCurrentTDSAmount = vdal.CurrentTDSAmount(txtPurchaseInvoiceNo.Text.Trim(), null, null, true, connVM);

                    //#region dtCurrentTDSAmount Loop

                    //foreach (DataRow citem in dtCurrentTDSAmount.Rows)
                    //{
                    //    PreviousTotalTotal = 0;
                    //    CurrentAmount = 0;
                    //    PreviousTDSAmount = 0;
                    //    a = 0;
                    //    b = 0;
                    //    c = 0;
                    //    tds = 0;
                    //    tds1 = 0;
                    //    tds2 = 0;
                    //    tds3 = 0;


                    //    ds = new DataSet();
                    //    ds = vdal.TDSAmount(citem["VendorID"].ToString().Trim(), dtpReceiveDate.Value.ToString("dd-MMM-yyyy"), citem["TDSCode"].ToString().Trim());
                    //    CurrentAmount = Convert.ToDecimal(citem["PurchaseBillAmount"]);
                    //    txtPreviousSubTotal.Text = "0";
                    //    if (ds.Tables[0].Rows.Count > 0)
                    //    {
                    //        PreviousTotalTotal = Convert.ToDecimal(ds.Tables[0].Rows[0]["PreviousSubTotal"]);
                    //    }
                    //    if (ds.Tables[1].Rows.Count > 0)
                    //    {
                    //        PreviousTDSAmount = Convert.ToDecimal(ds.Tables[1].Rows[0]["PreviousTDSAmount"]);
                    //    }
                    //    #region Comments

                    //    //PreviousTotalTotal = PreviousTotalTotal + CurrentAmount;
                    //    //CurrentAmount = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                    //    //TotalAmount = 2000000;
                    //    //CurrentAmount = 1000000;

                    //    //if (ds.Tables[1].Rows.Count <= 0)
                    //    //{
                    //    //    MessageBox.Show("This Vendor not Assign any TDS Group");
                    //    //    return;
                    //    //}
                    //    //if (CurrentAmount <= 0)
                    //    //{
                    //    //    MessageBox.Show("There have no value for TDS Calculation");
                    //    //    return;
                    //    //}

                    //    #endregion

                    //    row = 0;
                    //    foreach (DataRow item in ds.Tables[2].Rows)
                    //    {
                    //        row++;
                    //        decimal minvalue = Convert.ToDecimal(item["MinValue"]);
                    //        decimal maxvalue = Convert.ToDecimal(item["MaxValue"]);
                    //        decimal rate = Convert.ToDecimal(item["Rate"]);
                    //        decimal totalBill = PreviousTotalTotal + CurrentAmount;
                    //        if (totalBill > maxvalue)
                    //        {
                    //            //return;
                    //        }
                    //        else
                    //        {
                    //            tds = (totalBill * rate / 100);
                    //            tds = tds - PreviousTDSAmount;
                    //            break;
                    //        }
                    //        #region Comments

                    //        //if (row == 1)
                    //        //{
                    //        //    if (PreviousTotalTotal <= maxvalue)
                    //        //    {
                    //        //        decimal t1 = (maxvalue - PreviousTotalTotal);
                    //        //        if (t1 > CurrentAmount)
                    //        //        {
                    //        //            tds1 = CurrentAmount * rate / 100;
                    //        //            CurrentAmount = 0;
                    //        //            PreviousTotalTotal = maxvalue;
                    //        //        }
                    //        //        else
                    //        //        {
                    //        //            decimal t2 = CurrentAmount - t1;
                    //        //            tds1 = (CurrentAmount - t2) * rate / 100;
                    //        //            CurrentAmount = t2;
                    //        //            PreviousTotalTotal = maxvalue;
                    //        //        }
                    //        //    }
                    //        //}
                    //        //else if (row == 2)
                    //        //{
                    //        //    if (PreviousTotalTotal <= maxvalue)
                    //        //    {
                    //        //        decimal t1 = (maxvalue - PreviousTotalTotal);
                    //        //        if (t1 > CurrentAmount)
                    //        //        {
                    //        //            tds2 = CurrentAmount * rate / 100;
                    //        //            CurrentAmount = 0;
                    //        //            PreviousTotalTotal = maxvalue;
                    //        //        }
                    //        //        else
                    //        //        {
                    //        //            decimal t2 = CurrentAmount - t1;
                    //        //            tds2 = (CurrentAmount - t2) * rate / 100;
                    //        //            CurrentAmount = t2;
                    //        //            PreviousTotalTotal = maxvalue;
                    //        //        }
                    //        //    }
                    //        //}
                    //        //else if (row == 3)
                    //        //{
                    //        //    if (CurrentAmount > 0)
                    //        //    {
                    //        //        tds3 = CurrentAmount * rate / 100;
                    //        //    }

                    //        //}
                    //        #endregion

                    //    }
                    //    //tds = tds1 + tds2 + tds3;
                    //    //tds = tds - PreviousTDSAmount;
                    //    vdal.UpdatePurchaseTDSs(citem["Id"].ToString().Trim(), tds);
                    //    totalTds = totalTds + tds;
                    //}
                    //#endregion

                    //txtTDSAmount.Text = Program.FormatingNumeric(totalTds.ToString(), 3);
                    //txtNetBill.Text = Convert.ToDecimal(Convert.ToDecimal(txtTotalSubTotal.Text.Trim()) + Convert.ToDecimal(txtTotalVATAmount.Text.Trim()) - Convert.ToDecimal(txtTotalVDSAmount.Text.Trim()) - Convert.ToDecimal(txtTDSAmount.Text.Trim())).ToString();
                    //IPurchase PurchaseDal = OrdinaryVATDesktop.GetObject<PurchaseDAL, PurchaseRepo, IPurchase>(OrdinaryVATDesktop.IsWCF);

                    //PurchaseDal.UpdateTDSAmount(txtPurchaseInvoiceNo.Text.Trim(), totalTds, connVM);

                    #endregion

                    #region 01-Feb-2021

                    TDSCalcVM tdsVm = new TDSCalcVM();

                    tdsVm.VendorId = vendorid;
                    tdsVm.InvoiceNo = invoiceNo;
                    tdsVm.ReceiveDate = receiveDate;
                    if (!string.IsNullOrWhiteSpace(TotalSubTotal))
                    {
                        tdsVm.TotalSubTotal = Convert.ToDecimal(TotalSubTotal);
                    }
                    if (!string.IsNullOrWhiteSpace(TotalVatAmount))
                    {
                        tdsVm.TotalVatAmount = Convert.ToDecimal(TotalVatAmount);
                    }
                    if (!string.IsNullOrWhiteSpace(TotalVDSAmount))
                    {
                        tdsVm.TotalVDSAmount = Convert.ToDecimal(TotalVDSAmount);
                    }

                    PurchaseDAL pDal = new PurchaseDAL();

                    tdsVm = pDal.TDSCalculation(tdsVm);
                    txtPreviousSubTotal.Text = tdsVm.PreviousSubTotal.ToString();
                    txtTDSAmount.Text = Program.FormatingNumeric(tdsVm.TDSAmount.ToString(), 3);
                    txtNetBill.Text = tdsVm.NetBill.ToString();

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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTDSCalc_Click", exMessage);
            }
            #endregion

        }

        private void btnTDSCalc_Click(object sender, EventArgs e)
        {
            TDSCalc();
        }

        public void TDSPreveiw(string[] conditionFields = null, string[] conditionValues = null, string FormNumeric = "2", int BranchId = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ds = ReportDSDal.TDSAmountReport(conditionFields, conditionValues, connVM);

                ReportDocument objrpt = new ReportDocument();

                ds.Tables[0].TableName = "dtTDSAmount";


                objrpt = new RptTDSAmount();

                objrpt.SetDataSource(ds);


                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (BranchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula

                //string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt);


                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();

                reports.setReportSource(objrpt);

                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

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

        private void btnPrev_Click(object sender, EventArgs e)
        {
            TDSPreveiw();
        }

        #endregion

        #region Methods 18

        private void txtAITInpValue_Leave(object sender, EventArgs e)
        {
            //DutyCalculation();
        }

        private void txtAITInpValue_TextChanged(object sender, EventArgs e)
        {
            txtAITAmount.Text = txtAITInpValue.Text;

        }

        private void chkCD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void Lebelchange()
        {

            chkCD.Text = "CD (%)";
            if (chkCD.Checked)
            {
                chkCD.Text = "CD (F)";
            }
            chkVAT.Text = "VAT (%)";
            if (chkVAT.Checked)
            {
                chkVAT.Text = "VAT (F)";
            }
            chkAIT.Text = "AIT (%)";
            if (chkAIT.Checked)
            {
                chkAIT.Text = "AIT (F)";
            }
            chkRD.Text = "RD (%)";
            if (chkRD.Checked)
            {
                chkRD.Text = "RD (F)";
            }
            chkAT.Text = "AT (%)";
            if (chkAT.Checked)
            {
                chkAT.Text = "AT (F)";
            }

            chkSD.Text = "SD (%)";
            if (chkSD.Checked)
            {
                chkSD.Text = "SD (F)";
            }
        }

        private void chkRD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();

        }

        private void chkSD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();

        }

        private void chkVAT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();

        }

        private void chkAT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();

        }

        private void chkAIT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();

        }

        private void TxtSDRateAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtLocalSDAmount, "SDRateAmount");
            txtLocalSDAmount.Text = Program.ParseDecimalObject(txtLocalSDAmount.Text.Trim()).ToString();

        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
            txtQuantityInHand.Text = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();

        }

        private void txtPurQty_Leave(object sender, EventArgs e)
        {
            txtPurQty.Text = Program.ParseDecimalObject(txtPurQty.Text.Trim()).ToString();

            //Program.FormatTextBox(txtPurQty, "PurQty");
        }

        private void txtFixedVATAmount_Leave(object sender, EventArgs e)
        {
        }

        private void txtTDSAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTDSAmount, "TDSAmount");
            txtTDSAmount.Text = Program.ParseDecimalObject(txtTDSAmount.Text.Trim()).ToString();

        }

        private void txtTotalSubTotal_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalSubTotal, "TotalSubTotal");
            txtTotalSubTotal.Text = Program.ParseDecimalObject(txtTotalSubTotal.Text.Trim()).ToString();

        }

        #endregion

        #region Methods 19

        private void txtTotalVATAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalVATAmount, "TotalVATAmount");
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalAmount, "TotalAmount");
        }

        private void txtNetBill_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtNetBill, "NetBill");
        }

        private void txtTotalSDAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalSDAmount, "TotalSDAmount");
        }

        private void txtTotalVDSAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalVDSAmount, "TotalVDSAmount");
        }

        private void txtLandedCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtLandedCost, "LandedCost");
        }

        private void dtpReceiveDate_Leave(object sender, EventArgs e)
        {
            dtpReceiveDate.Value = Program.ParseDate(dtpReceiveDate);
        }

        private void dtpLCDate_Leave(object sender, EventArgs e)
        {
            //dtpLCDate.Value = Program.ParseDate(dtpLCDate);
        }

        private void dtpInvoiceDate_Leave(object sender, EventArgs e)
        {
            //dtpInvoiceDate.Value = Program.ParseDate(dtpInvoiceDate);
        }

        #endregion

        #region Methods 20

        private void chkIsHouseRent_CheckedChanged(object sender, EventArgs e)
        {

            //  cmbVDS.Text = chkIsHouseRent.Checked ? "Y" : "N";

        }

        private void txtCustomHouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                CustomsHouseLoad();
            }
        }

        private void CustomsHouseLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"  select ID, Code,CustomsHouseName,CustomsHouseAddress,ActiveStatus
                                    from CustomsHouse 
                                    
                                    where 1=1 and  ActiveStatus='Y'   ";
                string SQLTextRecordCount = @" select count(Code)RecordNo from CustomsHouse ";

                string[] shortColumnName = { "ID", "Code", "CustomsHouseName", "CustomsHouseAddress", "ActiveStatus" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtCustomHouse.Text = selectedRow.Cells["CustomsHouseName"].Value.ToString();
                    txtCustomHouseCode.Text = selectedRow.Cells["Code"].Value.ToString();

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

        private void txtQuantity_LocationChanged(object sender, EventArgs e)
        {

        }

        private void bgwPurchaseTemSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                PurchaseDAL purchaseDal = new PurchaseDAL();

                PurchaseDetailResult = new DataTable();

                string[] cVals = new string[] { Program.CurrentUserID };
                string[] cFields = new string[] { "pd.UserId" };
                PurchaseDetailResult = purchaseDal.SelectPurchaseDetailFromTemp(PurchaseDetailData, cFields, cVals, null, null, true, connVM);


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
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPurchaseDetailTemp_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                //PurchaseDAL purchaseDal = new PurchaseDAL();
                PurchaseDAL purchaseDal = new PurchaseDAL();

                PurchaseDetailResult = new DataTable();
                //PurchaseDetailResult = purchaseDal.SearchPurchaseDetailDTNew(PurchaseDetailData, transactionType);  // Change 04

                PurchaseDetailResult = purchaseDal.SelectPurchaseDetailFromTemp(PurchaseDetailData, new[] { "pd.UserId" }, new[] { Program.CurrentUserID }, null, null, false, connVM);
                PurchaseDutyResult = purchaseDal.SearchPurchaseDutyDTNewFromTemp(PurchaseDetailData, connVM, new[] { "PID.UserId" }, new[] { Program.CurrentUserID }, null, null);
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
                FileLogger.Log(this.Name, "bgwSearchPurchase_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPurchaseDetailTemp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                dgvPurchase.Rows.Clear();
                int j = 0;

                foreach (DataRow item2 in PurchaseDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvPurchase.Rows.Add(NewRow);
                    dgvPurchase.Rows[j].Cells["BOMId"].Value = item2["BOMId"].ToString();
                    dgvPurchase.Rows[j].Cells["LineNo"].Value = item2["POLineNo"].ToString();//= PurchaseDetailFields[1].ToString();
                    dgvPurchase.Rows[j].Cells["ItemNo"].Value = item2["ItemNo"].ToString();// PurchaseDetailFields[2].ToString();
                    dgvPurchase.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item2["Quantity"].ToString());// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item2["CostPrice"].ToString());// Convert.ToDecimal(PurchaseDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item2["NBRPrice"].ToString());// Convert.ToDecimal(PurchaseDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["UOM"].Value = item2["UOM"].ToString();// PurchaseDetailFields[6].ToString();
                    dgvPurchase.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item2["VATRate"].ToString());// Convert.ToDecimal(PurchaseDetailFields[7].ToString()).ToString();//"0.00");
                    dgvPurchase.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item2["VATAmount"].ToString());// Convert.ToDecimal(PurchaseDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item2["SubTotal"].ToString());// Convert.ToDecimal(PurchaseDetailFields[9].ToString()).ToString();//"0,00.00");
                    dgvPurchase.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// PurchaseDetailFields[10].ToString();
                    dgvPurchase.Rows[j].Cells["ItemName"].Value = item2["ProductName"].ToString();// PurchaseDetailFields[11].ToString();
                    dgvPurchase.Rows[j].Cells["Status"].Value = "Old";// PurchaseDetailFields[12].ToString();
                    dgvPurchase.Rows[j].Cells["Previous"].Value = item2["Quantity"].ToString();// Convert.ToDecimal(PurchaseDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item2["Stock"].ToString());// Convert.ToDecimal(PurchaseDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["Change"].Value = 0;
                    dgvPurchase.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item2["SD"].ToString());// Convert.ToDecimal(PurchaseDetailFields[13].ToString()).ToString();//"0.00");
                    dgvPurchase.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item2["SDAmount"].ToString());// Convert.ToDecimal(PurchaseDetailFields[14].ToString()).ToString();//"0,0.00");
                    dgvPurchase.Rows[j].Cells["Type"].Value = item2["Type"].ToString();// PurchaseDetailFields[15].ToString();
                    dgvPurchase.Rows[j].Cells["PCode"].Value = item2["ProductCode"].ToString();// PurchaseDetailFields[16].ToString();
                    dgvPurchase.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item2["Quantity"].ToString());// Convert.ToDecimal(PurchaseDetailFields[17].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UOMn"].Value = item2["UOMn"].ToString();// PurchaseDetailFields[18].ToString();
                    dgvPurchase.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item2["UOMc"].ToString());// Convert.ToDecimal(PurchaseDetailFields[19].ToString()).ToString();//"0,0.0000");
                    dgvPurchase.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item2["CostPrice"].ToString());//Convert.ToDecimal(PurchaseDetailFields[20].ToString()).ToString();//"0,0.00");

                    dgvPurchase.Rows[j].Cells["Rebate"].Value = Program.ParseDecimalObject(item2["RebateRate"].ToString());
                    dgvPurchase.Rows[j].Cells["RebateAmount"].Value = Program.ParseDecimalObject(item2["RebateAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CnF"].Value = Program.ParseDecimalObject(item2["CnFAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["Insurance"].Value = Program.ParseDecimalObject(item2["InsuranceAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["AV"].Value = Program.ParseDecimalObject(item2["AssessableValue"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["RD"].Value = Program.ParseDecimalObject(item2["RDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["TVB"].Value = Program.ParseDecimalObject(item2["TVBAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["TVA"].Value = Program.ParseDecimalObject(item2["TVAAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["ATV"].Value = Program.ParseDecimalObject(item2["ATVAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["CD"].Value = Program.ParseDecimalObject(item2["CDAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["VDSRate"].Value = Program.ParseDecimalObject(item2["VDSRate"].ToString());
                    dgvPurchase.Rows[j].Cells["VDSAmount"].Value = Program.ParseDecimalObject(item2["VDSAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["Others"].Value = Program.ParseDecimalObject(item2["OthersAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["AIT"].Value = Program.ParseDecimalObject(item2["AITAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["ReturnTransactionType"].Value = item2["ReturnTransactionType"].ToString();
                    dgvPurchase.Rows[j].Cells["USDValue"].Value = Program.ParseDecimalObject(item2["USDValue"].ToString());
                    dgvPurchase.Rows[j].Cells["USDVAT"].Value = Program.ParseDecimalObject(item2["USDVAT"].ToString());
                    dgvPurchase.Rows[j].Cells["VATableValue"].Value = Program.ParseDecimalObject(item2["VATableValue"].ToString());
                    dgvPurchase.Rows[j].Cells["TDSSection"].Value = item2["TDSSection"].ToString();
                    dgvPurchase.Rows[j].Cells["TDSCode"].Value = item2["TDSCode"].ToString();
                    dgvPurchase.Rows[j].Cells["FixedVATAmount"].Value = Program.ParseDecimalObject(item2["FixedVATAmount"].ToString());
                    dgvPurchase.Rows[j].Cells["IsFixedVAT"].Value = item2["IsFixedVAT"].ToString();



                    dgvPurchase.Rows[j].Cells["ExpireDate"].Value = "2100-01-01";
                    dgvPurchase.Rows[j].Cells["CPCName"].Value = "-";
                    dgvPurchase.Rows[j].Cells["BEItemNo"].Value = "-";
                    dgvPurchase.Rows[j].Cells["HSCode"].Value = "-";
                    dgvPurchase.Rows[j].Cells["FixedVATRebate"].Value = "Y";


                    j = j + 1;
                }

                if (
                    rbtnTradingImport.Checked
                    || rbtnImport.Checked
                    || rbtnServiceImport.Checked
                    || rbtnServiceNSImport.Checked
                    || rbtnInputServiceImport.Checked
                    || rbtnCommercialImporter.Checked
                )
                {
                    try
                    {
                        #region Statement

                        dgvDuty.Rows.Clear();
                        j = 0;

                        foreach (DataRow item2 in PurchaseDutyResult.Rows)
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvDuty.Rows.Add(NewRow);

                            dgvDuty.Rows[j].Cells["ItemNoDuty"].Value = item2["ItemNo"].ToString();
                            dgvDuty.Rows[j].Cells["ProductCodeDuty"].Value = item2["ProductCode"].ToString();
                            dgvDuty.Rows[j].Cells["ProductNameDuty"].Value = item2["ProductName"].ToString();
                            dgvDuty.Rows[j].Cells["CnFInp"].Value = item2["CnFInp"].ToString();
                            dgvDuty.Rows[j].Cells["CnFRate"].Value = item2["CnFRate"].ToString();
                            dgvDuty.Rows[j].Cells["CnFAmount"].Value = item2["CnFAmount"].ToString();
                            dgvDuty.Rows[j].Cells["InsuranceInp"].Value = item2["InsuranceInp"].ToString();
                            dgvDuty.Rows[j].Cells["InsuranceRate"].Value = item2["InsuranceRate"].ToString();
                            dgvDuty.Rows[j].Cells["InsuranceAmount"].Value = item2["InsuranceAmount"].ToString();
                            dgvDuty.Rows[j].Cells["AssessableInp"].Value = item2["AssessableInp"].ToString();
                            dgvDuty.Rows[j].Cells["AssessableValue"].Value = item2["AssessableValue"].ToString();
                            dgvDuty.Rows[j].Cells["CDInp"].Value = item2["CDInp"].ToString();
                            dgvDuty.Rows[j].Cells["CDRate"].Value = item2["CDRate"].ToString();
                            dgvDuty.Rows[j].Cells["CDAmount"].Value = item2["CDAmount"].ToString();
                            dgvDuty.Rows[j].Cells["RDInp"].Value = item2["RDInp"].ToString();
                            dgvDuty.Rows[j].Cells["RDRate"].Value = item2["RDRate"].ToString();
                            dgvDuty.Rows[j].Cells["RDAmount"].Value = item2["RDAmount"].ToString();
                            dgvDuty.Rows[j].Cells["TVBInp"].Value = item2["TVBInp"].ToString();
                            dgvDuty.Rows[j].Cells["TVBRate"].Value = item2["TVBRate"].ToString();
                            dgvDuty.Rows[j].Cells["TVBAmount"].Value = item2["TVBAmount"].ToString();
                            dgvDuty.Rows[j].Cells["SDInp"].Value = item2["SDInp"].ToString();
                            dgvDuty.Rows[j].Cells["SDuty"].Value = item2["SD"].ToString();
                            dgvDuty.Rows[j].Cells["SDutyAmount"].Value = item2["SDAmount"].ToString();
                            dgvDuty.Rows[j].Cells["VATInp"].Value = item2["VATInp"].ToString();
                            dgvDuty.Rows[j].Cells["VATRateDuty"].Value = item2["VATRate"].ToString();
                            dgvDuty.Rows[j].Cells["VATAmountDuty"].Value = item2["VATAmount"].ToString();
                            dgvDuty.Rows[j].Cells["TVAInp"].Value = item2["TVAInp"].ToString();
                            dgvDuty.Rows[j].Cells["TVARate"].Value = item2["TVARate"].ToString();
                            dgvDuty.Rows[j].Cells["TVAAmount"].Value = item2["TVAAmount"].ToString();
                            dgvDuty.Rows[j].Cells["ATVInp"].Value = item2["ATVInp"].ToString();
                            dgvDuty.Rows[j].Cells["ATVRate"].Value = item2["ATVRate"].ToString();
                            dgvDuty.Rows[j].Cells["ATVAmount"].Value = item2["ATVAmount"].ToString();
                            dgvDuty.Rows[j].Cells["OthersInp"].Value = item2["OthersInp"].ToString();
                            dgvDuty.Rows[j].Cells["OthersRate"].Value = item2["OthersRate"].ToString();
                            dgvDuty.Rows[j].Cells["OthersAmount"].Value = item2["OthersAmount"].ToString();
                            dgvDuty.Rows[j].Cells["AITInp"].Value = item2["AITInp"].ToString();
                            dgvDuty.Rows[j].Cells["AITAmount"].Value = item2["AITAmount"].ToString();


                            dgvDuty.Rows[j].Cells["Remarks"].Value = item2["Remarks"].ToString();
                            j = j + 1;
                        }

                        Rowcalculate();
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
                        FileLogger.Log(this.Name, "bgwDutiesSearch_RunWorkerCompleted", exMessage);
                    }
                    #endregion
                }
                else
                {
                    Rowcalculate();

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
                FileLogger.Log(this.Name, "bgwSearchPurchase_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnPurchaseInformation_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                MISReport _report = new MISReport();

                ReportDocument reportDocument = new ReportDocument();

                reportDocument = _report.PurchaseInformation(txtPurchaseInvoiceNo.Text.Trim(), "", "", "", "", "", "", transactionType,
                     "", "", "", "N", "-", "-", 0, 0, 0, false, "", Program.BranchId, "", "", "", "", "", connVM);


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
                FileLogger.Log(this.Name, "btnPurchaseInformation_Click", exMessage);
            }

            #endregion
        }

        #endregion

        #region Purchase Navigation

        private void txtPurchaseInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.Equals(Keys.Enter))
            {
                PurchaseNavigation("Current");
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            PurchaseNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            PurchaseNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            PurchaseNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            PurchaseNavigation("Last");
        }

        private void PurchaseNavigation(string ButtonName)
        {
            try
            {
                PurchaseDAL _PurchaseDAL = new PurchaseDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(txtId.Text);
                vm.InvoiceNo = txtPurchaseInvoiceNo.Text;


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _PurchaseDAL.Purchase_Navigation(vm, null, null, connVM);

                txtPurchaseInvoiceNo.Text = vm.InvoiceNo;
                txtHiddenInvoiceNo.Text = vm.InvoiceNo;

                if (!string.IsNullOrWhiteSpace(vm.InvoiceNo))
                {
                    SearchInvoice();
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
                FileLogger.Log(this.Name, "PurchaseNavigation", exMessage);
            }
            #endregion Catch

        }

        #endregion

        DataTable dtStock = new DataTable();

        private void bgwStock_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductDAL productDal = new ProductDAL();


                if (string.Equals(transactionType, "TollReceiveRaw", StringComparison.OrdinalIgnoreCase))
                {
                    var dtTollStock = productDal.GetTollStock(new ParameterVM()
                    {
                        ItemNo = txtItemNo.Text,
                        BranchId = Program.BranchId
                    });

                    e.Result = dtTollStock;
                }
                else
                {

                    dtStock = productDal.AvgPriceNew(txtItemNo.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                                                                     DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM, Program.CurrentUserID);

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
                FileLogger.Log(this.Name, "bgwStock_DoWork", exMessage);
            }
            #endregion Catch
        }

        private void bgwStock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (dtStock != null && dtStock.Rows.Count > 0 && transactionType != "TollReceiveRaw")
                {
                    txtQuantityInHand.Text = Program.ParseDecimalObject(dtStock.Rows[0]["Quantity"].ToString());
                }
                else if (string.Equals(transactionType, "TollReceiveRaw", StringComparison.OrdinalIgnoreCase))
                {
                    var dtTollStock = (DataTable)e.Result;

                    if (dtTollStock != null)
                    {
                        txtQuantityInHand.Text = Program.ParseDecimalObject(dtTollStock.Rows[0][0].ToString());
                    }
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
                FileLogger.Log(this.Name, "bgwStock_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
        }

        private void txtCustomHouse_DoubleClick(object sender, EventArgs e)
        {
            CustomsHouseLoad();
        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void txtInvoiceValue_Leave(object sender, EventArgs e)
        {
            txtInvoiceValue.Text = Program.ParseDecimalObject(txtInvoiceValue.Text.Trim()).ToString();
        }

        private void txtExchangeRate_Leave(object sender, EventArgs e)
        {
            txtExchangeRate.Text = Program.ParseDecimalObject(txtExchangeRate.Text.Trim()).ToString();
        }

        private void dgvReceiveHistory_DoubleClick(object sender, EventArgs e)
        {

        }

        private void txtHSCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                HSCodeLoad();
            }
        }

        private void HSCodeLoad()
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
                    txtCDInput.Text = selectedRow.Cells["CD"].Value.ToString();
                    txtRDInput.Text = selectedRow.Cells["RD"].Value.ToString();
                    txtSDInput.Text = selectedRow.Cells["SD"].Value.ToString();
                    txtVATInput.Text = selectedRow.Cells["VAT"].Value.ToString();
                    txtATVInput.Text = selectedRow.Cells["AT"].Value.ToString();
                    txtAITInput.Text = selectedRow.Cells["AIT"].Value.ToString();

                   

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "HSCodeLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void dgvPurchase_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                if (e.KeyCode.Equals(Keys.F3))
                {


                    DataGridViewSelectedRowCollection selectedRows = dgvPurchase.SelectedRows;
                    if (selectedRows != null && selectedRows.Count > 0)
                    {
                        int selectedrowindex = dgvPurchase.SelectedCells[0].RowIndex;
                        DataGridViewRow selectedRow = dgvPurchase.Rows[selectedrowindex];
                        PurchasePopUp vm = new PurchasePopUp();
                        vm.ExpireDateTracking = ExpireDateTracking;
                        vm.TransactionType = transactionType;
                        vm.ProductName = selectedRow.Cells["ItemName"].Value.ToString();
                        vm.ProductCode = selectedRow.Cells["PCode"].Value.ToString();
                        vm.ExpireDate = selectedRow.Cells["ExpireDate"].Value.ToString();
                        vm.CPCName = selectedRow.Cells["CPCName"].Value.ToString();
                        vm.ItemNo = selectedRow.Cells["BEItemNo"].Value.ToString();
                        vm.FixedVATRebate = selectedRow.Cells["FixedVATRebate"].Value.ToString();
                        vm.Section21 = selectedRow.Cells["Section21"].Value.ToString();

                        var result = FormPurchaseOtherPopUp.SelectOne(vm);
                        dgvPurchase["CPCName", dgvPurchase.CurrentRow.Index].Value = result.CPCName.ToString();
                        dgvPurchase["ExpireDate", dgvPurchase.CurrentRow.Index].Value = result.ExpireDate.ToString();
                        dgvPurchase["BEItemNo", dgvPurchase.CurrentRow.Index].Value = result.ItemNo.ToString();
                        dgvPurchase["FixedVATRebate", dgvPurchase.CurrentRow.Index].Value = result.FixedVATRebate.ToString();
                        dgvPurchase["Section21", dgvPurchase.CurrentRow.Index].Value = result.Section21;

                    }

                }

                else if (e.KeyCode.Equals(Keys.F2))
                {

                    DataGridViewSelectedRowCollection selectedRows = dgvPurchase.SelectedRows;
                    if (selectedRows != null && selectedRows.Count > 0)
                    {
                        int selectedrowindex = dgvPurchase.SelectedCells[0].RowIndex;
                        DataGridViewRow selectedRow = dgvPurchase.Rows[selectedrowindex];
                        PurchasePopUp vm = new PurchasePopUp();
                        vm.TDSCode = selectedRow.Cells["TDSCode"].Value.ToString();
                        vm.Section = selectedRow.Cells["TDSSection"].Value.ToString();

                        var result = FormPurchaseOtherTDSPopUp.SelectOne(vm);
                        dgvPurchase["TDSCode", dgvPurchase.CurrentRow.Index].Value = result.TDSCode.ToString();
                        dgvPurchase["TDSSection", dgvPurchase.CurrentRow.Index].Value = result.Section.ToString();

                    }

                }
                else if (e.KeyCode.Equals(Keys.F4))
                {

                    if (rbtnPurchaseReturn.Checked)
                    {

                        DataGridViewSelectedRowCollection selectedRows = dgvPurchase.SelectedRows;
                        if (selectedRows != null && selectedRows.Count > 0)
                        {
                            int selectedrowindex = dgvPurchase.SelectedCells[0].RowIndex;
                            DataGridViewRow selectedRow = dgvPurchase.Rows[selectedrowindex];

                            var result = FormPurchaseAdjustment.SelectOne(selectedRow);

                            string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());

                            if (PrintResult.Length==3)
                            {
                                string ReasonOfReturn = PrintResult[0].ToString();
                                bool IsReason = Convert.ToBoolean(PrintResult[1]);
                                bool IsAll = Convert.ToBoolean(PrintResult[2]);

                                if (IsAll)
                                {
                                    #region ApplyAll
                                    for (int i = 0; i < dgvPurchase.RowCount; i++)
                                    {
                                        if (IsReason)
                                        {
                                            dgvPurchase["ReasonOfReturn", i].Value = ReasonOfReturn;
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    dgvPurchase["ReasonOfReturn", dgvPurchase.CurrentRow.Index].Value = ReasonOfReturn;
                                }
                            }
                            
                        }


                    }
                }

            }


            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "dgvPurchase_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void HSCodePopUp_Click(object sender, EventArgs e)
        {
            try
            {
                FormHsCode frm = new FormHsCode();
                frm.ShowDialog();
            }

            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "HSCodePopUp_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                SaleReport _report = new SaleReport();
                ReportDocument reportDocument = new ReportDocument();

                if (IsPost == true)
                {
                    PreviewOnly = false;
                }
                else
                {
                    PreviewOnly = true;
                }
                reportDocument = _report.Report_VAT6_3_Completed(txtPurchaseInvoiceNo.Text.Trim(), transactionType
                    , false
                    , false
                    , false
                    , false
                    , false
                    , false
                    , PreviewOnly, 1, 0, false, false, false
                    , false, false, false, connVM, "", false, true);


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
                FileLogger.Log(this.Name, "btnPurchaseInformation_Click", exMessage);
            }

            #endregion
        }

        private void txtLocalVATAmount_Leave(object sender, EventArgs e)
        {

            #region Try
            try
            {
                decimal VATAmount = Convert.ToDecimal(txtLocalVATAmount.Text.Trim());
                decimal TotalPrice = Convert.ToDecimal(txtUnitCost.Text.Trim());

                //decimal VATRate = (VATAmount * 100 / TotalPrice).toFixed(2);
                decimal VATRate = (VATAmount * 100 / TotalPrice);

                txtVATRate.Text = VATRate.ToString();
                //cmbVATRateInput.SelectedText = VATRate.ToString();
                //cmbVATRateInput.Items.Add(VATRate.ToString());

                //DropDownList ddl = (DropDownList)FindControl("cmbVATRateInput");
                //if (ddl != null)
                //{
                //    if (ddl.Items.FindByText(VATRate.ToString()) == null)
                //    {
                //        ddl.Items.Add(VATRate.ToString());
                //    }
                //}

                if (!cmbVATRateInput.Items.Contains(VATRate.ToString()))
                {
                    cmbVATRateInput.Items.Add(VATRate.ToString());
                }

                cmbVATRateInput.SelectedItem = VATRate.ToString();

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
                FileLogger.Log(this.Name, "txtLocalVATAmount_Leave", exMessage);
            }

            #endregion
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



    }
}
