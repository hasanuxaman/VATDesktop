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
    public partial class FormDisposeFinishNew : Form
    {
        public FormDisposeFinishNew()
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
        ResultVM rVM = new ResultVM();

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

        private DataTable dtUOM = new DataTable();

        private DataTable dtDetail = new DataTable();

        private string result = string.Empty;
        private string resultPost = string.Empty;


        private DisposeFinishMasterVM masterVM;
        private List<DisposeFinishDetailVM> detailVMs = new List<DisposeFinishDetailVM>();

        private bool Add = false;
        private bool Edit = false;

        string ImportExcelID = string.Empty;

        #endregion

        #endregion

        #region Methods 01 / Form Load

        private void FormDisposeFinish_Load(object sender, EventArgs e)
        {
            try
            {
                #region ToolTip

                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ToolTip1.SetToolTip(this.btnSearchReceiveNo, "Search existing Information");
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

                if (rbtnDisposeTrading.Checked)
                {
                    this.Text = "Dispose Trading Entry";
                    label7.Text = "Trading (F9)";
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
                CommonDAL _CommonDAL = new CommonDAL();
                ParameterVM paramVM = new ParameterVM();


                #region Product



                paramVM.TableName = "Products pro ";
                paramVM.JoinClause = @" LEFT OUTER JOIN ProductCategories pcr ON pro.CategoryID = pcr.CategoryID ";
                paramVM.selectFields = new[] { "pro.ItemNo", "pro.ProductCode", "pro.ProductName" };
                paramVM.conditionFields = new[] { "pcr.IsRaw", "pro.ActiveStatus" };
                paramVM.conditionValues = new[] { IsRawParam, ActiveStatusProductParam };
                paramVM.OrderBy = " pro.ProductName";



                ProductResultDs = _CommonDAL.Select(paramVM,null,null,connVM);


                #endregion Product

                #region UOM

                _CommonDAL = new CommonDAL();

                paramVM = new ParameterVM();

                paramVM.TableName = "UOMName";
                paramVM.selectFields = new[] { "UOMName" };
                paramVM.OrderBy = " UOMName";

                dtUOM = _CommonDAL.Select(paramVM,null,null,connVM);


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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }

            #endregion

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {

                #region Product

                cmbItemNo.DataSource = null;

                DataTable dt = ProductResultDs.Copy();

                #region Add Select Text

                DataRow dr = dt.NewRow();
                dr["ProductName"] = "Select";
                dr["ItemNo"] = "";
                dt.Rows.InsertAt(dr, 0);

                #endregion

                cmbItemNo.DataSource = dt;
                cmbItemNo.DisplayMember = "ProductName";
                cmbItemNo.ValueMember = "ItemNo";

                #endregion Product

                #region UOM

                cmbUOM.DataSource = null;

                dt = dtUOM.Copy();

                #region Add Select Text

                ////dr = dt.NewRow();
                ////dr["UOMName"] = "Select";
                ////dr["ItemNo"] = "";
                ////dt.Rows.InsertAt(dr, 0);

                #endregion

                cmbUOM.DataSource = dt;
                cmbUOM.DisplayMember = "UOMName";
                cmbUOM.ValueMember = "UOMName";

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
                CategoryName = string.Empty;
                ActiveStatusProductParam = "Y";
                TradingParam = string.Empty;
                NonStockParam = string.Empty;
                ProductCodeParam = string.Empty;
                if (rbtnDisposeTrading.Checked)
                {
                    IsRawParam = "Trading";
                    txtCategoryName.Text = "Trading";

                }
                else
                {
                    IsRawParam = "Raw";
                    txtCategoryName.Text = "Raw";

                }
                
            }

            #endregion Product

            #region 2012 Law - Button Control


            #endregion



        }

        private void TransactionTypes()
        {
            #region Transaction Type

            transactionType = string.Empty;

            //////transactionType = "Other";

            if (rbtnOther.Checked)
            {
                transactionType = "Other";
            }
            if (rbtnDisposeTrading.Checked)
            {
                transactionType = "DisposeTrading";
            }

            #endregion Transaction Type
        }

        private void ClearAllFields()
        {
            try
            {
                txtDisposeNo.Text = "";
                txtIsRaw.Text = "";
                txtNBRPrice.Text = "0.00";
                txtFinishProduct.Text = "";
                txtFinishItemNo.Text = "";
                txtQuantity.Text = "0";
                txtBOMId.Text = "";

                txtUnitPrice.Text = "0.00";
                txtOfferUnitPrice.Text = "0.00";
                txtFinishUOM.Text = "";
                txtSerialNo.Text = "";

                cmbItemNo.SelectedItem = "";
                txtUsedQuantity.Text = "0";
                cmbUOM.SelectedItem = "";

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

                dgvDisposeFinish.Rows.Clear();

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

                cmbItemNo.SelectedIndex = 0;

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

                #region Null Check

                if (string.IsNullOrWhiteSpace(cmbItemNo.SelectedValue.ToString()))
                {
                    MessageBox.Show("Please select a Item!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUsedQuantity.Text) || Convert.ToDecimal(txtUsedQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please Input Quantity!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                #endregion

                #endregion

                #region Set Data Grid View

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvDisposeFinish.Rows.Add(NewRow);

                int Index = dgvDisposeFinish.RowCount - 1;

                dgvDisposeFinish["LineNo", Index].Value = Index + 1;

                SetDataGridView(Index, "add");

                #endregion

                #region Method Call - Row Calculate

                ////Rowcalculate();

                #endregion

                #region Method Call - Select Last Row

                selectLastRow();

                #endregion

                #region Reset Form Elements

                cmbItemNo.Focus();

                ////cmbItemNo.SelectedValue = "";
                txtUsedQuantity.Text = "0";
                txtRemainingQuantity.Text = "0";
                ////cmbUOM.SelectedValue = "";

                ////cmbItemNo.Text = "";

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

                dgvDisposeFinish["ItemNo", paramIndex].Value = cmbItemNo.SelectedValue;
                dgvDisposeFinish["ItemName", paramIndex].Value = cmbItemNo.Text.Trim();
                dgvDisposeFinish["UOM", paramIndex].Value = cmbUOM.Text.Trim();

                #region Conditional Values

                if (Program.CheckingNumericTextBox(txtUsedQuantity, "txtUsedQuantity") == true)
                {
                    txtUsedQuantity.Text = Program.FormatingNumeric(txtUsedQuantity.Text.Trim(), ReceivePlaceQty).ToString();
                }

                #endregion

                dgvDisposeFinish["UsedQuantity", paramIndex].Value = Program.ParseDecimalObject(txtUsedQuantity.Text.Trim());


                if (rowType == "change")
                {
                    dgvDisposeFinish.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
                    dgvDisposeFinish.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
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

                if (dgvDisposeFinish.RowCount > 0)
                {
                    string Message = MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvDisposeFinish.CurrentRow.Cells["ItemNo"].Value;

                    DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dlgRes == DialogResult.Yes)
                    {
                        dgvDisposeFinish.Rows.RemoveAt(dgvDisposeFinish.CurrentRow.Index);

                        #region Method Call - Row Calculate

                        ////Rowcalculate();

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


                if (dgvDisposeFinish.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #region Flag Update

                ChangeData = true;

                #endregion

                if (cmbItemNo.SelectedValue == "")
                {
                    MessageBox.Show("Please select a Item!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUsedQuantity.Text) || Convert.ToDecimal(txtUsedQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please Input Quantity!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #endregion

                #region Set Data Grid View

                int Index = dgvDisposeFinish.CurrentRow.Index;

                SetDataGridView(Index, "change");

                RebateCalculation(Index);

                #endregion

                #region Method Call - Row Calculate

                ////Rowcalculate();

                #endregion

                #region Reset Form Elements

                cmbItemNo.Focus();

                ////cmbItemNo.SelectedValue = "";
                ////txtUsedQuantity.Text = "1";
                ////cmbUOM.SelectedValue = "";

                ////cmbItemNo.Text = "";

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

        private void dgvDisposeFinish_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                if (dgvDisposeFinish.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign to Form Elements

                DataGridViewRow dgvr = dgvDisposeFinish.CurrentRow;

                decimal UsedQuantity = 0;
                decimal PurchaseQuantity = 0;
                decimal RemainingQuantity = 0;

                UsedQuantity = Convert.ToDecimal(dgvr.Cells["UsedQuantity"].Value ?? 0);
                PurchaseQuantity = Convert.ToDecimal(dgvr.Cells["PurchaseQuantity"].Value ?? 0);

                UsedQuantity = Convert.ToDecimal(Program.FormatingNumeric(UsedQuantity.ToString(), ReceivePlaceQty));
                PurchaseQuantity = Convert.ToDecimal(Program.FormatingNumeric(PurchaseQuantity.ToString(), ReceivePlaceQty));





                cmbItemNo.SelectedValue = dgvr.Cells["ItemNo"].Value.ToString();
                cmbUOM.SelectedValue = dgvr.Cells["UOM"].Value.ToString();
                txtUsedQuantity.Text = UsedQuantity.ToString();




                RemainingQuantity = UsedQuantity - PurchaseQuantity;
                RemainingQuantity = Convert.ToDecimal(Program.FormatingNumeric(RemainingQuantity.ToString(), ReceivePlaceQty));


                if (RemainingQuantity > 0)
                {
                    txtRemainingQuantity.Text = RemainingQuantity.ToString();
                }
                else
                {
                    txtRemainingQuantity.Text = "0";
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

                #region Check Point



                #region Exist Check

                DisposeFinishMasterVM vm = new DisposeFinishMasterVM();

                if (!string.IsNullOrWhiteSpace(txtDisposeNo.Text))
                {
                    vm = new DisposeFinishDAL().SelectVM(new[] { "df.DisposeNo" }, new[] { txtDisposeNo.Text },null,null,connVM).FirstOrDefault();
                }

                if (vm != null && vm.Id > 0)
                {
                    throw new Exception("This Dispose Already Exist! Cannot Add!" + Environment.NewLine + "Dispose No: " + txtDisposeNo.Text);

                }

                #endregion

                #region Null Check


                if (string.IsNullOrWhiteSpace(txtFinishProduct.Text))
                {
                    MessageBox.Show("Please Select Product!");
                    txtFinishProduct.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtQuantity.Text) || Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please Input Quantity!");
                    txtQuantity.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Product!");
                    cmbShift.Focus();
                    return;
                }


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

                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }

                if (dgvDisposeFinish.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

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

                #region Check Point

                string FinishItemNo = "";
                if (!string.IsNullOrWhiteSpace(txtFinishItemNo.Text))
                {
                    FinishItemNo = txtFinishItemNo.Text.Trim();
                }
                else
                {
                    MessageBox.Show("Please Select Product!");
                    return;
                }

                decimal Quantity = 0;

                if (!string.IsNullOrWhiteSpace(txtQuantity.Text) && Convert.ToDecimal(txtQuantity.Text) > 0)
                {
                    Quantity = Convert.ToDecimal(txtQuantity.Text);
                }
                else
                {
                    MessageBox.Show("Please Inpute Quantity!");
                    return;
                }


                int BOMId = 0;

                if (!string.IsNullOrWhiteSpace(txtBOMId.Text) && Convert.ToInt32(txtBOMId.Text) > 0)
                {
                    BOMId = Convert.ToInt32(txtBOMId.Text);
                }


                #endregion

                #region Assign Master

                masterVM = new DisposeFinishMasterVM();

                masterVM.DisposeNo = NextID;

                masterVM.FinishItemNo = FinishItemNo;
                masterVM.Quantity = Quantity;
                masterVM.UOM = txtFinishUOM.Text;

                masterVM.UnitPrice = Convert.ToDecimal(txtUnitPrice.Text.Trim());

                masterVM.UnitPrice = Convert.ToDecimal(txtUnitPrice.Text.Trim());
                masterVM.OfferUnitPrice = Convert.ToDecimal(txtOfferUnitPrice.Text.Trim());
                masterVM.IsSaleable = chkSaleable.Checked ? "Y" : "N";
                masterVM.BOMId = BOMId;

                masterVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.ShiftId = cmbShift.SelectedValue.ToString();
                masterVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                masterVM.Comments = txtComment.Text.Trim();
                masterVM.CreatedBy = Program.CurrentUser;
                masterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.LastModifiedBy = Program.CurrentUser;
                masterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.Post = "N";
                masterVM.TransactionType = transactionType;
                masterVM.BranchId = Program.BranchId;
                masterVM.AppVersion = Program.GetAppVersion();

                #endregion

                #region Assign Detail

                detailVMs = new List<DisposeFinishDetailVM>();

                DataGridViewRow dgvr = new DataGridViewRow();

                for (int i = 0; i < dgvDisposeFinish.RowCount; i++)
                {
                    dgvr = new DataGridViewRow();
                    dgvr = dgvDisposeFinish.Rows[i];

                    DisposeFinishDetailVM detailVM = new DisposeFinishDetailVM();

                    detailVM.DisposeNo = NextID;
                    detailVM.DisposeLineNo = dgvr.Cells["LineNo"].Value.ToString();
                    detailVM.ItemNo = dgvr.Cells["ItemNo"].Value.ToString();
                    detailVM.UOM = dgvr.Cells["UOM"].Value.ToString();
                    detailVM.UsedQuantity = Convert.ToDecimal(dgvr.Cells["UsedQuantity"].Value.ToString());
                    detailVM.PurchaseNo = (dgvr.Cells["PurchaseNo"].Value ?? Convert.DBNull).ToString();
                    detailVM.PurchaseQuantity = Convert.ToDecimal(dgvr.Cells["PurchaseQuantity"].Value ?? 0);

                    detailVM.VATRate = Convert.ToDecimal(dgvr.Cells["VATRate"].Value ?? 0);
                    detailVM.VATAmount = Convert.ToDecimal(dgvr.Cells["VATAmount"].Value ?? 0);

                    detailVM.RebateRate = Convert.ToDecimal(dgvr.Cells["RebateRate"].Value ?? 0);
                    detailVM.RebateVATAmount = Convert.ToDecimal(dgvr.Cells["RebateVATAmount"].Value ?? 0);


                    detailVM.Comments = (dgvr.Cells["Comment"].Value ?? "").ToString();
                    detailVM.Post = "N";
                    detailVM.TransactionType = transactionType;
                    detailVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");

                    detailVM.BranchId = Program.BranchId.ToString();
                    detailVM.CreatedBy = Program.CurrentUser;
                    detailVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detailVM.LastModifiedBy = Program.CurrentUser;
                    detailVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    detailVMs.Add(detailVM);

                }

                masterVM.Details = detailVMs;

                if (detailVMs.Count() <= 0)
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

                DisposeFinishDAL _DisposeFinishDAL = new DisposeFinishDAL();

                rVM = _DisposeFinishDAL.Insert(masterVM,null,null,connVM);

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

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                #region Set Form Elements / Fields

                txtDisposeNo.Text = rVM.InvoiceNo;
                IsPost = rVM.Post == "Y" ? true : false;

                SearchBranchId = Program.BranchId;

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

                if (txtComment.Text == "")
                {
                    txtComment.Text = "-";
                }


                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }



                if (dgvDisposeFinish.RowCount <= 0)
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

                DisposeFinishDAL _DisposeFinishDAL = new DisposeFinishDAL();

                rVM = _DisposeFinishDAL.Update(masterVM,null,null,connVM);

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
                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                #region Set Form Elements / Fields

                txtDisposeNo.Text = rVM.InvoiceNo;
                IsPost = rVM.Post == "Y" ? true : false;

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

                if (dgvDisposeFinish.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Set Master

                masterVM = new DisposeFinishMasterVM();
                masterVM.TransactionDateTime = dtpTransactionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.DisposeNo = txtDisposeNo.Text.Trim();
                masterVM.LastModifiedBy = Program.CurrentUser;
                masterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.Post = "Y";

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
                sqlResults = new string[4];
                DisposeFinishDAL _DisposeFinishDAL = new DisposeFinishDAL();

                ParameterVM paramVM = new ParameterVM();
                paramVM.InvoiceNo = txtDisposeNo.Text;

                rVM = _DisposeFinishDAL.Post(paramVM,null,null,connVM);

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
                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                #region Set Elements and Fields

                ////txtDisposeNo.Text = rVM.InvoiceNo.ToString();
                IsPost = rVM.Post == "Y" ? true : false;

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DisposeFinishDAL _DisposeFinishDAL = new DisposeFinishDAL();

            DataTable dataTable = new DataTable("Header");
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

                selectedRow = FormDisposeFinishNewSearch.SelectOne(transactionType);

                #endregion

                #region Load Form Elements

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    string[] cFields = { "DisposeNo" };
                    string[] cValues = { selectedRow.Cells["DisposeNo"].Value.ToString() };

                    dataTable = _DisposeFinishDAL.Select(cFields, cValues,null,null,connVM);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {

                        DataRow dr = dataTable.Rows[0];

                        txtDisposeNo.Text = dr["DisposeNo"].ToString();


                        txtFinishProduct.Text = dr["ProductName"].ToString();
                        txtFinishItemNo.Text = dr["FinishItemNo"].ToString();
                        txtQuantity.Text = Program.FormatingNumeric(dr["Quantity"].ToString(), ReceivePlaceQty);
                        txtFinishUOM.Text = dr["UOM"].ToString();
                        txtUnitPrice.Text = Program.FormatingNumeric(dr["UnitPrice"].ToString(), ReceivePlaceQty);
                        txtOfferUnitPrice.Text = Program.FormatingNumeric(dr["OfferUnitPrice"].ToString(), ReceivePlaceQty);
                        chkSaleable.Checked = Convert.ToString(dr["IsSaleable"]) == "Y" ? true : false;
                        txtBOMId.Text = dr["BOMId"].ToString();



                        cmbShift.SelectedValue = dr["ShiftId"];
                        txtSerialNo.Text = dr["SerialNo"].ToString();
                        dtpTransactionDate.Value = Convert.ToDateTime(dr["TransactionDateTime"]);

                        txtComment.Text = dr["Comments"].ToString();
                        IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                        ImportExcelID = dr["ImportIDExcel"].ToString();
                        SearchBranchId = Convert.ToInt32(dr["BranchId"].ToString());

                        #region Element States

                        this.btnSearchReceiveNo.Enabled = false;
                        this.progressBar1.Visible = true;

                        #endregion

                        #region Background Search

                        bgwDisposeDetailSearch.RunWorkerAsync();

                        #endregion

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

                dtDetail = new DataTable();
                DisposeFinishDAL _DDal = new DisposeFinishDAL();

                string[] cFields = new[] { "dfd.DisposeNo" };
                string[] cValues = new[] { txtDisposeNo.Text.Trim() };

                dtDetail = _DDal.SelectDetail(cFields, cValues,null,null,connVM);

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

                dgvDisposeFinish.Rows.Clear();
                int j = 0;

                foreach (DataRow item in dtDetail.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvDisposeFinish.Rows.Add(NewRow);

                    DataGridViewRow currentRow = dgvDisposeFinish.Rows[j];

                    currentRow.Cells["LineNo"].Value = item["DisposeLineNo"].ToString();
                    currentRow.Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    currentRow.Cells["ItemName"].Value = item["ProductName"].ToString();
                    currentRow.Cells["UOM"].Value = item["UOM"].ToString();
                    currentRow.Cells["UsedQuantity"].Value = Program.ParseDecimalObject(item["UsedQuantity"].ToString());
                    currentRow.Cells["PurchaseNo"].Value = item["PurchaseNo"].ToString();
                    currentRow.Cells["PurchaseQuantity"].Value = Program.ParseDecimalObject(item["PurchaseQuantity"].ToString());
                    currentRow.Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                    currentRow.Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    currentRow.Cells["RebateRate"].Value = Program.ParseDecimalObject(item["RebateRate"].ToString());
                    currentRow.Cells["RebateVATAmount"].Value = Program.ParseDecimalObject(item["RebateVATAmount"].ToString());
                    currentRow.Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                    currentRow.Cells["Comment"].Value = item["Comments"].ToString();

                    j = j + 1;

                }

                #endregion

                #region Method Call - Row Calculate

                ////Rowcalculate();

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

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        #endregion

        #region Methods 05 / MISC

        private void Rowcalculate()
        {
            try
            {
                #region Declarations

                decimal UsedQuantity = 0;
                decimal PurchaseQuantity = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal RebateRate = 0;
                decimal RebateVATAmount = 0;

                #endregion

                #region For Loop - Row Calculation

                for (int i = 0; i < dgvDisposeFinish.RowCount; i++)
                {
                    UsedQuantity = Convert.ToDecimal(dgvDisposeFinish["UsedQuantity", i].Value);
                    VATRate = Convert.ToDecimal(dgvDisposeFinish["VATRate", i].Value);

                    if (Program.CheckingNumericString(UsedQuantity.ToString(), "Quantity") == true)
                    {
                        UsedQuantity = Convert.ToDecimal(Program.FormatingNumeric(UsedQuantity.ToString(), ReceivePlaceQty));
                    }

                    if (Program.CheckingNumericString(VATAmount.ToString(), "VATAmount") == true)
                    {
                        VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), ReceivePlaceAmt));
                    }



                    dgvDisposeFinish[0, i].Value = i + 1;

                    dgvDisposeFinish["VATAmount", i].Value = Program.ParseDecimalObject(VATAmount).ToString();//"0,0.00");
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }

            #endregion Catch
        }

        private void selectLastRow()
        {
            try
            {
                if (dgvDisposeFinish.Rows.Count > 0)
                {

                    dgvDisposeFinish.Rows[dgvDisposeFinish.Rows.Count - 1].Selected = true;
                    dgvDisposeFinish.CurrentCell = dgvDisposeFinish.Rows[dgvDisposeFinish.Rows.Count - 1].Cells[1];

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

                #region Sql Text

                string SqlText = @"   
select 
p.ItemNo
,p.ProductCode
,p.ProductName
,p.ShortName
,p.UOM
,pc.CategoryName
,pc.IsRaw ProductType  
from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' 
and IsRaw in('Finish','Trading')
";


                string SQLTextRecordCount = @" 
select count(ProductCode)RecordNo 
from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' 
and IsRaw in('Finish','Trading')
";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";

                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();

                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtFinishProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();
                    txtFinishUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                    txtFinishItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();




                }
            }
            #region Catch

            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #endregion


        }

        private void RawProductLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                #region Sql Text

                string SqlText = @"   
select 
p.ItemNo
,p.ProductCode
,p.ProductName
,p.ShortName
,p.UOM
,pc.CategoryName
,pc.IsRaw ProductType  
from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' 
--and IsRaw in('Raw','Pack')
and IsRaw in('"+IsRawParam+"')";


                string SQLTextRecordCount = @" 
select count(ProductCode)RecordNo 
from Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' 
--and IsRaw in('Raw','Pack')
and IsRaw in('" + IsRawParam + "')";


                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";

                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();

                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    cmbItemNo.SelectedValue = selectedRow.Cells["ItemNo"].Value.ToString();
                    ////cmbItemNo.SelectedItem = selectedRow.Cells["ItemNo"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void BOMLoad()
        {
            try
            {

                DataGridViewRow selectedRow = new DataGridViewRow();

                string FinishItemNo = "";
                FinishItemNo = txtFinishItemNo.Text;

                #region Sql Text

                string SqlText = @"   
select 

BOMId
,BOMId
,FinishItemNo
,EffectDate
,VATName
,VATRate
,SD
,NBRPrice
,Post
,UOM
,FirstSupplyDate
,BranchId

from BOMs
where 1=1 and FinishItemNo=@FinishItemNo
";

                string SQLTextRecordCount = @" 
select count(FinishItemNo)RecordNo 
from BOMs
where 1=1 and FinishItemNo=@FinishItemNo
";

                SqlText = SqlText.Replace("@FinishItemNo", "'" + FinishItemNo + "'");
                SQLTextRecordCount = SQLTextRecordCount.Replace("@FinishItemNo", "'" + FinishItemNo + "'");


                string[] shortColumnName = {     "BOMId"
                                               ,"BOMId"
                                               , "FinishItemNo"
                                               , "EffectDate"
                                               , "VATName"
                                               , "VATRate"
                                               , "SD"
                                               , "NBRPrice"
                                               , "Post"
                                               , "UOM"
                                               , "FirstSupplyDate"
                                               , "BranchId"
                                           };
                string tableName = "";

                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();

                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);

                if (selectedRow != null && selectedRow.Index >= 0)
                {

                    txtBOMId.Text = selectedRow.Cells["BOMId"].Value.ToString();

                    dgvDisposeFinish.DataSource = null;
                    dgvDisposeFinish.Rows.Clear();

                }
            }
            #region Catch

            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "BOMLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #endregion
        }


        private DataGridViewRow PurchaseLoad(string ItemNo)
        {
            DataGridViewRow selectedRow = new DataGridViewRow();

            try
            {


                #region Sql Text

                string SqlText = @"   

select Id, PurchaseInvoiceNo, ReceiveDate,BENumber,InvoiceDateTime, UOMQty, VATRate, VATAmount, Post 
from PurchaseInvoiceDetails
where 1=1 and Post='Y'
and ItemNo=@ItemNo
";


                string SQLTextRecordCount = @" 
select count(PurchaseInvoiceNo)RecordNo 
from PurchaseInvoiceDetails
where 1=1 and Post='Y'
and ItemNo=@ItemNo
";

                SqlText = SqlText.Replace("@ItemNo", "'" + ItemNo + "'");
                SQLTextRecordCount = SQLTextRecordCount.Replace("@ItemNo", "'" + ItemNo + "'");

                string[] shortColumnName = { "Id", "PurchaseInvoiceNo", "UOMQty", "VATRate", "VATAmount", "Post", "BENumber" };
                string tableName = "";

                #endregion

                FormMultipleSearch frm = new FormMultipleSearch();

                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);

            }
            #region Catch

            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #endregion

            finally { }

            return selectedRow;
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

                reportDocument = _report.DisposeFinish(txtDisposeNo.Text.Trim(), Program.BranchId, PreviewOnly,connVM);

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


        #endregion


        private void txtFinishProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
                txtBOMId.Focus();
            }
        }

        private void txtFinishProduct_DoubleClick(object sender, EventArgs e)
        {
            ProductLoad();
            txtBOMId.Focus();
        }

        private void txtFinishProduct_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBOM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                BOMLoad();
            }
        }

        private void txtBOM_DoubleClick(object sender, EventArgs e)
        {
            BOMLoad();
        }


        private void cmbItemNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                RawProductLoad();
            }
        }

        private void cmbItemNo_Leave(object sender, EventArgs e)
        {
            try
            {
                var searchText = cmbItemNo.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    #region Declarations

                    ProductDAL _pdal = new ProductDAL();
                    DataTable pdt = new DataTable();

                    #endregion

                    #region Data Call

                    pdt = _pdal.ProductDTByItemNo("", searchText,null,null,connVM);

                    #endregion

                    #region Value Assign

                    if (pdt != null && pdt.Rows.Count > 0)
                    {
                        DataRow drProduct = pdt.Rows[0];

                        cmbUOM.SelectedValue = drProduct["UOM"].ToString();
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

        private void txtBOM_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvDisposeRows_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {

                if (e.RowIndex < 0)
                {
                    return;
                }

                int Index = dgvDisposeFinish.Columns["btnPurchaseNo"].Index;

                if (e.ColumnIndex == Index)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    var w = Properties.Resources.Delete.Width;
                    var h = Properties.Resources.Delete.Height;
                    var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                    var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                    e.Graphics.DrawImage(Properties.Resources.search, new Rectangle(x, y, w, h));
                    e.Handled = true;
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
                FileLogger.Log(this.Name, "dgvDisposeRows_CellPainting", exMessage);
            }
            #endregion
        }

        private void dgvDisposeRows_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                if (e.RowIndex < 0)
                {
                    return;
                }

                int Index = dgvDisposeFinish.Columns["btnPurchaseNo"].Index;

                if (e.ColumnIndex == Index)
                {

                    int RowIndex = e.RowIndex;

                    DataGridViewRow selectedRow = new DataGridViewRow();

                    string ItemNo = dgvDisposeFinish["ItemNo", RowIndex].Value.ToString();

                    selectedRow = PurchaseLoad(ItemNo);

                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        decimal UsedQuantity = 0; //F
                        decimal PurchaseQuantity = 0; // H
                        decimal VATRate = 0;
                        decimal VATAmount = 0;

                        UsedQuantity = Convert.ToDecimal(dgvDisposeFinish["UsedQuantity", RowIndex].Value);
                        UsedQuantity = Convert.ToDecimal(Program.FormatingNumeric(UsedQuantity.ToString(), ReceivePlaceQty));

                        PurchaseQuantity = Convert.ToDecimal(selectedRow.Cells["UOMQty"].Value);
                        PurchaseQuantity = Convert.ToDecimal(Program.FormatingNumeric(PurchaseQuantity.ToString(), ReceivePlaceQty));

                        VATRate = Convert.ToDecimal(selectedRow.Cells["VATRate"].Value);
                        VATRate = Convert.ToDecimal(Program.FormatingNumeric(VATRate.ToString(), ReceivePlaceQty));

                        VATAmount = Convert.ToDecimal(selectedRow.Cells["VATAmount"].Value);
                        VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), ReceivePlaceQty));

                        dgvDisposeFinish["PurchaseNo", RowIndex].Value = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                        dgvDisposeFinish["PurchaseQuantity", RowIndex].Value = PurchaseQuantity;
                        dgvDisposeFinish["VATRate", RowIndex].Value = VATRate;
                        dgvDisposeFinish["VATAmount", RowIndex].Value = VATAmount;

                        RebateCalculation(RowIndex);

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
                FileLogger.Log(this.Name, "dgvDisposeRows_CellContentClick", exMessage);
            }
            #endregion
        }

        private void RebateCalculation(int RowIndex)
        {


            try
            {
                decimal UsedQuantity = 0; //F
                decimal PurchaseQuantity = 0; // H
                decimal VATAmount = 0;
                decimal RemainingQuantity = 0; //F


                decimal RebateRate = 0;
                decimal RebateVATAmount = 0;


                UsedQuantity = Convert.ToDecimal(dgvDisposeFinish["UsedQuantity", RowIndex].Value);  //F
                PurchaseQuantity = Convert.ToDecimal(dgvDisposeFinish["PurchaseQuantity", RowIndex].Value); // H
                VATAmount = Convert.ToDecimal(dgvDisposeFinish["VATAmount", RowIndex].Value);
                VATAmount = Convert.ToDecimal(Program.FormatingNumeric(VATAmount.ToString(), ReceivePlaceQty));


                if (PurchaseQuantity <= UsedQuantity)
                {
                    RebateRate = 100;
                }
                else
                {
                    RebateRate = UsedQuantity * 100 / PurchaseQuantity;

                    RebateRate = Convert.ToDecimal(Program.FormatingNumeric(RebateRate.ToString(), ReceivePlaceQty));
                }

                RebateVATAmount = VATAmount * RebateRate / 100;

                RebateVATAmount = Convert.ToDecimal(Program.FormatingNumeric(RebateVATAmount.ToString(), ReceivePlaceQty));

                dgvDisposeFinish["RebateRate", RowIndex].Value = RebateRate;
                dgvDisposeFinish["RebateVATAmount", RowIndex].Value = RebateVATAmount;


                RemainingQuantity = UsedQuantity - PurchaseQuantity;

                if (RemainingQuantity > 0)
                {
                    txtUsedQuantity.Text = RemainingQuantity.ToString();
                    txtRemainingQuantity.Text = RemainingQuantity.ToString();

                    string ItemNo = "";
                    ItemNo = dgvDisposeFinish["ItemNo", RowIndex].Value.ToString();

                    cmbItemNo.SelectedValue = ItemNo;
                }
                else
                {
                    txtUsedQuantity.Text = "0";
                    txtRemainingQuantity.Text = "0";
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
                FileLogger.Log(this.Name, "RebateCalculation", exMessage);
            }
            #endregion

        }

        private void btnLoadBOMDetail_Click(object sender, EventArgs e)
        {
            try
            {
                #region Check Point

                if (string.IsNullOrWhiteSpace(txtBOMId.Text) || txtBOMId.Text == "0")
                {
                    MessageBox.Show("Please Select BOM!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBOMId.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtQuantity.Text) || Convert.ToDecimal(txtQuantity.Text) == 0)
                {
                    MessageBox.Show("Please Input Quantity!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQuantity.Focus();
                    return;
                }

                #endregion

                #region Get BOM Details

                int BOMId = 0;
                decimal FinishQuantity = 0;

                BOMId = Convert.ToInt32(txtBOMId.Text);
                FinishQuantity = Convert.ToDecimal(txtQuantity.Text);

                DataTable dtBOMRaws = new DataTable();

                ParameterVM paramVM = new ParameterVM();
                paramVM.selectFields = new[] { "br.BOMId", "p.ItemNo", "p.ProductCode", "p.ProductName", "p.UOM", "br.TotalQuantity", "pc.CategoryName", "pc.IsRaw ProductType" };

                paramVM.TableName = @"
BOMRaws br
left outer join Products p on br.RawItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
";
                paramVM.conditionFields = new[] { "br.BOMId", "br.Post", "p.ActiveStatus" };
                paramVM.conditionValues = new[] { BOMId.ToString(), "Y", "Y" };

                paramVM.AdditionalWhereClause = " and pc.IsRaw in('Raw','Pack') ";

                paramVM.OrderBy = " p.ProductName";
                dtBOMRaws = new CommonDAL().Select(paramVM);

                #endregion

                #region Load Detail Grid

                if (dtBOMRaws != null && dtBOMRaws.Rows.Count > 0)
                {
                    int rowIndex = 0;

                    decimal rawQuantity = 0;
                    decimal rawUsedQuantity = 0;

                    dgvDisposeFinish.DataSource = null;
                    dgvDisposeFinish.Rows.Clear();

                    foreach (DataRow dr in dtBOMRaws.Rows)
                    {

                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvDisposeFinish.Rows.Add(NewRow);


                        dgvDisposeFinish["LineNo", rowIndex].Value = rowIndex+1;
                        dgvDisposeFinish["ItemNo", rowIndex].Value = dr["ItemNo"];
                        dgvDisposeFinish["ItemName", rowIndex].Value = dr["ProductName"];
                        dgvDisposeFinish["UOM", rowIndex].Value = dr["UOM"];

                        rawQuantity = Convert.ToDecimal(dr["TotalQuantity"]);

                        rawUsedQuantity = FinishQuantity * rawQuantity;

                        dgvDisposeFinish["UsedQuantity", rowIndex].Value = rawUsedQuantity;

                        rowIndex++;

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
                FileLogger.Log(this.Name, "btnLoadBOMDetail_Click", exMessage);
            }
            #endregion
        }

        private void txtOfferUnitPrice_Leave(object sender, EventArgs e)
        {
            txtOfferUnitPrice.Text = Program.ParseDecimalObject(txtOfferUnitPrice.Text.Trim()).ToString();
        }

        private void txtUnitPrice_Leave(object sender, EventArgs e)
        {
            txtUnitPrice.Text = Program.ParseDecimalObject(txtUnitPrice.Text.Trim()).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
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
                IsRawParam = ProductCategoryInfo[4];

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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerProductSearchDsFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CommonDAL _CommonDAL = new CommonDAL();
                ParameterVM paramVM = new ParameterVM();


                #region Product



                paramVM.TableName = "Products pro ";
                paramVM.JoinClause = @" LEFT OUTER JOIN ProductCategories pcr ON pro.CategoryID = pcr.CategoryID ";
                paramVM.selectFields = new[] { "pro.ItemNo", "pro.ProductCode", "pro.ProductName" };
                paramVM.conditionFields = new[] { "pcr.IsRaw", "pro.ActiveStatus" };
                paramVM.conditionValues = new[] { IsRawParam, ActiveStatusProductParam };
                paramVM.OrderBy = " pro.ProductName";



                ProductResultDs = _CommonDAL.Select(paramVM, null, null, connVM);


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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Product

                cmbItemNo.DataSource = null;

                DataTable dt = ProductResultDs.Copy();

                #region Add Select Text

                DataRow dr = dt.NewRow();
                dr["ProductName"] = "Select";
                dr["ItemNo"] = "";
                dt.Rows.InsertAt(dr, 0);

                #endregion

                cmbItemNo.DataSource = dt;
                cmbItemNo.DisplayMember = "ProductName";
                cmbItemNo.ValueMember = "ItemNo";

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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }

        }



    }
}
