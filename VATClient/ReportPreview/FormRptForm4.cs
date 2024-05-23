using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using SymphonySofttech.Reports.List;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATServer.License;
using VATViewModel.DTOs;
using CrystalDecisions.CrystalReports.Engine;

namespace VATClient.ReportPreview
{
    public partial class FormRptForm4 : Form
    {
        public FormRptForm4()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }

			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string BanderolFromDate, BanderolToDate;
        
        private bool PreviewOnly = false;
        


        #region Global Variables For BackGroundWorker

        private string vSalePlaceTaka, vSalePlaceQty, vSalePlaceDollar, vSalePlaceRate;
        private int SalePlaceTaka, SalePlaceQty, SalePlaceDollar, SalePlaceRate = 0;
        
        private DataSet ReportResult;
        string post1, post2 = string.Empty;
        private decimal TCloseQty = 0;
        
        #endregion

        private void FormRptForm4_Load(object sender, EventArgs e)
        {

            CommonDAL commonDal = new CommonDAL();

            #region Preview button

            btnPrev.Visible = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM) == "Y";

            #endregion
 
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                Program.R_F = "";
                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormBanderolNameSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtBanderolId.Text = selectedRow.Cells["BanderolID"].Value.ToString();
                    txtBanderolName.Text = selectedRow.Cells["BanderolName"].Value.ToString();

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
        private void NullCheck()
        {
            try
            {
               
                if (dtpFromDate.Checked == false)
                {
                    BanderolFromDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    BanderolFromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpToDate.Checked == false)
                {
                    BanderolToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    BanderolToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");
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
        private void button1_Click(object sender, EventArgs e)
        {
            //this.btnPrev.Enabled = false;
            //this.progressBar1.Visible = true;
            //this.button1.Enabled = false;
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                PreviewOnly = false;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtBanderolName.Text == "")
                {
                    MessageBox.Show("Please select the Banderol", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
               
                if (dtpFromDate.Value > dtpToDate.Value.AddDays(1))
                {
                    MessageBox.Show("End date never greater tand From Date", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    dtpToDate.Focus();
                    return;

                }


                //var tt = rbtnService.Checked;
                ReportShowDs();
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //===========================================================
        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                //this.btnPrev.Enabled = false;
                //this.progressBar1.Visible = true;
                //this.button1.Enabled = false;

                PreviewOnly = true;
                if (Program.CheckLicence(dtpToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtBanderolName.Text == "")
                {
                    MessageBox.Show("Please Select the Product", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (dtpFromDate.Value > dtpToDate.Value.AddDays(1))
                {
                    MessageBox.Show("End date never greater tand From Date", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    dtpToDate.Focus();
                    return;

                }

                ReportShowDs();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrev_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrev_Click", exMessage);
            }
            #endregion Catch
        }
        private void ReportShowDs()
        {
            try
            {
                NullCheck();

                if (PreviewOnly == true)
                {
                    post1 = "Y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                
                

                this.btnPrev.Enabled = false;
                this.progressBar1.Visible = true;
                this.button1.Enabled = false;

                backgroundWorkerPreview.RunWorkerAsync();

                
                #region Complete
                //if (ReportResult.Tables.Count <= 0)
            
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

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.BanderolForm_4(txtBanderolId.Text.Trim(), post1, BanderolFromDate, BanderolToDate, post2, connVM);

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
        private void temp17()
        {
            #region Statement

            decimal vColSerialNo = 0;
            string vColFiscalYear = string.Empty;
            DateTime vColTranDate = DateTime.MinValue;
            string vColDemandNo = string.Empty;
            string vColProductName = string.Empty;
            string vColPack = string.Empty;
            string vColBanderolInfo = string.Empty;
            decimal vColDemQuantity = 0;
            string vColRecNoDate = string.Empty;
            decimal vColRecQuantity = 0;
            decimal vColCFYRecQty = 0;
            decimal vColUsedQty = 0;
            decimal vColWastageQty = 0;
            decimal vColClosingQty = 0;
            decimal vColSaleQty = 0;
            string vColRemarks = string.Empty;
            string vTransType = string.Empty;

            string vColBanderolID = string.Empty;
            string vColPackagingId = string.Empty;

            decimal vClosingQuantity = 0;
            decimal OpeningQty = 0;
            decimal ReceiveQty = 0;
            decimal SaleQty = 0;
            decimal UsedQty = 0;
            decimal WastageQty = 0;
            decimal OpQty = 0;
            decimal CFYRQty = 0;


            decimal vTempColSerialNo = 0;
            string vTempColFiscalYear = string.Empty;
            DateTime vTempColTranDate = DateTime.MinValue;
            string vTempColDemandNo = string.Empty;
            string vTempColProductName = string.Empty;
            string vTempColPack = string.Empty;
            string vTempColBanderolInfo = string.Empty;
            decimal vTempColDemQuantity = 0;
            string vTempColRecNoDate = string.Empty;
            decimal vTempColRecQuantity = 0;
            decimal vTempColCFYRecQty = 0;
            decimal vTempColUsedQty = 0;
            decimal vTempColWastageQty = 0;
            decimal vTempColClosingQty = 0;
            decimal vTempColSaleQty = 0;
            string vTempColRemarks = string.Empty;
            string vTemptransType = string.Empty;
              #endregion Statement           
            #region
            //DateTime vColumn11 = DateTime.MinValue;


           
            //decimal vColumn16 = 0;
            //decimal vColumn17 = 0;
            //decimal vColumn18 = 0;
            //string vColumn19 = string.Empty;

            //decimal vTempSerial = 0;
            //DateTime vTempStartDateTime = DateTime.MinValue;
            //decimal vTempStartingQuantity = 0;
            //decimal vTempStartingAmount = 0;
            //decimal vTempQuantity = 0;
            //decimal vTempSubtotal = 0;
            //string vTempCustomerName = string.Empty;
            //string vTempVATRegistrationNo = string.Empty;
            //string vTempAddress1 = string.Empty;
            //string vTempTransID = string.Empty;
            //DateTime vTemptransDate = DateTime.MinValue;
            //string vTempProductName = string.Empty;
            //decimal vTempSDAmount = 0;
            //decimal vTempVATAmount = 0;
            //string vTempremarks = string.Empty;
            //string vTemptransType = string.Empty;

            //decimal vClosingQuantity = 0;
            //decimal vClosingAmount = 0;
            //decimal vClosingAvgRatet = 0;

            //decimal OpeningQty = 0;
            //decimal OpeningAmnt = 0;
            //decimal ReceiveQty = 0;
            //decimal ProductionAmnt = 0;
            //decimal SaleQty = 0;
            //decimal SaleAmnt = 0;
            

            //decimal OpQty = 0;
            //decimal OpAmnt = 0;
            //decimal avgRate = 0;
            //string HSCode = string.Empty;
            //string ProductName = string.Empty;



            #endregion

            int i = 1;
                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("remarks='Opening'");
                #region Opening
                foreach (DataRow row in DetailRawsOpening)
                {
                    vTempColRemarks = row["remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();

                    vTempColFiscalYear = row["FiscalYear"].ToString().Trim();
                    vTempColTranDate = Convert.ToDateTime(row["TranDate"].ToString().Trim());

                    OpQty = Convert.ToDecimal(row["ColDemQuantity"].ToString().Trim());

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpQty = 0;

                    ReceiveQty = 0;
                    UsedQty = 0;
                    WastageQty = 0;
                    SaleQty = 0;


                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                         Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                    vColFiscalYear = vTempColFiscalYear;
                    vColTranDate = vTempColTranDate;
                    vColDemandNo = "-";
                    vColProductName = "-";
                    vColPack = "-";
                    vColBanderolInfo = " ";
                    vColDemQuantity = OpeningQty;
                    vColRecNoDate = "";
                    vColRecQuantity = ReceiveQty;
                    vColCFYRecQty = 0;
                    vColUsedQty = UsedQty;
                    vColWastageQty = WastageQty;
                    vColClosingQty = TCloseQty;
                    vColSaleQty = SaleQty;
                    vColRemarks = vTempColRemarks;
                    vTransType = vTemptransType;


                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                        Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                    #endregion

                    #region AssignValue to List

                    i = i + 1;

                    #endregion AssignValue to List
                }
                
                #endregion Opening


            DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("Remarks<>'Opening'");
            if (DetailRawsOthers == null || !DetailRawsOthers.Any())
            {
                //MessageBox.Show("There is no data to preview", this.Text);
                return;
            }
            
            string strSort = "TranDate ASC, SerialNo ASC";

            DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
            dtview.Sort = strSort;
            DataTable dtsorted = dtview.ToTable();

            foreach (DataRow item in dtsorted.Rows)
            {
                #region Get from Datatable

                OpeningQty = 0;
                ReceiveQty = 0;
                UsedQty = 0;
                WastageQty = 0;
                SaleQty = 0;
                TCloseQty = 0;
                

                vColSerialNo = i;
                vTempColFiscalYear = item["FiscalYear"].ToString(); // Date
                vTempColTranDate = Convert.ToDateTime(item["TranDate"].ToString()); // Opening Quantity
                vTempColDemandNo = item["DemandNo"].ToString(); // Opening Price
                vTempColProductName = item["ProductName"].ToString(); // Production Quantity
                vTempColPack = item["Pack"].ToString(); // Production Price
                //vTempColBanderolInfo = item["CustomerName"].ToString(); // Customer Name
                vTempColDemQuantity = Convert.ToDecimal(item["DemQuantity"].ToString()); // Customer VAT Reg No
                vTempColRecNoDate = item["RecNoDate"].ToString(); // Customer Address
                vTempColRecQuantity =Convert.ToDecimal( item["RecQuantity"].ToString()); // Sale Invoice No
                vTempColUsedQty = Convert.ToDecimal(item["UsedQty"].ToString()); // Sale Invoice Date and Time
                vTempColWastageQty =Convert.ToDecimal( item["WastageQty"].ToString()); // Sale Product Name
                vTempColSaleQty = Convert.ToDecimal(item["SaleQty"].ToString()); // SD Amount
                vTempColRemarks = item["remarks"].ToString(); // Remarks
                vTemptransType = item["TransType"].ToString().Trim();

                #endregion Get from Datatable

                if (vTempColRemarks.Trim() == "Sale")
                {
                    #region if row 1 Opening

                    OpeningQty = OpQty + vClosingQuantity;
                    OpQty = 0;
                    ReceiveQty = 0;
                    UsedQty = vTempColUsedQty;
                    WastageQty = vTempColWastageQty;
                    SaleQty = vTempColSaleQty;

                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                         Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                    vColFiscalYear = vTempColFiscalYear;
                    vColTranDate = vTempColTranDate;
                    vColDemandNo = "-";
                    vColProductName = vTempColProductName;
                    vColPack = vTempColPack;
                    //vColBanderolInfo = " ";
                    vColDemQuantity = vTempColDemQuantity;
                    vColRecNoDate = vTempColRecNoDate;
                    vColRecQuantity = vTempColRecQuantity;
                    vColCFYRecQty = 0;
                    vColUsedQty = vTempColUsedQty;
                    vColWastageQty = vTempColWastageQty;
                    vColClosingQty = TCloseQty;
                    vColSaleQty = vTempColSaleQty;
                    vColRemarks = vTempColRemarks;
                    vTransType = vTemptransType;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                        Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));
                   
                    #endregion
                    #region AssignValue to List

                    i = i + 1;

                    #endregion AssignValue to List

                }
                else if (vTempColRemarks == "Receive")
                {

                    #region if row 1 Opening

                    OpeningQty = OpQty + vClosingQuantity;
                    OpQty = 0;
                    ReceiveQty = vTempColRecQuantity;
                    UsedQty = vTempColUsedQty;
                    WastageQty = vTempColWastageQty;
                    SaleQty = vTempColSaleQty;
                    CFYRQty += ReceiveQty;
                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                         Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));


                    vColFiscalYear = vTempColFiscalYear;
                    vColTranDate = vTempColTranDate;
                    vColDemandNo = vTempColDemandNo;
                    vColProductName = vTempColProductName;
                    vColPack = vTempColPack;
                    //vColBanderolInfo = " ";
                    vColDemQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColDemQuantity.ToString(),2));
                    vColRecNoDate = vTempColRecNoDate;
                    vColRecQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColRecQuantity.ToString(),2));
                    vColCFYRecQty = Convert.ToDecimal(Program.FormatingNumeric(CFYRQty.ToString(),2));
                    vColUsedQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColUsedQty.ToString(),2));
                    vColWastageQty =Convert.ToDecimal(Program.FormatingNumeric( vTempColWastageQty.ToString(),2));
                    vColClosingQty = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(),2));
                    vColSaleQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColSaleQty.ToString(),2));
                    vColRemarks = vTempColRemarks;
                    vTransType = vTemptransType;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                        Convert.ToDecimal(SaleQty));
                   
                    #endregion

                    #region AssignValue to List

                    i = i + 1;

                    #endregion AssignValue to List
                }

            }

            
            #region Report preview
            
            // End Complete
            #endregion Report preview

            #endregion
        }
        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                DBSQLConnection _dbsqlConnection = new DBSQLConnection(); 
                
                // Start Complete

                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                //temp17();

                #region
                decimal vColSerialNo = 0;
                string vColFiscalYear = string.Empty;
                DateTime vColTranDate = DateTime.MinValue;
                string vColDemandNo = string.Empty;
                string vColProductName = string.Empty;
                string vColPack = string.Empty;
                string vColBanderolInfo = string.Empty;
                decimal vColDemQuantity = 0;
                string vColRecNoDate = string.Empty;
                decimal vColRecQuantity = 0;
                decimal vColCFYRecQty = 0;
                decimal vColUsedQty = 0;
                decimal vColWastageQty = 0;
                decimal vColClosingQty = 0;
                decimal vColSaleQty = 0;
                string vColRemarks = string.Empty;
                string vTransType = string.Empty;

                string vColBanderolID = string.Empty;
                string vColPackagingId = string.Empty;

                decimal vClosingQuantity = 0;
                decimal OpeningQty = 0;
                decimal ReceiveQty = 0;
                decimal SaleQty = 0;
                decimal UsedQty = 0;
                decimal WastageQty = 0;
                decimal OpQty = 0;
                decimal CFYRQty = 0;


                //decimal vTempColSerialNo = 0;
                string vTempColFiscalYear = string.Empty;
                DateTime vTempColTranDate = DateTime.MinValue;
                string vTempColDemandNo = string.Empty;
                string vTempColProductName = string.Empty;
                string vTempColPack = string.Empty;
                string vTempColBanderolInfo = string.Empty;
                decimal vTempColDemQuantity = 0;
                string vTempColRecNoDate = string.Empty;
                decimal vTempColRecQuantity = 0;
                decimal vTempColCFYRecQty = 0;
                decimal vTempColUsedQty = 0;
                decimal vTempColWastageQty = 0;
                //decimal vTempColClosingQty = 0;
                decimal vTempColSaleQty = 0;
                string vTempColRemarks = string.Empty;
                string vTemptransType = string.Empty;


                #endregion

                if (ReportResult.Tables[1].Rows.Count > 0)
                {
                    vColBanderolInfo = ReportResult.Tables[1].Rows[0][0].ToString();
                }


                List<BanderolForm_4> form4s = new List<BanderolForm_4>();
                BanderolForm_4 form4 = new BanderolForm_4();

                int i = 1;
                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("Remarks='Opening'");
                #region Opening
                foreach (DataRow row in DetailRawsOpening)
                {
                    vTempColRemarks = row["remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();

                    vTempColFiscalYear = row["FiscalYear"].ToString().Trim();
                    vTempColTranDate = Convert.ToDateTime(row["TranDate"].ToString().Trim());
                    vTempColCFYRecQty = Convert.ToDecimal(row["CFyRecQty"].ToString().Trim());

                    //decimal q11 = Convert.ToDecimal(row["DemQuantity"].ToString().Trim());
                    OpQty = Convert.ToDecimal(row["DemQuantity"].ToString().Trim());

                    //OpQty = TCloseQty;
                    form4 = new BanderolForm_4();

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                   
                    OpQty = 0;

                    ReceiveQty = 0;
                    UsedQty = 0;
                    WastageQty = 0;
                    SaleQty = 0;

                    CFYRQty = vTempColCFYRecQty;
                    TCloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                     Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                    vColFiscalYear = vTempColFiscalYear;
                    vColTranDate = vTempColTranDate;
                    vColDemandNo = "-";
                    vColProductName = "-";
                    vColPack = "-";
                    //vColBanderolInfo = " ";
                    vColDemQuantity = OpeningQty;
                    vColRecNoDate = "";
                    vColRecQuantity = ReceiveQty;
                    vColCFYRecQty = 0;
                    vColUsedQty = UsedQty;
                    vColWastageQty = WastageQty;
                    vColClosingQty = TCloseQty;
                    vColSaleQty = SaleQty;
                    vColRemarks = vTempColRemarks;
                    vTransType = vTemptransType;


                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                        Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                    #endregion

                    #region AssignValue to List

                    form4.ColSerialNo = i; //    i.ToString();      // Serial No   
                    form4.ColFiscalYear = vColFiscalYear; //    item["StartDateTime"].ToString();      // Date
                    form4.ColTranDate = vColTranDate; //    item["StartingQuantity"].ToString();      // Opening Quantity
                    form4.ColDemandNo = vColDemandNo; //    item["StartingAmount"].ToString();      // Opening Price
                    form4.ColProductName = vColProductName; //    item["Quantity"].ToString();      // Production Quantity
                    form4.ColPack = vColPack; //    item["UnitCost"].ToString();      // Production Price
                    form4.ColBanderolInfo = vColBanderolInfo; //    item["CustomerName"].ToString();      // Customer Name
                    //form4.ColDemQuantity = vColDemQuantity;
                    form4.ColDemQuantity = 0;  
                    form4.ColRecNoDate = vColRecNoDate;   //    item["Address1"].ToString();      // Customer Address
                    form4.ColRecQuantity = vColRecQuantity; //    item["TransID"].ToString();      // Sale Invoice No
                    form4.ColCFYRecQty = vColCFYRecQty; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                    form4.ColUsedQty = vColUsedQty; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                    form4.ColWastageQty = vColWastageQty; //    item["ProductName"].ToString();      // Sale Product Name
                    form4.ColClosingQty = vColClosingQty; //    item["Quantity"].ToString();      // Sale Product Quantity
                    form4.ColSaleQty = vColSaleQty; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                    form4.ColRemarks = vColRemarks; //    item["SD"].ToString();      // SD Amount
                    form4.ColTransType = vColRemarks; //    item["VATRate"].ToString();      // VAT Amount

                    form4s.Add(form4);
                    i = i + 1;

                    #endregion AssignValue to List

                }
                #endregion Opening

                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("Remarks<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    MessageBox.Show("There is no data to preview", this.Text);
                    return;
                }
                string strSort = "TranDate ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable


                    OpeningQty = 0;
                    ReceiveQty = 0;
                    UsedQty = 0;
                    WastageQty = 0;
                    SaleQty = 0;
                    TCloseQty = 0;




                    vColSerialNo = i;
                    vTempColFiscalYear = item["FiscalYear"].ToString(); // Date
                    vTempColTranDate = Convert.ToDateTime(item["TranDate"].ToString()); // Opening Quantity
                    vTempColDemandNo = item["DemandNo"].ToString(); // Opening Price
                    vTempColProductName = item["ProductName"].ToString(); // Production Quantity
                    vTempColPack = item["Pack"].ToString(); // Production Price
                    //vTempColBanderolInfo = item["CustomerName"].ToString(); // Customer Name
                    vTempColDemQuantity = Convert.ToDecimal(item["DemQuantity"].ToString()); // Customer VAT Reg No
                    vTempColRecNoDate = item["RecNoDate"].ToString(); // Customer Address
                    vTempColRecQuantity = Convert.ToDecimal(item["RecQuantity"].ToString()); // Sale Invoice No
                    vTempColUsedQty = Convert.ToDecimal(item["UsedQty"].ToString()); // Sale Invoice Date and Time
                    vTempColWastageQty = Convert.ToDecimal(item["WastageQty"].ToString()); // Sale Product Name
                    vTempColSaleQty = Convert.ToDecimal(item["SaleQty"].ToString()); // SD Amount
                    vTempColRemarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();


                    #endregion Get from Datatable

                    if (vTempColRemarks.Trim() == "Demand Receive")
                    {
                        form4 = new BanderolForm_4();

                        #region if row 1 Opening

                        
                        OpeningQty = OpQty + vClosingQuantity;
                        OpQty = 0;
                        ReceiveQty = vTempColRecQuantity;
                        UsedQty = vTempColUsedQty;
                        WastageQty = vTempColWastageQty;
                        SaleQty = vTempColSaleQty;
                        CFYRQty = CFYRQty + ReceiveQty;
                        TCloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                             Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));
                        
                        vColFiscalYear = vTempColFiscalYear;
                        vColTranDate = vTempColTranDate;
                        vColDemandNo = vTempColDemandNo;
                        vColProductName = vTempColProductName;
                        vColPack = vTempColPack;
                        //vColBanderolInfo = " ";
                        vColDemQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColDemQuantity.ToString(), 2));
                        vColRecNoDate = vTempColRecNoDate;
                        vColRecQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColRecQuantity.ToString(), 2));
                        vColCFYRecQty = Convert.ToDecimal(Program.FormatingNumeric(CFYRQty.ToString(), 2));
                        vColUsedQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColUsedQty.ToString(), 2));
                        vColWastageQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColWastageQty.ToString(), 2));
                        vColClosingQty = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), 2));
                        vColSaleQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColSaleQty.ToString(), 2));
                        vColRemarks = vTempColRemarks;
                        vTransType = vTemptransType;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                            Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                        #endregion

                        #region AssignValue to List

                        form4.ColSerialNo = i;
                        form4.ColFiscalYear = vColFiscalYear;
                        form4.ColTranDate = vColTranDate;
                        form4.ColDemandNo = vColDemandNo;
                        form4.ColProductName = vColProductName;
                        form4.ColPack = vColPack;
                        form4.ColBanderolInfo = vColBanderolInfo;
                        form4.ColDemQuantity = vColDemQuantity;
                        form4.ColRecNoDate = vColRecNoDate;
                        form4.ColRecQuantity = vColRecQuantity;
                        form4.ColCFYRecQty = vColCFYRecQty;
                        form4.ColUsedQty = vColUsedQty;
                        form4.ColWastageQty = vColWastageQty;
                        form4.ColClosingQty = vColClosingQty;
                        form4.ColSaleQty = vColSaleQty;
                        form4.ColRemarks = vColRemarks;
                        form4.ColTransType = vTransType;

                        form4s.Add(form4);
                        i = i + 1;

                        #endregion AssignValue to List
                    }
                    else if (vTempColRemarks == "Sale")
                    {
                        form4 = new BanderolForm_4();
                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpQty = 0;
                        ReceiveQty = 0;
                        UsedQty = vTempColUsedQty;
                        WastageQty = vTempColWastageQty;
                        SaleQty = vTempColSaleQty;

                        TCloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                             Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                        vColFiscalYear = vTempColFiscalYear;
                        vColTranDate = vTempColTranDate;
                        vColDemandNo = vTempColDemandNo;
                        vColProductName = vTempColProductName;
                        vColPack = vTempColPack;
                        //vColBanderolInfo = " ";
                        vColDemQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColDemQuantity.ToString(), 2));
                        vColRecNoDate = vTempColRecNoDate;
                        vColRecQuantity = Convert.ToDecimal(Program.FormatingNumeric(vTempColRecQuantity.ToString(), 2));
                        //vColCFYRecQty = Convert.ToDecimal(Program.FormatingNumeric(CFYRQty.ToString(), 2));
                        vColCFYRecQty = 0;
                        vColUsedQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColUsedQty.ToString(), 2));
                        vColWastageQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColWastageQty.ToString(), 2));
                        vColClosingQty = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), 2));
                        vColSaleQty = Convert.ToDecimal(Program.FormatingNumeric(vTempColSaleQty.ToString(), 2));
                        vColRemarks = vTempColRemarks;
                        vTransType = vTemptransType;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                                            Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                        #endregion

                        #region AssignValue to List

                        form4.ColSerialNo = i;
                        form4.ColFiscalYear = vColFiscalYear;
                        form4.ColTranDate = vColTranDate;
                        form4.ColDemandNo = vColDemandNo;
                        form4.ColProductName = vColProductName;
                        form4.ColPack = vColPack;
                        form4.ColBanderolInfo = vColBanderolInfo;
                        form4.ColDemQuantity = vColDemQuantity;
                        form4.ColRecNoDate = vColRecNoDate;
                        form4.ColRecQuantity = vColRecQuantity;
                        form4.ColCFYRecQty = vColCFYRecQty;
                        form4.ColUsedQty = vColUsedQty;
                        form4.ColWastageQty = vColWastageQty;
                        form4.ColClosingQty = vColClosingQty;
                        form4.ColSaleQty = vColSaleQty;
                        form4.ColRemarks = vColRemarks;
                        form4.ColTransType = vTransType;


                        form4s.Add(form4);
                        i = i + 1;

                        #endregion AssignValue to List
                    }
                }
                // Total Sale Info
                List<BanderolSaleInfo> form4Sales = new List<BanderolSaleInfo>();
               BanderolSaleInfo form4Sale = new BanderolSaleInfo();
                if (ReportResult.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow item in ReportResult.Tables[2].Rows)
                    {
                        form4Sale = new BanderolSaleInfo();
                        form4Sale.ItemNo = item[0].ToString();
                        form4Sale.SaleProduct = item[1].ToString();
                        form4Sale.ColTotalSale = Convert.ToDecimal(item[2].ToString());
                        form4Sales.Add(form4Sale);
                        //i = i + 1;
                    }
                   
                }
                #region Report preview
                RptBanderolForm4 objrpt = new RptBanderolForm4();

                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                //RptForm4Sale objrpt = new RptForm4Sale();
                //objrpt.SetDataSource(form4Sales);
                objrpt.SetDataSource(form4s);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                FormReport reports = new FormReport();
                objrpt.Subreports[0].SetDataSource(form4Sales);
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete
                #endregion Report preview

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
                this.btnPrev.Enabled = true;
                this.progressBar1.Visible = false;
                this.button1.Enabled = true;
            }

        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
        //============================================================

    }
}
