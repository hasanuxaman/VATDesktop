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
using System.Data.Odbc;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Utilities;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Threading;
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using SymphonySofttech.Reports;

namespace VATClient.ReportPages
{
    public partial class FormRptSaleReceive : Form
    {
        public FormRptSaleReceive()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet ReportMIS;
        private static string ShiftId = "0";
        private static string ShiftName = "[All]";
        string[] Condition = new string[] { "one" };


        #region Global Variables For BackGroundWorker
        private ReportDocument reportDocument = new ReportDocument(); 

        private DataSet ReportResult;
        private string cmbPostText;
        private string TransactionType;
        public string Toll = string.Empty;
        public string FormNumeric = string.Empty;

        #endregion

        string ReceiveFromDate, ReceiveToDate;

        private void grbBankInformation_Enter(object sender, EventArgs e)
        {

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ClearAllFields()
        {
            txtItemNo.Text = "";
            dtpReceiveFromDate.Text = "";
            dtpReceiveToDate.Text = "";
            dtpReceiveFromDate.Checked = false;
            dtpReceiveToDate.Checked = false;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void NullCheck()
        {
            try
            {
                if (dtpReceiveFromDate.Checked == false)
                {
                    ReceiveFromDate = "1900-01-01";
                }
                else
                {
                    ReceiveFromDate = dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpReceiveToDate.Checked == false)
                {
                    ReceiveToDate = "2900-12-31";
                }
                else
                {
                    ReceiveToDate = dtpReceiveToDate.Value.ToString("yyyy-MMM-dd  HH:mm:ss");// dtpReceiveToDate.Value.ToString("yyyy-MMM-dd");
                }
                if (chkShiftAll.Checked)
                {
                    ShiftId = "0";
                    ShiftName = cmbShift.Text.ToString();
                }
                else
                {
                    ShiftId = cmbShift.SelectedValue.ToString();
                    ShiftName = cmbShift.Text.ToString();
                }

                cmbPostText = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";


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

        private void btnSearch_Click(object sender, EventArgs e)
        {
           
            
        }
        private void btnProduct_Click(object sender, EventArgs e)
        {
             
        }
        private void FormRptReceiveInformation_Load(object sender, EventArgs e)
        {
            #region cmbShift
            CommonDAL cDal = new CommonDAL();
            cmbShift.DataSource = null;
            cmbShift.Items.Clear();
            Condition = new string[0];
            cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true,true,connVM);
            #endregion cmbShift

            #region Preview Only

            CommonDAL commonDal = new CommonDAL();
            FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
            cmbDecimal.Text = FormNumeric;

            string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);

            if (PreviewOnly.ToLower() == "n")
            {
                cmbPost.Text = "Y";
                cmbPost.Visible = false;
                label9.Visible = false;
            }

            #endregion

            dtpReceiveFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            dtpReceiveToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));

        }
        private void FormMaker()
        {
            
        }
        private void PTypeSearch()
        {
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
             
        }

        //=========================================================
        private void btnPreview_Click(object sender, EventArgs e)
        {
            Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            FormNumeric = cmbDecimal.Text.Trim();
            Toll = chbToll.Checked ? "Y" : "N";

            NullCheck();
            this.progressBar1.Visible = true;
            this.btnPreview.Enabled = false;
            bgw.RunWorkerAsync();

        }

      
        #region backgroundWorkerReport

        private void backgroundWorkerPreviewDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                MISReport _report = new MISReport();

                reportDocument = _report.SaleReceiveMIS(ReceiveFromDate, ReceiveToDate, ShiftId, cmbPostText, Toll, FormNumeric,connVM);

               
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
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();


                #region Old

                //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //ReportClass objrpt= new ReportClass();
                 
                //    ReportResult.Tables[0].TableName = "dtSaleReceive";
                //    objrpt = new RptSaleReceive();
                

                //objrpt.SetDataSource(ReportResult);
                ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Receive Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                ////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                ////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";

                
                //if (Convert.ToInt32( ShiftId)<=1)
                //{
                //    ShiftName = "[All]";
                //}
                //objrpt.DataDefinition.FormulaFields["shift"].Text = "'" + ShiftName + "'";

                //if (dtpReceiveFromDate.Checked == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                //                                                            dtpReceiveFromDate.Value.ToString(
                //                                                                "dd/MMM/yyyy") + "'  ";
                //}

                //if (dtpReceiveToDate.Checked == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                //                                                          dtpReceiveToDate.Value.ToString("dd/MMM/yyyy") +
                //                                                          "'  ";
                //}
                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                //FormReport reports = new FormReport();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();


                #endregion

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

        private void button3_Click(object sender, EventArgs e)
        {
             
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void chkCategoryLike_Click(object sender, EventArgs e)
        {
             
        }

        private void cmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpReceiveFromDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtpReceiveFromDate.Checked == true)
            //{
            //    dtpReceiveFromDate.Value = Convert.ToDateTime(dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd 00:01"));
            //}
        }

        private void dtpReceiveToDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtpReceiveToDate.Checked == true)
            //{
            //    dtpReceiveToDate.Value = Convert.ToDateTime(dtpReceiveToDate.Value.ToString("yyyy-MMM-dd 23:59"));
            //}
        }

        //==========================================================


    }
}
