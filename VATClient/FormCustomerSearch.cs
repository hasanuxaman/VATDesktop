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
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormCustomerSearch : Form
    {
        public FormCustomerSearch()
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

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private int searchBranchId = 0;

        #region RecordCount
        private string RecordCount = "0";
        #endregion

        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private string SelectedValue = string.Empty;
        //public string VFIN = "310";
        string CustomerData;
        private List<CustomerDTO> Customers = new List<CustomerDTO>();

        DataGridViewRow selectedRow = new DataGridViewRow();


        #endregion

        public static string SelectOne1()
        {
            string frmSearchSelectValue = String.Empty;
            try
            {
                FormCustomerSearch frmCustomerSearch = new FormCustomerSearch();
                frmCustomerSearch.ShowDialog();
                frmSearchSelectValue = frmCustomerSearch.SelectedValue;
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

            return frmSearchSelectValue;
            //FormSearchProduct frmSearch = new FormSearchProduct();
            //frmSearch.ShowDialog();
            //return frmSearch.SelectedValue;
        }

        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;

            #region try

            try
            {
                FormCustomerSearch frmSearch = new FormCustomerSearch();
                frmSearch.ShowDialog();

                selectedRowTemp = frmSearch.selectedRow;
            }
            #endregion
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

            return selectedRowTemp;
            
        }



        string FromDate, ToDate;

        #region Global Variables For BackGroundWorker


        // Datatable instant CustomerGroupResult
        private DataTable CustomerResult;
        string activeStatus = string.Empty;
        string type = string.Empty;


        #endregion



        private void NullCheck()
        {
            try
            {
                if (dtpDateFrom.Checked == false)
                {
                    FromDate = "";
                }
                else
                {
                    FromDate = dtpDateFrom.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpDateTo.Checked == false)
                {
                    ToDate = "";
                }
                else
                {
                    ToDate = dtpDateTo.Value.ToString("yyyy-MMM-dd");
                }
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
        //=======================Search=======================
        private void Search()
        {
            try
            {
                NullCheck();



                activeStatus = string.Empty;
                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                type = string.Empty;
                type = cmbType.Text.Trim();

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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);

            RecordCount = cmbRecordCount.Text.Trim();

            Search();
        }
        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // Start DoWork
                //CustomerDAL customerDAL = new CustomerDAL();
                ICustomer customerDAL = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                //CustomerResult = customerDAL.SearchCustomer(txtCustomerCode.Text.Trim(), txtCustomerName.Text.Trim(),
                //txtCustomerGroupID.Text.Trim(),txtCustomerGroupName.Text.Trim(),txtCity.Text.Trim(),
                //txtTelephoneNo.Text.Trim(),txtFaxNo.Text.Trim(),txtEmail.Text.Trim(),FromDate, 
                //ToDate,txtContactPerson.Text.Trim(),txtContactPersonDesignation.Text.Trim(),
                //txtContactPersonTelephone.Text.Trim(),txtContactPersonEmail.Text.Trim(),
                //txtTINNo.Text.Trim(), txtVATRegistrationNo.Text.Trim(),activeStatus,type); 

                // End DoWork
                string[] cValues = {        OrdinaryVATDesktop.SanitizeInput(txtCustomerCode.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtCustomerName.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtCustomerGroupID.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtCustomerGroupName.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtCity.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtTelephoneNo.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtFaxNo.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtEmail.Text.Trim())
                                           ,FromDate
                                           ,ToDate
                                           ,OrdinaryVATDesktop.SanitizeInput(txtContactPerson.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtContactPersonDesignation.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtContactPersonTelephone.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtContactPersonEmail.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtTINNo.Text.Trim())
                                           ,OrdinaryVATDesktop.SanitizeInput(txtVATRegistrationNo.Text.Trim())
                                           ,activeStatus
                                           ,type
                                           ,searchBranchId.ToString()
                                           ,RecordCount
                                   };

                string[] cFields = {       "c.CustomerCode             "
                                           ,"c.CustomerName             like"
                                           ,"c.CustomerGroupID          like"
                                           ,"cg.CustomerGroupName       like"
                                           ,"c.City                     like"
                                           ,"c.TelephoneNo              like"
                                           ,"c.FaxNo                    like"
                                           ,"c.Email                    like"
                                           ,"c.StartDateTime>"
                                           ,"c.StartDateTime<"
                                           ,"c.ContactPerson            like"
                                           ,"c.ContactPersonDesignation like"
                                           ,"c.ContactPersonTelephone   like"
                                           ,"c.ContactPersonEmail       like"
                                           ,"c.TINNo                    like"
                                           ,"c.VATRegistrationNo        like"
                                           ,"c.ActiveStatus like"
                                           ,"cg.GroupType               like"
                                           ,"c.BranchId "
                                           ,"SelectTop"
                                       };
                CustomerResult = customerDAL.SelectAll("0", cFields, cValues, null, null, true,connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                CustomerResult = OrdinaryVATDesktop.DtDeleteColumns(CustomerResult, columnNames);


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
            #region TotalRecordCount
            string TotalRecordCount = "0";
            #endregion

            try
            {
                #region Statement

                dgvCustomer.DataSource = null;
                if (CustomerResult != null && CustomerResult.Rows.Count > 0)
                {

                    TotalRecordCount = CustomerResult.Rows[CustomerResult.Rows.Count - 1][0].ToString();

                    CustomerResult.Rows.RemoveAt(CustomerResult.Rows.Count - 1);
                    #region Specific Coloumns Visible False
                    dgvCustomer.DataSource = CustomerResult;
                    dgvCustomer.Columns["CustomerID"].Visible = false;
                    dgvCustomer.Columns["CustomerGroupID"].Visible = false;
                    dgvCustomer.Columns["BranchId"].Visible = false;
                    #endregion
                }

                #region Old

                
                //// Start Complete

                //dgvCustomer.Rows.Clear();
                //int i = 0;
                //foreach (DataRow item2 in CustomerResult.Rows)
                //{
                //    //var cGroup = new CustomerDTO();

                //    DataGridViewRow NewRow = new DataGridViewRow();
                //    dgvCustomer.Rows.Add(NewRow);

                //    dgvCustomer.Rows[i].Cells["CustomerID"].Value = item2["CustomerID"].ToString();
                //    dgvCustomer.Rows[i].Cells["CustomerName"].Value = item2["CustomerName"].ToString();
                //    dgvCustomer.Rows[i].Cells["CustomerGroupID"].Value = item2["CustomerGroupID"].ToString();
                //    dgvCustomer.Rows[i].Cells["CustomerGroupName"].Value = item2["CustomerGroupName"].ToString();
                //    dgvCustomer.Rows[i].Cells["Address1"].Value = item2["Address1"].ToString();
                //    dgvCustomer.Rows[i].Cells["Address2"].Value = item2["Address2"].ToString();
                //    dgvCustomer.Rows[i].Cells["Address3"].Value = item2["Address3"].ToString();
                //    dgvCustomer.Rows[i].Cells["City"].Value = item2["City"].ToString();
                //    dgvCustomer.Rows[i].Cells["TelephoneNo"].Value = item2["TelephoneNo"].ToString();
                //    dgvCustomer.Rows[i].Cells["FaxNo"].Value = item2["FaxNo"].ToString();
                //    dgvCustomer.Rows[i].Cells["Email"].Value = item2["Email"].ToString();
                //    dgvCustomer.Rows[i].Cells["StartDate"].Value = item2["StartDateTime"].ToString();
                //    dgvCustomer.Rows[i].Cells["ContactPerson"].Value = item2["ContactPerson"].ToString();
                //    dgvCustomer.Rows[i].Cells["ContactPersonDesignation"].Value =
                //    item2["ContactPersonDesignation"].ToString(); // CustomerGroupFields[13].ToString();
                //    dgvCustomer.Rows[i].Cells["ContactPersonTelephone"].Value =
                //    item2["ContactPersonTelephone"].ToString(); // CustomerGroupFields[14].ToString();
                //    dgvCustomer.Rows[i].Cells["ContactPersonEmail"].Value = item2["ContactPersonEmail"].ToString();
                //    // CustomerGroupFields[15].ToString();
                //    dgvCustomer.Rows[i].Cells["TINNo"].Value = item2["TINNo"].ToString();
                //    // CustomerGroupFields[16].ToString();
                //    dgvCustomer.Rows[i].Cells["VATRegistrationNo"].Value = item2["VATRegistrationNo"].ToString();
                //    // CustomerGroupFields[17].ToString();
                //    dgvCustomer.Rows[i].Cells["Comments"].Value = item2["Comments"].ToString();
                //    // CustomerGroupFields[18].ToString();
                //    dgvCustomer.Rows[i].Cells["ActiveStatus1"].Value = item2["ActiveStatus"].ToString();
                //    // CustomerGroupFields[19].ToString();
                //    dgvCustomer.Rows[i].Cells["Country"].Value = item2["Country"].ToString();
                //    // CustomerGroupFields[20].ToString();
                //    dgvCustomer.Rows[i].Cells["GroupType"].Value = item2["GroupType"].ToString();
                //    dgvCustomer.Rows[i].Cells["Code"].Value = item2["CustomerCode"].ToString();

                //    dgvCustomer.Rows[i].Cells["BusinessType"].Value = item2["BusinessType"].ToString();
                //    dgvCustomer.Rows[i].Cells["BusinessCode"].Value = item2["BusinessCode"].ToString();
                //    dgvCustomer.Rows[i].Cells["IsVDSWithHolder"].Value = item2["IsVDSWithHolder"].ToString();
                //    dgvCustomer.Rows[i].Cells["BranchId"].Value = item2["BranchId"].ToString();

                //    // CustomerGroupFields[20].ToString();

                //    //cmbCustomerGroupName.Items.Add(item2["CustomerGroupName"]);
                //    //customerGroups.Add(cGroup);
                //    i = i + 1;

                //}

                //// End Complete
                #endregion

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
                //LRecordCount.Text = "Record Count: " + dgvCustomer.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (CustomerResult.Rows.Count) + " of " + TotalRecordCount.ToString();

            }

        }
        //====================================================
        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void ClearAllFields()
        {
            txtCity.Text = "";
            txtContactPerson.Text = "";
            txtContactPersonDesignation.Text = "";
            txtContactPersonEmail.Text = "";
            txtContactPersonTelephone.Text = "";
            txtCustomerGroupID.Text = "";
            txtCustomerGroupName.Text = "";
            txtCustomerCode.Text = "";
            txtCustomerName.Text = "";
            txtEmail.Text = "";
            txtFaxNo.Text = "";
            txtTelephoneNo.Text = "";
            txtTINNo.Text = "";
            txtVATRegistrationNo.Text = "";
            cmbActive.SelectedIndex = -1;
            dtpDateFrom.Text = "";
            dtpDateTo.Text = "";
            dgvCustomer.Rows.Clear();
            cmbType.SelectedIndex = -1;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        #region TextBox KeyDown Event

        private void txtCustomerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCustomerGroupID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCustomerGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDateFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDateTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTINNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVATRegistrationNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void cmbActiveStatus_KeyDown(object sender, KeyEventArgs e)
        {
            //btnSearch.Focus();
        }

        #endregion
        private void GridSelected()
        {
            try
            {

                if (dgvCustomer.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvCustomer.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }


                #region Old

                //if (dgvCustomer.Rows.Count > 0)
                //{
                //    string CustomerInfo = string.Empty;

                //    int ColIndex = dgvCustomer.CurrentCell.ColumnIndex;
                //    int RowIndex1 = dgvCustomer.CurrentCell.RowIndex;
                //    if (RowIndex1 >= 0)
                //    {
                //        //if (Program.fromOpen != "Me" &&
                //        //    dgvCustomer.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                //        //{
                //        //    MessageBox.Show("This Selected Item is not Active");
                //        //    return;
                //        //}

                //        string CustomerID = dgvCustomer.Rows[RowIndex1].Cells["CustomerID"].Value.ToString();
                //        string CustomerName = dgvCustomer.Rows[RowIndex1].Cells["CustomerName"].Value.ToString();
                //        string CustomerGroupID = dgvCustomer.Rows[RowIndex1].Cells["CustomerGroupID"].Value.ToString();
                //        string CustomerGroupName =
                //            dgvCustomer.Rows[RowIndex1].Cells["CustomerGroupName"].Value.ToString();
                //        string Address1 = dgvCustomer.Rows[RowIndex1].Cells["Address1"].Value.ToString();
                //        string Address2 = dgvCustomer.Rows[RowIndex1].Cells["Address2"].Value.ToString();
                //        string Address3 = dgvCustomer.Rows[RowIndex1].Cells["Address3"].Value.ToString();
                //        string City = dgvCustomer.Rows[RowIndex1].Cells["City"].Value.ToString();
                //        string TelephoneNo = dgvCustomer.Rows[RowIndex1].Cells["TelephoneNo"].Value.ToString();
                //        string FaxNo = dgvCustomer.Rows[RowIndex1].Cells["FaxNo"].Value.ToString();
                //        string Email = dgvCustomer.Rows[RowIndex1].Cells["Email"].Value.ToString();
                //        //DateTime StartDate =
                //        //    Convert.ToDateTime(dgvCustomer.Rows[RowIndex1].Cells["StartDate"].Value.ToString());
                //        DateTime StartDate =
                //            Convert.ToDateTime(dgvCustomer.Rows[RowIndex1].Cells["StartDateTime"].Value.ToString());
                //        string ContactPerson = dgvCustomer.Rows[RowIndex1].Cells["ContactPerson"].Value.ToString();
                //        string ContactPersonDesignation =
                //            dgvCustomer.Rows[RowIndex1].Cells["ContactPersonDesignation"].Value.ToString();
                //        string ContactPersonTelephone =
                //            dgvCustomer.Rows[RowIndex1].Cells["ContactPersonTelephone"].Value.ToString();
                //        string ContactPersonEmail =
                //            dgvCustomer.Rows[RowIndex1].Cells["ContactPersonEmail"].Value.ToString();
                //        string TINNo = dgvCustomer.Rows[RowIndex1].Cells["TINNo"].Value.ToString();
                //        string VATRegistrationNo =
                //            dgvCustomer.Rows[RowIndex1].Cells["VATRegistrationNo"].Value.ToString();
                //        string Comments = dgvCustomer.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                //        //string ActiveStatus = dgvCustomer.Rows[RowIndex1].Cells["ActiveStatus1"].Value.ToString();
                //        string ActiveStatus = dgvCustomer.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString();
                //        string Country = dgvCustomer.Rows[RowIndex1].Cells["Country"].Value.ToString();
                //        string GroupType = dgvCustomer.Rows[RowIndex1].Cells["GroupType"].Value.ToString();
                //        string code = dgvCustomer.Rows[RowIndex1].Cells["Code"].Value.ToString();
                //        //string code = dgvCustomer.Rows[RowIndex1].Cells["CustomerCode"].Value.ToString();
                //        string BusinessType = dgvCustomer.Rows[RowIndex1].Cells["BusinessType"].Value.ToString();
                //        string BusinessCode = dgvCustomer.Rows[RowIndex1].Cells["BusinessCode"].Value.ToString();
                //        string IsVDSWithHolder = dgvCustomer.Rows[RowIndex1].Cells["IsVDSWithHolder"].Value.ToString();
                //        string BranchId = dgvCustomer.Rows[RowIndex1].Cells["BranchId"].Value.ToString();
                //        string IsInstitution = dgvCustomer.Rows[RowIndex1].Cells["IsInstitution"].Value.ToString();
                //        string ShortName = dgvCustomer.Rows[RowIndex1].Cells["ShortName"].Value.ToString();
                        
                //        CustomerInfo =
                //            CustomerID + FieldDelimeter +//0
                //            CustomerName + FieldDelimeter +//1
                //            CustomerGroupID + FieldDelimeter +//2
                //            CustomerGroupName + FieldDelimeter +//3
                //            Address1 + FieldDelimeter +//4
                //            Address2 + FieldDelimeter +
                //            Address3 + FieldDelimeter +
                //            City + FieldDelimeter +
                //            TelephoneNo + FieldDelimeter +
                //            FaxNo + FieldDelimeter +
                //            Email + FieldDelimeter +
                //            StartDate + FieldDelimeter +
                //            ContactPerson + FieldDelimeter +
                //            ContactPersonDesignation + FieldDelimeter +
                //            ContactPersonTelephone + FieldDelimeter +
                //            ContactPersonEmail + FieldDelimeter +
                //            TINNo + FieldDelimeter +
                //            VATRegistrationNo + FieldDelimeter +
                //            Comments + FieldDelimeter +
                //            ActiveStatus + FieldDelimeter +
                //            Country + FieldDelimeter + GroupType + FieldDelimeter + code
                //            + FieldDelimeter + BusinessType
                //            + FieldDelimeter + BusinessCode
                //            + FieldDelimeter + IsVDSWithHolder
                //            + FieldDelimeter + BranchId
                //            + FieldDelimeter + IsInstitution
                //             + FieldDelimeter + ShortName;
                //        SelectedValue = CustomerInfo;

                //    }

                //}
                #endregion

                this.Close();
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
        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();


        }
        private void dgvCustomer_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                //MDIMainInterface mdi = new MDIMainInterface();
                FormCustomer frmCustomer = new FormCustomer();
                //mdi.RollDetailsInfo(frmCustomer.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmCustomer.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmCustomer.Show();
            }
            #region catch

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

        }
        private void FormCustomerSearch_Load(object sender, EventArgs e)
        {
            try
            {
                //dgvCustomer.Columns["StartDate"].Visible = false;
                //dgvCustomer.Columns["CustomerGroupID"].Visible = false;
                //dgvCustomer.Columns["Address1"].Visible = false;
                //dgvCustomer.Columns["Address2"].Visible = false;
                //dgvCustomer.Columns["Address3"].Visible = false;
                //dgvCustomer.Columns["Email"].Visible = false;
                //dgvCustomer.Columns["ContactPerson"].Visible = false;
                //dgvCustomer.Columns["ContactPersonDesignation"].Visible = false;
                //dgvCustomer.Columns["ContactPersonTelephone"].Visible = false;
                //dgvCustomer.Columns["ContactPersonEmail"].Visible = false;
                //dgvCustomer.Columns["VatRegistrationNo"].Visible = false;
                //dgvCustomer.Columns["TINNo"].Visible = false;
                //dgvCustomer.Columns["Comments"].Visible = false;
                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                CommonDAL dal = new CommonDAL();
                //ICommon dal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true);
                cmbBranch.SelectedValue = Program.BranchId;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = dal.settingsDesktop("Branch", "BranchDropDownWidth");
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                #region RecordCount
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;
                #endregion

                cmbImport.SelectedIndex = 0;
                //Search();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormCustomerSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormCustomerSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormCustomerSearch_Load", exMessage);
            }
            #endregion
        }

        private void dgvCustomer_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                var ids = new List<string>();

                for (int i = 0; i < dgvCustomer.RowCount; i++)
                {
                    if (Convert.ToBoolean(dgvCustomer["Select", i].Value))
                    {
                        ids.Add(dgvCustomer["CustomerCode", i].Value.ToString());
                    }
                }

                //var customerDal = new CustomerDAL();
                var customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = customerDal.GetExcelData(ids,null,null,connVM);
                var address = customerDal.GetExcelAddress(ids,null,null,connVM);

                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] {"Customer"};

                if (address.Rows.Count > 0)
                {
                    dataSet.Tables.Add(address);
                    sheetNames = new[] { "Customer", "CustomerAddress" };
                }


                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcelMultiple(dataSet,"Customers",sheetNames);
                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "Customer");
                }


                #region temp

                //if (CustomerResult != null && CustomerResult.Rows.Count > 0)
                //{
                //    dt = CustomerResult;
                //}
                //string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                //string fileDirectory = pathRoot + "\\Excel Files";
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}
                //fileDirectory += "\\customers.xlsx";
                //FileStream objFileStrm = File.Create(fileDirectory);

                //using (ExcelPackage pck = new ExcelPackage(objFileStrm))
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("customers");
                //    ws.Cells["A1"].LoadFromDataTable(dt, true);
                //    pck.Save();
                //    objFileStrm.Close();
                //}
                //MessageBox.Show("Successfully Exported data as customers.xlsx in Excel files of the root directory");

                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void grbCustomer_Enter(object sender, EventArgs e)
        {

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer["Select", i].Value = chkSelectAll.Checked;
            }
        }




    }
}
