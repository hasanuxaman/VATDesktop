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
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormBranchProfiles : Form
    {
        #region Global Variables
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        //List<CustomerGroupDTO> customerGroups = new List<CustomerGroupDTO>();
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;

        string NextID;
        string CustomerData;
        string CustomerGroupData;
        string[] CustomerGroupLines;
        private bool ChangeData = false;
        public string VFIN = "132";
        private bool IsUpdate = false;
        private int searchBranchId = 0;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        DataGridViewRow selectedRow = new DataGridViewRow();
        //string vCustomerId = "0";
        //string vCustomerAddressId = "0";
        private bool IsSymphonyUser = false;

        private DataTable BranchMapDetailsResult;

        #endregion

        #region Global Variables

        private string activestatus = string.Empty;
        //private DataTable CustomerGroupResult;
        //private DataTable CustomerAddressResult;

        private string result;

        #endregion

        public FormBranchProfiles()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        public int ErrorReturn()
        {
            if (txtBranchCode.Text.Trim().Length <1)
            {
                MessageBox.Show("Please Enter Branch Code", this.Text);
                txtBranchCode.Focus();
                return 1;
            }
            if (textBranchName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Branch Name.", this.Text);
                textBranchName.Focus();
                return 1;
            }
            if (textBranchLegalName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Branch Legal Name.", this.Text);
                textBranchLegalName.Focus();
                return 1;
            }
            if (txtVATRegistrationNo.Text == "")
            {
                txtVATRegistrationNo.Text = "-";
                ////MessageBox.Show("Please enter VAT Registration No.", this.Text);
                ////txtVATRegistrationNo.Focus();
                ////return 1;
            }
            if (textBIN.Text == "")
            {
                textBIN.Text = "-";
            }
            if (textTINNo.Text == "")
            {
                textTINNo.Text = "-";
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
            if (textFaxNo.Text == "")
            {
                textFaxNo.Text = "-";
            }
            if (txtContactPersonEmail.Text == "")
            {
                txtContactPersonEmail.Text = "-";
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

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                txtId.Text = "-";
            }
            if (string.IsNullOrWhiteSpace(txtIP.Text))
            {
                txtIP.Text = "-";
            }
            if (string.IsNullOrWhiteSpace(txtDbName.Text))
            {
                txtIP.Text = "-";
            }
            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                txtIP.Text = "-";
            }

            return 0;
        }

        private void ClearAll()
        {
            //DateTime vdateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));

            chkIsCentral.Checked = false;

            textBranchLegalName.Text = "";
            textBranchBanglaLegalName.Text = "";
            textFaxNo.Text = "";
            txtVATRegistrationNo.Text = "";
            textBIN.Text = "";

            textBranchName.Text = "";

            txtTelephoneNo.Text = "";
            textFaxNo.Text = "";
            txtContactPersonEmail.Text = "";
            txtContactPerson.Text = "";
            txtContactPersonDesignation.Text = "";
            txtContactPersonTelephone.Text = "";
            txtContactPersonEmail.Text = "";
            textTINNo.Text = "";
            textAddress.Text = "";
            textBanglaAddress.Text = "";
            textEmail.Text = "";
            textZipCode.Text = "";

            txtCity.Text = "";
            //dtpStartDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + " " + vdateTime.ToString("HH:mm:ss"));//Program.SessionDate;
            txtComments.Text = "";
            txtBranchCode.Text = string.Empty;
            //chIsArchive.Checked = false;
            //chkVDSWithHolder.Checked = false;

            txtIP.Text = "";
            txtDbName.Text = "";
            txtId.Text = "";
            txtPass.Text = "";

            #region Clear

            BranchDetailsClear();

            dgvBranchDetails.Rows.Clear();

            #endregion


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BranchProfiles_Load(object sender, EventArgs e)
        {
            try
            {

                
                if (btnUpdate.Visible == true)
                {

                    if (DialogResult.No != MessageBox.Show("Are you Symphony user?", this.Text, MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question,
                                                            MessageBoxDefaultButton.Button2))
                    {
                        IsSymphonyUser = FormSupperAdministrator.SelectOne();
                        if (!IsSymphonyUser)
                        {
                            btnAdd.Visible = false;
                            groupBox1.Visible = false;
                        }
                        else
                        {
                            btnAdd.Visible = true;
                            groupBox1.Visible = true;

                            //DataTable dt = new DataTable();
                            //dt = new BranchProfileDAL().SelectAll(null, null, null, null, null, false, connVM);
                            //if (dt.Rows.Count <= Program.Depos+1)
                            //{
                            //    btnAdd.Visible = true;

                            //}
                            //else
                            //{
                            //    btnAdd.Visible = false;

                            //}
                        }

                    }
                    else
                    {
                        btnAdd.Visible = false;
                    }

                    this.progressBar1.Enabled = true;

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
                FileLogger.Log(this.Name, "BranchProfiles_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BranchProfiles_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BranchProfiles_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "BranchProfiles_Load", exMessage);
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
                FileLogger.Log(this.Name, "BranchProfiles_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BranchProfiles_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BranchProfiles_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "BranchProfiles_Load", exMessage);
            }
            #endregion
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void txtTINNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void VatRegistrationNo_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void Business_Enter(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                //if (IsUpdate == false)
                //{
                //    NextID = string.Empty;
                //}
                //else
                //{
                //    NextID = textBranchId.Text.Trim();
                //}

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
                        string nextId = sqlResults[2];
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

                            txtBranchId.Text = nextId;

                            txtBranchCode.Text = code;

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

        private void backgroundWorkerAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                // Start DoWork
                //BranchProfileDAL BranchProfileDAL = new BranchProfileDAL();
                IBranchProfile BranchProfileDAL = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(OrdinaryVATDesktop.IsWCF);

                BranchProfileVM vm = new BranchProfileVM();

                //vm.BranchID = NextID.ToString();
                vm.BranchCode = txtBranchCode.Text.Trim();
                vm.BranchName = textBranchName.Text.Trim();
                vm.BranchLegalName = textBranchLegalName.Text.Trim();
                vm.BranchBanglaLegalName = textBranchBanglaLegalName.Text.Trim();
                vm.Address = textAddress.Text.Trim();
                vm.BanglaAddress = textBanglaAddress.Text.Trim();
                vm.City = txtCity.Text.Trim();
                vm.ZipCode = textZipCode.Text.Trim();
                vm.TelephoneNo = txtTelephoneNo.Text.Trim();
                vm.FaxNo = textFaxNo.Text.Trim();
                vm.Email = textEmail.Text.Trim();
                vm.ContactPerson = txtContactPerson.Text.Trim();
                vm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                vm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                vm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                vm.VatRegistrationNo = txtVATRegistrationNo.Text.Trim();
                vm.BIN = textBIN.Text.Trim();
                vm.TINNo = textTINNo.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = activestatus.ToString();
                vm.CreatedBy = Program.CurrentUser; ;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                vm.IsArchive = chkIsArchive.Checked;
                vm.IsCentral = chkIsCentral.Checked;

                vm.IP = txtIP.Text;
                vm.Id = txtId.Text;
                vm.DbName = txtDbName.Text;
                vm.Pass = txtPass.Text;

                sqlResults = BranchProfileDAL.InsertToBranchProfileNew(vm, false, null, null, connVM);

                SAVE_DOWORK_SUCCESS = true;
                //ClearAll();
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            #endregion
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnSearchBranch_click(object sender, EventArgs e)
        {
            #region try
            try
            {
                DataGridViewRow selectedRow = null;
                //vBranchId = "0";
                Program.fromOpen = "Me";
                //string result = FromBranchProfilesSearch.SelectOne();
                selectedRow = FormBranchProfilesSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtBranchCode.Text = selectedRow.Cells["BranchCode"].Value.ToString();
                    string[] cFields = { "BranchCode" };
                    string[] cValeus = { txtBranchCode.Text.Trim() };
                    BranchProfileVM vm = new BranchProfileVM();
                    vm = new BranchProfileDAL().SelectAllList(null, cFields, cValeus, null, null, connVM).FirstOrDefault();

                    textBranchName.Text = vm.BranchName;
                    textBranchLegalName.Text = vm.BranchLegalName;
                    textBranchBanglaLegalName.Text = vm.BranchBanglaLegalName;
                    txtVATRegistrationNo.Text = vm.VatRegistrationNo;
                    textBIN.Text = vm.BIN;
                    textTINNo.Text = vm.TINNo;
                    txtContactPerson.Text = vm.ContactPerson;
                    txtContactPersonEmail.Text = vm.ContactPersonDesignation;
                    txtTelephoneNo.Text = vm.TelephoneNo;
                    textEmail.Text = vm.Email;
                    textAddress.Text = vm.Address;
                    textBanglaAddress.Text = vm.BanglaAddress;
                    txtBranchId.Text = vm.BranchID.ToString();

                    txtCity.Text = vm.City;
                    txtComments.Text = vm.Comments;
                    checkBox1.Checked = vm.ActiveStatus == "Y" ? true : false;
                    chkIsArchive.Checked = vm.IsArchive;
                    chkIsCentral.Checked = vm.IsCentral;
                    txtIP.Text = vm.IP;
                    txtId.Text = vm.Id;
                    txtDbName.Text = vm.DbName;
                    txtPass.Text = vm.Pass;

                    IsUpdate = true;

                    #region Comments //Mar-14-2020

                    ////textBranchName.Text = selectedRow.Cells["BranchName"].Value.ToString();
                    ////textBranchLegalName.Text = selectedRow.Cells["BranchLegalName"].Value.ToString();
                    ////txtVATRegistrationNo.Text = selectedRow.Cells["VatRegistrationNo"].Value.ToString();
                    ////textBIN.Text = selectedRow.Cells["BIN"].Value.ToString();
                    ////textTINNo.Text = selectedRow.Cells["TINNo"].Value.ToString();
                    ////txtContactPerson.Text = selectedRow.Cells["ContactPerson"].Value.ToString();
                    ////txtContactPersonEmail.Text = selectedRow.Cells["ContactPersonDesignation"].Value.ToString();
                    ////txtTelephoneNo.Text = selectedRow.Cells["TelephoneNo"].Value.ToString();
                    ////textEmail.Text = selectedRow.Cells["Email"].Value.ToString();
                    ////textAddress.Text = selectedRow.Cells["Address"].Value.ToString();
                    ////txtBranchId.Text = selectedRow.Cells["BranchId"].Value.ToString();

                    ////txtCity.Text = selectedRow.Cells["City"].Value.ToString();
                    ////txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    ////checkBox1.Checked = Convert.ToString(selectedRow.Cells["ActiveStatus"].Value.ToString()) == "Y" ? true : false;
                    ////chkIsArchive.Checked = Convert.ToBoolean(selectedRow.Cells["IsArchive"].Value); 
                    //////txtContactPersonTelephone.Text = selectedRow.Cells["ContactPersonTelephone"].Value.ToString();
                    //////IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;

                    #endregion


                    fnBranchMapDetails(vm);

                }
                ChangeData = false;



                if (result == "")
                {
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                // Start DoWork
                //BranchProfileDAL BranchProfileDAL = new BranchProfileDAL();
                IBranchProfile BranchProfileDAL = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(OrdinaryVATDesktop.IsWCF);

                BranchProfileVM vm = new BranchProfileVM();

                //vm.BranchID = NextID.ToString();
                vm.BranchCode = txtBranchCode.Text.Trim();
                vm.BranchName = textBranchName.Text.Trim();
                vm.BranchLegalName = textBranchLegalName.Text.Trim();
                vm.BranchBanglaLegalName = textBranchBanglaLegalName.Text.Trim();
                vm.Address = textAddress.Text.Trim();
                vm.BanglaAddress = textBanglaAddress.Text.Trim();
                vm.City = txtCity.Text.Trim();
                vm.ZipCode = textZipCode.Text.Trim();
                vm.TelephoneNo = txtTelephoneNo.Text.Trim();
                vm.FaxNo = textFaxNo.Text.Trim();
                vm.Email = textEmail.Text.Trim();
                vm.ContactPerson = txtContactPerson.Text.Trim();
                vm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                vm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                vm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                vm.VatRegistrationNo = txtVATRegistrationNo.Text.Trim();
                vm.BIN = textBIN.Text.Trim();
                vm.TINNo = textTINNo.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ActiveStatus = activestatus.ToString();
                vm.CreatedBy = Program.CurrentUser; ;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                vm.IsArchive = chkIsArchive.Checked;
                vm.IsCentral = chkIsCentral.Checked;
                vm.BranchID = Convert.ToInt32(txtBranchId.Text.Trim());

                vm.IP = txtIP.Text;
                vm.Id = txtId.Text;
                vm.DbName = txtDbName.Text;
                vm.Pass = txtPass.Text;

                sqlResults = BranchProfileDAL.UpdateToBranchProfileNew(vm, connVM);

                UPDATE_DOWORK_SUCCESS = true;
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
                        string nextId = sqlResults[2];
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

                            txtBranchId.Text = nextId;
                            txtBranchCode.Text = code;

                        }

                    }

                ChangeData = false;
                if (IsSymphonyUser)
                {
                    this.btnAdd.Visible = true;

                }
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
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

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearAll();
            ChangeData = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorkerDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement




                // Start DoWork
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //BranchProfileDAL BranchProfileDAL = new BranchProfileDAL();
                IBranchProfile BranchProfileDAL = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(OrdinaryVATDesktop.IsWCF);

                BranchProfileVM vm = new BranchProfileVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString();
                string[] ids = new string[] { txtBranchId.Text.Trim(), "" };

                sqlResults = BranchProfileDAL.Delete(vm, ids, null, null, connVM);
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
                FileLogger.Log(this.Name, "bgwDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwDelete_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwDelete_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwDelete_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Statement
                if (DELETE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearAll();
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
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwDelete_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerDelete.RunWorkerAsync();
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
            #endregion Catch

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnDetailsAdd_Click(object sender, EventArgs e)
        {
            #region try
            
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search Branch first");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtIntegrationCode.Text))
                {
                    MessageBox.Show("Please Add Integration Code first");
                    txtIntegrationCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBranchName.Text))
                {
                    MessageBox.Show("Please Add Branch Name first");
                    txtBranchName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBranchDetailsAddress.Text))
                {
                    MessageBox.Show("Please Add Address first");
                    txtBranchDetailsAddress.Focus();
                    return;
                }

              

                sqlResults = new string[4];

                BranchProfileVM vm = new BranchProfileVM();
                vm.BranchCode = txtBranchCode.Text.Trim();
                vm.IntegrationCode = txtIntegrationCode.Text.Trim();
                vm.BranchID = Convert.ToInt32(txtBranchId.Text.Trim());
                vm.DetailsAddress = txtBranchDetailsAddress.Text;
                vm.BranchName = txtBranchName.Text;

                BranchProfileDAL pDal = new BranchProfileDAL();

                sqlResults = pDal.InsertToBranchMapDetails(vm,null,null,connVM);
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

                        if (result == "Success")
                        {
                            IsUpdate = true;

                            txtSL.Text = newId;

                            #region Clear

                            BranchDetailsClear();

                            #endregion
                        }


                    }

                    #region fnBranchMapDetails

                    fnBranchMapDetails(vm);

                    #endregion

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

        private void fnBranchMapDetails(BranchProfileVM vm)
        {
            try
            {


                BranchMapDetailsResult = new DataTable();

                //BranchProfileVM vm = new BranchProfileVM();


                BranchProfileDAL BranchProfileDAL = new BranchProfileDAL();

                BranchMapDetailsResult = BranchProfileDAL.SearchBranchMapDetails(vm,null,null,connVM);

                dgvBranchDetails.Rows.Clear();

                int j = 0;

                foreach (DataRow item in BranchMapDetailsResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvBranchDetails.Rows.Add(NewRow);


                    dgvBranchDetails.Rows[j].Cells["SLNo"].Value = (j + 1).ToString();
                    dgvBranchDetails.Rows[j].Cells["SL"].Value = item["SL"].ToString();
                    dgvBranchDetails.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                    dgvBranchDetails.Rows[j].Cells["IntegrationCode"].Value = item["IntegrationCode"].ToString();
                    dgvBranchDetails.Rows[j].Cells["BranchNameD"].Value = item["BranchName"].ToString();
                    dgvBranchDetails.Rows[j].Cells["BranchDetailsAddress"].Value = item["Address"].ToString();
                    j = j + 1;

                }
                //txtProductNames.Text = "";
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

        private void btnChange_Click(object sender, EventArgs e)
        {
            sqlResults = new string[4];

            try
            {

                if (string.IsNullOrWhiteSpace(txtSL.Text))
                {
                    MessageBox.Show("Please Select Item first");
                    dgvBranchDetails.Focus();
                    return;
                }
                

                BranchProfileVM vm = new BranchProfileVM();
                vm.DetailsSL = Convert.ToInt32(txtSL.Text.Trim());

                vm.IntegrationCode = txtIntegrationCode.Text ;
                vm.DetailsAddress = txtBranchDetailsAddress.Text;
                vm.BranchID = Convert.ToInt32(txtBranchId.Text);
                vm.BranchName = txtBranchName.Text;


                BranchProfileDAL bDal = new BranchProfileDAL();

                sqlResults = bDal.UpdateToBranchMapDetails(vm,null,null,connVM);

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

                    #region Clear

                    BranchDetailsClear();

                    #endregion

                    #region fnBranchMapDetails

                    fnBranchMapDetails(vm);

                    #endregion

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

        private void dgvBranchDetails_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvBranchDetails.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                txtSL.Text = userSelRow.Cells["SL"].Value.ToString();
                txtIntegrationCode.Text = userSelRow.Cells["IntegrationCode"].Value.ToString();
                txtBranchName.Text = userSelRow.Cells["BranchNameD"].Value.ToString();
                txtBranchDetailsAddress.Text = userSelRow.Cells["BranchDetailsAddress"].Value.ToString();
            }
        }

        private void btnDetailsDelete_Click(object sender, EventArgs e)
        {

            try
            {
                sqlResults = new string[4];

                #region Validation

                if (string.IsNullOrWhiteSpace(txtSL.Text))
                {
                    MessageBox.Show("Please Select Item first");
                    dgvBranchDetails.Focus();
                    return;
                }

                #endregion

                #region Branch Profile VM

                BranchProfileVM vm = new BranchProfileVM();
                vm.DetailsSL = Convert.ToInt32(txtSL.Text.Trim());

                vm.IntegrationCode = txtIntegrationCode.Text;
                vm.DetailsAddress = txtBranchDetailsAddress.Text;
                vm.BranchID = Convert.ToInt32(txtBranchId.Text);

                #endregion

                #region Branch Profile DAL

                BranchProfileDAL bDal = new BranchProfileDAL();

                sqlResults = bDal.DeleteBranchMapDetails(vm,null,null,connVM);

                #endregion

                #region sqlResults

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

                    #region Clear

                    BranchDetailsClear();

                    #endregion

                    #region fnBranchMapDetails

                    fnBranchMapDetails(vm);

                    #endregion

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

        private void BranchDetailsClear()
        {
            txtSL.Clear();
            txtIntegrationCode.Clear();
            txtBranchDetailsAddress.Clear();
            txtBranchName.Clear();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string text = cmbDBType.Text;

                progressBar1.Visible = true;

                bgwConnection.RunWorkerAsync(text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void bgwConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            BranchProfileDAL dal = new BranchProfileDAL();
            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);
            CommonDAL commonDal = new CommonDAL();


            if (e.Argument.ToString() == "MSSQL")
            {
                e.Result = commonDal.TestSqlConnection(dt);

            }
            else if (e.Argument.ToString() == "ORACLE")
            {

                e.Result = commonDal.TestOracleConnection(dt);

            }
        }

        private void bgwConnection_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.ToString());
                    return;
                }

                if (Convert.ToBoolean(e.Result))
                {
                    MessageBox.Show("Connected !!!");
                }
            }

            finally
            {
                progressBar1.Visible = false;

            }


        }

    }
}
