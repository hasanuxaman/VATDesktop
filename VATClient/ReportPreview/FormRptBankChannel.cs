using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormRptBankChannel : Form
    {
        #region Global Variables

        private string PurchaseFromDate = "", PurchaseToDate = "";
        private string IsBankingChannelPay = "";
        private string PaymentType = "";
        private DataSet ReportResult;
        private ReportDocument reportDocument = new ReportDocument();
        string bankPayRange = "";

        #endregion


        public FormRptBankChannel()
        {
            InitializeComponent();
             
            connVM = Program.OrdinaryLoad();
			 
			   }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {


            #region try

            try
            {
                OrdinaryVATDesktop.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                if (Program.CheckLicence(dtpPurchaseToDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;

                #region Purchase Date

                if (dtpPurchaseFromDate.Checked)
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                else
                {
                    PurchaseFromDate = "";
                }
                if (dtpPurchaseToDate.Checked)
                {
                    PurchaseToDate = dtpPurchaseToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }
                else
                {
                    PurchaseToDate = "";
                }
                #endregion

                IsBankingChannelPay = cmbIsBankingChannelPay.Text.Trim();
                if (IsBankingChannelPay == "All")
                {
                    IsBankingChannelPay = "";
                }

                PaymentType = cmbPaymentType.Text.Trim();
                if (PaymentType == "All")
                {
                    PaymentType = "";
                }

                if (chkBankingPay.Checked)
                {
                    bankPayRange = chkBankingPay.Text.Trim();
                }
                else
                {
                    bankPayRange = "";
                }

                backgroundWorkerMIS.RunWorkerAsync();

            }

            #endregion

            #region catch

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            #endregion

        }

        private void backgroundWorkerMIS_DoWork(object sender, DoWorkEventArgs e)
        {

            #region Try

            ReportResult = null;

            try
            {

                BankChannelPaymentVM vm = new BankChannelPaymentVM();

                if (!string.IsNullOrWhiteSpace(PurchaseFromDate))
                {

                }
                vm.PurchaseFromDate = PurchaseFromDate;
                vm.PurchaseToDate = PurchaseToDate;
                vm.IsBankingChannelPay = IsBankingChannelPay;
                vm.PaymentType = PaymentType;
                vm.bankPayRange = bankPayRange;

                if (string.IsNullOrWhiteSpace(vm.PurchaseFromDate))
                {
                    vm.PurchaseFromDate = "ALL";
                }
                if (string.IsNullOrWhiteSpace(vm.PurchaseToDate))
                {
                    vm.PurchaseToDate = "ALL";
                }
                if (string.IsNullOrWhiteSpace(vm.IsBankingChannelPay))
                {
                    vm.IsBankingChannelPay = "ALL";
                }

                if (string.IsNullOrWhiteSpace(vm.PaymentType))
                {
                    vm.PaymentType = "ALL";
                }
                if (string.IsNullOrWhiteSpace(vm.bankPayRange))
                {
                    vm.bankPayRange = "ALL";
                }

                string[] cValues = { PurchaseFromDate, PurchaseToDate, IsBankingChannelPay, PaymentType, bankPayRange };
                string[] cFields = { "pur.ReceiveDate>=", "pur.ReceiveDate<=", "pur.IsBankingChannelPay", "bcp.PaymentType", "pur.TotalAmount>=" };

                MISReport _report = new MISReport();
                reportDocument = _report.BankChannelMISReport(cFields, cValues, vm,connVM);

            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_DoWork", ex.Message);
            }

            #endregion

        }

        private void backgroundWorkerMIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                reports.crystalReportViewer1.Refresh();

                reports.setReportSource(reportDocument);

                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
            }
            #endregion

            #region catch

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message);
            }

            #endregion

            #region finally

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
            #endregion

        }

        private void FormRptBankChannel_Load(object sender, EventArgs e)
        {
            try
            {

                #region Purchase Value Check

                string settingValue = new CommonDAL().settingValue("Purchase", "BankingChannelPayRange",connVM);

                chkBankingPay.Text = settingValue;

                #endregion


            }
            #region catch

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerMIS_RunWorkerCompleted", ex.Message);
            }

            #endregion

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            dtpPurchaseFromDate.Value = DateTime.Now;
            dtpPurchaseToDate.Value = DateTime.Now;
            cmbIsBankingChannelPay.Text = "";
            cmbPaymentType.Text = "";
            chkBankingPay.Checked = false;

        }

    }
}
