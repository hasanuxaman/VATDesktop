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
//
using VATClient.ModelDTO;
//
using System.IO;
using System.Security.Cryptography;
using SymphonySofttech.Utilities;
using System.Data.SqlClient;
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormDisposeFinishSearch : Form
    {
        #region Constructors

        public FormDisposeFinishSearch()
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
        List<DisposeItemsDTO> DisposeItems = new List<DisposeItemsDTO>();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string startdate, enddate;
        private static string transactionType { get; set; }
        private string SelectedValue = string.Empty;
        public string VFIN = "307";
        private int searchBranchId = 0;
        #region Global Variables For BackGroundWorker


        // Datatable instant CustomerGroupResult
        private DataTable DisposeTypeResult;
        //string activeStatus = string.Empty;
        string TypeData = string.Empty;
        private string cmbPostText;


        #endregion

        #endregion

        public static string SelectOne(string tType)
        {

            string frmSearchSelectValue = String.Empty;

            try
            {
                FormDisposeFinishSearch frmSaleSearch = new FormDisposeFinishSearch();
                transactionType = tType;
                frmSaleSearch.ShowDialog();
                return frmSearchSelectValue = frmSaleSearch.SelectedValue;

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDisposeFinishSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDisposeFinishSearch", "SelectOne", exMessage);
            }
            #endregion

            return frmSearchSelectValue;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void Null()
        {
            try
            {
                if (dtpPostFrom.Checked)
                {
                    startdate = dtpPostFrom.Value.ToString("yyyy-MMM-dd");
                }
                else
                {
                    startdate = dtpPostFrom.MinDate.ToString("yyyy-MMM-dd");
                }
                if (dtpPostTo.Checked)
                {
                    enddate = dtpPostTo.Value.ToString("yyyy-MMM-dd");
                }
                else
                {
                    enddate = dtpPostTo.MaxDate.ToString("yyyy-MMM-dd");
                }
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Null", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Null", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Null", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Null", exMessage);
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
                FileLogger.Log(this.Name, "Null", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Null", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Null", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Null", exMessage);
            }
            #endregion
        }

        private void FormDisposeFinishSearch_Load(object sender, EventArgs e)
        {

            CommonDAL dal = new CommonDAL();
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
            cmbBranch.SelectedValue = Program.BranchId;
            //Search();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            txtDisposeNumber.Text = "";
            dtpPostFrom.Checked = false;
            dtpPostTo.Checked = false;
            dgvDFI.DataSource = null;
            cmbPost.SelectedIndex = -1;
            //dgvDFI.Rows.Clear();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void dgvDFI_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void GridSeleted()
        {
            try
            {
                if (dgvDFI.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (dgvDFI.Rows.Count > 0)
                {
                    string IssueInfo = string.Empty;
                    int RowIndex1 = dgvDFI.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        string DisposeNumber = dgvDFI.Rows[RowIndex1].Cells["DisposeNumber"].Value.ToString();
                        string DisposeDate = dgvDFI.Rows[RowIndex1].Cells["DisposeDate"].Value.ToString();
                        string RefNumber = dgvDFI.Rows[RowIndex1].Cells["RefNumber"].Value.ToString();
                        string Remarks = dgvDFI.Rows[RowIndex1].Cells["Remarks"].Value.ToString();
                        string Post = dgvDFI.Rows[RowIndex1].Cells["Post"].Value.ToString();
                        string AppTotalPrice = dgvDFI.Rows[RowIndex1].Cells["AppTotalPrice"].Value.ToString();
                        string AppVATAmount = dgvDFI.Rows[RowIndex1].Cells["AppVATAmount"].Value.ToString();
                        string AppDate = dgvDFI.Rows[RowIndex1].Cells["AppDate"].Value.ToString();
                        string AppRefNumber = dgvDFI.Rows[RowIndex1].Cells["AppRefNumber"].Value.ToString();
                        string AppRemarks = dgvDFI.Rows[RowIndex1].Cells["AppRemarks"].Value.ToString();
                        string AppVATAmountImport = dgvDFI.Rows[RowIndex1].Cells["AppVATAmountImport"].Value.ToString();
                        string AppTotalPriceImport = dgvDFI.Rows[RowIndex1].Cells["AppTotalPriceImport"].Value.ToString();
                        string searchBranchId = dgvDFI.Rows[RowIndex1].Cells["BranchId"].Value.ToString();

                       
                        IssueInfo =
                            DisposeNumber + FieldDelimeter + DisposeDate + FieldDelimeter + RefNumber + FieldDelimeter +
                            Remarks + FieldDelimeter + Post + FieldDelimeter + AppTotalPrice + FieldDelimeter +
                            AppVATAmount + FieldDelimeter + AppDate + FieldDelimeter + AppRefNumber + FieldDelimeter +
                            AppRemarks + FieldDelimeter + AppVATAmountImport + FieldDelimeter + AppTotalPriceImport + searchBranchId + FieldDelimeter;
                        SelectedValue = IssueInfo;
                    }
                }
                this.Close();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSeleted", exMessage);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            #endregion
        }

        // == Search == //
        #region // == Search == //

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.btnSearch.Enabled = false;
            this.progressBar1.Visible = true;
            searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);
            Search();
        }

        private void Search()
        {
            this.btnSearch.Enabled = false;
            this.progressBar1.Visible = true;

            #region try
            try
            {
                Null();
                cmbPostText = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";

                //TypeData =
                //    txtDisposeNumber.Text.Trim() + FieldDelimeter +
                //    startdate + FieldDelimeter +
                //    enddate + FieldDelimeter +
                //    transactionType + FieldDelimeter +
                //    cmbPost.Text.Trim()
                //    + LineDelimeter;
                backgroundWorkerSearch.RunWorkerAsync();

                #region Start
                //string encriptedTypeData = Converter.DESEncrypt(PassPhrase, EnKey, TypeData);
                //DisposeSoapClient ProductTypeSearch = new DisposeSoapClient();
                //DataTable ProductTypeResult = ProductTypeSearch.SearchHeaderDT(encriptedTypeData.ToString(),
                //                                                               Program.DatabaseName);
                #endregion

                #region Complete
                //dgvDFI.DataSource = null;
                //if (ProductTypeResult != null)
                //{
                //    dgvDFI.DataSource = ProductTypeResult;
                //}
                #endregion
            }
            #endregion

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

        #region  background Worker Search Events

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                //DisposeDAL disposeDal = new DisposeDAL();
                IDispose disposeDal = OrdinaryVATDesktop.GetObject<DisposeDAL, DisposeRepo, IDispose>(OrdinaryVATDesktop.IsWCF);
                DisposeTypeResult = disposeDal.SearchDisposeHeaderDTNew(txtDisposeNumber.Text.Trim()
                    , startdate, enddate, transactionType, cmbPostText, Program.DatabaseName, searchBranchId,connVM);
                // End DoWork

                #endregion
            }
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
            try
            {
                #region Statement

                // Start Complete
                dgvDFI.Rows.Clear();
                int j = 0;
                foreach (DataRow item in DisposeTypeResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvDFI.Rows.Add(NewRow);

                    dgvDFI.Rows[j].Cells["DisposeNumber"].Value = item["DisposeNumber"].ToString();// SaleFields[0].ToString();
                    dgvDFI.Rows[j].Cells["DisposeDate"].Value = item["DisposeDate"].ToString();// SaleFields[1].ToString();
                    dgvDFI.Rows[j].Cells["AppVATAmountImport"].Value = item["AppVATAmountImport"].ToString();// SaleFields[2].ToString();
                    dgvDFI.Rows[j].Cells["AppTotalPriceImport"].Value = item["AppTotalPriceImport"].ToString();// SaleFields[3].ToString();
                    dgvDFI.Rows[j].Cells["AppTotalPrice"].Value = item["AppTotalPrice"].ToString();// SaleFields[4].ToString();
                    dgvDFI.Rows[j].Cells["RefNumber"].Value = item["RefNumber"].ToString();// SaleFields[5].ToString();
                    dgvDFI.Rows[j].Cells["AppVATAmount"].Value = item["AppVATAmount"].ToString();// SaleFields[6].ToString();
                    dgvDFI.Rows[j].Cells["AppDate"].Value = item["AppDate"].ToString();// SaleFields[7].ToString();
                    dgvDFI.Rows[j].Cells["Remarks"].Value = item["Remarks"].ToString();// SaleFields[8].ToString();
                    dgvDFI.Rows[j].Cells["AppRefNumber"].Value = item["AppRefNumber"].ToString();// SaleFields[9].ToString();
                    dgvDFI.Rows[j].Cells["AppRemarks"].Value = item["AppRemarks"].ToString();// SaleFields[10].ToString();
                    dgvDFI.Rows[j].Cells["Post"].Value = item["Post"].ToString();// SaleFields[11].ToString();
                    dgvDFI.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();// SaleFields[11].ToString();

                   
                    j = j + 1;

                }


                // End Complete

                #endregion

                #region Statement

                // Start Complete

                //dgvDFI.DataSource = null;
                //if (DisposeTypeResult != null)
                //{
                //    dgvDFI.DataSource = DisposeTypeResult;
                //}

                // End Complete

                #endregion
            }
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
                LRecordCount.Text = "Record Count: " + dgvDFI.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        #endregion

        private void cmbPost_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void txtDisposeNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void cmbPost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpPostFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpPostTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //=============//

    }
}
