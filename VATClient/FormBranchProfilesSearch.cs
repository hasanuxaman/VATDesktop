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
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATClient.ModelDTO;
using VATServer.Library;
using OfficeOpenXml;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATServer.Interface;
using CrystalDecisions.CrystalReports.Engine;
using VATClient.ReportPreview;
using SymphonySofttech.Reports;


namespace VATClient
{
    public partial class FormBranchProfilesSearch : Form
    {
        public FormBranchProfilesSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        //private string SelectedValue = string.Empty;
        DataGridViewRow SelectedValue = new DataGridViewRow();
        private DataTable CustomerResult;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private ReportDocument reportDocument = null;
        List<string> BranchList = new List<string>();

        private void FromBranchProfilesSearch_Load(object sender, EventArgs e)
        {

        }
        private void ClearAll()
        {
            //DateTime vdateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));


            txtBranchCode.Text = "";
            txtBranchName.Text = "";
            txtVATRegistrationNo.Text = "";
            textBIN.Text = "";

            txtContactPerson.Text = "";

            txtTINNo.Text = "";
            txtVATRegistrationNo.Text = "";
         
            //chIsArchive.Checked = false;
            //chkVDSWithHolder.Checked = false;
        }
        private void GridSelected()
        {
            try
            {
                #region Statement


                if (dgvCustomer.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvCustomer.SelectedRows;
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

        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow frmSearchSelectRow = null;
            
            try
            {
                FormBranchProfilesSearch frmBranchProfilesSearch = new FormBranchProfilesSearch();
                frmBranchProfilesSearch.ShowDialog();
                frmSearchSelectRow = frmBranchProfilesSearch.selectedRow;
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormCustomerSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormCustomerSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormCustomerSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormCustomerSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormCustomerSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormCustomerSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormCustomerSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormCustomerSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormCustomerSearch", "SelectOne", exMessage);
            }
            #endregion

            return frmSearchSelectRow;
            //FormSearchProduct frmSearch = new FormSearchProduct();
            //frmSearch.ShowDialog();
            //return frmSearch.SelectedValue;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
        private void Search()
        {
            try
            {
                //NullCheck();



                //activeStatus = string.Empty;
                //activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                //type = string.Empty;
                //type = cmbType.Text.Trim();

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerSearch.RunWorkerAsync();


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
        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                #region Statement
                // Start DoWork
                //BranchProfileDAL BranchProfileDAL = new BranchProfileDAL();
                IBranchProfile BranchProfileDAL = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(OrdinaryVATDesktop.IsWCF);

                

                // End DoWork
                string[] cValues = {    txtBranchCode.Text.Trim()
                                           ,txtBranchName.Text.Trim()
                                           ,txtContactPerson.Text.Trim()
                                           ,textBIN.Text.Trim()
                                           ,txtVATRegistrationNo.Text.Trim()
                                           ,txtTINNo.Text.Trim()
                                    
                                            };

                string[] cFields = {       "BranchCode             like"
                                           ,"BranchName            like"
                                           ,"ContactPerson         like"
                                           ,"BIN                   like"
                                           ,"VATRegistrationNo     like"
                                           ,"TINNo                 like"
                                          
                                       };
                CustomerResult = BranchProfileDAL.SelectAll(null, cFields, cValues, null, null,true,connVM);
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

                dgvCustomer.Rows.Clear();
                int i = 0;
                foreach (DataRow item2 in CustomerResult.Rows)
                {
                    //var cGroup = new CustomerDTO();

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvCustomer.Rows.Add(NewRow);

                    dgvCustomer.Rows[i].Cells["BranchID"].Value = item2["BranchID"].ToString();
                    dgvCustomer.Rows[i].Cells["BranchCode"].Value = item2["BranchCode"].ToString();
                    dgvCustomer.Rows[i].Cells["BranchName"].Value = item2["BranchName"].ToString();
                    dgvCustomer.Rows[i].Cells["BranchLegalName"].Value = item2["BranchLegalName"].ToString();
                    dgvCustomer.Rows[i].Cells["Address"].Value = item2["Address"].ToString();
                    dgvCustomer.Rows[i].Cells["City"].Value = item2["City"].ToString();
                    dgvCustomer.Rows[i].Cells["ZipCode"].Value = item2["ZipCode"].ToString();
                    dgvCustomer.Rows[i].Cells["City"].Value = item2["City"].ToString();
                    dgvCustomer.Rows[i].Cells["TelephoneNo"].Value = item2["TelephoneNo"].ToString();
                    dgvCustomer.Rows[i].Cells["FaxNo"].Value = item2["FaxNo"].ToString();
                    dgvCustomer.Rows[i].Cells["Email"].Value = item2["Email"].ToString();
                    dgvCustomer.Rows[i].Cells["ContactPerson"].Value = item2["ContactPerson"].ToString();
                    dgvCustomer.Rows[i].Cells["ContactPersonDesignation"].Value =
                        item2["ContactPersonDesignation"].ToString(); // CustomerGroupFields[13].ToString();
                    dgvCustomer.Rows[i].Cells["ContactPersonTelephone"].Value =
                        item2["ContactPersonTelephone"].ToString(); // CustomerGroupFields[14].ToString();
                    dgvCustomer.Rows[i].Cells["ContactPersonEmail"].Value = item2["ContactPersonEmail"].ToString();
                    // CustomerGroupFields[15].ToString();
                    dgvCustomer.Rows[i].Cells["TINNo"].Value = item2["TINNo"].ToString();
                    // CustomerGroupFields[16].ToString();
                    dgvCustomer.Rows[i].Cells["VATRegistrationNo"].Value = item2["VATRegistrationNo"].ToString();
                    // CustomerGroupFields[17].ToString();
                    dgvCustomer.Rows[i].Cells["Comments"].Value = item2["Comments"].ToString();
                    // CustomerGroupFields[18].ToString();
            
                    // CustomerGroupFields[19].ToString();
          
                    // CustomerGroupFields[20].ToString();
                    dgvCustomer.Rows[i].Cells["ActiveStatus"].Value = item2["ActiveStatus"].ToString();
                    dgvCustomer.Rows[i].Cells["IsArchive"].Value = item2["IsArchive"].ToString();
                    dgvCustomer.Rows[i].Cells["BIN"].Value = item2["BIN"].ToString();

                    // CustomerGroupFields[20].ToString();

                    //cmbCustomerGroupName.Items.Add(item2["CustomerGroupName"]);
                    //customerGroups.Add(cGroup);
                    i = i + 1;

                }

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
                LRecordCount.Text = "Record Count: " + dgvCustomer.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void dgvCustomer_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var BranchIDList = new List<string>();

                var len = dgvCustomer.Rows.Count;


                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvCustomer["Select", i].Value)
                        )
                    {
                        BranchIDList.Add(dgvCustomer["BranchID", i].Value.ToString());
                    }
                }

                var dal = new BranchProfileDAL();

                DataTable data = dal.GetExcelData(BranchIDList);
                var details = dal.GetExcelBranchMapDetails(BranchIDList, null, null, connVM);
                var dataSet = new DataSet();

                var sheetNames = new List<string> { "BranchProfiles" };
                dataSet.Tables.Add(data);

                if (data.Rows.Count == 0)
                {
                    data.Rows.Add(data.NewRow());
                }
                if (details.Rows.Count > 0)
                {
                    dataSet.Tables.Add(details);
                    sheetNames.Add("BranchMapDetails");
                }
                //OrdinaryVATDesktop.SaveExcel(data, "BranchProfiles", "BranchProfileM");
                OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "BranchProfiles", sheetNames.ToArray());
                MessageBox.Show("Successfully Exported data in Excel files of root directory");
              
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }
        }

        private void SelectAll()
        {
            try
            {
         
               if (chkSelectAll.Checked == true)
               {
                   for (int i = 0; i < dgvCustomer.RowCount; i++)
                   {
                       dgvCustomer["Select", i].Value = true;
                   }
               }
               else
               {
                   for (int i = 0; i < dgvCustomer.RowCount; i++)
                   {
                       dgvCustomer["Select", i].Value = false;
                   }
               }
          }
          catch (Exception)
              {
              
                  throw;
              }
        }

        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ReportShowDs();
        }
        private void ReportShowDs()
        {
            try
            {
                this.progressBar1.Visible = true;
                this.btnPrint.Enabled = false;
                
                var len = dgvCustomer.Rows.Count;


                for (var i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvCustomer["Select", i].Value)
                        )
                    {
                        BranchList.Add(dgvCustomer["BranchID", i].Value.ToString());
                    }
                }
                backgroundWorkerPreview.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                #region Start

                MISReport _report = new MISReport();
                reportDocument = _report.BranchListReport(BranchList, connVM);
                
                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                #region Complete

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }
                FormReport reports = new FormReport();

                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //////reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
               
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrint.Enabled = true;
            }
        }

    }
}
