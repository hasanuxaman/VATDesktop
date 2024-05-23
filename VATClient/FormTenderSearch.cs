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
using VATClient.ModelDTO;
//
using System.IO;
using System.Security.Cryptography;
using SymphonySofttech.Utilities;
using System.Data.SqlClient;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormTenderSearch : Form
    {
        #region Constructors

        public FormTenderSearch()
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
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        //private static string PassPhrase = DBConstant.PassPhrase;
        //private static string EnKey = DBConstant.EnKey;
        private DateTime startdate, enddate;
        //private List<TenderHeadersDTO> TenderHeaders = new List<TenderHeadersDTO>();
        List<CustomerSinmgleDTO> customers = new List<CustomerSinmgleDTO>();
        private DataTable CustomerGroupResult;
        private string SearchBranchId = "0";
        //private string SelectedValue = string.Empty;

        private bool ImportByCustGrp;
        #region Global Variables For BackGroundWorker

        private string TypeData = string.Empty;
        private DataTable TenderTypeResult;
        private string varTenderId, varRefNo, varCustomerName;

        #endregion

        private string customerCode = string.Empty;
        private string customerName = string.Empty;
        private string customerGId = string.Empty;
        private string customerGName = string.Empty;
        DataGridViewRow selectedRow = new DataGridViewRow();


        #endregion

        #region Methods

        //public static string SelectOne()
        //{
        //    string frmSearchSelectedValue = string.Empty;

        //    try
        //    {
        //        #region Statement

        //        FormTenderSearch frmSearch = new FormTenderSearch();
        //        frmSearch.ShowDialog();
        //        return frmSearchSelectedValue = frmSearch.SelectedValue;

        //        #endregion
        //    }
        //    #region catch

        //    catch (IndexOutOfRangeException ex)
        //    {
        //        FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (FormatException ex)
        //    {

        //        FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }

        //    catch (SoapHeaderException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }

        //        FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (SoapException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
        //    }
        //    catch (EndpointNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
        //    }
        //    #endregion

        //    return frmSearchSelectedValue;
        //}

        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;
            try
            {
                #region Statement

                FormTenderSearch frmTenderSearch = new FormTenderSearch();
                frmTenderSearch.ShowDialog();
                selectedRowTemp = frmTenderSearch.selectedRow;

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTenderSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormTenderSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormTenderSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        private void FormTenderSearch_Load(object sender, EventArgs e)
        {
            #region Settings
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
            cmbBranch.SelectedValue = Program.BranchId;

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion


            string vCustGrp = string.Empty;


            CommonDAL commonDal = new CommonDAL();

            vCustGrp = commonDal.settingsDesktop("ImportTender", "CustomerGroup",null,connVM);
            if (string.IsNullOrEmpty(vCustGrp))
                
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

             ImportByCustGrp = Convert.ToBoolean(vCustGrp == "Y" ? true : false);
            #endregion Settings
            //Search();
            SearchCustomer();
        }
        private void SearchCustomer()
        {
            try
            {

                if (ImportByCustGrp == true)
                {
                    cmbCustomerName.Enabled = false;
                    chkCustCode.Enabled = false;
                }
                else
                {
                    cmbCustomerName.Enabled = true;
                    chkCustCode.Enabled = true;
                }
                #region Customer

               
                customerCode = string.Empty;
                customerName = string.Empty;
                customerGId = string.Empty;
                customerGName = string.Empty;
                
                #endregion Customer

                #region Statement

                this.progressBar1.Visible = true;
                backgroundWorkerSearchCustomer.RunWorkerAsync();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SearchCustomer", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchCustomer", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchCustomer", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchCustomer", exMessage);
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
                FileLogger.Log(this.Name, "SearchCustomer", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchCustomer", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchCustomer", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchCustomer", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerSearchCustomer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Customer
                //CustomerResult = new DataTable();
                //CustomerDAL customerDal = new CustomerDAL();
                //CustomerResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId,
                //customerGName, tinNo, customerVReg, activeStatusCustomer, CGType); // Change 04

                #endregion Customer

                
                CustomerGroupResult = new DataTable();
                CustomerDAL customerDal = new CustomerDAL();

                string[] cValues = new string[] { customerCode, customerName, customerGId, customerGName };
                string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.CustomerGroupID like", "cg.CustomerGroupName like" };
                CustomerGroupResult = customerDal.SelectAll(null, cFields, cValues, null, null, false,connVM);

                ////CustomerGroupResult = customerDal.SearchCustomerSingleDTNew("", "", "", "", "", "", "", "");  // Change 04
                //CustomerGroupResult = customerDal.SearchCustomerSingleDTNew(customerCode, customerName, customerGId, customerGName, "", "", "", "");  // Change 04

                //End DoWork
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerSearchCustomer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                cmbCustomerName.Items.Clear();
                customers.Clear();

                foreach (DataRow item2 in CustomerGroupResult.Rows)
                {
                    var customer = new CustomerSinmgleDTO();
                    customer.CustomerID = item2["CustomerID"].ToString();
                    customer.CustomerCode = item2["CustomerCode"].ToString();
                    customer.CustomerName = item2["CustomerName"].ToString();
                    customer.GroupType = item2["GroupType"].ToString();
                    customer.CustomerGroupName = item2["CustomerGroupName"].ToString();
                    customer.CustomerGroupID = item2["CustomerGroupID"].ToString();
                    
                    //cmbCustomerName.Items.Add(item2["CustomerName"]);
                    customers.Add(customer);
                }

                #region Customer Group
                if (CustomerGroupResult != null || CustomerGroupResult.Rows.Count > 0)
                {
                    cmbCustGrp.Items.Clear();
                    var vcmbCGroup = customers.Where(x => x.GroupType == "Local").Select(x => x.CustomerGroupName).Distinct().ToList();

                    if (vcmbCGroup.Any())
                    {
                        cmbCustGrp.Items.AddRange(vcmbCGroup.ToArray());
                    }
                    //cmbCustGrp.Items.Insert(0, "Select");
                    cmbCustGrp.SelectedIndex = 0;
                }
                #endregion Customer Group
                //End Complete
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchCustomer_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                if (ImportByCustGrp == true)
                {
                    cmbCustGrp_Leave(sender, e);
                }
                else
                {
                    customerloadToCombo();
                }
                
            }

        }
        private void customerloadToCombo()
        {
            try
            {
                //if (string.IsNullOrEmpty(cmbCustGrp.Text.Trim().ToLower()))
                //{
                //    MessageBox.Show("Please select Customer Group first", this.Text);
                //    return;
                //}

                cmbCustomerName.Items.Clear();

                if (chkCustCode.Checked == true)
                {
                    var CByCode = from cus in customers.ToList()
                                  where cus.CustomerGroupName.ToLower() == cmbCustGrp.Text.Trim().ToLower()
                                  orderby cus.CustomerCode
                                  select cus.CustomerCode;


                    if (CByCode != null && CByCode.Any())
                    {
                        cmbCustomerName.Items.AddRange(CByCode.ToArray());
                    }
                }
                else
                {
                    var CByName = from cus in customers.ToList()
                                  where cus.CustomerGroupName.ToLower() == cmbCustGrp.Text.Trim().ToLower()
                                  orderby cus.CustomerName
                                  select cus.CustomerName;


                    if (CByName != null && CByName.Any())
                    {
                        cmbCustomerName.Items.AddRange(CByName.ToArray());
                    }
                }

                cmbCustomerName.Items.Insert(0, "Select");
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
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
                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "customerloadToCombo", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "customerloadToCombo", exMessage);
            }
            #endregion Catch

        }
       

        private void ClearAllFields()
        {
            txtTenderId.Text = "";
            txtRefNo.Text = "";
            txtCustomerName.Text = "";
            //dgvTenderSearch.DataSource = null;
            dgvTenderSearch.Rows.Clear();
        }

        private void GridSeleted()
        {

            try
            {


                if (dgvTenderSearch.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvTenderSearch.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }
                this.Close();


                #region Statement



                //if (dgvTenderSearch.Rows.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //if (dgvTenderSearch.Rows.Count > 0)
                //{
                    






                //    string VDSInfo = string.Empty;

                //    int ColIndex = dgvTenderSearch.CurrentCell.ColumnIndex;
                //    int RowIndex1 = dgvTenderSearch.CurrentCell.RowIndex;
                //    if (RowIndex1 >= 0)
                //    {

                //        string TenderId = dgvTenderSearch.Rows[RowIndex1].Cells["TenderId"].Value.ToString();
                //        string RefNo = dgvTenderSearch.Rows[RowIndex1].Cells["RefNo"].Value.ToString();
                //        string CustomerId = "0";
                //        if (ImportByCustGrp==false)
                //        {
                //            CustomerId = dgvTenderSearch.Rows[RowIndex1].Cells["CustomerId"].Value.ToString();
                //        }
                //        else
                //        {
                //            CustomerId = dgvTenderSearch.Rows[RowIndex1].Cells["CustomerGroupID"].Value.ToString();
                //        }
                //        string CustomerName = dgvTenderSearch.Rows[RowIndex1].Cells["CustomerName"].Value.ToString();
                //        //string Address1 = dgvTenderSearch.Rows[RowIndex1].Cells["Address1"].Value.ToString();
                //        //string Address2 = dgvTenderSearch.Rows[RowIndex1].Cells["Address2"].Value.ToString();
                //        //string Address3 = dgvTenderSearch.Rows[RowIndex1].Cells["Address3"].Value.ToString();
                //        string Address1 = "";
                //        string Address2 = "";
                //        string Address3 = "";

                //        string TenderDate = dgvTenderSearch.Rows[RowIndex1].Cells["TenderDate"].Value.ToString();
                //        string DeleveryDate = dgvTenderSearch.Rows[RowIndex1].Cells["DeleveryDate"].Value.ToString();
                //        string Comments = dgvTenderSearch.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                //        string CustomerGroupName = dgvTenderSearch.Rows[RowIndex1].Cells["CustomerGroupName"].Value.ToString();

                //        VDSInfo =
                //            TenderId + FieldDelimeter +
                //            RefNo + FieldDelimeter +
                //            CustomerId + FieldDelimeter +
                //            CustomerName + FieldDelimeter +
                //            Address1 + FieldDelimeter +
                //            Address2 + FieldDelimeter +
                //            Address3 + FieldDelimeter +
                //            TenderDate + FieldDelimeter +
                //            DeleveryDate + FieldDelimeter +
                //            Comments + FieldDelimeter +
                //            CustomerGroupName;
                //        SelectedValue = VDSInfo;

                //    }
                //}
                

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

        #endregion

        #region Search

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
            //Tenderload();//no DAL Dependency
        }

        private void Search()
        {
            //string TypeData = string.Empty;

            try
            {
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                #region Statement

                varTenderId = string.IsNullOrEmpty(txtTenderId.Text) ? "" : txtTenderId.Text.Trim();

                varRefNo = string.IsNullOrEmpty(txtRefNo.Text) ? "" : txtRefNo.Text.Trim();

                varCustomerName = string.IsNullOrEmpty(txtCustomerName.Text) ? "" : txtCustomerName.Text.Trim();
                SearchBranchId = cmbBranch.SelectedValue.ToString();
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

        #region backgroundWorkerSearch Event

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TenderTypeResult = new DataTable();

                //TenderDAL tenderDal = new TenderDAL();
                ITender tenderDal = OrdinaryVATDesktop.GetObject<TenderDAL, TenderRepo, ITender>(OrdinaryVATDesktop.IsWCF);

                //if (ImportByCustGrp == true)
                //{
                //    //customerGId
                //    TenderTypeResult = tenderDal.SearchTenderHeaderByCustomerGrp(varTenderId, varRefNo, customerGId);
                //}
                //else
                //{
                //    TenderTypeResult = tenderDal.SearchTenderHeader(varTenderId, varRefNo, varCustomerName);
                //}

                string[] cValues = { varTenderId, varRefNo, varCustomerName, SearchBranchId };
                string[] cFields = { "th.TenderId like", "th.RefNo like", "c.CustomerName like","th.BranchId" };

                TenderTypeResult = tenderDal.SelectAll("0", cFields, cValues, null, null, true,connVM);

                // End DoWork
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
                dgvTenderSearch.Rows.Clear();
                int j = 0;
                foreach (DataRow row in TenderTypeResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvTenderSearch.Rows.Add(NewRow);
                    dgvTenderSearch.Rows[j].Cells["TenderId"].Value = row["TenderId"].ToString();
                    dgvTenderSearch.Rows[j].Cells["RefNo"].Value = row["RefNo"].ToString();
                    dgvTenderSearch.Rows[j].Cells["CustomerId"].Value = row["CustomerId"].ToString();
                    dgvTenderSearch.Rows[j].Cells["CustName"].Value = row["CustomerName"].ToString();
                    dgvTenderSearch.Rows[j].Cells["GroupId"].Value = row["CustomerGroupID"].ToString();
                    dgvTenderSearch.Rows[j].Cells["CustomerGroupName"].Value = row["CustomerGroupName"].ToString();
                    dgvTenderSearch.Rows[j].Cells["TenderDate"].Value = row["TenderDate"].ToString();
                    dgvTenderSearch.Rows[j].Cells["DeleveryDate"].Value = row["DeleveryDate"].ToString();
                    dgvTenderSearch.Rows[j].Cells["Comments"].Value = row["Comments"].ToString();
                    dgvTenderSearch.Rows[j].Cells["BranchId"].Value = row["BranchId"].ToString();

                    j = j + 1;
                }

                if (ImportByCustGrp == true)
                {
                  dgvTenderSearch.Columns["CustomerId"].Visible=false;
                  dgvTenderSearch.Columns["CustName"].Visible = false;  
                }
                else
                {
                    dgvTenderSearch.Columns["GroupId"].Visible = false;
                    dgvTenderSearch.Columns["CustomerGroupName"].Visible = false;  
                }

                #region OLD
 // Start Complete
                //dgvTenderSearch.DataSource = null;

                //dgvTenderSearch.DataSource = TenderTypeResult;
                //if (ImportByCustGrp == true)
                //{
                //    DataRow[] TenderRows = TenderTypeResult.Select("CustomerGroupID <> '0'");
                //    //TenderHeaders = (from DataRow row in TenderTypeResult.Rows
                //    TenderHeaders = (from DataRow row in TenderRows
                //                     select new TenderHeadersDTO()
                //                     {
                //                         TenderId = row["TenderId"].ToString(),
                //                         RefNo = row["RefNo"].ToString(),
                //                         CustomerGroupID = row["CustomerGroupID"].ToString(),
                //                         CustomerGroupName = row["CustomerGroupName"].ToString(),
                //                         //Address1 = row["Address1"].ToString(),
                //                         //Address2 = row["Address2"].ToString(),
                //                         //Address3 = row["Address3"].ToString(),
                //                         TenderDate = Convert.ToDateTime(row["TenderDate"]),
                //                         DeleveryDate = Convert.ToDateTime(row["DeleveryDate"]),
                //                         Comments = row["Comments"].ToString(),
                //                         CustomerName = " ",


                //                     }
                //            ).ToList();
                //}
                //else
                //{
                //     DataRow[] TenderRows = TenderTypeResult.Select("CustomerId <> '0'");
                //    //TenderHeaders = (from DataRow row in TenderTypeResult.Rows
                //    TenderHeaders = (from DataRow row in TenderRows
                //                     select new TenderHeadersDTO()
                //                     {
                //                         TenderId = row["TenderId"].ToString(),
                //                         RefNo = row["RefNo"].ToString(),
                //                         CustomerId = row["CustomerId"].ToString(),
                //                         CustomerName = row["CustomerName"].ToString(),
                //                         //Address1 = row["Address1"].ToString(),
                //                         //Address2 = row["Address2"].ToString(),
                //                         //Address3 = row["Address3"].ToString(),
                //                         TenderDate = Convert.ToDateTime(row["TenderDate"]),
                //                         DeleveryDate = Convert.ToDateTime(row["DeleveryDate"]),
                //                         Comments = row["Comments"].ToString(),
                //                         CustomerGroupName = row["CustomerGroupName"].ToString(),


                //                     }
                //           ).ToList();
                //}
                
                //Tenderload();
                // End Complete

                #endregion OLD
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
                LRecordCount.Text = "Record Count: " + dgvTenderSearch.Rows.Count.ToString();
               this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
          

        }

        #endregion

        private void Tenderload()
        {
            try
            {
                #region Statement

                //var TenderId = txtTenderId.Text.Trim();
                //var RefNo = txtRefNo.Text.Trim();
                //var CustomerName = txtCustomerName.Text.Trim();

                //if (!string.IsNullOrEmpty(TenderId) || !string.IsNullOrEmpty(RefNo) || !string.IsNullOrEmpty(CustomerName))
                //{

                //    var aa = TenderHeaders.ToList();

                //    var tender = TenderHeaders.Where(x =>
                //        x.TenderId.ToLower().Contains(TenderId.Trim().ToLower()) ||
                //        x.RefNo.ToLower().Contains(RefNo.Trim().ToLower()) ||
                //        x.CustomerName.ToLower().Contains(CustomerName.Trim().ToLower()));

                //    if (tender != null && tender.Any())
                //    {
                //        dgvTenderSearch.DataSource = null;

                //        dgvTenderSearch.DataSource = tender.ToList();
                //    }

                //}
                //else
                //{
                //    var prodtype = TenderHeaders.ToList();
                //    if (prodtype != null && prodtype.Any())
                //    {
                //        dgvTenderSearch.DataSource = null;
                //        dgvTenderSearch.DataSource = prodtype.ToList();
                //    }
                //}

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Tenderload", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Tenderload", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Tenderload", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Tenderload", exMessage);
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
                FileLogger.Log(this.Name, "Tenderload", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Tenderload", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Tenderload", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Tenderload", exMessage);
            }
            #endregion

        }

        #endregion

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void dgTenderSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void chkCustCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustCode.Checked)
            {
                chkCustCode.Text = "Code";
            }
            else
            {
                chkCustCode.Text = "Name";
            }
            customerloadToCombo();
        }

        private void cmbCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbCustomerName_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                txtCustomerName.Text = "";

                var searchText = cmbCustomerName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {

                    var custs = new List<CustomerSinmgleDTO>();
                    if (chkCustCode.Checked)
                    {

                        custs = customers.Where(x => x.CustomerCode.ToLower() == searchText).ToList();
                    }
                    else
                    {

                        custs = customers.Where(x => x.CustomerName.ToLower() == searchText).ToList();
                    }
                    if (custs.Any())
                    {
                        var customer = custs.First();
                        txtCustomerName.Text = customer.CustomerName;

                    }

                }

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            #endregion
        }

        private void txtTenderId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbCustGrp_Leave(object sender, EventArgs e)
        {
            try
            {
                if (ImportByCustGrp == true)
                {

                    var searchText = cmbCustGrp.Text.Trim().ToLower();
                    if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                    {
                        var custGrp = customers.Where(xx => xx.CustomerGroupName.ToLower() == searchText).First();
                        customerGId = custGrp.CustomerGroupID.ToString();
                    }
                    else
                    {
                        customerGId = "";
                    }
                }
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            #endregion
            customerloadToCombo();
        }

        private void cmbCustGrp_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void dgvTenderSearch_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }

    }
}
