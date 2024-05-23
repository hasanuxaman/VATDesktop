using SymphonySofttech.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormBranchReport : Form
    {
        public FormBranchReport()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string NextID;
        private bool IsSymphonyUser = false;
        private string[] sqlResults;
        private bool ChangeData = false;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DataTable BrReportResult;
        private DataSet companyList;
        private List<CmpanyListVM> companies = new List<CmpanyListVM>();
        private BranchReportVM VMLoad()
        {
            BranchReportVM vm = new BranchReportVM();
            vm.Id = Convert.ToInt32(NextID);
            vm.Name = cmbName.Text.Trim();
            vm.IsHeadOffice = chkHeadOffice.Checked;
            vm.IsSelf = chkSelf.Checked;
          

            vm.DBName = companies.Where(x => x.CompanyName.ToString().ToLower() == vm.Name.ToLower()).FirstOrDefault().DatabaseName;
            return vm;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
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
                btnAdd.Enabled = false;
                progressBar1.Visible = true;

                BranchReportDAL _dal = new BranchReportDAL();
                BranchReportVM  brReportVM = new BranchReportVM();
                brReportVM = VMLoad();
                sqlResults = _dal.Insert(brReportVM,connVM);
                //sqlResults = _dal.Insert(NextID.ToString(), txtName.Text.Trim(), txtDBName.Text.Trim());

                  if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                             
                        }
                       ChangeData = false;
                    }
                  searchDT();
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
            #endregion

             finally
            {
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
                ChangeData = false;
            }
        }
        private int ErrorReturn()
        {
            #region try

            if (string.IsNullOrEmpty(cmbName.Text.Trim()))
                {
                    MessageBox.Show("Please enter Branch Name.");
                    cmbName.Focus();
                    return 1;
                }
               
                
           
            #endregion
            return 0;
        }
        private void FormMaker()
        {
            try
            {
                companyList = new DataSet();
                CommonDAL commonDal = new CommonDAL();
                Program.successLogin = false;

                companyList = commonDal.CompanyList("mXSJfsAdbf0=",connVM); //Y="mXSJfsAdbf0=";
                cmbName.Items.Clear();
                companies.Clear();
                if (companyList.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no Company to select", this.Text);
                    return;
                }
                foreach (DataRow item2 in companyList.Tables[0].Rows)
                {
                    //var tt1 = Converter.DESEncrypt(PassPhrase, EnKey, item2["CompanySl"].ToString());
                    //var tt2 = Converter.DESEncrypt(PassPhrase, EnKey,item2["CompanyID"].ToString());
                    //var tt3 = Converter.DESEncrypt(PassPhrase, EnKey,item2["CompanyName"].ToString());
                    //var tt4 = Converter.DESEncrypt(PassPhrase, EnKey,item2["DatabaseName"].ToString());
                    //var tt5 = Converter.DESEncrypt(PassPhrase, EnKey,item2["ActiveStatus"].ToString());
                    //var tt6 = Converter.DESEncrypt(PassPhrase, EnKey,item2["Serial"].ToString());

                    var company = new CmpanyListVM();
                    company.CompanySl = item2["CompanySl"].ToString();
                    company.CompanyID = Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyID"].ToString());
                    company.CompanyName = Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyName"].ToString());
                    company.DatabaseName = Converter.DESDecrypt(PassPhrase, EnKey, item2["DatabaseName"].ToString());
                    company.ActiveStatus = Converter.DESDecrypt(PassPhrase, EnKey, item2["ActiveStatus"].ToString());
                    company.Serial = item2["Serial"].ToString();

                    cmbName.Items.Add(Converter.DESDecrypt(PassPhrase, EnKey, item2["CompanyName"].ToString()));
                    companies.Add(company);
                }


            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Exception occured.\nMessage: {0}", exc.Message));
            }
        }

        private void FormBranchReport_Load(object sender, EventArgs e)
        {
            if (DialogResult.No != MessageBox.Show("Are you Symphony user?", this.Text, MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question,
                                                           MessageBoxDefaultButton.Button2))
            {
                IsSymphonyUser = FormSupperAdministrator.SelectOne();
                if (!IsSymphonyUser)
                {
                    btnAdd.Enabled = false;
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;

                }
            }
            else
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            FormMaker();
            searchDT();
        }
        private void searchDT()
        {
            try
            {
                BranchReportDAL _dal = new BranchReportDAL();
                BrReportResult = _dal.SearchBranchReport(txtName.Text.Trim(),connVM);

                dgvBranchHistory.Rows.Clear();
                int j = 0;
                foreach (DataRow item in BrReportResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvBranchHistory.Rows.Add(NewRow);

                    dgvBranchHistory.Rows[j].Cells["BranchId"].Value = item["Id"].ToString();
                    dgvBranchHistory.Rows[j].Cells["BranchName"].Value = item["Name"].ToString();
                    dgvBranchHistory.Rows[j].Cells["DBName"].Value = item["DBName"].ToString();
                    dgvBranchHistory.Rows[j].Cells["IsSelf"].Value = item["IsSelf"].ToString();
                    dgvBranchHistory.Rows[j].Cells["IsHeadOffice"].Value = item["IsHeadOffice"].ToString();
                    j = j + 1;
                }
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "searchDT", exMessage);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            #endregion
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

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
                BranchReportDAL _dal = new BranchReportDAL();
                 BranchReportVM  brReportVM = new BranchReportVM();
                brReportVM = VMLoad();
                sqlResults = _dal.Update(brReportVM,connVM);
                //sqlResults = _dal.Update(NextID.ToString(), txtName.Text.Trim(), txtDBName.Text.Trim());

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("", "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                      
                    }
                    ChangeData = false;
                }
                searchDT();

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
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
                ChangeData = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region initial check & confirmation

                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }


                if (MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    this.btnDelete.Enabled = true;
                    this.progressBar1.Visible = false;
                    return;
                }

                #endregion
                int ErR = 0;
                //int ErR = DataAlreadyUsed();
                if (ErR != 0)
                {
                    return;
                }
                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;

                //backgroundWorkerDelete.RunWorkerAsync();
                BranchReportDAL _dal = new BranchReportDAL();
                sqlResults = _dal.Delete(OrdinaryVATDesktop.SanitizeInput(cmbName.Text.Trim()),connVM);

                  if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string recId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            ClearAll();
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                  searchDT();

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
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }
            #endregion
            finally
            {
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
                ChangeData = false;
            }
        }
        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearAll();
            ChangeData = false;
        }
        private void ClearAll()
        {
            cmbName.SelectedIndex = 0;

        }


        private void txtAdjName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAdjName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormBranchReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region Try
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
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormBranchReport_FormClosing", exMessage);
            }
            #endregion
        }

        private void dgvBranchHistory_DoubleClick(object sender, EventArgs e)
        {
            cmbName.Text = dgvBranchHistory.CurrentRow.Cells["BranchName"].Value.ToString();
            cmbName.Text = dgvBranchHistory.CurrentRow.Cells["IsSelf"].Value.ToString();
            chkSelf.Checked = Convert.ToBoolean(dgvBranchHistory.CurrentRow.Cells["IsSelf"].Value.ToString() == "Y" ? true : false);
            chkHeadOffice.Checked = Convert.ToBoolean(dgvBranchHistory.CurrentRow.Cells["IsHeadOffice"].Value.ToString() == "Y" ? true : false);

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            searchDT();
        }
    }
}
