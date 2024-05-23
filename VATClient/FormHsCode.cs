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
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using VATClient.ReportPages;
using System.Drawing.Printing;
using System.Threading;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormHsCode : Form
    {
        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string[] sqlResults;
        string NextID;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataTable YearResult;
        private int YearLines;

        DataSet ds;
        DataTable dt;

        #endregion
        public FormHsCode()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                Program.fromOpen = "Me";
                string result = FormHSCodeSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] HSCodeInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtId.Text = HSCodeInfo[0];
                    //HSCodeDAL HDAL = new HSCodeDAL();
                    IHSCode HDAL = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                    DataTable HSCodeResult = new DataTable();

                    string[] cFields = { "Id" };
                    string[] cValues = { HSCodeInfo[0] };
                    HSCodeResult = HDAL.SelectAll(0, cFields, cValues, null, null, true,connVM);

                    txtCode.Text = HSCodeResult.Rows[0]["Code"].ToString();
                    txtHSCode.Text = HSCodeResult.Rows[0]["HSCode"].ToString();// HSCodeInfo[2];
                    cmbFiscalyear.Text = HSCodeResult.Rows[0]["FiscalYear"].ToString();// HSCodeInfo[2];
                    txtDescription.Text = HSCodeResult.Rows[0]["Description"].ToString();// HSCodeInfo[3];

                    txtCD.Text = Program.ParseDecimalObject(HSCodeResult.Rows[0]["CD"].ToString());// HSCodeInfo[4];
                    txtRD.Text = Program.ParseDecimalObject(HSCodeResult.Rows[0]["RD"].ToString());// HSCodeInfo[6];
                    txtSD.Text = Program.ParseDecimalObject(HSCodeResult.Rows[0]["SD"].ToString());// HSCodeInfo[5];
                    txtVAT.Text =Program.ParseDecimalObject(HSCodeResult.Rows[0]["VAT"].ToString());// HSCodeInfo[8];
                    txtAT.Text = Program.ParseDecimalObject(HSCodeResult.Rows[0]["AT"].ToString());// HSCodeInfo[7];
                    txtAIT.Text =Program.ParseDecimalObject(HSCodeResult.Rows[0]["AIT"].ToString());// HSCodeInfo[9];

                    txtOtherSD.Text = Program.ParseDecimalObject(HSCodeResult.Rows[0]["OtherSD"].ToString());// HSCodeInfo[10];
                    txtOtherVAT.Text =Program.ParseDecimalObject(HSCodeResult.Rows[0]["OtherVAT"].ToString());// HSCodeInfo[11];

                    chkOtherVAT.Checked = (HSCodeResult.Rows[0]["IsFixedOtherVAT"].ToString()) == "Y" ? true : false;
                    chkOtherCD.Checked = (HSCodeResult.Rows[0]["IsFixedOtherSD"].ToString()) == "Y" ? true : false;

                    chkSD.Checked = (HSCodeResult.Rows[0]["IsFixedSD"].ToString()) == "Y" ? true : false;
                    chkCD.Checked = (HSCodeResult.Rows[0]["IsFixedCD"].ToString()) == "Y" ? true : false;
                    chkRD.Checked = (HSCodeResult.Rows[0]["IsFixedRD"].ToString()) == "Y" ? true : false;
                    chkVAT.Checked = (HSCodeResult.Rows[0]["IsFixedVAT"].ToString()) == "Y" ? true : false;
                    chkAT.Checked = (HSCodeResult.Rows[0]["IsFixedAT"].ToString()) == "Y" ? true : false;
                    chkAIT.Checked = (HSCodeResult.Rows[0]["IsFixedAIT"].ToString()) == "Y" ? true : false;
                    chkIsVDS.Checked = HSCodeResult.Rows[0]["IsVDS"].ToString() == "Y";

                    txtComments.Text = HSCodeResult.Rows[0]["Comments"].ToString();
             
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
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch

            finally
            {
               
            }

        }
        private void ClearAll()
        {
            #region Try
            try
            {
                txtCode.Text = "~~~ New ~~~";
                txtAIT.Text = "0.00";
                txtAT.Text = "0.00";
                txtCD.Text = "0.00";
                txtSD.Text = "0.00";
                txtVAT.Text = "0.00";
                txtRD.Text = "0.00";
                txtOtherSD.Text = "0.00"; 
                txtOtherVAT.Text = "0.00";
                txtDescription.Text = "";
                txtComments.Text = "";
                txtHSCode.Text = "";
                txtId.Text = string.Empty; ;
                chkOtherVAT.Checked = false;
                chkSD.Checked = false;
                chkRD.Checked = false;
                chkCD.Checked = false;
                chkAIT.Checked = false;
                chkVAT.Checked = false;
                chkAT.Checked = false;
                chkOtherCD.Checked = false;
            }

            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                ClearAll();
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRD_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAIT_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVAT_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAT_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormHsCode_Load(object sender, EventArgs e)
        {
            #region Try
            try
            {
                SelectYear();
                ClearAll();
                
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {

                if (string.IsNullOrWhiteSpace(txtHSCode.Text.Trim()))
                {
                    MessageBox.Show("Please enter HSCode");
                  
                }
                NextID = txtId.Text.Trim();
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = true;

                HSCodeVM vm = new HSCodeVM();

                vm.Code = txtCode.Text.Trim();
                vm.HSCode = txtHSCode.Text.Trim();
                vm.FiscalYear = cmbFiscalyear.Text.Trim();
                vm.Description = txtDescription.Text.Trim();
                vm.Comments = txtComments.Text.Trim();

                vm.CD = Convert.ToDecimal((txtCD.Text.Trim()));
                vm.RD = Convert.ToDecimal((txtRD.Text.Trim()));
                vm.SD = Convert.ToDecimal((txtSD.Text.Trim()));
                vm.VAT = Convert.ToDecimal((txtVAT.Text.Trim()));
                vm.AT = Convert.ToDecimal((txtAT.Text.Trim()));
                vm.AIT = Convert.ToDecimal((txtAIT.Text.Trim()));

                vm.IsFixedCD = Convert.ToString(chkCD.Checked ? "Y" : "N");
                vm.IsFixedRD = Convert.ToString(chkRD.Checked ? "Y" : "N");
                vm.IsFixedSD = Convert.ToString(chkSD.Checked ? "Y" : "N");
                vm.IsFixedVAT = Convert.ToString(chkVAT.Checked ? "Y" : "N");
                vm.IsFixedAT = Convert.ToString(chkAT.Checked ? "Y" : "N");
                vm.IsFixedAIT = Convert.ToString(chkAIT.Checked ? "Y" : "N");

                vm.OtherSD = Convert.ToDecimal((txtOtherSD.Text.Trim()));
                vm.OtherVAT = Convert.ToDecimal((txtOtherVAT.Text.Trim()));

                vm.IsFixedOtherVAT = Convert.ToString(chkOtherVAT.Checked ? "Y" : "N");
                vm.IsFixedOtherSD = Convert.ToString(chkOtherCD.Checked ? "Y" : "N");

                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                vm.IsVDS = chkIsVDS.Checked ? "Y" : "N";

                bgwSave.RunWorkerAsync(vm);
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                //HSCodeDAL Hdal = new HSCodeDAL();
                IHSCode Hdal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                HSCodeVM vm = (HSCodeVM)e.Argument;

                sqlResults = Hdal.InsertToHSCode(vm,connVM);
                SAVE_DOWORK_SUCCESS = true;
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
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
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        txtId.Text = newId;
                        txtCode.Text = code;

                    }

                this.btnAdd.Visible = true;
                this.progressBar1.Visible = false;
              
                #endregion
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHSCode.Text.Trim()))
            {
                MessageBox.Show("Please enter HSCode");

            }
            NextID = txtId.Text.Trim();
            this.btnUpdate.Visible = true;
            this.progressBar1.Visible = true;

            HSCodeVM vm = new HSCodeVM();
            vm.Id = Convert.ToInt32(NextID);
            vm.Code = txtCode.Text.Trim();
            vm.HSCode = txtHSCode.Text.Trim();
            vm.FiscalYear = cmbFiscalyear.Text.Trim();
            vm.Description = txtDescription.Text.Trim();
            vm.Comments = txtComments.Text.Trim();
            vm.CD = Convert.ToDecimal((txtCD.Text.Trim()));
            vm.RD = Convert.ToDecimal((txtRD.Text.Trim()));
            vm.SD = Convert.ToDecimal((txtSD.Text.Trim()));
            vm.VAT = Convert.ToDecimal((txtVAT.Text.Trim()));
            vm.AT = Convert.ToDecimal((txtAT.Text.Trim()));
            vm.AIT = Convert.ToDecimal((txtAIT.Text.Trim()));

            vm.IsFixedCD = Convert.ToString(chkCD.Checked ? "Y" : "N");
            vm.IsFixedRD = Convert.ToString(chkRD.Checked ? "Y" : "N");
            vm.IsFixedSD = Convert.ToString(chkSD.Checked ? "Y" : "N");
            vm.IsFixedVAT = Convert.ToString(chkVAT.Checked ? "Y" : "N");
            vm.IsFixedAT = Convert.ToString(chkAT.Checked ? "Y" : "N");
            vm.IsFixedAIT = Convert.ToString(chkAIT.Checked ? "Y" : "N");

            vm.OtherSD = Convert.ToDecimal((txtOtherSD.Text.Trim()));
            vm.OtherVAT = Convert.ToDecimal((txtOtherVAT.Text.Trim()));

            vm.IsFixedOtherVAT = Convert.ToString(chkOtherVAT.Checked ? "Y" : "N");
            vm.IsFixedOtherSD = Convert.ToString(chkOtherCD.Checked ? "Y" : "N");

            vm.LastModifiedBy = Program.CurrentUser;
            vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

            vm.IsVDS = chkIsVDS.Checked ? "Y" : "N";

            bgwUpdate.RunWorkerAsync(vm);
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
           
            #region Try

            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //HSCodeDAL Hdal = new HSCodeDAL();
                IHSCode Hdal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);

                HSCodeVM vm = (HSCodeVM)e.Argument;
                sqlResults = Hdal.UpdateHSCode(vm,connVM);
                UPDATE_DOWORK_SUCCESS = true;
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
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
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        txtId.Text = newId;
                        txtCode.Text = code;

                    }
                         this.btnUpdate.Visible = true;
                         this.progressBar1.Visible = false;
              

                #endregion
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void bgwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                #region Statement

               

                // Start DoWork
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //HSCodeDAL Hdal = new HSCodeDAL();
                IHSCode Hdal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                HSCodeVM vm = new HSCodeVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                string[] ids = new string[] { txtId.Text.Trim(), "" };

                sqlResults = Hdal.Delete(vm, ids,null,null,connVM); 
                
                // End DoWork
                DELETE_DOWORK_SUCCESS = true;

                #endregion
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void bgwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            try
            {
                #region Statement
                if (DELETE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwDelete_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearAll();
                        }
                        this.btnDelete.Visible = true;
                        this.progressBar1.Visible = false;
                        this.btnDelete.Enabled = true;
                    }

          
                #endregion
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            #region try 
            try
            {
                if (txtId.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (txtId.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (
                    MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;
                bgwDelete.RunWorkerAsync();
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void chkIsFixedVAT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
            
        }
        private void Lebelchange()
        {
            chkOtherVAT.Text = "VAT (%)";
            if (chkOtherVAT.Checked)
            {
                chkOtherVAT.Text = "VAT (F)";
            }
        chkCD.Text = "CD (%)";
        if (chkCD.Checked)
        {
        chkCD.Text = "CD (F)";
        }
        chkVAT.Text = "VAT (%)";
        if (chkVAT.Checked)
        {
        chkVAT.Text = "VAT (F)";
        }
        chkAIT.Text = "AIT (%)";
        if (chkAIT.Checked)
        {
        chkAIT.Text = "AIT (F)";
        }
        chkRD.Text = "RD (%)";
        if (chkRD.Checked)
        {
        chkRD.Text = "RD (F)";
        }
        chkAT.Text = "AT (%)";
        if (chkAT.Checked)
        {
        chkAT.Text = "AT (F)";
        }
        chkOtherCD.Text = "SD (%)";
        if (chkOtherCD.Checked)
        {
        chkOtherCD.Text = "SD (F)";
        }
        chkSD.Text = "SD (%)";
        if (chkSD.Checked)
        {
        chkSD.Text = "SD (F)";
        }
        }


        private void chkCD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkVAT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkAIT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkRD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkAT_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkLocalCD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void chkSD_CheckedChanged(object sender, EventArgs e)
        {
            Lebelchange();
        }

        private void txtCD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtCD, "CD");
            txtCD.Text = Program.ParseDecimalObject(txtCD.Text.Trim()).ToString();
        }

        private void txtRD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtRD, "RD");
            txtRD.Text = Program.ParseDecimalObject(txtRD.Text.Trim()).ToString();
        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtSD, "SD");
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();
        }

        private void txtVAT_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtVAT, "VAT");
            txtVAT.Text = Program.ParseDecimalObject(txtVAT.Text.Trim()).ToString();
        }

        private void txtAT_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtAT, "AT");
            txtAT.Text = Program.ParseDecimalObject(txtAT.Text.Trim()).ToString();
        }

        private void txtAIT_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtAIT, "AIT");
            txtAIT.Text = Program.ParseDecimalObject(txtAIT.Text.Trim()).ToString();
        }

        private void txtOtherSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtOtherSD, "OtherSD");
            txtOtherSD.Text = Program.ParseDecimalObject(txtOtherSD.Text.Trim()).ToString();
        }

        private void txtOtherVAT_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtOtherVAT, "OtherVAT");
            txtOtherVAT.Text = Program.ParseDecimalObject(txtOtherVAT.Text.Trim()).ToString();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Program.ImportFileName = null;

                if (string.IsNullOrWhiteSpace(Program.ImportFileName))
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    //MessageBox.Show(this, "Please select the right file for import");
                    return;
                }
                else
                {
                    string[] extention = fileName.Split(".".ToCharArray());

                    if (extention[extention.Length - 1] == "xls" || extention[extention.Length - 1] == "xlsx")
                    {
                        ds = loadExcel();
                        dt = ds.Tables["HSCode"];

                        Program.ImportFileName = null;

                        //HSCodeDAL HSCodeDal = new HSCodeDAL();
                        IHSCode HSCodeDal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);

                        string Createdby = Program.CurrentUser;
                        string Createdon = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HSCodeVM vm = new HSCodeVM();

                            vm.HSCode = dt.Rows[i]["HSCode"].ToString();
                            vm.CD =Convert.ToDecimal(dt.Rows[i]["CD"].ToString());
                            vm.VAT = Convert.ToDecimal(dt.Rows[i]["VAT"].ToString());
                            vm.SD = Convert.ToDecimal(dt.Rows[i]["SD"].ToString());
                            vm.AIT = Convert.ToDecimal(dt.Rows[i]["AIT"].ToString());
                            vm.RD = Convert.ToDecimal(dt.Rows[i]["RD"].ToString());
                            vm.AT = Convert.ToDecimal(dt.Rows[i]["AT"].ToString());
                            vm.OtherSD = Convert.ToDecimal(dt.Rows[i]["OtherSD"].ToString());
                            vm.OtherVAT = Convert.ToDecimal(dt.Rows[i]["OtherVAT"].ToString());
                            vm.IsFixedVAT = dt.Rows[i]["IsFixedVAT"].ToString();
                            vm.IsFixedSD = dt.Rows[i]["IsFixedSD"].ToString();
                            vm.IsFixedCD = dt.Rows[i]["IsFixedCD"].ToString();
                            vm.IsFixedRD = dt.Rows[i]["IsFixedRD"].ToString();
                            vm.IsFixedAIT = dt.Rows[i]["IsFixedAIT"].ToString();
                            vm.IsFixedAT = dt.Rows[i]["IsFixedAT"].ToString();
                            vm.IsFixedOtherVAT = dt.Rows[i]["IsFixedOtherVAT"].ToString();
                            vm.IsFixedOtherSD = dt.Rows[i]["IsFixedOtherSD"].ToString();
                            vm.Description = dt.Rows[i]["Description"].ToString();
                            vm.Comments = dt.Rows[i]["Comments"].ToString();

                            vm.CreatedBy = Createdby;
                            vm.CreatedOn = Createdon;

                            vm.LastModifiedBy = Createdby;
                            vm.LastModifiedOn = Createdon;


                            sqlResults = HSCodeDal.InsertfromExcel(vm,connVM);
                            
                        }

                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (extention[extention.Length - 1] != "xls" || extention[extention.Length - 1] != "xlsx")
                    {
                        MessageBox.Show("You can select Excel files only");
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private DataSet loadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                //IHSCode HSCodeDal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                ds = new ImportDAL().GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            return ds;
        }

        private void BrowsFile()
        {
            try
            {

                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "HSCode Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
       
                //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //BugsBD
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*|CSV files (*.csv*)|*.csv*";

                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }

                else
                {
                    return;
                }



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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

        }

        private void SelectYear()
        {
            try
            {
               // this.progressBar.Visible = true;
                bgwSelectYear.RunWorkerAsync();
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

        private void bgwSelectYear_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // Start DoWork
                YearResult = new DataTable();
                //FiscalYearDAL fiscalYearDal = new FiscalYearDAL();
                IFiscalYear fiscalYearDal = OrdinaryVATDesktop.GetObject<FiscalYearDAL, FiscalYearRepo, IFiscalYear>(OrdinaryVATDesktop.IsWCF);
                YearResult = fiscalYearDal.SearchYear(connVM);
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwSelectYear_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                YearLines = 0;
                YearLines = YearResult.Rows.Count;
                cmbFiscalyear.Items.Clear();
                for (int j = 0; j < Convert.ToInt32(YearLines); j++)
                {
                    cmbFiscalyear.Items.Add(YearResult.Rows[j][0].ToString());
                }
                cmbFiscalyear.Text = (DateTime.Now.ToString("yyyy"));
                // End Complete
                #endregion
                //////ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

       

       
    }
}
