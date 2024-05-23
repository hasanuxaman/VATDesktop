using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using VATServer.Library.Integration;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormPurchaseReturnsImportNESTLE : Form
    {
        #region Variables

        NESTLEIntegrationDAL _IntegrationDAL = new NESTLEIntegrationDAL();
        CommonDAL _cDal = new CommonDAL();
        ResultVM rVM = new ResultVM();
        IntegrationParam paramVM = new IntegrationParam();

        string[] sqlResults = new string[6];
        DataTable dtTableResult;
        public bool isFileSelected = false;
        private bool _isDeleteTemp = true;
        public string transactionType;
        private string DateRange = "";
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        string loadedTable;
        static string selectedType;

        public bool IsCDN = false;
        private string _saleRow;
        private string preSelectTable;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormPurchaseReturnsImportNESTLE()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormPurchaseReturnsImportNESTLE form = new FormPurchaseReturnsImportNESTLE();

            form.IsCDN = cdn;

            form.preSelectTable = "Purchase";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        #region Form Load

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            dgvLoadedTable.ReadOnly = true;
            dgvLoadedTable.ReadOnly = true;
            typeLoad();
            //cmbDBList.SelectedIndex = 0;

        }
        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_DATABASE);

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection",null,connVM);

            cmbImportType.SelectedItem = CONST_EXCEL;

            cmbImportType.Text = value;

            #region IsIntegrationExcel & Other Lisence Check
            if (Program.IsIntegrationExcel == false)
            {
                cmbImportType.Items.Remove(CONST_EXCEL);
                cmbImportType.Items.Remove(CONST_TEXT);
                cmbImportType.SelectedItem = CONST_DATABASE;

            }
            if (Program.IsIntegrationOthers == false)
            {
                cmbImportType.Items.Remove(CONST_DATABASE);
                cmbImportType.SelectedItem = CONST_EXCEL;

            }
            #endregion


        }

        #endregion

        #region Data Load

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {

                var invoiceNo = txtId.Text.Trim();

                if (string.IsNullOrEmpty(invoiceNo))
                {
                    MessageBox.Show(@"Please Enter Transaction No");
                    return;
                }


                #region Load Start
                //string LoadStartTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load Start Time: " + LoadStartTime);

                #endregion

                DataLoad();

                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

                #region Load End

                //string LoadEndTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                //FileLogger.Log(this.Name, "btnLoad_Click", "Date Range: " + DateRange + Environment.NewLine + "Load End Time: " + LoadEndTime);

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void DataLoad()
        {
            BranchProfileDAL dal = new BranchProfileDAL();
            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

            #region Data Load

            IntegrationParam paramVM = new IntegrationParam();

            paramVM.RefNo = txtId.Text.Trim();
            paramVM.TransactionType = transactionType;
            paramVM.dtConnectionInfo = dt;


            dtTableResult = _IntegrationDAL.GetSource_PurchaseReturnData_Master(paramVM,connVM);

            dgvLoadedTable.DataSource = dtTableResult;

            #endregion

        }

        #endregion

        #region Save Data


        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadedTable == "")
                {
                    return;
                }

                selectedType = cmbImportType.Text;

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

                DataTable purchaseData = dtTableResult.Copy();


                sqlResults = purchaseDal.SaveTempPurchase(purchaseData, Program.BranchCode, transactionType, Program.CurrentUser,
                    Program.BranchId, () => { },null,null,connVM);

          
                    UpdateOtherDB(purchaseData);
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
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

        #endregion

        #region MISC Functions

        private void TableValidation(DataTable salesData)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") };
            DataColumn userId = new DataColumn("UserId") { DefaultValue = Program.CurrentUserID };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);
            salesData.Columns.Add(userId);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
        }


        private void SetSteps(int steps = 4)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
        }

        private void BulkCallBack()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { progressBar1.Value += 1; }));

        }

        private void UpdateProgressBar()
        {
            progressBar1.Value += 1;
        }

        private void PercentBar(int maximum)
        {
            progressBar1.Style = ProgressBarStyle.Blocks;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = maximum;

            progressBar1.Value = 0;
            //var percent = (int) ((progressBar1.Value - progressBar1.Minimum) /
            //                     (double) (progressBar1.Maximum - progressBar1.Minimum) * 100);
            //using (var gr = progressBar1.CreateGraphics())
            //{
            //    gr.DrawString(percent + "%",
            //        SystemFonts.DefaultFont,
            //        Brushes.Black,
            //        new PointF(progressBar1.Width / 2 - (gr.MeasureString(percent + "%",
            //                                                 SystemFonts.DefaultFont).Width / 2.0F),
            //            y: progressBar1.Height / 2 - (gr.MeasureString(percent + "%",
            //                                           SystemFonts.DefaultFont).Height / 2.0F)));
            //}
        }

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {
            //_saleRow = dgvLoadedTable.CurrentRow.Cells["ID"].Value.ToString();

            //this.Hide();

        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
                _saleRow = dgvLoadedTable.CurrentRow.Cells["ID"].Value.ToString();

                this.Hide();

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        #endregion

        private void UpdateOtherDB(DataTable table)
        {
            if (sqlResults[0].ToLower() == "success")
            {
                ImportDAL importDal = new ImportDAL();

                //DataTable table = salesData;//salesDal.GetInvoiceNoFromTemp();

                string[] results = importDal.UpdateNestlePurchaseReturnTransactions(table, settingVM.BranchInfoDT,connVM, "PurchaseInvoiceReturns");

                if (results[0].ToLower() != "success")
                {
                    string message = "These Id and PurchaseInvoices failed to insert to other database\n";

                    foreach (DataRow row in table.Rows)
                    {
                        message += row["ID"] + "\n";
                    }

                    FileLogger.Log("Nestle", "UpdateOtherDB", message);

                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            
        }


    }
}
