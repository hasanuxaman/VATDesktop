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
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Utilities;

using System.IO;
using System.Data.Odbc;
//
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Threading;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using VATServer.Ordinary;

namespace VATClient.ReportPages
{
    public partial class FormRptVendorInformation : Form
    {
        public FormRptVendorInformation()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #region Global Variables For BackGroundWorker
        //private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataSet ReportResult;
        private ReportDocument reportDocument = null; 

        #endregion
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public string VFIN = "418";
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void grbBankInformation_Enter(object sender, EventArgs e)
        {

        }
        private void ClearAllFields()
        {
            try
            {
                txtTINNo.Text = "";
                txtVATRegistrationNo.Text = "";
                txtVendorID.Text = "";
                txtVendorName.Text = "";
                txtVGroupId.Text = "";
                txtVGroup.Text = "";
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

        //==============================================================
        private void btnPreview_Click(object sender, EventArgs e)
        {
            OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            ReportShowDs();
        }
        private void ReportShowDs()
        {
            try
            {
                this.btnPreview.Enabled = false;
                this.progressBar1.Visible = true;
                //string ReportData =

                //    txtVendorID.Text.Trim() + FieldDelimeter +
                //    txtVGroupId.Text.Trim() + FieldDelimeter +
                //    txtTINNo.Text.Trim() + FieldDelimeter +
                //    txtVATRegistrationNo.Text.Trim() + LineDelimeter;

                backgroundWorkerPreview.RunWorkerAsync();

                #region Start
                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //ReportDSSoapClient ShowReport = new ReportDSSoapClient();
                //DataSet ReportResult = ShowReport.Vendor(encriptedData.ToString(), Program.DatabaseName);
                #endregion
                #region Complete
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //ReportResult.Tables[0].TableName = "DsVendor";
                //RptVendorListing objrpt = new RptVendorListing();
                //objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) reports.ShowDialog();
                #endregion
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion Catch

        }

        #region backgroundWorkerPreview
        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                VendorReport _report = new VendorReport();
                reportDocument = _report.VendorReportNew(txtVendorID.Text.Trim(), txtVGroupId.Text.Trim(),connVM);
                //ReportResult = new DataSet();

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                DBSQLConnection _dbsqlConnection = new DBSQLConnection(); 
                
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //ReportResult.Tables[0].TableName = "DsVendor";
                //RptVendorListing objrpt = new RptVendorListing();
                //objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                ////if (reportDocument != null)
                ////{
                ////    reportDocument.Close();
                ////    reportDocument.Dispose();
                ////}

                this.btnPreview.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }
        #endregion

        //=============================================================

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormVendorSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtVendorID.Text = selectedRow.Cells["VendorID"].Value.ToString();
                    txtVendorName.Text = selectedRow.Cells["VendorName"].Value.ToString();


                    txtVGroupId.Text = selectedRow.Cells["VendorGroupID"].Value.ToString();
                    txtVGroup.Text = selectedRow.Cells["VendorGroupName"].Value.ToString();

                    txtVATRegistrationNo.Text = selectedRow.Cells["VATRegistrationNo"].Value.ToString();
                    txtTINNo.Text = selectedRow.Cells["TINNo"].Value.ToString();
                }

                //string result = FormVendorSearch.SelectOne();
                //if (result == "")
                //{
                //    return;
                //}
                //else //if (result != "")
                //{
                //    string[] VendorInfo = result.Split(FieldDelimeter.ToCharArray());

                //    txtVendorID.Text = VendorInfo[0];
                //    txtVendorName.Text = VendorInfo[1];
                //    txtVGroupId.Text = VendorInfo[2];
                //    txtVGroup.Text = VendorInfo[3];

                //    txtVATRegistrationNo.Text = VendorInfo[16];
                //    txtTINNo.Text = VendorInfo[17];
                //}
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

        //        private void ReportShow()
        //        {
        //            string[] ReportLines;
        //            try
        //            {
        //                string ReportData =

        //                    txtVendorID.Text.Trim() + FieldDelimeter +
        //                    txtVendorName.Text.Trim() + FieldDelimeter +
        //                    txtTINNo.Text.Trim() + FieldDelimeter +
        //                    txtVATRegistrationNo.Text.Trim()  +

        //                 LineDelimeter;

        //                string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);

        //                ReportSoapClient ShowReport = new ReportSoapClient();

        //                string ReportResult = ShowReport.Vendor(encriptedData.ToString(), Program.DatabaseName);

        //                string decriptedSData = Converter.DESDecrypt(PassPhrase, EnKey, ReportResult);

        //                ReportLines = decriptedSData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //            }
        //            catch (Exception ex)
        //            {
        //                if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //            try
        //            {
        //                OdbcConnection sqlConn2 = new OdbcConnection("PROVIDER=MSDASQL;dsn=VAT;uid=;pwd=;database=VATAccessDB.mdb");
        //                sqlConn2.Open();

        //                string sql2 = "Delete from RptVendor";
        //                OdbcCommand sqlCmd = new OdbcCommand(sql2, sqlConn2);
        //                sqlCmd.ExecuteNonQuery();

        //                for (int j = 0; j < Convert.ToInt32(ReportLines.Length); j++)
        //                {
        //                    string[] ProductFields = ReportLines[j].Split(FieldDelimeter.ToCharArray());

        //                    try
        //                    {
        //                        sql2 = @"insert into  RptVendor(
        //
        //VendorID,
        //VendorName,
        //VATRegistrationNo,
        //TINNo,
        //Comments,
        //Address1,
        //Address2,
        //Address3,
        //TelephoneNo,
        //ContactPerson,
        //ContactPersonTelephone,
        //ContactPersonEmail,
        //VendorGroupName
        //)
        //                         values('" + ProductFields[1].ToString()
        //                        + "','" + ProductFields[2].ToString()
        //                        + "','" + ProductFields[3].ToString()
        //                        + "','" + ProductFields[4].ToString()
        //                        + "','" + ProductFields[5].ToString()
        //                        + "','" + ProductFields[6].ToString()
        //                        + "','" + ProductFields[7].ToString()
        //                        + "','" + ProductFields[8].ToString()
        //                        + "','" + ProductFields[9].ToString()
        //                        + "','" + ProductFields[10].ToString()
        //                        + "','" + ProductFields[11].ToString()
        //                        + "','" + ProductFields[12].ToString()
        //                        + "','" + ProductFields[13].ToString()
        //                        + "')";
        //                        //values(1,2,3,4,5,6,7)";                               

        //                        //                                '" + CustomerListFields[3].ToString() + "')";

        //                        sqlCmd = new OdbcCommand(sql2, sqlConn2);
        //                        sqlCmd.ExecuteNonQuery();
        //                    }

        //                    catch (Exception ex)
        //                    {
        //                        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                        return;
        //                    }
        //                }




        //                Thread.Sleep(1000); FormReport Reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'"; Reports.crystalReportViewer1.Refresh();
        //                Reports.crystalReportViewer1.ReportSource = @"c:\report\VendorListing.rpt";

        //                //string sql21 = "Delete from RptVendor";
        //                //OleDbCommand sqlCmd1 = new OleDbCommand(sql21, sqlConn2);

        //                //sqlCmd1.ExecuteNonQuery();
        //                //ParameterField CompanyName1 = new ParameterField();
        //                //CompanyName1.Name = "CompanyName";
        //                //ParameterDiscreteValue CompanyNameValue = new ParameterDiscreteValue();// CrystalDecisions.Shared.
        //                //CompanyNameValue.Value = DBConstant.CompanyName;
        //                //CompanyName1.CurrentValues.Add(CompanyNameValue);
        //                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyName1);

        //                //ParameterField CompanyAddress1 = new ParameterField();
        //                //CompanyAddress1.Name = "CompanyAddress";
        //                //ParameterDiscreteValue CompanyAddressValue = new ParameterDiscreteValue();// CrystalDecisions.Shared.
        //                //CompanyAddressValue.Value = DBConstant.CompanyAddress;
        //                //CompanyAddress1.CurrentValues.Add(CompanyAddressValue);
        //                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyAddress1);

        //                //ParameterField CompanyNumber1 = new ParameterField();
        //                //CompanyNumber1.Name = "CompanyNumber";
        //                //ParameterDiscreteValue CompanyNumberValue = new ParameterDiscreteValue();// CrystalDecisions.Shared.
        //                //CompanyNumberValue.Value = DBConstant.CompanyContactNumber;
        //                //CompanyNumber1.CurrentValues.Add(CompanyNumberValue);
        //                //Reports.crystalReportViewer1.ParameterFieldInfo.Add(CompanyNumber1);
        //                Reports.Text = "VendorListing";
        //                Reports.Show();
        //            }
        //            catch (Exception ex)
        //            {
        //                if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //        }

        private void FormRptVendorInformation_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                Program.fromOpen = "Other";
                string result = FormVendorGroupSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result != "")
                {
                    string[] VendorGroupInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtVGroupId.Text = VendorGroupInfo[0];
                    txtVGroup.Text = VendorGroupInfo[1];
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button1_Click", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion Catch
        }
    }
}
