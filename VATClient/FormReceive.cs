using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using SymphonySofttech.Utilities;
using VATClient.ReportPages;
using System.Globalization;
using VATClient.ReportPreview;
using VATClient.ModelDTO;
using System.Collections.Generic;
using DataGrid = System.Windows.Forms.DataGrid;
using VATViewModel.DTOs;
using VATServer.Library;
using System.Threading;
using System.Data.SqlClient;
using System.Data.OleDb;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using SymphonySofttech.Reports;
using CrystalDecisions.CrystalReports.Engine;
using VATClient.Integration.Berger;

namespace VATClient
{
    public partial class FormReceive : Form
    {
        public FormReceive()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #region Global Declarations

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string[] Condition = new string[] { "one" };
        CommonDAL commonDal = new CommonDAL();

        private List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private List<TenderDetailVM> tenderDetails = new List<TenderDetailVM>();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private ReportDocument reportDocument = new ReportDocument();

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private string transactionType = string.Empty;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private DataTable TenderResult;
        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private DataTable uomResult;
        private string CategoryId { get; set; }
        private string[] ProductLines;
        private string[] ProductFields;
        private string ProductResult;
        private bool ChangeData = false;
        private string ProductName;
        private String ProductCode;
        private string[] BOMDLines;
        private int IssHas = 0;
        private string NextID = string.Empty;
        private DataTable ProductResultDs;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        public string VFIN = "223";
        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string CategoryName = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";
        private bool IssueFromBOM = false;
        private bool CustomerWiseBOM = false;
        private decimal unitCost = 0;
        private int ReceivePlaceQty;
        private int ReceivePlaceAmt;
        private bool ChangeableNBRPrice;
        private bool LocalInVAT1;
        private bool LocalInVAT1KaTarrif;
        private bool TenderInVAT1;
        private bool TenderInVAT1Tender;
        private string vReceivePlaceQty, vReceivePlaceAmt, vIssueFromBOM, vChangeableNBRPrice
                 , vCustomerWiseBOM, shiftRequired, vLocalInVAT1, vLocalInVAT1KaTarrif, vTenderInVAT1, vTenderInVAT1Tender, vImportPrice = string.Empty;
        private bool PriceDeclarationForImport;
        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();
        public int SearchBranchId = 0;
        #endregion Global Variables

        #region Global Variables For BackgroundWorker

        private string ProductData = string.Empty;

        private string ReceiveDetailData = string.Empty;

        private DataTable ReceiveDetailResult = new DataTable();

        private string result = string.Empty;
        private string resultPost = string.Empty;


        private DataTable ResultUseQty;

        private ReceiveMasterVM receiveMasterVM;
        private List<ReceiveDetailVM> receiveDetailVMs = new List<ReceiveDetailVM>();
        private List<UomDTO> UOMs = new List<UomDTO>();

        private string varFinishItemNo = string.Empty;
        private decimal varQuantity;
        //private DateTime varEffectDate = new DateTime();
        private string varEffectDate = "";
        private bool Add = false;
        private bool Edit = false;
        private string ReturnTransType = string.Empty;

        string ImportExcelID = string.Empty;
        #endregion

        #region Vabiables for tracking
        List<TrackingCmbDTO> trackingsCmb = new List<TrackingCmbDTO>();
        List<TrackingVM> trackingsVm = new List<TrackingVM>();
        private bool TrackingTrace;
        #endregion

        public static string vCustomerID = "0";
        public static string vItemNo = "0";
        private string realTimeEntry;
        private string ImportId = string.Empty;

        #endregion

        #region Methods 01 / Load

        private void FormReceive_Load(object sender, EventArgs e)
        {
            try 
            {
                #region ToolTip

                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchReceiveNo, "Search existing Information");
                ToolTip1.SetToolTip(this.btnProductType, "Search Product Type");
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");
                ToolTip1.SetToolTip(this.cmbShift, "Production Shift");

                #endregion

                #region VAT Name

                VATName vname = new VATName();
                cmbVAT1Name.DataSource = vname.VATNameList;

                #endregion

                #region Reset Fields / Elements

                ClearAllFields();

                txtReceiveNo.Text = "~~~ New ~~~";
                ChangeData = false;

                #endregion

                #region Settings Value

                CommonDAL commonDal = new CommonDAL();

                shiftRequired = commonDal.settingsDesktop("Receive", "ShiftRequired",null,connVM);
                vCustomerWiseBOM = commonDal.settingsDesktop("Receive", "CustomerWiseBOM",null,connVM);
                vReceivePlaceQty = commonDal.settingsDesktop("Receive", "Quantity",null,connVM);
                vReceivePlaceAmt = commonDal.settingsDesktop("Receive", "Amount",null,connVM);
                vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM",null,connVM);
                if (rbtnTollFinishReceive.Checked || rbtnTollFinishReceiveWithoutBOM.Checked)
                {
                    vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "TollFGReceive",null,connVM);

                }

                vLocalInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3",null,connVM);
                vLocalInVAT1KaTarrif = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3Ka(Tarrif)",null,connVM);
                vTenderInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3",null,connVM);
                vTenderInVAT1Tender = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3(Tender)",null,connVM);
                vImportPrice = commonDal.settingsDesktop("Receive", "PriceDeclarationForImport",null,connVM);
                vChangeableNBRPrice = commonDal.settingsDesktop("Production", "ChangeableNBRPrice",null,connVM);
                string vByProduct = commonDal.settingsDesktop("ByProduct", "ByProduct",null,connVM);
                ChangeableNBRPrice = Convert.ToBoolean(vChangeableNBRPrice == "Y" ? true : false);

                realTimeEntry = commonDal.settingsDesktop("Receive", "EntryRealTime",null,connVM);

                #endregion

                #region Check Point

                if (string.IsNullOrEmpty(vReceivePlaceQty)
                    || string.IsNullOrEmpty(vReceivePlaceAmt)
                    || string.IsNullOrEmpty(vIssueFromBOM)
                    || string.IsNullOrEmpty(vLocalInVAT1)
                    || string.IsNullOrEmpty(vLocalInVAT1KaTarrif)
                    || string.IsNullOrEmpty(vTenderInVAT1)
                    || string.IsNullOrEmpty(vTenderInVAT1Tender)
                    || string.IsNullOrEmpty(vImportPrice)
                    || string.IsNullOrEmpty(vCustomerWiseBOM)
                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                #endregion


                #region Settings Setup

                CustomerWiseBOM = Convert.ToBoolean(vCustomerWiseBOM.ToString() == "Y" ? true : false);

                ReceivePlaceQty = Convert.ToInt32(vReceivePlaceQty);
                ReceivePlaceAmt = Convert.ToInt32(vReceivePlaceAmt);
                IssueFromBOM = Convert.ToBoolean(vIssueFromBOM.ToString() == "Y" ? true : false);
                LocalInVAT1 = Convert.ToBoolean(vLocalInVAT1 == "Y" ? true : false);
                LocalInVAT1KaTarrif = Convert.ToBoolean(vLocalInVAT1KaTarrif == "Y" ? true : false);
                TenderInVAT1 = Convert.ToBoolean(vTenderInVAT1 == "Y" ? true : false);
                TenderInVAT1Tender = Convert.ToBoolean(vTenderInVAT1Tender == "Y" ? true : false);
                PriceDeclarationForImport = Convert.ToBoolean(vImportPrice == "Y" ? true : false);

                chkIssueFromBOM.Checked = IssueFromBOM;
                if (vByProduct.ToLower() == "n")
                {

                    chkIssueFromBOM.Visible = false;
                }

                #endregion

                #region Element Stats

                if (IssueFromBOM == true)
                {
                    btnVAT16.Visible = true;
                    chkPercent.Checked = true;
                }
                else
                {
                    btnVAT16.Visible = false;

                    chkPercent.Checked = false;
                }

                string vTracking = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking",null,connVM);
                if (string.IsNullOrEmpty(vTracking))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Button Stats

                if (rbtnTollFinishReceive.Checked || rbtnTollFinishReceiveWithoutBOM.Checked)
                {
                    btnVAT17.Visible = false;
                }
                else
                {
                    btnVAT17.Visible = true;
                }

                #endregion

                #region Shift Drop Down

                IShift _sDal = OrdinaryVATDesktop.GetObject<ShiftDAL, ShiftRepo, IShift>(OrdinaryVATDesktop.IsWCF);

                cmbShift.DataSource = _sDal.SearchForTime(DateTime.Now.ToString("HH:mm"), connVM);
                cmbShift.DisplayMember = "ShiftName";
                cmbShift.ValueMember = "Id";

                #endregion

                #region Focus

                cmbProductName.Focus();

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
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }

            #endregion Catch

        }

        private void FormMaker()
        {
            try
            {
                #region Element Stats

                cmbVAT1Name.Enabled = true;
                //cmbVAT1Name.SelectedIndex = 0;

                btnVAT16.Visible = false;
                btnVAT17.Visible = false;
                btnVAT18.Visible = false;
                chkTender.Visible = false;
                txtReceiveNoPre.Visible = false;
                btnPreviousSearch.Visible = false;
                lblReturn.Visible = false;
                btnAdd.Enabled = true;


                #endregion

                #region Transaction Type


                if (rbtnOther.Checked)//start11
                {
                    this.Text = "Production Receive";
                    ////chkTender.Visible = true;
                    if (LocalInVAT1 == true)
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

                    if (TrackingTrace == true)
                    {
                        btnTracking.Visible = true;
                    }
                    else
                    {
                        btnTracking.Visible = false;
                    }

                }

                else if (rbtnTollFinishReceive.Checked)
                {
                    this.Text = "Production Receive(Toll Production)";

                    ////////cmbVAT1Name.SelectedIndex = 7;
                    brnTR.Visible = true;
                    brnTR.Text = "TR(16)";
                    btnVAT17.Text = "TR(17)";
                    btnVAT16.Visible = false;
                    //btnVAT17.Visible = false;
                    cmbVAT1Name.Text = "VAT 4.3 (Toll Issue)";

                    //06-Dec-2020
                    btnVAT16.Visible = false;
                    btnVAT17.Visible = false;


                }
                else if (rbtnTollFinishReceiveWithoutBOM.Checked)
                {
                    this.Text = "Production Receive Without BOM(Toll Production)";

                    ////////cmbVAT1Name.SelectedIndex = 7;
                    brnTR.Visible = true;
                    brnTR.Text = "TR(16)";
                    btnVAT17.Text = "TR(17)";
                    btnVAT16.Visible = false;
                    //btnVAT17.Visible = false;
                    cmbVAT1Name.Text = "VAT 4.3 (Toll Issue)";

                    //06-Dec-2020
                    btnVAT16.Visible = false;
                    btnVAT17.Visible = false;


                }
                else if (rbtnWIP.Checked)
                {
                    this.Text = "WIP (Production Receive)";
                    btnVAT16.Visible = true;
                    btnVAT17.Visible = false;

                }
                else if (rbtnReceiveReturn.Checked)
                {
                    this.Text = "Production Receive Return";
                    txtReceiveNoPre.Visible = true;
                    btnPreviousSearch.Visible = true;
                    lblReturn.Visible = true;
                    ////btnAdd.Enabled = false;

                    if (TrackingTrace == true)
                    {
                        btnTracking.Visible = true;
                    }
                    else
                    {
                        btnTracking.Visible = false;
                    }
                }
                else if (rbtnPackageProduction.Checked)
                {
                    this.Text = "Production (Package)";
                    btnVAT16.Visible = true;
                    btnVAT17.Visible = false;
                    cmbVAT1Name.SelectedIndex = 9;

                }
                txtUnitCost.ReadOnly = ChangeableNBRPrice;
                txtNBRPrice.ReadOnly = ChangeableNBRPrice;

                #endregion Transaction Type

                #region Button Import Integration Lisence
                if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
                {
                    btnImport.Visible = false;
                }
                #endregion

                if (realTimeEntry == "N")
                {
                    dtpReceiveDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy 00:00:00"));

                    dtpReceiveDate.CustomFormat = "dd-MM-yyyy";
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

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Product

                ProductDAL productDal = new ProductDAL();
                ////IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, CategoryName,
                                                                 ActiveStatusProductParam,
                                                                 TradingParam, NonStockParam, ProductCodeParam, connVM);

                #endregion Product

                #region UOM
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);
                //UOMDAL uomdal = new UOMDAL();
                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

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

                #region Product

                ProductsMini.Clear();
                //cmbProduct.DisplayMember = "ProductNameCode";

                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
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
                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());

                    //bool TradingF = false;
                    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                    //prod.Trading = Trading;

                    //bool NonStockF = false;
                    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                    //prod.NonStock = NonStock;
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                    ProductsMini.Add(prod);

                } //End For
                ProductSearchDsLoad();

                #endregion Product

                #region UOM
                UOMs.Clear(); cmbUom.Items.Clear();
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

                txtNBRPrice.ReadOnly = ChangeableNBRPrice == true ? false : true;
                txtUnitCost.ReadOnly = ChangeableNBRPrice == true ? false : true;

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
                ChangeData = false;
                this.progressBar1.Visible = false;
            }

        }

        private void FormLoad()
        {

            #region Product

            ItemNoParam = string.Empty;
            CategoryIDParam = string.Empty;
            IsRawParam = string.Empty;
            CategoryName = string.Empty;
            ActiveStatusProductParam = string.Empty;
            TradingParam = string.Empty;
            NonStockParam = string.Empty;
            ProductCodeParam = string.Empty;


            if (CategoryId != null)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = CategoryId;
                IsRawParam = string.Empty;
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
            }
            else if (rbtnTollFinishReceive.Checked || rbtnTollFinishReceiveWithoutBOM.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Overhead";
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Overhead";
            }
            else if (rbtnWIP.Checked)
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "WIP";
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "WIP";
            }
            else
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Finish";
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Finish";
            }

            #endregion Product

            #region 2012 Law - Button Control

            btnVAT16.Text = "6.1";
            btnVAT17.Text = "6.2";
            btnVAT18.Visible = false;

            #endregion



        }

        private void TransactionTypes()
        {
            #region Transaction Type

            transactionType = string.Empty;
            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }

            else if (rbtnTollFinishReceive.Checked)
            {
                transactionType = "TollFinishReceive";

            }
            else if (rbtnTollFinishReceiveWithoutBOM.Checked)
            {
                transactionType = "TollFinishReceiveWithoutBOM";

            }
            else if (rbtnWIP.Checked)
            {
                transactionType = "WIP";
            }
            else if (rbtnReceiveReturn.Checked)
            {
                transactionType = "ReceiveReturn";
            }
            else if (rbtnTender.Checked)
            {
                transactionType = "Tender";
            }
            else if (rbtnPackageProduction.Checked)
            {
                transactionType = "PackageProduction";
            }




            #endregion Transaction Type
        }

        private void ProductSearchDsFormLoad()
        {
            //string ProductData = string.Empty;
            try
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = string.Empty;
                CategoryName = string.Empty;
                ActiveStatusProductParam = string.Empty;
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;


                if (CategoryId != null)
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = CategoryId;
                    IsRawParam = string.Empty; // txtCategoryName.Text.Trim();
                    CategoryName = txtCategoryName.Text.Trim();
                    ActiveStatusProductParam = "Y";
                    TradingParam = string.Empty;
                    NonStockParam = string.Empty;
                    ProductCodeParam = string.Empty;
                }
                else
                {
                    ItemNoParam = string.Empty;
                    CategoryIDParam = string.Empty;
                    IsRawParam = "Finish";
                    CategoryName = string.Empty;
                    ActiveStatusProductParam = "Y";
                    TradingParam = "N";
                    NonStockParam = "N";
                    ProductCodeParam = string.Empty;
                    txtCategoryName.Text = "Finish";
                }


                backgroundWorkerReceiveProductSearch.RunWorkerAsync();

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

            #endregion Catch
        }

        private void backgroundWorkerReceiveProductSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryId, IsRawParam, CategoryName,
                                                                 ActiveStatusProductParam,
                                                                 TradingParam, NonStockParam, ProductCodeParam, connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerReceiveProductSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ProductsMini.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
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
                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());

                    //bool TradingF = false;
                    //bool Trading = Boolean.TryParse(item2["Trading"].ToString(), out TradingF);
                    //prod.Trading = Trading;

                    //bool NonStockF = false;
                    //bool NonStock = Boolean.TryParse(item2["NonStock"].ToString(), out NonStockF);
                    //prod.NonStock = NonStock;
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                    ProductsMini.Add(prod);

                } //End For
                ProductSearchDsLoad();
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveProductSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {

                this.btnProductType.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void ProductSearchDsLoad()
        {
            //No SOAP Service

            try
            {
                cmbProductName.Items.Clear();
                string var = "1,2";
                if (true)
                {

                    var prodByCode = from prd in ProductsMini.ToList()
                                     orderby prd.ProductCode
                                     select prd.ProductCode;


                    if (prodByCode != null && prodByCode.Any())
                    {
                    }


                }
                if (true)
                {
                    var prodByName = from prd in ProductsMini.ToList()
                                     orderby prd.ProductName
                                     ////select prd.ProductName;
                                     select prd.ProductName + "~" + prd.ProductCode;


                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName.Items.AddRange(prodByName.ToArray());
                    }
                }
                cmbProductName.Focus();

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

            #endregion Catch


        }

        private void ProductSearchDsLoadTender()
        {
            //No SOAP Service

            try
            {
                //string var = iDs;
                if (chkPCode.Checked == true)
                {

                    var prodByCode = from prd in tenderDetails.ToList()
                                     orderby prd.PCode
                                     select prd.PCode;



                    if (prodByCode != null && prodByCode.Any())
                    {
                    }


                }
                else
                {
                    var prodByName = from prd in tenderDetails.ToList()
                                     orderby prd.ItemName
                                     select prd.ItemName;


                    if (prodByName != null && prodByName.Any())
                    {
                    }
                }

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
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoadTender", exMessage);
            }

            #endregion Catch


        }

        private void FindBomId()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            for (int i = 0; i < dgvReceive.RowCount; i++)
            {
                string BOMId = dgvReceive["BOMId", i].Value.ToString();
                string ItemNo = dgvReceive["ItemNo", i].Value.ToString();
                string VATName = dgvReceive["VATName", i].Value.ToString();
                if (BOMId == "0")
                {
                    if (CustomerWiseBOM)
                    {
                        BOMId = productDal.GetBOM(ItemNo, VATName, dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID, connVM).Rows[0]["BOMId"].ToString();

                    }
                    else
                    {
                        BOMId = productDal.GetBOM(ItemNo, VATName,
                                                      dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, "", connVM).Rows[0]["BOMId"].ToString();
                    }
                    dgvReceive["BOMId", i].Value = string.IsNullOrEmpty(BOMId) ? "0" : BOMId;
                }
            }

        }

        #endregion

        #region Methods 02 / Add, Change, Remove

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //if (txtProductCode.Text.Trim() != vItemNo)
                //{
                //    if (CustomerWiseBOM)
                //    {
                //        MessageBox.Show("This item Not for the Selected Customer ", this.Text);
                //        return;
                //    }
                //}
                BOMDAL bomdal = new BOMDAL();


                #region Comments 12-Sep-2019

                //////string BOMId = "0";
                //////if (rbtnTollFinishReceive.Checked)
                //////{
                //////    BOMId = bomdal.FindBOMIDOverHead(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);

                //////}
                //////else
                //////{
                //////    BOMId = bomdal.FindBOMID(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);
                //////}

                #endregion



                if (IssueFromBOM == true)
                {
                    if (string.IsNullOrEmpty(txtBOMId.Text.Trim()))
                    {
                        MessageBox.Show("There is no Price declaration for '" + txtProductName.Text.Trim() + "'", this.Text);
                        return;
                    }
                }


                ChangeData = true;

                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtUnitCost.Focus();
                    return;
                }
                #region null
                if (chkNonStock.Checked == true)
                {
                    MessageBox.Show("Nonstock item receive not required");
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "-";
                }
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item");
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
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input quantity");
                    txtQuantity.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtUnitCost.Text) <= 0)
                {
                    MessageBox.Show("Please declare price first");
                    return;
                }

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    if (dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text)
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        return;
                    }
                }

                #endregion null
                UomsValue();
                #region Check Stock


                if (rbtnReceiveReturn.Checked)
                {
                    if (
                        Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                    {
                        MessageBox.Show("Stock Not available");
                        txtQuantity.Focus();
                        return;
                    }
                }

                #endregion Check Stock

                txtNBRPrice.Text = txtUnitCost.Text.Trim();
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvReceive.Rows.Add(NewRow);

                dgvReceive["ItemNo", dgvReceive.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvReceive["BOMId", dgvReceive.RowCount - 1].Value = txtBOMId.Text.Trim();
                dgvReceive["ItemName", dgvReceive.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvReceive["PCode", dgvReceive.RowCount - 1].Value = txtPCode.Text.Trim();

                dgvReceive["VATRate", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();//"0.00");
                dgvReceive["SD", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();//"0.00");

                dgvReceive["Comments", dgvReceive.RowCount - 1].Value = txtCommentsDetail.Text.Trim();

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), ReceivePlaceAmt).ToString();

                dgvReceive["NBRPrice", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();
                dgvReceive["Status", dgvReceive.RowCount - 1].Value = "New";
                dgvReceive["Stock", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
                dgvReceive["Previous", dgvReceive.RowCount - 1].Value = 0; // txtQuantity.Text.Trim();
                dgvReceive["Change", dgvReceive.RowCount - 1].Value = 0;

                dgvReceive["UOM", dgvReceive.RowCount - 1].Value = cmbUom.Text.Trim();
                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();

                dgvReceive["UnitPrice", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();//"0,0.00");
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();

                dgvReceive["Quantity", dgvReceive.RowCount - 1].Value = txtQuantity.Text.Trim();//"0,0.0000");
                dgvReceive["UOMPrice", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                dgvReceive["UOMc", dgvReceive.RowCount - 1].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim()).ToString(); // txtUOM.Text.Trim();
                dgvReceive["UOMn", dgvReceive.RowCount - 1].Value = txtUOM.Text.Trim(); // txtUOM.Text.Trim();
                dgvReceive["UOMQty", dgvReceive.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvReceive["VATName", dgvReceive.RowCount - 1].Value = cmbVAT1Name.Text.Trim(); // txtUOM.Text.Trim();



                Rowcalculate();
                selectLastRow();


                cmbProductName.Focus();


                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "";
                txtQuantity.Text = "";
                //txtVATRate.Text = "";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0";
                if (rbtnTollFinishReceive.Checked || rbtnTollFinishReceiveWithoutBOM.Checked)
                {

                    cmbVAT1Name.Text = "VAT 4.3 (Toll Issue)";


                }
                else
                {
                    cmbVAT1Name.SelectedIndex = 0;

                }
                cmbProductName.Text = "";
                cmbUom.Text = "";
                txtBOMId.Text = "";
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
                FileLogger.Log(this.Name, "ReceiveAddSingle", exMessage);
            }

            #endregion Catch

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtProductCode.Text.Trim() != vItemNo)
                {
                    if (CustomerWiseBOM)
                    {
                        MessageBox.Show("This item Not for the Selected Customer ", this.Text);
                        return;
                    }
                }
                BOMDAL bomdal = new BOMDAL();

                //////string BOMId = bomdal.FindBOMID(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);

                if (IssueFromBOM == true)
                {
                    if (string.IsNullOrEmpty(txtBOMId.Text))
                    {
                        MessageBox.Show("There is no Price declaration for '" + txtProductName.Text.Trim() + "'", this.Text);
                        return;
                    }
                }


                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtUnitCost.Focus();
                    return;
                }
                #region 1st if
                if (dgvReceive.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ChangeData = true;
                if (chkNonStock.Checked == true)
                {
                    MessageBox.Show("Nonstock item receive not required");
                    return;
                }

                if (string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    if (Edit == false)
                    {


                        MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.",
                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "-";
                }

                UomsValue();

                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text);

                if (rbtnReceiveReturn.Checked)
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
                        else if (CurrentValue == PreviousValue)
                        {

                        }
                        else
                        {
                            MessageBox.Show("Return quantity can not be greater than actual quantity.");
                            txtQuantity.Text = PreviousValue.ToString();
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
                    //        Convert.ToDecimal(PreviousValue - CurrentValue) >
                    //        Convert.ToDecimal(StockValue))
                    //    {
                    //        MessageBox.Show("Stock Not available");
                    //        txtQuantity.Focus();
                    //        return;
                    //    }

                    //}

                }

                #endregion Stock Chekc

                txtNBRPrice.Text = txtUnitCost.Text.Trim();



                dgvReceive["VATRate", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();//"0.00");
                dgvReceive["Comments", dgvReceive.CurrentRow.Index].Value = txtCommentsDetail.Text.Trim();

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), ReceivePlaceAmt).ToString();

                dgvReceive["NBRPrice", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();//"0,0.00");
                dgvReceive["SD", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();//"0.00");

                dgvReceive["PCode", dgvReceive.CurrentRow.Index].Value = txtPCode.Text.Trim();
                dgvReceive["ItemName", dgvReceive.CurrentRow.Index].Value = txtProductName.Text.Trim();
                dgvReceive["ItemNo", dgvReceive.CurrentRow.Index].Value = txtProductCode.Text;




                dgvReceive["BOMId", dgvReceive.CurrentRow.Index].Value = txtBOMId.Text.Trim();

                dgvReceive["UOM", dgvReceive.CurrentRow.Index].Value = cmbUom.Text.Trim();


                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();


                dgvReceive["Quantity", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();//"0,0.0000");

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();

                dgvReceive["UnitPrice", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();

                dgvReceive["UOMPrice", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                dgvReceive["UOMc", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(txtUomConv.Text.Trim()).ToString(); // txtUOM.Text.Trim();
                dgvReceive["UOMn", dgvReceive.CurrentRow.Index].Value = txtUOM.Text.Trim(); // txtUOM.Text.Trim();
                dgvReceive["UOMQty", dgvReceive.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvReceive["VATName", dgvReceive.CurrentRow.Index].Value = cmbVAT1Name.Text.Trim(); // cmbVAT1Name.Text.Trim();



                if (dgvReceive.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvReceive["Status", dgvReceive.CurrentRow.Index].Value = "Change";
                }
                dgvReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;

                dgvReceive.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "";
                txtQuantity.Text = "";
                //txtVATRate.Text = "";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0";
                cmbVAT1Name.SelectedIndex = 0;
                txtBOMId.Text = "";

                cmbProductName.Focus();


                #endregion



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
                FileLogger.Log(this.Name, "ReceiveChangeSingle", exMessage);
            }

            #endregion Catch
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedRow();

        }

        private void RemoveSelectedRow()
        {
            try
            {
                if (dgvReceive.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvReceive.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvReceive.Rows.RemoveAt(dgvReceive.CurrentRow.Index);
                        Rowcalculate();
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion
        
        }

        private void Rowcalculate()
        {
            try
            {
                decimal SumVATAmount = 0;
                decimal SumSubTotal = 0;
                decimal Quantity = 0;
                decimal SD = 0;
                decimal VATRate = 0;
                decimal Cost = 0;
                decimal SDAmount = 0;
                decimal VATAmount = 0;
                decimal SubAmount = 0;

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    Quantity = Convert.ToDecimal(dgvReceive["Quantity", i].Value);
                    VATRate = Convert.ToDecimal(dgvReceive["VATRate", i].Value);
                    Cost = Convert.ToDecimal(dgvReceive["UnitPrice", i].Value);
                    SD = Convert.ToDecimal(dgvReceive["SD", i].Value);

                    if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                        Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), ReceivePlaceQty));

                    if (Program.CheckingNumericString(Cost.ToString(), "Cost") == true)
                        Cost = Convert.ToDecimal(Program.FormatingNumeric(Cost.ToString(), ReceivePlaceAmt));

                    SDAmount = (Quantity * Cost) * SD / 100;
                    if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                        SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), ReceivePlaceAmt));


                    VATAmount = (SDAmount + (Quantity * Cost)) * VATRate / 100;
                    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                        VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), ReceivePlaceAmt));


                    SubAmount = (Quantity * Cost) + (SDAmount + VATAmount);
                    if (Program.CheckingNumericString(SubAmount.ToString(), "SubAmount") == true)
                        SubAmount = Convert.ToDecimal(Program.FormatingNumeric(SubAmount.ToString(), ReceivePlaceAmt));



                    dgvReceive[0, i].Value = i + 1;

                    dgvReceive["VATAmount", i].Value = Program.ParseDecimalObject(VATAmount).ToString();//"0,0.00");
                    dgvReceive["SDAmount", i].Value = Program.ParseDecimalObject(SDAmount).ToString();//"0,0.00");
                    dgvReceive["SubTotal", i].Value = Program.ParseDecimalObject(SubAmount).ToString();//"0,0.00");
                    SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvReceive["VATAmount", i].Value);
                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvReceive["SubTotal", i].Value);

                    dgvReceive["Change", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(dgvReceive["Quantity", i].Value)
                                                                      -
                                                                      Convert.ToDecimal(dgvReceive["Previous", i].Value))
                        .ToString();//"0,0.0000");
                }
                if (Program.CheckingNumericString(SumVATAmount.ToString(), "SumVATAmount") == true)
                    SumVATAmount = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), ReceivePlaceAmt));

                if (Program.CheckingNumericString(SumSubTotal.ToString(), "SumSubTotal") == true)
                    SumSubTotal = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), ReceivePlaceAmt));

                txtTotalVATAmount.Text = Program.ParseDecimalObject(SumVATAmount).ToString();//"0,0.00");
                txtTotalAmount.Text = Program.ParseDecimalObject(SumSubTotal).ToString();//"0,0.00");

                FindBomId();
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
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion Catch
        }

        private void selectLastRow()
        {
            try
            {
                if (dgvReceive.Rows.Count > 0)
                {

                    dgvReceive.Rows[dgvReceive.Rows.Count - 1].Selected = true;
                    dgvReceive.CurrentCell = dgvReceive.Rows[dgvReceive.Rows.Count - 1].Cells[1];

                }
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

            #endregion Catch
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

        private void ReceiveRemoveSingle()
        {
            try
            {
                if (dgvReceive.RowCount > 0)
                {
                    if (string.IsNullOrEmpty(txtUOM.Text)
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
                    MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                ChangeData = true;

                if (chkNonStock.Checked == true)
                {
                    MessageBox.Show("Nonstock item receive not required");
                    return;
                }
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show("Please select a Item");
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "-";
                }

                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                CurrentValue = 0;

                if (rbtnReceiveReturn.Checked)
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


                if (Convert.ToDecimal(dgvReceive.CurrentCellAddress.Y) < 0)
                {
                    MessageBox.Show("There have no Data to Delete");
                    return;
                }

                dgvReceive.CurrentRow.Cells["Status"].Value = "Delete";
                //dgvReceive.CurrentRow.Cells["Quantity"].Value = 0.00;   
                //dgvReceive.CurrentRow.Cells["SD"].Value = 0.00;
                //dgvReceive.CurrentRow.Cells["UnitPrice"].Value = 0.00;

                dgvReceive.CurrentRow.Cells["Quantity"].Value = 0.00;

                dgvReceive.CurrentRow.Cells["UnitPrice"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["SubTotal"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["VATRate"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["VATAmount"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["NBRPrice"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["SD"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["SDAmount"].Value = 0.00;

                dgvReceive.CurrentRow.Cells["UOMPrice"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["UOMc"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["UOMQty"].Value = 0.00;

                dgvReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvReceive.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                Rowcalculate();
                txtProductName.Text = "";
                txtProductCode.Text = "";


                cmbProductName.Focus();

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
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReceiveRemoveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReceiveRemoveSingle", exMessage);
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
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReceiveRemoveSingle", exMessage);
            }

            #endregion Catch
        }


        private decimal CountReturnTrackingItem(string Item)
        {
            decimal noOfReturnItems = 0;

            for (int i = 0; i < trackingsVm.Count; i++)
            {
                if (trackingsVm[i].ItemNo == Item)
                {
                    noOfReturnItems = noOfReturnItems + 1;
                }
            }
            return noOfReturnItems;

        }

        #endregion

        #region Methods 03 / Save, Update, Post, Search

        private void btnSave_Click(object sender, EventArgs e)
        {
            // master details save

            try
            {

                #region Check Point


                #region Find Fiscal Year Lock

                string PeriodName = dtpReceiveDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues,null,null,connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion

                #region Null Check

                if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                {
                    MessageBox.Show("Please Select Shift");
                    cmbShift.Focus();
                    return;
                }


                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}


                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtRefNo.Text == "")
                {
                    txtRefNo.Text = "-";
                }
                string code = commonDal.settingValue("CompanyCode", "Code",connVM);

                if (code == "SCBL")
                {
                    if (txtRefNo.Text.Trim().Length <= 2)
                    {
                        MessageBox.Show("Please Enter Ref/Trip # More Then Two Digit!");
                        return;
                    }
                }
               
                #endregion

                #region Exist Check

                ReceiveMasterVM vm = new ReceiveMasterVM();
                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new ReceiveDAL().SelectAllList(0, new[] { "rh.ReceiveNo" }, new[] { NextID },null,null,null,connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #endregion

                #region Check Ref/Trip
                
                if (!string.IsNullOrEmpty(txtRefNo.Text.Trim()) && txtRefNo.Text.Trim() != "" && txtRefNo.Text.Trim() != "-")
                {



                    CommonDAL _CDAL = new CommonDAL();
                    //ICommon _CDAL = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                    if (_CDAL.DataAlreadyUsed("ReceiveHeaders", "ReferenceNo", txtRefNo.Text.Trim(), null, null, connVM) != 0)
                    {
                        throw new Exception("This Ref/Trip # Already Used." + " SalesInvoiceHeaders");
                    }
                }
                if (dgvReceive.RowCount <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                #endregion

                #region Tracking

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
                    //if (rbtnReceiveReturn.Checked)
                    //{
                    //    string item = "";
                    //    decimal qty, returnItems = 0;

                    //    for (int i = 0; i < dgvReceive.RowCount; i++)
                    //    {
                    //        item = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    //        qty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    //        returnItems = CountReturnTrackingItem(item);
                    //        if (qty != returnItems)
                    //        {
                    //            MessageBox.Show("Please select tracking quantity " + qty + " for Item Code : " + dgvReceive.Rows[i].Cells["PCode"].Value.ToString() + ".",
                    //                this.Text, MessageBoxButtons.OK,
                    //                 MessageBoxIcon.Information);

                    //            btnTracking_Click(sender, e);
                    //            return;
                    //        }
                    //    }


                    //}

                }

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Value Assign

                #region Assign Master

                receiveMasterVM = new ReceiveMasterVM();
                receiveMasterVM.ReceiveNo = NextID;
                receiveMasterVM.IssueFromBOM = chkIssueFromBOM.Checked == true ? "Y" : "N";
                receiveMasterVM.ShiftId = cmbShift.SelectedValue.ToString(); //Post+
                receiveMasterVM.ReceiveDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +DateTime.Now.ToString(" HH:mm:ss"); //dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                receiveMasterVM.CustomerID = vCustomerID;
                receiveMasterVM.WithToll = chkWithToll.Checked == true ? "Y" : "N";
                receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                receiveMasterVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                receiveMasterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                receiveMasterVM.Comments = txtComments.Text.Trim();
                receiveMasterVM.CreatedBy = Program.CurrentUser;
                receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.LastModifiedBy = Program.CurrentUser;
                receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.transactionType = transactionType;
                receiveMasterVM.Post = "N";
                receiveMasterVM.ReferenceNo = txtRefNo.Text;
                receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
                //receiveMasterVM.VatName = cmbVAT1Name.Text;
                receiveMasterVM.ReturnId = txtReceiveNoPre.Text.Trim();

                receiveMasterVM.BranchId = Program.BranchId;
                receiveMasterVM.AppVersion = Program.GetAppVersion();

                //encriptedReceiveHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, ReceiveHeaderData);

                #endregion

                #region Assign Detail

                receiveDetailVMs = new List<ReceiveDetailVM>();

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {

                    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();

                    //receiveDetailVM.ReceiveLineNo = NextID;
                    receiveDetailVM.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    receiveDetailVM.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    receiveDetailVM.ItemCode = dgvReceive.Rows[i].Cells["PCode"].Value.ToString();
                    receiveDetailVM.ItemName = dgvReceive.Rows[i].Cells["ItemName"].Value.ToString();
                    receiveDetailVM.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    receiveDetailVM.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    receiveDetailVM.NBRPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["NBRPrice"].Value.ToString());
                    receiveDetailVM.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    receiveDetailVM.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    receiveDetailVM.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    receiveDetailVM.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    receiveDetailVM.CommentsD = Program.CurrentUser;
                    receiveDetailVM.SD = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SD"].Value.ToString());
                    receiveDetailVM.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());
                    receiveDetailVM.BOMId = Convert.ToInt32(string.IsNullOrWhiteSpace(dgvReceive.Rows[i].Cells["BOMId"].Value.ToString()) ? 0 : dgvReceive.Rows[i].Cells["BOMId"].Value);
                    receiveDetailVM.UOMn = dgvReceive.Rows[i].Cells["UOMn"].Value.ToString();
                    receiveDetailVM.UOMPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMPrice"].Value.ToString());
                    receiveDetailVM.UOMQty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString());
                    receiveDetailVM.UOMc = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMc"].Value.ToString());
                    receiveDetailVM.VatName = dgvReceive.Rows[i].Cells["VATName"].Value.ToString();

                    if (rbtnReceiveReturn.Checked)
                    {
                        receiveDetailVM.ReturnTransactionType = dgvReceive.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                    }

                    receiveDetailVM.BranchId = Program.BranchId;
                    receiveDetailVMs.Add(receiveDetailVM);
                }

                if (receiveDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Tracking


                if (TrackingTrace == true)
                {
                    for (int i = 0; i < dgvReceive.Rows.Count; i++)
                    {
                        string itemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());

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

                #endregion

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Save

                backgroundWorkerSave.RunWorkerAsync();

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

            #endregion Catch


        }

        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;

                sqlResults = new string[4];
                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.ReceiveInsert(receiveMasterVM, receiveDetailVMs, trackingsVm, null, null, Program.BranchId, connVM,Program.CurrentUserID);
                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }

            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 2)
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();


                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvReceive.RowCount; i++)
                            {
                                dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
                            }
                        }

                    }

                //End Complete
                ChangeData = false;
                SearchBranchId = Program.BranchId;

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                #region Find Fiscal Year Lock

                string PeriodName = dtpReceiveDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues,null,null,connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }

                #endregion

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

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtRefNo.Text == "")
                {
                    txtRefNo.Text = "-";
                }
                string code = commonDal.settingValue("CompanyCode", "Code",connVM);

                if (code == "SCBL")
                {
                    if (txtRefNo.Text.Trim().Length <= 2)
                    {
                        MessageBox.Show("Please Enter Ref/Trip # More Then Two Digit!");
                        return;
                    }
                    if (txtRefNo.Text.Trim().Length >= 2)
                    {
                        ISale sDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);
                        var Sale = sDal.SelectAllList(0, new[] { "sih.SerialNo" }, new[] { txtRefNo.Text.Trim() },null,null,null,connVM).FirstOrDefault();
                        if (Sale != null)
                        {
                            if (Sale.Post == "Y")
                            {
                                MessageBox.Show("This Ref/Trip # Already Used In Sales Chalan You Can't Update this Transaction." + " ReceiveHeaders", this.Text);
                                return;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(txtRefNo.Text.Trim()) && txtRefNo.Text.Trim() != "" && txtRefNo.Text.Trim() != "-")
                {
                    CommonDAL _CDAL = new CommonDAL();
                    //ICommon _CDAL = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                    if (_CDAL.DataAlreadyUsedWithoutThis("ReceiveHeaders", "ReferenceNo", txtRefNo.Text.Trim(), "ReceiveNo", txtHiddenInvoiceNo.Text.Trim(), null, null, connVM) != 0)
                    {
                        MessageBox.Show("This Ref/Trip # Already Used." + " ReceiveHeaders", this.Text);
                        return;
                    }
                }

                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Tracking

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

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Value Assign

                #region Master

                receiveMasterVM = new ReceiveMasterVM();

                receiveMasterVM.IssueFromBOM = chkIssueFromBOM.Checked == true ? "Y" : "N";
                receiveMasterVM.ShiftId = cmbShift.SelectedValue.ToString(); //Post
                receiveMasterVM.ReceiveNo = NextID;
                receiveMasterVM.CustomerID = vCustomerID;
                receiveMasterVM.ReceiveDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +DateTime.Now.ToString(" HH:mm:ss");
                receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                receiveMasterVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                receiveMasterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                receiveMasterVM.Comments = txtComments.Text.Trim();
                receiveMasterVM.ReferenceNo = txtRefNo.Text;
                receiveMasterVM.CreatedBy = Program.CurrentUser;
                receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.LastModifiedBy = Program.CurrentUser;
                receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.WithToll = chkWithToll.Checked == true ? "Y" : "N"; ;
                receiveMasterVM.transactionType = transactionType;
                receiveMasterVM.Post = "N";
                receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
                receiveMasterVM.ReturnId = txtReceiveNoPre.Text.Trim();
                receiveMasterVM.BranchId = Program.BranchId;

                #endregion

                #region Details

                receiveDetailVMs = new List<ReceiveDetailVM>();

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {


                    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();

                    //receiveDetailVM.ReceiveLineNo = NextID;
                    receiveDetailVM.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    receiveDetailVM.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    receiveDetailVM.ItemCode = dgvReceive.Rows[i].Cells["PCode"].Value.ToString();
                    receiveDetailVM.ItemName = dgvReceive.Rows[i].Cells["ItemName"].Value.ToString();
                    receiveDetailVM.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    receiveDetailVM.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    receiveDetailVM.NBRPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["NBRPrice"].Value.ToString());
                    receiveDetailVM.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    receiveDetailVM.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    receiveDetailVM.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    receiveDetailVM.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    receiveDetailVM.CommentsD = Program.CurrentUser;

                    receiveDetailVM.SD = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SD"].Value.ToString());
                    receiveDetailVM.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());
                    receiveDetailVM.BOMId = Convert.ToInt32(string.IsNullOrWhiteSpace(dgvReceive.Rows[i].Cells["BOMId"].Value.ToString()) ? 0 : dgvReceive.Rows[i].Cells["BOMId"].Value);
                    receiveDetailVM.UOMn = dgvReceive.Rows[i].Cells["UOMn"].Value.ToString();
                    receiveDetailVM.UOMPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMPrice"].Value.ToString());
                    receiveDetailVM.UOMQty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString());
                    receiveDetailVM.UOMc = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMc"].Value.ToString());
                    receiveDetailVM.VatName = dgvReceive.Rows[i].Cells["VATName"].Value.ToString();
                    if (rbtnReceiveReturn.Checked)
                    {
                        receiveDetailVM.ReturnTransactionType = dgvReceive.Rows[i].Cells["ReturnTransactionType"].Value.ToString();
                    }
                    receiveDetailVM.BranchId = Program.BranchId;
                    receiveDetailVMs.Add(receiveDetailVM);
                }

                if (receiveDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                #endregion

                #region Tracking

                if (TrackingTrace == true)
                {
                    for (int i = 0; i < dgvReceive.Rows.Count; i++)
                    {
                        string itemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());

                        var p = from productCmb in trackingsVm.ToList()
                                where productCmb.FinishItemNo == itemNo
                                select productCmb;

                        if (p != null && p.Any())
                        {
                            var trackingInfo = p.First();
                            decimal fQty = trackingInfo.Quantity;

                            if (Quantity > fQty || Quantity < fQty)
                            {
                                MessageBox.Show("Please insert correct number of tracking Qty.", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                }
                #endregion

                #endregion

                #region Element Stats

                this.btnUpdate.Enabled = false;

                this.progressBar1.Visible = true;

                #endregion

                #region Update / Background Workder

                bgwUpdate.RunWorkerAsync();

                #endregion
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
            #endregion
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.ReceiveUpdate(receiveMasterVM, receiveDetailVMs, trackingsVm, connVM,null,null,Program.CurrentUserID);
                UPDATE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }

            #endregion

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();


                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvReceive.RowCount; i++)
                            {
                                dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
                            }
                        }
                    }
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try

            try
            {

                #region Checkpoint

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId)
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

                if (
                    MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text,
                                    MessageBoxButtons.YesNo,
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
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Set Master

                receiveMasterVM = new ReceiveMasterVM();
                receiveMasterVM.ShiftId = cmbShift.SelectedValue.ToString(); //Post

                receiveMasterVM.ReceiveNo = NextID;
                receiveMasterVM.CustomerID = vCustomerID;

                receiveMasterVM.ReceiveDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// +DateTime.Now.ToString(" HH:mm:ss"); //dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                receiveMasterVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                receiveMasterVM.SerialNo = txtSerialNo.Text.Trim();
                receiveMasterVM.ReferenceNo = txtRefNo.Text;
                receiveMasterVM.Comments = txtComments.Text.Trim();
                receiveMasterVM.CreatedBy = Program.CurrentUser;
                receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.LastModifiedBy = Program.CurrentUser;
                receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.transactionType = transactionType;
                receiveMasterVM.WithToll = chkWithToll.Checked == true ? "Y" : "N"; ;
                receiveMasterVM.Post = "Y";
                receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
                //receiveMasterVM.VatName = cmbVAT1Name.Text;
                receiveMasterVM.ReturnId = txtReceiveNoPre.Text.Trim();

                //encriptedReceiveHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, ReceiveHeaderData);
                #endregion

                #region Set Details

                receiveDetailVMs = new List<ReceiveDetailVM>();

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {

                    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();

                    //receiveDetailVM.ReceiveLineNo = NextID;
                    receiveDetailVM.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    receiveDetailVM.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    receiveDetailVM.ItemCode = dgvReceive.Rows[i].Cells["PCode"].Value.ToString();
                    receiveDetailVM.ItemName = dgvReceive.Rows[i].Cells["ItemName"].Value.ToString();
                    receiveDetailVM.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    receiveDetailVM.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    receiveDetailVM.NBRPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["NBRPrice"].Value.ToString());
                    receiveDetailVM.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    receiveDetailVM.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    receiveDetailVM.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    receiveDetailVM.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    receiveDetailVM.CommentsD = Program.CurrentUser;

                    receiveDetailVM.SD = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SD"].Value.ToString());
                    receiveDetailVM.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());

                    if (!string.IsNullOrEmpty(dgvReceive.Rows[i].Cells["BOMId"].Value.ToString()))
                    {
                        receiveDetailVM.BOMId = Convert.ToInt32(dgvReceive.Rows[i].Cells["BOMId"].Value);
                    }
                    else
                    {
                        receiveDetailVM.BOMId = 0;
                    }

                    receiveDetailVM.VatName = dgvReceive.Rows[i].Cells["VATName"].Value.ToString();


                    receiveDetailVMs.Add(receiveDetailVM);

                } //End For

                if (receiveDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #region Post Background Workder

                backgroundWorkerPost.RunWorkerAsync();

                #endregion

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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }

            #endregion Catch

        }

        private void backgroundWorkerPost_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement

                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.ReceivePost(receiveMasterVM, receiveDetailVMs, trackingsVm, null, null, connVM,Program.CurrentUserID);
                POST_DOWORK_SUCCESS = true;

                //End DoWork

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
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region try

            try
            {
                if (POST_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerPost_RunWorkerCompleted",
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();


                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvReceive.RowCount; i++)
                            {
                                dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
                            }
                        }
                    }

                ChangeData = false;
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
            }

            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void btnSearchReceiveNo_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region selecte Row

                DataGridViewRow selectedRow = null;

                selectedRow = FormReceiveSearch.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtReceiveNo.Text = selectedRow.Cells["ReceiveNo"].Value.ToString();
                    txtHiddenInvoiceNo.Text = selectedRow.Cells["ReceiveNo"].Value.ToString();

                    SearchInvoice();
                }

                #endregion

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
                FileLogger.Log(this.Name, "btnSearchReceiveNo_Click", exMessage);
            }

            #endregion Catch

        }

        private void SearchInvoice()
        {

            try
            {

                #region Data Call

                IReceive rDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                DataTable dt = new DataTable();
                dt = rDal.SearchReceiveHeaderDTNew(txtReceiveNo.Text,connVM);

                #endregion

                #region Value Assign to Form Elements

                DataRow dr = dt.Rows[0];

                cmbShift.SelectedValue = dr["ShiftId"];

                #region Conditional Values

                string varIssueFromBOM = Convert.ToString(dr["IssueFromBOM"]);

                if (varIssueFromBOM.ToLower() == "n")
                {
                    varIssueFromBOM = vIssueFromBOM;
                }

                #endregion

                txtId.Text = dr["Id"].ToString();
                txtFiscalYear.Text = dr["FiscalYear"].ToString();

                chkIssueFromBOM.Checked = varIssueFromBOM == "Y" ? true : false;
                dtpReceiveDate.Value = Convert.ToDateTime(dr["ReceiveDateTime"]);
                txtTotalAmount.Text = Program.ParseDecimalObject(dr["TotalAmount"].ToString()).ToString();//"0,0.00");
                txtTotalVATAmount.Text = Program.ParseDecimalObject(dr["TotalVATAmount"].ToString()).ToString();//"0,0.00");
                txtSerialNo.Text = dr["SerialNo"].ToString();
                txtRefNo.Text = dr["ReferenceNo"].ToString();
                txtComments.Text = dr["Comments"].ToString();
                IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                txtReceiveNoPre.Text = dr["ReturnId"].ToString();
                ImportExcelID = dr["ImportIDExcel"].ToString();
                chkWithToll.Checked = Convert.ToString(dr["WithToll"].ToString()) == "Y" ? true : false;
                txtCustomer.Text = dr["CustomerName"].ToString();
                vCustomerID = dr["CustomerID"].ToString();
                SearchBranchId = Convert.ToInt32(dr["BranchId"].ToString());

                #endregion

                #region Conditional Values

                if (txtReceiveNo.Text == "")
                {
                    ReceiveDetailData = "0";
                }
                else
                {
                    ReceiveDetailData = txtReceiveNo.Text.Trim();
                }

                #endregion

                #region Tracking

                InsertTrackingInfo();

                #endregion

                #region Button Stats

                this.btnFirst.Enabled = false;
                this.btnPrevious.Enabled = false;
                this.btnNext.Enabled = false;
                this.btnLast.Enabled = false;

                this.btnSearchReceiveNo.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker

                backgroundWorkerReceiveBasicSearch.RunWorkerAsync();

                #endregion

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
                FileLogger.Log(this.Name, "SearchInvoice", exMessage);
            }

            #endregion Catch
        }


        private void backgroundWorkerReceiveBasicSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReceiveDetailResult = new DataTable();
                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                ReceiveDetailResult = receiveDal.SearchReceiveDetailNew(txtReceiveNo.Text.Trim(), Program.DatabaseName, connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerReceiveBasicSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                dgvReceive.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ReceiveDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceive.Rows.Add(NewRow);
                    //dgvReceive.Rows[j].Cells["PurchaseInvoiceNo"].Value = PurchaseDetailFields[0].ToString();
                    dgvReceive.Rows[j].Cells["LineNo"].Value = item["ReceiveLineNo"].ToString();
                    dgvReceive.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();

                    dgvReceive.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvReceive.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["CostPrice"].ToString());
                    dgvReceive.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["NBRPrice"].ToString());

                    dgvReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();

                    dgvReceive.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    dgvReceive.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    dgvReceive.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());

                    dgvReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    dgvReceive.Rows[j].Cells["Status"].Value = "Old";

                    dgvReceive.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvReceive.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item["Stock"].ToString());
                    dgvReceive.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    dgvReceive.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());

                    dgvReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvReceive.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();

                    dgvReceive.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                    dgvReceive.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());

                    dgvReceive.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();

                    dgvReceive.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item["UOMc"].ToString());

                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvReceive.Rows[j].Cells["VATName"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvReceive.Rows[j].Cells["VATName"].Value = item["VATName"].ToString();

                    }


                    dgvReceive.Rows[j].Cells["Change"].Value = 0;
                    dgvReceive.Rows[j].Cells["ReturnTransactionType"].Value = item["ReturnTransactionType"].ToString();
                    j = j + 1;

                }
                Rowcalculate();
                //btnSave.Text = "&Save";
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;

                #region Button Stats

                this.btnFirst.Enabled = true;
                this.btnPrevious.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnLast.Enabled = true;

                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;

                #endregion
            }

        }


        #endregion

        #region Methods 04

        private void ReceiveSingle() // issue with BOM
        {

            try
            {
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtHiddenInvoiceNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please enter product and quantity");
                    return;
                }

                //new ReceiveMasterVM();

                receiveMasterVM = new ReceiveMasterVM();

                receiveMasterVM.ReceiveNo = NextID;
                receiveMasterVM.ReceiveDateTime =
                        dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                                           DateTime.Now.ToString("HH:mm:ss");
                receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                receiveMasterVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                receiveMasterVM.SerialNo = txtSerialNo.Text.Trim();
                receiveMasterVM.Comments = txtComments.Text.Trim();
                receiveMasterVM.CreatedBy = Program.CurrentUser;
                receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.LastModifiedBy = Program.CurrentUser;
                receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                receiveMasterVM.transactionType = "Other";
                receiveMasterVM.Post = "N";
                receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";

                //encriptedReceiveHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, ReceiveHeaderData);

                receiveDetailVMs = new List<ReceiveDetailVM>();

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {


                    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();

                    //receiveDetailVM.ReceiveLineNo = NextID;
                    receiveDetailVM.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    receiveDetailVM.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    receiveDetailVM.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    receiveDetailVM.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    receiveDetailVM.NBRPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["NBRPrice"].Value.ToString());
                    receiveDetailVM.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    receiveDetailVM.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    receiveDetailVM.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    receiveDetailVM.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    receiveDetailVM.CommentsD = Program.CurrentUser;

                    receiveDetailVM.SD = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SD"].Value.ToString());
                    receiveDetailVM.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());


                    receiveDetailVMs.Add(receiveDetailVM);
                }



                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSave.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "ReceiveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReceiveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReceiveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReceiveSingle", exMessage);
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
                FileLogger.Log(this.Name, "ReceiveSingle", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveSingle", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReceiveSingle", exMessage);
            }

            #endregion Catch
        }

        private void ReceiveSinglePost()
        {

            try
            {
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtHiddenInvoiceNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please enter product and quantity");
                    return;
                }

                new ReceiveMasterVM();


                receiveMasterVM.ReceiveNo = NextID;
                receiveMasterVM.ReceiveDateTime =
                    dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString("HH:mm:ss");
                receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                receiveMasterVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                receiveMasterVM.SerialNo = txtSerialNo.Text.Trim();
                receiveMasterVM.Comments = txtComments.Text.Trim();
                receiveMasterVM.CreatedBy = Program.CurrentUser;
                receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                receiveMasterVM.LastModifiedBy = Program.CurrentUser;
                receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                receiveMasterVM.transactionType = "Other";
                receiveMasterVM.Post = "Y";
                receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";

                //encriptedReceiveHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, ReceiveHeaderData);

                receiveDetailVMs = new List<ReceiveDetailVM>();

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {

                    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();

                    //receiveDetailVM.ReceiveLineNo = NextID;
                    receiveDetailVM.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    receiveDetailVM.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    receiveDetailVM.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    receiveDetailVM.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    receiveDetailVM.NBRPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["NBRPrice"].Value.ToString());
                    receiveDetailVM.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    receiveDetailVM.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    receiveDetailVM.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    receiveDetailVM.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    receiveDetailVM.CommentsD = Program.CurrentUser;

                    receiveDetailVM.SD = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SD"].Value.ToString());
                    receiveDetailVM.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());

                    receiveDetailVMs.Add(receiveDetailVM);

                } //End For



                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerPost.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ReceiveSinglePost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReceiveSinglePost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReceiveSinglePost", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReceiveSinglePost", exMessage);
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
                FileLogger.Log(this.Name, "ReceiveSinglePost", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveSinglePost", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveSinglePost", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReceiveSinglePost", exMessage);
            }

            #endregion Catch
        }

        private void ClearAllFields()
        {
            try
            {

                txtId.Text = "0";
                SearchBranchId = 0;
                txtFiscalYear.Text = "0";
                txtHiddenInvoiceNo.Text = "";
                txtReceiveNoPre.Text = "";
                //cmbProductType.Text = "Select";
                txtQuantityInHand.Text = "0.0";
                txtComments.Text = "";
                txtCommentsDetail.Text = "";
                txtHSCode.Text = "";
                txtIsRaw.Text = "";
                txtNBRPrice.Text = "0.00";
                txtProductCode.Text = "";
                txtProductName.Text = "";
                txtQuantity.Text = "0.00";
                txtRefNo.Text = "";
                txtSerialNo.Text = "";
                txtTotalAmount.Text = "0.00";
                txtTotalVATAmount.Text = "0.00";
                txtUnitCost.Text = "0.00";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtSD.Text = "0.00";
                txtPCode.Text = "";

                string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate",null,connVM);
                if (AutoSessionDate.ToLower() != "y")
                {
                    dtpReceiveDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

                }
                else
                {
                    dtpReceiveDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                }

                dgvReceive.Rows.Clear();
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

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtUnitCost, "Unit Cost");
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();

            txtNBRPrice.Text = txtUnitCost.Text;
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {

            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
            //Program.FormatTextBoxQty4(txtQuantity, "Quantity");
            //if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            //{
            //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();

            //}
        }

        private void dgvReceive_DoubleClick(object sender, EventArgs e)
        {
            unitCost = 0;
            try
            {
                if (dgvReceive.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #region Value Assign to Form Elements

                cmbProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString() + "~" +
                                                    dgvReceive.CurrentRow.Cells["PCode"].Value.ToString();

                //txtLineNo.Text = dgvReceive.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtBOMId.Text = dgvReceive.CurrentRow.Cells["BOMId"].Value.ToString();
                txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                txtPCode.Text = dgvReceive.CurrentRow.Cells["PCode"].Value.ToString();
                vItemNo = txtProductCode.Text.Trim();
                txtUOM.Text = dgvReceive.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());
                Uoms();
                cmbUom.Text = dgvReceive.CurrentRow.Cells["UOM"].Value.ToString();

                txtQuantity.Text = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                unitCost = Convert.ToDecimal(Convert.ToDecimal(dgvReceive.CurrentRow.Cells["UnitPrice"].Value).ToString());//"0,0.00"));
                txtUnitCost.Text = Convert.ToDecimal(Convert.ToDecimal(dgvReceive.CurrentRow.Cells["UOMPrice"].Value)).ToString();//"0,0.00"));

                txtVATRate.Text = "0.00";
                //Convert.ToDecimal(dgvReceive.CurrentRow.Cells["VATRate"].Value).ToString();//"0.00");
                txtComments.Text = dgvReceive.CurrentRow.Cells["Comments"].Value.ToString();
                txtNBRPrice.Text = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["NBRPrice"].Value).ToString();//"0,0.00");
                txtSD.Text = "0.00"; // Convert.ToDecimal(dgvReceive.CurrentRow.Cells["SD"].Value).ToString();//"0.00");
                //txtQuantityInHand.Text =Convert.ToDecimal(dgvReceive.CurrentRow.Cells["Stock"].Value).ToString();//"0,0.00000");
                txtPrevious.Text = Program.ParseDecimalObject(dgvReceive.CurrentRow.Cells["Previous"].Value).ToString();//"0,0.0000");
                cmbVAT1Name.Text = dgvReceive.CurrentRow.Cells["VATName"].Value.ToString();

                #endregion

                #region Stock and PriceCall

                //PriceCall();

                //ProductDAL productDal = new ProductDAL();


                ProductDAL productDal = new ProductDAL();

                if (string.Equals(transactionType, "TollFinishReceive", StringComparison.OrdinalIgnoreCase))
                {
                    var dtTollStock = productDal.GetTollStock(new ParameterVM()
                    {
                        ItemNo = txtProductCode.Text,
                        BranchId = Program.BranchId
                    });

                    txtQuantityInHand.Text = dtTollStock.Rows[0]["Stock"].ToString();


                }
                else
                {

                    DataTable QuantityInHand = productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                        DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM, Program.CurrentUserID);

                    if (QuantityInHand != null && QuantityInHand.Rows.Count > 0)
                    {
                        txtQuantityInHand.Text = QuantityInHand.Rows[0]["Quantity"].ToString();

                    }

                }

                #endregion

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
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", exMessage);
            }

            #endregion Catch


        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                FormRptReceiveInformation frmRptReceiveInformation = new FormRptReceiveInformation();
                frmRptReceiveInformation.Show();
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

            #endregion Catch
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }



        private void dgvReceive_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void cmbProduct_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormProductCategorySearch frm = new FormProductCategorySearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                    //string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                    //CategoryId = ProductCategoryInfo[0];
                    //txtCategoryName.Text = ProductCategoryInfo[1];
                    //cmbProductType.Text = ProductCategoryInfo[4];
                }
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];
                ProductSearchDsFormLoad();
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

            #endregion Catch
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtReceiveNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpReceiveDate_ValueChanged(object sender, EventArgs e)
        {
            dtpBOMDate.Value = dtpReceiveDate.Value;
            ChangeData = true;
        }

        private void txtHSCode_TextChanged(object sender, EventArgs e)
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

        private void txtUOM_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 05

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

        private void txtCommentsDetail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantityInHand_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalVATAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormReceive_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormReceive_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormReceive_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormReceive_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormReceive_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormReceive_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormReceive_FormClosing", exMessage);
            }

            #endregion Catch
        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtSD, "SD Rate");
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpReceiveDate_KeyDown(object sender, KeyEventArgs e)
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

                CommonDAL commonDal = new CommonDAL();

                #region VAT Name

                VATName vname = new VATName();
                cmbVAT1Name.DataSource = vname.VATNameList;

                #endregion

                #region Reset Fields

                ClearAllFields();

                txtReceiveNo.Text = "~~~ New ~~~";

                #endregion

                #region Flag Update

                ChangeData = false;

                #endregion

                #region Settings Value

                vCustomerWiseBOM = commonDal.settingsDesktop("Receive", "CustomerWiseBOM",null,connVM);
                vReceivePlaceQty = commonDal.settingsDesktop("Receive", "Quantity",null,connVM);
                vReceivePlaceAmt = commonDal.settingsDesktop("Receive", "Amount",null,connVM);
                vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM",null,connVM);

                vLocalInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3",null,connVM);
                vLocalInVAT1KaTarrif = commonDal.settingsDesktop("PriceDeclaration", "LocalInVAT4_3Ka(Tarrif)",null,connVM);
                vTenderInVAT1 = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3",null,connVM);
                vTenderInVAT1Tender = commonDal.settingsDesktop("PriceDeclaration", "TenderInVAT4_3(Tender)",null,connVM);
                vImportPrice = commonDal.settingsDesktop("Receive", "PriceDeclarationForImport",null,connVM);
                vChangeableNBRPrice = commonDal.settingsDesktop("Production", "ChangeableNBRPrice",null,connVM);
                ChangeableNBRPrice = Convert.ToBoolean(vChangeableNBRPrice == "Y" ? true : false);

                if (string.IsNullOrEmpty(vReceivePlaceQty)
                    || string.IsNullOrEmpty(vReceivePlaceAmt)
                    || string.IsNullOrEmpty(vIssueFromBOM)
                    || string.IsNullOrEmpty(vLocalInVAT1)
                    || string.IsNullOrEmpty(vLocalInVAT1KaTarrif)
                    || string.IsNullOrEmpty(vTenderInVAT1)
                    || string.IsNullOrEmpty(vTenderInVAT1Tender)
                    || string.IsNullOrEmpty(vImportPrice)
                    || string.IsNullOrEmpty(vCustomerWiseBOM)
                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                CustomerWiseBOM = Convert.ToBoolean(vCustomerWiseBOM.ToString() == "Y" ? true : false);

                ReceivePlaceQty = Convert.ToInt32(vReceivePlaceQty);
                ReceivePlaceAmt = Convert.ToInt32(vReceivePlaceAmt);
                IssueFromBOM = Convert.ToBoolean(vIssueFromBOM.ToString() == "Y" ? true : false);
                LocalInVAT1 = Convert.ToBoolean(vLocalInVAT1 == "Y" ? true : false);
                LocalInVAT1KaTarrif = Convert.ToBoolean(vLocalInVAT1KaTarrif == "Y" ? true : false);
                TenderInVAT1 = Convert.ToBoolean(vTenderInVAT1 == "Y" ? true : false);
                TenderInVAT1Tender = Convert.ToBoolean(vTenderInVAT1Tender == "Y" ? true : false);
                PriceDeclarationForImport = Convert.ToBoolean(vImportPrice == "Y" ? true : false);

                chkIssueFromBOM.Checked = IssueFromBOM;

                #endregion

                #region Conditional Values


                if (IssueFromBOM == true)
                {
                    btnVAT16.Visible = true;
                    chkPercent.Checked = true;
                }
                else
                {
                    btnVAT16.Visible = false;

                    chkPercent.Checked = false;
                }

                #endregion

                #region Tracking

                string vTracking = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking",null,connVM);
                if (string.IsNullOrEmpty(vTracking))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

                #endregion

                #region Flag Update

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Button Sats

                btnVAT17.Visible = true;

                #endregion

                #region Flag Update

                IsPost = false;

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
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }

            #endregion Catch
            finally
            {
                ChangeData = false;
            }

        }

        private void btnAddNew_MouseHover(object sender, EventArgs e)
        {

        }



        #endregion

        #region Methods 06

        private void btnVAT17_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi1 = new MDIMainInterface();
                FormRptVAT17 frmRptVAT17 = new FormRptVAT17();


                //mdi1.RollDetailsInfo("8301");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvReceive.Rows.Count > 0)
                {
                    frmRptVAT17.txtItemNo.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT17.txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT17.txtUOM.Text = dgvReceive.CurrentRow.Cells["UOMn"].Value.ToString();
                    frmRptVAT17.dtpToDate.Value = dtpReceiveDate.Value;
                    frmRptVAT17.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT17.rbtnService.Checked = false;
                }

                frmRptVAT17.ShowDialog();
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

            #endregion Catch
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                TransactionTypes();
                FormRptReceiveInformation frmRptReceiveInformation = new FormRptReceiveInformation();


                if (txtReceiveNo.Text == "~~~ New ~~~")
                {
                    frmRptReceiveInformation.txtReceiveNo.Text = "";

                }
                else
                {
                    frmRptReceiveInformation.txtReceiveNo.Text = txtReceiveNo.Text.Trim();

                }
                frmRptReceiveInformation.txtTransactionType.Text = transactionType;
                frmRptReceiveInformation.ShowDialog();
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
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
            }

            #endregion Catch
        }

        private void btnVAT18_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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
                frmRptVAT18.dtpToDate.Value = dtpReceiveDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpReceiveDate.Value;
                frmRptVAT18.ShowDialog();
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

            #endregion Catch
        }

        private void chkPCode_CheckedChanged(object sender, EventArgs e)
        {

            if (rbtnTender.Checked)
            {
                ProductSearchDsLoadTender();
            }
            else
            {
                ProductSearchDsLoad();
            }
        }

        private void cmbProduct_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void PriceCall()
        {
            decimal lastDeclaredPrice = 0;
            decimal tenderprice = 0;

            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


            if (string.IsNullOrEmpty(cmbVAT1Name.Text.Trim()))
            {
                MessageBox.Show("Please Select Price Declaration", this.Text);
            }

            var prodByCode = from prd in ProductsMini.ToList()
                             where prd.ProductCode.ToLower() == txtProductCode.Text.Trim().ToLower()
                             select prd;
            if (prodByCode != null && prodByCode.Any())
            {
                var products = prodByCode.First();

                tenderprice = products.NBRPrice;

            }


            #region BOM - NBRPrice

            DataTable dt = new DataTable();
            string productCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());
            string vAT1Name = OrdinaryVATDesktop.SanitizeInput(cmbVAT1Name.Text.Trim());

            if (CustomerWiseBOM)
            {
                

                dt = productDal.GetBOMReferenceNo(productCode, vAT1Name,dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID, connVM);

            }
            else
            {
                dt = productDal.GetBOMReferenceNo(productCode, vAT1Name,
                                              dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, "", connVM);

            }




            int BOMId = 0;
            decimal NBRPrice = 0;

            if (dt != null && dt.Rows.Count > 0)
            {
                #region ReferenceNo

                cmbBOMReferenceNo.DataSource = dt;
                cmbBOMReferenceNo.DisplayMember = "ReferenceNo";
                cmbBOMReferenceNo.ValueMember = "ReferenceNo";

                cmbBOMReferenceNo.SelectedIndex = 0;

                #endregion

                #region BOMId and NBRPrice

                DataRow dr = dt.Rows[0];
                string tempBOMId = dr["BOMId"].ToString();
                string tempNBRPrice = dr["NBRPrice"].ToString();
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



            var tt = transactionType;

            lastDeclaredPrice = NBRPrice;

            #region Comments

            ////////if (rbtnTollFinishReceive.Checked)
            ////////{

            ////////    lastDeclaredPrice = productDal.GetLastTollChargeFBOMOH(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim()
            ////////        , dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);
            ////////}
            ////////else
            //////if (CustomerWiseBOM)
            //////{
            //////    lastDeclaredPrice = NBRPrice;////// productDal.GetLastNBRPriceFromBOM(txtProductCode.Text.Trim(),
            //////    //////cmbVAT1Name.Text.Trim(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID);

            //////}
            //////else
            //////{
            //////    lastDeclaredPrice = NBRPrice; ////// productDal.GetLastNBRPriceFromBOM(txtProductCode.Text.Trim(),
            //////    ////// cmbVAT1Name.Text.Trim(),dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);
            //////}

            #endregion


            #region Value Assign

            if (rbtnTender.Checked)
            {
                txtUnitCost.Text = tenderprice.ToString();
            }
            else if (rbtnReceiveReturn.Checked)
            {
                txtUnitCost.Text = unitCost.ToString();
            }
            else
            {
                //if (string.IsNullOrEmpty(ImportExcelID))
                //{
                //    txtUnitCost.Text = lastDeclaredPrice.ToString();
                //}
                //else
                //{
                //    txtUnitCost.Text = unitCost.ToString();
                //}

                txtUnitCost.Text = lastDeclaredPrice.ToString();


            }
            if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
            {
                txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();

            }
            else
            {

                return;
            }
            #endregion


            // if (rbtnTender.Checked)
            //{
            //    lastDeclaredPrice = productDal.GetLastNBRPriceFromBOMId(txtBOMId.Text.Trim(), null, null);

            //}


        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void dgvReceive_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //Rowcalculate();
        }

        private void chkPercent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (IssueFromBOM == true)
                {
                    if (chkPercent.Checked)
                    {
                        txtPercent.ReadOnly = false;
                    }
                    else
                    {
                        txtPercent.ReadOnly = true;
                        txtPercent.Text = "100";
                    }

                    //return;
                }
                else
                {
                    MessageBox.Show("Please select issue from BOM in setup.");
                }
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
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkPercent_CheckedChanged", exMessage);
            }

            #endregion Catch


        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dgvReceive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F2))
            {
                UseQty();
            }
        }

        private void UseQty()
        {
            try
            {

                varFinishItemNo = dgvReceive["ItemNo", dgvReceive.CurrentRow.Index].Value.ToString();
                varQuantity = Convert.ToDecimal(dgvReceive["Quantity", dgvReceive.CurrentRow.Index].Value);
                varEffectDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd");

                backgroundWorkerUseQty.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "UseQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UseQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UseQty", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "UseQty", exMessage);
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
                FileLogger.Log(this.Name, "UseQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UseQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UseQty", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UseQty", exMessage);
            }

            #endregion Catch
        }

        private void dgvUseQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F1))
            {
                dgvUseQty.Visible = false;
            }
        }

        private void txtPercent_TextChanged(object sender, EventArgs e)
        {

        }

        #region backgroundWorkerSearch Event


        private void backgroundWorkerUseQty_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                ResultUseQty = new DataTable();
                //BOMDAL bomdal = new BOMDAL();
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);


                ResultUseQty = bomdal.UseQuantityDT(varFinishItemNo, varQuantity, varEffectDate, "", connVM);
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerUseQty_RunWorkerCompleted(object sender,
                                                               System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                dgvUseQty.DataSource = ResultUseQty;
                dgvUseQty.Top = dgvReceive.Top;
                dgvUseQty.Left = dgvReceive.Left;
                dgvUseQty.Height = dgvReceive.Height;
                dgvUseQty.Width = dgvReceive.Width;
                dgvUseQty.Visible = true;

                DataGridViewCellStyle red = dgvUseQty.DefaultCellStyle.Clone();
                red.BackColor = Color.Red;

                foreach (DataGridViewRow r in dgvUseQty.Rows)
                {
                    if (Convert.ToDecimal(r.Cells["Rest"].Value) < 0)
                    {
                        r.DefaultCellStyle = red;

                    }
                } //End foreach
                //End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerUseQty_RunWorkerCompleted", exMessage);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Methods 07

        private void dgvUseQty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                if (dgvReceive.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpReceiveDate.Value;
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
                string err = ex.Message.ToString();
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

        private void btnTender_Click(object sender, EventArgs e)
        {
            Program.fromOpen = "Me";
            DataGridViewRow selectedRow = new DataGridViewRow();
            selectedRow = FormTenderSearch.SelectOne();
            if (selectedRow != null && selectedRow.Selected == true)
            {
                txtTenderId.Text = selectedRow.Cells["TenderId"].Value.ToString();
                bgwTenderSearch.RunWorkerAsync();
            }
            //result = FormTenderSearch.SelectOne();

            //if (result == "")
            //{
            //    return;
            //}
            //else
            //{
            //    string[] Tenderinfo = result.Split(FieldDelimeter.ToCharArray());
            //    //Thread.Sleep(100);
            //    if (Tenderinfo != null && Tenderinfo.Length > 0)
            //    {


            //        txtTenderId.Text = Tenderinfo[0];
            //        bgwTenderSearch.RunWorkerAsync();
            //    }

            //}
        }

        private void bgwTenderSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region

                TenderResult = new DataTable();
                //TenderDAL tenderDal = new TenderDAL();
                ITender tenderDal = OrdinaryVATDesktop.GetObject<TenderDAL, TenderRepo, ITender>(OrdinaryVATDesktop.IsWCF);

                string tenderId = OrdinaryVATDesktop.SanitizeInput(txtTenderId.Text.Trim());

                TenderResult = tenderDal.SearchTenderDetail(tenderId, dtpReceiveDate.Value.ToString("yyyy/MMM/dd"), connVM);

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

        private void bgwTenderSearchh_RunWorkerCompleted(object sender,
                                                         System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                ProductsMini.Clear();
                // Start Complete
                foreach (DataRow item2 in TenderResult.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    prod.CategoryID = item2["CategoryID"].ToString();
                    prod.CategoryName = item2["CategoryName"].ToString();
                    prod.UOM = item2["UOM"].ToString();
                    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                    prod.IsRaw = item2["IsRaw"].ToString();
                    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    prod.BOMId = item2["BOMId"].ToString();

                    ProductsMini.Add(prod);
                }
                ProductSearchDsLoad();

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
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "bgwTenderSearchh_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.btnSearchReceiveNo.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void cmbVAT1Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbVAT1Name_Leave(object sender, EventArgs e)
        {
            //if (cmbVAT1Name.Text.Trim() == "VAT 1 Ga (Export)")
            //{
            //    txtUnitCost.ReadOnly = false;
            //}
            //else
            //{
            //    txtUnitCost.ReadOnly = true;

            //}


        }


        private void txtBOMId_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxQty(txtBOMId, "BOMId");
        }

        private void chkTender_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTender.Checked)
            {
                rbtnTender.Checked = true;
                btnTender.Visible = true;
                btnProductType.Visible = false;
                ProductSearchDsLoadTender();
                chkTender.Text = "Tender";
                //cmbVAT1Name.Items.Clear();
                //cmbVAT1Name.Items.Add("VAT 4.3 (Tender)");
                cmbVAT1Name.SelectedIndex = 8;

                //cmbVAT1Name.Text = "VAT 4.3 (Tender)";
                cmbVAT1Name.Enabled = false;

            }
            else
            {
                rbtnOther.Checked = true;
                btnProductType.Visible = true;
                btnTender.Visible = false;
                ProductSearchDsLoad();
                chkTender.Text = "Other";
                //cmbVAT1Name.Text = "VAT 1";
                cmbVAT1Name.SelectedIndex = 0;
                cmbVAT1Name.Enabled = true;


            }
        }

        private void cmbVAT1Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpReceiveDate_Leave(object sender, EventArgs e)
        {
            dtpReceiveDate.Value = Program.ParseDate(dtpReceiveDate);
        }

        private void txtTenderId_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtBOMId_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement


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

        #endregion

        #region Methods 08

        private void cmbProductName_Leave(object sender, EventArgs e)
        {
            decimal stockInHand = 0;

            try
            {
                //if (cmbProductName.SelectedIndex != -1)
                //{
                var searchText = cmbProductName.Text.Trim().ToLower();

                string ProductCode = "";
                if (searchText.Contains('~'))
                {
                    ProductCode = searchText.Split('~')[1];
                }

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    #region Value Assign

                    var prodByName = from prd in ProductsMini.ToList()
                                     //where prd.ProductName.ToLower() == searchText.ToLower()
                                     where prd.ProductCode.ToLower() == ProductCode.ToLower()
                                     select prd;

                    if (prodByName != null && prodByName.Any())
                    {
                        var products = prodByName.First();
                        txtProductName.Text = products.ProductName;
                        txtProductCode.Text = products.ItemNo;
                        //lastDeclaredPrice = products.NBRPrice;
                        txtUOM.Text = products.UOM;
                        txtHSCode.Text = products.HSCodeNo;
                        txtPCode.Text = products.ProductCode;
                        txtBOMId.Text = products.BOMId;
                        txtUnitCost.Text = products.NBRPrice.ToString();
                        cmbUom.Text = products.UOM;
                        vItemNo = txtProductCode.Text;

                    }

                    var TBomId = from tn in tenderDetails.ToList()
                                 where tn.ItemNo.ToLower() == txtProductCode.Text.Trim()
                                 select tn;
                    string BomId = string.Empty;
                    if (TBomId != null && TBomId.Any())
                    {
                        txtBOMId.Text = string.Empty;
                        var tnd = TBomId.First();
                        txtBOMId.Text = tnd.BOMId;
                    }

                    #endregion

                    #region Stock / PriceCall


                    //ProductDAL productDal = new ProductDAL();
                    ProductDAL productDal = new ProductDAL();

                    if (string.Equals(transactionType, "TollFinishReceive", StringComparison.OrdinalIgnoreCase))
                    {
                        var dtTollStock = productDal.GetTollStock(new ParameterVM()
                        {
                            ItemNo = txtProductCode.Text,
                            BranchId = Program.BranchId
                        });

                        stockInHand = Convert.ToDecimal(dtTollStock.Rows[0][0]);
                    }
                    else
                    {
                        stockInHand = Convert.ToDecimal(productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                            DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM, Program.CurrentUserID).Rows[0]["Quantity"].ToString());
                    }



                    if (chkTender.Checked == false)
                    {
                        PriceCall();
                    }
                    txtQuantity.Focus();

                    #endregion

                }

                //}
                Uoms();
                txtQuantityInHand.Text = "" + stockInHand;
                //txtQuantity.Focus();

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
                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
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

        private void cmbProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
                cmbProductName.Focus();
            }
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void btnPreviousSearch_Click(object sender, EventArgs e)
        {


            try
            {

                dgvReceivePrevious.Top = dgvReceive.Top;
                dgvReceivePrevious.Left = dgvReceive.Left;
                dgvReceivePrevious.Height = dgvReceive.Height;
                dgvReceivePrevious.Width = dgvReceive.Width;

                dgvReceive.Visible = false;
                dgvReceivePrevious.Visible = true;

                // Start Click
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = null;
                TransactionTypes();

                ////////selectedRow = FormReceiveSearch.SelectOne("All");
                selectedRow = FormReceiveSearch.SelectOne("Other");

                if (selectedRow != null && selectedRow.Selected == true)
                {



                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }


                    txtReceiveNoPre.Text = selectedRow.Cells["ReceiveNo"].Value.ToString(); // Receiveinfo[0];
                    //string ReceiveDetailData;
                    if (txtReceiveNo.Text == "")
                    {
                        ReceiveDetailData = "0";
                    }
                    else
                    {
                        ReceiveDetailData = txtReceiveNoPre.Text.Trim();


                    }
                    // End Click

                    //if (rbtnReceiveReturn.Checked)
                    //{
                    //    ReturnTransType = transactionType;
                    //}

                    this.btnSearchReceiveNo.Enabled = false;
                    this.progressBar1.Visible = true;

                    bgwPreviousTrans.RunWorkerAsync();
                }
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
                FileLogger.Log(this.Name, "btnPrevious_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrevious_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrevious_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrevious_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrevious_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrevious_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrevious_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrevious_Click", exMessage);
            }

            #endregion Catch

        }

        private void bgwPreviousTrans_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReceiveDetailResult = new DataTable();
                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);


                ReceiveDetailResult = receiveDal.SearchReceiveDetailNew(ReceiveDetailData, Program.DatabaseName, connVM);


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
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork",
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

                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork",
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
                FileLogger.Log(this.Name, "bgwPreviousTrans_DoWork", exMessage);
            }

            #endregion
        }

        private void bgwPreviousTrans_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                // Start Complete
                dgvReceivePrevious.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ReceiveDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceivePrevious.Rows.Add(NewRow);
                    dgvReceivePrevious.Rows[j].Cells["Select"].Selected = true;


                    dgvReceivePrevious.Rows[j].Cells["LineNoR"].Value = item["ReceiveLineNo"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["ItemNoR"].Value = item["ItemNo"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["QuantityR"].Value = item["Quantity"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["UnitPriceR"].Value = item["CostPrice"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["NBRPriceR"].Value = item["NBRPrice"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["UOMR"].Value = item["UOM"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["VATRateR"].Value = item["VATRate"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["VATAmountR"].Value = item["VATAmount"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["SubTotalR"].Value = item["SubTotal"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["CommentsR"].Value = item["Comments"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["ItemNameR"].Value = item["ProductName"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["SDR"].Value = item["SD"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["SDAmountR"].Value = item["SDAmount"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["PCodeR"].Value = item["ProductCode"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["BOMIdR"].Value = item["BOMId"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["StatusR"].Value = "Old";
                    //dgvReceivePrevious.Rows[j].Cells["PreviousR"].Value = item["Quantity"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["StockR"].Value = item["Stock"].ToString();

                    dgvReceivePrevious.Rows[j].Cells["UOMQtyR"].Value = item["UOMQty"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["UOMPriceR"].Value = item["UOMPrice"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["UOMnR"].Value = item["UOMn"].ToString();
                    dgvReceivePrevious.Rows[j].Cells["UOMcR"].Value = item["UOMc"].ToString();
                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvReceivePrevious.Rows[j].Cells["VATNameR"].Value = cmbVAT1Name.Text.Trim();

                    }
                    else
                    {
                        dgvReceivePrevious.Rows[j].Cells["VATNameR"].Value = item["VATName"].ToString();

                    }

                    //dgvReceivePrevious.Rows[j].Cells["VATNameR"].Value = item["VATName"].ToString();


                    dgvReceivePrevious.Rows[j].Cells["ChangeR"].Value = 0;
                    decimal returnQty = receiveDal.ReturnReceiveQty(ReceiveDetailData, item["ItemNo"].ToString(),connVM);
                    dgvReceivePrevious.Rows[j].Cells["RestQtyR"].Value = (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvReceivePrevious.Rows[j].Cells["PreviousR"].Value = (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();

                    if (rbtnReceiveReturn.Checked)
                    {
                        ReturnTransType = item["TransactionType"].ToString();
                    }



                    j = j + 1;

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPreviousTrans_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.btnSearchReceiveNo.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void dgvReceivePrevious_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F2))
            {
                dgvReceive.Rows.Clear();
                int j = 0;
                for (int i = 0; i < dgvReceivePrevious.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvReceivePrevious["Select", i].Value) == true)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvReceive.Rows.Add(NewRow);

                        dgvReceive.Rows[j].Cells["LineNo"].Value = j + 1;
                        dgvReceive.Rows[j].Cells["BOMId"].Value = dgvReceivePrevious["BOMIdR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["ItemNo"].Value = dgvReceivePrevious["ItemNoR", i].Value.ToString();
                        //dgvReceive.Rows[j].Cells["Quantity"].Value = dgvReceivePrevious["QuantityR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["Quantity"].Value = dgvReceivePrevious["RestQtyR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["UnitPrice"].Value = dgvReceivePrevious["UnitPriceR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["NBRPrice"].Value = dgvReceivePrevious["NBRPriceR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["UOM"].Value = dgvReceivePrevious["UOMR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["VATRate"].Value = dgvReceivePrevious["VATRateR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["VATAmount"].Value = dgvReceivePrevious["VATAmountR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["SubTotal"].Value = dgvReceivePrevious["SubTotalR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["Comments"].Value = dgvReceivePrevious["CommentsR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["ItemName"].Value = dgvReceivePrevious["ItemNameR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["Status"].Value = "Old";
                        //dgvReceive.Rows[j].Cells["Previous"].Value = dgvReceivePrevious["PreviousR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["Previous"].Value = dgvReceivePrevious["RestQtyR", i].Value.ToString();

                        dgvReceive.Rows[j].Cells["Stock"].Value = dgvReceivePrevious["StockR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["SD"].Value = dgvReceivePrevious["SDR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["SDAmount"].Value = dgvReceivePrevious["SDAmountR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["PCode"].Value = dgvReceivePrevious["PCodeR", i].Value.ToString();

                        dgvReceive.Rows[j].Cells["UOMQty"].Value = dgvReceivePrevious["UOMQtyR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["UOMPrice"].Value = dgvReceivePrevious["UOMPriceR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["UOMn"].Value = dgvReceivePrevious["UOMnR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["UOMc"].Value = dgvReceivePrevious["UOMcR", i].Value.ToString();
                        dgvReceive.Rows[j].Cells["VATName"].Value = dgvReceivePrevious["VATNameR", i].Value.ToString();

                        dgvReceive.Rows[j].Cells["Change"].Value = 0;
                        if (rbtnReceiveReturn.Checked)
                        {
                            dgvReceive.Rows[j].Cells["ReturnTransactionType"].Value = ReturnTransType;
                        }


                        j = j + 1;

                    }



                }
                dgvReceivePrevious.Visible = false;
                dgvReceive.Visible = true;
                Rowcalculate();
            }
        }

        private void dgvReceivePrevious_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvReceivePrevious["Select", e.RowIndex].Value = !Convert.ToBoolean(dgvReceivePrevious["Select", e.RowIndex].Value);
            }

        }

        private void btnBatchTrack_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //MDIMainInterface mdi = new MDIMainInterface();
                FormRptBatchTracking frmBatchTracking = new FormRptBatchTracking();


                //mdi.RollDetailsInfo("8401");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                frmBatchTracking.txtBatchNumber.Text = txtSerialNo.Text.Trim();
                frmBatchTracking.ShowDialog();
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
                FileLogger.Log(this.Name, "btnBatchTrack_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnBatchTrack_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnBatchTrack_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnBatchTrack_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnBatchTrack_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnBatchTrack_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnBatchTrack_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnBatchTrack_Click", exMessage);
            }

            #endregion Catch
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorkerSetupBOMIssue_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void dgvReceivePrevious_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            TransactionTypes();

            #region try

            try
            {


                string value = new CommonDAL().settingValue("CompanyCode", "Code",connVM);

                if (OrdinaryVATDesktop.IsACICompany(value))
                {
                    //FormPReceiveImportACI aci = new FormPReceiveImportACI();
                    //aci.preSelectTable = "Receives";
                    //aci.transactionType = transactionType;
                    //aci.Show();
                    string ImportId = FormPReceiveImportACI.SelectOne(transactionType, false);
                    //string ImportId = "REC-0006/0920~0520~38334";
                    ReceiveDAL _rlDal = new ReceiveDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _rlDal.SelectAll(0, new[] { "rh.ImportIDExcel" }, new[] { ImportId },null,null,null,false,connVM);
                        ReceiveDataLoad(dt);
                    }


                }
                else if (OrdinaryVATDesktop.IsNourishCompany(value))
                {
                  
                    string ImportId = FormReceiveImportNourish.SelectOne(transactionType);

                    ReceiveDAL _rlDal = new ReceiveDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _rlDal.SelectAll(0, new[] { "rh.ImportIDExcel" }, new[] { ImportId },null,null,null,false,connVM);
                        ReceiveDataLoad(dt);
                    }


                }

                else if (value == "SQR")
                {
                    string[] ids = FormReceiveImportSQR.SelectOne(transactionType).Split('~');
                    string ImportId = ids[0];

                    ////string ImportId = "01/23/2021 12:00:00 AM";

                    //////IsCustomerExempted = ids[1];

                    ReceiveDAL receiveDal = new ReceiveDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {

                        DataTable dt = receiveDal.SelectAllHeaderTemp(0, new[] { "trd.ID", "trd.UserId" }, new[] { ImportId, Program.CurrentUserID },null,null,null,false,connVM);
                        DataLoadBeforeSave(dt);
                    }
                }
                else if (value == "EON" || value.ToLower() == "purofood" || value.ToLower() == "eahpl" || value.ToLower() == "eail" || value.ToLower() == "eeufl" || value.ToLower() == "exfl")
                {
                    string ImportId = FormPReceiveImportEON.SelectOne(transactionType);
                    ReceiveDAL _slDal = new ReceiveDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SelectAllHeaderTemp(0, new[] { "trd.ID", "trd.UserId" }, new[] { ImportId, Program.CurrentUserID },null,null,null,false,connVM);
                        DataLoadBeforeSave(dt);
                    }
                }

                else if (value.ToLower() == "berger")
                {
                    string ImportId = FormPReceiveImportBerger.SelectOne(transactionType);
                    ////ReceiveDAL _slDal = new ReceiveDAL();

                    //////if (!string.IsNullOrEmpty(ImportId))
                    //////{
                    //////    DataTable dt = _slDal.SelectAllHeaderTemp(0, new[] { "trd.ID", "trd.UserId" }, new[] { ImportId, Program.CurrentUserID }, null, null, null, false, connVM);
                    //////    DataLoadBeforeSave(dt);
                    //////}
                }
                else
                {
                    FormMasterImport fmi = new FormMasterImport();
                    fmi.preSelectTable = "Receives";
                    fmi.transactionType = transactionType;
                    fmi.Show();

                }


                #region new process for bom import
                //BrowsFile();
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
                ////retResults = Import();
                //string result = retResults[0];
                //string message = retResults[1];
                //if (string.IsNullOrEmpty(result))
                //{
                //    throw new ArgumentNullException("BomImport",
                //                                    "Unexpected error.");
                //}
                //else if (result == "Success" || result == "Fail")
                //{
                //    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                //}

                #endregion new process for bom import
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
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }
            #endregion

        }

        private void DataLoadBeforeSave(DataTable dataTable)
        {

            #region try

            try
            {

                #region Check Point

                if (dataTable.Rows.Count == 0)
                {
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Reset Fields / Elments

                ClearAllFields();

                #endregion

                #region VAT Name

                VATName vname = new VATName();
                //cmbVAT1Name.DataSource = vname.VATNameList;

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Reset Fields

                txtReceiveNo.Text = "~~~ New ~~~";
                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Flag Update

                IsPost = false;

                #endregion

                ImportId = dataTable.Rows[0]["ID"].ToString();

                #region Value Assign to Form Elements

                txtCustomer.Text = dataTable.Rows[0]["CustomerName"].ToString();
                vCustomerID = dataTable.Rows[0]["CustomerID"].ToString();
                dtpReceiveDate.Value = Convert.ToDateTime(dataTable.Rows[0]["ReceiveDateTime"].ToString());
                txtTotalVATAmount.Text = "0";//Program.ParseDecimalObject(dataTable.Rows[0]["TotalVATAmount"].ToString()).ToString();//"0,0.00");
                txtTotalAmount.Text = "0";//Program.ParseDecimalObject(dataTable.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
                txtRefNo.Text = dataTable.Rows[0]["ReferenceNo"].ToString();
                txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
                SearchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"].ToString());
                chkWithToll.Checked = Convert.ToString(dataTable.Rows[0]["WithToll"].ToString()) == "Y" ? true : false;
                txtReceiveNoPre.Text = dataTable.Rows[0]["ReturnId"].ToString();

                IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;

                #endregion

                #region Detail Search / Background Worker

                bgwDetailsSearch.RunWorkerAsync();

                #endregion
                this.progressBar1.Visible = true;
                this.Enabled = true;
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
                FileLogger.Log(this.Name, "DataLoadBeforeSave", exMessage);
            }
            #endregion
        }

        private void bgwDetailsSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReceiveDAL receiveDal = new ReceiveDAL();

                ReceiveDetailResult = receiveDal.SearchReceiveDetailTemp(ImportId, Program.CurrentUserID,connVM);

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
                FileLogger.Log(this.Name, "bgwDetailsSearch_DoWork", exMessage);
            }
            #endregion



        }

        private void bgwDetailsSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                ProductDAL productDal = new ProductDAL();

                #region Statement

                // Start Complete
                dgvReceive.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ReceiveDetailResult.Rows)
                {
                    decimal stockInHand = 0;
                    decimal Quantity = 0;
                    decimal UOMc = 0;
                    decimal UnitCost = 0;

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvReceive.Rows.Add(NewRow);

                    dgvReceive.Rows[j].Cells["LineNo"].Value = j + 1;
                    dgvReceive.Rows[j].Cells["BOMId"].Value = item["BomId"].ToString();
                    dgvReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();

                    Quantity = Convert.ToDecimal(Program.ParseDecimalObject(item["Quantity"].ToString()));
                    UOMc = Convert.ToDecimal(Program.ParseDecimalObject(item["UOMc"].ToString()));
                    UnitCost = Convert.ToDecimal(Program.ParseDecimalObject(item["NBRPrice"].ToString()));

                    dgvReceive.Rows[j].Cells["Quantity"].Value = Quantity;
                    dgvReceive.Rows[j].Cells["UOMc"].Value = UOMc;

                    dgvReceive.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["NBRPrice"].ToString());

                    dgvReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvReceive.Rows[j].Cells["VATRate"].Value = "0.00";
                    dgvReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    dgvReceive.Rows[j].Cells["Status"].Value = "New";
                    dgvReceive.Rows[j].Cells["Previous"].Value = 0;
                    dgvReceive.Rows[j].Cells["Change"].Value = 0;
                    if (item["VATName"].ToString().Trim() == "NA")
                    {
                        dgvReceive.Rows[j].Cells["VATName"].Value = cmbVAT1Name.Text.Trim();
                    }
                    else
                    {
                        dgvReceive.Rows[j].Cells["VATName"].Value = item["VATName"].ToString();
                    }

                    stockInHand = Convert.ToDecimal(productDal.AvgPriceNew(item["ItemNo"].ToString(), dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                               DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM,Program.CurrentUserID).Rows[0]["Quantity"].ToString());

                    dgvReceive.Rows[j].Cells["Stock"].Value = stockInHand;
                    dgvReceive.Rows[j].Cells["SD"].Value = "0.00";
                    dgvReceive.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvReceive.Rows[j].Cells["UOMQty"].Value = Quantity * UOMc;
                    dgvReceive.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["NBR_Price"].ToString());

                    dgvReceive.Rows[j].Cells["UnitPrice"].Value = UnitCost * UOMc;


                    j = j + 1;

                }

                Rowcalculate();

                ChangeData = true;

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
                FileLogger.Log(this.Name, "bgwDetailsSearch_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                ChangeData = false;

                #region Button Stats

                this.btnFirst.Enabled = true;
                this.btnPrevious.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnLast.Enabled = true;

                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;

                #endregion
            }


        }


        private void ReceiveDataLoad(DataTable dataTable)
        {


            #region CheckPoint

            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            #endregion

            DataRow dr = dataTable.Rows[0];
            txtReceiveNo.Text = dr["ReceiveNo"].ToString();

            SearchInvoice();

        }
        #endregion

        #region Methods 09

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
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
                string err = ex.Message.ToString();
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

        #region OLD
        //private string[] Import()
        //        {
        //            #region Close1
        //            string[] sqlResults = new string[4];
        //            sqlResults[0] = "Fail";
        //            sqlResults[1] = "Fail";
        //            sqlResults[2] = "";
        //            sqlResults[3] = "";
        //            #endregion Close1

        //            #region try
        //            OleDbConnection theConnection = null;
        //            try
        //            {
        //                TransactionTypes();

        //                #region Load Excel

        //                string str1 = "";
        //                string fileName = Program.ImportFileName;
        //                if (string.IsNullOrEmpty(fileName))
        //                {
        //                    MessageBox.Show("Please select the right file for import");
        //                    return sqlResults;
        //                }
        //                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
        //                                          + "Data Source=" + fileName + ";"
        //                                          + "Extended Properties=" + "\""
        //                                          + "Excel 12.0;HDR=YES;" + "\"";
        //                theConnection = new OleDbConnection(connectionString);
        //                theConnection.Open();
        //                OleDbDataAdapter ReceiveM = new OleDbDataAdapter("SELECT * FROM [ReceiveM$]", theConnection);
        //                System.Data.DataTable dtReceiveM = new System.Data.DataTable();
        //                ReceiveM.Fill(dtReceiveM);

        //                OleDbDataAdapter adapterReceiveD = new OleDbDataAdapter("SELECT * FROM [ReceiveD$]", theConnection);
        //                System.Data.DataTable dtReceiveD = new System.Data.DataTable();
        //                adapterReceiveD.Fill(dtReceiveD);

        //                theConnection.Close();

        //                #endregion Load Excel


        //                #region RowCount
        //                int MRowCount = 0;
        //                int MRow = dtReceiveM.Rows.Count;
        //                for (int i = 0; i < dtReceiveM.Rows.Count; i++)
        //                {
        //                    if (!string.IsNullOrEmpty(dtReceiveM.Rows[i]["ID"].ToString()))
        //                    {
        //                        MRowCount++;
        //                    }

        //                }
        //                if (MRow != MRowCount)
        //                {
        //                    MessageBox.Show("you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.");
        //                    return sqlResults;
        //                }
        //                #endregion RowCount

        //                #region ID in Master or Detail table

        //                for (int i = 0; i < MRowCount; i++)
        //                {
        //                    string importID = dtReceiveM.Rows[i]["ID"].ToString();

        //                    if (!string.IsNullOrEmpty(importID))
        //                    {
        //                        DataRow[] DetailRaws = dtReceiveD.Select("ID='" + importID + "'");
        //                        if (DetailRaws.Length == 0)
        //                        {
        //                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
        //                        }

        //                    }

        //                }

        //                #endregion

        //                #region Double ID in Master

        //                for (int i = 0; i < MRowCount; i++)
        //                {
        //                    string id = dtReceiveM.Rows[i]["ID"].ToString();
        //                    DataRow[] tt = dtReceiveM.Select("ID='" + id + "'");
        //                    if (tt.Length > 1)
        //                    {
        //                        MessageBox.Show("you have duplicate master id " + id + " in import file.");
        //                        return sqlResults;
        //                    }

        //                }

        //                #endregion Double ID in Master


        //                #region Process model
        //                CommonImport cImport = new CommonImport();

        //                #region checking from database is exist the information(NULL Check)
        //                #region Master

        //                for (int j = 0; j < MRowCount; j++)
        //                { 
        //                    #region Checking Date is null or different formate

        //                    bool IsReceiveDate;
        //                    IsReceiveDate = cImport.CheckDate(dtReceiveM.Rows[j]["Receive_DateTime"].ToString().Trim());
        //                    if (IsReceiveDate != true)
        //                    {
        //                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
        //                    }
        //                    #endregion Checking Date is null or different formate

        //                    #region Checking Y/N value 
        //                    bool post;
        //                    post = cImport.CheckYN(dtReceiveM.Rows[j]["Post"].ToString().Trim());
        //                    if (post != true)
        //                    {
        //                        throw new ArgumentNullException("Please insert Y/N in Post field.");
        //                    }
        //                    #endregion Checking Y/N value

        //                    #region Check Return receive id
        //                    string ReturnId = string.Empty;
        //                    ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[j]["Return_Id"].ToString().Trim(), null, null);
        //                    #endregion Check Return receive id
        //                }

        //                #endregion Master

        //                #region Details
        //                #region Row count for details table
        //                int DRowCount = 0;
        //                for (int i = 0; i < dtReceiveD.Rows.Count; i++)
        //                {
        //                    if (!string.IsNullOrEmpty(dtReceiveD.Rows[i]["ID"].ToString()))
        //                    {
        //                        DRowCount++;
        //                    }

        //                }

        //                #endregion Row count for details table

        //                for (int i = 0; i < DRowCount; i++)
        //                {
        //                    string ItemNo = string.Empty;
        //                    string UOMn = string.Empty;

        //                    #region FindItemId

        //                    ItemNo = cImport.FindItemId(dtReceiveD.Rows[i]["Item_Name"].ToString().Trim()
        //                                 , dtReceiveD.Rows[i]["Item_Code"].ToString().Trim(), null, null);
        //                    #endregion FindItemId

        //                    #region FindUOMn
        //                    UOMn = cImport.FindUOMn(ItemNo, null, null);
        //                    #endregion FindUOMn
        //                    #region FindUOMn
        //                    cImport.FindUOMc(UOMn, dtReceiveD.Rows[i]["UOM"].ToString().Trim(), null, null);
        //                    #endregion FindUOMn

        //                    #region VATName
        //                    cImport.FindVatName(dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim());

        //                    #endregion VATName

        //                    #region FindLastNBRPrice
        //                    DataRow[] vmaster; //= new DataRow[];//
        //                    string nbrPrice = string.Empty;

        //                    //var transactionDate = DateTime.MinValue;
        //                    var transactionDate = "";

        //                    vmaster = dtReceiveM.Select("ID='" + dtReceiveD.Rows[i]["ID"].ToString().Trim() + "'");

        //                    foreach (DataRow row in vmaster)
        //                    {
        //                        var tt =Convert.ToDateTime(row["Receive_DateTime"].ToString().Trim()).ToString("yyyy-MMM-dd HH:mm:ss");
        //                        transactionDate = tt;

        //                    }

        //                    nbrPrice = cImport.FindLastNBRPrice(ItemNo, dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim(),
        //                        transactionDate,null,null);


        //                    #endregion FindLastNBRPrice

        //                    #region Numeric value check
        //                    bool IsQuantity = cImport.CheckNumericBool(dtReceiveD.Rows[i]["Quantity"].ToString().Trim());
        //                    if (IsQuantity != true)
        //                    {
        //                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
        //                    }
        //                    #endregion Numeric value check
        //                }
        //                #endregion Details

        //                #endregion checking from database is exist the information(NULL Check)



        //                for (int i = 0; i < MRowCount; i++)
        //                {
        //                    #region Master Receive
        //                    string importID = dtReceiveM.Rows[i]["ID"].ToString().Trim();
        //                    string receiveDateTime = dtReceiveM.Rows[i]["Receive_DateTime"].ToString().Trim();
        //                    #region CheckNull
        //                    string serialNo = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Reference_No"].ToString().Trim());
        //                    string comments = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Comments"].ToString().Trim());
        //                    #endregion CheckNull
        //                    string post = dtReceiveM.Rows[i]["Post"].ToString().Trim();
        //                    #region Check Return receive id
        //                    string ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[i]["Return_Id"].ToString().Trim(), null, null);
        //                    #endregion Check Return receive id
        //                    receiveMasterVM = new ReceiveMasterVM();

        //                    receiveMasterVM.ReceiveDateTime = Convert.ToDateTime(receiveDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); //dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
        //                    //receiveMasterVM.VatName = "NA";
        //                    receiveMasterVM.Post = post;
        //                    receiveMasterVM.ReturnId = ReturnId;
        //                    receiveMasterVM.SerialNo = serialNo.Replace(" ", "");
        //                    receiveMasterVM.Comments = comments;
        //                    receiveMasterVM.CreatedBy = Program.CurrentUser;
        //                    receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //                    receiveMasterVM.LastModifiedBy = Program.CurrentUser;
        //                    receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //                    receiveMasterVM.transactionType = transactionType;
        //                    receiveMasterVM.FromBOM = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
        //                    receiveMasterVM.TotalVATAmount = Convert.ToDecimal(0);
        //                    receiveMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
        //                    receiveMasterVM.ImportId = importID;


        //                    DataRow[] RDRaws;//= new DataRow[];//

        //                    #region fitemno
        //                    if (!string.IsNullOrEmpty(importID))
        //                    {
        //                        RDRaws =
        //                           dtReceiveD.Select("ID='" + importID + "'");
        //                    }
        //                    else
        //                    {
        //                        RDRaws = null;
        //                    }
        //                    #endregion fitemno
        //                    #endregion Master Receive
        //                    #region Details Receive
        //                    int counter = 1;
        //                    receiveDetailVMs = new List<ReceiveDetailVM>();
        //                    foreach (DataRow row in RDRaws)
        //                    {
        //                        string itemCode = row["Item_Code"].ToString().Trim();
        //                        string itemName = row["Item_Name"].ToString().Trim();
        //                        string itemNo = cImport.FindItemId(itemName, itemCode, null, null);
        //                        string quantity = row["Quantity"].ToString().Trim();
        //                        string uOM = row["UOM"].ToString().Trim();
        //                        string uOMn = cImport.FindUOMn(itemNo, null, null);
        //                        string uOMc = cImport.FindUOMc(uOMn, uOM, null, null);
        //                        string vATName = row["VAT_Name"].ToString().Trim();
        //                        string LastNBRPrice = cImport.FindLastNBRPrice(itemNo, vATName,

        //                                                                   Convert.ToDateTime(receiveDateTime).ToString("yyyy-MMM-dd"),null,null);

        //                        ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();
        //                        receiveDetailVM.ReceiveLineNo = counter.ToString();
        //                        receiveDetailVM.ItemNo = itemNo.ToString();
        //                        receiveDetailVM.Quantity = Convert.ToDecimal(quantity);
        //                        receiveDetailVM.UOM = uOM;
        //                        receiveDetailVM.UOMn = uOMn.ToString();
        //                        receiveDetailVM.UOMc = Convert.ToDecimal(uOMc);
        //                        receiveDetailVM.VATRate = Convert.ToDecimal(0);
        //                        receiveDetailVM.VATAmount = Convert.ToDecimal(0);
        //                        receiveDetailVM.SD = Convert.ToDecimal(0);
        //                        receiveDetailVM.SDAmount = Convert.ToDecimal(0);
        //                        receiveDetailVM.BOMId = "0";
        //                        receiveDetailVM.CommentsD = "NA";
        //                        receiveDetailVM.CostPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
        //                        receiveDetailVM.NBRPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
        //                        receiveDetailVM.SubTotal = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc) *
        //                                                   Convert.ToDecimal(quantity);
        //                        receiveDetailVM.UOMPrice = Convert.ToDecimal(LastNBRPrice);
        //                        receiveDetailVM.UOMQty = Convert.ToDecimal(uOMc) *
        //                                                       Convert.ToDecimal(quantity);
        //                        receiveDetailVMs.Add(receiveDetailVM);

        //                        counter++;

        //                    }// detail
        //                    SAVE_DOWORK_SUCCESS = false;

        //                    sqlResults = new string[4];
        //                    ReceiveDAL receiveDal = new ReceiveDAL();
        //                    sqlResults = receiveDal.ReceiveInsert(receiveMasterVM, receiveDetailVMs,null,null,null);
        //                    SAVE_DOWORK_SUCCESS = true;
        //                }//master
        //                return sqlResults;

        //                    #endregion Details Sale

        //                #endregion
        //                }
        //            #endregion try
        //            #region catch
        //            catch (ArgumentNullException ex)
        //            {
        //                string err = ex.Message.ToString();
        //                string[] error = err.Split(FieldDelimeter.ToCharArray());
        //                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
        //                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            }
        //            catch (IndexOutOfRangeException ex)
        //            {
        //                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            }
        //            catch (NullReferenceException ex)
        //            {
        //                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            }
        //            catch (FormatException ex)
        //            {

        //                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            }

        //            catch (SoapHeaderException ex)
        //            {
        //                string exMessage = ex.Message;
        //                if (ex.InnerException != null)
        //                {
        //                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                                ex.StackTrace;

        //                }

        //                FileLogger.Log(this.Name, "ProductImport", exMessage);
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            }
        //            catch (SoapException ex)
        //            {
        //                string exMessage = ex.Message;
        //                if (ex.InnerException != null)
        //                {
        //                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                                ex.StackTrace;

        //                }
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                FileLogger.Log(this.Name, "ProductImport", exMessage);
        //            }
        //            catch (EndpointNotFoundException ex)
        //            {
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
        //            }
        //            catch (WebException ex)
        //            {
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
        //            }
        //            catch (Exception ex)
        //            {
        //                string exMessage = ex.Message + Environment.NewLine +
        //                                ex.StackTrace;
        //                if (ex.InnerException != null)
        //                {
        //                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                                ex.StackTrace;

        //                }
        //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                FileLogger.Log(this.Name, "ProductImport", exMessage);
        //            }
        //            return sqlResults;

        //            #endregion
        //        }
        #endregion OLD

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


            DataTable dtReceiveM = new DataTable();
            DataTable dtReceiveD = new DataTable();


            #region Master table
            dtReceiveM.Columns.Add("Identifier");
            dtReceiveM.Columns.Add("ID");
            dtReceiveM.Columns.Add("Receive_DateTime");
            dtReceiveM.Columns.Add("Reference_No");
            dtReceiveM.Columns.Add("Comments");
            dtReceiveM.Columns.Add("Post");
            dtReceiveM.Columns.Add("Return_Id");
            dtReceiveM.Columns.Add("Transection_Type").DefaultValue = transactionType;
            dtReceiveM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
            dtReceiveM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
            dtReceiveM.Columns.Add("From_BOM").DefaultValue = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
            dtReceiveM.Columns.Add("Total_VAT_Amount").DefaultValue = "0";
            dtReceiveM.Columns.Add("Total_Amount").DefaultValue = txtTotalAmount.Text.Trim();
            #endregion Master table

            #region Details table
            dtReceiveD.Columns.Add("Identifier");
            dtReceiveD.Columns.Add("ID");
            dtReceiveD.Columns.Add("Item_Code");
            dtReceiveD.Columns.Add("Item_Name");
            dtReceiveD.Columns.Add("Quantity");
            dtReceiveD.Columns.Add("UOM");
            dtReceiveD.Columns.Add("VAT_Name");
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
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper().Trim();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper().Trim() == "M")
                            {
                                dtReceiveM.Rows.Add(mItems);
                            }
                        }
                    }
                    else
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper().Trim() == "D")
                            {
                                dtReceiveD.Rows.Add(dItems);
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
                //    if (items[0].Replace("\n", "").Trim().ToUpper() == "M")
                //    {
                //        dtReceiveM.Rows.Add(items);
                //    }
                //    else if (items[0].Replace("\n", "").Trim().ToUpper() == "D")
                //    {
                //        dtReceiveD.Rows.Add(items);
                //    }
                //}
                //if (sr != null)
                //{
                //    sr.Close();
                //}
                #endregion Load Text file

                SAVE_DOWORK_SUCCESS = false;
                //ReceiveDAL receiveDal = new ReceiveDAL();
                IReceive receiveDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.ImportData(dtReceiveM, dtReceiveD, 0, null, null, null, connVM);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

        private void txtRefNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            { SendKeys.Send("{TAB}"); }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            #region Try


            try
            {
                trackingsCmb.Clear();

                if (dgvReceive.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvReceive.Rows.Count; i++)
                    {
                        var trackingCmb = new TrackingCmbDTO();
                        trackingCmb.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                        trackingCmb.ProductCode = dgvReceive.Rows[i].Cells["PCode"].Value.ToString();
                        trackingCmb.ProductName = dgvReceive.Rows[i].Cells["ItemName"].Value.ToString();
                        trackingCmb.VatName = dgvReceive.Rows[i].Cells["VATName"].Value.ToString();
                        trackingCmb.BOMId = dgvReceive.Rows[i].Cells["BOMId"].Value.ToString();
                        trackingCmb.Qty = dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString();
                        trackingCmb.EffectiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");

                        if (rbtnReceiveReturn.Checked)
                        {
                            trackingCmb.ReceiveNo = txtReceiveNoPre.Text;
                        }
                        else if (IsUpdate == true)
                        {
                            trackingCmb.ReceiveNo = txtReceiveNo.Text;
                        }


                        trackingsCmb.Add(trackingCmb);
                    }

                    trackingsVm.Clear();

                    Program.fromOpen = "Me";

                    if (rbtnReceiveReturn.Checked)
                    {
                        trackingsVm = FormTracking.SelectOne(trackingsCmb, "Receive_Return");
                    }
                    else
                    {
                        trackingsVm = FormTracking.SelectOne(trackingsCmb, "Receive");
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
                //TrackingDAL trackingDal = new TrackingDAL();
                ITracking trackingDal = OrdinaryVATDesktop.GetObject<TrackingDAL, TrackingRepo, ITracking>(OrdinaryVATDesktop.IsWCF);

                DataTable trackingInfoDt = new DataTable();
                string recNo = "";
                if (rbtnReceiveReturn.Checked)
                {
                    recNo = txtReceiveNoPre.Text;
                }
                else
                {
                    recNo = ReceiveDetailData;
                }

                trackingInfoDt = trackingDal.SearchExistingTrackingItems("Y", recNo, "", "", connVM);
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


                        trackingsVm.Add(trackingVm);
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

        private void txtCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                CustomerLoad();
            }

        }

        private void CustomerLoad1()
        {
            DataGridViewRow selectedRow = new DataGridViewRow();
            selectedRow = FormCustomerFinder.SelectOne();
            if (selectedRow != null && selectedRow.Selected == true)
            {
                vCustomerID = "0";
                txtCustomer.Text = "";
                vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
            }
            txtCustomer.Focus();

        }

        private void CustomerLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select cg.CustomerGroupName,c.CustomerCode,c.CustomerName,c.ShortName,c.Address1,c.CustomerID from Customers c 
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID where 1=1 and c.ActiveStatus='Y' ";

                string SQLTextRecordCount = @" select count(CustomerCode)RecordNo from Customers ";

                string[] shortColumnName = { "cg.CustomerGroupName", "c.CustomerCode", "c.CustomerName", "c.ShortName", "c.Address1" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    vCustomerID = "0";
                    txtCustomer.Text = "";
                    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                    txtCustomer.Focus();


                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            vCustomerID = "0";
            txtCustomer.Text = "";
        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomer_DoubleClick(object sender, EventArgs e)
        {
            CustomerLoad();
        }

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                if (CustomerWiseBOM)
                {
                    SqlText += @"  and p.ItemNo in (select distinct FinishItemNo from BOMs where CustomerID ='" + vCustomerID + "'  )";

                }
                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products 
                  ";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                    //cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();

                    

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerAddressLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //DataGridViewRow selectedRow = new DataGridViewRow();
            //selectedRow = FormProductFinder.SelectOne(vCustomerID, IsRawParam);
            //if (selectedRow != null && selectedRow.Selected == true)
            //{
            //    //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
            //    vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
            //    cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
            //    cmbProduct.Text = selectedRow.Cells["ProductCode"].Value.ToString();// ProductInfo[27].ToString();//27
            //}
        }

        #endregion

        #region Methods 10

        private void button4_Click(object sender, EventArgs e)
        {
            ProductLoad();
        }

        private void brnTR_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT16 frmRptVAT16 = new FormRptVAT16();

                //mdi.RollDetailsInfo("8201");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvReceive.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.txtTransType.Text = transactionType;
                }
                //if (rbtnTollReceive.Checked)
                //{
                // frmRptVAT16.rbtnTollRegisterRaw.Checked = true;
                frmRptVAT16.Text = "Toll Register";
                //}
                frmRptVAT16.ShowDialog();

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

        private void dgvReceive_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {

                if (e.ColumnIndex == 1) // checking remove button
                {
                    RemoveSelectedRow();
                }
                
            }
        }

        private void txtTotalVATAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalVATAmount, "Total VATAmount");
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalAmount, "TotalAmount");
        }

        private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbReferenceNo_Leave(object sender, EventArgs e)
        {
            try
            {


                DataTable dt = new DataTable();
                ProductDAL ProductDal = new ProductDAL();
                //IProduct ProductDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                if (CustomerWiseBOM)
                {
                    dt = ProductDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                                             dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID, connVM);

                }
                else
                {
                    dt = ProductDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                  dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null,"0",connVM);

                }
                txtBOMReferenceNo.Text = cmbBOMReferenceNo.Text;
                txtBOMId.Text = "";
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbReferenceNo_Leave", exMessage);
            }
            #endregion Catch

        }

        private void SetBOM(DataTable dt)
        {
            try
            {
                int BOMId = 0;
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region ReferenceNo
                    if (!string.IsNullOrWhiteSpace(txtBOMId.Text))
                    {
                        BOMNBRVM bomnbrvm = bomdal.SelectAllList(txtBOMId.Text, null, null, null, null, null, connVM).FirstOrDefault();

                        if (bomnbrvm != null) txtBOMReferenceNo.Text = bomnbrvm.ReferenceNo;
                    }


                    string ReferenceNo = txtBOMReferenceNo.Text;

                    DataView dv = new DataView(dt);
                    dv.RowFilter = "ReferenceNo = '" + ReferenceNo + "'";

                    DataTable dtBOM = new DataTable();
                    dtBOM = dv.ToTable();


                    #endregion

                    #region BOMId
                    if (dtBOM != null && dtBOM.Rows.Count > 0)
                    {
                        DataRow dr = dtBOM.Rows[0];
                        string tempBOMId = dr["BOMId"].ToString();
                        
                        string tempNBRPrice = dr["NBRPrice"].ToString();
                        if (!string.IsNullOrWhiteSpace(tempBOMId))
                        {
                            BOMId = Convert.ToInt32(tempBOMId);
                        }

                        if (!string.IsNullOrWhiteSpace(tempNBRPrice))
                        {
                            txtUnitCost.Text = tempNBRPrice;
                        }

                        if (!string.IsNullOrWhiteSpace(ReferenceNo))
                        {
                            cmbBOMReferenceNo.Text = ReferenceNo;
                        }
                       
                       
                    }
                    #endregion
                }


                txtBOMId.Text = BOMId.ToString();

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
                FileLogger.Log(this.Name, "cmbReferenceNo_Leave", exMessage);
            }
            #endregion Catch
        }

        private void chkIssueFromBOM_CheckedChanged(object sender, EventArgs e)
        {
            IssueFromBOM = chkIssueFromBOM.Checked;
        }

        #endregion

        #region Receive Navigation

        private void txtReceiveNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                ReceiveNavigation("Current");
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            ReceiveNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            ReceiveNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ReceiveNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            ReceiveNavigation("Last");
        }

        private void ReceiveNavigation(string ButtonName)
        {
            try
            {
                ReceiveDAL _ReceiveDAL = new ReceiveDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(txtId.Text);
                vm.InvoiceNo = txtReceiveNo.Text;


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _ReceiveDAL.Receive_Navigation(vm,null,null,connVM);

                txtReceiveNo.Text = vm.InvoiceNo;
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
                FileLogger.Log(this.Name, "ReceiveNavigation", exMessage);
            }
            #endregion Catch

        }

        #endregion

        private void btnReceiveInformation_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {

                if (string.IsNullOrWhiteSpace(txtReceiveNo.Text) && (txtReceiveNo.Text == "~~~ New ~~~"))
                {
                    MessageBox.Show("Please Select Issue No");
                    return;
                }
                MISReport _report = new MISReport();


                reportDocument = _report.ReceiveInformation(txtReceiveNo.Text.Trim(), Program.BranchId,connVM);


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
                FileLogger.Log(this.Name, "btnReceiveInformation_Click", exMessage);
            }

            #endregion
        }

        private void dgvReceive_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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




    }
}
