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
using VATViewModel.DTOs;

namespace VATClient.Integration.DBH
{
    public partial class FormPurchaseImportDBH : Form
    {

        #region Variables

        private string DateRange;
        private string ToDate;
        private string FromDate;
        DataTable dtTableResult = new DataTable();
        string[] sqlResults = new string[6];
        public string transactionType;
        private string _saleRow = "";

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormPurchaseImportDBH()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormPurchaseImportDBH form = new FormPurchaseImportDBH();

            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }


        void NullCheck()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                if (dtpSaleFromDate.Checked == false)
                {
                    dtpSaleFromDate.Checked = true;
                    dtpSaleFromDate.Value = DateTime.Now;
                }

                if (dtpSaleToDate.Checked == false)
                {
                    dtpSaleToDate.Checked = true;
                    dtpSaleToDate.Value = DateTime.Now;
                }
            }

            FromDate = dtpSaleFromDate.Checked == false ? "" : dtpSaleFromDate.Value.ToString("yyyy-MM-dd");
            ToDate = dtpSaleToDate.Checked == false ? "" : dtpSaleToDate.Value.ToString("yyyy-MM-dd");
            DateRange = FromDate + " - " + ToDate;

        }


        private void btnLoad_Click(object sender, EventArgs e)
        {

            try
            {
                ImportDAL importDal = new ImportDAL();

                string invoiceNo = txtId.Text.Trim();

                BranchProfileDAL dal = new BranchProfileDAL();
                NullCheck();
                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

                //dtTableResult = importDal.GetPurchaseSQRData(invoiceNo, dt);

                string branchCode = dt.Rows[0]["IntegrationCode"].ToString();

                dtTableResult = importDal.GetPurchaseData_DBH(invoiceNo, dt, FromDate, ToDate);
               
                dgvLoadedTable.DataSource = dtTableResult;

            }
            catch (Exception ex)
            {
                FileLogger.Log("FormPurchaseImportDBH", "btnLoad_Click", ex.Message + "\n" + ex.StackTrace);

                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {

            try
            {

                this.progressBar1.Visible = true;
                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PurchaseDAL purchaseDal = new PurchaseDAL();

                var purchaseData = dtTableResult.Copy();

                purchaseData.Columns.Remove("VAT_RATE");

                sqlResults = purchaseDal.SaveTempPurchase(purchaseData, Program.BranchCode, transactionType, Program.CurrentUser, 0, () => { });

                //////sqlResults = purchaseDal.SaveToPurchaseTemp(purchaseData, Program.CurrentUserID, Program.BranchId, null, null, connVM);

            }
            catch (Exception ex)
            {

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }

            finally
            {
                this.progressBar1.Visible = false;

            }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    ImportDAL importDal = new ImportDAL();

                    importDal.UpdateDBHPurchaseTable(Program.CurrentUserID, Program.BranchId.ToString());

                    MessageBox.Show(this, "Import completed successfully");

                }
                else
                {
                    MessageBox.Show(this, sqlResults[1]);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }
    }
}
