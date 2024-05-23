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
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormSalesInvoiceExp : Form
    {
        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string[] sqlResults;
        string NextID;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        public FormSalesInvoiceExp()
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
                string result = FormSalesInvoiceExpSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] SalesInvoiceExpInfo = result.Split(FieldDelimeter.ToCharArray());
                    txtId.Text = SalesInvoiceExpInfo[0];
                    dtpLCDate.Text = SalesInvoiceExpInfo[1];
                    txtLCBank.Text = SalesInvoiceExpInfo[2];
                    txtPINo.Text = SalesInvoiceExpInfo[3];
                    dtpPIDate.Text = SalesInvoiceExpInfo[4];
                    txtEXPNo.Text = SalesInvoiceExpInfo[5];
                    dtpEXPDate.Text = SalesInvoiceExpInfo[6];
                    txtPortFrom.Text = SalesInvoiceExpInfo[7];
                    txtPortTo.Text = SalesInvoiceExpInfo[8];
                    txtLCNumber.Text = SalesInvoiceExpInfo[9];
                    txtRemarks.Text = SalesInvoiceExpInfo[10];
                


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
                txtId.Text = string.Empty;
                txtLCBank.Text = "";
                txtLCBank.Text = "";
                txtPINo.Text = "";
                txtEXPNo.Text = "";
                txtRemarks.Text = "";
                txtPortFrom.Text = "";
                txtPortTo.Text = "";
                txtLCNumber.Text = "";
                
              
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



        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormHsCode_Load(object sender, EventArgs e)
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {

                if (string.IsNullOrWhiteSpace(txtPINo.Text.Trim()))
                {
                    MessageBox.Show("Please enter PI No");

                }
                //NextID = Convert.ToString(txtId.Text.Trim());
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = true;
                bgwSave.RunWorkerAsync();
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
                //SalesInvoiceExpDAL Sdal = new SalesInvoiceExpDAL();
                ISalesInvoiceExp Sdal = OrdinaryVATDesktop.GetObject<SalesInvoiceExpDAL, SalesInvoiceExpRepo, ISalesInvoiceExp>(OrdinaryVATDesktop.IsWCF);

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                SalesInvoiceExpVM vm = new SalesInvoiceExpVM();
                //vm.ID = Convert.ToInt32(NextID);
                vm.LCDate = dtpLCDate.Text.Trim();
                vm.LCBank = txtLCBank.Text.Trim();
                vm.PINo = txtPINo.Text.Trim();
                vm.PIDate = dtpPIDate.Text.Trim();
                vm.EXPNo = txtEXPNo.Text.Trim();
                vm.EXPDate = dtpEXPDate.Text.Trim();
                vm.Remarks = txtRemarks.Text.Trim();
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.PortFrom = txtPortFrom.Text.Trim();
                vm.PortTo = txtPortTo.Text.Trim();
                vm.LCNumber = txtLCNumber.Text.Trim();

                sqlResults = Sdal.InsertToSalesInvoiceExp(vm,connVM);
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
                        //txtCode.Text = code;

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
            if (string.IsNullOrWhiteSpace(txtPINo.Text.Trim()))
            {
                MessageBox.Show("Please enter PI No");

            }
            NextID = Convert.ToString(txtId.Text.Trim());
            this.btnUpdate.Visible = true;
            this.progressBar1.Visible = true;
            bgwUpdate.RunWorkerAsync();
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
           
            #region Try
            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //SalesInvoiceExpDAL Sdal = new SalesInvoiceExpDAL();
                ISalesInvoiceExp Sdal = OrdinaryVATDesktop.GetObject<SalesInvoiceExpDAL, SalesInvoiceExpRepo, ISalesInvoiceExp>(OrdinaryVATDesktop.IsWCF);

                SalesInvoiceExpVM vm = new SalesInvoiceExpVM();
                vm.ID = Convert.ToInt32(NextID);
                vm.LCDate = dtpLCDate.Text.Trim();
                vm.LCBank = txtLCBank.Text.Trim();
                vm.PINo = txtPINo.Text.Trim();
                vm.PIDate = dtpPIDate.Text.Trim();
                vm.EXPNo = txtEXPNo.Text.Trim();
                vm.EXPDate = dtpEXPDate.Text.Trim();
                vm.Remarks = txtRemarks.Text.Trim();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.PortFrom = txtPortFrom.Text.Trim();
                vm.PortTo = txtPortTo.Text.Trim();

                vm.LCNumber = txtLCNumber.Text.Trim();

                sqlResults = Sdal.UpdateSalesInvoiceExps(vm,connVM);
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
                        //txtCode.Text = code;

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
                //SalesInvoiceExpDAL Sdal = new SalesInvoiceExpDAL();
                ISalesInvoiceExp Sdal = OrdinaryVATDesktop.GetObject<SalesInvoiceExpDAL, SalesInvoiceExpRepo, ISalesInvoiceExp>(OrdinaryVATDesktop.IsWCF);
                SalesInvoiceExpVM vm = new SalesInvoiceExpVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                string[] ids = new string[] { txtId.Text.Trim(), "" };

                sqlResults = Sdal.Delete(vm, ids,null,null,connVM); 
                
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

        //private void chkIsFixedVAT_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
            
        //}
        //private void Lebelchange()
        //{
        //    chkLocalVAT.Text = "VAT (%)";
        //    if (chkLocalVAT.Checked)
        //    {
        //        chkLocalVAT.Text = "VAT (F)";
        //    }
        //chkCD.Text = "CD (%)";
        //if (chkCD.Checked)
        //{
        //chkCD.Text = "CD (F)";
        //}
        //chkVAT.Text = "VAT (%)";
        //if (chkVAT.Checked)
        //{
        //chkVAT.Text = "VAT (F)";
        //}
        //chkAIT.Text = "AIT (%)";
        //if (chkAIT.Checked)
        //{
        //chkAIT.Text = "AIT (F)";
        //}
        //chkRD.Text = "RD (%)";
        //if (chkRD.Checked)
        //{
        //chkRD.Text = "RD (F)";
        //}
        //chkAT.Text = "AT (%)";
        //if (chkAT.Checked)
        //{
        //chkAT.Text = "AT (F)";
        //}
        //chkLocalCD.Text = "SD (%)";
        //if (chkLocalCD.Checked)
        //{
        //chkLocalCD.Text = "SD (F)";
        //}
        //chkSD.Text = "SD (%)";
        //if (chkSD.Checked)
        //{
        //chkSD.Text = "SD (F)";
        //}
        //}


        //private void chkCD_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkVAT_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkAIT_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkRD_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkAT_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkLocalCD_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void chkSD_CheckedChanged(object sender, EventArgs e)
        //{
        //    Lebelchange();
        //}

        //private void txtCD_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtCD, "CD");
        //}

        //private void txtRD_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtRD, "RD");
        //}

        //private void txtSD_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtSD, "SD");
        //}

        //private void txtVAT_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtVAT, "VAT");
        //}

        //private void txtAT_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtAT, "AT");
        //}

        //private void txtAIT_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtAIT, "AIT");
        //}

        //private void txtOtherSD_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtOtherSD, "OtherSD");
        //}

        //private void txtOtherVAT_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBox(txtOtherVAT, "OtherVAT");
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
