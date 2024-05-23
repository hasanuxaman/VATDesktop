using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormTransferImportKCCL : Form
    {
        #region Variables
        string[] sqlResults = new string[6];
        string selectedTable;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult;
        DataSet ds;
        public bool isFileSelected = false;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private DataTable _noBranch;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_DBNAME = "VATImport_DB";
        private const string CONST_SINGLEIMPORT = "SingleFileImport";
        private const string CONST_SALETYPE = "Sales";
        private const string CONST_PURCHASETYPE = "Purchases";
        private const string CONST_ISSUETYPE = "Issues";
        private const string CONST_RECEIVETYPE = "Receives";
        private const string CONST_VDSTYPE = "VDS";//
        private const string CONST_TRANSFERTYPE = "TransferIssues";
        private const string CONST_BOMTYPE = "BOM";
        private const string CONST_SALES_BIGDATA = "Sales_Big_Data";
        private const string CONST_TransferReceive = "TransferReceive";
        private bool Integration = false;
        public string _saleRow = "";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion

        public FormTransferImportKCCL()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }
        // private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public static string SelectOne(string transactionType)
        {
            FormTransferImportKCCL form = new FormTransferImportKCCL();

            form.preSelectTable = "TransferIssues";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }




        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            tableLoad();
            typeLoad();
        }

        private void tableLoad()
        {

            cmbTable.Items.Add(CONST_TRANSFERTYPE);
            cmbTable.Items.Add(CONST_TransferReceive);
            if (!string.IsNullOrWhiteSpace(preSelectTable))
            {
                cmbTable.SelectedItem = preSelectTable;
            }
            cmbTable.Enabled = false;
        }

        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_EXCEL);
            cmbImportType.Items.Add(CONST_TEXT);
            cmbImportType.Items.Add(CONST_DATABASE);
            cmbImportType.SelectedItem = CONST_EXCEL;

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "TransferImportSelection",null,connVM);

            cmbImportType.SelectedItem = CONST_EXCEL;

            cmbImportType.Text = value;
        }

        private void LoadDataGrid()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Program.ImportFileName) || !chkSame.Checked)
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show(this, "Please select the right file for import");
                    return;
                }
                selectedType = cmbImportType.Text;
                #region Excel validation
                if (selectedType == CONST_EXCEL)
                {
                    //string[] extention = fileName.Split(".".ToCharArray());
                    //string[] retResults = new string[4];
                    //if (extention[extention.Length - 1] == "xls")
                    //{

                    //}
                    //else
                    //{
                    //    MessageBox.Show(this, "You can select Excel(.xls) files only");
                    //    return;
                    //}
                }
                #endregion
                #region Text Validation
                else if (selectedType == CONST_TEXT)
                {
                    string[] extention = fileName.Split(".".ToCharArray());
                    string[] retResults = new string[4];
                    if (extention[extention.Length - 1] == "txt")
                    {

                    }
                    else
                    {
                        MessageBox.Show(this, "You can select Text(.txt) files only");
                        return;
                    }
                }
                #endregion

                var flag = "N";
                var commonDal = new CommonDAL();

                progressBar1.Visible = true;
                selectedTable = cmbTable.Text;
                selectedType = cmbImportType.Text;
                #region Excel
                if (selectedType == CONST_EXCEL)
                {
                    ds = LoadFromExcel();
                    switch (selectedTable)
                    {
                        case CONST_SALETYPE:
                            dtTableResult = ds.Tables["SaleM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Delivery_Date_Time", "Invoice_Date_Time" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_SALETYPE;
                            break;
                        case CONST_PURCHASETYPE:
                            dtTableResult = ds.Tables["PurchaseM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Invoice_Date", "Receive_Date" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_PURCHASETYPE;
                            break;
                        case CONST_ISSUETYPE:
                            dtTableResult = ds.Tables["IssueM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Issue_DateTime" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_ISSUETYPE;
                            break;
                        case CONST_RECEIVETYPE:
                            dtTableResult = ds.Tables["ReceiveM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Receive_DateTime" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_RECEIVETYPE;
                            break;
                        case CONST_VDSTYPE:
                            dtTableResult = ds.Tables["VDSM"];
                            DataTable detailTable = ds.Tables["VDSD"];


                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] {"Bill_Date","Issue_Date", "Cheque_Date", "Deposit_Date", "BankDepositDate" });
                            //dtTableResult = OrdinaryVATDesktop.DtColumnAdd(dtTableResult, "Transection_Type1", "Kamrul", "string");


                            detailTable = OrdinaryVATDesktop.DtDateCheck(detailTable, new string[] { "Bill_Date","Issue_Date" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_VDSTYPE;
                            break;
                        case CONST_BOMTYPE:
                            dtTableResult = ds.Tables["Product"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "EffectDate", "FirstSupplyDate" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_BOMTYPE;
                            break;
                        case CONST_TRANSFERTYPE:
                            dtTableResult = ds.Tables["TransferIssueM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "TransactionDateTime" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_TRANSFERTYPE;
                            break;
                        case CONST_TransferReceive:
                            dtTableResult = ds.Tables["Receive"];
                            //dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "TransactionDateTime" });
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_TransferReceive;
                            break;
                        default:
                            break;
                    }

                    if (dtTableResult != null)
                    {
                        lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;
                    }
                }
                #endregion
                #region Text
                
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cmbImportType.Text != CONST_DATABASE)
            {
                _isDeleteTemp = true;
                LoadDataGrid();
            }
            else
            {
                try
                {
                    selectedTable = cmbTable.Text;
                    selectedType = cmbImportType.Text;

                    switch (selectedTable)
                    {
                        
                        case CONST_TRANSFERTYPE:
                            LoadFG();
                            break;
                        default:
                            break;
                    }
                    //dtTableResult.Columns.Remove("IsVATComplete");
                    dgvLoadedTable.DataSource = dtTableResult;


                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LoadFG()
        {
            CommonDAL cdal = new CommonDAL();
            TransferIssueDAL tDal = new TransferIssueDAL();

            var invoiceNo = txtId.Text.Trim();

            string value = cdal.settingValue("Import", "TransferExistContinue",connVM);

            if (value == "N")
            {
                List<TransferIssueVM> issues = tDal.SelectAllList(0, new[] { "ti.ImportIDExcel" }, new[] { invoiceNo }, null, null,connVM);

                if (issues != null && issues.Count > 0)
                {
                    var sale = issues.FirstOrDefault();

                    if (sale != null)
                        MessageBox.Show(@"This Transaction No is already in system with Transfer no - " +
                                        sale.TransferIssueNo);

                    return;
                }
            }

            ImportDAL dal = new ImportDAL();
            BranchProfileDAL branchDAL = new BranchProfileDAL();

            DataTable dt = branchDAL.SelectAll(Program.BranchId.ToString(),null,null,null,null,true,connVM);
            Integration = true;

            //BugsBD
            string IdText = OrdinaryVATDesktop.SanitizeInput(txtId.Text);
            
            //dtTableResult = dal.GetSaleKohinoorFGData(txtId.Text,dt);
            dtTableResult = dal.GetSaleKohinoorFGData(IdText, dt);


            var pDal = new ProductDAL();

            foreach (DataRow row in dtTableResult.Rows)
            {
                var vms = pDal.SelectAll("0", new string[] { "Pr.ProductName" }, new[] { row["ProductName"].ToString() },null,null,null,connVM);

                if (vms == null || !vms.Any())
                {
                    var nameVMs = pDal.SelectProductName("0", new[] { "Pr.ProductName" }, new[] { row["ProductName"].ToString() },null,null,null,connVM);

                    if (nameVMs != null && nameVMs.Any())
                    {
                        vms = pDal.SelectAll(nameVMs.FirstOrDefault().ItemNo,null,null,null,null,null,connVM);

                        row["VAT_Rate"] = vms.FirstOrDefault().VATRate;
                        //row["SD_Rate"] = vms.FirstOrDefault().SD;

                    }
                }
                else
                {
                    row["VAT_Rate"] = vms.FirstOrDefault().VATRate;
                    //row["SD_Rate"] = vms.FirstOrDefault().SD;

                }


                string branchTo = row["TransferToBranchCode"].ToString().Trim();
                string branchFrom = row["BranchCode"].ToString().Trim();



                var transfer = branchDAL.SelectAllList(null, new[] { "IntegrationCode" },
                    new[] { branchTo }).FirstOrDefault();

                var branchProfileVm = branchDAL.SelectAllList(null, new[] { "IntegrationCode" }, 
                    new[] { branchFrom }).FirstOrDefault();




                if (transfer != null && branchProfileVm != null)
                {
                    row["TransferToBranchCode"] = transfer.BranchCode;
                    row["BranchCode"] = branchProfileVm.BranchCode;

                }
                FileLogger.Log("TransferIssue", "Load", branchFrom + " " + dt.Rows[0]["IntegrationCode"]);
                
                row["TransactionType"] = transactionType;
            }

            Integration = true;
            loadedTable = CONST_TRANSFERTYPE;
        }

        private void BrowsFile()
        {
            #region try
            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                if (cmbImportType.Text == CONST_EXCEL)
                {
                    //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                    //BugsBD
                    fdlg.Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (*.xlsm)|*.xlsm|Excel(97-2003) files (*.xls)|*.xls|Text files (*.txt)|*.txt";
                }
                else
                {
                    //fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";
                    //BugsBD
                    fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";
                }
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }
            }
            #endregion

            #region catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (loadedTable == "")
            {
                return;
            }
              
            if (!IsRowSelected())
            {
                MessageBox.Show(this, "Please select at least one row!");
                return;
            }

            if (loadedTable == CONST_TRANSFERTYPE)
            {
                SaveFGOut();
            }
            
        }

        private void SaveFGOut()
        {

            this.progressBar1.Visible = true;
            bgwTransferIssue.RunWorkerAsync();
        }

        private bool IsRowSelected()
        {
            DataGridView gd1 = dgvLoadedTable;
            DataTable dt1 = (DataTable)gd1.DataSource;

            dtTableResult = new DataTable();
            ////adding column name
            for (int j = 0; j < dt1.Columns.Count; j++)
            {
                dtTableResult.Columns.Add(dt1.Columns[j].ColumnName);
            }

            ////adding data rows
            for (int i = 0; i < gd1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(gd1["Select", i].Value) == true)
                {
                    dtTableResult.Rows.Add(dt1.Rows[i].ItemArray);
                }
            }

            return dtTableResult.Rows.Count > 0;
        }


        private DataSet LoadFromExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                ds = new ImportDAL().GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return ds;
        }



        private void SetPercentBar()
        {
            var count = dtTableResult
                .AsEnumerable()
                .Select(x => x.Field<string>("ID"))
                .Count();

            if (InvokeRequired)
                Invoke((MethodInvoker) delegate { PercentBar(count); });
        }



        private void bgwTransferIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TransferIssueDAL issueDal = new TransferIssueDAL();

                SetPercentBar();


                sqlResults = issueDal.SaveTempTransfer(dtTableResult, Program.BranchCode, transactionType,
                    Program.CurrentUser, Program.BranchId,BulkCallBack,null,null,Integration,connVM,"",Program.CurrentUserID);
                 
                // SaveTransferIssue();
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
                FileLogger.Log("FormMasterImport", "bgwTransferIssue", exception.Message);

            }
        }


        private void bgwTransferIssue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void SelectAll()
        {
            if (chkSelectAll.Checked == true)
            {
                for (int i = 0; i < dgvLoadedTable.RowCount; i++)
                {
                    dgvLoadedTable["Select", i].Value = true;
                }
            }
            else
            {
                for (int i = 0; i < dgvLoadedTable.RowCount; i++)
                {
                    dgvLoadedTable["Select", i].Value = false;
                }
            }
        }


        private void BulkCallBack()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { progressBar1.Value += 1; }));

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

       

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string Id = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();
            string refNo = dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value != null ? dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value.ToString() : "";


            _saleRow = string.IsNullOrEmpty(refNo) || refNo == "-" ? Id : refNo;

            this.Hide();
        }
    }
}
