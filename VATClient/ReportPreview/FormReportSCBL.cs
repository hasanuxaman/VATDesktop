using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using SymphonySofttech.Utilities;
////
using VATClient.ModelDTO;
using VATClient.ReportPages;
using VATServer.Library;
using VATViewModel.DTOs;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using VATServer.Ordinary;
using SymphonySofttech.Reports;
using System.Diagnostics;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormReportSCBL : Form
    {
        #region Variable
        private int BranchId = 0;
        private int TransferTo = 0;
        string IssueFromDate, IssueToDate;
        public string reportType = string.Empty;
        public string InEnglish = string.Empty;
        public string Toll = string.Empty;
        public int SenderBranchId = 0;
        private DataSet ds;

        private ReportDocument reportDocument = new ReportDocument();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion
        public FormReportSCBL()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
   
        private void ReportSCBL_Load(object sender, EventArgs e)
        {
            //if (reportType == "StockReportRM")
            //{
            //    dtpFromDate.Checked = true;
            //    dtpToDate.Checked = true;

            //    dtpFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            //    dtpToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));
            //}
            //else
            //{
            //    dtpFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            //    dtpFromDate.Checked = false;
            //    dtpToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            //    dtpToDate.Checked = false;
            //}
            dtpFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 00:00:00"));
            dtpToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy 23:59:59"));
            //dtpFromDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            dtpFromDate.Checked = false;
            //dtpToDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:") + 00);
            dtpToDate.Checked = false;

            if (reportType == "ChakKha")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                this.Text = "Report Chak-Kha";
            }

            if (reportType == "ChakKa")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                this.Text = "Report Chak-Ka";
            }
            if (reportType == "LocalPurchase")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                btnPrev.Visible = false;
                this.Text = "Report Local Purchase";
            }
            if (reportType == "ImportData")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = "Report Import Data";
            }
            if (reportType == "ReceiedVsSale")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                chbToll.Visible = true;
                this.Text = "Report ReceiedVsSale";
            }
            if (reportType == "SalesStatementForService")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                chbToll.Visible = true;
                this.Text = "Report Sales Statement For Service";
            }
            if (reportType == "SalesStatementDelivery")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                chbToll.Visible = true;
                this.Text = "Report Sales Statement Delivery";
            }
            if (reportType == "StockReportFG")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = "Report Stock Report FG";
            }
            if (reportType == "StockReportRM")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = "Report Stock Report RM";
            }
            if (reportType == "TransferToDepot")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = "Report Transfer To Depot";
            }
            if (reportType == "VDSStatement")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = "Report VDS Statement";
            }
            if (reportType == "SaleSummaryAllShift")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = true;
                this.Text = "Report Sale Summary All Shift";
            }

            if (reportType == "SaleSummarybyProduct")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                chbToll.Visible = true;
                this.Text = "Sale Summary by Product";
            }
            if (reportType == "InputValueChange")
            {
                lblTransferTo.Visible = false;
                cmbTransferTo.Visible = false;
                cmbBranchName.Visible = false;
                label2.Visible = false;
                btnPrev.Visible = true;
                chbInEnglish.Visible = false;
                Download.Visible = false;
                this.Text = " Input Value (7.5%) Change";
            }
           
            CommonDAL dal = new CommonDAL();
            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = dal.ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);
            cmbBranchName.SelectedValue = Program.BranchId;
            cmbTransferTo = dal.ComboBoxLoad(cmbTransferTo, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = dal.settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
            cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            
        }
        private void NullCheck()
        {
            try
            {
                if (dtpFromDate.Checked == false)
                {
                    IssueFromDate = "";
                    IssueFromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    IssueFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// dtpFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpToDate.Checked == false)
                {
                    IssueToDate = "";
                    IssueToDate = dtpFromDate.MaxDate.ToString("yyyy-MMM-dd");

                }
                else
                {
                    IssueToDate = dtpToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");//  dtpToDate.Value.ToString("yyyy-MMM-dd");
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
        private void btnPrev_Click(object sender, EventArgs e)
        {
            //this.progressBar1.Visible = true;
            //this.btnPrev.Enabled = false;
            //OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            //InEnglish = chbInEnglish.Checked ? "Y" : "N";
            //BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
            //BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
            //TransferTo = Convert.ToInt32(cmbTransferTo.SelectedValue);
            //NullCheck();

            #region try
            this.progressBar1.Visible = true;
            this.btnPrev.Enabled = false;
            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                InEnglish = chbInEnglish.Checked ? "Y" : "N";
                Toll = chbToll.Checked ? "Y" : "N";

                #region LocalPurchase
                if (reportType=="LocalPurchase")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().LocalPurchaseReport(IssueFromDate, IssueToDate, BranchId,connVM);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblLocalPurcchase";



                    objrpt = new RptScblLocalPurchasge();

                    objrpt.SetDataSource(ds);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region ImportData
                else if (reportType == "ImportData")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().ImportDataReport(IssueFromDate, IssueToDate, BranchId, connVM);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblImportData";



                    objrpt = new RptScblImportData();

                    objrpt.SetDataSource(ds);




                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Statement of Import Purchase");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region ReceiedVsSale
                else if (reportType == "ReceiedVsSale")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().ReceiedVsSaleReport(IssueFromDate, IssueToDate, BranchId, connVM, Toll);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblReceiedVsSaleFromDepot";



                    objrpt = new RptScblReceiedVsSaleFromDepot();

                    objrpt.SetDataSource(ds);

                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Received and Sales Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }



                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region SalesStatementForService
                else if (reportType == "SalesStatementForService")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().SalesStatementForServiceReport(IssueFromDate, IssueToDate, BranchId, connVM, Toll);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblSalesStatementService";



                    objrpt = new RptScblSalesStatementService();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Freight/Transport Service Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region SalesStatementDelivery
                else if (reportType == "SalesStatementDelivery")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().SalesStatementDeliveryReport(IssueFromDate, IssueToDate, BranchId,connVM,Toll);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblSalesStatementDelivery";

                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptScblSalesStatementDelivery.rpt");


                    objrpt = new RptScblSalesStatementDelivery();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Sales/Delivery Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region StockReportFG
                else if (reportType == "StockReportFG")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().StockReportFGReport(IssueFromDate, IssueToDate, BranchId, connVM, null, Program.CurrentUserID);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblStockReportFG";



                    objrpt = new RptScblStockReportFG();

                    objrpt.SetDataSource(ds);

                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Report-Finished Goods");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }



                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region StockReportRM
                else if (reportType == "StockReportRM")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().StockReportRMReport(IssueFromDate, IssueToDate, BranchId, connVM, Program.CurrentUserID);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblStockReportRM";



                    objrpt = new RptScblStockReportRM();

                    objrpt.SetDataSource(ds);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Report-Raw Material");

                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region TransferToDepot
                else if (reportType == "TransferToDepot")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().TransferToDepotReport(IssueFromDate, IssueToDate, BranchId, connVM);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblTransferDepot";



                    objrpt = new RptScblTransferDepot();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Transfer Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region VDSStatement
                else if (reportType == "VDSStatement")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().VDSStatementReport(IssueFromDate, IssueToDate, BranchId, connVM);

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblVDSStatement";



                    objrpt = new RptScblVDSStatement();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Statement of Local Purchase(Goods & Services)");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region Chak Ka & Kha
                #region Chak Ka
                else if (reportType == "ChakKa")
                {

                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    TransferTo = Convert.ToInt32(cmbTransferTo.SelectedValue);
                    NullCheck();

                    NBRReports _report = new NBRReports();
                    reportDocument = _report.Chak_kaReport(IssueFromDate, IssueToDate, BranchId, TransferTo,InEnglish,connVM);
                    #region Report
                    //DataSet ds = new DataSet();

                    //ds = new ReportDSDAL().Chak_kaReport(IssueFromDate, IssueToDate, BranchId, TransferTo);//

                    //ReportDocument objrpt = new ReportDocument();

                    //ds.Tables[0].TableName = "dtChakKa";



                    //objrpt = new RptChakKa();

                    //objrpt.SetDataSource(ds);


                    //FormulaFieldDefinitions crFormulaF;
                    //crFormulaF = objrpt.DataDefinition.FormulaFields;
                    //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    #endregion

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

                }
                #endregion
                #region Chak Kha
                else if (reportType == "ChakKha")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();


                    NBRReports _report = new NBRReports();
                    reportDocument = _report.Chak_khaReport(IssueFromDate, IssueToDate, BranchId, InEnglish,connVM);
                    #region Report 
                    //DataSet ds = new DataSet();

                    //ds = new ReportDSDAL().Chak_khaReport(IssueFromDate, IssueToDate, BranchId);

                    //ReportDocument objrpt = new ReportDocument();

                    //ds.Tables[0].TableName = "dtChakKha";



                    //objrpt = new RptChakKha();

                    //objrpt.SetDataSource(ds);


                    //FormulaFieldDefinitions crFormulaF;
                    //crFormulaF = objrpt.DataDefinition.FormulaFields;
                    //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    #endregion
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

                }
                #endregion
                #endregion
                #region SaleSummaryAllShift
                if (reportType == "SaleSummaryAllShift")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();
                    DataSet ReportResult = new DataSet();

                    ds = new ReportDSDAL().SaleSummaryAllShiftReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    ds.Tables[0].TableName = "DsScblSaleSummary";
                    ReportResult = new ReportDSDAL().SaleNew("", IssueFromDate, IssueToDate, "", "", "", "", "", "", "", false, "", false, "", "0", 0, "", connVM,"","","N","","");
                    DataTable dt = ds.Tables[0].Copy();
                    ReportDocument objrpt = new ReportDocument();
                    ReportResult.Tables.Add(dt);
                    ReportResult.Tables[0].TableName = "DsSale";
                    ReportResult.Tables[1].TableName = "DsSalePCategory";
                    ReportResult.Tables[2].TableName = "DsSaleCustomerPCategory";
                    //ReportResult.Tables[3].TableName = "DsScblSaleSummary";


                    //objrpt = new RptScblSalesSummery();
                    objrpt = new RptSalesSummery_CustomerAndAllShift();
                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptSalesSummery(Customer&AllShift).rpt");
                    objrpt.SetDataSource(ReportResult);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Sales/Delivery Statement");
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", IssueFromDate);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", IssueToDate);
                  
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion

                #region SaleSummarybyProduct
                else if (reportType == "SaleSummarybyProduct")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().SaleNew("", IssueFromDate, IssueToDate, "", "", "","", "", "","", false,
                        "", false, "", "0", BranchId, "", connVM, "ProductCode", "", Toll, "", "SummaryByProduct");

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "DsSale";




                    objrpt = new RptScblSalesSummeryByProduct();

                    objrpt.SetDataSource(ds);
                    #region Formula
                    string Promotional = "Sales Report (Local) Summary" + ". Shift : " +"All Shift";
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + Promotional + "' ";

                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";


                    if (dtpFromDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                                                                                dtpFromDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                                "'  ";
                    }

                    if (dtpToDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                                                                              dtpToDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                              "'  ";
                    }


                    #endregion Formula


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion

                #region InputValue7_5Change
                else if (reportType == "InputValueChange")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();
                    ds = new ReportDSDAL().InputValue7_5percent(IssueFromDate, IssueToDate, Program.BranchId, connVM);

                    ds.Tables[0].TableName = "dtInputValue7_5";

                    ReportDocument objrpt = new ReportDocument();





                    objrpt = new RptScblInputValue7_5();

                    objrpt.SetDataSource(ds);
                    #region Formula
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Statement of Input Value (7.5%) Change'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    if (dtpFromDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["IssueFromDate"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["IssueFromDate"].Text = "'" +
                                                                                dtpFromDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                                "'  ";
                    }

                    if (dtpToDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["IssueToDate"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["IssueToDate"].Text = "'" +
                                                                              dtpToDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                              "'  ";
                    }


                    #endregion Formula


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion


            }
                  #endregion
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrev.Enabled = true;
            }
        }

        //private void NullCheck()
        //{
        //    try
        //    {
                

        //        if (dtpSalesFromDate.Checked == false)
        //        {
        //            SalesFromDate = "";
        //        }
        //        else
        //        {
        //            SalesFromDate = dtpSalesFromDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");// dtpSalesFromDate.Value.ToString("yyyy-MMM-dd");
        //        }
        //        if (dtpSalesToDate.Checked == false)
        //        {
        //            SalesToDate = "";
        //        }
        //        else
        //        {
        //            SalesToDate = dtpSalesToDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");//  dtpSalesToDate.Value.ToString("yyyy-MMM-dd");
        //        }
               
        //    }
        //    #region Catch
        //    catch (IndexOutOfRangeException ex)
        //    {
        //        FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (FormatException ex)
        //    {

        //        FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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

        //        FileLogger.Log(this.Name, "ClearAllFields", exMessage);
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
        //        FileLogger.Log(this.Name, "ClearAllFields", exMessage);
        //    }
        //    catch (EndpointNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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
        //        FileLogger.Log(this.Name, "ClearAllFields", exMessage);
        //    }
        //    #endregion Catch
        //}
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void SaveExcel(DataSet ds, string ReportType="",string BranchName="")
        {
            DataTable dt = ds.Tables["Table"];
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            //tempDS = new ReportDSDAL().ComapnyProfile("");
            IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            tempDS = ReportDSDal.ComapnyProfile("",connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            var ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            var VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            var Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();



            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo, ReportType, " Period: " + IssueFromDate + " TO " + IssueToDate, " Branch Name: " + BranchName };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k], OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }



            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };



                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }
                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }

                //ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
                #endregion


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);

        }

        private void Download_Click(object sender, EventArgs e)
        {
            try
            {
                #region LocalPurchase
                if (reportType == "LocalPurchase")
                {
                    string ReportType = "Statement of Local Purchase";
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    string BranchName = cmbBranchName.Text;
                    NullCheck();
                    DataSet ds = new DataSet();

                    //ds = new ReportDSDAL().LocalPurchaseReport(IssueFromDate, IssueToDate, BranchId);
                    //SaveExcel(ds, ReportType, BranchName);
                    IReport ReportDSDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
                    ds = ReportDSDal.LocalPurchaseReport(IssueFromDate, IssueToDate, BranchId,connVM);
                    SaveExcel(ds, ReportType, BranchName);


                }
                #endregion

                #region ImportData
               else if (reportType == "ImportData")
                {
                    string ReportType = "Statement of Import Purchase";
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    string BranchName = cmbBranchName.Text;
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().ImportDataReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    SaveExcel(ds, ReportType, BranchName);


                }
                #endregion

                #region ReceiedVsSale
                else if (reportType == "ReceiedVsSale")
                {
                    string ReportType = "Statement of ReceiedVsSale";
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    string BranchName = cmbBranchName.Text;
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().ReceiedVsSaleReport(IssueFromDate, IssueToDate, BranchId, connVM,"N");
                    SaveExcel(ds, ReportType, BranchName);


                }
                #endregion

                #region SalesStatementForService
                else if (reportType == "SalesStatementForService")
                {
                    string ReportType = "Sales Statement For Service";
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    string BranchName = cmbBranchName.Text;
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().SalesStatementForServiceReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    SaveExcel(ds, ReportType, BranchName);


                }
                #endregion

                #region SalesStatementDelivery
                else if (reportType == "SalesStatementDelivery")
                {
                    string ReportType = "Sales Statement Delivery";
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    string BranchName = cmbBranchName.Text;
                    NullCheck();
                    DataSet ds = new DataSet();

                    ds = new ReportDSDAL().SalesStatementDeliveryReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    SaveExcel(ds, ReportType, BranchName);


                }
                #endregion

                #region SaleSummaryAllShift
                if (reportType == "SaleSummaryAllShift")
                {
                    BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);
                    NullCheck();
                    DataSet ds = new DataSet();
                    DataSet ReportResult = new DataSet();
                    DataSet result = new DataSet();

                    ds = new ReportDSDAL().SaleSummaryAllShiftReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    ds.Tables[0].TableName = "DsScblSaleSummary";
                    ReportResult = new ReportDSDAL().SaleNew("", IssueFromDate, IssueToDate, "", "", "", "", "", "", "", false, "", false, "", "0", 0, "", connVM, "", "", "N", "", "");
                    DataTable dt = ds.Tables[0].Copy();
                    DataTable dtt = ReportResult.Tables[0].Copy();
                    OrdinaryVATDesktop.DtDeleteColumns(dtt, new[] { "SalesInvoiceNo", "InvoiceDateTime", "Address1", "VATRegistrationNo", "ItemNo", "UOMn", "UOMc", "VehicleNo", "PromotionalQuantity", "SaleQuantity", "UOMQty", "NBRPrice" 
                      , "Discount", "UOMPrice", "SD", "SDAmount", "VATRate", "VATAmount", "SubTotal", "Total", "Trading", "TradingMarkUp", "NonStock", "SaleType", "LCNumber", "LCDate", "CategoryName","IsRaw","ProductType","Heading1","Heading2"});
                    ReportDocument objrpt = new ReportDocument();

                    result.Tables.Add(dtt);
                    result.Tables.Add(dt);
                    OrdinaryVATDesktop.SaveExcelMultiple(result, " Sale Summary All Shift", new[] { "SalesStatement", "SaleSummaryAllShift" });

                } 
                #endregion




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
                FileLogger.Log(this.Name, "Download_Click", exMessage);
            }

        }

        private void chbInEnglish_Click(object sender, EventArgs e)
        {
            chbInEnglish.Text = "Bangla";
            if (chbInEnglish.Checked)
            {
                chbInEnglish.Text = "English";

            }
        }

        private void bgwPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
           
            try
            {
                

                #region LocalPurchase
                if (reportType == "LocalPurchase")
                {


                    ds = new ReportDSDAL().LocalPurchaseReport(IssueFromDate, IssueToDate, BranchId, connVM);
            

                }
                #endregion
                #region ImportData
                else if (reportType == "ImportData")
                {


                    ds = new ReportDSDAL().ImportDataReport(IssueFromDate, IssueToDate, BranchId, connVM);

                    

                }
                #endregion
                #region ReceiedVsSale
                else if (reportType == "ReceiedVsSale")
                {


                    ds = new ReportDSDAL().ReceiedVsSaleReport(IssueFromDate, IssueToDate, BranchId, connVM);

                  

                }
                #endregion
                #region SalesStatementForService
                else if (reportType == "SalesStatementForService")
                {


                    ds = new ReportDSDAL().SalesStatementForServiceReport(IssueFromDate, IssueToDate, BranchId, connVM);

                 

                }
                #endregion
                #region SalesStatementDelivery
                else if (reportType == "SalesStatementDelivery")
                {


                    ds = new ReportDSDAL().SalesStatementDeliveryReport(IssueFromDate, IssueToDate, BranchId, connVM);

                   

                }
                #endregion
                #region StockReportFG
                else if (reportType == "StockReportFG")
                {

                    ds = new ReportDSDAL().StockReportFGReport(IssueFromDate, IssueToDate, BranchId, connVM, null, Program.CurrentUserID);

                   

                }
                #endregion
                #region StockReportRM
                else if (reportType == "StockReportRM")
                {


                    ds = new ReportDSDAL().StockReportRMReport(IssueFromDate, IssueToDate, BranchId,connVM,
                        Program.CurrentUserID);



                }
                #endregion
                #region TransferToDepot
                else if (reportType == "TransferToDepot")
                {
                   

                    ds = new ReportDSDAL().TransferToDepotReport(IssueFromDate, IssueToDate, BranchId, connVM);

                   

                }
                #endregion
                #region VDSStatement
                else if (reportType == "VDSStatement")
                {
                   

                    ds = new ReportDSDAL().VDSStatementReport(IssueFromDate, IssueToDate, BranchId, connVM);

                   

                }
                #endregion
                #region Chak Ka & Kha
                #region Chak Ka
                else if (reportType == "ChakKa")
                {

                    
                    NullCheck();

                    NBRReports _report = new NBRReports();
                    reportDocument = _report.Chak_kaReport(IssueFromDate, IssueToDate, BranchId, TransferTo, InEnglish,connVM);
                   

                }
                #endregion
                #region Chak Kha
                else if (reportType == "ChakKha")
                {
                    


                    NBRReports _report = new NBRReports();
                    reportDocument = _report.Chak_khaReport(IssueFromDate, IssueToDate, BranchId, InEnglish,connVM);
                   

                }
                #endregion
                #endregion
                #region SaleSummaryAllShift
                if (reportType == "SaleSummaryAllShift")
                {
                    

                    ds = new ReportDSDAL().SaleSummaryAllShiftReport(IssueFromDate, IssueToDate, BranchId, connVM);
                    

                }
                #endregion

                #region SaleSummarybyProduct
                else if (reportType == "SaleSummarybyProduct")
                {
                   

                    ds = new ReportDSDAL().SaleNew("", IssueFromDate, IssueToDate, "", "", "", "", "", "", "",false,
                        "", false, "", "0", BranchId, "", connVM, "ProductCode");

               

                }
                #endregion


            }
            #endregion
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion
            finally
            {
               
            }
        }

        private void bgwPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            
            try
            {
           

                #region LocalPurchase
                if (reportType == "LocalPurchase")
                {
                    
                  

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblLocalPurcchase";



                    objrpt = new RptScblLocalPurchasge();

                    objrpt.SetDataSource(ds);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region ImportData
                else if (reportType == "ImportData")
                {
                  

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblImportData";



                    objrpt = new RptScblImportData();

                    objrpt.SetDataSource(ds);




                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Statement of Import Purchase");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region ReceiedVsSale
                else if (reportType == "ReceiedVsSale")
                {


                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblReceiedVsSaleFromDepot";



                    objrpt = new RptScblReceiedVsSaleFromDepot();

                    objrpt.SetDataSource(ds);

                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Received and Sales Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }



                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region SalesStatementForService
                else if (reportType == "SalesStatementForService")
                {
;

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblSalesStatementService";



                    objrpt = new RptScblSalesStatementService();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Freight/Transport Service Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region SalesStatementDelivery
                else if (reportType == "SalesStatementDelivery")
                {


                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblSalesStatementDelivery";



                    objrpt = new RptScblSalesStatementDelivery();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Sales/Delivery Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region StockReportFG
                else if (reportType == "StockReportFG")
                {
                   

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblStockReportFG";



                    objrpt = new RptScblStockReportFG();

                    objrpt.SetDataSource(ds);

                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Report-Finished Goods");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }



                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region StockReportRM
                else if (reportType == "StockReportRM")
                {
           

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblStockReportRM";



                    objrpt = new RptScblStockReportRM();

                    objrpt.SetDataSource(ds);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Report-Raw Material");

                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region TransferToDepot
                else if (reportType == "TransferToDepot")
                {
                 

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblTransferDepot";



                    objrpt = new RptScblTransferDepot();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Transfer Statement");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region VDSStatement
                else if (reportType == "VDSStatement")
                {
              

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "dtScblVDSStatement";



                    objrpt = new RptScblVDSStatement();

                    objrpt.SetDataSource(ds);


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Statement of Local Purchase(Goods & Services)");
                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", "All");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueFromDate", IssueFromDate);
                    }
                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", "All");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IssueToDate", IssueToDate);
                    }

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion
                #region Chak Ka & Kha
                #region Chak Ka
                else if (reportType == "ChakKa")
                {

                    

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

                }
                #endregion
                #region Chak Kha
                else if (reportType == "ChakKha")
                {
                   
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

                }
                #endregion
                #endregion
                #region SaleSummaryAllShift
                if (reportType == "SaleSummaryAllShift")
                {
                  
                    DataSet ReportResult = new DataSet();

                    ds.Tables[0].TableName = "DsScblSaleSummary";
                    ReportResult = new ReportDSDAL().SaleNew("", IssueFromDate, IssueToDate, "", "", "", "", "", "", "", false, "", false, "", "0", 0, "", connVM,"","");
                    DataTable dt = ds.Tables[0].Copy();
                    ReportDocument objrpt = new ReportDocument();
                    ReportResult.Tables.Add(dt);
                    ReportResult.Tables[0].TableName = "DsSale";
                    ReportResult.Tables[1].TableName = "DsSalePCategory";
                    ReportResult.Tables[2].TableName = "DsSaleCustomerPCategory";
                    //ReportResult.Tables[3].TableName = "DsScblSaleSummary";


                    //objrpt = new RptScblSalesSummery();
                    objrpt = new RptSalesSummery_CustomerAndAllShift();
                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptSalesSummery(Customer&AllShift).rpt");
                    objrpt.SetDataSource(ReportResult);



                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Sales/Delivery Statement");
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", IssueFromDate);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", IssueToDate);

                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion

                #region SaleSummarybyProduct
                else if (reportType == "SaleSummarybyProduct")
                {
                    

                    ReportDocument objrpt = new ReportDocument();

                    ds.Tables[0].TableName = "DsSale";




                    objrpt = new RptScblSalesSummeryByProduct();

                    objrpt.SetDataSource(ds);
                    #region Formula
                    string Promotional = "Sales Report (Local) Summary" + ". Shift : " + "All Shift";
                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + Promotional + "' ";

                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";


                    if (dtpFromDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                                                                                dtpFromDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                                "'  ";
                    }

                    if (dtpToDate.Checked == false)
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                                                                              dtpToDate.Value.ToString("dd/MMM/yyyy HH:mm") +
                                                                              "'  ";
                    }


                    #endregion Formula


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();

                    reports.setReportSource(objrpt);

                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();

                }
                #endregion


            }
            #endregion
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrev.Enabled = true;
            }
        }
    }
}
