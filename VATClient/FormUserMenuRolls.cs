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
using VATServer.Library;
using VATViewModel.DTOs;
using System.Threading;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormUserRolls : Form
    {
        #region Constructors

        public FormUserRolls()
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
        private bool ChangeData = false;
        string FolderPath = "";
        private DataTable SettingResult;
        private string SearchUserId = "0";

        List<UserMenuSettingsVM> UserMenuSetting = new List<UserMenuSettingsVM>();
        private string preSettingValue = string.Empty;
        private bool IsSerialTracking = false;

        #region sql save, update, delete

        private SettingDAL settingDal = new SettingDAL();
        private string[] sqlResults;
        private string sqlResultssettings;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods





        #endregion

        private void FormSetting_Load(object sender, EventArgs e)
        {
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbUserName.Items.Clear();
            cmbUserName = new CommonDAL().ComboBoxLoad(cmbUserName, "UserInformations", "UserID", "UserName", Condition, "varchar", true, false,connVM);
            cmbUserName.Text = Program.CurrentUser;
            SearchUserId = Program.CurrentUserID;
            bgwUserMenuAllRolls.RunWorkerAsync();
            
        }

     
        
        private void bgwUserMenuAllRolls_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {
                UPDATE_DOWORK_SUCCESS = false;
                sqlResultssettings = string.Empty;
                UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();
                UserMenuRollsDal.UserMenuRollsInsertByUser(SearchUserId, null, null, connVM);
                SettingResult = UserMenuRollsDal.UserMenuRollsSelectAll(SearchUserId, null, null, null, null, false, connVM);
                UPDATE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUserMenuAllRolls_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try statement
            try
            {
                if (UPDATE_DOWORK_SUCCESS)
                {
                    dgvSettings.DataSource = null;
                    dgvSettings.Rows.Clear();
                    dgvSettings.DataSource = SettingResult;
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUserMenuAllRolls_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }
        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try Statement
            try
            {
                
                SettingDAL settingDal = new SettingDAL();
                    //ISetting settingDal = OrdinaryVATDesktop.GetObject<SettingDAL, SettingRepo, ISetting>(OrdinaryVATDesktop.IsWCF);
                SettingResult = settingDal.SearchUserMenuAllRolls(connVM);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                
              

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
            }
        }
        private void dgvSettings_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (dgvSettings.Rows.Count > 0)
                {
                    var SettingFormName = dgvSettings.CurrentRow.Cells["FormName"].Value.ToString();
                    //var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                    SettingFormName = SettingFormName.Replace(" ", "");
                    //SettingName = SettingName.Replace(" ", "");

                    //if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "vat11savelocation")
                    //{
                    //    dgvSettings.CurrentRow.ReadOnly = true;
                    //    return;
                    //}
                }

                //getting SettingValue from datagrid by rowindex and columindex
                string settingValueExiting = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                preSettingValue = string.Empty;
                preSettingValue = settingValueExiting;

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
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", exMessage);
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
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvSettings_CellBeginEdit", exMessage);
            }
            #endregion
        }

        private void dgvSettings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region try

            try
            {
                //#region Maximum copy count


                //if (dgvSettings.Rows.Count > 0)
                //{
                //    var SettingFormName = dgvSettings.CurrentRow.Cells["FormName"].Value.ToString();
                //    //var SettingName = dgvSettings.CurrentRow.Cells["SettingName"].Value.ToString();
                //    SettingFormName = SettingFormName.Replace(" ", "");
                   

                //    if (SettingGroup.ToLower() == "printer" && SettingName.ToLower() == "maxnoofprint")
                //    {
                //        var SettingValue = dgvSettings.CurrentRow.Cells["SettingValue"].Value.ToString();
                //        if (Convert.ToInt32(SettingValue) > 100)
                //        {
                //            dgvSettings["SettingValue", dgvSettings.CurrentRow.Index].Value = "100";
                //        }
                //    }
                //    #region Serial Tracking


                //    if (SettingGroup.ToLower() == "trackingtrace" && SettingName.ToLower() == "tracking")
                //    {
                //        DisplaySerialTrackingRows();
                //    }

                //    #endregion Serial Tracking

                //}
                //#endregion Maximum copy count

                //getting SettingValue from datagrid by rowindex and columindex
                string settingValue = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                //getting SettingType from datagrid by rowindex and columindex
                string settingType = dgvSettings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

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
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", exMessage);
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
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvSettings_CellEndEdit", exMessage);
            }
            #endregion
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try statement
            try
            {
                this.progressBar1.Visible = true;
                this.btnUpdate.Enabled = false;

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
                UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();        
                DataTable dt = (DataTable)dgvSettings.DataSource;
              dt=  OrdinaryVATDesktop.DtNullCheck(dt, new string[] { "Access", "PostAccess", "AddAccess", "EditAccess" }, "0");
              sqlResults = UserMenuRollsDal.UserMenuRollsUpdate(dt, null, null, connVM);

              


                UPDATE_DOWORK_SUCCESS = true;
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSettings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSettings.Rows.Count > 0)
            {
                var SettingFormName = dgvSettings.CurrentRow.Cells["FormName"].Value.ToString();
                SettingFormName = SettingFormName.Replace(" ", "");
                //SettingName = SettingName.Replace(" ", "");

                //if (SettingGroup.ToLower() == "sale" && SettingName.ToLower() == "vat11savelocation")
                //{
                //    dgvSettings.CurrentRow.ReadOnly = true;
                //    return;
                //}

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
                //#region Printer Setup

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
                //#endregion Printer Setup
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ChangeData = false;
           
            //ChangeData = true;
            //Thread.Sleep(100000);
            //btnUpdate_Click(sender, e);

        }
     

        private void dgvSettings_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(dgvSettings.SelectedCells[0].Value.ToString());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();
            SearchUserId =  cmbUserName.SelectedValue.ToString();

            string[] cFields = new string[] { "FormName like", "UserID" };
            string[] cValues = new string[] { txtSearch.Text.Trim(), SearchUserId.ToString() };

            SettingResult = UserMenuRollsDal.UserMenuRollsSelectAll(null, cFields, cValues, null, null, false, connVM);
            //dgvSettings.Rows.Clear();
            dgvSettings.DataSource = SettingResult;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchUserId = cmbUserName.SelectedValue.ToString();

            bgwUserMenuAllRolls.RunWorkerAsync();
        }

        private void chkAccess_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSettings.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["Access"];
                chk.Value = chkAccess.Checked ? "1" : "0"; //because chk.Value is initialy null
            }
        }

        private void chkPost_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSettings.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["PostAccess"];
                chk.Value = chkPost.Checked ? "1" : "0"; //because chk.Value is initialy null
            }
        }

        private void chkAdd_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSettings.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["AddAccess"];
                chk.Value = chkAdd.Checked ? "1" : "0"; //because chk.Value is initialy null
            }
        }

        private void chkEdit_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSettings.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["EditAccess"];
                chk.Value = chkEdit.Checked ? "1" : "0"; //because chk.Value is initialy null
            }
            
        }

        private void chkPost_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAll_Click(object sender, EventArgs e)
        {
            if(chkAll.Checked==true)
            {
                btnSearch.Enabled=false;
                button1.Enabled = false;
                btnUpdate.Enabled = false;
                btnExport.Visible = true;
                btnImport.Visible = true;

            }
            else
            {
                btnSearch.Enabled = true;
                button1.Enabled = true;
                btnUpdate.Enabled = true;
                btnExport.Visible=false;
                btnImport.Visible = false;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
             try
            {

                UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();


                DataTable dt = UserMenuRollsDal.GetExcelUserMenuRolls(null, null, null, null, null, false,connVM);
                OrdinaryVATDesktop.SaveExcel(dt, "UserMenuRollsSettings", "UserMenuRollsSettings");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                Program.ImportFileName = null;

                if (string.IsNullOrWhiteSpace(Program.ImportFileName))
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show(this, "Please select the right file for import");
                }
                else
                {

                    string[] extention = fileName.Split(".".ToCharArray());
                    if (extention[extention.Length - 1] == "xls" || extention[extention.Length - 1] == "xlsx")
                    {
                        ds = loadExcel();

                        dt = ds.Tables["UserMenuRollsSettings"];
                        //dgvUserGrid.DataSource = dt;

                        Program.ImportFileName = null;

                        //UserBranchDetailDAL dal = new UserBranchDetailDAL();
                        //IUserBranchDetail dal = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string UserName = dt.Rows[i]["UserName"].ToString();
                            UserMenuSettingsVM vm = new UserMenuSettingsVM();
                            var UserInformation = new UserInformationDAL().SelectAll(0, new[] { "UserName", }, new[] { UserName },null,null,connVM).FirstOrDefault();
                            if (UserInformation==null)
                            {

                                MessageBox.Show("Could Not Find '" + UserName + "' In UserInformation Table", "UserMenuImport", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            vm.FormID = dt.Rows[i]["FormID"].ToString();
                            vm.UserID = UserInformation.UserID;
                            vm.Access = dt.Rows[i]["Access"].ToString();
                            vm.FormName = dt.Rows[i]["FormName"].ToString();
                            vm.AddAccess = dt.Rows[i]["AddAccess"].ToString();
                            vm.EditAccess = dt.Rows[i]["EditAccess"].ToString();
                            vm.PostAccess = dt.Rows[i]["PostAccess"].ToString();
                            vm.CreatedBy = Program.CurrentUser;
                            vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            vm.LastModifiedBy = Program.CurrentUser;
                            vm.LastModifiedOn=DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            UserMenuSetting.Add(vm);
                        }

                        UserMenuRollsDAL dal = new UserMenuRollsDAL();
                        sqlResults = dal.ImportUserMenuRolls(UserMenuSetting, connVM);

                        if (sqlResults[0] == "Success")
                        {
                            MessageBox.Show("Data Import successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //dtUserBranchDetailResult=dal.SelectAll()
                        }
                    }

                    else
                    {
                        MessageBox.Show("You can select Excel files only");
                    }


                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private DataSet loadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                IImport dal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);

                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                ds = dal.GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            return ds;
        }

        private void BrowsFile()
        {
            #region try
            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "User Information Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }
            }
            #endregion
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }


        }

    }
}
