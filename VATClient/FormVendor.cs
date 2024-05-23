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
//
using VATClient.ReportPages;
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Threading;
using VATServer.Library;
using VATClient.ModelDTO;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using System.Text.RegularExpressions;


namespace VATClient
{
    public partial class FormVendor : Form
    {
        public FormVendor()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }
        // ----------- Declare from DBConstant Start--------//

        List<VendorGroupDTO> VendorGroups= new List<VendorGroupDTO>();

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        private bool ChangeData = false;
        string NextID;
        string vTDSCode;
        string vTDSDescription;
        private bool IsUpdate = false;
        string[] VendorGroupLines;
        public string VFIN = "122";
        private int SearchBranchId = 0;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        string vVendorAddressId = "0";
        string vVendorId = "0";
        private DataTable VendorAddressResult;
        #region Global Variables for BackgroundWorker
        private string activestatus = string.Empty;
        private string VendorGroupData = string.Empty;
        private string VendorData = string.Empty;
        private string result = string.Empty;
        private string[] results  = new string[5];
        private string resultDelete = string.Empty;
        DataTable VendorGroupResult = new DataTable();
        private string SelectedVendorGroup = string.Empty;

        #endregion
        
       
        //string VendorData;
        private int ErrorReturn()
        {
            try
            {
                if (txtVendorName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter Vendor Name.", this.Text);
                    txtVendorName.Focus();
                    return 1;
                }
                if (txtVendorGroupID.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter Valid Vendore Group.", this.Text);
                    txtVendorGroupID.Focus();
                    return 1;
                }
                if (txtVATRegistrationNo.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter VATReg.No/BIN/NID.", this.Text);
                    txtVATRegistrationNo.Focus();
                    return 1;
                }



                string VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
                string VATRegNo = VATRegistrationNo.Replace("-", "");


                if (!OrdinaryVATDesktop.IsNumeric(VATRegNo))
                {
                    MessageBox.Show("Please Enter VATRegistrationNo only number");
                    txtVATRegistrationNo.Focus();
                    return 1;
                }

                if (VATRegNo.Length >= 14)
                {
                    MessageBox.Show("Please Enter Valid VATRegistrationNo No/Not more than 13 digit.");
                    txtVATRegistrationNo.Focus();
                    return 1;
                }


                



                if (txtNIDNo.Text.Trim()=="")
                {
                    txtNIDNo.Text = "-";
                }

                if (txtNIDNo.Text != "-")
                {
                    if (!OrdinaryVATDesktop.IsNumber(txtNIDNo.Text))
                    {
                        MessageBox.Show("Please Enter National ID  only number");
                        txtNIDNo.Focus();
                        return 1;
                    }

                    if (txtNIDNo.Text.Length >= 18)
                    {
                        MessageBox.Show("Please Enter Valid National ID No/Not more than 17 digit.");
                        txtNIDNo.Focus();
                        return 1;
                    }

                }


                if (txtTINNo.Text.Trim() == "")
                {
                    txtTINNo.Text = "-";
                }

                if (txtBusinessType.Text.Trim() == "")
                {
                    txtBusinessType.Text = "-";
                }
                if (txtBusinessCode.Text.Trim() == "")
                {
                    txtBusinessCode.Text = "-";
                }
                if (txtContactPerson.Text.Trim() == "")
                {
                    txtContactPerson.Text = "-";
                }
                if (txtContactPersonDesignation.Text == "")
                {
                    txtContactPersonDesignation.Text = "-";
                }
                if (txtContactPersonTelephone.Text.Trim() == "")
                {
                    txtContactPersonTelephone.Text = "-";
                }
                if (txtContactPersonEmail.Text.Trim() == "")
                {
                    txtContactPersonEmail.Text = "-";
                }
                if (txtTelephoneNo.Text.Trim() == "")
                {
                    txtTelephoneNo.Text = "-";
                }
                if (txtFaxNo.Text.Trim() == "")
                {
                    txtFaxNo.Text = "-";
                }
                if (txtEmail.Text.Trim() == "")
                {
                    txtEmail.Text = "-";
                }
                if (txtAddress1.Text.Trim() == "")
                {
                    txtAddress1.Text = "-";
                }
                //if (txtAddress2.Text.Trim() == "")
                //{
                //    txtAddress2.Text = "-";
                //}
                //if (txtAddress3.Text.Trim() == "")
                //{
                //    txtAddress3.Text = "-";
                //}
                if (txtCity.Text.Trim() == "")
                {
                    txtCity.Text = "-";
                }
                if (txtComments.Text.Trim() == "")
                {
                    txtComments.Text = "-";
                }
                if (txtCountry.Text.Trim() == "")
                {
                    txtCountry.Text = "-";
                }
                if (txtShortName.Text.Trim() == "")
                {
                    txtShortName.Text = "-";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            #endregion Catch
            return 0;
        }
        private void ClearAll()
        {
            try
            {
                cmbVendorGroupName.Text = "Select";
                txtVendorGroupDesc.Text = "";
                txtVendorID.Text = "";
                txtVendorName.Text = "";
                txtBusinessCode.Text = "";
                txtBusinessType.Text = "";
                txtVDS.Text = "0";

                txtVendorGroupID.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtAddress3.Text = "";
                txtCity.Text = "";
                txtTelephoneNo.Text = "";
                txtFaxNo.Text = "";
                txtEmail.Text = "";
                txtContactPerson.Text = "";
                txtContactPersonDesignation.Text = "";
                txtContactPersonTelephone.Text = "";
                txtContactPersonEmail.Text = "";
                txtVATRegistrationNo.Text = "";
                txtTINNo.Text = "";
                txtComments.Text = "";
                dtpStartDate.Value = Program.SessionDate;
                txtVendorGroupName.Text = "";
                txtCountry.Text = "";
                txtVendorCode.Text = "";
                chkActiveStatus.Checked = true;
                chkRegister.Checked = false;
                chkTurnover.Checked = false;
                chkVDSWithHolder.Checked = false;
                txtShortName.Text = "";
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAll", exMessage);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            #endregion Catch
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to refresh without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {
                        VendorGroupSearch();
                        ClearAll();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
                    VendorGroupSearch();
                    ClearAll();
                    ChangeData = false;
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            #endregion Catch
        }
        private void btnSearchVendor_Click(object sender, EventArgs e)
        {
            // No SOAP Service
            try
            {
                Program.fromOpen = "Me";
                
                DataGridViewRow selectedRow = new DataGridViewRow();
                
                selectedRow = FormVendorSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();//[0];
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();//[1];
                    txtVendorGroupID.Text = selectedRow.Cells["VendorGroupID"].Value.ToString();//[2];
                    txtVendorGroupName.Text = selectedRow.Cells["VendorGroupName"].Value.ToString();//[3];
                    cmbVendorGroupName.Text = selectedRow.Cells["VendorGroupName"].Value.ToString();//[3];
                    txtCity.Text = selectedRow.Cells["City"].Value.ToString();//[4];
                    txtAddress1.Text = selectedRow.Cells["Address1"].Value.ToString();//[5];
                    txtAddress2.Text = selectedRow.Cells["Address2"].Value.ToString();//[6];
                    txtAddress3.Text = selectedRow.Cells["Address3"].Value.ToString();//[7];
                    txtTelephoneNo.Text = selectedRow.Cells["TelephoneNo"].Value.ToString();//[8];
                    txtFaxNo.Text = selectedRow.Cells["FaxNo"].Value.ToString();//[9];
                    txtEmail.Text = selectedRow.Cells["Email"].Value.ToString();//[10];
                    dtpStartDate.Value = Convert.ToDateTime(selectedRow.Cells["StartDateTime"].Value.ToString());//[11]);
                    txtContactPerson.Text = selectedRow.Cells["ContactPerson"].Value.ToString();//[12];
                    txtContactPersonDesignation.Text = selectedRow.Cells["ContactPersonDesignation"].Value.ToString();//[13];
                    txtContactPersonTelephone.Text = selectedRow.Cells["ContactPersonTelephone"].Value.ToString();//[14];
                    txtContactPersonEmail.Text = selectedRow.Cells["ContactPersonEmail"].Value.ToString();//[15];
                    txtVATRegistrationNo.Text = selectedRow.Cells["VATRegistrationNo"].Value.ToString();//[16];
                    txtTINNo.Text = selectedRow.Cells["TINNo"].Value.ToString();//[17];
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();//[18];
                    txtCountry.Text = selectedRow.Cells["Country"].Value.ToString();//[20];
                    txtVendorCode.Text = selectedRow.Cells["VendorCode"].Value.ToString();//[21];
                    txtVDS.Text = Program.ParseDecimalObject(selectedRow.Cells["VDSPercent"].Value.ToString());//[22];
                    txtBusinessType.Text = selectedRow.Cells["BusinessType"].Value.ToString();//[23];
                    txtBusinessCode.Text = selectedRow.Cells["BusinessCode"].Value.ToString();//[24];
                    chkActiveStatus.Checked = Convert.ToBoolean(selectedRow.Cells["ActiveStatus"].Value.ToString() == "Y" ? true : false);
                    chkVDSWithHolder.Checked = Convert.ToBoolean(selectedRow.Cells["IsVDSWithHolder"].Value.ToString() == "Y" ? true : false);
                    chkRegister.Checked = Convert.ToBoolean(selectedRow.Cells["IsRegister"].Value.ToString() == "Y" ? true : false);
                    chkTurnover.Checked = Convert.ToBoolean(selectedRow.Cells["IsTurnover"].Value.ToString() == "Y" ? true : false);

                    SearchBranchId = Convert.ToInt32(selectedRow.Cells["BranchId"].Value.ToString());//[28]);
                    txtShortName.Text = selectedRow.Cells["ShortName"].Value.ToString();//[29]; 
                    txtNIDNo.Text = selectedRow.Cells["NIDNo"].Value.ToString();//[30];


                    //////txtTDSCode.Text = VendorInfo[29];
                    fnVendorAddress(txtVendorID.Text);
                    //btnAdd.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchVendor_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchVendor_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchVendor_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
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

        private void dtpStartDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVATRegistrationNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTINNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonDesignation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonTelephone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtContactPersonEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTelephoneNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtFaxNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtAddress3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        #endregion
        
        private void btnPrintList_Click(object sender, EventArgs e)
        {
            try
            {
                FormRptVendorInformation frmRptVendorInformation = new FormRptVendorInformation();
                frmRptVendorInformation.txtVendorID.Text = txtVendorID.Text.Trim();
                frmRptVendorInformation.Show();
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrintList_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrintList_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrintList_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrintList_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrintList_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintList_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrintList_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrintList_Click", exMessage);
            }
            #endregion Catch
        }
        private void btnPrintGrid_Click(object sender, EventArgs e)
        {

        }
        private void btnSearchVendorGroup_Click(object sender, EventArgs e)
        {
            try
            {
                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormVendorGroupSearch frm = new FormVendorGroupSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                Program.fromOpen = "Other";
                string result = FormVendorGroupSearch.SelectOne();

                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] VendorGroupInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtVendorGroupID.Text = VendorGroupInfo[0];
                    txtVendorGroupName.Text = VendorGroupInfo[1];
                    cmbVendorGroupName.Text = VendorGroupInfo[1];
                    txtVendorGroupDesc.Text = VendorGroupInfo[2];
                }
                // method
                VendorGroupSearch();
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchVendorGroup_Click", exMessage);
            }
            #endregion Catch
        }

        private void VendorGroupSearch()
        {
            try
            {

                this.btnSearchVendorGroup.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerVendorGrpSearch.RunWorkerAsync();
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VendorGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "VendorGroupSearch", exMessage);
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
                FileLogger.Log(this.Name, "VendorGroupSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VendorGroupSearch", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerVendorGrpSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //VendorGroupDAL vendorGroupDal = new VendorGroupDAL();
                IVendorGroup vendorGroupDal = OrdinaryVATDesktop.GetObject<VendorGroupDAL, VendorGroupRepo, IVendorGroup>(OrdinaryVATDesktop.IsWCF);


                VendorGroupResult = vendorGroupDal.SelectAll(0, null, null, null, null, true,connVM);

                //string decriptedVendorGroupData = Converter.DESDecrypt(PassPhrase, EnKey, VendorGroupResult);
                //VendorGroupLines = decriptedVendorGroupData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                // End DoWork
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_DoWork", exMessage);
            }
            #endregion Catch
        }
        private void backgroundWorkerVendorGrpSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                cmbVendorGroupName.Items.Clear();
                VendorGroups.Clear();

                foreach (DataRow items in VendorGroupResult.Rows)
                {
                    var vGroups = new VendorGroupDTO();

                    vGroups.VendorGroupID = items["VendorGroupID"].ToString();
                    vGroups.VendorGroupName = items["VendorGroupName"].ToString();
                    vGroups.VendorGroupDescription = items["VendorGroupDescription"].ToString();
                    
                    cmbVendorGroupName.Items.Add(items["VendorGroupName"]);

                    VendorGroups.Add(vGroups);
                }
                
                if (cmbVendorGroupName.Items.Count <= 0)
                {
                    MessageBox.Show("Please input Vendor Group first", this.Text);
                    return;
                }
                cmbVendorGroupName.SelectedIndex = 0;
                //cmbVendorGroupName.SelectedIndex = -1;
                //cmbVendorGroupName.Items.Insert(0, "Select");
                //cmbVendorGroupName.Text = "Select a Group";
                ChangeData = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
            finally
            {
                this.btnSearchVendorGroup.Enabled = true;
                this.progressBar1.Visible = false;
                ChangeData = false;
            }
        }

        private void cmbVendorGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {

            VendorGroupDetailsInfo();
            ChangeData = true;
        }

        private void VendorGroupDetailsInfo()
        {
            try
            {
                string selectedValue=string.Empty;
                DataRow[] dr = null;
                if(cmbVendorGroupName.SelectedIndex!=-1)
                {
                    selectedValue = cmbVendorGroupName.SelectedItem.ToString();
                }
                dr = VendorGroupResult.Select("VendorGroupName = '" + selectedValue + "'");
                if (dr != null && dr.Any())
                {
                    txtVendorGroupID.Text = dr[0]["VendorGroupID"].ToString();
                    txtVendorGroupName.Text = dr[0]["VendorGroupName"].ToString();
                    txtVendorGroupDesc.Text = dr[0]["VendorGroupDescription"].ToString();
                    if (dr[0]["GroupType"].ToString() == "Local")
                    {
                        txtVDS.Visible = true;
                        LVDS.Visible = true;
                    }
                    else
                    {
                        txtVDS.Visible = false;
                        LVDS.Visible = false;
                    }

                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", exMessage);
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
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VendorGroupDetailsInfo", exMessage);
            }
            #endregion Catch
        }

        private void FormVendor_Load(object sender, EventArgs e)
        {
            
            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                //////ToolTip1.SetToolTip(this.txtTDSCode, vTDSDescription);
                ToolTip1.SetToolTip(this.btnSearchVendor, "Existing Information");
                ToolTip1.SetToolTip(this.btnSearchVendorGroup, "Vendor Group");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ClearAll();
                VendorGroupSearch();
                #region read only for AutoCode
                CommonDAL commonDal = new CommonDAL();

                string AutoCode = commonDal.settingsDesktop("AutoCode", "Vendor",null,connVM);

                if (AutoCode == "Y")
                {
                    txtVendorCode.ReadOnly = true;
                    label24.Visible = false;

                }
                else
                {
                    label24.Visible = true;

                }
                #endregion

                #region ExtraRequiredFeildVisibilty
                string ExtraFeildVisibilty = commonDal.settingsDesktop("Menu", "ExtraRequiredField",null,connVM);
                if (ExtraFeildVisibilty == "N")
                {
                    chkRegister.Visible = false;
                    chkTurnover.Visible = false;
                    LVDS.Visible = false;
                    txtVDS.Visible = false;
                   
                }
                #endregion


                //btnAdd.Text = "&Add";
                txtVendorID.Text = "~~~ New ~~~";
                ChangeData = false;

                string value = new CommonDAL().settingValue("CompanyCode", "Code",connVM);
                btnSync.Visible = false;

                ////if (OrdinaryVATDesktop.IsACICompany(value)|| value.ToUpper()=="NESTLE")
                ////{
                ////    btnSync.Visible = true;
                ////}

                if (value.ToUpper() == "NESTLE")
                {
                    btnSync.Visible = true;
                }

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormVendor_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormVendor_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormVendor_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormVendor_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormVendor_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendor_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendor_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormVendor_Load", exMessage);
            }
            #endregion Catch
        }
        private void txtVendorName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtVendorGroupDesc_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtVATRegistrationNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtTINNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtTelephoneNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress2_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtAddress3_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtFaxNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPerson_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonDesignation_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonTelephone_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtContactPersonEmail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void FormVendor_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormVendor_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormVendor_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormVendor_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormVendor_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormVendor_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendor_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVendor_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormVendor_FormClosing", exMessage);
            }
            #endregion Catch
        }
        private void cmbVendorGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2))
                    {
                        VendorGroupSearch();
                        ClearAll();
                        //btnAdd.Text = "&Add";
                        txtVendorID.Text = "~~~ New ~~~";
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    VendorGroupSearch();
                    ClearAll();
                    //btnAdd.Text = "&Add";
                    txtVendorID.Text = "~~~ New ~~~";
                    ChangeData = false;
                }
                IsUpdate = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                ////MDIMainInterface mdi = new MDIMainInterface();
                FormRptVendorInformation frmRptVendorInformation = new FormRptVendorInformation();

                //mdi.RollDetailsInfo(frmRptVendorInformation.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVendorInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //frmRptVendorInformation.txtVendorID.Text = txtVendorID.Text.Trim();
                //frmRptVendorInformation.txtVendorName.Text = txtVendorName.Text.Trim();
                //frmRptVendorInformation.txtTINNo.Text = txtTINNo.Text.Trim();
                //frmRptVendorInformation.txtVATRegistrationNo.Text = txtVATRegistrationNo.Text.Trim();
                frmRptVendorInformation.ShowDialog();
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            #endregion Catch
        }
       //==============ADD VENDOR=============
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtVendorID.Text.Trim();
                }
                int ErR = ErrorReturn();

                if (ErR != 0)
                {
                    return;
                }

                activestatus = string.Empty;
                activestatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                SelectedVendorGroup = Convert.ToString(cmbVendorGroupName.Text.Trim());
                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerAdd.RunWorkerAsync();

                #endregion

            }
            #region Catch
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
            #endregion Catch
        }
        private void backgroundWorkerAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                
                // Start DoWork
                    //VendorDAL vendorDAL = new VendorDAL();
                IVendor vendorDAL = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);

                #region //Commented
                //sqlResults = vendorDAL.InsertToVendorNewSQL
                    //(NextID, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                    //txtAddress1.Text.Trim(), "-",
                    //"-", txtCity.Text.Trim(),
                    //txtTelephoneNo.Text.Trim(), txtFaxNo.Text.Trim(),
                    //txtEmail.Text.Trim(),
                    //dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"),
                    //txtContactPerson.Text.Trim(), txtContactPersonDesignation.Text.Trim(),
                    //txtContactPersonTelephone.Text.Trim(), txtContactPersonEmail.Text.Trim(),
                    //txtVATRegistrationNo.Text.Trim(), txtTINNo.Text.Trim(),
                    //txtComments.Text.Trim(), activestatus,
                    //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                    //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                    //txtCountry.Text.Trim(),txtVendorCode.Text.Trim(), txtVDS.Text.Trim()
                    //, txtBusinessType.Text.Trim(), txtBusinessCode.Text.Trim()
                //); 
                #endregion

                #region model binding
                VendorVM nvm = new VendorVM();
                    nvm.VendorID = NextID.ToString();
                    nvm.VendorName = txtVendorName.Text.Trim();
                    nvm.VendorGroupID = txtVendorGroupID.Text.Trim();
                    nvm.Address1 = txtAddress1.Text.Trim();
                    nvm.Address2 = "-";
                    nvm.Address3 = "-";
                    nvm.City = txtCity.Text.Trim();
                    nvm.TelephoneNo = txtTelephoneNo.Text.Trim();
                    nvm.FaxNo = txtFaxNo.Text.Trim();
                    nvm.Email = txtEmail.Text.Trim();
                    nvm.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    nvm.ContactPerson = txtContactPerson.Text.Trim();
                    nvm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                    nvm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                    nvm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                    nvm.VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
                    nvm.NIDNo = txtNIDNo.Text.Trim();
                    nvm.TINNo = txtTINNo.Text.Trim();
                    nvm.Comments = txtComments.Text.Trim();
                    nvm.ActiveStatus = activestatus.ToString();
                    nvm.CreatedBy = Program.CurrentUser;
                    nvm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    nvm.Country = txtCountry.Text.Trim();
                    nvm.VendorCode = txtVendorCode.Text.Trim();
                    nvm.VDSPercent =Convert.ToDecimal(txtVDS.Text.Trim());
                    nvm.BusinessType = txtBusinessType.Text.Trim();
                    nvm.BusinessCode = txtBusinessType.Text.Trim();

                    nvm.IsVDSWithHolder = chkVDSWithHolder.Checked ? "Y" : "N";
                    nvm.IsRegister = chkRegister.Checked ? "Y" : "N";
                    nvm.IsTurnover = chkTurnover.Checked ? "Y" : "N";
                    nvm.BranchId = Program.BranchId;
                    nvm.ShortName = txtShortName.Text.Trim();

                    #endregion
                    sqlResults = vendorDAL.InsertToVendorNewSQL(nvm,false,null,null,connVM);

                    SAVE_DOWORK_SUCCESS = true;
                 
                // End DoWork

                #endregion
            } 
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            #endregion Catch
        }
        private void backgroundWorkerAdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            txtVendorID.Text = newId;
                            txtVendorCode.Text = code;
                            fnVendorAddress(txtVendorID.Text.Trim());
                         
                        }

                    }
                #endregion
                ChangeData = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }
            
        }
        //====================================
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //============DELETE VENDOR===========
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (SearchBranchId != Program.BranchId && SearchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                else if (txtVendorID.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (txtVendorID.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                     MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
                else if (MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                int ErR = DataAlreadyUsed();
                if (ErR != 0)
                {
                    return;
                }
                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerDelete.RunWorkerAsync();

              }
            #region Catch
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
            #endregion Catch
        }
        private int DataAlreadyUsed()
        {
            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                if (commonDal.DataAlreadyUsed("VDS", "VendorId", txtVendorID.Text.Trim(), null, null,connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " VDS" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
                }
                if (commonDal.DataAlreadyUsed("PurchaseInvoiceHeaders", "VendorID", txtVendorID.Text.Trim(), null, null,connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Purchase" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }

            #endregion

            return 0;
        }
        private void backgroundWorkerDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //resultDelete = VendorDAL.DeleteVendor(txtVendorID.Text.Trim(), Program.DatabaseName);  // Change 04

                #region Statement
                // Start DoWork
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //VendorDAL vendorDAL = new VendorDAL();
                IVendor vendorDAL = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);


                VendorVM vm = new VendorVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString();
                var ids = new string[] { txtVendorID.Text.Trim(), "" };
                sqlResults = vendorDAL.Delete(vm, ids,null,null,connVM);
                //sqlResults = vendorDAL.DeleteVendorInformation(txtVendorID.Text.Trim());
                // End DoWork
                DELETE_DOWORK_SUCCESS = true;
                #endregion
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }
            #endregion Catch
        }
        private void backgroundWorkerDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                
                if (DELETE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string recId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerDelete_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            ClearAll();
                            IsUpdate = false;
                          
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }

                #endregion
                ChangeData = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerVendorGrpSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
            finally 
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }
        //============UPDATE VENDOR===========
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
              
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtVendorID.Text.Trim();
                }
                if (SearchBranchId != Program.BranchId && SearchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }


                activestatus = string.Empty;
                activestatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerUpdate.RunWorkerAsync();
            }
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
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion


        }
        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                // Start DoWork
                //VendorDAL vendorDAL = new VendorDAL();

                IVendor vendorDAL = OrdinaryVATDesktop.GetObject<VendorDAL, VendorRepo, IVendor>(OrdinaryVATDesktop.IsWCF);
                #region //Commented
                //sqlResults = vendorDAL.UpdateVendorNewSQL(NextID, txtVendorName.Text.Trim(), txtVendorGroupID.Text.Trim(),
                //txtAddress1.Text.Trim(), "-",
                //"-", txtCity.Text.Trim(),
                //txtTelephoneNo.Text.Trim(), txtFaxNo.Text.Trim(),
                //txtEmail.Text.Trim(),
                //dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtContactPerson.Text.Trim(), txtContactPersonDesignation.Text.Trim(),
                //txtContactPersonTelephone.Text.Trim(), txtContactPersonEmail.Text.Trim(),
                //txtVATRegistrationNo.Text.Trim(), txtTINNo.Text.Trim(),
                //txtComments.Text.Trim(), activestatus,
                //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //Program.CurrentUser, DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //txtCountry.Text.Trim(),txtVendorCode.Text.Trim(), txtVDS.Text.Trim()
                //, txtBusinessType.Text.Trim(), txtBusinessCode.Text.Trim()
                //);  
                // Change 04
                #endregion

                #region model binding
                VendorVM nvm = new VendorVM();
                nvm.VendorID = NextID.ToString();
                nvm.VendorName = txtVendorName.Text.Trim();
                nvm.VendorGroupID = txtVendorGroupID.Text.Trim();
                nvm.Address1 = txtAddress1.Text.Trim();
                nvm.Address2 = "-";
                nvm.Address3 = "-";
                nvm.City = txtCity.Text.Trim();
                nvm.TelephoneNo = txtTelephoneNo.Text.Trim();
                nvm.FaxNo = txtFaxNo.Text.Trim();
                nvm.Email = txtEmail.Text.Trim();
                nvm.StartDateTime = dtpStartDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                nvm.ContactPerson = txtContactPerson.Text.Trim();
                nvm.ContactPersonDesignation = txtContactPersonDesignation.Text.Trim();
                nvm.ContactPersonTelephone = txtContactPersonTelephone.Text.Trim();
                nvm.ContactPersonEmail = txtContactPersonEmail.Text.Trim();
                nvm.VATRegistrationNo = txtVATRegistrationNo.Text.Trim();
                nvm.NIDNo = txtNIDNo.Text.Trim();
                nvm.TINNo = txtTINNo.Text.Trim();
                nvm.Comments = txtComments.Text.Trim();
                nvm.ActiveStatus = activestatus.ToString();
                nvm.LastModifiedBy = Program.CurrentUser;
                nvm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                nvm.Country = txtCountry.Text.Trim();
                nvm.VendorCode = txtVendorCode.Text.Trim();
                nvm.VDSPercent = Convert.ToDecimal(txtVDS.Text.Trim());
                nvm.BusinessType = txtBusinessType.Text.Trim();
                nvm.BusinessCode = txtBusinessType.Text.Trim();

                nvm.IsVDSWithHolder = chkVDSWithHolder.Checked ? "Y" : "N";
                nvm.IsRegister = chkRegister.Checked ? "Y" : "N";
                nvm.IsTurnover = chkTurnover.Checked ? "Y" : "N";
                nvm.ShortName = txtShortName.Text.Trim();
                #endregion
                sqlResults = vendorDAL.UpdateVendorNewSQL(nvm,connVM);

                UPDATE_DOWORK_SUCCESS = true;

                // End DoWork

                #endregion

            }
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            txtVendorID.Text = newId;
                            txtVendorCode.Text = code;
                         
                        }

                    }

                ChangeData = false;
                this.btnUpdate.Visible = true;
                this.progressBar1.Visible = false;
                #endregion

            }
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
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }
            
        }

        private void txtVDS_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVDS, "VDS Percent"); 
            txtVDS.Text = Program.ParseDecimalObject(txtVDS.Text.Trim()).ToString();
        }

        private void txtVDS_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtVDS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtCountry_KeyDown(object sender, KeyEventArgs e)
        {
            
               // btnAdd.Focus();
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
            
        }

        private void txtVendorCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtCountry_Leave(object sender, EventArgs e)
        {
            btnAdd.Focus();
        }

        private void txtTDSCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                TDSLoad();
            }
        }
        
        private void TDSLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" select distinct Code, Description ,MinValue MinValue, MaxValue MaxValue, Rate  from TDSs where 1=1  ";
                string[] shortColumnName = { "Code", "Description" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    ////vTDSCode = "0";
                    //////vTDSDescription  = "";
                    //////txtTDSCode.Text = "";
                    //////vTDSCode = selectedRow.Cells["Code"].Value.ToString();
                    //////vTDSDescription = selectedRow.Cells["Description"].Value.ToString();//ProductInfo[0];
                    //////txtTDSCode.Text = vTDSCode;
                    //////txtTDSCode.Focus();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "customerLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
        }

        private void txtTDSCode_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.txtTDSCode, vTDSDescription);
        }

        private void txtTDSCode_DoubleClick(object sender, EventArgs e)
        {
            TDSLoad();
        }

        private void txtTDSCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

               
                bgwVendorSync.RunWorkerAsync();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }
            finally
            {

            }
        }

        private void bgwVendorSync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();
                CommonDAL commonDAL = new CommonDAL();
                DataTable vendorsDt = new DataTable();

                string code = commonDAL.settingValue("CompanyCode", "Code");
                if (code.ToUpper() != "NESTLE")
                {
                     vendorsDt = importDal.GetVendorACIDbData(settingVM.BranchInfoDT,connVM);
                }
                else
                {
                    vendorsDt = importDal.GetVendorNesatleDbData(settingVM.BranchInfoDT,connVM);
                }
                results[0] = "fail";

                List<VendorVM> vendors = new List<VendorVM>();

                int rowsCount = vendorsDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonDAL.settingsDesktop("AutoSave", "DefaultVendorGroup",null,connVM);
                //if(defaultGroup == "-")
                //{
                //    MessageBox.Show("Default Vendor Group Not Found");
                //}

                for (int i = 0; i < rowsCount; i++)
                {
                    VendorVM vendor = new VendorVM();
                    vendor.VendorCode = Program.RemoveStringExpresion(vendorsDt.Rows[i]["VendorCode"].ToString());
                    vendor.VendorName = Program.RemoveStringExpresion(vendorsDt.Rows[i]["VendorName"].ToString());

                    vendor.VendorGroup = vendorsDt.Rows[i]["VendorGroup"].ToString();

                    if (vendor.VendorGroup == "-")
                    {
                        if (defaultGroup == "-")
                        {
                            throw new Exception("Default Vendor Group Not Found.\nPlease set Default Vendor Group in Setting .");
                        }
                        vendor.VendorGroup = defaultGroup;
                    }

                    vendor.Address1 = vendorsDt.Rows[i]["Address"].ToString();
                    vendor.Address2 = "-";
                    vendor.Address3 = "-";

                    vendor.City = "-";
                    vendor.TelephoneNo = vendorsDt.Rows[i]["TelephoneNo"].ToString();
                    if (code.ToUpper() != "NESTLE")
                    {
                        vendor.FaxNo = "-";
                        vendor.Email = "-";
                        vendor.ContactPerson = "-";
                        vendor.TINNo = "-";


                    }
                    else
                    {
                        vendor.FaxNo = vendorsDt.Rows[i]["FaxNo"].ToString();
                        vendor.Email = vendorsDt.Rows[i]["Email"].ToString();
                        vendor.ContactPerson = Program.RemoveStringExpresion(vendorsDt.Rows[i]["ContactPerson"].ToString()); ;
                        vendor.TINNo = vendorsDt.Rows[i]["TINNo"].ToString();
                    }
                    vendor.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    vendor.ContactPersonDesignation = "-";
                    vendor.ContactPersonTelephone = "-";
                    vendor.ContactPersonEmail = "-";
                    vendor.VATRegistrationNo = vendorsDt.Rows[i]["BIN_No"].ToString();
                    vendor.Comments = "-";
                    vendor.ActiveStatus = "Y";
                    vendor.CreatedBy = Program.CurrentUser;
                    vendor.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vendor.Country = "-";
                    vendor.BranchId = Program.BranchId;
                    vendors.Add(vendor);

                    ids.Add(vendorsDt.Rows[i]["SL"].ToString());
                }


                results = importDal.ImportVendor(vendors);
                
                    if (results[0].ToLower() == "success")
                    {
                        if (code.ToUpper() != "NESTLE")
                        {
                            results = importDal.UpdateACIMaster(ids, settingVM.BranchInfoDT,"Vendors",connVM);
                        }
                        else
                        {
                            results = importDal.UpdateNestleMaster(ids, settingVM.BranchInfoDT,"Vendors",connVM);

                        }
                    }
                
            }
            catch (Exception ex)
            {
                results[0] = "exception";
                results[1] = ex.Message;

                FileLogger.Log(this.Name, "ItegrationVendor",
                    ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void bgwVendorSync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (results[0].ToLower() == "success")
            {
                MessageBox.Show("Synchronized");
            }
            else if (results[0].ToLower() == "exception")
            {
                MessageBox.Show(results[1]);
            }
            else
            {
                MessageBox.Show("Nothing to Syncronize");

            }


            progressBar1.Visible = false;

        }

        private void txtVendorGroupID_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {


                sqlResults = new string[4];

                VendorAddressVM vm = new VendorAddressVM();
                vm.Id = Convert.ToInt32(vVendorAddressId);
                vm.VendorID = vVendorId;
                vm.VendorAddress = txtVendorAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                //ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                //sqlResults = cDal.DeleteCustomerAddress("", vm.Id.ToString(), null, null, connVM);
                sqlResults = new VendorDAL().DeleteVendorAddress("", vm.Id.ToString(), null, null, connVM);

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }
                    fnVendorAddress(vVendorId);

                }
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void dgvVendorAddress_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!IsUpdate)
                {
                    MessageBox.Show("Please Add/ Search vendor first");
                    return;
                }

                sqlResults = new string[4];

                VendorAddressVM vm = new VendorAddressVM();
                vm.Id = Convert.ToInt32(vVendorAddressId);
                vm.VendorID = txtVendorID.Text.Trim();
                vm.VendorAddress = txtVendorAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                //ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);
                //sqlResults = cDal.InsertToCustomerAddress(vm, null, null, connVM);
                sqlResults = new VendorDAL().InsertToVendorAddress(vm, null, null, connVM);
                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }

                    fnVendorAddress(vm.VendorID);

                }
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

        }

        private void fnVendorAddress(string vendorId)
        {
            try
            {
                VendorAddressResult = new DataTable();
                //CustomerDAL customerDal = new CustomerDAL();
                //ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                //VendorAddressResult = customerDal.SearchCustomerAddress(vendorId, "", "", connVM); // Change 04
                VendorAddressResult = new VendorDAL().SearchVendorAddress(vendorId, "", "", connVM); // Change 04

                dgvVendorAddress.Rows.Clear();
                int j = 0;
                foreach (DataRow item in VendorAddressResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvVendorAddress.Rows.Add(NewRow);


                    dgvVendorAddress.Rows[j].Cells["Sl"].Value = (j + 1).ToString();
                    dgvVendorAddress.Rows[j].Cells["Id"].Value = item["Id"].ToString();
                    dgvVendorAddress.Rows[j].Cells["VendorID"].Value = item["VendorID"].ToString();
                    dgvVendorAddress.Rows[j].Cells["VendorAddress"].Value = item["VendorAddress"].ToString();
                    j = j + 1;

                }
                txtVendorAddress.Text = "";
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {


                sqlResults = new string[4];

                VendorAddressVM vm = new VendorAddressVM();
                vm.Id = Convert.ToInt32(vVendorAddressId);
                vm.VendorID = vVendorId;
                vm.VendorAddress = txtVendorAddress.Text;

                //CustomerDAL cDal = new CustomerDAL();
                //ICustomer cDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);
                //sqlResults = cDal.UpdateToCustomerAddress(vm, null, null, connVM);
                sqlResults = new VendorDAL().UpdateToVendorAddress(vm, null, null, connVM);


                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string code = sqlResults[3];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }
                    fnVendorAddress(vVendorId);

                }
            }
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
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void dgvVendorAddress_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dgvVendorAddress.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
            {
                DataGridViewRow userSelRow = selectedRows[0];
                vVendorAddressId = userSelRow.Cells["Id"].Value.ToString();
                vVendorId = userSelRow.Cells["VendorID"].Value.ToString();
                txtVendorAddress.Text = userSelRow.Cells["VendorAddress"].Value.ToString();
            }
        }
        //====================================
      }
}
