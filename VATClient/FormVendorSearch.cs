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
using System.IO;
using System.Security.Cryptography;
using SymphonySofttech.Utilities;
using System.Data.SqlClient;
using VATServer.Library;
using OfficeOpenXml;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormVendorSearch : Form
    {
        public FormVendorSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #region Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private string SelectedValue = string.Empty;
        private string StartFromDate, StartToDate;
        //public string VFIN = "308";
        private int searchBranchId = 0;
        DataGridViewRow selectedRow = new DataGridViewRow();

        #region
        private string RecordCount = "0";
        #endregion

        #region Global Variable For BackGoundWork

        private string activeStatus = string.Empty;
        private string VendorData = string.Empty;
        private DataTable VendorResult;
        private string vendortype;

        #endregion

        #endregion


        public static string SelectOne1()
        {
            string SearchValue = string.Empty;
            try
            {
                FormVendorSearch frmSearch = new FormVendorSearch();
                frmSearch.ShowDialog();
                SearchValue = frmSearch.SelectedValue;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormVendorSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormVendorSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormVendorSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormVendorSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormVendorSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormVendorSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormVendorSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormVendorSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormVendorSearch", "SelectOne", exMessage);
            }
            #endregion Catch

            return SearchValue;
        }
        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;

            #region try

            try
            {
                FormVendorSearch frmSearch = new FormVendorSearch();
                frmSearch.ShowDialog();

                selectedRowTemp = frmSearch.selectedRow;
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormProductSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormProductSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormProductSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormProductSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Product Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductSearch", "SelectOne", exMessage);
            }

            #endregion



            return selectedRowTemp;
        }

        private void NullCheck()
        {

            try
            {
                if (dtpStartDateFrom.Checked == false)
                {
                    StartFromDate = "";
                }
                else
                {
                    StartFromDate = dtpStartDateFrom.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpStartDateTo.Checked == false)
                {
                    StartToDate = "";
                }
                else
                {
                    StartToDate = dtpStartDateTo.Value.ToString("yyyy-MMM-dd");
                }
            }
            #region Catch
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
            #endregion Catch
        }
        private void dgvVendor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void txtVendorID_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            txtVendorCode.Text = String.Format("{1:#,0.00}", 1, Convert.ToDouble(txtVendorName.Text));
        }
        public void ClearAllFields()
        {
            try
            {
                txtCity.Text = "";
                txtContactPerson.Text = "";
                txtContactPersonDesignation.Text = "";
                txtContactPersonEmail.Text = "";
                txtContactPersonTelephone.Text = "";
                txtEmail.Text = "";
                txtFaxNo.Text = "";
                txtTelephoneNo.Text = "";
                txtTINNo.Text = "";
                txtVATRegistrationNo.Text = "";
                txtVendorGroupID.Text = "";
                txtVendorCode.Text = "";
                txtVendorName.Text = "";
                cmbActive.Text = "";
                dtpStartDateFrom.Text = "";
                dtpStartDateTo.Text = "";
                //dgvVendor.DataSource = null;
                dgvVendor.Rows.Clear();
                txtVendorGroupName.Text = "";
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtVendorID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVendorGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpStartDateFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpStartDateTo_KeyDown(object sender, KeyEventArgs e)
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
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        #endregion
        private void GridSelected()
        {
            // No SOAP Service

            try
            {
                if (dgvVendor.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvVendor.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }


                this.Close();

                //if (dgvVendor.Rows.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //string VehicleInfo = string.Empty;
                //int ColIndex = dgvVendor.CurrentCell.ColumnIndex;
                //int RowIndex1 = dgvVendor.CurrentCell.RowIndex;
                //if (RowIndex1 >= 0)
                //{
                //    if (Program.fromOpen != "Me" &&
                //        dgvVendor.Rows[RowIndex1].Cells["ActiveStatus1"].Value.ToString() != "Y")
                //    {
                //        MessageBox.Show("This Selected Item is not Active");
                //        return;
                //    }

                //    string VendorID = dgvVendor.Rows[RowIndex1].Cells["VendorID"].Value.ToString();
                //    string VendorName = dgvVendor.Rows[RowIndex1].Cells["VendorName"].Value.ToString();
                //    string VendorGroupID = dgvVendor.Rows[RowIndex1].Cells["VendorGroupID"].Value.ToString();
                //    string VendorGroupName = dgvVendor.Rows[RowIndex1].Cells["VendorGroupName"].Value.ToString();
                //    string City = dgvVendor.Rows[RowIndex1].Cells["City"].Value.ToString();
                //    string Address1 = dgvVendor.Rows[RowIndex1].Cells["Address1"].Value.ToString();
                //    string Address2 = dgvVendor.Rows[RowIndex1].Cells["Address2"].Value.ToString();
                //    string Address3 = dgvVendor.Rows[RowIndex1].Cells["Address3"].Value.ToString();
                //    string TelephoneNo = dgvVendor.Rows[RowIndex1].Cells["TelephoneNo"].Value.ToString();
                //    string FaxNo = dgvVendor.Rows[RowIndex1].Cells["FaxNo"].Value.ToString();
                //    string Email = dgvVendor.Rows[RowIndex1].Cells["Email"].Value.ToString();
                //    string StartDateTime = dgvVendor.Rows[RowIndex1].Cells["StartDateTime"].Value.ToString();
                //    string ContactPerson = dgvVendor.Rows[RowIndex1].Cells["ContactPerson"].Value.ToString();
                //    string ContactPersonDesignation =
                //        dgvVendor.Rows[RowIndex1].Cells["ContactPersonDesignation"].Value.ToString();
                //    string ContactPersonTelephone =
                //        dgvVendor.Rows[RowIndex1].Cells["ContactPersonTelephone"].Value.ToString();
                //    string ContactPersonEmail = dgvVendor.Rows[RowIndex1].Cells["ContactPersonEmail"].Value.ToString();
                //    string VATRegistrationNo = dgvVendor.Rows[RowIndex1].Cells["VATRegistrationNo"].Value.ToString();
                //    string TINNo = dgvVendor.Rows[RowIndex1].Cells["TINNo"].Value.ToString();
                //    string Comments = dgvVendor.Rows[RowIndex1].Cells["Comments"].Value.ToString();
                //    //string ActiveStatus = dgvVendor.Rows[RowIndex1].Cells["ActiveStatus1"].Value.ToString();
                //    string ActiveStatus = dgvVendor.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString();
                //    string Country = dgvVendor.Rows[RowIndex1].Cells["Country"].Value.ToString();
                //    string code = dgvVendor.Rows[RowIndex1].Cells["Code"].Value.ToString();
                //    string VDSPercent = dgvVendor.Rows[RowIndex1].Cells["VDSPercent"].Value.ToString();
                //    string BusinessType = dgvVendor.Rows[RowIndex1].Cells["BusinessType"].Value.ToString();
                //    string BusinessCode = dgvVendor.Rows[RowIndex1].Cells["BusinessCode"].Value.ToString();
                //    string IsVDSWithHolder = dgvVendor.Rows[RowIndex1].Cells["IsVDSWithHolder"].Value.ToString();
                //    string IsRegister = dgvVendor.Rows[RowIndex1].Cells["IsRegister"].Value.ToString();
                //    string IsTurnover = dgvVendor.Rows[RowIndex1].Cells["IsTurnover"].Value.ToString();
                //    string BranchId = dgvVendor.Rows[RowIndex1].Cells["BranchId"].Value.ToString();
                //    string ShortName = dgvVendor.Rows[RowIndex1].Cells["ShortName"].Value.ToString();


                //    VehicleInfo =
                //        VendorID + FieldDelimeter +0
                //        VendorName + FieldDelimeter +1
                //        VendorGroupID + FieldDelimeter +2
                //        VendorGroupName + FieldDelimeter +3
                //        City + FieldDelimeter +4
                //        Address1 + FieldDelimeter +5
                //        Address2 + FieldDelimeter +6
                //        Address3 + FieldDelimeter +7
                //        TelephoneNo + FieldDelimeter +8
                //        FaxNo + FieldDelimeter +9
                //        Email + FieldDelimeter +10
                //        StartDateTime + FieldDelimeter +11
                //        ContactPerson + FieldDelimeter +12
                //        ContactPersonDesignation + FieldDelimeter +13
                //        ContactPersonTelephone + FieldDelimeter +14
                //        ContactPersonEmail + FieldDelimeter +15
                //        VATRegistrationNo + FieldDelimeter +16
                //        TINNo + FieldDelimeter +17
                //        Comments + FieldDelimeter +18

                //        ActiveStatus + FieldDelimeter +19
                //        Country + FieldDelimeter +20
                        //code -21 + FieldDelimeter + VDSPercent  022
                //        + FieldDelimeter + BusinessType23
                //        + FieldDelimeter + BusinessCode24
                //        + FieldDelimeter + IsVDSWithHolder25
                //        + FieldDelimeter + IsRegister26
                //        + FieldDelimeter + IsTurnover27
                //        + FieldDelimeter + BranchId28
                //        +FieldDelimeter + ShortName;29

                //    SelectedValue = VehicleInfo;

                //}
            }
            #region Catch
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
            #endregion Catch
            //this.Close();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();

        }
        private void dgvVendor_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // No SOAP Service

            try
            {
                //MDIMainInterface mdi = new MDIMainInterface();
                FormVendor frmVendor = new FormVendor();
                //mdi.RollDetailsInfo(frmVendor.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmVendor.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.fromOpen == "Me")
                {
                    this.Close();
                    return;
                }

                frmVendor.Show();
            }
            #region Catch
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
            #endregion Catch
        }
        //public void BranchLoad(ComboBox cmbBranch)
        //{

        //    BranchProfileDAL _BranchProfileDAL = new BranchProfileDAL();
        //    DataTable dtBranchProfile = new DataTable();

        //    dtBranchProfile = _BranchProfileDAL.SelectAll(null, null, null, null, null, true);


        //    DataRow dr = dtBranchProfile.NewRow();
        //    dr["BranchID"] = "0";
        //    dr["BranchName"] = "All Branch";
        //    dtBranchProfile.Rows.InsertAt(dr, 0);

        //    cmbBranch.DataSource = dtBranchProfile;
        //    cmbBranch.ValueMember = "BranchID";
        //    cmbBranch.DisplayMember = "BranchName";

        //}
        private void FormVendorSearch_Load(object sender, EventArgs e)
        {
            try
            {

                if (Program.fromOpen == "Me")
                {
                    btnAdd.Visible = false;
                }
                //BranchLoad(cmbBranch);
                CommonDAL dal = new CommonDAL();
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = dal.ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
                cmbBranch.SelectedValue = Program.BranchId;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = dal.settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                #region RecordCount
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;
                #endregion

                //Search();
                cmbImport.SelectedIndex = 0;

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormVendorSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormVendorSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormVendorSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormVendorSearch_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormVendorSearch_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendorSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendorSearch_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormVendorSearch_Load", exMessage);
            }
            #endregion Catch
        }
        //===============Search==============
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchBranchId = Convert.ToInt32(cmbBranch.SelectedValue);

            RecordCount = cmbRecordCount.Text.Trim();

            Search();
        }
        private void Search()
        {
            try
            {
                NullCheck();

                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                vendortype = cmbType.Text.Trim();
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerSearch.RunWorkerAsync();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchDT", exMessage);
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
                FileLogger.Log(this.Name, "SearchDT", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchDT", exMessage);
            }
            #endregion Catch

        }
        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //VendorResult = VendorDAL.SearchVendorDT
                //VendorDAL vendorDAL = new VendorDAL();
                IVendor vendorDAL = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);

                #region Not used

                //VendorResult = vendorDAL.SearchVendor(
                //    txtVendorCode.Text.Trim()
                //    , txtVendorName.Text.Trim()
                //    , txtVendorGroupID.Text.Trim()
                //    , txtVendorGroupName.Text.Trim()
                //    , txtCity.Text.Trim()
                //    , txtTelephoneNo.Text.Trim()
                //    , txtFaxNo.Text.Trim()
                //    , txtEmail.Text.Trim()
                //    , StartFromDate
                //    , StartToDate
                //    , txtContactPerson.Text.Trim()
                //    , txtContactPersonDesignation.Text.Trim()
                //    , txtContactPersonTelephone.Text.Trim()
                //    , txtContactPersonEmail.Text.Trim()
                //    , txtTINNo.Text.Trim()
                //    , txtVATRegistrationNo.Text.Trim()
                //    , activeStatus
                //    );

                #endregion

                //BugsBD
                string VendorCode = OrdinaryVATDesktop.SanitizeInput(txtVendorCode.Text.Trim());
                string VendorName = OrdinaryVATDesktop.SanitizeInput(txtVendorName.Text.Trim());
                string VendorGroupID = OrdinaryVATDesktop.SanitizeInput(txtVendorGroupID.Text.Trim());
                string VendorGroupName = OrdinaryVATDesktop.SanitizeInput(txtVendorGroupName.Text.Trim());
                string City = OrdinaryVATDesktop.SanitizeInput(txtCity.Text.Trim());
                string TelephoneNo = OrdinaryVATDesktop.SanitizeInput(txtTelephoneNo.Text.Trim());
                string FaxNo = OrdinaryVATDesktop.SanitizeInput(txtFaxNo.Text.Trim());
                string Email = OrdinaryVATDesktop.SanitizeInput(txtEmail.Text.Trim());
                string ContactPerson = OrdinaryVATDesktop.SanitizeInput(txtContactPerson.Text.Trim());
                string ContactPersonDesignation = OrdinaryVATDesktop.SanitizeInput(txtContactPersonDesignation.Text.Trim());
                string ContactPersonTelephone = OrdinaryVATDesktop.SanitizeInput(txtContactPersonTelephone.Text.Trim());
                string ContactPersonEmail = OrdinaryVATDesktop.SanitizeInput(txtContactPersonEmail.Text.Trim());
                string TINNo = OrdinaryVATDesktop.SanitizeInput(txtTINNo.Text.Trim());
                string VATRegistrationNo = OrdinaryVATDesktop.SanitizeInput(txtVATRegistrationNo.Text.Trim());

                string[] cValues = { 

                     //  txtVendorCode.Text.Trim()
                     //, txtVendorName.Text.Trim()
                     //, txtVendorGroupID.Text.Trim()
                     //, txtVendorGroupName.Text.Trim()
                     //, txtCity.Text.Trim()
                     //, txtTelephoneNo.Text.Trim()
                     //, txtFaxNo.Text.Trim()
                     //, txtEmail.Text.Trim()

                     //, txtContactPerson.Text.Trim()
                     //, txtContactPersonDesignation.Text.Trim()
                     //, txtContactPersonTelephone.Text.Trim()
                     //, txtContactPersonEmail.Text.Trim()
                     //, txtTINNo.Text.Trim()
                     //, txtVATRegistrationNo.Text.Trim()

                      VendorCode                  
                    , VendorName                  
                    , VendorGroupID
                    , VendorGroupName
                    , City
                    , TelephoneNo
                    , FaxNo
                    , Email 
                  
                    , StartFromDate
                    , StartToDate

                    , ContactPerson
                    , ContactPersonDesignation
                    , ContactPersonTelephone
                    , ContactPersonEmail
                    , TINNo
                    , VATRegistrationNo
                  
                    , activeStatus
                    , searchBranchId.ToString()
                    , vendortype
                    , RecordCount
                    };


                string[] cFields = {  "v.VendorCode like"
                                     ,"v.VendorName like"
                                     ,"v.VendorGroupID like"
                                     ,"vg.VendorGroupName like"
                                     ,"v.City like"
                                     ,"v.TelephoneNo like"
                                     ,"v.FaxNo like"
                                     ,"v.Email like"
                                     ,"v.StartDateTime>"
                                     ,"v.StartDateTime<"
                                     ,"v.ContactPerson like"
                                     ,"v.ContactPersonDesignation like"
                                     ,"v.ContactPersonTelephone like"
                                     ,"v.ContactPersonEmail like"
                                     ,"v.TINNo like"
                                     ,"v.VATRegistrationNo like"
                                     ,"v.ActiveStatus like"
                                     ,"v.BranchId"
                                     ,"vg.GroupType"
                                     ,"SelectTop"
                                   };



                VendorResult = vendorDAL.SelectAll(0, cFields, cValues, null, null, true,connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                VendorResult = OrdinaryVATDesktop.DtDeleteColumns(VendorResult, columnNames);


            }
            #region Catch
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
            #endregion Catch
        }
        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region TotalRecordCount
            string TotalRecordCount = "0";
            #endregion

            try
            {
                // Start Complete

                dgvVendor.DataSource = null;
                if (VendorResult != null && VendorResult.Rows.Count > 0)
                {

                    TotalRecordCount = VendorResult.Rows[VendorResult.Rows.Count - 1][0].ToString();

                    VendorResult.Rows.RemoveAt(VendorResult.Rows.Count - 1);

                    dgvVendor.DataSource = VendorResult;
                    #region Specific Columns Visibility False
                    dgvVendor.Columns["VendorID"].Visible = false;
                    dgvVendor.Columns["VendorGroupID"].Visible = false;
                    dgvVendor.Columns["BranchId"].Visible = false;
                    #endregion
                }

                #region Old

                //int j = 0;
                //dgvVendor.Rows.Clear();
                //if (VendorResult != null)
                //    foreach (DataRow item in VendorResult.Rows)
                //    {
                //        DataGridViewRow dgvNew = new DataGridViewRow();
                //        dgvVendor.Rows.Add(dgvNew);
                //        dgvVendor.Rows[j].Cells["VendorID"].Value = item["VendorID"].ToString();
                //        dgvVendor.Rows[j].Cells["VendorName"].Value = item["VendorName"].ToString();
                //        dgvVendor.Rows[j].Cells["VendorGroupID"].Value = item["VendorGroupID"].ToString();
                //        dgvVendor.Rows[j].Cells["VendorGroupName"].Value = item["VendorGroupName"].ToString();
                //        dgvVendor.Rows[j].Cells["City"].Value = item["City"].ToString();
                //        dgvVendor.Rows[j].Cells["Address1"].Value = item["Address1"].ToString();
                //        dgvVendor.Rows[j].Cells["Address2"].Value = item["Address2"].ToString();
                //        dgvVendor.Rows[j].Cells["Address3"].Value = item["Address3"].ToString();
                //        dgvVendor.Rows[j].Cells["TelephoneNo"].Value = item["TelephoneNo"].ToString();
                //        dgvVendor.Rows[j].Cells["FaxNo"].Value = item["FaxNo"].ToString();
                //        dgvVendor.Rows[j].Cells["Email"].Value = item["Email"].ToString();
                //        dgvVendor.Rows[j].Cells["StartDate"].Value = item["StartDateTime"].ToString();
                //        dgvVendor.Rows[j].Cells["ContactPerson"].Value = item["ContactPerson"].ToString();
                //        dgvVendor.Rows[j].Cells["ContactPersonDesignation"].Value = item["ContactPersonDesignation"].ToString();
                //        dgvVendor.Rows[j].Cells["ContactPersonTelephone"].Value = item["ContactPersonTelephone"].ToString();
                //        dgvVendor.Rows[j].Cells["ContactPersonEmail"].Value = item["ContactPersonEmail"].ToString();
                //        dgvVendor.Rows[j].Cells["VATRegistrationNo"].Value = item["VATRegistrationNo"].ToString();
                //        dgvVendor.Rows[j].Cells["TINNo"].Value = item["TINNo"].ToString();
                //        dgvVendor.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                //        dgvVendor.Rows[j].Cells["ActiveStatus1"].Value = item["ActiveStatus"].ToString();
                //        dgvVendor.Rows[j].Cells["Country"].Value = item["Country"].ToString();
                //        dgvVendor.Rows[j].Cells["Code"].Value = item["VendorCode"].ToString();
                //        dgvVendor.Rows[j].Cells["VDSPercent"].Value = item["VDSPercent"].ToString();
                //        dgvVendor.Rows[j].Cells["BusinessType"].Value = item["BusinessType"].ToString();
                //        dgvVendor.Rows[j].Cells["BusinessCode"].Value = item["BusinessCode"].ToString();
                //        dgvVendor.Rows[j].Cells["IsVDSWithHolder"].Value = item["IsVDSWithHolder"].ToString();
                //        dgvVendor.Rows[j].Cells["IsRegister"].Value = item["IsRegister"].ToString();
                //        dgvVendor.Rows[j].Cells["IsTurnover"].Value = item["IsTurnover"].ToString();
                //        dgvVendor.Rows[j].Cells["BranchId"].Value = item["BranchId"].ToString();
                //        //////dgvVendor.Rows[j].Cells["TDSCode"].Value = item["TDSCode"].ToString();

                //        j = j + 1;
                //    }
                //dgvVendor.DataSource = null;

                //dgvVendor.Columns["VendorGroupID"].Visible = false;
                //dgvVendor.Columns["Address2"].Visible = false;
                //dgvVendor.Columns["Address3"].Visible = false;
                //dgvVendor.Columns["FaxNo"].Visible = false;
                //dgvVendor.Columns["ContactPerson"].Visible = false;
                //dgvVendor.Columns["ContactPersonDesignation"].Visible = false;
                //dgvVendor.Columns["ContactPersonTelephone"].Visible = false;
                //dgvVendor.Columns["ContactPersonEmail"].Visible = false;
                //dgvVendor.Columns["Comments"].Visible = false;

                #endregion

                //// End Complete
            }
            #region Catch
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
            #endregion Catch


            finally
            {
                //LRecordCount.Text = "Record Count: " + dgvVendor.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (VendorResult.Rows.Count) + " of " + TotalRecordCount.ToString();

            }

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                var ids = new List<string>();

                var len = dgvVendor.RowCount;

                for (int i = 0; i < len; i++)
                {
                    if (Convert.ToBoolean(dgvVendor["Select", i].Value))
                    {
                        ids.Add(dgvVendor["VendorCode", i].Value.ToString());
                    }
                }

                //var dal = new VendorDAL();
                IVendor dal = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = dal.GetExcelData(ids,null,null,connVM);
               


                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("There is no data found");
                    return;
                }

                if (cmbImport.Text == "Excel")
                {
                    OrdinaryVATDesktop.SaveExcel(dt, "Vendors", "Vendor");

                }
                else if (cmbImport.Text == "Text")
                {
                    OrdinaryVATDesktop.WriteDataToFile(dt, "Vendors");

                }


                //string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                //string fileDirectory = pathRoot + "\\Excel Files";
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}
                //fileDirectory += "\\vendors.xlsx";
                //FileStream objFileStrm = File.Create(fileDirectory);

                //using (ExcelPackage pck = new ExcelPackage(objFileStrm))
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("vendors");
                //    ws.Cells["A1"].LoadFromDataTable(dt, true);
                //    pck.Save();
                //    objFileStrm.Close();
                //}
                //MessageBox.Show("Successfully Exported data as vendors.xlsx in Excel files of the root directory");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvVendor.RowCount; i++)
            {
                dgvVendor["Select", i].Value = chkSelectAll.Checked;
            }
        }
    }
}
