using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATClient.ModelDTO;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormTollContInOut : Form
    {


        #region Global Variables


        private string[] sqlResults;


        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        string transactionType = string.Empty;
        private bool ChangeData = false;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string CategoryId { get; set; }
        private int IssuePlaceQty;
        private bool IsUpdate = false;
        private string NextID = string.Empty;
        private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;
        private DataTable ProductResultDs;
        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        List<UomDTO> UOMs = new List<UomDTO>();
        private int IssuePlaceAmt;
        private TollContInOutVM tollContMasterVM;
        private List<TollContInOutDetailVM> tollContDetailVMs = new List<TollContInOutDetailVM>();
        private bool IsPost = false;
        private int SearchBranchId = 0;
        private int TollContPlaceQty;
        private int TollContPlaceAmt;
        ResultVM rVM = new ResultVM();
        private DataTable TollContDetailResult;


        private bool Post = false;
        private bool Edit = false;
        private bool Add = false;
        private DataTable uomResult;
        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private int TollContInOutPlaceQty;
        private int TollContPInOutlaceAmt;

        #endregion

        public FormTollContInOut()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }



        private void FormTollContInOut_Load(object sender, EventArgs e)
        {

            #region try
            try
            {
                #region Button Stats

                btnSave.Text = "&Add";
                txtCode.Text = "~~~ New ~~~";

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

                vIssuePlaceQty = commonDal.settingsDesktop("Issue", "Quantity", null, connVM);
                vIssuePlaceAmt = commonDal.settingsDesktop("Issue", "Amount", null, connVM);
                vIssueFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM", null, connVM);
                vIssueAutoPost = commonDal.settingsDesktop("IssueFromBOM", "IssueAutoPost", null, connVM);
                if (string.IsNullOrEmpty(vIssuePlaceQty)
                    || string.IsNullOrEmpty(vIssuePlaceAmt)
                    || string.IsNullOrEmpty(vIssueFromBOM)
                    || string.IsNullOrEmpty(vIssueAutoPost))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                TollContInOutPlaceQty = Convert.ToInt32(vIssuePlaceQty);
                TollContPInOutlaceAmt = Convert.ToInt32(vIssuePlaceAmt);
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


        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region UOM

                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);


                #endregion UOM

                #region Product
                ProductDAL productDal = new ProductDAL();

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



        private void FormMaker()
        {
            try
            {

                #region Transaction Type

                if (rbtnTollCont6_4Outs.Checked)
                {
                    this.Text = "Toll Cont Outs";

                }
                if (rbtnTollCont6_4Ins.Checked)
                {
                    this.Text = "Toll Cont Ins";

                }
                if (rbtnTollCont6_4Backs.Checked)
                {
                    this.Text = "Toll Cont Backs";

                }


                #endregion Transaction Type


                #region ChangeableNBRPrice  Change
                CommonDAL commonDal = new CommonDAL();
                bool ChangeableNbrPrice = commonDal.settingsDesktop("Production", "ChangeableNBRPrice", null, connVM) == "Y" ? false : true;
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

            //UOMIdParam = string.Empty;
            //UOMFromParam = string.Empty;
            //UOMToParam = string.Empty;
            //ActiveStatusUOMParam = string.Empty;
            //UOMIdParam = string.Empty;
            //UOMFromParam = string.Empty;
            //UOMToParam = string.Empty;
            //ActiveStatusUOMParam = "Y";

            #endregion UOM

            txtCategoryName.Text = "Raw";
            #endregion Product


            #region 2012 Law - Button Control

            //btnVAT16.Text = "6.1";
            //btnVAT18.Visible = false;

            #endregion

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                ChangeData = true;

                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item", this.Text);
                    return;
                }
                if (txtUnitCost.Text == "")
                {
                    txtUnitCost.Text = "0.00";
                }

                if (txtQuantity.Text == "")
                {
                    MessageBox.Show("Please insert Quantity");
                    txtQuantity.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtQuantity.Focus();
                    return;
                }


                UomsValue();

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvTollCont.Rows.Add(NewRow);

                //dgvTollProduction["BOMId", dgvTollProduction.RowCount - 1].Value = txtBOMId.Text.Trim();

                dgvTollCont["ItemNo", dgvTollCont.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvTollCont["ItemName", dgvTollCont.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvTollCont["PCode", dgvTollCont.RowCount - 1].Value = txtPCode.Text.Trim();
                string strUom = cmbUom.Text.ToString();
                dgvTollCont["UOM", dgvTollCont.RowCount - 1].Value = strUom.Trim();

                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), TollContPlaceQty).ToString();

                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), TollContPlaceAmt).ToString();


                dgvTollCont["Status", dgvTollCont.RowCount - 1].Value = "New";
                dgvTollCont["Quantity", dgvTollCont.RowCount - 1].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
                dgvTollCont["UnitPrice", dgvTollCont.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvTollCont["UOMc", dgvTollCont.RowCount - 1].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim());
                dgvTollCont["UOMn", dgvTollCont.RowCount - 1].Value = txtUOM.Text.Trim();

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

                for (int i = 0; i < dgvTollCont.RowCount; i++)
                {
                    dgvTollCont[0, i].Value = i + 1;


                    Quantity = Convert.ToDecimal(dgvTollCont["Quantity", i].Value);
                    Cost = Convert.ToDecimal(dgvTollCont["UnitPrice", i].Value);

                    SubTotal = Cost * Quantity;
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), TollContPlaceQty));

                    dgvTollCont["SubTotal", i].Value = Program.ParseDecimalObject(SubTotal).ToString();

                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvTollCont["SubTotal", i].Value);
                    SumQuantity = SumQuantity + Quantity;

                    //dgvTollProduction["Change", i].Value = Program.ParseDecimalObject(Convert.ToDecimal(dgvTollProduction["Quantity", i].Value)
                    //    - Convert.ToDecimal(dgvTollProduction["Previous", i].Value)).ToString();


                }

                txtTotalAmount.Text = Program.ParseDecimalObject(SumSubTotal).ToString();
                txtTotalQuantity.Text = Program.ParseDecimalObject(SumQuantity).ToString();

                //FindBomId();

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

        private void AllClear()
        {
            txtProductCode.Text = "";
            txtProductName.Text = "";
            //txtHSCode.Text = "";
            txtUnitCost.Text = "0.00";
            txtQuantity.Text = "";
            txtUOM.Text = "";
            txtQuantityInHand.Text = "0.00";
            cmbProductName.Text = "";
            cmbUom.Text = "";
            //txtBOMId.Text = "";


        }

        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvTollCont.Rows.Count > 0)
                {
                    dgvTollCont.Rows[dgvTollCont.Rows.Count - 1].Selected = true;
                    dgvTollCont.CurrentCell = dgvTollCont.Rows[dgvTollCont.Rows.Count - 1].Cells[1];
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

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
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

                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products ";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
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
                        txtUnitCost.Text = products.IssuePrice.ToString();
                        txtUOM.Text = products.UOM;
                        cmbUom.Text = products.UOM;
                        txtPCode.Text = products.ProductCode;

                    }

                    #endregion
                }
                string strProductCode = txtProductCode.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    #region Stock and PriceCall

                    ProductDAL productDal = new ProductDAL();

                    txtQuantity.Focus();

                    Uoms();

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

        private void button1_Click(object sender, EventArgs e)
        {


            #region try
            try
            {

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


        private void ProductSearchDsFormLoad()
        {
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

        private void backgroundWorkerProductSearchDsFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProductDAL productDal = new ProductDAL();

                ProductResultDs = new DataTable();
                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode, connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }
            #endregion
        }


        private void ProductSearchDsLoad()
        {
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

        private void backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                    ProductsMini.Add(prod);
                }

                ProductSearchDsLoad();
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.button1.Enabled = true;
            this.progressBar1.Visible = false;


        }

        private void dgvTollCont_DoubleClick(object sender, EventArgs e)
        {


            #region try
            try
            {
                if (dgvTollCont.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //txtBOMId.Text = dgvIssue.CurrentRow.Cells["BOMId"].Value.ToString();

                cmbProductName.Text = dgvTollCont.CurrentRow.Cells["ItemName"].Value.ToString();
                //txtLineNo.Text = dgvTollProduction.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvTollCont.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvTollCont.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvTollCont.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvTollCont.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvTollCont.CurrentRow.Cells["Quantity"].Value).ToString();
                txtUnitCost.Text = Program.ParseDecimalObject(dgvTollCont.CurrentRow.Cells["UnitPrice"].Value).ToString();
                txtUOM.Text = dgvTollCont.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());

                Uoms();

                cmbUom.Text = dgvTollCont.CurrentRow.Cells["UOM"].Value.ToString();
                txtUomConv.Text = dgvTollCont.CurrentRow.Cells["UOMc"].Value.ToString();

                ProductDAL productDal = new ProductDAL();

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
                FileLogger.Log(this.Name, "dgvTollProduction_DoubleClick", exMessage);
            }
            #endregion

        }

        private void PirceCall()
        {

            ProductDAL productDal = new ProductDAL();

            #region BOM Set

            DataTable dt = new DataTable();

            dt = productDal.SelectBOMRaw(txtProductCode.Text.Trim(), dtpTollContDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

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

            //txtBOMId.Text = BOMId.ToString();

            #endregion

            string strProductCode = txtProductCode.Text;

            if (!string.IsNullOrEmpty(strProductCode))
            {
                if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), TollContPlaceAmt).ToString();

                }
                else
                {

                    return;
                }
            }
        }




        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dgvTollCont.RowCount > 0)
            {
                ReceiveChangeSingle();
            }
            else
            {
                MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

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



                ChangeData = true;


                if (string.IsNullOrEmpty(cmbUom.Text))
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }


                UomsValue();


                #region Stock Chekc

                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                //StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text);


                #endregion Stock Chekc


                string strUom = cmbUom.Text;

                //dgvIssue["BOMId", dgvIssue.CurrentRow.Index].Value = txtBOMId.Text.Trim();

                dgvTollCont["UOM", dgvTollCont.CurrentRow.Index].Value = strUom.Trim();
                dgvTollCont["Quantity", dgvTollCont.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();
                dgvTollCont["UnitPrice", dgvTollCont.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvTollCont["UOMc", dgvTollCont.CurrentRow.Index].Value = Program.ParseDecimalObject(txtUomConv.Text.Trim());
                dgvTollCont["UOMn", dgvTollCont.CurrentRow.Index].Value = txtUOM.Text.Trim();


                if (dgvTollCont.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvTollCont["Status", dgvTollCont.CurrentRow.Index].Value = "Change";
                }

                dgvTollCont.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
                dgvTollCont.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();

                txtProductCode.Text = "";
                txtProductName.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "0.00";
                //txtBOMId.Text = "";

                cmbProductName.Focus();

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
                if (dgvTollCont.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvTollCont.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvTollCont.Rows.RemoveAt(dgvTollCont.CurrentRow.Index);
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

        private void btnSave_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                #region Check Point

                #region Find Fiscal Year Lock

                string PeriodName = dtpTollContDate.Value.ToString("MMMM-yyyy");
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

                if (Program.CheckLicence(dtpTollContDate.Value) == true)
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
                    NextID = txtCode.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtRefNo.Text == "" || txtRefNo == null)
                {
                    txtRefNo.Text = "-";
                }
                if (txtImportID.Text == "" || txtImportID == null)
                {
                    txtImportID.Text = "-";
                }

                if (txtVendorName.Text == "")
                {
                    MessageBox.Show("Please Select Vendor", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                #endregion

                #region Exist Check

                TollContInOutVM vm = new TollContInOutVM();

                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new TollContInOutDAL().SelectAllList(0, new[] { "Code" }, new[] { NextID }, null, null, null, connVM).FirstOrDefault();
                }

                if (vm != null && vm.Id != 0)
                {
                    throw new Exception("This Code Already Exist! Cannot Add!" + Environment.NewLine + "Code No: " + NextID);

                }

                #endregion

                if (dgvTollCont.RowCount <= 0)
                {
                    throw new Exception("Please insert Details information for transaction");
                }

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Master


                tollContMasterVM = new TollContInOutVM();

                tollContMasterVM.Code = NextID.ToString();
                tollContMasterVM.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                tollContMasterVM.Comments = txtComments.Text.Trim();
                tollContMasterVM.CreatedBy = Program.CurrentUser;
                tollContMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.LastModifiedBy = Program.CurrentUser;
                tollContMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TransactionType = transactionType;
                tollContMasterVM.Post = "N";
                tollContMasterVM.BranchId = Program.BranchId;
                tollContMasterVM.AppVersion = Program.GetAppVersion();
                tollContMasterVM.RefNo = txtRefNo.Text;
                tollContMasterVM.ImportID = txtImportID.Text;
                tollContMasterVM.PeriodID = "-";
                tollContMasterVM.VendorID = txtVendorID.Text;

                #endregion

                #region Details
                tollContDetailVMs = new List<TollContInOutDetailVM>();

                for (int i = 0; i < dgvTollCont.RowCount; i++)
                {
                    TollContInOutDetailVM detail = new TollContInOutDetailVM();

                    //detail.BomId =dgvTollProduction.Rows[i].Cells["BOMId"].Value.ToString();
                    detail.Code = NextID.ToString();
                    detail.TollLineNo = dgvTollCont.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvTollCont.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.UOM = dgvTollCont.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.FinishItemNo = "0";
                    detail.UOMn = dgvTollCont.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = dgvTollCont.Rows[i].Cells["UOMc"].Value.ToString();
                    detail.UnitCost = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.BranchId = Program.BranchId;

                    tollContDetailVMs.Add(detail);

                }
                #endregion

                #region Check Point

                if (tollContDetailVMs.Count() <= 0)
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

        private void TransactionTypes()
        {
            #region Transaction Type

            transactionType = string.Empty;

            if (rbtnTollCont6_4Outs.Checked)
            {
                transactionType = "TollCont6_4Outs";
            }
            if (rbtnTollCont6_4Ins.Checked)
            {
                transactionType = "TollCont6_4Ins";
            }
            if (rbtnTollCont6_4Backs.Checked)
            {
                transactionType = "TollCont6_4Backs";
            }

            #endregion Transaction Type
        }

        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                TollContInOutDAL tollContDal = new TollContInOutDAL();

                rVM = tollContDal.SaveData(tollContMasterVM, tollContDetailVMs, null, null, connVM);

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
                        string result = rVM.Status;
                        string message = rVM.Message;
                        string newId = rVM.Id;

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {

                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            ChangeData = false;

                        }

                        if (result == "Success")
                        {

                            txtCode.Text = rVM.InvoiceNo;
                            txtHeaderId.Text = rVM.Id;

                            //IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                            for (int i = 0; i < dgvTollCont.RowCount; i++)
                            {
                                dgvTollCont["Status", dgvTollCont.RowCount - 1].Value = "Old";
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

                string PeriodName = dtpTollContDate.Value.ToString("MMMM-yyyy");
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

                if (Convert.ToInt32(SearchBranchId) != Program.BranchId && Convert.ToInt32(SearchBranchId) != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Program.CheckLicence(dtpTollContDate.Value) == true)
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

                    NextID = txtCode.Text.Trim();

                }

                #region Transaction Types

                TransactionTypes();

                #endregion

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtRefNo.Text == "" || txtRefNo == null)
                {
                    txtRefNo.Text = "-";
                }
                if (txtImportID.Text == "" || txtImportID == null)
                {
                    txtImportID.Text = "-";
                }



                if (dgvTollCont.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign

                #region Master Data


                tollContMasterVM = new TollContInOutVM();

                tollContMasterVM.Code = NextID.ToString();
                tollContMasterVM.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                tollContMasterVM.Comments = txtComments.Text.Trim();
                tollContMasterVM.CreatedBy = Program.CurrentUser;
                tollContMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.LastModifiedBy = Program.CurrentUser;
                tollContMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TransactionType = transactionType;
                tollContMasterVM.Post = "N";
                tollContMasterVM.BranchId = Program.BranchId;
                tollContMasterVM.RefNo = txtRefNo.Text;
                tollContMasterVM.ImportID = txtImportID.Text;
                tollContMasterVM.Id = Convert.ToInt32(txtHeaderId.Text);
                tollContMasterVM.VendorID = txtVendorID.Text;

                #endregion

                #region Detail Data

                tollContDetailVMs = new List<TollContInOutDetailVM>();

                for (int i = 0; i < dgvTollCont.RowCount; i++)
                {


                    TollContInOutDetailVM detail = new TollContInOutDetailVM();

                    detail.Code = NextID.ToString();
                    detail.TollLineNo = dgvTollCont.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvTollCont.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.UOM = dgvTollCont.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.FinishItemNo = "0";
                    detail.UOMn = dgvTollCont.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = dgvTollCont.Rows[i].Cells["UOMc"].Value.ToString();
                    detail.UnitCost = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["UnitPrice"].Value.ToString());


                    detail.BranchId = Program.BranchId;

                    tollContDetailVMs.Add(detail);


                }

                #endregion

                #endregion

                #region Check Point

                if (tollContDetailVMs.Count() <= 0)
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

                TollContInOutDAL tollContDal = new TollContInOutDAL();

                rVM = tollContDal.UpdateData(tollContMasterVM, tollContDetailVMs, connVM, Program.CurrentUserID);

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

                        string result = rVM.Status;
                        string message = rVM.Message;
                        string newId = rVM.Id;

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }

                        else if (result == "Success" || result == "Fail")
                        {

                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            ChangeData = false;

                        }


                        if (result == "Success")
                        {

                            txtCode.Text = rVM.InvoiceNo;
                            txtHeaderId.Text = rVM.Id;

                            for (int i = 0; i < dgvTollCont.RowCount; i++)
                            {
                                dgvTollCont["Status", dgvTollCont.RowCount - 1].Value = "Old";
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

                if (Program.CheckLicence(dtpTollContDate.Value) == true)
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

                    NextID = txtCode.Text.Trim();

                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (dgvTollCont.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Set Master


                tollContMasterVM = new TollContInOutVM();

                tollContMasterVM.Code = NextID.ToString();
                tollContMasterVM.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                tollContMasterVM.Comments = txtComments.Text.Trim();
                tollContMasterVM.CreatedBy = Program.CurrentUser;
                tollContMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.LastModifiedBy = Program.CurrentUser;
                tollContMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                tollContMasterVM.TransactionType = transactionType;
                tollContMasterVM.Post = "Y";
                tollContMasterVM.Id = Convert.ToInt32(txtHeaderId.Text);

                #endregion

                #region Set Details

                tollContDetailVMs = new List<TollContInOutDetailVM>();

                for (int i = 0; i < dgvTollCont.RowCount; i++)
                {
                    TollContInOutDetailVM detail = new TollContInOutDetailVM();

                    detail.Code = NextID.ToString();
                    detail.TollLineNo = dgvTollCont.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvTollCont.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.UOM = dgvTollCont.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.DateTime = dtpTollContDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.FinishItemNo = "0";
                    detail.UOMn = dgvTollCont.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = dgvTollCont.Rows[i].Cells["UOMc"].Value.ToString();
                    detail.UnitCost = Convert.ToDecimal(dgvTollCont.Rows[i].Cells["UnitPrice"].Value.ToString());

                    tollContDetailVMs.Add(detail);

                }


                if (tollContDetailVMs.Count() <= 0)
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

                TollContInOutDAL tollContDal = new TollContInOutDAL();
                rVM = tollContDal.PostData(tollContMasterVM, tollContDetailVMs, null, null, connVM, Program.CurrentUser);
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

                        string result = rVM.Status;
                        string message = rVM.Message;
                        string newId = rVM.Id;

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {

                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            IsPost = true;

                        }

                        if (result == "Success")
                        {
                            txtCode.Text = rVM.InvoiceNo;
                            txtHeaderId.Text = rVM.Id;

                            for (int i = 0; i < dgvTollCont.RowCount; i++)
                            {
                                dgvTollCont["Status", dgvTollCont.RowCount - 1].Value = "Old";
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

        private void btnSearchTollContNo_Click(object sender, EventArgs e)
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

                selectedRow = FormTollContSearch.SelectOne(transactionType);

                #endregion Transaction Type

                #region selected Row

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtCode.Text = selectedRow.Cells["Code"].Value.ToString();
                    txtHeaderId.Text = selectedRow.Cells["Id"].Value.ToString();

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
            TollContInOutDAL IsDal = new TollContInOutDAL();

            try
            {
                DataTable dataTable = new DataTable("SearchIssueHeader");

                #region Data Call

                string[] cValues = { txtCode.Text };
                string[] cFields = { "Code like" };

                dataTable = IsDal.SelectAll(0, cFields, cValues, null, null, null, connVM);

                DataRow dr = dataTable.Rows[0];

                #endregion

                #region Value Assign to Form Elements

                txtHeaderId.Text = dr["Id"].ToString();              
                dtpTollContDate.Value = Convert.ToDateTime(dr["DateTime"].ToString());
                txtImportID.Text = dr["ImportID"].ToString();
                txtRefNo.Text = dr["RefNo"].ToString();
                txtComments.Text = dr["Comments"].ToString();
                SearchBranchId = Convert.ToInt32(dr["BranchId"].ToString());
                IsPost = dr["Post"].ToString() == "Y";
                txtVendorName.Text = dr["VendorName"].ToString();
                txtVendorID.Text = dr["VendorID"].ToString();

                #endregion

                #region Button Stats

                this.btnFirst.Enabled = false;
                this.btnPrevious.Enabled = false;
                this.btnNext.Enabled = false;
                this.btnLast.Enabled = false;
                

                #endregion

                #region Background Worker

                backgroundWorkerSearchTollContNo.RunWorkerAsync();

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

        private void backgroundWorkerSearchTollContNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                TollContDetailResult = new DataTable();

                TollContInOutDAL tolleDal = new TollContInOutDAL();

                TollContDetailResult = tolleDal.SelectDetails((txtHeaderId.Text.Trim()), null, null, connVM);

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

        private void backgroundWorkerSearchTollContNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement


                dgvTollCont.Rows.Clear();

                int j = 0;
                foreach (DataRow item in TollContDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvTollCont.Rows.Add(NewRow);

                    dgvTollCont.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    dgvTollCont.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["UnitCost"].ToString());
                    dgvTollCont.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvTollCont.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvTollCont.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());
                    dgvTollCont.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    dgvTollCont.Rows[j].Cells["Status"].Value = "Old";
                    dgvTollCont.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item["UOMc"].ToString());
                    dgvTollCont.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvTollCont.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();

                    j = j + 1;
                }

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
                
                this.progressBar1.Visible = false;

                #endregion

            }

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

        private void txtVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorLoad();
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                btnSave.Text = "&Add";
                txtCode.Text = "~~~ New ~~~";


                ClearAllFields();

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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }


        public void ClearAllFields()
        {
            txtHeaderId.Text = "";
            //txtId.Text = "0";
            SearchBranchId = 0;
            //txtFiscalYear.Text = "0";
            cmbIsRaw.Text = "Select";
            txtQuantityInHand.Text = "0.0";
            txtPCode.Text = "";
            //txtSD.Text = "0.00";
            txtComments.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtTotalAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            txtVATRate.Text = "0.00";
            cmbIsRaw.Text = "";
            txtUOM.Text = "";
            txtTotalQuantity.Text = "0.00";
            cmbUom.Text = "";

            CommonDAL commonDal = new CommonDAL();
            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate", null, connVM);
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpTollContDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
            else
            {
                dtpTollContDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            }

            dgvTollCont.Rows.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            TollContInOutNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            TollContInOutNavigation("Last");
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            TollContInOutNavigation("Previous");
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            TollContInOutNavigation("First");
        }



        private void TollContInOutNavigation(string ButtonName)
        {
            try
            {
                TollContInOutDAL _TollContDAL = new TollContInOutDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                //vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                if (txtHeaderId.Text == "")
                {
                    vm.Id = 0;
                }
                else
                {
                    vm.Id = Convert.ToInt32(txtHeaderId.Text);
                }

                vm.InvoiceNo = txtCode.Text;

                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _TollContDAL.TollCont_Navigation(vm, null, null, connVM);

                txtCode.Text = vm.InvoiceNo;
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

        



    }
}
