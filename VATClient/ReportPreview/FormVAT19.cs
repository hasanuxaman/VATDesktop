using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
//

using SymphonySofttech.Utilities;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using System.Collections.Generic;


namespace VATClient.ReportPreview
{
    public partial class FormVAT19 : Form
    {
        public FormVAT19()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        string VAT19Data;
        public string VFIN = "35";
        private string vExportInBDT = string.Empty;
        private string ExportInBDT = string.Empty;

        #region Global Variables For BackGroundWorker

        private DataSet VAT19Result;

        #endregion


        #region ReportShowDs unused
        //private void ReportShowDs()
        //{
        //    try
        //    {
        //        VAT19Data =
        //           dtpDate.Value.ToString("MMMM-yyyy")
        //           + FieldDelimeter +
        //  txt9a.Text.Trim() + FieldDelimeter +
        //  txt9b.Text.Trim() + FieldDelimeter +
        //  txt10.Text.Trim() + FieldDelimeter +
        //  txt18.Text.Trim() + FieldDelimeter +
        //  txt19.Text.Trim() + FieldDelimeter +
        //   LineDelimeter;

        //        string encriptedVAT19Data = Converter.DESEncrypt(PassPhrase, EnKey, VAT19Data);
        //        ReportDSSoapClient VAT19 = new ReportDSSoapClient();
        //        DataSet VAT19Result = VAT19.VAT19(encriptedVAT19Data.ToString(), Program.DatabaseName);  // Change 04

        //        if (VAT19Result.Tables.Count <= 0)
        //        {
        //            MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        VAT19Result.Tables[0].TableName = "DsVAT19";
        //        RptVAT19 objrpt = new RptVAT19();
        //        objrpt.SetDataSource(VAT19Result);
        //        //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
        //        //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
        //        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
        //        //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
        //        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
        //        objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
        //        objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
        //        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
        //        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
        //        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
        //        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

        //        FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'"; objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
        //        reports.crystalReportViewer1.Refresh();
        //        reports.setReportSource(objrpt);
        //        if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) reports.ShowDialog();                
        //    }
        //    #region Catch
        //    catch (IndexOutOfRangeException ex)
        //    {
        //        FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (FormatException ex)
        //    {

        //        FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }

        //    catch (SoapHeaderException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }

        //        FileLogger.Log(this.Name, "ReportShowDs", exMessage);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (SoapException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReportShowDs", exMessage);
        //    }
        //    catch (EndpointNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReportShowDs", exMessage);
        //    }
        //    #endregion Catch
        //}
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
        private void label34_Click(object sender, EventArgs e)
        {

        }

      

        private void txt5_TextChanged(object sender, EventArgs e)
        {
            ////txt6.Text = Convert.ToDecimal(Convert.ToDecimal(txt4.Text) + Convert.ToDecimal(txt5.Text)).ToString();
        }

        private void txt1b_TextChanged(object sender, EventArgs e)
        {
            ////txt4.Text = Convert.ToDecimal(Convert.ToDecimal(txt1b.Text) + Convert.ToDecimal(txt1c.Text)).ToString();
        }

        private void txt7b_TextChanged(object sender, EventArgs e)
        {
            ////txt11.Text =Convert.ToDecimal(Convert.ToDecimal( txt7b.Text) + Convert.ToDecimal(txt8b.Text) + Convert.ToDecimal(txt9b.Text)).ToString("0.00");
        }

        private void txt8b_TextChanged(object sender, EventArgs e)
        {
            ////txt11.Text = Convert.ToDecimal(Convert.ToDecimal(txt7b.Text) + Convert.ToDecimal(txt8b.Text) + Convert.ToDecimal(txt9b.Text)).ToString("0.00");
        }

        private void txt9b_TextChanged(object sender, EventArgs e)
        {
            //////txt11.Text = Convert.ToDecimal(Convert.ToDecimal(txt7b.Text) + Convert.ToDecimal(txt8b.Text) + Convert.ToDecimal(txt9b.Text)).ToString("0.00");
        }

        private void txt11_TextChanged(object sender, EventArgs e)
        {
            ////txt14.Text = Convert.ToDecimal(Convert.ToDecimal( txt11.Text )+ Convert.ToDecimal(txt12.Text) + Convert.ToDecimal(txt13.Text)).ToString("0.00");
        }

        private void txt12_TextChanged(object sender, EventArgs e)
        {
            ////txt14.Text = Convert.ToDecimal(Convert.ToDecimal(txt11.Text) + Convert.ToDecimal(txt12.Text) + Convert.ToDecimal(txt13.Text)).ToString("0.00");
        }

        private void txt13_TextChanged(object sender, EventArgs e)
        {
            ////txt14.Text = Convert.ToDecimal(Convert.ToDecimal(txt11.Text) + Convert.ToDecimal(txt12.Text) + Convert.ToDecimal(txt13.Text)).ToString("0.00");
        }

        private void txt1c_TextChanged(object sender, EventArgs e)
        {
            //////txt4.Text = Convert.ToDecimal(Convert.ToDecimal(txt1b.Text) + Convert.ToDecimal(txt1c.Text)).ToString();
        }

        private void txt1c_Leave(object sender, EventArgs e)
        {

        }

        private void txt1b_Leave(object sender, EventArgs e)
        {
            try
            {
                decimal a = 0;
                decimal b = 0;
                a = Convert.ToDecimal(txt1b.Text);
                b = Convert.ToDecimal(txt1c.Text);
                //MessageBox.Show(Convert.ToString(a + b));

                //txt4a.Text = Convert.ToString(a + b);
                txt4.Text = Convert.ToString(a + b);
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "txt1b_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "txt1b_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "txt1b_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "txt1b_Leave", exMessage);
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
                FileLogger.Log(this.Name, "txt1b_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txt1b_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txt1b_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "txt1b_Leave", exMessage);
            }
            #endregion Catch
        }

        private void txt4_TextChanged(object sender, EventArgs e)
        {
            ////txt6.Text = Convert.ToDecimal(Convert.ToDecimal(txt4.Text) + Convert.ToDecimal(txt5.Text)).ToString();
        }

        private void txt14_TextChanged(object sender, EventArgs e)
        {
            //////txt15.Text = Convert.ToDecimal(Convert.ToDecimal(txt6.Text) - Convert.ToDecimal(txt14.Text)).ToString("0.00");
        }

        private void txt6_TextChanged(object sender, EventArgs e)
        {
            ////txt15.Text = Convert.ToDecimal(Convert.ToDecimal(txt6.Text) - Convert.ToDecimal(txt14.Text)).ToString("0.00");
        }

        private void FormVAT19_Load(object sender, EventArgs e)
        {
            try
            {
                dtpDate.Text = DateTime.Now.ToString("MMMM-yyyy");
                FormMaker();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormVAT19_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormVAT19_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormVAT19_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormVAT19_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormVAT19_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT19_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormVAT19_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormVAT19_Load", exMessage);
            }
            #endregion Catch
        }
        private void FormMaker()
        {
            #region Settings
            CommonDAL commonDal = new CommonDAL();

            vExportInBDT = commonDal.settingsDesktop("VAT9_1", "ExportInBDT", null, connVM);


            if (string.IsNullOrEmpty(vExportInBDT))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }
            ExportInBDT =vExportInBDT ;
            if(ExportInBDT=="Y")
            {
                LBDT.Text = "BDT";
                
            }
            else
            {
                LBDT.Text = "USD";
            }
            
            #endregion Settings
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                //Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                if (Program.CheckLicence(dtpDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (chkMLock.Checked == false)
                {
                    MessageBox.Show("Fiscal month not lock, please lock the month first. ", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
              
                ReportShowDsNew();

            }
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
        }

        private void ReportShowDsNew()
        {
            try
            {
               
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                backgroundWorkerPreview.RunWorkerAsync();
               
                

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
            }
            #endregion Catch
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            
            if (Program.CheckLicence(dtpDate.Value) == true)
            {
                MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                return;
            }
           
            ReportShowDsLoad();
        }

        private void ReportShowDsLoad()
        {
            try
            {
                         this.progressBar1.Visible = true;
                this.btnLoad.Enabled = false;
                this.btnPreview.Enabled = false;
                
                backgroundWorkerLoad.RunWorkerAsync();
                

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
            }
            #endregion Catch
        }

        #region Background Worker Events

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                VAT19Result = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                if (Program.IsBureau == true)
                {
                    VAT19Result = reportDsdal.BureauVAT19Report(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT,connVM);  // Change 04

                }
                else
                {
                    //if (chkNewFormat.Checked)
                    //{
                        VAT19Result = reportDsdal.VAT19NewNewformat(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT,connVM);  // Change 04

                    //}
                    //else
                    //{
                    //    VAT19Result = reportDsdal.VAT19New(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT);  // Change 04
                    //}
                }
                //VAT19Result = reportDsdal.VAT19ReportNew(dtpDate.Value.ToString("MMMM-yyyy"), Program.DatabaseName); // Change 04
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (VAT19Result.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                //ReportResult.Tables[0].TableName = "DsStockNew";

                ReportClass objrpt = new ReportClass();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";

                if (chkBreakDown.Checked == true)
                {
                    VAT19Result.Tables[1].TableName = "Ds19Break";
                    objrpt = new Rpt19Break();
                    objrpt.SetDataSource(VAT19Result);
                    
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                    objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";

                }
                else if (chkNewFormat.Checked)
                {
                    VAT19Result.Tables[2].TableName = "DsVAT19New";

                    //objrpt = new RptVAT19NewFormat();
                    objrpt.SetDataSource(VAT19Result);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                    objrpt.DataDefinition.FormulaFields["Currency"].Text = "'" + LBDT.Text.Trim() + "'";
                }
                else
                {
                    VAT19Result.Tables[0].TableName = "DsVAT19";

                    //objrpt = new RptVAT19();
                    objrpt.SetDataSource(VAT19Result);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                    objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                    objrpt.DataDefinition.FormulaFields["Currency"].Text = "'" + LBDT.Text.Trim() + "'";
                }
                FormReport reports = new FormReport(); 
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'"; 
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                VAT19Result = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                if (Program.IsBureau == true)
                {
                    VAT19Result = reportDsdal.BureauVAT19Report(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT,  connVM);  // Change 04

                }
                else
                {
                    //if (chkNewFormat.Checked)
                    //{
                    VAT19Result = reportDsdal.VAT19NewNewformat(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT, connVM);  // Change 04

                    //}
                    //else
                    //{
                    //    VAT19Result = reportDsdal.VAT19New(dtpDate.Value.ToString("MMMM-yyyy"), ExportInBDT);  // Change 04
                    //}
                }
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                VAT19Result.Tables[0].TableName = "DsVAT19";
                txt10.Text = VAT19Result.Tables[0].Rows[0][1].ToString();
                txt1a.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][1]).ToString("0,0.00");
                txt1b.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][2]).ToString("0,0.00");
                txt1c.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][3]).ToString("0,0.00");
                txt2a.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][4]).ToString("0,0.00");
                txt2b.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][5]).ToString("0,0.00");
                txt2c.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][6]).ToString("0,0.00");
                txt3.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][7]).ToString("0,0.00");
                txt4.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][8]).ToString("0,0.00");
                txt5.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][9]).ToString("0,0.00");
                txt6.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][10]).ToString("0,0.00");
                txt7a.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][11]).ToString("0,0.00");
                txt7b.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][12]).ToString("0,0.00");
                txt8a.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][13]).ToString("0,0.00");
                txt8b.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][14]).ToString("0,0.00");
                txt9a.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][15]).ToString("0,0.00");
                txt9b.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][16]).ToString("0,0.00");
                txt10.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][17]).ToString("0,0.00");
                txt11.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][18]).ToString("0,0.00");
                txt12.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][19]).ToString("0,0.00");
                txt13.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][20]).ToString("0,0.00");
                txt14.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][21]).ToString("0,0.00");
                txt15.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][22]).ToString("0,0.00");
                txt16.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][23]).ToString("0,0.00");
                txt17.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][24]).ToString("0,0.00");
                txt18.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][25]).ToString("0,0.00");
                txt19.Text = Convert.ToDecimal(VAT19Result.Tables[0].Rows[0][26]).ToString("0,0.00");
                chkMLock.Checked = true ? VAT19Result.Tables[0].Rows[0][27].ToString() == "Y" : 
                VAT19Result.Tables[0].Rows[0][27].ToString() == "N";
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted",

                               exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnLoad.Enabled = true;
                this.btnPreview.Enabled = true;
                
            }
        }
        #endregion

        private void btn9_1_Click(object sender, EventArgs e)
        {
            try
            {

                //if (Program.CheckLicence(dtpDate.Value) == true)
                //{
                //    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                //    return;
                //}
                //if (chkMLock.Checked == false)
                //{
                //    MessageBox.Show("Fiscal month not lock, please lock the month first. ", this.Text,
                //                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                ReportShow9_1();

            }
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
        }

        private void ReportShow9_1()
        {
            List<VAT9_1ConsolateVM> vat9_1CVMs = new List<VAT9_1ConsolateVM>();
            RptVAT9_1 objRpt = new RptVAT9_1();
            objRpt.SetDataSource(vat9_1CVMs);
            FormReport reports = new FormReport(); 

            //objRpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 9.1 Breakdown Report'";
            //objRpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
            //objRpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
            //objRpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
            //objRpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
            //objRpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
            //objRpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
            //objRpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
            //objRpt.DataDefinition.FormulaFields["OperationalCode"].Text = "'" + Program.OperationalCode + "'";
            reports.crystalReportViewer1.Refresh();
            reports.setReportSource(objRpt);
            //reports.ShowDialog();
            reports.WindowState = FormWindowState.Maximized;
            reports.Show();
        }

       
    }
}
