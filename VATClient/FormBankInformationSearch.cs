using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormBankInformationSearch : Form
    {
        #region Constructors

        public FormBankInformationSearch()
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

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private string SelectedValue = string.Empty;
        private string BankInformationData;
        private string[] BankInformationLines;
        //public string VFIN = "312";
        private DataTable BankInformationResult;
        private int searchBranchId = 0;
        string activeStatus = string.Empty;

        #endregion
        #region Methods

        public static string SelectOne()
        {
            string SearchValue = string.Empty;
            try
            {
                FormBankInformationSearch frmBankInformationSearch = new FormBankInformationSearch();
                frmBankInformationSearch.ShowDialog();
                SearchValue = frmBankInformationSearch.SelectedValue;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
            }
            #endregion Catch

            return SearchValue;

        }

        private void Search()
        {
            try
            {
                
                #region  ---off

                
                    //activeStatus = Convert.ToString(chkActive.Checked ? "Y" : "N");

                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                this.btnSearch.Visible = false;
                this.progressBar1.Visible = true;
                bgwSearch.RunWorkerAsync();
                #endregion

            }
            #region Catch
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

     

        private void GridSelected()
        {
            try
            {
                if (dgvBankInformation.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dgvBankInformation.Rows.Count > 0)
                {
                    string BankInformationInfo = string.Empty;
                    int ColIndex = dgvBankInformation.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvBankInformation.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        //if (Program.fromOpen != "Me" &&
                        //    dgvBankInformation.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                        //{
                        //    MessageBox.Show("This Selected Item is not Active");
                        //    return;
                        //}
                        string BankID = dgvBankInformation.Rows[RowIndex1].Cells["BankID"].Value.ToString();
                        string BankName = dgvBankInformation.Rows[RowIndex1].Cells["BankName"].Value.ToString();
                        string BranchName = dgvBankInformation.Rows[RowIndex1].Cells["BranchName"].Value.ToString();
                        string AccountNumber = dgvBankInformation.Rows[RowIndex1].Cells["AccountNumber"].Value.ToString();
                        string Address1 = dgvBankInformation.Rows[RowIndex1].Cells["Address1"].Value.ToString();
                        string Address2 = dgvBankInformation.Rows[RowIndex1].Cells["Address2"].Value.ToString();
                        string Address3 = dgvBankInformation.Rows[RowIndex1].Cells["Address3"].Value.ToString();
                        string City = dgvBankInformation.Rows[RowIndex1].Cells["City"].Value.ToString();
                        string TelephoneNo = dgvBankInformation.Rows[RowIndex1].Cells["TelephoneNo"].Value.ToString();
                        string FaxNo = dgvBankInformation.Rows[RowIndex1].Cells["FaxNo"].Value.ToString();
                        string Email = dgvBankInformation.Rows[RowIndex1].Cells["Email"].Value.ToString();
                        string ContactPerson = dgvBankInformation.Rows[RowIndex1].Cells["ContactPerson"].Value.ToString();
                        string ContactPersonDesignation = dgvBankInformation.Rows[RowIndex1].Cells["ContactPersonDesignation"].Value.ToString();
                        string ContactPersonTelephone = dgvBankInformation.Rows[RowIndex1].Cells["ContactPersonTelephone"].Value.ToString();
                        string ContactPersonEmail = dgvBankInformation.Rows[RowIndex1].Cells["ContactPersonEmail"].Value.ToString();
                        string Comments = dgvBankInformation.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                        string ActiveStatus = dgvBankInformation.Rows[RowIndex1].Cells["ActiveStatus1"].Value.ToString();
                        string code = dgvBankInformation.Rows[RowIndex1].Cells["Code"].Value.ToString();
                        string BranchId = dgvBankInformation.Rows[RowIndex1].Cells["BranchId"].Value.ToString();
                        BankInformationInfo =
                            BankID + FieldDelimeter +
                            BankName + FieldDelimeter +
                            BranchName + FieldDelimeter +
                            AccountNumber + FieldDelimeter +
                            Address1 + FieldDelimeter +
                            Address2 + FieldDelimeter +
                            Address3 + FieldDelimeter +
                            City + FieldDelimeter +
                            TelephoneNo + FieldDelimeter +
                            FaxNo + FieldDelimeter +
                            Email + FieldDelimeter +
                            ContactPerson + FieldDelimeter +
                            ContactPersonDesignation + FieldDelimeter +
                            ContactPersonTelephone + FieldDelimeter +
                            ContactPersonEmail + FieldDelimeter +
                            Comments + FieldDelimeter +
                            ActiveStatus + FieldDelimeter + 
                            code + FieldDelimeter +
                            BranchId + FieldDelimeter;

                        SelectedValue = BankInformationInfo;

                    }
                } this.Close();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSelected", exMessage);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion Catch


        }

        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //MDIMainInterface mdi = new MDIMainInterface();
                //FormBankInformation frm = new FormBankInformation();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}


                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }
                FormBankInformation frmBankInformation = new FormBankInformation();
                frmBankInformation.Show();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Clic", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Clic", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAdd_Clic", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnAdd_Clic", exMessage);
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
                FileLogger.Log(this.Name, "btnAdd_Clic", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Clic", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Clic", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAdd_Clic", exMessage);
            }
            #endregion Catch
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);
            Search();
        }
        private void dgvBankInformation_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.button1.Visible = false;
        }
      
        #region TextBox KeyDown Event

        private void txtBankID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtBranchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void cmbActiveStatus_KeyDown(object sender, KeyEventArgs e)
        {
            //btnSearch.Focus();
        }

        #endregion
        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();
        }
        private void dgvBankInformation_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();

        }
        //public void BranchLoad(ComboBox cmbBranch)
        //{

        //    BranchProfileDAL _BranchProfileDAL = new BranchProfileDAL();
        //    DataTable dtBranchProfile = new DataTable();

        //    dtBranchProfile = _BranchProfileDAL.SelectAll(null, null, null, null, null, true);


        //    DataRow dr = dtBranchProfile.NewRow();
        //    dr["BranchID"] = "0";
        //    dr["BranchName"] = "All Branch";
        //    dtBranchProfile.Rows.InsertAt(dr, 0);

        //    cmbBranch.DataSource = dtBranchProfile;
        //    cmbBranch.ValueMember = "BranchID";
        //    cmbBranch.DisplayMember = "BranchName";

        //}
        private void FormBankInformationSearch_Load(object sender, EventArgs e)
        {
            try
            {

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                //BranchLoad(cmbBranch);
                CommonDAL dal = new CommonDAL();
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
                cmbBranch.SelectedValue = Program.BranchId;
                cmbImport.SelectedIndex = 0;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = dal.settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                Search();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormBankInformationSearch_Load", exMessage);
            }
            #endregion Catch
        }
        #region backgroundWorker Event
        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ////// Start DoWork
                //////BankInformationResult = BankInformationResult.Clear();

                //BankInformationDAL bankInformationDal = new BankInformationDAL();
                IBankInformation bankInformationDal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);

                string[] cFields = { "BankCode like", "BankName like", "BranchName like", "AccountNumber like", "City like", "TelephoneNo like ", "FaxNo like"
                                      , "Email like", "ContactPerson like", "ContactPersonDesignation like" , "ContactPersonTelephone like"
                                        , "ContactPersonEmail like"  , "ActiveStatus like","BranchId" };
                string[] cValues = { txtBankCode.Text.Trim(), txtBankName.Text.Trim(), txtBranchName.Text.Trim(),
                    txtAccountNumber.Text.Trim(), txtCity.Text.Trim(), txtTelephoneNo.Text.Trim(), txtFaxNo.Text.Trim(), txtEmail.Text.Trim(),
                    txtContactPerson.Text.Trim(), txtContactPersonDesignation.Text.Trim(), txtContactPersonTelephone.Text.Trim()
                    , txtContactPersonEmail.Text.Trim(),
                    activeStatus,searchBranchId.ToString() };
                BankInformationResult = bankInformationDal.SelectAll(0, cFields, cValues, null, null, true,connVM);


                //BankInformationResult = bankInformationDal.SearchBankDT(txtBankCode.Text.Trim(), txtBankName.Text.Trim(), txtBranchName.Text.Trim(),
                //    txtAccountNumber.Text.Trim(), txtCity.Text.Trim(), txtTelephoneNo.Text.Trim(), txtFaxNo.Text.Trim(), txtEmail.Text.Trim(),
                //    txtContactPerson.Text.Trim(), txtContactPersonDesignation.Text.Trim(), txtContactPersonTelephone.Text.Trim(), txtContactPersonEmail.Text.Trim(),
                //    activeStatus);

                // End DoWork

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                int j = 0;
                dgvBankInformation.Rows.Clear();
                foreach (DataRow item2 in BankInformationResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvBankInformation.Rows.Add(NewRow);

                    dgvBankInformation.Rows[j].Cells["BankID"].Value = item2["BankID"].ToString();// Convert.ToDecimal(BankInformationFields[0]);
                    dgvBankInformation.Rows[j].Cells["BankName"].Value = item2["BankName"].ToString();// BankInformationFields[1].ToString();
                    dgvBankInformation.Rows[j].Cells["BranchName"].Value = item2["BranchName"].ToString();// BankInformationFields[2].ToString();
                    dgvBankInformation.Rows[j].Cells["AccountNumber"].Value = item2["AccountNumber"].ToString();// BankInformationFields[3].ToString();
                    dgvBankInformation.Rows[j].Cells["Address1"].Value = item2["Address1"].ToString();// BankInformationFields[4].ToString();
                    dgvBankInformation.Rows[j].Cells["Address2"].Value = item2["Address2"].ToString();// BankInformationFields[5].ToString();
                    dgvBankInformation.Rows[j].Cells["Address3"].Value = item2["Address3"].ToString();// BankInformationFields[6].ToString();
                    dgvBankInformation.Rows[j].Cells["City"].Value = item2["City"].ToString();// BankInformationFields[7].ToString();
                    dgvBankInformation.Rows[j].Cells["TelephoneNo"].Value = item2["TelephoneNo"].ToString();// BankInformationFields[8].ToString();
                    dgvBankInformation.Rows[j].Cells["FaxNo"].Value = item2["FaxNo"].ToString();// BankInformationFields[9].ToString();
                    dgvBankInformation.Rows[j].Cells["Email"].Value = item2["Email"].ToString();// BankInformationFields[10].ToString();
                    dgvBankInformation.Rows[j].Cells["ContactPerson"].Value = item2["ContactPerson"].ToString();// BankInformationFields[11].ToString();
                    dgvBankInformation.Rows[j].Cells["ContactPersonDesignation"].Value = item2["ContactPersonDesignation"].ToString();//BankInformationFields[12].ToString();
                    dgvBankInformation.Rows[j].Cells["ContactPersonTelephone"].Value = item2["ContactPersonTelephone"].ToString();//BankInformationFields[13].ToString();
                    dgvBankInformation.Rows[j].Cells["ContactPersonEmail"].Value = item2["ContactPersonEmail"].ToString();//BankInformationFields[14].ToString();
                    dgvBankInformation.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// BankInformationFields[15].ToString();
                    dgvBankInformation.Rows[j].Cells["ActiveStatus1"].Value = item2["ActiveStatus"].ToString();// BankInformationFields[16].ToString();
                    dgvBankInformation.Rows[j].Cells["Code"].Value = item2["BankCode"].ToString();// BankInformationFields[16].ToString();
                    dgvBankInformation.Rows[j].Cells["BranchId"].Value = item2["BranchId"].ToString();
                    j = j + 1;
                }

                #region Specific Coloumns Visible False

                dgvBankInformation.Columns["City"].Visible = false;
                dgvBankInformation.Columns["Email"].Visible = false;
                dgvBankInformation.Columns["ContactPersonDesignation"].Visible = false;
                dgvBankInformation.Columns["ContactPersonTelephone"].Visible = false;
                dgvBankInformation.Columns["ContactPersonEmail"].Visible = false;
                dgvBankInformation.Columns["Comments"].Visible = false;
                dgvBankInformation.Columns["Address1"].Visible = false;
                dgvBankInformation.Columns["Address2"].Visible = false;
                dgvBankInformation.Columns["Address3"].Visible = false;
                dgvBankInformation.Columns["BranchId"].Visible = false;
                #endregion

                // End Complete

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                LRecordCount.Text = "Record Count: " + dgvBankInformation.Rows.Count.ToString();
                this.btnSearch.Visible = true;
                this.progressBar1.Visible = false;
            }


        }
        #endregion
        //=====================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            txtAccountNumber.Text = "";
            txtBankCode.Text = "";
            txtBankName.Text = "";
            txtBranchName.Text = "";
            txtCity.Text = "";
            cmbActive.SelectedIndex = -1;
            //chkActive.Checked = false;
            //dgvBankInformation.DataSource = null;
            dgvBankInformation.Rows.Clear();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvBankInformation.RowCount; i++)
            {
                dgvBankInformation["Select", i].Value = chkSelectAll.Checked;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                var ids = new List<string>();

                var len = dgvBankInformation.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvBankInformation["Select", i].Value))
                    {
                        ids.Add(dgvBankInformation["Code", i].Value.ToString());
                    }
                }

                //var dal = new BankInformationDAL();
                IBankInformation dal = OrdinaryVATDesktop.GetObject<BankInformationDAL, BankInformationRepo, IBankInformation>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = dal.GetExcelData(ids,null,null,connVM);



                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("There is no data found");
                    return;
                }

                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcel(dt, "Banks", "Bank");

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "Vendors");

                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //=====================

    }
}
