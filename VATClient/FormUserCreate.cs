// ---------form //
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
////
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using System.Text.RegularExpressions;
//using VATClient.UserInformation;

namespace VATClient
{
    public partial class FormUserCreate : Form
    {
        #region Constructors

        public FormUserCreate()
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
        private bool ChangeData = false;
        private string NextID = string.Empty;
        public string VFIN = "41";
        private string activeStatus = string.Empty;
        private byte[] byteImage = new byte[0]; 

        #region Global Variables For BackGroundWorker

        private string result = string.Empty;

        #endregion

        #region sql save, update, delete

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
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
                    var regexItem = new Regex("[^A-Za-z0-9]");

                    if (regexItem.IsMatch(txtUserName.Text.Trim()))
                    {
                        MessageBox.Show("User name only Alphabets & Numeric.Space not allowed");
                        txtUserName.Focus();
                        return 1;
                    }
                    if (txtUserPassword.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Password.");
                        txtUserPassword.Focus();
                        return 1;
                    }
                    if (txtUserPassword.Text.Length <= 5)
                    {
                        MessageBox.Show("Please Enter Password At Least 6 Digit");
                        txtUserPassword.Focus();
                        return 1;
                    }
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

                    if (txtDesignation.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Designation.");
                        txtDesignation.Focus();
                        return 1;
                    }

                    if (txtContactNo.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Contact No.");
                        txtContactNo.Focus();
                        return 1;
                    }
                    if (txtEmail.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Email.");
                        txtEmail.Focus();
                        return 1;
                    }
                    else
                    {
                        if ( !IsValidEmail(txtEmail.Text.Trim()))
                        {
                             MessageBox.Show("Email Address Not Valid");
                        txtEmail.Focus();
                        return 1;
                        } 
                    }
                    if (txtNationalID.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter National ID No.");
                        txtNationalID.Focus();
                        return 1;
                    }

                    int i;
                    ////////if (!int.TryParse(txtNationalID.Text, out i))//|| !int.TryParse(txtNationalID.Text, out i)
                    ////////{
                    ////////    MessageBox.Show("Please Enter National ID  only number");
                    ////////    txtNationalID.Focus();
                    ////////    return 1;
                    ////////}

                    if (!OrdinaryVATDesktop.IsNumber(txtNationalID.Text))
                    {
                        MessageBox.Show("Please Enter National ID  only number");
                        txtNationalID.Focus();
                        return 1;
                    }

                    //if (txtNationalID.Text.Length <= 9)
                    //{
                    //    MessageBox.Show("Please Enter Valid National ID No.");
                    //    txtNationalID.Focus();
                    //    return 1;
                    //}

                    if (txtNationalID.Text.Length >= 18)
                    {
                        MessageBox.Show("Please Enter Valid National ID No/Not more than 17 digit.");
                        txtNationalID.Focus();
                        return 1;
                    }

                    if (txtAddress.Text.Trim() == "")
                    {
                        MessageBox.Show("Please enter Address.");
                        txtAddress.Focus();
                        return 1;
                    }
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
                txtDesignation.Text = "";
                txtContactNo.Text = "";
                txtAddress.Text = "";
                txtNationalID.Text = "";
                txtEmail.Text = "";
                txtUserName.ReadOnly = false;
                txtNationalID.ReadOnly = false;
                txtUserPassword.ReadOnly = false;
                txtUserPasswordC.ReadOnly = false;
                chkIsLock.Checked = false;
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
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAll", exMessage);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            #endregion

        }

        private void ListViewSelect()
        {
            try
            {
                txtUserName.Text = listView1.SelectedItems[0].SubItems[1].Text;
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
                FileLogger.Log(this.Name, "ListViewSelect", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ListViewSelect", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ListViewSelect", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ListViewSelect", exMessage);
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
                FileLogger.Log(this.Name, "ListViewSelect", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ListViewSelect", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ListViewSelect", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ListViewSelect", exMessage);
            }
            #endregion
        }

        #endregion

        private void FormUserCreate_Load(object sender, EventArgs e)
        {
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            //try
            //{


            //    if (e.KeyCode.Equals(Keys.Down))
            //    {
            //        //UserInformationLoad(txtUserName.Text);
            //        if (listView1.Items.Count <= 0)
            //        {
            //            MessageBox.Show("There Have No Data to Selected");
            //            listView1.Visible = false;
            //            return;
            //        }
            //        else
            //        {

            //            listView1.Visible = true;
            //            //listView1.Focus();
            //        }

            //        listView1.Visible = true;
            //        listView1.Focus();

            //    }

            //    if (e.KeyCode.Equals(Keys.Enter))
            //    {
            //        SendKeys.Send("{TAB}");
            //    }
            //}
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "txtUserName_KeyDown", exMessage);
            //}
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.KeyCode.Equals(Keys.Enter))
                {
                    ListViewSelect();
                    if (txtUserName.Text.Trim() == "")
                    {
                        MessageBox.Show("Data Not Selected");
                        listView1.Visible = false;
                        return;
                    }

                    txtUserName.Focus();
                    listView1.Visible = false;
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
                FileLogger.Log(this.Name, "listView1_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "listView1_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "listView1_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "listView1_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "listView1_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "listView1_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "listView1_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "listView1_KeyDown", exMessage);
            }
            #endregion 
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
        }

        private void textFind_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //UserInformationLoad(txtUserName.Text);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under constraction");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                        ClearAll();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
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

        #region TextBox KeyDown Event

        private void txtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUserPasswordC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }
        }

        #endregion

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {
            try
            {


                if (btnAdd.Text == "Add")
                {
                    txtUserPassword.Enabled = true;
                    txtUserPasswordC.Enabled = true;
                }
                else if (btnAdd.Text == "Save")
                {
                    txtUserPassword.Enabled = false;
                    txtUserPasswordC.Enabled = false;
                }
                ChangeData = true;
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
                FileLogger.Log(this.Name, "txtUserID_TextChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "txtUserID_TextChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "txtUserID_TextChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "txtUserID_TextChanged", exMessage);
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
                FileLogger.Log(this.Name, "txtUserID_TextChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtUserID_TextChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtUserID_TextChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "txtUserID_TextChanged", exMessage);
            }
            #endregion
        }

        private void FormUserCreate_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void FormUserCreate_FormClosing(object sender, FormClosingEventArgs e)
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
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormUserCreate_FormClosing", exMessage);
            }
            #endregion
        }

        private void txtUserName_TextChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUserPassword_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUserPasswordC_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormUserCreate_Load_1(object sender, EventArgs e)
        {
            try
            {


                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearch, "Existing Information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ClearAll();
                //btnAdd.Text = "&Add";
                txtUserID.Text = "~~~ New ~~~";

                ChangeData = false;
                txtUserPassword.Enabled = true;
                txtUserPasswordC.Enabled = true;
                btnUpdate.Enabled = false;
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
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormUserCreate_Load_1", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormUserCreate_Load_1", exMessage);
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
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormUserCreate_Load_1", exMessage);
            }
            #endregion

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //no DAL Side Method
            try
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                Program.fromOpen = "Me";
                string result = FormUserSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] UserInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtUserID.Text = UserInfo[0];
                    txtUserName.Text = UserInfo[1];

                    chkActiveStatus.Checked = UserInfo[2] == "Y";
                    txtFullName.Text = UserInfo[4];
                    txtDesignation.Text = UserInfo[5];
                    txtContactNo.Text = UserInfo[6];
                    txtAddress.Text = UserInfo[7];
                    chkSettings.Checked = UserInfo[8] == "Y";
                    txtEmail.Text = UserInfo[9];
                    txtNationalID.Text = UserInfo[10];
                    chkIsLock.Checked = UserInfo[11] == "Y";

                    DataTable dt = new UserInformationDAL().SearchUserDataTable(UserInfo[0], "", "", "", connVM);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Assuming 'Signature' is the 12th column (index 11)
                        object imageData = dt.Rows[0][12]; // Change the index according to your data structure
                       
                        if (imageData != DBNull.Value)
                        {
                            byte[] byteArr = (byte[])imageData; // Assuming the 'Signature' column contains byte array data
                            byteImage = byteArr;
                            if (!byteArr.SequenceEqual(new byte[0]))
                            {
                                // Convert byte array to an Image
                                using (MemoryStream ms = new MemoryStream(byteArr))
                                {
                                    Image img = Image.FromStream(ms);

                                    // Display the image in a PictureBox
                                    pictureBoxSignature.Image = img;
                                }
                            }
                            else
                            {
                                pictureBoxSignature.Image = null;
                            }
                        }
                        else
                        {
                            pictureBoxSignature.Image = null;
                        }
                    }

                    //if (UserInfo[2] == "Y")
                    //{
                    //    chkActiveStatus.Checked = true;
                    //}
                    //else
                    //{
                    //    chkActiveStatus.Checked = false;
                    //}

                }

                if (Program.CurrentUser!="admin")
                {
                    chkIsLock.Visible = false;
                }
               

                txtUserPassword.Enabled = false;
                txtUserPasswordC.Enabled = false;
                txtUserName.ReadOnly = true;
                // txtNationalID.ReadOnly = true;
                //btnAdd.Text = "&Save";
                ChangeData = false;
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
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
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
                        ClearAll();
                        //btnAdd.Text = "&Add";
                        txtUserID.Text = "~~~ New ~~~";
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    ClearAll();
                    btnAdd.Enabled = true;
                    txtUserID.Text = "~~~ New ~~~";
                    ChangeData = false;
                }
                txtUserPassword.Enabled = true;
                txtUserPasswordC.Enabled = true;
                btnAdd.Enabled = true;
                btnUpdate.Enabled = false;

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
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
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
                    NextID = ""; //"DBConstant.NextIDFinder("UserInformations", "UserID").ToString();
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
            #endregion Catch

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try
            try
            {


                //string UserInformationData = string.Empty;

                if (txtUserID.Text == "")
                {
                    MessageBox.Show("No data updated." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("No data updated." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter User Name.");
                    txtUserName.Focus();
                    return;
                }
                activeStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                //int ErR = ErrorReturn();

                //if (ErR != 0)
                //{
                //    return;
                //}

                //UserInformationData = txtUserID.Text.Trim() + FieldDelimeter + //Change 02               
                //                      txtUserName.Text.Trim() + FieldDelimeter +
                //    // ------- Common Fields -----------//
                //                      Convert.ToString(chkActiveStatus.Checked ? "Y" : "N") + FieldDelimeter +
                //                      Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + FieldDelimeter +
                //                      "Info1" + FieldDelimeter + "Info2" + FieldDelimeter + "Info3" + FieldDelimeter +
                //                      "Info4" + FieldDelimeter + "Info5" + LineDelimeter;

                this.progressBar1.Visible = true;
                this.btnUpdate.Enabled = false;

                backgroundWorkerUpdate.RunWorkerAsync();

                #region do work
                //string encriptedUserInformationData = Converter.DESEncrypt(PassPhrase, EnKey, UserInformationData);
                //UserInformationSoapClient UserInformationUpdate = new UserInformationSoapClient();

                //string result = UserInformationUpdate.Update(encriptedUserInformationData.ToString(), Program.DatabaseName); // Change 04
                #endregion

                #region complete
                //if (Convert.ToDecimal(result) < 0)
                //{
                //    MessageBox.Show("Data Not Update", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //else
                //{
                //    MessageBox.Show("Data update successfully", "Create User", MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    ChangeData = false;
                //}
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

        #region backgroundWorker Event

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                // Start DoWork

                //result = UserInformationDAL.InsertToUserInformation(NextID, txtUserName.Text.Trim(), Converter.DESEncrypt(PassPhrase, EnKey, txtUserPassword.Text.Trim()), activeStatus, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")), Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")), Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")), Program.DatabaseName); // Change 04

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                byte[] bimage = null;

                string image = pictureBoxSignature.Text;

                if (!string.IsNullOrEmpty(image))
                {
                    Bitmap bmp = new Bitmap(image);
                    FileStream fs = new FileStream(image, FileMode.Open, FileAccess.Read);
                    bimage = new byte[fs.Length];
                    fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }
                else
                {
                    bimage = new byte[0];
                }


                //UserInformationDAL userInformationDal = new UserInformationDAL();
                IUserInformation userInformationDal = OrdinaryVATDesktop.GetObject<UserInformationDAL, UserInformationRepo, IUserInformation>(OrdinaryVATDesktop.IsWCF);


                sqlResults = userInformationDal.InsertUserInformation(NextID, txtUserName.Text.Trim(), 
                                                    Converter.DESEncrypt(PassPhrase, EnKey, txtUserPassword.Text.Trim()),txtFullName.Text.Trim()
                                                    , txtDesignation.Text.Trim()
                                                    , txtContactNo.Text.Trim()
                                                    , txtEmail.Text.Trim()
                                                    , txtAddress.Text.Trim()
                                                    , activeStatus
                                                    , chkSettings.Checked ? "Y":"N",
                                                    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"), Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                                                    Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"), Program.DatabaseName, connVM, txtNationalID.Text.Trim(), bimage); // Change 04

                SAVE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                                txtNationalID.ReadOnly = true;
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

                        //btnAdd.Text = "&Save";
                      

                    }
                }

                // Start Complete

                //if (Convert.ToDecimal(result) == -266)
                //{
                //    MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    this.btnAdd.Visible = true;
                //    this.progressBar1.Visible = false;
                //    return;
                //}
                //else if (Convert.ToDecimal(result) < 0)
                //{
                //    MessageBox.Show("Data Not Saved", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    this.btnAdd.Visible = true;
                //    this.progressBar1.Visible = false;
                //    return;
                //}
                //else
                //{
                //    MessageBox.Show("Data save successfully", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //    txtUserID.Text = result.ToString();
                //    btnAdd.Text = "&Save";
                //    ChangeData = false;
                //    this.btnAdd.Visible = true;
                //    this.progressBar1.Visible = false;
                //}

                // End Complete

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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region do work

                //string encriptedUserInformationData = Converter.DESEncrypt(PassPhrase, EnKey, UserInformationData);
                //UserInformationSoapClient UserInformationUpdate = new UserInformationSoapClient();
                //result = UserInformationDAL.UpdateUserInformation(txtUserID.Text.Trim(), txtUserName.Text.Trim(), Convert.ToString(chkActiveStatus.Checked ? "Y" : "N"), Program.CurrentUser, DateTime.Now, "Info1", "Info2", "Info3", "Info4", "Info5", Program.DatabaseName); // Change 04

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                string image = pictureBoxSignature.Text;
                byte[] bimage=null;
                if (!string.IsNullOrEmpty(image))
                {
                    Bitmap bmp = new Bitmap(image);
                    FileStream fs = new FileStream(image, FileMode.Open, FileAccess.Read);
                     bimage = new byte[fs.Length];
                    fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }
                else
                {
                    bimage = byteImage;
                    byteImage = new byte[0]; 
                }
                //UserInformationDAL userInformationDal = new UserInformationDAL();
                IUserInformation userInformationDal = OrdinaryVATDesktop.GetObject<UserInformationDAL, UserInformationRepo, IUserInformation>(OrdinaryVATDesktop.IsWCF);

                //BugsBD
                string UserID = OrdinaryVATDesktop.SanitizeInput(txtUserID.Text.Trim());
                string UserName = OrdinaryVATDesktop.SanitizeInput(txtUserName.Text.Trim());
                string FUllName = OrdinaryVATDesktop.SanitizeInput(txtFullName.Text.Trim());
                string Designation = OrdinaryVATDesktop.SanitizeInput(txtDesignation.Text.Trim());
                string ContactNo = OrdinaryVATDesktop.SanitizeInput(txtContactNo.Text.Trim());
                string Email = OrdinaryVATDesktop.SanitizeInput(txtEmail.Text.Trim());
                string Address = OrdinaryVATDesktop.SanitizeInput(txtAddress.Text.Trim());
                string NationalID = OrdinaryVATDesktop.SanitizeInput(txtNationalID.Text.Trim());

                sqlResults = userInformationDal.UpdateUserInformation(

                      UserID
                    , UserName
                    , FUllName
                    , Designation
                    , ContactNo
                    , Email
                    , Address
                    , activeStatus, chkSettings.Checked ? "Y" : "N"
                    , Program.CurrentUser
                    , DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")
                    , Program.DatabaseName
                    , connVM
                    , NationalID
                    , chkIsLock.Checked
                    , bimage); 

                    //txtUserID.Text.Trim()
                    //,txtUserName.Text.Trim()
                    //,txtFullName.Text.Trim()
                    //, txtDesignation.Text.Trim()
                    //, txtContactNo.Text.Trim()
                    //, txtEmail.Text.Trim()
                    //, txtAddress.Text.Trim()
                    //, activeStatus, chkSettings.Checked ? "Y" : "N", Program.CurrentUser,
                    //DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"), Program.DatabaseName, connVM, txtNationalID.Text.Trim(), chkIsLock.Checked, bimage); // Change 04

                UPDATE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                #region complete

                if (UPDATE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUserID.Text = newId;
                        }
                       
                    }
                }

                //if (Convert.ToDecimal(result) < 0)
                //{
                //    MessageBox.Show("Data Not Update", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //else
                //{
                //    MessageBox.Show("Data update successfully", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    ChangeData = false;
                //}

                #endregion
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
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        #endregion

        private void btnSignature_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = "Image Files (*.jpg;*.jpeg;*.gif;*.png;)|*.jpg;*.jpeg;.*.gif;*.png;";
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxSignature.Image = new Bitmap(opnfd.FileName);
              
                // image file path
                pictureBoxSignature.Text = opnfd.FileName;
            }

           
        }

       

    }
}
