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
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormDutyDrawBackSearch : Form
    {
        public FormDutyDrawBackSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
        #region Global Variable
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string POSTCMB = string.Empty;
        private DataTable ddbackResult;
        string DDBackFromDate, DDBackToDate, DDBackSaleFromDate, DDBackSaleToDate;
        private DataTable CustomerNameResult;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private string SearchBranchId = "0";
       // List<Customer> customer = new List<Customer>();
        private static string transactionType { get; set; }
        #endregion

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            try
            {
                //DutyDrawBackDAL ddDal = new DutyDrawBackDAL();
                IDutyDrawBack ddDal = OrdinaryVATDesktop.GetObject<DutyDrawBackDAL, DutyDrawBackRepo, IDutyDrawBack>(OrdinaryVATDesktop.IsWCF);

                ddbackResult = ddDal.SearchDDBackHeader(txtDDBackNo.Text.Trim(), DDBackFromDate, DDBackToDate, DDBackSaleFromDate, DDBackSaleToDate,
                    txtSalesInvoicNo.Text.Trim(), txtCustomerName.Text.Trim(), txtFinishGood.Text.Trim(), POSTCMB, SearchBranchId, transactionType,connVM);  // Change 04
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            try
            {
                dgvDDBackHistory.Rows.Clear();
                int j = 0;
                if (ddbackResult != null)
                    foreach (DataRow item in ddbackResult.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvDDBackHistory.Rows.Add(NewRow);

                        dgvDDBackHistory.Rows[j].Cells["DDBackNo"].Value = item["DDBackNo"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["DDBackDate"].Value = item["DDBackDate"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["SalesInvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["SalesDate"].Value = item["SalesDate"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["CustormerID"].Value = item["CustormerID"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["CustormerName"].Value = item["CustomerName"].ToString(); 
                        dgvDDBackHistory.Rows[j].Cells["CurrencyId"].Value = item["CurrencyId"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["CurrencyCode"].Value = item["CurrencyCode"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["ExpCurrency"].Value = item["ExpCurrency"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["BDTCurrency"].Value = item["BDTCurrency"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["FgItemNo"].Value = item["FgItemNo"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["FgItemName"].Value = item["ProductName"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimCD"].Value = item["TotalClaimCD"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimRD"].Value = item["TotalClaimRD"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimSD"].Value = item["TotalClaimSD"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalDDBack"].Value = item["TotalDDBack"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimVAT"].Value = item["TotalClaimVAT"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimCnFAmount"].Value = item["TotalClaimCnFAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimInsuranceAmount"].Value = item["TotalClaimInsuranceAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimTVBAmount"].Value = item["TotalClaimTVBAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimTVAAmount"].Value = item["TotalClaimTVAAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimATVAmount"].Value = item["TotalClaimATVAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalClaimOthersAmount"].Value = item["TotalClaimOthersAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["ApprovedSD"].Value = item["ApprovedSD"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["TotalSDAmount"].Value = item["TotalSDAmount"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["CreatedBy"].Value = item["CreatedBy"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["CreatedOn"].Value = item["CreatedOn"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["LastModifiedBy"].Value = item["LastModifiedBy"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["LastModifiedOn"].Value = item["LastModifiedOn"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["Post"].Value = item["Post"].ToString();
                        dgvDDBackHistory.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();



                       
                        
                        j = j + 1;
                    }
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                LRecordCount.Text = "Record Count: " + dgvDDBackHistory.Rows.Count.ToString();
                this.progressBar1.Visible = false;
                this.btnSearch.Enabled = true;
            }

        }



        private void FormDutyDrawBackSearch_Load(object sender, EventArgs e)
        {
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
            cmbBranch.SelectedValue = Program.BranchId;

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            //Search();
        }

        private void Search()
        {
            try
            {
                #region Statement
                POSTCMB = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";

                //transactionType = "Other";
                DtpNullChecking();
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                backgroundWorkerSearch.RunWorkerAsync();

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Search", exMessage);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion
        }

       

        private void DtpNullChecking()
        {

            try
            {
                #region Statement

                if (ddbackFromDate.Checked == false)
                {
                    DDBackFromDate = "";
                }
                else
                {
                    DDBackFromDate = ddbackFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (ddbackToDate.Checked == false)
                {
                    DDBackToDate = "";
                }
                else
                {
                    DDBackToDate = ddbackToDate.Value.ToString("yyyy-MMM-dd");
                }


                if (ddbackSalesFormDate.Checked == false)
                {
                    DDBackSaleFromDate = "";
                }
                else
                {
                    DDBackSaleFromDate = ddbackSalesFormDate.Value.ToString("yyyy-MMM-dd");
                }
                if (ddbackSalesToDate.Checked == false)
                {
                    DDBackSaleToDate = "";
                }
                else
                {
                    DDBackSaleToDate = ddbackSalesToDate.Value.ToString("yyyy-MMM-dd");
                }



                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DtpNullChecking", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
            }
            #endregion

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBranchId = cmbBranch.SelectedValue.ToString();
            Search();

        }

        public static DataGridViewRow SelectOne(string tType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                #region Statement
                transactionType = tType;

                FormDutyDrawBackSearch frmDDBSearch = new FormDutyDrawBackSearch();

                frmDDBSearch.ShowDialog();

                selectedRowTemp = frmDDBSearch.selectedRow;

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDutyDrawBackSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDutyDrawBackSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        private void dgvDDBackHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                GridSelected();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvDDBackHistory_DoubleClick", exMessage);
            }
            #endregion
        }

        private void GridSelected()
        {
            try
            {
                #region Statement

                if (dgvDDBackHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvDDBackHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                this.Close();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSelected", exMessage);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion

        }

    }
}
