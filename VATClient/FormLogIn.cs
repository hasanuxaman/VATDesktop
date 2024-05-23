using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using MS.Internal.Xml.XPath;
using VATClient.ModelDTO;
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using System.Data.Odbc;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using Microsoft.Win32;
////
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;
using SymphonySofttech.Decrypt;


namespace VATClient
{
    public partial class FormLogIn : Form
    {
        #region Constructors

        public FormLogIn()
        {
            InitializeComponent();

            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #endregion

        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool ChangeData = false;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private const string VATUserName = "VATUserName";
        private const string LASTCOMPANY = "LASTCOMPANY";
        private bool _isExit = false;
        private string[] ReportLines;
        private string[] CReportLines;
        private DataTable UserResult;
        private DataSet companyList;
        private DataTable loginInfo;
        private List<CmpanyListVM> companies = new List<CmpanyListVM>();

        #endregion

        private string GetBits(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in Encoding.Unicode.GetBytes(input))
            {
                sb.Append(Convert.ToString(b, 2));
            }
            return sb.ToString();
        }

        private void FormLogIn_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                //this.BackColor = Color.Red;
                string bitString = GetBits("Blue Box");
                var tt1 = "";
                _isExit = false;
                dtpSessionDate.Value = DateTime.Now;
                Program.IsLoading = false;
                btnLogIn.Enabled = false;
                button1.Enabled = false;
                progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();

                if (commonDal.SuperInformationFileExist() == false)
                {
                    Program.successLogin = false;
                    FormSuperInfo frms = new FormSuperInfo();
                    //FormSupperAdministrator frms=new FormSupperAdministrator();
                    frms.ShowDialog();
                    return;
                }
                else if (SuperLoginInfo() == false)
                {
                    Program.successLogin = false;
                    FormSuperInfo frms = new FormSuperInfo();
                    frms.ShowDialog();
                    return;
                }


                else
                {

                    bgwCompanyList.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "FormLogIn_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormLogIn_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormLogIn_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormLogIn_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormLogIn_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormLogIn_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormLogIn_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormLogIn_Load", exMessage);
            }
            #endregion
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void LoadUserName()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Symphony");
            if (key == null) return;

            var company = cmbCompany.Text;
            var userName = key.GetValue(company);

            if (userName != null && !string.IsNullOrEmpty(userName.ToString()))
            {
                txtUserName.Text = userName.ToString();
            }

        }

        private string LoadValue(string keyName)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Symphony");
            if (key == null) return null;

            var userName = key.GetValue(keyName);

            return userName == null ? null : userName.ToString();
        }

        private bool SuperLoginInfo()
        {
            bool result = false;
            try
            {
                DataSet superInfo = new DataSet();
                CommonDAL commonDal = new CommonDAL();
                superInfo = commonDal.SuperDBInformation();  // super admin login info from XML file
                //done
                var tt = string.Empty;
                if (superInfo.Tables.Count <= 0)
                {

                    return result = false;

                }
                else if (superInfo.Tables[0].Rows.Count < 0)
                {
                    return result = false;


                }

                SysDBInfoVM.SysUserName = Converter.DESDecrypt(PassPhrase, EnKey, superInfo.Tables[0].Rows[0]["tom"].ToString());
                SysDBInfoVM.SysPassword = Converter.DESDecrypt(PassPhrase, EnKey, superInfo.Tables[0].Rows[0]["jery"].ToString());
                SysDBInfoVM.SysdataSource = Converter.DESDecrypt(PassPhrase, EnKey, superInfo.Tables[0].Rows[0]["mini"].ToString());
                return result = true;

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
                FileLogger.Log(this.Name, "SuperLoginInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return result = false;

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SuperLoginInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return result = false;
            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SuperLoginInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return result = false;
            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                    return result = false;

                }

                FileLogger.Log(this.Name, "SuperLoginInfo", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return result = false;

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace; return result = false;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SuperLoginInfo", exMessage); return result = false;
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SuperLoginInfo", ex.Message + Environment.NewLine + ex.StackTrace); return result = false;
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SuperLoginInfo", ex.Message + Environment.NewLine + ex.StackTrace); return result = false;
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                    return result = false;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SuperLoginInfo", exMessage);
                return result = false;
            }
            #endregion

            return result;
            //finally
            //{
            //    return result;
            //}
        }

        private void bgwCompanyList_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                companyList = new DataSet();
                CommonDAL commonDal = new CommonDAL();
                DataSet CompanyDs = new DataSet();

                Program.successLogin = false;

                string CompanyList = "All";
                try
                {
                    CompanyDs = commonDal.SettingInformation();

                    if (CompanyDs != null && CompanyDs.Tables[0].Rows.Count > 0)
                    {
                        CompanyList = CompanyDs.Tables[0].Rows[0]["CompanyList"].ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    CompanyList = "All";
                }

                companyList = commonDal.CompanyList("mXSJfsAdbf0=", null, CompanyList); //Y="mXSJfsAdbf0=";


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
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwCompanyList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                cmbCompany.Items.Clear();
                companies.Clear();
                if (companyList.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no Company to select", this.Text);
                    return;
                }
                foreach (DataRow item2 in companyList.Tables[0].Rows)
                {

                    var company = new CmpanyListVM();
                    company.CompanySl = item2["CompanySl"].ToString();
                    company.CompanyID = Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyID"].ToString());
                    company.CompanyName = Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyName"].ToString());
                    company.DatabaseName = Converter.DESDecrypt(PassPhrase, EnKey, item2["DatabaseName"].ToString());
                    company.ActiveStatus = Converter.DESDecrypt(PassPhrase, EnKey, item2["ActiveStatus"].ToString());
                    company.Serial = item2["Serial"].ToString();

                    cmbCompany.Items.Add(Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyName"].ToString()));
                    companies.Add(company);
                }

                cmbCompany.SelectedIndex = 0;

                var lastCompany = LoadValue(LASTCOMPANY);

                if (lastCompany != null)
                {
                    var index = cmbCompany.Items.IndexOf(lastCompany);

                    cmbCompany.SelectedIndex = index;
                }

                var commonDal = new CommonDAL();
                txtUserPassword.Text = commonDal.GetDevPass();
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
                FileLogger.Log(this.Name, "bgwCompanyList_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnLogIn.Enabled = true;
                button1.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        public int ErrorReturn()
        {
            #region try
            try
            {
                if (cmbCompany.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select company.");
                    txtUserPassword.Focus();
                    return 1;
                }
                if (txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter user name.");
                    txtUserName.Focus();
                    return 1;
                }
                if (txtUserPassword.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter password.");
                    txtUserPassword.Focus();
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

        private void btnLogIn_Click(object sender, EventArgs e)
        {

            #region try

            try
            {

                this.btnLogIn.Enabled = false;
                this.progressBar1.Visible = true;

                if (ErrorReturn() != 0)
                {
                    return;
                }

                if (SuperLoginInfo() == false)
                {

                    FormSuperInfo frms = new FormSuperInfo();
                    frms.Show();
                    return;
                }

                BDName();

                SessionDate();
                 

                Program.BranchCode = "login";
                bgwUserHas.RunWorkerAsync();
               

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
                FileLogger.Log(this.Name, "btnLogIn_Click", exMessage);
            }
            finally
            {
                this.btnLogIn.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion
        }

        public static bool IsAccountDisabled(DirectoryEntry user)
        {
            const string uac = "userAccountControl";
            if (user.NativeGuid == null) return false;

            if (user.Properties[uac] != null && user.Properties[uac].Value != null)
            {
                var userFlags = (UserFlags)user.Properties[uac].Value;
                return userFlags.Contains(UserFlags.AccountDisabled);
            }

            return false;
        }


        public bool DoesUserExist(string userName)
        {
            using (var domainContext = new PrincipalContext(ContextType.Domain, "symphonysoftt.local"))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName))
                {
                    return foundUser != null;
                }
            }
        }

        public bool CheckUserinAD(string domain, string username)
        {
            using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
            {
                using (var user = new UserPrincipal(domainContext))
                {
                    user.SamAccountName = username;

                    using (var pS = new PrincipalSearcher())
                    {
                        pS.QueryFilter = user;

                        using (PrincipalSearchResult<Principal> results = pS.FindAll())
                        {
                            if (results != null && results.Count() > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


        private void SaveUserName()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Symphony", true)
                      ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Symphony");

            if (key != null)
            {
                var companyName = cmbCompany.Text;
                key.SetValue(companyName, txtUserName.Text);
            }
        }

        private void SaveInReg(string keyName, string value)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Symphony", true)
                      ?? Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Symphony");

            if (key != null)
            {
                key.SetValue(keyName, value);
            }
        }

        private void SessionDate()
        {
            Program.SessionDate = Convert.ToDateTime(dtpSessionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
            BureauInfoVM.SessionDate = Program.SessionDate.ToString("yyyy-MMM-dd HH:mm:ss");

        }

        private void BDName()
        {
            #region try
            try
            {
                DatabaseInfoVM.DatabaseName = string.Empty;
                Program.CompanyID = string.Empty;

                var searchText = cmbCompany.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    var company = from prd in companies.ToList()
                                  where prd.CompanyName.ToLower() == searchText.ToLower()
                                  select prd;
                    if (company != null && company.Any())
                    {
                        var comp = company.First();
                        DatabaseInfoVM.DatabaseName = comp.DatabaseName;
                        Program.CompanyID = comp.CompanyID;
                        if (Program.IsTrial == true)
                        {
                            Program.Trial = Program.TrialComments;
                        }
                        else
                        {
                            Program.Trial = "";
                        }
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
                FileLogger.Log(this.Name, "BDName", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BDName", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BDName", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "BDName", exMessage);
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
                FileLogger.Log(this.Name, "BDName", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BDName", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BDName", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "BDName", exMessage);
            }
            #endregion
        }

        private void cmbCompany_Leave(object sender, EventArgs e)
        {
            txtUserPassword.Focus();
        }

        private int Companyprofile()
        {
            string Companyprofileresult = "-1";
            string a = "-1";
            #region try
            try
            {
                ReportDSDAL reportDsdal = new ReportDSDAL();
                DataSet ReportResult = reportDsdal.ComapnyProfileString(Program.CompanyID, Program.CurrentUserID);
                if (ReportResult.Tables[0].Rows.Count <= 0)
                {
                    //MessageBox.Show("Company not in database", this.Text);
                    FileLogger.Log("Company Profile", "Company Profile", Program.CompanyID);
                    Companyprofileresult = "-1";
                    return -1;

                }

                UserInfoVM.IsMainSetting = ReportResult.Tables[4].Rows[0]["IsMainSettings"] == null ||
                                           ReportResult.Tables[4].Rows[0]["IsMainSettings"].ToString() == "Y";

                if (ReportResult.Tables[4].Rows[0]["IsMainSettings"] == null || ReportResult.Tables[4].Rows[0]["IsMainSettings"].ToString() == "Y")
                {
                    settingVM.SettingsDT = ReportResult.Tables[2].Copy();
                    settingVM.SettingsDTUser = ReportResult.Tables[2].Copy();
                }
                else
                {
                    settingVM.SettingsDT = ReportResult.Tables[2];
                    settingVM.SettingsDTUser = ReportResult.Tables[3];
                }


                //settingVM.SettingsDT = ReportResult.Tables[2];
                //settingVM.SettingsDTUser = ReportResult.Tables[3];
                settingVM.UserInfoDT = ReportResult.Tables[4];

                //done
                foreach (DataRow item2 in ReportResult.Tables[0].Rows)
                {
                    Program.CurrentUser = txtUserName.Text.Trim();
                    Program.CompanyID = item2["CompanyID"].ToString(); // ReportFields[1].ToString();
                    //Program.CompanyID =Converter.DESDecrypt(PassPhrase,EnKey,item2["CompanyID"].ToString()); // ReportFields[1].ToString();

                    Program.CompanyName = item2["CompanyLegalName"].ToString(); // ReportFields[3].ToString(); //CompanyName
                    Program.CompanyNameLog = item2["CompanyName"].ToString(); // ReportFields[2].ToString();
                    Program.CompanyLegalName = item2["CompanyLegalName"].ToString(); // ReportFields[3].ToString();
                    Program.Address1 = item2["Address1"].ToString(); // ReportFields[4].ToString();
                    Program.VatRegistrationNo = item2["VatRegistrationNo"].ToString(); // ReportFields[17].ToString();
                    if (Program.CompanyNameLog == "BVCPSCTG")
                    {
                        if (Convert.ToDateTime(BureauInfoVM.SessionDate) < Convert.ToDateTime("01-Dec-2017"))
                        {
                            Program.Address1 = "Central Store Area, Karnaphuli EPZ, North Patenga";
                            Program.VatRegistrationNo = "24021011433"; // ReportFields[4].ToString();
                        }
                    }

                    Program.Address2 = item2["Address2"].ToString(); // ReportFields[5].ToString();
                    Program.Address3 = item2["Address3"].ToString(); // ReportFields[6].ToString();
                    Program.City = item2["City"].ToString(); // ReportFields[7].ToString();
                    Program.ZipCode = item2["ZipCode"].ToString(); // ReportFields[8].ToString();
                    Program.TelephoneNo = item2["TelephoneNo"].ToString(); // ReportFields[9].ToString();
                    Program.FaxNo = item2["FaxNo"].ToString(); // ReportFields[10].ToString();
                    Program.Email = item2["Email"].ToString(); // ReportFields[11].ToString();
                    Program.ContactPerson = item2["ContactPerson"].ToString(); // ReportFields[12].ToString();
                    Program.ContactPersonDesignation = item2["ContactPersonDesignation"].ToString(); // ReportFields[13].ToString();
                    Program.ContactPersonTelephone = item2["ContactPersonTelephone"].ToString(); // ReportFields[14].ToString();
                    Program.ContactPersonEmail = item2["ContactPersonEmail"].ToString(); // ReportFields[15].ToString();
                    Program.TINNo = item2["TINNo"].ToString(); // ReportFields[16].ToString();
                    Program.Comments = item2["Comments"].ToString(); // ReportFields[18].ToString();
                    Program.ActiveStatus = item2["ActiveStatus"].ToString(); // ReportFields[19].ToString();
                    Program.FMonthStart = Convert.ToDateTime(item2["FYearStart"].ToString()); // Convert.ToDateTime(ReportFields[20]);
                    Program.FMonthEnd = Convert.ToDateTime(item2["FYearEnd"].ToString()); // Convert.ToDateTime(ReportFields[21]);
                    Program.LicenseKey = item2["License"].ToString(); // Convert.ToDateTime(ReportFields[21]);
                    Program.Section = item2["Section"].ToString();
                    SymphonyDecryption _symDesc = new SymphonyDecryption();

                    string PlaneCode = _symDesc.Decrypt(Program.LicenseKey);
                    if (PlaneCode.ToLower() != "na")
                    {
                        string[] DataA = PlaneCode.Trim().Split('~');
                        foreach (string data in DataA)
                        {
                            string[] DataB = data.Trim().Split(':');
                            string Name = DataB[0].ToString();
                            string Value = DataB[1].ToString();

                            if (Name == "Manufacturing") Program.IsManufacturing = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "Trading") Program.IsTrading = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "Service") Program.IsService = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "Tender") Program.IsTender = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "TDS") Program.IsTDS = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "Bandroll") Program.IsBandroll = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "TollClient") Program.IsTollClient = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "TollContractor") Program.IsTollContractor = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "IntegrationExcel") Program.IsIntegrationExcel = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "IntegrationOthers") Program.IsIntegrationOthers = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "IntegrationAPI") Program.IsIntegrationAPI = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "CentralBIN") Program.IsCentralBIN = Convert.ToBoolean(Value == "Y" ? true : false);
                            if (Name == "Depo") Program.Depos = Convert.ToInt32(Value);
                        }
                    }
                    else
                    {

                    }



                }
                //Program.IssueFromBOM = ReportResult.Tables[1].Rows[0]["IssueFromBOM"].ToString();
                //Program.PrepaidVAT = ReportResult.Tables[1].Rows[0]["PrepaidVAT"].ToString();
                Companyprofileresult = "1";

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
                FileLogger.Log(this.Name, "Companyprofile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Companyprofile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Companyprofile", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Companyprofile", exMessage);
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
                FileLogger.Log(this.Name, "Companyprofile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Companyprofile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Companyprofile", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Companyprofile", exMessage);
            }
            #endregion
            finally
            {
                a = Companyprofileresult;
            }
            return Convert.ToInt16(a);

        }

        private void bgwUserHas_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Statement

            CommonDAL commonDal = new CommonDAL();


            // Start DoWork
            UserResult = new DataTable();
            string userName = OrdinaryVATDesktop.SanitizeInput(txtUserName.Text.Trim());
            string password = txtUserPassword.Text.Trim().Replace("'", "");
            UserInformationDAL userInformationDal = new UserInformationDAL();
            UserResult = userInformationDal.SearchUserHas(userName, null, password);
            //done
            // End DoWork

            #endregion
        }

        private void bgwUserHas_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                UserInformationDAL userInformationDal = new UserInformationDAL();
                CommonDAL commonDal = new CommonDAL();
                commonDal.AddUserInfo();

                string userName = OrdinaryVATDesktop.SanitizeInput(txtUserName.Text.Trim());

                if (e.Error != null)
                    throw e.Error;

                if(UserResult.Rows.Count == 0)
                {
                   throw new Exception("User Name / Password is invalid!");
                }
                

                Program.CurrentUserID = string.Empty;
                txtUserNameDB.Text = UserResult.Rows[0]["UserName"].ToString();
                txtPasswordDB.Text = Converter.DESDecrypt(PassPhrase, EnKey, UserResult.Rows[0]["UserPassword"].ToString());
                //string tt = Converter.DESEncrypt(PassPhrase, EnKey, "admin");
                Program.CurrentUserID = UserResult.Rows[0]["UserID"].ToString();
                //var pp = Converter.DESEncrypt(PassPhrase, EnKey, txtUserPassword.Text);
                DateTime LastPasswordChangeDate = Convert.ToDateTime(UserResult.Rows[0]["LastPasswordChangeDate"].ToString());
               
                if (Convert.ToBoolean(UserResult.Rows[0]["IsLock"].ToString()) == true)
                {
                    MessageBox.Show("User temporarily locked. Please contact your administrator");
                    return;
                }

                if (UserResult.Rows[0]["ActiveStatus"].ToString() == "Y")
                {
                    chkActiveStatusDB.Checked = true;
                }
                else
                {
                    chkActiveStatusDB.Checked = false;
                }

                commonDal.BranchCheck();

                if (txtUserName.Text.Trim() != txtUserNameDB.Text.Trim())
                {
                    if (UserResult.Rows.Count > 0)
                    {
                        userInformationDal.WorngAttemptProcess(true, userName);
                    }
                    else
                    {
                        userInformationDal.WorngAttemptProcess(false, userName);
                    }
                    MessageBox.Show("User not exist");
                    txtUserName.Text = "";
                    txtUserPassword.Text = "";
                    txtUserName.Focus();
                    return;
                }

                else if (chkActiveStatusDB.Checked == false)
                {
                   
                    MessageBox.Show("User not active");
                    txtUserName.Text = "";
                    txtUserPassword.Text = "";
                    txtUserName.Focus();
                    return;
                }

                else if (txtPasswordDB.Text.Trim() != txtUserPassword.Text.Trim())
                {
                    if (UserResult.Rows.Count > 0)
                    {
                        userInformationDal.WorngAttemptProcess(true, userName);
                    }
                    else
                    {
                        userInformationDal.WorngAttemptProcess(false, userName);
                    }

                    MessageBox.Show("Password not match");
                    txtUserName.Text = "";
                    txtUserPassword.Text = "";
                    txtUserName.Focus();
                    return;
                }
                else if (Companyprofile() != 1)
                {
                    MessageBox.Show("Company not match or company not exists");
                    cmbCompany.Focus();
                    return;
                }
                else
                {

                    SaveUserName();

                    SaveInReg(LASTCOMPANY, cmbCompany.Text);



                    if (string.IsNullOrEmpty(Program.CurrentUserID))
                    {
                        this.Close();
                    }

                    //Program.CurrentUserRollSearch(Program.CurrentUserID);

                    bool isSecured = CheckSecurity();
                    if (!isSecured)
                    {
                        Program.IsTrial = true;
                    }
                    else
                    {
                        Program.IsTrial = false;
                    }

                    Program.MdiForm.MenuMaker();

                    if (Program.IsTrial == true)
                    {
                        Program.Trial = Program.TrialComments;
                    }
                    else if (Program.IsAlpha == true)
                    {
                        Program.Trial = Program.AlphaComments;
                    }
                    else if (Program.IsBeta == true)
                    {
                        Program.Trial = Program.BetaComments;
                    }
                    else
                    {
                        Program.Trial = "";
                    }

                    CheckAppVersion();

                    UserInfoVM.UserName = Program.CurrentUser;
                    UserInfoVM.UserId = Program.CurrentUserID;
                    UserInfoVM.Password = txtUserPassword.Text;
                    string VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3");

                    //int ChangeDate = Convert.ToInt32(commonDal.settingsDesktop("Password", "ChangeDate") ?? "120");
                    int ChangeDate = 120;
                    DateTime NewDate = LastPasswordChangeDate.AddDays(ChangeDate);

                    DateTime CurrentDateTime = DateTime.Now;
                    if (CurrentDateTime > NewDate)
                    {
                        MessageBox.Show("Please Reset your Password ");
                    }
                    Program.IsBureau = false;
                    if (VAT11Name.ToLower() == "bvcps")
                    {
                        Program.IsBureau = true;
                    }

                    userInformationDal.UpdateUserWromgAttempt(userName);

                    BureauInfoVM.IsBureau = Program.IsBureau;
                    int transResult = 0;

                    #region UserMenuAllFinalRolls
                    //                    if (commonDal.NewTableExistCheck("UserMenuAllFinalRolls", null, null) == 0)
//                    {
//                        #region UserMenuAllFinalRolls
//                        string UserMenuAllFinalRolls = @"
//CREATE TABLE [dbo].[UserMenuAllFinalRolls](
//	[LineID] [int] IDENTITY(1,1) NOT NULL,
//	[FormId] [nvarchar](255) NOT NULL,
//	[FormName] [nvarchar](255) NULL,
//	[RibbonName] [nvarchar](255) NULL,
//	[Access] [varchar](1) NULL,
//	[AccessRoll] [nvarchar](255) NULL,
//	[AccessType] [nvarchar](255) NULL,
//	[Len] [float] NULL,
//	[TabName] [nvarchar](255) NULL,
//	[PanelName] [nvarchar](255) NULL,
//	[ButtonName] [nvarchar](255) NULL,
//	[LastModifiedBy] [varchar](120) NULL,
//	[LastModifiedOn] [datetime] NULL,
// CONSTRAINT [PK_UserMenuAllFinalRollTemps] PRIMARY KEY CLUSTERED 
//(
//	[LineID] ASC
//)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
//) ON [PRIMARY]
//
//
//SET IDENTITY_INSERT [dbo].[UserMenuAllFinalRolls] ON 
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (1, N'110', N'Setup', N'rTabSetup', N'1', N'o8bQC0by0vs=', N'Tab', 3, N'rTabSetup', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (2, N'110110', N'Setup/ItemInformation', N'rpItemInformation', N'1', N'uxKoIJMe/MCafr/f1Ml2mQ==', N'Panel', 6, N'rTabSetup', N'rpItemInformation', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (3, N'110110110', N'Setup/ItemInformation/Group', N'ribbonButton10', N'1', N'Pq75PBbVpfM1NgGVt3WerQ==', N'Button', 9, N'rTabSetup', N'rpItemInformation', N'ribbonButton10', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (4, N'110110120', N'Setup/ItemInformation/Product', N'ribbonButton81', N'1', N'8l6la5IH2L/c4/h9n9arYw==', N'Button', 9, N'rTabSetup', N'rpItemInformation', N'ribbonButton81', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (5, N'110110130', N'Setup/ItemInformation/Overhead', N'ribbonButton93', N'1', N'eO1tDdj79rQuemidi6PdVg==', N'Button', 9, N'rTabSetup', N'rpItemInformation', N'ribbonButton93', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (6, N'110110140', N'Setup/ItemInformation/TDS', N'rbtnTDS1', N'0', N'+3NaEDhvrQdeM90rLW4lcw==', N'Button', 9, N'rTabSetup', N'rpItemInformation', N'rbtnTDS1', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (7, N'110110150', N'Setup/ItemInformation/HSCode', N'rbtnHSCode', N'1', N'KpnrUCGugmMnIWPfgzt2IA==', N'Button', 9, N'rTabSetup', N'rpItemInformation', N'rbtnHSCode', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (8, N'110120', N'Setup/Vedor', N'rpVendor', N'1', N'CVHdra/bVswdwxkFw/li0w==', N'Panel', 6, N'rTabSetup', N'rpVendor', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (9, N'110120110', N'Setup/Vedor/Group', N'ribbonButton11', N'1', N'fLT6daM06K/s01pvBCKqNQ==', N'Button', 9, N'rTabSetup', N'rpVendor', N'ribbonButton11', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (10, N'110120120', N'Setup/Vedor/Vendor', N'ribbonButton12', N'1', N'nQtAx9guxNSS/R+Hy+h3mw==', N'Button', 9, N'rTabSetup', N'rpVendor', N'ribbonButton12', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (11, N'110130', N'Setup/Customer', N'rpCustomer', N'1', N'59KPcZnxguJZjFuaggebZQ==', N'Panel', 6, N'rTabSetup', N'rpCustomer', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (12, N'110130110', N'Setup/Customer/Group', N'ribbonButton13', N'1', N'UErYaX744hCwBuXditZA4g==', N'Button', 9, N'rTabSetup', N'rpCustomer', N'ribbonButton13', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (13, N'110130120', N'Setup/Customer/Customer', N'ribbonButton80', N'1', N'BRUOdumXveI9Vv8Kb3aMqg==', N'Button', 9, N'rTabSetup', N'rpCustomer', N'ribbonButton80', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (14, N'110140', N'Setup/BankVehicle', N'rpBankVehicle', N'1', N'XpLm5quTHooCvmSQBjIANg==', N'Panel', 6, N'rTabSetup', N'rpBankVehicle', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (15, N'110140110', N'Setup/BankVehicle/Bank', N'ribbonButton14', N'1', N'5PIz6jcia9o3XezAMTjtSA==', N'Button', 9, N'rTabSetup', N'rpBankVehicle', N'ribbonButton14', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (16, N'110140120', N'Setup/BankVehicle/Vehicle', N'rbtnVehicle', N'1', N'D5djpihaBP51me6H+V8Nbg==', N'Button', 9, N'rTabSetup', N'rpBankVehicle', N'rbtnVehicle', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (17, N'110150', N'Setup/PriceDeclaration', N'rpPriceDeclaration', N'1', N'A6d4YaouECUlsH05Vdncbg==', N'Panel', 6, N'rTabSetup', N'rpPriceDeclaration', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (18, N'110150110', N'Setup/PriceDeclaration/BOM', N'ribbonButton17', N'1', N'4n8ALrqOFwQhVEaJ/Kxf5Q==', N'Button', 9, N'rTabSetup', N'rpPriceDeclaration', N'ribbonButton17', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (19, N'110150120', N'Setup/PriceDeclaration/Service', N'ribbonButton96', N'1', N'nd8tx3oGgyRshfn2wFsYeg==', N'Button', 9, N'rTabSetup', N'rpPriceDeclaration', N'ribbonButton96', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (20, N'110150130', N'Setup/PriceDeclaration/Tender', N'ribbonButton89', N'1', N'4PXk75GDvVBJ54e3MtUoyA==', N'Button', 9, N'rTabSetup', N'rpPriceDeclaration', N'ribbonButton89', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (21, N'110160', N'Setup/Company', N'rpCompany', N'1', N'hvL1yIIuen+QVF0QfbCDiQ==', N'Panel', 6, N'rTabSetup', N'rpCompany', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (22, N'110160110', N'Setup/Company/CommpanyProfile', N'ribbonButton18', N'1', N'j1GkdAIkOBcpMilTuniA8g==', N'Button', 9, N'rTabSetup', N'rpCompany', N'ribbonButton18', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (23, N'110160120', N'Setup/Company/BranchProfile', N'rbtnBranchTransfer', N'1', N'mK8RMts2up+q+KdGruyolQ==', N'Button', 9, N'rTabSetup', N'rpCompany', N'rbtnBranchTransfer', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (24, N'110170', N'Setup/FiscalYear', N'rpFiscalYear', N'1', N'tjlr3aNtCAv5f2Bvpuyswg==', N'Panel', 6, N'rTabSetup', N'rpFiscalYear', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (25, N'110170110', N'Setup/FiscalYear/FiscalYear', N'ribbonButton75', N'1', N'IrhOuTwPDyM0X1Sxv4xWpw==', N'Button', 9, N'rTabSetup', N'rpFiscalYear', N'ribbonButton75', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (26, N'110180', N'Setup/Configuration', N'rpConfiguration', N'1', N'HcwjdUOQ6SuY1KPew9sKGg==', N'Panel', 6, N'rTabSetup', N'rpConfiguration', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (27, N'110180110', N'Setup/Configuration/Settings', N'rbtnSettings', N'1', N'EC+4cor6Iys0DEOAngXbYg==', N'Button', 9, N'rTabSetup', N'rpConfiguration', N'rbtnSettings', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (28, N'110180120', N'Setup/Configuration/Prefix', N'ribbonButton47', N'1', N'RvyoBUo+igA1nPrFG5HK5w==', N'Button', 9, N'rTabSetup', N'rpConfiguration', N'ribbonButton47', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (29, N'110180130', N'Setup/Configuration/Shift', N'rbtnShift', N'1', N'hHu0S8m+9n85Zvb2IYTHCw==', N'Button', 9, N'rTabSetup', N'rpConfiguration', N'rbtnShift', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (30, N'110190', N'Setup/ImportSync', N'rpImportSync', N'1', N'S6DCf3+ASffKd1wc0KzHcw==', N'Panel', 6, N'rTabSetup', N'rpImportSync', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (31, N'110190110', N'Setup/ImportSync/Import', N'ribbonButton46', N'1', N'G8/bO9JI360p6d4NMUu8lA==', N'Button', 9, N'rTabSetup', N'rpImportSync', N'ribbonButton46', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (32, N'110190120', N'Setup/ImportSync/Sync', N'ribbonButton35', N'1', N'7KtQzDU3R+AotV7eIrwszw==', N'Button', 9, N'rTabSetup', N'rpImportSync', N'ribbonButton35', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (33, N'110190130', N'Setup/ImportSync/Update/Delete query', N'rbtnExecute', N'1', N'KzHfm/ontlWKWLdBfCEnxw==', N'Button', 9, N'rTabSetup', N'rpImportSync', N'rbtnExecute', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (34, N'110200', N'Setup/Measurment', N'rpMeasurement', N'1', N'YGo+zvqkJT9EJbfwFWdW0Q==', N'Panel', 6, N'rTabSetup', N'rpMeasurement', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (35, N'110200110', N'Setup/Measurment/Name', N'ribbonButton16', N'1', N'Dww2usM9AdjF6jH97ejlxA==', N'Button', 9, N'rTabSetup', N'rpMeasurement', N'ribbonButton16', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (36, N'110200120', N'Setup/Measurment/Conversion', N'ribbonButton65', N'1', N'6e6IZ30lBmi5wjgw3G3DnQ==', N'Button', 9, N'rTabSetup', N'rpMeasurement', N'ribbonButton65', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (37, N'110210', N'Setup/Currency', N'rpCurrency', N'1', N'vUQq4ZgZUe9/pqxRAXlS4Q==', N'Panel', 6, N'rTabSetup', N'rpCurrency', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (38, N'110210110', N'Setup/Currency/Currency', N'ribbonButton98', N'1', N'R95QeG0s34MPX79lVdV4WA==', N'Button', 9, N'rTabSetup', N'rpCurrency', N'ribbonButton98', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (39, N'110210120', N'Setup/Currency/Conversion', N'ribbonButton120', N'1', N'srXR7VgEtiJiDIw5h4zujg==', N'Button', 9, N'rTabSetup', N'rpCurrency', N'ribbonButton120', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (40, N'110220', N'Setup/Banderol', N'rpBanderol', N'0', N'aYoDS3TkmbaDXlIFmzsTSw==', N'Panel', 6, N'rTabSetup', N'rpBanderol', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (41, N'110220110', N'Setup/Banderol/Banderol', N'ribBtnBanderol', N'0', N'HZc75p7V6LmTvXAhZlDkcQ==', N'Button', 9, N'rTabSetup', N'rpBanderol', N'ribBtnBanderol', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (42, N'110220120', N'Setup/Banderol/Packaging', N'ribBtnPackage', N'0', N'dJWhpN4HcqEbAhqn/A60aQ==', N'Button', 9, N'rTabSetup', N'rpBanderol', N'ribBtnPackage', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (43, N'110220130', N'Setup/Banderol/Product', N'rbnBtnProduct', N'0', N'uGDMthxVoRTSOj2q7exDnw==', N'Button', 9, N'rTabSetup', N'rpBanderol', N'rbnBtnProduct', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (44, N'120', N'Purchase', N'rTabPurchase', N'1', N'IFiVJVdgVHk=', N'Tab', 3, N'rTabPurchase', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (45, N'120110', N'Purchase/Purchase', N'rpPurchase', N'1', N'KZ4NQRlP+QFJi2n2ZTj9gw==', N'Panel', 6, N'rTabPurchase', N'rpPurchase', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (46, N'120110110', N'Purchase/Purchase/Local', N'ribbonButton68', N'1', N'p+Wc/fnvfs6WlGmJG2lVQQ==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'ribbonButton68', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (47, N'120110120', N'Purchase/Purchase/Import', N'rbtnImport', N'1', N'r9TFW2m8RwyKOSj+UqGiEQ==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'rbtnImport', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (48, N'120110130', N'Purchase/Purchase/InputService', N'rbtnInputService', N'1', N'5HlT7bXxf9Anhbd6v/HKwA==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'rbtnInputService', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (49, N'120110140', N'Purchase/Purchase/PurchaseReturn', N'ribbonButton50', N'1', N'dl/RuTL+qT+0/4uR6X3F3A==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'ribbonButton50', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (50, N'120110150', N'Purchase/Purchase/Service Stock', N'ribbonButton53', N'1', N'prmxTLNOnwwdksV7T6O/Tw==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'ribbonButton53', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (51, N'120110160', N'Purchase/Purchase/Service Non Stock', N'ribbonButton64', N'1', N'5sf2my4bmnh0V9TAfCiC7w==', N'Button', 9, N'rTabPurchase', N'rpPurchase', N'ribbonButton64', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (52, N'130', N'Production', N'rTabProduction', N'1', N'SKyJOMPJNTM=', N'Tab', 3, N'rTabProduction', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (53, N'130110', N'Production/Issue', N'rpIssue', N'1', N'4cqpjylke1OSlpmpBkxNgA==', N'Panel', 6, N'rTabProduction', N'rpIssue', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (54, N'130110110', N'Production/Issue/Issue', N'ribbonButton130', N'1', N'oPuX7ZR7A/9eRuThFbcNXw==', N'Button', 9, N'rTabProduction', N'rpIssue', N'ribbonButton130', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (55, N'130110120', N'Production/Issue/Return', N'ribbonButton131', N'1', N'Wdy3QaY4mWsxn4q4xs+FXg==', N'Button', 9, N'rTabProduction', N'rpIssue', N'ribbonButton131', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (56, N'130110130', N'Production/Issue/Issue WithOut BOM', N'rbtnIssueWOBom', N'1', N'VHufu575mjYcZ7QxpMz6NQ==', N'Button', 9, N'rTabProduction', N'rpIssue', N'rbtnIssueWOBom', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (57, N'130110140', N'Production/Issue/Wastage', N'rbtnWastageIssue', N'1', N'KoL5YDECaJmUGQ0rrBC8rQ==', N'Button', 9, N'rTabProduction', N'rpIssue', N'rbtnWastageIssue', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (58, N'130110150', N'Production/Issue/Transfer', N'ribBtnTransfer', N'1', N'XByXiDSM6KlSRIO4mJllSA==', N'Button', 9, N'rTabProduction', N'rpIssue', N'ribBtnTransfer', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (59, N'130120', N'Production/Receive', N'rpReceive', N'1', N'BVGUQDSBTUfZrZeH5s2Nyg==', N'Panel', 6, N'rTabProduction', N'rpReceive', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (60, N'130120110', N'Production/Receive/WIP', N'ribbonButton133', N'1', N'+XGjiZrVX65AacnK/gc1jQ==', N'Button', 9, N'rTabProduction', N'rpReceive', N'ribbonButton133', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (61, N'130120120', N'Production/Receive/FGReceive', N'ribbonButton135', N'1', N'818iP6dlw39EyIm6wbNEPQ==', N'Button', 9, N'rTabProduction', N'rpReceive', N'ribbonButton135', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (62, N'130120130', N'Production/Receive/Return', N'ribbonButton132', N'1', N'rAbp5cUoQM5mqHxQfbNpSQ==', N'Button', 9, N'rTabProduction', N'rpReceive', N'ribbonButton132', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (63, N'130120140', N'Production/Receive/Package', N'ribbonBtnpackage', N'1', N'OSWshlKUEA3rNgzlXVPZTw==', N'Button', 9, N'rTabProduction', N'rpReceive', N'ribbonBtnpackage', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (64, N'140', N'Sale', N'rTabSale', N'1', N'+gLobR/Ob7w=', N'Tab', 3, N'rTabSale', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (65, N'140110', N'Sale/Sale', N'rpSale', N'1', N'5I1Cq1qSfo2N8xgDvInPHw==', N'Panel', 6, N'rTabSale', N'rpSale', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (66, N'140110110', N'Sale/Sale/Local', N'ribbonButton134', N'1', N'ewwYaOW7Svq2LF7K8iqgDQ==', N'Button', 9, N'rTabSale', N'rpSale', N'ribbonButton134', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (67, N'140110120', N'Sale/Sale/Export', N'ribbonButton138', N'1', N'9gV4VHKtVm246yC+8xyeLw==', N'Button', 9, N'rTabSale', N'rpSale', N'ribbonButton138', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (68, N'140110130', N'Sale/Sale/Tender', N'ribbonButton139', N'1', N'uRW48PGjrM8XvSsoMaB2Ag==', N'Button', 9, N'rTabSale', N'rpSale', N'ribbonButton139', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (69, N'140110140', N'Sale/Sale/Trading', N'rbnTenderTrading', N'1', N'KEd95grrvH+RGPIUvJeX9w==', N'Button', 9, N'rTabSale', N'rpSale', N'rbnTenderTrading', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (70, N'140110150', N'Sale/Sale/Service Stock', N'ribbonButton136', N'1', N'rsskOQkadeHvDlM1M2Tz9g==', N'Button', 9, N'rTabSale', N'rpSale', N'ribbonButton136', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (71, N'140110160', N'Sale/Sale/Service Non Stock', N'ribbonButton19', N'1', N'sh8L5H+zjEymaBqXN7c5yw==', N'Button', 9, N'rTabSale', N'rpSale', N'ribbonButton19', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (72, N'140110170', N'Sale/Sale/RawSale', N'rbtnRawSale', N'1', N'L5DG2UM6ZqlSZ75D9aTMfg==', N'Button', 9, N'rTabSale', N'rpSale', N'rbtnRawSale', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (73, N'140110180', N'Sale/Sale/Wastage', N'rbtnWastageSale', N'1', N'9El+L1Sqqt5MKngfL6SeHQ==', N'Button', 9, N'rTabSale', N'rpSale', N'rbtnWastageSale', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (74, N'140120', N'Sale/package', N'rpPackage', N'1', N'WIOPMZIKquRrCuW6mVxlyg==', N'Panel', 6, N'rTabSale', N'rpPackage', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (75, N'140120110', N'Sale/package/package', N'ribbonButton23', N'1', N'50u57WTn6anaRxWQl/a/EA==', N'Button', 9, N'rTabSale', N'rpPackage', N'ribbonButton23', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (76, N'140130', N'Sale/Transfer  IssueRecieve', N'rpTReceive', N'1', N'vbzH9ST6YctkJTyE3PKXOw==', N'Panel', 6, N'rTabSale', N'rpTReceive', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (77, N'140130110', N'Sale/Transfer  IssueRecieve/RM In', N'rbtn61In', N'1', N'Ra/xxJFyZEJKuMs2hr+qtg==', N'Button', 9, N'rTabSale', N'rpTReceive', N'rbtn61In', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (78, N'140130120', N'Sale/Transfer  IssueRecieve/FG In', N'rbtn62In', N'1', N'4w+i1oZjFbLL/2rLWhQrog==', N'Button', 9, N'rTabSale', N'rpTReceive', N'rbtn62In', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (79, N'140130130', N'Sale/Transfer IssueRecieve/RM  Out', N'rbtn61Out1', N'1', N'4WEbudSocKnpXfpAUNrpBg==', N'Button', 9, N'rTabSale', N'rpTReceive', N'rbtn61Out1', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (80, N'140130140', N'Sale/Transfer IssueRecieve/FG Out', N'rbtn62Out1', N'1', N'z4883CUeI3tZNmBKmzKypw==', N'Button', 9, N'rTabSale', N'rpTReceive', N'rbtn62Out1', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (81, N'140140', N'Sale/EXP', N'rpExp', N'1', N'TzHxQtqfYnallkSRx8TyBA==', N'Panel', 6, N'rTabSale', N'rpExp', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (82, N'140140110', N'Sale/EXP/EXP', N'ribbonButton15', N'1', N'WERtXl3SnwuhgI6AVRkxcg==', N'Button', 9, N'rTabSale', N'rpExp', N'ribbonButton15', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (83, N'150', N'Deposit', N'rTabDeposit', N'1', N'AYXvLiKmfRY=', N'Tab', 3, N'rTabDeposit', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (84, N'150110', N'Deposit/Treasury', N'rpDeposit', N'1', N'xV6CFU5FbPlWgMfBbX5x0Q==', N'Panel', 6, N'rTabDeposit', N'rpDeposit', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (85, N'150110110', N'Deposit/Treasury/Treasury', N'ribbonButton140', N'1', N'xQWxdMxy7WNiFoJxdzSX3A==', N'Button', 9, N'rTabDeposit', N'rpDeposit', N'ribbonButton140', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (86, N'150120', N'Deposit/VDS', N'rpVDS', N'1', N'msgxArPF38UWE+xhP1jJMA==', N'Panel', 6, N'rTabDeposit', N'rpVDS', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (87, N'150120110', N'Deposit/VDS/Purchage', N'ribbonButton141', N'1', N'uDoWnjKRXSbNGHNvVnq6dw==', N'Button', 9, N'rTabDeposit', N'rpVDS', N'ribbonButton141', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (88, N'150120120', N'Deposit/VDS/Sale', N'rbtnSaleVDS', N'1', N'n55XIflLe0xv1oe+NjXlcA==', N'Button', 9, N'rTabDeposit', N'rpVDS', N'rbtnSaleVDS', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (89, N'150130', N'Deposit/SD', N'rpSD', N'1', N'pZSSGL6mOz8NJO5c9Nyt8g==', N'Panel', 6, N'rTabDeposit', N'rpSD', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (90, N'150130110', N'Deposit/SD/SD', N'ribbonButton143', N'1', N'F4r5z5z0ZzNvk85E6SgGoQ==', N'Button', 9, N'rTabDeposit', N'rpSD', N'ribbonButton143', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (91, N'150140', N'Deposit/CashPayble', N'rpCashPayble', N'1', N'N5y5wnPaoTgrySl1wNJy5Q==', N'Panel', 6, N'rTabDeposit', N'rpCashPayble', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (92, N'150140110', N'Deposit/CashPayble/CashPayble', N'rbtnCashPayble', N'1', N'TV1nzjMefVWWG5KylBj8XQ==', N'Button', 9, N'rTabDeposit', N'rpCashPayble', N'rbtnCashPayble', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (93, N'150150', N'Deposit/Reverse', N'rpReverse', N'1', N'N8e72DKXMDzXf1t57/uGtg==', N'Panel', 6, N'rTabDeposit', N'rpReverse', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (94, N'150150110', N'Deposit/Reverse/Reverse', N'ribBtnReverse', N'1', N'wAPpRusUbb+putnO8KSuHA==', N'Button', 9, N'rTabDeposit', N'rpReverse', N'ribBtnReverse', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (95, N'150160', N'Deposit/TDS', N'rpTDS', N'0', N'ajROjgiTky7Nl6twTQlQLg==', N'Panel', 6, N'rTabDeposit', N'rpTDS', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (96, N'150160110', N'Deposit/TDS/TDS', N'rbtnTDS', N'0', N'cAwWLZ/t7kHzys7PbLnZ6w==', N'Button', 9, N'rTabDeposit', N'rpTDS', N'rbtnTDS', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (97, N'160', N'Toll', N'rTabToll', N'1', N'WSMUMkokCeI=', N'Tab', 3, N'rTabToll', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (98, N'160110', N'Toll/Client', N'rpClient', N'1', N'oLOUf5DmbdTuwYha0hAcKA==', N'Panel', 6, N'rTabToll', N'rpClient', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (99, N'160110110', N'Toll/Client/Issue 6.4', N'ribbonButton94', N'1', N'vvBvMMzh0R4Dv839wPYc9w==', N'Button', 9, N'rTabToll', N'rpClient', N'ribbonButton94', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (100, N'160110120', N'Toll/Client/FGReceive', N'ribbonButton95', N'1', N'2Bk1n8/GnxGVWmehjM0bRQ==', N'Button', 9, N'rTabToll', N'rpClient', N'ribbonButton95', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (101, N'160110130', N'Toll/Client/VAT11GAGA', N'ribbonButton145', N'1', N'qdINTSEWt6WXBI5nPV8/4g==', N'Button', 9, N'rTabToll', N'rpClient', N'ribbonButton145', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (102, N'160120', N'Toll/Contractor', N'rpContractor', N'1', N'RT+Qq4J6NspHoBUPUUDgXw==', N'Panel', 6, N'rTabToll', N'rpContractor', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (103, N'160120110', N'Toll/Contractor/RawReceive', N'ribbonButton116', N'1', N'15V+l0gVIWVMDaVzBzKZ7A==', N'Button', 9, N'rTabToll', N'rpContractor', N'ribbonButton116', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (104, N'160120120', N'Toll/Contractor/FGProduction', N'ribbonButton117', N'1', N'Uy6ZN4H6WjTeP8CVWLwZIA==', N'Button', 9, N'rTabToll', N'rpContractor', N'ribbonButton117', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (105, N'160120130', N'Toll/Contractor/FGIssue', N'ribbonButton118', N'1', N'gD4RlkJifS2WtWkRyEvIwA==', N'Button', 9, N'rTabToll', N'rpContractor', N'ribbonButton118', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (106, N'160120140', N'Toll/Contractor/Toll 6.3', N'rbtnToll6_3', N'1', N'1HUJ6kZD7ztyiTsTHVApbg==', N'Button', 9, N'rTabToll', N'rpContractor', N'rbtnToll6_3', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (107, N'160130', N'Toll/Toll Register', N'rpTollRegister', N'1', N'w6Waj7QBB5E4HwsJhKQn6A==', N'Panel', 6, N'rTabToll', N'rpTollRegister', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (108, N'160130110', N'Toll/Toll Register/Toll 6.1', N'rbtnToll6_1', N'1', N'u7Dgq9GiiygFMnSGiJkg8Q==', N'Button', 9, N'rTabToll', N'rpTollRegister', N'rbtnToll6_1', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (109, N'160130120', N'Toll/Toll Register/Toll 6.2', N'rbtnToll6_2', N'1', N'cCCC8uDnYwBICv7gNXFnYg==', N'Button', 9, N'rTabToll', N'rpTollRegister', N'rbtnToll6_2', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (110, N'170', N'Adjustment', N'rTabAdjustment', N'1', N'ZpelHmGbKLo=', N'Tab', 3, N'rTabAdjustment', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (111, N'170110', N'Adjustment/AdjustmentHead', N'rpOtherAdjustment', N'1', N'5f7OqmGghwlFA5gzmKOg+w==', N'Panel', 6, N'rTabAdjustment', N'rpOtherAdjustment', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (112, N'170110110', N'Adjustment/AdjustmentHead/Head', N'ribbonButton78', N'1', N'p3kQPg0OAcrZghDO2alcsg==', N'Button', 9, N'rTabAdjustment', N'rpOtherAdjustment', N'ribbonButton78', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (113, N'170110120', N'Adjustment/AdjustmentHead/Transaction', N'ribbonButton129', N'1', N'gUxx53wzGszVMHrIinlZjQ==', N'Button', 9, N'rTabAdjustment', N'rpOtherAdjustment', N'ribbonButton129', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (114, N'170120', N'Adjustment/Purchase', N'rpnPurchase', N'1', N'VB3NtkhmLu9j8R4wCtq5yQ==', N'Panel', 6, N'rTabAdjustment', N'rpnPurchase', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (115, N'170120110', N'Adjustment/Purchase/DN', N'ribbonButton122', N'1', N'vMzt/LwS47sZvSAk43V44A==', N'Button', 9, N'rTabAdjustment', N'rpnPurchase', N'ribbonButton122', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (116, N'170120120', N'Adjustment/Purchase/CN', N'ribbonButton123', N'1', N'WiQ1GOi2PXtZgxPazvfIwQ==', N'Button', 9, N'rTabAdjustment', N'rpnPurchase', N'ribbonButton123', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (117, N'170130', N'Adjustment/Sale', N'rplSale', N'1', N'tgwiz+jHYcPPvJFVG0IFQw==', N'Panel', 6, N'rTabAdjustment', N'rplSale', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (118, N'170130110', N'Adjustment/Sale/CN', N'ribbonButton124', N'1', N'n9A2hrajFSPBdmKpMd8alA==', N'Button', 9, N'rTabAdjustment', N'rplSale', N'ribbonButton124', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (119, N'170130120', N'Adjustment/Sale/DN', N'ribbonButton125', N'1', N'mA9MUoMS1aQ/V82m2L/bkA==', N'Button', 9, N'rTabAdjustment', N'rplSale', N'ribbonButton125', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (120, N'170140', N'Adjustment/Dispose', N'rpDispose', N'0', N'G/tcVZInBS3sbavZZEhlTw==', N'Panel', 6, N'rTabAdjustment', N'rpDispose', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (121, N'170140110', N'Adjustment/Dispose/26', N'ribbonButton51', N'0', N'8AyX/Ja3/C+Q0rzTkORJZQ==', N'Button', 9, N'rTabAdjustment', N'rpDispose', N'ribbonButton51', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (122, N'170140120', N'Adjustment/Dispose/27', N'ribbonButton52', N'0', N'EMoQnZpih6ncw05eW/Sjnw==', N'Button', 9, N'rTabAdjustment', N'rpDispose', N'ribbonButton52', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (123, N'170150', N'Adjustment/DDB', N'rpDrawBack', N'1', N'15jzQQrb4s4ZUupKQxgdiQ==', N'Panel', 6, N'rTabAdjustment', N'rpDrawBack', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (124, N'170150110', N'Adjustment/DDB/DDB', N'ribbonButton40', N'1', N'2G2xcclE1nWt7kRdImx+Lw==', N'Button', 9, N'rTabAdjustment', N'rpDrawBack', N'ribbonButton40', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (125, N'170160', N'Adjustment/VAT & SD Adjutment', N'rpVATAdjustment', N'1', N'77VoOJErHvpwM2nTE23z7w==', N'Panel', 6, N'rTabAdjustment', N'rpVATAdjustment', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (126, N'170160110', N'Adjustment/VAT & SD Adjutment/VAT', N'rbtnVATAdjustment', N'1', N'km9xivlQVyQWVgjk7SSSpg==', N'Button', 9, N'rTabAdjustment', N'rpVATAdjustment', N'rbtnVATAdjustment', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (127, N'170160120', N'Adjustment/VAT & SD Adjutment/SD', N'rbtnSDAdjustment', N'1', N'd+dtT+eHCbc3WoHLgy43Xw==', N'Button', 9, N'rTabAdjustment', N'rpVATAdjustment', N'rbtnSDAdjustment', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (128, N'180', N'NBRReport', N'rTabNBRReport', N'1', N'aXrcr+om/mQ=', N'Tab', 3, N'rTabNBRReport', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (129, N'180110', N'NBRReport/VAT4.3', N'rp43', N'1', N'oGz9wLiEiZP8s7/dKm7HFQ==', N'Panel', 6, N'rTabNBRReport', N'rp43', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (130, N'180110110', N'NBRReport/VAT4.3/VAT4.3', N'ribbonButton49', N'1', N'AY+f62ffFT7PKp1SDQME4g==', N'Button', 9, N'rTabNBRReport', N'rp43', N'ribbonButton49', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (131, N'180120', N'NBRReport/VAT 6.1', N'rp61', N'1', N'd9AhOb8shnaChPEirL5/lA==', N'Panel', 6, N'rTabNBRReport', N'rp61', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (132, N'180120110', N'NBRReport/VAT 6.1/VAT 6.1', N'ribbonButton48', N'1', N'JyqHXL1YVKR9I3LVcHKFag==', N'Button', 9, N'rTabNBRReport', N'rp61', N'ribbonButton48', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (133, N'180130', N'NBRReport/VAT 6.2', N'rp62', N'1', N'e5J41Z+rtshBDjhUgRQyMA==', N'Panel', 6, N'rTabNBRReport', N'rp62', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (134, N'180130110', N'NBRReport/VAT 6.2/VAT 6.2', N'ribbonButton45', N'1', N'iaMYtr4HDjFZT5bLdDLlQg==', N'Button', 9, N'rTabNBRReport', N'rp62', N'ribbonButton45', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (135, N'180140', N'NBRReport/VAT 9.1', N'rp91', N'1', N'3nTN89bt5MnsJNCMiiT9Ew==', N'Panel', 6, N'rTabNBRReport', N'rp91', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (136, N'180140110', N'NBRReport/VAT 9.1/VAT 9.1', N'ribbonButton43', N'1', N'5CgNvEuSC4RE0sM+UpqKvw==', N'Button', 9, N'rTabNBRReport', N'rp91', N'ribbonButton43', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (137, N'180150', N'NBRReport/VAT 6.10', N'rp610', N'1', N'hAEf35dJFWM1ZpUHdZBEEw==', N'Panel', 6, N'rTabNBRReport', N'rp610', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (138, N'180150110', N'NBRReport/VAT 6.10/VAT 6.10', N'rbtn6_10', N'1', N'7qAS8FcpsS0ghxHRQ0GJpA==', N'Button', 9, N'rTabNBRReport', N'rp610', N'rbtn6_10', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (139, N'180160', N'NBRReport/SDReport', N'rpSDReport', N'1', N'8BNn4BB63XfJKh8ki4QdKA==', N'Panel', 6, N'rTabNBRReport', N'rpSDReport', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (140, N'180160110', N'NBRReport/SDReport/SDReport', N'ribbonButton9', N'1', N'v5lS3VAWCLPTiKtABpxNrw==', N'Button', 9, N'rTabNBRReport', N'rpSDReport', N'ribbonButton9', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (141, N'180170', N'NBRReport/VAT 6.3', N'rp63', N'1', N'8+Y+HVY9S5d5c4ClivFoMw==', N'Panel', 6, N'rTabNBRReport', N'rp63', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (142, N'180170110', N'NBRReport/VAT 6.3/VAT 6.3', N'ribBtnVAT11', N'1', N'2V05hH9AObQwF+6XrUELrQ==', N'Button', 9, N'rTabNBRReport', N'rp63', N'ribBtnVAT11', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (143, N'180180', N'NBRReport/VAT 6.5', N'rp65', N'1', N'vRJhKLanexcwymd7/8ZTWg==', N'Panel', 6, N'rTabNBRReport', N'rp65', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (144, N'180180110', N'NBRReport/VAT 6.5/VAT 6.5', N'rbtn65', N'1', N'F4ZxDS9s3hrJx1oST9hDOg==', N'Button', 9, N'rTabNBRReport', N'rp65', N'rbtn65', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (145, N'180190', N'NBRReport/VAT 7', N'rp7', N'0', N'XyfE14bs6Zb4LPbDNs9sEw==', N'Panel', 6, N'rTabNBRReport', N'rp7', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (146, N'180190110', N'NBRReport/VAT 7/VAT 7', N'ribBtnVAT7', N'1', N'fopIvHS/bTvni21SykIS9A==', N'Button', 9, N'rTabNBRReport', N'rp7', N'ribBtnVAT7', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (147, N'180200', N'NBRReport/VAT 20', N'rp20', N'0', N'SQgPclHAqgIZYYIvp2nnpQ==', N'Panel', 6, N'rTabNBRReport', N'rp20', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (148, N'180200110', N'NBRReport/VAT 20/VAT 20', N'ribbonButton26', N'1', N'WOGORDgc7kDePypSJYKOIA==', N'Button', 9, N'rTabNBRReport', N'rp20', N'ribbonButton26', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (149, N'180210', N'NBRReport/Banderol', N'rpBandRoll', N'0', N'1Fi/FdzklXPGh5xVa5+MLw==', N'Panel', 6, N'rTabNBRReport', N'rpBandRoll', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (150, N'180210110', N'NBRReport/Banderol/Form 4', N'ribBtnForm4', N'1', N'7MeFnHgSqhNQkC6wYaV8aw==', N'Button', 9, N'rTabNBRReport', N'rpBandRoll', N'ribBtnForm4', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (151, N'180210120', N'NBRReport/Banderol/Form 5', N'rbtnRecForm5', N'1', N'8G96W1/vZIExL331nlLzoQ==', N'Button', 9, N'rTabNBRReport', N'rpBandRoll', N'rbtnRecForm5', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (152, N'180220', N'NBRReport/Summery-Current Account', N'rpSumCurAcc', N'0', N'kXSyM/Cc3L9341vIkFkhaA==', N'Panel', 6, N'rTabNBRReport', N'rpSumCurAcc', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (153, N'180220110', N'NBRReport/Summery-Current Account/Summery-Current Account', N'ribbonButtonCVAT18', N'1', N'59itlHIJuYGoGojbh9YOKA==', N'Button', 9, N'rTabNBRReport', N'rpSumCurAcc', N'ribbonButtonCVAT18', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (154, N'180220120', N'NBRReport/Summery-Current Account/Breakdwon-Current Account', N'rbtnBrakDown', N'1', N'YBMVlGsjuTfNfVzkf94JMg==', N'Button', 9, N'rTabNBRReport', N'rpSumCurAcc', N'rbtnBrakDown', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (155, N'180230', N'NBRReport/Chak', N'rpKaKha', N'1', N'devnSDmj0blWuKhRjUxhRA==', N'Panel', 6, N'rTabNBRReport', N'rpKaKha', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (156, N'180230110', N'NBRReport/Chak/Chak Ka', N'ribbonButtonChakKa', N'1', N'kkrI11YutrvIPDg2rIcz3w==', N'Button', 9, N'rTabNBRReport', N'rpKaKha', N'ribbonButtonChakKa', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (157, N'180230120', N'NBRReport/Chak/Chak kha', N'ribbonButtonChakKha', N'1', N'Gm5wFuVDVbomB/Cc4+N/FQ==', N'Button', 9, N'rTabNBRReport', N'rpKaKha', N'ribbonButtonChakKha', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (158, N'190', N'MISReport', N'rTabMISReport', N'1', N'fJRLa/oU/0c=', N'Tab', 3, N'rTabMISReport', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (159, N'190110', N'MISReport/Purchase', N'rbpPurchase', N'1', N'I5GOx2Yoa1SUW4FcXqpMQw==', N'Panel', 6, N'rTabMISReport', N'rbpPurchase', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (160, N'190110110', N'MISReport/Purchase/Purchase', N'ribbonButton57', N'1', N'aV8ytmfDXy4VGnX7OXAa9Q==', N'Button', 9, N'rTabMISReport', N'rbpPurchase', N'ribbonButton57', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (161, N'190120', N'MISReport/Production', N'rbpProduction', N'1', N'gi2w0Umb+Rhm3Vds1OGOtw==', N'Panel', 6, N'rTabMISReport', N'rbpProduction', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (162, N'190120110', N'MISReport/Production/Issue', N'ribbonButton56', N'1', N'riefdnszTMdFrC9xAQ2CTQ==', N'Button', 9, N'rTabMISReport', N'rbpProduction', N'ribbonButton56', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (163, N'190120120', N'MISReport/Production/Receive', N'ribbonButton92', N'1', N'7nliQSq0Ee7Ncz1HOkhLpw==', N'Button', 9, N'rTabMISReport', N'rbpProduction', N'ribbonButton92', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (164, N'190130', N'MISReport/Sale', N'rbpSale', N'1', N'1W0qoo4RwUGWYuXr1IaX3Q==', N'Panel', 6, N'rTabMISReport', N'rbpSale', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (165, N'190130110', N'MISReport/Sale/Sale', N'ribbonButton60', N'1', N'FeM1adm4VK6It9ZIJQ37ow==', N'Button', 9, N'rTabMISReport', N'rbpSale', N'ribbonButton60', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (166, N'190140', N'MISReport/Stock', N'rpStock', N'1', N'o5vIbP6ieT7cUOb3+exE9Q==', N'Panel', 6, N'rTabMISReport', N'rpStock', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (167, N'190140110', N'MISReport/Stock/Stock', N'ribbonButton87', N'1', N'5Kvq4AbF8nukOitMAhoNlQ==', N'Button', 9, N'rTabMISReport', N'rpStock', N'ribbonButton87', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (168, N'190140120', N'MISReport/Stock/Receive Sale', N'rbtnReceiveSale', N'1', N'+8B+OkyppvSuhmVLce3H/w==', N'Button', 9, N'rTabMISReport', N'rpStock', N'rbtnReceiveSale', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (169, N'190140130', N'MISReport/Stock/Reconsciliation', N'rbtnReconsciliation', N'1', N'AU0egEvMAnqoFuUe2n9Q0w==', N'Button', 9, N'rTabMISReport', N'rpStock', N'rbtnReconsciliation', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (170, N'190140140', N'MISReport/Stock/Branch Stock Movement', N'rbtnBranchStockMovement', N'1', N'x86FMl0UCz5k33zTpBJQXQ==', N'Button', 9, N'rTabMISReport', N'rpStock', N'rbtnBranchStockMovement', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (171, N'190150', N'MISReport/Deposit', N'rbpDeposit', N'1', N'jVvfcreXE+TEH6bDh+YePQ==', N'Panel', 6, N'rTabMISReport', N'rbpDeposit', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (172, N'190150110', N'MISReport/Deposit/Deposit', N'ribbonButton88', N'1', N'xq/ZqVIFusd8ZaD2wJIKiA==', N'Button', 9, N'rTabMISReport', N'rbpDeposit', N'ribbonButton88', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (173, N'190150120', N'MISReport/Deposit/Current Account', N'rbtnCurrentAccount', N'0', N'8HRNgiHyR7VekzwCFOw5qA==', N'Button', 9, N'rTabMISReport', N'rbpDeposit', N'rbtnCurrentAccount', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (174, N'190160', N'MISReport/Other', N'rpOthers', N'1', N'+Y2LLUWNe1ZW87lQIeUYdA==', N'Panel', 6, N'rTabMISReport', N'rpOthers', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (175, N'190160110', N'MISReport/Other/Adjustment', N'ribbonButton20', N'1', N'A7wx54ZcSuYJFVoEoQ6DoQ==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButton20', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (176, N'190160120', N'MISReport/Other/Co-Efficient', N'ribbonButtonCoEfficient', N'1', N'3C143qohDV9j83o8A41rBw==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButtonCoEfficient', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (177, N'190160130', N'MISReport/Other/Wastage', N'ribbonButtonWastage', N'1', N'oaoPs0YBz70Nnc95Ht1UQA==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButtonWastage', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (178, N'190160140', N'MISReport/Other/Value Change', N'ribbonButtonValueChange', N'1', N'HfeUi4kGfbz9M0cQ2loqDg==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButtonValueChange', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (179, N'190160150', N'MISReport/Other/Serial Stock', N'ribbonButtonSerStock', N'1', N'Ep1E+o4bgDjL+B6ULd9AdQ==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButtonSerStock', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (180, N'190160160', N'MISReport/Other/Purchage LC', N'ribbonButtonPurchaseLC', N'1', N'DZkzzbhZDw8yqKwwYbjK7g==', N'Button', 9, N'rTabMISReport', N'rpOthers', N'ribbonButtonPurchaseLC', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (181, N'190170', N'MISReport/Sale C/E', N'rpSaleCE', N'1', N'JHanP2K+QaYDvsDqbFJuCw==', N'Panel', 6, N'rTabMISReport', N'rpSaleCE', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (182, N'190170110', N'MISReport/Sale C/E/Sale C/E', N'ribbonButton27', N'1', N'TRYVH8KaBwYJPLWyBLsVkg==', N'Button', 9, N'rTabMISReport', N'rpSaleCE', N'ribbonButton27', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (183, N'190180', N'MISReport/Comparision Satement', N'rpMIS19', N'1', N'T3dqxU5vOrbSK0ZOnOa90A==', N'Panel', 6, N'rTabMISReport', N'rpMIS19', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (184, N'190180110', N'MISReport/Comparision Satement', N'rbtnMIS19', N'1', N'mvjIUA/u4aElEwdZy4OH7g==', N'Button', 9, N'rTabMISReport', N'rpMIS19', N'rbtnMIS19', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (185, N'200', N'UserAccount', N'rTabUserAccount', N'1', N'0FLpNLqK2po=', N'Tab', 3, N'rTabUserAccount', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (186, N'200110', N'UserAccount/NewAccount', N'rpNewAccount', N'1', N'KrxuVeNcznK44dRdoPrTRw==', N'Panel', 6, N'rTabUserAccount', N'rpNewAccount', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (187, N'200110110', N'UserAccount/NewAccount/NewAccount', N'ribbonButton63', N'1', N'KcbIBsaO+59IgN085c+QKQ==', N'Button', 9, N'rTabUserAccount', N'rpNewAccount', N'ribbonButton63', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (188, N'200120', N'UserAccount/PasswordChange', N'rpPasswordChange', N'1', N'DYZFSbkEic5QrljsVprZxw==', N'Panel', 6, N'rTabUserAccount', N'rpPasswordChange', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (189, N'200120110', N'UserAccount/PasswordChange/PasswordChange', N'ribbonButton62', N'1', N'UH+YKQlycIBxNpERqd8ohg==', N'Button', 9, N'rTabUserAccount', N'rpPasswordChange', N'ribbonButton62', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (190, N'200130', N'UserAccount/UserRole', N'rpUserRole', N'1', N'UlShmtxsH+grASimklyFjA==', N'Panel', 6, N'rTabUserAccount', N'rpUserRole', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (191, N'200130110', N'UserAccount/UserRole/UserRole', N'ribbonButton61', N'1', N'Rq7gZiP8sXsFm94p6cvMjQ==', N'Button', 9, N'rTabUserAccount', N'rpUserRole', N'ribbonButton61', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (192, N'200140', N'UserAccount/SettingsRole', N'rpSettingsRole', N'1', N'xQNcU79rzkyK4VxI/eeP/Q==', N'Panel', 6, N'rTabUserAccount', N'rpSettingsRole', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (193, N'200140110', N'UserAccount/SettingsRole/SettingsRole', N'ribBtnSettingRole', N'1', N'LZ4q/3jFJTWDbmetoxsR4Q==', N'Button', 9, N'rTabUserAccount', N'rpSettingsRole', N'ribBtnSettingRole', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (194, N'200150', N'UserAccount/Log Out', N'rpLout', N'1', N'Z/NG8A0wmt9P7YQSBInabw==', N'Panel', 6, N'rTabUserAccount', N'rpLout', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (195, N'200150110', N'UserAccount/Log Out/Log Out', N'ribbonButton7', N'1', N'I9CPejABG7glGEW7kg9Qxg==', N'Button', 9, N'rTabUserAccount', N'rpLout', N'ribbonButton7', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (196, N'200160', N'UserAccount/Log In', N'rpLogin', N'1', N'xWQzB6Smbwn7gK9W4BeZJg==', N'Panel', 6, N'rTabUserAccount', N'rpLogin', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (197, N'200160110', N'UserAccount/Log In/Log In', N'ribbonButton66', N'1', N'0Lk5fCnJCEyOY0oQXQxIFA==', N'Button', 9, N'rTabUserAccount', N'rpLogin', N'ribbonButton66', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (198, N'200170', N'UserAccount/Logs', N'rpLogs', N'1', N'LrpPJoQZNW4V/Doazt0iXg==', N'Panel', 6, N'rTabUserAccount', N'rpLogs', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (199, N'200170110', N'UserAccount/Logs/Logs', N'ribbonButton21', N'1', N'pO9CE+iVK5m2ATy9UnfsmQ==', N'Button', 9, N'rTabUserAccount', N'rpLogs', N'ribbonButton21', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (200, N'200180', N'UserAccount/Close All', N'rpCloseAll', N'1', N'e+Zl/y3iI8XGfpS9WaLO0g==', N'Panel', 6, N'rTabUserAccount', N'rpCloseAll', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (201, N'200180110', N'UserAccount/Close All/Close All', N'ribbonButton144', N'1', N'e1ETm43+psnqdmnTSD9zMQ==', N'Button', 9, N'rTabUserAccount', N'rpCloseAll', N'ribbonButton144', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (202, N'200190', N'UserAccount/User Branch', N'rpUserBranch', N'1', N'SWyp/VgcqpLRr/iPe7jUmQ==', N'Panel', 6, N'rTabUserAccount', N'rpUserBranch', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (203, N'200190110', N'UserAccount/User Branch/User Branch', N'ribbonButton36', N'1', N'FCm2tgQjHBBVD0PiQvYJ0A==', N'Button', 9, N'rTabUserAccount', N'rpUserBranch', N'ribbonButton36', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (204, N'200200', N'UserAccount/User Menu', N'rpnlUserMenu', N'1', N'B79/onxeVkWRybB/A82w+Q==', N'Panel', 6, N'rTabUserAccount', N'rpnlUserMenu', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (205, N'200200110', N'UserAccount/User Menu/All', N'rbtnMenuAll', N'1', N'0Sp2HJMO1R0CgwDqHZ3SMw==', N'Button', 9, N'rTabUserAccount', N'rpnlUserMenu', N'rbtnMenuAll', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (206, N'200200120', N'UserAccount/User Manu/User ', N'rbtnUserMenu', N'1', N'scHKIfuJVQX6hMqo7G037A==', N'Button', 9, N'rTabUserAccount', N'rpnlUserMenu', N'rbtnUserMenu', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (207, N'210', N'Banderol', N'rTabBanderol', N'0', N'7m3OliNu3U8=', N'Tab', 3, N'rTabBanderol', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (208, N'210110', N'Banderol/Demand', N'rpDemand', N'1', N'WkJcIG/fBbWK9gyOovGTmw==', N'Panel', 6, N'rTabBanderol', N'rpDemand', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (209, N'210110110', N'Banderol/Demand/Demand', N'ribBtnDemand', N'1', N'6kmtsH/4imDL//HaLe1x4A==', N'Button', 9, N'rTabBanderol', N'rpDemand', N'ribBtnDemand', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (210, N'210120', N'Banderol/Receive', N'rbpReceive', N'1', N'FET143Lar1Qn3lW56Od3Rg==', N'Panel', 6, N'rTabBanderol', N'rbpReceive', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (211, N'210120110', N'Banderol/Receive/Receive', N'rbnBtnBandReceive', N'1', N'yKFc6/tju7NbcLjbrN77zQ==', N'Button', 9, N'rTabBanderol', N'rbpReceive', N'rbnBtnBandReceive', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (212, N'220', N'SCBL', N'ribbonTabSCBL', N'0', N'aKuVzjghLLk=', N'Tab', 3, N'ribbonTabSCBL', N'NA', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (213, N'220110', N'SCBL/SCBLMIS', N'rpScbl', N'0', N'pT6uR1DVUjmtEHwUbADVUg==', N'Panel', 6, N'ribbonTabSCBL', N'rpScbl', N'NA', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (214, N'220110110', N'SCBL/SCBLMIS/Local Purchase', N'ribbonButtonLocalPurcchase', N'0', N'GZIS/64CHRPMDFnXN2AY4Q==', N'Button', 9, N'ribbonTabSCBL', N'rpScbl', N'ribbonButtonLocalPurcchase', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (215, N'220110120', N'SCBL/SCBLMIS/Local Sale', N'ribbonButtonLocalSales', N'0', N'm0GnNhS0zy/B5Bd5BXmicA==', N'Button', 9, N'ribbonTabSCBL', N'rpScbl', N'ribbonButtonLocalSales', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuAllFinalRolls] ([LineID], [FormId], [FormName], [RibbonName], [Access], [AccessRoll], [AccessType], [Len], [TabName], [PanelName], [ButtonName], [LastModifiedBy], [LastModifiedOn]) VALUES (216, N'220110130', N'SCBL/SCBLMIS/Receive Sale', N'rbtnReceiveSales', N'0', N'XBC+zAmARC27amd+2zA7dg==', N'Button', 9, N'ribbonTabSCBL', N'rpScbl', N'rbtnReceiveSales', N'admin', CAST(N'2020-02-16T19:56:58.000' AS DateTime))
//
//SET IDENTITY_INSERT [dbo].[UserMenuAllFinalRolls] OFF
//";

//                        transResult = commonDal.NewTableAdd("UserMenuAllFinalRolls", UserMenuAllFinalRolls, null, null);
//                        #endregion UserMenuAllFinalRolls

//                    }
//                    if (commonDal.NewTableExistCheck("UserMenuRolls", null, null) == 0)
//                    {
//                        #region UserMenuRolls
//                        string UserMenuRolls = @"
//
//CREATE TABLE [dbo].[UserMenuRolls](
//	[LineID] [int] IDENTITY(1,1) NOT NULL,
//	[FormID] [int] NOT NULL,
//	[UserID] [varchar](20) NOT NULL,
//	[FormName] [varchar](300) NOT NULL,
//	[Access] [int] NOT NULL,
//	[PostAccess] [int] NOT NULL,
//	[AddAccess] [int] NOT NULL,
//	[EditAccess] [int] NOT NULL,
//	[CreatedBy] [varchar](120) NULL,
//	[CreatedOn] [datetime] NULL,
//	[LastModifiedBy] [varchar](120) NULL,
//	[LastModifiedOn] [datetime] NULL,
// CONSTRAINT [PK_UserMenuRolls] PRIMARY KEY CLUSTERED 
//(
//	[FormID] ASC,
//	[UserID] ASC
//)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
//) ON [PRIMARY]
//
//
//SET IDENTITY_INSERT [dbo].[UserMenuRolls] ON 
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (1, 110110110, N'10', N'Setup/ItemInformation/Group', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (2, 110110120, N'10', N'Setup/ItemInformation/Product', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (3, 110110130, N'10', N'Setup/ItemInformation/Overhead', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (4, 110110140, N'10', N'Setup/ItemInformation/TDS', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (5, 110110150, N'10', N'Setup/ItemInformation/HSCode', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (6, 110120110, N'10', N'Setup/Vedor/Group', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (7, 110120120, N'10', N'Setup/Vedor/Vendor', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (8, 110130110, N'10', N'Setup/Customer/Group', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (9, 110130120, N'10', N'Setup/Customer/Customer', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (10, 110140110, N'10', N'Setup/BankVehicle/Bank', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (11, 110140120, N'10', N'Setup/BankVehicle/Vehicle', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (12, 110150110, N'10', N'Setup/PriceDeclaration/BOM', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (13, 110150120, N'10', N'Setup/PriceDeclaration/Service', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (14, 110150130, N'10', N'Setup/PriceDeclaration/Tender', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (15, 110160110, N'10', N'Setup/Company/CommpanyProfile', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (16, 110160120, N'10', N'Setup/Company/BranchProfile', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (17, 110170110, N'10', N'Setup/FiscalYear/FiscalYear', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (18, 110180110, N'10', N'Setup/Configuration/Settings', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (19, 110180120, N'10', N'Setup/Configuration/Prefix', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (20, 110180130, N'10', N'Setup/Configuration/Shift', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (21, 110190110, N'10', N'Setup/ImportSync/Import', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (22, 110190120, N'10', N'Setup/ImportSync/Sync', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (23, 110190130, N'10', N'Setup/ImportSync/Update/Delete query', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (24, 110200110, N'10', N'Setup/Measurment/Name', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (25, 110200120, N'10', N'Setup/Measurment/Conversion', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (26, 110210110, N'10', N'Setup/Currency/Currency', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (27, 110210120, N'10', N'Setup/Currency/Conversion', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (28, 110220110, N'10', N'Setup/Banderol/Banderol', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (29, 110220120, N'10', N'Setup/Banderol/Packaging', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (30, 110220130, N'10', N'Setup/Banderol/Product', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (31, 120110110, N'10', N'Purchase/Purchase/Local', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (32, 120110120, N'10', N'Purchase/Purchase/Import', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (33, 120110130, N'10', N'Purchase/Purchase/InputService', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (34, 120110140, N'10', N'Purchase/Purchase/PurchaseReturn', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (35, 120110150, N'10', N'Purchase/Purchase/Service Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (36, 120110160, N'10', N'Purchase/Purchase/Service Non Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (37, 130110110, N'10', N'Production/Issue/Issue', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (38, 130110120, N'10', N'Production/Issue/Return', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (39, 130110130, N'10', N'Production/Issue/Issue WithOut BOM', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (40, 130110140, N'10', N'Production/Issue/Wastage', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (41, 130110150, N'10', N'Production/Issue/Transfer', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (42, 130120110, N'10', N'Production/Receive/WIP', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (43, 130120120, N'10', N'Production/Receive/FGReceive', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (44, 130120130, N'10', N'Production/Receive/Return', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (45, 130120140, N'10', N'Production/Receive/Package', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (46, 140110110, N'10', N'Sale/Sale/Local', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (47, 140110120, N'10', N'Sale/Sale/Export', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (48, 140110130, N'10', N'Sale/Sale/Tender', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (49, 140110140, N'10', N'Sale/Sale/Trading', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (50, 140110150, N'10', N'Sale/Sale/Service Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (51, 140110160, N'10', N'Sale/Sale/Service Non Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (52, 140110170, N'10', N'Sale/Sale/RawSale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (53, 140110180, N'10', N'Sale/Sale/Wastage', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (54, 140120110, N'10', N'Sale/package/package', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (55, 140130110, N'10', N'Sale/Transfer  IssueRecieve/RM In', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (56, 140130120, N'10', N'Sale/Transfer  IssueRecieve/FG In', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (57, 140130130, N'10', N'Sale/Transfer IssueRecieve/RM  Out', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (58, 140130140, N'10', N'Sale/Transfer IssueRecieve/FG Out', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (59, 140140110, N'10', N'Sale/EXP/EXP', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (60, 150110110, N'10', N'Deposit/Treasury/Treasury', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (61, 150120110, N'10', N'Deposit/VDS/Purchage', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (62, 150120120, N'10', N'Deposit/VDS/Sale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (63, 150130110, N'10', N'Deposit/SD/SD', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (64, 150140110, N'10', N'Deposit/CashPayble/CashPayble', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (65, 150150110, N'10', N'Deposit/Reverse/Reverse', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (66, 150160110, N'10', N'Deposit/TDS/TDS', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (67, 160110110, N'10', N'Toll/Client/Issue 6.4', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (68, 160110120, N'10', N'Toll/Client/FGReceive', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (69, 160110130, N'10', N'Toll/Client/VAT11GAGA', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (70, 160120110, N'10', N'Toll/Contractor/RawReceive', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (71, 160120120, N'10', N'Toll/Contractor/FGProduction', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (72, 160120130, N'10', N'Toll/Contractor/FGIssue', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (73, 160120140, N'10', N'Toll/Contractor/Toll 6.3', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (74, 160130110, N'10', N'Toll/Toll Register/Toll 6.1', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (75, 160130120, N'10', N'Toll/Toll Register/Toll 6.2', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (76, 170110110, N'10', N'Adjustment/AdjustmentHead/Head', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (77, 170110120, N'10', N'Adjustment/AdjustmentHead/Transaction', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (78, 170120110, N'10', N'Adjustment/Purchase/DN', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (79, 170120120, N'10', N'Adjustment/Purchase/CN', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (80, 170130110, N'10', N'Adjustment/Sale/CN', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (81, 170130120, N'10', N'Adjustment/Sale/DN', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (82, 170140110, N'10', N'Adjustment/Dispose/26', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (83, 170140120, N'10', N'Adjustment/Dispose/27', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (84, 170150110, N'10', N'Adjustment/DDB/DDB', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (85, 170160110, N'10', N'Adjustment/VAT & SD Adjutment/VAT', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (86, 170160120, N'10', N'Adjustment/VAT & SD Adjutment/SD', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (87, 180110110, N'10', N'NBRReport/VAT4.3/VAT4.3', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (88, 180120110, N'10', N'NBRReport/VAT 6.1/VAT 6.1', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (89, 180130110, N'10', N'NBRReport/VAT 6.2/VAT 6.2', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (90, 180140110, N'10', N'NBRReport/VAT 9.1/VAT 9.1', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (91, 180150110, N'10', N'NBRReport/VAT 6.10/VAT 6.10', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (92, 180160110, N'10', N'NBRReport/SDReport/SDReport', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (93, 180170110, N'10', N'NBRReport/VAT 6.3/VAT 6.3', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (94, 180180110, N'10', N'NBRReport/VAT 6.5/VAT 6.5', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (95, 180190110, N'10', N'NBRReport/VAT 7/VAT 7', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (96, 180200110, N'10', N'NBRReport/VAT 20/VAT 20', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (97, 180210110, N'10', N'NBRReport/Banderol/Form 4', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (98, 180210120, N'10', N'NBRReport/Banderol/Form 5', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (99, 180220110, N'10', N'NBRReport/Summery-Current Account/Summery-Current Account', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (100, 180220120, N'10', N'NBRReport/Summery-Current Account/Breakdwon-Current Account', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (101, 180230110, N'10', N'NBRReport/Chak/Chak Ka', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (102, 180230120, N'10', N'NBRReport/Chak/Chak kha', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (103, 190110110, N'10', N'MISReport/Purchase/Purchase', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (104, 190120110, N'10', N'MISReport/Production/Issue', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (105, 190120120, N'10', N'MISReport/Production/Receive', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (106, 190130110, N'10', N'MISReport/Sale/Sale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (107, 190140110, N'10', N'MISReport/Stock/Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (108, 190140120, N'10', N'MISReport/Stock/Receive Sale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (109, 190140130, N'10', N'MISReport/Stock/Reconsciliation', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (110, 190140140, N'10', N'MISReport/Stock/Branch Stock Movement', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (111, 190150110, N'10', N'MISReport/Deposit/Deposit', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (112, 190150120, N'10', N'MISReport/Deposit/Current Account', 0, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (113, 190160110, N'10', N'MISReport/Other/Adjustment', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (114, 190160120, N'10', N'MISReport/Other/Co-Efficient', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (115, 190160130, N'10', N'MISReport/Other/Wastage', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (116, 190160140, N'10', N'MISReport/Other/Value Change', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (117, 190160150, N'10', N'MISReport/Other/Serial Stock', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (118, 190160160, N'10', N'MISReport/Other/Purchage LC', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (119, 190170110, N'10', N'MISReport/Sale C/E/Sale C/E', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (120, 190180110, N'10', N'MISReport/Comparision Satement', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (121, 200110110, N'10', N'UserAccount/NewAccount/NewAccount', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (122, 200120110, N'10', N'UserAccount/PasswordChange/PasswordChange', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (123, 200130110, N'10', N'UserAccount/UserRole/UserRole', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (124, 200140110, N'10', N'UserAccount/SettingsRole/SettingsRole', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (125, 200150110, N'10', N'UserAccount/Log Out/Log Out', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (126, 200160110, N'10', N'UserAccount/Log In/Log In', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (127, 200170110, N'10', N'UserAccount/Logs/Logs', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (128, 200180110, N'10', N'UserAccount/Close All/Close All', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (129, 200190110, N'10', N'UserAccount/User Branch/User Branch', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (130, 200200110, N'10', N'UserAccount/User Menu/All', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (131, 200200120, N'10', N'UserAccount/User Manu/User ', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (132, 210110110, N'10', N'Banderol/Demand/Demand', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (133, 210120110, N'10', N'Banderol/Receive/Receive', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (134, 220110110, N'10', N'SCBL/SCBLMIS/Local Purchase', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (135, 220110120, N'10', N'SCBL/SCBLMIS/Local Sale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//INSERT [dbo].[UserMenuRolls] ([LineID], [FormID], [UserID], [FormName], [Access], [PostAccess], [AddAccess], [EditAccess], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn]) VALUES (136, 220110130, N'10', N'SCBL/SCBLMIS/Receive Sale', 1, 1, 1, 1, N'Admin', CAST(N'1900-01-01T00:00:00.000' AS DateTime), N'admin', CAST(N'2020-02-16T17:17:58.000' AS DateTime))
//
//SET IDENTITY_INSERT [dbo].[UserMenuRolls] OFF
//";

//                        transResult = commonDal.NewTableAdd("UserMenuRolls", UserMenuRolls, null, null);

//                        #endregion UserMenuRolls
                    //                    }

                    #endregion

                    UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();
                    UserMenuRollsDal.UserMenuRollsInsertByUser(Program.CurrentUserID, null, null, null);


                    Program.serverDate = OrdinaryVATDesktop.StringToDateAsDate(commonDal.ServerDateTime().ToString());
                    UserMenuAllRollDAL _Dal = new UserMenuAllRollDAL();
                    Program.UserMenuAllRolls = new DataTable("UserMenuAllRolls");
                    Program.UserMenuRolls = new DataTable("UserMenuRolls");

                    Program.UserMenuAllRolls = _Dal.UserMenuAllRollsSelectAll(null, null, null, null, null, true, null);
                    string[] cFields = new string[] { "UserID" };
                    string[] cValues = new string[] { Program.CurrentUserID };
                    Program.UserMenuRolls = UserMenuRollsDal.UserMenuRollsSelectAll(null, cFields, cValues, null, null, true, null);


                    this.Close();


                }

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                _dbsqlConnection.ServerDateTime();

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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnLogIn.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void CheckAppVersion()
        {
            CompanyprofileDAL companyprofileDAL = new CompanyprofileDAL();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            string VersionMinor = Convert.ToString(version.Minor);
            string VersionBuild = Convert.ToString(version.Build);

            if (version.Minor < 10)
            {
                VersionMinor = "0" + version.Minor;

            }
            if (version.Build < 10)
            {
                VersionBuild = "0" + version.Build;
            }
            DataTable dtCompany = companyprofileDAL.SelectAll();
            string appVersion = "" + version.Major + VersionMinor + VersionBuild + version.Revision;
            if (dtCompany.Rows[0]["AppVersion"].ToString() != "-")
            {
                int dbVersion = Convert.ToInt32(dtCompany.Rows[0]["AppVersion"]);
                int currentVersion = Convert.ToInt32(appVersion);

                if (dbVersion > currentVersion)
                {
                    FormMessageShow formMessagemessage = new FormMessageShow();
                    formMessagemessage.ShowDialog();
                }
            }


        }

        public void UserMenuAdd()
        {
            #region Objects and Variables

            SettingDAL settingDal = new SettingDAL();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string transResult = "";

            #endregion

            #region try

            try
            {
                #region Connection and Transaction

                currConn = _dbsqlConnection.GetConnectionNoTimeOut();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);

                #endregion

                #region UserMenuAdd
                #region button
                transResult = settingDal.UserMenuAllRollsInsert("110110160", "Setup/ItemInformation/Transfer", "0", "rbtnProductTransfer", "Button", 9, "rTabSetup", "rpItemInformation", "rbtnProductTransfer", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("110120130", "Setup/Vedor/Custom House", "0", "rbtnCustomHouse", "Button", 9, "rTabSetup", "rpVendor", "rbtnCustomHouse", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("110190130", "Setup/ImportSync/Update/Day End", "0", "rbtnDayEnd", "Button", 9, "rTabSetup", "rpImportSync", "rbtnDayEnd", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("160110140", "Toll/Client/Raw Receive From Contractor", "0", "RawReceiveFromContractor", "Button", 9, "rTabToll", "rpImportSync", "RawReceiveFromContractor", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("160110150", "Toll/Client/FG Receive (WO BOM)", "0", "rbtnFGReceiveWOBom", "Button", 9, "rTabToll", "rpClient", "rbtnFGReceiveWOBom", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("160110160", "Toll/Client/Toll 6.3", "0", "rbtnTollClient6_3", "Button", 9, "rTabToll", "rpClient", "rbtnTollClient6_3", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("160120150", "Toll/Contractor/Issue With Bom", "0", "btnContractorIssueWOBom", "Button", 9, "rTabToll", "rpContractor", "btnContractorIssueWOBom", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("160120160", "Toll/Contractor/Raw Issue To Client", "0", "rbtnRawIssueToClient", "Button", 9, "rTabToll", "rpContractor", "rbtnRawIssueToClient", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("170120130", "Adjustment/Purchase/Raw CN", "0", "rbtnRawSaleCN", "Button", 9, "rTabAdjustment", "rplSale", "rbtnRawSaleCN", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180240110", "NBRReport/VAT 6.6/VAT 6.6", "0", "rbtn6_6", "Button", 9, "rTabNBRReport", "rp6_6", "rbtn6_6", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180240120", "NBRReport/VAT 6.7/VAT 6.7", "0", "rbtnVAT6_7", "Button", 9, "rTabNBRReport", "rpnlVAT6_7", "rbtnVAT6_7", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180240120", "NBRReport/VAT 6.8/VAT 6.8", "0", "rbtnVAT6_8", "Button", 9, "rTabNBRReport", "rpnlVAT6_8", "rbtnVAT6_8", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180240120", "NBRReport/VAT 6.10/VAT 6.10", "0", "rbtn6_10", "Button", 9, "rTabNBRReport", "rp610", "rbtn6_10", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110140", "SCBL/SCBLMIS/Sale Summary (AllShift)", "0", "rbtnSaleSummary", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnSaleSummary", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110150", "SCBL/SCBLMIS/Sale Summary by Product", "0", "rbtnSaleSummarybyProduct", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnSaleSummarybyProduct", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110160", "SCBL/SCBLMIS/Import Data", "0", "rbtnImportData", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnImportData", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110170", "SCBL/SCBLMIS/Receive Vs Sale", "0", "rbtnReceiveVsSale", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnReceiveVsSale", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110180", "SCBL/SCBLMIS/Sale Statement For Service", "0", "rbtnSalesStatementForService", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnSalesStatementForService", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110190", "SCBL/SCBLMIS/Stock Report FG", "0", "rbtnStockReportFG", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnStockReportFG", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110200", "SCBL/SCBLMIS/Stock Report RM", "0", "rbtnStockReportRM", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnStockReportRM", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110210", "SCBL/SCBLMIS/Transfer To Depo", "0", "rbtnTransferToDepo", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnTransferToDepo", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110220", "SCBL/SCBLMIS/VDS Statement", "0", "rbtnVDSStatement", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnVDSStatement", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("220110230", "SCBL/SCBLMIS/Monthly Production&Delivery", "0", "rbtnMonthly_Production_Delivery", "Button", 9, "ribbonTabSCBL", "rpScbl", "rbtnMonthly_Production_Delivery", currConn, transaction);
                #endregion

                #region Tab

                #endregion

                #region Panel
                transResult = settingDal.UserMenuAllRollsInsert("180240", "NBRReport/VAT 6.6", "0", "rp6_6", "Panel", 6, "rTabNBRReport", "rp6_6", "NA", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180250", "NBRReport/VAT 6.7", "0", "rp6_7", "Panel", 6, "rTabNBRReport", "rp6_7", "NA", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180260", "NBRReport/VAT 6.8", "0", "rp6_8", "Panel", 6, "rTabNBRReport", "rp6_8", "NA", currConn, transaction);
                transResult = settingDal.UserMenuAllRollsInsert("180270", "NBRReport/VAT 6.10", "0", "rp6_10", "Panel", 6, "rTabNBRReport", "rp6_10", "NA", currConn, transaction);

                #endregion
                #endregion
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + "sqlText" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

        }
        private bool CheckSecurity()
        {
            bool isSecured = false;
            try
            {

                ReportDSDAL reportDsdal = new ReportDSDAL();
                DataSet ReportResult = reportDsdal.ComapnyProfileSecurity(Program.CompanyID);
                string hardwareInfo = ReportResult.Tables[0].Rows[0]["Mouse"].ToString();
                if (!string.IsNullOrEmpty(hardwareInfo))
                {
                    string tom = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, ReportResult.Tables[0].Rows[0]["Tom"].ToString());
                    string jary = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, ReportResult.Tables[0].Rows[0]["Jary"].ToString());
                    string miki = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, ReportResult.Tables[0].Rows[0]["Miki"].ToString());
                    string mouse = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, ReportResult.Tables[0].Rows[0]["Mouse"].ToString());

                    string cName = ReportResult.Tables[0].Rows[0]["CompanyName"].ToString();
                    string legalName = ReportResult.Tables[0].Rows[0]["CompanyLegalName"].ToString();
                    string vatId = ReportResult.Tables[0].Rows[0]["VatRegistrationNo"].ToString();
                    CommonDAL commonDal = new CommonDAL();

                    //string processorId = commonDal.GetHardwareID();
                    string processorId = commonDal.GetServerHardwareId();

                    //for security purpose retrive server processor id and match from database.
                    if (tom == cName && jary == legalName && miki == vatId && mouse == processorId)
                    {
                        isSecured = true;
                    }

                }
            }
            catch (Exception)
            {
                isSecured = false;
            }

            return isSecured;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                DialogResult dialogResult = MessageBox
                    .Show("Are you sure you want to exit?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                this.DialogResult = DialogResult.None;

                if (dialogResult == DialogResult.Yes)
                {
                    Program.CurrentUser = string.Empty;
                    Program.CurrentUserID = string.Empty;
                    //CommonDAL.DBClose();
                    _isExit = true;

                    this.Close();
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
                FileLogger.Log(this.Name, "btnExit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnExit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnExit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnExit_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnExit_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnExit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnExit_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnExit_Click", exMessage);
            }
            #endregion

        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnLogIn_Click(sender, e);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                SessionDate();

                FormSupperAdministrator frmSupperAdministrator = new FormSupperAdministrator();
                frmSupperAdministrator.Show();

                //FormCompanyProfile frmCompanyProfile = new FormCompanyProfile();
                ////frmCompanyProfile.MdiParent = this;
                //frmCompanyProfile.btnAdd.Visible = false;
                //frmCompanyProfile.btnNewCompany.Visible = true;
                //frmCompanyProfile.Show();
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
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button1_Click", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion
        }

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
            LoadUserName();

            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                if (!txtUserName.Focus())
                    this.ActiveControl = txtUserName;
            }
            else
                txtUserPassword.Focus();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            //var tt=Program.FormatingNumeric("1000", 4);
            //MessageBox.Show(tt.ToString());

            // var result = "@e123-abcd3 3.";
            ////result= Regex.Replace(result, @"[0-9\-]", "_");
            //  //result = Regex.Replace(result, @"[\d-]", "1");
            // result= Regex.Replace(result, "[^a-zA-Z0-9_.]+", "_", RegexOptions.Compiled);
            // MessageBox.Show(result);

        }

        private void cmbCompany_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtUserPassword_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void label5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.symphonysoftt.com");
        }

        private void bgwLoginInfo_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void FormLogIn_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.BranchCode == "login" && !string.IsNullOrEmpty(Program.CurrentUserID))
            {
                FormLoginBranch frmlLogIn = new FormLoginBranch();
                frmlLogIn.ShowDialog();
            }
        }

        private void FormLogIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Program.CurrentUserID) && Program.BranchId == 0)
            //{
            //    FormLoginBranch frmlLogIn = new FormLoginBranch();
            //    frmlLogIn.ShowDialog();
            //}
        }

    }
}
