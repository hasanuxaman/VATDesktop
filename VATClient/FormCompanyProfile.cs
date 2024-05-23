using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Utilities;
//
using VATClient.ReportPreview;
using VATServer.Library;
using VATServer.License;
using VATViewModel;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormCompanyProfile : Form
    {
        #region Constructors

        public FormCompanyProfile()
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

        // ----------- Declare from DBConstant Start--------//
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        // ----------- Declare from DBConstant End--------//
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        private string encriptedFiscalYearData;
        private string encriptedCompanyProfileData;
        private string CompanyProfileData;
        private bool ChangeData = false;
        //string NextID;
        private string result = string.Empty;
        public string VFIN = "181";
        private DataTable companyResult;
        private DataSet ReportResult;
        private string DBName = string.Empty;
        private string DBName2 = string.Empty;
        private string NextID = string.Empty;
        private bool IsSymphonyUser = false;

        private CompanyProfileVM companyProfiles = new CompanyProfileVM();
        private List<FiscalYearVM> fiscalyears = new List<FiscalYearVM>();

        #region sql save, update, delete

        

        #endregion

        #endregion

        private void frmCompanyProfile_Load(object sender, EventArgs e)
        {
            try
            {
                
                formMaker();
                if (btnNewCompany.Visible == true)
                {

                    btnCancel.Enabled = true;
                    label29.Visible = false;
                    label33.Visible = false;
                    cmbCompanyType.Visible = false;
                    ClearAll();
                }
                else if (btnUpdate.Visible == true)
                {
                    #region Company Type

                    string[] Condition = new string[] { "1=1" };
                    cmbCompanyType = new CommonDAL().ComboBoxLoad(cmbCompanyType, "CompanyCategory", "CATEGORY_ID", "CATEGORY", Condition, "varchar", true, false, null, true);
                    #endregion

                   
                   if (DialogResult.No != MessageBox.Show("Are you Symphony user?", this.Text, MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question,
                                                           MessageBoxDefaultButton.Button2))
                    {
                        IsSymphonyUser = FormSupperAdministrator.SelectOne();
                        if (!IsSymphonyUser)
                        {
                            txtCompanyLegalName.ReadOnly = true;
                            txtVatRegistrationNo.ReadOnly = true;
                            txtCompanyName.ReadOnly = true;
                            btnSearch.Visible = false;
                            txtCode.Visible = false;
                        }
                        

                    }
                    else
                    {
                        txtCompanyLegalName.ReadOnly = true;
                        txtVatRegistrationNo.ReadOnly = true;
                        txtCompanyName.ReadOnly = true;
                        btnSearch.Visible = false;
                        txtCode.Visible = false;
                    }

                    this.progressBar1.Enabled = true;
                    btnCancel.Enabled = false;
                    bgwLoad.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "frmCompanyProfile_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "frmCompanyProfile_Load", exMessage);
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
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "frmCompanyProfile_Load", exMessage);
            }
            #endregion
        }
        private void bgwLoad_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                companyResult= new DataTable();
                CompanyprofileDAL companyprofileDal = new CompanyprofileDAL();
                //ICompanyprofile companyprofileDal = OrdinaryVATDesktop.GetObject<CompanyprofileDAL, CompanyprofileRepo, ICompanyprofile>(OrdinaryVATDesktop.IsWCF);

                companyResult = companyprofileDal.SearchCompanyProfile(connVM); // Change 04
                //done
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
        private void bgwLoad_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                foreach (DataRow item in companyResult.Rows)
                {
                    //string NewCompanyID = "";
                    //string NewCompanyName = "";
                    //string NewCompanyLegalName = "";
                    //string NewVatRegistrationNo = "";

                    //try
                    //{
                    //   var NewCompanyID1 = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyId"].ToString());
                    //}
                    //catch (Exception)
                    //{
                    //    NewCompanyID = item["CompanyId"].ToString();
                    //}
                    //try
                    //{
                    //    NewCompanyName = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyName"].ToString());
                    //}
                    //catch (Exception)
                    //{
                    //     NewCompanyName = item["CompanyName"].ToString();
                    //}
                    //try
                    //{
                    //    NewCompanyLegalName = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyLegalName"].ToString());
                    //}
                    //catch (Exception)
                    //{
                    //   NewCompanyLegalName = item["CompanyLegalName"].ToString();
                    //}
                    //try
                    //{
                    //    NewVatRegistrationNo = Converter.DESDecrypt(PassPhrase, EnKey, item["VatRegistrationNo"].ToString());
                    //}
                    //catch (Exception)
                    //{
                    //    NewVatRegistrationNo =item["VatRegistrationNo"].ToString();
                    //}
                    //string NewCompanyID = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyId"].ToString());
                    //string NewCompanyName = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyName"].ToString());
                    //string NewCompanyLegalName = Converter.DESDecrypt(PassPhrase, EnKey, item["CompanyLegalName"].ToString());
                    //string NewVatRegistrationNo = Converter.DESDecrypt(PassPhrase, EnKey, item["VatRegistrationNo"].ToString());


                    txtCompanyID.Text = item["CompanyID"].ToString();// CompanyProfileFields[0].ToString();
                    txtCompanyName.Text =item["CompanyName"].ToString() ;//CompanyProfileFields[1].ToString();
                    txtCompanyLegalName.Text = item["CompanyLegalName"].ToString();//CompanyProfileFields[2].ToString();
                    txtAddress1.Text = item["Address1"].ToString();//CompanyProfileFields[3].ToString();
                    txtAddress2.Text = item["Address2"].ToString();//CompanyProfileFields[4].ToString();
                    txtAddress3.Text = item["Address3"].ToString();//CompanyProfileFields[5].ToString();
                    txtCity.Text = item["City"].ToString();//CompanyProfileFields[6].ToString();
                    txtZipCode.Text = item["ZipCode"].ToString();//CompanyProfileFields[7].ToString();
                    txtTelephoneNo.Text = item["TelephoneNo"].ToString();//CompanyProfileFields[8].ToString();
                    txtFaxNo.Text = item["FaxNo"].ToString();//CompanyProfileFields[9].ToString();
                    txtEmail.Text = item["Email"].ToString();//CompanyProfileFields[10].ToString();
                    txtContactPerson.Text = item["ContactPerson"].ToString();//CompanyProfileFields[11].ToString();
                    txtContactPersonDesignation.Text = item["ContactPersonDesignation"].ToString();//CompanyProfileFields[12].ToString();
                    txtContactPersonTelephone.Text = item["ContactPersonTelephone"].ToString();//CompanyProfileFields[13].ToString();
                    txtContactPersonEmail.Text = item["ContactPersonEmail"].ToString();//CompanyProfileFields[14].ToString();
                    txtVatRegistrationNo.Text = item["VatRegistrationNo"].ToString();//CompanyProfileFields[15].ToString();
                    txtTINNo.Text = item["TINNo"].ToString();//CompanyProfileFields[16].ToString();
                    txtComments.Text = item["Comments"].ToString();//CompanyProfileFields[17].ToString();
                    chkActiveStatus.Checked = item["ActiveStatus"].ToString() == "Y" ? true : false;
                    dtpStartDate.Value =Convert.ToDateTime(item["StartDateTime"].ToString());//CompanyProfileFields[19]);
                    dtpFYearStart.Value = Convert.ToDateTime(item["FYearStart"].ToString());//CompanyProfileFields[20]);
                    dtpFYearEnd.Value = Convert.ToDateTime(item["FYearEnd"].ToString());//CompanyProfileFields[21]);

                    chkVDSWithHolder.Checked = item["IsVDSWithHolder"].ToString() == "Y" ? true : false;
                    txtBusinessNature.Text = item["BusinessNature"].ToString();
                    txtAccountingNature.Text = item["AccountingNature"].ToString();
                    txtBIN.Text = item["BIN"].ToString();
                    txtCode.Text = item["License"].ToString();
                    cmbCompanyType.SelectedValue = item["CompanyType"].ToString();
                    txtSection.Text = item["Section"].ToString();

                }


                // End Complete
                dtpStartDate.Enabled = false;
                dtpFYearStart.Enabled = false;
                dtpFYearEnd.Enabled = false;
                #endregion
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
                this.progressBar1.Enabled = false;
            }
            //this.btnSearchReceiveNo.Enabled = true;
            this.progressBar1.Enabled = false;
        }
        private void formMaker()
        {
           
            #region ExtraRequiredFeildVisibilty
            CommonDAL commonDal = new CommonDAL();
            string ExtraFeildVisibilty = commonDal.settingsDesktop("Menu", "ExtraRequiredField",null,connVM);
            if (ExtraFeildVisibilty == "N")
            {
                chkVDSWithHolder.Visible = false;
            }
            #endregion

            btnNewCompany.Left = btnUpdate.Left;
            btnNewCompany.Top = btnUpdate.Top;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void picCompanyID_Click(object sender, EventArgs e)
        {

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
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
                    ClearAll();
                    ChangeData = false;
                    //return;
                }
            }
            if (ChangeData == false)
            {

                ClearAll();
                ChangeData = false;
            }
        }
        public int ErrorReturn()
        {
           
            if (dtpStartDate.Value.ToString("dd/MMM/yyyy") == "01/Jan/1900")
            {
                MessageBox.Show("Please check Company Start Date.");
                dtpStartDate.Focus();
                return 1;
            }

            if (dtpFYearStart.Value.ToString("dd/MMM/yyyy") == "01/Jan/1900")
            {
                MessageBox.Show("Please check Fiscal Year Start Date.");
                dtpFYearStart.Focus();
                return 1;
            }
            if (dtpFYearStart.Value.ToString("MMM") != "Jul" && dtpFYearStart.Value.ToString("MMM") != "Jan")
            {
                MessageBox.Show("Please check Fiscal Year Start Date.");
                dtpFYearStart.Focus();
                return 1;
            }
            if (txtCompanyName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Company Name.");
                txtCompanyName.Focus();
                return 1;
            }
            if (txtCompanyLegalName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Company Legal Name.");
                txtCompanyLegalName.Focus();
                return 1;
            }
            if (txtVatRegistrationNo.Text.Trim() == "")
            {
                MessageBox.Show("Please enter VAT Reg Number.");
                txtVatRegistrationNo.Focus();
                return 1;
            }
                if (btnNewCompany.Visible == false)
                {
                    if (cmbCompanyType.Text.Trim() == "" || cmbCompanyType.Text.Trim().ToLower() == "select")
                    {
                        MessageBox.Show("Please Select Company Type.");
                        cmbCompanyType.Focus();
                        return 1;
                    }
                }
         
            //if (Convert.ToInt32(txtVatRegistrationNo.Text.Trim().Length) == 11 || Convert.ToInt32(txtVatRegistrationNo.Text.Trim().Length) == 9)
            //{
            //    //MessageBox.Show("Please enter 11 digits VAT Reg Number. ro Please enter 9 digits BIN Number.");
            //    //txtVatRegistrationNo.Focus();
            //    //return 1;
            //}
            //else
            //{
            //    MessageBox.Show("Please enter 11 digits VAT Reg Number. ro Please enter 9 digits BIN Number.");
            //    txtVatRegistrationNo.Focus();
            //    return 1;
            //}
            if (txtTelephoneNo.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Telephone number");
                txtTelephoneNo.Focus();
                return 1;
            }
            if (txtFaxNo.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Fax number");
                txtFaxNo.Focus();
                return 1;
            }
            if (txtAddress1.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Address");
                txtAddress1.Focus();
                return 1;
            }
            //if (txtAddress2.Text.Trim() == "")
            //{
            //    MessageBox.Show("Please enter Address");
            //    txtAddress2.Focus();
            //    return 1;
            //}
            //if (txtAddress3.Text.Trim() == "")
            //{
            //    MessageBox.Show("Please enter Address");
            //    txtAddress3.Focus();
            //    return 1;
            //}
            if (txtTINNo.Text.Trim() == "")
            {
                txtTINNo.Text = "-";
            }
            if (txtContactPerson.Text.Trim() == "")
            {
                txtContactPerson.Text = "-";
            }
            if (txtContactPersonDesignation.Text.Trim() == "")
            {
                txtContactPersonDesignation.Text = "-";
            }
            if (txtContactPersonTelephone.Text.Trim() == "")
            {
                txtContactPersonTelephone.Text = "-";
            }
            if (txtContactPersonEmail.Text.Trim() == "")
            {
                txtContactPersonEmail.Text = "-";
            }

            if (txtEmail.Text.Trim() == "")
            {
                txtEmail.Text = "-";
            }

            if (txtCity.Text.Trim() == "")
            {
                txtCity.Text = "-";
            }
            if (txtZipCode.Text.Trim() == "")
            {
                txtZipCode.Text = "-";
            }
            if (txtComments.Text.Trim() == "")
            {
                txtComments.Text = "-";
            }
            if (txtSection.Text.Trim() == "")
            {
                txtSection.Text = "-";
            }

            return 0;
        }
        private void ClearAll()
        {
            txtCompanyID.Text = "";
            txtCompanyName.Text = "";
            txtCompanyLegalName.Text = "";
            txtAddress1.Text = "";
            //txtAddress2.Text = "-";
            //txtAddress3.Text = "-";
            txtCity.Text = "";
            txtZipCode.Text = "";
            txtTelephoneNo.Text = "";
            txtFaxNo.Text = "";
            txtEmail.Text = "";
            txtContactPerson.Text = "";
            txtContactPersonDesignation.Text = "";
            txtContactPersonTelephone.Text = "";
            txtContactPersonEmail.Text = "";
            txtTINNo.Text = "";
            txtVatRegistrationNo.Text = "";
            txtComments.Text = "";

            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);

            dtpStartDate.Value = startDate;
            dtpFYearStart.Value = startDate;

            
            
        }

        #region TextBox KeyDown Event


        private void txtCompanyID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCompanyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCompanyLegalName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVatRegistrationNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtTINNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtContactPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtContactPersonDesignation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtContactPersonTelephone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtContactPersonEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtTelephoneNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtFaxNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAddress2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAddress3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtZipCode_KeyDown(object sender, KeyEventArgs e)
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

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ReportShowDs();
        }

        private void ReportShowDs()
        {

            try
            {
                this.btnPrintList.Enabled = false;
                this.progressBar1.Enabled = true;
                bgwReport.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion


        }

        private void bgwReport_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // Start DoWork
                ReportResult = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                //IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ReportResult = reportDsdal.ComapnyProfile(txtCompanyID.Text,connVM);
                //done

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
                FileLogger.Log(this.Name, "bgwReport_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwReport_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwReport_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwReport_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwReport_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReport_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReport_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwReport_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwReport_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                // Start Complete
                ReportResult.Tables[0].TableName = "DsCompanyProfile";
                RptComapnyProfile objrpt = new RptComapnyProfile();

                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Company Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'"; objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();


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
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwReport_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {


                this.btnPrintList.Enabled = true;
                this.progressBar1.Enabled = false;
            }
        }

        private void txtCompanyName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCompanyLegalName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVatRegistrationNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTINNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

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

        private void txtContactPersonEmail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtFaxNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtZipCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormCompanyProfile_FormClosing(object sender, FormClosingEventArgs e)
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
            #endregion
        }

        private void FormCompanyProfile_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        private void AddFiscalYear()
        {
            try
            {
                dtpFYearStart.Value = Convert.ToDateTime(dtpFYearStart.Value.ToString("dd/MMM/yyyy"));
                dgvFYear.Rows.Clear();
                for (int j = 0; j < Convert.ToInt32(12); j++)
                {
                    DateTime a =
                        Convert.ToDateTime(dtpFYearStart.Value.ToString("MMMM-yyyy")).AddMonths(j);


                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvFYear.Rows.Add(NewRow);

                    dgvFYear.Rows[j].Cells["LineNo"].Value = Convert.ToDecimal(j + 1);
                    dgvFYear.Rows[j].Cells["MonthName"].Value = Convert.ToDateTime(dtpFYearStart.Value.ToString("MMMM-yyyy")).AddMonths(j);
                    dgvFYear.Rows[j].Cells["PeriodStart"].Value = Convert.ToDateTime(dtpFYearStart.Value.ToString("dd/MMM/yyyy")).AddMonths(j);
                    dgvFYear.Rows[j].Cells["PeriodEnd"].Value = Convert.ToDateTime(dtpFYearStart.Value.ToString("dd/MMM/yyyy")).AddMonths(j + 1).AddDays(-1);

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
            #endregion
        }

        private void FiscalYearSave()
        {
            try
            {
                fiscalyears.Clear();
                for (int i = 0; i < dgvFYear.RowCount; i++)
                {
                    FiscalYearVM detail = new FiscalYearVM();

                    detail.FiscalYearName = dtpFYearStart.Value.ToString("dd/MMM/yyyy") + " To " + dtpFYearEnd.Value.ToString("dd/MMM/yyyy");
                    detail.CurrentYear = dtpFYearEnd.Value.ToString("yyyy");
                    detail.PeriodID = Convert.ToDateTime(dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString("MMyyyy");
                    detail.PeriodName = Convert.ToDateTime(dgvFYear.Rows[i].Cells["MonthName"].Value).ToString("MMMM-yyyy");
                    detail.PeriodStart = Convert.ToDateTime(dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString("yyyy-MMM-dd");
                    detail.PeriodEnd = Convert.ToDateTime(dgvFYear.Rows[i].Cells["PeriodEnd"].Value).ToString("yyyy-MMM-dd");
                    detail.PeriodLock = Convert.ToString("N");
                    detail.GLLock = Convert.ToString("N");
                    detail.CreatedBy = Program.CurrentUser;
                    detail.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.LastModifiedBy = Program.CurrentUser;
                    detail.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    fiscalyears.Add(detail);

                }

                //string FiscalYearData = string.Empty;
                //for (int i = 0; i < dgvFYear.RowCount; i++)
                //{
                //    if (FiscalYearData == string.Empty)
                //    {
                //        FiscalYearData =

                //            dtpFYearStart.Value.ToString("dd/MMM/yyyy") + " To " +dtpFYearEnd.Value.ToString("dd/MMM/yyyy") + FieldDelimeter +
                //            dtpFYearEnd.Value.ToString("yyyy") + FieldDelimeter +
                //            dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString("MMyyyy") +FieldDelimeter +
                //            dgvFYear.Rows[i].Cells["MonthName"].Value).ToString("MMMM-yyyy") +FieldDelimeter +
                //            dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString("yyyy-MMM-dd") +FieldDelimeter +
                //            dgvFYear.Rows[i].Cells["PeriodEnd"].Value).ToString("yyyy-MMM-dd") +FieldDelimeter +
                //            Convert.ToString("N") + FieldDelimeter +
                //            Convert.ToString("N") + FieldDelimeter +
                //            Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") +FieldDelimeter +
                //            Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") +FieldDelimeter;

                //    }
                //    else
                //    {
                //        FiscalYearData = FiscalYearData +

                //                         dtpFYearStart.Value.ToString("dd/MMM/yyyy") + " To " +
                //                         dtpFYearStart.Value.ToString("dd/MMM/yyyy") + FieldDelimeter +
                //                         dtpFYearStart.Value.ToString("yyyy") + FieldDelimeter +
                //                         dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString(
                //                             "MMyyyy") + FieldDelimeter +
                //                         dgvFYear.Rows[i].Cells["MonthName"].Value).ToString(
                //                             "MMMM-yyyy") + FieldDelimeter +
                //                         dgvFYear.Rows[i].Cells["PeriodStart"].Value).ToString(
                //                             "yyyy-MMM-dd") + FieldDelimeter +
                //                         dgvFYear.Rows[i].Cells["PeriodEnd"].Value).ToString(
                //                             "yyyy-MMM-dd") + FieldDelimeter +
                //                         Convert.ToString("N") + FieldDelimeter +
                //                         Convert.ToString("N") + FieldDelimeter +
                //                         Program.CurrentUser + FieldDelimeter +
                //                         DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + FieldDelimeter +
                //                         Program.CurrentUser + FieldDelimeter +
                //                         DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + FieldDelimeter;
                //    }
                //    FiscalYearData = FiscalYearData + LineDelimeter;
                //}

                //encriptedFiscalYearData = Converter.DESEncrypt(PassPhrase, EnKey, FiscalYearData);
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
            #endregion
        }

        //============================
        private void btnNewCompany_Click(object sender, EventArgs e)
        {
            try
            {
                //progressBar1.Visible = true;
                //btnNewCompany.Enabled = false;
                if (Program.CheckLicence(dtpStartDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (Program.CheckLicence(dtpFYearStart.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }


                if (DialogResult.No != MessageBox.Show("Is Fiscal Year Okay?", this.Text, MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question,
                                                           MessageBoxDefaultButton.Button2))
                {


                }
                else
                {
                    return;
                }



                if (txtCompanyID.Text != "")
                {
                    MessageBox.Show(
                        "Data already saved" + "\n" + "To change click update button or for new click refresh button",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }
                progressBar1.Visible = true;
                btnNewCompany.Enabled = false;

                NextID = "0";

                NextID = DateTime.Now.ToString("yyMMddHHmmss");
                companyProfiles = new CompanyProfileVM();
                companyProfiles.CompanyID = NextID;
                companyProfiles.CompanyName = txtCompanyName.Text.Trim();
                companyProfiles.CompanyLegalName = txtCompanyLegalName.Text.Trim();
                companyProfiles.Address1 = txtAddress1.Text.Trim();
                companyProfiles.Address2 = "-";
                companyProfiles.Address3 = "-";
                companyProfiles.City = txtCity.Text.Trim();
                companyProfiles.ZipCode = txtZipCode.Text.Trim();
                companyProfiles.TelephoneNo = txtTelephoneNo.Text.Trim();
                companyProfiles.FaxNo = txtFaxNo.Text.Trim();
                companyProfiles.Email = txtEmail.Text.Trim();
                companyProfiles.ContactPerson = txtContactPerson.Text.Trim();
                companyProfiles.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                companyProfiles.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                companyProfiles.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                companyProfiles.TINNo = txtTINNo.Text.Trim();
                companyProfiles.VatRegistrationNo = txtVatRegistrationNo.Text.Trim();
                companyProfiles.Comments = txtComments.Text.Trim();
                companyProfiles.ActiveStatus = "Y";// active status
                companyProfiles.CreatedBy = Program.CurrentUser;
                companyProfiles.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                companyProfiles.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                companyProfiles.FYearStart = dtpFYearStart.Value.ToString("yyyy-MMM-dd");
                companyProfiles.FYearEnd = dtpFYearEnd.Value.ToString("yyyy-MMM-dd");
                companyProfiles.BIN = txtBIN.Text;
                companyProfiles.Section = txtSection.Text.Trim();
                



                AddFiscalYear();
                FiscalYearSave();

                //DBName = string.Empty;
                //DBName = txtCompanyName.Text.Replace(".", " ");
                //DBName = DBName.Replace(".", " ");
                //DBName = DBName.Replace(" ", "_");

                var result = txtCompanyName.Text.Trim();
                DBName = Regex.Replace(result, @"[0-9\-]", "_");
                DBName = Regex.Replace(result, "[^a-zA-Z0-9_.]+", "_", RegexOptions.Compiled);
                DBName = DBName.Replace(".", "_");
                DBName = DBName.Replace(" ", "_");

                DBName = DBName.Replace("@", "_");
                DBName = DBName.Replace("$", "_");
                DBName = DBName.Replace("#", "_");
                DBName = DBName.Replace(",", "_");
                DBName = DBName.Replace("'", "_");



                progressBar1.Visible = true;
                btnNewCompany.Enabled = false;
                bgwNewCompany.RunWorkerAsync();


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
                FileLogger.Log(this.Name, "btnNewCompany_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnNewCompany_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnNewCompany_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnNewCompany_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnNewCompany_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnNewCompany_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnNewCompany_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnNewCompany_Click", exMessage);
            }
            #endregion
        }
        private void bgwNewCompany_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                #region Statement


                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);


                sqlResults = commonDal.NewDBCreate(companyProfiles, DBName + "_DB", fiscalyears,connVM);
                //done
                SAVE_DOWORK_SUCCESS = true;

                #endregion

                // Start DoWork



                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length>1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwNewCompany_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwNewCompany_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (SAVE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwNewCompany_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCompanyID.Text = newId;
                        }

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
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwNewCompany_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                progressBar1.Visible = false;
                btnNewCompany.Enabled = true;
            }

        }
        //=============================



        //=============================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (Program.CheckLicence(dtpFYearStart.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (Program.CheckLicence(dtpStartDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtCompanyID.Text == "")
                {
                    MessageBox.Show("No data updated." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }
                companyProfiles = new CompanyProfileVM();
                companyProfiles.CompanyID = txtCompanyID.Text.Trim();
                companyProfiles.CompanyName = txtCompanyName.Text.Trim();
                companyProfiles.CompanyLegalName = txtCompanyLegalName.Text.Trim();
                companyProfiles.Address1 = txtAddress1.Text.Trim();
                companyProfiles.Address2 = "-";
                companyProfiles.Address3 = "-";
                companyProfiles.City = txtCity.Text.Trim();
                companyProfiles.ZipCode = txtZipCode.Text.Trim();
                companyProfiles.TelephoneNo = txtTelephoneNo.Text.Trim();
                companyProfiles.FaxNo = txtFaxNo.Text.Trim();
                companyProfiles.Email = txtEmail.Text.Trim();
                companyProfiles.ContactPerson = txtContactPerson.Text.Trim();
                companyProfiles.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                companyProfiles.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                companyProfiles.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                companyProfiles.TINNo = txtTINNo.Text.Trim();
                companyProfiles.VatRegistrationNo = txtVatRegistrationNo.Text.Trim();
                companyProfiles.Comments = txtComments.Text.Trim();
                companyProfiles.ActiveStatus = "Y";// active status
                companyProfiles.CreatedBy = Program.CurrentUser;
                companyProfiles.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                companyProfiles.LastModifiedBy = Program.CurrentUser;
                companyProfiles.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                companyProfiles.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                companyProfiles.FYearStart = dtpFYearStart.Value.ToString("yyyy-MMM-dd");
                companyProfiles.FYearEnd = dtpFYearEnd.Value.ToString("yyyy-MMM-dd");
                companyProfiles.IsVDSWithHolder = chkVDSWithHolder.Checked ? "Y" : "N";
                companyProfiles.BusinessNature = txtBusinessNature.Text.Trim();
                companyProfiles.AccountingNature = txtAccountingNature.Text.Trim();
                companyProfiles.BIN = txtBIN.Text.Trim();
                companyProfiles.License = txtCode.Text.Trim();
                companyProfiles.Section = txtSection.Text.Trim();
                companyProfiles.CompanyType = cmbCompanyType.SelectedValue.ToString();

                if (IsSymphonyUser)
                {
                    companyProfiles.Tom = Converter.DESEncrypt(PassPhrase, EnKey, txtCompanyName.Text.Trim());  //encrypted CompanyName
                    companyProfiles.Jary = Converter.DESEncrypt(PassPhrase, EnKey, txtCompanyLegalName.Text.Trim());  //encrypted CompanyLegalName
                    companyProfiles.Miki = Converter.DESEncrypt(PassPhrase, EnKey, txtVatRegistrationNo.Text.Trim()); //encrypted VatRegistrationNo

                    CommonDAL commonDal = new CommonDAL();
                    //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                    //string processorId = commonDal.GetHardwareID();
                    string processorId = commonDal.GetServerHardwareId(connVM);
                    companyProfiles.Mouse = Converter.DESEncrypt(PassPhrase, EnKey, processorId);
                }

                this.btnUpdate.Enabled = false;
                this.progressBar1.Enabled = true;
                bgwSaveCompany.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
        private void bgwSaveCompany_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                CompanyprofileDAL companyprofileDAL = new CompanyprofileDAL();
                //ICompanyprofile companyprofileDAL = OrdinaryVATDesktop.GetObject<CompanyprofileDAL, CompanyprofileRepo, ICompanyprofile>(OrdinaryVATDesktop.IsWCF);


                sqlResults = companyprofileDAL.UpdateCompanyProfileNew(companyProfiles,connVM);
                //done
                UPDATE_DOWORK_SUCCESS = true;


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
                FileLogger.Log(this.Name, "bgwSaveCompany_DoWork", exMessage);
            }
            #endregion
        }
        private void bgwSaveCompany_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                if (UPDATE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSaveCompany_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCompanyID.Text = newId;

                            if (result == "Success")
                            {
                                Program.CompanyName = txtCompanyLegalName.Text.Trim();
                                Program.CompanyLegalName = txtCompanyLegalName.Text.Trim();
                                Program.CompanyNameLog = txtCompanyName.Text.Trim();
                                Program.Address1 = txtAddress1.Text.Trim();
                                Program.Address2 = txtAddress2.Text.Trim();
                                Program.Address3 = txtAddress3.Text.Trim();
                                Program.City = txtCity.Text.Trim();
                                Program.ZipCode = txtZipCode.Text.Trim();
                                Program.TelephoneNo = txtTelephoneNo.Text.Trim();
                                Program.FaxNo = txtFaxNo.Text.Trim();
                                Program.Email = txtEmail.Text.Trim();
                                Program.ContactPerson = txtContactPerson.Text.Trim();
                                Program.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                                Program.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                                Program.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                                Program.TINNo = txtTINNo.Text.Trim();
                                Program.VatRegistrationNo = txtVatRegistrationNo.Text.Trim();
                                Program.Comments = txtComments.Text.Trim();
                                Program.FMonthStart = dtpFYearStart.Value;
                                Program.FMonthEnd = dtpFYearEnd.Value;
                                
                            }
                            ChangeData = false;
                        }

                    }
                }

                //if (Convert.ToDecimal(result) < 0)
                //{
                //    MessageBox.Show("Data not save", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //else
                //{
                //    MessageBox.Show("Data save successfully", this.Text, MessageBoxButtons.OK,  MessageBoxIcon.Information);
                //    Program.CompanyName = txtCompanyName.Text.Trim();
                //    Program.CompanyLegalName = txtCompanyLegalName.Text.Trim();
                //    Program.Address1 = txtAddress1.Text.Trim();
                //    Program.Address2 = txtAddress2.Text.Trim();
                //    Program.Address3 = txtAddress3.Text.Trim();
                //    Program.City = txtCity.Text.Trim();
                //    Program.ZipCode = txtZipCode.Text.Trim();
                //    Program.TelephoneNo = txtTelephoneNo.Text.Trim();
                //    Program.FaxNo = txtFaxNo.Text.Trim();
                //    Program.Email = txtEmail.Text.Trim();
                //    Program.ContactPerson = txtContactPerson.Text.Trim();
                //    Program.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                //    Program.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                //    Program.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                //    Program.TINNo = txtTINNo.Text.Trim();
                //    Program.VatRegistrationNo = txtVatRegistrationNo.Text.Trim();
                //    Program.Comments = txtComments.Text.Trim();
                //    Program.FMonthStart = dtpFYearStart.Value;
                //    Program.FMonthEnd = dtpFYearEnd.Value;
                //    ChangeData = false;
                //}


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
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSaveCompany_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Enabled = false;
            }

        }
        //=============================

        private void dtpFYearStart_ValueChanged(object sender, EventArgs e)
        {
            dtpFYearEnd.Value = dtpFYearStart.Value.AddYears(1).AddDays(-1);
            ChangeData = true;

        }

        private void dtpStartDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpFYearStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpFYearEnd_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (gbLicense.Visible==false)
            {
                gbLicense.Visible = true;
                gbLicense.Width = 338;//(338, 158);
                gbLicense.Height = 158;//(338, 158);

                chkManufacturing.Checked = Program.IsManufacturing;
                chkTrading.Checked = Program.IsTrading;
                chkService.Checked = Program.IsService;
                chkTender.Checked = Program.IsTender;
                chkTDS.Checked = Program.IsTDS;
                chkBandroll.Checked = Program.IsBandroll;
                chkTollClient.Checked = Program.IsTollClient;
                chkTollContractor.Checked = Program.IsTollContractor;
                chkIntegrationExcel.Checked = Program.IsIntegrationExcel;
                chkIntegrationOthers.Checked = Program.IsIntegrationOthers;
                chkIntegrationAPI.Checked = Program.IsIntegrationAPI;
                chkCentral.Checked = Program.IsCentralBIN;
                txtDepos.Value = Program.Depos;

            }
            else
            {
                gbLicense.Visible = false;
                gbLicense.Width = 338;//(338, 158);
                gbLicense.Height = 158;//(338, 158);
            }
           
        }

    }
}
