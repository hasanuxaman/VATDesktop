using SymphonySofttech.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormUserCreateIVAS : Form
    {
        #region Global Variables

        private string NextID = string.Empty;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string activeStatus = string.Empty;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool ChangeData = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        public FormUserCreateIVAS()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                if (btnAdd.Text == "&Add")
                {
                    NextID = "";
                    if (txtUserPassword.Text.Trim() != txtUserPasswordC.Text.Trim())
                    {
                        MessageBox.Show("Password not Matched Please Confirm Again");
                        txtUserPassword.Text = "";
                        txtUserPasswordC.Text = "";
                        return;
                    }
                }
                else
                {
                    NextID = txtUserID.Text.Trim();
                }

                activeStatus = string.Empty;
                activeStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");

                this.progressBar1.Visible = true;

                bgwSave.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion Catch
        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                UserInformationVM vm = new UserInformationVM();

                vm.UserID = NextID;
                vm.UserName = txtUserName.Text.Trim();
                vm.UserPassword = Converter.DESEncrypt(PassPhrase, EnKey, txtUserPassword.Text.Trim());
                vm.FullName = txtFullName.Text.Trim();
                vm.ActiveStatus = activeStatus == "Y";
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                UserInformationDAL userInformationDal = new UserInformationDAL();

                sqlResults = userInformationDal.InsertIVASUserInformation(vm, null, null, connVM); // Change 04

                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (SAVE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUserID.Text = newId;
                            if (result == "Success")
                            {
                                txtUserName.ReadOnly = true;
                                txtUserPassword.ReadOnly = true;
                                txtUserPasswordC.ReadOnly = true;
                                this.btnAdd.Enabled = false;
                                btnUpdate.Enabled = true;
                            }
                            else
                            {
                                this.btnAdd.Enabled = true;
                                btnUpdate.Enabled = true;
                            }

                        }

                    }
                }

                #endregion
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = false;
            }

        }

        private int ErrorReturn()
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (txtUserName.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter User Name.");
                        txtUserName.Focus();
                        return 1;
                    }
                    ////var regexItem = new Regex("[^A-Za-z0-9]");

                    ////if (regexItem.IsMatch(txtUserName.Text.Trim()))
                    ////{
                    ////    MessageBox.Show("User name only Alphabets & Numeric.Space not allowed");
                    ////    txtUserName.Focus();
                    ////    return 1;
                    ////}

                    if (txtUserPassword.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Password.");
                        txtUserPassword.Focus();
                        return 1;
                    }
                    //if (txtUserPassword.Text.Length <= 5)
                    //{
                    //    MessageBox.Show("Please Enter Password At Least 6 Digit");
                    //    txtUserPassword.Focus();
                    //    return 1;
                    //}
                    if (txtUserPasswordC.Text.Trim() == "")
                    {
                        MessageBox.Show("Please Confirm Password Again.");
                        txtUserPasswordC.Focus();
                        return 1;
                    }

                    if (txtFullName.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Full Name.");
                        txtFullName.Focus();
                        return 1;
                    }

                    int i;

                }
                else
                {
                    if (txtUserName.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter User Name.");
                        txtUserName.Focus();
                        return 1;
                    }
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            #endregion Catch
            return 0;
        }

        private void ClearAll()
        {
            try
            {
                txtUserID.Text = "";
                txtUserName.Text = "";
                txtUserPassword.Text = "";
                txtUserPasswordC.Text = "";
                txtFullName.Text = "";
                txtUserName.ReadOnly = false;
                txtUserPassword.ReadOnly = false;
                txtUserPasswordC.ReadOnly = false;
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            #endregion

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("No data updated." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter User Name.");
                    txtUserName.Focus();
                    return;
                }
                activeStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");

                this.progressBar1.Visible = true;
                this.btnUpdate.Enabled = false;

                backgroundWorkerUpdate.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                UserInformationVM vm = new UserInformationVM();

                vm.UserID = txtUserID.Text;
                vm.UserName = txtUserName.Text.Trim();
                vm.FullName = txtFullName.Text.Trim();
                vm.ActiveStatus = activeStatus == "Y";
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                UserInformationDAL userInformationDal = new UserInformationDAL();

                sqlResults = userInformationDal.UpdateIVASUserInformation(vm, null, null, connVM);

                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (SAVE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUserID.Text = newId;
                            if (result == "Success")
                            {
                                txtUserName.ReadOnly = true;
                                txtUserPassword.ReadOnly = true;
                                txtUserPasswordC.ReadOnly = true;
                                this.btnAdd.Enabled = false;
                                btnUpdate.Enabled = true;
                            }
                            else
                            {
                                this.btnAdd.Enabled = true;
                                btnUpdate.Enabled = true;
                            }

                        }

                    }
                }



                #endregion
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                Program.fromOpen = "Me";
                string result = FormUserSearchIVAS.SelectOne();
                if (result == "")
                {
                    return;
                }
                else 
                {
                    string[] UserInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtUserID.Text = UserInfo[0];
                    txtUserName.Text = UserInfo[1];
                    chkActiveStatus.Checked = UserInfo[2] == "Y";
                    txtFullName.Text = UserInfo[4];

                }

                txtUserPassword.Enabled = false;
                txtUserPasswordC.Enabled = false;
                txtUserName.ReadOnly = true;
                ChangeData = false;
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
        }




    }
}
