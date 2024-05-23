using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;

using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;

using VATServer.Library;

using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using VATViewModel.DTOs;
using VATServer.License;
using SymphonySofttech.Reports;
using VATServer.Ordinary; 


namespace VATClient.ReportPages
{
    public partial class FormRptPurchaseWithLC : Form
    {
        public FormRptPurchaseWithLC()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }

			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        private string TransType = string.Empty;
        private string post = string.Empty;
        private string PurchaseType = string.Empty;
        private ReportDocument reportDocument = new ReportDocument(); 


        #region Global Variables For BackGroundWorker

        private DataSet ReportResult;
        private DataSet ReportMIS;
        private string cmbPostText;
        //private string TransactionType;
        private string reportName;

        #endregion

       
        string LCFromDate, LCToDate;

        #region Excel

        private DataTable ReportDataTable;
        private DataTable dtsorted;
        private DataGridView dgvSale;
        private Microsoft.Office.Interop.Excel.Application xlApp;
        private Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        private object misValue = System.Reflection.Missing.Value;
        private string saveLocation;
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearAllFields()
        {
            try
            {
                txtVendorGroupID.Text = "";
                txtVendorGroupName.Text = "";

                txtInvoiceNo.Text = "";
                txtItemNo.Text = "";
                txtLCNo.Text = "";
                txtPGroupId.Text = "";
               
                txtProductTypeId.Text = "";
                txtVendorName.Text = "";
                txtVendorId.Text = "";
                cmbPost.SelectedIndex = -1;
                
                dtpLCFromDate.Value = Program.SessionDate;
                dtpLCToDate.Value = Program.SessionDate;
                dtpLCFromDate.Checked = false;
                dtpLCToDate.Checked = false;
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

        private void NullCheck()
        {
            try
            {
                if (dtpLCFromDate.Checked == false)
                {
                    LCFromDate = "";
                }
                else
                {
                    LCFromDate = dtpLCFromDate.Value.ToString("yyyy-MMM-dd") .ToString();// dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpLCToDate.Checked == false)
                {
                    LCToDate = "";
                }
                else
                {
                    LCToDate = dtpLCToDate.Value.ToString("yyyy-MMM-dd") .ToString();//  dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
                }

                post = cmbPost.Text.Trim();
               

                
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
        //No server Side Method
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region TransactionType

                DataGridViewRow selectedRow = null;
                 
                string result;

                selectedRow = FormPurchaseSearch.SelectOne("");
               

                #endregion TransactionType

                if (selectedRow != null && selectedRow.Selected==true)
                {

                    txtInvoiceNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    //txtVendorName.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    //reportName = selectedRow.Cells["TransType"].Value.ToString();

                }



               



            }
                #region Catch

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
        }

        //No server Side Method
        private void btnProduct_Click(object sender, EventArgs e)
        {
           
            try
            {
                Program.fromOpen = "Me";
                Program.R_F = "";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtLCNo.Text = selectedRow.Cells["ProductName"].Value.ToString();//
                }

                
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            #endregion Catch

        }
        //No server Side Method
        private void btnVendor_Click(object sender, EventArgs e)
        {
            
            try
            {
                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormVendorSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtVendorId.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();
                }


                //string result = FormVendorSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result == ""){return;}else//if (result != "")
                //{
                //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());
                //    txtVendorId.Text = VendorInfo[0];
                //    txtVendorName.Text = VendorInfo[1];
                //}
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendor_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVendor_Click", exMessage);
            }
            #endregion Catch
        }

        private void FormRptPurchaseWithLC_Load(object sender, EventArgs e)
        {
            //RollDetailsInfo();
            FormMaker();
        }

        private void FormMaker()
        {
            try
            {
                #region Preview Only

                CommonDAL commonDal = new CommonDAL();

                string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);

                if (PreviewOnly.ToLower() == "n")
                {
                    cmbPost.Text = "Y";
                    cmbPost.Visible = false;
                    label9.Visible = false;
                }

                #endregion
             
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormMaker", exMessage);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            #endregion Catch
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            #region Try

            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //Detail Summery Single Monthly
                if (Program.CheckLicence(dtpLCToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                // cmbPostText = Convert.ToString(chkPost.Checked ? "Y" : "N");
                MISReport _report = new MISReport();
                _report.dtpLCFromDate = dtpLCFromDate.Checked;
                _report.dtpLCToDate = dtpLCToDate.Checked;
                _report.VendorName = txtVendorName.Text;
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;

                NullCheck();

                backgroundWorkerPreviewDetails.RunWorkerAsync();


            }

            #endregion
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            #endregion Catch
            #region bad
            
            
            //try
            //{
                
            //    if (Program.CheckLicence(dtpPurchaseToDate.Value) == true)
            //    {
            //        MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
            //        return;
            //    }
            //    // cmbPostText = Convert.ToString(chkPost.Checked ? "Y" : "N");
            //    NullCheck();
            //    transType();
            //   if (rbtnSingle.Checked == true)
            //    {
            //        if (txtInvoiceNo.Text == "")
            //        {
            //            MessageBox.Show("Please select the Purchase No", this.Text, MessageBoxButtons.OK,
            //                            MessageBoxIcon.Information);
            //            return;
            //        }
            //        this.progressBar1.Visible = true;
            //        this.btnPreview.Enabled = false;
            //        transTypeZamil();

            //        backgroundWorkerMIS.RunWorkerAsync();
            //    }
            //    else
            //    {
            //        transType();
            //        backgroundWorkerPreviewDetails.RunWorkerAsync();
                    
            //        this.progressBar1.Visible = true;
            //        this.btnPreview.Enabled = false;
            //    }
                

                

            //}
            //#region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            //}
            //#endregion Catch
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
               
            }
        }

     

        #region Background Worker Events

       
        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            ReportResult = null;

            try
            {
                
                MISReport _report = new MISReport();

                reportDocument = _report.PurchaseWithLCInfo(txtInvoiceNo.Text, LCFromDate, LCToDate, txtVendorId.Text, txtItemNo.Text, txtVendorGroupID.Text, txtLCNo.Text, post,connVM);
            
            
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreviewDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                FormReport reports = new FormReport();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                #region

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                
                // ReportClass objrpt= new ReportClass();
                 
                //     ReportResult.Tables[0].TableName = "DsPurchaseLC";
                     
                //     objrpt = new RptPurchase_LCInfo(); 
                //     objrpt.SetDataSource(ReportResult);

                 
               
                //#region Formulla
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                //if (txtLCNo.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'" + txtLCNo.Text.Trim() + "'  "; }

                //if (txtVendorName.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + txtVendorName.Text.Trim() + "'  "; }

                //if (txtInvoiceNo.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + txtInvoiceNo.Text.Trim() + "'  "; }

                //if (dtpLCFromDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'" + dtpLCFromDate.Value.ToString("dd/MMM/yyyy") + "'  "; }

                //if (dtpLCToDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'" + dtpLCToDate.Value.ToString("dd/MMM/yyyy") + "'  "; }

                //#endregion Formulla

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                //FormReport reports = new FormReport(); 
                ////objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                #endregion


            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewDetails_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        #endregion
        
        private void btnMis_Click(object sender, EventArgs e)
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
               
                if (txtInvoiceNo.Text == "")
                {
                    MessageBox.Show("Please select the Purchase No", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                //transType();
                this.progressBar1.Visible = true;
                backgroundWorkerMIS.RunWorkerAsync();
                btnMis.Enabled = false;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnMis_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnMis_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnMis_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnMis_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnMis_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnMis_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnMis_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnMis_Click", exMessage);
            }
            #endregion Catch

        }

       

        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                ReportMIS = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportMIS = reportDsdal.PurchaseMis(txtInvoiceNo.Text.Trim(), 0, "", connVM);

                
                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                ReportClass objrpt = new ReportClass();                // Start Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection(); 
                

                if (ReportMIS.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportMIS.Tables[0].TableName = "DsPurchaseHeader";
                ReportMIS.Tables[1].TableName = "DsPurchaseDetails";
                //ReportMIS.Tables[2].TableName = "DsPurchaseDuty";
                
                objrpt = new RptMISPurchase1();
                
                objrpt.SetDataSource(ReportMIS);

                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnMis.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
                string result = FormProductCategorySearch.SelectOne();
                if (result == "") { return; }
                else//if (result == ""){return;}else//if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtPGroupId.Text = ProductCategoryInfo[0];
                    //txtProductGroup.Text = ProductCategoryInfo[1];
                }
        }

        private void btnVendorGroup_Click(object sender, EventArgs e)
        {
            Program.fromOpen = "Me";
            string result = FormVendorGroupSearch.SelectOne();
            try
            {
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] VendorGroupInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtVendorGroupID.Text = VendorGroupInfo[0];
                    txtVendorGroupName.Text = VendorGroupInfo[1];
                }
            }
            #region Catch
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
        }

        private void chkLocal_CheckedChanged(object sender, EventArgs e)
        {
         //if(chkLocal.Checked==false)
         //{
         //    chkLocal.Text = "Local";
         //}
         //else if (chkLocal.Checked == true)
         //{
         //    chkLocal.Text = "Import";
         //}
        }

        private void backgroundWorkerMonth_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                #region Start

                ReportDataTable = new DataTable();

               
                #endregion

            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerMonth_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (ReportDataTable.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataView dtview = new DataView(ReportDataTable);
                dtview.Sort = "YearSerial ASC, MonthSerial ASC";
                dtsorted = dtview.ToTable();

                #region assign column name of datagridview

//purchase.Product_Name,purchase.Product_Code,SUM(purchase.Quantity) Quantity,SUM(purchase.SubTotal) Amount,
// SUM(purchase.VATAmount) VAT,purchase.MonthNames,purchase.ItemNo,purchase.VendorName,
//                                    purchase.MonthSerial,purchase.YearSerial
                dgvSale = new DataGridView();
                int ee = dgvSale.Rows.Count;
                dgvSale.Columns.Add("sl", "S.N");
                dgvSale.Columns.Add("Product_Name", dtsorted.Columns["Product_Name"].ColumnName.Replace('_', ' '));
                dgvSale.Columns.Add("Product_Code", dtsorted.Columns["Product_Code"].ColumnName.Replace('_', ' '));

                DataTable monthNames = dtsorted.DefaultView.ToTable(true, "MonthNames");

                for (int i = 0; i < monthNames.Rows.Count; i++)
                {
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Quantity",
                                        monthNames.Rows[i][0].ToString() + "_" + "Quantity");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "Amount",
                                        monthNames.Rows[i][0].ToString() + "_" + "Amount");
                    dgvSale.Columns.Add(monthNames.Rows[i][0].ToString() + "_" + "VAT",
                                        monthNames.Rows[i][0].ToString() + "_" + "VAT");

                }

                string lastClmn = monthNames.Rows[0][0].ToString() + "-" +
                                  monthNames.Rows[monthNames.Rows.Count - 1][0].ToString() + " Summery";
                dgvSale.Columns.Add("TotQty", lastClmn + "_Quantity");
                dgvSale.Columns.Add("Total_Amount", "Total_Amount");
                dgvSale.Columns.Add("Total_VAT", "Total_VAT");

                #endregion

                #region assign value to grid columns

                int j = 0;
                decimal totalQty = 0;
                decimal totalAmount = 0;
                decimal totalVAT = 0;


                dgvSale.Rows.Clear();
                int aaa = dgvSale.Rows.Count;
                foreach (DataRow item in dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name").Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);
                    dgvSale["sl", j].Value = j + 1;
                    dgvSale["Product_Code", j].Value = item["Product_Code"].ToString();
                    dgvSale["Product_Name", j].Value = item["Product_Name"].ToString();
                    for (int i = 0; i < monthNames.Rows.Count; i++)
                    {
                        dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "Amount", j].Value = 0;
                        dgvSale[monthNames.Rows[i][0] + "_" + "VAT", j].Value = 0;
                    }

                    j++;

                }
                int a = dgvSale.Rows.Count;
                int aa = dtsorted.DefaultView.ToTable(true, "Product_Code", "Product_Name").Rows.Count;
                for (int rowItem = 0; rowItem < dgvSale.Rows.Count - 1; rowItem++)
                {
                    totalQty = 0;
                    totalAmount = 0;
                    totalVAT = 0;
                    DataRow[] detail =
                        dtsorted.Select("Product_Code='" + dgvSale["Product_Code", rowItem].Value.ToString() + "'");

                    foreach (var row1 in detail)
                    {
                        for (int i = 0; i < monthNames.Rows.Count; i++)
                        {
                            if (row1["MonthNames"].ToString() == monthNames.Rows[i][0].ToString())
                            {

                                dgvSale[monthNames.Rows[i][0] + "_" + "Quantity", rowItem].Value =
                                    Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                totalQty += Convert.ToDecimal(row1["Quantity"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "Amount", rowItem].Value =
                                    Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                totalAmount += Convert.ToDecimal(row1["Amount"].ToString().Trim());

                                dgvSale[monthNames.Rows[i][0] + "_" + "VAT", rowItem].Value =
                                    Convert.ToDecimal(row1["VAT"].ToString().Trim());

                                totalVAT += Convert.ToDecimal(row1["VAT"].ToString().Trim());
                            }
                        }
                    }
                    dgvSale["TotQty", rowItem].Value = totalQty.ToString();
                    dgvSale["Total_Amount", rowItem].Value = totalAmount.ToString();
                    dgvSale["Total_VAT", rowItem].Value = totalVAT.ToString();
                }

                #endregion

                //ExportToExcel();
                //OpenExcel();
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreviewSummary_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }
        //=================================================

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        //private void ExportToExcel()
        //{
        //    Microsoft.Office.Interop.Excel.Range chartRange;
        //    try
        //    {
        //        xlApp = new Excel.Application();
        //        xlWorkBook = xlApp.Workbooks.Add(misValue);
        //        xlWorkSheet = (Excel.Worksheet) xlWorkBook.Worksheets.get_Item(1);

        //        #region Report Header

        //        string companyName = Program.CompanyName;
        //        string companyAddress = Program.Address1;
        //        string reportName = "Vendor wise Monthly Purchase Statement";
        //        string customerName = "Vendor: " + dtsorted.Rows[0]["VendorName"].ToString();
        //        string productName = "ProductName: All";
        //        if (!string.IsNullOrEmpty(txtLCNo.Text))
        //            productName = "ProductName: " + txtLCNo.Text;
        //        string dateFrom = dtpLCFromDate.Value.ToString("MMMM") + "," +
        //                          dtpLCFromDate.Value.Year.ToString();
        //        string dateTo = dtpLCToDate.Value.ToString("MMMM") + "," +
        //                        dtpLCToDate.Value.Year.ToString();

        //        #endregion Report Header

        //        #region Report Header Style

        //        chartRange = xlWorkSheet.get_Range("a1", "m1");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyName;

        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;
        //        chartRange.Font.Bold = true;
        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a2", "m2");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = companyAddress;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a4", "m4");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = reportName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 16;
        //        chartRange.Font.Bold = true;


        //        chartRange = xlWorkSheet.get_Range("a5", "m5");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = customerName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;
        //        chartRange.Font.Bold = true;

        //        chartRange = xlWorkSheet.get_Range("a7", "m7");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = productName;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("a8", "f8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date From: " + dateFrom;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        chartRange = xlWorkSheet.get_Range("g8", "m8");
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = "Date To: " + dateTo;
        //        chartRange.HorizontalAlignment = 1;
        //        chartRange.VerticalAlignment = 1;

        //        chartRange.Font.Size = 11;

        //        #endregion Report Header

        //        #region Table Caption

        //        // Excel column name
        //        string[] col = new string[]
        //                           {
        //                               "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
        //                               "q",
        //                               "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac",
        //                               "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap"
        //                           };
        //        for (int colItem = 0; colItem < dgvSale.Columns.Count; colItem++)
        //        {
        //            if (colItem < 3)
        //            {
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText;
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //            }
        //            else
        //            {
        //                if (colItem%3 == 0)
        //                {
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem] + "9");
        //                    chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[0];
        //                    chartRange = xlWorkSheet.get_Range(col[colItem] + "9", col[colItem + 2] + "9");
        //                    chartRange.Merge(false);
        //                    chartRange.Font.Bold = true;
        //                    chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                    chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                    chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                            Excel.XlColorIndex.xlColorIndexAutomatic);

        //                }
        //                chartRange = xlWorkSheet.get_Range(col[colItem] + "10", col[colItem] + "10");
        //                //chartRange.FormulaR1C1 = dgvSale.Columns[colItem].Name.Substring(3);
        //                chartRange.FormulaR1C1 = dgvSale.Columns[colItem].HeaderText.Split('_')[1];
        //                chartRange.Font.Bold = true;
        //                chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //                chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);

        //            }
        //        }

        //        string headerStrRange = col[0] + "9";
        //        string headerEndRange = col[dgvSale.Columns.Count - 1].ToString() + "10";
        //        chartRange = xlWorkSheet.get_Range(headerStrRange, headerEndRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);


        //        #endregion Table Caption

        //        #region Assign value

        //        string data = null;
        //        int rowNo = 0;
        //        int colNo = 0;
        //        for (rowNo = 0; rowNo < dgvSale.Rows.Count - 1; rowNo++)
        //        {
        //            for (colNo = 0; colNo < dgvSale.Columns.Count; colNo++)
        //            {
        //                data = dgvSale.Rows[rowNo].Cells[colNo].Value.ToString();
        //                xlWorkSheet.Cells[10 + rowNo + 1, colNo + 1] = data;
        //                chartRange = xlWorkSheet.get_Range(col[colNo] + (10 + rowNo + 1).ToString(),
        //                                                   col[colNo] + (10 + rowNo + 1).ToString());
        //                chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                        Excel.XlColorIndex.xlColorIndexAutomatic);
        //            }
        //        }
        //        // table border
        //        string cellStartName = col[0] + "11";
        //        string cellEndName = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        chartRange = xlWorkSheet.get_Range(cellStartName, cellEndName);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //        #endregion Assign value

        //        #region Grand Total Calculation

        //        //grand total style
        //        string gSRange = col[dgvSale.Columns.Count - 6].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        string gEndRange = col[dgvSale.Columns.Count - 4].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(gSRange, gEndRange);
        //        chartRange.Merge(false);
        //        chartRange.FormulaR1C1 = " Grand Total:";

        //        chartRange.Font.Bold = true;
        //        chartRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        chartRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //        //calculate total Qty
        //        string totQtyRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totQtyRange, totQtyRange);
        //        string startRange = col[dgvSale.Columns.Count - 3].ToString() + "11";
        //        string endRange = col[dgvSale.Columns.Count - 3].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        //calculate total Amount
        //        string totAmtRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totAmtRange, totAmtRange);
        //        startRange = col[dgvSale.Columns.Count - 2].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 2].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        //calculate total VAT
        //        string totVatRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count + 10).ToString();
        //        chartRange = xlWorkSheet.get_Range(totVatRange, totVatRange);
        //        startRange = col[dgvSale.Columns.Count - 1].ToString() + "11";
        //        endRange = col[dgvSale.Columns.Count - 1].ToString() + (dgvSale.Rows.Count - 1 + 10).ToString();
        //        chartRange.Formula = "=SUM(" + startRange + ":" + endRange + ")";
        //        chartRange.NumberFormat = "000,00";

        //        chartRange = xlWorkSheet.get_Range(gSRange, totVatRange);
        //        chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic,
        //                                Excel.XlColorIndex.xlColorIndexAutomatic);

        //        #endregion

        //        #region Save location

        //        string printLocation = Directory.GetCurrentDirectory() + @"\MIS_Reports\Purchase";
        //        saveLocation = printLocation + @"\" + "Monthly Purchase" + "_" +
        //                       dateFrom + " - " + dateTo + "_ " + DateTime.Now.ToString("MMM,yy, HH.mm") + ".xls";

        //        if (!Directory.Exists(printLocation))
        //            Directory.CreateDirectory(printLocation);

        //        xlWorkBook.SaveAs(saveLocation, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue,
        //                          misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue,
        //                          misValue);
        //        xlApp.Visible = true;

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        xlWorkBook.Close(true, misValue, misValue);

        //        xlApp.Quit();

        //        releaseObject(xlWorkSheet);
        //        releaseObject(xlWorkBook);
        //        releaseObject(xlApp);


        //    }
        //    finally
        //    {

        //    }
        //}

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //txtProductType.Text = cmbProductType.Text;
        }



    }
}
