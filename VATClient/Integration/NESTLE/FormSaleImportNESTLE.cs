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
using System.Drawing;
using VATClient.ReportPreview;

namespace VATClient
{
    public partial class FormSaleImportNESTLE : Form
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
        private string ChalanNo;
        private string preSelectTable;
        private string SalesFromDate = "";
        private string SalesToDate = "";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormSaleImportNESTLE()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportNESTLE form = new FormSaleImportNESTLE();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
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

                NullCheck();
                DataLoad();
                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void DataLoad()
        {
            BranchProfileDAL dal = new BranchProfileDAL();
            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

            #region Data Load

            IntegrationParam paramVM = new IntegrationParam();

            paramVM.RefNo = txtId.Text.Trim();
            paramVM.FromDate = SalesFromDate;
            paramVM.ToDate = SalesToDate;
            paramVM.TransactionType = transactionType;
            paramVM.dtConnectionInfo = dt;


            dtTableResult = _IntegrationDAL.GetSource_SaleData_Master(paramVM,connVM);

            dgvLoadedTable.DataSource = dtTableResult;
            dgvLoadedTable.Columns[6].Width = 150;




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
                var salesDal = new SaleDAL();


                BranchProfileDAL dal = new BranchProfileDAL();
                UserInformationDAL Userdal = new UserInformationDAL();
                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);
                var UserInfo = Userdal.SelectAll(Convert.ToInt32(Program.CurrentUserID),null,null,null,null,connVM).SingleOrDefault();

                #region Data Load

                IntegrationParam paramVM = new IntegrationParam();

                paramVM.RefNo =ChalanNo;
                
                paramVM.TransactionType = transactionType;
                paramVM.dtConnectionInfo = dt;


                dtTableResult = _IntegrationDAL.GetSource_SaleData_Details(paramVM,connVM);

                //dgvLoadedTable.DataSource = dtTableResult;



                #endregion

                var SalesData = dtTableResult.Copy();
                SalesData.Columns.Add(new DataColumn("SignatoryName") { DefaultValue = UserInfo.FullName });
                SalesData.Columns.Add(new DataColumn("SignatoryDesig") { DefaultValue = UserInfo.Designation });


                TableValidation(SalesData);

                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { PercentBar(5); });

                    BulkCallBack();

                    sqlResults = salesDal.SaveAndProcess(SalesData, BulkCallBack, Program.BranchId,"",connVM, null , null,null, Program.CurrentUserID);
                    UpdateOtherDB(SalesData);



               


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
                   DataLoad();
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
                txtId.Enabled = true;
                dtpSaleFromDate.Enabled = true;
                dtpSaleToDate.Enabled = true;
                btnLoad.Enabled = true;
                btnRefresh.Enabled = true;
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
                //_saleRow = dgvLoadedTable.CurrentRow.Cells["ID"].Value.ToString();

                //this.Hide();

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView senderGrid = (DataGridView)sender;
                string Status = dgvLoadedTable.CurrentRow.Cells["Status"].Value.ToString();
                ChalanNo = dgvLoadedTable.CurrentRow.Cells["ChallanNo"].Value.ToString();
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                    e.RowIndex >= 0)
                {

                    if (e.ColumnIndex == 0) // checking remove button
                    {


                        if (Status.ToLower() == "saved")
                        {
                            MessageBox.Show(this, "This Chalan No: " + ChalanNo + " Already Saved!");
                        }
                        else
                        {
                            txtId.Enabled = false;
                            dtpSaleFromDate.Enabled = false;
                            dtpSaleToDate.Enabled = false;
                            btnLoad.Enabled = false;
                            btnRefresh.Enabled = false;
                            SaveRow();

                        }
                    }
                    else if (e.ColumnIndex == 1)
                    {
                        if (Status.ToLower() == "saved")
                        {
                            DetailsShow();
                        }
                        else
                        {
                            MessageBox.Show(this, "This Chalan No: " + ChalanNo + " Need To Save First!");

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        #endregion

        void NullCheck()
        {

            var invoiceNo = txtId.Text.Trim();

            
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
            

            SalesFromDate = dtpSaleFromDate.Checked == false ? "" : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
            SalesToDate = dtpSaleToDate.Checked == false ? "" : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
            

        }

        private void dgvLoadedTable_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.add_over.Width;
                var h = Properties.Resources.add_over.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.add_over, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

            if (e.ColumnIndex == 1)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.code.Width;
                var h = Properties.Resources.code.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.code, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void SaveRow()
        {
            try
            {

                if (dgvLoadedTable.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToSave + "\nChallan No: " + dgvLoadedTable.CurrentRow.Cells["ChallanNo"].Value,
                            this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ChalanNo = dgvLoadedTable.CurrentRow.Cells["ChallanNo"].Value.ToString();
                        this.progressBar1.Visible = true;
                        bgwBigData.RunWorkerAsync();
                    }
                }
                else
                {
                    MessageBox.Show("No Challan No Found To Save.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DetailsShow()
        {
            FormNestleSaleDetails frm = new FormNestleSaleDetails();
            frm.txtChalanNo.Text = ChalanNo;
          
            frm.ShowDialog();
        }


        private void UpdateOtherDB(DataTable table)
        {
            try
            {
                if (sqlResults[0].ToLower() == "success")
                {
                    NESTLEIntegrationDAL importDal = new NESTLEIntegrationDAL();

                    //DataTable table = salesData;//salesDal.GetInvoiceNoFromTemp();

                    string[] results = importDal.UpdateTransactions(table, settingVM.BranchInfoDT, "SaleInvoices",null,connVM);

                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Chalan No failed to Update the database\n";

                        foreach (DataRow row in table.Rows)
                        {
                            message += row["ID"] + "\n";
                        }

                        FileLogger.Log("Nestle", "UpdateOtherDB", message);

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            dtpSaleFromDate.Value = DateTime.Now.Date;
            dtpSaleToDate.Value = DateTime.Now.Date;
            dtpSaleFromDate.Checked = false;
            dtpSaleToDate.Checked = false;
        }

       

    }
}
