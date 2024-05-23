using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
//
//
using SymphonySofttech.Utilities;
//
//
//
using VATClient.ReportPreview;
using VATClient.ModelDTO;
using System.Collections.Generic;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.OleDb;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPages;
using VATServer.License;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using SymphonySofttech.Reports;

namespace VATClient
{
    public partial class FormBOM : Form
    {
        #region Constructors

        public FormBOM()
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

        private List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private List<ProductMiniDTO> finishProducts = new List<ProductMiniDTO>();
        private List<UomDTO> UOMs = new List<UomDTO>();
        private List<BomOhDTO> BomOHs = new List<BomOhDTO>();

        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private bool IsPost = false;
        private string TransType = "";
        private string IssueRequired = "";
        private string CustomerID = "0";
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private int searchBranchId = 0;


        #region variable

        private string bomGridIndex = string.Empty;
        private string type = string.Empty;
        //string BOMResult = string.Empty;
        private DataTable BOMRawResult;
        private DataTable BOMMasterResult;
        private string effectDate = string.Empty;
        private string decriptedBOMDData = string.Empty;
        private string[] BOMDLines = new string[] { };
        private string[] BOMDFields = new string[] { };
        private string productId = string.Empty;

        //private string OHResult = string.Empty;
        private DataTable OHResult;
        private string decriptedOHData = string.Empty;
        //private string[] OHLines = new string[] { };
        //private string[] OHFields = new string[] { };

        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string HSCodeNoParam = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";

        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private string BOMId = "";

        private DataTable ProductResultDs;


        private DataTable uomResult;

        private bool IsImport = false;
        private bool IsUpdate = false;
        private bool ChangeData = false;
        private string CategoryId { get; set; }
        //private DataSet OverHeadResult;
        private DataTable OverHeadResult;
        private string CategoryIdR { get; set; }
        private string CategoryIdF { get; set; }
        public string VFIN = "171";
        private string NBR1 = "0";
        private int BOMDPlaceQ;
        private int BOMDPlaceA;
        private int BOMDPlaceNetCost;
        private string CategoryIsRaw { get; set; }

        private decimal oldNbrPrice = 0;

        #endregion variable

        #endregion

        private void FormBOM_Load(object sender, EventArgs e)
        {

            try
            {
                #region Settings Data

                BOMDPlaceQ = 0;
                BOMDPlaceA = 0;
                string vBOMDPlaceQ, vBOMDPlaceA,vBOMDPlaceNetCost = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                vBOMDPlaceQ = commonDal.settingsDesktop("BOM", "Quantity",null,connVM);
                vBOMDPlaceA = commonDal.settingsDesktop("BOM", "Amount",null,connVM);
                vBOMDPlaceNetCost = commonDal.settingsDesktop("BOM", "NetCost",null,connVM);
                IssueRequired = commonDal.settingsDesktop("BOM", "IssueRequired",null,connVM);


                if (string.IsNullOrEmpty(vBOMDPlaceQ)
                    || string.IsNullOrEmpty(vBOMDPlaceA)
                    || string.IsNullOrEmpty(vBOMDPlaceNetCost))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                BOMDPlaceQ = Convert.ToInt32(vBOMDPlaceQ);
                BOMDPlaceA = Convert.ToInt32(vBOMDPlaceA);
                BOMDPlaceNetCost = Convert.ToInt32(vBOMDPlaceNetCost);

                #endregion Settings

                #region Reset Elements

                lblBOMId.Text = "";

                AllClear();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Element Stats

                cmbType.Text = "VAT 4.3";

                progressBar1.Visible = true;

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion

                #region Flag Update

                ChangeData = false;

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
                FileLogger.Log(this.Name, "FormBOM_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormBOM_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormBOM_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormBOM_Load", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBOM_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBOM_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBOM_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBOM_Load", exMessage);
            }

            #endregion Catch
        }

        private string vTollIssueCostWithOthers;
        private bool TollIssueCostWithOthers;

        #region Methods 01

        private void FormMaker()
        {
            CommonDAL commonDal = new CommonDAL();
            //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

            vTollIssueCostWithOthers = commonDal.settingsDesktop("BOM", "TollIssueCostWithOthers",null,connVM);
            TollIssueCostWithOthers = vTollIssueCostWithOthers == "Y" ? true : false;

        }

        private void FormLoad()
        {

            VATName vname = new VATName();

            cmbType.DataSource = vname.VATNameList;

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

            #region Product

            ItemNoParam = string.Empty;
            CategoryIDParam = string.Empty;
            IsRawParam = string.Empty;
            HSCodeNoParam = string.Empty;
            ActiveStatusProductParam = string.Empty;
            TradingParam = string.Empty;
            NonStockParam = string.Empty;
            ProductCodeParam = string.Empty;

            #endregion Product


        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region UOM

                UOMDAL uomdal = new UOMDAL();
                //IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);
                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam,Program.DatabaseName);

                #endregion UOM

                #region Product

                ProductResultDs = new DataTable();
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(ItemNoParam, CategoryIDParam, IsRawParam, HSCodeNoParam,
                                                                 ActiveStatusProductParam, TradingParam, NonStockParam,
                                                                 ProductCodeParam, connVM);

                #endregion Product

                #region OverHeadSearch

                OverHeadResult = new DataTable();
                ProductDAL overHeadDal = new ProductDAL();
                //IProduct overHeadDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                OverHeadResult = overHeadDal.SearchOverheadForBOMNew("Y", connVM);

                #endregion OverHeadSearch



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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
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

                #region Product

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

                    //prod.LIFOPrice = Convert.ToDecimal(item2["LIFOPrice"].ToString());
                    //prod.BOMPrice = Convert.ToDecimal(item2["BOMPrice"].ToString());
                    ProductsMini.Add(prod);
                }

                #endregion Product

                #region OverHeadSearch

                cmbOverHead.Items.Clear();

                for (int j = 0; j < Convert.ToInt32(OverHeadResult.Rows.Count); j++)
                {
                    if (OverHeadResult.Rows[j]["Headname"] != "Margin")
                    {

                        cmbOverHead.Items.Add(OverHeadResult.Rows[j]["Headname"]);
                    }
                }
                if (cmbOverHead.Items.Count <= 0)
                {
                    MessageBox.Show("Please input Over head first", this.Text);
                    return;
                }
                cmbOverHead.SelectedIndex = 0;

                #endregion OverHeadSearch

                #region OverHeadSearch

                //cmbOverHead.Items.Clear();
                // foreach (DataRow item2 in OverHeadResult.Rows)
                // {

                //     var Bh = new BomOhDTO();
                //     Bh.HeadID = item2["HeadID"].ToString();
                //     Bh.Headname = item2["Headname"].ToString();
                //     Bh.OHCode = item2["OHCode"].ToString();
                //     Bh.RebatePercent =Convert.ToDecimal(item2["RebatePercent"].ToString());
                //     if (item2["Headname"].ToString() != "Margin")
                //    {

                //        cmbOverHead.Items.Add(item2["Headname"].ToString());
                //    }

                // }
                //if (cmbOverHead.Items.Count <= 0)
                //{
                //    MessageBox.Show("Please input Over head first", this.Text);
                //    return;
                //}
                //cmbOverHead.SelectedIndex = 0;


                #endregion OverHeadSearch

                ChangeData = false;

                FormMaker();
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.progressBar1.Visible = false;
        }

        private void Uoms()
        {

            string uOMFrom = txtRUOM.Text.Trim().ToLower();
            try
            {
                cmbUom.Items.Clear();
                if (UOMs != null && UOMs.Any())
                {

                    var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom).ToList()
                               select uom.UOMTo;

                    if (uoms != null && uoms.Any())
                    {
                        cmbUom.Items.AddRange(uoms.ToArray());
                        cmbUom.Items.Add(txtRUOM.Text.Trim());
                        //txtUomConv.Text = uoms.First().ToString();
                    }

                }
                cmbUom.Text = txtRUOM.Text.Trim();


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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }

            #endregion Catch
        }


        private void FUoms()
        {


            string uOMFrom = txtFUOM.Text.Trim().ToLower();
            try
            {
                cmbFUOM.Items.Clear();
                if (UOMs != null && UOMs.Any())
                {

                    var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom).ToList()
                               select uom.UOMTo;

                    if (uoms != null && uoms.Any())
                    {
                        cmbFUOM.Items.AddRange(uoms.ToArray());
                        cmbFUOM.Items.Add(txtRUOM.Text.Trim());
                        //txtUomConv.Text = uoms.First().ToString();
                    }

                }
                cmbFUOM.Text = txtFUOM.Text.Trim();


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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }

            #endregion Catch
        }

        private void UOMSearch()
        {
            try
            {

                #region Statement

                UOMIdParam = string.Empty;
                UOMFromParam = string.Empty;
                UOMToParam = string.Empty;
                ActiveStatusUOMParam = string.Empty;

                UOMIdParam = string.Empty;
                UOMFromParam = string.Empty;
                UOMToParam = string.Empty;
                ActiveStatusUOMParam = "Y";


                this.progressBar1.Visible = true;
                bgwUOM.RunWorkerAsync();
                cmbUom.Enabled = false;

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
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UOMSearch", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", exMessage);
            }

            #endregion Catch
        }

        private void bgwUOM_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                //string str1 = "";

                #region Statement

                // Start DoWork

                uomResult = new DataTable();

                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam,Program.DatabaseName);
                //str1 = "";

                // End DoWork

                #endregion

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

            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
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

                #endregion UOM

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
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                cmbUom.Enabled = true;
                this.progressBar1.Visible = false;

            }
        }

        private void AllClear()
        {
            try
            {
                dtpEffectDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                dtpFirstSupplyDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                //dtpEffectDate.Value = Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

                //txtTrading.Text = "";
                txtComments.Text = "";
                txtFCategoryName.Text = "";
                txtFHSCode.Text = "";
                txtFinishItemNo.Text = "";
                txtFProductName.Text = "";
                txtGTotal.Text = "0.00";
                txtIsRaw.Text = "";
                txtLineNo.Text = "0.00";
                txtNBRPrice.Text = "0.00";
                txtNetCost.Text = "0.00000";
                txtOHCost.Text = "0.00";
                txtOHGTotal.Text = "0.00";
                txtPrevious.Text = "0.00";
                txtRCategoryName.Text = "";
                txtRHSCode.Text = "";
                txtRProductCode.Text = "";
                txtRProductName.Text = "";
                txtRQuantity.Text = "0";
                //txtRSD.Text = "0";
                txtRUnitCost.Text = "0.00";//s
                txtRUOM.Text = "";
                //txtRVATRate.Text = "0.00";
                txtRWastage.Text = "0";
                txtRWastageInp.Text = "0";
                txtUomConv.Text = "0";
                txtTotalCost.Text = "0.00";
                txtTotalQty.Text = "0";
                cmbFProduct.Text = "Select";
                cmbOverHead.Text = "Select";
                cmbRProduct.Text = "Select";
                txtPacketPrice.Text = "0.00";
                txtPInvoiceNo.Text = "";
                txtFProductCode.Text = "";
                txtFUOM.Text = "";
                txtOHCode.Text = "";
                txtGrandTotal.Text = "0.00";
                txtMargin.Text = "0.00";
                dgvBOM.Rows.Clear();
                dgvOverhead.Rows.Clear();
                cmbType.Text = "VAT 1";

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
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "AllClear", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", exMessage);
            }

            #endregion Catch

        }

        private void Rowcalculate()
        {
            decimal SumRWCost = 0;
            try
            {

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    dgvBOM["LineNo", i].Value = i + 1;

                    dgvBOM["TotalQty", i].Value =
                        (Convert.ToDecimal(dgvBOM["Quantity", i].Value) + Convert.ToDecimal(dgvBOM["Wastage", i].Value))
                            .ToString("");

                    if (cmbType.Text.Trim() == "VAT 4.3 (Toll Issue)")
                    {
                        if (TollIssueCostWithOthers == true)
                        {
                            SumRWCost = SumRWCost + Convert.ToDecimal(dgvBOM["Cost", i].Value);
                        }
                        else
                        {
                            if (dgvBOM["InputType", i].Value.ToString() == "Overhead")
                            {
                                SumRWCost = SumRWCost + Convert.ToDecimal(dgvBOM["Cost", i].Value);
                            }
                        }

                    }
                    else
                    {
                        SumRWCost = SumRWCost + Convert.ToDecimal(dgvBOM["Cost", i].Value);
                    }

                }

                //txtTotalCost.Text = Convert.ToDecimal(SumRWCost).ToString("0.00");
                //txtTotalCost.Text = Convert.ToDecimal(Program.FormatingNumeric(SumRWCost.ToString(), BOMDPlaceA)).ToString();
                txtTotalCost.Text = Convert.ToDecimal(Program.FormatingNumeric(SumRWCost.ToString(), BOMDPlaceNetCost)).ToString();
                GTotal();
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

        private void GTotal()
        {
            try
            {
                if (txtRSD.Text == "")
                {
                    txtRSD.Text = "0";
                }
                if (txtRVATRate.Text == "")
                {
                    txtRVATRate.Text = "0";
                }
                if (txtTrading.Text == "")
                {
                    txtTrading.Text = "0";
                }
                if (txtPacketPrice.Text == "")
                {
                    txtPacketPrice.Text = "0";
                }

                decimal SubBOM = 0;
                decimal SubOH = 0;
                decimal PackPrice = 0;
                decimal Trading = 0;
                decimal SD = 0;
                decimal VATRate = 0;
                decimal TradingSum = 0;
                decimal SDSum = 0;
                decimal VATSum = 0;
                decimal SubTotal = 0;
                decimal GrandTotal = 0;
                decimal Margin = 0;


                Trading = Convert.ToDecimal(txtTrading.Text.Trim());
                SD = Convert.ToDecimal(Program.ParseDecimalObject(txtRSD.Text.Trim()));
                VATRate = Convert.ToDecimal(Program.ParseDecimalObject(txtRVATRate.Text.Trim()));

                SubBOM = Convert.ToDecimal(Program.ParseDecimalObject(txtTotalCost.Text.Trim()));
                SubOH = Convert.ToDecimal(Program.ParseDecimalObject(txtOHGTotal.Text.Trim()));
                Margin = Convert.ToDecimal(txtMargin.Text.Trim());
                SubTotal = SubBOM + SubOH + Margin;
                TradingSum = SubTotal * Trading / 100;
                SDSum = (SubTotal + TradingSum) * SD / 100;
                VATSum = (SubTotal + SDSum + TradingSum) * VATRate / 100;
                GrandTotal = SubTotal + TradingSum + SDSum + VATSum;



                PackPrice = Convert.ToDecimal(Program.ParseDecimalObject(txtPacketPrice.Text.Trim()));

                //txtGTotal.Text = Convert.ToDecimal(SubTotal).ToString("0");
                txtGTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), BOMDPlaceA)).ToString();


                //txtGrandTotal.Text = Convert.ToDecimal(GrandTotal).ToString("0");
                //txtGrandTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(GrandTotal.ToString(), BOMDPlaceA)).ToString();

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
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "GTotal", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", exMessage);
            }

            #endregion Catch


        }

        private void btnProductName_Click(object sender, EventArgs e)
        {

            try
            {
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                Program.fromOpen = "Other";
                Program.R_F = "Finish";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtFinishItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();// ProductInfo[0];
                    txtFProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    cmbFProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();// ProductInfo[1];
                    txtFCategoryName.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];
                    txtFHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString();//ProductInfo[11];
                    txtRSD.Text = selectedRow.Cells["SD"].Value.ToString();//ProductInfo[18];
                    txtRVATRate.Text = selectedRow.Cells["VATRate"].Value.ToString();// ProductInfo[12];
                    txtTrading.Text = selectedRow.Cells["TradingMarkUp"].Value.ToString();// ProductInfo[21];
                    ChangeData = false;


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
                FileLogger.Log(this.Name, "btnProductName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProductName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProductName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnProductName_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductName_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductName_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductName_Click", exMessage);
            }

            #endregion Catch

        }

        private void cmbFProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void BOMProductSearchDetails(string ProductID)
        {
            try
            {
                productId = ProductID;
                if (productId == "")
                {
                    return;
                }

                type = string.Empty;
                effectDate = string.Empty;
                decriptedBOMDData = string.Empty;
                BOMDLines = new string[] { };
                BOMDFields = new string[] { };

                type = cmbType.Text.Trim();

                effectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");

                this.progressBar1.Visible = true;
                bgwBOMProductSearch.RunWorkerAsync();
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

            #endregion Catch
        }

        private void bgwBOMProductSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                BOMMasterResult = new DataTable();
                BOMRawResult = new DataTable();
                OHResult = new DataTable();

                //BOMDAL bomdal = new BOMDAL();
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                string strBomId = lblBOMId.Text;

                BOMMasterResult = bomdal.SearchBOMMasterNew(BOMId, connVM);
                BOMRawResult = bomdal.SearchBOMRawNew(BOMId, connVM); // Change 04
                OHResult = bomdal.SearchOHNew(BOMId, connVM); // Change 04



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
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void bgwBOMProductSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region product search

                if (BOMRawResult != null)
                {
                    dgvBOM.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in BOMRawResult.Rows)
                    {
                        #region Declarations

                        string strUseQty = Program.ParseDecimal(item["UseQuantity"].ToString());
                        string strWastageQty =Program.ParseDecimal( item["WastageQuantity"].ToString());

                        decimal vUseQty = 0;
                        decimal vWastageQty = 0;
                        decimal vTotalQty = 0;
                        decimal vCost = 0;
                        decimal vRebateRate = 0;
                        decimal vHeadAmount = 0;

                        #endregion

                        #region Conditional Values

                        if (!string.IsNullOrEmpty(strUseQty))
                        {
                            vUseQty = Convert.ToDecimal(strUseQty);
                        }
                        if (!string.IsNullOrEmpty(strWastageQty))
                        {
                            vWastageQty = Convert.ToDecimal(strWastageQty);
                        }

                        vTotalQty = vUseQty + vWastageQty;

                        #endregion

                        #region Value Assign in Detail Grid

                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvBOM.Rows.Add(NewRow);

                        dgvBOM.Rows[j].Cells["LineNo"].Value = j + 1;
                        dgvBOM.Rows[j].Cells["RawItemNo"].Value = item["RawItemNo"].ToString();
                        dgvBOM.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                        dgvBOM.Rows[j].Cells["UOM"].Value = item["UOM"].ToString(); // BOMDFields[4].ToString();
                        dgvBOM.Rows[j].Cells["Quantity"].Value = Program.ParseDecimal(item["UseQuantity"].ToString());
                        dgvBOM.Rows[j].Cells["Wastage"].Value = item["WastageQuantity"].ToString();
                        dgvBOM.Rows[j].Cells["Cost"].Value = item["Cost"].ToString();
                        dgvBOM.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();
                        dgvBOM.Rows[j].Cells["IssueOnProduction"].Value = item["IssueOnProduction"].ToString();
                        dgvBOM.Rows[j].Cells["Status"].Value = "Old";
                        //NBR1 = item["NBRPrice"].ToString();
                        dgvBOM.Rows[j].Cells["UnitCost"].Value = item["UnitCost"].ToString();
                        dgvBOM.Rows[j].Cells["RawPCode"].Value = item["ProductCode"].ToString();
                        dgvBOM.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();
                        dgvBOM.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString(); // BOMDFields[20].ToString();
                        dgvBOM.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString(); // BOMDFields[21].ToString();
                        dgvBOM.Rows[j].Cells["UOMUQty"].Value = Program.ParseDecimal(item["UOMUQty"].ToString());
                        dgvBOM.Rows[j].Cells["UOMWQty"].Value = Program.ParseDecimal(item["UOMWQty"].ToString());
                        dgvBOM.Rows[j].Cells["RebatePercent"].Value = item["RebateRate"].ToString();
                        dgvBOM.Rows[j].Cells["TotalQty"].Value = vTotalQty;
                        dgvBOM.Rows[j].Cells["InputType"].Value = item["rawitemtype"].ToString();
                        dgvBOM.Rows[j].Cells["PBOMId"].Value = item["PBOMId"].ToString();
                        dgvBOM.Rows[j].Cells["PInvoiceNo"].Value = item["PInvoiceNo"].ToString();
                        dgvBOM.Rows[j].Cells["TransactionType"].Value = item["TransactionType"].ToString();
                        dgvBOM.Rows[j].Cells["CostPrice"].Value = "0";

                        #region Conditional Values

                        string strCost = item["Cost"].ToString();
                        string strRebateRate = item["RebateRate"].ToString();
                        if (!string.IsNullOrEmpty(strCost))
                        {
                            vCost = Convert.ToDecimal(strCost);
                        }
                        if (!string.IsNullOrEmpty(strRebateRate))
                        {
                            vRebateRate = Convert.ToDecimal(strRebateRate);
                        }
                        if (vRebateRate > 0)
                        {
                            vHeadAmount = vCost * 100 / vRebateRate;
                        }

                        #endregion

                        dgvBOM.Rows[j].Cells["HeadAmountBOM"].Value = vHeadAmount;
                        j = j + 1;

                        #endregion


                    }
                }

                #endregion product search

                #region Overhead search

                if (OHResult != null)
                {
                    dgvOverhead.Rows.Clear();

                    txtMargin.Text = "0.00";

                    int j = 0;

                    foreach (DataRow item in OHResult.Rows)
                    {
                        if (item["HeadName"].ToString().Trim() == "Margin")
                        {
                            txtMargin.Text = item["HeadAmount"].ToString();
                        }


                        if (item["HeadName"].ToString().Trim() != "Margin")
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvOverhead.Rows.Add(NewRow);
                            dgvOverhead.Rows[j].Cells["HeadName1"].Value = item["HeadName"].ToString();
                            dgvOverhead.Rows[j].Cells["HeadAmount"].Value = item["HeadAmount"].ToString();
                            dgvOverhead.Rows[j].Cells["Cost1"].Value = item["AdditionalCost"].ToString();
                            dgvOverhead.Rows[j].Cells["LineNo1"].Value = item["OHLineNo"].ToString();
                            dgvOverhead.Rows[j].Cells["Percent"].Value = item["RebatePercent"].ToString();
                            dgvOverhead.Rows[j].Cells["OHCode"].Value = item["OHCode"].ToString();
                            dgvOverhead.Rows[j].Cells["HeadID"].Value = item["HeadID"].ToString();
                            j = j + 1;

                        }

                    }

                    //for (int j = 0; j < Convert.ToInt32(OHLines.Length); j++)
                    //{
                    //    if (dgvOverhead.Rows[j].Cells["HeadName1"].Value.ToString() == "Margin")
                    //    {
                    //        dgvOverhead.Rows.RemoveAt(j);
                    //        j = OHLines.Length + 1;
                    //    }
                    //}

                    // End Complete
                }

                #endregion Overhead search

                #region BOMMaster Data

                foreach (DataRow item in BOMMasterResult.Rows)
                {


                    //BOMMasterResult
                    txtComments.Text = item["Comments"].ToString(); // BOMDFields[8].ToString();
                    txtRVATRate.Text = item["VATRate"].ToString(); // BOMDFields[10].ToString();
                    txtRSD.Text = item["SD"].ToString(); // BOMDFields[11].ToString();
                    cmbType.Text = item["VATName"].ToString(); // BOMDFields[14].ToString();
                    txtTrading.Text = item["TradingMarkUp"].ToString();
                    txtPacketPrice.Text = item["PacketPrice"].ToString();
                    txtGrandTotal.Text = item["WholeSalePrice"].ToString();
                    txtTotalCost.Text = item["RawOHCost"].ToString();
                }

                #endregion

                #region Other method call

                RowcalculateOH();

                #region Comments - Oct-01-2020

                //Rowcalculate();
                ////cmbFProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();
                ////txtFProductCode.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                ////txtFProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();
                //GTotal();
                ////txtGTotal.Text = Convert.ToDecimal(selectedRow.Cells["SalePrice"].Value.ToString()).ToString("0,0.00");

                #endregion

                #region Flag Update

                IsUpdate = true;
                ChangeData = false;

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                this.progressBar1.Visible = false;
            }

        }

        #endregion

        #region Methods 02 / Add, Change, Remove

        #region Comments Nov-01-2020

        ////private void BOMOHSearchDetails(string ProductID)
        ////{
        ////    string OHData;
        ////    try
        ////    {
        ////        productId = ProductID;
        ////        if (ProductID == "")
        ////        {
        ////            return;
        ////        }

        ////        type = string.Empty;
        ////        effectDate = string.Empty;

        ////        //OHResult = string.Empty;

        ////        OHResult = new DataTable();

        ////        decriptedOHData = string.Empty;

        ////        //OHLines = new string[] { };
        ////        //OHFields = new string[] { };

        ////        type = cmbType.Text.Trim();
        ////        effectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");



        ////        this.progressBar1.Visible = true;
        ////        bgwBOMOHSearch.RunWorkerAsync();
        ////    }
        ////    #region Catch
        ////    catch (IndexOutOfRangeException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (NullReferenceException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (FormatException ex)
        ////    {

        ////        FileLogger.Log(this.Name, "SearchDetailsOH", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }

        ////    catch (SoapHeaderException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }

        ////        FileLogger.Log(this.Name, "SearchDetailsOH", exMessage);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (SoapException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", exMessage);
        ////    }
        ////    catch (EndpointNotFoundException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (WebException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "SearchDetailsOH", exMessage);
        ////    }
        ////    #endregion Catch
        ////}
        ////private void bgwBOMOHSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        ////{
        ////    try
        ////    {
        ////        #region Statement

        ////        // Start DoWork

        ////        //BOMDAL bomdal = new BOMDAL();
        ////        //OHResult = bomdal.SearchOH(productId, effectDate, type, Program.DatabaseName); // Change 04

        ////        OHResult = new DataTable();
        ////        BOMDAL bomdal = new BOMDAL();
        ////        OHResult = bomdal.SearchOHNew(productId, effectDate, type, Program.DatabaseName); // Change 04

        ////        // End DoWork

        ////        #endregion

        ////    }
        ////    #region catch

        ////    catch (IndexOutOfRangeException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (NullReferenceException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (FormatException ex)
        ////    {

        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }

        ////    catch (SoapHeaderException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }

        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", exMessage);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (SoapException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", exMessage);
        ////    }
        ////    catch (EndpointNotFoundException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (WebException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_DoWork", exMessage);
        ////    }
        ////    #endregion
        ////}
        ////private void bgwBOMOHSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        ////{
        ////    try
        ////    {
        ////        #region Statement

        ////        //if (!string.IsNullOrEmpty(OHResult))
        ////        if (OHResult != null)
        ////        {
        ////            // Start Complete
        ////            //decriptedOHData = Converter.DESDecrypt(PassPhrase, EnKey, OHResult);
        ////            //OHLines = decriptedOHData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        ////            dgvOverhead.Rows.Clear();

        ////            txtMargin.Text = "0.00";

        ////            int j = 0;

        ////            //for (int j = 0; j < Convert.ToInt32(OHLines.Length); j++)
        ////            foreach (DataRow item in OHResult.Rows)
        ////            {
        ////                //OHFields = OHLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        ////                //if (OHFields[0].ToString() == "Margin")
        ////                if (item["HeadName"].ToString() == "Margin")
        ////                {
        ////                    txtMargin.Text = item["HeadAmount"].ToString(); //Convert.ToDecimal(OHFields[1].ToString()).ToString("0.00");
        ////                }


        ////                if (item["HeadName"].ToString() != "Margin")
        ////                {
        ////                    DataGridViewRow NewRow = new DataGridViewRow();
        ////                    dgvOverhead.Rows.Add(NewRow);
        ////                    dgvOverhead.Rows[j].Cells["HeadName1"].Value = item["HeadName"].ToString(); // OHFields[0].ToString();
        ////                    dgvOverhead.Rows[j].Cells["Cost1"].Value = item["HeadAmount"].ToString(); // Convert.ToDecimal(OHFields[1].ToString()).ToString("0.00");
        ////                    dgvOverhead.Rows[j].Cells["LineNo1"].Value = item["OHLineNo"].ToString(); // OHFields[2].ToString();
        ////                    dgvOverhead.Rows[j].Cells["Percent"].Value = item["RebatePercent"].ToString(); // OHFields[5].ToString();
        ////                    j = j + 1;

        ////                }

        ////            }

        ////            //for (int j = 0; j < Convert.ToInt32(OHLines.Length); j++)
        ////            //{
        ////            //    if (dgvOverhead.Rows[j].Cells["HeadName1"].Value.ToString() == "Margin")
        ////            //    {
        ////            //        dgvOverhead.Rows.RemoveAt(j);
        ////            //        j = OHLines.Length + 1;
        ////            //    }
        ////            //}

        ////            // End Complete
        ////        }

        ////        #endregion
        ////    }
        ////    #region catch

        ////    catch (IndexOutOfRangeException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (NullReferenceException ex)
        ////    {
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (FormatException ex)
        ////    {

        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }

        ////    catch (SoapHeaderException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }

        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", exMessage);
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        ////    }
        ////    catch (SoapException ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", exMessage);
        ////    }
        ////    catch (EndpointNotFoundException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (WebException ex)
        ////    {
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        string exMessage = ex.Message;
        ////        if (ex.InnerException != null)
        ////        {
        ////            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        ////                        ex.StackTrace;

        ////        }
        ////        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ////        FileLogger.Log(this.Name, "bgwBOMOHSearch_RunWorkerCompleted", exMessage);
        ////    }
        ////    #endregion
        ////    finally
        ////    {
        ////        this.progressBar1.Visible = false;
        ////    }
        ////}

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                #region UOMs Call

                uomsValue();

                #endregion

                #region SUM Call

                SumQty();

                #endregion

                #region Check Point

                if (Convert.ToDecimal(txtTotalQty.Text) <= 0)
                {
                    MessageBox.Show("Product quantity can't <0");
                    txtTotalQty.Text = "1";
                    return;
                }
                if (txtUomConv.Text == "0")
                {
                    MessageBox.Show("Please select Convert UOM");
                    cmbUom.Focus();
                    return;
                }
                else if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                else if (txtFinishItemNo.Text == "")
                {
                    MessageBox.Show("Finish product not selected.", this.Text);
                    return;
                }
                else if (TransType != "TollReceiveRaw" || cmbType.Text.Trim() != "VAT 4.3 (Wastage)")
                {
                    if (Convert.ToDecimal(txtRUnitCost.Text) <= 0)
                    {
                        if (
     MessageBox.Show("Cost Price Not Input\nDo you want to input without Cost price", this.Text, MessageBoxButtons.YesNo,
        MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                    else if (txtNetCost.Text == "0.00")
                    {
                        MessageBox.Show("product Not Selected of Quantity not Issued.", this.Text);
                        txtRQuantity.Focus();
                        return;
                    }
                }

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    if (dgvBOM.Rows[i].Cells["RawItemNo"].Value.ToString().Trim().ToLower() == txtRProductId.Text.Trim().ToLower())
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        return;
                    }
                }

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Set Data Grid View

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvBOM.Rows.Add(NewRow);

                int Index = dgvBOM.RowCount - 1;

                SetDataGridView(Index, "New");

                #endregion

                

                #region Row Calculation

                Rowcalculate();

                #endregion

                #region Reset Elements

                txtPBOMId.Text = "0";
                txtNetCost.Text = "0";
                txtRWastage.Text = "0";
                txtRQuantity.Text = "0";
                txtTotalQty.Text = "0";
                txtPInvoiceNo.Text = "";

                #endregion

                #region Focus

                cmbRProduct.Focus();

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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }

            #endregion Catch
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {

                #region UOMs Call

                uomsValue();

                #endregion

                #region SUM Call

                SumQty();

                #endregion

                #region Check Point

                if (txtUomConv.Text == "0")
                {
                    MessageBox.Show("Please select Convert UOM");
                    cmbUom.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtTotalQty.Text) <= 0)
                {
                    MessageBox.Show("Product quantity can't <0");
                    txtTotalQty.Text = "1";
                    return;
                }
                if (cmbType.Text == "VAT 1 Kha (Trading)")
                {

                    if (txtFinishItemNo.Text.Trim() != txtRProductCode.Text.Trim())
                    {
                        MessageBox.Show(
                            "For trading item raw and finish product must same, Please select right product", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (Convert.ToDecimal(txtRQuantity.Text) <= 0)
                    {
                        MessageBox.Show("Product quantity can't >1");
                        txtRQuantity.Text = "1";
                        return;
                    }
                }
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (txtRProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item");
                    return;
                }
                if (TransType != "TollReceiveRaw" || cmbType.Text.Trim() != "VAT 4.3 (Wastage)")
                {
                    if (Convert.ToDecimal(txtRUnitCost.Text) <= 0)
                    {
                        if (
                            MessageBox.Show("Cost Price Not Input\nDo you want to input without Cost price", this.Text, MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }

                        //MessageBox.Show("Please select the unit cost.", this.Text);
                        //txtRUnitCost.Focus();
                        //return;
                    }
                    else if (txtNetCost.Text == "0.00")
                    {
                        MessageBox.Show("product Not Selected of Quantity not Issued.", this.Text);
                        txtRQuantity.Focus();
                        return;
                    }
                }

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Set DataGridView

                int Index = dgvBOM.CurrentRow.Index;
                SetDataGridView(Index, "Change");

                

                #endregion

                #region Row Calculate

                Rowcalculate();

                #endregion

                #region Reset Elements

                txtPBOMId.Text = "0";
                txtNetCost.Text = "0";
                txtRWastage.Text = "0";
                txtRQuantity.Text = "0";
                txtTotalQty.Text = "0";

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
                FileLogger.Log(this.Name, "btnChange_Click_1", exMessage);
            }

            #endregion Catch
        }

        private void SetDataGridView(int paramIndex, string RowType)
        {
            #region Set Data Grid View

            decimal TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
            decimal RQuantity = Convert.ToDecimal(txtRQuantity.Text.Trim());
            decimal UomConv = Convert.ToDecimal(txtUomConv.Text.Trim());

            dgvBOM["IssueOnProduction", paramIndex].Value = Convert.ToString(chkIssueOnProduction.Checked == true ? "Y" : "N");

            dgvBOM["RawItemNo", paramIndex].Value = txtRProductId.Text.Trim();
            dgvBOM["RawPCode", paramIndex].Value = txtRProductCode.Text.Trim();
            dgvBOM["ProductName", paramIndex].Value = txtRProductName.Text.Trim();

            dgvBOM["Quantity", paramIndex].Value = RQuantity;
            dgvBOM["Wastage", paramIndex].Value = Convert.ToDecimal(txtRWastage.Text.Trim());
            dgvBOM["UOM", paramIndex].Value = cmbUom.Text.Trim();
            dgvBOM["UOMn", paramIndex].Value = txtRUOM.Text.Trim();
            dgvBOM["UOMPrice", paramIndex].Value = txtRUnitCost.Text.Trim();
            dgvBOM["UOMc", paramIndex].Value = txtUomConv.Text.Trim();
            dgvBOM["UOMUQty", paramIndex].Value = RQuantity * UomConv;
            dgvBOM["UOMWQty", paramIndex].Value = (TotalQty - TotalQty) * UomConv;
            dgvBOM["UnitCost", paramIndex].Value = (Convert.ToDecimal(txtRUnitCost.Text.Trim()) * UomConv).ToString();
            dgvBOM["Cost", paramIndex].Value = Convert.ToDecimal(Convert.ToDecimal(txtNetCost.Text.Trim())).ToString();
            dgvBOM["VATRate", paramIndex].Value = 0;
            dgvBOM["SD", paramIndex].Value = 0;
            dgvBOM["PBOMId", paramIndex].Value = txtPBOMId.Text.Trim();
            dgvBOM["TransactionType", paramIndex].Value = TransType;

            if (oldNbrPrice != Convert.ToDecimal(txtRUnitCost.Text))
            {
                txtPInvoiceNo.Text = "0";
            }
            if (string.IsNullOrEmpty(txtPInvoiceNo.Text.Trim()))
            {
                txtPInvoiceNo.Text = "0";
            }

            dgvBOM["PInvoiceNo", paramIndex].Value = txtPInvoiceNo.Text.Trim();

            dgvBOM["CostPrice", dgvBOM.CurrentRow.Index].Value = "0";
            dgvBOM["Mark", paramIndex].Value = "Raw";
            dgvBOM["RebatePercent", paramIndex].Value = "0";
            dgvBOM["HeadAmountBOM", paramIndex].Value = "0";

            dgvBOM["Status", paramIndex].Value = "New";

            if (RowType == "Change")
            {
                dgvBOM["Status", paramIndex].Value = "Change";
                dgvBOM.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvBOM.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
            }


            dgvBOM["ActiveStatus", paramIndex].Value = "Y";

            #region Linq Select

            var tt20 = from pit in ProductsMini.ToList()
                       where pit.ProductCode.ToLower() == txtRProductCode.Text.Trim().ToLower()
                       select pit;


            if (tt20 != null && tt20.Any())
            {
                var ttiputtype = tt20.First().IsRaw;
                dgvBOM["InputType", paramIndex].Value = ttiputtype;
            }

            #endregion

            #endregion

        }


        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBOM.CurrentRow != null)
                {


                    uomsValue();

                    if (dgvBOM["InputType", dgvBOM.CurrentRow.Index].Value.ToString() == "Overhead")
                    {
                        MessageBox.Show("This item is not Raw, you can't remove");
                        return;
                    }
                    if (cmbType.Text == "VAT 1 Kha (Trading)")
                    {
                        if (txtFinishItemNo.Text.Trim() != txtRProductCode.Text.Trim())
                        {
                            MessageBox.Show(
                                "For trading item raw and finish product must same, Please select right product",
                                this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    if (cmbType.Text == "")
                    {
                        MessageBox.Show("Please select the VAT name.", this.Text);
                        cmbType.Focus();
                        return;
                    }
                    if (txtRProductCode.Text == "")
                    {
                        MessageBox.Show("Please select a Item", this.Text);
                        return;
                    }

                    if (Convert.ToDecimal(dgvBOM.CurrentCellAddress.Y) < 0)
                    {
                        MessageBox.Show("There have no Data to Delete", this.Text);
                        return;
                    }
                    ChangeData = true;

                    dgvBOM.Rows.RemoveAt(dgvBOM.CurrentRow.Index);
                     
                    Rowcalculate();
                    txtNetCost.Text = "0";
                    txtRWastage.Text = "0";
                    txtRQuantity.Text = "0";
                    txtTotalQty.Text = "0";
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion Catch
        }


        private void ProductDetailsInfoF()
        {
            try
            {
                txtRWastage.Enabled = true;
                btnSerachGroup.Enabled = true;
                button1.Enabled = true;
                txtRQuantity.Enabled = true;
                chkPRCode.Enabled = true;
                chkWastage.Enabled = true;
                var searchText = cmbFProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {


                    if (chkPFCode.Checked)
                    {
                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                               && prd.CategoryID == CategoryIdF
                                         orderby prd.ProductCode
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();

                            txtFProductName.Text = products.ProductName;
                            txtFinishItemNo.Text = products.ItemNo;
                            txtFHSCode.Text = products.HSCodeNo;
                            txtRVATRate.Text = products.VATRate.ToString();
                            txtRSD.Text = products.SD.ToString();
                            txtTrading.Text = products.TradingMarkUp.ToString();
                            txtFProductCode.Text = products.ProductCode;
                            txtFUOM.Text = products.UOM;
                            if (chkLIFO.Checked)
                            {
                                txtFIssuePrice.Text = products.LIFOPrice.ToString();

                            }
                            else
                            {
                                txtFIssuePrice.Text = products.IssuePrice.ToString();

                            }

                            if (cmbType.Text.Trim() == "VAT 1 Kha (Trading)")
                            {
                                txtRCategoryName.Text = txtFCategoryName.Text;
                                cmbRProduct.Text = cmbFProduct.Text;
                                txtRProductName.Text = products.ProductName;
                                txtRProductCode.Text = products.ProductCode;
                                txtRHSCode.Text = products.HSCodeNo;
                                txtRProductId.Text = products.ItemNo;
                                txtRUOM.Text = products.UOM;
                                cmbUom.Text = products.UOM;

                                //txtRWastage.Text = "0.0";
                                //txtRQuantity.Text = "1";

                                //txtRWastage.Enabled = false;
                                //btnSerachGroup.Enabled = false;
                                //button1.Enabled = false;
                                //txtRQuantity.Enabled = false;
                                //chkPRCode.Enabled = false;
                                //chkWastage.Enabled = false;

                            }
                            //s
                            else if (cmbType.Text.Trim() == "VAT 4.3 (Internal Issue)")
                            {
                                txtRCategoryName.Text = txtFCategoryName.Text;
                                cmbRProduct.Text = cmbFProduct.Text;
                                txtRProductName.Text = products.ProductName;
                                txtRProductCode.Text = products.ProductCode;
                                txtRHSCode.Text = products.HSCodeNo;
                                txtRProductId.Text = products.ItemNo;
                                txtRUOM.Text = products.UOM;
                                cmbUom.Text = products.UOM;

                                //txtRWastage.Text = "0.0";
                                //txtRQuantity.Text = "1";

                                //txtRWastage.Enabled = false;
                                //btnSerachGroup.Enabled = false;
                                //button1.Enabled = false;
                                //txtRQuantity.Enabled = false;
                                //chkPRCode.Enabled = false;
                                //chkWastage.Enabled = false;

                            }

                        }
                    }
                    else
                    {
                        var prodByName = from prd in ProductsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                               && prd.CategoryID == CategoryIdF
                                         orderby prd.ProductName
                                         select prd;

                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();

                            txtFProductName.Text = products.ProductName;
                            txtFinishItemNo.Text = products.ItemNo;
                            txtFHSCode.Text = products.HSCodeNo;
                            txtRVATRate.Text = products.VATRate.ToString();
                            txtRSD.Text = products.SD.ToString();
                            txtTrading.Text = products.TradingMarkUp.ToString();
                            txtFProductCode.Text = products.ProductCode;
                            txtFUOM.Text = products.UOM;


                            if (chkLIFO.Checked)
                            {
                                txtFIssuePrice.Text = products.LIFOPrice.ToString();

                            }
                            else
                            {
                                txtFIssuePrice.Text = products.IssuePrice.ToString();

                            }

                            if (cmbType.Text.Trim() == "VAT 1 Kha (Trading)")
                            {
                                txtRCategoryName.Text = txtFCategoryName.Text;
                                cmbRProduct.Text = cmbFProduct.Text;
                                txtRProductName.Text = products.ProductName;
                                txtRProductCode.Text = products.ProductCode;
                                txtRHSCode.Text = products.HSCodeNo;
                                txtRProductId.Text = products.ItemNo;
                                txtRUOM.Text = products.UOM;
                                cmbUom.Text = products.UOM;

                                txtRWastage.Text = "0.0";
                                txtRQuantity.Text = "1";

                                txtRWastage.Enabled = false;
                                btnSerachGroup.Enabled = false;
                                button1.Enabled = false;
                                txtRQuantity.Enabled = false;
                                chkPRCode.Enabled = false;
                                chkWastage.Enabled = false;

                            }

                            else if (cmbType.Text.Trim() == "VAT 4.3 (Internal Issue)")
                            {
                                txtRCategoryName.Text = txtFCategoryName.Text;
                                cmbRProduct.Text = cmbFProduct.Text;
                                txtRProductName.Text = products.ProductName;
                                txtRProductCode.Text = products.ProductCode;
                                txtRHSCode.Text = products.HSCodeNo;
                                txtRProductId.Text = products.ItemNo;
                                txtRUOM.Text = products.UOM;
                                cmbUom.Text = products.UOM;

                                //txtRWastage.Text = "0.0";
                                //txtRQuantity.Text = "1";

                                //txtRWastage.Enabled = false;
                                //btnSerachGroup.Enabled = false;
                                //button1.Enabled = false;
                                //txtRQuantity.Enabled = false;
                                //chkPRCode.Enabled = false;
                                //chkWastage.Enabled = false;

                            }
                        }
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

            #endregion Catch

        }

        private void ProductDetailsInfoR()
        {
            try
            {
                var searchText = cmbRProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {


                    if (chkPRCode.Checked)
                    {
                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtRProductName.Text = products.ProductName;
                            txtRProductId.Text = products.ItemNo;


                            txtRUOM.Text = products.UOM;
                            txtRHSCode.Text = products.HSCodeNo;
                            txtRProductCode.Text = products.ProductCode;

                        }
                    }
                    else
                    {
                        var prodByName = from prd in ProductsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                         select prd;

                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtRProductName.Text = products.ProductName;
                            txtRProductId.Text = products.ItemNo;

                            txtRUOM.Text = products.UOM;
                            txtRHSCode.Text = products.HSCodeNo;
                            txtRProductCode.Text = products.ProductCode;
                        }
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
                FileLogger.Log(this.Name, "ProductDetailsInfoR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductDetailsInfoR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductDetailsInfoR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductDetailsInfoR", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductDetailsInfoR", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductDetailsInfoR", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductDetailsInfoR", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductDetailsInfoR", exMessage);
            }

            #endregion Catch

        }

        private void txtFProductCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvBOM_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvBOM.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (IsImport == true)
                {
                    return;
                }
                string inputTypeValue = dgvBOM.CurrentRow.Cells["InputType"].Value.ToString();
                if (!string.IsNullOrEmpty(inputTypeValue))
                    if (inputTypeValue == "Overhead")
                    {
                        MessageBox.Show("Selected data is Overhead item\nplease select Overhead items from Overhead Details", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {

                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.ItemNo.ToLower() == dgvBOM.CurrentRow.Cells["RawItemNo"].Value.ToString().ToLower()
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtRHSCode.Text = products.HSCodeNo;
                            CategoryIdR = products.CategoryID;
                            txtRCategoryName.Text = products.CategoryName;

                            //txtRProductName.Text = products.ProductName;
                            //txtRProductId.Text = products.ItemNo;

                            //txtRUOM.Text = products.UOM;
                            //txtRHSCode.Text = products.HSCodeNo;
                            //txtRProductCode.Text = products.ProductCode;

                        }
                        if (chkPRCode.Checked == true)
                        {
                            cmbRProduct.Text = dgvBOM.CurrentRow.Cells["RawPCode"].Value.ToString();
                        }
                        else
                        {
                            cmbRProduct.Text = dgvBOM.CurrentRow.Cells["ProductName"].Value.ToString();

                        }
                        //dgvBOM["RawItemNo", dgvBOM.RowCount - 1].Value = txtRProductId.Text.Trim();
                        //dgvBOM["RawPCode", dgvBOM.RowCount - 1].Value = txtRProductCode.Text.Trim();

                        txtRProductId.Text = dgvBOM.CurrentRow.Cells["RawItemNo"].Value.ToString();
                        txtRProductCode.Text = dgvBOM.CurrentRow.Cells["RawPCode"].Value.ToString();

                        txtRProductName.Text = dgvBOM.CurrentRow.Cells["ProductName"].Value.ToString();
                        cmbUom.Text = dgvBOM.CurrentRow.Cells["UOM"].Value.ToString();


                        txtRUOM.Text = dgvBOM.CurrentRow.Cells["UOMn"].Value.ToString();

                        txtTotalQty.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["TotalQty"].Value).ToString();
                        txtUomConv.Text =
                            Convert.ToDecimal(dgvBOM.CurrentRow.Cells["UOMc"].Value).ToString();
                        txtNetCost.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Cost"].Value).ToString();

                        txtRQuantity.Text =
                            Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Quantity"].Value).ToString();
                        txtRWastage.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Wastage"].Value).ToString();
                        txtRWastageInp.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Wastage"].Value).ToString();
                        txtRUnitCost.Text = dgvBOM.CurrentRow.Cells["UOMPrice"].Value.ToString();
                        oldNbrPrice =Convert.ToDecimal( txtRUnitCost.Text);
                        txtPInvoiceNo.Text = dgvBOM.CurrentRow.Cells["PInvoiceNo"].Value.ToString();
                        chkIssueOnProduction.Checked = Convert.ToBoolean(dgvBOM.CurrentRow.Cells["IssueOnProduction"].Value.ToString() == "Y" ? true : false);



                        //txtRVATRate.Text = "0.00";
                        //txtRSD.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["SD"].Value).ToString("0.00"); 

                        txtPBOMId.Text = dgvBOM.CurrentRow.Cells["PBOMId"].Value.ToString();
                        ProductSearchDsLoadR();
                        chkWastage.Checked = true;
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
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
            }

            #endregion Catch
        }



        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                MessageBox.Show(MessageVM.msgWantToRemoveRow, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                sqlResults = new string[3];

                //BOMDAL bomDal = new BOMDAL();
                IBOM bomDal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                sqlResults = bomDal.DeleteBOM(txtFinishItemNo.Text.Trim(), cmbType.Text, dtpEffectDate.Value.ToString(), null,
                                                null, null, connVM);

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];

                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("btnCancel_Click", "Unexpected error.");
                    }
                    else if (result == "Success")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AllClear();
                        ChangeData = false;
                    }
                    else if (result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }

            #endregion Catch
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void RowcalculateOH()
        {
            try
            {
                decimal OHCost = 0;
                for (int i = 0; i < dgvOverhead.RowCount; i++)
                {
                    dgvOverhead["LineNo1", i].Value = i + 1;
                    OHCost = OHCost + Convert.ToDecimal(dgvOverhead["Cost1", i].Value);
                }

                //txtOHGTotal.Text = Convert.ToDecimal(OHCost).ToString("0.00");
                txtOHGTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(OHCost.ToString(), BOMDPlaceA)).ToString();
                GTotal();
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
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
            }

            #endregion Catch
        }

        #endregion

        #region Methods 03 / Save, Update, Import

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                //if (IsPost == true)
                //{
                //    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                //    return;
                //}
                //lblBOMId.Text = "";
                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #region Null Check

                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (cmbType.Text.Trim() != "VAT 4.3 (Wastage)")
                {
                    if (Convert.ToDecimal(txtTotalCost.Text.Trim()) <= 0)
                    {
                        MessageBox.Show("Price not issued.", this.Text);
                        txtGTotal.Focus();
                        return;
                    }

                }



                if (txtFinishItemNo.Text == "" && IsImport == false)
                {
                    MessageBox.Show("Finish product not selected.", this.Text);
                    txtFinishItemNo.Focus();
                    return;
                }
                if (dgvBOM.RowCount <= 0)
                {
                    MessageBox.Show("There is no data in raw product grid", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (cmbType.Text.Trim() == "VAT 1")
                {
                    if (dgvOverhead.RowCount <= 0)
                    {
                        MessageBox.Show("There is no data in overhead grid", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                }


                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtMasterComments.Text == "")
                {
                    txtMasterComments.Text = "-";
                }


                #endregion

                #endregion

                #region Value Assign

                #region Details Data

                #region Item Details

                #region Declarations

                bomItems = new List<BOMItemVM>();
                string strVatRate = txtRVATRate.Text.Trim();
                string strSDRate = txtRSD.Text.Trim();
                decimal vMarkupPercent = 0;
                decimal vMarkupValue = 0;
                decimal vVateRate = 0;
                decimal vSDRate = 0;
                decimal vVatAmount = 0;
                decimal vSDAmount = 0;
                decimal vRawTotal = 0;
                decimal vPackingTotal = 0;
                decimal vRebateTotal = 0;
                decimal vAdditionalTotal = 0;
                decimal vRebatePercent = 0;

                decimal vSubTotal = 0;
                string strSubTotal = "0";
                string strRawItemType = string.Empty;
                string pInvNo = string.Empty;

                #endregion

                #region Conditional Values

                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vVateRate = Convert.ToDecimal(strVatRate);
                }
                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vSDRate = Convert.ToDecimal(strSDRate);
                }

                string strMarkupPercent = txtTrading.Text;

                if (!string.IsNullOrEmpty(strMarkupPercent))
                {
                    vMarkupPercent = Convert.ToDecimal(strMarkupPercent);
                }

                #endregion

                #region Raw Material

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {

                    vSubTotal = 0;
                    strRawItemType = dgvBOM.Rows[i].Cells["InputType"].Value.ToString();
                    strSubTotal = dgvBOM.Rows[i].Cells["Cost"].Value.ToString();

                    if (!string.IsNullOrEmpty(strSubTotal))
                    {
                        vSubTotal = Convert.ToDecimal(strSubTotal);
                    }

                    if (!string.IsNullOrEmpty(strRawItemType))
                    {
                        if (strRawItemType != "Pack" && strRawItemType != "Overhead")
                        {
                            vRawTotal = vRawTotal + vSubTotal;
                        }
                        else if (strRawItemType == "Pack")
                        {
                            vPackingTotal = vPackingTotal + vSubTotal;
                        }
                        else if (strRawItemType == "Overhead")
                        {
                            vRebateTotal = vRebateTotal + vSubTotal;
                        }
                    }
                    //}
                    vMarkupValue = vSubTotal * vMarkupPercent / 100;
                    vSDAmount = (vSubTotal + vMarkupValue) * vSDRate / 100;
                    vVatAmount = (vSubTotal + vMarkupValue + vSDAmount) * vVateRate / 100;

                    //vRebatePercent
                    string strRebatePercent = dgvBOM.Rows[i].Cells["RebatePercent"].Value.ToString();
                    if (!string.IsNullOrEmpty(strRebatePercent))
                    {
                        vRebatePercent = Convert.ToDecimal(strRebatePercent);
                    }

                    //retrive Purcahse invoice no

                    pInvNo = dgvBOM.Rows[i].Cells["PInvoiceNo"].Value.ToString();

                    BOMItemVM bomItem = new BOMItemVM();
                    bomItem.RebateRate = vRebatePercent;
                    if (IsImport == false)
                    {
                        bomItem.FinishItemNo = txtFinishItemNo.Text.Trim();
                        bomItem.RawItemNo = dgvBOM.Rows[i].Cells["RawItemNo"].Value.ToString();

                        bomItem.UOMPrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMPrice"].Value.ToString());
                        bomItem.UOMc = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMc"].Value.ToString());
                        bomItem.UOMn = dgvBOM.Rows[i].Cells["UOMn"].Value.ToString();
                        bomItem.UOMUQty = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMUQty"].Value.ToString());
                        bomItem.UOMWQty = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMWQty"].Value.ToString());
                        bomItem.PBOMId = dgvBOM.Rows[i].Cells["PBOMId"].Value.ToString();

                    }
                    bomItem.IssueOnProduction = dgvBOM.Rows[i].Cells["IssueOnProduction"].Value.ToString();
                    bomItem.RawOHCost = Convert.ToDecimal(Convert.ToDecimal(txtTotalCost.Text.Trim()) +
                        Convert.ToDecimal(txtMargin.Text) + Convert.ToDecimal(txtOHGTotal.Text.Trim()));
                    bomItem.Comments = txtComments.Text.Trim();
                    bomItem.ActiveStatus = "Y";

                    bomItem.UseQuantity = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Quantity"].Value);
                    bomItem.UnitCost = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UnitCost"].Value.ToString());
                    bomItem.RawItemName = dgvBOM.Rows[i].Cells["ProductName"].Value.ToString();
                    bomItem.WastageQuantity = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Wastage"].Value);
                    bomItem.TotalQuantity = Convert.ToDecimal(dgvBOM.Rows[i].Cells["TotalQty"].Value);


                    bomItem.CreatedBy = Program.CurrentUser;
                    bomItem.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomItem.LastModifiedBy = Program.CurrentUser;
                    bomItem.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomItem.UOM = dgvBOM.Rows[i].Cells["UOM"].Value.ToString();
                    bomItem.VATRate = Convert.ToDecimal(txtRVATRate.Text.Trim());
                    bomItem.SD = Convert.ToDecimal(txtRSD.Text.Trim());
                    bomItem.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                    bomItem.Cost = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Cost"].Value.ToString());

                    bomItem.BOMLineNo = dgvBOM.Rows[i].Cells["LineNo"].Value.ToString();
                    bomItem.PacketPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                    bomItem.NBRPrice = Convert.ToDecimal(txtGTotal.Text.Trim());
                    bomItem.TradingMarkUp = Convert.ToDecimal(txtTrading.Text.Trim());
                    bomItem.RawItemType = dgvBOM.Rows[i].Cells["InputType"].Value.ToString();
                    bomItem.Post = "N";
                    bomItem.TradingMarkUp = vMarkupPercent;
                    bomItem.SDAmount = vSDAmount;
                    bomItem.MarkUpValue = vMarkupValue;
                    bomItem.VatAmount = vVatAmount;
                    //get purchase invoice no
                    bomItem.PInvoiceNo = dgvBOM.Rows[i].Cells["PInvoiceNo"].Value.ToString();
                    if (string.IsNullOrEmpty(bomItem.PInvoiceNo.Trim()))
                    {
                        bomItem.PInvoiceNo = "0";
                    }
                    if (string.IsNullOrEmpty(dgvBOM.Rows[i].Cells["TransactionType"].Value.ToString()))
                    {
                        bomItem.TransactionType = "0";
                    }
                    else
                    {
                        bomItem.TransactionType = dgvBOM.Rows[i].Cells["TransactionType"].Value.ToString();
                    }


                    bomItem.BranchId = Program.BranchId;
                    bomItems.Add(bomItem);



                }
                #endregion RAW

                #endregion Item

                #region Overhead - OH

                bomOhs = new List<BOMOHVM>();
                for (int i = 0; i < dgvOverhead.RowCount; i++)
                {
                    BOMOHVM bomOh = new BOMOHVM();
                    decimal vRebateAmount = 0;
                    decimal vAdditionalCost = 0;
                    decimal vHeadAmount = 0;
                    decimal vAdditionalPercent = 0;


                    if (dgvOverhead.Rows[i].Cells["Percent"].Value != null)
                        vAdditionalPercent = Convert.ToDecimal(dgvOverhead.Rows[i].Cells["Percent"].Value.ToString());
                    if (dgvOverhead.Rows[i].Cells["HeadAmount"].Value != null)
                        vHeadAmount = Convert.ToDecimal(dgvOverhead.Rows[i].Cells["HeadAmount"].Value.ToString());


                    vAdditionalCost = vHeadAmount * vAdditionalPercent / 100;
                    vRebateAmount = vHeadAmount - vAdditionalCost;
                    bomOh.RebateAmount = vRebateAmount;
                    bomOh.AdditionalCost = vAdditionalCost;

                    bomOh.HeadName = dgvOverhead.Rows[i].Cells["HeadName1"].Value.ToString();
                    bomOh.HeadID = dgvOverhead.Rows[i].Cells["HeadID"].Value.ToString();
                    bomOh.OHCode = dgvOverhead.Rows[i].Cells["OHCode"].Value.ToString();

                    bomOh.HeadAmount = vHeadAmount;
                    bomOh.CreatedBy = Program.CurrentUser;
                    bomOh.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh.LastModifiedBy = Program.CurrentUser;
                    bomOh.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh.FinishItemNo = txtFinishItemNo.Text.Trim();
                    bomOh.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                    bomOh.OHLineNo = dgvOverhead.Rows[i].Cells["LineNo1"].Value.ToString();
                    bomOh.RebatePercent = vAdditionalPercent;
                    bomOh.Post = "N";

                    bomOh.BranchId = Program.BranchId;
                    vAdditionalTotal = vAdditionalTotal + vAdditionalCost;

                    bomOhs.Add(bomOh);



                }
                BOMOHVM bomOh1 = new BOMOHVM();
                bomOh1.HeadName = "Margin";
                bomOh1.OHCode = "ovh0";
                bomOh1.HeadID = "ovh0";

                bomOh1.HeadAmount = Convert.ToDecimal(txtMargin.Text.Trim());
                bomOh1.CreatedBy = Program.CurrentUser;
                bomOh1.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomOh1.LastModifiedBy = Program.CurrentUser;
                bomOh1.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomOh1.FinishItemNo = txtFinishItemNo.Text.Trim();
                bomOh1.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                bomOh1.OHLineNo = "100";
                bomOh1.RebatePercent = 0;
                bomOh1.Post = "N";
                bomOh1.AdditionalCost = bomOh1.HeadAmount;

                bomOh1.BranchId = Program.BranchId;
                bomOhs.Add(bomOh1);
                vAdditionalTotal = vAdditionalTotal + bomOh1.HeadAmount;

                #endregion OH

                #endregion

                #region Master Data

                #region BOMNBR - NBR / Master

                //BOMNBRVM bomNbr = new BOMNBRVM();
                if (IsImport == false)
                {
                    bomNbrs.ItemNo = txtFinishItemNo.Text.Trim();

                }
                bomNbrs.ReferenceNo = txtReferenceNo.Text.Trim();


                bomNbrs.FinishItemName = txtFProductName.Text.Trim();
                bomNbrs.Margin = Convert.ToDecimal(txtMargin.Text.Trim());
                bomNbrs.VATName = cmbType.Text.Trim();
                var tt1 = Convert.ToDecimal(txtGTotal.Text.Trim());
                var tt2 = tt1 + tt1 * vMarkupPercent / 100;
                bomNbrs.PNBRPrice = tt2; // vp
                bomNbrs.PPacketPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                bomNbrs.ReceiveCost = Convert.ToDecimal(Convert.ToDecimal(txtTotalCost.Text.Trim()) +
                                      Convert.ToDecimal(txtOHGTotal.Text.Trim()));
                var tt3 = tt2 + tt2 * vSDRate / 100;
                bomNbrs.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                bomNbrs.FirstSupplyDate = dtpFirstSupplyDate.Value.ToString("yyyy-MMM-dd");
                //bomItem.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd"));
                bomNbrs.CreatedBy = Program.CurrentUser;
                bomNbrs.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomNbrs.LastModifiedBy = Program.CurrentUser;
                bomNbrs.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomNbrs.ActiveStatus = "Y";
                bomNbrs.Post = "N";
                bomNbrs.RawTotal = vRawTotal;
                bomNbrs.PackingTotal = vPackingTotal;
                bomNbrs.RebateTotal = vRebateTotal;
                bomNbrs.AdditionalTotal = vAdditionalTotal;
                bomNbrs.RebateAdditionTotal = vAdditionalTotal + vRebateTotal;
                bomNbrs.FUOMc = Convert.ToDecimal(uomsValue(txtFUOM.Text, cmbFUOM.Text));

                //if (bomNbrs.FUOMc == 0)
                //{
                //    MessageBox.Show("Conversion not found for Finish Product");
                //    return;
                //}

                bomNbrs.FUOMPrice = bomNbrs.PNBRPrice * bomNbrs.FUOMc;
                bomNbrs.FUOMn = txtFUOM.Text;

                if (cmbType.Text.Trim() == "VAT 4.3 (Toll Issue)")
                {
                    if (TollIssueCostWithOthers == true)
                    {
                        bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;
                    }
                    else
                    {
                        bomNbrs.RawOHCost = vRebateTotal;
                    }

                    //bomNbrs.RawOHCost = vRebateTotal;
                }
                else
                {
                    bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;

                }

                bomNbrs.VATRate = vVateRate;
                bomNbrs.SDRate = vSDRate;
                bomNbrs.TradingMarkup = vMarkupPercent;
                bomNbrs.RebateRate = vRebatePercent;
                bomNbrs.Comments = txtComments.Text.Trim();
                bomNbrs.MasterComments = txtMasterComments.Text.Trim();

                bomNbrs.UOM = cmbFUOM.Text.Trim();
                bomNbrs.NBRWithSDAmount = tt3;
                bomNbrs.CustomerID = CustomerID;

                bomNbrs.BranchId = Program.BranchId;
                bomNbrs.AutoIssue = chkAutoIssue.Checked?"Y":"N";
                #endregion NBR

                #endregion

                #endregion

                #region Element States

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker

                if (IsImport == true)
                {
                    bgwImport.RunWorkerAsync();
                }
                else
                {
                    bgwSave.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion Catch
        }

        private void bgwImport_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];



                sqlResults = BOMImport();

                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void bgwImport_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

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
                        if (!string.IsNullOrEmpty(newId))
                        {
                            lblBOMId.Text = newId;
                        }


                    }
                // Start Complete



                // End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void bgwSave_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                #region assign summary values

                decimal vVatRate = 0;
                decimal vVatAmount = 0;
                decimal vNBRPrice = 0;
                decimal vSDPercent = 0;
                decimal vSDAmount = 0;
                decimal vWholeSaleAmount = 0;
                decimal vMarkupPercent = 0;
                decimal vMarkupValue = 0;

                string strVatRate = txtRVATRate.Text;

                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vVatRate = Convert.ToDecimal(strVatRate);
                }

                string strNNRPrice = txtGTotal.Text;
                if (!string.IsNullOrEmpty(strNNRPrice))
                {
                    vNBRPrice = Convert.ToDecimal(strNNRPrice);
                }

                string strSDPercent = txtRSD.Text;
                if (!string.IsNullOrEmpty(strSDPercent))
                {
                    vSDPercent = Convert.ToDecimal(strSDPercent);
                }

                string strMarkupPercent = txtTrading.Text;
                if (!string.IsNullOrEmpty(strMarkupPercent))
                {
                    vMarkupPercent = Convert.ToDecimal(strMarkupPercent);
                }


                vMarkupValue = vNBRPrice * vMarkupPercent / 100;
                vSDAmount = (vNBRPrice + vMarkupValue) * vSDPercent / 100;
                bomNbrs.NBRWithSDAmount = vNBRPrice + vSDAmount + vMarkupValue;
                //vNBRPrice = vNBRPrice + vSDAmount;

                vVatAmount = (vNBRPrice + vMarkupValue + vSDAmount) * vVatRate / 100;
                //vWholeSaleAmount = vNBRPrice + vVatAmount;
                //vWholeSaleAmount = vNBRPrice + vVatAmount + vMarkupValue + vSDAmount;
                vWholeSaleAmount = Convert.ToDecimal(Program.ParseDecimal(txtGrandTotal.Text.Trim()));

                bomNbrs.VatAmount = vVatAmount;
                bomNbrs.SDAmount = vSDAmount;
                bomNbrs.MarkupValue = vMarkupValue;

                bomNbrs.WholeSalePrice = vWholeSaleAmount;
                bomNbrs.CustomerID = CustomerID;

                #endregion

                BOMRawResult = new DataTable();

                #region Insert Data

                IBOM bom = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);
                sqlResults = bom.BOMInsert(bomItems, bomOhs, bomNbrs, connVM);

                #endregion

                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void bgwSave_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

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
                            //MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsPost = false;
                        }

                        if (!string.IsNullOrEmpty(newId))
                        {
                            lblBOMId.Text = newId;
                            BOMId = newId;
                        }


                    }
                // Start Complete



                // End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement



                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                #region assign summary values

                decimal vVatRate = 0;
                decimal vVatAmount = 0;
                decimal vNBRPrice = 0;
                decimal vSDPercent = 0;
                decimal vSDAmount = 0;
                decimal vWholeSaleAmount = 0;
                decimal vMarkupPercent = 0;
                decimal vMarkupValue = 0;

                string strVatRate = txtRVATRate.Text;
                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vVatRate = Convert.ToDecimal(strVatRate);
                }
                string strNNRPrice = txtGTotal.Text;
                if (!string.IsNullOrEmpty(strNNRPrice))
                {
                    vNBRPrice = Convert.ToDecimal(strNNRPrice);
                }
                string strSDPercent = txtRSD.Text;
                if (!string.IsNullOrEmpty(strSDPercent))
                {
                    vSDPercent = Convert.ToDecimal(strSDPercent);
                }
                string strMarkupPercent = txtTrading.Text;
                if (!string.IsNullOrEmpty(strMarkupPercent))
                {
                    vMarkupPercent = Convert.ToDecimal(strMarkupPercent);
                }


                vMarkupValue = vNBRPrice * vMarkupPercent / 100;
                vSDAmount = (vNBRPrice + vMarkupValue) * vSDPercent / 100;
                bomNbrs.NBRWithSDAmount = vNBRPrice + vSDAmount + vMarkupValue;
                //vNBRPrice = vNBRPrice + vSDAmount;

                vVatAmount = (vNBRPrice + vMarkupValue + vSDAmount) * vVatRate / 100;
                //vWholeSaleAmount = vNBRPrice + vVatAmount;
                //vWholeSaleAmount = vNBRPrice + vVatAmount + vMarkupValue + vSDAmount;
                vWholeSaleAmount = Convert.ToDecimal(Program.ParseDecimal(txtGrandTotal.Text.Trim()));
                if (bomNbrs.VATName.ToLower() == "vat porisisto ka")
                {
                    vSDAmount = (vNBRPrice) * vSDPercent / 100;
                    vVatAmount = (vNBRPrice) * vVatRate / 100;

                }
                bomNbrs.VatAmount = vVatAmount;
                bomNbrs.SDAmount = vSDAmount;
                bomNbrs.MarkupValue = vMarkupValue;

                bomNbrs.WholeSalePrice = vWholeSaleAmount;
                bomNbrs.BOMId = lblBOMId.Text;
                bomNbrs.CustomerID = CustomerID;
                #endregion assign summary values


                //BOMDAL bomdal = new BOMDAL();
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                sqlResults = bomdal.BOMUpdate(bomItems, bomOhs, bomNbrs, connVM);
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
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }


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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #region NulCheck Region

                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (cmbType.Text.Trim() != "VAT 4.3 (Wastage)")
                {
                    if (Convert.ToDecimal(txtTotalCost.Text.Trim()) <= 0)
                    {
                        MessageBox.Show("Price not issued.", this.Text);
                        txtGTotal.Focus();
                        return;
                    }
                }



                if (txtFinishItemNo.Text == "")
                {
                    MessageBox.Show("Finish product not selected.", this.Text);
                    txtFinishItemNo.Focus();
                    return;
                }
                if (dgvBOM.RowCount <= 0)
                {
                    MessageBox.Show("There is no data in raw product grid", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (cmbType.Text.Trim() == "VAT 1")
                {
                    if (dgvOverhead.RowCount <= 0)
                    {
                        MessageBox.Show("There is no data in overhead grid", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                }


                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtMasterComments.Text == "")
                {
                    txtMasterComments.Text = "-";
                }
                #endregion

                #region Item


                string strVatRate = txtRVATRate.Text.Trim();
                string strSDRate = txtRSD.Text.Trim();
                decimal vMarkupPercent = 0;
                decimal vMarkupValue = 0;
                decimal vVateRate = 0;
                decimal vSDRate = 0;
                decimal vVatAmount = 0;
                decimal vSDAmount = 0;
                decimal vRawTotal = 0;
                decimal vPackingTotal = 0;
                decimal vRebateTotal = 0;
                decimal vAdditionalTotal = 0;
                decimal vRebatePercent = 0;


                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vVateRate = Convert.ToDecimal(strVatRate);
                }
                if (!string.IsNullOrEmpty(strVatRate))
                {
                    vSDRate = Convert.ToDecimal(strSDRate);
                }

                string strMarkupPercent = txtTrading.Text;
                if (!string.IsNullOrEmpty(strMarkupPercent))
                {
                    vMarkupPercent = Convert.ToDecimal(strMarkupPercent);
                }
                bomItems = new List<BOMItemVM>();
                decimal vSubTotal = 0;
                string strSubTotal = "0";
                string strRawItemType = string.Empty;
                for (int i = 0; i < dgvBOM.RowCount; i++)
                {

                    vSubTotal = 0;
                    strRawItemType = dgvBOM.Rows[i].Cells["InputType"].Value.ToString();
                    strSubTotal = dgvBOM.Rows[i].Cells["Cost"].Value.ToString();
                    if (!string.IsNullOrEmpty(strSubTotal))
                    {
                        vSubTotal = Convert.ToDecimal(strSubTotal);
                    }


                    if (!string.IsNullOrEmpty(strRawItemType))
                    {
                        if (strRawItemType != "Pack" && strRawItemType != "Overhead")
                        {
                            vRawTotal = vRawTotal + vSubTotal;
                        }

                        else if (strRawItemType == "Pack")
                        {
                            vPackingTotal = vPackingTotal + vSubTotal;
                        }
                        else if (strRawItemType == "Overhead")
                        {
                            vRebateTotal = vRebateTotal + vSubTotal;
                        }
                    }
                    //}
                    vMarkupValue = vSubTotal * vMarkupPercent / 100;
                    vSDAmount = (vSubTotal + vMarkupValue) * vSDRate / 100;
                    vVatAmount = (vSubTotal + vMarkupValue + vSDAmount) * vVateRate / 100;

                    //vRebatePercent
                    string strRebatePercent = dgvBOM.Rows[i].Cells["RebatePercent"].Value.ToString();
                    if (!string.IsNullOrEmpty(strRebatePercent))
                    {
                        vRebatePercent = Convert.ToDecimal(strRebatePercent);
                    }

                    BOMItemVM bomItem = new BOMItemVM();
                    bomItem.IssueOnProduction = dgvBOM.Rows[i].Cells["IssueOnProduction"].Value.ToString();
                    bomItem.RebateRate = vRebatePercent;
                    bomItem.FinishItemNo = txtFinishItemNo.Text.Trim();
                    bomItem.RawItemNo = dgvBOM.Rows[i].Cells["RawItemNo"].Value.ToString();
                    bomItem.UseQuantity = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Quantity"].Value);
                    bomItem.WastageQuantity = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Wastage"].Value);
                    bomItem.TotalQuantity = bomItem.UseQuantity + bomItem.WastageQuantity;
                    bomItem.Comments = txtComments.Text.Trim();
                    bomItem.ActiveStatus = dgvBOM.Rows[i].Cells["ActiveStatus"].Value.ToString();
                    bomItem.CreatedBy = Program.CurrentUser;
                    bomItem.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomItem.LastModifiedBy = Program.CurrentUser;
                    bomItem.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomItem.UOM = dgvBOM.Rows[i].Cells["UOM"].Value.ToString();
                    bomItem.VATRate = Convert.ToDecimal(txtRVATRate.Text.Trim());
                    bomItem.SD = Convert.ToDecimal(txtRSD.Text.Trim());
                    bomItem.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                    //bomItms.= cmbType.Text.Trim() ;
                    bomItem.Cost = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Cost"].Value.ToString());
                    bomItem.BOMLineNo = dgvBOM.Rows[i].Cells["LineNo"].Value.ToString();
                    bomItem.PacketPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                    bomItem.NBRPrice = Convert.ToDecimal(txtGTotal.Text.Trim());
                    bomItem.UnitCost = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UnitCost"].Value.ToString());
                    bomItem.TradingMarkUp = Convert.ToDecimal(txtTrading.Text.Trim());
                    bomItem.RawOHCost = Convert.ToDecimal(Convert.ToDecimal(txtTotalCost.Text.Trim()) + Convert.ToDecimal(txtMargin.Text) + Convert.ToDecimal(txtOHGTotal.Text.Trim()));
                    bomItem.UOMPrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMPrice"].Value.ToString());
                    bomItem.UOMc = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMc"].Value.ToString());
                    bomItem.UOMn = dgvBOM.Rows[i].Cells["UOMn"].Value.ToString();
                    bomItem.UOMUQty = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMUQty"].Value.ToString());
                    bomItem.UOMWQty = Convert.ToDecimal(dgvBOM.Rows[i].Cells["UOMWQty"].Value.ToString());
                    bomItem.RawItemType = dgvBOM.Rows[i].Cells["InputType"].Value.ToString();
                    bomItem.PBOMId = dgvBOM.Rows[i].Cells["PBOMId"].Value.ToString();
                    bomItem.PInvoiceNo = dgvBOM.Rows[i].Cells["PInvoiceNo"].Value.ToString();

                    bomItem.Post = "N";
                    bomItem.TradingMarkUp = vMarkupPercent;
                    bomItem.SDAmount = vSDAmount;
                    bomItem.MarkUpValue = vMarkupValue;
                    bomItem.VatAmount = vVatAmount;
                    bomItem.BOMId = lblBOMId.Text.Trim();

                    bomItem.BranchId = Program.BranchId;

                    if (cmbType.Text.Trim() == "VAT 4.3 (Wastage)")
                    {
                        bomItem.TransactionType = "0";
                    }
                    else
                    {
                        bomItem.TransactionType = dgvBOM.Rows[i].Cells["TransactionType"].Value.ToString();
                    }


                    bomItems.Add(bomItem);



                }

                #endregion Item

                #region OH

                bomOhs = new List<BOMOHVM>();
                for (int i = 0; i < dgvOverhead.RowCount; i++)
                {
                    BOMOHVM bomOh = new BOMOHVM();
                    decimal vRebateAmount = 0;
                    decimal vAdditionalCost = 0;
                    decimal vHeadAmount = 0;
                    decimal vAdditionalPercent = 0;


                    if (dgvOverhead.Rows[i].Cells["Percent"].Value != null)
                        vAdditionalPercent = Convert.ToDecimal(dgvOverhead.Rows[i].Cells["Percent"].Value.ToString());
                    if (dgvOverhead.Rows[i].Cells["HeadAmount"].Value != null)
                        vHeadAmount = Convert.ToDecimal(dgvOverhead.Rows[i].Cells["HeadAmount"].Value.ToString());

                    //vRebateAmount = vHeadAmount*vAdditionalPercent/100;
                    //vAdditionalCost = vHeadAmount - vRebateAmount;

                    vAdditionalCost = vHeadAmount * vAdditionalPercent / 100;
                    vRebateAmount = vHeadAmount - vAdditionalCost;

                    //bomOh.RebateAmount = vRebateAmount;
                    //bomOh.AdditionalCost = vAdditionalCost;
                    bomOh.RebateAmount = vRebateAmount;
                    bomOh.AdditionalCost = vAdditionalCost;

                    bomOh.HeadName = dgvOverhead.Rows[i].Cells["HeadName1"].Value.ToString();

                    bomOh.HeadID = dgvOverhead.Rows[i].Cells["HeadID"].Value.ToString();
                    bomOh.OHCode = dgvOverhead.Rows[i].Cells["OHCode"].Value.ToString();

                    bomOh.HeadAmount = vHeadAmount;
                    bomOh.CreatedBy = Program.CurrentUser;
                    bomOh.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh.LastModifiedBy = Program.CurrentUser;
                    bomOh.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh.FinishItemNo = txtFinishItemNo.Text.Trim();
                    bomOh.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                    bomOh.OHLineNo = dgvOverhead.Rows[i].Cells["LineNo1"].Value.ToString();
                    bomOh.RebatePercent = vAdditionalPercent;
                    bomOh.Post = "N";
                    vAdditionalTotal = vAdditionalTotal + vAdditionalCost;
                    bomOh.BOMId = lblBOMId.Text.Trim();

                    bomOh.BranchId = Program.BranchId;

                    bomOhs.Add(bomOh);



                }
                BOMOHVM bomOh1 = new BOMOHVM();
                bomOh1.HeadName = "Margin";
                bomOh1.OHCode = "ovh0";
                bomOh1.HeadID = "ovh0";
                bomOh1.HeadAmount = Convert.ToDecimal(txtMargin.Text.Trim());
                bomOh1.CreatedBy = Program.CurrentUser;
                bomOh1.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomOh1.LastModifiedBy = Program.CurrentUser;
                bomOh1.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomOh1.FinishItemNo = txtFinishItemNo.Text.Trim();
                bomOh1.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                bomOh1.OHLineNo = "100";
                bomOh1.RebatePercent = 0;
                bomOh1.Post = "N";
                bomOh1.AdditionalCost = bomOh1.HeadAmount;
                bomOh1.BOMId = lblBOMId.Text.Trim();
                bomOhs.Add(bomOh1);
                vAdditionalTotal = vAdditionalTotal + bomOh1.HeadAmount;

                #endregion OH

                #region NBR

                //BOMNBRVM bomNbr = new BOMNBRVM();
                bomNbrs.ReferenceNo = txtReferenceNo.Text.Trim();

                bomNbrs.BOMId = lblBOMId.Text.Trim();
                bomNbrs.ItemNo = txtFinishItemNo.Text.Trim();
                //bomNbrs.PNBRPrice = Convert.ToDecimal(txtGTotal.Text.Trim());
                var gTotal = Convert.ToDecimal(txtGTotal.Text.Trim());
                var nbrPrice = gTotal + gTotal * vMarkupPercent / 100;
                bomNbrs.PNBRPrice = nbrPrice; // vp

                bomNbrs.PPacketPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                bomNbrs.ReceiveCost =
                    Convert.ToDecimal(Convert.ToDecimal(txtTotalCost.Text.Trim()) +
                                      Convert.ToDecimal(txtOHGTotal.Text.Trim()));
                bomNbrs.Margin = Convert.ToDecimal(txtFinishItemNo.Text.Trim());
                bomNbrs.VATName = cmbType.Text.Trim();
                bomNbrs.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                bomNbrs.FirstSupplyDate = dtpFirstSupplyDate.Value.ToString("yyyy-MMM-dd");
                //bomItem.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd"));
                //bomNbrs.CreatedBy = Program.CurrentUser;
                //bomNbrs.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomNbrs.LastModifiedBy = Program.CurrentUser;
                bomNbrs.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                bomNbrs.ActiveStatus = "Y";
                bomNbrs.Post = "N";
                bomNbrs.RawTotal = vRawTotal;
                bomNbrs.PackingTotal = vPackingTotal;
                bomNbrs.RebateTotal = vRebateTotal;
                bomNbrs.AdditionalTotal = vAdditionalTotal;
                bomNbrs.RebateAdditionTotal = vAdditionalTotal + vRebateTotal;
                if (cmbType.Text.Trim() == "VAT 4.3 (Toll Issue)")
                {
                    if (TollIssueCostWithOthers == true)
                    {
                        bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;
                    }
                    else
                    {
                        bomNbrs.RawOHCost = vRebateTotal;
                    }

                    //bomNbrs.RawOHCost = vRebateTotal;
                }
                else
                {
                    bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;

                }
                //bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;

                bomNbrs.VATRate = vVateRate;
                bomNbrs.SDRate = vSDRate;
                bomNbrs.TradingMarkup = vMarkupPercent;
                bomNbrs.RebateRate = vRebatePercent;
                bomNbrs.Comments = txtComments.Text.Trim();
                bomNbrs.MasterComments = txtMasterComments.Text.Trim();
                if (string.IsNullOrEmpty(lblBOMId.Text.Trim()))
                {
                    throw new ArgumentNullException("btnUpdate_Click", "Could not find BOMId, Unable to process update");
                }
                else
                {
                    bomNbrs.BOMId = lblBOMId.Text;
                }


                bomNbrs.UOM = cmbFUOM.Text.Trim();
                //// add by Ruba
                var nbrWithSD = nbrPrice + nbrPrice * vSDRate / 100;
                bomNbrs.NBRWithSDAmount = nbrWithSD;
                bomNbrs.CustomerID = CustomerID;
                bomNbrs.BranchId = Program.BranchId;

                bomNbrs.FUOMc = Convert.ToDecimal(uomsValue(txtFUOM.Text, cmbFUOM.Text));

                //if (bomNbrs.FUOMc == 0)
                //{
                //    MessageBox.Show("Conversion not found for Finish Product");
                //    return;
                //}

                bomNbrs.FUOMPrice = bomNbrs.PNBRPrice * bomNbrs.FUOMc;
                bomNbrs.FUOMn = txtFUOM.Text;
                //// close
                #endregion NBR


                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                bgwUpdate.RunWorkerAsync();
            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }

            #endregion Catch

        }

        private void dgvBOM_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvOverhead_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private string SelectBOMOverHead()
        {
            string vbomGridIndex = string.Empty;

            if (dgvOverhead.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return vbomGridIndex;
            }

            bomGridIndex = string.Empty;
            for (int i = 0; i < dgvBOM.RowCount; i++)
            {
                if (dgvBOM.Rows[i].Cells["ProductName"].Value.ToString().Trim().ToLower() == dgvOverhead.CurrentRow.Cells["HeadName1"].Value.ToString().Trim().ToLower())
                {
                    vbomGridIndex = i.ToString();
                    break;
                }
            }
            return bomGridIndex = vbomGridIndex;
        }

        private void dgvOverhead_DoubleClick(object sender, EventArgs e)
        {
            SelectBOMOverHead();
            cmbOverHead.Text = dgvOverhead.CurrentRow.Cells["HeadName1"].Value.ToString();
            txtOHCode.Text = dgvOverhead.CurrentRow.Cells["OHCode"].Value.ToString();
            txtHeadID.Text = dgvOverhead.CurrentRow.Cells["HeadID"].Value.ToString();

            txtOHCost.Text = Convert.ToDecimal(dgvOverhead.CurrentRow.Cells["HeadAmount"].Value).ToString();


            string strPercent = dgvOverhead.CurrentRow.Cells["Percent"].Value.ToString();
            decimal vPercent = 0;
            decimal vRebatePercent = 0;
            if (!string.IsNullOrEmpty(strPercent))
            {
                vPercent = Convert.ToDecimal(strPercent);
                vRebatePercent = 100 - vPercent;
                txtPercent1.Text = "" + vRebatePercent;
                txtPercent2.Text = "" + vPercent;
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void txtFCategoryName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtFHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtOHGTotal_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormVAT1_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormVAT1_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormVAT1_FormClosing", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT1_FormClosing", exMessage);
            }

            #endregion Catch
        }

        private void cmbRProduct_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbOverHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }



        #endregion

        #region Methods 04

        private void btnOHAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (cmbOverHead.Text.Trim() == "Margin")
                {
                    MessageBox.Show(
                        cmbOverHead.Text.Trim() + " is basic head name." + "\n" + "no need to add this head list",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (cmbType.Text != "VAT 1")
                //{
                //    MessageBox.Show("Overhead not require in this VAT, please select the right VAT");
                //    return;
                //}
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtOHCost.Text) || txtOHCost.Text == "0")
                {
                    MessageBox.Show("Please input overhead Cost.", this.Text);
                    return;
                }
                if (string.IsNullOrEmpty(txtPercent1.Text))
                {
                    txtPercent1.Text = "0";
                }
                for (int i = 0; i < dgvOverhead.RowCount; i++)
                {
                    if (dgvOverhead.Rows[i].Cells["HeadName1"].Value.ToString().Trim().ToLower() == cmbOverHead.Text.Trim().ToLower())
                    {
                        MessageBox.Show("Same Head already exist.", this.Text);

                        return;
                    }
                }


                ChangeData = true;




                #region Add Overhead item to BOM grid

                string strHeadId = "";
                string strOHCode = "";
                DataRow[] ty20 = OverHeadResult.Select("Headname='" + cmbOverHead.Text.Trim() + "'");
                if (ty20 != null && ty20.Count() > 0)
                {
                    strOHCode = ty20[0]["OHCode"].ToString();
                    strHeadId = ty20[0]["HeadID"].ToString();
                }

                string strHeadAmount = Program.ParseDecimalObject(txtOHCost.Text);
                string strRebatePercent = Program.ParseDecimalObject(txtPercent1.Text);

                decimal vRebatePercent = 0;
                decimal vHeadAmount = 0;
                decimal vRebateAmount = 0;
                if (!string.IsNullOrEmpty(strHeadAmount))
                {
                    vHeadAmount = Convert.ToDecimal(strHeadAmount);
                }
                if (!string.IsNullOrEmpty(strRebatePercent))
                {
                    vRebatePercent = Convert.ToDecimal(strRebatePercent);
                }
                vRebateAmount = vHeadAmount * vRebatePercent / 100;

                DataGridViewRow NewRowBOM = new DataGridViewRow();
                dgvBOM.Rows.Add(NewRowBOM);

                dgvBOM["IssueOnProduction", dgvBOM.RowCount - 1].Value = "N";
                dgvBOM["RawItemNo", dgvBOM.RowCount - 1].Value = strHeadId;
                dgvBOM["RawPCode", dgvBOM.RowCount - 1].Value = strOHCode;
                dgvBOM["ProductName", dgvBOM.RowCount - 1].Value = cmbOverHead.Text.Trim();
                dgvBOM["Quantity", dgvBOM.RowCount - 1].Value = 1.0;
                dgvBOM["Wastage", dgvBOM.RowCount - 1].Value = 0.0;


                dgvBOM["UOM", dgvBOM.RowCount - 1].Value = "-";

                dgvBOM["UOMn", dgvBOM.RowCount - 1].Value = "-";
                dgvBOM["UOMPrice", dgvBOM.RowCount - 1].Value = vRebateAmount;
                dgvBOM["UOMc", dgvBOM.RowCount - 1].Value = "1";
                dgvBOM["UOMUQty", dgvBOM.RowCount - 1].Value = "1.0";
                dgvBOM["UOMWQty", dgvBOM.RowCount - 1].Value = "1.0";


                dgvBOM["UnitCost", dgvBOM.RowCount - 1].Value = vRebateAmount;

                //HeadAmountBOM
                dgvBOM["RebatePercent", dgvBOM.RowCount - 1].Value = Program.ParseDecimalObject(txtPercent1.Text);
                dgvBOM["Cost", dgvBOM.RowCount - 1].Value = vRebateAmount;
                dgvBOM["VATRate", dgvBOM.RowCount - 1].Value = 0;
                dgvBOM["Status", dgvBOM.RowCount - 1].Value = "New";
                dgvBOM["ActiveStatus", dgvBOM.RowCount - 1].Value = "Y";
                dgvBOM["SD", dgvBOM.RowCount - 1].Value = 0;
                dgvBOM["Mark", dgvBOM.RowCount - 1].Value = "Raw";
                dgvBOM["InputType", dgvBOM.RowCount - 1].Value = "Overhead";
                dgvBOM["TotalQty", dgvBOM.RowCount - 1].Value = "1.0";
                dgvBOM["HeadAmountBOM", dgvBOM.RowCount - 1].Value = vHeadAmount;
                dgvBOM["PBOMId", dgvBOM.RowCount - 1].Value = "0";
                dgvBOM["PInvoiceNo", dgvBOM.RowCount - 1].Value = "0";
                dgvBOM["TransactionType", dgvBOM.RowCount - 1].Value = "0";
                //dgvBOM["Cost", dgvBOM.RowCount - 1].Value = (100 - vRebatePercent) * vHeadAmount / 100;
                //dgvOverhead["Cost1", dgvOverhead.RowCount - 1].Value = (100 - vRebatePercent) * vHeadAmount / 100;

                //






                ////Rowcalculate();

                #endregion Add Overhead item to BOM grid


                DataGridViewRow NewRow = new DataGridViewRow();
                dgvOverhead.Rows.Add(NewRow);

                //dgvOverhead["LineNo1", dgvOverhead.RowCount - 1].Value = txtRProductCode.Text.Trim();
                dgvOverhead["HeadName1", dgvOverhead.RowCount - 1].Value = cmbOverHead.Text.Trim();
                dgvOverhead["OHCode", dgvOverhead.RowCount - 1].Value = txtOHCode.Text.Trim();
                dgvOverhead["HeadID", dgvOverhead.RowCount - 1].Value = txtHeadID.Text.Trim();
                dgvOverhead["HeadAmount", dgvOverhead.RowCount - 1].Value = vHeadAmount;
                ////dgvOverhead["Cost1", dgvOverhead.RowCount - 1].Value = Convert.ToDecimal(txtOHCost.Text.Trim()).ToString("0.00");
                ////dgvOverhead["Percent", dgvOverhead.RowCount - 1].Value = Convert.ToDecimal(txtPercent1.Text.Trim()).ToString("0.00");

                dgvOverhead["Percent", dgvOverhead.RowCount - 1].Value = 100 - vRebatePercent;
                dgvOverhead["Cost1", dgvOverhead.RowCount - 1].Value = (100 - vRebatePercent) * vHeadAmount / 100;


                //cmbOverHead.Focus();
                RowcalculateOH();
                Rowcalculate();
                txtOHCost.Text = "0";

                cmbOverHead.Focus();
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
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnOHAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnOHAdd_Click_1", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHAdd_Click_1", exMessage);
            }

            #endregion Catch
        }

        private void btnOHChange_Click(object sender, EventArgs e)
        {
            try
            {
                SelectBOMOverHead();


                ///////
                //cmbOverHead.Text = dgvOverhead.CurrentRow.Cells["HeadName1"].Value.ToString();
                //txtOHCost.Text = Convert.ToDecimal(dgvOverhead.CurrentRow.Cells["HeadAmount"].Value).ToString();

                //string strPercent = dgvOverhead.CurrentRow.Cells["Percent"].Value.ToString();
                //decimal vPercent = 0;
                //decimal vRebatePercent = 0;
                //if (!string.IsNullOrEmpty(strPercent))
                //{
                //    vPercent = Convert.ToDecimal(strPercent);
                //    vRebatePercent = 100 - vPercent;
                //    txtPercent1.Text = "" + vRebatePercent;
                //    txtPercent2.Text = "" + vPercent;
                //}
                /////
                int vbomGridIndex;
                if (cmbOverHead.Text.Trim() == "Margin")
                {
                    MessageBox.Show(
                        cmbOverHead.Text.Trim() + " is basic head name." + "\n" + "no need to add this head list",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrEmpty(bomGridIndex))
                {
                    MessageBox.Show("Please select Overhead first.", this.Text);
                    return;
                }
                else
                {
                    vbomGridIndex = Convert.ToInt32(bomGridIndex);

                }
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (txtOHCost.Text == "0.00")
                {
                    MessageBox.Show("Please input overhead Cost.", this.Text);
                    return;
                }
                if (txtPercent1.Text == "")
                {
                    txtPercent1.Text = "0.0";
                }


                ChangeData = true;




                #region Add Overhead item to BOM grid


                string strHeadId = "";
                string strOHCode = "";
                DataRow[] ty20 = OverHeadResult.Select("Headname='" + cmbOverHead.Text.Trim() + "'");
                if (ty20 != null && ty20.Count() > 0)
                {
                    strOHCode = ty20[0]["OHCode"].ToString();
                    strHeadId = ty20[0]["HeadID"].ToString();
                }

                string strHeadAmount = Program.ParseDecimalObject(txtOHCost.Text);
                string strRebatePercent = Program.ParseDecimalObject(txtPercent1.Text);

                decimal vRebatePercent = 0;
                decimal vHeadAmount = 0;
                decimal vRebateAmount = 0;
                if (!string.IsNullOrEmpty(strHeadAmount))
                {
                    vHeadAmount = Convert.ToDecimal(strHeadAmount);
                }
                if (!string.IsNullOrEmpty(strRebatePercent))
                {
                    vRebatePercent = Convert.ToDecimal(strRebatePercent);
                }
                vRebateAmount = vHeadAmount * vRebatePercent / 100;

                //DataGridViewRow NewRowBOM = new DataGridViewRow();
                //dgvBOM.Rows.Add(NewRowBOM);

                dgvBOM["IssueOnProduction", dgvBOM.RowCount - 1].Value = "N";
                dgvBOM["RawItemNo", vbomGridIndex].Value = strHeadId;
                dgvBOM["RawPCode", vbomGridIndex].Value = strOHCode;
                dgvBOM["ProductName", vbomGridIndex].Value = cmbOverHead.Text.Trim();
                dgvBOM["Quantity", vbomGridIndex].Value = 1.0;
                dgvBOM["Wastage", vbomGridIndex].Value = 0.0;


                dgvBOM["UOM", vbomGridIndex].Value = "-";

                dgvBOM["UOMn", vbomGridIndex].Value = "-";
                dgvBOM["UOMPrice", vbomGridIndex].Value = vRebateAmount;
                dgvBOM["UOMc", vbomGridIndex].Value = "1";
                dgvBOM["UOMUQty", vbomGridIndex].Value = "1.0";
                dgvBOM["UOMWQty", vbomGridIndex].Value = "1.0";


                dgvBOM["UnitCost", vbomGridIndex].Value = vRebateAmount;

                //HeadAmountBOM
                dgvBOM["RebatePercent", vbomGridIndex].Value = Program.ParseDecimalObject(txtPercent1.Text);
                dgvBOM["Cost", vbomGridIndex].Value = vRebateAmount;
                dgvBOM["VATRate", vbomGridIndex].Value = 0;
                dgvBOM["Status", vbomGridIndex].Value = "New";
                dgvBOM["ActiveStatus", vbomGridIndex].Value = "Y";
                dgvBOM["SD", vbomGridIndex].Value = 0;
                dgvBOM["Mark", vbomGridIndex].Value = "Raw";
                dgvBOM["InputType", vbomGridIndex].Value = "Overhead";
                dgvBOM["TotalQty", vbomGridIndex].Value = "1.0";
                dgvBOM["HeadAmountBOM", vbomGridIndex].Value = vHeadAmount;
                dgvBOM["PBOMId", vbomGridIndex].Value = "0";
                dgvBOM["PInvoiceNo", vbomGridIndex].Value = "0";

                //






                ////Rowcalculate();

                #endregion Add Overhead item to BOM grid


                //DataGridViewRow NewRow = new DataGridViewRow();
                //dgvOverhead.Rows.Add(NewRow);

                //dgvOverhead["LineNo1", dgvOverhead.RowCount - 1].Value = txtRProductCode.Text.Trim();
                dgvOverhead["HeadName1", dgvOverhead.CurrentRow.Index].Value = cmbOverHead.Text.Trim();
                dgvOverhead["HeadID", dgvOverhead.CurrentRow.Index].Value = txtHeadID.Text.Trim();
                dgvOverhead["OHCode", dgvOverhead.CurrentRow.Index].Value = txtOHCode.Text.Trim();
                dgvOverhead["HeadAmount", dgvOverhead.CurrentRow.Index].Value = vHeadAmount;
                ////dgvOverhead["Cost1", dgvOverhead.RowCount - 1].Value = Convert.ToDecimal(txtOHCost.Text.Trim()).ToString("0.00");
                ////dgvOverhead["Percent", dgvOverhead.RowCount - 1].Value = Convert.ToDecimal(txtPercent1.Text.Trim()).ToString("0.00");

                dgvOverhead["Percent", dgvOverhead.CurrentRow.Index].Value = 100 - vRebatePercent;
                dgvOverhead["Cost1", dgvOverhead.CurrentRow.Index].Value = (100 - vRebatePercent) * vHeadAmount / 100;

                dgvOverhead.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                dgvOverhead.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                //dgvBOM["PBOMId", vbomGridIndex].Value = "0";

                dgvBOM.Rows[vbomGridIndex].DefaultCellStyle.ForeColor = Color.Green;// Blue;
                dgvBOM.Rows[vbomGridIndex].DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                //cmbOverHead.Focus();
                RowcalculateOH();
                Rowcalculate();
                txtOHCost.Text = "0.00";

                cmbOverHead.Focus();
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
                FileLogger.Log(this.Name, "btnOHChange_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnOHChange_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnOHChange_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnOHChange_Click_1", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHChange_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHChange_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHChange_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHChange_Click_1", exMessage);
            }

            #endregion CATCH
        }

        private void btnOHDelete_Click_1(object sender, EventArgs e)
        {
            //if (cmbType.Text != "VAT 1")
            //{
            //    MessageBox.Show("Overhead not require in this VAT, please select the right VAT");
            //    return;
            //}
            try
            {
                SelectBOMOverHead();


                int vbomGridIndex;
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(bomGridIndex))
                {
                    MessageBox.Show("Please select Overhead first.", this.Text);
                    return;
                }
                else
                {
                    vbomGridIndex = Convert.ToInt32(bomGridIndex);

                }

                if (Convert.ToDecimal(dgvOverhead.CurrentCellAddress.Y) < 0)
                {
                    MessageBox.Show("There have no Data to Delete");
                    return;
                }
                ChangeData = true;


                dgvBOM["Quantity", vbomGridIndex].Value = 0;
                dgvBOM["Wastage", vbomGridIndex].Value = 0;


                dgvBOM["UOMPrice", vbomGridIndex].Value = 0;
                dgvBOM["UOMc", vbomGridIndex].Value = "0";
                dgvBOM["UOMUQty", vbomGridIndex].Value = "0";
                dgvBOM["UOMWQty", vbomGridIndex].Value = "0";


                dgvBOM["UnitCost", vbomGridIndex].Value = 0;

                //HeadAmountBOM
                dgvBOM["RebatePercent", vbomGridIndex].Value = txtPercent1.Text;
                dgvBOM["Cost", vbomGridIndex].Value = 0;
                dgvBOM["VATRate", vbomGridIndex].Value = 0;
                dgvBOM["SD", vbomGridIndex].Value = 0;
                dgvBOM["TotalQty", vbomGridIndex].Value = "0";
                dgvBOM["HeadAmountBOM", vbomGridIndex].Value = 0;
                dgvBOM["RebatePercent", vbomGridIndex].Value = 0;

                dgvBOM["PInvoiceNo", vbomGridIndex].Value = "0";

                dgvOverhead.CurrentRow.Cells["Cost1"].Value = "0";
                dgvOverhead.CurrentRow.Cells["RebateCost"].Value = "0";
                dgvOverhead.CurrentRow.Cells["HeadAmount"].Value = "0";
                dgvOverhead.CurrentRow.Cells["Percent"].Value = "0";

                //dgvOverhead.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                //dgvOverhead.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                dgvOverhead.Rows.RemoveAt(dgvOverhead.CurrentRow.Index);

                //dgvBOM.Rows[vbomGridIndex].DefaultCellStyle.ForeColor = Color.Red;
                //dgvBOM.Rows[vbomGridIndex].DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                dgvBOM.Rows.RemoveAt(vbomGridIndex);
                RowcalculateOH();
                Rowcalculate();
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
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnOHDelete_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnOHDelete_Click_1", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnOHDelete_Click_1", exMessage);
            }

            #endregion Catch
        }

        private void cmbFProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            else if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
                cmbFProduct.Focus();
            }
        }

        private void ProductLoad()
        {
            DataGridViewRow selectedRow = new DataGridViewRow();
            selectedRow = FormProductFinder.SelectOne(vCustomerID, IsRawParam);
            if (selectedRow != null && selectedRow.Selected == true)
            {
                //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                //vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                if (chkPFCode.Checked)
                {
                    cmbFProduct.Text = selectedRow.Cells["ProductCode"].Value.ToString();// ProductInfo[27].ToString();//27
                }
                else
                {
                    cmbFProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                }
            }

        }

        private void cmbRProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");

            }
        }

        private void cmbOverHead_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtOHCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbRProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtOHCost_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtRProductCode_TextChanged_1(object sender, EventArgs e)
        {


        }

        private void txtOHCost_Leave_1(object sender, EventArgs e)
        {
            txtOHCost.Text = Program.ParseDecimalObject(txtOHCost.Text.Trim()).ToString();

            if (Program.CheckingNumericTextBox(txtOHCost, "OH Cost") == true)
            {
                txtOHCost.Text = Program.FormatingNumeric(txtOHCost.Text.Trim(), BOMDPlaceA).ToString();
            }
            else
            {
                MessageBox.Show("Please enter numeric value.", "OH Cost", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                txtOHCost.Text = "0";
                txtOHCost.Focus();
            }

            //Program.FormatTextBox(txtOHCost, "OH Cost");
            //txtPercent1.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            ////MDIMainInterface mdi = new MDIMainInterface();
            //FormOverHeadSearch frm = new FormOverHeadSearch();
            //mdi.RollDetailsInfo(frm.VFIN);
            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                string result = string.Empty;// FormOverHeadSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] OverHeadInfo = result.Split(FieldDelimeter.ToCharArray());
                    //dgvOverHead.Rows[j].Cells["ActiveStatus"].Value = OverHeadFields[5].ToString();

                    //txtHeadID.Text = OverHeadInfo[0];
                    if (OverHeadInfo[1] == "Margin")
                    {
                        MessageBox.Show("Margin Not to be selected", this.Text);
                        return;

                    }
                    cmbOverHead.Text = OverHeadInfo[1];

                    //txtOHCost.Text = OverHeadInfo[2];
                    //txtDescription.Text = OverHeadInfo[3];
                    //txtComments.Text = OverHeadInfo[4];
                    ChangeData = false;
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

        private void cmbOverHead_TextChanged(object sender, EventArgs e)
        {
            txtOHCost.Text = "0.00";

            //for (int j = 0; j < Convert.ToInt32(OverHeadLines.Length); j++)
            //{
            //    string[] OverHeadFields = OverHeadLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    try
            //    {
            //        if (OverHeadFields[1].ToString() == cmbOverHead.Text.Trim())
            //        {
            //            //txtOHCost.Text = Convert.ToDecimal(OverHeadFields[2].ToString()).ToString("0.00");

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
        }

        private void SumQty()
        {
            try
            {
                decimal TotalQty = 0;
                decimal TotalCost = 0;
                //decimal NetCost = 0;
                decimal RUnitCost = 0;
                decimal RQuantity = 0;
                decimal RWastage = 0;
                decimal RWastageInp = 0;


                uomsValue();
                RUnitCost = Convert.ToDecimal(Convert.ToDecimal(txtRUnitCost.Text.Trim()) *
                                      Convert.ToDecimal(txtUomConv.Text.Trim()));
                TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                RWastageInp = Convert.ToDecimal(txtRWastageInp.Text.Trim());


                if (chkWastage.Checked)
                {
                    RWastage = Convert.ToDecimal(RWastageInp);
                }
                else
                {
                    //RWastage = (RWastageInp * TotalQty) / (100 + RWastageInp);

                    RWastage = TotalQty * RWastageInp / (100);
                }
                RQuantity = TotalQty - RWastage;


                //TotalQty = RWastage + RQuantity;

                TotalCost = TotalQty * RUnitCost;


                txtRWastage.Text = Convert.ToDecimal(Program.FormatingNumeric(RWastage.ToString(), BOMDPlaceQ)).ToString();
                txtRQuantity.Text = Convert.ToDecimal(Program.FormatingNumeric(RQuantity.ToString(), BOMDPlaceQ)).ToString();
                //txtNetCost.Text = Convert.ToDecimal(Program.FormatingNumeric(TotalCost.ToString(), BOMDPlaceA)).ToString();
                txtNetCost.Text = Convert.ToDecimal(Program.FormatingNumeric(TotalCost.ToString(), BOMDPlaceNetCost)).ToString();
                ChangeData = true;
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
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "SumQty", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", exMessage);
            }

            #endregion Catch

            ChangeData = true;
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            ToolTip tip = new ToolTip();

            if (this.Cursor == Cursors.Hand)
            {
                MessageBox.Show("OK");
            }
            //tip.Show("My tooltip",true);
            else
                tip.Hide(button1);

        }

        #endregion

        #region Methods 05

        private void button1_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                Program.fromOpen = "Other";
                Program.R_F = "Raw";


                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {


                    if (selectedRow.Cells["IsRaw"].Value.ToString() == "Service")//9
                    {
                        MessageBox.Show("Service item's Price not declare from here", this.Text);
                        return;
                    }
                    txtRProductCode.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                    txtRProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    cmbRProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    txtRCategoryName.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];
                    txtRHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString();//ProductInfo[11];

                    txtIsRaw.Text = selectedRow.Cells["IsRaw"].Value.ToString();//ProductInfo[9];
                    txtRUnitCost.Text = selectedRow.Cells["IssuePrice"].Value.ToString();//ProductInfo[26]; // cost price
                    txtRVATRate.Text = selectedRow.Cells["VATRate"].Value.ToString();//ProductInfo[12];
                    txtRUOM.Text = selectedRow.Cells["UOM"].Value.ToString();// ProductInfo[5];

                }

                ChangeData = true;
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

            #endregion Catch
        }

        private void dtpEffectDate_ValueChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //BOMDAL BDal = new BOMDAL();
            IBOM BDal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

            DataTable dataTable = new DataTable("SearchBOMHeader");
            try
            {
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = FormBOMSearch.SelectOne("Other");
                IsImport = false;
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    AllClear();
                    dataTable = BDal.SelectAll(selectedRow.Cells["BOMId"].Value.ToString(), null, null, null, null, null, true, connVM);

                    lblBOMId.Text = dataTable.Rows[0]["BOMId"].ToString();
                    txtReferenceNo.Text = dataTable.Rows[0]["ReferenceNo"].ToString();
                    BOMId = dataTable.Rows[0]["BOMId"].ToString();

                    dtpEffectDate.Value = Convert.ToDateTime(dataTable.Rows[0]["EffectDate"].ToString());
                    dtpFirstSupplyDate.Value = Convert.ToDateTime(dataTable.Rows[0]["FirstSupplyDate"].ToString());
                    cmbType.Text = dataTable.Rows[0]["VATName"].ToString();

                    //ProductDetailsInfoF();
                    BOMProductSearchDetails(dataTable.Rows[0]["FinishItemNo"].ToString());

                    //cmbFProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();
                    txtFinishItemNo.Text = dataTable.Rows[0]["FinishItemNo"].ToString();
                    txtFProductName.Text = dataTable.Rows[0]["ProductName"].ToString();
                    txtFProductCode.Text = dataTable.Rows[0]["ProductCode"].ToString();
                    txtGTotal.Text = Convert.ToDecimal(dataTable.Rows[0]["NBRPrice"].ToString()).ToString();
                    txtTrading.Text = Convert.ToDecimal(dataTable.Rows[0]["TradingMarkUp"].ToString()).ToString();
                    txtRSD.Text = Convert.ToDecimal(dataTable.Rows[0]["SD"].ToString()).ToString();
                    txtRVATRate.Text = Convert.ToDecimal(dataTable.Rows[0]["VATRate"].ToString()).ToString();
                    txtFUOM.Text = dataTable.Rows[0]["UOMn"].ToString();

                    FUoms();
                    cmbFUOM.Text = dataTable.Rows[0]["UOM"].ToString();

                    txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
                    txtMasterComments.Text = dataTable.Rows[0]["MasterComments"].ToString();
                    txtFHSCode.Text = dataTable.Rows[0]["HSCodeNo"].ToString();
                    CustomerID = dataTable.Rows[0]["CustomerID"].ToString();
                    txtCustomer.Text = dataTable.Rows[0]["CustomerName"].ToString();
                    IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;
                    chkAutoIssue.Checked = dataTable.Rows[0]["AutoIssue"].ToString() == "Y" ? true : false;
                    if (chkPFCode.Checked == true)
                    {
                        cmbFProduct.Text = dataTable.Rows[0]["ProductCode"].ToString();
                    }
                    else
                    {
                        cmbFProduct.Text = dataTable.Rows[0]["ProductName"].ToString();

                    }
                    var prodByCode = from prd in ProductsMini.ToList()
                                     where prd.ItemNo.ToLower() == dataTable.Rows[0]["FinishItemNo"].ToString().ToLower()
                                     select prd;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        var products = prodByCode.First();
                        CategoryIdF = products.CategoryID;
                        txtFCategoryName.Text = products.CategoryName;
                    }
                    searchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"].ToString());

                    ProductSearchDsLoadF();

                }


                Rowcalculate();
                RowcalculateOH();

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
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }

            #endregion Catch



        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtPacketPrice_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtPacketPrice, "PacketPrice");
            txtPacketPrice.Text = Program.ParseDecimalObject(txtPacketPrice.Text.Trim()).ToString();
            GTotal();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRSD_TextChanged_1(object sender, EventArgs e)
        {
            GTotal();
            ChangeData = true;

        }

        private void txtRVATRate_TextChanged_1(object sender, EventArgs e)
        {
            GTotal();
            ChangeData = true;
        }

        private void txtRSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtRSD, "SD");
            txtRSD.Text = Program.ParseDecimalObject(txtRSD.Text.Trim()).ToString();
        }

        private void txtRVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtRVATRate, "VAT Rate");
            txtRVATRate.Text = Program.ParseDecimalObject(txtRVATRate.Text.Trim()).ToString();
        }

        private void txtRWastage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRQuantity_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtRUnitCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalQty_TextChanged_2(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtNetCost_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txtTrading_TextChanged(object sender, EventArgs e)
        {
            GTotal();
        }

        #endregion

        #region Methods 06

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (IsImport == true)
                {
                    MessageBox.Show("this price declaration is imported, please search the declaration for print",
                                    this.Text);
                    return;
                }
                FormRptVAT1 frmRptVAT1 = new FormRptVAT1();

                if (searchBranchId > 0)
                {
                    frmRptVAT1.SenderBranchId = searchBranchId;
                }
                else
                {
                    frmRptVAT1.SenderBranchId = Program.BranchId;
                }


                MDIMainInterface mdi = new MDIMainInterface();
                //mdi.RollDetailsInfo("8101");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                frmRptVAT1.txtBomID.Text = BOMId;
                frmRptVAT1.txtItemNo.Text = txtFinishItemNo.Text.Trim();
                frmRptVAT1.txtProductName.Text = txtFProductName.Text.Trim();
                frmRptVAT1.txtVATName.Text = cmbType.Text.Trim();
                frmRptVAT1.dtpFromDate.Value = Convert.ToDateTime(dtpEffectDate.Value.ToString("yyyy-MMM-dd"));
                frmRptVAT1.Show();
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

            #endregion Catch)
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void txtPacketPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtMargin_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtMargin_Leave(object sender, EventArgs e)
        {
            txtMargin.Text = Program.ParseDecimalObject(txtMargin.Text.Trim()).ToString();
            //Program.FormatTextBox(txtMargin, "Margin");

            ////////if (Program.CheckingNumericTextBox(txtMargin, "Margin") == true)
            ////////{
            ////////    txtMargin.Text = Program.FormatingNumeric(txtMargin.Text.Trim(), BOMDPlaceA).ToString();
            ////////    SumQty();
            ////////GTotal();
            ////////}
            ////////txtPacketPrice.Focus();


        }

        private void txtRUnitCost_Leave(object sender, EventArgs e)
        {

            if (Program.CheckingNumericTextBox(txtRUnitCost, "Product Cost") == true)
            {
                var tt = txtRUnitCost.Text;
                txtRUnitCost.Text = Program.FormatingNumeric(txtRUnitCost.Text.Trim(), BOMDPlaceA).ToString();
                SumQty();
                cmbUom.Focus();
            }
            else
            {
                MessageBox.Show("Please enter numeric value in Unit Cost field ", "Unit Cost", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);


                txtRUnitCost.Text = "0";
                txtRUnitCost.Focus();
            }


        }

        private void txtRQuantity_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRQuantity, "RQuantity");
            txtRQuantity.Text = Program.ParseDecimalObject(txtRQuantity.Text.Trim()).ToString();
            //Program.FormatTextBoxQty4(txtRQuantity, "Quantity");
            //SumQty();
        }

        private void txtRWastage_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxQty(txtRWastage, "Wastage");

        }

        private void txtGTotal_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkPRCode_CheckedChanged(object sender, EventArgs e)
        {

            ProductSearchDsLoadR();

            cmbRProduct.Focus();
        }

        private void btnSerachGroup_Click(object sender, EventArgs e)
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

                    if (ProductCategoryInfo[4] == "Service")
                    {
                        MessageBox.Show("Service item's Price not declare from here", this.Text);
                        return;
                    }
                    else if (ProductCategoryInfo[4] == "Overhead")
                    {
                        MessageBox.Show("Overhead item's Price not declare from here", this.Text);
                        return;
                    }
                    //else if (ProductCategoryInfo[4] == "WIP")
                    //{
                    //    MessageBox.Show("WIP item's Price not declare from here", this.Text);
                    //    return;
                    //}
                    ChangeData = false;
                    CategoryIdR = ProductCategoryInfo[0];
                    txtRCategoryName.Text = ProductCategoryInfo[1];
                    CategoryIsRaw = ProductCategoryInfo[4];
                }
                ProductSearchDsLoadR();
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
                FileLogger.Log(this.Name, "btnSerachGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSerachGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSerachGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSerachGroup_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSerachGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSerachGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSerachGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSerachGroup_Click", exMessage);
            }

            #endregion Catch
            finally
            {
                ChangeData = false;
            }
        }

        private void ProductSearchDsLoadR()
        {

            try
            {

                cmbRProduct.Items.Clear();
                if (!string.IsNullOrEmpty(CategoryIdR))
                {


                    if (CategoryIdR != null)
                    {
                        if (chkPRCode.Checked == true)
                        {
                            var prodByCode = from prd in ProductsMini.ToList()
                                             where prd.CategoryID.ToLower() == CategoryIdR.ToLower()
                                             select prd.ProductCode;
                            if (prodByCode != null && prodByCode.Any())
                            {
                                cmbRProduct.Items.AddRange(prodByCode.ToArray());
                            }
                        }
                        else
                        {
                            var prodByName = from prd in ProductsMini.ToList()
                                             where prd.CategoryID.ToLower() == CategoryIdR.ToLower()
                                             select prd.ProductName;


                            if (prodByName != null && prodByName.Any())
                            {
                                cmbRProduct.Items.AddRange(prodByName.ToArray());
                            }
                        }


                        cmbRProduct.Items.Insert(0, "Select");
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
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsLoadR", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductSearchDsLoadR", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadR", exMessage);
            }

            #endregion Catch

        }

        private void ProductSearchDsLoadF()
        {
            try
            {
                cmbFProduct.DataSource = null;
                cmbFProduct.Items.Clear();
                if (CategoryIdF != null)
                {


                    if (chkPFCode.Checked == true)
                    {
                        string[] cValues = { CategoryIdF };
                        string[] cFields = { "Pr.CategoryID" };
                        ////BOMResult = bomdal.SelectAll(null, cFields, cValues, null, null, null, true);


                        //var prodByCode = new ProductDAL().SelectAll("0", cFields, cValues);

                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.CategoryID.ToLower() == CategoryIdF.ToLower()
                                         select prd.ProductCode;


                        if (prodByCode != null && prodByCode.Any())
                        {
                            cmbFProduct.Items.AddRange(prodByCode.ToArray());

                            //cmbFProduct.DisplayMember = "ProductName";// displayMember.Replace("[", "").Replace("]", "").Trim();

                            //cmbFProduct.DisplayMember=

                        }


                    }
                    else
                    {
                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.CategoryID.ToLower() == CategoryIdF.ToLower()
                                         select prd.ProductName;


                        if (prodByCode != null && prodByCode.Any())
                        {
                            cmbFProduct.Items.AddRange(prodByCode.ToArray());
                        }
                    }


                    cmbFProduct.Items.Insert(0, "Select");
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
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsLoadF", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductSearchDsLoadF", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoadF", exMessage);
            }

            #endregion Catch

        }

        private void GetLIFOPurchaseInformation(string PurchaseInvoiceNo = "")
        {
            try
            {

                #region Check Point
                if (!string.IsNullOrEmpty(PurchaseInvoiceNo))
                {
                    PurchaseInvoiceNo = OrdinaryVATDesktop.SanitizeInput(PurchaseInvoiceNo.Trim());
                }

                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }

                #endregion

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Method Call - ProductDetailsInfoR

                ProductDetailsInfoR();

                #endregion

                #region Reset Elements

                txtTotalQty.Text = "0";
                txtRWastageInp.Text = "0";
                txtRQuantity.Text = "0";

                #endregion

                #region Method Call - Uoms

                Uoms();

                #endregion

                if (!string.IsNullOrEmpty(cmbRProduct.Text))
                {

                    #region Declarations
                    ProductDAL productDal = new ProductDAL();

                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    decimal PurchaseCostPrice = 0;
                    decimal PurchaseQuantity = 0;
                    decimal NewCostPrice = 0;
                    string PinvoiceNo = "0";
                    string selectedItem = cmbRProduct.Text;

                    #endregion


                    if (selectedItem != "select")
                    {
                        string storedValue = OrdinaryVATDesktop.SanitizeInput(txtRProductId.Text.Trim());
                        if (!string.IsNullOrEmpty(storedValue))
                        {
                            string strEffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");

                            DataTable dtLastPurchase = new DataTable();

                            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:00");

                            #region Method Call - Get Last Purchase Detail

                            dtLastPurchase = new PurchaseDAL().GetLastPurchaseDetail(PurchaseInvoiceNo,OrdinaryVATDesktop.SanitizeInput(txtRProductCode.Text.Trim()), CurrentDateTime, null, null,connVM);

                            #endregion

                            #region Value Assign

                            decimal nbrPrice = 0;

                            if (dtLastPurchase != null && dtLastPurchase.Rows.Count > 0)
                            {
                                nbrPrice = Convert.ToDecimal(dtLastPurchase.Rows[0]["CostPrice"]);

                                CategoryIsRaw = dtLastPurchase.Rows[0]["IsRaw"].ToString();
                            }

                            txtRUnitCost.Text = nbrPrice.ToString();

                            #endregion

                            ////if (true)
                            ////{

                            #region Category "Raw"

                            if (CategoryIsRaw == "Raw")
                            {
                                string transType = productDal.GetTransactionType(storedValue, strEffectDate,connVM);

                                if (transType == "TollReceiveRaw")
                                {
                                    NewCostPrice = 0;
                                }
                            }

                            #endregion

                            #region Category "Raw" || "Pack" || "Trading"

                            if (CategoryIsRaw == "Raw" || CategoryIsRaw == "Pack" || CategoryIsRaw == "Trading" || CategoryIsRaw == "WIP")
                            {
                                #region Method Call - GetLIFOPurchaseInformation

                                DataTable dt = productDal.GetLIFOPurchaseInformation(storedValue, strEffectDate, PurchaseInvoiceNo,connVM);

                                #endregion

                                #region Value Assign

                                if (dt.Rows.Count > 0)
                                {
                                    PurchaseCostPrice = Convert.ToDecimal(dt.Rows[0]["PurchaseCostPrice"].ToString());
                                    PurchaseQuantity = Convert.ToDecimal(dt.Rows[0]["PurchaseQuantity"].ToString());
                                    NewCostPrice = 0;
                                    PinvoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                                    if (PurchaseQuantity != 0)
                                    {
                                        NewCostPrice = PurchaseCostPrice / PurchaseQuantity;
                                        txtPInvoiceNo.Text = PinvoiceNo;

                                    }

                                }
                                else
                                {
                                    MessageBox.Show("This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no purchase price. ", this.Text);
                                }

                                #endregion

                                #region Method Call - GetTransactionType

                                TransType = productDal.GetTransactionType(storedValue, strEffectDate,connVM);

                                #endregion

                                #region Conditional Values

                                if (TransType == "TollReceiveRaw")
                                {
                                    NewCostPrice = 0;
                                    txtRUnitCost.Enabled = false;
                                }
                                else
                                {
                                    txtRUnitCost.Enabled = true;

                                }

                                #endregion

                            }

                            #endregion

                            #region Category Other then "Raw" || "Pack" || "Trading"

                            else
                            {
                                #region Method Call - GetLastPurchaseDetail

                                dtLastPurchase = new DataTable();

                                CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:00");

                                dtLastPurchase = new PurchaseDAL().GetLastPurchaseDetail("", OrdinaryVATDesktop.SanitizeInput(txtRProductCode.Text.Trim()), CurrentDateTime, null, null, connVM);

                                #endregion

                                #region Value Assign

                                nbrPrice = 0;

                                if (dtLastPurchase != null && dtLastPurchase.Rows.Count > 0)
                                {
                                    nbrPrice = Convert.ToDecimal(dtLastPurchase.Rows[0]["CostPrice"]);
                                }

                                if (nbrPrice == 0)
                                {

                                    #region Method Call - GetLastNBRPriceFromBOM_VatName

                                    nbrPrice = productDal.GetLastNBRPriceFromBOM_VatName(OrdinaryVATDesktop.SanitizeInput(txtRProductCode.Text.Trim()), "VAT 1 Ka (Tarrif)", dtpEffectDate.Value.ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                                    #endregion

                                    NewCostPrice = nbrPrice;
                                }
                                else
                                {
                                    NewCostPrice = nbrPrice;
                                }

                                #endregion

                                #region Check Point

                                if (nbrPrice == 0)
                                {
                                    string message = "This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no price declaration. ";

                                    MessageBox.Show(message, this.Text);

                                }

                                #endregion

                            }

                            #endregion

                            #region Value Assign

                            txtRUnitCost.Text = NewCostPrice.ToString();

                            if (Program.CheckingNumericTextBox(txtRUnitCost, "Product Cost") == true)
                            {
                                var tt = txtRUnitCost.Text;
                                txtRUnitCost.Text =
                                    Program.FormatingNumeric(txtRUnitCost.Text.Trim(), BOMDPlaceA).ToString();
                                oldNbrPrice = Convert.ToDecimal(txtRUnitCost.Text);
                            }

                            #endregion

                            ////}

                        }

                    }
                }

                #region Oct-01-2020
                string sr1 = "";

                //txtRUnitCost.Focus();
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
                FileLogger.Log(this.Name, "cmbRProduct_Leave", exMessage);
            }

            #endregion Catch
        }

        private void cmbRProduct_Leave(object sender, EventArgs e)
        {

            if (ChkLIFOPrice.Checked)
            {
                GetLIFOPurchaseInformation();
            }
            else
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string[] shortColumnName = { "p.ProductCode", "p.ProductName", "h.BENumber", "h.PurchaseInvoiceNo", "h.ReceiveDate", "h.InvoiceDateTime" };
                string SQLQuery = @"select  p.ProductCode, p.ProductName,h.BENumber,h.PurchaseInvoiceNo,h.ReceiveDate,h.InvoiceDateTime from PurchaseInvoiceDetails d
                left outer join PurchaseInvoiceHeaders h on d.PurchaseInvoiceNo=h.PurchaseInvoiceNo
                left outer join Products p on p.ItemNo=d.ItemNo";

                string SQLTextRecordCount = @" select count(d.PurchaseInvoiceNo)RecordNo from PurchaseInvoiceDetails d
                left outer join PurchaseInvoiceHeaders h on d.PurchaseInvoiceNo=h.PurchaseInvoiceNo
                left outer join Products p on p.ItemNo=d.ItemNo"
                    ;

                if (chkPRCode.Checked)
                {
                    SQLQuery += " where p.ProductCode='" + cmbRProduct.Text.Trim() + "'";
                    SQLTextRecordCount += " where p.ProductCode='" + cmbRProduct.Text.Trim() + "'";
                }
                else
                {
                    SQLQuery += " where p.ProductName='" + cmbRProduct.Text.Trim() + "'";
                    SQLTextRecordCount += " where p.ProductName='" + cmbRProduct.Text.Trim() + "'";

                }
                //string tt1 = cmbRProduct.Text.Trim();



                string tableName = "";
                selectedRow = FormMultipleSearch.SelectOne(SQLQuery, tableName, "", shortColumnName, "", SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    string PurchaseInvoiceNo = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();//ProductInfo[0];

                    GetLIFOPurchaseInformation(PurchaseInvoiceNo);
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {

                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    CategoryIdF = ProductCategoryInfo[0];
                    txtFCategoryName.Text = ProductCategoryInfo[1];

                    ChangeData = false;
                }

                ProductSearchDsLoadF();

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
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button3_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", exMessage);
            }

            #endregion Catch
            finally
            {
                ChangeData = false;
            }

        }

        #endregion

        #region Methods 07

        private void chkPFCode_CheckedChanged(object sender, EventArgs e)
        {
            ProductSearchDsLoadF();
            cmbRProduct.Focus();
        }

        private void cmbFProduct_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cmbType.Text == "")
                {
                    MessageBox.Show("Please select the VAT name.", this.Text);
                    cmbType.Focus();
                    return;
                }
                ProductDetailsInfoF();
                ////BOMProductSearchDetails(txtFProductCode.Text.Trim());

                ////Rowcalculate();
                ////RowcalculateOH();
                ChangeData = true;
                Uoms();
                SumQty();
                GTotal();
                FUoms();
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
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPFCode_CheckedChanged", exMessage);
            }

            #endregion Catch
        }

        private void txtRProductId_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");

            }
        }

        private void txtRQuantity_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtRWastage_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                //btnAdd.Focus();
            }
        }

        private void cmbType_Leave(object sender, EventArgs e)
        {
            try
            {
                txtRWastage.Enabled = true;
                btnSerachGroup.Enabled = true;
                button1.Enabled = true;
                txtRQuantity.Enabled = true;
                chkWastage.Enabled = true;
                this.Width = 1010;
                this.Height = 540;
                txtGTotal.ReadOnly = true;

                if (cmbType.Text == "VAT 4.3 (Internal Issue)")
                {
                    if (txtFCategoryName.Text != "")
                    {

                        txtRProductCode.Text = txtFinishItemNo.Text;
                        txtRCategoryName.Text = txtFCategoryName.Text;
                        cmbRProduct.Text = cmbFProduct.Text;
                        txtRProductName.Text = txtFProductName.Text;
                        txtRProductCode.Text = txtFProductCode.Text;
                        txtRHSCode.Text = txtFHSCode.Text;
                        txtRProductId.Text = txtFinishItemNo.Text;
                        txtRUOM.Text = txtFUOM.Text;
                        cmbUom.Text = txtFUOM.Text;
                        ////txtRUnitCost.Text = txtFIssuePrice.Text;
                        //txtRWastage.Text = "0.00000";
                        //txtRQuantity.Text = "1.0000";
                        //txtRQuantity.Enabled = false;
                        //txtRWastage.Enabled = false;
                        //btnSerachGroup.Enabled = false;
                        //button1.Enabled = false;
                        //chkWastage.Enabled = false;
                    }
                }
                else if (cmbType.Text == "VAT 1 Kha (Trading)")
                {
                    if (txtFCategoryName.Text != "")
                    {
                        txtRProductCode.Text = txtFinishItemNo.Text;
                        txtRCategoryName.Text = txtFCategoryName.Text;
                        cmbRProduct.Text = cmbFProduct.Text;
                        txtRProductName.Text = txtFProductName.Text;
                        txtRProductCode.Text = txtFProductCode.Text;
                        txtRHSCode.Text = txtFHSCode.Text;
                        txtRProductId.Text = txtFinishItemNo.Text;
                        txtRUOM.Text = txtFUOM.Text;
                        cmbUom.Text = txtFUOM.Text;
                        ////txtRUnitCost.Text = txtFIssuePrice.Text;
                        //txtRWastage.Text = "0.00000";
                        //txtRQuantity.Text = "1.0000";
                        //txtRQuantity.Enabled = false;
                        //txtRWastage.Enabled = false;
                        //btnSerachGroup.Enabled = false;
                        //button1.Enabled = false;
                        //chkWastage.Enabled = false;
                    }


                }
                else if (cmbType.Text == "VAT 1 Ka (Tarrif)")
                {
                    txtGTotal.ReadOnly = false;
                }
                else if (cmbType.Text == "VAT 4.3 (Tender)")
                {
                    this.Width = 1270;
                }
                Uoms();
                SumQty();
                GTotal();
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
                FileLogger.Log(this.Name, "cmbType_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbType_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbType_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "cmbType_Leave", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbType_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbType_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbType_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbType_Leave", exMessage);
            }

            #endregion Catch
        }

        private void chkWastage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWastage.Checked)
            {
                chkWastage.Text = "Wastage(Q)";

            }
            else
            {
                chkWastage.Text = "Wastage(%)";
            }
        }

        private void cmbOverHead_Leave(object sender, EventArgs e)
        {
            try
            {
                txtHeadID.Text = string.Empty;
                txtOHCode.Text = string.Empty;
                for (int i = 0; i < OverHeadResult.Rows.Count; i++)
                {
                    if (cmbOverHead.Text == OverHeadResult.Rows[i]["Headname"].ToString())
                    {

                        txtPercent1.Text = OverHeadResult.Rows[i]["Rebatepercent"].ToString();
                        txtHeadID.Text = OverHeadResult.Rows[i]["HeadID"].ToString();
                        txtOHCode.Text = OverHeadResult.Rows[i]["OHCode"].ToString();


                        Program.FormatTextBoxQty(txtPercent1, "Percent ");

                        if (Program.IsNumeric(txtPercent1.Text.Trim()) == true)
                        {
                            if (Convert.ToDecimal(txtPercent1.Text.Trim()) <= 100)
                            {
                                txtPercent2.Text = Convert.ToDecimal(100 - Convert.ToDecimal(txtPercent1.Text)).ToString();
                            }
                            else
                            {
                                MessageBox.Show("Please Input <= 100 Value", this.Text);
                                txtPercent1.Text = "0";
                                txtPercent2.Text = "0";
                                txtPercent1.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please Input Numeric Value", this.Text);
                            txtPercent1.Text = "0";
                            txtPercent2.Text = "0";
                            txtPercent1.Focus();
                        }
                        return;

                    }
                }
                if (!string.IsNullOrEmpty(txtHeadID.Text.Trim()))
                {
                    MessageBox.Show("Please select Overhead Item", this.Text);
                    return;
                }
                //txtOHCost.Focus();
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
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkWastage_CheckedChanged", exMessage);
            }

            #endregion Catch
        }

        private void txtPercent1_Leave(object sender, EventArgs e)
        {
            txtPercent1.Text = Program.ParseDecimalObject(txtPercent1.Text.Trim()).ToString();
            //Program.FormatTextBoxRate(txtPercent1, "RebatePercent");
            try
            {
                txtPercent2.Text = "";
                Program.FormatTextBoxRate(txtPercent1, "Percent ");

                if (Program.IsNumeric(txtPercent1.Text.Trim()) == true)
                {
                    if (Convert.ToDecimal(txtPercent1.Text.Trim()) <= 100)
                    {
                        txtPercent2.Text = Convert.ToDecimal(100 - Convert.ToDecimal(txtPercent1.Text)).ToString();
                    }
                    else
                    {
                        MessageBox.Show("Please Input <= 100 Value", this.Text);
                        txtPercent1.Text = "0";
                        txtPercent2.Text = "0";
                        txtPercent1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Please Input Numeric Value", this.Text);
                    txtPercent1.Text = "0";
                    txtPercent2.Text = "0";
                    txtPercent1.Focus();
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
                FileLogger.Log(this.Name, "txtPercent1_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "txtPercent1_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "txtPercent1_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "txtPercent1_Leave", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtPercent1_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtPercent1_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtPercent1_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtPercent1_Leave", exMessage);
            }

            #endregion Catch

            //btnOHAdd.Focus();
        }

        private void txtPercent1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnOHAdd.Focus();
            }
        }

        private void txtPercent2_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPercent2, "Percent2");
            txtPercent2.Text = Program.ParseDecimalObject(txtPercent2.Text.Trim()).ToString();
            //btnOHAdd.Focus();
        }

        private void txtPercent2_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(Keys.Enter)) { btnOHAdd.Focus(); }
        }

        private void txtPercent1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void uomsValue()
        {
            try
            {
                string uOMFrom = txtRUOM.Text.Trim().ToLower();
                string uOMTo = cmbUom.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    txtUomConv.Text = "0";
                    if (uOMFrom == uOMTo)
                    {
                        txtUomConv.Text = "1";
                        //txtRQuantity.Focus();
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
                FileLogger.Log(this.Name, "uomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "uomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "uomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "uomsValue", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "uomsValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "uomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "uomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "uomsValue", exMessage);
            }

            #endregion Catch
        }



        private string uomsValue(string from, string to)
        {
            string conversion = "0";

            try
            {

                string uOMFrom = from.Trim().ToLower();
                string uOMTo = to.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    conversion = "0";
                    if (uOMFrom == uOMTo)
                    {
                        conversion = "1";
                        //txtRQuantity.Focus();
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
                            conversion = uoms.First().ToString();
                        }
                        else
                        {
                            conversion = "0";
                        }
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
                FileLogger.Log(this.Name, "uomsValue", exMessage);
            }

            #endregion Catch

            return conversion;
        }
        #endregion

        #region Methods 08

        private void cmbUom_Leave(object sender, EventArgs e)
        {

            uomsValue();
            //txtRQuantity.Focus();
            // txtTotalQty.Focus();
        }

        private void chkLIFO_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLIFO.Checked)
            {
                chkLIFO.Text = "LIFO";
            }
            else
            {
                chkLIFO.Text = "AVG";
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtRProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdPost_Click(object sender, EventArgs e)
        {
            if (IsPost == true)
            {
                MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                return;
            }
            if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            if (searchBranchId != Program.BranchId && searchBranchId != 0)
            {
                MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            #region NBR

            if (IsImport == false)
            {
                bomNbrs.ItemNo = txtFinishItemNo.Text.Trim();

            }
            bomNbrs.ReferenceNo = txtReferenceNo.Text.Trim();


            bomNbrs.FinishItemName = txtFProductName.Text.Trim();
            bomNbrs.VATName = cmbType.Text.Trim();
            bomNbrs.EffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
            bomNbrs.LastModifiedBy = Program.CurrentUser;
            bomNbrs.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
            bomNbrs.Post = "Y";
            bomNbrs.BOMId = lblBOMId.Text;
            bomNbrs.CustomerID = CustomerID;

            #endregion NBR
            this.cmdPost.Enabled = false;
            this.progressBar1.Visible = true;
            bgwPost.RunWorkerAsync();
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {


                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                string strBomId = lblBOMId.Text;
                //BOMDAL bomdal = new BOMDAL();
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                sqlResults = bomdal.BOMPost(bomNbrs, connVM);

                POST_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }

            #endregion

        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

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
                            IsPost = true;

                        }
                    }

                ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.cmdPost.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void txtTotalQty_Leave(object sender, EventArgs e)
        {
            txtTotalQty.Text = Program.ParseDecimalObject(txtTotalQty.Text.Trim()).ToString();

            if (Program.CheckingNumericTextBox(txtTotalQty, "Total Quantity") == true)
            {
                txtTotalQty.Text = Program.FormatingNumeric(txtTotalQty.Text.Trim(), BOMDPlaceQ).ToString();
                SumQty();
            }
            else
            {
                MessageBox.Show("Please enter numeric value.", "Total Quantity", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);


                txtTotalQty.Text = "0";
            }

            //txtRWastageInp.Focus();

        }

        private void txtRWastageInp_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtRWastageInp_Leave(object sender, EventArgs e)
        {
            txtRWastageInp.Text = Program.ParseDecimalObject(txtRWastageInp.Text.Trim()).ToString();

            if (Program.CheckingNumericTextBox(txtRWastageInp, "Wastage") == true)
            {
                txtRWastageInp.Text = Program.FormatingNumeric(txtRWastageInp.Text.Trim(), BOMDPlaceQ).ToString();
                SumQty();
            }
            else
            {
                MessageBox.Show("Please enter numeric value.", "Wastage Quantity", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                txtRWastageInp.Text = "0";
                txtRWastageInp.Focus();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeData = false;
            Program.fromOpen = "Me";
            DataGridViewRow selectedRow = FormBOMSearch.SelectOne("Other");

            if (selectedRow != null && selectedRow.Selected == true)
            {
                txtPBOMId.Text = selectedRow.Cells["BOMId"].Value.ToString();
                txtRProductId.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                txtRProductName.Text = selectedRow.Cells["productname"].Value.ToString();
                txtRProductCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();
                cmbUom.Text = selectedRow.Cells["UOM"].Value.ToString();
                txtRUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                txtRHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString();

                //txtRUnitCost.Text = Convert.ToDecimal(selectedRow.Cells["SalePrice"].Value.ToString()).ToString("0,0.00");

                txtRUnitCost.Text = Program.FormatingNumeric(selectedRow.Cells["SalePrice"].Value.ToString(), BOMDPlaceA).ToString();


            }
            ChangeData = false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            CommonDAL commonDAL = new CommonDAL();

            string code = commonDAL.settings("CompanyCode", "Code");

            if (string.Equals(code, "bata", StringComparison.InvariantCultureIgnoreCase))
            {
                FormPriceDeclarationBata formPriceDeclarationBata = new FormPriceDeclarationBata();
                formPriceDeclarationBata.Show();
            }
            else
            {
                FormMasterImport fmi = new FormMasterImport();
                fmi.preSelectTable = "BOM";
                fmi.Show();
            }


            

            #region Old process

            //if (chkSame.Checked == false)
            //{
            //    BrowsFile();
            //    string fileName = Program.ImportFileName;
            //    if (string.IsNullOrEmpty(fileName))
            //    {
            //        MessageBox.Show("Please select the right file for import");
            //        return;
            //    }
            //}
            //chkSame.Checked = true;
            //#region new process for bom import

            //string[] retResults = BOMImport();
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

            //#endregion new process for bom import
            #endregion
        }

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*";
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

        private string[] BOMImport()
        {
            #region Close1
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            #endregion Close1

            #region try
            OleDbConnection theConnection = null;
            try
            {
                #region Load Excel
                string str1 = "";
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                bool AutoItem = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item", connVM) == "Y" ? true : false);
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return retResults;
                }
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                                          + "Data Source=" + fileName + ";"
                                          + "Extended Properties=" + "\""
                                          + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter adapterProduct = new OleDbDataAdapter("SELECT * FROM [Product$]", theConnection);
                //DataSet dsProdduct = new DataSet();
                System.Data.DataTable dtProduct = new System.Data.DataTable();
                adapterProduct.Fill(dtProduct);

                OleDbDataAdapter adapterOH = new OleDbDataAdapter("SELECT * FROM [InputOH$]", theConnection);
                //DataSet dsOH = new DataSet();
                System.Data.DataTable dtOH = new System.Data.DataTable();
                adapterOH.Fill(dtOH);

                theConnection.Close();
                #endregion Load Excel

                #region Process model
                //BOMDAL bom1 = new BOMDAL();
                IBOM bom1 = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);



                string Vat_Name = "";
                DateTime dateEffectDate = new DateTime();
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                //CustomerDAL customerDAL = new CustomerDAL();
                ICustomer customerDAL = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);


                bomNbrs = new BOMNBRVM();
                for (int i = 0; i < dtProduct.Rows.Count; i++)
                {
                    #region Finish
                    decimal vVateRate = 0;
                    decimal vSDRate = 0;
                    decimal vVatAmount = 0;
                    decimal vMarkupPercent = 0;
                    decimal vMarkupValue = 0;
                    decimal vPacketPrice = 0;
                    decimal vNbrPrice = 0;
                    decimal vAdditionalTotal = 0;
                    decimal vRawTotal = 0;
                    decimal vSDAmount = 0;

                    decimal vPackingTotal = 0;
                    decimal vRebateTotal = 0;

                    string productGroup = dtProduct.Rows[i]["Type"].ToString().Trim();
                    //productGroup = productGroup.Replace(" ", "");

                    string pCode = dtProduct.Rows[i]["P-Code"].ToString().Trim();
                    //pCode = pCode.Replace(" ", "");

                    string finishItemName = dtProduct.Rows[i]["FinishItemName"].ToString().Trim();
                    // finishItemName = finishItemName.Replace(" ", "");

                    string packSize = dtProduct.Rows[i]["Pack size"].ToString().Trim();
                    string vatName = dtProduct.Rows[i]["VATName"].ToString().Trim();
                    //vatName = vatName.Replace(" ", "");

                    //string effecrDate = DateTime.Now.ToString("yyyy-MMM-dd");
                    string effecrDateTemp = dtProduct.Rows[i]["EffectDate"].ToString().Trim();
                    //string effecrDate = dtProduct.Rows[i]["EffectDate"].ToString().Trim();
                    string nbrPrice = dtProduct.Rows[i]["VATABLE PRICE"].ToString().Trim();
                    nbrPrice = Program.FormatingNumeric(nbrPrice, BOMDPlaceA).ToString();

                    string packetPrice = dtProduct.Rows[i]["PacketPrice"].ToString().Trim();
                    string vatRae = dtProduct.Rows[i]["VATRate"].ToString().Trim();
                    string sdRae = dtProduct.Rows[i]["SDRate"].ToString().Trim();
                    string tradingMarkup = dtProduct.Rows[i]["TradingMarkup"].ToString().Trim();
                    string margin = dtProduct.Rows[i]["Margin"].ToString().Trim();
                    string comments = dtProduct.Rows[i]["Remarks"].ToString().Trim();
                    string CustomerCode = dtProduct.Rows[i]["CustomerCode"].ToString().Trim();

                    string finishItemNo = "";

                    Debug.WriteLine("Finish Name '" + finishItemName + "' and code '" + pCode + "'");

                    if (string.IsNullOrEmpty(productGroup))
                        throw new ArgumentNullException("ProductGroup", "Could not find product Type in Product Sheet for ('" + finishItemName + "'('" + pCode + "'))");
                    DataRow[] OHOverheads;
                    DataRow[] OHRaws;//= new DataRow[];//

                    #region fitemno
                    if (string.IsNullOrEmpty(pCode))
                    {
                        if (string.IsNullOrEmpty(finishItemName))
                        {
                            throw new ArgumentNullException("ProductName", "Could not find product name('" + finishItemName + "'('" + pCode + "'))");
                        }
                        else
                        {
                            finishItemNo = productDal.GetProductNoByGroup(finishItemName, productGroup, connVM);
                            if (string.IsNullOrEmpty(finishItemNo))
                            {
                                throw new ArgumentNullException("ProductName", "Could not find product('" + finishItemName + "'('" + pCode + "')) in database");
                            }
                            else
                            {
                                OHRaws =
                                   dtOH.Select("Product_Name='" + finishItemName +
                               "'and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead' )");

                                OHOverheads = dtOH.Select("Product_Name='" + finishItemName + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                            }
                        }
                    }
                    else
                    {
                        finishItemNo = productDal.GetProductNoByGroup_Code(pCode, productGroup, connVM);//
                        if (string.IsNullOrEmpty(finishItemNo))
                        {
                            if (string.IsNullOrEmpty(finishItemName))
                            {
                                throw new ArgumentNullException("ProductName", "Could not find product name('" + finishItemName + "'('" + pCode + "'))");
                            }
                            else
                            {
                                finishItemNo = productDal.GetProductNoByGroup(finishItemName, productGroup, connVM);
                                if (string.IsNullOrEmpty(finishItemNo))
                                {
                                    throw new ArgumentNullException("ProductName", "Could not find product('" + finishItemName + "'('" + pCode + "')) in database");
                                }
                                else
                                {

                                    OHRaws =
                                   dtOH.Select("Product_Name='" + finishItemName +
                               "' and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead' )");
                                    OHOverheads = dtOH.Select("Product_Name='" + finishItemName + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                                }
                            }

                        }
                        else
                        {
                            OHRaws =
                                   dtOH.Select("Product_Code='" + pCode +
                               "'and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead' )");
                            OHOverheads = dtOH.Select("Product_Code='" + pCode + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                        }

                    }
                    #endregion fitemno

                    DataTable dt = new DataTable();
                    dt = customerDAL.SearchCustomerByCode(CustomerCode, connVM);
                    string CustomerID = "0";
                    if (dt.Rows.Count > 0)
                    {
                        CustomerID = dt.Rows[0]["CustomerID"].ToString();
                    }
                    bomNbrs.ItemNo = finishItemNo;
                    bomNbrs.FinishItemName = finishItemName;
                    bomNbrs.CustomerID = CustomerID;

                    if (!string.IsNullOrEmpty(vatName))
                    {
                        Vat_Name = vatName;
                    }
                    if (!string.IsNullOrEmpty(vatRae))
                    {
                        vVateRate = Convert.ToDecimal(vatRae);
                    }

                    if (!string.IsNullOrEmpty(nbrPrice))
                    {
                        vNbrPrice = Convert.ToDecimal(nbrPrice);

                    }
                    if (!string.IsNullOrEmpty(sdRae))
                    {
                        vSDRate = Convert.ToDecimal(sdRae);
                    }

                    if (!string.IsNullOrEmpty(tradingMarkup))
                    {
                        vMarkupPercent = Convert.ToDecimal(tradingMarkup);
                    }
                    if (string.IsNullOrEmpty(Vat_Name))
                    {
                        throw new ArgumentNullException("VATName", "Vat name is empty");
                    }
                    if (!string.IsNullOrEmpty(margin))
                        //txtMargin.Text = margin;

                        bomNbrs.VATName = Vat_Name;
                    if (!string.IsNullOrEmpty(nbrPrice))
                        bomNbrs.PNBRPrice = Convert.ToDecimal(nbrPrice); // vp



                    if (string.IsNullOrEmpty(effecrDateTemp))
                        throw new ArgumentNullException("VATName", "unable to process Effect date");
                    dateEffectDate = Convert.ToDateTime(effecrDateTemp);
                    bomNbrs.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");



                    bomNbrs.VATRate = vVateRate;

                    if (!string.IsNullOrEmpty(tradingMarkup))
                        bomNbrs.TradingMarkup = Convert.ToDecimal(tradingMarkup);

                    //bomNbrs.RebateRate = vRebatePercent;
                    bomNbrs.Comments = comments;


                    bomNbrs.SDRate = vSDRate;
                    bomNbrs.UOM = packSize.Trim();

                    #endregion Finish

                    #region Material Process Pack & Raw
                    DataRow[] inpType =
                       dtOH.Select("InputType <>'Raw' and InputType<>'Pack' and InputType<>'Overhead' and InputType<>'Trading' and InputType<>'Finish'");
                    if (inpType != null || !inpType.Any())
                    {
                        //var aa = inpType.Where(x => x.)

                        foreach (DataRow row in inpType)
                        {
                            string materialName = row["Material Name"].ToString().Trim();
                            string inputType = row["InputType"].ToString().Trim();

                            throw new ArgumentNullException("Input Type", "Input Type ('" + inputType + "')  for Material Name ('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) not in database");

                        }

                        str1 = "";
                    }
                    //DataRow[] OHRaws =
                    //    dtOH.Select("Product_Code='" + pCode +
                    //                "'and VATName= '" + vatName + "' AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead')");

                    //foreach (DataRow row in result)
                    //{
                    //    Console.WriteLine("{0}, {1}", row[0], row[1]);
                    //}
                    //if (OHRaws == null || !OHRaws.Any())

                    str1 = "";
                    int counter = 1;
                    //if (OHRaws == null || !OHRaws.Any())
                    //{
                    //    throw new ArgumentNullException("Finish Item not Found", "Finish Product ('" + finishItemName + "'('" + pCode + "'))  for VAT Name ('" + vatName + "')  not Found in import file");

                    //}
                    bomItems = new List<BOMItemVM>();

                    double wastageQty, ohUnitCose, totalQty, useQty, vRebatePercent, vSubTotal = 0;
                    foreach (DataRow row in OHRaws)
                    {
                        BOMItemVM bomItem = new BOMItemVM();
                        string materialItemNo = "";
                        string inputType = row["InputType"].ToString().Trim();
                        //inputType = inputType.Replace(" ", "");

                        string ohPacksize = row["Packsize"].ToString().Trim();
                        string materialCode = row["Material_Code"].ToString().Trim();

                        //materialCode = materialCode.Replace(" ", "");
                        string materialName = row["Material Name"].ToString().Trim();
                        //materialName = materialName.Replace(" ", "");

                        string ohUOM = row["UOM"].ToString().Trim();
                        //var vohUnitCose = row["UnitCost"].ToString().Trim();




                        Debug.WriteLine("Materials Name '" + materialName + "' and code '" + materialCode + "' Finish Name '" + finishItemName + "' and code '" + pCode + "'");
                        if (string.IsNullOrEmpty(inputType))
                        {
                            throw new ArgumentNullException("InputType", "Unable to process input type value  of Product ('" + finishItemName + "'('" + pCode + "'))");
                        }

                        else if (inputType.ToLower() != "pack" && inputType.ToLower() != "overhead" && inputType.ToLower() != "finish" && inputType.ToLower() != "trading" && inputType.ToLower() != "raw")
                        {
                            throw new ArgumentNullException("Input Type", "Input Type ('" + inputType + "')  of Product ('" + finishItemName + "'('" + pCode + "')) not in database");

                        }



                        ////////////////////////
                        #region Find materialCode
                        if (string.IsNullOrEmpty(inputType))
                            throw new ArgumentNullException("ProductGroup", "Could not find product Type in InputOH Sheet for ('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))");

                        if (string.IsNullOrEmpty(materialCode))
                        {
                            if (string.IsNullOrEmpty(materialName))
                            {
                                throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                            }
                            else
                            {
                                materialItemNo = productDal.GetProductNoByGroup(materialName, inputType, connVM);
                                if (string.IsNullOrEmpty(materialItemNo))
                                {
                                    throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                }
                            }
                        }
                        else
                        {
                            materialItemNo = productDal.GetProductNoByGroup_Code(materialCode, inputType, connVM);
                            if (string.IsNullOrEmpty(materialItemNo))
                            {
                                if (string.IsNullOrEmpty(materialName))
                                {
                                    throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                }
                                else
                                {
                                    materialItemNo = productDal.GetProductNoByGroup(materialName, inputType, connVM);
                                    if (string.IsNullOrEmpty(materialItemNo))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                    }
                                }

                            }

                        }
                        #endregion Find materialCode

                        /// ///////////////////





                        //if (string.IsNullOrEmpty(vohUnitCose))
                        //{
                        //    ohUnitCose = 0;
                        //}
                        //else
                        //{
                        //    ohUnitCose = Convert.ToDouble(vohUnitCose);
                        //}
                        var vtotalQty = row["TotalQty"].ToString().Trim();

                        if (string.IsNullOrEmpty(vtotalQty))
                        {
                            totalQty = 0;
                        }
                        else
                        {
                            vtotalQty = Program.FormatingNumeric(vtotalQty, BOMDPlaceQ).ToString();
                            totalQty = Convert.ToDouble(vtotalQty);
                        }



                        var wqty = row["WastageQty"].ToString().Trim();
                        if (string.IsNullOrEmpty(wqty))
                        {
                            wastageQty = 0;
                        }
                        else
                        {
                            wqty = Program.FormatingNumeric(wqty, BOMDPlaceQ).ToString();
                            wastageQty = Convert.ToDouble(wqty);
                        }

                        var vuseQty = totalQty - wastageQty;// row["UseQty"].ToString().Trim();
                        //if (string.IsNullOrEmpty(vuseQty))
                        //{
                        //    useQty = 0;
                        //}
                        //else
                        //{
                        useQty = Convert.ToDouble(vuseQty);
                        //}

                        var vRebate = row["Rebate%"].ToString().Trim();
                        if (string.IsNullOrEmpty(vRebate))
                        {
                            vRebatePercent = 0;
                        }
                        else
                        {
                            vRebatePercent = Convert.ToDouble(vRebate);
                        }

                        var vSTotal = row["SubTotal"].ToString().Trim();
                        if (string.IsNullOrEmpty(vSTotal))
                        {
                            vSubTotal = 0;
                        }
                        else
                        {
                            vSTotal = Program.FormatingNumeric(vSTotal, BOMDPlaceA).ToString();
                            vSubTotal = Convert.ToDouble(vSTotal);
                        }











                        decimal vuomc = 0;
                        decimal vuomPrice = 0;
                        decimal vuomUseQty = 0;
                        decimal vuomWastageQty = 0;
                        string uomc = string.Empty;//= row["UOMc"].ToString().Trim();
                        string uomn = string.Empty;//= row["UOMn"].ToString().Trim();

                        #region uomn
                        if (inputType != "Overhead")
                        {
                            if (string.IsNullOrEmpty(materialCode))
                            {
                                if (string.IsNullOrEmpty(materialName))
                                {
                                    throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                }
                                else
                                {
                                    uomn = productDal.GetProductUOMn(materialName, inputType, connVM);

                                    if (string.IsNullOrEmpty(uomn))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))in database");
                                    }
                                }
                            }
                            else
                            {
                                uomn = productDal.GetProductCodeUOMn(materialCode, inputType, connVM);

                                if (string.IsNullOrEmpty(uomn))
                                {
                                    if (string.IsNullOrEmpty(materialName))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                    }
                                    else
                                    {
                                        uomn = productDal.GetProductUOMn(materialName, inputType, connVM);
                                        if (string.IsNullOrEmpty(uomn))
                                        {
                                            throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                        }
                                    }

                                }

                            }

                        #endregion uomn

                            uomc = productDal.GetProductUOMc(uomn, ohUOM, connVM);

                            if (!string.IsNullOrEmpty(uomc))
                            {

                                vuomc = Convert.ToDecimal(uomc);
                            }
                            else
                            {
                                throw new ArgumentNullException("ProductName", "Could not find product's( '" + materialName + "') UOM : ('" + uomn + "')  to : ('" + ohUOM + "')  of Product ('" + finishItemName + "'('" + pCode + "')) in database");

                            }

                        }







                        vuomUseQty = Convert.ToDecimal(useQty) * vuomc;
                        vuomWastageQty = Convert.ToDecimal(wastageQty) * vuomc;







                        if (!string.IsNullOrEmpty(inputType))
                        {
                            if (inputType != "Pack" && inputType != "Overhead")
                            {
                                vRawTotal = vRawTotal + Convert.ToDecimal(vSubTotal);
                            }
                            else if (inputType == "Pack")
                            {
                                vPackingTotal = vPackingTotal + Convert.ToDecimal(vSubTotal);
                            }
                            else if (inputType == "Overhead" && vRebatePercent > 0)
                            {
                                vRebateTotal = vRebateTotal + Convert.ToDecimal(vSubTotal);
                                //bomItem.UseQuantity = 1;
                            }
                        }
                        vMarkupValue = Convert.ToDecimal(vSubTotal) * vMarkupPercent / 100;
                        vSDAmount = (Convert.ToDecimal(vSubTotal) + vMarkupValue) * vSDRate / 100;
                        vVatAmount = (Convert.ToDecimal(vSubTotal) + vMarkupValue + vSDAmount) * vVateRate / 100;

                        ////    //vRebatePercent
                        ////    string strRebatePercent = dgvBOM.Rows[i].Cells["RebatePercent"].Value.ToString();



                        bomItem.RawItemNo = materialItemNo;
                        bomItem.FinishItemNo = finishItemNo;
                        bomItem.RebateRate = Convert.ToDecimal(vRebatePercent);
                        if (bomItem.RawItemNo == "41")
                        {

                        }


                        //bomItem.RawOHCost = Convert.ToDecimal(Convert.ToDecimal(txtTotalCost.Text.Trim()) +
                        //    Convert.ToDecimal(txtMargin.Text) + Convert.ToDecimal(txtOHGTotal.Text.Trim()));
                        //bomItem.Comments = txtComments.Text.Trim();
                        bomItem.ActiveStatus = "Y";
                        //if (!string.IsNullOrEmpty(useQty))
                        //{
                        //    bomItem.UseQuantity = Convert.ToDecimal(useQty);
                        //}
                        //else 


                        bomItem.RawItemName = materialName;


                        //vSubTotal,vRebatePercent,if (inputType == "Overhead")
                        if (inputType == "Overhead")
                        {
                            if (vRebatePercent == 0)
                            {
                                bomItem.UseQuantity = 1;
                                bomItem.WastageQuantity = 0;
                                bomItem.TotalQuantity = 1;
                                bomItem.UnitCost = 0;
                                bomItem.UOM = "-";
                                bomItem.UOMc = 1;
                                bomItem.UOMn = "-";
                                bomItem.UOMPrice = 0;
                                bomItem.UOMUQty = 1;
                                bomItem.UOMWQty = 0;
                                bomItem.Cost = 0;
                            }
                            else
                            {
                                bomItem.UseQuantity = 1;
                                bomItem.WastageQuantity = 0;
                                bomItem.TotalQuantity = 1;
                                bomItem.UnitCost = Convert.ToDecimal(vSubTotal);
                                bomItem.UOM = "-";
                                bomItem.UOMc = 1;
                                bomItem.UOMn = "-";
                                bomItem.UOMPrice = Convert.ToDecimal(vSubTotal);
                                bomItem.UOMUQty = 1;
                                bomItem.UOMWQty = 0;
                                bomItem.Cost = Convert.ToDecimal(vSubTotal);
                            }
                        }
                        else
                        {
                            bomItem.UseQuantity = Convert.ToDecimal(useQty);
                            if (!string.IsNullOrEmpty(wastageQty.ToString().Trim()))
                                bomItem.WastageQuantity = Convert.ToDecimal(wastageQty);
                            //if (string.IsNullOrEmpty(totalQty.ToString().Trim()))
                            //    throw new ArgumentNullException("TotalQty", "Unable to process Total Quantity");
                            bomItem.TotalQuantity = vuomUseQty + vuomWastageQty;// Convert.ToDecimal(totalQty);
                            var tt12 = vuomUseQty + vuomWastageQty;
                            if (Convert.ToDecimal(tt12) <= 0)
                                throw new ArgumentNullException("Quantity", "Could not find product's( '" + materialName + "') useQty and WastageQty  of Product ('" + finishItemName + "'('" + pCode + "')) in import sheet");

                            //if (!string.IsNullOrEmpty(ohUnitCose.ToString().Trim()))

                            //    bomItem.UnitCost = Convert.ToDecimal(ohUnitCose);
                            //bomItem.UnitCost = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty) );
                            //vuomPrice = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty)) * vuomc;
                            vuomPrice = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty));
                            bomItem.UnitCost = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty)) * vuomc;

                            bomItem.UOM = ohUOM;
                            bomItem.UOMc = vuomc;
                            bomItem.UOMn = uomn;
                            bomItem.UOMPrice = vuomPrice;
                            bomItem.UOMUQty = vuomUseQty;
                            bomItem.UOMWQty = vuomWastageQty;
                            bomItem.Cost = Convert.ToDecimal(vSubTotal);
                        }




                        bomItem.CreatedBy = Program.CurrentUser;
                        bomItem.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        bomItem.LastModifiedBy = Program.CurrentUser;
                        bomItem.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        bomItem.VATRate = vVateRate;
                        bomItem.SD = vSDRate;
                        bomItem.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");


                        bomItem.BOMLineNo = "" + counter;
                        bomItem.PacketPrice = vPacketPrice;
                        bomItem.NBRPrice = vNbrPrice;
                        bomItem.TradingMarkUp = vMarkupValue;
                        bomItem.RawItemType = inputType;
                        bomItem.Post = "N";
                        bomItem.TradingMarkUp = vMarkupPercent;
                        bomItem.SDAmount = vSDAmount;
                        bomItem.MarkUpValue = vMarkupValue;
                        bomItem.VatAmount = vVatAmount;


                        bomItem.PBOMId = "0";
                        bomItems.Add(bomItem);

                        counter++;

                    }

                    #endregion Material Process
                    #region Overhead  Input

                    bomOhs = new List<BOMOHVM>();
                    counter = 1;
                    //DataRow[] OHOverheads = dtOH.Select("Product_Code='" + pCode + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");
                    //if (OHOverheads == null || !OHOverheads.Any())


                    foreach (DataRow row in OHOverheads)
                    {

                        //string inputType = row["Input Type"].ToString().Trim();

                        BOMOHVM bomOh = new BOMOHVM();
                        decimal vRebateAmount = 0;
                        decimal vAdditionalCost = 0;
                        decimal vHeadAmount = 0;
                        decimal vAdditionalPercent = 0;

                        string overheadItemNo = "";

                        string overheadInputType = row["InputType"].ToString().Trim();
                        //string ohPacksize = row["Packsize"].ToString().Trim();
                        string overheadCode = row["Material_Code"].ToString().Trim();
                        string materialName = row["Material Name"].ToString().Trim();
                        //materialName = materialName.Replace(" ", "_");
                        Debug.WriteLine("Overhead Name '" + materialName + "' Finish Name '" + finishItemName + "' and code '" + pCode + "'");

                        //string headAmount = row["HeadAmount"].ToString().Trim();
                        //string rebateAmount = row["RebateAmount"].ToString().Trim();
                        //string additionalCost = row["AdditionalCost"].ToString().Trim();
                        //string rebate = row["Rebate%"].ToString().Trim();
                        //string subtotal = row["SubTotal"].ToString().Trim();
                        if (string.IsNullOrEmpty(overheadInputType))
                        { throw new ArgumentNullException("BOMImport", "Cound not find input type for  ('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))"); }
                        else
                        {
                            overheadItemNo = productDal.GetProductNoByGroup(materialName, "Overhead", connVM);
                        }
                        //if (!string.IsNullOrEmpty(overheadCode))
                        //{
                        //    overheadItemNo = productDal.GetProductNoByCode(overheadCode);
                        //}
                        //if (string.IsNullOrEmpty(overheadItemNo))
                        //    if (overheadInputType == "Value addition")
                        //        overheadItemNo = productDal.GetProductNoByGroup(materialName, "Overhead");

                        var vRebate = row["Rebate%"].ToString().Trim();
                        if (string.IsNullOrEmpty(vRebate))
                        {
                            vRebatePercent = 0;
                        }
                        else
                        {
                            vRebatePercent = Convert.ToDouble(vRebate);
                        }

                        var vSTotal = row["SubTotal"].ToString().Trim().Trim();
                        if (string.IsNullOrEmpty(vSTotal))
                        {
                            vSubTotal = 0;
                        }
                        else
                        {
                            vSubTotal = Convert.ToDouble(vSTotal);
                        }

                        //decimal vRebatePercent = 0;
                        //if (!string.IsNullOrEmpty(rebate))
                        //{
                        //    vRebatePercent = Convert.ToDecimal(rebate);
                        //}
                        //decimal vSubTotal = 0;
                        //if (!string.IsNullOrEmpty(subtotal))
                        //{
                        //    vSubTotal = Convert.ToDecimal(subtotal);
                        //}






                        if (vRebatePercent == 0)
                        {
                            vHeadAmount = Convert.ToDecimal(vSubTotal);
                            vAdditionalCost = vHeadAmount;
                            vAdditionalPercent = 100;

                        }
                        else
                        {
                            vHeadAmount = Convert.ToDecimal(vSubTotal) * 100 / Convert.ToDecimal(vRebatePercent);
                            vAdditionalCost = vHeadAmount - Convert.ToDecimal(vSubTotal);
                            vAdditionalPercent = 100 - Convert.ToDecimal(vRebatePercent);

                        }


                        vRebateAmount = vHeadAmount - vAdditionalCost;
                        //vSubTotal,vRebatePercent,if (inputType == "Overhead")


                        bomOh.RebateAmount = vRebateAmount;
                        bomOh.AdditionalCost = vAdditionalCost;

                        bomOh.HeadName = materialName;
                        bomOh.HeadID = overheadItemNo;


                        bomOh.HeadAmount = vHeadAmount;
                        bomOh.CreatedBy = Program.CurrentUser;
                        bomOh.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        bomOh.LastModifiedBy = Program.CurrentUser;
                        bomOh.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        bomOh.FinishItemNo = finishItemNo;
                        bomOh.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");
                        bomOh.OHLineNo = "" + counter;
                        bomOh.RebatePercent = vAdditionalPercent;
                        bomOh.Post = "N";
                        vAdditionalTotal = vAdditionalTotal + vAdditionalCost;

                        bomOhs.Add(bomOh);
                        counter++;

                    }
                    str1 = "";
                    #endregion Overhead  Input
                    #region Overhead Margin Input

                    BOMOHVM bomOh1 = new BOMOHVM();
                    bomOh1.HeadName = "Margin";
                    bomOh1.HeadID = "ovh0";

                    if (!string.IsNullOrEmpty(margin))
                        bomOh1.HeadAmount = Convert.ToDecimal(margin);
                    bomOh1.CreatedBy = Program.CurrentUser;
                    bomOh1.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh1.LastModifiedBy = Program.CurrentUser;
                    bomOh1.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomOh1.FinishItemNo = finishItemNo;
                    bomOh1.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");
                    bomOh1.OHLineNo = "100";
                    bomOh1.RebatePercent = 0;
                    bomOh1.Post = "N";
                    bomOh1.AdditionalCost = bomOh1.HeadAmount;
                    bomOhs.Add(bomOh1);
                    vAdditionalTotal = vAdditionalTotal + bomOh1.HeadAmount;

                    #endregion Overhead Margin Input
                    #region NBR

                    bomNbrs.RawTotal = vRawTotal;
                    bomNbrs.PackingTotal = vPackingTotal;
                    bomNbrs.RebateTotal = vRebateTotal;
                    bomNbrs.AdditionalTotal = vAdditionalTotal;
                    bomNbrs.RebateAdditionTotal = vAdditionalTotal + vRebateTotal;

                    var tt_TradeMarkup = vMarkupPercent;
                    var tt_SDRate = vSDRate;
                    var tt_VATRate = vVateRate;

                    string previousSDApplyedPrice = dtProduct.Rows[i]["PreviousSDApplyedPrice"].ToString().Trim();
                    if (!string.IsNullOrEmpty(previousSDApplyedPrice))
                        bomNbrs.LastNBRPrice = Convert.ToDecimal(previousSDApplyedPrice); //col 10

                    var tt_11P = vRawTotal + vPackingTotal + vRebateTotal + vAdditionalTotal;

                    var tt11 = tt_11P * tt_TradeMarkup / 100;
                    bomNbrs.PNBRPrice = tt11 + tt_11P; // col 11
                    var tt_12 = (tt11 + tt_11P) * tt_SDRate / 100;
                    bomNbrs.SDAmount = tt_12; // col 12

                    string prevNbrPrice = dtProduct.Rows[i]["PreviousVATABLE PRICE"].ToString().Trim();
                    if (!string.IsNullOrEmpty(prevNbrPrice))
                        bomNbrs.LastNBRWithSDAmount = Convert.ToDecimal(prevNbrPrice); //col 13



                    bomNbrs.NBRWithSDAmount = tt11 + tt_11P + tt_12; // col 14
                    var tt15 = (tt11 + tt_11P + tt_12) + ((tt11 + tt_11P + tt_12) * tt_VATRate / 100);
                    bomNbrs.WholeSalePrice = tt15;
                    if (!string.IsNullOrEmpty(packetPrice))
                        vPacketPrice = Convert.ToDecimal(packetPrice);
                    if (!string.IsNullOrEmpty(packetPrice))
                    {
                        bomNbrs.PPacketPrice = Convert.ToDecimal(packetPrice);
                    }

                    // col 16

                    bomNbrs.IsImported = "Y";
                    if (Vat_Name == "VAT 4.3 (Toll Issue)")
                    {
                        bomNbrs.RawOHCost = vRebateTotal;
                    }
                    else
                    {
                        bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;

                    }
                    bomNbrs.CreatedBy = Program.CurrentUser;
                    bomNbrs.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomNbrs.LastModifiedBy = Program.CurrentUser;
                    bomNbrs.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    bomNbrs.ActiveStatus = "Y";
                    bomNbrs.Post = "N";
                    bomNbrs.CustomerID = "0";
                    #endregion NBR

                    retResults = bom1.BOMInsert(bomItems, bomOhs, bomNbrs, connVM);
                    MessageBox.Show("Peice decleration of Product ('" + finishItemName + "'('" + pCode + "') and VAT name ('" + bomNbrs.VATName + "') successsfully complete");





                    //this.btnSave.Enabled = false;
                    //this.progressBar1.Visible = true;
                }
                return retResults;
                #endregion Process model


                //bgwSave.RunWorkerAsync();
                retResults[0] = "Success";
                retResults[1] = "Price declaration import successful";
                if (string.IsNullOrEmpty(retResults[0]))

                    str1 = "";
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                if (error.Length <= 1)
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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

            return retResults;

            #endregion
        }

        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpEffectDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                cmbRProduct.Focus();
            }
        }

        #endregion

        #region Methods 09

        private void txtMargin_KeyDown(object sender, KeyEventArgs e)
        {
            //btnSave.Focus();
        }

        private void txtPacketPrice_KeyDown(object sender, KeyEventArgs e)
        {
            //txtMargin.Focus();
        }

        private void txtTotalQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtRWastageInp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }

        }

        private void txtPBOMId_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtFIssuePrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void bgwProductSearch_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void btnCosting_Click(object sender, EventArgs e)
        {
            #region Elements Stats

            progressBar1.Visible = true;

            #endregion

            #region Declarations

            sqlResults = new string[2];
            IBOM bomDal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);
            CommonDAL commonDal = new CommonDAL();
            //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);
            IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

            DataSet ReportResult = new DataSet();
            DataTable dtCost = new DataTable();
            ReportDocument objrpt = new ReportDocument();

            #endregion

            #region Try Statement

            try
            {

                #region Check Point

                if (dgvBOM.RowCount <= 0)
                {
                    MessageBox.Show("No Items Found for Costing.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                #endregion

                string RptBOM_Costing_A4 = commonDal.settingsDesktop("BOM", "RptBOMCostingA4",null,connVM);

                #region For Loop

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    string itemNo = dgvBOM["RawItemNo", i].Value.ToString();
                    string effectDate = dtpEffectDate.Value.AddDays(1).ToString("yyyy-MMM-dd HH:mm:ss");
                    string isPurchase = dgvBOM["PInvoiceNo", i].Value.ToString();

                    #region Debug

                   

                    #endregion

                    #region Continue Statement

                    if (isPurchase == "0")
                    {
                        continue;
                    }

                    #endregion
                    if (!string.IsNullOrEmpty(isPurchase) || isPurchase != "-")
                    {
                        string RawItemNo = dgvBOM["RawItemNo", i].Value.ToString();
                        string UOMc = dgvBOM["UOMc", i].Value.ToString();
                        string TotalQty = dgvBOM["TotalQty", i].Value.ToString();
                        string CostPrice = dgvBOM["CostPrice", i].Value.ToString();
                        string UOM = dgvBOM["UOM", i].Value.ToString();
                        string UOMn = dgvBOM["UOMn", i].Value.ToString();

                        //FileLogger.Log(this.Name, "btnCosting_Click", UOMc + " "+ TotalQty + " "+ CostPrice);

                        ReportResult = reportDsdal.PurchaseNew(isPurchase,
                                                               "", "", "", RawItemNo,
                                                               "", "", "", "Y", "", "", "Y"
                                                               , UOM
                                                               , UOMn
                                                               , Convert.ToDecimal(UOMc)
                                                               , Convert.ToDecimal(TotalQty)
                                                               , Convert.ToDecimal(CostPrice)
                                                               , false, null, 0, "","", connVM);

                        if (ReportResult.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] dr = ReportResult.Tables[0].Select("IsRaw <> 'Overhead'");
                            if (dr.Length > 0)
                            {
                                DataTable drDatatable = new DataTable();
                                drDatatable = dr.CopyToDataTable();
                                //drDatatable = OrdinaryVATDesktop.CopyTableInString(drDatatable);

                                dtCost.Merge(drDatatable);
                                //if (dtCost.Rows.Count == 1)
                                //{
                                //    dtCost = OrdinaryVATDesktop.CopyTableInString(dtCost);
                                //}
                            }

                        }
                    }
                    else
                    {
                        #region Find Costing From

                        sqlResults = bomDal.FindCostingFrom(itemNo, effectDate, null, null, null, connVM);

                        #endregion

                        if (sqlResults.Length > 0)
                        {
                            string invoiceId = sqlResults[0];
                            string message = sqlResults[1];

                            #region From Purchase

                            if (message == "FromPurchase")
                            {
                                string RawItemNo = dgvBOM["RawItemNo", i].Value.ToString();
                                string UOMc = dgvBOM["UOMc", i].Value.ToString();
                                string TotalQty = dgvBOM["TotalQty", i].Value.ToString();
                                string CostPrice = dgvBOM["CostPrice", i].Value.ToString();
                                string UOM = dgvBOM["UOM", i].Value.ToString();
                                string UOMn = dgvBOM["UOMn", i].Value.ToString();

                                //FileLogger.Log(this.Name, "btnCosting_Click", UOMc + " "+ TotalQty + " "+ CostPrice);

                                ReportResult = reportDsdal.PurchaseNew(invoiceId,
                                                                       "", "", "", RawItemNo,
                                                                       "", "", "", "Y", "", "", "Y"
                                                                       , UOM
                                                                       , UOMn
                                                                       , Convert.ToDecimal(UOMc)
                                                                       , Convert.ToDecimal(TotalQty)
                                                                       , Convert.ToDecimal(CostPrice)
                                                                       , false, null, 0, "","", connVM);

                                if (ReportResult.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] dr = ReportResult.Tables[0].Select("IsRaw <> 'Overhead'");
                                    if (dr.Length > 0)
                                    {
                                        DataTable drDatatable = new DataTable();
                                        drDatatable = dr.CopyToDataTable();
                                        //drDatatable = OrdinaryVATDesktop.CopyTableInString(drDatatable);

                                        dtCost.Merge(drDatatable);
                                        //if (dtCost.Rows.Count == 1)
                                        //{
                                        //    dtCost = OrdinaryVATDesktop.CopyTableInString(dtCost);
                                        //}
                                    }

                                }
                            }

                            #endregion

                            #region From Costing

                            else if (message == "FromCosting")
                            {
                                ReportResult = reportDsdal.CostingNew(invoiceId, itemNo,
                                                                       dgvBOM["UOM", i].Value.ToString(),
                                                                       dgvBOM["UOMn", i].Value.ToString(),
                                                                       Convert.ToDecimal(dgvBOM["UOMc", i].Value.ToString()),
                                                                       Convert.ToDecimal(
                                                                           dgvBOM["TotalQty", i].Value.ToString()),
                                                                       Convert.ToDecimal(
                                                                           dgvBOM["CostPrice", i].Value.ToString()), connVM);


                                dtCost.Merge(ReportResult.Tables[0]);
                            }

                            #endregion

                        }
                    }
                }

                #endregion

                #region Check Point

                if (dtCost.Rows.Count <= 0)
                {
                    MessageBox.Show("There have no data to preview", this.Text);
                    return;
                }

                #endregion

                #region Datatable Name

                dtCost.TableName = "DsPurchase";

                #endregion

                #region Report Call

                if (RptBOM_Costing_A4.ToLower() == "y")
                {
                    objrpt = new ReportDocument();
                    objrpt = new RptBOM_CostingA4();
                    ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptBOM_CostingA4.rpt");

                }
                else
                {
                    objrpt = new ReportDocument();
                    objrpt = new RptBOM_Costing_Legal();
                }

                #endregion

                #region Set Data Source

                objrpt.SetDataSource(dtCost);

                #endregion

                #region Formula Fields

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["FinishItemCode"].Text = "'" + txtFProductCode.Text + "'";
                objrpt.DataDefinition.FormulaFields["FinishItemName"].Text = "'" + txtFProductName.Text + "'";
                objrpt.DataDefinition.FormulaFields["BOMCode"].Text = "'" + lblBOMId.Text + "'";

                #endregion

                #region Show Report

                FormReport reports = new FormReport();
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);

                #region License Check

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    reports.WindowState = FormWindowState.Maximized;
                }

                #endregion

                reports.Show();

                #endregion

            }

            #endregion

            #region Catch Statement

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCosting_Click", ex.ToString());
            }
            finally
            {
                progressBar1.Visible = false;
            }

            #endregion
        }

        private void btnPurchaseRateReload_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to Reload Cost Price?" + "\n" + "BOM", this.Text, MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {


                string PinvoiceNo = string.Empty;
                string InputType = string.Empty;
                string ItemNo = string.Empty;

                decimal uomPrice = 0;
                decimal uomC = 0;
                decimal UnitCost = 0;
                decimal Cost = 0;
                decimal TotalQty = 0;

                decimal PurchaseCostPrice = 0;
                decimal PurchaseQuantity = 0;
                decimal CostPrice = 0;
                decimal NewCostPrice = 0;

                DataTable dt = new DataTable();
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                string strEffectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    ItemNo = dgvBOM["RawItemNo", i].Value.ToString();
                    InputType = dgvBOM["InputType", i].Value.ToString();
                    uomPrice = Convert.ToDecimal(dgvBOM["UOMPrice", i].Value);
                    uomC = Convert.ToDecimal(dgvBOM["UOMc", i].Value);
                    UnitCost = Convert.ToDecimal(dgvBOM["UnitCost", i].Value);
                    Cost = Convert.ToDecimal(dgvBOM["Cost", i].Value);
                    TotalQty = Convert.ToDecimal(dgvBOM["TotalQty", i].Value);
                    PinvoiceNo = dgvBOM["PinvoiceNo", i].Value.ToString();
                    CostPrice = Convert.ToDecimal(dgvBOM["CostPrice", i].Value);

                    if (InputType != "Overhead")
                    {
                        dt = new DataTable();
                        dt = productDal.GetLIFOPurchaseInformation(ItemNo, strEffectDate, "", connVM);
                        if (dt.Rows.Count > 0)
                        {
                            PurchaseCostPrice = Convert.ToDecimal(dt.Rows[0]["PurchaseCostPrice"].ToString());
                            PurchaseQuantity = Convert.ToDecimal(dt.Rows[0]["PurchaseQuantity"].ToString());
                            CostPrice = Convert.ToDecimal(dt.Rows[0]["CostPrice"].ToString());
                            CostPrice = Convert.ToDecimal(Program.FormatingNumeric(Cost.ToString(), BOMDPlaceNetCost));
                            NewCostPrice = 0;

                            PinvoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                            if (PurchaseQuantity != 0)
                            {
                                NewCostPrice = PurchaseCostPrice / PurchaseQuantity;
                            }
                            NewCostPrice = Convert.ToDecimal(Program.FormatingNumeric(NewCostPrice.ToString(), BOMDPlaceA));
                            UnitCost = NewCostPrice * uomC;
                            UnitCost = Convert.ToDecimal(Program.FormatingNumeric(UnitCost.ToString(), BOMDPlaceA));
                            Cost = NewCostPrice * uomC * TotalQty;
                            Cost = Convert.ToDecimal(Program.FormatingNumeric(Cost.ToString(), BOMDPlaceNetCost));

                        }
                        else
                        {
                            PinvoiceNo = "0";
                        }

                        dgvBOM["UOMPrice", i].Value = NewCostPrice;
                        dgvBOM["UnitCost", i].Value = UnitCost;
                        dgvBOM["Cost", i].Value = Cost;
                        dgvBOM["PinvoiceNo", i].Value = PinvoiceNo;
                        dgvBOM["CostPrice", i].Value = CostPrice;

                    }



                }
                Rowcalculate();
                MessageBox.Show("Reload Cost Price Successfully", this.Text);
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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCosting_Click",

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

                FileLogger.Log(this.Name, "btnCosting_Click",

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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCosting_Click",

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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               exMessage);
            }

            #endregion
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            FormRptFinishProductInformation frmFinishProductSearch = new FormRptFinishProductInformation();
            frmFinishProductSearch.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to refresh without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {

                        //ProductSearchDs();

                        //OverHeadSearch();
                        AllClear();
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {


                    //ProductSearchDs();

                    //OverHeadSearch();

                    AllClear();
                    ChangeData = false;
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
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }

            #endregion Catch
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to refresh without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {

                        //ProductSearchDs();

                        //OverHeadSearch();
                        AllClear();
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {


                    //ProductSearchDs();

                    //OverHeadSearch();

                    AllClear();
                    ChangeData = false;
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

            #endregion Catch
        }

        private void chkCustCode_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 10

        private DataTable CustomerResult;
        private List<CustomerSinmgleDTO> customerMini = new List<CustomerSinmgleDTO>();

        private void cmbCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void cmbCustomerName_Leave(object sender, EventArgs e)
        {
        }

        private void txtCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormCustomerFinder.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    vCustomerID = "0";
                    txtCustomer.Text = "";
                    CustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                }
                txtCustomer.Focus();

            }
        }

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            vCustomerID = "0";
            txtCustomer.Text = "";
        }

        public string vCustomerID = "0";

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkIssueOnProduction_CheckedChanged(object sender, EventArgs e)
        {
            if (IssueRequired.ToLower()=="y")
            {
                chkIssueOnProduction.Checked = true;
            }
            else
            {
                chkIssueOnProduction.Text = "Not Issue";

                if (chkIssueOnProduction.Checked)
                {
                    chkIssueOnProduction.Text = "Issue";
                }
            }
            
        }

        private void txtSalePrice_TextChanged(object sender, EventArgs e)
        {
            //decimal SalePrice = 0;
            //decimal GTotal = 0;
            //decimal  Margin = 0;
            //SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            //GTotal = Convert.ToDecimal(txtGTotal.Text.Trim());
            ////SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            //Margin = SalePrice - GTotal;
            //txtMargin.Text = Margin.ToString();

        }

        private void txtSalePrice_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtSalePrice, "SalePrice");
            txtSalePrice.Text = Program.ParseDecimalObject(txtSalePrice.Text.Trim()).ToString();

            decimal SalePrice = 0;
            decimal GTotal = 0;
            decimal Margin = 0;
            SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            GTotal = Convert.ToDecimal(txtGTotal.Text.Trim());
            //SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            Margin = SalePrice - GTotal;
            txtMargin.Text = Margin.ToString();
        }

        private void btnMarginLoad_Click(object sender, EventArgs e)
        {
            decimal TotalCost = 0;
            decimal OHGTotal = 0;

            decimal SalePrice = 0;
            decimal Margin = 0;
            SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            TotalCost = Convert.ToDecimal(txtTotalCost.Text.Trim());
            OHGTotal = Convert.ToDecimal(txtOHGTotal.Text.Trim());
            //txtGrandTotal.Text = txtSalePrice.Text.Trim();
            //SalePrice = Convert.ToDecimal(txtSalePrice.Text.Trim());
            Margin = SalePrice - (TotalCost + OHGTotal);
            txtMargin.Text = Margin.ToString();

            if (Program.CheckingNumericTextBox(txtMargin, "Margin") == true)
            {
                txtMargin.Text = Program.FormatingNumeric(txtMargin.Text.Trim(), BOMDPlaceA).ToString();
                SumQty();
                GTotal();
            }
            //txtPacketPrice.Focus();
        }

        private void txtUomConv_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtUomConv, "UomConv");
            txtUomConv.Text = Program.ParseDecimalObject(txtUomConv.Text.Trim()).ToString();
        }

        private void txtNetCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtNetCost, "NetCost");
            txtNetCost.Text = Program.ParseDecimalObject(txtNetCost.Text.Trim()).ToString();
        }

        private void txtTrading_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTrading, "Trading");
            txtTrading.Text = Program.ParseDecimalObject(txtTrading.Text.Trim()).ToString();
        }

        #endregion

        #region Methods 11

        private void txtTotalCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalCost, "TotalCost");
            txtTotalCost.Text = Program.ParseDecimalObject(txtTotalCost.Text.Trim()).ToString();
        }

        private void txtGTotal_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtGTotal, "GTotal");
            txtGTotal.Text = Program.ParseDecimalObject(txtGTotal.Text.Trim()).ToString();
        }

        private void txtOHGTotal_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtOHGTotal, "OHGTotal");
            txtOHGTotal.Text = Program.ParseDecimalObject(txtOHGTotal.Text.Trim()).ToString();
        }

        private void txtGrandTotal_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtGrandTotal, "GrandTotal");
            txtGrandTotal.Text = Program.ParseDecimalObject(txtGrandTotal.Text.Trim()).ToString();
        }

        private void dtpEffectDate_Leave(object sender, EventArgs e)
        {
            dtpEffectDate.Value = Program.ParseDate(dtpEffectDate);
        }

        private void dtpFirstSupplyDate_Leave(object sender, EventArgs e)
        {
            dtpFirstSupplyDate.Value = Program.ParseDate(dtpFirstSupplyDate);
        }

        #endregion

        private void txtFUOM_Leave(object sender, EventArgs e)
        {

        }

        private void cmbFUOM_Leave(object sender, EventArgs e)
        {

        }



    }
}
