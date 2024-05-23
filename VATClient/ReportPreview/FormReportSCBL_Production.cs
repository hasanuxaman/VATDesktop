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
    public partial class FormReportSCBL_Production : Form
    {
        #region Variable
        private int BranchId = 0;
        private int TransferTo = 0;
        string IssueFromDate, IssueToDate;
        public string reportType = string.Empty;
        public string InEnglish = string.Empty;
        public int SenderBranchId = 0;
        private int YearLines;
        private ReportDocument reportDocument = new ReportDocument();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable YearResult;
        private DataSet ds;
        string FiscalYear = null;
        #endregion
        public FormReportSCBL_Production()
        {
            InitializeComponent();

            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
       
            connVM = Program.OrdinaryLoad();

        }
   
        private void ReportSCBL_Load(object sender, EventArgs e)
        {
            SelectYear();
           
            
        }
        private void SelectYear()
        {
            try
            {
                this.progressBar1.Visible = true;
                bgwSelectYear.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion
        }
       

        private void label2_Click(object sender, EventArgs e)
        {

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
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

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
                YearLines = YearResult.Rows.Count-1;
                //dtpFYearStart.Value = Convert.ToDateTime(Program.FMonthStart.ToString("dd/MMM/yyyy"));// Convert.ToDateTime(YearResult.Tables[1].Rows[0][0]);
                //dtpFYearEnd.Value = Convert.ToDateTime(Program.FMonthEnd.ToString("dd/MMM/yyyy"));//Convert.ToDateTime(YearResult.Tables[1].Rows[0][1]);
                cmbFiscalYear.Items.Clear();
                for (int j = 0; j <(YearLines); j++)
                {
                    cmbFiscalYear.Items.Add(YearResult.Rows[j][0].ToString());
                }
                cmbFiscalYear.Text = (DateTime.Now.ToString("yyyy"));
                // End Complete
                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

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
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void btnPrev_Click_1(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtItemNo.Text))
            {
                MessageBox.Show("Please Select Product Name First");
                txtItemNo.Focus();

                return;

            }
            this.progressBar1.Visible = true;
            this.btnPrev.Enabled = false;
            OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
             FiscalYear = cmbFiscalYear.Text;

            bgwPreview.RunWorkerAsync();
        }

        private void bgwPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            
            #region try

            try
            {

                #region StockReportFG

                BranchId = Program.BranchId;
                //DataSet ds = new DataSet();

                ds = new ReportDSDAL().MonthlyProduction_DeliveryReport(txtItemNo.Text, BranchId,connVM, FiscalYear,Program.CurrentUserID);

             
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

            try
            {
                ReportDocument objrpt = new ReportDocument();

                ds.Tables[0].TableName = "dtScblStockMonthlyProduction";



                objrpt = new RptScblMonthly_Production_Delivery();

                objrpt.SetDataSource(ds);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", OrdinaryVATDesktop.CompanyName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", OrdinaryVATDesktop.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", OrdinaryVATDesktop.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", txtProName.Text);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", txtUOM.Text);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Monthly Production & Delivery Report");


                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();

                reports.setReportSource(objrpt);

                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

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
                FileLogger.Log(this.Name, "bgwPreview_RunWorkerCompleted", exMessage);
            }
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrev.Enabled = true;
            }

        }

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();
        }

        private void ProductSelect()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' and pc.IsRaw='Finish' ";
                //if (!string.IsNullOrEmpty(CategoryId))
                //{
                //    SqlText += @"  and pc.CategoryID='" + CategoryId + "'  ";
                //}
                //else if (!string.IsNullOrEmpty(IsRawParam))
                //{
                //    SqlText += @"  and pc.IsRaw='" + IsRawParam + "'  ";
                //}

                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                    txtProName.Text = selectedRow.Cells["ProductName"].Value.ToString();
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                }

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductGroupLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtProName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductSelect();
            }
        }

    }
}
