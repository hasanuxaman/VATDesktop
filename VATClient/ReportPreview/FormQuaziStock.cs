using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
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
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormQuaziStock : Form
    {
        public FormQuaziStock()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;


        #region Global Variables For BackGroundWorker
        private string vdtpStartDate;
        private string vdtpToDate;
        private string BOMReferenceNo;

        #endregion
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
            cmbBOMReferenceNo.Text = "= All =";
            dtpFromDate.Checked = false;
            dtpToDate.Checked = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        #region backgroundWorkerPreview
        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataSet ReportResult = new DataSet();
                ReportResult = reportDsdal.BOMWiseStock(BOMReferenceNo, vdtpStartDate, vdtpToDate);
                OrdinaryVATDesktop.SaveExcelMultiple(ReportResult, "MIS Stock(BOM Reference)", new[] { "Production VS Sale", "Sale", "Production" });

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Visible = false;
        }
        #endregion
        private void ReportShowDs()
        {
            try
            {
                this.progressBar1.Visible = true;
                NullCheck();
                BOMReferenceNo = cmbBOMReferenceNo.Text;
                if (BOMReferenceNo == "= All =")
                {
                    BOMReferenceNo = "";
                }
                backgroundWorkerPreview.RunWorkerAsync();

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




        private void NullCheck()
        {
          
            try
            {
               
                if (dtpFromDate.Checked == false)
                {
                    vdtpStartDate = "";

                    //vdtpStartDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd 00:00:00");

                }
                else
                {
                    vdtpStartDate = dtpFromDate.Value.ToString("yyyy-MMM-dd  00:00:00").ToString();
                }
                if (dtpToDate.Checked == false)
                {
                    vdtpToDate = "";
                    //vdtpToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd 23:59:59");
                }
                else
                {
                    vdtpToDate = dtpToDate.Value.ToString("yyyy-MMM-dd 23:59:59").ToString();
                }
     
            }
            #region Catch
            
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
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {

                ReportShowDs();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDownload", ex.Message);
            }
        }

        private void FormQuaziStock_Load(object sender, EventArgs e)
        {
            //dtpFromDate.Value = DateTime.Now;
            //dtpToDate.Value = DateTime.Now;


            #region BOMRefrebce DropDown

            string[] Conditions = new string[] { "ActiveStatus='Y'","ReferenceNo is not null" };
            cmbBOMReferenceNo = new CommonDAL().ComboBoxLoad(cmbBOMReferenceNo, "BOMs", "ReferenceNo", "ReferenceNo", Conditions, "varchar", true, true, connVM);

           

            #endregion

        }


    }
}
