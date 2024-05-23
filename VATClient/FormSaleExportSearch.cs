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
    public partial class FormSaleExportSearch : Form
    {
        public FormSaleExportSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName; 
        }
        private static string transactionType { get; set; }
        DataGridViewRow selectedRow = new DataGridViewRow();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable AdjNameResult;
        private int BranchId = 0;
        private string activeStatus;
        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;
            try
            {
                #region Statement

                FormSaleExportSearch frmSaleSearch = new FormSaleExportSearch();

                frmSaleSearch.ShowDialog();
                selectedRowTemp = frmSaleSearch.selectedRow;

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
        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }
        private void GridSeleted()
        {
            try
            {
                #region Statement


                if (dgvHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvHistory.SelectedRows;
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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
            Search();
        }
        private void Search()
        {
            string FromDate,ToDate;
                if (dtpFromDate.Checked == false)
                {
                    FromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    FromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpToDate.Checked == false)
                {
                    ToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    ToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");
                }
               string Post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : string.Empty;
            DataTable dt = new DataTable();
            //SaleExportDAL Dal = new SaleExportDAL();
            ISaleExport Dal = OrdinaryVATDesktop.GetObject<SaleExportDAL, SaleExportRepo, ISaleExport>(OrdinaryVATDesktop.IsWCF);  //new SaleDAL();

            dt = Dal.SearchSaleExportDTNew(txtSaleExportNo.Text.Trim(), FromDate, ToDate, Post, BranchId,connVM);
            dgvHistory.Rows.Clear();
            int j = 0;
            foreach (DataRow item in dt.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();

                dgvHistory.Rows.Add(NewRow);

                dgvHistory.Rows[j].Cells["SaleExportNo"].Value = item["SaleExportNo"].ToString();
                dgvHistory.Rows[j].Cells["SaleExportDate"].Value = item["SaleExportDate"].ToString();
                dgvHistory.Rows[j].Cells["Description"].Value = item["Description"].ToString();
                dgvHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                dgvHistory.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                dgvHistory.Rows[j].Cells["GrossWeight"].Value = item["GrossWeight"].ToString();
                dgvHistory.Rows[j].Cells["NetWeight"].Value = item["NetWeight"].ToString();
                dgvHistory.Rows[j].Cells["NumberFrom"].Value = item["NumberFrom"].ToString();
                dgvHistory.Rows[j].Cells["NumberTo"].Value = item["NumberTo"].ToString();
                dgvHistory.Rows[j].Cells["PortFrom"].Value = item["PortFrom"].ToString();
                dgvHistory.Rows[j].Cells["PortTo"].Value = item["PortTo"].ToString();
                dgvHistory.Rows[j].Cells["Post"].Value = item["Post"].ToString();
                dgvHistory.Rows[j].Cells["BranchId1"].Value = item["BranchId"].ToString();

                j = j + 1;

            }
     
        }

        private void FormSaleExportSearch_Load(object sender, EventArgs e)
        {
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
            cmbBranch.SelectedValue = Program.BranchId;

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

        }

        private void dgvHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvHistory_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();

        }


    }
}
