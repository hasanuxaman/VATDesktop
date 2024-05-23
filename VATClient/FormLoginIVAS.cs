using SymphonySofttech.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormLoginIVAS : Form
    {
        #region Variable

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool ChangeData = false;
        private bool SuperAdministrator = false;
        private static bool IsIVASUser = false;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataTable UserResult;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        public FormLoginIVAS()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static bool SelectOne()
        {
            bool isSymphonyUser;
            IsIVASUser = false;
            FormLoginIVAS frmSupper = new FormLoginIVAS();
            frmSupper.ShowDialog();

            isSymphonyUser = IsIVASUser;

            return isSymphonyUser;

        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                if (ErrorReturn() != 0)
                {
                    return;
                }

                //SuperNamelist();
                this.btnLogIn.Enabled = false;
                IsIVASUser = false;
                this.progressBar1.Visible = true;
                bgwLogin.RunWorkerAsync();

                #endregion

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
                FileLogger.Log(this.Name, "btnLogIn_Click", exMessage);
            }
            #endregion

        }

        private void bgwLogin_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork
                IsIVASUser = false;

                UserResult = new DataTable();
                string userName = txtUserName.Text;
                string password = txtUserPassword.Text;

                UserInformationDAL userInformationDal = new UserInformationDAL();
                UserResult = userInformationDal.SearchIVASUserHas(userName, password, null, null, connVM);

                // End DoWork

                #endregion

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
                FileLogger.Log(this.Name, "bgwLogin_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UserResult != null && UserResult.Rows.Count > 0)
                {

                    string DBUserName = UserResult.Rows[0]["UserName"].ToString();
                    string DBPassword = Converter.DESDecrypt(PassPhrase, EnKey, UserResult.Rows[0]["UserPassword"].ToString());

                    if (UserResult.Rows[0]["ActiveStatus"].ToString() != "Y")
                    {
                        IsIVASUser = false;

                        MessageBox.Show("User not active");
                        return;
                    }

                    if (txtUserName.Text.Trim() != DBUserName.Trim())
                    {
                        IsIVASUser = false;
                        MessageBox.Show("User not exist");
                        txtUserName.Focus();
                        return;
                    }

                    else if (txtUserPassword.Text.Trim() != DBPassword.Trim())
                    {
                        IsIVASUser = false;
                        MessageBox.Show("Password not match");
                        txtUserPassword.Focus();
                        return;
                    }

                    IsIVASUser = true;
                    this.Close();

                }
                else
                {
                    IsIVASUser = false;
                    MessageBox.Show("User not exist");
                    txtUserName.Focus();
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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLogin_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnLogIn.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        public int ErrorReturn()
        {
            #region try
            try
            {

                if (string.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
                {
                    MessageBox.Show("Please enter user name.");
                    txtUserName.Focus();
                    return 1;
                }
                if (string.IsNullOrWhiteSpace(txtUserPassword.Text.Trim()))
                {
                    MessageBox.Show("Please enter password.");
                    txtUserPassword.Focus();
                    return 1;
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            #endregion
            return 0;
        }


    }
}
