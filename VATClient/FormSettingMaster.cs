using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Threading;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormSettingMaster : Form
    {
        #region Constructors

        public FormSettingMaster()
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
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet SettingsResult;
        private List<SettingsVM> settingsVMs;
        private IList<SettingsVM> settingsVMList;
        private IList<SettingsVM> settingsVMListClone;
        private bool ChangeData = false;
        string FolderPath = "";

        private string preSettingValue = string.Empty;
        private string UserID = string.Empty;
        private bool IsSerialTracking = false;

        ProductDAL _ProductDAL = new ProductDAL();
        ResultVM rVM = new ResultVM();

        #region sql save, update, delete

        private SettingDAL settingDal = new SettingDAL();
        private string[] sqlResults;
        private string sqlResultssettings;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Form Load

        private void GetLoad()
        {
            try
            {
                #region Statement

                this.progressBar1.Visible = true;

                backgroundWorkerLoad.RunWorkerAsync();


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
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GetLoad", exMessage);
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
                FileLogger.Log(this.Name, "GetLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GetLoad", exMessage);
            }
            #endregion
        }

        private void DataGridLoad(List<SettingsVM> dgvSettingVms)
        {
            try
            {
                #region Code

                var settingDataTable = ConvertToDataTable(dgvSettingVms);

                dgvSettings.Rows.Clear();
                int j = 0;
                foreach (DataRow item in settingDataTable.Rows)
                {
                    DataGridView dgv = new DataGridView();
                    dgvSettings.Rows.Add(dgv);
                    dgvSettings.Rows[j].Cells["SettingId"].Value = item["SettingId"].ToString();
                    dgvSettings.Rows[j].Cells["SettingGroup"].Value = Program.AddSpacesToSentence(item["SettingGroup"].ToString());
                    dgvSettings.Rows[j].Cells["SettingName"].Value = Program.AddSpacesToSentence(item["SettingName"].ToString());

                    //dgvSettings.Rows[j].Cells["SettingGroup"].Value = item["SettingGroup"].ToString();
                    //dgvSettings.Rows[j].Cells["SettingName"].Value = item["SettingName"].ToString();


                    dgvSettings.Rows[j].Cells["SettingValue"].Value = item["SettingValue"].ToString();
                    dgvSettings.Rows[j].Cells["SettingType"].Value = item["SettingType"].ToString();
                    dgvSettings.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();

                    j += 1;
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
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
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
                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
            }

            #endregion
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            FormMaker();
            bgwSettingsValue.RunWorkerAsync();

        }

        private void bgwSettingsValue_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {
                if (rbtnRole.Checked)
                {
                    UserID = Program.CurrentUserID;
                }
                UPDATE_DOWORK_SUCCESS = false;
                sqlResultssettings = string.Empty;


                var a = "";
                CommonDAL _cdal = new CommonDAL();
                //ICommon _cdal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                _cdal.SettingChangeMaster(connVM);




                UPDATE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSettingsValue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try statement
            try
            {
                if (UPDATE_DOWORK_SUCCESS)
                {
                    GetLoad();
                }
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
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }

        private void FormMaker()
        {
            //if (rbtnRole.Checked)
            //{
            //    lblUser.Visible = true;
            //    txtUserName.Visible = true;
            //    btnMigration.Visible = false;
            //    txtUserID.Text = Program.CurrentUserID;
            //    txtUserName.Text = Program.CurrentUser;
            //}
            //else
            //{
            //    lblUser.Visible = false;
            //    txtUserName.Visible = false;
            //    txtUserID.Visible = false;
            //    btnMigration.Visible = true;
            //}

            lblUser.Visible = false;
            txtUserName.Visible = false;
            txtUserID.Visible = false;
            btnMigration.Visible = true;
        }

        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try Statement
            try
            {
                //if (rbtnRole.Checked)
                //{
                //    //SettingRoleDAL settingRoleDal = new SettingRoleDAL();
                //    ISettingRole settingRoleDal = OrdinaryVATDesktop.GetObject<SettingRoleDAL, SettingRoleRepo, ISettingRole>(OrdinaryVATDesktop.IsWCF);
                //    SettingsResult = settingRoleDal.SearchSettingsRole(txtUserID.Text.Trim(), txtUserName.Text.Trim(), connVM);
                //}
                //else
                //{
                //    //SettingDAL settingDal = new SettingDAL();
                //    ISetting settingDal = OrdinaryVATDesktop.GetObject<SettingDAL, SettingRepo, ISetting>(OrdinaryVATDesktop.IsWCF);
                //    SettingsResult = settingDal.SearchSettings(connVM);
                //}

                //SettingDAL settingDal = new SettingDAL();
                SettingDAL settingDal = new SettingDAL();

                SettingsResult = settingDal.SearchSettingsMaster(connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                dgvSettings.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SettingsResult.Tables[0].Rows)
                {
                    DataGridView dgv = new DataGridView();
                    dgvSettings.Rows.Add(dgv);
                    dgvSettings.Rows[j].Cells["SettingId"].Value = item["SettingId"].ToString();
                    //dgvSettings.Rows[j].Cells["SettingGroup"].Value = item["SettingGroup"].ToString();
                    //dgvSettings.Rows[j].Cells["SettingName"].Value = item["SettingName"].ToString();

                    dgvSettings.Rows[j].Cells["SettingGroup"].Value = Program.AddSpacesToSentence(item["SettingGroup"].ToString());
                    dgvSettings.Rows[j].Cells["SettingName"].Value = Program.AddSpacesToSentence(item["SettingName"].ToString());

                    dgvSettings.Rows[j].Cells["SettingValue"].Value = item["SettingValue"].ToString();
                    dgvSettings.Rows[j].Cells["SettingType"].Value = item["SettingType"].ToString();
                    dgvSettings.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();

                    j += 1;
                }

                settingsVMList = new List<SettingsVM>();
                settingsVMListClone = new List<SettingsVM>();

                foreach (DataRow item in SettingsResult.Tables[0].Rows)
                {
                    SettingsVM settingVm = new SettingsVM();

                    settingVm.SettingId = item["SettingId"].ToString();
                    settingVm.SettingGroup = item["SettingGroup"].ToString();
                    settingVm.SettingName = item["SettingName"].ToString();
                    settingVm.SettingValue = item["SettingValue"].ToString();
                    settingVm.SettingType = item["SettingType"].ToString();
                    settingVm.ActiveStatus = item["ActiveStatus"].ToString();
                    settingsVMList.Add(settingVm);

                }

                cmbSettingGroup.Items.Clear();
                foreach (DataRow item in SettingsResult.Tables[1].Rows)
                {
                    cmbSettingGroup.Items.Add(Program.AddSpacesToSentence(item["SettingGroup"].ToString()));

                }

                cmbSettingGroup.Items.Insert(0, " All ");

                cmbSettingGroup.SelectedIndex = 0;
                cmbSettingGroup.Text = " All ";

                settingsVMListClone = settingsVMList;

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        #endregion



        #region Settings Update

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try statement
            try
            {
                this.progressBar1.Visible = true;
                this.btnUpdate.Enabled = false;

                settingsVMs = new List<SettingsVM>();

                for (int i = 0; i < dgvSettings.RowCount; i++)
                {
                    SettingsVM settingsVM = new SettingsVM();
                    //DBName = DBName.Replace(" ", "_");
                    settingsVM.SettingId = dgvSettings.Rows[i].Cells["SettingId"].Value.ToString();
                    settingsVM.SettingGroup = dgvSettings.Rows[i].Cells["SettingGroup"].Value.ToString().Replace(" ", "");
                    settingsVM.SettingName = dgvSettings.Rows[i].Cells["SettingName"].Value.ToString().Replace(" ", "");
                    settingsVM.SettingValue = dgvSettings.Rows[i].Cells["SettingValue"].Value.ToString().ToUpper();

                    settingsVM.SettingType = dgvSettings.Rows[i].Cells["SettingType"].Value.ToString();
                    settingsVM.ActiveStatus = dgvSettings.Rows[i].Cells["ActiveStatus"].Value.ToString();
                    settingsVM.CreatedBy = Program.CurrentUser;
                    settingsVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    settingsVM.LastModifiedBy = Program.CurrentUser;
                    settingsVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    settingsVMs.Add(settingsVM);

                }//End For

                backgroundWorkerUpdate.RunWorkerAsync();
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

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                //BugsBD
                string UserName = OrdinaryVATDesktop.SanitizeInput(txtUserName.Text);
                string UserID = OrdinaryVATDesktop.SanitizeInput(txtUserID.Text);

                if (rbtnRole.Checked)
                {
                    //SettingRoleDAL settingRoleDal = new SettingRoleDAL();

                    ISettingRole settingRoleDal = OrdinaryVATDesktop.GetObject<SettingRoleDAL, SettingRoleRepo, ISettingRole>(OrdinaryVATDesktop.IsWCF);

                    if (Program.CurrentUser.ToLower() == "admin" && Program.CurrentUserID == "10")
                    {
                        sqlResults =
                            //settingRoleDal.SettingsUpdate(settingsVMs, connVM, txtUserName.Text, txtUserID.Text);
                            settingRoleDal.SettingsUpdate(settingsVMs, connVM, UserName, UserID);

                    }
                    else
                    {
                        sqlResults = settingRoleDal.SettingsUpdate(settingsVMs, connVM);

                    }

                }
                else
                {
                    //SettingRoleDAL settingRoleDal = new SettingRoleDAL();
                    //ISettingRole settingRoleDal = OrdinaryVATDesktop.GetObject<SettingRoleDAL, SettingRoleRepo, ISettingRole>(OrdinaryVATDesktop.IsWCF);
                    //sqlResults = settingRoleDal.SettingsUpdate(settingsVMs, connVM);

                    //SettingDAL settingDal = new SettingDAL();
                    SettingDAL settingDal = new SettingDAL();
                    sqlResults = settingDal.SettingsUpdatelistMaster(settingsVMs, connVM);
                }



                UPDATE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try statement
            try
            {
                if (UPDATE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (result == "Success")
                            {
                                foreach (var item in settingsVMs)
                                {
                                    var tempSettingVM = settingsVMListClone.SingleOrDefault(x => x.SettingId == item.SettingId);
                                    tempSettingVM.SettingValue = item.SettingValue;
                                }

                                var tempSettingsDT = settingsVMListClone.Where(x => x.ActiveStatus == "Y").ToList();

                                var dataTable = ConvertToDataTable(tempSettingsDT);

                                settingVM.SettingsDT = dataTable;
                                ChangeData = false;
                            }
                        }

                    }
                }

                SetSettingsDT();
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }

        #endregion

        #region DB Migration

        private void btnMigration_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            //this.button1.Enabled = false;
            bgwDBProcess.RunWorkerAsync();
            this.Enabled = false;
        }

        private void bgwDBProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {
                UPDATE_DOWORK_SUCCESS = false;
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                //SettingDAL settingDal = new SettingDAL();
                ISetting settingDal = OrdinaryVATDesktop.GetObject<SettingDAL, SettingRepo, ISetting>(OrdinaryVATDesktop.IsWCF);

                string companyId = Program.CompanyID;
                settingDal.SettingsUpdate(companyId, Program.BranchId, connVM, version);

                UPDATE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwDBProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (UPDATE_DOWORK_SUCCESS)
                {
                    MessageBox.Show("All The Patch Updated", this.Text);
                }

                #region Background Worker for Stock Update
                
                //bgwStockUpdate.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "bgwDBProcess_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.Enabled = true;
                this.progressBar1.Visible = false;
                //this.button1.Enabled = true;

            }

        }

        private void bgwStockUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                rVM = new ResultVM();

                ParameterVM vm = new ParameterVM();
                vm.IsDBMigration = true;
                vm.BranchId = Program.BranchId;

                rVM = _ProductDAL.Product_Stock_Update(vm,null,null,connVM,Program.CurrentUserID);
            }
            catch (Exception ex)
            {

                //throw ex;
            }
        }

        private void bgwStockUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show("Stock Process Completed");
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
                FileLogger.Log(this.Name, "bgwDBProcess_RunWorkerCompleted", exMessage);
            }

            finally
            {
                progressBar1.Visible = false;
                this.Enabled = true;

            }
        }

        #endregion

        private void dgvSettings_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (dgvSettings.Rows.Count > 0)
                {
                    var SettingGroup = dgvSettings.CurrentRow.Cells["SettingGroup"].Value.ToString();
                    var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                    SettingGroup = SettingGroup.Replace(" ", "");
                    SettingName = SettingName.Replace(" ", "");

                    if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "vat11savelocation")
                    {
                        dgvSettings.CurrentRow.ReadOnly = true;
                        return;
                    }
                }

                //getting SettingValue from datagrid by rowindex and columindex
                string settingValueExiting = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                preSettingValue = string.Empty;
                preSettingValue = settingValueExiting;

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
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", exMessage);
            }
            #endregion
        }

        private void dgvSettings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region try

            try
            {
                #region Maximum copy count


                if (dgvSettings.Rows.Count > 0)
                {
                    var SettingGroup = dgvSettings.CurrentRow.Cells["SettingGroup"].Value.ToString();
                    var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                    SettingGroup = SettingGroup.Replace(" ", "");
                    SettingName = SettingName.Replace(" ", "");

                    if (SettingGroup.ToLower() == "printer" && SettingName.ToLower() == "maxnoofprint")
                    {
                        var SettingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();
                        if (Convert.ToInt32(SettingValue) > 100)
                        {
                            dgvSettings["SettingValue", dgvSettings.CurrentRow.Index].Value = "100";
                        }
                    }
                    #region Serial Tracking


                    if (SettingGroup.ToLower() == "trackingtrace" && SettingName.ToLower() == "tracking")
                    {
                        DisplaySerialTrackingRows();
                    }

                    #endregion Serial Tracking

                }
                #endregion Maximum copy count

                //getting SettingValue from datagrid by rowindex and columindex
                string settingValue = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                //getting SettingType from datagrid by rowindex and columindex
                string settingType = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();

                DataGridViewCellStyle cellStyleValid = new DataGridViewCellStyle();
                cellStyleValid.ForeColor = Color.Black;

                DataGridViewCellStyle cellStyleError = new DataGridViewCellStyle();
                cellStyleError.ForeColor = Color.Red;

                if (settingType == "int") //if settingType Value int
                {
                    if (Program.IsSettingInteger(settingValue))
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        ChangeData = true;
                        return;
                    }
                    else
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preSettingValue;
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        MessageBox.Show("Please, insert integer type value.", this.Text);
                        return;
                    }
                }
                else if (settingType == "Decimal") //if settingType Value int
                {
                    if (Program.IsSettingDecimal(settingValue))
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        ChangeData = true;
                        return;
                    }
                    else
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preSettingValue;
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        MessageBox.Show("Please, insert integer type value.", this.Text);
                        return;
                    }
                }
                else if (settingType == "bool") //if settingType Value bool
                {
                    //settingValue = settingValue.ToUpper();
                    if (Program.IsSettingActive(settingValue))
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        ChangeData = true;
                        return;
                    }
                    else
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preSettingValue;
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        MessageBox.Show("Please, insert bool type(Y or N) value.", this.Text);
                        return;
                    }
                }
                else if (settingType == "string") //if settingType Value string
                {
                    if (Program.IsSettingString(settingValue))
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        ChangeData = true;
                        return;
                    }
                    else
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preSettingValue;
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        MessageBox.Show("Please, insert string type value.", this.Text);
                        return;
                    }
                }
                else if (settingType == "date") //if settingType Value date
                {

                    string settingValueFormat = Convert.ToDateTime(settingValue).ToString("dd/MMM/yyyy");

                    if (Program.IsSettingDate(settingValue) && settingValueFormat == settingValue)
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        ChangeData = true;
                        return;
                    }
                    else
                    {
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preSettingValue;
                        dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        MessageBox.Show("Please, insert date type(dd/mm/yyyy) value.", this.Text);
                        return;
                    }

                }

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
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", exMessage);
            }
            #endregion
        }

        private void DisplaySerialTrackingRows()
        {
            #region Code

            string cmbSettingGroupText = cmbSettingGroup.Text.Replace(" ", "");
            int cmbSettingGroupIndex = cmbSettingGroup.SelectedIndex;
            if (cmbSettingGroupIndex >= 0 && cmbSettingGroupText.Trim().ToLower() != "all")

            //if (cmbSettingGroupIndex >= 0)
            {
                #region Serial Tracking
                if (cmbSettingGroupText == "TrackingTrace")
                {
                    if (dgvSettings.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgvSettings.Rows.Count; i++)
                        {
                            string settingName = dgvSettings.Rows[i].Cells["SettingName"].Value.ToString().Replace(" ", "");
                            string settingValue = dgvSettings.Rows[i].Cells["SettingValue"].Value.ToString().Replace(" ", "");

                            if (settingName == "Tracking" && settingValue == "Y")
                            {
                                dgvSettings.Rows[2].Visible = true; // noOfHeading
                                dgvSettings.Rows[3].Visible = true; // Heading_1
                                dgvSettings.Rows[1].Visible = true; // Heading_2

                            }
                            else if (settingName == "Tracking" && settingValue == "N")
                            {
                                dgvSettings.Rows[2].Visible = false; // noOfHeading
                                dgvSettings.Rows[3].Visible = false; // Heading_1
                                dgvSettings.Rows[1].Visible = false; // Heading_2
                            }
                        }
                    }
                }
                #endregion

            }
            else //if (cmbSettingGroupIndex < 0)
            {
                var tempSettingsVMs = settingsVMList.ToList();
                DataGridLoad(tempSettingsVMs);
            }

            #endregion
        }

        private static DataTable ConvertToDataTable(List<SettingsVM> settingsDTList)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(SettingsVM));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                dataTable.Columns.Add(prop.Name, prop.PropertyType);

            }

            object[] values = new object[props.Count];

            foreach (SettingsVM item in settingsDTList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSettings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSettings.Rows.Count > 0)
            {
                var SettingGroup = dgvSettings.CurrentRow.Cells["SettingGroup"].Value.ToString();
                var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                SettingGroup = SettingGroup.Replace(" ", "");
                SettingName = SettingName.Replace(" ", "");

                if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "vat11savelocation")
                {
                    dgvSettings.CurrentRow.ReadOnly = true;
                    return;
                }

                //if (SettingGroup.ToLower() == "serialtracking" && SettingName.ToLower() == "noofheader")
                //{

                //    string settingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();

                //    for (int i = 1; i <= Convert.ToInt32(settingValue); i++)
                //    {
                //        sqlResultssettings = settingDal.settingsDataInsert("SerialTracking", "Heading_" + i, "string", "-", null, null);
                //    }
                //    //ChangeData = false;
                //    //GetLoad();
                //    return;
                //}
                #region Printer Setup

                if (SettingGroup.ToLower() == "printer" && SettingName.ToLower() == "defaultprinter")
                {
                    PrintDialog printDlg = new PrintDialog();
                    string printer = "";
                    dgvSettings.CurrentRow.ReadOnly = true;
                    //Call ShowDialog
                    if (printDlg.ShowDialog() == DialogResult.OK)
                    {
                        printer = printDlg.PrinterSettings.PrinterName.ToString();
                        dgvSettings.CurrentRow.Cells["SettingValue"].Value = printer;
                    }

                }
                #endregion Printer Setup
            }
        }

        private void cmbSettingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    DialogResult dialogResult = MessageBox.Show(@"Recent changes have not been saved ." + "\n" + " Want to close without saving?", @"", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        return;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        cmbSettingGroup.SelectedIndex = cmbSettingGroup.SelectedIndex;
                    }
                }

                #region Code

                string cmbSettingGroupText = cmbSettingGroup.Text.Replace(" ", "");
                int cmbSettingGroupIndex = cmbSettingGroup.SelectedIndex;
                if (cmbSettingGroupIndex >= 0 && cmbSettingGroupText.Trim().ToLower() != "all")
                {

                    var tempSettingVms = settingsVMList.Where(x => x.SettingGroup == cmbSettingGroupText).ToList();
                    DataGridLoad(tempSettingVms);
                    DisplaySerialTrackingRows();

                }
                else
                {
                    var tempSettingsVMs = settingsVMList.ToList();
                    DataGridLoad(tempSettingsVMs);
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
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", exMessage);
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
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbSettingGroup_SelectedIndexChanged", exMessage);
            }

            #endregion
        }

        private void cmbSettingGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void FormSetting_FormClosing(object sender, FormClosingEventArgs e)
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

                SetSettingsDT();


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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void SetSettingsDT()
        {
            IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

            DataSet ReportResult = reportDsdal.ComapnyProfileString(Program.CompanyID, Program.CurrentUserID, connVM);


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


        }

        private void btnVATSaveLocation_Click(object sender, EventArgs e)
        {
            #region Old


            //if (dgvSettings.Rows.Count <= 0)
            //{
            //    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //var SettingGroup = dgvSettings.CurrentRow.Cells["SettingGroup"].Value.ToString();
            //var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
            //var SettingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();
            //SettingGroup = SettingGroup.Replace(" ", "");
            //SettingName = SettingName.Replace(" ", "");

            //if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "reportsavelocation")
            //{
            //    dgvSettings.CurrentRow.ReadOnly = true;

            //    FolderBrowserDialog fbd = new FolderBrowserDialog();
            //    DialogResult result = fbd.ShowDialog();
            //    FolderPath = fbd.SelectedPath;
            //    if (FolderPath.Length==0)
            //    {

            //        //MessageBox.Show("Please Select folder for saving print location");
            //        FolderPath = SettingValue;
            //    }


            //    else if (FolderPath.Length > 120)
            //    {
            //        MessageBox.Show("Please Select small folder location");
            //        FolderPath = SettingValue;
            //        return;
            //    }
            //    dgvSettings.CurrentRow.Cells["SettingValue"].Value =FolderPath.Trim();
            //    //dgvSettings["SettingValue", dgvSettings.RowCount - 1].Value = 
            //}
            //else
            //{
            //    MessageBox.Show("Please select 'Sale' in Setting Group and 'VAT 11 Save Location' in Setting Name for saving print location.");
            //    return;
            //}
            #endregion Old

            //SettingDAL settingDal = new SettingDAL();
            ISetting settingDal = OrdinaryVATDesktop.GetObject<SettingDAL, SettingRepo, ISetting>(OrdinaryVATDesktop.IsWCF);

            string msg = settingDal.UpdateInternalIssueValue(connVM);
            MessageBox.Show(msg);
            btnVATSaveLocation.Visible = false;
        }

        private void dgvSettings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSettings.Rows.Count > 0)
            {
                var SettingGroup = dgvSettings.CurrentRow.Cells["SettingGroup"].Value.ToString();
                var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                SettingGroup = SettingGroup.Replace(" ", "");
                SettingName = SettingName.Replace(" ", "");

                #region Serial Tracking


                if (SettingGroup.ToLower() == "serialtracking" && SettingName.ToLower() == "serialtracking")
                {
                    string settingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();
                    if (settingValue == "Y")
                    {
                        string settingID = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();

                        sqlResultssettings = settingDal.settingsDataInsert("SerialTracking", "NoOfHeader", "int", "2", null, null,connVM);

                        int j = dgvSettings.CurrentRow.Index;

                        DataGridView dgv = new DataGridView();
                        dgvSettings.Rows.Add(dgv);
                        dgvSettings.Rows[j].Cells["SettingId"].Value = (Convert.ToInt32(settingID) + 1).ToString();
                        dgvSettings.Rows[j].Cells["SettingGroup"].Value = Program.AddSpacesToSentence("SerialTracking");
                        dgvSettings.Rows[j].Cells["SettingName"].Value = Program.AddSpacesToSentence("NoOfHeader");

                        dgvSettings.Rows[j].Cells["SettingValue"].Value = "2";
                        dgvSettings.Rows[j].Cells["SettingType"].Value = "int";
                        dgvSettings.Rows[j].Cells["ActiveStatus"].Value = "Y";

                    }
                }
                if (SettingGroup.ToLower() == "serialtracking" && SettingName.ToLower() == "noofheader")
                {

                    //string settingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();
                    //string settingId = dgvSettings.CurrentRow.Cells["SettingID"].Value.ToString();

                    //for (int i = 1; i <= Convert.ToInt32(settingValue); i++)
                    //{
                    //    sqlResultssettings = settingDal.settingsDataInsert("SerialTracking", "Heading_" + i, "string", "-", null, null);

                    //    int j = dgvSettings.Rows.Count;;

                    //    DataGridView dgv = new DataGridView();
                    //    dgvSettings.Rows.Add(dgv);
                    //    dgvSettings.Rows[j].Cells["SettingId"].Value = (Convert.ToInt32(settingId) + 1).ToString();
                    //    dgvSettings.Rows[j].Cells["SettingGroup"].Value = Program.AddSpacesToSentence("SerialTracking");
                    //    dgvSettings.Rows[j].Cells["SettingName"].Value = Program.AddSpacesToSentence("Heading_" + i);

                    //    dgvSettings.Rows[j].Cells["SettingValue"].Value = "-";
                    //    dgvSettings.Rows[j].Cells["SettingType"].Value = "string";
                    //    dgvSettings.Rows[j].Cells["ActiveStatus"].Value = "Y";

                    //}
                }
                #endregion Serial Tracking
                #region OldTransaction
                //if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "reportsavelocation")
                //{
                //    dgvSettings.CurrentRow.ReadOnly = true;
                //    btnVATSaveLocation_Click(sender, e);
                //}


                //if (SettingGroup.ToLower() == "transaction" && SettingName.ToLower() == "accessuser")
                //{
                //    for (int i = 0; i < dgvSettings.Rows.Count; i++)
                //    {
                //        if (dgvSettings["SettingName",i].Value.ToString().Replace(" ","").ToLower()=="accesstransaction")
                //        {
                //            dgvSettings.CurrentRow.ReadOnly = true;
                //            //dgvSettings["SettingValue", dgvSettings.CurrentRow.Index + 1].ReadOnly = true;

                //            if (dgvSettings["SettingValue", i].Value.ToString()=="N")
                //            {
                //               string result = FormUserSearch.SelectOne();
                //                if (result == "")
                //                {
                //                    MessageBox.Show("Please Select User who access all transaction.");
                //                    return;

                //                }
                //                else 
                //                {
                //                    string[] UserInfo = result.Split(FieldDelimeter.ToCharArray());

                //                    string userId = UserInfo[0]; ///UserID
                //                    string userName = UserInfo[1]; //userName
                //                    dgvSettings.CurrentRow.Cells["SettingValue"].Value = userName;
                //                    //dgvSettings["SettingValue", dgvSettings.CurrentRow.Index + 1].Value = userId;
                //                }
                //            }
                //            else
                //            {
                //                dgvSettings.CurrentRow.Cells["SettingValue"].Value = " ";
                //                dgvSettings["SettingValue", dgvSettings.CurrentRow.Index + 1].Value = " ";

                //            }
                //            break;

                //        }
                //    }


                //}
                #endregion OldTransaction
                #region Printer Setup

                //if (SettingGroup.ToLower() == "printer" && SettingName.ToLower() == "defaultprinter")
                //{
                //    PrintDialog printDlg = new PrintDialog();
                //    string printer = "";
                //    dgvSettings.CurrentRow.ReadOnly = true;
                //    //Call ShowDialog
                //    if (printDlg.ShowDialog() == DialogResult.OK)
                //    {
                //        printer = printDlg.PrinterSettings.PrinterName.ToString();
                //        dgvSettings.CurrentRow.Cells["SettingValue"].Value = printer;
                //    }

                //}
                #endregion Printer Setup



            }
        }



        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ChangeData = false;
            GetLoad();
            //ChangeData = true;
            //Thread.Sleep(100000);
            //btnUpdate_Click(sender, e);

        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                UserLoad();
            }
        }

        private void UserLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select UserName,UserID,FullName from UserInformations
 where 1=1 and ActiveStatus='Y' ";

                string SQLTextRecordCount = @" select count(UserID)RecordNo from UserInformations";

                string[] shortColumnName = { "UserName", "UserID", "FullName" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);


                //string[] shortColumnName = {  "VehicleNo", "VehicleType"};
                //string tableName = "Vehicles";
                //selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtUserID.Text = selectedRow.Cells["UserID"].Value.ToString();//products.VehicleNo;
                    txtUserName.Text = selectedRow.Cells["UserName"].Value.ToString();//products.VehicleType;
                }
                GetLoad();
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "VehicleLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUserName_DoubleClick(object sender, EventArgs e)
        {
            UserLoad();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower().Replace(" ", "");

            var tempCodeVms = settingsVMList.Where(x =>
                      x.SettingGroup.ToLower().Contains(search)
                      || x.SettingName.ToLower().Contains(search)
                      || x.SettingValue.ToLower().Contains(search)

                      ).ToList();
            DataGridLoad(tempCodeVms);
        }

        private void btnAvgPrice_Click(object sender, EventArgs e)
        {
            try
            {
                FormIssueMultiple issueMultiple = new FormIssueMultiple();
                issueMultiple.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                #region Background Worker for Stock Update

                progressBar1.Visible = true;
                bgwStockUpdate.RunWorkerAsync();
                this.Enabled = false;

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }



    }
}
