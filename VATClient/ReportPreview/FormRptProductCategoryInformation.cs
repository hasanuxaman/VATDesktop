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
using SymphonySofttech.Reports.Report;
using VATClient.ModelDTO;
//
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Data.Odbc;
using System.Threading;
//
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using CrystalDecisions.CrystalReports.Engine;
using VATServer.Ordinary;
using SymphonySofttech.Reports;

namespace VATClient.ReportPages
{
    public partial class FormRptProductCategoryInformation : Form
    {
        public FormRptProductCategoryInformation()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        List<ProductTypeDTO> ProductTypes = new List<ProductTypeDTO>();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        #region Global Variables For BackGroundWorker
        private ReportDocument reportDocument = new ReportDocument(); 
        private DataSet ReportResult;
        private DataTable ProductTypeResult;
        private string cmbProductTypeText;

        #endregion

        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public string VFIN = "416";
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearAllFields()
        {
            txtCategoryName.Text = "";
            txtHSCodeNo.Text = "";
            cmbProductType.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        //No server Side Method
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string result = FormProductCategorySearch.SelectOne();
            if (result == "") { return; }
            else//if (result == ""){return;}else//if (result != "")
            {
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                txtCGId.Text = ProductCategoryInfo[0];
                txtCategoryName.Text = ProductCategoryInfo[1];
                //txtHSCodeNo.Text = ProductCategoryInfo[5];
                //cmbProductType.Text = ProductCategoryInfo[4].ToString();
            }
        }

        private void FormRptProductCategoryInformation_Load(object sender, EventArgs e)
        {
            //PTypeSearch();//skipped as suggested by Project Manager Sir(L)
            //RollDetailsInfo();
        }
        private void PTypeSearch()
        {
            try
            {
                string TypeData = string.Empty;

                TypeData =
                    FieldDelimeter +
                     FieldDelimeter +
                    FieldDelimeter +

                  FieldDelimeter + FieldDelimeter + FieldDelimeter + FieldDelimeter + LineDelimeter;
                //"Info1" + FieldDelimeter + "Info2" + FieldDelimeter + "Info3" + FieldDelimeter + "Info4" + FieldDelimeter + "Info5"


                //string encriptedTypeData = Converter.DESEncrypt(PassPhrase, EnKey, TypeData);

                //ProductTypeSoapClient ProductTypeSearch = new ProductTypeSoapClient();

                //DataTable ProductTypeResult = ProductTypeSearch.Search(encriptedTypeData.ToString(), Program.DatabaseName);

                //cmbProductType.Items.Clear();
                //var prods = (from DataRow row in ProductTypeResult.Rows
                //             select new ProductTypeDTO()
                //             {
                //                 TypeID = row["TypeID"].ToString(),
                //                 ProductType = row["ProductType"].ToString(),
                //                 Comments = row["Comments"].ToString(),
                //                 Description = row["Description"].ToString(),
                //                 ActiveStatus = row["ActiveStatus"].ToString()

                //             }
                //           ).ToList();
                //if (prods != null && prods.Any())
                //{
                //    var prodtype = from prd in prods.ToList()
                //                   select prd.ProductType;
                //    cmbProductType.Items.AddRange(prodtype.ToArray());
                //}
                ////string decriptedProductTypeData = Converter.DESDecrypt(PassPhrase, EnKey, ProductTypeResult);
                ////string[] ProductTypeLines = decriptedProductTypeData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                ////cmbProductType.Items.Clear();
                ////for (int j = 0; j < Convert.ToInt32(ProductTypeLines.Length); j++)
                ////{
                ////    string[] ProductTypeFields = ProductTypeLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                ////    try
                ////    {
                ////        cmbProductType.Items.Add(ProductTypeFields[1].ToString());
                ////    }
                ////    catch (Exception ex)
                ////    {
                ////        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699){MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);  return;} MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ////        return;
                ////    }
                ////}
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            ReportShowDs();
        }

        private void ReportShowDs()
        {
            ReportDocument objrpt = new ReportDocument();

            try
            {
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                cmbProductTypeText = cmbProductType.SelectedIndex != 1 ? cmbProductType.Text.Trim() : "";


              objrpt=  ReportShow();

                if(objrpt == null)
                {
                    return;
                }

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                 //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                //backgroundWorkerPreview.RunWorkerAsync();

                #region Commented Kodz

                //string ReportData =

                //    txtCategoryName.Text.Trim() + FieldDelimeter +
                //    "" + FieldDelimeter +
                //    cmbProductType.Text.Trim() +
                // LineDelimeter;

                //string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, ReportData);
                //ReportDSSoapClient ShowReport = new ReportDSSoapClient();
                //DataSet ReportResult = ShowReport.ProductCategory(encriptedData.ToString(), Program.DatabaseName);

                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //ReportResult.Tables[0].TableName = "DsProductCategory";
                //RptProductCategoryListing objrpt = new RptProductCategoryListing();

                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Product Category Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                //FormReport reports = new FormReport(); objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(objrpt);
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs",

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

                FileLogger.Log(this.Name, "ReportShowDs",

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
                FileLogger.Log(this.Name, "ReportShowDs",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDs",

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
                FileLogger.Log(this.Name, "ReportShowDs",

                               exMessage);
            }

            #endregion
        }
        #region Background Worker Events

       

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try

            try
            {
                
                
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
        private ReportDocument ReportShow()
        {
            ReportDocument objrpt = new ReportDocument();
            try
            {

                ProductCategoryInformationReport _report = new ProductCategoryInformationReport();
                reportDocument = _report.ProductCategoryNew(txtCGId.Text.Trim(),connVM);

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return reportDocument;
                }

            //ReportResult = new DataSet();

            //ReportDSDAL reportDsdal = new ReportDSDAL();

            //ReportResult = reportDsdal.ProductCategoryNew(txtCGId.Text.Trim());
            }
            catch (Exception ex )
            {

                throw ex;
            }

            return reportDocument;
            #region Comment
            // #region Try

           // try
           // {
           //     if (ReportResult.Tables.Count <= 0)
           //     {
           //         MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
           //         return objrpt;
           //     }
           //     ReportResult.Tables[0].TableName = "DsProductCategory";
           //     objrpt = new RptProductCategoryListing();

           //     objrpt.SetDataSource(ReportResult);

           //     objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
           //     objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Product Category Information'";
           //     objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
           //     objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
           //     objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
           //     objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
           //     objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
           //     objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
           //     //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
           //     //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
           //     //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

           //     FormulaFieldDefinitions crFormulaF;
           //     crFormulaF = objrpt.DataDefinition.FormulaFields;
           //     CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
           //     _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

           //     objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";

           //     //FormReport reports = new FormReport();
           //     //reports.crystalReportViewer1.Refresh();
           //     //reports.setReportSource(objrpt);
           // }
           // #endregion

           // #region catch

           // catch (IndexOutOfRangeException ex)
           // {
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    ex.Message + Environment.NewLine + ex.StackTrace);
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

           // }
           // catch (NullReferenceException ex)
           // {
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    ex.Message + Environment.NewLine + ex.StackTrace);
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

           // }
           // catch (FormatException ex)
           // {

           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    ex.Message + Environment.NewLine + ex.StackTrace);
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

           // }

           // catch (SoapHeaderException ex)
           // {
           //     string exMessage = ex.Message;
           //     if (ex.InnerException != null)
           //     {
           //         exMessage = exMessage + Environment.NewLine +

           //                     ex.InnerException.Message + Environment.NewLine +
           //                     ex.StackTrace;

           //     }

           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    exMessage);
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

           // }
           // catch (SoapException ex)
           // {
           //     string exMessage = ex.Message;
           //     if (ex.InnerException != null)
           //     {
           //         exMessage = exMessage + Environment.NewLine +

           //                     ex.InnerException.Message + Environment.NewLine +
           //                     ex.StackTrace;

           //     }
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    exMessage);
           // }
           // catch (EndpointNotFoundException ex)
           // {
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    ex.Message + Environment.NewLine + ex.StackTrace);
           // }
           // catch (WebException ex)
           // {
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    ex.Message + Environment.NewLine + ex.StackTrace);
           // }
           // catch (Exception ex)
           // {
           //     string exMessage = ex.Message;
           //     if (ex.InnerException != null)
           //     {
           //         exMessage = exMessage + Environment.NewLine +

           //                     ex.InnerException.Message + Environment.NewLine +
           //                     ex.StackTrace;

           //     }
           //     MessageBox.Show(ex.Message, this.Text,

           //                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
           //     FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

           //                    exMessage);
           // }

           // #endregion
           // finally
           // {
           //     this.progressBar1.Visible = false;
           //     this.btnPreview.Enabled = true;
           // }
            //// return objrpt;
            #endregion

        }
        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                    if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsProductCategory";
                RptProductCategoryListing objrpt = new RptProductCategoryListing();

                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Product Category Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

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

        #endregion

    }
}
