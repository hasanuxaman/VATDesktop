using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;
using SymphonySofttech.Utilities;

namespace VATClient
{
    public partial class FormDBMigration : Form
    {
        public FormDBMigration()
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

        // ----------- Declare from DBConstant Start--------//
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       

        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        // ----------- Declare from DBConstant End--------//
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        private SettingDAL settingDal = new SettingDAL();
        private string sqlResultssettings;



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
        private DataSet companyList;
       // private List<CmpanyListVM> companies = new List<CmpanyListVM>();
        #endregion

        private void btnDBMigration_Click(object sender, EventArgs e)
        {
           
            try
            {
                this.progressBar1.Visible = true;
                this.btnDBMigration.Enabled = false;
                this.btnClose.Enabled = false;

                bgwCompanyList.RunWorkerAsync();
               
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
                FileLogger.Log(this.Name, "bgwCompanyList_RunWorkerCompleted", exMessage);
            }
           
            finally
            {
                ////this.progressBar1.Visible = true;
                ////this.progressBar1.Enabled = true;
                ////this.btnDBMigration.Enabled = false;
                ////this.btnClose.Enabled = false;
                
            }
        }

        private void FormDBMigration_Load(object sender, EventArgs e)
        {
            
        }

        private void bgwCompanyList_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                companyList= new DataSet();
                CommonDAL commonDal = new CommonDAL();
                string currentDatabase = DatabaseInfoVM.DatabaseName;

                Program.successLogin = false;

                companyList = commonDal.CompanyList("mXSJfsAdbf0=",connVM,"All"); //Y="mXSJfsAdbf0=";
                // done
                //End DoWork

                if (companyList.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no Company to select", this.Text);
                    return;
                }
                foreach (DataRow item2 in companyList.Tables[0].Rows)
                {
                    var company = new CmpanyListVM();
                    company.CompanySl = item2["CompanySl"].ToString();
                    company.CompanyID = SymphonySofttech.Utilities.Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyID"].ToString());
                    company.CompanyName = SymphonySofttech.Utilities.Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyName"].ToString());
                    company.DatabaseName = SymphonySofttech.Utilities.Converter.DESDecrypt(PassPhrase, EnKey, item2["DatabaseName"].ToString());
                    company.ActiveStatus = SymphonySofttech.Utilities.Converter.DESDecrypt(PassPhrase, EnKey, item2["ActiveStatus"].ToString());
                    company.Serial = item2["Serial"].ToString();

                    DatabaseInfoVM.DatabaseName = company.DatabaseName;
                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { lblDbMigration.Text = company.DatabaseName; });

                    commonDal.DatabaseTableChanges(connVM);
                }

                DatabaseInfoVM.DatabaseName = currentDatabase;
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
        }

        private void bgwCompanyList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
               
                //////this.progressBar1.Enabled = false;
                //////this.progressBar1.Visible = false;

                this.progressBar1.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
                this.btnDBMigration.Enabled = true;
                this.btnClose.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
