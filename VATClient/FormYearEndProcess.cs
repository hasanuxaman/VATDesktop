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
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormYearEndProcess : Form
    {

        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool ChangeData = false;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private bool _isExit = false;
        private DataTable dt;
        private DataSet companyList;
        private DataTable dtResults;
        private List<CmpanyListVM> companies = new List<CmpanyListVM>();
        private static string EnKey = DBConstant.EnKey;

        string DatabaseName;

        #endregion


        public FormYearEndProcess()
        {
            InitializeComponent();
        }

        private void FormDayEndProcess_Load(object sender, EventArgs e)
        {

            #region try

            try
            {
                btnLoad.Enabled = false;
                btnExport.Enabled = false;

                bgwCompanyList.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "FormDayEndProcess_Load", exMessage);
            }
            #endregion


        }

        private void bgwCompanyList_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            
            try
            {
                companyList = new DataSet();
                CommonDAL commonDal = new CommonDAL();
                DataSet CompanyDs = new DataSet();

                Program.successLogin = false;

                string CompanyList = "All";
                //try
                //{
                //    CompanyDs = commonDal.SettingInformation();

                //    if (CompanyDs != null && CompanyDs.Tables[0].Rows.Count > 0)
                //    {
                //        CompanyList = CompanyDs.Tables[0].Rows[0]["CompanyList"].ToString();
                //    }
                   
                //}
                //catch (Exception ex)
                //{
                //    CompanyList = "All";
                //}

                companyList = commonDal.CompanyList("mXSJfsAdbf0=", null, CompanyList);

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
                FileLogger.Log(this.Name, "bgwCompanyList_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwCompanyList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            
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
                FileLogger.Log(this.Name, "bgwCompanyList_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                
                btnLoad.Enabled = true;
                btnExport.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                btnLoad.Enabled = false;
                btnExport.Enabled = false;
                progressBar1.Visible = true;

                DatabaseName = "";

                BDName();

                bgwDataLoad.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnLoad_Click", exMessage);
            }
            #endregion
        }

        private void bgwDataLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                string currentDatabase = DatabaseInfoVM.DatabaseName;

                DatabaseInfoVM.DatabaseName = DatabaseName;

                YearEndProcessDAL _dal = new YearEndProcessDAL();

                dtResults = _dal.SelectAllData();

                DatabaseInfoVM.DatabaseName = currentDatabase;

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
                FileLogger.Log(this.Name, "bgwDataLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwDataLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region try

            try
            {
                #region Statement

                dgvProduct.DataSource = null;

                if (dtResults != null && dtResults.Rows.Count > 0)
                {
                    dgvProduct.DataSource = dtResults;
                    dgvProduct.Columns["BranchId"].Visible = false;
                    dgvProduct.Columns["ItemNo"].Visible = false;

                }

                #endregion

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
                FileLogger.Log(this.Name, "bgwDataLoad_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {

                btnLoad.Enabled = true;
                btnExport.Enabled = true;
                progressBar1.Visible = false;

                LRecordCount.Text = "Total Record Count " + dgvProduct.Rows.Count.ToString();

            }
        }

        private void BDName()
        {
            #region try
            try
            {
                //////DatabaseInfoVM.DatabaseName = string.Empty;
                //////Program.CompanyID = string.Empty;

                var searchText = cmbCompany.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    var company = from prd in companies.ToList()
                                  where prd.CompanyName.ToLower() == searchText.ToLower()
                                  select prd;
                    if (company != null && company.Any())
                    {
                        var comp = company.First();
                        DatabaseName = comp.DatabaseName;
                        //////Program.CompanyID = comp.CompanyID;
                       
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
                FileLogger.Log(this.Name, "BDName", exMessage);
            }
            #endregion

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            #region try

            try
            {
                btnLoad.Enabled = false;
                btnExport.Enabled = false;
                progressBar1.Visible = true;

                DatabaseName = "";

                BDName();

                bgwDownload.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnExport_Click", exMessage);
            }
            #endregion

        }

        private void bgwDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                string currentDatabase = DatabaseInfoVM.DatabaseName;

                DatabaseInfoVM.DatabaseName = DatabaseName;

                YearEndProcessDAL _dal = new YearEndProcessDAL();

                dtResults = _dal.SelectAllData();

                DatabaseInfoVM.DatabaseName = currentDatabase;

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
                FileLogger.Log(this.Name, "bgwDownload_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region try

            try
            {
                #region Statement

                if (dtResults.Rows.Count == 0)
                {
                    MessageBox.Show("There is no data found");
                    return;
                }
                dtResults.Columns.Remove("BranchId");
                dtResults.Columns.Remove("ItemNo");

                OrdinaryVATDesktop.SaveExcel(dtResults, "YearEndProcess", "Permanent_Branch");

                #endregion

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
                FileLogger.Log(this.Name, "bgwDownload_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {

                btnLoad.Enabled = true;
                btnExport.Enabled = true;
                progressBar1.Visible = false;

                LRecordCount.Text = "Total Record Count " + dgvProduct.Rows.Count.ToString();

            }
            #endregion

        }





    }
}
