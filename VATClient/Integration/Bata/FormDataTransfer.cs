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
using VATServer.Library.Integration;
using VATViewModel.DTOs;

namespace VATClient.Integration.Bata
{
    public partial class FormDataTransfer : Form
    {
        private string SalesFromDate = "";
        private string SalesToDate = "";
        IntegrationParam paramVM = new IntegrationParam();
        public string transactionType;
        private string _saleRow = "";
        public string preSelectTable;
        ResultVM rVM = new ResultVM();
        BataIntegrationDAL _IntegrationDAL = new BataIntegrationDAL();
        string[] sqlResults = new string[6];
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public FormDataTransfer()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType)
        {
            FormDataTransfer form = new FormDataTransfer();

            //form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }


        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                btnTransfer.Enabled = false;
                this.progressBar1.Visible = true;


                NullCheck();

                #region Data Load

                paramVM = new IntegrationParam();

                paramVM.FromDate = SalesFromDate;
                paramVM.ToDate = SalesToDate;

                #endregion

                bgwBigData.RunWorkerAsync();


            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        void NullCheck()
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

            SalesFromDate = dtpSaleFromDate.Checked == false ? new DateTime(2010, 1, 1).ToString("yyyy-MMM-dd") : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
            SalesToDate = dtpSaleToDate.Checked == false ? new DateTime(2030, 1, 1).ToString("yyyy-MMM-dd") : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");

        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL salesDal = new SaleDAL();

                ImportDAL importDal = new ImportDAL();
                var dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);


                paramVM.callBack = () => { };
                paramVM.SetSteps = (step) => { };
                paramVM.TransactionType = transactionType;
                paramVM.BranchCode = Program.BranchCode;
                paramVM.CurrentUser = Program.CurrentUser;
                paramVM.DefaultBranchId = Program.BranchId;
                paramVM.dtConnectionInfo = dt;


                rVM = _IntegrationDAL.SaleDataProcess(paramVM,connVM);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Save End

                string SaveEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                btnTransfer.Enabled = true;

                ////FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Save End Time: " + SaveEndTime);

                #endregion

                if (rVM.Status == "Fail")
                {
                    throw new ArgumentException(rVM.Message);
                }

                MessageBox.Show(rVM.Message, "Sales", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ////loadedTable = CONST_SALETYPE;


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

        private void FormDataTransfer_Load(object sender, EventArgs e)
        {

        }




    }
}
