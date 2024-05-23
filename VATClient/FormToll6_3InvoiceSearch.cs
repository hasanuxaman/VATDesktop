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
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;
namespace VATClient
{
    public partial class FormToll6_3InvoiceSearch : Form
    {

        #region Constructors

        public FormToll6_3InvoiceSearch()
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

        private static string transactionType = string.Empty;

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private string p = "";

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private string SelectedValue = string.Empty;
        ////private static string transactionType { get; set; }
        //public string VFIN = "303";
        private string Type = string.Empty;
        private string TradingItem = string.Empty;
        private string Print = string.Empty;
        private string post = string.Empty;
        private DataTable SaleResult;
        string[] IdArray;

        private string TollFromDate, TollToDate;
        private int SearchBranchId = 0;

        public int SenderBranchId = 0;

        #endregion

        public static DataGridViewRow SelectOne(string tType = "")
        {
            // transactionType = type;

            DataGridViewRow selectedRowTemp = null;

            if (tType == "All")
            {
                transactionType = "All";
            }
            else
            {
                transactionType = tType;
            }

            try
            {
                #region Statement


                FormToll6_3InvoiceSearch frmToll6_3InvoiceSearch = new FormToll6_3InvoiceSearch();

                if (tType == "Contractor63")
                {
                    frmToll6_3InvoiceSearch.Text = "Toll6_3 Invoice Search";
                }
                else if (tType == "Client63")
                {
                    frmToll6_3InvoiceSearch.Text = "6.3 Receive Search";
                }

                frmToll6_3InvoiceSearch.ShowDialog();
                selectedRowTemp = frmToll6_3InvoiceSearch.selectedRow;

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        private void FormSaleSearch_Load(object sender, EventArgs e)
        {
            try
            {

                #region Statement

                if (transactionType == "Client63")
                {
                    label2.Text = "Vendor Name";
                }

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                cmbBranch.SelectedValue = Program.BranchId;


                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormSaleSearch_Load", exMessage);
            }
            #endregion

        }

        private void NullCheck()
        {
            try
            {
                #region Statement

                if (dtpTollFromDate.Checked == false)
                {
                    TollFromDate = "1900-01-01";
                    //SalesFromDate = Convert.ToDateTime(dtpSaleFromDate.MinDate.ToString("yyyy-MMM-dd"));
                }
                else
                {
                    TollFromDate = dtpTollFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpTollToDate.Checked == false)
                {
                    TollToDate = "9000-12-31";
                    //SalesToDate = Convert.ToDateTime(dtpSaleToDate.MaxDate.ToString("yyyy-MMM-dd"));
                }
                else
                {
                    TollToDate = dtpTollToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "NullCheck", exMessage);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion

        }

        public void ClearAllFields()
        {
            txtCustomerID.Text = "";
            txtTollNo.Text = "";
            dtpTollFromDate.Text = "";
            dtpTollToDate.Text = "";
            txtCustomerName.Text = "";
            txtVehicleType.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void GridSeleted()
        {
            try
            {
                #region Statement


                if (dgvSalesHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvSalesHistory.SelectedRows;
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

        private void dgvSalesHistory_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                GridSeleted();

                #endregion

            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvSalesHistory_DoubleClick", exMessage);
            }

            #endregion

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);
            SearchDT();
        }


        private void SearchDT()
        {
            try
            {
                //////this.btnSearch.Enabled = false;
                //////this.progressBar1.Visible = true;

                //////NullCheck();

                string[] cValues=null;
                string[] cFields=null;

                post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : string.Empty;

                cValues = new string[] {  txtTollNo.Text.Trim(), 
                    txtCustomerName.Text.Trim(),
                    TollFromDate,
                    TollToDate, 
                    post, 
                   SearchBranchId.ToString()
                   
                     };

                if (transactionType == "Client63")
                {
                    cFields = new string[] { "ti.TollNo like",
                    "c.VendorName like",
                    "ti.TollDateTime>",
                    "ti.TollDateTime<",
                    "ti.Post like",
                    "ti.BranchId"
                    
                       };

                }
                else
                {

                     cFields = new string[] { "ti.TollNo like",
                    "c.CustomerName like",
                    "ti.TollDateTime>",
                    "ti.TollDateTime<",
                    "ti.Post like",
                    "ti.BranchId"
                    
                       };

                    //string[] cValues = { ""};
                    //string[] cFields = {"" };

                    //Toll6_3InvoiceDAL _Toll6_3InvoiceDAL = new Toll6_3InvoiceDAL();

                }
                IToll6_3Invoice _Toll6_3InvoiceDAL = OrdinaryVATDesktop.GetObject<Toll6_3InvoiceDAL, Toll6_3InvoiceRepo, IToll6_3Invoice>(OrdinaryVATDesktop.IsWCF);

                SaleResult = _Toll6_3InvoiceDAL.SelectAll(0, cFields, cValues, null, null, true, connVM, transactionType);

                #region DGV
                int j = 0;
                dgvSalesHistory.Rows.Clear();

                foreach (DataRow dr in SaleResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSalesHistory.Rows.Add(NewRow);
                    dgvSalesHistory.Rows[j].Cells["ID"].Value = dr["Id"].ToString();
                    dgvSalesHistory.Rows[j].Cells["TollNo"].Value = dr["TollNo"].ToString();
                    dgvSalesHistory.Rows[j].Cells["CustomerID"].Value = dr["CustomerID"].ToString();
                    if (transactionType == "Client63")
                    {
                        dgvSalesHistory.Rows[j].Cells["CustomerName"].Value = dr["VendorName"].ToString();
                    }
                    else
                    {
                        dgvSalesHistory.Rows[j].Cells["CustomerName"].Value = dr["CustomerName"].ToString();
                    }

                    dgvSalesHistory.Rows[j].Cells["Address"].Value = dr["Address"].ToString();
                    dgvSalesHistory.Rows[j].Cells["TollDateTime"].Value = dr["TollDateTime"].ToString();
                    dgvSalesHistory.Rows[j].Cells["PostStatus"].Value = dr["Post"].ToString();
                    dgvSalesHistory.Rows[j].Cells["Comments"].Value = dr["Comments"].ToString();
                    dgvSalesHistory.Rows[j].Cells["BranchId"].Value = dr["BranchId"].ToString();

                    j = j + 1;

                }
                #endregion

            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDT", exMessage);
            }

            #endregion


        }


        private void btnPost_Click(object sender, EventArgs e)
        {
            int j = 0;
            string ids = "";
            for (int i = 0; i < dgvSalesHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvSalesHistory["Select", i].Value) == true)
                {
                    if (dgvSalesHistory["Post", i].Value.ToString() == "N")
                    {
                        string Id = dgvSalesHistory["ID", i].Value.ToString();
                        ids += Id + "~";
                    }
                }
            }
            IdArray = ids.Split('~');
            if (IdArray.Length <= 1)
            {
                MessageBox.Show(this, "All data already posted !");
                return;
            }
            bgwMultiplePost.RunWorkerAsync();
        }

        private void bgwMultiplePost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (IdArray == null || IdArray.Length <= 0)
                {
                    return;
                }
                else if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                //SaleDAL _salDal = new SaleDAL();
                ISale _salDal = OrdinaryVATDesktop.GetObject<SaleDAL, SaleRepo, ISale>(OrdinaryVATDesktop.IsWCF);

                //progressBar1.Visible = true;
                string[] results = _salDal.MultiplePost(IdArray, connVM);
                if (results[0] == "Success")
                {
                    MessageBox.Show("All Toll posted successfully");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwMultiplePost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchDT();
        }


        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();

        }
        private void SelectAll()
        {
            for (int i = 0; i < dgvSalesHistory.RowCount; i++)
            {
                dgvSalesHistory["Select", i].Value = chkSelectAll.Checked;
            }

        }


    }
}