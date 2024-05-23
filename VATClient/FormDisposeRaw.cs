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
// // Change 01
//
//
using SymphonySofttech.Utilities;
using VATClient.ReportPages;

using System.Globalization;
using VATClient.ReportPreview;
//
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
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using System.Drawing.Printing;

namespace VATClient
{
    public partial class FormDisposeRaw : Form
    {
        public FormDisposeRaw()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #region Declarations

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public static string vItemNo = "0";


        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private ReportDocument reportDocument = new ReportDocument();

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private string transactionType = string.Empty;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        private string VPrinterName = string.Empty;
        private bool PreviewOnly;
        private string WantToPrint = "N";

        private string CategoryId { get; set; }
        private bool ChangeData = false;
        private string NextID = string.Empty;
        private DataTable ProductResultDs;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;

        private string ItemNoParam = "";
        private string CategoryIDParam = "";
        private string IsRawParam = "";
        private string CategoryName = "";
        private string ActiveStatusProductParam = "";
        private string TradingParam = "";
        private string NonStockParam = "";
        private string ProductCodeParam = "";


        private int ReceivePlaceQty;
        private int ReceivePlaceAmt;

        private string vReceivePlaceQty, vReceivePlaceAmt;


        public int SearchBranchId = 0;
        #endregion Global Variables

        #region Global Variables For BackgroundWorker

        private DataTable DisposeRawsDetailResult = new DataTable();

        private string result = string.Empty;
        private string resultPost = string.Empty;


        private DisposeRawsMasterVM DisposeRawsMasterVM;
        private List<DisposeRawsDetailVM> DisposeRawsDetailVMs = new List<DisposeRawsDetailVM>();
        private List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();


        private bool Add = false;
        private bool Edit = false;

        string ImportExcelID = string.Empty;

        #endregion

        #endregion

        #region Methods 01 / Form Load

        private void FormDisposeRaw_Load(object sender, EventArgs e)
        {
            try
            {
                #region ToolTip

                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchReceiveNo, "Search existing Information");
                ToolTip1.SetToolTip(this.btnPurchaseSearch, "Search Purchase Information");
                ToolTip1.SetToolTip(this.cmbShift, "Production Shift");

                #endregion

                #region Initial Setup

                VATName vname = new VATName();

                ClearAllFields();
                txtDisposeNo.Text = "~~~ New ~~~";
                ChangeData = false;

                #endregion

                #region Settings Read

                CommonDAL commonDal = new CommonDAL();

                vReceivePlaceQty = commonDal.settingsDesktop("Receive", "Quantity",null,connVM);
                vReceivePlaceAmt = commonDal.settingsDesktop("Receive", "Amount",null,connVM);

                #endregion


                #region Initial Setup

                #region Settings Setup

                ReceivePlaceQty = Convert.ToInt32(vReceivePlaceQty);
                ReceivePlaceAmt = Convert.ToInt32(vReceivePlaceAmt);

                #endregion

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

                #region Shift Dropdown

                IShift _sDal = OrdinaryVATDesktop.GetObject<ShiftDAL, ShiftRepo, IShift>(OrdinaryVATDesktop.IsWCF);

                cmbShift.DataSource = _sDal.SearchForTime(DateTime.Now.ToString("HH:mm"), connVM);
                cmbShift.DisplayMember = "ShiftName";
                cmbShift.ValueMember = "Id";

                #endregion

                cmbProductName.Focus();

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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }

            #endregion Catch

        }

        private void FormMaker()
        {
            try
            {

                #region Form Elements - Visibility Setup


                btnAdd.Enabled = true;


                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                IsUpdate = false;
                Post = false;
                IsPost = false;

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

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Product

                CommonDAL _CommonDAL = new CommonDAL();

                ParameterVM paramVM = new ParameterVM();

                paramVM.TableName = "Products pro ";
                paramVM.JoinClause = @" LEFT OUTER JOIN ProductCategories pcr ON pro.CategoryID = pcr.CategoryID ";
                paramVM.selectFields = new[] { "pro.ItemNo", "pro.ProductCode", "pro.ProductName" };
                paramVM.conditionFields = new[] { "pcr.IsRaw", "pro.ActiveStatus" };
                paramVM.conditionValues = new[] { IsRawParam, ActiveStatusProductParam };
                paramVM.OrderBy = " pro.ProductName";



                ProductResultDs = _CommonDAL.Select(paramVM,null,null,connVM);


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

                cmbProductName.DataSource = null;

                DataTable dt = ProductResultDs.Copy();

                #region Add Select Text

                DataRow dr = dt.NewRow();
                dr["ProductName"] = "Select";
                dr["ItemNo"] = "";
                dt.Rows.InsertAt(dr, 0);

                #endregion

                cmbProductName.DataSource = dt;
                cmbProductName.DisplayMember = "ProductName";
                cmbProductName.ValueMember = "ItemNo";

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

            else
            {
                ItemNoParam = string.Empty;
                CategoryIDParam = string.Empty;
                IsRawParam = "Raw";
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                txtCategoryName.Text = "Raw";

            }

            #endregion Product

            #region 2012 Law - Button Control


            #endregion



        }

        private void TransactionTypes()
        {
            #region Transaction Type

            transactionType = string.Empty;

            transactionType = "Other";

            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }

            #endregion Transaction Type
        }

        private void ClearAllFields()
        {
            try
            {
                //cmbProductType.Text = "Select";
                txtComments.Text = "";
                txtIsRaw.Text = "";
                txtNBRPrice.Text = "0.00";
                txtItemNo.Text = "";
                txtProductName.Text = "";
                ////cmbProductName.DataSource = null;


                txtQuantity.Text = "0.00";
                txtPurchaseNo.Text = "";
                txtSerialNo.Text = "";
                txtRefNo.Text = "";
                txtTotalAmount.Text = "0.00";
                txtOfferUnitPrice.Text = "0.00";
                txtTotalSubTotal.Text = "0.00";
                txtTotalQuantity.Text = "0.00";
                txtTotalVATAmount.Text = "0.00";
                txtTotalSDAmount.Text = "0.00";
                txtUnitCost.Text = "0.00";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtSD.Text = "0.00";
                txtPCode.Text = "";
                txtDisposeNo.Text = "";

                CommonDAL commonDal = new CommonDAL();

                string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate",null,connVM);


                if (AutoSessionDate.ToLower() != "y")
                {
                    dtpTransactionDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

                }
                else
                {
                    dtpTransactionDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                }

                dgvDisposeRows.Rows.Clear();

                FormMaker();


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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }

            #endregion Catch
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {

                #region Reset Form Elements

                ClearAllFields();
                txtDisposeNo.Text = "~~~ New ~~~";
                ChangeData = false;

                #endregion

                #region Settings Value

                CommonDAL commonDal = new CommonDAL();
                vReceivePlaceQty = commonDal.settingsDesktop("Receive", "Quantity",null,connVM);
                vReceivePlaceAmt = commonDal.settingsDesktop("Receive", "Amount",null,connVM);

                ReceivePlaceQty = Convert.ToInt32(vReceivePlaceQty);
                ReceivePlaceAmt = Convert.ToInt32(vReceivePlaceAmt);

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

                #region Reset Fields

                IsPost = false;

                cmbProductName.SelectedIndex = 0;

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
                FileLogger.Log(this.Name, "FormReceive_Load", exMessage);
            }

            #endregion Catch

            finally
            {
                ChangeData = false;
            }

        }

        #endregion

        #region Methods 02 / Add, Change, Double Click, Remove

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                #region Flag Update

                ChangeData = true;

                #endregion

                #region Check Point

                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtUnitCost.Focus();
                    return;
                }

                #region Null Check

                if (txtItemNo.Text == "")
                {
                    MessageBox.Show("Please select a Item");
                    return;
                }
                if (txtPurchaseNo.Text == "")
                {
                    MessageBox.Show("Please select Purchase No");
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

                #endregion

                #endregion

                #region Set Data Grid View

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvDisposeRows.Rows.Add(NewRow);

                int Index = dgvDisposeRows.RowCount - 1;

                SetDataGridView(Index, "add");

                #endregion

                #region Method Call - Row Calculate

                Rowcalculate();

                #endregion

                #region Method Call - Select Last Row

                selectLastRow();

                #endregion

                #region Reset Form Elements

                cmbProductName.Focus();

                txtItemNo.Text = "";
                txtProductName.Text = "";
                txtPurchaseNo.Text = "";
                txtUnitCost.Text = "0";
                txtQuantity.Text = "0";
                txtVATRate.Text = "0";
                txtUOM.Text = "";
                txtOfferUnitPrice.Text = "0";

                cmbProductName.Text = "";

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
                FileLogger.Log(this.Name, "ReceiveAddSingle", exMessage);
            }

            #endregion Catch

        }

        private void SetDataGridView(int paramIndex, string rowType)
        {

            try
            {

                dgvDisposeRows["IsSaleable", paramIndex].Value = chkSaleable.Checked ? "Y" : "N";

                dgvDisposeRows["ItemNo", paramIndex].Value = txtItemNo.Text.Trim();
                dgvDisposeRows["ItemName", paramIndex].Value = txtProductName.Text.Trim();
                dgvDisposeRows["PurchaseNo", paramIndex].Value = txtPurchaseNo.Text.Trim();
                dgvDisposeRows["UOM", paramIndex].Value = txtUOM.Text.Trim();
                dgvDisposeRows["VATRate", paramIndex].Value = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
                dgvDisposeRows["SD", paramIndex].Value = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();
                dgvDisposeRows["Comment", paramIndex].Value = txtComment.Text.Trim();

                #region Conditional Values

                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();
                }

                dgvDisposeRows["UnitPrice", paramIndex].Value = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();

                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                {
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();
                }

                #endregion

                dgvDisposeRows["Quantity", paramIndex].Value = Program.ParseDecimalObject(txtQuantity.Text.Trim());
                dgvDisposeRows["OfferUnitPrice", paramIndex].Value = Program.ParseDecimalObject(txtOfferUnitPrice.Text.Trim());

                if (rowType == "change")
                {
                    dgvDisposeRows.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
                    dgvDisposeRows.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                #region Check Point

                if (dgvDisposeRows.RowCount > 0)
                {
                    string Message = MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvDisposeRows.CurrentRow.Cells["ItemNo"].Value;

                    DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dlgRes == DialogResult.Yes)
                    {
                        dgvDisposeRows.Rows.RemoveAt(dgvDisposeRows.CurrentRow.Index);

                        #region Method Call - Row Calculate

                        Rowcalculate();

                        #endregion
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                #region Check Point

                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtUnitCost.Focus();
                    return;
                }

                if (dgvDisposeRows.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #region Flag Update

                ChangeData = true;

                #endregion

                if (string.IsNullOrEmpty(txtUOM.Text) || string.IsNullOrEmpty(txtItemNo.Text) || string.IsNullOrEmpty(txtQuantity.Text))
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


                #endregion

                #region Set Data Grid View

                int Index = dgvDisposeRows.CurrentRow.Index;

                SetDataGridView(Index, "change");

                #endregion

                #region Method Call - Row Calculate

                Rowcalculate();

                #endregion

                #region Reset Form Elements

                txtItemNo.Text = "";
                txtProductName.Text = "";
                txtPurchaseNo.Text = "";
                txtUnitCost.Text = "0";
                txtQuantity.Text = "0";
                txtVATRate.Text = "0";
                txtUOM.Text = "";
                txtOfferUnitPrice.Text = "0";

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

        private void dgvDisposeRows_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (dgvDisposeRows.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign to Form Elements

                DataGridViewRow dgvr = dgvDisposeRows.CurrentRow;

                chkSaleable.Checked = dgvr.Cells["IsSaleable"].Value.ToString() == "Y" ? true : false;

                cmbProductName.Text = dgvr.Cells["ItemName"].Value.ToString();

                txtItemNo.Text = dgvr.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = dgvr.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvr.Cells["UOM"].Value.ToString();
                txtPurchaseNo.Text = dgvr.Cells["PurchaseNo"].Value.ToString();
                txtQuantity.Text = Program.ParseDecimalObject(dgvr.Cells["Quantity"].Value).ToString();
                txtUnitCost.Text = Program.ParseDecimalObject(dgvr.Cells["UnitPrice"].Value).ToString();
                txtOfferUnitPrice.Text = Program.ParseDecimalObject(dgvr.Cells["OfferUnitPrice"].Value).ToString();
                txtSD.Text = Program.ParseDecimalObject(dgvr.Cells["SD"].Value).ToString();
                txtVATRate.Text = Convert.ToDecimal(dgvr.Cells["VATRate"].Value).ToString();


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
                FileLogger.Log(this.Name, "dgvDisposeRows_DoubleClick", exMessage);
            }

            #endregion Catch
        }



        #endregion

        #region Methods 03 / Save, Update, Post, Search

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                #region Null Check

                if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                {
                    MessageBox.Show("Please Select Shift");
                    cmbShift.Focus();
                    return;
                }

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpTransactionDate.Value) == true)
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
                    NextID = txtDisposeNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtRefNo.Text == "")
                {
                    txtRefNo.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvDisposeRows.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region TransactionTypes

                TransactionTypes();

                #endregion

                #region Value Assign
                
                ValueAssign();

                #endregion

                #region Element States

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker - Save

                backgroundWorkerSave.RunWorkerAsync();

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

        private void ValueAssign()
        {
            try
            {
                #region Assign Master

                DisposeRawsMasterVM = new DisposeRawsMasterVM();

                DisposeRawsMasterVM.DisposeNo = NextID;
                DisposeRawsMasterVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                DisposeRawsMasterVM.ShiftId = cmbShift.SelectedValue.ToString();
                DisposeRawsMasterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                DisposeRawsMasterVM.Comments = txtComments.Text.Trim();
                DisposeRawsMasterVM.CreatedBy = Program.CurrentUser;
                DisposeRawsMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                DisposeRawsMasterVM.LastModifiedBy = Program.CurrentUser;
                DisposeRawsMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                DisposeRawsMasterVM.Post = "N";
                DisposeRawsMasterVM.ReferenceNo = txtRefNo.Text.Trim();
                DisposeRawsMasterVM.TransactionType = transactionType;
                DisposeRawsMasterVM.BranchId = Program.BranchId.ToString();
                DisposeRawsMasterVM.AppVersion = Program.GetAppVersion();

                #endregion

                #region Assign Detail

                DisposeRawsDetailVMs = new List<DisposeRawsDetailVM>();

                DataGridViewRow dgvr = new DataGridViewRow();

                for (int i = 0; i < dgvDisposeRows.RowCount; i++)
                {
                    dgvr = new DataGridViewRow();
                    dgvr = dgvDisposeRows.Rows[i];

                    DisposeRawsDetailVM varDisposeRawsDetailVM = new DisposeRawsDetailVM();

                    varDisposeRawsDetailVM.DisposeNo = NextID;
                    varDisposeRawsDetailVM.IsSaleable = dgvr.Cells["IsSaleable"].Value.ToString();
                    varDisposeRawsDetailVM.DisposeLineNo = dgvr.Cells["LineNo"].Value.ToString();
                    varDisposeRawsDetailVM.ItemNo = dgvr.Cells["ItemNo"].Value.ToString();
                    varDisposeRawsDetailVM.Quantity = Convert.ToDecimal(dgvr.Cells["Quantity"].Value.ToString());
                    varDisposeRawsDetailVM.UOM = dgvr.Cells["UOM"].Value.ToString();
                    varDisposeRawsDetailVM.UnitPrice = Convert.ToDecimal(dgvr.Cells["UnitPrice"].Value.ToString());
                    varDisposeRawsDetailVM.OfferUnitPrice = Convert.ToDecimal(dgvr.Cells["OfferUnitPrice"].Value.ToString());
                    varDisposeRawsDetailVM.SD = Convert.ToDecimal(dgvr.Cells["SD"].Value.ToString());
                    varDisposeRawsDetailVM.SDAmount = Convert.ToDecimal(dgvr.Cells["SDAmount"].Value.ToString());
                    varDisposeRawsDetailVM.VATRate = Convert.ToDecimal(dgvr.Cells["VATRate"].Value.ToString());
                    varDisposeRawsDetailVM.VATAmount = Convert.ToDecimal(dgvr.Cells["VATAmount"].Value.ToString());
                    varDisposeRawsDetailVM.SubTotal = Convert.ToDecimal(dgvr.Cells["SubTotal"].Value.ToString());

                    varDisposeRawsDetailVM.Comments = dgvr.Cells["Comment"].Value.ToString();
                    varDisposeRawsDetailVM.Post = "N";
                    varDisposeRawsDetailVM.TransactionType = transactionType;
                    varDisposeRawsDetailVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");

                    varDisposeRawsDetailVM.PurchaseNo = dgvr.Cells["PurchaseNo"].Value.ToString();
                    varDisposeRawsDetailVM.SaleNo = "0";
                    varDisposeRawsDetailVM.BranchId = Program.BranchId.ToString();
                    varDisposeRawsDetailVM.CreatedBy = Program.CurrentUser;
                    varDisposeRawsDetailVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    varDisposeRawsDetailVM.LastModifiedBy = Program.CurrentUser;
                    varDisposeRawsDetailVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    DisposeRawsDetailVMs.Add(varDisposeRawsDetailVM);


                }

                if (DisposeRawsDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void backgroundWorkerSave_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {

                SAVE_DOWORK_SUCCESS = false;

                sqlResults = new string[4];
                DisposeRawDAL DisposeRowsDAL = new DisposeRawDAL();

                sqlResults = DisposeRowsDAL.DisposeRawsInsert(DisposeRawsMasterVM, DisposeRawsDetailVMs,connVM);
                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerSave_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (SAVE_DOWORK_SUCCESS)
                {
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
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        }

                    }
                }

                #region Flag Update

                ChangeData = false;

                #endregion

                #region BranchId

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

                ////if (IsPost == true)
                ////{
                ////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                ////    return;
                ////}
                if (Program.CheckLicence(dtpTransactionDate.Value) == true)
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
                    NextID = txtDisposeNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtRefNo.Text == "")
                {
                    txtRefNo.Text = "-";
                }

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }



                if (dgvDisposeRows.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Value Assign

                ValueAssign();

                #endregion

                #region Element States
                
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker - Update
                
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
            try
            {
                #region Statement



                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                DisposeRawDAL DisposeRowsDAL = new DisposeRawDAL();

                sqlResults = DisposeRowsDAL.DisposeRawsUpdate(DisposeRawsMasterVM, DisposeRawsDetailVMs, connVM);
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
                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
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
                if (Program.CheckLicence(dtpTransactionDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (dgvDisposeRows.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Set Master

                DisposeRawsMasterVM = new DisposeRawsMasterVM();
                DisposeRawsMasterVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                DisposeRawsMasterVM.DisposeNo = txtDisposeNo.Text.Trim();
                DisposeRawsMasterVM.LastModifiedBy = Program.CurrentUser;
                DisposeRawsMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                DisposeRawsMasterVM.Post = "Y";

                #endregion

                #region Form Elements

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker - Post

                backgroundWorkerPost.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }

            #endregion Catch

        }

        private void backgroundWorkerPost_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                DisposeRawDAL DisposeRowsDAL = new DisposeRawDAL();

                sqlResults = DisposeRowsDAL.DisposeRawsPost(DisposeRawsMasterVM, connVM);
                POST_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPost_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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
                            throw new ArgumentNullException("backgroundWorkerPost_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                        }

                        if (result == "Success")
                        {
                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                        }
                    }
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }

        private void btnSearchReceiveNo_Click(object sender, EventArgs e)
        {
            DisposeRawDAL rDal = new DisposeRawDAL();

            DataTable dataTable = new DataTable("SearchSalesHeader");
            try
            {

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Static Values
                
                Program.fromOpen = "Me";

                #endregion

                #region Select Data
                
                DataGridViewRow selectedRow = null;

                selectedRow = FormDisposeRawSearch.SelectOne(transactionType);

                #endregion

                #region Load Form Elements

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    string[] cFields = { "DisposeNo" };
                    string[] cValues = { selectedRow.Cells["DisposeNo"].Value.ToString() };

                    dataTable = rDal.SelectAll(cFields, cValues,null,null,connVM);

                    txtDisposeNo.Text = dataTable.Rows[0]["DisposeNo"].ToString();
                    cmbShift.SelectedValue = dataTable.Rows[0]["ShiftId"];
                    txtSerialNo.Text = dataTable.Rows[0]["SerialNo"].ToString();
                    dtpTransactionDate.Value = Convert.ToDateTime(dataTable.Rows[0]["TransactionDateTime"]);
                    txtRefNo.Text = dataTable.Rows[0]["ReferenceNo"].ToString();

                    txtComments.Text = dataTable.Rows[0]["Comments"].ToString();
                    IsPost = Convert.ToString(dataTable.Rows[0]["Post"].ToString()) == "Y" ? true : false;
                    ImportExcelID = dataTable.Rows[0]["ImportIDExcel"].ToString();
                    SearchBranchId = Convert.ToInt32(dataTable.Rows[0]["BranchId"].ToString());

                    #region Element States

                    this.btnSearchReceiveNo.Enabled = false;
                    this.progressBar1.Visible = true;

                    #endregion

                    #region Background Search

                    bgwDisposeDetailSearch.RunWorkerAsync();

                    #endregion

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
                FileLogger.Log(this.Name, "btnSearchReceiveNo_Click", exMessage);
            }

            #endregion Catch
        }

        #region backgroundWorkerSearch Event

        private void bgwDisposeDetailSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                DisposeRawsDetailResult = new DataTable();
                DisposeRawDAL _DDal = new DisposeRawDAL();

                DisposeRawsDetailResult = _DDal.Select_DisposeRawDetail(txtDisposeNo.Text.Trim(), null, null, null, null, connVM);

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

        private void bgwDisposeDetailSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Set Data Grid View

                dgvDisposeRows.Rows.Clear();
                int j = 0;

                foreach (DataRow item in DisposeRawsDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvDisposeRows.Rows.Add(NewRow);

                    DataGridViewRow currentRow = dgvDisposeRows.Rows[j];

                    currentRow.Cells["IsSaleable"].Value = item["IsSaleable"].ToString();
                    currentRow.Cells["LineNo"].Value = item["DisposeLineNo"].ToString();
                    currentRow.Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    currentRow.Cells["ItemName"].Value = item["ProductName"].ToString();
                    currentRow.Cells["PurchaseNo"].Value = item["PurchaseNo"].ToString();
                    currentRow.Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                    currentRow.Cells["UOM"].Value = item["UOM"].ToString();
                    currentRow.Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["UnitPrice"].ToString());
                    currentRow.Cells["OfferUnitPrice"].Value = Program.ParseDecimalObject(item["OfferUnitPrice"].ToString());
                    currentRow.Cells["SD"].Value = Program.ParseDecimalObject(item["SD"].ToString());
                    currentRow.Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());
                    currentRow.Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    currentRow.Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    currentRow.Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());
                    currentRow.Cells["Comment"].Value = item["Comments"].ToString();

                    j = j + 1;

                }

                #endregion

                #region Method Call - Row Calculate

                Rowcalculate();

                #endregion

                #region Flag Update

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }

            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion


        }

        #endregion

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();


        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {

            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        #endregion

        #region Methods 04 / Call Data from Other Transaction

        private void btnPurchaseSearch_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (String.IsNullOrEmpty(txtItemNo.Text.Trim()))
                {
                    MessageBox.Show("Please Select Product First", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #endregion

                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region Value Assign

                transactionType = "All";

                bool IsDisposeRaw = true;

                #endregion

                #region Select Purchase

                DataTable table = FormPurchaseSearch.SelectMultiple("All", false, true, false, IsDisposeRaw, txtItemNo.Text.Trim());

                if (table.Rows.Count == 0)
                {
                    return;
                }

                #endregion

                #region Set Grid View

                PopulateGrid(table);

                #endregion

                #region Element States

                this.btnPurchaseSearch.Enabled = true;
                this.progressBar1.Visible = false;

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
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;

            }
        }

        private void PopulateGrid(DataTable table)
        {
            try
            {
                #region For Loop - Set Data Grid View

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    PurchaseDAL _purchaseDAL = new PurchaseDAL();

                    DataTable dt = new DataTable();
                    string PurchaseInvoiceNo = table.Rows[i]["PurchaseInvoiceNo"].ToString();
                    string[] cValues = { PurchaseInvoiceNo, txtItemNo.Text.Trim() };
                    string[] cFields = { "pd.PurchaseInvoiceNo like", "pd.ItemNo like" };
                    dt = _purchaseDAL.SelectPurchaseDetail(null, cFields, cValues, null, null, true, connVM);
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvDisposeRows.Rows.Add(NewRow);

                    DataRow dr = dt.Rows[0];

                    int Index = dgvDisposeRows.RowCount - 1;

                    dgvDisposeRows["IsSaleable", Index].Value = "N";

                    dgvDisposeRows["LineNo", Index].Value = i + 1;
                    dgvDisposeRows["ItemName", Index].Value = dr["ProductName"];
                    dgvDisposeRows["ItemNo", Index].Value = dr["ItemNo"];
                    dgvDisposeRows["PurchaseNo", Index].Value = dr["PurchaseInvoiceNo"].ToString();

                    decimal Quantity = Convert.ToDecimal(Program.ParseDecimalObject(dr["Quantity"].ToString()));
                    decimal UnitPrice = Convert.ToDecimal(Program.ParseDecimalObject(dr["CostPrice"].ToString()));

                    dgvDisposeRows["Quantity", Index].Value = Quantity;
                    dgvDisposeRows["UOM", Index].Value = dr["UOM"];
                    dgvDisposeRows["SD", Index].Value = dr["SD"];
                    dgvDisposeRows["VATRate", Index].Value = dr["VATRate"];
                    dgvDisposeRows["UnitPrice", Index].Value = UnitPrice;
                    dgvDisposeRows["OfferUnitPrice", Index].Value = UnitPrice;

                    dgvDisposeRows["Comment", Index].Value = dr["Comments"].ToString();

                    #region Method Call - Row Calculate

                    Rowcalculate();

                    #endregion

                    #region Method Call - Select Last Row

                    selectLastRow();

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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            #endregion

        }


        #endregion

        #region Methods 05 / MISC

        private void Rowcalculate()
        {
            try
            {
                #region Declarations

                decimal SumVATAmount = 0;
                decimal SumSubTotal = 0;
                decimal SumSDAmount = 0;
                decimal TotalQuantuty = 0;
                decimal SumGTotal = 0;

                decimal Quantity = 0;
                decimal SD = 0;
                decimal VATRate = 0;
                decimal Cost = 0;
                decimal SDAmount = 0;
                decimal VATAmount = 0;
                decimal SubAmount = 0;
                decimal Subtotal = 0;

                #endregion

                #region For Loop - Row Calculation

                for (int i = 0; i < dgvDisposeRows.RowCount; i++)
                {
                    Quantity = Convert.ToDecimal(dgvDisposeRows["Quantity", i].Value);
                    VATRate = Convert.ToDecimal(dgvDisposeRows["VATRate", i].Value);
                    Cost = Convert.ToDecimal(dgvDisposeRows["UnitPrice", i].Value);
                    SD = Convert.ToDecimal(dgvDisposeRows["SD", i].Value);

                    if (Program.CheckingNumericString(Quantity.ToString(), "Quantity") == true)
                    {
                        Quantity = Convert.ToDecimal(Program.FormatingNumeric(Quantity.ToString(), ReceivePlaceQty));
                    }

                    if (Program.CheckingNumericString(Cost.ToString(), "Cost") == true)
                    {
                        Cost = Convert.ToDecimal(Program.FormatingNumeric(Cost.ToString(), ReceivePlaceAmt));
                    }

                    SDAmount = (Quantity * Cost) * SD / 100;

                    if (Program.CheckingNumericString(SDAmount.ToString(), "SDAmount") == true)
                    {
                        SDAmount = Convert.ToDecimal(Program.FormatingNumeric(SDAmount.ToString(), ReceivePlaceAmt));
                    }

                    VATAmount = (SDAmount + (Quantity * Cost)) * VATRate / 100;

                    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                    {
                        VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), ReceivePlaceAmt));
                    }

                    Subtotal = Convert.ToDecimal(Program.ParseDecimalObject((Quantity * Cost)));

                    SubAmount = (Quantity * Cost) + (SDAmount + VATAmount);

                    if (Program.CheckingNumericString(SubAmount.ToString(), "SubAmount") == true)
                    {
                        SubAmount = Convert.ToDecimal(Program.FormatingNumeric(SubAmount.ToString(), ReceivePlaceAmt));
                    }


                    dgvDisposeRows[0, i].Value = i + 1;

                    dgvDisposeRows["VATAmount", i].Value = Program.ParseDecimalObject(VATAmount).ToString();//"0,0.00");
                    dgvDisposeRows["SDAmount", i].Value = Program.ParseDecimalObject(SDAmount).ToString();//"0,0.00");
                    dgvDisposeRows["SubTotal", i].Value = Program.ParseDecimalObject(Subtotal).ToString();//"0,0.00");
                    SumVATAmount = SumVATAmount + Convert.ToDecimal(dgvDisposeRows["VATAmount", i].Value);
                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvDisposeRows["SubTotal", i].Value);
                    SumSDAmount = SumSDAmount + Convert.ToDecimal(dgvDisposeRows["SDAmount", i].Value);
                    SumGTotal = (SumGTotal + SubAmount);
                    TotalQuantuty = TotalQuantuty + Quantity;
                }

                #endregion

                #region Gradn Total Values

                if (Program.CheckingNumericString(SumVATAmount.ToString(), "SumVATAmount") == true)
                {
                    SumVATAmount = Convert.ToDecimal(Program.FormatingNumeric(SumVATAmount.ToString(), ReceivePlaceAmt));
                }

                if (Program.CheckingNumericString(SumSubTotal.ToString(), "SumSubTotal") == true)
                {
                    SumSubTotal = Convert.ToDecimal(Program.FormatingNumeric(SumSubTotal.ToString(), ReceivePlaceAmt));
                }

                if (Program.CheckingNumericString(SumSDAmount.ToString(), "SumSDAmount") == true)
                {
                    SumSDAmount = Convert.ToDecimal(Program.FormatingNumeric(SumSDAmount.ToString(), ReceivePlaceAmt));
                }

                txtTotalVATAmount.Text = Program.ParseDecimalObject(SumVATAmount).ToString();
                txtTotalSubTotal.Text = Program.ParseDecimalObject(SumSubTotal).ToString();
                txtTotalSDAmount.Text = Program.ParseDecimalObject(SumSDAmount).ToString();
                txtTotalQuantity.Text = Program.ParseDecimalObject(TotalQuantuty).ToString();
                txtTotalAmount.Text = Program.ParseDecimalObject(SumGTotal).ToString();

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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion Catch
        }

        private void selectLastRow()
        {
            try
            {
                if (dgvDisposeRows.Rows.Count > 0)
                {

                    dgvDisposeRows.Rows[dgvDisposeRows.Rows.Count - 1].Selected = true;
                    dgvDisposeRows.CurrentCell = dgvDisposeRows.Rows[dgvDisposeRows.Rows.Count - 1].Cells[1];

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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }

            #endregion Catch
        }

        private void ProductLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   
select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
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

                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products ";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();
                    cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerAddressLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }




        #endregion

        #region Method 06 / Reporting

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                #region Check Point

                if (txtDisposeNo.Text == "~~~ New ~~~" || txtDisposeNo.Text == "")
                {
                    MessageBox.Show("Please , Select Dispose No First!", this.Text);
                    return;
                }

                #endregion

                #region Report Preview

                PreviewDetails(true);

                #endregion
            }

            #region Catch

            catch (Exception ex)
            {

                throw ex;
            }

            #endregion
        }

        private void PreviewDetails(bool PreviewOnly = false)
        {
            #region Try
            try
            {
                #region Get Data

                NBRReports _report = new NBRReports();

                reportDocument = _report.DisposeRaw(txtDisposeNo.Text.Trim(), Program.BranchId, PreviewOnly,connVM);

                #endregion

                #region Show Report

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (PreviewOnly == true)
                {
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {

                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                    printerSettings.PrinterName = VPrinterName;
                    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

                    MessageBox.Show("You have successfully print ");

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
                    exMessage = exMessage + Environment.NewLine +
                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PreviewDetails",
                               exMessage);
            }
            #endregion

            #region Finally
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrint.Enabled = true;
            }
            #endregion
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (txtDisposeNo.Text == "~~~ New ~~~" || txtDisposeNo.Text == "")
                {
                    MessageBox.Show("Please , Select Dispose No First!", this.Text);
                    return;
                }

                #endregion

                #region Flag Update

                PreviewOnly = false;

                #endregion

                #region Element States

                this.progressBar1.Visible = true;

                #endregion

                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region Print Progress

                string result = FormSalePrint.SelectOne(0);
                string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());
                WantToPrint = PrintResult[1];


                if (WantToPrint == "N")
                {
                    this.progressBar1.Visible = false;
                    return;


                }
                else
                {

                    WantToPrint = PrintResult[1];
                    VPrinterName = PrintResult[2];

                }

                #endregion

                #region Report Preview

                PreviewDetails(false);

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
                FileLogger.Log(this.Name, "btPrint_Click", exMessage);
            }

            #endregion
        }

        #endregion

        #region Methods 07 / Less Used

        private void dtpReceiveDate_ValueChanged(object sender, EventArgs e)
        {
            dtpBOMDate.Value = dtpTransactionDate.Value;
            ChangeData = true;
        }

        private void dtpReceiveDate_Leave(object sender, EventArgs e)
        {
            dtpTransactionDate.Value = Program.ParseDate(dtpTransactionDate);
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

        private void txtSD_Leave(object sender, EventArgs e)
        {
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();

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

        private void btnAddNew_MouseHover(object sender, EventArgs e)
        {

        }

        private void cmbProductName_Leave(object sender, EventArgs e)
        {
            try
            {
                var searchText = cmbProductName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    #region Value Assign


                    ProductDAL _pdal = new ProductDAL();
                    DataTable pdt = new DataTable();

                    pdt = _pdal.ProductDTByItemNo("", searchText,null,null,connVM);

                    if (pdt != null && pdt.Rows.Count > 0)
                    {
                        DataRow drProduct = pdt.Rows[0];

                        txtProductName.Text = drProduct["ProductName"].ToString();
                        txtItemNo.Text = drProduct["ItemNo"].ToString();
                        txtUOM.Text = drProduct["UOM"].ToString();
                        txtSD.Text = Program.ParseDecimalObject(drProduct["SD"].ToString());
                        txtVATRate.Text = Program.ParseDecimalObject(drProduct["VATRate"].ToString());


                    }



                    #endregion
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
                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
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

        private void txtTotalVATAmount_Leave(object sender, EventArgs e)
        {
            txtTotalVATAmount.Text = Program.ParseDecimalObject(txtTotalVATAmount.Text.Trim()).ToString();

        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            txtTotalAmount.Text = Program.ParseDecimalObject(txtTotalAmount.Text.Trim()).ToString();

        }


        #endregion

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
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


                backgroundWorkerDisposeRawProductSearch.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }

            #endregion Catch
        }

        private void backgroundWorkerDisposeRawProductSearch_DoWork(object sender, DoWorkEventArgs e)
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
         
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDisposeRawProductSearch_DoWork", exMessage);
            }

            #endregion

        }

        private void backgroundWorkerDisposeRawProductSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
           
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDisposeRawProductSearch_RunWorkerCompleted", exMessage);
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
                cmbProductName.DataSource = null;
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
                                     select prd.ProductName;


                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName.Items.AddRange(prodByName.ToArray());
                    }
                }
                cmbProductName.Focus();

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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
            }

            #endregion Catch


        }


    }
}
