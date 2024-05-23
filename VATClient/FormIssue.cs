using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.IO;
using VATClient.ReportPreview;
using VATClient.ReportPages;
using VATClient.ModelDTO;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using SymphonySofttech.Reports;
using CrystalDecisions.CrystalReports.Engine;

namespace VATClient
{
    public partial class FormIssue : Form
    {
        #region Constructors

        public FormIssue()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

            //SetupBOMIssue();
            //if (Program.IssueFromBOM == "Y")
            //{
            //    MessageBox.Show("Raw product is issue with Finish product receiving", this.Text, MessageBoxButtons.OK,
            //                    MessageBoxIcon.Information);
            //    //this.Close();
            //    return;
            //}

        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        string transactionType = string.Empty;

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        string encriptedIssueHeaderData;
        private string NextID = string.Empty;
        private bool ChangeData = false;
        private DataTable ProductResultDs;
        public string VFIN = "221";
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string CategoryId { get; set; }

        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private DataTable uomResult;
        List<UomDTO> UOMs = new List<UomDTO>();
        private int IssuePlaceQty;
        private int IssuePlaceAmt;
        private int SearchBranchId = 0;
        private ReportDocument reportDocument = new ReportDocument();

        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();

        #region Global Variables For BackGroundWorker

        private string IssueHeaderData = string.Empty;

        private string IssueDetailData = string.Empty;

        private string IssueResult = string.Empty;

        private string IssueResultPost = string.Empty;

        private DataTable IssueDetailResult;
        private DataTable IssueDetailsResult;

        private string ProductData = string.Empty;
        private string ImportId = string.Empty;

        private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;

        private IssueMasterVM issueMasterVM;

        private List<IssueDetailVM> issueDetailVMs = new List<IssueDetailVM>();

        private DataSet StatusResult;

        private DataTable ProductTypeResult;
        private bool Edit = false;
        private bool Add = false;

        #endregion

        #endregion

        #region Methods 01 / Load

        private void FormIssue_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Button Stats

                btnSave.Text = "&Add";
                txtIssueNo.Text = "~~~ New ~~~";

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                if (transactionType == "IssueWastage")
                {
                    label16.Visible = false;
                }

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Reset Form

                ClearAllFields();

                #endregion

                #region Flag Update

                ChangeData = false;

                #endregion

                #region Tool Tip

                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");

                #endregion

                #region Button Stats

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region Settings Set


                string vIssuePlaceQty, vIssuePlaceAmt, vIssueFromBOM, vIssueAutoPost = string.Empty;


                CommonDAL commonDal = new CommonDAL();

                vIssuePlaceQty = commonDal.settingsDesktop("Issue", "Quantity",null,connVM);
                vIssuePlaceAmt = commonDal.settingsDesktop("Issue", "Amount",null,connVM);
                vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM",null,connVM);
                vIssueAutoPost = commonDal.settingsDesktop("IssueFromBOM", "IssueAutoPost",null,connVM);
                if (string.IsNullOrEmpty(vIssuePlaceQty)
                    || string.IsNullOrEmpty(vIssuePlaceAmt)
                    || string.IsNullOrEmpty(vIssueFromBOM)
                    || string.IsNullOrEmpty(vIssueAutoPost))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                IssuePlaceQty = Convert.ToInt32(vIssuePlaceQty);
                IssuePlaceAmt = Convert.ToInt32(vIssuePlaceAmt);
                bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

                if (IssueFromBOM == true)
                {
                    bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                    if (IssueAutoPost == false)
                    {

                        btnSave.Enabled = false;
                    }

                }
                #endregion Settings

                #region ProgressBar Stats

                progressBar1.Visible = true;

                #endregion

                #region Background Load

                bgwLoad.RunWorkerAsync();

                #endregion


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
                FileLogger.Log(this.Name, "FormIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormIssue_Load", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormIssue_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormIssue_Load", exMessage);
            }
            #endregion
        }

        private void FormMaker()
        {
            try
            {
                btnSearchIssueNoP.Visible = false;
                txtIssueNoP.Visible = false;
                #region Transaction Type

                if (rbtnOther.Checked)
                {
                    this.Text = "Item Issue ";
                    label16.Visible = false;
                }
                if (rbtnTollitemIssueWithoutBOM.Checked)
                {
                    this.Text = "Toll Item Issue Without BOM ";
                    btnVAT16.Visible = false;
                }
                else if (btnContractorIssue.Checked)
                {
                    this.Text = "Item Cnotractor Issue ";

                }

                else if (rbtnIssueReturn.Checked)
                {
                    this.Text = "Item Issue Return";
                    btnSearchIssueNoP.Visible = true;
                    txtIssueNoP.Visible = true;
                }

                if (rbtnIssueWastage.Checked)
                {
                    this.Text = "Issue Wastage";
                }

                #endregion Transaction Type

                #region ChangeableNBRPrice  Change
                CommonDAL commonDal = new CommonDAL();
                bool ChangeableNbrPrice = commonDal.settingsDesktop("Production", "ChangeableNBRPrice",null,connVM) == "Y" ? false : true;
                txtUnitCost.ReadOnly = ChangeableNbrPrice;
                #endregion

                #region Button Import Integration Lisence
                if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
                {
                    btnImport.Visible = false;
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

        private void FormLoad()
        {
            #region Product
            if (CategoryId == null)
            {


                varItemNo = "";
                varCategoryID = "";
                varIsRaw = "Raw";
                varHSCodeNo = "";
                varActiveStatus = "Y";
                varTrading = "N";
                varNonStock = "N";
                varProductCode = "";
            }
            else
            {


                varItemNo = "";
                varCategoryID = "CategoryId";
                varIsRaw = "";
                varHSCodeNo = "";
                varActiveStatus = "";
                varTrading = "";
                varNonStock = "";
                varProductCode = "";
            }
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

            txtCategoryName.Text = "Raw";
            #endregion Product


            #region 2012 Law - Button Control

            btnVAT16.Text = "6.1";
            btnVAT18.Visible = false;

            #endregion


        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region UOM
                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

                #region Product
                ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, "", "", varProductCode, connVM);

                #endregion Product

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

                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                    ProductsMini.Add(prod);
                }//End FOR
                ProductSearchDsLoad();

                #endregion Product

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
                ChangeData = false;
                this.progressBar1.Visible = false;
            }

        }

        public void ClearAllFields()
        {
            txtHiddenInvoiceNo.Text = "";
            txtId.Text = "0";
            SearchBranchId = 0;
            txtFiscalYear.Text = "0";
            cmbIsRaw.Text = "Select";
            txtQuantityInHand.Text = "0.0";
            txtPCode.Text = "";
            txtSD.Text = "0.00";
            txtComments.Text = "";
            txtCommentsDetail.Text = "NA";
            txtHSCode.Text = "";
            txtIssueNo.Text = "";
            txtNBRPrice.Text = "0.00";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtSerialNo.Text = "";
            txtTotalAmount.Text = "0.00";
            txtTotalVATAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            txtVATRate.Text = "0.00";
            cmbIsRaw.Text = "";
            txtUOM.Text = "";
            txtTotalQuantity.Text = "0.00";

            CommonDAL commonDal = new CommonDAL();
            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate",null,connVM);
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpIssueDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
            else
            {
                dtpIssueDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            }

            dgvIssue.Rows.Clear();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void picIssue_Click(object sender, EventArgs e)
        {

        }

        private void TransactionTypes()
        {
            #region Transaction Type
            transactionType = string.Empty;
            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }
            if (rbtnTollitemIssueWithoutBOM.Checked)
            {
                transactionType = "TollitemIssueWithoutBOM";
            }


            else if (rbtnIssueReturn.Checked)
            {
                transactionType = "IssueReturn";
            }

            else if (rbtnIssueWastage.Checked)
            {
                transactionType = "IssueWastage";
            }

            else if (btnContractorIssue.Checked)
            {
                transactionType = "ContractorIssueWoBOM";
            }

            #endregion Transaction Type
        }

        #endregion

        #region Methods 02 / Add, Change, Remove

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                string ReportName = new CommonDAL().settingValue("Reports", "VAT6_3");

                ChangeData = true;
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item", this.Text);
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

                if (transactionType != "IssueWastage")
                {
                    if (ReportName.ToLower() != "bpl1")
                    {
                        if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                        {
                            MessageBox.Show(MessageVM.msgNegPrice);
                            txtUnitCost.Focus();
                            return;
                        }
                    }

                    
                }


                if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }

                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }

                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtQuantity.Focus();
                    return;
                }


                //for (int i = 0; i < dgvIssue.RowCount; i++)
                //{
                //    if (dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text)
                //    {
                //        MessageBox.Show("Same Product already exist.", this.Text);

                //        return;
                //    }
                //}

                #region Check Stock


                if (rbtnIssueReturn.Checked == false)
                {
                    //if (
                    //    Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text.Trim()) > Convert.ToDecimal(txtQuantityInHand.Text))
                    //{
                    //    MessageBox.Show("Stock Not available");
                    //    txtQuantity.Focus();
                    //    return;
                    //}
                }

                #endregion Check Stock
                //if (cmbUom.SelectedIndex == -1)
                //{
                //    throw new ArgumentNullException(this.Text, "Please select pack size");
                //}

                UomsValue();


                DataGridViewRow NewRow = new DataGridViewRow();
                dgvIssue.Rows.Add(NewRow);

                dgvIssue["BOMId", dgvIssue.RowCount - 1].Value = txtBOMId.Text.Trim();

                dgvIssue["ItemNo", dgvIssue.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvIssue["ItemName", dgvIssue.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvIssue["PCode", dgvIssue.RowCount - 1].Value = txtPCode.Text.Trim();
                //dgvIssue["UOM", dgvIssue.RowCount - 1].Value = txtUOM.Text.Trim();

                string strUom = cmbUom.Text.ToString();
                dgvIssue["UOM", dgvIssue.RowCount - 1].Value = strUom.Trim();

                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();
                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();

                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), IssuePlaceAmt).ToString();

                dgvIssue["Quantity", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();


                dgvIssue["UnitPrice", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();

                dgvIssue["VATRate", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();//"0.00");
                dgvIssue["Comments", dgvIssue.RowCount - 1].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvIssue["NBRPrice", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();
                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "New";
                dgvIssue["Stock", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
                dgvIssue["Previous", dgvIssue.RowCount - 1].Value = 0;// txtQuantity.Text.Trim();
                dgvIssue["Change", dgvIssue.RowCount - 1].Value = 0;
                dgvIssue["SD", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();//"0.00");

                //dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();

                dgvIssue["UOMc", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvIssue["UOMn", dgvIssue.RowCount - 1].Value =
                    txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim()));


                Rowcalculate();

                AllClear();
                selectLastRow();

                cmbProductName.Focus();

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

        private void dgvIssue_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvIssue.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (chkPCode.Checked)
                //{
                //    cmbProduct.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();

                //}
                //else
                //{
                //    cmbProduct.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();

                //}


                txtBOMId.Text = dgvIssue.CurrentRow.Cells["BOMId"].Value.ToString();

                cmbProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();

                txtLineNo.Text = dgvIssue.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                txtUnitCost.Text = Program.ParseDecimalObject(dgvIssue.CurrentRow.Cells["UnitPrice"].Value).ToString();//"0,0.00");
                txtVATRate.Text = Program.ParseDecimalObject(dgvIssue.CurrentRow.Cells["VATRate"].Value).ToString();//"0.00");
                txtCommentsDetail.Text = "NA";// dgvIssue.CurrentRow.Cells["Comments"].Value.ToString();
                txtNBRPrice.Text = Program.ParseDecimalObject(dgvIssue.CurrentRow.Cells["NBRPrice"].Value).ToString();//"0,0.00");

                txtPrevious.Text = Program.ParseDecimalObject(dgvIssue.CurrentRow.Cells["Previous"].Value).ToString();//"0,0.0000");

                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());
                Uoms();
                cmbUom.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();

                /*UOM Convertion */



                txtUomConv.Text = dgvIssue.CurrentRow.Cells["UOMc"].Value.ToString();

                /*UOM Convertion */
                ProductDAL productDal = new ProductDAL();
                //productDal productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                //txtQuantityInHand.Text = productDal.StockInHand(txtProductCode.Text, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                //                                                                DateTime.Now.ToString(" HH:mm:ss"),
                //                                                            null,
                //                                                            null).ToString();

                if (string.Equals(transactionType, "ContractorIssueWoBOM", StringComparison.OrdinalIgnoreCase))
                {
                    var dtTollStock = productDal.GetTollStock(new ParameterVM()
                    {
                        ItemNo = txtProductCode.Text,
                    });

                    txtQuantityInHand.Text = dtTollStock.Rows[0][0].ToString();

                }
                else
                {
                    txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                        DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, false, connVM, Program.CurrentUserID).Rows[0]["Quantity"].ToString();
 
                }

             
                PirceCall();
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
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", exMessage);
            }
            #endregion
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dgvIssue.RowCount > 0)
            {
                ReceiveChangeSingle();
            }
            else
            {
                MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvIssue.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvIssue.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvIssue.Rows.RemoveAt(dgvIssue.CurrentRow.Index);
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

        private void remove()
        {
            #region try
            try
            {
                ChangeData = true;

                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show("Please select a Item", this.Text);
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }

                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                CurrentValue = 0;

                if (rbtnIssueReturn.Checked == false)
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

                dgvIssue.CurrentRow.Cells["Status"].Value = "Delete";



                dgvIssue.CurrentRow.Cells["Quantity"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["UnitPrice"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["VATRate"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["VATAmount"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SubTotal"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["NBRPrice"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SD"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SDAmount"].Value = 0.00;

                dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvIssue.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                Rowcalculate();
                //AllClear();
                txtProductCode.Text = "";
                txtProductName.Text = "";


                cmbProductName.Focus();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReceiveChangeSingle()
        {
            try
            {
                string ReportName = new CommonDAL().settingValue("Reports", "VAT6_3");

                #region try
                if (
                    string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (transactionType != "IssueWastage")
                {
                    if (ReportName.ToLower() != "bpl1")
                    {
                        if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                        {
                            MessageBox.Show(MessageVM.msgNegPrice);
                            txtUnitCost.Focus();
                            return;
                        }
                    }
                }

                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }

                ChangeData = true;


                if (string.IsNullOrEmpty(cmbUom.Text))
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }


                UomsValue();

                //decimal quantity = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value);
                //if (Convert.ToDecimal(txtQuantity.Text) > quantity)
                //{
                //    MessageBox.Show("Return quantity can not be greater than actual quantity.");
                //    txtQuantity.Text = quantity.ToString();
                //    txtQuantity.Focus();
                //    return;
                //}   
                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text);

                if (rbtnIssueReturn.Checked == false)
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
                    if (CurrentValue <= PreviousValue)
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
                    else
                    {
                        MessageBox.Show("Return quantity can not be greater than actual quantity.");
                        txtQuantity.Text = PreviousValue.ToString();
                        txtQuantity.Focus();
                        return;
                    }

                }

                #endregion Stock Chekc


                string strUom = cmbUom.Text;
                dgvIssue["BOMId", dgvIssue.CurrentRow.Index].Value = txtBOMId.Text.Trim();// txtUOM.Text.Trim();


                dgvIssue["UOM", dgvIssue.CurrentRow.Index].Value = strUom.Trim();
                //dgvIssue["PCode", dgvIssue.CurrentRow.Index].Value = txtPCode.Text.Trim();
                dgvIssue["Quantity", dgvIssue.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();//"0,0.000");

                dgvIssue["UnitPrice", dgvIssue.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvIssue["UOMPrice", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();

                //dgvIssue["UnitPrice", dgvIssue.CurrentRow.Index].Value = Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString();//"0,0.00");
                dgvIssue["VATRate", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();//"0.00");
                dgvIssue["Comments", dgvIssue.CurrentRow.Index].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvIssue["NBRPrice", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();//"0,0.00");
                dgvIssue["SD", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();//"0.00");

                dgvIssue["UOMc", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvIssue["UOMn", dgvIssue.CurrentRow.Index].Value = txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                    Convert.ToDecimal(txtUomConv.Text.Trim()));

                if (dgvIssue.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvIssue["Status", dgvIssue.CurrentRow.Index].Value = "Change";
                }
                dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;

                //dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvIssue.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtCommentsDetail.Text = "NA";
                txtQuantityInHand.Text = "0.00";
                txtBOMId.Text = "";



                cmbProductName.Focus();

                #endregion
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                //string err = ex.Message.ToString();
                //string[] error = err.Split(FieldDelimeter.ToCharArray());
                //FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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


        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {

            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();

            ////////Program.FormatTextBoxQty4(txtQuantity, "Quantity");

            //if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            //{
            //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();

            //}
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void ProductSearchDsFormLoad()
        {
            //string ProductData = string.Empty;
            #region try
            try
            {

                if (CategoryId == null)
                {
                    varItemNo = "";
                    varCategoryID = "";
                    varIsRaw = "Raw";
                    varHSCodeNo = "";
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                    txtCategoryName.Text = "Raw";

                }
                else
                {
                    varItemNo = "";
                    varCategoryID = CategoryId;
                    varIsRaw = "";
                    varHSCodeNo = txtCategoryName.Text.Trim();
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                }


                this.button1.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerProductSearchDsFormLoad.RunWorkerAsync();


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

        #endregion

        #region Methods 03 / Save, Update, Post, Search

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try

            try
            {

                #region Check Point

                #region Find Fiscal Year Lock

                string PeriodName = dtpIssueDate.Value.ToString("MMMM-yyyy");
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

                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }



                #region Null Check

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtIssueNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                #endregion

                #region Exist Check

                IssueMasterVM vm = new IssueMasterVM();

                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new IssueDAL().SelectAllList(0, new[] { "IssueNo" }, new[] { NextID },null,null,null,connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #endregion

                if (dgvIssue.RowCount <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Master


                issueMasterVM = new IssueMasterVM();

                issueMasterVM.IssueNo = NextID.ToString();
                issueMasterVM.IssueDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.TotalVATAmount = 0;
                issueMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                issueMasterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                issueMasterVM.Comments = txtComments.Text.Trim();
                issueMasterVM.CreatedBy = Program.CurrentUser;
                issueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.LastModifiedBy = Program.CurrentUser;
                issueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.ReceiveNo = NextID.ToString();

                issueMasterVM.transactionType = transactionType;
                issueMasterVM.Post = "N";
                issueMasterVM.ReturnId = txtIssueNoP.Text.Trim();
                issueMasterVM.BranchId = Program.BranchId;
                issueMasterVM.AppVersion = Program.GetAppVersion();
                //encriptedIssueHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, IssueHeaderData);
                #endregion

                #region Details


                issueDetailVMs = new List<IssueDetailVM>();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    IssueDetailVM detail = new IssueDetailVM();

                    detail.BOMId = Convert.ToInt32(dgvIssue.Rows[i].Cells["BOMId"].Value);
                    detail.IssueNoD = NextID.ToString();
                    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = 0;
                    detail.VATAmount = 0;
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.CommentsD = "FromIssue";

                    detail.ReceiveNoD = NextID.ToString();
                    detail.IssueDateTimeD = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.SD = 0;
                    detail.SDAmount = 0;
                    detail.Wastage = 0;
                    detail.BOMDate = "1900-01-01";
                    detail.FinishItemNo = "0";

                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());
                    detail.BranchId = Program.BranchId;
                    issueDetailVMs.Add(detail);

                }
                #endregion

                #region Check Point

                if (issueDetailVMs.Count() <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker

                backgroundWorkerSave.RunWorkerAsync();

                #endregion

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
            #region try

            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                //sqlResults = issueDal.IssueInsert(issueMasterVM, issueDetailVMs);
                sqlResults = issueDal.IssueInsert(issueMasterVM, issueDetailVMs, null, null, connVM);
                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }

            #endregion

            #region catch

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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            #endregion


        }

        private void backgroundWorkerSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

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
                            txtIssueNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvIssue.RowCount; i++)
                            {
                                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
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
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                #region Check Point

                #region Find Fiscal Year Lock

                string PeriodName = dtpIssueDate.Value.ToString("MMMM-yyyy");
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

                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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

                #region Transaction Types

                TransactionTypes();

                #endregion

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign

                #region Master Data

                issueMasterVM = new IssueMasterVM();

                issueMasterVM.IssueNo = NextID.ToString();
                issueMasterVM.IssueDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.TotalVATAmount = 0;
                issueMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                issueMasterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                issueMasterVM.Comments = txtComments.Text.Trim();
                issueMasterVM.CreatedBy = Program.CurrentUser;
                issueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.LastModifiedBy = Program.CurrentUser;
                issueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.ReceiveNo = NextID.ToString();
                issueMasterVM.transactionType = transactionType;
                issueMasterVM.Post = "N";
                issueMasterVM.ReturnId = txtIssueNoP.Text.Trim();
                issueMasterVM.BranchId = Program.BranchId;

                #endregion

                #region Detail Data

                issueDetailVMs = new List<IssueDetailVM>();

                for (int i = 0; i < dgvIssue.RowCount; i++)
                {


                    IssueDetailVM detail = new IssueDetailVM();

                    detail.BOMId = Convert.ToInt32(dgvIssue.Rows[i].Cells["BOMId"].Value);
                    detail.IssueNoD = NextID.ToString();
                    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = 0;
                    detail.VATAmount = 0;
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.CommentsD = "FromIssue";

                    detail.ReceiveNoD = NextID.ToString();
                    detail.IssueDateTimeD = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.SD = 0;
                    detail.SDAmount = 0;
                    detail.Wastage = 0;
                    detail.BOMDate = "1900-01-01";
                    if (dgvIssue.Rows[i].Cells["FinishItemNo"].Value != null)
                    {
                        detail.FinishItemNo = dgvIssue.Rows[i].Cells["FinishItemNo"].Value.ToString();
                    }
                    else
                    {
                        detail.FinishItemNo = "0";
                    }


                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());

                    detail.BranchId = Program.BranchId;

                    issueDetailVMs.Add(detail);

                }

                #endregion

                #endregion

                #region Check Point

                if (issueDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Element Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Update Background Worker

                bgwUpdate.RunWorkerAsync();

                #endregion
            }

            #endregion

            #region catch
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
            #region try

            try
            {
                #region Statement

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = issueDal.IssueUpdate(issueMasterVM, issueDetailVMs, connVM,Program.CurrentUserID);
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
                            txtIssueNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvIssue.RowCount; i++)
                            {
                                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                            }
                        }
                    }
                ChangeData = false;
                #endregion

            }

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

        private void SaveUpdate()
        {

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Initial Setup

                TransactionTypes();

                #endregion

                #region Checkpoint and Validations

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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
                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Set Master

                issueMasterVM = new IssueMasterVM();

                issueMasterVM.IssueNo = NextID.ToString();
                issueMasterVM.IssueDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.TotalVATAmount = 0;
                issueMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                issueMasterVM.SerialNo = txtSerialNo.Text.Trim();
                issueMasterVM.Comments = txtComments.Text.Trim();
                issueMasterVM.CreatedBy = Program.CurrentUser;
                issueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.LastModifiedBy = Program.CurrentUser;
                issueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                issueMasterVM.ReceiveNo = NextID.ToString();

                issueMasterVM.transactionType = transactionType;
                issueMasterVM.Post = "Y";
                issueMasterVM.ReturnId = txtIssueNoP.Text.Trim();

                #endregion

                #region Set Details

                issueDetailVMs = new List<IssueDetailVM>();

                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    IssueDetailVM detail = new IssueDetailVM();

                    detail.IssueNoD = NextID.ToString();
                    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.VATRate = 0;
                    detail.VATAmount = 0;
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.CommentsD = "FromIssue";

                    detail.ReceiveNoD = NextID.ToString();
                    detail.IssueDateTimeD = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.SD = 0;
                    detail.SDAmount = 0;
                    detail.Wastage = 0;
                    detail.BOMDate = "1900-01-01";
                    detail.FinishItemNo = "0";

                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());

                    issueDetailVMs.Add(detail);
                }


                if (issueDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #region Post Background Worker

                backgroundWorkerPost.RunWorkerAsync();

                #endregion Post Background Worker

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

        private void backgroundWorkerPost_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);
                sqlResults = issueDal.IssuePost(issueMasterVM, issueDetailVMs, null, null, connVM, Program.CurrentUser);
                POST_DOWORK_SUCCESS = true;

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
                            txtIssueNo.Text = sqlResults[2].ToString();
                            txtHiddenInvoiceNo.Text = sqlResults[2].ToString();

                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                            for (int i = 0; i < dgvIssue.RowCount; i++)
                            {
                                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
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

            #endregion finally

        }

        private void btnSearchIssueNo_Click(object sender, EventArgs e)
        {

            #region try
            try
            {
                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region Transaction Type

                TransactionTypes();

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormIssueSearch.SelectOne(transactionType);

                #endregion Transaction Type

                #region selected Row

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtIssueNo.Text = selectedRow.Cells["IssueNo"].Value.ToString();
                    txtHiddenInvoiceNo.Text = selectedRow.Cells["IssueNo"].Value.ToString();
                    SearchInvoice();
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
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", exMessage);
            }
            #endregion
        }

        private void SearchInvoice()
        {
            IssueDAL IsDal = new IssueDAL();
            //IIssue IsDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

            try
            {
                DataTable dataTable = new DataTable("SearchIssueHeader");

                #region Data Call

                string[] cValues = { txtIssueNo.Text };
                string[] cFields = { "IssueNo like" };

                dataTable = IsDal.SelectAll(0, cFields, cValues, null, null, null, true, connVM);

                DataRow dr = dataTable.Rows[0];

                #endregion

                #region Value Assign to Form Elements

                txtId.Text = dr["Id"].ToString();
                txtFiscalYear.Text = dr["FiscalYear"].ToString();
                dtpIssueDate.Value = Convert.ToDateTime(dr["IssueDateTime"].ToString());
                txtTotalVATAmount.Text = Program.ParseDecimalObject(dr["TotalVATAmount"].ToString()).ToString();
                txtTotalAmount.Text = Program.ParseDecimalObject(dr["TotalAmount"].ToString()).ToString();
                txtSerialNo.Text = dr["SerialNo"].ToString();
                txtComments.Text = dr["Comments"].ToString();
                SearchBranchId = Convert.ToInt32(dr["BranchId"].ToString());
                IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                IssueDetailData = txtIssueNo.Text == "" ? "" : txtIssueNo.Text.Trim();

                #endregion

                #region Button Stats

                this.btnFirst.Enabled = false;
                this.btnPrevious.Enabled = false;
                this.btnNext.Enabled = false;
                this.btnLast.Enabled = false;
                this.btnSearchIssueNo.Enabled = false;

                #endregion

                #region Background Worker

                backgroundWorkerSearchIssueNo.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "SearchInvoice", exMessage);
            }
            #endregion

        }

        #region backgroundWorkerSearchIssueNo Event

        private void backgroundWorkerSearchIssueNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                IssueDetailResult = new DataTable();

                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                IssueDetailResult = issueDal.SearchIssueDetailDTNew((txtIssueNo.Text.Trim()), Program.DatabaseName, connVM); // Change 04

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerSearchIssueNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                #region Statement

                //string decriptedPurchaseDetailData = Converter.DESDecrypt(PassPhrase, EnKey, IssueDetailResult);
                //string[] IssueDetailLines = decriptedPurchaseDetailData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


                dgvIssue.Rows.Clear();
                int j = 0;
                foreach (DataRow item in IssueDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvIssue.Rows.Add(NewRow);
                    dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();// IssueDetailFields[1].ToString();
                    dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();// IssueDetailFields[2].ToString();

                    dgvIssue.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["CostPrice"].ToString());//Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["NBRPrice"].Value = Program.ParseDecimalObject(item["NBRPrice"].ToString());//Convert.ToDecimal(IssueDetailFields[5].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();// IssueDetailFields[6].ToString();

                    dgvIssue.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());//Convert.ToDecimal(IssueDetailFields[7].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());//Convert.ToDecimal(IssueDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());//Convert.ToDecimal(IssueDetailFields[9].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// IssueDetailFields[10].ToString();
                    dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();// IssueDetailFields[11].ToString();
                    dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();// IssueDetailFields[15].ToString();
                    dgvIssue.Rows[j].Cells["Status"].Value = "Old";

                    dgvIssue.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvIssue.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item["Stock"].ToString());//Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());//Convert.ToDecimal(IssueDetailFields[13].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());//Convert.ToDecimal(IssueDetailFields[14].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["Change"].Value = 0;
                    dgvIssue.Rows[j].Cells["FinishItemNo"].Value = item["FinishItemNo"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductCode"].Value = item["FinishProductCode"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductName"].Value = item["FinishProductName"].ToString();

                    dgvIssue.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item["UOMc"].ToString());
                    dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvIssue.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                    dgvIssue.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());

                    dgvIssue.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvIssue.Rows[j].Cells["VATName"].Value = item["VATName"].ToString();
                    dgvIssue.Rows[j].Cells["UOMWastage"].Value = item["UOMWastage"].ToString();



                    j = j + 1;
                }  //End For

                Rowcalculate();

                btnSave.Text = "&Save";
                IsUpdate = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                #region Button Stats

                ChangeData = false;

                this.btnFirst.Enabled = true;
                this.btnPrevious.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnLast.Enabled = true;
                this.btnSearchIssueNo.Enabled = true;

                this.progressBar1.Visible = false;

                #endregion

            }


        }

        #endregion

        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvIssue.Rows.Count > 0)
                {
                    dgvIssue.Rows[dgvIssue.Rows.Count - 1].Selected = true;
                    dgvIssue.CurrentCell = dgvIssue.Rows[dgvIssue.Rows.Count - 1].Cells[1];
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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            #endregion
        }

        private void AllClear()
        {
            txtProductCode.Text = "";
            txtProductName.Text = "";
            //txtCategoryName.Text = "";
            txtHSCode.Text = "";
            txtUnitCost.Text = "0.00";
            txtQuantity.Text = "";
            txtVATRate.Text = "0.00";
            txtUOM.Text = "";
            txtCommentsDetail.Text = "NA";
            txtQuantityInHand.Text = "0.00";
            cmbProductName.Text = "";
            cmbUom.Text = "";
            txtBOMId.Text = "";


        }

        private void FindBomId()
        {
            try
            {

                ProductDAL productDal = new ProductDAL();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    string BOMId = dgvIssue["BOMId", i].Value.ToString();
                    string ItemNo = dgvIssue["ItemNo", i].Value.ToString();
                    //////string VATName = dgvIssue["VATName", i].Value.ToString();
                    if (BOMId == "0")
                    {
                        ProductDAL ProductDal = new ProductDAL();
                        //IProduct ProductDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                        var dtbom = ProductDal.SelectBOMRaw(ItemNo, dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);
                        //var dtbom = new ProductDAL().SelectBOMRaw(ItemNo, dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null);

                        if (dtbom != null && dtbom.Rows.Count > 0)
                        {
                            BOMId = dtbom.Rows[0]["BOMId"].ToString();
                        }
                        dgvIssue["BOMId", i].Value = string.IsNullOrEmpty(BOMId) ? "0" : BOMId;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void Rowcalculate()
        {
            #region try
            try
            {

                decimal SumSubTotal = 0;
                decimal SumQuantity = 0;
                decimal Quantity = 0;
                decimal Cost = 0;
                decimal SubTotal = 0;

                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    dgvIssue[0, i].Value = i + 1;


                    Quantity = Convert.ToDecimal(dgvIssue["Quantity", i].Value);
                    Cost = Convert.ToDecimal(dgvIssue["UnitPrice", i].Value);

                    SubTotal = Cost * Quantity;
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), IssuePlaceQty));

                    dgvIssue["SubTotal", i].Value = Program.ParseDecimalObject(SubTotal).ToString();

                    dgvIssue["VATAmount", i].Value = 0;
                    dgvIssue["SDAmount", i].Value = 0;
                    //dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubAmount).ToString();//"0,0.00");

                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvIssue["SubTotal", i].Value);
                    SumQuantity = SumQuantity + Quantity;

                    dgvIssue["Change", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(dgvIssue["Quantity", i].Value)
                        - Convert.ToDecimal(dgvIssue["Previous", i].Value)).ToString();//"0,0.0000");


                }
                txtTotalVATAmount.Text = "0";
                txtTotalAmount.Text = Program.ParseDecimalObject(SumSubTotal).ToString();//"0,0.00");
                txtTotalQuantity.Text = Program.ParseDecimalObject(SumQuantity).ToString();//"0,0.00");

                FindBomId();
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 04

        private void ProductSearchDsLoad()
        {
            //No SOAP Service

            #region try
            try
            {
                cmbProductName.Items.Clear();




                var prodByName = from prd in ProductsMini.ToList()
                                 orderby prd.ProductName
                                 select prd.ProductName;


                if (prodByName != null && prodByName.Any())
                {
                    cmbProductName.Items.AddRange(prodByName.ToArray());
                }

                cmbProductName.Items.Insert(0, "Select");
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();
                //frmRptIssueInformation.txtIssueNo.Text = txtIssueNo.Text.Trim();
                frmRptIssueInformation.Show();
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

        private void btnSearchProductName_Click(object sender, EventArgs e)
        {
            string itemNo = "72";
            decimal costPrice = 550;

            var dd = ProductsMini.ToList();

            var tt = ProductsMini.SingleOrDefault(x => x.ItemNo == itemNo);

            tt.CostPrice = costPrice;

            var aa = tt;

        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

            //ChangeData = true;
            //txtProductName.Text = "";
            ////cmbProduct.Text = "";
            //txtCategoryName.Text = "";
            //txtUOM.Text = "";
            //txtUnitCost.Text = "0.00";
            //txtNBRPrice.Text = "0.00";
            //txtHSCode.Text = "";
            //txtVATRate.Text = "0.00";
            //txtQuantityInHand.Text = "";

            //for (int j = 0; j < Convert.ToInt32(ProductResultDs.Tables[0].Rows.Count); j++) //for (int j = 0; j < Convert.ToInt32(ProductLines.Length); j++)
            //{
            //    //string[] ProductFields = ProductLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    try
            //    {
            //        if (ProductResultDs.Tables[0].Rows[j]["ItemNo"].ToString() == txtProductCode.Text.Trim())
            //        {
            //            //txtQuantityInHand.Text = ProductFields[17].ToString();

            //            txtProductName.Text = ProductResultDs.Tables[0].Rows[j]["ProductName"].ToString(); //ProductFields[1].ToString();
            //            //cmbProduct.Text = ProductResultDs.Tables[0].Rows[j]["ProductName"].ToString();// ProductFields[1].ToString();
            //            txtUnitCost.Text = ProductResultDs.Tables[0].Rows[j]["ReceivePrice"].ToString(); //Convert.ToDecimal(ProductFields[25].ToString()).ToString();//"0.00");
            //            txtCategoryName.Text = ProductResultDs.Tables[0].Rows[j]["CategoryName"].ToString(); //ProductFields[4].ToString();
            //            txtUOM.Text = ProductResultDs.Tables[0].Rows[j]["UOM"].ToString(); //ProductFields[5].ToString();
            //            txtHSCode.Text = ProductResultDs.Tables[0].Rows[j]["HSCodeNo"].ToString(); //ProductFields[11].ToString();
            //            //txtVATRate.Text = "0.00";// ProductFields[12].ToString();
            //            txtQuantityInHand.Text = ProductResultDs.Tables[0].Rows[j]["IssuePrice"].ToString(); //ProductFields[17].ToString();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
        }

        private void txtIssueNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }


            if (e.KeyCode.Equals(Keys.Enter))
            {
                IssueNavigation("Current");
            }

        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpIssueDate_KeyDown(object sender, KeyEventArgs e)
        {
            //cmbProduct.Focus();
        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
            //#region try
            //try
            //{
            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());
            //        cmbProductType.Text = TypeInfo[1];
            //    }
            //    //ProductSearchDs();
            //    //ProductSearch();
            //}
            //#endregion

            //#region catch
            //catch (ArgumentNullException ex)
            //{
            //    string err = ex.Message.ToString();
            //    string[] error = err.Split(FieldDelimeter.ToCharArray());
            //    FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
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
            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            //}
            //#endregion
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
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

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
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

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpIssueDate_ValueChanged(object sender, EventArgs e)
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

        private void FormIssue_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtSD_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtSD, "SD Rate");
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                btnSave.Text = "&Add";
                txtIssueNo.Text = "~~~ New ~~~";

                FormMaker();

                FormLoad();

                ClearAllFields();

                ChangeData = false;



                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                #region Settings


                string vIssuePlaceQty, vIssuePlaceAmt, vIssueFromBOM, vIssueAutoPost = string.Empty;


                CommonDAL commonDal = new CommonDAL();

                vIssuePlaceQty = commonDal.settingsDesktop("Issue", "Quantity",null,connVM);
                vIssuePlaceAmt = commonDal.settingsDesktop("Issue", "Amount",null,connVM);
                vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM",null,connVM);
                vIssueAutoPost = commonDal.settingsDesktop("IssueFromBOM", "IssueAutoPost",null,connVM);
                if (string.IsNullOrEmpty(vIssuePlaceQty)
                    || string.IsNullOrEmpty(vIssuePlaceAmt)
                    || string.IsNullOrEmpty(vIssueFromBOM)
                    || string.IsNullOrEmpty(vIssueAutoPost))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                IssuePlaceQty = Convert.ToInt32(vIssuePlaceQty);
                IssuePlaceAmt = Convert.ToInt32(vIssuePlaceAmt);
                bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

                if (IssueFromBOM == true)
                {
                    bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                    if (IssueAutoPost == false)
                    {

                        btnSave.Enabled = false;
                    }

                }
                #endregion Settings

                IsPost = false;
                progressBar1.Visible = true;

                bgwLoad.RunWorkerAsync();
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
            finally
            {
                ChangeData = false;
            }
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                TransactionTypes();

                FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();

                //mdi.RollDetailsInfo(frmRptIssueInformation.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                if (txtIssueNo.Text == "~~~ New ~~~")
                {
                    frmRptIssueInformation.txtIssueNo.Text = "";
                }
                else
                {
                    frmRptIssueInformation.txtIssueNo.Text = txtIssueNo.Text.Trim();
                }

                frmRptIssueInformation.txtTransactionType.Text = transactionType;


                frmRptIssueInformation.ShowDialog();
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
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
            }
            #endregion
        }

        private void btnVAT16_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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
                if (dgvIssue.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.txtUOM.Text = dgvIssue.CurrentRow.Cells["UOMn"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpIssueDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpIssueDate.Value;

                }

                frmRptVAT16.ShowDialog();
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 05

        private void btnVAT18_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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
                frmRptVAT18.dtpToDate.Value = dtpIssueDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpIssueDate.Value;
                frmRptVAT18.ShowDialog();
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
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            #endregion
        }

        private void PirceCall()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            #region BOM Set

            DataTable dt = new DataTable();

            dt = productDal.SelectBOMRaw(txtProductCode.Text.Trim(), dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

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
            #endregion

            string strProductCode = txtProductCode.Text;

            if (!string.IsNullOrEmpty(strProductCode))
            {
                if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();

                }
                else
                {

                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];

                //this.button1.Enabled = false;
                //this.progressBar1.Visible = true;

                ProductSearchDsFormLoad();
                //ProductSearchDs(); //Replease
                //ProductSearch();
                //ProductSearchDsLoad(); //No SOAP Service
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion
        }


        //===========================================================

        #region backgroundWorkerSave Event


        #endregion

        //===========================================================

        

        private void backgroundWorkerSetupBOMIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Start DoWork
                //SetupSoapClient ShowStatus = new SetupSoapClient();
                //StatusResult = ShowStatus.IssueBOMStatus(Program.DatabaseName);
                //StatusResult = SetupDAL.ResultIssueBOM(Program.DatabaseName);

                //SetupDAL setupDal = new SetupDAL();
                //StatusResult = setupDal.ResultIssueBOMNew(Program.DatabaseName);

                //End DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSetupBOMIssue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                //Program.IssueFromBOM = StatusResult.Tables[0].Rows[0][0].ToString();
                //MessageBox.Show(StatusResult.Tables[0].Rows[0][0].ToString());   // Issue From BOM
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
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSetupBOMIssue_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.progressBar1.Visible = false;

        }

        private void backgroundWorkerProductSearchDsFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                ProductResultDs = new DataTable();
                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode, connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
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
                }//End For
                //End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.button1.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void backgroundWorkerPTypeSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Start DoWork
                //string encriptedTypeData = Converter.DESEncrypt(PassPhrase, EnKey, TypeData);
                //ProductTypeSoapClient ProductTypeSearch = new ProductTypeSoapClient();
                //ProductTypeResult = ProductTypeSearch.Search(encriptedTypeData.ToString(), Program.DatabaseName);
                //ProductTypeDAL productTypeDal = new ProductTypeDAL();
                //ProductTypeResult = productTypeDal.SearchProductTypeNew("", "", "Y");

                //End DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPTypeSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                //cmbProductType.Items.Clear();
                var prods = (from DataRow row in ProductTypeResult.Rows
                             select new ProductTypeDTO()
                             {
                                 TypeID = row["TypeID"].ToString(),
                                 ProductType = row["ProductType"].ToString(),
                                 Comments = row["Comments"].ToString(),
                                 Description = row["Description"].ToString(),
                                 ActiveStatus = row["ActiveStatus"].ToString()

                             }
                           ).ToList();
                if (prods != null && prods.Any())
                {
                    var prodtype = from prd in prods.ToList()
                                   select prd.ProductType;
                    cmbProductType.Items.AddRange(prodtype.ToArray());
                    cmbProductType.Text = "Raw";

                }
                ProductDAL productTypeDal = new ProductDAL();
                cmbProductType.DataSource = productTypeDal.ProductTypeList;


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
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPTypeSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 06

        //===========================================================

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
                        txtQuantity.Focus();
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
                //cmbUom.Text = uOMTo;
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

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void cmbProductName_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {

                var searchText = cmbProductName.Text.Trim().ToLower();

                ProductMiniDTO products = new ProductMiniDTO();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    #region Value Assign

                    var prodByName = from prd in ProductsMini.ToList()
                                     where prd.ProductName.ToLower() == searchText.ToLower()
                                     select prd;

                    if (prodByName != null && prodByName.Any())
                    {
                        products = prodByName.First();
                        txtProductName.Text = products.ProductName;
                        txtProductCode.Text = products.ItemNo;
                        //txtUnitCost.Text = products.IssuePrice.ToString();//"0,0.00");
                        txtUOM.Text = products.UOM;
                        cmbUom.Text = products.UOM;
                        txtHSCode.Text = products.HSCodeNo;
                        //txtQuantityInHand.Text = products.Stock.ToString();//"0,0.0000");
                        txtPCode.Text = products.ProductCode;
                    }

                    #endregion
                }
                string strProductCode = txtProductCode.Text;////this is itemNo
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    #region Stock and PriceCall

                    //ProductDAL productDal = new ProductDAL();
                    ProductDAL productDal = new ProductDAL();


                    if (string.Equals(transactionType, "ContractorIssueWoBOM", StringComparison.OrdinalIgnoreCase))
                    {
                        var dtTollStock = productDal.GetTollStock(new ParameterVM()
                        {
                            ItemNo = strProductCode,
                        });

                        txtQuantityInHand.Text = dtTollStock.Rows[0][0].ToString();

                    }
                    else
                    {
                        DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpIssueDate.Value.ToString("yyyy-MMM-dd")
                            + DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true, connVM, Program.CurrentUserID);

                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        if (quan > 0)
                        {
                            txtUnitCost.Text = (amount / quan).ToString();
                        }
                        else
                        {
                            txtUnitCost.Text = "0";
                        }

                        txtQuantityInHand.Text = quan.ToString();
                    }


                   

                    PirceCall();

                    txtQuantity.Focus();

                    Uoms();

                    #endregion
                }


                #region TransactionType IssueWastage

                if (transactionType == "IssueWastage")
                {
                    txtUnitCost.Text = "0";
                    txtQuantityInHand.Text = "0";

                    DataSet ds = new DataSet();

                    IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                    ds = ReportDSDal.Wastage(products.ItemNo, "", "", "Y", "Y", "1900-Jan-01", "2100 - Dec - 31", Program.BranchId, null);
                    //ds = new ReportDSDAL().Wastage(products.ItemNo, null);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string WastageBalance = "0";
                            WastageBalance = ds.Tables[0].Rows[0]["WastageBalance"].ToString();

                            txtQuantityInHand.Text = WastageBalance;
                        }

                    }


                }

                #endregion
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
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                    cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();
                    txtProductCode.SelectedText = selectedRow.Cells["ItemNo"].Value.ToString();

                }
                cmbProductName.Focus();
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void btnImport_Click(object sender, EventArgs e)
        {

            try
            {
                TransactionTypes();


                string value = new CommonDAL().settingValue("CompanyCode", "Code");

                if (OrdinaryVATDesktop.IsACICompany(value))
                {
                    //FormPIssueImportACI aci = new FormPIssueImportACI();
                    //aci.preSelectTable = "Receives";
                    //aci.transactionType = transactionType;
                    //aci.Show();
                    string ImportId = FormPIssueImportACI.SelectOne(transactionType, false);
                    //string ImportId = "S-3/RM/142/2020";
                    IssueDAL _ilDal = new IssueDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _ilDal.SelectAll(0, new[] { "ImportIDExcel" },
                                new[] { ImportId });
                        IssueDataLoad(dt);
                    }
                }
                else if (OrdinaryVATDesktop.IsNourishCompany(value))
                {
                    string ImportId = FormIssueImportNourish.SelectOne(transactionType);
                    IssueDAL _ilDal = new IssueDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _ilDal.SelectAll(0, new[] { "ImportIDExcel" },
                                new[] { ImportId });
                        IssueDataLoad(dt);
                    }
                }
                else if (value == "SQR")
                {
                    string[] ids = FormIssueImportSQR.SelectOne(transactionType).Split('~');
                    string ImportId = ids[0];
                    //string ImportId = "ISU-001/0001/0320";

                    //IsCustomerExempted = ids[1];
                    IssueDAL issueDal = new IssueDAL();

                    //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        //DataTable dt = _slDal.SearchSalesHeaderDTNew("", ImportId);
                        //SaleDataLoad(dt);

                        DataTable dt = issueDal.SelectAllHeaderTemp(0, new[] { "Ish.ID", "Ish.UserId" },
                            new[] { ImportId, Program.CurrentUserID });
                        SaleDataLoadBeforeSave(dt);
                    }
                }
                else if (value == "EON" || value.ToLower() == "purofood" || value.ToLower() == "eahpl" || value.ToLower() == "eail" || value.ToLower() == "eeufl" || value.ToLower() == "exfl")
                {
                    string ImportId = FormPIssueImportEON.SelectOne(transactionType);
                    IssueDAL _slDal = new IssueDAL();

                    if (!string.IsNullOrEmpty(ImportId))
                    {
                        DataTable dt = _slDal.SelectAllHeaderTemp(0, new[] { "Ish.ID", "Ish.UserId" },
                            new[] { ImportId, Program.CurrentUserID });
                        IssueDataLoad(dt);
                    }
                }
                else
                {
                    FormMasterImport fmi = new FormMasterImport();
                    fmi.preSelectTable = "Issues";
                    fmi.transactionType = transactionType;
                    fmi.Show(this);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                FileLogger.Log(this.Name, "IssueImport", exception.Message + "\n" + exception.StackTrace);
            }



            #region new process for bom import

            //string[] extention = fileName.Split(".".ToCharArray());
            ////string[] extention = fileNameM.Split(".".ToCharArray());
            //string[] retResults = new string[4];
            //if (extention[extention.Length - 1] == "txt")
            //{
            //    retResults = ImportFromText();
            //}
            //else
            //{
            //    retResults = ImportFromExcel();
            //}
            ////string[] retResults = Import();
            //string result = retResults[0];
            //string message = retResults[1];
            //if (string.IsNullOrEmpty(result))
            //{
            //    throw new ArgumentNullException("BomImport", "Unexpected error.");
            //}
            //else if (result == "Success" || result == "Fail")
            //{
            //    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

            #endregion new process for bom import
        }

        private void IssueDataLoad(DataTable dataTable)
        {


            #region CheckPoint

            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            #endregion

            DataRow dr = dataTable.Rows[0];

            txtIssueNo.Text = dr["IssueNo"].ToString();

            SearchInvoice();

        }

        private void SaleDataLoadBeforeSave(DataTable dataTable)
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

            txtIssueNo.Text = "~~~ New ~~~";
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
            //cmbCurrency.Text = "BDT";

            #region Detail Search / Background Worker

            bgwDetailsSearch.RunWorkerAsync();

            #endregion


            dtpIssueDate.Value = Convert.ToDateTime(dataTable.Rows[0]["IssueDateTime"].ToString());
            txtTotalVATAmount.Text = "0";//Program.ParseDecimalObject(dataTable.Rows[0]["TotalVATAmount"].ToString()).ToString();//"0,0.00");
            txtTotalAmount.Text = "0";//Program.ParseDecimalObject(dataTable.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
            txtSerialNo.Text = dataTable.Rows[0]["SerialNo"].ToString();
            txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
            SearchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"].ToString());

            IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;

            this.btnSearchIssueNo.Enabled = false;
            this.progressBar1.Visible = true;
            this.Enabled = true;
            //btnPrint.Enabled = true;

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
                fdlg.Multiselect = true;
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
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

        private string[] ImportFromText()
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

            DataTable dtIssueM = new DataTable();
            DataTable dtIssueD = new DataTable();

            #region Master table
            dtIssueM.Columns.Add("Identifier");
            dtIssueM.Columns.Add("ID");
            dtIssueM.Columns.Add("Issue_DateTime");
            dtIssueM.Columns.Add("Reference_No");
            dtIssueM.Columns.Add("Comments");
            dtIssueM.Columns.Add("Return_Id");
            dtIssueM.Columns.Add("Post");
            dtIssueM.Columns.Add("Transection_Type").DefaultValue = transactionType;
            dtIssueM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
            dtIssueM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

            #endregion Master table

            #region Details table
            dtIssueD.Columns.Add("Identifier");
            dtIssueD.Columns.Add("ID");
            dtIssueD.Columns.Add("Item_Code");
            dtIssueD.Columns.Add("Item_Name");
            dtIssueD.Columns.Add("Quantity");
            dtIssueD.Columns.Add("UOM");

            #endregion Details table



            //string fileNameM = fileNames[fileNames.Length - 1].ToString();
            //string fileNameD = fileNames[fileNames.Length - 2].ToString();

            //StreamReader sr = new StreamReader(fileName);
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
                                dtIssueM.Rows.Add(mItems);
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
                                dtIssueD.Rows.Add(dItems);
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
                //        dtIssueM.Rows.Add(items);
                //    }
                //    else if (items[0].Replace("\n", "").ToUpper() == "D")
                //    {
                //        dtIssueD.Rows.Add(items);
                //    }
                //}
                //if (sr != null)
                //{
                //    sr.Close();
                //}
                #endregion Load Text file

                SAVE_DOWORK_SUCCESS = false;
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);
                sqlResults = issueDal.ImportData(dtIssueM, dtIssueD, 0, null, null, null, connVM);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally

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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
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

        private void bgwP_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                IssueDetailResult = new DataTable();

                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                IssueDetailResult = issueDal.SearchIssueDetailDTNew(IssueDetailData, Program.DatabaseName, connVM); // Change 04

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                #region Statement

                //string decriptedPurchaseDetailData = Converter.DESDecrypt(PassPhrase, EnKey, IssueDetailResult);
                //string[] IssueDetailLines = decriptedPurchaseDetailData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


                dgvIssue.Rows.Clear();
                int j = 0;
                foreach (DataRow item in IssueDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvIssue.Rows.Add(NewRow);
                    dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();// IssueDetailFields[1].ToString();
                    dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();// IssueDetailFields[2].ToString();
                    dgvIssue.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();//Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();//Convert.ToDecimal(IssueDetailFields[5].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();// IssueDetailFields[6].ToString();
                    dgvIssue.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();//Convert.ToDecimal(IssueDetailFields[7].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();//Convert.ToDecimal(IssueDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();//Convert.ToDecimal(IssueDetailFields[9].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// IssueDetailFields[10].ToString();
                    dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();// IssueDetailFields[11].ToString();
                    dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();// IssueDetailFields[15].ToString();
                    dgvIssue.Rows[j].Cells["Status"].Value = "Old";
                    dgvIssue.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvIssue.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();//Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["SD"].Value = item["SD"].ToString();//Convert.ToDecimal(IssueDetailFields[13].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();//Convert.ToDecimal(IssueDetailFields[14].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["Change"].Value = 0;
                    dgvIssue.Rows[j].Cells["FinishItemNo"].Value = item["FinishItemNo"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductCode"].Value = item["FinishProductCode"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductName"].Value = item["FinishProductName"].ToString();

                    dgvIssue.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvIssue.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                    dgvIssue.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();

                    dgvIssue.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvIssue.Rows[j].Cells["UOMWastage"].Value = item["UOMWastage"].ToString();



                    j = j + 1;
                }  //End For
                btnSave.Text = "&Save";
                IsUpdate = true;
                ChangeData = false;

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSearchIssueNo.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }

        private void btnSearchIssueNoP_Click(object sender, EventArgs e)
        {
            try
            {

                dgvIssueReturn.Top = dgvIssue.Top;
                dgvIssueReturn.Left = dgvIssue.Left;
                dgvIssueReturn.Height = dgvIssue.Height;
                dgvIssueReturn.Width = dgvIssue.Width;

                dgvIssue.Visible = false;
                dgvIssueReturn.Visible = true;

                // Start Click
                Program.fromOpen = "Me";
                DataGridViewRow selectedRow = null;
                TransactionTypes();

                selectedRow = FormIssueSearch.SelectOne("All");



                if (selectedRow != null && selectedRow.Selected == true)
                {

                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }


                    txtIssueNoP.Text = selectedRow.Cells["IssueNo"].Value.ToString(); // Issueinfo[0];
                    //string IssueDetailData;
                    if (txtIssueNoP.Text == "")
                    {
                        IssueDetailData = "0";
                    }
                    else
                    {
                        IssueDetailData = txtIssueNoP.Text.Trim();
                    }
                    // End Click



                    this.btnSearchIssueNo.Enabled = false;
                    this.progressBar1.Visible = true;


                    bgwReturnIssue.RunWorkerAsync();
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

        #endregion

        #region Methods 07

        private void bgwReturnIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                IssueDetailResult = new DataTable();
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                IssueDetailResult = issueDal.SearchIssueDetailDTNew(IssueDetailData, Program.DatabaseName, connVM);


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
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork",
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

                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork",
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
                FileLogger.Log(this.Name, "bgwReturnIssue_DoWork", exMessage);
            }

            #endregion
        }

        private void bgwReturnIssue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                // Start Complete
                dgvIssueReturn.Rows.Clear();
                int j = 0;
                foreach (DataRow item in IssueDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvIssueReturn.Rows.Add(NewRow);
                    dgvIssueReturn.Rows[j].Cells["chkSelectR"].Selected = true;


                    dgvIssueReturn.Rows[j].Cells["LineNoR"].Value = item["IssueLineNo"].ToString();
                    dgvIssueReturn.Rows[j].Cells["PCodeR"].Value = item["ProductCode"].ToString();
                    dgvIssueReturn.Rows[j].Cells["ItemNoR"].Value = item["ItemNo"].ToString();
                    dgvIssueReturn.Rows[j].Cells["ItemNameR"].Value = item["ProductName"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMR"].Value = item["UOM"].ToString();
                    dgvIssueReturn.Rows[j].Cells["QuantityR"].Value = item["Quantity"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UnitPriceR"].Value = item["CostPrice"].ToString();
                    dgvIssueReturn.Rows[j].Cells["VATRateR"].Value = item["VATRate"].ToString();
                    dgvIssueReturn.Rows[j].Cells["VATAmountR"].Value = item["VATAmount"].ToString();
                    dgvIssueReturn.Rows[j].Cells["SubTotalR"].Value = item["SubTotal"].ToString();
                    dgvIssueReturn.Rows[j].Cells["CommentsR"].Value = item["Comments"].ToString();
                    dgvIssueReturn.Rows[j].Cells["NBRPriceR"].Value = item["NBRPrice"].ToString();
                    dgvIssueReturn.Rows[j].Cells["StatusR"].Value = "Old";
                    dgvIssueReturn.Rows[j].Cells["SDR"].Value = item["SD"].ToString();
                    dgvIssueReturn.Rows[j].Cells["SDAmountR"].Value = item["SDAmount"].ToString();
                    //dgvIssueReturn.Rows[j].Cells["PreviousR"].Value = item["Quantity"].ToString();
                    dgvIssueReturn.Rows[j].Cells["ChangeR"].Value = 0;
                    dgvIssueReturn.Rows[j].Cells["StockR"].Value = item["Stock"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMPriceR"].Value = item["UOMPrice"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMQtyR"].Value = item["UOMQty"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMnR"].Value = item["UOMn"].ToString();
                    dgvIssueReturn.Rows[j].Cells["FinishItemNoR"].Value = item["FinishItemNo"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMcR"].Value = item["UOMc"].ToString();
                    dgvIssueReturn.Rows[j].Cells["UOMWastageR"].Value = item["UOMWastage"].ToString();
                    dgvIssueReturn.Rows[j].Cells["BOMIdR"].Value = item["BOMId"].ToString();
                    dgvIssueReturn.Rows[j].Cells["FinishProductCodeR"].Value = item["FinishProductCode"].ToString();
                    dgvIssueReturn.Rows[j].Cells["FinishProductNameR"].Value = item["FinishProductName"].ToString();
                    decimal returnQty = issueDal.ReturnIssueQty(IssueDetailData, item["ItemNo"].ToString());
                    dgvIssueReturn.Rows[j].Cells["RestQtyR"].Value = (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();
                    dgvIssueReturn.Rows[j].Cells["PreviousR"].Value = (Convert.ToDecimal(item["Quantity"]) - returnQty).ToString();

                    j = j + 1;

                }
                IsUpdate = true;

                ChangeData = false;

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
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "bgwReturnIssue_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.btnSearchIssueNo.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void dgvIssueReturn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F2))
            {
                dgvIssue.Rows.Clear();
                int j = 0;
                for (int i = 0; i < dgvIssueReturn.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvIssueReturn["chkSelectR", i].Value) == true)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvIssue.Rows.Add(NewRow);

                        dgvIssue.Rows[j].Cells["LineNo"].Value = j + 1;
                        dgvIssue.Rows[j].Cells["PCode"].Value = dgvIssueReturn["PCodeR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["ItemNo"].Value = dgvIssueReturn["ItemNoR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["ItemName"].Value = dgvIssueReturn["ItemNameR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOM"].Value = dgvIssueReturn["UOMR", i].Value.ToString();
                        //dgvIssue.Rows[j].Cells["Quantity"].Value = dgvIssueReturn["QuantityR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["Quantity"].Value = dgvIssueReturn["RestQtyR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UnitPrice"].Value = dgvIssueReturn["UnitPriceR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["VATRate"].Value = dgvIssueReturn["VATRateR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["VATAmount"].Value = dgvIssueReturn["VATAmountR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["SubTotal"].Value = dgvIssueReturn["SubTotalR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["Comments"].Value = dgvIssueReturn["CommentsR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["NBRPrice"].Value = dgvIssueReturn["NBRPriceR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["Status"].Value = "Old";
                        dgvIssue.Rows[j].Cells["SD"].Value = dgvIssueReturn["SDR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["SDAmount"].Value = dgvIssueReturn["SDAmountR", i].Value.ToString();
                        //dgvIssue.Rows[j].Cells["Previous"].Value = dgvIssueReturn["QuantityR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["Previous"].Value = dgvIssueReturn["PreviousR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["Change"].Value = 0;
                        dgvIssue.Rows[j].Cells["Stock"].Value = dgvIssueReturn["StockR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOMPrice"].Value = dgvIssueReturn["UOMPriceR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOMQty"].Value = dgvIssueReturn["UOMQtyR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOMn"].Value = dgvIssueReturn["UOMnR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["FinishItemNo"].Value = dgvIssueReturn["FinishItemNoR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOMc"].Value = dgvIssueReturn["UOMcR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["UOMWastage"].Value = dgvIssueReturn["UOMWastageR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["BOMId"].Value = dgvIssueReturn["BOMIdR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["FinishProductCode"].Value = dgvIssueReturn["FinishProductCodeR", i].Value.ToString();
                        dgvIssue.Rows[j].Cells["FinishProductName"].Value = dgvIssueReturn["FinishProductNameR", i].Value.ToString();

                        j = j + 1;

                    }



                }
                dgvIssueReturn.Visible = false;
                dgvIssue.Visible = true;
                Rowcalculate();
            }


        }

        private void dgvIssueReturn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvIssueReturn["chkSelectR", e.RowIndex].Value = !Convert.ToBoolean(dgvIssueReturn["chkSelectR", e.RowIndex].Value);
            }
        }

        private void dgvIssueReturn_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
            txtQuantityInHand.Text = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtUnitCost, "UnitCost");
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalAmount, "TotalAmount");
            txtTotalAmount.Text = Program.ParseDecimalObject(txtTotalAmount.Text.Trim()).ToString();
        }

        private void dtpIssueDate_Leave(object sender, EventArgs e)
        {
            dtpIssueDate.Value = Program.ParseDate(dtpIssueDate);
        }


        private void txtNBRPrice_Leave(object sender, EventArgs e)
        {
            txtNBRPrice.Text = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();
        }

        private void bgwDetailsSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            IssueDAL issueDal = new IssueDAL();
            //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

            IssueDetailsResult = issueDal.SearchIssueDetailTemp(ImportId, Program.CurrentUserID, connVM);
        }

        private void bgwDetailsSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                FileLogger.Log(this.Name, "Details Load Time before Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));

                dgvIssue.Rows.Clear();
                int j = 0;

                FileLogger.Log(this.Name, "Foreach", (IssueDetailsResult == null).ToString());
                CommonDAL cmnDal = new CommonDAL();
                ProductDAL productDal = new ProductDAL();

                foreach (DataRow item in IssueDetailsResult.Rows)
                {

                    FileLogger.Log(this.Name, "Foreach", IssueDetailsResult.Rows.Count.ToString());

                    DataGridViewRow NewRow = new DataGridViewRow();

                    DataTable priceData = productDal.AvgPriceNew(item["ItemNo"].ToString(),
                        Convert.ToDateTime(item["IssueDateTime"]).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null,
                        null, true, true, true, false, null,Program.CurrentUserID);
                    decimal uomc = Convert.ToDecimal(item["UOMc"].ToString());
                    decimal quantity = Convert.ToDecimal(item["Quantity"].ToString());

                    decimal avgPrice = 0;
                    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                    if (quan > 0)
                    {
                        avgPrice = cmnDal.FormatingDecimal((amount / quan).ToString());
                    }
                    else
                    {
                        avgPrice = 0;
                    }

                    dgvIssue.Rows.Add(NewRow);
                    dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();// IssueDetailFields[1].ToString();
                    dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();// IssueDetailFields[2].ToString();

                    dgvIssue.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["UnitPrice"].Value = uomc * avgPrice; //Program.ParseDecimalObject(item["CostPrice"].ToString());//Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["NBRPrice"].Value = 0;//Program.ParseDecimalObject(item["NBRPrice"].ToString());//Convert.ToDecimal(IssueDetailFields[5].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();// IssueDetailFields[6].ToString();

                    dgvIssue.Rows[j].Cells["VATRate"].Value = 0; //Program.ParseDecimalObject(item["VATRate"].ToString());//Convert.ToDecimal(IssueDetailFields[7].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["VATAmount"].Value = 0; //Program.ParseDecimalObject(item["VATAmount"].ToString());//Convert.ToDecimal(IssueDetailFields[8].ToString()).ToString();//"0,0.00");
                    dgvIssue.Rows[j].Cells["SubTotal"].Value = 0;//Program.ParseDecimalObject(item["SubTotal"].ToString());//Convert.ToDecimal(IssueDetailFields[9].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// IssueDetailFields[10].ToString();
                    dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();// IssueDetailFields[11].ToString();
                    dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();// IssueDetailFields[15].ToString();
                    dgvIssue.Rows[j].Cells["Status"].Value = "Old";

                    dgvIssue.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvIssue.Rows[j].Cells["Stock"].Value = 0;//Program.ParseDecimalObject(item["Stock"].ToString());//Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["SD"].Value = 0;//Program.ParseDecimalObject(item["SD"].ToString());//Convert.ToDecimal(IssueDetailFields[13].ToString()).ToString();//"0.00");
                    dgvIssue.Rows[j].Cells["SDAmount"].Value = 0; //Program.ParseDecimalObject(item["SDAmount"].ToString());//Convert.ToDecimal(IssueDetailFields[14].ToString()).ToString();//"0,0.00");

                    dgvIssue.Rows[j].Cells["Change"].Value = 0;
                    dgvIssue.Rows[j].Cells["FinishItemNo"].Value = "N/A";//item["FinishItemNo"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductCode"].Value = "N/A";//item["FinishProductCode"].ToString();
                    dgvIssue.Rows[j].Cells["FinishProductName"].Value = "N/A"; //item["FinishProductName"].ToString();


                    dgvIssue.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(uomc.ToString());
                    dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvIssue.Rows[j].Cells["UOMQty"].Value = quantity * uomc;//Program.ParseDecimalObject(item["UOMQty"].ToString());
                    dgvIssue.Rows[j].Cells["UOMPrice"].Value = avgPrice;//Program.ParseDecimalObject(item["UOMPrice"].ToString());

                    dgvIssue.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();
                    dgvIssue.Rows[j].Cells["VATName"].Value = "N/A";//item["VATName"].ToString();
                    dgvIssue.Rows[j].Cells["UOMWastage"].Value = 0; //item["UOMWastage"].ToString();




                    j = j + 1;
                } // End For
                btnSave.Text = "&Save";

                Rowcalculate();

                IsUpdate = true;



                FileLogger.Log(this.Name, "Details Load Time after Grid Load", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss:mmm"));

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
                this.btnSearchIssueNo.Enabled = true;
                this.progressBar1.Visible = false;

            }
        }

        #endregion

        #region Issue Navigation


        private void btnFirst_Click(object sender, EventArgs e)
        {
            IssueNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            IssueNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            IssueNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            IssueNavigation("Last");
        }

        private void IssueNavigation(string ButtonName)
        {
            try
            {
                IssueDAL _IssueDAL = new IssueDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(txtId.Text);
                vm.InvoiceNo = txtIssueNo.Text;


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _IssueDAL.Issue_Navigation(vm,null,null,connVM);

                txtIssueNo.Text = vm.InvoiceNo;
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
                FileLogger.Log(this.Name, "btnPrevious_Click", exMessage);
            }
            #endregion Catch

        }

        #endregion

        private void btnIssueInformation_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {

                if (string.IsNullOrWhiteSpace(txtIssueNo.Text))
                {
                    MessageBox.Show("Please Select Issue No");
                    return;
                }

                MISReport _report = new MISReport();


                reportDocument = _report.IssueInformation(txtIssueNo.Text.Trim(), Program.BranchId,connVM);


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

        private void txtIssueNo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
