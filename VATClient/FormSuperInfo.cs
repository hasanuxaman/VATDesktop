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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormSuperInfo : Form
    {
        private bool ChangeData = false;
        public FormSuperInfo()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                if (chkIsWindowsAuthentication.Checked == true)
                {
                    if (string.IsNullOrEmpty(txtDataSource.Text.Trim()))
                    {
                        MessageBox.Show("All field must fillup", this.Text);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtUserName.Text.Trim())
                        || string.IsNullOrEmpty(txtUserPassword.Text.Trim())
                        || string.IsNullOrEmpty(txtDataSource.Text.Trim()))
                    {
                        MessageBox.Show("All field must fillup", this.Text);
                        return;
                    }
                }

                SysDBInfoVM.SysUserName = txtUserName.Text.Trim();
                SysDBInfoVM.SysPassword = txtUserPassword.Text.Trim();
                SysDBInfoVM.SysdataSource = txtDataSource.Text.Trim();
                SysDBInfoVM.IsWindowsAuthentication = chkIsWindowsAuthentication.Checked;
                if( Program.SaveToSuperFile(Converter.DESEncrypt(PassPhrase, EnKey, txtUserName.Text.Trim()),
                                        Converter.DESEncrypt(PassPhrase, EnKey, txtUserPassword.Text.Trim()),
                                        Converter.DESEncrypt(PassPhrase, EnKey, txtDataSource.Text.Trim()),
                                        Converter.DESEncrypt(PassPhrase, EnKey, chkIsWindowsAuthentication.Checked ==true?"Y":"N".ToString())
                                        )==false)
                {
                    MessageBox.Show("Data not save ", this.Text);

                }
                else
                {
                    MessageBox.Show("Data save successfully", this.Text);
                    
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
                FileLogger.Log(this.Name, "btnLogIn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnLogIn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnLogIn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnLogIn_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnLogIn_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnLogIn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnLogIn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnLogIn_Click", exMessage);
            }
            #endregion
            finally
            {
                progressBar1.Visible = false;
            }


        }

        private void FormSuperInfo_Load(object sender, EventArgs e)
        {
            LoadExist();
        }
        private bool LoadExist()
        {
            try
            {
             if (System.IO.File.Exists(Program.AppPath + "/SuperInformation.xml"))
             {
                 System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                 doc.Load(Program.AppPath + "/SuperInformation.xml");
                 DataSet ds = new DataSet();

                 ds.ReadXml(Program.AppPath + "/SuperInformation.xml");

                 txtUserName.Text = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["Tom"].ToString());
                 txtUserPassword.Text =  Converter.DESDecrypt(PassPhrase, EnKey,ds.Tables[0].Rows[0]["jery"].ToString());
                 txtDataSource.Text =  Converter.DESDecrypt(PassPhrase, EnKey,ds.Tables[0].Rows[0]["mini"].ToString());
                 //doc.Load("");
                 ds.Clear();
             }
             else
             {
                 MessageBox.Show("System Login information not found",this.Text);                 
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
                FileLogger.Log(this.Name, "LoadExist", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "LoadExist", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "LoadExist", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "LoadExist", exMessage);
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
                FileLogger.Log(this.Name, "LoadExist", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "LoadExist", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "LoadExist", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "LoadExist", exMessage);
            }
            #endregion
            return false;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestConn_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConn.Enabled = false;
                progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                if (
                    commonDal.TestConnection(txtUserName.Text.Trim(), txtUserPassword.Text.Trim(),
                                             txtDataSource.Text.Trim(), chkIsWindowsAuthentication.Checked,connVM) == true)
                {
                    MessageBox.Show("Database Connection Stablished successfully", this.Text);
                }
                else
                {
                    MessageBox.Show("Database Connection not Stablish", this.Text);

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
                FileLogger.Log(this.Name, "btnTestConn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnTestConn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnTestConn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnTestConn_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnTestConn_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTestConn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnTestConn_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnTestConn_Click", exMessage);
            }
            #endregion
            finally
            {
                btnTestConn.Enabled = true;
                progressBar1.Visible = false;
            }

        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtUserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtDataSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtUserPassword_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtDataSource_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void chkIsWindowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            txtUserName.Text = "";
            txtUserPassword.Text = "";
        }
    }
}
