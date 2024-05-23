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
//
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
    public partial class FormUserSearch : Form
    {
        #region Constructors

        public FormUserSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
           
        //    connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        //    connVM.SysPassword = SysDBInfoVM.SysPassword;
        //    connVM.SysUserName = SysDBInfoVM.SysUserName;
        //    connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #endregion

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string SelectedValue = string.Empty;
        private DataTable UserResult;
        private string activeStatus = string.Empty;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;

        #endregion

        #region Methods

        private void GridSelected()
        {
            try
            {
                if (dgvUserInformation.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (dgvUserInformation.Rows.Count > 0)
                {
                    string UserInfo = string.Empty;
                    int ColIndex = dgvUserInformation.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvUserInformation.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        if (Program.fromOpen != "Me" &&
                            dgvUserInformation.Rows[RowIndex1].Cells["ActiveStatusN"].Value.ToString() != "Y")
                        {
                            MessageBox.Show("This Selected Item is not Active");
                            return;
                        }
                        string UserID = dgvUserInformation.Rows[RowIndex1].Cells["UserID"].Value.ToString();
                        string UserName = dgvUserInformation.Rows[RowIndex1].Cells["UserName"].Value.ToString();
                        string ActiveStatus = dgvUserInformation.Rows[RowIndex1].Cells["ActiveStatusN"].Value.ToString();
                        string Password = dgvUserInformation.Rows[RowIndex1].Cells["Password"].Value.ToString();
                        string FullName = dgvUserInformation.Rows[RowIndex1].Cells["FullName"].Value.ToString();
                        string Designation = dgvUserInformation.Rows[RowIndex1].Cells["Designation"].Value.ToString();
                        string ContactNo = dgvUserInformation.Rows[RowIndex1].Cells["ContactNo"].Value.ToString();
                        string Email = dgvUserInformation.Rows[RowIndex1].Cells["Email"].Value.ToString();
                        string Address = dgvUserInformation.Rows[RowIndex1].Cells["Address"].Value.ToString();
                        string IsMainSettings = dgvUserInformation.Rows[RowIndex1].Cells["IsMainSettings"].Value.ToString();
                        string NationalID = dgvUserInformation.Rows[RowIndex1].Cells["NationalID"].Value.ToString();
                        string IsLock = dgvUserInformation.Rows[RowIndex1].Cells["IsLock"].Value.ToString();
                        //string Signature = dgvUserInformation.Rows[RowIndex1].Cells["Signature"].Value.ToString();


                        UserInfo =
                            UserID + FieldDelimeter +
                            UserName + FieldDelimeter +
                            ActiveStatus + FieldDelimeter +
                            Password+ FieldDelimeter +
                            FullName + FieldDelimeter +
                            Designation + FieldDelimeter +
                            ContactNo + FieldDelimeter +
                            Address + FieldDelimeter +
                            IsMainSettings + FieldDelimeter + Email + FieldDelimeter + NationalID + FieldDelimeter + IsLock 
                            
                            ;

                        SelectedValue = UserInfo;

                    }
                }
                this.Close();
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

        public static string SelectOne()
        {
            string searchValue = string.Empty;
            try
            {
                FormUserSearch frmUserSearch = new FormUserSearch();
                frmUserSearch.ShowDialog();
                searchValue = frmUserSearch.SelectedValue;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormUserSearc", "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormUserSearc", "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormUserSearc", "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormUserSearc", "Search", exMessage);
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormUserSearc", "Search", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormUserSearc", "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormUserSearc", "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormUserSearc", "Search", exMessage);
            }
            #endregion Catch

            return searchValue;
        }

        private void ClearAllFields()
        {
            try
            {
                txtUserID.Text = "";
                txtUserName.Text = "";
                cmbActive.Text = "";
                dgvUserInformation.Rows.Clear();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }

        private void Search()
        {


            try
            {
                activeStatus = string.Empty;

                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                this.btnSearch.Enabled= false;
                this.progressBar1.Visible = true;
                bgwUser.RunWorkerAsync();


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

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        #region TextBox KeyDown Event

        private void txtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpLastLoginFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpLastLoginToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void cmbActiveStatus_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void FormSysUserInformationSearch_Load(object sender, EventArgs e)
        {
            Search();
        }

        private void dgvUserInformation_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvUserInformation_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        #region backgournWorker Event

        private void bgwUser_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork

                //UserResult = UserInformationDAL.SearchUserDT(txtUserID.Text.Trim(), txtUserName.Text.Trim(), activeStatus, Program.DatabaseName);

                //UserInformationDAL userInformationDal = new UserInformationDAL();
                IUserInformation userInformationDal = OrdinaryVATDesktop.GetObject<UserInformationDAL, UserInformationRepo, IUserInformation>(OrdinaryVATDesktop.IsWCF);

                UserResult = userInformationDal.SearchUserDataTable(txtUserID.Text.Trim(), txtUserName.Text.Trim(), activeStatus, Program.DatabaseName,connVM);

                // End DoWork

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUser_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUser_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUser_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUser_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUser_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUser_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUser_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUser_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwUser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                dgvUserInformation.Rows.Clear();
                int j = 0;
                foreach (DataRow item2 in UserResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvUserInformation.Rows.Add(NewRow);
                    dgvUserInformation.Rows[j].Cells["Select"].Value = false;// Convert.ToDecimal(UserFields[0]);
                    dgvUserInformation.Rows[j].Cells["UserID"].Value = item2["UserID"].ToString();// Convert.ToDecimal(UserFields[0]);
                    dgvUserInformation.Rows[j].Cells["UserName"].Value = item2["UserName"].ToString();//UserFields[1].ToString();
                    dgvUserInformation.Rows[j].Cells["ActiveStatusN"].Value = item2["ActiveStatus"].ToString();//UserFields[2].ToString();
                    dgvUserInformation.Rows[j].Cells["Password"].Value = item2["UserPassword"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["FullName"].Value = item2["FullName"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["Designation"].Value = item2["Designation"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["ContactNo"].Value = item2["ContactNo"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["Email"].Value = item2["Email"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["NationalID"].Value = item2["NationalID"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["Address"].Value = item2["Address"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["IsMainSettings"].Value = item2["IsMainSettings"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["IsLock"].Value = item2["IsLock"].ToString() == "True" ? "Y" : "N";//UserFields[3].ToString();
                    //if ((byte[])(item2["Signature"]) != null)
                    //{
                    //    byte[] bimge = (byte[])(item2["Signature"]);
                    //}
                    
                    
                    j = j + 1;
                }

                // End Complete

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                LRecordCount.Text = "Record Count: " + dgvUserInformation.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
          
        }

        #endregion

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {

                var ids = new List<string>();
                var len = dgvUserInformation.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvUserInformation["Select", i].Value))
                    {
                    ids.Add(dgvUserInformation["UserID", i].Value.ToString());
                    }
                }
                IUserInformation userdal = OrdinaryVATDesktop.GetObject<UserInformationDAL, UserInformationRepo, IUserInformation>(OrdinaryVATDesktop.IsWCF);

                //var userdal = new UserInformationDAL();

                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = userdal.GetExcelData(ids,null,null,connVM);

                //var ds = new DataSet();
                //var NameSheet = new[] { "UserInformation" };
                //ds.Tables.Add(dt);

                OrdinaryVATDesktop.SaveExcel(dt, "UserInformation", "UserInformation");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvUserInformation.RowCount; i++)
            {
                dgvUserInformation["Select", i].Value = chkSelectAll.Checked;
            }
        }

    }
}
