using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using SymphonySofttech.Utilities;
////
using VATClient.ModelDTO;
using VATClient.ReportPages;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Library.Integration;
using VATServer.Ordinary;


namespace VATClient
{
    public partial class FormCustomer : Form
    {
        public FormCustomer()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;


        }

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        List<CustomerGroupDTO> customerGroups = new List<CustomerGroupDTO>();
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        string NextID;
        string CustomerData;
        string CustomerGroupData;
        string[] CustomerGroupLines;
        private bool ChangeData = false;
        public string VFIN = "132";
        public string[] results = new string[5];
        private bool IsUpdate = false;
        private int searchBranchId = 0;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        string vCustomerId = "0";
        string vCustomerAddressId = "0";
        private string activestatus = string.Empty;
        private DataTable CustomerGroupResult;
        private DataTable CustomerAddressResult;
        private DataTable CustomerDiscountResult;
        private string result;

        #endregion

        #region Methods 01

        public int ErrorReturn()
        {
            if (txtCustomerName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Customer Name.", this.Text);
                txtCustomerName.Focus();
                return 1;
            }
            if (txtCustomerGroupID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Customer Group.", this.Text);
                cmbCustomerGroupName.Focus();
                return 1;
            }
            if (txtTINNo.Text == "")
            {
                txtTINNo.Text = "-";
            }
            if (txtBusinessCode.Text == "")
            {
                txtBusinessCode.Text = "-";
            }
            if (txtBusinessType.Text == "")
            {
                txtBusinessType.Text = "-";
            }
            if (txtVATRegistrationNo.Text == "")
            {
                MessageBox.Show("Please enter VAT Reg.No/BIN/NID.", this.Text);
                txtVATRegistrationNo.Focus();
                return 1;
            }


            string VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
            string VATRegNo = VATRegistrationNo.Replace("-", "");


            if (!OrdinaryVATDesktop.IsNumeric(VATRegNo))
            {
                MessageBox.Show("Please Enter VATRegistrationNo only number");
                txtVATRegistrationNo.Focus();
                return 1;
            }

            if (VATRegNo.Length >= 14)
            {
                MessageBox.Show("Please Enter Valid VATRegistrationNo No/Not more than 13 digit.");
                txtVATRegistrationNo.Focus();
                return 1;
            }






            if (txtNIDNo.Text.Trim() == "")
            {
                txtNIDNo.Text = "-";
            }

            if (txtNIDNo.Text != "-")
            {
                if (!OrdinaryVATDesktop.IsNumber(txtNIDNo.Text))
                {
                    MessageBox.Show("Please Enter National ID  only number");
                    txtNIDNo.Focus();
                    return 1;
                }

                if (txtNIDNo.Text.Length >= 18)
                {
                    MessageBox.Show("Please Enter Valid National ID No/Not more than 17 digit.");
                    txtNIDNo.Focus();
                    return 1;
                }

            }



            if (txtContactPerson.Text == "")
            {
                txtContactPerson.Text = "-";
            }
            if (txtContactPersonDesignation.Text == "")
            {
                txtContactPersonDesignation.Text = "-";
            }
            if (txtContactPersonTelephone.Text == "")
            {
                txtContactPersonTelephone.Text = "-";
            }
            if (txtContactPersonEmail.Text == "")
            {
                txtContactPersonEmail.Text = "-";
            }
            if (txtTelephoneNo.Text == "")
            {
                txtTelephoneNo.Text = "-";
            }
            if (txtFaxNo.Text == "")
            {
                txtFaxNo.Text = "-";
            }
            if (txtEmail.Text == "")
            {
                txtEmail.Text = "-";
            }
            if (txtAddress1.Text == "")
            {
                txtAddress1.Text = "-";
            }
            //if (txtAddress2.Text == "")
            //{
            //    txtAddress2.Text = "-";
            //}
            //if (txtAddress3.Text == "")
            //{
            //    txtAddress3.Text = "-";
            //}
            if (txtCity.Text == "")
            {
                txtCity.Text = "-";
            }
            if (txtComments.Text == "")
            {
                txtComments.Text = "-";
            }
            if (txtCountry.Text == "")
            {
                txtCountry.Text = "-";
            }
            if (txtShortName.Text.Trim() == "")
            {
                txtShortName.Text = "-";
            }
            return 0;
        }
        private void ClearAll()
        {
            DateTime vdateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));

            txtCustomerID.Text = "";
            txtBusinessCode.Text = "";
            txtBusinessType.Text = "";
            txtCountry.Text = "";
            txtCGType.Text = "";

            txtCustomerName.Text = "";
            txtCustomerGroupID.Text = "";
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtCity.Text = "";
            txtTelephoneNo.Text = "";
            txtFaxNo.Text = "";
            txtEmail.Text = "";
            txtContactPerson.Text = "";
            txtContactPersonDesignation.Text = "";
            txtContactPersonTelephone.Text = "";
            txtContactPersonEmail.Text = "";
            txtTINNo.Text = "";
            txtVATRegistrationNo.Text = "";
            dtpStartDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + " " + vdateTime.ToString("HH:mm:ss"));//Program.SessionDate;
            txtComments.Text = "";
            txtCustomerCode.Text = string.Empty;
            cmbCustomerGroupName.Text = "Select";
            chkVDSWithHolder.Checked = false;
            chkInstitution.Checked = false;
            chkIsExamted.Checked = false;
            txtShortName.Text = "";
            chkIsTax.Checked = false;
            chkTCS.Checked = false;

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnCancel_Click(object sender, EventArgs e)
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
                        CustomerGroupSearch();
                        ClearAll();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
                    CustomerGroupSearch();
                    ClearAll();
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
            #endregion
        }
        private void picCustomerID_Click(object sender, EventArgs e)
        {

        }
        private void picCustomerGroupID_Click(object sender, EventArgs e)
        {

        }

        #region TextBox KeyDown Event

        private void txtCustomerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpStartDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTINNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVATRegistrationNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonDesignation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonTelephone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTelephoneNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtFaxNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        #endregion

        private void cmbCustomerGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtGroupDesc_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnPrintGrid_Click(object sender, EventArgs e)
        {

        }
        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                vCustomerId = "0";
                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormCustomerSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();//[0]

                    SearchCustomer();

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
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }

        }

        #endregion

        #region Methods 02

        private void SearchCustomer()
        {
            try
            {
                #region Data Call

                DataTable dt = new DataTable();
                dt = new CustomerDAL().SelectAll(txtCustomerID.Text, null, null, null, null, true, connVM);

                vCustomerId = txtCustomerID.Text;

                #endregion

                #region Value Assign to Form Elements

                if (dt != null && dt.Rows.Count > 0)
                {

                    DataRow dr = dt.Rows[0];

                    txtCustomerCode.Text = dr["CustomerCode"].ToString();
                    txtSearchCustomerCode.Text = dr["CustomerCode"].ToString();
                    txtCustomerName.Text = dr["CustomerName"].ToString();
                    txtCustomerGroupID.Text = dr["CustomerGroupID"].ToString();
                    cmbCustomerGroupName.Text = dr["CustomerGroupName"].ToString();
                    txtAddress1.Text = dr["Address1"].ToString();
                    txtAddress2.Text = dr["Address2"].ToString();
                    txtAddress3.Text = dr["Address3"].ToString();
                    txtCity.Text = dr["City"].ToString();
                    txtTelephoneNo.Text = dr["TelephoneNo"].ToString();
                    txtFaxNo.Text = dr["FaxNo"].ToString();
                    txtEmail.Text = dr["Email"].ToString();
                    dtpStartDate.Value = Convert.ToDateTime(dr["StartDateTime"].ToString());
                    txtContactPerson.Text = dr["ContactPerson"].ToString();
                    txtContactPersonDesignation.Text = dr["ContactPersonDesignation"].ToString();
                    txtContactPersonTelephone.Text = dr["ContactPersonTelephone"].ToString();
                    txtContactPersonEmail.Text = dr["ContactPersonEmail"].ToString();
                    txtTINNo.Text = dr["TINNo"].ToString();
                    txtVATRegistrationNo.Text = dr["VATRegistrationNo"].ToString();
                    txtNIDNo.Text = dr["NIDNo"].ToString();
                    txtComments.Text = dr["Comments"].ToString();

                    if (dr["ActiveStatus"].ToString() == "Y")
                    {
                        chkActiveStatus.Checked = true;
                    }
                    else
                    {
                        chkActiveStatus.Checked = false;
                    }

                    txtCountry.Text = dr["Country"].ToString();
                    txtCGType.Text = dr["GroupType"].ToString();
                    txtCustomerCode.Text = dr["CustomerCode"].ToString();
                    txtBusinessType.Text = dr["BusinessType"].ToString();
                    txtBusinessCode.Text = dr["BusinessCode"].ToString();

                    if (dr["IsVDSWithHolder"].ToString() == "Y")
                    {
                        chkVDSWithHolder.Checked = true;
                    }
                    else
                    {
                        chkVDSWithHolder.Checked = false;
                    }

                    searchBranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    chkInstitution.Checked = dr["IsInstitution"].ToString() == "Y" ? true : false;
                    chkIsTax.Checked = dr["IsTax"].ToString() == "Y" ? true : false;
                    txtShortName.Text = dr["ShortName"].ToString();

                    if (dr["IsExamted"].ToString() == "Y")
                    {
                        chkIsExamted.Checked = true;
                    }
                    else
                    {
                        chkIsExamted.Checked = false;
                    }
                    chkTCS.Checked = dr["IsTCS"].ToString() == "Y" ? true : false;

                    chkIsSpecialRate.Checked = dr["IsSpecialRate"].ToString() == "Y" ? true : false;

                }

                #endregion

                #region Customer Address

                fnCustomerAddress(vCustomerId);

                #endregion

                #region Customer Discount

                fnCustomerDiscount(vCustomerId);

                #endregion

                #region Flag Update

                ////btnAdd.Text = "&Save";
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
                FileLogger.Log(this.Name, "btnSearchCustomer_Click", exMessage);
            }
            #endregion

        }

        private void FormCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchCustomer, "Existing Information");
                ToolTip1.SetToolTip(this.btnSearchCustomerGroup, "Existing Information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                dtpStartDate.Value = DateTime.Now;
                ClearAll();

                CustomerGroupSearch();

                #region read only for AutoCode
                CommonDAL commonDal = new CommonDAL();

                string AutoCode = commonDal.settingsDesktop("AutoCode", "Customer", null, connVM);


                if (AutoCode == "Y")
                {
                    //////txtCustomerCode.ReadOnly = true;
                }
                #endregion
                #region ExtraRequiredFeildVisibilty
                string ExtraFeildVisibilty = commonDal.settingsDesktop("Menu", "ExtraRequiredField", null, connVM);
                if (ExtraFeildVisibilty == "N")
                {
                    label19.Visible = false;
                    chkIsExamted.Visible = false;
                    label24.Visible = false;
                    chkIsSpecialRate.Visible = false;
                    label18.Visible = false;
                    chkInstitution.Visible = false;
                }
                #endregion
                //btnAdd.Text = "&Add";
                txtCustomerID.Text = "~~~ New ~~~";
                ChangeData = false;


                string value = new CommonDAL().settingValue("CompanyCode", "Code", connVM);

                btnSync.Visible = false || value.ToLower() == "motorsservice" || value.ToUpper() == "NESTLE" ||
                                  OrdinaryVATDesktop.IsNourishCompany(value) || value.ToLower() == "eon" || value.ToLower() == "eahpl" || value.ToLower() == "eail" || value.ToLower() == "eeufl" || value.ToLower() == "exfl";

                //if (OrdinaryVATDesktop.IsACICompany(value) || value.ToUpper()=="NESTLE")
                //{
                //    //btnSync.Visible = true;
                //}
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
                FileLogger.Log(this.Name, "FormCustomer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormCustomer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormCustomer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormCustomer_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormCustomer_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormCustomer_Load", exMessage);
            }
            #endregion

        }
        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtTINNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtVATRegistrationNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtTelephoneNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtFaxNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress2_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress3_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 03

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonEmail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPerson_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonDesignation_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonTelephone_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void FormCustomer_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormCustomer_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormCustomer_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormCustomer_FormClosing", exMessage);
            }
            #endregion
        }
        private void cmbCustomerGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }
        private void btnAddNew_Click(object sender, EventArgs e)
        {
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
                        CustomerGroupSearch();
                        ClearAll();
                        //btnAdd.Text = "&Add";
                        txtCustomerID.Text = "~~~ New ~~~";
                        ChangeData = false;
                    }

                }
                else if (ChangeData == false)
                {
                    CustomerGroupSearch();
                    ClearAll();
                    //btnAdd.Text = "&Add";
                    txtCustomerID.Text = "~~~ New ~~~";
                    ChangeData = false;
                }
                IsUpdate = false;
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

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                ////MDIMainInterface mdi = new MDIMainInterface();
                FormRptCustomerInformation frmRptCustomerInformation = new FormRptCustomerInformation();

                //mdi.RollDetailsInfo(frmRptCustomerInformation.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptCustomerInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //frmRptCustomerInformation.txtCustomerID.Text = txtCustomerID.Text.Trim();
                //frmRptCustomerInformation.txtCustomerName.Text = txtCustomerName.Text.Trim();
                //frmRptCustomerInformation.txtTINNo.Text = txtTINNo.Text.Trim();
                //frmRptCustomerInformation.txtVATRegistrationNo.Text = txtVATRegistrationNo.Text.Trim();
                frmRptCustomerInformation.ShowDialog();
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
        private void cmbCustomerGroupName_Leave(object sender, EventArgs e)
        {
            try
            {
                var searchText = cmbCustomerGroupName.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var CusGroup = from prd in customerGroups.ToList()
                                   where prd.CustomerGroupName.ToLower() == searchText
                                   select prd;
                    if (CusGroup != null && CusGroup.Any())
                    {
                        var products = CusGroup.First();
                        txtCustomerGroupID.Text = products.CustomerGroupID;
                        txtCGType.Text = products.GroupType;
                    }

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
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbCustomerGroupName_Leave", exMessage);
            }
            #endregion
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods 04

        //=======================ADD CUSTOMERS============
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (Program.CheckLicence(dtpStartDate.Value) == true)
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
                    NextID = txtCustomerID.Text.Trim();
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }


                activestatus = string.Empty;
                activestatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerAdd.RunWorkerAsync();
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
        private void backgroundWorkerAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                // Start DoWork
                //CustomerDAL customerDal = new CustomerDAL();
                ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                //sqlResults = customerDal.InsertToCustomerNew(NextID.ToString(),
                //txtCustomerName.Text.Trim(),
                //txtCustomerGroupID.Text.Trim(),
                //txtAddress1.Text.Trim(),
                //"-",
                //"-",
                //txtCity.Text.Trim(),
                //txtTelephoneNo.Text.Trim(),
                //txtFaxNo.Text.Trim(),
                //txtEmail.Text.Trim(),
                //dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtContactPerson.Text.Trim(),
                //txtContactPersonDesignation.Text.Trim(),
                //txtContactPersonTelephone.Text.Trim(),
                //txtContactPersonEmail.Text.Trim(),
                //txtTINNo.Text.Trim(),
                //txtVATRegistrationNo.Text.Trim(),
                //txtComments.Text.Trim(),
                //activestatus,
                //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtCountry.Text.Trim(),txtCustomerCode.Text.Trim()
                //, txtBusinessType.Text.Trim(), txtBusinessCode.Text.Trim()
                //);

                CustomerVM vm = new CustomerVM();

                vm.CustomerID = NextID.ToString();
                vm.CustomerCode = txtCustomerCode.Text.Trim();
                vm.CustomerName = txtCustomerName.Text.Trim();
                vm.CustomerGroupID = txtCustomerGroupID.Text.Trim();
                vm.Address1 = txtAddress1.Text.Trim();
                vm.Address2 = "-";
                vm.Address3 = "-";
                vm.City = txtCity.Text.Trim();
                vm.TelephoneNo = txtTelephoneNo.Text.Trim();
                vm.FaxNo = txtFaxNo.Text.Trim();
                vm.Email = txtEmail.Text.Trim();
                vm.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.ContactPerson = txtContactPerson.Text.Trim();
                vm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                vm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                vm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                vm.TINNo = txtTINNo.Text.Trim();
                vm.VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
                vm.NIDNo = txtNIDNo.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = activestatus.ToString();
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.Country = txtCountry.Text.Trim();
                vm.BusinessType = txtBusinessType.Text.Trim();
                vm.BusinessCode = txtBusinessCode.Text.Trim();

                vm.IsVDSWithHolder = chkVDSWithHolder.Checked ? "Y" : "N";
                vm.IsExamted = chkIsExamted.Checked ? "Y" : "N";
                vm.IsSpecialRate = chkIsSpecialRate.Checked ? "Y" : "N";
                vm.BranchId = Program.BranchId;
                vm.IsInstitution = chkInstitution.Checked ? "Y" : "N";
                vm.IsTax = chkIsTax.Checked ? "Y" : "N";
                vm.IsTCS = chkTCS.Checked ? "Y" : "N";
                vm.ShortName = txtShortName.Text.Trim();
                sqlResults = customerDal.InsertToCustomerNew(vm, false, null, null, connVM);

                SAVE_DOWORK_SUCCESS = true;
                //ClearAll();
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerAdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                            txtCustomerID.Text = newId;
                            txtCustomerCode.Text = code;
                            fnCustomerAddress(txtCustomerID.Text.Trim());
                        }

                    }
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
            finally
            {
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }
        //=================UPDATE CUSTOMERS===============
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (Program.CheckLicence(DateTime.Now) == true)
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
                    NextID = txtCustomerID.Text.Trim();
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                CommonDAL commonDAL = new CommonDAL();

                string showAllBranch = commonDAL.settingsDesktop("Setup", "ShowAllCustomer", null, connVM);

                if (showAllBranch == "N")
                {
                    if (searchBranchId != Program.BranchId && searchBranchId != 0)
                    {
                        MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }


                activestatus = string.Empty;
                activestatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerUpdate.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion

        }
        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                // Start DoWork
                //CustomerDAL customerDal = new CustomerDAL();
                ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                //sqlResults = customerDal.UpdateToCustomerNew(NextID.ToString(), txtCustomerName.Text.Trim(),
                //txtCustomerGroupID.Text.Trim(), txtAddress1.Text.Trim(),
                //"-", "-", txtCity.Text.Trim(), txtTelephoneNo.Text.Trim(),
                //txtFaxNo.Text.Trim(), txtEmail.Text.Trim(),
                //dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtContactPerson.Text.Trim(), txtContactPersonDesignation.Text.Trim(), txtContactPersonTelephone.Text.Trim(),
                //txtContactPersonEmail.Text.Trim(), txtTINNo.Text.Trim(), txtVATRegistrationNo.Text.Trim(), txtComments.Text.Trim(),
                //activestatus,
                //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtCountry.Text.Trim(),
                //txtCustomerCode.Text.Trim()
                //, txtBusinessType.Text.Trim(), txtBusinessCode.Text.Trim());

                CustomerVM vm = new CustomerVM();

                vm.CustomerID = NextID.ToString();
                vm.CustomerCode = txtCustomerCode.Text.Trim();
                vm.CustomerName = txtCustomerName.Text.Trim();
                vm.CustomerGroupID = txtCustomerGroupID.Text.Trim();
                vm.Address1 = txtAddress1.Text.Trim();
                vm.Address2 = "-";
                vm.Address3 = "-";
                vm.City = txtCity.Text.Trim();
                vm.TelephoneNo = txtTelephoneNo.Text.Trim();
                vm.FaxNo = txtFaxNo.Text.Trim();
                vm.Email = txtEmail.Text.Trim();
                vm.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.ContactPerson = txtContactPerson.Text.Trim();
                vm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                vm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                vm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                vm.TINNo = txtTINNo.Text.Trim();
                vm.VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
                vm.NIDNo = txtNIDNo.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = activestatus.ToString();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.Country = txtCountry.Text.Trim();
                vm.BusinessType = txtBusinessType.Text.Trim();
                vm.BusinessCode = txtBusinessCode.Text.Trim();

                vm.IsVDSWithHolder = chkVDSWithHolder.Checked ? "Y" : "N";
                vm.IsExamted = chkIsExamted.Checked ? "Y" : "N";
                vm.IsSpecialRate = chkIsSpecialRate.Checked ? "Y" : "N";
                vm.IsInstitution = chkInstitution.Checked ? "Y" : "N";
                vm.IsTax = chkIsTax.Checked ? "Y" : "N";
                vm.ShortName = txtShortName.Text.Trim();
                vm.IsTCS = chkTCS.Checked ? "Y" : "N";

                sqlResults = customerDal.UpdateToCustomerNew(vm, connVM);

                UPDATE_DOWORK_SUCCESS = true;
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
            try
            {
                #region Statement

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

                            txtCustomerID.Text = newId;
                            txtCustomerCode.Text = code;

                        }

                    }

                ChangeData = false;
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = false;
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
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }
        //===================DELETE CUSTOMERS=============
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                else if (txtCustomerID.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                else if (txtCustomerID.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                     MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
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
                this.progressBar1.Visible = true;
                backgroundWorkerDelete.RunWorkerAsync();



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
        private int DataAlreadyUsed()
        {
            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                if (commonDal.DataAlreadyUsed("TenderHeaders", "CustomerId", txtCustomerID.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Tender" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                if (commonDal.DataAlreadyUsed("SalesInvoiceHeaders", "CustomerID", txtCustomerID.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Sale" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }

                if (commonDal.DataAlreadyUsed("DutyDrawBackHeader", "CustormerID", txtCustomerID.Text.Trim(), null, null, connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " DDB" + MessageVM.msgDeleteOperationterminated, this.Text);
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
        private void backgroundWorkerDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Statement
                // Start DoWork
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //CustomerDAL customerDal = new CustomerDAL();
                ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);


                CustomerVM vm = new CustomerVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString();
                string[] ids = new string[] { txtCustomerID.Text.Trim(), "" };
                sqlResults = customerDal.Delete(vm, ids, null, null, connVM);
                //sqlResults = customerGroupDal.DeleteCustomerNew(txtCustomerID.Text.Trim(), Program.DatabaseName);
                // End DoWork
                DELETE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion

        }
        private void backgroundWorkerDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Statement
                // Start Complete
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
                            ClearAll();
                            IsUpdate = false;
                            ChangeData = false;
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }

                // End Complete
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
            finally
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }
        //======================SEARCH====================
        private void CustomerGroupSearch()
        {
            try
            {

                this.btnSearchCustomerGroup.Visible = false;
                this.progressBar1.Visible = true;
                backgroundWorkerCustomerGroupSearch.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "CustomerGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CustomerGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CustomerGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "CustomerGroupSearch", exMessage);
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
                FileLogger.Log(this.Name, "CustomerGroupSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CustomerGroupSearch", exMessage);
            }
            #endregion

        }
        private void btnSearchCustomerGroup_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                Program.fromOpen = "Other";
                string result = FormCustomerGroupSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else
                {
                    string[] CustomerGroupinfo = result.Split(FieldDelimeter.ToCharArray());
                    txtCustomerGroupID.Text = CustomerGroupinfo[0];
                    cmbCustomerGroupName.Text = CustomerGroupinfo[1];
                    txtCGType.Text = CustomerGroupinfo[5];
                }
                CustomerGroupSearch();
                customerGroups.Clear();
                cmbCustomerGroupName.Items.Clear();

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
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchCustomerGroup_Click", exMessage);
            }
            #endregion
        }
        private void fnCustomerAddress(string customerId)
        {
            try
            {


                CustomerAddressResult = new DataTable();
                //CustomerDAL customerDal = new CustomerDAL();
                ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                CustomerAddressResult = customerDal.SearchCustomerAddress(customerId, "", "", connVM); // Change 04

                dgvCustomerAddress.Rows.Clear();
                int j = 0;
                foreach (DataRow item in CustomerAddressResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvCustomerAddress.Rows.Add(NewRow);


                    dgvCustomerAddress.Rows[j].Cells["Sl"].Value = (j + 1).ToString();
                    dgvCustomerAddress.Rows[j].Cells["Id"].Value = item["Id"].ToString();
                    dgvCustomerAddress.Rows[j].Cells["CustomerID"].Value = item["CustomerID"].ToString();
                    dgvCustomerAddress.Rows[j].Cells["CustomerAddress"].Value = item["CustomerAddress"].ToString();
                    j = j + 1;

                }
                txtCustomerAddress.Text = "";
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
        }
        private void fnCustomerDiscount(string customerId)
        {
            try
            {


                CustomerDiscountResult = new DataTable();
                //CustomerDiscountDAL DiscountDal = new CustomerDiscountDAL();
                ICustomerDiscount DiscountDal = OrdinaryVATDesktop.GetObject<CustomerDiscountDAL, CustomerDiscountRepo, ICustomerDiscount>(OrdinaryVATDesktop.IsWCF);


                CustomerDiscountResult = DiscountDal.SearchCustomerDiscount(customerId, "", "", connVM); // Change 04

                dgvCustomerDiscount.Rows.Clear();
                int j = 0;
                foreach (DataRow item in CustomerDiscountResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvCustomerDiscount.Rows.Add(NewRow);


                    dgvCustomerDiscount.Rows[j].Cells["DiscountId"].Value = item["Id"].ToString();
                    dgvCustomerDiscount.Rows[j].Cells["NCustomerID"].Value = item["CustomerID"].ToString();
                    dgvCustomerDiscount.Rows[j].Cells["MinValue"].Value = Program.ParseDecimalObject(item["MinValue"].ToString());
                    dgvCustomerDiscount.Rows[j].Cells["MaxValue"].Value = Program.ParseDecimalObject(item["MaxValue"].ToString());
                    dgvCustomerDiscount.Rows[j].Cells["Rate"].Value = Program.ParseDecimalObject(item["Rate"].ToString());
                    dgvCustomerDiscount.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    j = j + 1;

                }
                txtMinValue.Text = "";
                txtMaxValue.Text = "";
                txtRate.Text = "";
                txtDescription.Text = "";
                txtDiscountId.Text = "";

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 05

        private void backgroundWorkerCustomerGroupSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            try
            {
                #region Statement

                CustomerGroupResult = new DataTable();
                //CustomerGroupDAL customerGroupDal = new CustomerGroupDAL();
                ICustomerGroup customerGroupDal = OrdinaryVATDesktop.GetObject<CustomerGroupDAL, CustomerGroupRepo, ICustomerGroup>(OrdinaryVATDesktop.IsWCF);

                //CustomerGroupResult = customerGroupDal.SearchCustomerGroupNew("", "", "", "Y", "", Program.DatabaseName); // Change 04
                string[] cValues = { "Y" };
                string[] cFields = { "ActiveStatus like" };
                CustomerGroupResult = customerGroupDal.SelectAll(0, cFields, cValues, null, null, true, connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerCustomerGroupSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            try
            {
                //dgvCustomerAddress.DataSource = CustomerAddressResult;

                cmbCustomerGroupName.Items.Clear();
                foreach (DataRow item2 in CustomerGroupResult.Rows)
                {
                    var cGroup = new CustomerGroupDTO();
                    cGroup.CustomerGroupID = item2["CustomerGroupID"].ToString();
                    cGroup.CustomerGroupName = item2["CustomerGroupName"].ToString();
                    cGroup.CustomerGroupDescription = item2["CustomerGroupDescription"].ToString();
                    cGroup.Comments = item2["Comments"].ToString();
                    cGroup.ActiveStatus = item2["ActiveStatus"].ToString();
                    cGroup.GroupType = item2["GroupType"].ToString();
                    cmbCustomerGroupName.Items.Add(item2["CustomerGroupName"]);
                    customerGroups.Add(cGroup);
                }
                if (cmbCustomerGroupName.Items.Count <= 0)
                {
                    MessageBox.Show("Please input Customer Group first", this.Text);
                    //this.Close();
                    return;
                }
                cmbCustomerGroupName.SelectedIndex = 0;
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
            finally
            {
                ChangeData = false;
                this.btnSearchCustomerGroup.Visible = true;
                this.progressBar1.Visible = false;
            }

        }

        private void txtCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
            //btnAdd.Focus();
        }

        private void txtCustomerCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCountry_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtCountry_Leave(object sender, EventArgs e)
        {
            btnAdd.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormDBBackupRestore dbBackupRestore = new FormDBBackupRestore();
            dbBackupRestore.Show();
        }

        private void dgvCustomerAddress_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvCustomerAddress.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                vCustomerAddressId = userSelRow.Cells["Id"].Value.ToString();
                vCustomerId = userSelRow.Cells["CustomerID"].Value.ToString();
                txtCustomerAddress.Text = userSelRow.Cells["CustomerAddress"].Value.ToString();
            }
        }

        private void dgvCustomerAddress_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search customer first");
                    return;
                }

                sqlResults = new string[4];

                CustomerAddressVM vm = new CustomerAddressVM();
                vm.Id = Convert.ToInt32(vCustomerAddressId);
                vm.CustomerID = txtCustomerID.Text.Trim();
                vm.CustomerAddress = txtCustomerAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                sqlResults = cDal.InsertToCustomerAddress(vm, null, null, connVM);
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
                    fnCustomerAddress(vm.CustomerID);

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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {


                sqlResults = new string[4];

                CustomerAddressVM vm = new CustomerAddressVM();
                vm.Id = Convert.ToInt32(vCustomerAddressId);
                vm.CustomerID = vCustomerId;
                vm.CustomerAddress = txtCustomerAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                sqlResults = cDal.UpdateToCustomerAddress(vm, null, null, connVM);
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
                    fnCustomerAddress(vCustomerId);

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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {


                sqlResults = new string[4];

                CustomerAddressVM vm = new CustomerAddressVM();
                vm.Id = Convert.ToInt32(vCustomerAddressId);
                vm.CustomerID = vCustomerId;
                vm.CustomerAddress = txtCustomerAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                sqlResults = cDal.DeleteCustomerAddress("", vm.Id.ToString(), null, null, connVM);
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
                    fnCustomerAddress(vCustomerId);

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

        private void btnChange_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 06

        private void txtProductStockValue_Leave(object sender, EventArgs e)
        {

        }

        private void txtProductStockQuantity_Leave(object sender, EventArgs e)
        {

        }

        private void btnProductStockRemove_Click(object sender, EventArgs e)
        {

        }

        private void btnProductStockAdd_Click(object sender, EventArgs e)
        {

        }

        private void txtRate_Leave(object sender, EventArgs e)
        {

        }

        private void txtMaxValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMaxValue_Leave(object sender, EventArgs e)
        {

        }

        private void txtMinValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMinValue_Leave(object sender, EventArgs e)
        {

        }

        private void MinValue_Click(object sender, EventArgs e)
        {

        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void Description_Click(object sender, EventArgs e)
        {

        }

        private void Comments_Click(object sender, EventArgs e)
        {

        }

        private void Rate_Click(object sender, EventArgs e)
        {

        }

        private void MaxValue_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 07

        private void btnDiscountAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Customer first");
                    return;
                }

                string DiscountMaxValue = txtMaxValue.Text;
                string DiscountMinValue = txtMinValue.Text;

                string DiscountRateValue = txtRate.Text;



                if (DiscountMaxValue == "" || DiscountMinValue == "")
                {
                    if (DiscountMaxValue == "" && DiscountMaxValue == "")
                    {
                        MessageBox.Show("Entered Discount Max Value & Min Value first !", this.Text);
                        return;
                    }

                    else if (DiscountMinValue == "")
                    {
                        MessageBox.Show("Entered a Min Value first !", this.Text);
                        return;
                    }
                    else if (DiscountMaxValue == "")
                    {
                        MessageBox.Show("Entered a Max Value first !", this.Text);
                        return;
                    }

                }
                if (txtDescription.Text == "")
                {
                    txtDescription.Text = "NA";
                }


                CustomerDiscountVM vm = new CustomerDiscountVM();
                vm.MinValue = Convert.ToDecimal(DiscountMinValue);
                vm.MaxValue = Convert.ToDecimal(DiscountMaxValue);
                vm.Rate = Convert.ToDecimal(DiscountRateValue);
                vm.Comments = txtDescription.Text;
                vm.CustomerID = vCustomerId;

                //CustomerDiscountDAL pDal = new CustomerDiscountDAL();
                ICustomerDiscount pDal = OrdinaryVATDesktop.GetObject<CustomerDiscountDAL, CustomerDiscountRepo, ICustomerDiscount>(OrdinaryVATDesktop.IsWCF);

                sqlResults = pDal.InsertToCustomerDiscountNew(vm, false, null, null, connVM);

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                }

                fnCustomerDiscount(vCustomerId);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void dgvCustomerDiscount_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvCustomerDiscount.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                vCustomerId = userSelRow.Cells["NCustomerID"].Value.ToString();
                txtMinValue.Text = userSelRow.Cells["MinValue"].Value.ToString();
                txtMaxValue.Text = userSelRow.Cells["MaxValue"].Value.ToString();
                txtRate.Text = userSelRow.Cells["Rate"].Value.ToString();
                txtDescription.Text = userSelRow.Cells["Comments"].Value.ToString();
                txtDiscountId.Text = userSelRow.Cells["DiscountId"].Value.ToString();
            }
        }

        private void btnChange_Click_1(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(txtDiscountId.Text))
                {
                    MessageBox.Show("Please Select A Raw  first");
                    dgvCustomerDiscount.Focus();
                    return;
                }

                sqlResults = new string[4];
                CustomerDiscountVM vm = new CustomerDiscountVM();
                vm.MinValue = Convert.ToDecimal(txtMinValue.Text);
                vm.MaxValue = Convert.ToDecimal(txtMaxValue.Text);
                vm.Rate = Convert.ToDecimal(txtRate.Text);
                vm.Comments = txtDescription.Text;
                vm.CustomerID = vCustomerId;
                vm.Id = Convert.ToInt32(txtDiscountId.Text);

                //CustomerDiscountDAL pDal = new CustomerDiscountDAL();
                ICustomerDiscount pDal = OrdinaryVATDesktop.GetObject<CustomerDiscountDAL, CustomerDiscountRepo, ICustomerDiscount>(OrdinaryVATDesktop.IsWCF);

                sqlResults = pDal.UpdateToCustomerDiscountNew(vm, connVM);
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
                    fnCustomerDiscount(vCustomerId);

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

        private void btnDiscountRemove_Click(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(txtDiscountId.Text))
                {
                    MessageBox.Show("Please Select A Raw  first");
                    dgvCustomerDiscount.Focus();
                    return;
                }

                sqlResults = new string[4];
                CustomerDiscountVM vm = new CustomerDiscountVM();
                //vm.MinValue = Convert.ToDecimal(txtMinValue.Text);
                //vm.MaxValue = Convert.ToDecimal(txtMaxValue.Text);
                //vm.Rate = Convert.ToDecimal(txtRate.Text);
                //vm.Comments = txtDescription.Text;
                vm.CustomerID = vCustomerId;

                vm.Id = Convert.ToInt32(txtDiscountId.Text);

                //CustomerDiscountDAL pDal = new CustomerDiscountDAL();
                ICustomerDiscount pDal = OrdinaryVATDesktop.GetObject<CustomerDiscountDAL, CustomerDiscountRepo, ICustomerDiscount>(OrdinaryVATDesktop.IsWCF);

                sqlResults = pDal.DeleteCustomerDiscount("", vm.Id.ToString(), null, null, connVM);
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
                    fnCustomerDiscount(vCustomerId);

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void bgwCustomerSync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (results[0].ToLower() == "success")
            {
                MessageBox.Show("Synchronized");
            }
            else if (results[0].ToLower() == "exception")
            {
                MessageBox.Show(results[1]);
            }
            else
            {
                MessageBox.Show("Nothing to sync");
            }

            progressBar1.Visible = false;

        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                bgwCustomerSync.RunWorkerAsync();

            }
            catch (Exception exception)
            {

            }
            finally
            {

            }
        }

        private void bgwCustomerSync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();
                CommonDAL commonDAL = new CommonDAL();
                DataTable customerDt = new DataTable();

                results[0] = "fail";
                string code = commonDAL.settingValue("CompanyCode", "Code");

                if (OrdinaryVATDesktop.IsACICompany(code))
                {
                    customerDt = importDal.GetCustomerACIDbData(settingVM.BranchInfoDT, connVM);
                }
                else if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                {
                    customerDt = importDal.GetCustomerNestleDbData(settingVM.BranchInfoDT, connVM);

                }
                else if (OrdinaryVATDesktop.IsNourishCompany(code))
                {
                    NourishIntegrationDAL nourishIntegrationDal = new NourishIntegrationDAL();

                    customerDt = nourishIntegrationDal.GetCustomer(settingVM.BranchInfoDT, connVM);

                }

                else if (code.ToLower() == "eon" || code.ToLower() == "eahpl" || code.ToLower() == "eail" || code.ToLower() == "eeufl" || code.ToLower() == "exfl")              
                {
                    EONIntegrationDAL eonIntegrationDal = new EONIntegrationDAL();

                    customerDt = eonIntegrationDal.GetCustomerDataAPI(settingVM.BranchInfoDT, connVM);

                }

                List<CustomerVM> customers = new List<CustomerVM>();

                int rowsCount = customerDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonDAL.settingsDesktop("AutoSave", "DefaultCustomerGroup", null, connVM);

                FileLogger.Log(this.Name, "Integration Customer", customerDt.Rows.Count + "");

                for (int i = 0; i < rowsCount; i++)
                {
                    CustomerVM customer = new CustomerVM();

                    customer.CustomerName =
                        Program.RemoveStringExpresion(customerDt.Rows[i]["CustomerName"].ToString());
                    customer.CustomerCode =
                        Program.RemoveStringExpresion(customerDt.Rows[i]["CustomerCode"].ToString());
                    customer.CustomerGroup = customerDt.Rows[i]["CustomerGroup"].ToString();
                    customer.Address1 = customerDt.Rows[i]["Address"].ToString();

                    if (customer.CustomerGroup == "-")
                    {
                        if (defaultGroup == "-")
                        {
                            throw new Exception("Default Customer Group Not Found.\nPlease set Default Customer Group in Setting .");
                        }
                        customer.CustomerGroup = defaultGroup;
                    }

                    customer.City = "-";
                    customer.TelephoneNo = "-";


                    if (OrdinaryVATDesktop.IsACICompany(code) || OrdinaryVATDesktop.IsNourishCompany(code))
                    {
                        customer.FaxNo = "-";
                        customer.Email = "-";
                        customer.TINNo = "-";
                        customer.ContactPerson = "-";
                        customer.ContactPersonTelephone = "-";

                    }
                    else if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                    {
                        customer.FaxNo = customerDt.Rows[i]["FaxNo"].ToString(); ;
                        customer.Email = customerDt.Rows[i]["Email"].ToString(); ;
                        customer.TINNo = customerDt.Rows[i]["TINNo"].ToString(); ;
                        customer.ContactPerson = Program.RemoveStringExpresion(customerDt.Rows[i]["ContactPerson"].ToString()); ;
                        customer.ContactPersonTelephone = customerDt.Rows[i]["ContactPersonTelephone"].ToString(); ;
                    }


                    customer.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    customer.ContactPersonDesignation = "-";
                    customer.ContactPersonEmail = "-";


                    customer.VATRegistrationNo = customerDt.Rows[i]["BIN_No"].ToString();
                    customer.Comments = "-";
                    customer.ActiveStatus = "Y";
                    customer.CreatedBy = Program.CurrentUser;
                    customer.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    customer.Country = "-";
                    customer.IsVDSWithHolder = "N";
                    customer.BranchId = Program.BranchId;
                    customer.IsInstitution = "N";
                    customers.Add(customer);

                    ids.Add(customerDt.Rows[i]["SL"].ToString());
                }


                results = importDal.ImportCustomers(customers);

                if (results[0].ToLower() == "success")
                {

                    FileLogger.Log(this.Name, "Integration Customer", string.Join("\n", ids) + "");

                    if (OrdinaryVATDesktop.IsACICompany(code))
                    {
                        results = importDal.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Customers", connVM);
                    }
                    else if (string.Equals(code, "nestle", StringComparison.OrdinalIgnoreCase))
                    {
                        results = importDal.UpdateNestleMaster(ids, settingVM.BranchInfoDT, "Customers", connVM);
                    }
                }


            }
            catch (Exception ex)
            {
                results[0] = "exception";
                results[1] = ex.Message;

                FileLogger.Log(this.Name, "Integration Customer",
                    ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {

            }
        }

        private void txtMinValue_Leave_1(object sender, EventArgs e)
        {
            txtMinValue.Text = Program.ParseDecimalObject(txtMinValue.Text.Trim()).ToString();
        }

        private void txtMaxValue_Leave_1(object sender, EventArgs e)
        {
            txtMaxValue.Text = Program.ParseDecimalObject(txtMaxValue.Text.Trim()).ToString();
        }

        private void txtRate_Leave_1(object sender, EventArgs e)
        {
            txtRate.Text = Program.ParseDecimalObject(txtRate.Text.Trim()).ToString();
        }

        #endregion

        #region Navigation


        private void btnFirst_Click(object sender, EventArgs e)
        {
            CustomerNavigation("First");
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            CustomerNavigation("Previous");

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            CustomerNavigation("Next");

        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            CustomerNavigation("Last");

        }

        private void txtCustomerCode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void CustomerNavigation(string ButtonName)
        {
            try
            {
                CustomerDAL _CustomerDAL = new CustomerDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.Code = txtSearchCustomerCode.Text.Trim();

                vm = _CustomerDAL.Customer_Navigation(vm, null, null, connVM);

                txtCustomerID.Text = vm.Id.ToString();

                SearchCustomer();

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

        private void txtSearchCustomerCode_DoubleClick(object sender, EventArgs e)
        {

        }

        private void txtSearchCustomerCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                CustomerNavigation("Current");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
