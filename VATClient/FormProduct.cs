using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATClient.ModelDTO;
//using VATClient.ReportPages;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Drawing;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATServer.Library.Integration;
using VATClient.ReportPages;

namespace VATClient
{
    public partial class FormProduct : Form
    {

        public FormProduct()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //    connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //    connVM.SysPassword = SysDBInfoVM.SysPassword;
            //    connVM.SysUserName = SysDBInfoVM.SysUserName;
            //    connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private List<ProductTypeDTO> ProductTypes = new List<ProductTypeDTO>();
        private List<UomDTO> UOMs = new List<UomDTO>();
        CommonDAL commonDal = new CommonDAL();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private bool ChangeData = false;
        public string VFIN = "113";
        private bool IsUpdate = false;
        private int searchBranchId = 0;
        string vProductNameId = "0";
        string vItemNo = "0";
        public static string vCustomerID = "0";
        private string[] sqlResults;

        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private string itemType = string.Empty;
        //string MyID = "101";
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;

        private string[] ProductCategoryLines;
        private string NextID = string.Empty;
        private string uom = string.Empty;
        private string uom2 = string.Empty;
        private string ReportType = string.Empty;

        private decimal TotalCost = 0;
        private decimal OpeningQty = 0;
        private DataTable ProductNamesResult;
        private DataTable CustomerRateResult;
        private DataTable DtProductStock;
        private DataTable DtProductCustomerRate;
        private string[] results = new string[5];
        string vTDSCode;
        string vTDSDescription;

        private string[] stockResult = new string[4];

        #region Global Variables For BackGroundWorker

        private string ProductCategoryData = string.Empty;
        //private string ProductCategoryResult;
        private DataTable ProductCategoryResult;
        private DataTable uomResult;
        //private string result;

        public string UomData { get; set; }

        #endregion

        #region Serial Track
        private string TrackingTrace = string.Empty;
        private string NoOfHeader = string.Empty;
        private string SHeading1 = string.Empty;
        private string SHeading2 = string.Empty;
        List<TrackingVM> Trackings = new List<TrackingVM>();


        List<string> Headings = new List<string>();
        //List<string> list5 = new List<string>();


        #endregion

        #endregion

        #region Methods

        private void FormProduct_Load(object sender, EventArgs e)
        {

            #region try

            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchProduct, "Existing Information");
                ToolTip1.SetToolTip(this.btnSearchProductCategory, "Product Group");
                ToolTip1.SetToolTip(this.btnAddNew, "New");

                txtProductName.Focus();
                ClearAll();
                //btnAdd.Text = "&Add";
                txtItemNo.Text = "~~~ New ~~~";
                formMaker();
                ProductCategorySearch();
                ChangeData = false;

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, false, connVM);

                string value = new CommonDAL().settingValue("CompanyCode", "Code", connVM);
                btnSync.Visible = false;
                if (OrdinaryVATDesktop.IsACICompany(value) && value.ToLower() != "core cb")
                {
                    //btnSync.Visible = true;
                }
                if (value.ToUpper() == "NESTLE")
                {
                    btnSync.Visible = true;
                    txtShortName.Font = new Font("SutonnyMJ", 11, FontStyle.Regular);
                }

                if (OrdinaryVATDesktop.IsNourishCompany(value))
                {
                    btnSync.Visible = true;
                }

                if (value.ToLower() == "decathlon")
                {
                    btnSync.Visible = true;
                    //txtShortName.Font = new Font("SutonnyMJ", 11, FontStyle.Regular);
                }

                ReportTypeLoad();

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
                FileLogger.Log(this.Name, "FormProduct_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormProduct_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormProduct_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormProduct_Load", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProduct_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProduct_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProduct_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProduct_Load", exMessage);
            }

            #endregion

            #region finally



            #endregion
        }

        private void formMaker()
        {


            gpOpening.Visible = true;
            gpPrice.Visible = true;
            txtTradingMarkUp.Visible = true;
            txtSDRate.Visible = true;
            //LSD.Visible = true;
            lTM.Visible = true;
            txtRebate.Visible = false;
            LRebate.Visible = false;
            txtcrnbrprice.Visible = false;
            txtcustomerid.Visible = false;
            label46.Visible = false;
            label47.Visible = false;
            if (rbtnOther.Checked)
            {
                this.Text = "Product/Item Information";
                ////grbWastage.Visible = true;
                UOMSearch();
            }
            else if (rbtnOverHead.Checked)
            {
                //////gbRate.Visible = false;
                gpOpening.Visible = false;
                gpPrice.Visible = false;
                txtTradingMarkUp.Visible = false;
                //////txtSDRate.Visible = false;
                //////LSD.Visible = false;
                lTM.Visible = false;
                cmbUom.Items.Clear();
                cmbUom.Items.Add("-");
                cmbUom.Text = "-";
                cmbUom2.Items.Clear();
                cmbUom2.Items.Add("-");
                cmbUom2.Text = "-";
                this.Text = "Input Service Information";
                int MW = this.Size.Width;
                int gp = groupBox2.Size.Width;
                groupBox2.Left = (MW / 2) - (gp / 2);
                //groupBox1.Dwon = (MW / 2) - (gp / 2);
                txtRebate.Visible = true;
                LRebate.Visible = true;
                chkBanderol.Visible = false;

                txtCDRate.Visible = false;
                txtRDRate.Visible = false;
                txtATVRate.Visible = false;
                txtTVARate.Visible = false;

                //label11.Visible = false;
                label18.Visible = false;
                //label19.Visible = false;
                //label22.Visible = false;
                label39.Visible = true;

                label13.Text = "Service Code(F9)";

            }
            tabPage2.Visible = false;
            tabControl1.TabPages.Remove(tabPage2);

            string VAT11Name = new CommonDAL().settingsDesktop("Reports", "VAT6_3", null, connVM);
            if (VAT11Name.ToLower() != "scbl")
            {
                tabControl1.TabPages.Remove(tabPage5);

            }


            #region Settings
            CommonDAL commonDal = new CommonDAL();

            string vTrackingTrace, vNoOfHeader, vHeading1, vHeading2 = string.Empty;

            vTrackingTrace = commonDal.settingsDesktop("TrackingTrace", "Tracking", null, connVM);
            vNoOfHeader = commonDal.settingsDesktop("TrackingTrace", "TrackingNo", null, connVM);
            vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", null, connVM);
            vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", null, connVM);

            string IsPharmaceutical = commonDal.settingsDesktop("Products", "IsPharmaceutical", null, connVM);
            string ExtraFeildVisibilty = commonDal.settingsDesktop("Menu", "ExtraRequiredField", null, connVM);
            string ExpireDateTraking = commonDal.settingsDesktop("Purchase", "ExpireDateTracking", null, connVM);
            //  string CodeUpdate = commonDal.settings("SettingsMaster", "CodeUpdate", null, connVM);

            if (ExpireDateTraking == "Y")
            {
                lblTransactionholddate.Visible = true;
                txtTransactionholddate.Visible = true;
            }
            else
            {
                lblTransactionholddate.Visible = false;
                txtTransactionholddate.Visible = false;

            }


            grbPharmaceutical.Visible = IsPharmaceutical == "Y";

            string AutoCode = commonDal.settingsDesktop("AutoCode", "Item", null, connVM);

            if (string.IsNullOrEmpty(vTrackingTrace)
                   || string.IsNullOrEmpty(vNoOfHeader)
                   || string.IsNullOrEmpty(vHeading1)
                   || string.IsNullOrEmpty(vHeading2)
                   )
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

            TrackingTrace = vTrackingTrace;
            NoOfHeader = vNoOfHeader;
            SHeading1 = vHeading1;
            SHeading2 = vHeading2;

            #region Tracking Trace

            if (TrackingTrace == "Y")
            {
                tabControl1.TabPages.Add(tabPage2);
                tabPage2.Visible = true;

                lblHeading1.Text = SHeading1.ToString();
                lblHeading2.Text = SHeading2.ToString();
                dgvSerialTrack.Columns["Heading1"].HeaderText = SHeading1.ToString();
                dgvSerialTrack.Columns["Heading2"].HeaderText = SHeading2.ToString();
            }
            else
            {
                tabControl1.TabPages.Remove(tabPage2);
                tabPage2.Visible = false;

            }

            #endregion Tracking Trace
            #endregion

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = commonDal.settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            #region read only for AutoCode
            if (AutoCode == "Y")
            {
                txtPCode.ReadOnly = true;
                label33.Visible = false;

            }
            else
            {
                label33.Visible = true;
            }
            #endregion

            #region ExtraRequiredFeildVisibilty
            if (ExtraFeildVisibilty == "N")
            {
                chkBanderol.Visible = false;
                chkTollProduct.Visible = false;
                chkTransport.Visible = false;
                chkIsHouseRent.Visible = false;
                chkTrading.Visible = false;
                chkIsConfirmed.Visible = false;
                label8.Visible = false;
                txtSerialNo.Visible = false;
                label11.Visible = false;
                txtShortName.Visible = false;
                label24.Visible = false;
                txtVATRate2.Visible = false;
            }
            #endregion

            #region TDS Lisence Issue
            if (Program.IsTDS == false)
            {
                label26.Visible = false;
                txtTDSCode.Visible = false;
            }
            #endregion

            #region Bandroll Lisence Issue
            if (Program.IsBandroll == false)
            {
                chkBanderol.Visible = false;
            }
            #endregion

            #region Trading Lisence Issue
            if (Program.IsTrading == false)
            {
                chkTrading.Visible = false;
            }
            #endregion

            #region Trading Lisence Issue
            if (Program.IsTollClient == false && Program.IsTollContractor)
            {
                chkTollProduct.Visible = false;
            }
            #endregion
            string value = new CommonDAL().settingValue("CompanyCode", "Code", connVM);
            if (value.ToUpper() == "NESTLE")
            {
                gbRate.Visible = false;
                chkBanderol.Visible = false;
                chkTollProduct.Visible = false;
                chkTransport.Visible = false;
                chkIsHouseRent.Visible = false;
                chkTrading.Visible = true;
                chkIsConfirmed.Visible = false;
                label8.Visible = false;
                txtSerialNo.Visible = false;
                label11.Visible = false;
                label24.Visible = false;
                txtVATRate2.Visible = false;
                chkIsVDS.Visible = false;
                lblHPSRate.Visible = false;
                txtHPSRate.Visible = false;

            }

            dtpPriceHistoryEffectDate.Value=DateTime.Now;

        }

        private int ErrorReturn()
        {
            #region try

            try
            {
                if (txtCostPrice.Text == "")
                {
                    txtCostPrice.Text = "0.00";
                }
                if (string.IsNullOrWhiteSpace(txtTollOpeningQuantity.Text))
                {
                    txtCostPrice.Text = "0.00";
                }
                ////if (string.IsNullOrWhiteSpace(txtWastageTotalQuantity.Text))
                ////{
                ////    txtWastageTotalQuantity.Text = "0.00";
                ////}
                ////if (string.IsNullOrWhiteSpace(txtWastageTotalValue.Text))
                ////{
                ////    txtWastageTotalValue.Text = "0.00";
                ////}

                if (txtSalesPrice.Text == "")
                {
                    txtSalesPrice.Text = "0.00";
                }
                if (txtTollCharge.Text == "")
                {
                    txtTollCharge.Text = "0.00";
                }

                if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }
                if (txtRebate.Text == "")
                {
                    txtRebate.Text = "0.00";
                }

                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                if (txtSDRate.Text == "")
                {
                    txtSDRate.Text = "0.00";
                }
                if (txtShortName.Text.Trim() == "")
                {
                    txtShortName.Text = "-";
                }
                if (txtPacketPrice.Text == "")
                {
                    txtPacketPrice.Text = "0.00";
                }
                if (txtOpeningBalance.Text == "")
                {
                    txtOpeningBalance.Text = "0.00";
                }
                if (txtHSDescription.Text == "")
                {
                    txtHSDescription.Text = "-";
                }

                if (txtGenericName.Text == "")
                {
                    txtGenericName.Text = "-";
                }
                if (txtDARNo.Text == "")
                {
                    txtDARNo.Text = "-";
                }

                if (string.IsNullOrWhiteSpace(txtConvertion.Text.Trim()))
                {
                    txtConvertion.Text = "1";
                }
                else
                {
                    txtConvertion.Text = "1";
                }
                if (cmbUom.Text.Trim() == cmbUom2.Text.Trim())
                {

                    if (Convert.ToDecimal(txtConvertion.Text) != 1)
                    {
                        txtConvertion.Text = "1";
                    }
                }
                //else
                //{
                //    if (Convert.ToDecimal(txtConvertion.Text) == 0)
                //    {
                //        MessageBox.Show("Please enter UOM Conversion.");
                //        txtConvertion.Focus();
                //        return 1;
                //    }
                //}
                if (txtProductName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter product Name.");
                    txtProductName.Focus();
                    return 1;
                }
                else if (txtVATRate.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter product VAT Rate.");
                    txtVATRate.Focus();
                    return 1;
                }
                else if (cmbUom.Text.Trim() == "" || cmbUom.Text.Trim().ToLower() == "select")
                {
                    MessageBox.Show("Please Select Unit of Mesurment.");
                    cmbUom.Focus();
                    return 1;
                }


                //else if (cmbUom.Text.Trim() != cmbUom2.Text.Trim())
                //{
                //    if (txtConvertion.Text == "0")
                //    {
                //        MessageBox.Show("Please Inter UOM Conversion.");
                //        txtConvertion.Focus();
                //        return 1;
                //    }

                //}
                else if (cmbUom.Text.Trim() == cmbUom2.Text.Trim())
                {
                    if (txtConvertion.Text == "0")
                    {
                        txtConvertion.Text = "1";
                    }

                }
                else if (cmbProductCategoryName.Text.Trim() == ""
                    || cmbProductCategoryName.Text.Trim().ToLower() == "select"
                    )
                {
                    MessageBox.Show("Please Select Item Group.");
                    cmbProductCategoryName.Focus();
                    return 1;
                }
                else if (txtOpeningBalance.Text == "")
                {
                    txtOpeningBalance.Text = "0.00";
                }
                else if (txtNBRPrice.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter NRB Price.");
                    txtNBRPrice.Focus();
                    return 1;
                }
                else if (txtCostPrice.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter product Cost Price.");
                    txtCostPrice.Focus();
                    return 1;
                }
                else if (txtSalesPrice.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter product Sales price .");
                    txtSalesPrice.Focus();
                    return 1;
                }
                else if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                else if (txtProductDescription.Text == "")
                {
                    txtProductDescription.Text = "-";
                }
                else if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                else if (rbtnOther.Checked)
                {
                    if (txtHSCode.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Product HS Code.");
                        txtHSCodeNo.Focus();
                        return 1;
                    }
                    //////else
                    //////{
                    //////    txtHSCode.Text = "NA";
                    //////}
                }
                if (rbtnOverHead.Checked)
                {
                    if (txtHSCode.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Service Code.");
                        txtHSCodeNo.Focus();
                        return 1;
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }

            #endregion

            return 0;
        }

        private void ClearAll()
        {
            txtCostPrice.Text = "0.00";

            txtWastageTotalQuantity.Clear();
            txtWastageTotalValue.Clear();
            dtpOpeningDate.Value = Program.SessionDate;
            txtOpeningBalance.Enabled = true;
            txtCostPrice.Enabled = true;
            dtpOpeningDate.Enabled = true;
            chkNonStock.Checked = false;
            chkTrading.Checked = false;
            chkActiveStatus.Checked = true;
            txtItemNo.Text = "";
            txtProductName.Text = "";
            txtProductDescription.Text = "";
            txtCategoryID.Text = "";
            txtAITRate.Text = "0";
            txtRebate.Text = "0";
            txtTradingMarkUp.Text = "0";
            txtNBRPrice.Text = "0";
            txtSerialNo.Text = "";
            txtHSCodeNo.Text = "";
            txtVATRate.Text = "0";
            txtComments.Text = "";
            txtCategoryName.Text = "";
            txtHSDescription.Text = "";
            txtOpeningBalance.Text = "0";
            txtTotalCost.Text = "0";
            txtTollCharge.Text = "0";
            txtSDRate.Text = "0";
            txtPacketPrice.Text = "0";
            txtImpSDRate.Text = "0";
            txtimpVATRate.Text = "0";
            txtCDRate.Text = "0";
            txtRDRate.Text = "0";
            txtTVARate.Text = "0";
            txtATVRate.Text = "0";
            txtVATRate2.Text = "0";
            txtConvertion.Text = "0";
            chkIsHouseRent.Checked = false;
            txtShortName.Text = "";
            //cmbUom.SelectedIndex = 0;
            txtHSCode.Text = "";
            txtCostPriceInput.Text = "0";

            #region Conditional Values

            if (cmbUom.SelectedIndex != -1)
            {
                //cmbUom.SelectedIndex = 0;
                cmbUom.Text = "Select";
            }
            if (cmbUom2.SelectedIndex != -1)
            {
                //cmbUom.SelectedIndex = 0;
                cmbUom2.Text = "Select";
            }
            if (cmbProductCategoryName.SelectedIndex != -1)
            {
                cmbProductCategoryName.Text = "Select";
            }

            if (cmbProductType.SelectedIndex != -1)
            {
                cmbProductType.Text = "";
            }

            string AutoCode = commonDal.settingsDesktop("AutoCode", "Item", null, connVM);
            if (AutoCode == "Y")
            {
                txtPCode.ReadOnly = true;
                label33.Visible = false;

            }

            btnCodeUpdate.Visible = false;


            #endregion

            txtPCode.Text = "";
            txtVATRate.Text = "0";
            chkBanderol.Checked = false;
            txtHeading1.Text = "";
            txtHeading2.Text = "";
            chkHeading1.Checked = false;
            chkHeading2.Checked = false;
            chkExempted.Checked = false;
            chkZeroVAT.Checked = false;
            chkFixedVAT.Checked = false;
            chkIsFixedVATRebate.Checked = false;
            chkSD.Checked = false;
            chkRD.Checked = false;
            chkCD.Checked = false;
            chkAIT.Checked = false;
            chkVAT.Checked = false;
            chkAT.Checked = false;
            chkOtherSD.Checked = false;
            txtFixedVATAmount.Text = "0";
            txtTDSCode.Text = "";
            chkIsVDS.Checked = false;
            txtHPSRate.Text = "0";
            txtDARNo.Text = "";
            txtGenericName.Text = "";

            #region Grid View Clear

            dgvSerialTrack.Rows.Clear();
            dgvProductNames.Rows.Clear();
            dgvProductStock.Rows.Clear();

            #endregion


        }

        private void ReportShow()
        {
            #region try

            try
            {
                //string ReportData =

                //    txtItemNo.Text.Trim() + FieldDelimeter +
                //    FieldDelimeter +
                //    FieldDelimeter +
                //    FieldDelimeter +

                //    LineDelimeter;


                //Thread.Sleep(1000);
                //FormReport Reports = new FormReport(); //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //Reports.crystalReportViewer1.Refresh();
                //Reports.crystalReportViewer1.ReportSource = @"c:\report\ProductGrid.rpt";
                //ParameterField CompanyName1 = new ParameterField();
                //CompanyName1.Name = "CompanyName";
                //ParameterDiscreteValue CompanyNameValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                //CompanyNameValue.Value = DBConstant.CompanyName;
                //CompanyName1.CurrentValues.Add(CompanyNameValue);
                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyName1);

                //ParameterField CompanyAddress1 = new ParameterField();
                //CompanyAddress1.Name = "CompanyAddress";
                //ParameterDiscreteValue CompanyAddressValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                //CompanyAddressValue.Value = DBConstant.CompanyAddress;
                //CompanyAddress1.CurrentValues.Add(CompanyAddressValue);
                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyAddress1);

                //ParameterField CompanyNumber1 = new ParameterField();
                //CompanyNumber1.Name = "CompanyNumber";
                //ParameterDiscreteValue CompanyNumberValue = new ParameterDiscreteValue(); // CrystalDecisions.Shared.
                //CompanyNumberValue.Value = DBConstant.CompanyContactNumber;
                //CompanyNumber1.CurrentValues.Add(CompanyNumberValue);
                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyNumber1);
                //Reports.Text = "Product";
                //Reports.Show();
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
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ReportShow", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", exMessage);
            }

            #endregion
        }

        private void ReportTypeLoad()
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicVATType = new Dictionary<string, string>();
            cmbReportType.DataSource = null;
            cmbReportType.Items.Clear();

            dicVATType = commonDal.ReportType();

            if (dicVATType != null && dicVATType.Count > 0)
            {
                cmbReportType.DataSource = new BindingSource(dicVATType, null);
                cmbReportType.DisplayMember = "Key";
                cmbReportType.ValueMember = "Value";
            }

        }



        #endregion

        #region UOM Method

        private void btnUOMSearch_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                Program.fromOpen = "Other";

                //string result = FormUOMSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] UOMInfo = result.Split(FieldDelimeter.ToCharArray());
                //    cmbUom.Text = UOMInfo[1];
                //}
                DataGridViewRow selectedRow = null;
                selectedRow = FormUOMNameSearch.SelectOne();

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    cmbUom.Text = selectedRow.Cells["UOMName"].Value.ToString();
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
                FileLogger.Log(this.Name, "btnUOMSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUOMSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUOMSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnUOMSearch_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOMSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOMSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOMSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOMSearch_Click", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }

        }

        private void UOMSearch()
        {
            #region try

            try
            {

                #region COmmented kodz

                #endregion

                this.btnUOMSearch.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerUOMSearch.RunWorkerAsync();
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

            #endregion
        }

        private void backgroundWorkerUOMSearch_DoWork(object sender, DoWorkEventArgs e)
        {

            #region Try

            try
            {

                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);
                uomResult = uomdal.SearchUOMCodeOnly("Y", connVM);
                //end do work
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
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_DoWork", exMessage);
            }

            #endregion

        }

        private void backgroundWorkerUOMSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                //start complete
                UOMs.Clear();

                cmbUom.Items.Clear();
                cmbUom2.Items.Clear();

                if (uomResult != null)
                {
                    foreach (DataRow item2 in uomResult.Rows)
                    {
                        cmbUom.Items.Add(item2["UOMCode"].ToString());

                    }
                }

                cmbUom.Items.Insert(0, "Select");
                cmbUom.SelectedIndex = 0;
                if (uomResult != null)
                {
                    foreach (DataRow item2 in uomResult.Rows)
                    {
                        cmbUom2.Items.Add(item2["UOMCode"].ToString());

                    }
                }

                cmbUom2.Items.Insert(0, "Select");
                cmbUom2.SelectedIndex = 0;
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
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerUOMSearch_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.btnUOMSearch.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        #endregion

        #region Product Category

        private void btnSearchProductCategory_Click(object sender, EventArgs e)
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
                else //if (result == ""){return;}else//if (result != "")
                {
                    //ProductCategorySearch();
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtCategoryID.Text = ProductCategoryInfo[0];
                    txtCategoryName.Text = ProductCategoryInfo[1];
                    cmbProductCategoryName.Text = ProductCategoryInfo[1];
                    txtHSDescription.Text = ProductCategoryInfo[9];
                    txtHSCode.Text = ProductCategoryInfo[9];

                    txtSDRate.Text = Convert.ToDecimal(ProductCategoryInfo[10]).ToString("0.00");
                    cmbProductType.Text = ProductCategoryInfo[4];
                    //if (ProductCategoryInfo[4] == "R")
                    //{
                    //    rdbRaw.Checked = true;
                    //}
                    //else
                    //{
                    //    rdbFinished.Checked = true;
                    //}

                    txtHSCodeNo.Text = ProductCategoryInfo[5];
                    txtVATRate.Text = Convert.ToDecimal(ProductCategoryInfo[6]).ToString("0.00");
                    if (ProductCategoryInfo[11] == "Y")
                    {
                        chkTrading.Checked = true;
                    }
                    else
                    {

                        chkTrading.Checked = false;
                    }
                    if (ProductCategoryInfo[12] == "Y")
                    {
                        chkNonStock.Checked = true;
                    }
                    else
                    {
                        chkNonStock.Checked = false;
                    }
                    //txtComments.Text = CustomerGroupinfo[3];  
                }
                //
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click",
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

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click",
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void ProductCategorySearch()
        {
            #region try

            try
            {
                this.btnSearchProductCategory.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerProductGroupSearch.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategorySearch", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerProductGroupSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();

                IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);
                string[] cValues = new string[] { "0", "0", "Y", "0" };
                string[] cFields = new string[] { "VATRate>=", "VATRate<=", "ActiveStatus like", "SD" };
                ProductCategoryResult = productCategoryDal.SelectAll(0, cFields, cValues, null, null, false, "", connVM);

                //ProductCategoryResult = productCategoryDal.SearchProductCategory("", "", "", "", 0, 0, "Y", 0, "", "",
                //                                                                 Program.DatabaseName);

                // end do
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
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerProductGroupSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                if (rbtnOverHead.Checked)
                {
                    ProductCategoryResult = ProductCategoryResult.Select("IsRaw = 'Overhead'").CopyToDataTable();
                }
                else
                {
                    ProductCategoryResult = ProductCategoryResult.Select("IsRaw <> 'Overhead'").CopyToDataTable();
                }

                var prodCategories = (from DataRow row in ProductCategoryResult.Rows
                                      select row["CategoryName"].ToString()).ToList();

                if (prodCategories != null && prodCategories.Any())
                {
                    cmbProductCategoryName.Items.Clear();
                    cmbProductCategoryName.Items.AddRange(prodCategories.ToArray());
                }
                cmbProductCategoryName.Items.Insert(0, "Select");
                cmbProductCategoryName.SelectedIndex = 0;

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
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerProductGroupSearch_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.btnSearchProductCategory.Enabled = true;
            this.progressBar1.Visible = false;

        }

        private void cmbProductCategoryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProductCategoryName.SelectedIndex != -1)
            {
                ProductCategoryDetailsInfo();
                ChangeData = true;
            }
        }

        private void ProductCategoryDetailsInfo()
        {
            #region try

            try
            {
                cmbProductType.Items.Clear();
                foreach (DataRow row in ProductCategoryResult.Rows)
                {
                    if (row[1].ToString() == cmbProductCategoryName.Text.Trim())
                    {
                        txtCategoryID.Text = row["CategoryID"].ToString();
                        txtCategoryName.Text = row["CategoryName"].ToString();
                        cmbProductType.Text = row["Israw"].ToString();
                        //txtHSCodeNo.Text = ProductCategoryFields[5].ToString();
                        //txtHSCode.Text = ProductCategoryFields[9].ToString();
                        txtVATRate.Text = Convert.ToDecimal(row["VATRate"]).ToString("0.00");
                        //txtHSDescription.Text = ProductCategoryFields[9].ToString();
                        txtSDRate.Text = Convert.ToDecimal(row["SD"]).ToString("0.00");
                        if (row["Israw"].ToString() == "Overhead")
                        {
                            LRebate.Visible = true;
                            txtRebate.Visible = true;
                            chkNonStock.Checked = true;
                            chkTrading.Checked = false;
                        }
                        else
                        {
                            LRebate.Visible = false;
                            txtRebate.Visible = false;
                        }
                        if (row["Trading"].ToString() == "Y")
                        {
                            chkTrading.Checked = true;
                        }
                        else
                        {
                            chkTrading.Checked = false;
                        }
                        if (row["NonStock"].ToString() == "Y")
                        {
                            chkNonStock.Checked = true;
                        }
                        else
                        {
                            chkNonStock.Checked = false;
                        }
                        return;
                    }


                }

            #endregion

                #region catch
            }
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryDetailsInfo", exMessage);
            }

                #endregion
        }

        #endregion

        #region Methods 01

        private void btnAdd_Click(object sender, EventArgs e)
        {

            #region try

            try
            {
                #region CheckPoint

                //ExistProduct();
                if (!string.IsNullOrEmpty(ExistProduct()))
                {
                    if (MessageBox.Show("Product already exist. Do you want to save?", this.Text, MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        //this.progressBar1.Visible = false;
                        //this.btnAdd.Enabled = true;
                        return;
                    }
                }
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (rbtnOverHead.Checked)
                {
                    if (cmbProductType.Text.Trim() != "Overhead")
                    {
                        MessageBox.Show("Input service must overhead Type, Please select Appropriate group for this", this.Text);
                        return;
                    }
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtItemNo.Text.Trim();
                }
                if (cmbUom2.Text.Trim() == "" || cmbUom2.Text.Trim().ToLower() == "select")
                {
                    cmbUom2.Text = cmbUom.Text;
                }
                #endregion

                string ProductData = string.Empty;

                string data = string.Empty; //Change 02

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                #region Tracking

                if (TrackingTrace == "Y")
                {
                    if (dgvSerialTrack.Rows.Count == 0 && Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information not added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,

                         MessageBoxButtons.YesNo,

                         MessageBoxIcon.Question,

                         MessageBoxDefaultButton.Button2))
                        {
                            tabControl1.SelectedTab = tabPage2;
                            return;

                        }

                    }

                }

                #endregion

                itemType = string.Empty;
                if (rbtnOther.Checked)
                {
                    itemType = "Other";
                }
                else if (rbtnOverHead.Checked)
                {
                    itemType = "Overhead";

                }
                this.progressBar1.Visible = true;
                this.btnAdd.Enabled = false;
                uom = string.Empty;
                uom = cmbUom.Text.Trim();
                uom2 = string.Empty;
                uom2 = cmbUom2.Text.Trim();

                ReportType = cmbReportType.Text.Trim();
                if (ReportType == "Select" || string.IsNullOrWhiteSpace(ReportType))
                {
                    ReportType = "-";
                }
                backgroundWorkerAdd.RunWorkerAsync();

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

        private string ExistProduct()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            string proMessage = productDal.GetExistingProductName(txtProductName.Text, connVM);
            return proMessage;
        }

        private int DataAlreadyUsed()
        {
            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);
                if (commonDal.DataAlreadyUsed("BOMCompanyOverhead", "FinishItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " BOM" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                if (commonDal.DataAlreadyUsed("BOMCompanyOverhead", "HeadID", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " BOM" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                if (commonDal.DataAlreadyUsed("BOMRaws", "FinishItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " BOM" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("BOMRaws", "RawItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " BOM" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("BOMs", "FinishItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " BOM" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("IssueDetails", "ItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Issue" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("IssueDetails", "FinishItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Issue" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("ReceiveDetails", "ItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Receive" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("DutyDrawBackHeader", "FgItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " DDB" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("DutyDrawBackDetails", "FgItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " DDB" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("DutyDrawBackDetails", "ItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " DDB" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("PriceService", "ItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Service Price Declaration" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("PurchaseInvoiceDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("PurchaseInvoiceDuties", "ItemNo", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("TenderDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("DisposeDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Dispose" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("SalesInvoiceDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Sale" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("PurchaseInvoiceDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                else if (commonDal.DataAlreadyUsed("PurchaseInvoiceDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                if (commonDal.DataAlreadyUsed("PurchaseInvoiceDetails", "itemno", txtItemNo.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }

            #endregion

            return 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsUpdate == false)
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                else if (txtItemNo.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }
                else if (txtItemNo.Text.Trim() == "ovh0" || txtItemNo.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                     MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
                //if (MessageBox.Show("Do you want to delete data?", "Product", MessageBoxButtons.YesNo) != DialogResult.Yes)
                else if (
                    MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                int ErR = DataAlreadyUsed();
                if (ErR != 0)
                {
                    return;
                }
                this.btnDelete.Enabled = false;
                progressBar1.Visible = true;

                backgroundWorkerDelete.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }

            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            #region try

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
                        ProductCategorySearch();
                        txtProductName.Focus();
                        ClearAll();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
                    ProductCategorySearch();
                    txtProductName.Focus();
                    ClearAll();
                    ChangeData = false;
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

            #endregion


        }

        #region TextBox KeyDown Event

        private void txtItemNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            //if (e.KeyCode.Equals(Keys.Alt) & e.KeyCode.Equals(Keys.L)) 
            //{ MessageBox.Show("Alt+L"); }
            ////e.KeyCode = Keys.ControlKey & Keys.L;

        }

        private void txtHSCodeNo_KeyDown(object sender, KeyEventArgs e)
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

        private void txtHSDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtUOM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtNBRPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCostPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");

            }


        }

        private void txtSalesPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtProductDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        #endregion

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {

            ChangeData = true;
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHSCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnPrintList_Click(object sender, EventArgs e)
        {
            //FormRptProductInformation frmRptProductInformation = new FormRptProductInformation();
            //frmRptProductInformation.Show();
        }

        private void btnPrintGrid_Click(object sender, EventArgs e)
        {
            ReportShow();
        }

        // no server side method
        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            #region try

            try
            {

                #region Static Values

                Program.fromOpen = "Me";
                Program.R_F = "";
                Program.ItemType = "Other";


                if (rbtnOverHead.Checked)
                {
                    Program.ItemType = "Overhead";
                }


                #endregion

                #region Selected Row

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormProductSearch.SelectOne();

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();

                    ProdutSearch();

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
                FileLogger.Log(this.Name, "btnSearchProduct_Click", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        #endregion

        #region Methods 02

        private void ProdutSearch()
        {
            try
            {
                #region Declarations
                ProductDAL _pdal = new ProductDAL();

                //IProduct _pdal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductVM pvm = new ProductVM();

                #endregion

                #region Product Data Call

                pvm = _pdal.SelectAll(txtItemNo.Text.Trim(), null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                #region Value Assign to Form Elements

                txtProductName.Text = pvm.ProductName;
                txtPCode.Text = pvm.ProductCode;
                txtSearchProductCode.Text = pvm.ProductCode;
                txtProductDescription.Text = pvm.ProductDescription;
                txtCategoryID.Text = pvm.CategoryID;
                txtCategoryName.Text = pvm.CategoryName;
                cmbProductCategoryName.Text = pvm.CategoryName;
                cmbUom.Text = pvm.UOM;
                cmbUom2.Text = pvm.UOM2;
                txtConvertion.Text = Program.ParseDecimalObject(pvm.UOMConversion);
                txtSalesPrice.Text = Program.ParseDecimalObject(pvm.SalesPrice.ToString());
                txtNBRPrice.Text = Program.ParseDecimalObject(pvm.NBRPrice.ToString());
                cmbProductType.Text = pvm.IsRaw;
                txtSerialNo.Text = pvm.SerialNo;
                txtHSCodeNo.Text = pvm.HSCodeNo;
                txtVATRate.Text = Program.ParseDecimalObject(pvm.VATRate.ToString());
                txtVDSRate.Text = Program.ParseDecimalObject(pvm.VDSRate.ToString());
                chkActiveStatus.Checked = pvm.ActiveStatus.ToString() == "Y" ? true : false;
                txtOpeningBalance.Text = Program.ParseDecimalObject(pvm.OpeningBalance.ToString());
                txtCostPriceInput.Text = Program.ParseDecimalObject(pvm.OpeningTotalCost.ToString());

                #region Conditional Values

                if (Convert.ToDecimal(txtOpeningBalance.Text) == 0)
                {
                    txtCostPrice.Text = "0";
                }
                else
                {
                    txtCostPrice.Text = Convert.ToDecimal(
                        Convert.ToDecimal(txtCostPriceInput.Text)
                        / Convert.ToDecimal(txtOpeningBalance.Text)
                        ).ToString(); //6

                }

                #endregion

                txtComments.Text = pvm.Comments;
                txtHSDescription.Text = pvm.HSCodeNo;
                txtHSCode.Text = pvm.HSCodeNo;
                txtSDRate.Text = Program.ParseDecimalObject(pvm.SD.ToString());
                txtPacketPrice.Text = Program.ParseDecimalObject(pvm.Packetprice.ToString());
                txtTradingMarkUp.Text = pvm.TradingMarkUp.ToString();
                dtpOpeningDate.Value = Convert.ToDateTime(pvm.OpeningDate);
                txtTollCharge.Text = Program.ParseDecimalObject(pvm.TollCharge.ToString());
                txtRebate.Text = Program.ParseDecimalObject(pvm.RebatePercent.ToString());
                txtCDRate.Text = Program.ParseDecimalObject(pvm.CDRate.ToString());
                txtRDRate.Text = Program.ParseDecimalObject(pvm.RDRate.ToString());
                txtTVARate.Text = Program.ParseDecimalObject(pvm.TVARate.ToString());
                txtATVRate.Text = Program.ParseDecimalObject(pvm.ATVRate.ToString());
                txtVATRate2.Text = Program.ParseDecimalObject(pvm.VATRate2.ToString());
                txtVATRate3.Text = Program.ParseDecimalObject(pvm.TradingSaleVATRate.ToString());
                txtSDRate1.Text = Program.ParseDecimalObject(pvm.TradingSaleSD.ToString());
                searchBranchId = pvm.BranchId;
                txtShortName.Text = pvm.ShortName;
                chkTrading.Checked = pvm.Trading.ToString() == "Y" ? true : false;
                chkNonStock.Checked = pvm.NonStock.ToString() == "Y" ? true : false;
                chkBanderol.Checked = pvm.Banderol.ToString() == "Y" ? true : false;
                chkExempted.Checked = pvm.IsExempted.ToString() == "Y" ? true : false;
                chkZeroVAT.Checked = pvm.IsZeroVAT.ToString() == "Y" ? true : false;
                chkTollProduct.Checked = pvm.TollProduct.ToString() == "Y" ? true : false;
                chkTransport.Checked = pvm.IsTransport.ToString() == "Y" ? true : false;
                chkFixedVAT.Checked = pvm.IsFixedVAT.ToString() == "Y" ? true : false;
                chkSD.Checked = pvm.IsFixedSD.ToString() == "Y" ? true : false;
                chkCD.Checked = pvm.IsFixedCD.ToString() == "Y" ? true : false;
                chkRD.Checked = pvm.IsFixedRD.ToString() == "Y" ? true : false;
                chkAIT.Checked = pvm.IsFixedAIT.ToString() == "Y" ? true : false;
                chkVAT.Checked = pvm.IsFixedVAT1.ToString() == "Y" ? true : false;
                chkAT.Checked = pvm.IsFixedAT.ToString() == "Y" ? true : false;
                chkOtherSD.Checked = pvm.IsFixedOtherSD.ToString() == "Y" ? true : false;
                chkIsHouseRent.Checked = pvm.IsHouseRent == "Y" ? true : false;
                chkIsConfirmed.Checked = pvm.IsConfirmed == "Y";
                txtFixedVATAmount.Text = Program.ParseDecimalObject(pvm.FixedVATAmount.ToString());
                txtAITRate.Text = Program.ParseDecimalObject(pvm.AITRate.ToString());
                txtImpSDRate.Text = Program.ParseDecimalObject(pvm.SDRate.ToString());
                txtimpVATRate.Text = Program.ParseDecimalObject(pvm.VATRate3.ToString());
                txtTDSCode.Text = pvm.TDSCode;
                txtHPSRate.Text = Program.ParseDecimalObject(pvm.HPSRate.ToString());
                chkIsVDS.Checked = pvm.IsVDS.ToString() == "Y";
                txtDARNo.Text = pvm.DARNo;
                txtGenericName.Text = pvm.GenericName;
                cmbReportType.Text = pvm.ReportType;
                txtTransactionholddate.Text = pvm.TransactionHoldDate.ToString();
                txtCostPrice.Text = Program.ParseDecimalObject(pvm.CostPrice);
                txtPacketPrice.Text = Program.ParseDecimalObject(pvm.Packetprice);
                chkIsFixedVATRebate.Checked = pvm.IsFixedVATRebate.ToString() == "Y";
                chkIsSample.Checked = pvm.IsSample.ToString() == "Y";
                txtTollOpeningQuantity.Text = Program.ParseDecimalObject(pvm.TollOpeningQuantity.ToString());

                ////txtWastageTotalQuantity.Text = Program.ParseDecimalObject(pvm.WastageTotalQuantity.ToString());
                ////txtWastageTotalValue.Text = Program.ParseDecimalObject(pvm.WastageTotalValue.ToString());

                #endregion

                #region Product Name Function

                fnProductNames(vItemNo);

                #endregion

                #region Product Stock Function

                SelectProductStock(vItemNo);

                #endregion

                #region Product Mapping Function

                ProductMapVM pVM = new ProductMapVM();
                pVM.ItemNo = vItemNo;
                SelectProductMapDetails(pVM);

                #endregion

                #region Product Mapping Function

                ProductPriceHistoryVM PriceHistoryVM = new ProductPriceHistoryVM();
                PriceHistoryVM.ItemNo = vItemNo;
                SelectProductPriceHistory(PriceHistoryVM);

                #endregion

                #region Product Customer Rate Function
                SelectProductCustomerRate(vItemNo);
                #endregion

                #region Tracking
                if (TrackingTrace == "Y")
                {
                    Trackings.Clear();
                    InsertTrackingInfo(txtItemNo.Text);
                }
                #endregion

                #region Flag Update

                IsUpdate = true;
                ChangeData = false;
                ////txtPCode.ReadOnly = true;
                #endregion
                string CodeUpdate = commonDal.settingsMaster("Product", "CodeUpdate");

                if (CodeUpdate == "Y")
                {
                    txtPCode.ReadOnly = false;
                    btnCodeUpdate.Visible = true;
                }
                //else
                //{
                //    btnCodeUpdate.Visible = false;
                //}

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
                FileLogger.Log(this.Name, "ProdutSearch", exMessage);
            }
            #endregion Catch
        }

        private void cmbProductCategoryName_TextChanged(object sender, EventArgs e)
        {
            //ChangeTrue();
            ChangeData = true;
            ProductCategoryDetailsInfo();
        }

        private void txtCategoryID_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSalesPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPacketPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSDRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTradingMarkUp_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtNBRPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCostPrice_TextChanged(object sender, EventArgs e)
        {
            //txtTotalCost.Text =Convert.ToDecimal(Convert.ToDecimal(txtCostPrice.Text)*Convert.ToDecimal(txtOpeningBalance.Text)).ToString("0.00");
            ChangeData = true;
        }

        private void txtOpeningBalance_TextChanged(object sender, EventArgs e)
        {
            ///txtTotalCost.Text = Convert.ToDecimal(Convert.ToDecimal(txtCostPrice.Text) * Convert.ToDecimal(txtOpeningBalance.Text)).ToString("0.00");
            ChangeData = true;
        }

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtProductDescription_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 03

        private void FormProduct_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtUOM_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductType_TextChanged(object sender, EventArgs e)
        {
            ChangeTrue();
            ChangeData = true;
        }
        private void ChangeTrue()
        {
            txtVATRate3.Text = "0";
            txtSDRate1.Text = "0";

            if (cmbProductType.Text == "Trading")
            {
                txtVATRate3.Visible = true;
                txtSDRate1.Visible = true;
                label44.Visible = true;
                label45.Visible = true;
            }
            else
            {
                txtVATRate3.Visible = false;
                txtSDRate1.Visible = false;
                label44.Visible = false;
                label45.Visible = false;
            }
        }

        private void cmbProductCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void HSCodeLoad()
        {
            try
            {

                //if (string.IsNullOrEmpty(vItemNo) || vItemNo == "0")
                //{
                //    MessageBox.Show("Please select the product  first");
                //    return;
                //}
                DataGridViewRow selectedRow = new DataGridViewRow();


                string SqlText = @"  select 

 Id
,Code
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

                string[] shortColumnName = { "Code", "HSCode", "Description", "CD", "RD", "SD", "VAT", "AT", "AIT", "OtherSD", "OtherVAT", "IsFixedVAT", "IsFixedSD", "IsFixedCD", "IsFixedRD", "IsFixedAIT", "IsFixedAT", "IsFixedOtherVAT", "IsFixedOtherSD", "IsVDS" };
                string tableName = "";

                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtProductDescription.Text = "";
                    txtProductDescription.Text = selectedRow.Cells["Description"].Value.ToString();
                    txtHSCode.Text = "";
                    txtHSCode.Text = selectedRow.Cells["HSCode"].Value.ToString();
                    txtCDRate.Text = "";
                    txtCDRate.Text = selectedRow.Cells["CD"].Value.ToString();
                    txtImpSDRate.Text = "";
                    txtImpSDRate.Text = selectedRow.Cells["SD"].Value.ToString();
                    txtimpVATRate.Text = "";
                    txtimpVATRate.Text = selectedRow.Cells["VAT"].Value.ToString();
                    txtAITRate.Text = "";
                    txtAITRate.Text = selectedRow.Cells["AIT"].Value.ToString();
                    txtRDRate.Text = "";
                    txtRDRate.Text = selectedRow.Cells["RD"].Value.ToString();
                    txtATVRate.Text = "";
                    txtATVRate.Text = selectedRow.Cells["AT"].Value.ToString();
                    txtSDRate.Text = "";
                    txtSDRate.Text = Program.FormatingNumeric(selectedRow.Cells["OtherSD"].Value.ToString(), 2);
                    chkSD.Checked = selectedRow.Cells["IsFixedSD"].Value.ToString() == "Y" ? true : false;
                    chkCD.Checked = selectedRow.Cells["IsFixedCD"].Value.ToString() == "Y" ? true : false;
                    chkRD.Checked = selectedRow.Cells["IsFixedRD"].Value.ToString() == "Y" ? true : false;
                    chkAIT.Checked = selectedRow.Cells["IsFixedAIT"].Value.ToString() == "Y" ? true : false;
                    chkVAT.Checked = selectedRow.Cells["IsFixedVAT"].Value.ToString() == "Y" ? true : false;
                    chkAT.Checked = selectedRow.Cells["IsFixedAT"].Value.ToString() == "Y" ? true : false;

                    chkFixedVAT.Checked = selectedRow.Cells["IsFixedOtherVAT"].Value.ToString() == "Y" ? true : false;
                    chkOtherSD.Checked = selectedRow.Cells["IsFixedOtherSD"].Value.ToString() == "Y" ? true : false;
                    if (chkFixedVAT.Checked)
                    {
                        txtFixedVATAmount.Text = "";
                        txtFixedVATAmount.Text = Program.FormatingNumeric(selectedRow.Cells["OtherVAT"].Value.ToString(), 2);
                        txtVATRate.Text = "0";
                    }
                    else
                    {
                        txtVATRate.Text = "";
                        txtVATRate.Text = Program.FormatingNumeric(selectedRow.Cells["OtherVAT"].Value.ToString(), 2);
                        txtFixedVATAmount.Text = "0";
                    }
                    chkOtherSD.Checked = selectedRow.Cells["IsFixedOtherSD"].Value.ToString() == "Y" ? true : false;

                    chkIsVDS.Checked = selectedRow.Cells["IsVDS"].Value.ToString() == "Y";

                    txtHSCode.Focus();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "HSCodeLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

        private void txtHSCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                HSCodeLoad();
            }
        }

        private void txtSDRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtTradingMarkUp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }

            //txtNBRPrice.Focus();
        }

        private void txtPacketPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtUOM_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtOpeningBalance_KeyDown(object sender, KeyEventArgs e)
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
                txtCostPriceInput.Focus();
            }
        }

        #endregion

        #region Methods 04

        private void txtQIH_TextChanged(object sender, EventArgs e)
        {
            //if (Convert.ToDecimal(txtQIH.Text) != 0)
            //{
            //    txtOpeningBalance.Enabled = false;
            //    txtCostPrice.Enabled = false;
            //    dtpOpeningDate.Enabled = false;
            //}
            //else
            //{
            //    txtOpeningBalance.Enabled = true;
            //    txtCostPrice.Enabled = true;
            //    dtpOpeningDate.Enabled = true;
            //}

        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {
                        ProductCategorySearch();
                        txtProductName.Focus();
                        ClearAll();
                        //btnAdd.Text = "&Add";
                        txtItemNo.Text = "~~~ New ~~~";

                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    ProductCategorySearch();
                    txtProductName.Focus();
                    ClearAll();
                    //btnAdd.Text = "&Add";
                    txtItemNo.Text = "~~~ New ~~~";
                    ChangeData = false;

                    dgvProductStock.Rows.Clear();
                }

                IsUpdate = false;

                txtPCode.ReadOnly = false;


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

        private void chkTrading_CheckedChanged(object sender, EventArgs e)
        {


        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkNonStock_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkTrading_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (chkTrading.Checked == true)
                {
                    if (chkNonStock.Checked == true)
                    {

                        MessageBox.Show("Product should be either Trading or NonStock");
                        chkTrading.Checked = false;
                        return;
                    }
                }


                if (chkTrading.Checked == false)
                {
                    txtTradingMarkUp.Text = "0.00";
                    txtTradingMarkUp.Enabled = false;
                }
                if (chkTrading.Checked == true)
                {
                    //txtTradingMarkUp.Text = "0.00";
                    txtTradingMarkUp.Enabled = true;
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
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
            }

            #endregion
        }

        private void chkNonStock_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (chkNonStock.Checked == true)
                {
                    if (chkTrading.Checked == true)
                    {
                        MessageBox.Show("Product should be either Trading or NonStock");
                        chkNonStock.Checked = false;
                        return;
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
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
            }

            #endregion
        }

        private void txtItemNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                FormRptProductInformation frmRptProductInformation = new FormRptProductInformation();
                frmRptProductInformation.ShowDialog();
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

        private void txtVATRate_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                this.txtVATRate.SelectAll();
                //alreadyFocused = true;
            }

        }

        private void chkAutoCode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
               
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (rbtnOverHead.Checked)
                {
                    if (cmbProductType.Text.Trim() != "Overhead")
                    {
                        MessageBox.Show("Input service must overhead Type, Please select Appropriate group for this", this.Text);
                        return;
                    }
                }
                if (txtItemNo.Text.Trim() == "ovh0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
                if (cmbUom2.Text.Trim() == "" || cmbUom2.Text.Trim().ToLower() == "select")
                {
                    cmbUom2.Text = cmbUom.Text;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtItemNo.Text.Trim();
                }

                string ProductData = string.Empty;

                string data = string.Empty; //Change 02

                #region Null Check


                if (txtCostPrice.Text == "")
                {
                    txtCostPrice.Text = "0.00";
                }

                if (txtSalesPrice.Text == "")
                {
                    txtSalesPrice.Text = "0.00";
                }
                if (string.IsNullOrEmpty(txtRebate.Text.Trim()))
                {
                    txtRebate.Text = "0.00";
                }
                if (txtTollCharge.Text == "")
                {
                    txtTollCharge.Text = "0.00";
                }

                if (txtNBRPrice.Text == "")
                {
                    txtNBRPrice.Text = "0.00";
                }

                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }
                if (txtSDRate.Text == "")
                {
                    txtSDRate.Text = "0.00";
                }
                if (txtPacketPrice.Text == "")
                {
                    txtPacketPrice.Text = "0.00";
                }
                if (txtOpeningBalance.Text == "")
                {
                    txtOpeningBalance.Text = "0.00";
                }
                if (txtHPSRate.Text == "")
                {
                    txtHPSRate.Text = "0.00";
                }
                if (cmbUom2.Text.Trim() == "" || cmbUom2.Text.Trim().ToLower() == "select")
                {
                    cmbUom2.Text = cmbUom.Text;
                }
                #endregion

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                #region Tracking

                if (TrackingTrace == "Y")
                {
                    if (dgvSerialTrack.Rows.Count == 0 && Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information not added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,

                         MessageBoxButtons.YesNo,

                         MessageBoxIcon.Question,

                         MessageBoxDefaultButton.Button2))
                        {
                            tabControl1.SelectedTab = tabPage2;
                            return;

                        }

                    }
                    ////else if (Convert.ToDecimal(txtOpeningBalance.Text) > dgvSerialTrack.Rows.Count || Convert.ToDecimal(txtOpeningBalance.Text) < dgvSerialTrack.Rows.Count)
                    ////{
                    ////    MessageBox.Show("Please enter total tracking quantity (" + Convert.ToDecimal(txtOpeningBalance.Text) + ")", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ////    return;
                    ////}

                }

                #endregion

                if (string.IsNullOrEmpty(NextID))
                {

                }
                else
                {

                    this.progressBar1.Visible = true;
                    this.btnUpdate.Enabled = false;
                    itemType = string.Empty;
                    if (rbtnOther.Checked)
                    {
                        itemType = "Other";
                    }
                    else if (rbtnOverHead.Checked)
                    {
                        itemType = "Overhead";

                    }
                    ////if (searchBranchId != Program.BranchId && searchBranchId != 0)
                    ////{
                    ////    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ////    return;
                    ////}
                    uom = string.Empty;
                    uom = cmbUom.Text.Trim();
                    uom2 = string.Empty;
                    uom2 = cmbUom2.Text.Trim();
                    ReportType = cmbReportType.Text.Trim();

                    backgroundWorkerUpdate.RunWorkerAsync();
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

        #region backgroundWorker Events

        private void backgroundWorkerAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                #region Tracking
                Trackings = new List<TrackingVM>();
                if (Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                {

                    decimal trackCost = 0;
                    trackCost = Convert.ToDecimal(txtCostPriceInput.Text) / Convert.ToDecimal(txtOpeningBalance.Text);
                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = txtItemNo.Text;
                        trackingVm.TrackingLineNo = dgvSerialTrack.Rows[i].Cells["LineNoS"].Value.ToString();
                        trackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                        trackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();
                        trackingVm.Quantity = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["QuantityS"].Value.ToString());
                        trackingVm.UnitPrice = trackCost;
                        trackingVm.IsIssue = "N";
                        trackingVm.IsReceive = "N";
                        trackingVm.IsSale = "N";
                        trackingVm.IsPurchase = "N";

                        Trackings.Add(trackingVm);

                    }
                }
                #endregion Tracking

                #region Product DAL

                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                #endregion

                #region Product VM

                ProductVM vm = new ProductVM();

                vm.ItemNo = itemType.ToString();
                vm.ProductCode = txtPCode.Text.Trim();
                vm.ProductName = txtProductName.Text.Trim();
                vm.ProductDescription = txtProductDescription.Text.Trim();
                vm.CategoryID = txtCategoryID.Text.Trim();
                vm.UOM = uom.ToString();
                vm.UOM2 = uom2.ToString();
                vm.UOMConversion = Convert.ToDecimal(txtConvertion.Text.Trim());
                vm.CostPrice = Convert.ToDecimal(txtCostPrice.Text.Trim());
                vm.SalesPrice = Convert.ToDecimal(txtSalesPrice.Text.Trim());
                vm.NBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());
                vm.TollCharge = Convert.ToDecimal(txtTollCharge.Text.Trim());
                vm.OpeningBalance = Convert.ToDecimal(txtOpeningBalance.Text.Trim());
                vm.SerialNo = txtSerialNo.Text.Trim();
                vm.HSCodeNo = txtHSCode.Text.Trim();
                vm.VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.SD = Convert.ToDecimal(txtSDRate.Text.Trim());
                vm.Packetprice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                vm.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                vm.TradingMarkUp = Convert.ToDecimal(txtTradingMarkUp.Text.Trim());
                vm.NonStock = Convert.ToString(chkNonStock.Checked ? "Y" : "N");
                vm.RebatePercent = Convert.ToDecimal(txtRebate.Text.Trim());
                vm.OpeningTotalCost = Convert.ToDecimal(TotalCost);
                vm.OpeningDate = dtpOpeningDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.Banderol = Convert.ToString(chkBanderol.Checked ? "Y" : "N");
                vm.CDRate = Convert.ToDecimal(txtCDRate.Text.Trim());
                vm.RDRate = Convert.ToDecimal(txtRDRate.Text.Trim());
                vm.TVARate = Convert.ToDecimal(txtTVARate.Text.Trim());
                vm.ATVRate = Convert.ToDecimal(txtATVRate.Text.Trim());
                vm.VATRate2 = Convert.ToDecimal(txtVATRate2.Text.Trim());
                vm.TradingSaleVATRate = Convert.ToDecimal(txtVATRate3.Text.Trim());
                vm.TradingSaleSD = Convert.ToDecimal(txtSDRate1.Text.Trim());
                vm.VDSRate = Convert.ToDecimal(txtVDSRate.Text.Trim());
                vm.TollProduct = Convert.ToString(chkTollProduct.Checked ? "Y" : "N");
                vm.IsExempted = Convert.ToString(chkExempted.Checked ? "Y" : "N");
                vm.IsZeroVAT = Convert.ToString(chkZeroVAT.Checked ? "Y" : "N");
                vm.IsTransport = Convert.ToString(chkTransport.Checked ? "Y" : "N");
                vm.BranchId = Program.BranchId;
                vm.IsFixedVAT = Convert.ToString(chkFixedVAT.Checked ? "Y" : "N");
                vm.FixedVATAmount = Convert.ToDecimal(txtFixedVATAmount.Text.Trim());
                vm.TDSCode = txtTDSCode.Text.Trim();
                vm.AITRate = Convert.ToDecimal(txtAITRate.Text.Trim());
                vm.SDRate = Convert.ToDecimal(txtImpSDRate.Text.Trim());
                vm.VATRate3 = Convert.ToDecimal(txtimpVATRate.Text.Trim());
                vm.IsFixedSD = Convert.ToString(chkSD.Checked ? "Y" : "N");
                vm.IsFixedCD = Convert.ToString(chkCD.Checked ? "Y" : "N");
                vm.IsFixedRD = Convert.ToString(chkRD.Checked ? "Y" : "N");
                vm.IsFixedAIT = Convert.ToString(chkAIT.Checked ? "Y" : "N");
                vm.IsFixedVAT1 = Convert.ToString(chkVAT.Checked ? "Y" : "N");
                vm.IsFixedAT = Convert.ToString(chkAT.Checked ? "Y" : "N");
                vm.IsFixedOtherSD = Convert.ToString(chkOtherSD.Checked ? "Y" : "N");
                vm.OpeningTotalCost = Convert.ToDecimal(txtCostPriceInput.Text);
                vm.IsVDS = Convert.ToString(chkIsVDS.Checked ? "Y" : "N");
                vm.HPSRate = Convert.ToDecimal(txtHPSRate.Text.Trim());
                vm.IsHouseRent = Convert.ToString(chkIsHouseRent.Checked ? "Y" : "N");
                vm.IsConfirmed = chkIsConfirmed.Checked ? "Y" : "N";
                vm.ShortName = txtShortName.Text.Trim();
                vm.GenericName = txtGenericName.Text.Trim();
                vm.DARNo = txtDARNo.Text.Trim();
                vm.ReportType = ReportType;
                vm.TransactionHoldDate = Convert.ToDecimal(txtTransactionholddate.Text.Trim());
                vm.IsFixedVATRebate = chkIsFixedVATRebate.Checked ? "Y" : "N";
                vm.IsSample = chkIsSample.Checked ? "Y" : "N";
                vm.TollOpeningQuantity = Convert.ToDecimal(txtTollOpeningQuantity.Text.Trim());

                ////vm.WastageTotalQuantity = Convert.ToDecimal(txtWastageTotalQuantity.Text.Trim());
                ////vm.WastageTotalValue = Convert.ToDecimal(txtWastageTotalValue.Text.Trim());

                #endregion

                #region Insert To Product

                sqlResults = productDal.InsertToProduct(vm, null, null, false, null, null, connVM);

                #endregion

                #region Product Stock VM

                ProductStockVM psVM = new ProductStockVM();
                psVM.BranchId = Program.BranchId;

                psVM.ItemNo = sqlResults[2];
                psVM.StockValue = Convert.ToDecimal(txtCostPriceInput.Text);
                psVM.StockQuantity = Convert.ToDecimal(txtOpeningBalance.Text);
                psVM.Comments = txtComments.Text;

                #endregion

                #region Insert To Product Stock

                stockResult = productDal.InserToProductStock(psVM, null, null, connVM, Program.CurrentUserID);

                #endregion


                SAVE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerAdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted", "Unexpected error.");
                        }

                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                            IsUpdate = true;
                            txtItemNo.Text = newId;
                            txtPCode.Text = code;
                            fnProductNames(txtItemNo.Text.Trim());

                            SelectProductStock(newId);

                            if (result == "Success")
                            {
                                txtPCode.ReadOnly = true;
                            }


                        }

                    }
                //btnAdd.Text = "&Save";

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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }

            #endregion

            this.progressBar1.Visible = false;
            this.btnAdd.Enabled = true;
        }

        private void fnProductNames(string itemNo)
        {
            try
            {


                ProductNamesResult = new DataTable();

                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductNamesResult = productDal.SearchProductNames(itemNo, "", "", connVM); // Change 04

                dgvProductNames.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ProductNamesResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductNames.Rows.Add(NewRow);


                    dgvProductNames.Rows[j].Cells["Sl"].Value = (j + 1).ToString();
                    dgvProductNames.Rows[j].Cells["Id"].Value = item["Id"].ToString();
                    dgvProductNames.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvProductNames.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                    j = j + 1;

                }
                txtProductNames.Text = "";
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
                FileLogger.Log(this.Name, "fnProductNamess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "fnProductNamess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "fnProductNamess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "fnProductNamess", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "fnProductNamess", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "fnProductNamess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "fnProductNamess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "fnProductNamess", exMessage);
            }
            #endregion
        }

        private void fnProductStock(string ItemNo)
        {
            try
            {

                DataTable ProductStockResult = new DataTable();
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductStockResult = productDal.SearchProductStock(ItemNo, "", connVM); // Change 04

                dgvProductStock.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ProductStockResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductStock.Rows.Add(NewRow);

                    //dgvProductStock.Rows[j].Cells["Sl"].Value = (j + 1).ToString();
                    dgvProductStock.Rows[j].Cells["StockId"].Value = item["Id"].ToString();
                    dgvProductStock.Rows[j].Cells["StockQuantity"].Value = item["StockQuantity"].ToString();
                    dgvProductStock.Rows[j].Cells["StockValue"].Value = item["StockValue"].ToString();
                    dgvProductStock.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvProductStock.Rows[j].Cells["BranchName"].Value = item["BranchName"].ToString();
                    dgvProductStock.Rows[j].Cells["StockItemNo"].Value = item["ItemNo"].ToString();
                    dgvProductStock.Rows[j].Cells["WastageTotalQuantity"].Value = item["WastageTotalQuantity"].ToString();

                    j = j + 1;

                }
                txtProductStockQuantity.Text = "";
                txtProductStockValue.Text = "";
                textProductStockComments.Text = "";
                txtStockId.Text = "";
                txtWastageTotalQuantity.Text = "";
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
                FileLogger.Log(this.Name, "fnProductStock", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                ProductVM vm = new ProductVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString();
                string[] ids = new string[] { txtItemNo.Text.Trim(), "" };
                sqlResults = productDal.Delete(vm, ids, null, null, connVM);
                //sqlResults = productDal.DeleteProduct(txtItemNo.Text.Trim()); // Change 04
                DELETE_DOWORK_SUCCESS = true;
            }
            #endregion

            #region catch

            catch (ArgumentNullException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork",
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (DELETE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string recId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerDelete_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }


                IsUpdate = false;
                ClearAll();
                dgvSerialTrack.Rows.Clear();
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted",
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                #region Tracking
                Trackings = new List<TrackingVM>();
                if (Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                {
                    decimal trackCost = 0;
                    trackCost = Convert.ToDecimal(txtCostPriceInput.Text) / Convert.ToDecimal(txtOpeningBalance.Text);
                    for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = txtItemNo.Text;
                        trackingVm.TrackingLineNo = dgvSerialTrack.Rows[i].Cells["LineNoS"].Value.ToString();
                        trackingVm.Heading1 = dgvSerialTrack.Rows[i].Cells["Heading1"].Value.ToString();
                        trackingVm.Heading2 = dgvSerialTrack.Rows[i].Cells["Heading2"].Value.ToString();
                        trackingVm.Quantity = Convert.ToDecimal(dgvSerialTrack.Rows[i].Cells["QuantityS"].Value.ToString());
                        trackingVm.UnitPrice = trackCost;
                        trackingVm.IsIssue = "N";
                        trackingVm.IsReceive = "N";
                        trackingVm.IsSale = "N";
                        trackingVm.IsPurchase = "N";


                        Trackings.Add(trackingVm);

                    }

                }

                if (TrackingTrace == "Y")
                {
                    //TrackingDAL trackingDal = new TrackingDAL();
                    ITracking trackingDal = OrdinaryVATDesktop.GetObject<TrackingDAL, TrackingRepo, ITracking>(OrdinaryVATDesktop.IsWCF);
                    string result = trackingDal.TrackingDelete(Headings, connVM);
                }

                #endregion Tracking

                #region ProductDAL

                ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                #endregion

                #region Product VM

                ProductVM vm = new ProductVM();

                vm.ItemNo = NextID.ToString();
                vm.ProductCode = txtPCode.Text.Trim();
                vm.ProductName = txtProductName.Text.Trim();
                vm.ProductDescription = txtProductDescription.Text.Trim();
                vm.CategoryID = txtCategoryID.Text.Trim();
                vm.UOM = uom.ToString();
                vm.UOM2 = uom2.ToString();
                vm.UOMConversion = Convert.ToDecimal(txtConvertion.Text.Trim());
                vm.CostPrice = Convert.ToDecimal(txtCostPrice.Text.Trim());
                vm.SalesPrice = Convert.ToDecimal(txtSalesPrice.Text.Trim());
                vm.NBRPrice = Convert.ToDecimal(txtNBRPrice.Text.Trim());
                vm.TollCharge = Convert.ToDecimal(txtTollCharge.Text.Trim());
                vm.OpeningBalance = Convert.ToDecimal(txtOpeningBalance.Text.Trim());
                vm.SerialNo = txtSerialNo.Text.Trim();
                vm.HSCodeNo = txtHSCode.Text.Trim();
                vm.VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.SD = Convert.ToDecimal(txtSDRate.Text.Trim());
                vm.Packetprice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                vm.Trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                vm.TradingMarkUp = Convert.ToDecimal(txtTradingMarkUp.Text.Trim());
                vm.NonStock = Convert.ToString(chkNonStock.Checked ? "Y" : "N");
                vm.RebatePercent = Convert.ToDecimal(txtRebate.Text.Trim());
                //vm.OpeningTotalCost = Convert.ToDecimal(TotalCost);
                vm.OpeningTotalCost = Convert.ToDecimal(txtCostPriceInput.Text.Trim());
                vm.OpeningDate = dtpOpeningDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////////vm.OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.Banderol = Convert.ToString(chkBanderol.Checked ? "Y" : "N");
                vm.CDRate = Convert.ToDecimal(txtCDRate.Text.Trim());
                vm.RDRate = Convert.ToDecimal(txtRDRate.Text.Trim());
                vm.TVARate = Convert.ToDecimal(txtTVARate.Text.Trim());
                vm.ATVRate = Convert.ToDecimal(txtATVRate.Text.Trim());
                vm.VATRate2 = Convert.ToDecimal(txtVATRate2.Text.Trim());
                vm.TradingSaleVATRate = Convert.ToDecimal(txtVATRate3.Text.Trim());
                vm.TradingSaleSD = Convert.ToDecimal(txtSDRate1.Text.Trim());
                vm.VDSRate = Convert.ToDecimal(txtVDSRate.Text.Trim());
                vm.TollProduct = Convert.ToString(chkTollProduct.Checked ? "Y" : "N");
                vm.IsExempted = Convert.ToString(chkExempted.Checked ? "Y" : "N");
                vm.IsZeroVAT = Convert.ToString(chkZeroVAT.Checked ? "Y" : "N");
                vm.IsTransport = Convert.ToString(chkTransport.Checked ? "Y" : "N");
                vm.IsFixedVAT = Convert.ToString(chkFixedVAT.Checked ? "Y" : "N");
                vm.FixedVATAmount = Convert.ToDecimal(txtFixedVATAmount.Text.Trim());
                vm.TDSCode = txtTDSCode.Text.Trim();
                vm.AITRate = Convert.ToDecimal(txtAITRate.Text.Trim());
                vm.SDRate = Convert.ToDecimal(txtImpSDRate.Text.Trim());
                vm.VATRate3 = Convert.ToDecimal(txtimpVATRate.Text.Trim());
                vm.IsFixedSD = Convert.ToString(chkSD.Checked ? "Y" : "N");
                vm.IsFixedCD = Convert.ToString(chkCD.Checked ? "Y" : "N");
                vm.IsFixedRD = Convert.ToString(chkRD.Checked ? "Y" : "N");
                vm.IsFixedAIT = Convert.ToString(chkAIT.Checked ? "Y" : "N");
                vm.IsFixedVAT1 = Convert.ToString(chkVAT.Checked ? "Y" : "N");
                vm.IsFixedAT = Convert.ToString(chkAT.Checked ? "Y" : "N");
                vm.IsFixedOtherSD = Convert.ToString(chkOtherSD.Checked ? "Y" : "N");
                vm.IsVDS = Convert.ToString(chkIsVDS.Checked ? "Y" : "N");
                vm.IsConfirmed = Convert.ToString(chkIsConfirmed.Checked ? "Y" : "N");
                vm.HPSRate = Convert.ToDecimal(txtHPSRate.Text.Trim());
                vm.IsHouseRent = Convert.ToString(chkIsHouseRent.Checked ? "Y" : "N");
                vm.ShortName = txtShortName.Text.Trim();
                vm.GenericName = txtGenericName.Text.Trim();
                vm.DARNo = txtDARNo.Text.Trim();
                vm.ReportType = ReportType;
                vm.TransactionHoldDate = Convert.ToDecimal(txtTransactionholddate.Text.Trim());
                vm.IsFixedVATRebate = chkIsFixedVATRebate.Checked ? "Y" : "N";
                vm.IsSample = chkIsSample.Checked ? "Y" : "N";
                vm.TollOpeningQuantity = Convert.ToDecimal(txtTollOpeningQuantity.Text.Trim());

                //vm.IsCodeUpdate = Convert.ToString(chkcodeUpdate.Checked ? "Y" : "N");

                ////vm.WastageTotalQuantity = Convert.ToDecimal(txtWastageTotalQuantity.Text.Trim());
                ////vm.WastageTotalValue = Convert.ToDecimal(txtWastageTotalValue.Text.Trim());

                #endregion

                #region Update Product


                sqlResults = productDal.UpdateProduct(vm, new List<TrackingVM>(), null, connVM);

                #endregion

                UPDATE_DOWORK_SUCCESS = true;
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
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            txtItemNo.Text = newId;
                            txtPCode.Text = code;

                        }

                    }
                //btnAdd.Text = "&Save";
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }

        }

        #endregion

        #endregion

        #region Methods 05

        private void txtTollCharge_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTollCharge_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void chkPerUnitCost_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPerUnitCost.Checked)
            {
                chkPerUnitCost.Text = "Total Cost";
            }
            else
            {
                chkPerUnitCost.Text = "Per Unit Cost";
            }

        }

        private void txtCostPriceInput_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRebate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCostPriceInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtPCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRebate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void btnAddS_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                #region Statement
                if (Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                {

                    if (CountQuantity())
                    {
                        MessageBox.Show("Added all quantity.");
                        return;
                    }
                    decimal totalQty = 0;
                    totalQty = Convert.ToDecimal(txtOpeningBalance.Text);
                    if (chkHeading1.Checked == false && chkHeading2.Checked == false)
                    {
                        dgvSerialTrack.Rows.Add();
                        dgvSerialTrack[0, dgvSerialTrack.Rows.Count - 1].Value = dgvSerialTrack.Rows.Count;
                        dgvSerialTrack["Heading1", dgvSerialTrack.Rows.Count - 1].Value = txtHeading1.Text;
                        dgvSerialTrack["Heading2", dgvSerialTrack.Rows.Count - 1].Value = txtHeading2.Text;
                        dgvSerialTrack["QuantityS", dgvSerialTrack.Rows.Count - 1].Value = "1";
                        dgvSerialTrack["StatusS", dgvSerialTrack.Rows.Count - 1].Value = "Y";

                    }
                    else
                    {
                        for (int i = 0; i < totalQty; i++)
                        {
                            dgvSerialTrack.Rows.Add();

                            dgvSerialTrack[0, dgvSerialTrack.Rows.Count - 1].Value = dgvSerialTrack.Rows.Count;
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
                        }
                    }
                }
                #endregion

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

        private bool CountQuantity()
        {
            decimal TotalQty = Convert.ToDecimal(txtOpeningBalance.Text);
            decimal qty = 0;
            bool result = false;

            try
            {
                for (int i = 0; i < dgvSerialTrack.Rows.Count; i++)
                {
                    qty = qty + 1;
                    if (qty == TotalQty)
                    {
                        result = true;
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

            return result;
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

        private void btnChangeS_Click(object sender, EventArgs e)
        {
            dgvSerialTrack["Heading1", dgvSerialTrack.CurrentRow.Index].Value = txtHeading1.Text;
            dgvSerialTrack["Heading2", dgvSerialTrack.CurrentRow.Index].Value = txtHeading2.Text;
            dgvSerialTrack["QuantityS", dgvSerialTrack.CurrentRow.Index].Value = "1";
            dgvSerialTrack["StatusS", dgvSerialTrack.CurrentRow.Index].Value = "Y";

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

                txtHeading1.Text = dgvSerialTrack.CurrentRow.Cells["Heading1"].Value.ToString();
                txtHeading2.Text = dgvSerialTrack.CurrentRow.Cells["Heading2"].Value.ToString();
                #endregion Statement

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

        #endregion

        #region Methods 06

        private void InsertTrackingInfo(string itemNo)
        {
            #region Try
            try
            {
                //TrackingDAL trackingDal = new TrackingDAL();
                ITracking trackingDal = OrdinaryVATDesktop.GetObject<TrackingDAL, TrackingRepo, ITracking>(OrdinaryVATDesktop.IsWCF);
                DataTable trackingInfoDt = new DataTable();
                trackingInfoDt = trackingDal.SearchTrackings(itemNo, connVM);
                if (trackingInfoDt.Rows.Count > 0)
                {
                    for (int i = 0; i < trackingInfoDt.Rows.Count; i++)
                    {
                        TrackingVM trackingVm = new TrackingVM();
                        trackingVm.ItemNo = trackingInfoDt.Rows[i]["ItemNo"].ToString();
                        trackingVm.Heading1 = trackingInfoDt.Rows[i]["Heading1"].ToString();
                        trackingVm.Heading2 = trackingInfoDt.Rows[i]["Heading2"].ToString();
                        trackingVm.Quantity = Convert.ToDecimal(trackingInfoDt.Rows[i]["Quantity"].ToString());
                        trackingVm.UnitPrice = Convert.ToDecimal(trackingInfoDt.Rows[i]["UnitPrice"].ToString());


                        Trackings.Add(trackingVm);
                    }
                }
                #region Statement



                foreach (DataRow item2 in trackingInfoDt.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSerialTrack.Rows.Add(NewRow);
                    dgvSerialTrack["LineNoS", dgvSerialTrack.RowCount - 1].Value = dgvSerialTrack.RowCount;
                    //dgvSerialTrack["ProductCodeS", dgvSerialTrack.RowCount - 1].Value = item2["ProductCode"].ToString();
                    //dgvSerialTrack["ProductNameS", dgvSerialTrack.RowCount - 1].Value = item2["ProductName"].ToString();
                    //dgvSerialTrack["ItemNoS", dgvSerialTrack.RowCount - 1].Value = item2["ItemNo"].ToString();
                    dgvSerialTrack["Heading1", dgvSerialTrack.RowCount - 1].Value = item2["Heading1"].ToString();
                    dgvSerialTrack["Heading2", dgvSerialTrack.RowCount - 1].Value = item2["Heading2"].ToString();
                    dgvSerialTrack["QuantityS", dgvSerialTrack.RowCount - 1].Value = item2["Quantity"].ToString();
                    //dgvSerialTrack["UnitPrice", dgvSerialTrack.RowCount - 1].Value = item2["UnitPrice"].ToString();

                }

                IsUpdate = true;
                ChangeData = false;

                // End Complete

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

        private void txtHeading1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtHeading1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHeading2_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHeading2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnRemoveS_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                if (dgvSerialTrack.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\n " + SHeading1.ToString() + ": " + dgvSerialTrack.CurrentRow.Cells["Heading1"].Value, this.Text, MessageBoxButtons.YesNo,
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

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = new DataGridViewRow();
            if (rbtnOverHead.Checked)
            {
                Program.ItemType = "Overhead";

            }
            string[] shortColumnName = { "ItemNo", "ProductCode", "ProductName", "ProductDescription", "UOM", "OpeningBalance", "HSCodeNo", "VATRate", "SD", "Trading", "NonStock", "QuantityInHand", "OpeningDate", "RebatePercent", "ActiveStatus", "TDSCode" };
            string tableName = "Products";
            selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
            if (selectedRow != null && selectedRow.Selected == true)
            {
                txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
            }
        }

        private void txtTDSCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void TDSLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select distinct  Id, Code, Description ,MinValue MinValue, MaxValue MaxValue, Rate  from TDSs where 1=1  ";

                string SQLTextRecordCount = @" select count(Code)RecordNo from TDSs";

                string[] shortColumnName = { "Code", "Description" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    vTDSCode = "0";
                    vTDSDescription = "";
                    txtTDSCode.Text = "";
                    vTDSCode = selectedRow.Cells["Code"].Value.ToString();
                    vTDSDescription = selectedRow.Cells["Description"].Value.ToString();//ProductInfo[0];
                    txtTDSCode.Text = vTDSCode;
                    txtTDSCode.Focus();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void txtTDSCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                TDSLoad();
            }
        }

        private void txtTDSCode_DoubleClick(object sender, EventArgs e)
        {
            TDSLoad();

        }

        private void txtTDSCode_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtTDSCode, vTDSDescription);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 07

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Product first");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtProductNames.Text))
                {
                    MessageBox.Show("Please Add Product Name first");
                    txtProductNames.Focus();
                    return;
                }
                sqlResults = new string[4];

                ProductNameVM vm = new ProductNameVM();
                vm.Id = Convert.ToInt32(vProductNameId);
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductName = txtProductNames.Text;

                ProductDAL pDal = new ProductDAL();
                //IProduct pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                sqlResults = pDal.InsertToProductNames(vm, null, null, connVM);
                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }
                    fnProductNames(vm.ItemNo);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductNames.Text))
                {
                    MessageBox.Show("Please Select Product Name first");
                    dgvProductNames.Focus();
                    return;
                }
                sqlResults = new string[4];

                ProductNameVM vm = new ProductNameVM();
                vm.Id = Convert.ToInt32(vProductNameId);

                vm.ItemNo = vItemNo;
                vm.ProductName = txtProductNames.Text;

                ProductDAL pDal = new ProductDAL();
                //IProduct pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                sqlResults = pDal.UpdateToProductNames(vm, null, null, connVM);
                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }
                    fnProductNames(vm.ItemNo);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(txtProductNames.Text))
                {
                    MessageBox.Show("Please Select Product Name first");
                    return;
                }
                sqlResults = new string[4];

                ProductNameVM vm = new ProductNameVM();
                vm.Id = Convert.ToInt32(vProductNameId);
                vm.ItemNo = vItemNo;
                vm.ProductName = txtProductNames.Text;

                ProductDAL cDal = new ProductDAL();
                //IProduct cDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                sqlResults = cDal.DeleteProductNames("", vm.Id.ToString(), null, null, connVM);
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
                    }
                    fnProductNames(vItemNo);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
        }

        private void dgvProductNames_DoubleClick(object sender, EventArgs e)
        {

            DataGridViewSelectedRowCollection selectedRows = dgvProductNames.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                vProductNameId = userSelRow.Cells["Id"].Value.ToString();
                vItemNo = userSelRow.Cells["ItemNo"].Value.ToString();
                txtProductNames.Text = userSelRow.Cells["ProductName"].Value.ToString();
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtHSCode_DoubleClick(object sender, EventArgs e)
        {
            HSCodeLoad();
        }

        private void btnProductStockAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Product first");
                    return;
                }
                string ProductStockQuantity = txtProductStockQuantity.Text;

                string ProductStockValue = txtProductStockValue.Text;
                if (ProductStockQuantity == "" || ProductStockValue == "")
                {
                    if (ProductStockQuantity == "" && ProductStockValue == "")
                    {
                        MessageBox.Show("Entered Stock Quantity & Stock Value first !", this.Text);
                        return;
                    }
                    else if (ProductStockQuantity == "")
                    {
                        MessageBox.Show("Entered a Stock Quantity first !", this.Text);
                        return;
                    }

                    else if (ProductStockValue == "")
                    {
                        MessageBox.Show("Entered a Stock Value first !", this.Text);
                        return;
                    }

                }

                if (string.IsNullOrWhiteSpace(txtWastageTotalQuantity.Text))
                {
                    txtWastageTotalQuantity.Text = "0.00";
                }

                decimal StockQuantity = Convert.ToDecimal(txtProductStockQuantity.Text);
                decimal StockValue = Convert.ToDecimal(txtProductStockValue.Text);

                if (StockQuantity > 0 && StockValue <= 0)
                {
                    MessageBox.Show("Stock Value Can't be Zero !", this.Text);
                    txtProductStockValue.Focus();
                    return;
                }
                else if (StockQuantity <= 0 && StockValue > 0)
                {
                    MessageBox.Show("Stock Quantity Can't be Zero !", this.Text);
                    txtProductStockQuantity.Focus();
                    return;
                }

                if (textProductStockComments.Text == "")
                {
                    textProductStockComments.Text = "NA";
                }


                ProductStockVM psVM = new ProductStockVM();
                psVM.BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                psVM.ItemNo = txtItemNo.Text.Trim();
                psVM.StockQuantity = Convert.ToDecimal(ProductStockQuantity);
                psVM.StockValue = Convert.ToDecimal(ProductStockValue);
                psVM.Comments = textProductStockComments.Text;
                psVM.WastageTotalQuantity = Convert.ToDecimal(txtWastageTotalQuantity.Text);

                ProductDAL pDal = new ProductDAL();
                //IProduct pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                stockResult = pDal.InserToProductStock(psVM, null, null, connVM, Program.CurrentUserID);

                if (stockResult.Length > 0)
                {
                    string result = stockResult[0];
                    string message = stockResult[1];
                    string newId = stockResult[2];

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    if (result == "Success" && psVM.BranchId == 1)
                    {
                        txtCostPriceInput.Text = txtProductStockValue.Text;
                        txtOpeningBalance.Text = txtProductStockQuantity.Text;
                    }
                }

                SelectProductStock(psVM.ItemNo);


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void SelectProductStock(string ItemNo)
        {
            try
            {

                #region Declarations

                DtProductStock = new DataTable();
                ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                IUserBranchDetail userBranch = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);

                #endregion

                #region Data Call

                List<UserBranchDetailVM> vms = userBranch.SelectAllLst(0, new[] { "uf.UserId" }, new[] { Program.CurrentUserID });

                DtProductStock = productDal.SearchProductStock(ItemNo, "", connVM, vms);

                #endregion

                #region Reset Grid

                dgvProductStock.Rows.Clear();

                #endregion

                #region Value Assign to Grid

                int j = 0;
                foreach (DataRow item in DtProductStock.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductStock.Rows.Add(NewRow);

                    dgvProductStock.Rows[j].Cells["StockId"].Value = item["Id"].ToString();
                    dgvProductStock.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                    dgvProductStock.Rows[j].Cells["BranchName"].Value = item["BranchName"].ToString();
                    dgvProductStock.Rows[j].Cells["StockQuantity"].Value = item["StockQuantity"].ToString();
                    dgvProductStock.Rows[j].Cells["StockValue"].Value = item["StockValue"].ToString();
                    dgvProductStock.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvProductStock.Rows[j].Cells["StockItemNo"].Value = item["ItemNo"].ToString();
                    dgvProductStock.Rows[j].Cells["CurrentStock"].Value = item["CurrentStock"].ToString();
                    dgvProductStock.Rows[j].Cells["WastageTotalQuantity"].Value = item["WastageTotalQuantity"].ToString();

                    j = j + 1;

                }

                #endregion

                #region Reset Form Elements

                textProductStockComments.Text = "";
                txtProductStockQuantity.Text = "";
                txtProductStockValue.Text = "";
                txtWastageTotalQuantity.Text = "";
                //txtProductNames.Text = "";

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void SelectProductCustomerRate(string ItemNo)
        {
            try
            {

                #region Declarations

                DtProductCustomerRate = new DataTable();
                //ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                ProductDAL productDal = new ProductDAL();
                IUserBranchDetail userBranch = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);

                #endregion

                #region Data Call

                List<UserBranchDetailVM> vms = userBranch.SelectAllLst(0, new[] { "uf.UserId" }, new[] { Program.CurrentUserID });

                DtProductCustomerRate = productDal.SearchProductCustomerRate(ItemNo, Program.BranchId.ToString(), connVM, vms);

                #endregion

                #region Reset Grid

                dgvProductRate.Rows.Clear();

                #endregion

                #region Value Assign to Grid

                int j = 0;
                foreach (DataRow item in DtProductCustomerRate.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductRate.Rows.Add(NewRow);

                    dgvProductRate.Rows[j].Cells["CustomerId"].Value = item["CustomerId"].ToString();
                    dgvProductRate.Rows[j].Cells["CustomerName"].Value = item["CustomerName"].ToString();
                    // dgvProductRate.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                    dgvProductRate.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();
                    dgvProductRate.Rows[j].Cells["TollCharge"].Value = item["TollCharge"].ToString();
                    //dgvProductRate.Rows[j].Cells["StockValue"].Value = item["StockValue"].ToString();
                    //dgvProductStock.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    //dgvProductStock.Rows[j].Cells["StockItemNo"].Value = item["ItemNo"].ToString();
                    //dgvProductStock.Rows[j].Cells["CurrentStock"].Value = item["CurrentStock"].ToString();
                    //dgvProductStock.Rows[j].Cells["WastageTotalQuantity"].Value = item["WastageTotalQuantity"].ToString();

                    j = j + 1;

                }

                #endregion

                #region Reset Form Elements


                txtcrtollcharge.Text = "";
                txtcrtollcharge.Text = "";
                txtcustomer.Text = "";
                txtcustomerid.Text = "";

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void dgvProductStock_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvProductStock.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                txtStockId.Text = userSelRow.Cells["StockId"].Value.ToString();
                txtProductStockQuantity.Text = userSelRow.Cells["StockQuantity"].Value.ToString();
                txtProductStockValue.Text = userSelRow.Cells["StockValue"].Value.ToString();
                textProductStockComments.Text = userSelRow.Cells["Comments"].Value.ToString();
                txtWastageTotalQuantity.Text = userSelRow.Cells["WastageTotalQuantity"].Value.ToString();
                cmbBranch.Text = userSelRow.Cells["BranchName"].Value.ToString();


            }

        }

        private void btnProductStockRemove_Click(object sender, EventArgs e)
        {
            ProductStockVM psVM = new ProductStockVM();

            DataGridViewSelectedRowCollection selectedRows = dgvProductStock.SelectedRows;


            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                psVM.StockId = Convert.ToInt32(userSelRow.Cells["StockId"].Value);
                psVM.ItemNo = userSelRow.Cells["StockItemNo"].Value.ToString();

                ProductDAL pDal = new ProductDAL();

                //IProduct pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                stockResult = pDal.DeleteToProductStock(psVM, null, null, connVM);

                if (stockResult.Length > 0)
                {
                    string result = stockResult[0];
                    string message = stockResult[1];
                    string newId = stockResult[2];
                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    SelectProductStock(psVM.ItemNo);
                }

            }
            else
            {
                MessageBox.Show("Please Select a Row first");
                return;
            }



        }

        #region Leave Event

        private void txtTradingMarkUp_Leave(object sender, EventArgs e)
        {
            txtNBRPrice.Focus();
            txtTradingMarkUp.Text = Program.ParseDecimalObject(txtTradingMarkUp.Text.Trim()).ToString();

            //Program.FormatTextBoxRate(txtTradingMarkUp, "Trading MarkUp");
        }

        private void txtPacketPrice_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtPacketPrice, "Packet Price");
            txtPacketPrice.Text = Program.ParseDecimalObject(txtPacketPrice.Text.Trim()).ToString();

        }

        private void txtTollCharge_Leave(object sender, EventArgs e)
        {
            txtTollCharge.Text = Program.ParseDecimalObject(txtTollCharge.Text.Trim()).ToString();

            //Program.FormatTextBox(txtTollCharge, "Toll Charge");
        }

        private void txtVATRate2_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtVATRate2, "VATRate2");
            txtVATRate2.Text = Program.ParseDecimalObject(txtVATRate2.Text.Trim()).ToString();
            Program.CheckVATRate(txtVATRate2, "VAT Rate Input Box");

        }

        private void txtFixedVATAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtFixedVATAmount, "FixedVATAmount");
            txtFixedVATAmount.Text = Program.ParseDecimalObject(txtFixedVATAmount.Text.Trim()).ToString();
        }

        private void txtVDSRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtVDSRate, "VDSRate");
            txtVDSRate.Text = Program.ParseDecimalObject(txtVDSRate.Text.Trim()).ToString();
        }

        private void txtCDRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtCDRate, "CDRate");
            txtCDRate.Text = Program.ParseDecimalObject(txtCDRate.Text.Trim()).ToString();
        }

        private void txtRDRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtRDRate, "RDRate");
            txtRDRate.Text = Program.ParseDecimalObject(txtRDRate.Text.Trim()).ToString();
        }

        private void txtImpSDRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtImpSDRate, "ImpSDRate");
            txtImpSDRate.Text = Program.ParseDecimalObject(txtImpSDRate.Text.Trim()).ToString();
        }

        private void txtimpVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtimpVATRate, "impVATRate");
            txtimpVATRate.Text = Program.ParseDecimalObject(txtimpVATRate.Text.Trim()).ToString();
        }

        private void txtAITRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtAITRate, "AITRate");
            txtAITRate.Text = Program.ParseDecimalObject(txtAITRate.Text.Trim()).ToString();
        }

        private void txtATVRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtATVRate, "ATVRate");
            txtATVRate.Text = Program.ParseDecimalObject(txtATVRate.Text.Trim()).ToString();
        }

        private void txtProductStockQuantity_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtProductStockQuantity, "ProductStockQuantity");
            txtProductStockQuantity.Text = Program.ParseDecimalObject(txtProductStockQuantity.Text.Trim()).ToString();
        }

        private void txtProductStockValue_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtProductStockValue, "ProductStockValue");
            txtProductStockValue.Text = Program.ParseDecimalObject(txtProductStockValue.Text.Trim()).ToString();
        }

        private void txtHPSRate_Leave(object sender, EventArgs e)
        {
            txtHPSRate.Text = Program.ParseDecimalObject(txtHPSRate.Text.Trim()).ToString();
        }

        private void txtCostPriceInput_Leave(object sender, EventArgs e)
        {
            txtCostPriceInput.Text = Program.ParseDecimalObject(txtCostPriceInput.Text.Trim()).ToString();
            //Program.FormatTextBox(txtCostPriceInput, "Cost Price");
            //TotalCost = Convert.ToDecimal(txtCostPriceInput.Text.Trim());
            if (chkPerUnitCost.Checked)
            {
                decimal oQty = Convert.ToDecimal(txtOpeningBalance.Text.Trim());
                decimal totalCost = Convert.ToDecimal(txtCostPriceInput.Text.Trim());
                //decimal oQty = OpeningQty;
                //decimal totalCost = TotalCost;
                if (oQty == 0 || totalCost == 0)
                {
                    txtCostPrice.Text = "0";
                }
                else
                {
                    txtCostPrice.Text = (totalCost / oQty).ToString();

                }
            }
            else
            {
                txtCostPrice.Text = txtCostPriceInput.Text.Trim();
            }

        }

        private void txtRebate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtRebate, "Rebate(%)");
            txtRebate.Text = Program.ParseDecimalObject(txtRebate.Text.Trim()).ToString();
        }

        private void txtSDRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtSDRate, "SD Rate");
            txtSDRate.Text = Program.ParseDecimalObject(txtSDRate.Text.Trim()).ToString();
        }

        private void txtOpeningBalance_Leave(object sender, EventArgs e)
        {
            btnAdd.Focus();
            txtOpeningBalance.Text = Program.ParseDecimalObject(txtOpeningBalance.Text.Trim()).ToString();
            //Program.FormatTextBox(txtOpeningBalance, "Opening Balance");
            //OpeningQty = Convert.ToDecimal(txtOpeningBalance.Text.Trim());
            if (chkPerUnitCost.Checked)
            {
                decimal oQty = Convert.ToDecimal(txtOpeningBalance.Text.Trim());
                decimal totalCost = Convert.ToDecimal(txtCostPriceInput.Text.Trim());

                //decimal oQty = OpeningQty;
                //decimal totalCost = TotalCost;

                if (oQty == 0 || totalCost == 0)
                {
                    txtCostPrice.Text = "0";
                }
                else
                {
                    txtCostPrice.Text = (totalCost / oQty).ToString();
                }
            }
            else
            {
                txtCostPrice.Text = txtCostPriceInput.Text.Trim();
                //txtCostPrice.Text = TotalCost.ToString();
            }

            //txtTotalCost.Text =
            //    Convert.ToDecimal(Convert.ToDecimal(txtCostPrice.Text) * Convert.ToDecimal(txtOpeningBalance.Text)).
            //        ToString("0.00");
            txtTotalCost.Text =
                Convert.ToDecimal(Convert.ToDecimal(txtCostPrice.Text) * Convert.ToDecimal(txtOpeningBalance.Text)).
                    ToString();
        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
            Program.CheckVATRate(txtVATRate, "VAT Rate Input Box");

        }

        private void txtNBRPrice_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtNBRPrice, "NBR Price");
            txtNBRPrice.Text = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();
        }

        private void txtCostPrice_Leave(object sender, EventArgs e)
        {
            txtCostPrice.Text = Program.ParseDecimalObject(txtCostPrice.Text.Trim()).ToString();

            //Program.FormatTextBox(txtCostPrice, "Cost Price");
            txtTotalCost.Text =
                Convert.ToDecimal(Convert.ToDecimal(txtCostPrice.Text) * Convert.ToDecimal(txtOpeningBalance.Text)).
                    ToString("0.00");

        }

        private void txtSalesPrice_Leave(object sender, EventArgs e)
        {
            txtSalesPrice.Text = Program.ParseDecimalObject(txtSalesPrice.Text.Trim()).ToString();

            //Program.FormatTextBox(txtSalesPrice, "Sale Price");
        }

        #endregion

        #endregion

        #region Methods 08

        private void chkOtherCD_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkCD_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkIsVDS_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtStockId.Text))
                {
                    MessageBox.Show("Please Select A Raw  first");
                    dgvProductStock.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtWastageTotalQuantity.Text))
                {
                    txtWastageTotalQuantity.Text = "0.00";
                }

                sqlResults = new string[4];

                ProductStockVM psVM = new ProductStockVM();
                psVM.StockId = Convert.ToInt32(txtStockId.Text);
                psVM.BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                psVM.ItemNo = txtItemNo.Text.Trim();
                psVM.StockQuantity = Convert.ToDecimal(txtProductStockQuantity.Text);
                psVM.StockValue = Convert.ToDecimal(txtProductStockValue.Text);
                psVM.Comments = textProductStockComments.Text;
                psVM.WastageTotalQuantity = Convert.ToDecimal(txtWastageTotalQuantity.Text);

                ProductDAL pDal = new ProductDAL();
                //IProduct pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);
                sqlResults = pDal.UpdateToProductStock(psVM, null, null, connVM);
                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("btnChange_Click",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }

                    if (result == "Success" && psVM.BranchId == 1)
                    {
                        txtCostPriceInput.Text = txtProductStockValue.Text;
                        txtOpeningBalance.Text = txtProductStockQuantity.Text;
                    }
                    fnProductStock(psVM.ItemNo);

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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            #endregion
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                bgwProductSync.RunWorkerAsync();


            }
            catch (Exception exception)
            {
                FileLogger.Log("ProductSync", "ProductSync", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);

            }
            finally
            {

            }
        }

        private void bgwProductSync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();
                CommonDAL commonDAL = new CommonDAL();
                DataTable productDt = new DataTable();
                results[0] = "fail";
                string code = commonDAL.settingValue("CompanyCode", "Code", connVM);

                string dbName = DatabaseInfoVM.DatabaseName;

                if (OrdinaryVATDesktop.IsACICompany(code))
                {
                    productDt = importDal.GetProductACIDbData(settingVM.BranchInfoDT, dbName, connVM);

                }
                else if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                {
                    productDt = importDal.GetProductNestleDbData(settingVM.BranchInfoDT, connVM);

                }
                else if (OrdinaryVATDesktop.IsNourishCompany(code))
                {
                    NourishIntegrationDAL nourishIntegrationDal = new NourishIntegrationDAL();

                    productDt = nourishIntegrationDal.GetProduct(settingVM.BranchInfoDT, connVM);

                }

                else if (code.ToLower() == "decathlon")
                {
                    DecathlonIntegrationDAL DecathlonIntegrationDal = new DecathlonIntegrationDAL();

                    productDt = DecathlonIntegrationDal.GetProductDataAPI(connVM);

                }

                List<ProductVM> products = new List<ProductVM>();

                int rowsCount = productDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonDAL.settingsDesktop("AutoSave", "DefaultProductCategory", null, connVM);

                for (int i = 0; i < rowsCount; i++)
                {
                    ProductVM product = new ProductVM();

                    product.ProductName = Program.RemoveStringExpresion(productDt.Rows[i]["ProductName"].ToString());
                    product.ProductDescription = productDt.Rows[i]["Description"].ToString();
                    product.CategoryName = productDt.Rows[i]["ProductGroup"].ToString();

                    if (product.CategoryName == "-" || string.IsNullOrWhiteSpace(product.CategoryName))
                    {
                        product.CategoryName = defaultGroup;
                    }
                    if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                    {
                        product.TradingSaleVATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.TradingSaleSD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());

                        product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                        product.ShortName = productDt.Rows[i]["ProductNameBangla"].ToString();
                        product.Packetprice = Convert.ToDecimal(productDt.Rows[i]["Packetprice"].ToString());

                    }
                    if (OrdinaryVATDesktop.IsACICompany(code))
                    {
                        product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                        product.Packetprice = 0;

                    }


                    product.UOM = productDt.Rows[i]["UOM"].ToString();

                    product.NBRPrice = Convert.ToDecimal(productDt.Rows[i]["UnitPrice"].ToString());

                    product.SerialNo = "-";
                    product.HSCodeNo = productDt.Rows[i]["HSCode"].ToString();
                    product.Comments = "-";
                    product.ActiveStatus = "Y";
                    product.Trading = "N";
                    product.TradingMarkUp = 0;
                    product.NonStock = "N"; ;
                    product.OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    product.CreatedBy = Program.CurrentUser;
                    product.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    product.ProductCode = Program.RemoveStringExpresion(productDt.Rows[i]["ProductCode"].ToString());
                    product.TollCharge = 0;
                    product.BranchId = Program.BranchId;
                    product.IsConfirmed = "N";

                    if (code.ToLower() == "cepl")
                    {
                        product.UOM = "pcs";
                        product.VATRate = 15;
                    }

                    products.Add(product);

                    ids.Add(productDt.Rows[i]["SL"].ToString());
                }


                ////////results = importDal.ImportProduct(products, new List<TrackingVM>());
                ////results = importDal.ImportProductSync(products, new List<TrackingVM>());
                results = importDal.InsertImportProductSync(products, new List<TrackingVM>(), null, null, connVM);

                if (results[0].ToLower() == "success")
                {
                    if (OrdinaryVATDesktop.IsACICompany(code))
                    {
                        results = importDal.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Products", connVM);
                    }
                    else if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                    {
                        results = importDal.UpdateNestleMaster(ids, settingVM.BranchInfoDT, "Products", connVM);

                    }
                }

            }
            catch (Exception exception)
            {
                results[0] = "exception";
                results[1] = exception.Message + "\n" + exception.StackTrace;
            }
        }

        private void bgwProductSync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (results[0].ToLower() == "success")
                {
                    MessageBox.Show("Synchronized");
                }
                else if (results[0] == "exception")
                {
                    MessageBox.Show(results[1]);
                    FileLogger.Log("ProductSync", "ProductSync", results[1]);

                }
                else
                {
                    MessageBox.Show("Nothing to syncronize");
                }
            }
            catch (Exception exception)
            {
                FileLogger.Log("ProductSync", "ProductSync", exception.Message + "\n" + exception.StackTrace);

                MessageBox.Show(exception.Message);
            }

            progressBar1.Visible = false;

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btnUOM2Search_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                Program.fromOpen = "Other";

                //string result = FormUOMSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] UOMInfo = result.Split(FieldDelimeter.ToCharArray());
                //    cmbUom.Text = UOMInfo[1];
                //}
                DataGridViewRow selectedRow = null;
                selectedRow = FormUOMNameSearch.SelectOne();

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    cmbUom2.Text = selectedRow.Cells["UOMName"].Value.ToString();
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
                FileLogger.Log(this.Name, "btnUOM2Search_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUOM2Search_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUOM2Search_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnUOM2Search_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOM2Search_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOM2Search_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOM2Search_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUOM2Search_Click", exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void txtConvertion_Leave(object sender, EventArgs e)
        {
            txtConvertion.Text = Program.ParseDecimalObject(txtConvertion.Text.Trim()).ToString();

        }

        #endregion

        #region Navigation

        private void btnFirst_Click(object sender, EventArgs e)
        {
            ProductNavigation("First");
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            ProductNavigation("Previous");

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ProductNavigation("Next");

        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            ProductNavigation("Last");

        }

        private void ProductNavigation(string ButtonName)
        {
            try
            {
                ProductDAL _ProductDAL = new ProductDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.ProductType = cmbProductType.Text;

                if (rbtnOverHead.Checked)
                {
                    vm.ProductType = "Overhead";
                }

                vm.Code = txtSearchProductCode.Text.Trim();

                vm = _ProductDAL.Product_Navigation(vm, null, null, connVM);

                txtItemNo.Text = vm.ItemNo;
                vItemNo = vm.ItemNo;

                ProdutSearch();


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

        CommonFormMethod varCommonMethod = new CommonFormMethod();

        private void txtPCode_KeyDown(object sender, KeyEventArgs e)
        {



        }

        private void txtPCode_DoubleClick(object sender, EventArgs e)
        {


        }


        private void MultipleSearch()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                ProductVM vm = new ProductVM();
                vm.IsRaw = cmbProductType.Text;

                if (rbtnOverHead.Checked)
                {
                    vm.IsRaw = "Overhead";
                }

                selectedRow = varCommonMethod.ProductLoad(vm);

                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    vItemNo = txtItemNo.Text;

                    ProdutSearch();
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
                FileLogger.Log(this.Name, "MultipleSearch", exMessage);
            }
            #endregion Catch
        }






        #endregion

        private void txtSearchProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                MultipleSearch();

            }

            if (e.KeyCode.Equals(Keys.Enter))
            {
                ProductNavigation("Current");
            }

        }

        private void txtSearchProductCode_DoubleClick(object sender, EventArgs e)
        {
            MultipleSearch();
        }

        private void txtTollCharge_Leave_1(object sender, EventArgs e)
        {
            txtTollCharge.Text = Program.ParseDecimalObject(txtTollCharge.Text.Trim()).ToString();

        }

        private void txtVATRate3_Leave(object sender, EventArgs e)
        {
            txtVATRate3.Text = Program.ParseDecimalObject(txtVATRate3.Text.Trim()).ToString();
        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void txtcustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                VendorLoad();
            }
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
                    txtcustomerid.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtcustomer.Text = selectedRow.Cells["VendorName"].Value.ToString();

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

                // StackTrace st = new StackTrace();

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // FileLogger.Log(this.Name, st.GetFrame(0).GetMethod().Name, exMessage);
            }
            #endregion

        }
        private void customerLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select cg.CustomerGroupName,c.CustomerCode,c.CustomerName,c.ShortName,c.Address1,c.CustomerID, IsExamted,IsSpecialRate
                            from Vendors c 
                            left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID 
                            where 1=1 and c.ActiveStatus='Y'";
                string ShowAllCustomer = commonDal.settingsDesktop("Setup", "ShowAllCustomer", null, connVM);
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
                    txtcustomer.Text = "";
                    txtcustomerid.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtcustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];

                    txtcustomer.Focus();
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

        private void btnCustomerRateAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Product first");
                    return;
                }
                //string ProductStockQuantity = txtProductStockQuantity.Text;

                //string ProductStockValue = txtProductStockValue.Text;
                //if (ProductStockQuantity == "" || ProductStockValue == "")
                //{
                //    if (ProductStockQuantity == "" && ProductStockValue == "")
                //    {
                //        MessageBox.Show("Entered Stock Quantity & Stock Value first !", this.Text);
                //        return;
                //    }
                //    else if (ProductStockQuantity == "")
                //    {
                //        MessageBox.Show("Entered a Stock Quantity first !", this.Text);
                //        return;
                //    }

                //    else if (ProductStockValue == "")
                //    {
                //        MessageBox.Show("Entered a Stock Value first !", this.Text);
                //        return;
                //    }

                //}

                //if (string.IsNullOrWhiteSpace(txtWastageTotalQuantity.Text))
                //{
                //    txtWastageTotalQuantity.Text = "0.00";
                //}

                decimal TollCharge = Convert.ToDecimal(txtcrtollcharge.Text);
                // decimal NBRPrice = Convert.ToDecimal(txtcrnbrprice.Text);

                if (TollCharge < 0)
                {
                    MessageBox.Show("TollCharge Value Can't be Zero !", this.Text);
                    txtcrtollcharge.Focus();
                    return;
                }
                //else if (StockQuantity <= 0 && StockValue > 0)
                //{
                //    MessageBox.Show("Stock Quantity Can't be Zero !", this.Text);
                //    txtProductStockQuantity.Focus();
                //    return;
                //}

                //if (textProductStockComments.Text == "")
                //{
                //    textProductStockComments.Text = "NA";
                //}


                // ProductStockVM psVM = new ProductStockVM();
                CustomerRateVM psVM = new CustomerRateVM();
                psVM.BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                psVM.ItemNo = txtItemNo.Text.Trim();
                psVM.TollCharge = Convert.ToDecimal(TollCharge);
                psVM.NBRPrice = Convert.ToDecimal("0");
                psVM.CustomerId = txtcustomerid.Text.Trim();
                ProductDAL pDal = new ProductDAL();

                stockResult = pDal.InserToCustomerRate(psVM, null, null, connVM);

                if (stockResult.Length > 0)
                {
                    string result = stockResult[0];
                    string message = stockResult[1];

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    //if (result == "Success" && psVM.BranchId == 1)
                    //{
                    //    txtCostPriceInput.Text = txtProductStockValue.Text;
                    //    txtOpeningBalance.Text = txtProductStockQuantity.Text;
                    //}

                    SelectProductCustomerRate(vItemNo);
                    txtcustomer.Text = "";
                    txtcrtollcharge.Text = "";
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void txtcustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtcustomer_DoubleClick(object sender, EventArgs e)
        {
            VendorLoad();
        }

        private void txtcrtollcharge_Leave(object sender, EventArgs e)
        {
            txtcrtollcharge.Text = Program.ParseDecimalObject(txtcrtollcharge.Text.Trim()).ToString();

        }

        private void txtcrnbrprice_Leave(object sender, EventArgs e)
        {
            txtcrnbrprice.Text = Program.ParseDecimalObject(txtcrnbrprice.Text.Trim()).ToString();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(txtcustomer.Text))
                {
                    MessageBox.Show("Please Select Customer Name first");
                    return;
                }
                sqlResults = new string[4];

                CustomerRateVM vm = new CustomerRateVM();
                vm.CustomerId = txtcustomerid.Text;
                vm.ItemNo = txtItemNo.Text;
                vm.BranchId = Program.BranchId;


                ProductDAL cDal = new ProductDAL();
                //  IProduct cDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                sqlResults = cDal.DeleteToProductCustomerRate(vm.ItemNo, vm.CustomerId.ToString(), vm.BranchId, null, null, connVM);
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
                    }
                    // fnProductNames(vItemNo);

                    SelectProductCustomerRate(vItemNo);
                    txtcustomer.Text = "";
                    txtcrtollcharge.Text = "";


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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
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

        }

        private void dgvProductRate_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvProductRate.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow1 = selectedRows[0];
                txtcustomerid.Text = userSelRow1.Cells["CustomerId"].Value.ToString();
                txtcustomer.Text = userSelRow1.Cells["CustomerName"].Value.ToString();
                txtcrtollcharge.Text = userSelRow1.Cells["TollCharge"].Value.ToString();

            }
        }

        private void gbxNavigation_Enter(object sender, EventArgs e)
        {

        }

        //private void chkcodeUpdate_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkcodeUpdate.Checked)
        //    {
        //        txtPCode.ReadOnly = false;
        //    }
        //    else
        //    {
        //        txtPCode.ReadOnly = true;
        //    }

        //    //label33.Visible = false;
        //}

        private void btnCodeUpdate_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (txtPCode.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter Product code.");
                    txtPCode.Focus();
                    return;
                }

                //UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                if (IsUpdate == false)
                {
                    ////NextID = string.Empty;
                    MessageBox.Show("Its applicable for exiting product");
                    return;
                }
                else
                {
                    NextID = txtItemNo.Text.Trim();
                }

                #region ProductDAL

                ProductDAL productDal = new ProductDAL();

                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                #endregion

                #region Product VM

                ProductVM vm = new ProductVM();

                vm.ItemNo = NextID.ToString();
                vm.ProductCode = txtPCode.Text.Trim();


                #endregion

                #region Update Product


                //sqlResults = productDal.(vm, new List<TrackingVM>(), null, connVM);

                ProductDAL productdal = new ProductDAL();
                sqlResults = productdal.UpdateProductCode(vm, new List<TrackingVM>(), null, connVM);


                #endregion

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;

                        txtItemNo.Text = newId;


                    }

                }

                //UPDATE_DOWORK_SUCCESS = true;
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
                //FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }

            #endregion
        }

        private void btnProductMapAdd_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Product first");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMappingCode.Text))
                {
                    MessageBox.Show("Please Add Mapping Code first");
                    txtMappingCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtProductDescriptionMap.Text))
                {
                    MessageBox.Show("Please Add Product Description first");
                    txtProductDescriptionMap.Focus();
                    return;
                }

                sqlResults = new string[4];

                ProductMapVM vm = new ProductMapVM();
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductMappingCode = txtMappingCode.Text.Trim();
                vm.ProductDescription = txtProductDescriptionMap.Text;
                vm.ProductCode = txtPCode.Text;

                ProductDAL pDal = new ProductDAL();

                //////sqlResults = pDal.InsertToProductMapDetails(vm, null, null, connVM);
                sqlResults = pDal.InsertToProductChild(vm, null, null, connVM);

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("btnProductMapAdd_Click", "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (result == "Success")
                        {
                            IsUpdate = true;

                            txtSL.Text = newId;

                            #region Clear

                            ProductMapDetailsClear();

                            #endregion
                        }


                    }

                    #region Select ProductMapDetails

                    SelectProductMapDetails(vm);

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
                FileLogger.Log(this.Name, "btnProductMapAdd_Click", exMessage);
            }

            #endregion
        }

        private void ProductMapDetailsClear()
        {
            txtSL.Clear();
            txtMappingCode.Clear();
            txtProductDescriptionMap.Clear();
        }

        private void SelectProductMapDetails(ProductMapVM vm)
        {
            try
            {

                DataTable dtResult = new DataTable();

                ProductDAL pDAL = new ProductDAL();

                ////dtResult = pDAL.SearchProductMapDetails(vm, null, null, connVM);
                dtResult = pDAL.SearchChildProducts(vm, null, null, connVM);

                dgvProductMapping.Rows.Clear();

                int j = 0;

                ////foreach (DataRow item in dtResult.Rows)
                ////{
                ////    DataGridViewRow NewRow = new DataGridViewRow();
                ////    dgvProductMapping.Rows.Add(NewRow);

                ////    dgvProductMapping.Rows[j].Cells["SLNo"].Value = (j + 1).ToString();
                ////    dgvProductMapping.Rows[j].Cells["MappingSL"].Value = item["SL"].ToString();
                ////    dgvProductMapping.Rows[j].Cells["ProductMappingCode"].Value = item["ProductMappingCode"].ToString();
                ////    dgvProductMapping.Rows[j].Cells["ProductDescription"].Value = item["ProductDescription"].ToString();

                ////    j = j + 1;

                ////}
                foreach (DataRow item in dtResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvProductMapping.Rows.Add(NewRow);

                    dgvProductMapping.Rows[j].Cells["SLNo"].Value = (j + 1).ToString();
                    dgvProductMapping.Rows[j].Cells["ChildItemNo"].Value = item["ItemNo"].ToString();
                    dgvProductMapping.Rows[j].Cells["ProductMappingCode"].Value = item["ProductCode"].ToString();
                    dgvProductMapping.Rows[j].Cells["ProductDescription"].Value = item["ProductName"].ToString();

                    j = j + 1;

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
                FileLogger.Log(this.Name, "SelectProductMapDetails", exMessage);
            }
            #endregion

        }

        private void btnProductMapEdit_Click(object sender, EventArgs e)
        {
            sqlResults = new string[4];

            try
            {

                if (string.IsNullOrWhiteSpace(txtSL.Text))
                {
                    MessageBox.Show("Please Select Item first");
                    dgvProductMapping.Focus();
                    return;
                }


                ProductMapVM vm = new ProductMapVM();

                vm.SL = Convert.ToInt32(txtSL.Text.Trim());

                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductMappingCode = txtMappingCode.Text.Trim();
                vm.ProductDescription = txtProductDescriptionMap.Text;
                vm.ProductCode = txtPCode.Text;

                ProductDAL pDal = new ProductDAL();

                sqlResults = pDal.UpdateToProductMapDetails(vm, null, null, connVM);

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("btnProductMapEdit_Click", "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }

                    #region Clear

                    ProductMapDetailsClear();

                    #endregion

                    #region fnBranchMapDetails

                    SelectProductMapDetails(vm);

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
                FileLogger.Log(this.Name, "btnProductMapEdit_Click", exMessage);
            }
            #endregion
        }

        private void btnProductMapDelete_Click(object sender, EventArgs e)
        {

            try
            {
                sqlResults = new string[4];

                #region Validation

                if (string.IsNullOrWhiteSpace(txtSL.Text))
                {
                    MessageBox.Show("Please Select Item first");
                    dgvProductMapping.Focus();
                    return;
                }

                #endregion

                #region  Product VM

                ProductMapVM vm = new ProductMapVM();

                vm.SL = Convert.ToInt32(txtSL.Text.Trim());
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductMappingCode = txtMappingCode.Text.Trim();
                vm.ProductDescription = txtProductDescriptionMap.Text;
                vm.ProductCode = txtPCode.Text;

                #endregion

                #region Product DAL

                ProductDAL bDal = new ProductDAL();

                sqlResults = bDal.DeleteProductMapDetails(vm, null, null, connVM);

                #endregion

                #region sqlResults

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("btnProductMapDelete_Click", "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }

                    #region Clear

                    ProductMapDetailsClear();

                    #endregion

                    #region fnBranchMapDetails

                    SelectProductMapDetails(vm);

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
                FileLogger.Log(this.Name, "btnProductMapDelete_Click", exMessage);
            }

            #endregion
        }

        private void dgvProductMapping_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection selectedRows = dgvProductMapping.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    DataGridViewRow userSelRow = selectedRows[0];
                    txtSL.Text = userSelRow.Cells["MappingSL"].Value.ToString();
                    txtMappingCode.Text = userSelRow.Cells["ProductMappingCode"].Value.ToString();
                    txtProductDescriptionMap.Text = userSelRow.Cells["ProductDescription"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvProductMapping_DoubleClick", exMessage);
            }

        }

        private void btnPriceHistoryAdd_Click(object sender, EventArgs e)
        {
            ResultVM _rVM = new ResultVM();

            #region try

            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Product first");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(txtPriceHistorySL.Text))
                {
                    MessageBox.Show("Data already exits");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPriceHistoryVatablePrice.Text))
                {
                    MessageBox.Show("Please Add Vatable Price first");
                    txtPriceHistoryVatablePrice.Focus();
                    return;
                }

                ProductPriceHistoryVM vm = new ProductPriceHistoryVM();
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductCode = txtPCode.Text;
                vm.EffectDate = dtpPriceHistoryEffectDate.Value.ToString("yyyy-MM-dd");
                vm.VatablePrice = Convert.ToDecimal(txtPriceHistoryVatablePrice.Text);
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                ProductDAL pDal = new ProductDAL();

                _rVM = pDal.InsertToProductPriceHistory(vm, null, null, connVM);

                if (_rVM != null)
                {
                    string result = _rVM.Status;
                    string message = _rVM.Message;
                    string newId = _rVM.Id.ToString();

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (result == "Success")
                        {
                            IsUpdate = true;

                            txtPriceHistorySL.Text = newId;

                            #region Clear

                            ProductPriceHistoryClear();

                            #endregion
                        }

                    }

                    #region Select ProductMapDetails

                    SelectProductPriceHistory(vm);

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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPriceHistoryAdd_Click", exMessage);
            }

            #endregion
        }

        private void ProductPriceHistoryClear()
        {
            txtPriceHistorySL.Clear();
            txtPriceHistoryVatablePrice.Clear();
        }

        private void SelectProductPriceHistory(ProductPriceHistoryVM vm)
        {
            try
            {

                DataTable dtResult = new DataTable();

                ProductDAL pDAL = new ProductDAL();

                dtResult = pDAL.SearchProductPriceHistory(vm, null, null, connVM);

                dgvPriceHistory.Rows.Clear();

                int j = 0;

                foreach (DataRow item in dtResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvPriceHistory.Rows.Add(NewRow);

                    dgvPriceHistory.Rows[j].Cells["PriceHistorySL"].Value = (j + 1).ToString();
                    dgvPriceHistory.Rows[j].Cells["PriceHistoryItemNo"].Value = item["ItemNo"].ToString();
                    dgvPriceHistory.Rows[j].Cells["PriceHistoryId"].Value = item["Id"].ToString();
                    dgvPriceHistory.Rows[j].Cells["PriceHistoryEffectDate"].Value = Convert.ToDateTime(item["EffectDate"].ToString()).ToString("dd/MMM/yyyy");
                    dgvPriceHistory.Rows[j].Cells["PriceHistoryVatablePrice"].Value = item["VatablePrice"].ToString();

                    j = j + 1;

                }

            }

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SelectProductPriceHistory", exMessage);
            }
            #endregion

        }

        private void btnPriceHistoryUpdate_Click(object sender, EventArgs e)
        {
            ResultVM _rVM = new ResultVM();

            try
            {
                if (string.IsNullOrWhiteSpace(txtPriceHistorySL.Text))
                {
                    MessageBox.Show("Please Select row first");
                    dgvPriceHistory.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPriceHistoryVatablePrice.Text))
                {
                    MessageBox.Show("Please add Vatable Price first");
                    txtPriceHistoryVatablePrice.Focus();
                    return;
                }

                ProductPriceHistoryVM vm = new ProductPriceHistoryVM();

                vm.Id = Convert.ToInt32(txtPriceHistorySL.Text.Trim());
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductCode = txtPCode.Text;
                vm.EffectDate = dtpPriceHistoryEffectDate.Value.ToString("yyyy-MM-dd");
                vm.VatablePrice = Convert.ToDecimal(txtPriceHistoryVatablePrice.Text);
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                ProductDAL pDal = new ProductDAL();

                _rVM = pDal.UpdateToProductPriceHistory(vm, null, null, connVM);

                if (_rVM != null)
                {
                    string result = _rVM.Status;
                    string message = _rVM.Message;

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (result == "Success")
                        {
                            string newId = _rVM.Id.ToString();

                            txtPriceHistorySL.Text = newId;

                            #region Clear

                            ProductPriceHistoryClear();

                            #endregion
                        }

                    }

                    #region Select ProductMapDetails

                    SelectProductPriceHistory(vm);

                    #endregion

                }
            }

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPriceHistoryUpdate_Click", exMessage);
            }
            #endregion
        }

        private void btnPriceHistoryDelete_Click(object sender, EventArgs e)
        {
            ResultVM _rVM = new ResultVM();

            try
            {
                sqlResults = new string[4];

                #region Validation

                if (string.IsNullOrWhiteSpace(txtPriceHistorySL.Text))
                {
                    MessageBox.Show("Please Select row first");
                    dgvPriceHistory.Focus();
                    return;
                }

                #endregion

                #region VM

                ProductPriceHistoryVM vm = new ProductPriceHistoryVM();

                vm.Id = Convert.ToInt32(txtPriceHistorySL.Text.Trim());
                vm.ItemNo = txtItemNo.Text.Trim();
                vm.ProductCode = txtPCode.Text;
                
                #endregion

                #region Product DAL

                ProductDAL bDal = new ProductDAL();

                _rVM = bDal.DeleteProductPriceHistory(vm, null, null, connVM);

                #endregion

                #region sqlResults

                if (_rVM != null)
                {
                    string result = _rVM.Status;
                    string message = _rVM.Message;

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (result == "Success")
                        {

                            #region Clear

                            ProductPriceHistoryClear();

                            #endregion
                        }

                    }

                    #region Select ProductPriceHistory

                    SelectProductPriceHistory(vm);

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
                FileLogger.Log(this.Name, "btnProductMapDelete_Click", exMessage);
            }

            #endregion
        }

        private void dgvPriceHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection selectedRows = dgvPriceHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    DataGridViewRow userSelRow = selectedRows[0];
                    txtPriceHistorySL.Text = userSelRow.Cells["PriceHistoryId"].Value.ToString();
                    dtpPriceHistoryEffectDate.Value = Convert.ToDateTime(userSelRow.Cells["PriceHistoryEffectDate"].Value.ToString());
                    txtPriceHistoryVatablePrice.Text = userSelRow.Cells["PriceHistoryVatablePrice"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvPriceHistory_DoubleClick", exMessage);
            }
        }

        private void btnImportPriceHistory_Click(object sender, EventArgs e)
        {
            try
            {
                FormImportProductPriceHistory frm = new FormImportProductPriceHistory();
                frm.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

    }
}
